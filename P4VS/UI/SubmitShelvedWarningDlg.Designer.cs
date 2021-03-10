namespace Perforce.P4VS
{
	partial class SubmitShelvedWarningDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubmitShelvedWarningDlg));
            this.CancelBtn = new Perforce.I18nControls.GridButton();
            this.PromptLbl = new Perforce.I18nControls.GridLabel();
            this.revertFilestBtn = new Perforce.I18nControls.GridButton();
            this.moveFilestBtn = new Perforce.I18nControls.GridButton();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.CellHeight = 29;
            this.CancelBtn.CellWidth = 101;
            this.CancelBtn.Column = 3;
            this.CancelBtn.ColumnsSpanned = 0;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Row = 1;
            this.CancelBtn.RowsSpanned = 0;
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.YOffset = 0;
            // 
            // PromptLbl
            // 
            resources.ApplyResources(this.PromptLbl, "PromptLbl");
            this.PromptLbl.AutoEllipsis = true;
            this.PromptLbl.CellHeight = 42;
            this.PromptLbl.CellWidth = 445;
            this.PromptLbl.Column = 0;
            this.PromptLbl.ColumnsSpanned = 3;
            this.PromptLbl.Name = "PromptLbl";
            this.PromptLbl.Row = 0;
            this.PromptLbl.RowsSpanned = 0;
            this.PromptLbl.YOffset = 0;
            // 
            // revertFilestBtn
            // 
            resources.ApplyResources(this.revertFilestBtn, "revertFilestBtn");
            this.revertFilestBtn.CellHeight = 29;
            this.revertFilestBtn.CellWidth = 101;
            this.revertFilestBtn.Column = 2;
            this.revertFilestBtn.ColumnsSpanned = 0;
            this.revertFilestBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.revertFilestBtn.Name = "revertFilestBtn";
            this.revertFilestBtn.Row = 1;
            this.revertFilestBtn.RowsSpanned = 0;
            this.revertFilestBtn.UseVisualStyleBackColor = true;
            this.revertFilestBtn.YOffset = 0;
            // 
            // moveFilestBtn
            // 
            resources.ApplyResources(this.moveFilestBtn, "moveFilestBtn");
            this.moveFilestBtn.CellHeight = 29;
            this.moveFilestBtn.CellWidth = 203;
            this.moveFilestBtn.Column = 1;
            this.moveFilestBtn.ColumnsSpanned = 0;
            this.moveFilestBtn.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.moveFilestBtn.Name = "moveFilestBtn";
            this.moveFilestBtn.Row = 1;
            this.moveFilestBtn.RowsSpanned = 0;
            this.moveFilestBtn.UseVisualStyleBackColor = true;
            this.moveFilestBtn.YOffset = 0;
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.moveFilestBtn);
            this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
            this.gridLayoutPanel1.Controls.Add(this.PromptLbl);
            this.gridLayoutPanel1.Controls.Add(this.revertFilestBtn);
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
            this.gridPanel1.CellWidth = 40;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 1;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 2;
            // 
            // SubmitShelvedWarningDlg
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.gridLayoutPanel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubmitShelvedWarningDlg";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private I18nControls.GridLabel PromptLbl;
        private I18nControls.GridButton CancelBtn;
        private I18nControls.GridButton revertFilestBtn;
        private I18nControls.GridButton moveFilestBtn;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridPanel gridPanel1;
	}
}