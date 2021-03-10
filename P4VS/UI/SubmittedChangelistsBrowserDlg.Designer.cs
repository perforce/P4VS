namespace Perforce.P4VS
{
    partial class SubmittedChangelistsBrowserDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubmittedChangelistsBrowserDlg));
			this.submittedChangelistsToolWindowControl1 = new Perforce.P4VS.SubmittedChangelistsToolWindowControl();
			this.OkBtn = new Perforce.I18nControls.GridButton();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.CancelBtn = new Perforce.I18nControls.GridButton();
			this.gridLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// submittedChangelistsToolWindowControl1
			// 
			resources.ApplyResources(this.submittedChangelistsToolWindowControl1, "submittedChangelistsToolWindowControl1");
			this.submittedChangelistsToolWindowControl1.BackColor = System.Drawing.SystemColors.Menu;
			this.submittedChangelistsToolWindowControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.submittedChangelistsToolWindowControl1.fromBrowser = false;
			this.submittedChangelistsToolWindowControl1.Name = "submittedChangelistsToolWindowControl1";
			this.submittedChangelistsToolWindowControl1.Scm = null;
			this.submittedChangelistsToolWindowControl1.TreeListViewDoubleClicked += new System.Windows.Forms.MouseEventHandler(this.submittedChangelistsToolWindowControl1_TreeListViewDoubleClicked);
			// 
			// OkBtn
			// 
			resources.ApplyResources(this.OkBtn, "OkBtn");
			this.OkBtn.CellHeight = 29;
			this.OkBtn.CellWidth = 81;
			this.OkBtn.Column = 0;
			this.OkBtn.ColumnsSpanned = 0;
			this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkBtn.Name = "OkBtn";
			this.OkBtn.Row = 0;
			this.OkBtn.RowsSpanned = 0;
			this.OkBtn.UseVisualStyleBackColor = true;
			this.OkBtn.YOffset = 0;
			// 
			// gridLayoutPanel1
			// 
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.Controls.Add(this.OkBtn);
			this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
			this.gridLayoutPanel1.EnableDesignerGrid = false;
			this.gridLayoutPanel1.EnableDesignerLayout = false;
			this.gridLayoutPanel1.EnableParentResize = false;
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			// 
			// CancelBtn
			// 
			resources.ApplyResources(this.CancelBtn, "CancelBtn");
			this.CancelBtn.CellHeight = 29;
			this.CancelBtn.CellWidth = 81;
			this.CancelBtn.Column = 1;
			this.CancelBtn.ColumnsSpanned = 0;
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Row = 0;
			this.CancelBtn.RowsSpanned = 0;
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.YOffset = 0;
			// 
			// SubmittedChangelistsBrowserDlg
			// 
			this.AcceptButton = this.OkBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.Controls.Add(this.gridLayoutPanel1);
			this.Controls.Add(this.submittedChangelistsToolWindowControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SubmittedChangelistsBrowserDlg";
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		private SubmittedChangelistsToolWindowControl submittedChangelistsToolWindowControl1;

        #endregion
		private I18nControls.GridButton OkBtn;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridButton CancelBtn;
    }
}