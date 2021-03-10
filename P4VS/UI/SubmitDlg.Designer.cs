namespace Perforce.P4VS
{
	partial class SubmitDlg
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubmitDlg));
            this.JobListMnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.JLM_DeleteJobMI = new System.Windows.Forms.ToolStripMenuItem();
            this.FileListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.FLM_DiffVsHaveMI = new System.Windows.Forms.ToolStripMenuItem();
            this.FLM_ResolveMI = new System.Windows.Forms.ToolStripMenuItem();
            this.SubmitBtn = new Perforce.I18nControls.GridButton();
            this.CancelBtn = new Perforce.I18nControls.GridButton();
            this.SaveBtn = new Perforce.I18nControls.GridButton();
            this.JobListPanel = new Perforce.P4VS.SlidingPanel();
            this.gridLayoutPanel2 = new Perforce.I18nControls.GridLayoutPanel();
            this.BrowseJobsBtn = new Perforce.I18nControls.GridButton();
            this.AddJobBtn = new Perforce.I18nControls.GridButton();
            this.JobTB = new Perforce.I18nControls.GridTextBox();
            this.JobStatusLbl = new Perforce.I18nControls.GridLabel();
            this.JobStatusCB = new Perforce.I18nControls.GridComboBox();
            this.JobsListLV = new Perforce.I18nControls.GridDoubleBufferedListView();
            this.JobClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StatusClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.JobDescriptionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.JobLbl = new Perforce.I18nControls.GridLabel();
            this.resolveMsgImg = new Perforce.I18nControls.GridPictureBox();
            this.resolveWarningLbl = new Perforce.I18nControls.GridLabel();
            this.FileListPanel = new Perforce.P4VS.SlidingPanel();
            this.gridLayoutPanel3 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel2 = new Perforce.I18nControls.GridPanel();
            this.SelectAllFileListChk = new System.Windows.Forms.CheckBox();
            this.CheckOutAfterCB = new Perforce.I18nControls.GridCheckBox();
            this.FileListLV = new Perforce.I18nControls.GridDoubleBufferedListView();
            this.CheckedClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FolderClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ResolveStatusClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TypeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PendingActionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SubmitFilesCB = new Perforce.I18nControls.GridComboBox();
            this.OnSubmitLbl = new Perforce.I18nControls.GridLabel();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.gridPanel4 = new Perforce.I18nControls.GridPanel();
            this.DescriptionPanel = new Perforce.P4VS.SlidingPanel();
            this.DescriptionTB = new System.Windows.Forms.TextBox();
            this.slidingPanelContainer1 = new Perforce.I18nControls.GridSlidingPanelContainer();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.JobListMnu.SuspendLayout();
            this.FileListMenu.SuspendLayout();
            this.JobListPanel.SuspendLayout();
            this.gridLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resolveMsgImg)).BeginInit();
            this.FileListPanel.SuspendLayout();
            this.gridLayoutPanel3.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.DescriptionPanel.SuspendLayout();
            this.slidingPanelContainer1.SuspendLayout();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // JobListMnu
            // 
            this.JobListMnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.JLM_DeleteJobMI});
            this.JobListMnu.Name = "JobListMnu";
            resources.ApplyResources(this.JobListMnu, "JobListMnu");
            this.JobListMnu.Opening += new System.ComponentModel.CancelEventHandler(this.JobListMnu_Opening);
            this.JobListMnu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.JobListMnu_ItemClicked);
            // 
            // JLM_DeleteJobMI
            // 
            this.JLM_DeleteJobMI.Name = "JLM_DeleteJobMI";
            resources.ApplyResources(this.JLM_DeleteJobMI, "JLM_DeleteJobMI");
            this.JLM_DeleteJobMI.Tag = "DeleteJob";
            // 
            // FileListMenu
            // 
            this.FileListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FLM_DiffVsHaveMI,
            this.FLM_ResolveMI});
            this.FileListMenu.Name = "FileListMenu";
            resources.ApplyResources(this.FileListMenu, "FileListMenu");
            this.FileListMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FileListMenu_Opening);
            this.FileListMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.FileListMenu_ItemClicked);
            // 
            // FLM_DiffVsHaveMI
            // 
            this.FLM_DiffVsHaveMI.Name = "FLM_DiffVsHaveMI";
            resources.ApplyResources(this.FLM_DiffVsHaveMI, "FLM_DiffVsHaveMI");
            this.FLM_DiffVsHaveMI.Tag = "DiffVsHave";
            // 
            // FLM_ResolveMI
            // 
            this.FLM_ResolveMI.Name = "FLM_ResolveMI";
            resources.ApplyResources(this.FLM_ResolveMI, "FLM_ResolveMI");
            this.FLM_ResolveMI.Tag = "Resolve";
            // 
            // SubmitBtn
            // 
            resources.ApplyResources(this.SubmitBtn, "SubmitBtn");
            this.SubmitBtn.CellHeight = 29;
            this.SubmitBtn.CellWidth = 81;
            this.SubmitBtn.Column = 1;
            this.SubmitBtn.ColumnsSpanned = 0;
            this.SubmitBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Row = 1;
            this.SubmitBtn.RowsSpanned = 0;
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.YOffset = 0;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.CellHeight = 29;
            this.CancelBtn.CellWidth = 81;
            this.CancelBtn.Column = 3;
            this.CancelBtn.ColumnsSpanned = 0;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Row = 1;
            this.CancelBtn.RowsSpanned = 0;
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.YOffset = 0;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // SaveBtn
            // 
            resources.ApplyResources(this.SaveBtn, "SaveBtn");
            this.SaveBtn.CellHeight = 29;
            this.SaveBtn.CellWidth = 81;
            this.SaveBtn.Column = 2;
            this.SaveBtn.ColumnsSpanned = 0;
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Row = 1;
            this.SaveBtn.RowsSpanned = 0;
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.YOffset = 0;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // JobListPanel
            // 
            resources.ApplyResources(this.JobListPanel, "JobListPanel");
            this.JobListPanel.ButtonText = "Link jobs to changelist (optional)";
            this.JobListPanel.ButtonVisible = true;
            this.JobListPanel.ButtonWidth = 189;
            this.JobListPanel.Collapsed = false;
            this.JobListPanel.CollapsedHeight = 30;
            this.JobListPanel.Controls.Add(this.gridLayoutPanel2);
            this.JobListPanel.Controls.Add(this.JobLbl);
            this.JobListPanel.Hidden = false;
            this.JobListPanel.Name = "JobListPanel";
            this.JobListPanel.PanelHeight = 0;
            this.JobListPanel.PreferencesKey = null;
            this.JobListPanel.ShowAlert = false;
            this.JobListPanel.TabStop = true;
            this.JobListPanel.Weight = 10;
            // 
            // gridLayoutPanel2
            // 
            resources.ApplyResources(this.gridLayoutPanel2, "gridLayoutPanel2");
            this.gridLayoutPanel2.Controls.Add(this.BrowseJobsBtn);
            this.gridLayoutPanel2.Controls.Add(this.AddJobBtn);
            this.gridLayoutPanel2.Controls.Add(this.JobTB);
            this.gridLayoutPanel2.Controls.Add(this.JobStatusLbl);
            this.gridLayoutPanel2.Controls.Add(this.JobStatusCB);
            this.gridLayoutPanel2.Controls.Add(this.JobsListLV);
            this.gridLayoutPanel2.EnableDesignerGrid = false;
            this.gridLayoutPanel2.EnableDesignerLayout = true;
            this.gridLayoutPanel2.EnableParentResize = false;
            this.gridLayoutPanel2.MinimumColumnWidth = 10;
            this.gridLayoutPanel2.MinimumRowHeight = 10;
            this.gridLayoutPanel2.Name = "gridLayoutPanel2";
            // 
            // BrowseJobsBtn
            // 
            resources.ApplyResources(this.BrowseJobsBtn, "BrowseJobsBtn");
            this.BrowseJobsBtn.CellHeight = 29;
            this.BrowseJobsBtn.CellWidth = 70;
            this.BrowseJobsBtn.Column = 2;
            this.BrowseJobsBtn.ColumnsSpanned = 0;
            this.BrowseJobsBtn.Name = "BrowseJobsBtn";
            this.BrowseJobsBtn.Row = 1;
            this.BrowseJobsBtn.RowsSpanned = 0;
            this.BrowseJobsBtn.UseVisualStyleBackColor = true;
            this.BrowseJobsBtn.YOffset = 0;
            this.BrowseJobsBtn.Click += new System.EventHandler(this.BrowseJobsBtn_Click);
            // 
            // AddJobBtn
            // 
            resources.ApplyResources(this.AddJobBtn, "AddJobBtn");
            this.AddJobBtn.CellHeight = 29;
            this.AddJobBtn.CellWidth = 70;
            this.AddJobBtn.Column = 1;
            this.AddJobBtn.ColumnsSpanned = 0;
            this.AddJobBtn.Name = "AddJobBtn";
            this.AddJobBtn.Row = 1;
            this.AddJobBtn.RowsSpanned = 0;
            this.AddJobBtn.UseVisualStyleBackColor = true;
            this.AddJobBtn.YOffset = 0;
            this.AddJobBtn.Click += new System.EventHandler(this.AddJobBtn_Click);
            // 
            // JobTB
            // 
            resources.ApplyResources(this.JobTB, "JobTB");
            this.JobTB.CellHeight = 29;
            this.JobTB.CellWidth = 334;
            this.JobTB.Column = 0;
            this.JobTB.ColumnsSpanned = 0;
            this.JobTB.Name = "JobTB";
            this.JobTB.Row = 1;
            this.JobTB.RowsSpanned = 0;
            this.JobTB.YOffset = 1;
            this.JobTB.TextChanged += new System.EventHandler(this.JobTB_TextChanged);
            // 
            // JobStatusLbl
            // 
            resources.ApplyResources(this.JobStatusLbl, "JobStatusLbl");
            this.JobStatusLbl.CellHeight = 29;
            this.JobStatusLbl.CellWidth = 124;
            this.JobStatusLbl.Column = 3;
            this.JobStatusLbl.ColumnsSpanned = 0;
            this.JobStatusLbl.Name = "JobStatusLbl";
            this.JobStatusLbl.Row = 1;
            this.JobStatusLbl.RowsSpanned = 0;
            this.JobStatusLbl.YOffset = 5;
            // 
            // JobStatusCB
            // 
            resources.ApplyResources(this.JobStatusCB, "JobStatusCB");
            this.JobStatusCB.CellHeight = 29;
            this.JobStatusCB.CellWidth = 81;
            this.JobStatusCB.Column = 4;
            this.JobStatusCB.ColumnsSpanned = 0;
            this.JobStatusCB.DesignSize = new System.Drawing.Size(0, 0);
            this.JobStatusCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.JobStatusCB.FormattingEnabled = true;
            this.JobStatusCB.Items.AddRange(new object[] {
            resources.GetString("JobStatusCB.Items"),
            resources.GetString("JobStatusCB.Items1"),
            resources.GetString("JobStatusCB.Items2"),
            resources.GetString("JobStatusCB.Items3")});
            this.JobStatusCB.Name = "JobStatusCB";
            this.JobStatusCB.Row = 1;
            this.JobStatusCB.RowsSpanned = 0;
            this.JobStatusCB.YOffset = 1;
            // 
            // JobsListLV
            // 
            this.JobsListLV.AllowColumnReorder = true;
            resources.ApplyResources(this.JobsListLV, "JobsListLV");
            this.JobsListLV.CellHeight = 100;
            this.JobsListLV.CellWidth = 679;
            this.JobsListLV.CheckBoxes = true;
            this.JobsListLV.Column = 0;
            this.JobsListLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.JobClm,
            this.StatusClm,
            this.JobDescriptionClm});
            this.JobsListLV.ColumnsSpanned = 4;
            this.JobsListLV.ContextMenuStrip = this.JobListMnu;
            this.JobsListLV.FullRowSelect = true;
            this.JobsListLV.GridLines = true;
            this.JobsListLV.HideActionsColumn = false;
            this.JobsListLV.Name = "JobsListLV";
            this.JobsListLV.OwnerDraw = true;
            this.JobsListLV.Row = 0;
            this.JobsListLV.RowsSpanned = 0;
            this.JobsListLV.UseCompatibleStateImageBehavior = false;
            this.JobsListLV.View = System.Windows.Forms.View.Details;
            this.JobsListLV.YOffset = 0;
            this.JobsListLV.Enter += new System.EventHandler(this.JobsListLV_Enter);
            // 
            // JobClm
            // 
            resources.ApplyResources(this.JobClm, "JobClm");
            // 
            // StatusClm
            // 
            resources.ApplyResources(this.StatusClm, "StatusClm");
            // 
            // JobDescriptionClm
            // 
            resources.ApplyResources(this.JobDescriptionClm, "JobDescriptionClm");
            // 
            // JobLbl
            // 
            resources.ApplyResources(this.JobLbl, "JobLbl");
            this.JobLbl.CellHeight = 0;
            this.JobLbl.CellWidth = 0;
            this.JobLbl.Column = 0;
            this.JobLbl.ColumnsSpanned = 0;
            this.JobLbl.Name = "JobLbl";
            this.JobLbl.Row = 0;
            this.JobLbl.RowsSpanned = 0;
            this.JobLbl.YOffset = 0;
            // 
            // resolveMsgImg
            // 
            resources.ApplyResources(this.resolveMsgImg, "resolveMsgImg");
            this.resolveMsgImg.CellHeight = 24;
            this.resolveMsgImg.CellWidth = 22;
            this.resolveMsgImg.Column = 0;
            this.resolveMsgImg.ColumnsSpanned = 0;
            this.resolveMsgImg.Name = "resolveMsgImg";
            this.resolveMsgImg.Row = 0;
            this.resolveMsgImg.RowsSpanned = 0;
            this.resolveMsgImg.TabStop = false;
            this.resolveMsgImg.YOffset = 0;
            // 
            // resolveWarningLbl
            // 
            resources.ApplyResources(this.resolveWarningLbl, "resolveWarningLbl");
            this.resolveWarningLbl.CellHeight = 24;
            this.resolveWarningLbl.CellWidth = 165;
            this.resolveWarningLbl.Column = 1;
            this.resolveWarningLbl.ColumnsSpanned = 0;
            this.resolveWarningLbl.ForeColor = System.Drawing.Color.Red;
            this.resolveWarningLbl.Name = "resolveWarningLbl";
            this.resolveWarningLbl.Row = 0;
            this.resolveWarningLbl.RowsSpanned = 0;
            this.resolveWarningLbl.YOffset = 2;
            // 
            // FileListPanel
            // 
            resources.ApplyResources(this.FileListPanel, "FileListPanel");
            this.FileListPanel.ButtonText = "Choose files to submit";
            this.FileListPanel.ButtonVisible = true;
            this.FileListPanel.ButtonWidth = 140;
            this.FileListPanel.Collapsed = false;
            this.FileListPanel.CollapsedHeight = 30;
            this.FileListPanel.Controls.Add(this.gridLayoutPanel3);
            this.FileListPanel.Hidden = false;
            this.FileListPanel.Name = "FileListPanel";
            this.FileListPanel.PanelHeight = 0;
            this.FileListPanel.PreferencesKey = null;
            this.FileListPanel.ShowAlert = false;
            this.FileListPanel.Weight = 10;
            // 
            // gridLayoutPanel3
            // 
            resources.ApplyResources(this.gridLayoutPanel3, "gridLayoutPanel3");
            this.gridLayoutPanel3.Controls.Add(this.gridPanel2);
            this.gridLayoutPanel3.Controls.Add(this.SelectAllFileListChk);
            this.gridLayoutPanel3.Controls.Add(this.CheckOutAfterCB);
            this.gridLayoutPanel3.Controls.Add(this.FileListLV);
            this.gridLayoutPanel3.Controls.Add(this.SubmitFilesCB);
            this.gridLayoutPanel3.Controls.Add(this.OnSubmitLbl);
            this.gridLayoutPanel3.Controls.Add(this.gridLayoutSubpanel1);
            this.gridLayoutPanel3.EnableDesignerGrid = false;
            this.gridLayoutPanel3.EnableDesignerLayout = true;
            this.gridLayoutPanel3.EnableParentResize = false;
            this.gridLayoutPanel3.MinimumColumnWidth = 10;
            this.gridLayoutPanel3.MinimumRowHeight = 10;
            this.gridLayoutPanel3.Name = "gridLayoutPanel3";
            this.gridLayoutPanel3.AfterLayoutGrid += new Perforce.I18nControls.GridLayoutPanel.GridLayoutEvent(this.gridLayoutPanel3_AfterLayoutGrid);
            // 
            // gridPanel2
            // 
            resources.ApplyResources(this.gridPanel2, "gridPanel2");
            this.gridPanel2.CellHeight = 27;
            this.gridPanel2.CellWidth = 148;
            this.gridPanel2.Column = 2;
            this.gridPanel2.ColumnsSpanned = 0;
            this.gridPanel2.Name = "gridPanel2";
            this.gridPanel2.Row = 2;
            this.gridPanel2.RowsSpanned = 0;
            this.gridPanel2.YOffset = 0;
            // 
            // SelectAllFileListChk
            // 
            resources.ApplyResources(this.SelectAllFileListChk, "SelectAllFileListChk");
            this.SelectAllFileListChk.Name = "SelectAllFileListChk";
            this.SelectAllFileListChk.UseVisualStyleBackColor = true;
            // 
            // CheckOutAfterCB
            // 
            resources.ApplyResources(this.CheckOutAfterCB, "CheckOutAfterCB");
            this.CheckOutAfterCB.CellHeight = 27;
            this.CheckOutAfterCB.CellWidth = 207;
            this.CheckOutAfterCB.Column = 3;
            this.CheckOutAfterCB.ColumnsSpanned = 0;
            this.CheckOutAfterCB.Name = "CheckOutAfterCB";
            this.CheckOutAfterCB.Row = 2;
            this.CheckOutAfterCB.RowsSpanned = 0;
            this.CheckOutAfterCB.UseVisualStyleBackColor = true;
            this.CheckOutAfterCB.YOffset = 2;
            // 
            // FileListLV
            // 
            this.FileListLV.AllowColumnReorder = true;
            resources.ApplyResources(this.FileListLV, "FileListLV");
            this.FileListLV.CellHeight = 60;
            this.FileListLV.CellWidth = 680;
            this.FileListLV.CheckBoxes = true;
            this.FileListLV.Column = 0;
            this.FileListLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CheckedClm,
            this.NameClm,
            this.FolderClm,
            this.ResolveStatusClm,
            this.TypeClm,
            this.PendingActionClm});
            this.FileListLV.ColumnsSpanned = 3;
            this.FileListLV.ContextMenuStrip = this.FileListMenu;
            this.FileListLV.FullRowSelect = true;
            this.FileListLV.GridLines = true;
            this.FileListLV.HideActionsColumn = false;
            this.FileListLV.Name = "FileListLV";
            this.FileListLV.OwnerDraw = true;
            this.FileListLV.Row = 1;
            this.FileListLV.RowsSpanned = 0;
            this.FileListLV.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.FileListLV.UseCompatibleStateImageBehavior = false;
            this.FileListLV.View = System.Windows.Forms.View.Details;
            this.FileListLV.YOffset = 0;
            this.FileListLV.Enter += new System.EventHandler(this.FileListLV_Enter);
            // 
            // CheckedClm
            // 
            resources.ApplyResources(this.CheckedClm, "CheckedClm");
            // 
            // NameClm
            // 
            resources.ApplyResources(this.NameClm, "NameClm");
            // 
            // FolderClm
            // 
            resources.ApplyResources(this.FolderClm, "FolderClm");
            // 
            // ResolveStatusClm
            // 
            resources.ApplyResources(this.ResolveStatusClm, "ResolveStatusClm");
            // 
            // TypeClm
            // 
            resources.ApplyResources(this.TypeClm, "TypeClm");
            // 
            // PendingActionClm
            // 
            resources.ApplyResources(this.PendingActionClm, "PendingActionClm");
            // 
            // SubmitFilesCB
            // 
            resources.ApplyResources(this.SubmitFilesCB, "SubmitFilesCB");
            this.SubmitFilesCB.CellHeight = 27;
            this.SubmitFilesCB.CellWidth = 260;
            this.SubmitFilesCB.Column = 1;
            this.SubmitFilesCB.ColumnsSpanned = 0;
            this.SubmitFilesCB.DesignSize = new System.Drawing.Size(0, 0);
            this.SubmitFilesCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SubmitFilesCB.FormattingEnabled = true;
            this.SubmitFilesCB.Items.AddRange(new object[] {
            resources.GetString("SubmitFilesCB.Items"),
            resources.GetString("SubmitFilesCB.Items1"),
            resources.GetString("SubmitFilesCB.Items2")});
            this.SubmitFilesCB.Name = "SubmitFilesCB";
            this.SubmitFilesCB.Row = 2;
            this.SubmitFilesCB.RowsSpanned = 0;
            this.SubmitFilesCB.YOffset = 0;
            this.SubmitFilesCB.SelectedIndexChanged += new System.EventHandler(this.SubmitFilesCB_SelectedIndexChanged);
            // 
            // OnSubmitLbl
            // 
            resources.ApplyResources(this.OnSubmitLbl, "OnSubmitLbl");
            this.OnSubmitLbl.CellHeight = 27;
            this.OnSubmitLbl.CellWidth = 65;
            this.OnSubmitLbl.Column = 0;
            this.OnSubmitLbl.ColumnsSpanned = 0;
            this.OnSubmitLbl.Name = "OnSubmitLbl";
            this.OnSubmitLbl.Row = 2;
            this.OnSubmitLbl.RowsSpanned = 0;
            this.OnSubmitLbl.YOffset = 4;
            // 
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.CellHeight = 27;
            this.gridLayoutSubpanel1.CellWidth = 680;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 3;
            this.gridLayoutSubpanel1.Controls.Add(this.gridPanel4);
            this.gridLayoutSubpanel1.Controls.Add(this.resolveMsgImg);
            this.gridLayoutSubpanel1.Controls.Add(this.resolveWarningLbl);
            this.gridLayoutSubpanel1.EnableDesignerGrid = false;
            this.gridLayoutSubpanel1.EnableDesignerLayout = true;
            this.gridLayoutSubpanel1.EnableParentResize = false;
            this.gridLayoutSubpanel1.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel1.MinimumRowHeight = 10;
            this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
            this.gridLayoutSubpanel1.Row = 0;
            this.gridLayoutSubpanel1.RowsSpanned = 0;
            this.gridLayoutSubpanel1.YOffset = 0;
            // 
            // gridPanel4
            // 
            resources.ApplyResources(this.gridPanel4, "gridPanel4");
            this.gridPanel4.CellHeight = 24;
            this.gridPanel4.CellWidth = 487;
            this.gridPanel4.Column = 2;
            this.gridPanel4.ColumnsSpanned = 0;
            this.gridPanel4.Name = "gridPanel4";
            this.gridPanel4.Row = 0;
            this.gridPanel4.RowsSpanned = 0;
            this.gridPanel4.YOffset = 0;
            // 
            // DescriptionPanel
            // 
            resources.ApplyResources(this.DescriptionPanel, "DescriptionPanel");
            this.DescriptionPanel.ButtonText = "Write a changelist description";
            this.DescriptionPanel.ButtonVisible = true;
            this.DescriptionPanel.ButtonWidth = 198;
            this.DescriptionPanel.Collapsed = false;
            this.DescriptionPanel.CollapsedHeight = 30;
            this.DescriptionPanel.Controls.Add(this.DescriptionTB);
            this.DescriptionPanel.Hidden = false;
            this.DescriptionPanel.Name = "DescriptionPanel";
            this.DescriptionPanel.PanelHeight = 0;
            this.DescriptionPanel.PreferencesKey = null;
            this.DescriptionPanel.ShowAlert = false;
            this.DescriptionPanel.Weight = 10;
            // 
            // DescriptionTB
            // 
            this.DescriptionTB.AcceptsReturn = true;
            resources.ApplyResources(this.DescriptionTB, "DescriptionTB");
            this.DescriptionTB.Name = "DescriptionTB";
            this.DescriptionTB.TextChanged += new System.EventHandler(this.DescriptionTB_TextChanged);
            this.DescriptionTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DescriptionTB_KeyDown);
            // 
            // slidingPanelContainer1
            // 
            resources.ApplyResources(this.slidingPanelContainer1, "slidingPanelContainer1");
            this.slidingPanelContainer1.CellHeight = 443;
            this.slidingPanelContainer1.CellWidth = 701;
            this.slidingPanelContainer1.Column = 0;
            this.slidingPanelContainer1.ColumnsSpanned = 3;
            this.slidingPanelContainer1.Controls.Add(this.DescriptionPanel);
            this.slidingPanelContainer1.Controls.Add(this.FileListPanel);
            this.slidingPanelContainer1.Controls.Add(this.JobListPanel);
            this.slidingPanelContainer1.Name = "slidingPanelContainer1";
            this.slidingPanelContainer1.Row = 0;
            this.slidingPanelContainer1.RowsSpanned = 0;
            this.slidingPanelContainer1.YOffset = 0;
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.slidingPanelContainer1);
            this.gridLayoutPanel1.Controls.Add(this.SubmitBtn);
            this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
            this.gridLayoutPanel1.Controls.Add(this.SaveBtn);
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
            this.gridPanel1.CellWidth = 458;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 1;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // SubmitDlg
            // 
            this.AcceptButton = this.SubmitBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.gridLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubmitDlg";
            this.Activated += new System.EventHandler(this.SubmitDlg_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SubmitDlg_FormClosing);
            this.Shown += new System.EventHandler(this.SubmitDlg_Shown);
            this.JobListMnu.ResumeLayout(false);
            this.FileListMenu.ResumeLayout(false);
            this.JobListPanel.ResumeLayout(false);
            this.JobListPanel.PerformLayout();
            this.gridLayoutPanel2.ResumeLayout(false);
            this.gridLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resolveMsgImg)).EndInit();
            this.FileListPanel.ResumeLayout(false);
            this.FileListPanel.PerformLayout();
            this.gridLayoutPanel3.ResumeLayout(false);
            this.gridLayoutPanel3.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.DescriptionPanel.ResumeLayout(false);
            this.DescriptionPanel.PerformLayout();
            this.slidingPanelContainer1.ResumeLayout(false);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
/*
			//
			// slidingPanelContainer1
			// 
			resources.ApplyResources(this.slidingPanelContainer1, "slidingPanelContainer1");
			this.slidingPanelContainer1.Name = "slidingPanelContainer1";
			this.slidingPanelContainer1.Controls.Add(DescritionPanel);
			this.slidingPanelContainer1.Controls.Add(FileListPanel);
			this.slidingPanelContainer1.Controls.Add(JobListPanel);
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			this.panel1.Controls.Add(slidingPanelContainer1);
*/
		private SlidingPanel DescriptionPanel;
		private System.Windows.Forms.TextBox DescriptionTB;
		private SlidingPanel FileListPanel;
        private I18nControls.GridDoubleBufferedListView FileListLV;
		private SlidingPanel JobListPanel;
        private I18nControls.GridButton BrowseJobsBtn;
        private I18nControls.GridButton AddJobBtn;
        private I18nControls.GridTextBox JobTB;
        private I18nControls.GridLabel JobLbl;
        private I18nControls.GridLabel JobStatusLbl;
        private I18nControls.GridComboBox JobStatusCB;
        private I18nControls.GridDoubleBufferedListView JobsListLV;
        private I18nControls.GridCheckBox CheckOutAfterCB;
		private System.Windows.Forms.ColumnHeader NameClm;
		private System.Windows.Forms.ColumnHeader FolderClm;
		private System.Windows.Forms.ColumnHeader ResolveStatusClm;
		private System.Windows.Forms.ColumnHeader TypeClm;
		private System.Windows.Forms.ColumnHeader PendingActionClm;
        private I18nControls.GridComboBox SubmitFilesCB;
        private I18nControls.GridLabel OnSubmitLbl;
		private System.Windows.Forms.ColumnHeader JobClm;
		private System.Windows.Forms.ColumnHeader StatusClm;
		private System.Windows.Forms.ColumnHeader JobDescriptionClm;
		private System.Windows.Forms.ContextMenuStrip JobListMnu;
		private System.Windows.Forms.ToolStripMenuItem JLM_DeleteJobMI;
		private System.Windows.Forms.ColumnHeader CheckedClm;
		private System.Windows.Forms.ContextMenuStrip FileListMenu;
		private System.Windows.Forms.ToolStripMenuItem FLM_DiffVsHaveMI;
		private System.Windows.Forms.ToolStripMenuItem FLM_ResolveMI;
        private System.Windows.Forms.CheckBox SelectAllFileListChk;
        private I18nControls.GridLabel resolveWarningLbl;
        private I18nControls.GridPictureBox resolveMsgImg;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridPanel gridPanel1;
        private I18nControls.GridButton SubmitBtn;
        private I18nControls.GridButton SaveBtn;
        private I18nControls.GridButton CancelBtn;
        private I18nControls.GridSlidingPanelContainer slidingPanelContainer1;
        private I18nControls.GridLayoutPanel gridLayoutPanel2;
        private I18nControls.GridLayoutPanel gridLayoutPanel3;
        private I18nControls.GridPanel gridPanel2;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
        private I18nControls.GridPanel gridPanel4;
	}
}
