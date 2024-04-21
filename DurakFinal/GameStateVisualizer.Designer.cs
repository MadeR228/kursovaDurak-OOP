namespace DurakCommon
{
    partial class GameStateVisualizer
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
            this.dgvMainView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMainView)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMainView
            // 
            this.dgvMainView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMainView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMainView.Location = new System.Drawing.Point(0, 0);
            this.dgvMainView.Name = "dgvMainView";
            this.dgvMainView.ReadOnly = true;
            this.dgvMainView.RowHeadersVisible = false;
            this.dgvMainView.Size = new System.Drawing.Size(460, 400);
            this.dgvMainView.TabIndex = 0;
            // 
            // GameStateVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvMainView);
            this.Name = "GameStateVisualizer";
            this.Size = new System.Drawing.Size(460, 400);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMainView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMainView;
    }
}
