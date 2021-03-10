namespace Perforce.P4VS
{
	partial class P4LicenseDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P4LicenseDlg));
            this.OkBtn = new Perforce.I18nControls.GridButton();
            this.ErrorsTB = new Perforce.I18nControls.GridTextBox();
            this.PromptLbl = new Perforce.I18nControls.GridLabel();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OkBtn
            // 
            resources.ApplyResources(this.OkBtn, "OkBtn");
            this.OkBtn.CellHeight = 29;
            this.OkBtn.CellWidth = 81;
            this.OkBtn.Column = 2;
            this.OkBtn.ColumnsSpanned = 0;
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Row = 2;
            this.OkBtn.RowsSpanned = 0;
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.YOffset = 0;
            // 
            // ErrorsTB
            // 
            resources.ApplyResources(this.ErrorsTB, "ErrorsTB");
            this.ErrorsTB.CellHeight = 403;
            this.ErrorsTB.CellWidth = 530;
            this.ErrorsTB.Column = 0;
            this.ErrorsTB.ColumnsSpanned = 3;
            this.ErrorsTB.Name = "ErrorsTB";
            this.ErrorsTB.ReadOnly = true;
            this.ErrorsTB.Row = 1;
            this.ErrorsTB.RowsSpanned = 0;
            this.ErrorsTB.YOffset = 0;
            // 
            // PromptLbl
            // 
            resources.ApplyResources(this.PromptLbl, "PromptLbl");
            this.PromptLbl.CellHeight = 13;
            this.PromptLbl.CellWidth = 53;
            this.PromptLbl.Column = 0;
            this.PromptLbl.ColumnsSpanned = 0;
            this.PromptLbl.Name = "PromptLbl";
            this.PromptLbl.Row = 0;
            this.PromptLbl.RowsSpanned = 0;
            this.PromptLbl.YOffset = 0;
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.ErrorsTB);
            this.gridLayoutPanel1.Controls.Add(this.PromptLbl);
            this.gridLayoutPanel1.Controls.Add(this.OkBtn);
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
            this.gridPanel1.CellHeight = 29;
            this.gridPanel1.CellWidth = 396;
            this.gridPanel1.Column = 1;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 2;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // P4LicenseDlg
            // 
            this.AcceptButton = this.OkBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "P4LicenseDlg";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private I18nControls.GridLabel PromptLbl;
        private I18nControls.GridButton OkBtn;
		private I18nControls.GridTextBox ErrorsTB;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridPanel gridPanel1;
	}
}