namespace Perforce.P4VS
{
	partial class LabelsToolWindowFilesControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LabelsToolWindowFilesControl));
            this.LabeledRevisionsLV_ImageList = new System.Windows.Forms.ImageList(this.components);
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.LabeledRevisionsLV = new Perforce.P4VS.TreeListView();
            this.LrFileNameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LrRevisionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LrActionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LrFiletypeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LrFolderClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WorkspaceFilesLV = new Perforce.P4VS.P4ObjectTreeListView();
            this.WsFileNameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WsFolderClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WsRevisonClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WsDateModifiedClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WsSizeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WsTypeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WsFiletypeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WorkspaceVersionRB = new Perforce.I18nControls.GridRadioButton();
            this.LabeledRevisionsRB = new Perforce.I18nControls.GridRadioButton();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabeledRevisionsLV_ImageList
            // 
            this.LabeledRevisionsLV_ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("LabeledRevisionsLV_ImageList.ImageStream")));
            this.LabeledRevisionsLV_ImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.LabeledRevisionsLV_ImageList.Images.SetKeyName(0, "revision_icon_add.png");
            this.LabeledRevisionsLV_ImageList.Images.SetKeyName(1, "revision_icon_archive.png");
            this.LabeledRevisionsLV_ImageList.Images.SetKeyName(2, "revision_icon_base.png");
            this.LabeledRevisionsLV_ImageList.Images.SetKeyName(3, "revision_icon_branch.png");
            this.LabeledRevisionsLV_ImageList.Images.SetKeyName(4, "revision_icon_delete.png");
            this.LabeledRevisionsLV_ImageList.Images.SetKeyName(5, "revision_icon_edit.png");
            this.LabeledRevisionsLV_ImageList.Images.SetKeyName(6, "revision_icon_integrate.png");
            this.LabeledRevisionsLV_ImageList.Images.SetKeyName(7, "revision_icon_moveadd.png");
            this.LabeledRevisionsLV_ImageList.Images.SetKeyName(8, "revision_icon_movedelete.png");
            this.LabeledRevisionsLV_ImageList.Images.SetKeyName(9, "revision_icon_purge.png");
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.WorkspaceVersionRB);
            this.gridLayoutPanel1.Controls.Add(this.LabeledRevisionsRB);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = true;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.BackColor = System.Drawing.SystemColors.Window;
            this.gridPanel1.CellHeight = 131;
            this.gridPanel1.CellWidth = 448;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 1;
            this.gridPanel1.Controls.Add(this.LabeledRevisionsLV);
            this.gridPanel1.Controls.Add(this.WorkspaceFilesLV);
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 1;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // LabeledRevisionsLV
            // 
            this.LabeledRevisionsLV._maxLineOffset = 0;
            this.LabeledRevisionsLV.ActionColumn = -1;
            this.LabeledRevisionsLV.AllowColumnReorder = true;
            this.LabeledRevisionsLV.BackColor = System.Drawing.SystemColors.Window;
            this.LabeledRevisionsLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LrFileNameClm,
            this.LrRevisionClm,
            this.LrActionClm,
            this.LrFiletypeClm,
            this.LrFolderClm});
            resources.ApplyResources(this.LabeledRevisionsLV, "LabeledRevisionsLV");
            this.LabeledRevisionsLV.EnableIconOverlays = false;
            this.LabeledRevisionsLV.EnableSorting = true;
            this.LabeledRevisionsLV.GridLines = true;
            this.LabeledRevisionsLV.Name = "LabeledRevisionsLV";
            this.LabeledRevisionsLV.OverlayOffset = 0;
            this.LabeledRevisionsLV.RootCheckBoxes = false;
            this.LabeledRevisionsLV.ScrollPosition = 0;
            this.LabeledRevisionsLV.SmallImageList = this.LabeledRevisionsLV_ImageList;
            this.LabeledRevisionsLV.TreeView = true;
            this.LabeledRevisionsLV.UseClassicImageList = false;
            this.LabeledRevisionsLV.UseCompatibleStateImageBehavior = false;
            this.LabeledRevisionsLV.View = System.Windows.Forms.View.Details;
            // 
            // LrFileNameClm
            // 
            resources.ApplyResources(this.LrFileNameClm, "LrFileNameClm");
            // 
            // LrRevisionClm
            // 
            resources.ApplyResources(this.LrRevisionClm, "LrRevisionClm");
            // 
            // LrActionClm
            // 
            resources.ApplyResources(this.LrActionClm, "LrActionClm");
            // 
            // LrFiletypeClm
            // 
            resources.ApplyResources(this.LrFiletypeClm, "LrFiletypeClm");
            // 
            // LrFolderClm
            // 
            resources.ApplyResources(this.LrFolderClm, "LrFolderClm");
            // 
            // WorkspaceFilesLV
            // 
            this.WorkspaceFilesLV._maxLineOffset = 0;
            this.WorkspaceFilesLV.ActionColumn = -1;
            this.WorkspaceFilesLV.AllowColumnReorder = true;
            this.WorkspaceFilesLV.BackColor = System.Drawing.SystemColors.Window;
            this.WorkspaceFilesLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.WsFileNameClm,
            this.WsFolderClm,
            this.WsRevisonClm,
            this.WsDateModifiedClm,
            this.WsSizeClm,
            this.WsTypeClm,
            this.WsFiletypeClm});
            resources.ApplyResources(this.WorkspaceFilesLV, "WorkspaceFilesLV");
            this.WorkspaceFilesLV.EnableIconOverlays = true;
            this.WorkspaceFilesLV.EnableSorting = true;
            this.WorkspaceFilesLV.GridLines = true;
            this.WorkspaceFilesLV.Name = "WorkspaceFilesLV";
            this.WorkspaceFilesLV.OverlayOffset = 3;
            this.WorkspaceFilesLV.RootCheckBoxes = false;
            this.WorkspaceFilesLV.ScrollPosition = 0;
            this.WorkspaceFilesLV.TreeView = false;
            this.WorkspaceFilesLV.UseClassicImageList = false;
            this.WorkspaceFilesLV.UseCompatibleStateImageBehavior = false;
            this.WorkspaceFilesLV.View = System.Windows.Forms.View.Details;
            // 
            // WsFileNameClm
            // 
            resources.ApplyResources(this.WsFileNameClm, "WsFileNameClm");
            // 
            // WsFolderClm
            // 
            resources.ApplyResources(this.WsFolderClm, "WsFolderClm");
            // 
            // WsRevisonClm
            // 
            resources.ApplyResources(this.WsRevisonClm, "WsRevisonClm");
            // 
            // WsDateModifiedClm
            // 
            resources.ApplyResources(this.WsDateModifiedClm, "WsDateModifiedClm");
            // 
            // WsSizeClm
            // 
            resources.ApplyResources(this.WsSizeClm, "WsSizeClm");
            // 
            // WsTypeClm
            // 
            resources.ApplyResources(this.WsTypeClm, "WsTypeClm");
            // 
            // WsFiletypeClm
            // 
            resources.ApplyResources(this.WsFiletypeClm, "WsFiletypeClm");
            // 
            // WorkspaceVersionRB
            // 
            resources.ApplyResources(this.WorkspaceVersionRB, "WorkspaceVersionRB");
            this.WorkspaceVersionRB.CellHeight = 105;
            this.WorkspaceVersionRB.CellWidth = 167;
            this.WorkspaceVersionRB.Column = 1;
            this.WorkspaceVersionRB.ColumnsSpanned = 0;
            this.WorkspaceVersionRB.Name = "WorkspaceVersionRB";
            this.WorkspaceVersionRB.Row = 0;
            this.WorkspaceVersionRB.RowsSpanned = 0;
            this.WorkspaceVersionRB.UseVisualStyleBackColor = true;
            this.WorkspaceVersionRB.YOffset = 0;
            this.WorkspaceVersionRB.CheckedChanged += new System.EventHandler(this.WorkspaceVersionRB_CheckedChanged);
            // 
            // LabeledRevisionsRB
            // 
            resources.ApplyResources(this.LabeledRevisionsRB, "LabeledRevisionsRB");
            this.LabeledRevisionsRB.CellHeight = 105;
            this.LabeledRevisionsRB.CellWidth = 281;
            this.LabeledRevisionsRB.Checked = true;
            this.LabeledRevisionsRB.Column = 0;
            this.LabeledRevisionsRB.ColumnsSpanned = 0;
            this.LabeledRevisionsRB.Name = "LabeledRevisionsRB";
            this.LabeledRevisionsRB.Row = 0;
            this.LabeledRevisionsRB.RowsSpanned = 0;
            this.LabeledRevisionsRB.TabStop = true;
            this.LabeledRevisionsRB.UseVisualStyleBackColor = true;
            this.LabeledRevisionsRB.YOffset = 0;
            this.LabeledRevisionsRB.CheckedChanged += new System.EventHandler(this.LabeledRevisionsRB_CheckedChanged);
            // 
            // LabelsToolWindowFilesControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridLayoutPanel1);
            this.Name = "LabelsToolWindowFilesControl";
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.gridPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private I18nControls.GridRadioButton LabeledRevisionsRB;
        private I18nControls.GridRadioButton WorkspaceVersionRB;
		private System.Windows.Forms.ImageList LabeledRevisionsLV_ImageList;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridPanel gridPanel1;
        private TreeListView LabeledRevisionsLV;
        private System.Windows.Forms.ColumnHeader LrFileNameClm;
        private System.Windows.Forms.ColumnHeader LrRevisionClm;
        private System.Windows.Forms.ColumnHeader LrActionClm;
        private System.Windows.Forms.ColumnHeader LrFiletypeClm;
        private System.Windows.Forms.ColumnHeader LrFolderClm;
        private P4ObjectTreeListView WorkspaceFilesLV;
        private System.Windows.Forms.ColumnHeader WsFileNameClm;
        private System.Windows.Forms.ColumnHeader WsFolderClm;
        private System.Windows.Forms.ColumnHeader WsRevisonClm;
        private System.Windows.Forms.ColumnHeader WsDateModifiedClm;
        private System.Windows.Forms.ColumnHeader WsSizeClm;
        private System.Windows.Forms.ColumnHeader WsTypeClm;
        private System.Windows.Forms.ColumnHeader WsFiletypeClm;
	}
}
