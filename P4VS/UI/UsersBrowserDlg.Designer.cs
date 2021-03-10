namespace Perforce.P4VS
{
	partial class UsersBrowserDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsersBrowserDlg));
            this.OKBtn = new Perforce.I18nControls.GridButton();
            this.CancelBtn = new Perforce.I18nControls.GridButton();
            this.listView1 = new Perforce.I18nControls.GridDoubleBufferedListView();
            this.userHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.emailHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lastAccessedHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
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
            // listView1
            // 
            this.listView1.AllowColumnReorder = true;
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.CellHeight = 0;
            this.listView1.CellWidth = 0;
            this.listView1.Column = 0;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.userHeader,
            this.emailHeader,
            this.lastAccessedHeader,
            this.nameHeader});
            this.listView1.ColumnsSpanned = 2;
            this.listView1.FullRowSelect = true;
            this.listView1.HideActionsColumn = false;
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.OwnerDraw = true;
            this.listView1.Row = 0;
            this.listView1.RowsSpanned = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.YOffset = 0;
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // userHeader
            // 
            resources.ApplyResources(this.userHeader, "userHeader");
            // 
            // emailHeader
            // 
            resources.ApplyResources(this.emailHeader, "emailHeader");
            // 
            // lastAccessedHeader
            // 
            resources.ApplyResources(this.lastAccessedHeader, "lastAccessedHeader");
            // 
            // nameHeader
            // 
            resources.ApplyResources(this.nameHeader, "nameHeader");
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.listView1);
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
            // UsersBrowserDlg
            // 
            this.AcceptButton = this.OKBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.gridLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UsersBrowserDlg";
            this.gridLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		//private WorkspaceToolWindowControl workspaceToolWindowControl1;
        private I18nControls.GridButton OKBtn;
        private I18nControls.GridButton CancelBtn;
        private I18nControls.GridDoubleBufferedListView listView1;
		private System.Windows.Forms.ColumnHeader userHeader;
		private System.Windows.Forms.ColumnHeader emailHeader;
		private System.Windows.Forms.ColumnHeader lastAccessedHeader;
		private System.Windows.Forms.ColumnHeader nameHeader;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridPanel gridPanel1;
	}
}