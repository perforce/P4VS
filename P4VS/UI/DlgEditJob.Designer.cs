namespace Perforce.P4VS.UI
{
    partial class DlgEditJob
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgEditJob));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.fixesLV = new Perforce.P4VS.P4ObjectTreeListView();
            this.changelist = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.workspace = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dateSubmitted = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.submittedBy = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.accessType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fixesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeFixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diffAgainstPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixesImageList = new System.Windows.Forms.ImageList(this.components);
            this.cancelBtn = new Perforce.I18nControls.GridButton();
            this.jobLbl = new System.Windows.Forms.Label();
            this.messageLbl = new Perforce.I18nControls.GridLabel();
            this.browseBtn = new Perforce.I18nControls.GridButton();
            this.addBtn = new Perforce.I18nControls.GridButton();
            this.changelistTB = new Perforce.I18nControls.GridTextBox();
            this.changelistLbl = new Perforce.I18nControls.GridLabel();
            this.OKBtn = new Perforce.I18nControls.GridButton();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.fixesContextMenuStrip.SuspendLayout();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            resources.ApplyResources(this.splitContainer, "splitContainer");
            this.splitContainer.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer.ForeColor = System.Drawing.Color.Red;
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            resources.ApplyResources(this.splitContainer.Panel1, "splitContainer.Panel1");
            this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer.Panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.fixesLV);
            // 
            // fixesLV
            // 
            this.fixesLV._maxLineOffset = 0;
            this.fixesLV.ActionColumn = -1;
            this.fixesLV.AllowColumnReorder = true;
            this.fixesLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.changelist,
            this.workspace,
            this.dateSubmitted,
            this.submittedBy,
            this.accessType,
            this.description});
            this.fixesLV.ContextMenuStrip = this.fixesContextMenuStrip;
            resources.ApplyResources(this.fixesLV, "fixesLV");
            this.fixesLV.EnableIconOverlays = true;
            this.fixesLV.EnableSorting = true;
            this.fixesLV.FullRowSelect = true;
            this.fixesLV.MultiSelect = false;
            this.fixesLV.MultiSelectConditions = Perforce.P4VS.TreeListView.MultiSelectCondition.none;
            this.fixesLV.Name = "fixesLV";
            this.fixesLV.OverlayOffset = 0;
            this.fixesLV.RootCheckBoxes = true;
            this.fixesLV.ScrollPosition = 0;
            this.fixesLV.TreeView = true;
            this.fixesLV.UseClassicImageList = false;
            this.fixesLV.UseCompatibleStateImageBehavior = false;
            this.fixesLV.View = System.Windows.Forms.View.Details;
            this.fixesLV.BeforeExpand += new Perforce.P4VS.TreeListViewEvent(this.fixesLV_BeforeExpand);
            this.fixesLV.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.fixesLV_ColumnClick);
            this.fixesLV.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.fixesLV_ColumnReordered);
            this.fixesLV.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.fixesLV_ColumnWidthChanged);
            this.fixesLV.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.fixesLV_ColumnWidthChanging);
            // 
            // changelist
            // 
            resources.ApplyResources(this.changelist, "changelist");
            // 
            // workspace
            // 
            resources.ApplyResources(this.workspace, "workspace");
            // 
            // dateSubmitted
            // 
            resources.ApplyResources(this.dateSubmitted, "dateSubmitted");
            // 
            // submittedBy
            // 
            resources.ApplyResources(this.submittedBy, "submittedBy");
            // 
            // accessType
            // 
            resources.ApplyResources(this.accessType, "accessType");
            // 
            // description
            // 
            resources.ApplyResources(this.description, "description");
            // 
            // fixesContextMenuStrip
            // 
            this.fixesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeFixToolStripMenuItem,
            this.diffAgainstPreviousToolStripMenuItem});
            this.fixesContextMenuStrip.Name = "fixesContextMenuStrip";
            resources.ApplyResources(this.fixesContextMenuStrip, "fixesContextMenuStrip");
            this.fixesContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.fixesContextMenuStrip_Opening);
            // 
            // removeFixToolStripMenuItem
            // 
            this.removeFixToolStripMenuItem.Name = "removeFixToolStripMenuItem";
            resources.ApplyResources(this.removeFixToolStripMenuItem, "removeFixToolStripMenuItem");
            this.removeFixToolStripMenuItem.Click += new System.EventHandler(this.removeFixToolStripMenuItem_Click);
            // 
            // diffAgainstPreviousToolStripMenuItem
            // 
            this.diffAgainstPreviousToolStripMenuItem.Name = "diffAgainstPreviousToolStripMenuItem";
            resources.ApplyResources(this.diffAgainstPreviousToolStripMenuItem, "diffAgainstPreviousToolStripMenuItem");
            this.diffAgainstPreviousToolStripMenuItem.Click += new System.EventHandler(this.diffAgainstPreviousToolStripMenuItem_Click);
            // 
            // fixesImageList
            // 
            this.fixesImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("fixesImageList.ImageStream")));
            this.fixesImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.fixesImageList.Images.SetKeyName(0, "submitted_change_icon.png");
            this.fixesImageList.Images.SetKeyName(1, "revision_icon_add.png");
            this.fixesImageList.Images.SetKeyName(2, "revision_icon_archive.png");
            this.fixesImageList.Images.SetKeyName(3, "revision_icon_branch.png");
            this.fixesImageList.Images.SetKeyName(4, "revision_icon_delete.png");
            this.fixesImageList.Images.SetKeyName(5, "revision_icon_edit.png");
            this.fixesImageList.Images.SetKeyName(6, "revision_icon_integrate.png");
            this.fixesImageList.Images.SetKeyName(7, "revision_icon_moveadd.png");
            this.fixesImageList.Images.SetKeyName(8, "revision_icon_movedelete.png");
            this.fixesImageList.Images.SetKeyName(9, "revision_icon_purge.png");
            // 
            // cancelBtn
            // 
            resources.ApplyResources(this.cancelBtn, "cancelBtn");
            this.cancelBtn.CellHeight = 34;
            this.cancelBtn.CellWidth = 81;
            this.cancelBtn.Column = 4;
            this.cancelBtn.ColumnsSpanned = 0;
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Row = 1;
            this.cancelBtn.RowsSpanned = 0;
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.YOffset = 0;
            // 
            // jobLbl
            // 
            resources.ApplyResources(this.jobLbl, "jobLbl");
            this.jobLbl.Name = "jobLbl";
            // 
            // messageLbl
            // 
            resources.ApplyResources(this.messageLbl, "messageLbl");
            this.messageLbl.CellHeight = 34;
            this.messageLbl.CellWidth = 65;
            this.messageLbl.Column = 0;
            this.messageLbl.ColumnsSpanned = 0;
            this.messageLbl.ForeColor = System.Drawing.Color.Red;
            this.messageLbl.Name = "messageLbl";
            this.messageLbl.Row = 1;
            this.messageLbl.RowsSpanned = 0;
            this.messageLbl.YOffset = 5;
            // 
            // browseBtn
            // 
            resources.ApplyResources(this.browseBtn, "browseBtn");
            this.browseBtn.CellHeight = 30;
            this.browseBtn.CellWidth = 81;
            this.browseBtn.Column = 3;
            this.browseBtn.ColumnsSpanned = 0;
            this.browseBtn.Name = "browseBtn";
            this.browseBtn.Row = 0;
            this.browseBtn.RowsSpanned = 0;
            this.browseBtn.UseVisualStyleBackColor = true;
            this.browseBtn.YOffset = 0;
            this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
            // 
            // addBtn
            // 
            resources.ApplyResources(this.addBtn, "addBtn");
            this.addBtn.CellHeight = 30;
            this.addBtn.CellWidth = 81;
            this.addBtn.Column = 2;
            this.addBtn.ColumnsSpanned = 0;
            this.addBtn.Name = "addBtn";
            this.addBtn.Row = 0;
            this.addBtn.RowsSpanned = 0;
            this.addBtn.UseVisualStyleBackColor = true;
            this.addBtn.YOffset = 0;
            this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
            // 
            // changelistTB
            // 
            resources.ApplyResources(this.changelistTB, "changelistTB");
            this.changelistTB.CellHeight = 30;
            this.changelistTB.CellWidth = 180;
            this.changelistTB.Column = 1;
            this.changelistTB.ColumnsSpanned = 0;
            this.changelistTB.Name = "changelistTB";
            this.changelistTB.Row = 0;
            this.changelistTB.RowsSpanned = 0;
            this.changelistTB.YOffset = 2;
            this.changelistTB.TextChanged += new System.EventHandler(this.changelistTB_TextChanged);
            this.changelistTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.changelistTB_KeyPress);
            // 
            // changelistLbl
            // 
            resources.ApplyResources(this.changelistLbl, "changelistLbl");
            this.changelistLbl.CellHeight = 30;
            this.changelistLbl.CellWidth = 65;
            this.changelistLbl.Column = 0;
            this.changelistLbl.ColumnsSpanned = 0;
            this.changelistLbl.Name = "changelistLbl";
            this.changelistLbl.Row = 0;
            this.changelistLbl.RowsSpanned = 0;
            this.changelistLbl.YOffset = 5;
            // 
            // OKBtn
            // 
            resources.ApplyResources(this.OKBtn, "OKBtn");
            this.OKBtn.CellHeight = 34;
            this.OKBtn.CellWidth = 81;
            this.OKBtn.Column = 3;
            this.OKBtn.ColumnsSpanned = 0;
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Row = 1;
            this.OKBtn.RowsSpanned = 0;
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.YOffset = 0;
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.browseBtn);
            this.gridLayoutPanel1.Controls.Add(this.addBtn);
            this.gridLayoutPanel1.Controls.Add(this.messageLbl);
            this.gridLayoutPanel1.Controls.Add(this.changelistTB);
            this.gridLayoutPanel1.Controls.Add(this.changelistLbl);
            this.gridLayoutPanel1.Controls.Add(this.cancelBtn);
            this.gridLayoutPanel1.Controls.Add(this.OKBtn);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // DlgEditJob
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.Controls.Add(this.jobLbl);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.gridLayoutPanel1);
            this.Name = "DlgEditJob";
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.fixesContextMenuStrip.ResumeLayout(false);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label jobLbl;
		private I18nControls.GridButton OKBtn;
		private I18nControls.GridButton cancelBtn;
        private P4ObjectTreeListView fixesLV;
        private System.Windows.Forms.ColumnHeader changelist;
        private System.Windows.Forms.ColumnHeader workspace;
        private System.Windows.Forms.ColumnHeader dateSubmitted;
        private System.Windows.Forms.ColumnHeader submittedBy;
        private System.Windows.Forms.ColumnHeader accessType;
        private System.Windows.Forms.ColumnHeader description;
		private System.Windows.Forms.ImageList fixesImageList;
		private I18nControls.GridButton addBtn;
		private I18nControls.GridButton browseBtn;
		private I18nControls.GridLabel messageLbl;
        private System.Windows.Forms.ContextMenuStrip fixesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeFixToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diffAgainstPreviousToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridLabel changelistLbl;
		private I18nControls.GridTextBox changelistTB;
    }
}