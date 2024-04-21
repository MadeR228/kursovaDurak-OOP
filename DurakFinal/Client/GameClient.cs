using Durak.Common;
using Durak.Common.Cards;
using Durak.Server;
using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

namespace Durak.Client
{
    // Представляє гру. Це може бути бот, гравець
    public class GameClient
    {
        #region Стани

        // Зберігає тег цього клієнта
        private ClientTag myTag;
        // Зберігає однорангову мережу
        private NetPeer myPeer;
        // Зберігає IP-адресу клієнта
        private IPAddress myAddress;
        // Серверний тег підключеного сервера
        private ServerTag? myConnectedServer;
        // Зберігає ідентифікатор гравця клієнта
        private byte myPlayerId;
        // Зберігає незалежно від того, чи є цей клієнт хостом
        private bool isHost;
        // Зберігає обробники пакетів
        private Dictionary<MessageType, PacketHandler> myMessageHandlers;
        // Зберігає стан локальної гри клієнта
        private GameState myLocalState;
        // Зберігає колекцію місцевих ігор
        private PlayerCollection myKnownPlayers;
        // Зберігає, чи готовий цей клієнт
        private bool isReady = false;
        // Зберігає руку клієнта
        private CardCollection myHand;

        #endregion

        #region Події

        // Викликається, коли стан сервера було оновлено
        public event EventHandler<ServerState> OnServerStateUpdated;
        // Викликається, коли гравець або бот приєднується до гри
        public event EventHandler<Player> OnPlayerConnected;
        // Викликається, коли гравець залишив гру, але до того, як його буде видалено з локальної колекції гравців
        public event PlayerLeftEvent OnPlayerLeft;
        // Викликається, коли гравець зробив хід. Це може бути хід, який виконав клієнт
        public event EventHandler<GameMove> OnMoveReceived;
        // Викликається, коли клієнт зробив недійсний хід
        public event InvalidMoveEvent OnInvalidMove;
        // Викликається, коли надсилається повідомлення в чат
        public event PlayerChatEvent OnChat;
        // Викликається, коли клієнт отримує картку
        public event EventHandler<PlayingCard> OnHandCardAdded;
        // Викликається, коли рука клієнта втрачає картку
        public event EventHandler<PlayingCard> OnHandCardRemoved;
        // Викликається, коли кількість карток клієнта на руках змінилася
        public event PlayerCardCountChangedEvent OnPlayerCardCountChanged;
        // Викликається після завершення підключення клієнта до сервера
        public event EventHandler OnFinishedConnect;

        #endregion

        #region Властивості

        // Отримує або встановлює тег об’єкта для цього клієнта
        public object Tag
        {
            get;
            set;
        }
        // Отримує або встановлює, чи готовий цей клієнт до гри
        public bool IsReady
        {
            get { return isReady; }
            set
            {
                if (value != isReady)
                    SetReadiness(isReady);
            }
        }

        // Отримує інформацію про те, чи підключений цей клієнт до сервера
        public bool IsConnected
        {
            get { return (myConnectedServer != null && myPeer.ConnectionsCount > 0); }
        }
        // Отримує ідентифікатор гравця клієнта цієї гри
        public byte PlayerId
        {
            get { return myPlayerId; }
        }
        // Отримує локальний стан гри цього клієнта
        public GameState LocalState
        {
            get { return myLocalState; }
        }
        // Отримує колекцію гравців, які, як відомо цьому клієнту, існують на його сервері
        public PlayerCollection KnownPlayers
        {
            get { return myKnownPlayers; }
        }
        // Отримує тег поточного підключеного сервера
        public ServerTag? ConnectedServer
        {
            get { return myConnectedServer; }
        }
        // Отримує руку клієнта
        public CardCollection Hand
        {
            get { return myHand; }
        }
        // Отримує інформацію про те, чи є цей клієнт поточним хостом гри
        public bool IsHost
        {
            get { return isHost; }
        }

        #endregion

        #region Конструктори та ініціалізація

        // Створює новий ігровий клієнт із заданим тегом клієнта
        public GameClient(ClientTag tag)
        {
            myTag = tag;

            Initialize();
        }

        // Ініціалізує клієнт
        private void Initialize()
        {
            // Create a new net config
            NetPeerConfiguration netConfig = new NetPeerConfiguration(NetSettings.APP_IDENTIFIER);

            // Отримує IP клієнта
            myAddress = NetUtils.GetAddress();

            // Створює словник
            myMessageHandlers = new Dictionary<MessageType, PacketHandler>();

            // Визначає стан гри
            myLocalState = new GameState();
            // Визначає колекцію гравців
            myKnownPlayers = new PlayerCollection();
            // Визначає карти в руці
            myHand = new CardCollection();

   
            // Встановлює адресу
            netConfig.LocalAddress = myAddress;
            // Встановлює порт для використання
            netConfig.Port = NetUtils.GetOpenPort(NetSettings.DEFAULT_SERVER_PORT + 1);
            // Переробляємо старі повідомлення (покращує продуктивність)
            netConfig.UseMessageRecycling = true;

            // Приймаємо повідомлення про підтвердження підключення (запити на підключення)
            netConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            // Отримуємо дані
            netConfig.EnableMessageType(NetIncomingMessageType.Data);
            // Приймаємо повідомлення про зміну статусу (підключення/відключення клієнта)
            netConfig.EnableMessageType(NetIncomingMessageType.StatusChanged);
            // Приймаємо оновлення затримки підключення (пульси)
            netConfig.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            // Створює однорангову мережу
            myPeer = new NetServer(netConfig);

            // Зареєструйте функцію зворотного виклику. Лідгрен впорається з потоками за нас
            myPeer.RegisterReceivedCallback(new SendOrPostCallback(MessageReceived));

            // Додає всі обробники
            myMessageHandlers.Add(MessageType.PlayerConnectInfo, HandleConnectInfo);
            myMessageHandlers.Add(MessageType.GameStateChanged, HandleStateChanged);
            myMessageHandlers.Add(MessageType.FullGameState, HandleStateReceived);
            myMessageHandlers.Add(MessageType.NotifyServerStateChanged, HandleServerStateReceived);

            myMessageHandlers.Add(MessageType.PlayerJoined, HandlePlayerAdded);
            myMessageHandlers.Add(MessageType.BotKicked, HandlePlayerDeleted);

            myMessageHandlers.Add(MessageType.SucessfullMove, HandleMoveReceived);
            myMessageHandlers.Add(MessageType.InvalidMove, HandleInvalidMove);
            myMessageHandlers.Add(MessageType.LogChat, HandlePlayerChat);

            myMessageHandlers.Add(MessageType.CardCountChanged, HandleCardCountChanged);
            myMessageHandlers.Add(MessageType.PlayerHandChanged, HandleCardChanged);
        }

        // Запускає цей сервер
        public void Run()
        {
            if (myPeer.Status == NetPeerStatus.NotRunning || myPeer.Status == NetPeerStatus.ShutdownRequested)
            {
                myPeer.Configuration.Port = NetUtils.GetOpenPort(NetSettings.DEFAULT_SERVER_PORT + 4);

                myPeer.Start();

                Logger.Write("Starting game client on port {0}", myPeer.Configuration.Port);
            }
        }

        #endregion

        #region Обробники повідомлень

        // Викликається, коли одноранговий мережевий вузол отримав повідомлення
        private void MessageReceived(object state)
        {
            // Отримати вхідне повідомлення
            NetIncomingMessage inMsg = ((NetPeer)state).ReadMessage();

            try
            {
                // Визначте тип повідомлення, щоб правильно його обробити
                switch (inMsg.MessageType)
                {
                    // Обробляти, коли сервер отримав запит на відкриття
                    case NetIncomingMessageType.DiscoveryResponse:
                        ServerTag serverTag = ServerTag.ReadFromPacket(inMsg);

                        break;

                    // Обробляє, коли сервер отримав дані
                    case NetIncomingMessageType.Data:
                        HandleMessage(inMsg);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Write(e);
            }
        }

        // Обробляє вхідне повідомлення даних
        private void HandleMessage(NetIncomingMessage inMsg)
        {
            // Read the message type
            MessageType messageType = (MessageType)inMsg.ReadByte();

            if (myMessageHandlers.ContainsKey(messageType))
                myMessageHandlers[messageType].Invoke(inMsg);
        }

        // Обробляє пакет зі зміненою карткою
        private void HandleCardChanged(NetIncomingMessage inMsg)
        {
            bool added = inMsg.ReadBoolean();
            bool hasValue = inMsg.ReadBoolean();
            inMsg.ReadPadBits();

            if (hasValue)
            {
                CardRank rank = (CardRank)inMsg.ReadByte();
                CardSuit suit = (CardSuit)inMsg.ReadByte();

                PlayingCard card = new PlayingCard(rank, suit) { FaceUp = true };

                if (added)
                {
                    myHand.Add(card);
                    
                    OnHandCardAdded?.Invoke(this, card);
                }
                else
                {
                    myHand.Discard(card);
                    
                    OnHandCardRemoved?.Invoke(this, card);
                }
            }
        }

        // Обробляє повідомлення про зміну кількості карток
        private void HandleCardCountChanged(NetIncomingMessage inMsg)
        {
            byte playerId = inMsg.ReadByte();
            int numCards = inMsg.ReadInt32();

            myKnownPlayers[playerId].NumCards = numCards;
            
            OnPlayerCardCountChanged?.Invoke(myKnownPlayers[playerId], numCards);
        }

        // Обробляє, коли отримано повідомлення про зміну параметра стану
        private void HandleStateChanged(NetIncomingMessage inMsg)
        {
            StateParameter.Decode(inMsg, myLocalState);
        }


        // Обробляє, коли було отримано інформаційний пакет підключення
        private void HandleConnectInfo(NetIncomingMessage inMsg)
        {
            myPlayerId = inMsg.ReadByte();
            isHost = inMsg.ReadBoolean();
            inMsg.ReadPadBits();

            myConnectedServer = ServerTag.ReadFromPacket(inMsg);

            int supportedPlayers = inMsg.ReadInt32();
            int numPlayers = inMsg.ReadByte();

            myKnownPlayers.Resize(supportedPlayers);

            for (int index = 0; index < numPlayers; index++)
            {
                Player player = new Player(0, "", false);
                player.Decode(inMsg);

                myKnownPlayers[player.PlayerId] = player;
            }

            myLocalState.Decode(inMsg);

            OnFinishedConnect?.Invoke(this, EventArgs.Empty);
        }

        // Обробляє, коли отримано пакет передачі стану гри
        private void HandleStateReceived(NetIncomingMessage inMsg)
        {
            myLocalState.Decode(inMsg);
        }

        // Обробляє тип пакета serverStateUpdated
        private void HandleServerStateReceived(NetIncomingMessage inMsg)
        {
            if (myConnectedServer != null)
            {
                ServerTag tag = myConnectedServer.Value;
                tag.State = (ServerState)inMsg.ReadByte();
                myConnectedServer = tag;
                
                OnServerStateUpdated?.Invoke(this, tag.State);
            }
        }


        // Обробляє, коли гравець доданий
        private void HandlePlayerAdded(NetIncomingMessage inMsg)
        {
            byte playerId = inMsg.ReadByte();
            string name = inMsg.ReadString();
            bool isBot = inMsg.ReadBoolean();
            bool isHost = inMsg.ReadBoolean();
            inMsg.ReadPadBits();

            myKnownPlayers[playerId] = new Player(playerId, name, isBot)
            {
                IsHost = isHost
            };

            OnPlayerConnected?.Invoke(this, myKnownPlayers[playerId]);
        }

        // Обробляє, коли гравець видалений
        private void HandlePlayerDeleted(NetIncomingMessage inMsg)
        {
            byte playerId = inMsg.ReadByte();
            string reason = inMsg.ReadString();

            Player left = myKnownPlayers[playerId];

            myKnownPlayers[playerId] = null;
            
            OnPlayerLeft?.Invoke(this, left, reason);
        }


        // Обробляє відтворений пакет
        private void HandleMoveReceived(NetIncomingMessage inMsg)
        {
            GameMove move = GameMove.Decode(inMsg, myKnownPlayers);
            
            OnMoveReceived?.Invoke(this, move);
        }

        // Обробляє недійсний хід
        private void HandleInvalidMove(NetIncomingMessage inMsg)
        {
            GameMove move = GameMove.Decode(inMsg, myKnownPlayers);
            string reason = inMsg.ReadString();
            
            OnInvalidMove?.Invoke(this, move.Move, reason);
        }

        // Обробляє повідомлення чату гравця
        private void HandlePlayerChat(NetIncomingMessage inMsg)
        {
            byte playerId = inMsg.ReadByte();
            string message = inMsg.ReadString();
            
            OnChat?.Invoke(this, playerId == 255 ? null : myKnownPlayers[playerId], message);
        }

        #endregion

        #region Утиліти

        // Відключає цей клієнт від поточного підключеного сервера
        public void Disconnect()
        {
            if (myPeer.ConnectionsCount > 0)
            {
                if (myConnectedServer != null)
                {
                    myConnectedServer = null;
                    myKnownPlayers.Clear();
                    myLocalState.Clear();
                    myHand.Clear();
                    isReady = false;
                    isHost = false;
                }
            }
        }

        // Підключається до екземпляра локального сервера
        public void ConnectTo(GameServer server, string serverPassword = "", int port = NetSettings.DEFAULT_SERVER_PORT)
        {
            if (myConnectedServer == null)
            {
                NetOutgoingMessage hailMessage = myPeer.CreateMessage();
                myTag.WriteToPacket(hailMessage);
                hailMessage.Write(serverPassword);

                myConnectedServer = server.Tag;

                myPeer.Connect(new IPEndPoint(server.IP, port), hailMessage);
            }
        }

        // Відправляє повідомлення на сервер
        private void Send(NetOutgoingMessage msg)
        {
            myPeer.SendMessage(msg, myPeer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        // Запити на переїзд, які має зробити цей клієнт
        public void RequestMove(PlayingCard card)
        {
            if (IsConnected)
            {
                GameMove move = new GameMove(myKnownPlayers[myPlayerId], card);

                NetOutgoingMessage msg = myPeer.CreateMessage();
                msg.Write((byte)MessageType.SendMove);
                move.Encode(msg);
                Send(msg);
            }
        }

        // Запитує сервер встановити задане значення стану
        public void RequestState(StateParameter param)
        {
            if (IsConnected)
            {
                NetOutgoingMessage msg = myPeer.CreateMessage();
                msg.Write((byte)MessageType.RequestState);
                param.Encode(msg);
                Send(msg);
            }
        }


        // Запитує сервер перейти в заданий стан
        public void RequestServerState(ServerState param)
        {
            if (IsConnected && IsHost)
            {
                NetOutgoingMessage msg = myPeer.CreateMessage();
                msg.Write((byte)MessageType.RequestServerState);
                msg.Write((byte)param);
                Send(msg);
            }
        }

        // Запит на видалення бота з гри
        public void Kick(Player player, string reason)
        {
            if (IsConnected && isHost)
            {
                NetOutgoingMessage msg = myPeer.CreateMessage();
                msg.Write((byte)MessageType.KickBot);
                msg.Write(player.PlayerId);
                msg.Write(reason);
                Send(msg);
            }
        }

        // Запит на додавання бота до гри
        public void AddBot(byte difficulty, string name)
        {
            if (IsConnected && isHost)
            {
                NetOutgoingMessage msg = myPeer.CreateMessage();
                msg.Write((byte)MessageType.AddBot);
                msg.Write(difficulty);
                msg.Write(name);
                Send(msg);
            }
        }

        // Запит для запуску гру
        public void Start()
        {
            if (IsConnected && isHost)
            {
                NetOutgoingMessage msg = myPeer.CreateMessage();
                msg.Write((byte)MessageType.StartGame);
                msg.WritePadBits();
                Send(msg);
            }
        }

        // Встановлює готовність цього клієнта
        public void SetReadiness(bool ready)
        {
            if (myConnectedServer != null && myPeer.ConnectionsCount > 0)
            {
                isReady = ready;

                NetOutgoingMessage msg = myPeer.CreateMessage();
                msg.Write((byte)MessageType.GameReady);
                msg.Write(ready);
                Send(msg);
            }
        }

        #endregion
    }
}
