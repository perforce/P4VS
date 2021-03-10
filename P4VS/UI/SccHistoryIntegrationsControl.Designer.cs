namespace Perforce.P4VS
{
	partial class SccHistoryIntegrationsControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SccHistoryIntegrationsControl));
			this.IntegrationsTreeListView = new Perforce.P4VS.TreeListView();
			this.FileColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.RevisionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ActionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// IntegrationsTreeListView
			// 
			this.IntegrationsTreeListView._maxLineOffset = 0;
			this.IntegrationsTreeListView.AllowColumnReorder = true;
			this.IntegrationsTreeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FileColumn,
            this.RevisionClm,
            this.ActionClm});
			resources.ApplyResources(this.IntegrationsTreeListView, "IntegrationsTreeListView");
			this.IntegrationsTreeListView.EnableIconOverlays = false;
			this.IntegrationsTreeListView.Name = "IntegrationsTreeListView";
			this.IntegrationsTreeListView.OverlayOffset = 0;
			this.IntegrationsTreeListView.RootCheckBoxes = false;
			this.IntegrationsTreeListView.ScrollPosition = 0;
			this.IntegrationsTreeListView.TreeView = true;
			this.IntegrationsTreeListView.UseClassicImageList = false;
			this.IntegrationsTreeListView.UseCompatibleStateImageBehavior = false;
			this.IntegrationsTreeListView.View = System.Windows.Forms.View.Details;
			// 
			// FileColumn
			// 
			resources.ApplyResources(this.FileColumn, "FileColumn");
			// 
			// RevisionClm
			// 
			resources.ApplyResources(this.RevisionClm, "RevisionClm");
			// 
			// ActionClm
			// 
			resources.ApplyResources(this.ActionClm, "ActionClm");
			// 
			// SccHistoryIntegrationsControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Controls.Add(this.IntegrationsTreeListView);
			this.MinimumSize = new System.Drawing.Size(430, 130);
			this.Name = "SccHistoryIntegrationsControl";
			this.ResumeLayout(false);

		}

		#endregion

		private TreeListView IntegrationsTreeListView;
		private System.Windows.Forms.ColumnHeader FileColumn;
		private System.Windows.Forms.ColumnHeader RevisionClm;
		private System.Windows.Forms.ColumnHeader ActionClm;
	}
}
