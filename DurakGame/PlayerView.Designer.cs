namespace DurakGame
{
    partial class PlayerView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblPlayerName = new System.Windows.Forms.Label();
            this.imgPlayerType = new System.Windows.Forms.PictureBox();
            this.cmsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.kickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imgKick = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgPlayerType)).BeginInit();
            this.cmsContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgKick)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPlayerName
            // 
            this.lblPlayerName.AutoSize = true;
            this.lblPlayerName.Location = new System.Drawing.Point(59, 21);
            this.lblPlayerName.Name = "lblPlayerName";
            this.lblPlayerName.Size = new System.Drawing.Size(73, 13);
            this.lblPlayerName.TabIndex = 0;
            this.lblPlayerName.Text = "[Player Name]";
            // 
            // imgPlayerType
            // 
            this.imgPlayerType.Location = new System.Drawing.Point(5, 5);
            this.imgPlayerType.Margin = new System.Windows.Forms.Padding(0);
            this.imgPlayerType.Name = "imgPlayerType";
            this.imgPlayerType.Size = new System.Drawing.Size(50, 50);
            this.imgPlayerType.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgPlayerType.TabIndex = 2;
            this.imgPlayerType.TabStop = false;
            // 
            // cmsContextMenu
            // 
            this.cmsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kickToolStripMenuItem});
            this.cmsContextMenu.Name = "cmsContextMenu";
            this.cmsContextMenu.Size = new System.Drawing.Size(68, 26);
            // 
            // kickToolStripMenuItem
            // 
            this.kickToolStripMenuItem.Name = "kickToolStripMenuItem";
            this.kickToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
            // 
            // imgKick
            // 
            this.imgKick.Location = new System.Drawing.Point(205, 5);
            this.imgKick.Margin = new System.Windows.Forms.Padding(0);
            this.imgKick.Name = "imgReady";
            this.imgKick.Size = new System.Drawing.Size(50, 50);
            this.imgKick.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgKick.TabIndex = 1;
            this.imgKick.TabStop = false;
            // 
            // PlayerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.imgPlayerType);
            this.Controls.Add(this.imgKick);
            this.Controls.Add(this.lblPlayerName);
            this.Name = "PlayerView";
            this.Size = new System.Drawing.Size(260, 60);
            ((System.ComponentModel.ISupportInitialize)(this.imgPlayerType)).EndInit();
            this.cmsContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgKick)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPlayerName;
        private System.Windows.Forms.PictureBox imgPlayerType;
        private System.Windows.Forms.ContextMenuStrip cmsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem kickToolStripMenuItem;
        private System.Windows.Forms.PictureBox imgKick;
    }
}
