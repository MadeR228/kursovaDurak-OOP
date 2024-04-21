namespace DurakGame
{
    partial class frmDurakMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDurakMain));
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnPlaySingle = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Impact", 68.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTitle.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTitle.Location = new System.Drawing.Point(34, 23);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(332, 112);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ДУРЕНЬ";
            // 
            // btnPlaySingle
            // 
            this.btnPlaySingle.BackColor = System.Drawing.Color.Transparent;
            this.btnPlaySingle.Font = new System.Drawing.Font("Impact", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlaySingle.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnPlaySingle.Location = new System.Drawing.Point(95, 138);
            this.btnPlaySingle.Name = "btnPlaySingle";
            this.btnPlaySingle.Size = new System.Drawing.Size(202, 45);
            this.btnPlaySingle.TabIndex = 2;
            this.btnPlaySingle.Text = "Почати гру";
            this.btnPlaySingle.UseVisualStyleBackColor = false;
            this.btnPlaySingle.Click += new System.EventHandler(this.btnPlay_Click);
            this.btnPlaySingle.MouseEnter += new System.EventHandler(this.ButtonMouseEntered);
            this.btnPlaySingle.MouseLeave += new System.EventHandler(this.ButtonMouseLeft);
            // 
            // btnAbout
            // 
            this.btnAbout.BackColor = System.Drawing.Color.Transparent;
            this.btnAbout.Font = new System.Drawing.Font("Impact", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbout.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnAbout.Location = new System.Drawing.Point(95, 202);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(202, 45);
            this.btnAbout.TabIndex = 4;
            this.btnAbout.Text = "Правила гри";
            this.btnAbout.UseVisualStyleBackColor = false;
            this.btnAbout.Click += new System.EventHandler(this.btnRules_Click);
            this.btnAbout.MouseEnter += new System.EventHandler(this.ButtonMouseEntered);
            this.btnAbout.MouseLeave += new System.EventHandler(this.ButtonMouseLeft);
            // 
            // frmDurakMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGreen;
            this.BackgroundImage = global::DurakGame.Properties.Resources.background;
            this.ClientSize = new System.Drawing.Size(389, 285);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnPlaySingle);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDurakMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "\"Дурень\"";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnPlaySingle;
        private System.Windows.Forms.Button btnAbout;
    }
}