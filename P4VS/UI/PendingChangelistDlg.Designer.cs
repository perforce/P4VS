namespace Perforce.P4VS
{
	partial class PendingChangelistDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PendingChangelistDlg));
            this.FilesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.DiffvsHaveMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ResolveMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ShelvedFilesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowContentsSFCM = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.UnshelveSFCM = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteShelvedFileSFCM = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.DiffAgainstSourceSFCM = new System.Windows.Forms.ToolStripMenuItem();
            this.DiffAgainstWorkspaceSFCM = new System.Windows.Forms.ToolStripMenuItem();
            this.OkBtn = new Perforce.I18nControls.GridButton();
            this.CancelBtn = new Perforce.I18nControls.GridButton();
            this.slidingPanelContainer1 = new Perforce.I18nControls.GridSlidingPanelContainer();
            this.DescriptionPanel = new Perforce.P4VS.SlidingPanel();
            this.DescriptionTB = new System.Windows.Forms.TextBox();
            this.FilesPanel = new Perforce.P4VS.SlidingPanel();
            this.SelectAllFilesCB = new System.Windows.Forms.CheckBox();
            this.FileFlatList = new System.Windows.Forms.CheckedListBox();
            this.FileListLV = new Perforce.P4VS.P4ObjectTreeListView();
            this.SelectedClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FolderClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ResolveStatusClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TypeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PendingActionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ShelvedFilesPanel = new Perforce.P4VS.SlidingPanel();
            this.ShelvedFilesLV = new Perforce.P4VS.DoubleBufferedListView();
            this.ShelvedFileNameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ShelvedFileActionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ShelvedFileFolderClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.JobsPanel = new Perforce.P4VS.SlidingPanel();
            this.gridLayoutPanel2 = new Perforce.I18nControls.GridLayoutPanel();
            this.BrowseJobsBtn = new Perforce.I18nControls.GridButton();
            this.JobLbl = new Perforce.I18nControls.GridLabel();
            this.SelectAllJobsCB = new System.Windows.Forms.CheckBox();
            this.JobTB = new Perforce.I18nControls.GridTextBox();
            this.AddJobBtn = new Perforce.I18nControls.GridButton();
            this.JobsListLV = new Perforce.I18nControls.GridDoubleBufferedListView();
            this.JobClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StatusClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.JobDescriptionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.ChangelistTB = new Perforce.I18nControls.GridTextBox();
            this.label1 = new Perforce.I18nControls.GridLabel();
            this.WorkspaceTB = new Perforce.I18nControls.GridTextBox();
            this.RestrictAccessCB = new Perforce.I18nControls.GridCheckBox();
            this.UserTB = new Perforce.I18nControls.GridTextBox();
            this.label3 = new Perforce.I18nControls.GridLabel();
            this.DateTB = new Perforce.I18nControls.GridTextBox();
            this.label2 = new Perforce.I18nControls.GridLabel();
            this.label4 = new Perforce.I18nControls.GridLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.FilesContextMenuStrip.SuspendLayout();
            this.ShelvedFilesContextMenuStrip.SuspendLayout();
            this.slidingPanelContainer1.SuspendLayout();
            this.DescriptionPanel.SuspendLayout();
            this.FilesPanel.SuspendLayout();
            this.ShelvedFilesPanel.SuspendLayout();
            this.JobsPanel.SuspendLayout();
            this.gridLayoutPanel2.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FilesContextMenuStrip
            // 
            this.FilesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DiffvsHaveMI,
            this.ResolveMI});
            this.FilesContextMenuStrip.Name = "FilesContextMenuStrip";
            resources.ApplyResources(this.FilesContextMenuStrip, "FilesContextMenuStrip");
            this.FilesContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.FilesContextMenuStrip_Opening);
            this.FilesContextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.FilesContextMenuStrip_ItemClicked);
            // 
            // DiffvsHaveMI
            // 
            this.DiffvsHaveMI.Name = "DiffvsHaveMI";
            resources.ApplyResources(this.DiffvsHaveMI, "DiffvsHaveMI");
            this.DiffvsHaveMI.Tag = "DiffVsHave";
            // 
            // ResolveMI
            // 
            this.ResolveMI.Name = "ResolveMI";
            resources.ApplyResources(this.ResolveMI, "ResolveMI");
            this.ResolveMI.Tag = "Resolve";
            // 
            // ShelvedFilesContextMenuStrip
            // 
            this.ShelvedFilesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowContentsSFCM,
            this.toolStripSeparator1,
            this.UnshelveSFCM,
            this.DeleteShelvedFileSFCM,
            this.toolStripSeparator2,
            this.DiffAgainstSourceSFCM,
            this.DiffAgainstWorkspaceSFCM});
            this.ShelvedFilesContextMenuStrip.Name = "ShelvedFilesContextMenuStrip";
            resources.ApplyResources(this.ShelvedFilesContextMenuStrip, "ShelvedFilesContextMenuStrip");
            // 
            // ShowContentsSFCM
            // 
            this.ShowContentsSFCM.Name = "ShowContentsSFCM";
            resources.ApplyResources(this.ShowContentsSFCM, "ShowContentsSFCM");
            this.ShowContentsSFCM.Click += new System.EventHandler(this.OpenSFCM_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // UnshelveSFCM
            // 
            this.UnshelveSFCM.Name = "UnshelveSFCM";
            resources.ApplyResources(this.UnshelveSFCM, "UnshelveSFCM");
            this.UnshelveSFCM.Click += new System.EventHandler(this.UnshelveSFCM_Click);
            // 
            // DeleteShelvedFileSFCM
            // 
            this.DeleteShelvedFileSFCM.Name = "DeleteShelvedFileSFCM";
            resources.ApplyResources(this.DeleteShelvedFileSFCM, "DeleteShelvedFileSFCM");
            this.DeleteShelvedFileSFCM.Click += new System.EventHandler(this.DeleteShelvedFileSFCM_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // DiffAgainstSourceSFCM
            // 
            this.DiffAgainstSourceSFCM.Name = "DiffAgainstSourceSFCM";
            resources.ApplyResources(this.DiffAgainstSourceSFCM, "DiffAgainstSourceSFCM");
            this.DiffAgainstSourceSFCM.Click += new System.EventHandler(this.DiffAgainstSourceSFCM_Click);
            // 
            // DiffAgainstWorkspaceSFCM
            // 
            this.DiffAgainstWorkspaceSFCM.Name = "DiffAgainstWorkspaceSFCM";
            resources.ApplyResources(this.DiffAgainstWorkspaceSFCM, "DiffAgainstWorkspaceSFCM");
            this.DiffAgainstWorkspaceSFCM.Click += new System.EventHandler(this.DiffAgainstWorkspaceSFCM_Click);
            // 
            // OkBtn
            // 
            resources.ApplyResources(this.OkBtn, "OkBtn");
            this.OkBtn.CellHeight = 36;
            this.OkBtn.CellWidth = 81;
            this.OkBtn.Column = 1;
            this.OkBtn.ColumnsSpanned = 0;
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Row = 2;
            this.OkBtn.RowsSpanned = 0;
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.YOffset = 0;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.CellHeight = 36;
            this.CancelBtn.CellWidth = 81;
            this.CancelBtn.Column = 2;
            this.CancelBtn.ColumnsSpanned = 0;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Row = 2;
            this.CancelBtn.RowsSpanned = 0;
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.YOffset = 0;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // slidingPanelContainer1
            // 
            resources.ApplyResources(this.slidingPanelContainer1, "slidingPanelContainer1");
            this.slidingPanelContainer1.CellHeight = 491;
            this.slidingPanelContainer1.CellWidth = 572;
            this.slidingPanelContainer1.Column = 0;
            this.slidingPanelContainer1.ColumnsSpanned = 2;
            this.slidingPanelContainer1.Controls.Add(this.DescriptionPanel);
            this.slidingPanelContainer1.Controls.Add(this.FilesPanel);
            this.slidingPanelContainer1.Controls.Add(this.ShelvedFilesPanel);
            this.slidingPanelContainer1.Controls.Add(this.JobsPanel);
            this.slidingPanelContainer1.Name = "slidingPanelContainer1";
            this.slidingPanelContainer1.Row = 1;
            this.slidingPanelContainer1.RowsSpanned = 0;
            this.slidingPanelContainer1.YOffset = 0;
            // 
            // DescriptionPanel
            // 
            resources.ApplyResources(this.DescriptionPanel, "DescriptionPanel");
            this.DescriptionPanel.ButtonText = "Write a changelist description";
            this.DescriptionPanel.ButtonVisible = true;
            this.DescriptionPanel.ButtonWidth = 190;
            this.DescriptionPanel.Collapsed = false;
            this.DescriptionPanel.CollapsedHeight = 30;
            this.DescriptionPanel.Controls.Add(this.DescriptionTB);
            this.DescriptionPanel.Hidden = false;
            this.DescriptionPanel.Name = "DescriptionPanel";
            this.DescriptionPanel.PanelHeight = 0;
            this.DescriptionPanel.PreferencesKey = null;
            this.DescriptionPanel.ShowAlert = true;
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
            // FilesPanel
            // 
            resources.ApplyResources(this.FilesPanel, "FilesPanel");
            this.FilesPanel.ButtonText = "Files";
            this.FilesPanel.ButtonVisible = true;
            this.FilesPanel.ButtonWidth = 74;
            this.FilesPanel.Collapsed = false;
            this.FilesPanel.CollapsedHeight = 30;
            this.FilesPanel.Controls.Add(this.SelectAllFilesCB);
            this.FilesPanel.Controls.Add(this.FileFlatList);
            this.FilesPanel.Controls.Add(this.FileListLV);
            this.FilesPanel.Hidden = false;
            this.FilesPanel.Name = "FilesPanel";
            this.FilesPanel.PanelHeight = 0;
            this.FilesPanel.PreferencesKey = null;
            this.FilesPanel.ShowAlert = false;
            this.FilesPanel.Weight = 10;
            // 
            // SelectAllFilesCB
            // 
            resources.ApplyResources(this.SelectAllFilesCB, "SelectAllFilesCB");
            this.SelectAllFilesCB.Name = "SelectAllFilesCB";
            this.SelectAllFilesCB.UseVisualStyleBackColor = true;
            this.SelectAllFilesCB.CheckedChanged += new System.EventHandler(this.SelectAllFilesCB_CheckedChanged);
            // 
            // FileFlatList
            // 
            resources.ApplyResources(this.FileFlatList, "FileFlatList");
            this.FileFlatList.FormattingEnabled = true;
            this.FileFlatList.Name = "FileFlatList";
            // 
            // FileListLV
            // 
            this.FileListLV._maxLineOffset = 0;
            this.FileListLV.ActionColumn = -1;
            this.FileListLV.AllowColumnReorder = true;
            this.FileListLV.AllowDrop = true;
            resources.ApplyResources(this.FileListLV, "FileListLV");
            this.FileListLV.CheckBoxes = true;
            this.FileListLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SelectedClm,
            this.NameClm,
            this.FolderClm,
            this.ResolveStatusClm,
            this.TypeClm,
            this.PendingActionClm});
            this.FileListLV.ContextMenuStrip = this.FilesContextMenuStrip;
            this.FileListLV.EnableIconOverlays = true;
            this.FileListLV.EnableSorting = true;
            this.FileListLV.FullRowSelect = true;
            this.FileListLV.GridLines = true;
            this.FileListLV.MultiSelectConditions = Perforce.P4VS.TreeListView.MultiSelectCondition.none;
            this.FileListLV.Name = "FileListLV";
            this.FileListLV.OverlayOffset = 3;
            this.FileListLV.RootCheckBoxes = false;
            this.FileListLV.ScrollPosition = 0;
            this.FileListLV.TreeView = false;
            this.FileListLV.UseClassicImageList = false;
            this.FileListLV.UseCompatibleStateImageBehavior = false;
            this.FileListLV.View = System.Windows.Forms.View.Details;
            this.FileListLV.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.FileListLV_ItemChecked);
            this.FileListLV.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileListLV_DragDrop);
            this.FileListLV.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileListLV_DragEnter);
            this.FileListLV.DragOver += new System.Windows.Forms.DragEventHandler(this.FileListLV_DragOver);
            this.FileListLV.DragLeave += new System.EventHandler(this.FileListLV_DragLeave);
            this.FileListLV.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.FileListLV_GiveFeedback);
            this.FileListLV.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.FileListLV_QueryContinueDrag);
            // 
            // SelectedClm
            // 
            resources.ApplyResources(this.SelectedClm, "SelectedClm");
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
            // ShelvedFilesPanel
            // 
            resources.ApplyResources(this.ShelvedFilesPanel, "ShelvedFilesPanel");
            this.ShelvedFilesPanel.ButtonText = "Shelved files";
            this.ShelvedFilesPanel.ButtonVisible = true;
            this.ShelvedFilesPanel.ButtonWidth = 110;
            this.ShelvedFilesPanel.Collapsed = false;
            this.ShelvedFilesPanel.CollapsedHeight = 30;
            this.ShelvedFilesPanel.Controls.Add(this.ShelvedFilesLV);
            this.ShelvedFilesPanel.Hidden = false;
            this.ShelvedFilesPanel.Name = "ShelvedFilesPanel";
            this.ShelvedFilesPanel.PanelHeight = 0;
            this.ShelvedFilesPanel.PreferencesKey = null;
            this.ShelvedFilesPanel.ShowAlert = false;
            this.ShelvedFilesPanel.Weight = 10;
            // 
            // ShelvedFilesLV
            // 
            this.ShelvedFilesLV.AllowColumnReorder = true;
            resources.ApplyResources(this.ShelvedFilesLV, "ShelvedFilesLV");
            this.ShelvedFilesLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ShelvedFileNameClm,
            this.ShelvedFileActionClm,
            this.ShelvedFileFolderClm});
            this.ShelvedFilesLV.ContextMenuStrip = this.ShelvedFilesContextMenuStrip;
            this.ShelvedFilesLV.FullRowSelect = true;
            this.ShelvedFilesLV.GridLines = true;
            this.ShelvedFilesLV.HideActionsColumn = false;
            this.ShelvedFilesLV.Name = "ShelvedFilesLV";
            this.ShelvedFilesLV.OwnerDraw = true;
            this.ShelvedFilesLV.UseCompatibleStateImageBehavior = false;
            this.ShelvedFilesLV.View = System.Windows.Forms.View.Details;
            this.ShelvedFilesLV.DoubleClick += new System.EventHandler(this.ShelvedFilesLV_DoubleClick);
            // 
            // ShelvedFileNameClm
            // 
            resources.ApplyResources(this.ShelvedFileNameClm, "ShelvedFileNameClm");
            // 
            // ShelvedFileActionClm
            // 
            resources.ApplyResources(this.ShelvedFileActionClm, "ShelvedFileActionClm");
            // 
            // ShelvedFileFolderClm
            // 
            resources.ApplyResources(this.ShelvedFileFolderClm, "ShelvedFileFolderClm");
            // 
            // JobsPanel
            // 
            resources.ApplyResources(this.JobsPanel, "JobsPanel");
            this.JobsPanel.ButtonText = "Jobs";
            this.JobsPanel.ButtonVisible = true;
            this.JobsPanel.ButtonWidth = 80;
            this.JobsPanel.Collapsed = false;
            this.JobsPanel.CollapsedHeight = 30;
            this.JobsPanel.Controls.Add(this.gridLayoutPanel2);
            this.JobsPanel.Hidden = false;
            this.JobsPanel.Name = "JobsPanel";
            this.JobsPanel.PanelHeight = 0;
            this.JobsPanel.PreferencesKey = null;
            this.JobsPanel.ShowAlert = false;
            this.JobsPanel.Weight = 15;
            // 
            // gridLayoutPanel2
            // 
            resources.ApplyResources(this.gridLayoutPanel2, "gridLayoutPanel2");
            this.gridLayoutPanel2.Controls.Add(this.BrowseJobsBtn);
            this.gridLayoutPanel2.Controls.Add(this.JobLbl);
            this.gridLayoutPanel2.Controls.Add(this.SelectAllJobsCB);
            this.gridLayoutPanel2.Controls.Add(this.JobTB);
            this.gridLayoutPanel2.Controls.Add(this.AddJobBtn);
            this.gridLayoutPanel2.Controls.Add(this.JobsListLV);
            this.gridLayoutPanel2.EnableDesignerGrid = false;
            this.gridLayoutPanel2.EnableDesignerLayout = false;
            this.gridLayoutPanel2.EnableParentResize = false;
            this.gridLayoutPanel2.MinimumColumnWidth = 10;
            this.gridLayoutPanel2.MinimumRowHeight = 10;
            this.gridLayoutPanel2.Name = "gridLayoutPanel2";
            this.gridLayoutPanel2.AfterLayoutGrid += new Perforce.I18nControls.GridLayoutPanel.GridLayoutEvent(this.gridLayoutPanel2_AfterLayoutGrid);
            // 
            // BrowseJobsBtn
            // 
            resources.ApplyResources(this.BrowseJobsBtn, "BrowseJobsBtn");
            this.BrowseJobsBtn.CellHeight = 29;
            this.BrowseJobsBtn.CellWidth = 70;
            this.BrowseJobsBtn.Column = 3;
            this.BrowseJobsBtn.ColumnsSpanned = 0;
            this.BrowseJobsBtn.Name = "BrowseJobsBtn";
            this.BrowseJobsBtn.Row = 1;
            this.BrowseJobsBtn.RowsSpanned = 0;
            this.BrowseJobsBtn.UseVisualStyleBackColor = true;
            this.BrowseJobsBtn.YOffset = 0;
            this.BrowseJobsBtn.Click += new System.EventHandler(this.BrowseJobsBtn_Click);
            // 
            // JobLbl
            // 
            resources.ApplyResources(this.JobLbl, "JobLbl");
            this.JobLbl.CellHeight = 29;
            this.JobLbl.CellWidth = 42;
            this.JobLbl.Column = 0;
            this.JobLbl.ColumnsSpanned = 0;
            this.JobLbl.Name = "JobLbl";
            this.JobLbl.Row = 1;
            this.JobLbl.RowsSpanned = 0;
            this.JobLbl.YOffset = 5;
            // 
            // SelectAllJobsCB
            // 
            resources.ApplyResources(this.SelectAllJobsCB, "SelectAllJobsCB");
            this.SelectAllJobsCB.Name = "SelectAllJobsCB";
            this.SelectAllJobsCB.UseVisualStyleBackColor = true;
            this.SelectAllJobsCB.CheckedChanged += new System.EventHandler(this.SelectAllJobsCB_CheckedChanged);
            // 
            // JobTB
            // 
            resources.ApplyResources(this.JobTB, "JobTB");
            this.JobTB.CellHeight = 29;
            this.JobTB.CellWidth = 389;
            this.JobTB.Column = 1;
            this.JobTB.ColumnsSpanned = 0;
            this.JobTB.Name = "JobTB";
            this.JobTB.Row = 1;
            this.JobTB.RowsSpanned = 0;
            this.JobTB.YOffset = 1;
            this.JobTB.TextChanged += new System.EventHandler(this.JobTB_TextChanged);
            // 
            // AddJobBtn
            // 
            resources.ApplyResources(this.AddJobBtn, "AddJobBtn");
            this.AddJobBtn.CellHeight = 29;
            this.AddJobBtn.CellWidth = 70;
            this.AddJobBtn.Column = 2;
            this.AddJobBtn.ColumnsSpanned = 0;
            this.AddJobBtn.Name = "AddJobBtn";
            this.AddJobBtn.Row = 1;
            this.AddJobBtn.RowsSpanned = 0;
            this.AddJobBtn.UseVisualStyleBackColor = true;
            this.AddJobBtn.YOffset = 0;
            this.AddJobBtn.Click += new System.EventHandler(this.AddJobBtn_Click);
            // 
            // JobsListLV
            // 
            this.JobsListLV.AllowColumnReorder = true;
            resources.ApplyResources(this.JobsListLV, "JobsListLV");
            this.JobsListLV.CellHeight = 89;
            this.JobsListLV.CellWidth = 571;
            this.JobsListLV.CheckBoxes = true;
            this.JobsListLV.Column = 0;
            this.JobsListLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.JobClm,
            this.StatusClm,
            this.JobDescriptionClm});
            this.JobsListLV.ColumnsSpanned = 3;
            this.JobsListLV.GridLines = true;
            this.JobsListLV.HideActionsColumn = false;
            this.JobsListLV.Name = "JobsListLV";
            this.JobsListLV.OwnerDraw = true;
            this.JobsListLV.Row = 0;
            this.JobsListLV.RowsSpanned = 0;
            this.JobsListLV.UseCompatibleStateImageBehavior = false;
            this.JobsListLV.View = System.Windows.Forms.View.Details;
            this.JobsListLV.YOffset = 0;
            this.JobsListLV.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.JobsListLV_ItemChecked);
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
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.CellHeight = 80;
            this.gridLayoutSubpanel1.CellWidth = 572;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 2;
            this.gridLayoutSubpanel1.Controls.Add(this.ChangelistTB);
            this.gridLayoutSubpanel1.Controls.Add(this.label1);
            this.gridLayoutSubpanel1.Controls.Add(this.WorkspaceTB);
            this.gridLayoutSubpanel1.Controls.Add(this.RestrictAccessCB);
            this.gridLayoutSubpanel1.Controls.Add(this.UserTB);
            this.gridLayoutSubpanel1.Controls.Add(this.label3);
            this.gridLayoutSubpanel1.Controls.Add(this.DateTB);
            this.gridLayoutSubpanel1.Controls.Add(this.label2);
            this.gridLayoutSubpanel1.Controls.Add(this.label4);
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
            // ChangelistTB
            // 
            resources.ApplyResources(this.ChangelistTB, "ChangelistTB");
            this.ChangelistTB.CellHeight = 26;
            this.ChangelistTB.CellWidth = 211;
            this.ChangelistTB.Column = 1;
            this.ChangelistTB.ColumnsSpanned = 0;
            this.ChangelistTB.Name = "ChangelistTB";
            this.ChangelistTB.ReadOnly = true;
            this.ChangelistTB.Row = 0;
            this.ChangelistTB.RowsSpanned = 0;
            this.ChangelistTB.YOffset = 0;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.CellHeight = 26;
            this.label1.CellWidth = 65;
            this.label1.Column = 0;
            this.label1.ColumnsSpanned = 0;
            this.label1.Name = "label1";
            this.label1.Row = 0;
            this.label1.RowsSpanned = 0;
            this.label1.YOffset = 3;
            // 
            // WorkspaceTB
            // 
            resources.ApplyResources(this.WorkspaceTB, "WorkspaceTB");
            this.WorkspaceTB.CellHeight = 26;
            this.WorkspaceTB.CellWidth = 210;
            this.WorkspaceTB.Column = 4;
            this.WorkspaceTB.ColumnsSpanned = 0;
            this.WorkspaceTB.Name = "WorkspaceTB";
            this.WorkspaceTB.ReadOnly = true;
            this.WorkspaceTB.Row = 0;
            this.WorkspaceTB.RowsSpanned = 0;
            this.WorkspaceTB.YOffset = 0;
            // 
            // RestrictAccessCB
            // 
            resources.ApplyResources(this.RestrictAccessCB, "RestrictAccessCB");
            this.RestrictAccessCB.CellHeight = 23;
            this.RestrictAccessCB.CellWidth = 347;
            this.RestrictAccessCB.Column = 0;
            this.RestrictAccessCB.ColumnsSpanned = 3;
            this.RestrictAccessCB.Name = "RestrictAccessCB";
            this.RestrictAccessCB.Row = 2;
            this.RestrictAccessCB.RowsSpanned = 0;
            this.RestrictAccessCB.UseVisualStyleBackColor = true;
            this.RestrictAccessCB.YOffset = 0;
            // 
            // UserTB
            // 
            resources.ApplyResources(this.UserTB, "UserTB");
            this.UserTB.CellHeight = 26;
            this.UserTB.CellWidth = 210;
            this.UserTB.Column = 4;
            this.UserTB.ColumnsSpanned = 0;
            this.UserTB.Name = "UserTB";
            this.UserTB.ReadOnly = true;
            this.UserTB.Row = 1;
            this.UserTB.RowsSpanned = 0;
            this.UserTB.YOffset = 0;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.CellHeight = 26;
            this.label3.CellWidth = 65;
            this.label3.Column = 0;
            this.label3.ColumnsSpanned = 0;
            this.label3.Name = "label3";
            this.label3.Row = 1;
            this.label3.RowsSpanned = 0;
            this.label3.YOffset = 3;
            // 
            // DateTB
            // 
            resources.ApplyResources(this.DateTB, "DateTB");
            this.DateTB.CellHeight = 26;
            this.DateTB.CellWidth = 211;
            this.DateTB.Column = 1;
            this.DateTB.ColumnsSpanned = 0;
            this.DateTB.Name = "DateTB";
            this.DateTB.ReadOnly = true;
            this.DateTB.Row = 1;
            this.DateTB.RowsSpanned = 0;
            this.DateTB.YOffset = 0;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.CellHeight = 26;
            this.label2.CellWidth = 71;
            this.label2.Column = 3;
            this.label2.ColumnsSpanned = 0;
            this.label2.Name = "label2";
            this.label2.Row = 0;
            this.label2.RowsSpanned = 0;
            this.label2.YOffset = 3;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.CellHeight = 26;
            this.label4.CellWidth = 71;
            this.label4.Column = 3;
            this.label4.ColumnsSpanned = 0;
            this.label4.Name = "label4";
            this.label4.Row = 1;
            this.label4.RowsSpanned = 0;
            this.label4.YOffset = 3;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.slidingPanelContainer1);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.OkBtn);
            this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
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
            this.gridPanel1.CellHeight = 36;
            this.gridPanel1.CellWidth = 410;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 2;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // PendingChangelistDlg
            // 
            this.AcceptButton = this.OkBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.gridLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PendingChangelistDlg";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.FilesContextMenuStrip.ResumeLayout(false);
            this.ShelvedFilesContextMenuStrip.ResumeLayout(false);
            this.slidingPanelContainer1.ResumeLayout(false);
            this.DescriptionPanel.ResumeLayout(false);
            this.DescriptionPanel.PerformLayout();
            this.FilesPanel.ResumeLayout(false);
            this.FilesPanel.PerformLayout();
            this.ShelvedFilesPanel.ResumeLayout(false);
            this.ShelvedFilesPanel.PerformLayout();
            this.JobsPanel.ResumeLayout(false);
            this.JobsPanel.PerformLayout();
            this.gridLayoutPanel2.ResumeLayout(false);
            this.gridLayoutPanel2.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.gridLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private I18nControls.GridButton CancelBtn;
		private I18nControls.GridButton OkBtn;
		private System.Windows.Forms.ContextMenuStrip ShelvedFilesContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem UnshelveSFCM;
        private System.Windows.Forms.ToolStripMenuItem DeleteShelvedFileSFCM;
		private System.Windows.Forms.ToolStripMenuItem ShowContentsSFCM;
        private System.Windows.Forms.ContextMenuStrip FilesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem DiffvsHaveMI;
        private System.Windows.Forms.ToolStripMenuItem ResolveMI;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem DiffAgainstSourceSFCM;
        private System.Windows.Forms.ToolStripMenuItem DiffAgainstWorkspaceSFCM;
		private I18nControls.GridSlidingPanelContainer slidingPanelContainer1;
        private I18nControls.GridLabel label1;
        private I18nControls.GridCheckBox RestrictAccessCB;
        private I18nControls.GridLabel label3;
        private I18nControls.GridLabel label2;
        private I18nControls.GridLabel label4;
        private I18nControls.GridTextBox ChangelistTB;
        private I18nControls.GridTextBox DateTB;
        private I18nControls.GridTextBox UserTB;
        private I18nControls.GridTextBox WorkspaceTB;
        private SlidingPanel DescriptionPanel;
        private System.Windows.Forms.TextBox DescriptionTB;
        private SlidingPanel FilesPanel;
        private System.Windows.Forms.CheckBox SelectAllFilesCB;
        private P4ObjectTreeListView FileListLV;
        private System.Windows.Forms.ColumnHeader SelectedClm;
        private System.Windows.Forms.ColumnHeader NameClm;
        private System.Windows.Forms.ColumnHeader FolderClm;
        private System.Windows.Forms.ColumnHeader ResolveStatusClm;
        private System.Windows.Forms.ColumnHeader TypeClm;
        private System.Windows.Forms.ColumnHeader PendingActionClm;
        private SlidingPanel ShelvedFilesPanel;
        private DoubleBufferedListView ShelvedFilesLV;
        private System.Windows.Forms.ColumnHeader ShelvedFileNameClm;
        private System.Windows.Forms.ColumnHeader ShelvedFileActionClm;
        private System.Windows.Forms.ColumnHeader ShelvedFileFolderClm;
        private SlidingPanel JobsPanel;
		private I18nControls.GridButton BrowseJobsBtn;
		private System.Windows.Forms.CheckBox SelectAllJobsCB;
		private I18nControls.GridButton AddJobBtn;
		private I18nControls.GridTextBox JobTB;
		private I18nControls.GridLabel JobLbl;
		private I18nControls.GridDoubleBufferedListView JobsListLV;
        private System.Windows.Forms.ColumnHeader JobClm;
        private System.Windows.Forms.ColumnHeader StatusClm;
        private System.Windows.Forms.ColumnHeader JobDescriptionClm;
        private System.Windows.Forms.Panel panel1;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridLayoutPanel gridLayoutPanel2;
        private System.Windows.Forms.CheckedListBox FileFlatList;
        private I18nControls.GridPanel gridPanel1;
    }
}
