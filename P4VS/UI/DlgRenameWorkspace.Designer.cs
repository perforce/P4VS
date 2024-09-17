using System.ComponentModel;

namespace Perforce.P4VS
{
	partial class DlgRenameWorkspace
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgRenameWorkspace));
            this.newCancelBtn = new Perforce.I18nControls.GridButton();
            this.renameWorkspaceBtn = new Perforce.I18nControls.GridButton();
            this.ValueTB = new Perforce.I18nControls.GridTextBox();
            this.renameWorkspaceLbl = new Perforce.I18nControls.GridLabel();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.getNewNameLbl = new Perforce.I18nControls.GridLabel();
            this.warningLbl = new Perforce.I18nControls.GridLabel();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // newCancelBtn
            // 
            resources.ApplyResources(this.newCancelBtn, "newCancelBtn");
            this.newCancelBtn.CellHeight = 42;
            this.newCancelBtn.CellWidth = 256;
            this.newCancelBtn.Column = 1;
            this.newCancelBtn.ColumnsSpanned = 0;
            this.newCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.newCancelBtn.Name = "newCancelBtn";
            this.newCancelBtn.Row = 3;
            this.newCancelBtn.RowsSpanned = 0;
            this.newCancelBtn.UseVisualStyleBackColor = true;
            this.newCancelBtn.YOffset = 0;
            // 
            // renameWorkspaceBtn
            // 
            resources.ApplyResources(this.renameWorkspaceBtn, "renameWorkspaceBtn");
            this.renameWorkspaceBtn.CellHeight = 42;
            this.renameWorkspaceBtn.CellWidth = 397;
            this.renameWorkspaceBtn.Column = 0;
            this.renameWorkspaceBtn.ColumnsSpanned = 1;
            this.renameWorkspaceBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.renameWorkspaceBtn.Name = "renameWorkspaceBtn";
            this.renameWorkspaceBtn.Row = 3;
            this.renameWorkspaceBtn.RowsSpanned = 0;
            this.renameWorkspaceBtn.UseVisualStyleBackColor = true;
            this.renameWorkspaceBtn.YOffset = 0;
            // 
            // ValueTB
            // 
            resources.ApplyResources(this.ValueTB, "ValueTB");
            this.ValueTB.CellHeight = 49;
            this.ValueTB.CellWidth = 256;
            this.ValueTB.Column = 1;
            this.ValueTB.ColumnsSpanned = 0;
            this.ValueTB.Name = "ValueTB";
            this.ValueTB.Row = 1;
            this.ValueTB.RowsSpanned = 0;
            this.ValueTB.YOffset = 7;
            this.ValueTB.TextChanged += new System.EventHandler(this.ValueTB_TextChanged);
            this.ValueTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ValueTB_KeyDown);
            // 
            // renameWorkspaceLbl
            // 
            resources.ApplyResources(this.renameWorkspaceLbl, "renameWorkspaceLbl");
            this.renameWorkspaceLbl.CellHeight = 35;
            this.renameWorkspaceLbl.CellWidth = 397;
            this.renameWorkspaceLbl.Column = 0;
            this.renameWorkspaceLbl.ColumnsSpanned = 3;
            this.renameWorkspaceLbl.Name = "renameWorkspaceLbl";
            this.renameWorkspaceLbl.Row = 0;
            this.renameWorkspaceLbl.RowsSpanned = 0;
            this.renameWorkspaceLbl.YOffset = 0;
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.renameWorkspaceLbl);
            this.gridLayoutPanel1.Controls.Add(this.newCancelBtn);
            this.gridLayoutPanel1.Controls.Add(this.renameWorkspaceBtn);
            this.gridLayoutPanel1.Controls.Add(this.getNewNameLbl);
            this.gridLayoutPanel1.Controls.Add(this.ValueTB);
            this.gridLayoutPanel1.Controls.Add(this.warningLbl);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = true;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 15;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // getNewNameLbl
            // 
            resources.ApplyResources(this.getNewNameLbl, "getNewNameLbl");
            this.getNewNameLbl.CellHeight = 49;
            this.getNewNameLbl.CellWidth = 141;
            this.getNewNameLbl.Column = 0;
            this.getNewNameLbl.ColumnsSpanned = 0;
            this.getNewNameLbl.Name = "getNewNameLbl";
            this.getNewNameLbl.Row = 1;
            this.getNewNameLbl.RowsSpanned = 0;
            this.getNewNameLbl.YOffset = 0;
            // 
            // warningLbl
            // 
            resources.ApplyResources(this.warningLbl, "warningLbl");
            this.warningLbl.CellHeight = 49;
            this.warningLbl.CellWidth = 397;
            this.warningLbl.Column = 0;
            this.warningLbl.ColumnsSpanned = 2;
            this.warningLbl.Name = "warningLbl";
            this.warningLbl.Row = 2;
            this.warningLbl.RowsSpanned = 0;
            this.warningLbl.YOffset = 0;
            // 
            // DlgRenameWorkspace
            // 
            this.AcceptButton = this.renameWorkspaceBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.newCancelBtn;
            this.Controls.Add(this.gridLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgRenameWorkspace";
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private Perforce.I18nControls.GridLabel renameWorkspaceLbl;
		private Perforce.I18nControls.GridTextBox ValueTB;
		private Perforce.I18nControls.GridButton renameWorkspaceBtn;
		private Perforce.I18nControls.GridButton newCancelBtn;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridLabel warningLbl;
        private I18nControls.GridLabel getNewNameLbl;
    }
}