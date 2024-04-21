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
    // Форма для демонстрації правил гри
    public partial class frmRules : Form
    {
        // Створює нову форму «Правила».
        public frmRules()
        {
            InitializeComponent();

            wbrMain.Navigate(Environment.CurrentDirectory + "/Resources/rules.html");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
