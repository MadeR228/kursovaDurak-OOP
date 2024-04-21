using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DurakGame
{
    public partial class frmDurakMain : Form
    {
        public frmDurakMain()
        {
            InitializeComponent();
        }

        // Викликається, коли миша входить у межі кнопки(hover)
        private void ButtonMouseEntered(object sender, EventArgs e)
        {
            (sender as Button).BackColor = Color.White;
        }

        // Викликається, коли миша залишає межі кнопки
        private void ButtonMouseLeft(object sender, EventArgs e)
        {
            (sender as Button).BackColor = Color.White;
        }

        // Викликається, коли натиснута кнопка "Почати гру"
        private void btnPlay_Click(object sender, EventArgs e)
        {
            this.Hide();

            frmLobby lobby = new frmLobby();
            lobby.InitGame();
            lobby.ShowDialog();

            this.Show();
        }

        // Викликається, коли натиснута кнопка "Правила гри"
        private void btnRules_Click(object sender, EventArgs e)
        {
            Hide();

            new frmRules().ShowDialog();

            Show();
        }
    }
}
