namespace Perforce.P4VS
{
	partial class SubmittedChangelistDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubmittedChangelistDlg));
            this.revisionImages = new System.Windows.Forms.ImageList(this.components);
            this.OkBtn = new Perforce.I18nControls.GridButton();
            this.CancelBtn = new Perforce.I18nControls.GridButton();
            this.panel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.label1 = new Perforce.I18nControls.GridLabel();
            this.WorkspaceTB = new Perforce.I18nControls.GridTextBox();
            this.RestrictAccessCB = new Perforce.I18nControls.GridCheckBox();
            this.UserTB = new Perforce.I18nControls.GridTextBox();
            this.label3 = new Perforce.I18nControls.GridLabel();
            this.DateTB = new Perforce.I18nControls.GridTextBox();
            this.label2 = new Perforce.I18nControls.GridLabel();
            this.ChangelistTB = new Perforce.I18nControls.GridTextBox();
            this.label4 = new Perforce.I18nControls.GridLabel();
            this.slidingPanelContainer1 = new Perforce.I18nControls.GridSlidingPanelContainer();
            this.DescriptionPanel = new Perforce.P4VS.SlidingPanel();
            this.DescriptionTB = new System.Windows.Forms.TextBox();
            this.FilesPanel = new Perforce.P4VS.SlidingPanel();
            this.FileListLV = new Perforce.P4VS.P4ObjectTreeListView();
            this.NameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FolderClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RevisionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FiletypeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ActionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.JobsPanel = new Perforce.P4VS.SlidingPanel();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.BrowseJobsBtn = new Perforce.I18nControls.GridButton();
            this.SelectAllJobsCB = new System.Windows.Forms.CheckBox();
            this.AddJobBtn = new Perforce.I18nControls.GridButton();
            this.JobTB = new Perforce.I18nControls.GridTextBox();
            this.JobLbl = new Perforce.I18nControls.GridLabel();
            this.JobsListLV = new Perforce.I18nControls.GridDoubleBufferedListView();
            this.JobClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StatusClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.JobDescriptionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.changelistContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.diffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TLVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.slidingPanelContainer1.SuspendLayout();
            this.DescriptionPanel.SuspendLayout();
            this.FilesPanel.SuspendLayout();
            this.JobsPanel.SuspendLayout();
            this.gridLayoutPanel1.SuspendLayout();
            this.changelistContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // revisionImages
            // 
            this.revisionImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("revisionImages.ImageStream")));
            this.revisionImages.TransparentColor = System.Drawing.Color.Transparent;
            this.revisionImages.Images.SetKeyName(0, "revision_icon_add.png");
            this.revisionImages.Images.SetKeyName(1, "revision_icon_archive.png");
            this.revisionImages.Images.SetKeyName(2, "revision_icon_branch.png");
            this.revisionImages.Images.SetKeyName(3, "revision_icon_delete.png");
            this.revisionImages.Images.SetKeyName(4, "revision_icon_edit.png");
            this.revisionImages.Images.SetKeyName(5, "revision_icon_integrate.png");
            this.revisionImages.Images.SetKeyName(6, "revision_icon_moveadd.png");
            this.revisionImages.Images.SetKeyName(7, "revision_icon_movedelete.png");
            this.revisionImages.Images.SetKeyName(8, "revision_icon_purge.png");
            // 
            // OkBtn
            // 
            resources.ApplyResources(this.OkBtn, "OkBtn");
            this.OkBtn.CellHeight = 29;
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
            this.CancelBtn.CellHeight = 29;
            this.CancelBtn.CellWidth = 81;
            this.CancelBtn.Column = 2;
            this.CancelBtn.ColumnsSpanned = 0;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Row = 2;
            this.CancelBtn.RowsSpanned = 0;
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.YOffset = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gridPanel1);
            this.panel1.Controls.Add(this.OkBtn);
            this.panel1.Controls.Add(this.CancelBtn);
            this.panel1.Controls.Add(this.gridLayoutSubpanel1);
            this.panel1.Controls.Add(this.slidingPanelContainer1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.EnableDesignerGrid = false;
            this.panel1.EnableDesignerLayout = true;
            this.panel1.EnableParentResize = false;
            this.panel1.MinimumColumnWidth = 10;
            this.panel1.MinimumRowHeight = 10;
            this.panel1.Name = "panel1";
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.CellHeight = 29;
            this.gridPanel1.CellWidth = 441;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 2;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.CellHeight = 86;
            this.gridLayoutSubpanel1.CellWidth = 603;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 2;
            this.gridLayoutSubpanel1.Controls.Add(this.label1);
            this.gridLayoutSubpanel1.Controls.Add(this.WorkspaceTB);
            this.gridLayoutSubpanel1.Controls.Add(this.RestrictAccessCB);
            this.gridLayoutSubpanel1.Controls.Add(this.UserTB);
            this.gridLayoutSubpanel1.Controls.Add(this.label3);
            this.gridLayoutSubpanel1.Controls.Add(this.DateTB);
            this.gridLayoutSubpanel1.Controls.Add(this.label2);
            this.gridLayoutSubpanel1.Controls.Add(this.ChangelistTB);
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
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.CellHeight = 27;
            this.label1.CellWidth = 65;
            this.label1.Column = 0;
            this.label1.ColumnsSpanned = 0;
            this.label1.Name = "label1";
            this.label1.Row = 0;
            this.label1.RowsSpanned = 0;
            this.label1.YOffset = 4;
            // 
            // WorkspaceTB
            // 
            resources.ApplyResources(this.WorkspaceTB, "WorkspaceTB");
            this.WorkspaceTB.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.WorkspaceTB.CellHeight = 27;
            this.WorkspaceTB.CellWidth = 230;
            this.WorkspaceTB.Column = 3;
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
            this.RestrictAccessCB.CellWidth = 597;
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
            this.UserTB.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.UserTB.CellHeight = 27;
            this.UserTB.CellWidth = 230;
            this.UserTB.Column = 3;
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
            this.label3.CellHeight = 27;
            this.label3.CellWidth = 65;
            this.label3.Column = 0;
            this.label3.ColumnsSpanned = 0;
            this.label3.Name = "label3";
            this.label3.Row = 1;
            this.label3.RowsSpanned = 0;
            this.label3.YOffset = 4;
            // 
            // DateTB
            // 
            resources.ApplyResources(this.DateTB, "DateTB");
            this.DateTB.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.DateTB.CellHeight = 27;
            this.DateTB.CellWidth = 231;
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
            this.label2.CellHeight = 27;
            this.label2.CellWidth = 71;
            this.label2.Column = 2;
            this.label2.ColumnsSpanned = 0;
            this.label2.Name = "label2";
            this.label2.Row = 0;
            this.label2.RowsSpanned = 0;
            this.label2.YOffset = 4;
            // 
            // ChangelistTB
            // 
            resources.ApplyResources(this.ChangelistTB, "ChangelistTB");
            this.ChangelistTB.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ChangelistTB.CellHeight = 27;
            this.ChangelistTB.CellWidth = 231;
            this.ChangelistTB.Column = 1;
            this.ChangelistTB.ColumnsSpanned = 0;
            this.ChangelistTB.Name = "ChangelistTB";
            this.ChangelistTB.ReadOnly = true;
            this.ChangelistTB.Row = 0;
            this.ChangelistTB.RowsSpanned = 0;
            this.ChangelistTB.YOffset = 0;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.CellHeight = 27;
            this.label4.CellWidth = 71;
            this.label4.Column = 2;
            this.label4.ColumnsSpanned = 0;
            this.label4.Name = "label4";
            this.label4.Row = 1;
            this.label4.RowsSpanned = 0;
            this.label4.YOffset = 4;
            // 
            // slidingPanelContainer1
            // 
            resources.ApplyResources(this.slidingPanelContainer1, "slidingPanelContainer1");
            this.slidingPanelContainer1.CellHeight = 338;
            this.slidingPanelContainer1.CellWidth = 603;
            this.slidingPanelContainer1.Column = 0;
            this.slidingPanelContainer1.ColumnsSpanned = 2;
            this.slidingPanelContainer1.Controls.Add(this.DescriptionPanel);
            this.slidingPanelContainer1.Controls.Add(this.FilesPanel);
            this.slidingPanelContainer1.Controls.Add(this.JobsPanel);
            this.slidingPanelContainer1.Name = "slidingPanelContainer1";
            this.slidingPanelContainer1.Row = 1;
            this.slidingPanelContainer1.RowsSpanned = 0;
            this.slidingPanelContainer1.YOffset = 0;
            // 
            // DescriptionPanel
            // 
            resources.ApplyResources(this.DescriptionPanel, "DescriptionPanel");
            this.DescriptionPanel.ButtonText = "Description:";
            this.DescriptionPanel.ButtonVisible = true;
            this.DescriptionPanel.ButtonWidth = 110;
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
            this.FilesPanel.Controls.Add(this.FileListLV);
            this.FilesPanel.Hidden = false;
            this.FilesPanel.Name = "FilesPanel";
            this.FilesPanel.PanelHeight = 0;
            this.FilesPanel.PreferencesKey = null;
            this.FilesPanel.ShowAlert = false;
            this.FilesPanel.Weight = 10;
            // 
            // FileListLV
            // 
            this.FileListLV._maxLineOffset = 0;
            this.FileListLV.ActionColumn = -1;
            this.FileListLV.AllowColumnReorder = true;
            this.FileListLV.AllowDrop = true;
            resources.ApplyResources(this.FileListLV, "FileListLV");
            this.FileListLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameClm,
            this.FolderClm,
            this.RevisionClm,
            this.FiletypeClm,
            this.ActionClm});
            this.FileListLV.ContextMenuStrip = this.changelistContextMenuStrip;
            this.FileListLV.EnableIconOverlays = true;
            this.FileListLV.EnableSorting = true;
            this.FileListLV.FullRowSelect = true;
            this.FileListLV.GridLines = true;
            this.FileListLV.LargeImageList = this.revisionImages;
            this.FileListLV.MultiSelect = false;
            this.FileListLV.MultiSelectConditions = Perforce.P4VS.TreeListView.MultiSelectCondition.none;
            this.FileListLV.Name = "FileListLV";
            this.FileListLV.OverlayOffset = 3;
            this.FileListLV.RootCheckBoxes = false;
            this.FileListLV.ScrollPosition = 0;
            this.FileListLV.SmallImageList = this.revisionImages;
            this.FileListLV.TreeView = false;
            this.FileListLV.UseClassicImageList = false;
            this.FileListLV.UseCompatibleStateImageBehavior = false;
            this.FileListLV.View = System.Windows.Forms.View.Details;
            this.FileListLV.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileListLV_DragDrop);
            this.FileListLV.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileListLV_DragEnter);
            this.FileListLV.DragOver += new System.Windows.Forms.DragEventHandler(this.FileListLV_DragOver);
            this.FileListLV.DragLeave += new System.EventHandler(this.FileListLV_DragLeave);
            this.FileListLV.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.FileListLV_GiveFeedback);
            this.FileListLV.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.FileListLV_QueryContinueDrag);
            // 
            // NameClm
            // 
            resources.ApplyResources(this.NameClm, "NameClm");
            // 
            // FolderClm
            // 
            resources.ApplyResources(this.FolderClm, "FolderClm");
            // 
            // RevisionClm
            // 
            resources.ApplyResources(this.RevisionClm, "RevisionClm");
            // 
            // FiletypeClm
            // 
            resources.ApplyResources(this.FiletypeClm, "FiletypeClm");
            // 
            // ActionClm
            // 
            resources.ApplyResources(this.ActionClm, "ActionClm");
            // 
            // JobsPanel
            // 
            resources.ApplyResources(this.JobsPanel, "JobsPanel");
            this.JobsPanel.ButtonText = "Jobs";
            this.JobsPanel.ButtonVisible = true;
            this.JobsPanel.ButtonWidth = 80;
            this.JobsPanel.Collapsed = false;
            this.JobsPanel.CollapsedHeight = 30;
            this.JobsPanel.Controls.Add(this.gridLayoutPanel1);
            this.JobsPanel.Hidden = false;
            this.JobsPanel.Name = "JobsPanel";
            this.JobsPanel.PanelHeight = 0;
            this.JobsPanel.PreferencesKey = null;
            this.JobsPanel.ShowAlert = false;
            this.JobsPanel.Weight = 10;
            // 
            // gridLayoutPanel1
            // 
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.Controls.Add(this.BrowseJobsBtn);
            this.gridLayoutPanel1.Controls.Add(this.SelectAllJobsCB);
            this.gridLayoutPanel1.Controls.Add(this.AddJobBtn);
            this.gridLayoutPanel1.Controls.Add(this.JobTB);
            this.gridLayoutPanel1.Controls.Add(this.JobLbl);
            this.gridLayoutPanel1.Controls.Add(this.JobsListLV);
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            this.gridLayoutPanel1.AfterLayoutGrid += new Perforce.I18nControls.GridLayoutPanel.GridLayoutEvent(this.gridLayoutPanel1_AfterLayoutGrid);
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
            // SelectAllJobsCB
            // 
            resources.ApplyResources(this.SelectAllJobsCB, "SelectAllJobsCB");
            this.SelectAllJobsCB.Name = "SelectAllJobsCB";
            this.SelectAllJobsCB.UseVisualStyleBackColor = true;
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
            // JobTB
            // 
            resources.ApplyResources(this.JobTB, "JobTB");
            this.JobTB.CellHeight = 29;
            this.JobTB.CellWidth = 418;
            this.JobTB.Column = 1;
            this.JobTB.ColumnsSpanned = 0;
            this.JobTB.Name = "JobTB";
            this.JobTB.Row = 1;
            this.JobTB.RowsSpanned = 0;
            this.JobTB.YOffset = 1;
            this.JobTB.TextChanged += new System.EventHandler(this.JobTB_TextChanged);
            // 
            // JobLbl
            // 
            resources.ApplyResources(this.JobLbl, "JobLbl");
            this.JobLbl.CellHeight = 29;
            this.JobLbl.CellWidth = 33;
            this.JobLbl.Column = 0;
            this.JobLbl.ColumnsSpanned = 0;
            this.JobLbl.Name = "JobLbl";
            this.JobLbl.Row = 1;
            this.JobLbl.RowsSpanned = 0;
            this.JobLbl.YOffset = 5;
            // 
            // JobsListLV
            // 
            this.JobsListLV.AllowColumnReorder = true;
            resources.ApplyResources(this.JobsListLV, "JobsListLV");
            this.JobsListLV.CellHeight = 47;
            this.JobsListLV.CellWidth = 591;
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
            // changelistContextMenuStrip
            // 
            this.changelistContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.diffToolStripMenuItem,
            this.historyToolStripMenuItem,
            this.TLVToolStripMenuItem});
            this.changelistContextMenuStrip.Name = "changelistContextMenuStrip";
            resources.ApplyResources(this.changelistContextMenuStrip, "changelistContextMenuStrip");
            this.changelistContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.changelistContextMenuStrip_Opening);
            // 
            // diffToolStripMenuItem
            // 
            this.diffToolStripMenuItem.Name = "diffToolStripMenuItem";
            resources.ApplyResources(this.diffToolStripMenuItem, "diffToolStripMenuItem");
            this.diffToolStripMenuItem.Click += new System.EventHandler(this.diffToolStripMenuItem_Click);
            // 
            // historyToolStripMenuItem
            // 
            this.historyToolStripMenuItem.Name = "historyToolStripMenuItem";
            resources.ApplyResources(this.historyToolStripMenuItem, "historyToolStripMenuItem");
            this.historyToolStripMenuItem.Click += new System.EventHandler(this.historyToolStripMenuItem_Click);
            // 
            // TLVToolStripMenuItem
            // 
            this.TLVToolStripMenuItem.Name = "TLVToolStripMenuItem";
            resources.ApplyResources(this.TLVToolStripMenuItem, "TLVToolStripMenuItem");
            this.TLVToolStripMenuItem.Click += new System.EventHandler(this.TLVToolStripMenuItem_Click);
            // 
            // SubmittedChangelistDlg
            // 
            this.AcceptButton = this.OkBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubmittedChangelistDlg";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.slidingPanelContainer1.ResumeLayout(false);
            this.DescriptionPanel.ResumeLayout(false);
            this.DescriptionPanel.PerformLayout();
            this.FilesPanel.ResumeLayout(false);
            this.FilesPanel.PerformLayout();
            this.JobsPanel.ResumeLayout(false);
            this.JobsPanel.PerformLayout();
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.changelistContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private I18nControls.GridSlidingPanelContainer slidingPanelContainer1;
        private I18nControls.GridButton CancelBtn;
        private I18nControls.GridButton OkBtn;
		private SlidingPanel DescriptionPanel;
        private I18nControls.GridLabel label1;
        private I18nControls.GridCheckBox RestrictAccessCB;
        private I18nControls.GridLabel label3;
        private I18nControls.GridLabel label2;
        private I18nControls.GridLabel label4;
        private I18nControls.GridTextBox ChangelistTB;
        private I18nControls.GridTextBox DateTB;
        private I18nControls.GridTextBox UserTB;
        private I18nControls.GridTextBox WorkspaceTB;
		private SlidingPanel FilesPanel;
		private System.Windows.Forms.TextBox DescriptionTB;
		private System.Windows.Forms.ColumnHeader NameClm;
		private System.Windows.Forms.ColumnHeader FolderClm;
		private System.Windows.Forms.ColumnHeader RevisionClm;
		private System.Windows.Forms.ColumnHeader FiletypeClm;
		private System.Windows.Forms.ColumnHeader ActionClm;
		private SlidingPanel JobsPanel;
        private I18nControls.GridButton BrowseJobsBtn;
        private I18nControls.GridButton AddJobBtn;
        private I18nControls.GridTextBox JobTB;
        private I18nControls.GridLabel JobLbl;
        private I18nControls.GridDoubleBufferedListView JobsListLV;
		private System.Windows.Forms.ColumnHeader JobClm;
		private System.Windows.Forms.ColumnHeader StatusClm;
		private System.Windows.Forms.ColumnHeader JobDescriptionClm;
		private System.Windows.Forms.CheckBox SelectAllJobsCB;
		private System.Windows.Forms.ImageList revisionImages;
        private P4ObjectTreeListView FileListLV;
        private I18nControls.GridLayoutPanel panel1;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridPanel gridPanel1;
        private System.Windows.Forms.ContextMenuStrip changelistContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem diffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem historyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TLVToolStripMenuItem;
    }
}
