using Durak.Common;
using Durak.Common.Cards;
using Durak.Properties;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Durak.Server
{
    // Представляє сервер, який запускає логіку гри і саму гру
    public class GameServer
    {
        private static readonly string[] BOT_NAMES;

        static GameServer()
        {
            BOT_NAMES = Resources.names_corrected.ToLower().Split('\n');
        }

        // Зберігає IP-адресу цього сервера
        private IPAddress myAddress;
        // Текстове поле для журналу сервера
        private RichTextBox myOutput;
        // Зберігає тег сервера цього сервера
        private ServerTag myTag;
        // Зберігає список правил гри для використання
        private List<IGamePlayRule> myPlayRules;
        // Зберігає список правил стану гри для використання
        private List<IGameStateRule> myStateRules;
        // Зберігає список правил ініціалізації гри для використання
        private List<IGameInitRule> myInitRules;
        // Зберігає методи обробки пакетів
        private Dictionary<MessageType, PacketHandler> myMessageHandlers;
        // Зберігає список гравців
        private PlayerCollection myPlayers;
        // Зберігає одноранговий мережевий вузол
        private NetPeer myServer;
        // Зберігає поточний стан сервера
        private ServerState myState;
        // Зберігає стан гри
        private GameState myGameState;
        // Зберігає гравця, який створює гру
        private Player myGameHost;
        // Зберігає всіх ботів, що працюють на цьому сервері
        private List<BotPlayer> myBots;
        // Зберігає, чи цей сервер знаходиться в режимі одиночної гри
        private bool isSinglePlayer;

        private NetPeerConfiguration myNetConfig;

        // Зберігає, чи налаштовано цей сервер для поточної гри
        private bool isInitialized = false;

        // Отримує IP-адресу сервера
        public IPAddress IP
        {
            get { return myAddress; }
        }
        // Отримує колекцію гравців серверів
        public PlayerCollection Players
        {
            get { return myPlayers; }
        }
        // Отримує або встановлює, чи повинен цей сервер зберігати кожне правило
        public bool LogLongRules
        {
            get;
            set;
        }
        // Отримати стан гри цього сервера
        public GameState GameState
        {
            get
            {
                return myGameState;
            }
        }
        // Отримує тег екземпляра цього сервера
        public ServerTag Tag
        {
            get { return myTag; }
        }
       
        public bool SinglePlayerMode
        {
            get { return isSinglePlayer; }
            set
            {
                if (myServer.Status == NetPeerStatus.NotRunning)
                    isSinglePlayer = value;
                else
                    throw new ArgumentException("Cannot set server to singleplayer after server has been started");
            }
        }

        // Отримує або встановлює кількість гравців, яку підтримує цей сервер
        public int NumPlayers
        {
            get { return myPlayers.Count; }
            set
            {
                if (NumPlayers < value)
                {
                    myTag.SupportedPlayerCount = value;
                    myPlayers.Resize(value);
                }
                else if (NumPlayers > value)
                {
                    throw new InvalidOperationException("Cannot shrink player collection");
                }
            }
        }
        // Отримує поточний стан сервера
        public ServerState State
        {
            get
            {
                if (myServer.Status != NetPeerStatus.Running)
                    return ServerState.NotRunning;
                else
                    return myState;
            }
        }

        // Отримує порт, до якого вдалося підключитися ігровому серверу
        public int Port
        {
            get { return myNetConfig.Port; }
        }

        // Створює новий екземпляр ігрового сервера
        public GameServer(int numPlayers = 4)
        {
            myTag = new ServerTag
            {
                SupportedPlayerCount = numPlayers
            };

            myPlayRules = new List<IGamePlayRule>();
            myStateRules = new List<IGameStateRule>();
            myInitRules = new List<IGameInitRule>();

            myPlayers = new PlayerCollection(numPlayers);
            myBots = new List<BotPlayer>();

            myState = ServerState.InLobby;
            myGameState = new GameState();
            myGameState.OnStateChanged += MyGameState_OnStateChanged;

            myMessageHandlers = new Dictionary<MessageType, PacketHandler>();

            InitServer();
        }

        #region Ініціалізація


        // Ініціалізує сервер
        private void InitServer()
        {
            // Створіть нову конфігурацію мережі
            NetPeerConfiguration netConfig = new NetPeerConfiguration(NetSettings.APP_IDENTIFIER);

            myAddress = NetUtils.GetAddress();

            // Дозволити вхідні підключення
            netConfig.AcceptIncomingConnections = true;
            // Встановіть адресу
            netConfig.LocalAddress = myAddress;
            // Встановіть час очікування між тактами, перш ніж клієнт вважатиметься від’єднаним
            netConfig.ConnectionTimeout = NetSettings.DEFAULT_SERVER_TIMEOUT;
            // Встановіть максимальну кількість підключень до кількості гравців
            netConfig.MaximumConnections = myPlayers.Count;
            // Встановіть порт для використання
            netConfig.Port = NetUtils.GetOpenPort(NetSettings.DEFAULT_SERVER_PORT + 1);
            // Переробити старі повідомлення (покращує продуктивність)
            netConfig.UseMessageRecycling = true;

            // Повідомлення про підтвердження підключення (запити на підключення)
            netConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            // Отримуємо дані
            netConfig.EnableMessageType(NetIncomingMessageType.Data);
            // Приймаємо запити на відкриття
            netConfig.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            // Приймаємо повідомлення про зміну статусу (підключення/відключення клієнта)
            netConfig.EnableMessageType(NetIncomingMessageType.StatusChanged);
            // Приймаємо оновлення затримки підключення (пульси)
            netConfig.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            myNetConfig = netConfig;

            // Створює однорангову мережу
            myServer = new NetServer(netConfig);

            // Реєструє функцію зворотного виклику
            myServer.RegisterReceivedCallback(new SendOrPostCallback(MessageReceived));

            LoadRules();

            // Додайте обробники повідомлень
            myMessageHandlers.Add(MessageType.SendMove, HandleGameMove);
            myMessageHandlers.Add(MessageType.StartGame, HandleHostReqStart);
            myMessageHandlers.Add(MessageType.RequestServerState, HandleStateRequest);
            myMessageHandlers.Add(MessageType.AddBot, HandleAddBot);
            myMessageHandlers.Add(MessageType.KickBot, HandleHostReqKick);
            myMessageHandlers.Add(MessageType.LogChat, HandlePlayerChat);
        }

        // Завантажує правила гри
        private void LoadRules()
        {
            Utils.FillTypeList(AppDomain.CurrentDomain, myPlayRules);
            Utils.FillTypeList(AppDomain.CurrentDomain, myStateRules);
            myInitRules = Rules.INIT_RULES;
        }

        // Запускає цей сервер, щоб почати приймати повідомлення
        public void Run()
        {
            int port = myNetConfig.Port;
            int maxClients = 1000;
            int index = 0;

            while (index < maxClients)
            {
                try
                {
                    // Почати гру
                    myServer.Start();
                    break;
                }
                catch (Exception)
                {
                    myNetConfig = myNetConfig.Clone();
                    port++;
                    myNetConfig.Port = port;
                    myServer = new NetServer(myNetConfig);
                    index++;
                }
            }
        }

        // Зупиняє гру
        public void Stop()
        {
            myServer.Shutdown(NetSettings.DEFAULT_SERVER_SHUTDOWN_MESSAGE);
        }

        #endregion

        #region Утиліти

        // Отримує інформацію про те, чи може гравець зіграти карту, це просто повертає істину чи хибність і не виконує та не реєструє це
        public bool CanPlayMove(Player player, PlayingCard card)
        {
            card.FaceUp = true;

            GameMove move = new GameMove(player, card);

            string failReason = "";

            for (int index = 0; index < myPlayRules.Count; index++)
            {
                if (!myPlayRules[index].IsValidMove(this, move, ref failReason))
                {
                    return false;
                }
            }

            return true;
        }

        // Викликається станом гри, коли параметр змінено
        private void MyGameState_OnStateChanged(object sender, StateParameter e)
        {
            if (myState == ServerState.InGame && e.IsSynced)
            {
                NetOutgoingMessage msg = myServer.CreateMessage();
                msg.Write((byte)MessageType.GameStateChanged);
                e.Encode(msg);

                SendToAll(msg);
            }
        }

        // Зареєструвати логи
        private void Log(string message, params object[] format)
        {
            Logger.Write(message, format);

            myOutput?.AppendText(string.Format(message, format) + "\n");
        }

        // Встановлює стан гри для сервера та оновлює всіх клієнтів
        private void SetServerState(ServerState state, string reason = "Game Started")
        {
            if (myState != state)
            {
                myState = state;

                NetOutgoingMessage updateMessage = myServer.CreateMessage();
                updateMessage.Write((byte)MessageType.NotifyServerStateChanged);
                updateMessage.Write((byte)state);
                updateMessage.Write(reason);
                SendToAll(updateMessage);

                if (state == ServerState.InGame)
                {
                    Log("Starting game");

                    myGameState.SilentSets = true;

                    for (int index = 0; index < myInitRules.Count; index++)
                        myInitRules[index].InitState(this);

                    myGameState.SilentSets = false;

                    TransferGameState();

                    isInitialized = true;
                }
                else if (state == ServerState.InLobby)
                {
                    myGameState.Clear();

                    if (myPlayers.Where(X => !X.IsBot).Count() == 0)
                        myBots.Clear();

                    isInitialized = false;
                }
            }
        }

        // Передає весь стан гри всім клієнтам
        private void TransferGameState()
        {
            NetOutgoingMessage msg = myServer.CreateMessage();
            msg.Write((byte)MessageType.FullGameState);
            myGameState.Encode(msg);

            SendToAll(msg);
        }

        // Повідомляє підключення про те, що сервер наразі перебуває в неправильному стані для цього повідомлення
        private void NotifyBadState(NetConnection connection, string reason)
        {
            NetOutgoingMessage outMsg = myServer.CreateMessage();

            outMsg.Write((byte)MessageType.InvalidServerState);
            outMsg.Write(reason);

            myServer.SendMessage(outMsg, connection, NetDeliveryMethod.ReliableOrdered);
        }

        // Обробляє, коли гравця видалено
        private void PlayerLeft(Player player, string reason)
        {
            myPlayers.Remove(player);

            NetOutgoingMessage outMsg = myServer.CreateMessage();

            outMsg.Write((byte)MessageType.BotKicked);
            outMsg.Write(player.PlayerId);
            outMsg.Write(reason);

            myTag.PlayerCount = myPlayers.PlayerCount;

            SendToAll(outMsg);

            if (player == myGameHost)
            {
                for(byte index = 0; index < myPlayers.Count; index ++)
                {
                    if (myPlayers[index] != null)
                    {
                        myGameHost = myPlayers[index];

                        NotifyHostChanged(index);
                        break;
                    }
                }
            }
            else
            {
                if (State != ServerState.InLobby && !player.IsBot)
                {
                    BotPlayer bot = BotPlayer.CreateBot(player, this);

                    bot.Player.OnCardAddedToHand += PlayerGainedCard;
                    bot.Player.OnCardRemovedFromHand += PlayerRemovedCard;

                    myBots.Add(bot);
                    myPlayers[bot.Player.PlayerId] = bot.Player;
                    NotifyPlayerJoined(bot.Player);
                }
            }
        }

        // Повідомляє всіх підключених клієнтів про те, що до гри приєднався новий гравець
        private void NotifyPlayerJoined(Player player)
        {
            NetOutgoingMessage outMsg = myServer.CreateMessage();

            outMsg.Write((byte)MessageType.PlayerJoined);
            outMsg.Write(player.PlayerId);
            outMsg.Write(player.Name);
            outMsg.Write(player.IsBot);
            outMsg.Write(player.IsHost);
            outMsg.WritePadBits();

            SendToAll(outMsg);
        }

        // Обробляється, коли гравець приєднується до цього сервера
        private void PlayerJoined(ClientTag clientTag, NetConnection connection)
        {
            int id = myPlayers.GetNextAvailableId();

            if (id != -1)
            {
                Player player = new Player(clientTag, connection, (byte)id);

                player.OnCardAddedToHand += PlayerGainedCard;
                player.OnCardRemovedFromHand += PlayerRemovedCard;

                if (id == 0)
                {
                    myGameHost = player;
                    player.IsHost = true;
                    Log("Setting host to \"{0}\"", player.Name);
                }

                connection.Approve();
                
                myPlayers[player.PlayerId] = player;

                myTag.PlayerCount = myPlayers.PlayerCount;
            }
        }

        // Викликається, коли гравець викинув карту
        private void PlayerRemovedCard(object sender, PlayingCard e)
        {
            Player player = sender as Player;

            NotifyNewCardState(player, e, false);
        }

        // Викликається, коли гравець взяв карту
        private void PlayerGainedCard(object sender, PlayingCard e)
        {
            Player player = sender as Player;

            NotifyNewCardState(player, e, true);
        }

        // Повідомте клієнтів, коли клієнт змінив руку
        private void NotifyNewCardState(Player player, PlayingCard e, bool added)
        {
            if (player.Connection != null)
            {
                NetOutgoingMessage msg = myServer.CreateMessage();
                msg.Write((byte)MessageType.PlayerHandChanged);
                msg.Write(added);
                msg.Write(e != null);
                msg.WritePadBits();
                if (e != null)
                {
                    msg.Write((byte)e.Rank);
                    msg.Write((byte)e.Suit);
                }
                myServer.SendMessage(msg, player.Connection, NetDeliveryMethod.ReliableOrdered, 0);
            }

            NetOutgoingMessage msg2 = myServer.CreateMessage();
            msg2.Write((byte)MessageType.CardCountChanged);
            msg2.Write(player.PlayerId);
            msg2.Write(player.Hand.Count);
            SendToAll(msg2);
        }

        // Надсилає вітальний пакет зазначеному клієнту
        private void SendWelcomePacket(byte playerId)
        {
            Player player = myPlayers[playerId];

            NetOutgoingMessage msg = myServer.CreateMessage();
            msg.Write((byte)MessageType.PlayerConnectInfo);
            msg.Write((byte)playerId);
            msg.Write((bool)(player == myGameHost));
            msg.WritePadBits();

            myTag.WriteToPacket(msg);

            msg.Write(myPlayers.Count);
            msg.Write(myPlayers.PlayerCount);

            for (byte index = 0; index < myPlayers.Count; index++)
            {
                myPlayers[index]?.Encode(msg);
            }

            myGameState.Encode(msg);

            myServer.SendMessage(msg, player.Connection, NetDeliveryMethod.ReliableOrdered);
        }

        // Керується гравцем, який робить ігровий хід
        private void HandleMove(GameMove move)
        {
            string failReason = "Unknown";

            Log("Player {0} wants to play {1}", move.Player.PlayerId, move.Move);

            // Ітерує кожне правило гри
            for (int index = 0; index < myPlayRules.Count; index++)
            {
                if (!myPlayRules[index].IsValidMove(this, move, ref failReason))
                {
                    if (failReason == "Unknown")
                        failReason = "Помилка в " + myPlayRules[index].ReadableName;

                    NotifyInvalidMove(move, failReason);

                    if (LogLongRules)
                        Log("\tНе вдалось отримати \"{0}\": {1}", myPlayRules[index].ReadableName, failReason);
                    return; 
                }
                else if (LogLongRules)
                {
                    Log("\tПрийнято \"{0}\"", myPlayRules[index].ReadableName);
                }
            }

            Log("Move played");

            NetOutgoingMessage outMsg = myServer.CreateMessage();

            outMsg.Write((byte)MessageType.SucessfullMove);
            move.Encode(outMsg);

            SendToAll(outMsg);

            for (int index = 0; index < Rules.MOVE_SUCCESS_RULES.Count; index++)
            {
                Rules.MOVE_SUCCESS_RULES[index].UpdateState(this, move);
            }
        }

        // Повідомляє всіх клієнтів про те, що hsot гри змінився
        private void NotifyHostChanged(byte newHostId)
        {
            NetOutgoingMessage msg = myServer.CreateMessage();
            msg.Write(newHostId);
            SendToAll(msg);
        }

        // Повідомляє клієнта, що він зробив недійсний хід
        private void NotifyInvalidMove(GameMove move, string reason)
        {
            NetOutgoingMessage outMsg = myServer.CreateMessage();

            outMsg.Write((byte)MessageType.InvalidMove);
            move.Encode(outMsg);
            outMsg.Write(reason);

            if (move.Player.Connection != null)
                myServer.SendMessage(outMsg, move.Player.Connection, NetDeliveryMethod.ReliableOrdered);
        }

        // Надсилає повідомлення всім клієнтам із сервера
        public void SendServerMessage(string message, params object[] parameters)
        {
            // Prepare message
            NetOutgoingMessage send = myServer.CreateMessage();
            send.Write((byte)MessageType.LogChat);
            send.Write((byte)255);
            send.Write(string.Format(message, parameters));

            Log("[Server]: {0}", string.Format(message, parameters));

            SendToAll(send);
        }

        // Надсилає задане мережеве повідомлення всім підключеним клієнтам
        private void SendToAll(NetOutgoingMessage msg)
        {
            if (myServer.Connections.Count > 0)
                myServer.SendMessage(msg, myServer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        #endregion

        #region Обробка повідомлень

        // Обробляє, коли сервер отримав повідомлення
        private void MessageReceived(object peer)
        {
            
            NetIncomingMessage inMsg = ((NetPeer)peer).ReadMessage();

            try
            {
                switch (inMsg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)inMsg.ReadByte();
                        string reason = inMsg.ReadString();

                        switch (status)
                        {
                            case NetConnectionStatus.Connected:
                                SendWelcomePacket(myPlayers[inMsg.SenderConnection].PlayerId);
                                break;
                        }

                        break;

                    case NetIncomingMessageType.ConnectionApproval:
                   
                        ClientTag clientTag = ClientTag.ReadFromPacket(inMsg);

                        if (myState == ServerState.InLobby)
                        {
                                    PlayerJoined(clientTag, inMsg.SenderConnection);
                        }
                        break;

                    case NetIncomingMessageType.Data:
                        HandleMessage(inMsg);
                        break;
                }
            }
            catch (Exception e)
            {
                Log("Encountered exception parsing packet from {0}:\n\t{1}", inMsg.SenderEndPoint, e);
                Logger.Write(e);
            }

            PumpMessages();
        }

        // Прокачує чергу повідомлень, оновлюючи перевірки стану на сервері та ботів
        public void PumpMessages()
        {
            myTag.PlayerCount = myPlayers.PlayerCount;
            myTag.SupportedPlayerCount = myPlayers.Count;

            if (myState == ServerState.InGame && isInitialized)
            {
                foreach (IGameStateRule stateRule in Rules.STATE_RULES)
                {
                    stateRule.ValidateState(this);
                }

                foreach (BotPlayer botPlayer in myBots)
                {
                    botPlayer.StateUpdated();

                    if (botPlayer.ShouldInvoke)
                    {
                        if (botPlayer.InstantValidateCheck())
                        {
                            PlayingCard move = botPlayer.DetermineMove();
                            HandleMove(new GameMove(botPlayer.Player, move));
                        }
                        else
                            botPlayer.ShouldInvoke = false;
                    }
                }
            }
        }
                
        private void HandleMessage(NetIncomingMessage inMessage)
        {
            while(inMessage.PositionInBytes < inMessage.LengthBytes)
            {
                MessageType messageType = (MessageType)inMessage.ReadByte();

                if (myMessageHandlers.ContainsKey(messageType))
                    myMessageHandlers[messageType].Invoke(inMessage);
                else
                {
                    Log("Invalid message received from \"{0}\" ({1})", myPlayers[inMessage.SenderConnection].Name, inMessage.SenderEndPoint);
                    inMessage.ReadBytes(inMessage.LengthBytes - inMessage.PositionInBytes);
                }
            }
        }

        // Обробляє повідомлення, отримане, коли гравець просить зробити ігровий хід
        private void HandleGameMove(NetIncomingMessage msg)
        {
            GameMove move = GameMove.DecodeFromClient(msg, myPlayers);

            if (myState == ServerState.InGame)
            {
                if (move.Player == myPlayers[msg.SenderConnection])
                    HandleMove(move); 
                else
                    Log("Bad packet received from \"{0}\" ({1})", myPlayers[msg.SenderConnection].Name, msg.SenderEndPoint);
            }
            else
            {
                NotifyBadState(msg.SenderConnection, "Game is not currently running");
                Log("Player \"{0}\" attempted move during non-game state", myPlayers[msg.SenderConnection].Name, msg.SenderEndPoint);
            }
        }

        // Обробляє повідомлення, отримане, коли хост запитує запуск гри
        private void HandleHostReqStart(NetIncomingMessage msg)
        {
            bool start = msg.ReadBoolean();
            msg.ReadPadBits();

            if (myPlayers[msg.SenderConnection] == myGameHost)
            {

                bool isLobbyReady = true;

                if (isLobbyReady)
                    SetServerState(ServerState.InGame);
            }
        }

        // Обробляє повідомлення, отримане, коли клієнт запитує стан сервера
        private void HandleStateRequest(NetIncomingMessage msg)
        {
            ServerState state = (ServerState)msg.ReadByte();

            SetServerState(state);
        }

        // Обробляє, коли хост запитує додавання бота
        private void HandleAddBot(NetIncomingMessage msg)
        {
            byte difficulty = msg.ReadByte();
            string botName = msg.ReadString();

            if (string.IsNullOrWhiteSpace(botName))
            {                
                Random r = new Random();
                int randomLineNumber = r.Next(0, BOT_NAMES.Length - 1);
                botName = BOT_NAMES[randomLineNumber];
                botName = botName.Trim();
                botName = char.ToUpper(botName[0]) + botName.Substring(1);
            }

            if (myState == ServerState.InLobby)
            {
                int playerID = myPlayers.GetNextAvailableId();

                Log("Attempting to add bot \"{0}\" with difficulty {1}", botName, difficulty / 255.0f);

                if (playerID != -1)
                {
                    Player botPlayer = new Player(new ClientTag(botName), (byte)playerID) { IsBot = true };
                    BotPlayer bot = new BotPlayer(this, botPlayer, (difficulty / 256.0f));

                    botPlayer.OnCardAddedToHand += PlayerGainedCard;
                    botPlayer.OnCardRemovedFromHand += PlayerRemovedCard;

                    myPlayers[(byte)playerID] = botPlayer;

                    myBots.Add(bot);

                    NotifyPlayerJoined(botPlayer);

                    Log("Bot added");
                }
                else
                {
                    Log("Failed to add bot, lobby full");
                }
            }
            else
            {
                NotifyBadState(msg.SenderConnection, "Cannot add bot outside of lobby");
                Log("Failed to add bot during game");
            }
        }

        // Обробляє, коли господар хоче вдарити гравця ногою
        private void HandleHostReqKick(NetIncomingMessage msg)
        {
            byte playerId = msg.ReadByte();
            string reason = msg.ReadString();

            if (myPlayers[msg.SenderConnection] == myGameHost && myPlayers[playerId] != null && playerId != myGameHost.PlayerId)
            {
                NetOutgoingMessage send = myServer.CreateMessage();
                send.Write(playerId);
                send.Write(reason);

                if (myPlayers[playerId].IsBot)
                {
                    myBots.Remove(myBots.FirstOrDefault(x => x.Player == myPlayers[playerId]));
                    PlayerLeft(myPlayers[playerId], reason);
                }
            }

        }

        // Обробляє, коли гравець надіслав повідомлення чату
        private void HandlePlayerChat(NetIncomingMessage msg)
        {
            byte playerId = myPlayers[msg.SenderConnection].PlayerId;
            string message = msg.ReadString();

            NetOutgoingMessage send = myServer.CreateMessage();
            send.Write((byte)MessageType.LogChat);
            send.Write(playerId);
            send.Write(message);

            Log("[{0}]: {1}", myPlayers[msg.SenderConnection], message);

            SendToAll(send);
        }

        #endregion
    }
}
