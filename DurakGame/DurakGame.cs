using Durak;
using Durak.Client;
using Durak.Common;
using Durak.Common.Cards;
using Durak.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DurakGame
{
    // Представляє основну форму гри
    public partial class frmDurakGame : Form
    {
        SoundPlayer sf = new SoundPlayer("E:\\kursova\\DurakGame\\Resources\\sound.wav");
        // Структура для зберігання елементів інтерфейсу користувача для гравця
        private struct PlayerUITag
        {
            // Отримує або задає панель, на якій сидить цей гравець
            public BorderPanel Panel;
            // Отримує або задає мітку, яка показує ім'я гравця
            public Label NameLabel;
            // Отримує або задає мітку, яка показує кількість карт гравця
            public Label CardCountLabel;
            // Отримує або задає контейнер для карт, що показує картки цього гравця
            public CardBox CardBox;
        }

        // Зберігає ігровий клієнт
        private GameClient myClient;
        // Зберігає гру сервера
        private GameServer myServer;

        // Зберігає список тегів інтерфейсу гравця
        private readonly Dictionary<Player, PlayerUITag> myPlayerUIs;

        // Відстежування,чи грубо закрили клієнт
        bool isHardClose = true;

        // Створює нову форму гри клієнта
        public frmDurakGame()
        {
            InitializeComponent();

            myPlayerUIs = new Dictionary<Player, PlayerUITag>();

            // Початково встановити видимість панелей гравців на false, поки не буде вказано, чи вони в грі.
            pnlPlayer1.Visible = false;
            pnlPlayer2.Visible = false;
            pnlPlayer3.Visible = false;
            pnlPlayer4.Visible = false;
            pnlPlayer5.Visible = false;
        }

        // Встановлює сервер для цієї гри
        public void SetServer(GameServer server)
        {
            myServer = server;
        }

        // Встановлює клієнта для гри
        public void SetClient(GameClient client)
        {
            myClient = client;

            myClient.LocalState.AddStateChangedEvent("attacking_card", 0, (X, Y) => { cbxPlayerAttack1.Card = Y.GetValuePlayingCard(); });
            myClient.LocalState.AddStateChangedEvent("attacking_card", 1, (X, Y) => { cbxPlayerAttack2.Card = Y.GetValuePlayingCard(); });
            myClient.LocalState.AddStateChangedEvent("attacking_card", 2, (X, Y) => { cbxPlayerAttack3.Card = Y.GetValuePlayingCard(); });
            myClient.LocalState.AddStateChangedEvent("attacking_card", 3, (X, Y) => { cbxPlayerAttack4.Card = Y.GetValuePlayingCard(); });
            myClient.LocalState.AddStateChangedEvent("attacking_card", 4, (X, Y) => { cbxPlayerAttack5.Card = Y.GetValuePlayingCard(); });
            myClient.LocalState.AddStateChangedEvent("attacking_card", 5, (X, Y) => { cbxPlayerAttack6.Card = Y.GetValuePlayingCard(); });

            myClient.LocalState.AddStateChangedEvent("defending_card", 0, (X, Y) => { cbxDefence1.Card = Y.GetValuePlayingCard(); });
            myClient.LocalState.AddStateChangedEvent("defending_card", 1, (X, Y) => { cbxDefence2.Card = Y.GetValuePlayingCard(); });
            myClient.LocalState.AddStateChangedEvent("defending_card", 2, (X, Y) => { cbxDefence3.Card = Y.GetValuePlayingCard(); });
            myClient.LocalState.AddStateChangedEvent("defending_card", 3, (X, Y) => { cbxDefence4.Card = Y.GetValuePlayingCard(); });
            myClient.LocalState.AddStateChangedEvent("defending_card", 4, (X, Y) => { cbxDefence5.Card = Y.GetValuePlayingCard(); });
            myClient.LocalState.AddStateChangedEvent("defending_card", 5, (X, Y) => { cbxDefence6.Card = Y.GetValuePlayingCard(); });

            myClient.OnInvalidMove += (X, Y, Z) => { MessageBox.Show(Z, "Помилка"); };

            myClient.LocalState.AddStateChangedEvent(Names.TRUMP_CARD, (X, Y) => { cbxTrump.Card = Y.GetValuePlayingCard(); });


            myClient.LocalState.AddStateChangedEvent(Names.TRUMP_CARD_USED, TrumpPickedUp);


            myClient.LocalState.AddStateChangedEvent(Names.DISCARD, (X, Y) => { dscDiscard.Clear(); foreach (PlayingCard card in Y.GetValueCardCollection()) dscDiscard.AddCard(card); });

            myClient.LocalState.AddStateChangedEvent(Names.DECK_COUNT, (X, Y) => { lblCardsLeft.Text = "" + Y.GetValueInt(); if (Y.GetValueInt() == 0) cbxDeck.Card = null; });

            myClient.LocalState.AddStateChangedEvent(Names.GAME_OVER, GameOver);

            myClient.OnServerStateUpdated += (X, Y) => { if (Y == ServerState.InLobby) { isHardClose = false; this.Close(); } };

            myClient.OnChat += ReceivedChat;

            myClient.LocalState.AddStateChangedEvent(Names.ATTACKING_PLAYER, AttackingPlayersChanged);
            myClient.LocalState.AddStateChangedEvent(Names.DEFENDING_PLAYER, AttackingPlayersChanged);


            int localIndex = 0;
            for (byte index = 0; index < myClient.KnownPlayers.Count; index++)
            {
                Player player = myClient.KnownPlayers[index];
                if (player != null && player.PlayerId != myClient.PlayerId)
                {
                    PlayerUITag tag = new PlayerUITag();

                    switch (localIndex)
                    {
                        case 0:
                            tag.Panel = pnlPlayer1;
                            tag.NameLabel = lblPlayer1;
                            tag.CardCountLabel = lblPlayer1CardsLeft;
                            tag.CardBox = cbxPlayer1;
                            break;
                        case 1:
                            tag.Panel = pnlPlayer2;
                            tag.NameLabel = lblPlayer2;
                            tag.CardCountLabel = lblPlayer2CardsLeft;
                            tag.CardBox = cbxPlayer2;
                            break;
                        case 2:
                            tag.Panel = pnlPlayer3;
                            tag.NameLabel = lblPlayer3;
                            tag.CardCountLabel = lblPlayer3CardsLeft;
                            tag.CardBox = cbxPlayer3;
                            break;
                        case 3:
                            tag.Panel = pnlPlayer4;
                            tag.NameLabel = lblPlayer4;
                            tag.CardCountLabel = lblPlayer4CardsLeft;
                            tag.CardBox = cbxPlayer4;
                            break;
                        case 4:
                            tag.Panel = pnlPlayer5;
                            tag.NameLabel = lblPlayer5;
                            tag.CardCountLabel = lblPlayer5CardsLeft;
                            tag.CardBox = cbxPlayer5;
                            break;
                    }

                    myPlayerUIs.Add(player, tag);

                    tag.Panel.Visible = true;

                    localIndex++;
                }
            }

            myPlayerUIs.Add(myClient.KnownPlayers[myClient.PlayerId], new PlayerUITag() { Panel = pnlMyView });

            foreach (KeyValuePair<Player, PlayerUITag> pair in myPlayerUIs)
            {
                PlayerUITag tag = pair.Value;

                if (tag.NameLabel != null)
                    tag.NameLabel.Text = pair.Key.Name;

                if (tag.CardCountLabel != null)
                    tag.CardCountLabel.Text = pair.Key.NumCards.ToString();
            }

            myClient.OnPlayerCardCountChanged += PlayerCardCountChanged;

            cplPlayersHand.Cards = myClient.Hand;

        }


        // Викликається, коли козирна карта була піднята
        private void TrumpPickedUp(object sender, StateParameter p)
        {
            cbxTrump.Enabled = false;
        }

        // Викликається, коли отримано повідомлення чату
        private void ReceivedChat(object sender, Player player, string message)
        {
            int start = rtbChatLog.Text.Length;
            rtbChatLog.AppendText(player == null ? "[Сервер]" : "[" + player.Name + "]");
            rtbChatLog.Select(start, rtbChatLog.Text.Length - start);
            rtbChatLog.SelectionColor = player == null ? Color.Purple : player.IsHost ? Color.Yellow : Color.LightBlue;
            rtbChatLog.Select(rtbChatLog.Text.Length, 0);
            rtbChatLog.SelectionColor = Color.Black;

            rtbChatLog.AppendText(" " + message + "\n");
            rtbChatLog.Select(rtbChatLog.Text.Length - 1, 1);
            rtbChatLog.ScrollToCaret();
        }

        // Викликається, коли гра закінчена
        private void GameOver(object sender, StateParameter p)
        {
            string message = "Гру закінчено!\n";
            sf.Stop();

            if (myClient.LocalState.GetValueBool(Names.IS_TIE))
                message += "Нічия!";
            else
            {
                Player durak = myClient.KnownPlayers[myClient.LocalState.GetValueByte(Names.LOSER_ID)];

                message += durak.Name + " - \"Дурень\"";
            }

            DialogResult result = MessageBox.Show(message, "Гру закінчено", MessageBoxButtons.OK);

            if (myClient.IsHost && result == DialogResult.OK)
            {
                myClient.RequestServerState(ServerState.InLobby);
            }
        }

        // Викликається, коли змінився атакуючий або захисний гравець
        private void AttackingPlayersChanged(object sender, StateParameter p)
        {
            foreach (KeyValuePair<Player, PlayerUITag> pair in myPlayerUIs)
            {
                PlayerUITag tag = pair.Value;

                if (tag.Panel != null)
                    tag.Panel.ShowBorder = false;
            }

            Player attackingPlayer = myClient.KnownPlayers[myClient.LocalState.GetValueByte(Names.ATTACKING_PLAYER)];
            Player defendingPlayer = myClient.KnownPlayers[myClient.LocalState.GetValueByte(Names.DEFENDING_PLAYER)];

            BorderPanel myAttackingPlayerContainer = myPlayerUIs[attackingPlayer].Panel;
            BorderPanel myDefendingPlayerContainer = myPlayerUIs[defendingPlayer].Panel;

            myAttackingPlayerContainer.ShowBorder = true;
            myAttackingPlayerContainer.BorderColor = Color.Red;

            myDefendingPlayerContainer.ShowBorder = true;
            myDefendingPlayerContainer.BorderColor = Color.Blue;

            if (attackingPlayer.PlayerId == 0)
            {
                btnForfeit.Text = "Бито";
            }
            else
            {
                btnForfeit.Text = "Взяти карти";
            }
        }
         
        // Перевизначає подію OnClosing для цієї форми, буде припиняти роботу клієнта, якщо це грубе закриття
        protected override void OnClosing(CancelEventArgs e)
        {
            if (isHardClose)
            {
                myClient.Disconnect();
                myServer?.Stop();
                sf.Stop();
            }
        }

        // Викликається, коли змінився лічильник карт гравця
        private void PlayerCardCountChanged(Durak.Common.Player player, int newCardCount)
        {
            if (player.PlayerId != myClient.PlayerId)
            {
                PlayerUITag tag = myPlayerUIs[player];

                tag.CardCountLabel.Text = newCardCount.ToString();
            }
        }

        // Викликається, коли вибрана карта в гравця який бере карти
        private void cplPlayersHand_OnCardSelected(object sender, Durak.Common.Cards.CardEventArgs e)
        {
            myClient?.RequestMove(e.Card);
        }

        // Викликається, коли натиснуто кнопку "взяти карти/бито"
        private void btnForfeit_Click(object sender, EventArgs e)
        {
            myClient.RequestMove(null);
        }

        private void frmDurakGame_Load(object sender, EventArgs e)
        {
            sf.PlayLooping();
        }
    }
}