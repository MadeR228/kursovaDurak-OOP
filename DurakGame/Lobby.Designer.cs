namespace DurakGame
{
    partial class frmLobby
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLobby));
            this.pnlPlayers = new System.Windows.Forms.Panel();
            this.pnlAddBot = new System.Windows.Forms.Panel();
            this.btnAddBot = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtBotName = new System.Windows.Forms.TextBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.grpGameSettings = new System.Windows.Forms.GroupBox();
            this.chkSimulateBotThinkTime = new System.Windows.Forms.CheckBox();
            this.rbn36Cards = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.pnlPlayers.SuspendLayout();
            this.pnlAddBot.SuspendLayout();
            this.grpGameSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPlayers
            // 
            this.pnlPlayers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPlayers.Controls.Add(this.pnlAddBot);
            this.pnlPlayers.Location = new System.Drawing.Point(13, 14);
            this.pnlPlayers.Name = "pnlPlayers";
            this.pnlPlayers.Size = new System.Drawing.Size(270, 370);
            this.pnlPlayers.TabIndex = 0;
            // 
            // pnlAddBot
            // 
            this.pnlAddBot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAddBot.Controls.Add(this.btnAddBot);
            this.pnlAddBot.Controls.Add(this.label7);
            this.pnlAddBot.Controls.Add(this.txtBotName);
            this.pnlAddBot.Location = new System.Drawing.Point(5, 5);
            this.pnlAddBot.Name = "pnlAddBot";
            this.pnlAddBot.Size = new System.Drawing.Size(260, 60);
            this.pnlAddBot.TabIndex = 6;
            // 
            // btnAddBot
            // 
            this.btnAddBot.Location = new System.Drawing.Point(205, 2);
            this.btnAddBot.Name = "btnAddBot";
            this.btnAddBot.Size = new System.Drawing.Size(46, 46);
            this.btnAddBot.TabIndex = 8;
            this.btnAddBot.Text = "+";
            this.btnAddBot.UseVisualStyleBackColor = true;
            this.btnAddBot.Click += new System.EventHandler(this.btnAddBot_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Ім\'я бота:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBotName
            // 
            this.txtBotName.Location = new System.Drawing.Point(83, 17);
            this.txtBotName.Name = "txtBotName";
            this.txtBotName.Size = new System.Drawing.Size(102, 20);
            this.txtBotName.TabIndex = 2;
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(13, 401);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "Вийти";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // grpGameSettings
            // 
            this.grpGameSettings.Controls.Add(this.chkSimulateBotThinkTime);
            this.grpGameSettings.Controls.Add(this.rbn36Cards);
            this.grpGameSettings.Controls.Add(this.label3);
            this.grpGameSettings.Location = new System.Drawing.Point(290, 14);
            this.grpGameSettings.Name = "grpGameSettings";
            this.grpGameSettings.Size = new System.Drawing.Size(190, 91);
            this.grpGameSettings.TabIndex = 3;
            this.grpGameSettings.TabStop = false;
            this.grpGameSettings.Text = "Налаштування гри";
            // 
            // chkSimulateBotThinkTime
            // 
            this.chkSimulateBotThinkTime.AutoSize = true;
            this.chkSimulateBotThinkTime.Checked = true;
            this.chkSimulateBotThinkTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSimulateBotThinkTime.Enabled = false;
            this.chkSimulateBotThinkTime.Location = new System.Drawing.Point(7, 20);
            this.chkSimulateBotThinkTime.Name = "chkSimulateBotThinkTime";
            this.chkSimulateBotThinkTime.Size = new System.Drawing.Size(145, 17);
            this.chkSimulateBotThinkTime.TabIndex = 6;
            this.chkSimulateBotThinkTime.Text = "Імітація мислення ботів";
            this.chkSimulateBotThinkTime.UseVisualStyleBackColor = true;
            // 
            // rbn36Cards
            // 
            this.rbn36Cards.AutoSize = true;
            this.rbn36Cards.Checked = true;
            this.rbn36Cards.Enabled = false;
            this.rbn36Cards.Location = new System.Drawing.Point(7, 56);
            this.rbn36Cards.Name = "rbn36Cards";
            this.rbn36Cards.Size = new System.Drawing.Size(63, 17);
            this.rbn36Cards.TabIndex = 2;
            this.rbn36Cards.TabStop = true;
            this.rbn36Cards.Text = "36 карт";
            this.rbn36Cards.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Кількість карт:";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(391, 401);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Почати";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // frmLobby
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 435);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.grpGameSettings);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.pnlPlayers);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLobby";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "\"Дурень\"";
            this.pnlPlayers.ResumeLayout(false);
            this.pnlAddBot.ResumeLayout(false);
            this.pnlAddBot.PerformLayout();
            this.grpGameSettings.ResumeLayout(false);
            this.grpGameSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlPlayers;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.GroupBox grpGameSettings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel pnlAddBot;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtBotName;
        private System.Windows.Forms.RadioButton rbn36Cards;
        private System.Windows.Forms.Button btnAddBot;
        private System.Windows.Forms.CheckBox chkSimulateBotThinkTime;
    }
}