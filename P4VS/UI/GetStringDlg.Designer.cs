namespace Perforce.P4VS
{
	partial class GetStringDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetStringDlg));
            this.newCancelBtn = new Perforce.I18nControls.GridButton();
            this.newOKBtn = new Perforce.I18nControls.GridButton();
            this.ValueTB = new Perforce.I18nControls.GridTextBox();
            this.PromptLbl = new Perforce.I18nControls.GridLabel();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // newCancelBtn
            // 
            resources.ApplyResources(this.newCancelBtn, "newCancelBtn");
            this.newCancelBtn.CellHeight = 29;
            this.newCancelBtn.CellWidth = 81;
            this.newCancelBtn.Column = 2;
            this.newCancelBtn.ColumnsSpanned = 0;
            this.newCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.newCancelBtn.Name = "newCancelBtn";
            this.newCancelBtn.Row = 2;
            this.newCancelBtn.RowsSpanned = 0;
            this.newCancelBtn.UseVisualStyleBackColor = true;
            this.newCancelBtn.YOffset = 0;
            // 
            // newOKBtn
            // 
            resources.ApplyResources(this.newOKBtn, "newOKBtn");
            this.newOKBtn.CellHeight = 29;
            this.newOKBtn.CellWidth = 81;
            this.newOKBtn.Column = 1;
            this.newOKBtn.ColumnsSpanned = 0;
            this.newOKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.newOKBtn.Name = "newOKBtn";
            this.newOKBtn.Row = 2;
            this.newOKBtn.RowsSpanned = 0;
            this.newOKBtn.UseVisualStyleBackColor = true;
            this.newOKBtn.YOffset = 0;
            // 
            // ValueTB
            // 
            resources.ApplyResources(this.ValueTB, "ValueTB");
            this.ValueTB.CellHeight = 26;
            this.ValueTB.CellWidth = 193;
            this.ValueTB.Column = 0;
            this.ValueTB.ColumnsSpanned = 2;
            this.ValueTB.Name = "ValueTB";
            this.ValueTB.Row = 1;
            this.ValueTB.RowsSpanned = 0;
            this.ValueTB.YOffset = 0;
            this.ValueTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ValueTB_KeyDown);
            // 
            // PromptLbl
            // 
            resources.ApplyResources(this.PromptLbl, "PromptLbl");
            this.PromptLbl.CellHeight = 13;
            this.PromptLbl.CellWidth = 193;
            this.PromptLbl.Column = 0;
            this.PromptLbl.ColumnsSpanned = 2;
            this.PromptLbl.Name = "PromptLbl";
            this.PromptLbl.Row = 0;
            this.PromptLbl.RowsSpanned = 0;
            this.PromptLbl.YOffset = 0;
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.newCancelBtn);
            this.gridLayoutPanel1.Controls.Add(this.newOKBtn);
            this.gridLayoutPanel1.Controls.Add(this.ValueTB);
            this.gridLayoutPanel1.Controls.Add(this.PromptLbl);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = true;
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
            this.gridPanel1.CellWidth = 31;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 2;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // GetStringDlg
            // 
            this.AcceptButton = this.newOKBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.newCancelBtn;
            this.Controls.Add(this.gridLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GetStringDlg";
            this.Load += new System.EventHandler(this.GetStringDlg_Load);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private Perforce.I18nControls.GridLabel PromptLbl;
		private Perforce.I18nControls.GridTextBox ValueTB;
		private Perforce.I18nControls.GridButton newOKBtn;
		private Perforce.I18nControls.GridButton newCancelBtn;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridPanel gridPanel1;
	}
}