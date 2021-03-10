namespace Perforce.P4VS
{
	partial class LabelsBrowserDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LabelsBrowserDlg));
            this.labelsToolWindowControl1 = new Perforce.P4VS.LabelsToolWindowControl();
            this.CanceiBtn = new Perforce.I18nControls.GridButton();
            this.OkBtn = new Perforce.I18nControls.GridButton();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelsToolWindowControl1
            // 
            resources.ApplyResources(this.labelsToolWindowControl1, "labelsToolWindowControl1");
            this.labelsToolWindowControl1.BackColor = System.Drawing.SystemColors.Menu;
            this.labelsToolWindowControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelsToolWindowControl1.Label = null;
            this.labelsToolWindowControl1.Name = "labelsToolWindowControl1";
            this.labelsToolWindowControl1.scm = null;
            this.labelsToolWindowControl1.Scm = null;
            // 
            // CanceiBtn
            // 
            resources.ApplyResources(this.CanceiBtn, "CanceiBtn");
            this.CanceiBtn.Column = 1;
            this.CanceiBtn.ColumnsSpanned = 0;
            this.CanceiBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CanceiBtn.Name = "CanceiBtn";
            this.CanceiBtn.Row = 0;
            this.CanceiBtn.RowsSpanned = 0;
            this.CanceiBtn.UseVisualStyleBackColor = true;
            this.CanceiBtn.YOffset = 0;
            // 
            // OkBtn
            // 
            resources.ApplyResources(this.OkBtn, "OkBtn");
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
            this.gridLayoutPanel1.Controls.Add(this.CanceiBtn);
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // LabelsBrowserDlg
            // 
            this.AcceptButton = this.OkBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CanceiBtn;
            this.Controls.Add(this.labelsToolWindowControl1);
            this.Controls.Add(this.gridLayoutPanel1);
            this.Name = "LabelsBrowserDlg";
            this.gridLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private LabelsToolWindowControl labelsToolWindowControl1;
        private I18nControls.GridButton CanceiBtn;
        private I18nControls.GridButton OkBtn;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
	}
}