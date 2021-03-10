namespace Perforce.P4VS.UI
{
    partial class LaunchingHMFA
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LaunchingHMFA));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.closeBtn = new Perforce.I18nControls.GridButton();
            this.launchingGridLbl = new Perforce.I18nControls.GridLabel();
            this.expiredGridLbl = new Perforce.I18nControls.GridLabel();
            this.downloadBtn = new Perforce.I18nControls.GridButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.Image = global::Perforce.P4VS.Resources.download_icon;
            this.pictureBox.Location = new System.Drawing.Point(196, 79);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(98, 78);
            this.pictureBox.TabIndex = 3;
            this.pictureBox.TabStop = false;
            // 
            // closeBtn
            // 
            this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBtn.CellHeight = 0;
            this.closeBtn.CellWidth = 0;
            this.closeBtn.Column = 0;
            this.closeBtn.ColumnsSpanned = 0;
            this.closeBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeBtn.Location = new System.Drawing.Point(397, 226);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Row = 0;
            this.closeBtn.RowsSpanned = 0;
            this.closeBtn.Size = new System.Drawing.Size(75, 23);
            this.closeBtn.TabIndex = 6;
            this.closeBtn.Text = "Close";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.YOffset = 0;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // launchingGridLbl
            // 
            this.launchingGridLbl.AutoSize = true;
            this.launchingGridLbl.CellHeight = 0;
            this.launchingGridLbl.CellWidth = 0;
            this.launchingGridLbl.Column = 0;
            this.launchingGridLbl.ColumnsSpanned = 0;
            this.launchingGridLbl.Location = new System.Drawing.Point(143, 188);
            this.launchingGridLbl.Name = "launchingGridLbl";
            this.launchingGridLbl.Row = 0;
            this.launchingGridLbl.RowsSpanned = 0;
            this.launchingGridLbl.Size = new System.Drawing.Size(183, 13);
            this.launchingGridLbl.TabIndex = 5;
            this.launchingGridLbl.Text = "Launching Helix MFA Authenticator...";
            this.launchingGridLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.launchingGridLbl.YOffset = 0;
            // 
            // expiredGridLbl
            // 
            this.expiredGridLbl.AutoSize = true;
            this.expiredGridLbl.CellHeight = 0;
            this.expiredGridLbl.CellWidth = 0;
            this.expiredGridLbl.Column = 0;
            this.expiredGridLbl.ColumnsSpanned = 0;
            this.expiredGridLbl.Location = new System.Drawing.Point(13, 13);
            this.expiredGridLbl.MaximumSize = new System.Drawing.Size(440, 0);
            this.expiredGridLbl.Name = "expiredGridLbl";
            this.expiredGridLbl.Row = 0;
            this.expiredGridLbl.RowsSpanned = 0;
            this.expiredGridLbl.Size = new System.Drawing.Size(424, 26);
            this.expiredGridLbl.TabIndex = 4;
            this.expiredGridLbl.Text = "Your session has expired. You must authenticate your identity to continue working" +
    " in this open connection.";
            this.expiredGridLbl.YOffset = 0;
            // 
            // downloadBtn
            // 
            this.downloadBtn.CellHeight = 0;
            this.downloadBtn.CellWidth = 0;
            this.downloadBtn.Column = 0;
            this.downloadBtn.ColumnsSpanned = 0;
            this.downloadBtn.Location = new System.Drawing.Point(298, 226);
            this.downloadBtn.Name = "downloadBtn";
            this.downloadBtn.Row = 0;
            this.downloadBtn.RowsSpanned = 0;
            this.downloadBtn.Size = new System.Drawing.Size(75, 23);
            this.downloadBtn.TabIndex = 7;
            this.downloadBtn.Text = "Download";
            this.downloadBtn.UseVisualStyleBackColor = true;
            this.downloadBtn.YOffset = 0;
            this.downloadBtn.Click += new System.EventHandler(this.downloadBtn_Click);
            // 
            // LaunchingHMFA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeBtn;
            this.ClientSize = new System.Drawing.Size(484, 261);
            this.Controls.Add(this.downloadBtn);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.launchingGridLbl);
            this.Controls.Add(this.expiredGridLbl);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LaunchingHMFA";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Verify your identity for {0} on {1}";
            this.Shown += new System.EventHandler(this.LaunchingHMFA_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private I18nControls.GridLabel expiredGridLbl;
        private I18nControls.GridLabel launchingGridLbl;
        private I18nControls.GridButton closeBtn;
        private I18nControls.GridButton downloadBtn;
    }
}