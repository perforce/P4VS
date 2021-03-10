namespace Perforce.P4VS
{
	partial class SslPromptDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SslPromptDlg));
			this.WarningTB = new Perforce.I18nControls.GridTextBox();
			this.WarningLbl = new Perforce.I18nControls.GridLabel();
			this.pictureBox1 = new Perforce.I18nControls.GridPictureBox();
			this.TrustCB = new Perforce.I18nControls.GridCheckBox();
			this.ConnectBtn = new Perforce.I18nControls.GridButton();
			this.CancelBtn = new Perforce.I18nControls.GridButton();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.gridPanel1 = new Perforce.I18nControls.GridPanel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.gridLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// WarningTB
			// 
			resources.ApplyResources(this.WarningTB, "WarningTB");
			this.WarningTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.WarningTB.CellHeight = 90;
			this.WarningTB.CellWidth = 340;
			this.WarningTB.Column = 1;
			this.WarningTB.ColumnsSpanned = 2;
			this.WarningTB.Name = "WarningTB";
			this.WarningTB.ReadOnly = true;
			this.WarningTB.Row = 1;
			this.WarningTB.RowsSpanned = 0;
			this.WarningTB.TabStop = false;
			this.WarningTB.YOffset = 0;
			// 
			// WarningLbl
			// 
			resources.ApplyResources(this.WarningLbl, "WarningLbl");
			this.WarningLbl.CellHeight = 13;
			this.WarningLbl.CellWidth = 410;
			this.WarningLbl.Column = 0;
			this.WarningLbl.ColumnsSpanned = 3;
			this.WarningLbl.ForeColor = System.Drawing.Color.Red;
			this.WarningLbl.Name = "WarningLbl";
			this.WarningLbl.Row = 0;
			this.WarningLbl.RowsSpanned = 0;
			this.WarningLbl.YOffset = 0;
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.CellHeight = 90;
			this.pictureBox1.CellWidth = 70;
			this.pictureBox1.Column = 0;
			this.pictureBox1.ColumnsSpanned = 0;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Row = 1;
			this.pictureBox1.RowsSpanned = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.YOffset = 9;
			// 
			// TrustCB
			// 
			resources.ApplyResources(this.TrustCB, "TrustCB");
			this.TrustCB.CellHeight = 23;
			this.TrustCB.CellWidth = 178;
			this.TrustCB.Column = 1;
			this.TrustCB.ColumnsSpanned = 0;
			this.TrustCB.Name = "TrustCB";
			this.TrustCB.Row = 2;
			this.TrustCB.RowsSpanned = 0;
			this.TrustCB.UseVisualStyleBackColor = true;
			this.TrustCB.YOffset = 0;
			this.TrustCB.CheckedChanged += new System.EventHandler(this.TrustCB_CheckedChanged);
			// 
			// ConnectBtn
			// 
			resources.ApplyResources(this.ConnectBtn, "ConnectBtn");
			this.ConnectBtn.CellHeight = 29;
			this.ConnectBtn.CellWidth = 81;
			this.ConnectBtn.Column = 3;
			this.ConnectBtn.ColumnsSpanned = 0;
			this.ConnectBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ConnectBtn.Name = "ConnectBtn";
			this.ConnectBtn.Row = 3;
			this.ConnectBtn.RowsSpanned = 0;
			this.ConnectBtn.UseVisualStyleBackColor = true;
			this.ConnectBtn.YOffset = 0;
			// 
			// CancelBtn
			// 
			resources.ApplyResources(this.CancelBtn, "CancelBtn");
			this.CancelBtn.CellHeight = 29;
			this.CancelBtn.CellWidth = 81;
			this.CancelBtn.Column = 2;
			this.CancelBtn.ColumnsSpanned = 0;
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Row = 3;
			this.CancelBtn.RowsSpanned = 0;
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.YOffset = 0;
			// 
			// gridLayoutPanel1
			// 
			this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
			this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
			this.gridLayoutPanel1.Controls.Add(this.ConnectBtn);
			this.gridLayoutPanel1.Controls.Add(this.TrustCB);
			this.gridLayoutPanel1.Controls.Add(this.pictureBox1);
			this.gridLayoutPanel1.Controls.Add(this.WarningLbl);
			this.gridLayoutPanel1.Controls.Add(this.WarningTB);
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.EnableDesignerGrid = false;
			this.gridLayoutPanel1.EnableDesignerLayout = true;
			this.gridLayoutPanel1.EnableParentResize = false;
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			// 
			// gridPanel1
			// 
			resources.ApplyResources(this.gridPanel1, "gridPanel1");
			this.gridPanel1.CellHeight = 29;
			this.gridPanel1.CellWidth = 178;
			this.gridPanel1.Column = 1;
			this.gridPanel1.ColumnsSpanned = 0;
			this.gridPanel1.Name = "gridPanel1";
			this.gridPanel1.Row = 3;
			this.gridPanel1.RowsSpanned = 0;
			this.gridPanel1.YOffset = 0;
			// 
			// SslPromptDlg
			// 
			this.AcceptButton = this.ConnectBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.Controls.Add(this.gridLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SslPromptDlg";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private I18nControls.GridTextBox WarningTB;
		private I18nControls.GridLabel WarningLbl;
		private I18nControls.GridPictureBox pictureBox1;
		private I18nControls.GridCheckBox TrustCB;
		private I18nControls.GridButton ConnectBtn;
		private I18nControls.GridButton CancelBtn;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridPanel gridPanel1;
	}
}