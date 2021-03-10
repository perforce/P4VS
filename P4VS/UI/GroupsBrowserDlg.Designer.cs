namespace Perforce.P4VS
{
	partial class GroupsBrowserDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupsBrowserDlg));
            this.OKBtn = new Perforce.I18nControls.GridButton();
            this.CancelBtn = new Perforce.I18nControls.GridButton();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.groupGridP4ObjectTreeListView = new Perforce.I18nControls.GridP4ObjectTreeListView();
            this.groupHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKBtn
            // 
            resources.ApplyResources(this.OKBtn, "OKBtn");
            this.OKBtn.CellHeight = 0;
            this.OKBtn.CellWidth = 0;
            this.OKBtn.Column = 1;
            this.OKBtn.ColumnsSpanned = 0;
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Row = 1;
            this.OKBtn.RowsSpanned = 0;
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.YOffset = 0;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.CellHeight = 0;
            this.CancelBtn.CellWidth = 0;
            this.CancelBtn.Column = 2;
            this.CancelBtn.ColumnsSpanned = 0;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Row = 1;
            this.CancelBtn.RowsSpanned = 0;
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.YOffset = 0;
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.groupGridP4ObjectTreeListView);
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
            this.gridLayoutPanel1.Controls.Add(this.OKBtn);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // groupGridP4ObjectTreeListView
            // 
            this.groupGridP4ObjectTreeListView._maxLineOffset = 0;
            this.groupGridP4ObjectTreeListView.ActionColumn = -1;
            this.groupGridP4ObjectTreeListView.AllowColumnReorder = true;
            resources.ApplyResources(this.groupGridP4ObjectTreeListView, "groupGridP4ObjectTreeListView");
            this.groupGridP4ObjectTreeListView.CellHeight = 0;
            this.groupGridP4ObjectTreeListView.CellWidth = 0;
            this.groupGridP4ObjectTreeListView.Column = 0;
            this.groupGridP4ObjectTreeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.groupHeader});
            this.groupGridP4ObjectTreeListView.ColumnsSpanned = 5;
            this.groupGridP4ObjectTreeListView.EnableIconOverlays = false;
            this.groupGridP4ObjectTreeListView.EnableSorting = true;
            this.groupGridP4ObjectTreeListView.FullRowSelect = true;
            this.groupGridP4ObjectTreeListView.MultiSelect = false;
            this.groupGridP4ObjectTreeListView.MultiSelectConditions = Perforce.P4VS.TreeListView.MultiSelectCondition.none;
            this.groupGridP4ObjectTreeListView.Name = "groupGridP4ObjectTreeListView";
            this.groupGridP4ObjectTreeListView.OverlayOffset = 0;
            this.groupGridP4ObjectTreeListView.RootCheckBoxes = false;
            this.groupGridP4ObjectTreeListView.Row = 0;
            this.groupGridP4ObjectTreeListView.RowsSpanned = 0;
            this.groupGridP4ObjectTreeListView.ScrollPosition = 0;
            this.groupGridP4ObjectTreeListView.TreeView = true;
            this.groupGridP4ObjectTreeListView.UseClassicImageList = false;
            this.groupGridP4ObjectTreeListView.UseCompatibleStateImageBehavior = false;
            this.groupGridP4ObjectTreeListView.View = System.Windows.Forms.View.Details;
            this.groupGridP4ObjectTreeListView.YOffset = 0;
            this.groupGridP4ObjectTreeListView.BeforeExpand += new Perforce.P4VS.TreeListViewEvent(this.groupGridP4ObjectTreeListView_BeforeExpand);
            this.groupGridP4ObjectTreeListView.SelectedIndexChanged += new System.EventHandler(this.groupGridP4ObjectTreeListView_SelectedIndexChanged);
            this.groupGridP4ObjectTreeListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.groupGridP4ObjectTreeListView_MouseDoubleClick);
            // 
            // groupHeader
            // 
            resources.ApplyResources(this.groupHeader, "groupHeader");
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.CellHeight = 0;
            this.gridPanel1.CellWidth = 0;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 1;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // GroupsBrowserDlg
            // 
            this.AcceptButton = this.OKBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.gridLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GroupsBrowserDlg";
            this.gridLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		//private WorkspaceToolWindowControl workspaceToolWindowControl1;
        private I18nControls.GridButton OKBtn;
        private I18nControls.GridButton CancelBtn;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridPanel gridPanel1;
        private I18nControls.GridP4ObjectTreeListView groupGridP4ObjectTreeListView;
        private System.Windows.Forms.ColumnHeader groupHeader;
    }
}