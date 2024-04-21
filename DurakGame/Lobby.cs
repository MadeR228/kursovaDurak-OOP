using Durak.Client;
using Durak.Common;
using Durak.Server;
using DurakGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DurakGame
{
    // Форма для відображення лобі одиночної гри
    public partial class frmLobby : Form
    {
        // Зберігає гру
        private GameServer myServer;
        // Зберігає клієнт гри
        private GameClient myClient;

        // Зберігає список гравців
        private readonly List<PlayerView> myViews;

        // Створює нову форму лобі
        public frmLobby()
        {
            InitializeComponent();

            myViews = new List<PlayerView>();

            DialogResult = DialogResult.OK;

            chkSimulateBotThinkTime.Checked = Settings.Default.DefaultBotsThink;
        }

        // Ініціалізує гру
        public void InitGame()
        {
            myServer = new GameServer(Settings.Default.DefaultMaxPlayers)
            {
                SinglePlayerMode = true,
            };

            myServer.Run();

            InitClient();

            myClient.ConnectTo(myServer, "", myServer.Port);
        }



        // Обробляє налаштування місцевого клієнта та підключає його події
        private void InitClient()
        {
            if (myClient == null)
                myClient = new GameClient(new ClientTag(Settings.Default.UserName));

            myClient.OnServerStateUpdated += ServerStateUpdated;
            myClient.OnFinishedConnect += ClientConnected;
            myClient.OnPlayerConnected += PlayerConnected;
            myClient.OnPlayerLeft += PlayerLeft;

            if (!myClient.IsReady)
                myClient.Run();
        }

        // Викликається, коли гравець покинув гру
        private void PlayerLeft(object sender, Player player, string reason)
        {
            PlayerView toRemove = myViews.FirstOrDefault(x => x.Player == player);
            myViews.Remove(toRemove);

            if (toRemove != null)
                pnlPlayers.Controls.Remove(toRemove);

            UpdatePlayerView();
        }

        // Викликається, коли гравець приєднався до гри
        private void PlayerConnected(object sender, Player e)
        {
            PlayerView view = BuildView(e);
            pnlPlayers.Controls.Add(view);

            myViews.Add(view);

            UpdatePlayerView();
        }

        // Викликається після підключення клієнта до сервера та отримання вітального пакету
        private void ClientConnected(object sender, EventArgs e)
        {
            myViews.Clear();

            foreach (Player p in myClient.KnownPlayers)
            {
                PlayerView view = BuildView(p);
                if (p.PlayerId == myClient.PlayerId)
                    view.HasControl = true;
                view.Left = 5;

                pnlPlayers.Controls.Add(view);
                myViews.Add(view);
            }
            UpdatePlayerView();

            if (!myClient.IsHost)
                grpGameSettings.Visible = false;
        }

        // Оновлює панель гравця
        private void UpdatePlayerView()
        {
            for (int index = 0; index < myViews.Count; index++)
            {
                myViews[index].Top = 5 + index * myViews[index].Height;
            }

            if (myClient.KnownPlayers.PlayerCount == myClient.KnownPlayers.Count)
            {
                pnlAddBot.Visible = false;
            }
            else
            {
                pnlAddBot.Visible = myClient.IsHost;
                pnlAddBot.Top = 5 + myViews.Count * 60;
            }
        }

        // Створює перегляд гравця для вказаного гравця
        private PlayerView BuildView(Player p)
        {
            PlayerView view = new PlayerView
            {
                Client = myClient,
                Player = p,
                Left = 5
            };

            return view;
        }

        // Викликається, коли змінився стан підключеного сервера
        private void ServerStateUpdated(object sender, ServerState e)
        {
            if (e == ServerState.InGame)
            {
                this.Hide();

                frmDurakGame mainForm = new frmDurakGame();
                mainForm.SetClient(myClient);
                mainForm.SetServer(myServer);

                mainForm.ShowDialog();

                System.Threading.Thread.Sleep(15);

                if (myClient.ConnectedServer == null)
                {
                    this.Close();
                }
                else
                {
                    this.Show();

                }
            }
        }

        // Викликається, коли натискається кнопка "Вийти". Це повертає до головного меню
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Викликається, коли натискається кнопка "Додати бота"
        private void btnAddBot_Click(object sender, EventArgs e)
        {
            myClient.AddBot((byte)(Settings.Default.DefaultBotDifficulty * 255), txtBotName.Text);
        }


        // Запит на початок гри
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (myClient.IsHost)
            {
                if (myClient.KnownPlayers.PlayerCount > 1)
                {

                    int numCards = 36;

                    if (numCards / myClient.KnownPlayers.PlayerCount >= 6)
                    {

                        myClient.RequestState(StateParameter.Construct<int>(Names.NUM_INIT_CARDS, numCards, true));
                        myClient.Start();
                    }
                }
                else
                {
                    MessageBox.Show("Для гри потрібно принаймні 2 гравці", "Увага", MessageBoxButtons.OK);
                }
            }
            else
            {
                myClient.Start();
            }
        }
    }
}
