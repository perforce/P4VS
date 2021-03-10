

namespace Perforce.P4VS
{
	partial class DepotPathDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DepotPathDlg));
            this.mDepotTreeView = new System.Windows.Forms.TreeView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.getLatestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSolutionPathTxt = new System.Windows.Forms.TextBox();
            this.mOkBtn = new Perforce.I18nControls.GridButton();
            this.mCancelBtn = new Perforce.I18nControls.GridButton();
            this.OpenConnectionBtn = new Perforce.I18nControls.GridButton();
            this.DepotTreeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.connectionLbl = new System.Windows.Forms.Label();
            this.filterCB = new System.Windows.Forms.CheckBox();
            this.gridControl1 = new Perforce.I18nControls.GridLayoutPanel();
            this.envCB = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip.SuspendLayout();
            this.gridControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mDepotTreeView
            // 
            resources.ApplyResources(this.mDepotTreeView, "mDepotTreeView");
            this.mDepotTreeView.ContextMenuStrip = this.contextMenuStrip;
            this.mDepotTreeView.Name = "mDepotTreeView";
            this.mDepotTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("mDepotTreeView.Nodes")))});
            this.mDepotTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.mDepotTreeView_BeforeExpand);
            this.mDepotTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.mDepotTreeView_AfterSelect);
            this.mDepotTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.mDepotTreeView_NodeMouseClick);
            this.mDepotTreeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.mDepotTreeView_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getLatestToolStripMenuItem,
            this.removeFromToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // getLatestToolStripMenuItem
            // 
            this.getLatestToolStripMenuItem.Name = "getLatestToolStripMenuItem";
            resources.ApplyResources(this.getLatestToolStripMenuItem, "getLatestToolStripMenuItem");
            this.getLatestToolStripMenuItem.Click += new System.EventHandler(this.getLatestToolStripMenuItem_Click);
            // 
            // removeFromToolStripMenuItem
            // 
            this.removeFromToolStripMenuItem.Name = "removeFromToolStripMenuItem";
            resources.ApplyResources(this.removeFromToolStripMenuItem, "removeFromToolStripMenuItem");
            this.removeFromToolStripMenuItem.Click += new System.EventHandler(this.removeFromToolStripMenuItem_Click);
            // 
            // mSolutionPathTxt
            // 
            resources.ApplyResources(this.mSolutionPathTxt, "mSolutionPathTxt");
            this.mSolutionPathTxt.Name = "mSolutionPathTxt";
            this.mSolutionPathTxt.TextChanged += new System.EventHandler(this.mSolutionPathTxt_TextChanged);
            // 
            // mOkBtn
            // 
            resources.ApplyResources(this.mOkBtn, "mOkBtn");
            this.mOkBtn.CellHeight = 29;
            this.mOkBtn.CellWidth = 81;
            this.mOkBtn.Column = 1;
            this.mOkBtn.ColumnsSpanned = 0;
            this.mOkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOkBtn.Name = "mOkBtn";
            this.mOkBtn.Row = 0;
            this.mOkBtn.RowsSpanned = 0;
            this.mOkBtn.UseVisualStyleBackColor = true;
            this.mOkBtn.YOffset = 0;
            this.mOkBtn.Click += new System.EventHandler(this.mOkBtn_Click);
            // 
            // mCancelBtn
            // 
            resources.ApplyResources(this.mCancelBtn, "mCancelBtn");
            this.mCancelBtn.CellHeight = 29;
            this.mCancelBtn.CellWidth = 81;
            this.mCancelBtn.Column = 2;
            this.mCancelBtn.ColumnsSpanned = 0;
            this.mCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelBtn.Name = "mCancelBtn";
            this.mCancelBtn.Row = 0;
            this.mCancelBtn.RowsSpanned = 0;
            this.mCancelBtn.UseVisualStyleBackColor = true;
            this.mCancelBtn.YOffset = 0;
            // 
            // OpenConnectionBtn
            // 
            resources.ApplyResources(this.OpenConnectionBtn, "OpenConnectionBtn");
            this.OpenConnectionBtn.CellHeight = 29;
            this.OpenConnectionBtn.CellWidth = 124;
            this.OpenConnectionBtn.Column = 0;
            this.OpenConnectionBtn.ColumnsSpanned = 0;
            this.OpenConnectionBtn.Name = "OpenConnectionBtn";
            this.OpenConnectionBtn.Row = 0;
            this.OpenConnectionBtn.RowsSpanned = 0;
            this.OpenConnectionBtn.UseVisualStyleBackColor = true;
            this.OpenConnectionBtn.YOffset = 0;
            this.OpenConnectionBtn.Click += new System.EventHandler(this.OpenConnectionBtn_Click);
            // 
            // DepotTreeViewImageList
            // 
            this.DepotTreeViewImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.DepotTreeViewImageList, "DepotTreeViewImageList");
            this.DepotTreeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // connectionLbl
            // 
            resources.ApplyResources(this.connectionLbl, "connectionLbl");
            this.connectionLbl.Name = "connectionLbl";
            // 
            // filterCB
            // 
            resources.ApplyResources(this.filterCB, "filterCB");
            this.filterCB.Name = "filterCB";
            this.filterCB.UseVisualStyleBackColor = true;
            this.filterCB.CheckedChanged += new System.EventHandler(this.filterCB_CheckedChanged);
            // 
            // gridControl1
            // 
            resources.ApplyResources(this.gridControl1, "gridControl1");
            this.gridControl1.Controls.Add(this.mCancelBtn);
            this.gridControl1.Controls.Add(this.mOkBtn);
            this.gridControl1.Controls.Add(this.OpenConnectionBtn);
            this.gridControl1.EnableDesignerGrid = false;
            this.gridControl1.EnableDesignerLayout = false;
            this.gridControl1.EnableParentResize = false;
            this.gridControl1.MinimumColumnWidth = 10;
            this.gridControl1.MinimumRowHeight = 10;
            this.gridControl1.Name = "gridControl1";
            // 
            // envCB
            // 
            resources.ApplyResources(this.envCB, "envCB");
            this.envCB.Name = "envCB";
            this.envCB.UseVisualStyleBackColor = true;
            this.envCB.CheckedChanged += new System.EventHandler(this.envCB_CheckedChanged);
            // 
            // DepotPathDlg
            // 
            this.AcceptButton = this.mOkBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelBtn;
            this.Controls.Add(this.envCB);
            this.Controls.Add(this.filterCB);
            this.Controls.Add(this.connectionLbl);
            this.Controls.Add(this.mSolutionPathTxt);
            this.Controls.Add(this.mDepotTreeView);
            this.Controls.Add(this.gridControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DepotPathDlg";
            this.contextMenuStrip.ResumeLayout(false);
            this.gridControl1.ResumeLayout(false);
            this.gridControl1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView mDepotTreeView;
		private System.Windows.Forms.TextBox mSolutionPathTxt;
		private Perforce.I18nControls.GridButton mOkBtn;
		private Perforce.I18nControls.GridButton mCancelBtn;
		private Perforce.I18nControls.GridButton OpenConnectionBtn;
		private System.Windows.Forms.ImageList DepotTreeViewImageList;
        private System.Windows.Forms.Label connectionLbl;
        private System.Windows.Forms.CheckBox filterCB;
		private I18nControls.GridLayoutPanel gridControl1;
        private System.Windows.Forms.CheckBox envCB;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem getLatestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeFromToolStripMenuItem;
	}
}
