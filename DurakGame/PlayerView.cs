using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DurakGame.Properties;
using Durak.Common;
using Durak.Client;

namespace DurakGame
{
    // Власний елемент управління, який використовується для відображення гравця в лобі
    public partial class PlayerView : UserControl
    {
        // Забезпечується поле для властивості hasControl
        private bool hasControl;
        // Забезпечується поле для властивості Player
        private Player myPlayer;
        // Забезпечується поле для властивості Client
        private GameClient myClient;

        // Отримує або задає, чи має клієнт контроль над гравцем
        public bool HasControl
        {
            get { return hasControl; }
            set
            {
                if (hasControl != value)
                {
                    hasControl = value;

                }
            }
        }

        // Отримує або задає гравця, якого представляє цей вигляд
        public Player Player
        {
            get { return myPlayer; }
            set
            {
                myPlayer = value;

                if (myPlayer != null)
                {
                    if (myPlayer.IsBot)
                    {
                        imgPlayerType.Image = Resources.bot;

                        if (Client.IsHost)
                        {
                            imgKick.Click += KickClicked;
                        }
                    }
                    else
                        imgPlayerType.Image = Resources.silhoutte;

                    lblPlayerName.Text = myPlayer.Name;
                    DetermineImage();
                }
            }
        }

        // Отримує або задає клієнта, який рендерить цей вигляд
        public GameClient Client
        {
            get { return myClient; }
            set
            {
                myClient = value;

                cmsContextMenu.Enabled = myClient.IsHost;
            }
        }


        // Створює новий порожній вигляд гравця
        public PlayerView()
        {
            InitializeComponent();
        }

        // Викликається, коли натиснуто кнопку вигнання
        private void KickClicked(object sender, EventArgs e)
        {
            if (myPlayer.IsBot && Client.IsHost)
            {
                Client.Kick(myPlayer, "Вигнати бота");
            }
        }

        // Визначає зображення готовності для цього вигляду
        private void DetermineImage()
        {
            if (Player.IsBot)
                imgKick.Image = Client.IsHost ? Resources.delete : null;
        }
    }
}
