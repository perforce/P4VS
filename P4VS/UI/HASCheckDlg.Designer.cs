namespace Perforce.P4VS.UI
{
    partial class HASCheckDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HASCheckDlg));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.timeoutGridLbl = new Perforce.I18nControls.GridLabel();
            this.authenticatingGridLbl = new Perforce.I18nControls.GridLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.Image = global::Perforce.P4VS.Resources.download_icon;
            this.pictureBox.Location = new System.Drawing.Point(189, 71);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(82, 81);
            this.pictureBox.TabIndex = 3;
            this.pictureBox.TabStop = false;
            // 
            // timeoutGridLbl
            // 
            this.timeoutGridLbl.AutoSize = true;
            this.timeoutGridLbl.CellHeight = 0;
            this.timeoutGridLbl.CellWidth = 0;
            this.timeoutGridLbl.Column = 0;
            this.timeoutGridLbl.ColumnsSpanned = 0;
            this.timeoutGridLbl.Location = new System.Drawing.Point(81, 188);
            this.timeoutGridLbl.Name = "launchingGridLbl";
            this.timeoutGridLbl.Row = 0;
            this.timeoutGridLbl.RowsSpanned = 0;
            this.timeoutGridLbl.Size = new System.Drawing.Size(299, 13);
            this.timeoutGridLbl.TabIndex = 5;
            this.timeoutGridLbl.Text = "This dialog will timeout if no external authentication is provided";
            this.timeoutGridLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.timeoutGridLbl.YOffset = 0;
            // 
            // authenticatingGridLbl
            // 
            this.authenticatingGridLbl.AutoSize = true;
            this.authenticatingGridLbl.CellHeight = 0;
            this.authenticatingGridLbl.CellWidth = 0;
            this.authenticatingGridLbl.Column = 0;
            this.authenticatingGridLbl.ColumnsSpanned = 0;
            this.authenticatingGridLbl.Location = new System.Drawing.Point(188, 22);
            this.authenticatingGridLbl.MaximumSize = new System.Drawing.Size(440, 0);
            this.authenticatingGridLbl.Name = "expiredGridLbl";
            this.authenticatingGridLbl.Row = 0;
            this.authenticatingGridLbl.RowsSpanned = 0;
            this.authenticatingGridLbl.Size = new System.Drawing.Size(84, 13);
            this.authenticatingGridLbl.TabIndex = 4;
            this.authenticatingGridLbl.Text = "Authenticating...";
            this.authenticatingGridLbl.YOffset = 0;
            // 
            // HASCheckDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 218);
            this.ControlBox = false;
            this.Controls.Add(this.timeoutGridLbl);
            this.Controls.Add(this.authenticatingGridLbl);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HASCheckDlg";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Verify your identity for {0} on {1}";
            this.Shown += new System.EventHandler(this.LaunchingHMFA_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private I18nControls.GridLabel authenticatingGridLbl;
        private I18nControls.GridLabel timeoutGridLbl;
    }
}