namespace Perforce.P4VS
{
    partial class SSLCertificateErrorDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SSLCertificateErrorDlg));
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.CopyToClipboardBtn = new Perforce.I18nControls.GridButton();
            this.ErrorsLB = new Perforce.I18nControls.GridListBox();
            this.gridLabel3 = new Perforce.I18nControls.GridLabel();
            this.YesBtn = new Perforce.I18nControls.GridButton();
            this.gridLabel2 = new Perforce.I18nControls.GridLabel();
            this.NoBtn = new Perforce.I18nControls.GridButton();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLabel1 = new Perforce.I18nControls.GridLabel();
            this.CertificateTextTB = new Perforce.I18nControls.GridTextBox();
            this.MessageLbl = new Perforce.I18nControls.GridLabel();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.CopyToClipboardBtn);
            this.gridLayoutPanel1.Controls.Add(this.ErrorsLB);
            this.gridLayoutPanel1.Controls.Add(this.gridLabel3);
            this.gridLayoutPanel1.Controls.Add(this.YesBtn);
            this.gridLayoutPanel1.Controls.Add(this.gridLabel2);
            this.gridLayoutPanel1.Controls.Add(this.NoBtn);
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.gridLabel1);
            this.gridLayoutPanel1.Controls.Add(this.CertificateTextTB);
            this.gridLayoutPanel1.Controls.Add(this.MessageLbl);
            this.gridLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLayoutPanel1.EnableDesignerGrid = true;
            this.gridLayoutPanel1.EnableDesignerLayout = false;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            this.gridLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.gridLayoutPanel1.Size = new System.Drawing.Size(372, 392);
            this.gridLayoutPanel1.TabIndex = 0;
            // 
            // CopyToClipboardBtn
            // 
            this.CopyToClipboardBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CopyToClipboardBtn.CellHeight = 29;
            this.CopyToClipboardBtn.CellWidth = 111;
            this.CopyToClipboardBtn.Column = 1;
            this.CopyToClipboardBtn.ColumnsSpanned = 0;
            this.CopyToClipboardBtn.Location = new System.Drawing.Point(55, 361);
            this.CopyToClipboardBtn.Name = "CopyToClipboardBtn";
            this.CopyToClipboardBtn.Row = 6;
            this.CopyToClipboardBtn.RowsSpanned = 0;
            this.CopyToClipboardBtn.Size = new System.Drawing.Size(105, 23);
            this.CopyToClipboardBtn.TabIndex = 3;
            this.CopyToClipboardBtn.Text = "Copy To Clipboard";
            this.CopyToClipboardBtn.UseVisualStyleBackColor = true;
            this.CopyToClipboardBtn.YOffset = 0;
            this.CopyToClipboardBtn.Click += new System.EventHandler(this.CopyToClipboardBtn_Click);
            // 
            // ErrorsLB
            // 
            this.ErrorsLB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorsLB.CellHeight = 62;
            this.ErrorsLB.CellWidth = 362;
            this.ErrorsLB.Column = 0;
            this.ErrorsLB.ColumnsSpanned = 3;
            this.ErrorsLB.FormattingEnabled = true;
            this.ErrorsLB.HorizontalScrollbar = true;
            this.ErrorsLB.Location = new System.Drawing.Point(8, 62);
            this.ErrorsLB.Name = "ErrorsLB";
            this.ErrorsLB.Row = 2;
            this.ErrorsLB.RowsSpanned = 0;
            this.ErrorsLB.Size = new System.Drawing.Size(356, 56);
            this.ErrorsLB.TabIndex = 1;
            this.ErrorsLB.YOffset = 0;
            // 
            // gridLabel3
            // 
            this.gridLabel3.AutoSize = true;
            this.gridLabel3.CellHeight = 13;
            this.gridLabel3.CellWidth = 260;
            this.gridLabel3.Column = 0;
            this.gridLabel3.ColumnsSpanned = 2;
            this.gridLabel3.Location = new System.Drawing.Point(8, 46);
            this.gridLabel3.Name = "gridLabel3";
            this.gridLabel3.Row = 1;
            this.gridLabel3.RowsSpanned = 0;
            this.gridLabel3.Size = new System.Drawing.Size(87, 13);
            this.gridLabel3.TabIndex = 6;
            this.gridLabel3.Text = "Certificate Errors:";
            this.gridLabel3.YOffset = 0;
            // 
            // YesBtn
            // 
            this.YesBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.YesBtn.CellHeight = 29;
            this.YesBtn.CellWidth = 102;
            this.YesBtn.Column = 2;
            this.YesBtn.ColumnsSpanned = 0;
            this.YesBtn.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.YesBtn.Location = new System.Drawing.Point(166, 361);
            this.YesBtn.Name = "YesBtn";
            this.YesBtn.Row = 6;
            this.YesBtn.RowsSpanned = 0;
            this.YesBtn.Size = new System.Drawing.Size(96, 23);
            this.YesBtn.TabIndex = 4;
            this.YesBtn.Text = "Add Exception";
            this.YesBtn.UseVisualStyleBackColor = true;
            this.YesBtn.YOffset = 0;
            // 
            // gridLabel2
            // 
            this.gridLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridLabel2.CellHeight = 31;
            this.gridLabel2.CellWidth = 362;
            this.gridLabel2.Column = 0;
            this.gridLabel2.ColumnsSpanned = 3;
            this.gridLabel2.Location = new System.Drawing.Point(8, 327);
            this.gridLabel2.Name = "gridLabel2";
            this.gridLabel2.Row = 5;
            this.gridLabel2.RowsSpanned = 0;
            this.gridLabel2.Size = new System.Drawing.Size(356, 31);
            this.gridLabel2.TabIndex = 5;
            this.gridLabel2.Text = "Would you like to add an exception and proceed to the specified Swarm server?";
            this.gridLabel2.YOffset = 0;
            // 
            // NoBtn
            // 
            this.NoBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.NoBtn.CellHeight = 29;
            this.NoBtn.CellWidth = 102;
            this.NoBtn.Column = 3;
            this.NoBtn.ColumnsSpanned = 0;
            this.NoBtn.DialogResult = System.Windows.Forms.DialogResult.No;
            this.NoBtn.Location = new System.Drawing.Point(268, 361);
            this.NoBtn.Name = "NoBtn";
            this.NoBtn.Row = 6;
            this.NoBtn.RowsSpanned = 0;
            this.NoBtn.Size = new System.Drawing.Size(96, 23);
            this.NoBtn.TabIndex = 0;
            this.NoBtn.Text = "Don\'t Trust";
            this.NoBtn.UseVisualStyleBackColor = true;
            this.NoBtn.YOffset = 0;
            // 
            // gridPanel1
            // 
            this.gridPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPanel1.CellHeight = 29;
            this.gridPanel1.CellWidth = 47;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Location = new System.Drawing.Point(8, 361);
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 6;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.Size = new System.Drawing.Size(41, 23);
            this.gridPanel1.TabIndex = 3;
            this.gridPanel1.YOffset = 0;
            // 
            // gridLabel1
            // 
            this.gridLabel1.AutoSize = true;
            this.gridLabel1.CellHeight = 18;
            this.gridLabel1.CellWidth = 47;
            this.gridLabel1.Column = 0;
            this.gridLabel1.ColumnsSpanned = 0;
            this.gridLabel1.Location = new System.Drawing.Point(8, 121);
            this.gridLabel1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.gridLabel1.Name = "gridLabel1";
            this.gridLabel1.Row = 3;
            this.gridLabel1.RowsSpanned = 0;
            this.gridLabel1.Size = new System.Drawing.Size(83, 13);
            this.gridLabel1.TabIndex = 2;
            this.gridLabel1.Text = "Certificate Data:";
            this.gridLabel1.YOffset = 0;
            // 
            // CertificateTextTB
            // 
            this.CertificateTextTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CertificateTextTB.CellHeight = 188;
            this.CertificateTextTB.CellWidth = 362;
            this.CertificateTextTB.Column = 0;
            this.CertificateTextTB.ColumnsSpanned = 3;
            this.CertificateTextTB.Location = new System.Drawing.Point(10, 142);
            this.CertificateTextTB.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.CertificateTextTB.Multiline = true;
            this.CertificateTextTB.Name = "CertificateTextTB";
            this.CertificateTextTB.ReadOnly = true;
            this.CertificateTextTB.Row = 4;
            this.CertificateTextTB.RowsSpanned = 0;
            this.CertificateTextTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.CertificateTextTB.Size = new System.Drawing.Size(352, 182);
            this.CertificateTextTB.TabIndex = 2;
            this.CertificateTextTB.YOffset = 0;
            // 
            // MessageLbl
            // 
            this.MessageLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageLbl.CellHeight = 41;
            this.MessageLbl.CellWidth = 362;
            this.MessageLbl.Column = 0;
            this.MessageLbl.ColumnsSpanned = 3;
            this.MessageLbl.Location = new System.Drawing.Point(10, 10);
            this.MessageLbl.Margin = new System.Windows.Forms.Padding(5);
            this.MessageLbl.Name = "MessageLbl";
            this.MessageLbl.Row = 0;
            this.MessageLbl.RowsSpanned = 0;
            this.MessageLbl.Size = new System.Drawing.Size(352, 31);
            this.MessageLbl.TabIndex = 0;
            this.MessageLbl.Text = "There was a problem with the certificate provided by the Swarm server";
            this.MessageLbl.YOffset = 0;
            // 
            // SSLCertificateErrorDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.NoBtn;
            this.ClientSize = new System.Drawing.Size(372, 392);
            this.Controls.Add(this.gridLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SSLCertificateErrorDlg";
            this.Text = "SSL Certificate Error";
            this.Load += new System.EventHandler(this.SSLCertificateErrorDlg_Load);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridButton NoBtn;
        private I18nControls.GridButton YesBtn;
        private I18nControls.GridPanel gridPanel1;
        private I18nControls.GridLabel gridLabel1;
        private I18nControls.GridTextBox CertificateTextTB;
        private I18nControls.GridLabel MessageLbl;
        private I18nControls.GridLabel gridLabel2;
        private I18nControls.GridListBox ErrorsLB;
        private I18nControls.GridLabel gridLabel3;
        private I18nControls.GridButton CopyToClipboardBtn;
    }
}