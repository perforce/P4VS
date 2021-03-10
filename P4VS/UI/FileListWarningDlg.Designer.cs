namespace Perforce.P4VS
{
	partial class FileListWarningDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileListWarningDlg));
			this.OkBtn = new Perforce.I18nControls.GridButton();
			this.CancelBtn = new Perforce.I18nControls.GridButton();
			this.PromptLbl = new Perforce.I18nControls.GridLabel();
			this.FilesList = new Perforce.I18nControls.GridDoubleBufferedListView();
			this.FileNameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.FilePAthClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ReasonClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.DontGetLatestBtn = new Perforce.I18nControls.GridButton();
			this.GetLatestBtn = new Perforce.I18nControls.GridButton();
			this.BtnBar = new Perforce.I18nControls.GridLayoutSubpanel();
			this.gridPanel2 = new Perforce.I18nControls.GridPanel();
			this.gridPanel1 = new Perforce.I18nControls.GridPanel();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.BtnBar.SuspendLayout();
			this.gridPanel1.SuspendLayout();
			this.gridLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// OkBtn
			// 
			resources.ApplyResources(this.OkBtn, "OkBtn");
			this.OkBtn.CellHeight = 0;
			this.OkBtn.CellWidth = 0;
			this.OkBtn.Column = 1;
			this.OkBtn.ColumnsSpanned = 0;
			this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkBtn.Name = "OkBtn";
			this.OkBtn.Row = 0;
			this.OkBtn.RowsSpanned = 0;
			this.OkBtn.UseVisualStyleBackColor = true;
			this.OkBtn.YOffset = 0;
			// 
			// CancelBtn
			// 
			resources.ApplyResources(this.CancelBtn, "CancelBtn");
			this.CancelBtn.CellHeight = 29;
			this.CancelBtn.CellWidth = 101;
			this.CancelBtn.Column = 3;
			this.CancelBtn.ColumnsSpanned = 0;
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Row = 0;
			this.CancelBtn.RowsSpanned = 0;
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.YOffset = 0;
			// 
			// PromptLbl
			// 
			resources.ApplyResources(this.PromptLbl, "PromptLbl");
			this.PromptLbl.AutoEllipsis = true;
			this.PromptLbl.CellHeight = 26;
			this.PromptLbl.CellWidth = 445;
			this.PromptLbl.Column = 0;
			this.PromptLbl.ColumnsSpanned = 0;
			this.PromptLbl.Name = "PromptLbl";
			this.PromptLbl.Row = 0;
			this.PromptLbl.RowsSpanned = 0;
			this.PromptLbl.YOffset = 0;
			// 
			// FilesList
			// 
			this.FilesList.AllowColumnReorder = true;
			resources.ApplyResources(this.FilesList, "FilesList");
			this.FilesList.CellHeight = 173;
			this.FilesList.CellWidth = 445;
			this.FilesList.Column = 0;
			this.FilesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FileNameClm,
            this.FilePAthClm,
            this.ReasonClm});
			this.FilesList.ColumnsSpanned = 0;
			this.FilesList.FullRowSelect = true;
			this.FilesList.HideActionsColumn = false;
			this.FilesList.Name = "FilesList";
			this.FilesList.OwnerDraw = true;
			this.FilesList.Row = 1;
			this.FilesList.RowsSpanned = 0;
			this.FilesList.UseCompatibleStateImageBehavior = false;
			this.FilesList.View = System.Windows.Forms.View.Details;
			this.FilesList.YOffset = 0;
			// 
			// FileNameClm
			// 
			resources.ApplyResources(this.FileNameClm, "FileNameClm");
			// 
			// FilePAthClm
			// 
			resources.ApplyResources(this.FilePAthClm, "FilePAthClm");
			// 
			// ReasonClm
			// 
			resources.ApplyResources(this.ReasonClm, "ReasonClm");
			// 
			// DontGetLatestBtn
			// 
			resources.ApplyResources(this.DontGetLatestBtn, "DontGetLatestBtn");
			this.DontGetLatestBtn.CellHeight = 0;
			this.DontGetLatestBtn.CellWidth = 0;
			this.DontGetLatestBtn.Column = 1;
			this.DontGetLatestBtn.ColumnsSpanned = 0;
			this.DontGetLatestBtn.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			this.DontGetLatestBtn.Name = "DontGetLatestBtn";
			this.DontGetLatestBtn.Row = 0;
			this.DontGetLatestBtn.RowsSpanned = 0;
			this.DontGetLatestBtn.UseVisualStyleBackColor = true;
			this.DontGetLatestBtn.YOffset = 0;
			this.DontGetLatestBtn.Click += new System.EventHandler(this.DontGetLatestBtn_Click);
			// 
			// GetLatestBtn
			// 
			resources.ApplyResources(this.GetLatestBtn, "GetLatestBtn");
			this.GetLatestBtn.CellHeight = 29;
			this.GetLatestBtn.CellWidth = 101;
			this.GetLatestBtn.Column = 1;
			this.GetLatestBtn.ColumnsSpanned = 0;
			this.GetLatestBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.GetLatestBtn.Name = "GetLatestBtn";
			this.GetLatestBtn.Row = 0;
			this.GetLatestBtn.RowsSpanned = 0;
			this.GetLatestBtn.UseVisualStyleBackColor = true;
			this.GetLatestBtn.YOffset = 0;
			this.GetLatestBtn.Click += new System.EventHandler(this.DontGetLatestBtn_Click);
			// 
			// BtnBar
			// 
			resources.ApplyResources(this.BtnBar, "BtnBar");
			this.BtnBar.CellHeight = 29;
			this.BtnBar.CellWidth = 445;
			this.BtnBar.Column = 0;
			this.BtnBar.ColumnsSpanned = 0;
			this.BtnBar.Controls.Add(this.gridPanel2);
			this.BtnBar.Controls.Add(this.gridPanel1);
			this.BtnBar.Controls.Add(this.CancelBtn);
			this.BtnBar.Controls.Add(this.GetLatestBtn);
			this.BtnBar.EnableDesignerGrid = false;
			this.BtnBar.EnableDesignerLayout = true;
			this.BtnBar.EnableParentResize = false;
			this.BtnBar.MinimumColumnWidth = 10;
			this.BtnBar.MinimumRowHeight = 10;
			this.BtnBar.Name = "BtnBar";
			this.BtnBar.Row = 2;
			this.BtnBar.RowsSpanned = 0;
			this.BtnBar.YOffset = 0;
			// 
			// gridPanel2
			// 
			resources.ApplyResources(this.gridPanel2, "gridPanel2");
			this.gridPanel2.CellHeight = 29;
			this.gridPanel2.CellWidth = 136;
			this.gridPanel2.Column = 0;
			this.gridPanel2.ColumnsSpanned = 0;
			this.gridPanel2.Name = "gridPanel2";
			this.gridPanel2.Row = 0;
			this.gridPanel2.RowsSpanned = 0;
			this.gridPanel2.YOffset = 3;
			// 
			// gridPanel1
			// 
			resources.ApplyResources(this.gridPanel1, "gridPanel1");
			this.gridPanel1.CellHeight = 29;
			this.gridPanel1.CellWidth = 101;
			this.gridPanel1.Column = 2;
			this.gridPanel1.ColumnsSpanned = 0;
			this.gridPanel1.Controls.Add(this.DontGetLatestBtn);
			this.gridPanel1.Controls.Add(this.OkBtn);
			this.gridPanel1.Name = "gridPanel1";
			this.gridPanel1.Row = 0;
			this.gridPanel1.RowsSpanned = 0;
			this.gridPanel1.YOffset = 0;
			// 
			// gridLayoutPanel1
			// 
			this.gridLayoutPanel1.Controls.Add(this.FilesList);
			this.gridLayoutPanel1.Controls.Add(this.PromptLbl);
			this.gridLayoutPanel1.Controls.Add(this.BtnBar);
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.EnableDesignerGrid = false;
			this.gridLayoutPanel1.EnableDesignerLayout = true;
			this.gridLayoutPanel1.EnableParentResize = false;
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			// 
			// FileListWarningDlg
			// 
			this.AcceptButton = this.GetLatestBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.Controls.Add(this.gridLayoutPanel1);
			this.DoubleBuffered = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FileListWarningDlg";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.BtnBar.ResumeLayout(false);
			this.gridPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private I18nControls.GridLabel PromptLbl;
		private I18nControls.GridDoubleBufferedListView FilesList;
		private Perforce.I18nControls.GridButton CancelBtn;
		private Perforce.I18nControls.GridButton OkBtn;
		private System.Windows.Forms.ColumnHeader FileNameClm;
		private System.Windows.Forms.ColumnHeader FilePAthClm;
		private System.Windows.Forms.ColumnHeader ReasonClm;
		private Perforce.I18nControls.GridButton DontGetLatestBtn;
		private Perforce.I18nControls.GridButton GetLatestBtn;
		private I18nControls.GridLayoutSubpanel BtnBar;
		private I18nControls.GridPanel gridPanel1;
		private I18nControls.GridPanel gridPanel2;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
	}
}