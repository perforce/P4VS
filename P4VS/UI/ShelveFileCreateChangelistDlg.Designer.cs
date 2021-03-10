namespace Perforce.P4VS
{
	partial class ShelveFileCreateChangelistDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShelveFileCreateChangelistDlg));
			this.PromptLbl = new Perforce.I18nControls.GridLabel();
			this.label1 = new Perforce.I18nControls.GridLabel();
			this.DescriptionTB = new Perforce.I18nControls.GridTextBox();
			this.CancelBtn = new Perforce.I18nControls.GridButton();
			this.OkBtn = new Perforce.I18nControls.GridButton();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.gridPanel1 = new Perforce.I18nControls.GridPanel();
			this.gridLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// PromptLbl
			// 
			resources.ApplyResources(this.PromptLbl, "PromptLbl");
			this.PromptLbl.AutoEllipsis = true;
			this.PromptLbl.CellHeight = 47;
			this.PromptLbl.CellWidth = 304;
			this.PromptLbl.Column = 0;
			this.PromptLbl.ColumnsSpanned = 2;
			this.PromptLbl.Name = "PromptLbl";
			this.PromptLbl.Row = 0;
			this.PromptLbl.RowsSpanned = 0;
			this.PromptLbl.YOffset = 0;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.CellHeight = 13;
			this.label1.CellWidth = 304;
			this.label1.Column = 0;
			this.label1.ColumnsSpanned = 2;
			this.label1.Name = "label1";
			this.label1.Row = 1;
			this.label1.RowsSpanned = 0;
			this.label1.YOffset = 0;
			// 
			// DescriptionTB
			// 
			resources.ApplyResources(this.DescriptionTB, "DescriptionTB");
			this.DescriptionTB.CellHeight = 87;
			this.DescriptionTB.CellWidth = 304;
			this.DescriptionTB.Column = 0;
			this.DescriptionTB.ColumnsSpanned = 2;
			this.DescriptionTB.Name = "DescriptionTB";
			this.DescriptionTB.Row = 2;
			this.DescriptionTB.RowsSpanned = 0;
			this.DescriptionTB.YOffset = 0;
			this.DescriptionTB.TextChanged += new System.EventHandler(this.DescriptionTB_TextChanged);
			// 
			// CancelBtn
			// 
			resources.ApplyResources(this.CancelBtn, "CancelBtn");
			this.CancelBtn.CellHeight = 29;
			this.CancelBtn.CellWidth = 81;
			this.CancelBtn.Column = 2;
			this.CancelBtn.ColumnsSpanned = 0;
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Row = 3;
			this.CancelBtn.RowsSpanned = 0;
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.YOffset = 0;
			// 
			// OkBtn
			// 
			resources.ApplyResources(this.OkBtn, "OkBtn");
			this.OkBtn.CellHeight = 29;
			this.OkBtn.CellWidth = 81;
			this.OkBtn.Column = 1;
			this.OkBtn.ColumnsSpanned = 0;
			this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkBtn.Name = "OkBtn";
			this.OkBtn.Row = 3;
			this.OkBtn.RowsSpanned = 0;
			this.OkBtn.UseVisualStyleBackColor = true;
			this.OkBtn.YOffset = 0;
			// 
			// gridLayoutPanel1
			// 
			this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
			this.gridLayoutPanel1.Controls.Add(this.OkBtn);
			this.gridLayoutPanel1.Controls.Add(this.DescriptionTB);
			this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
			this.gridLayoutPanel1.Controls.Add(this.PromptLbl);
			this.gridLayoutPanel1.Controls.Add(this.label1);
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
			this.gridPanel1.CellWidth = 142;
			this.gridPanel1.Column = 0;
			this.gridPanel1.ColumnsSpanned = 0;
			this.gridPanel1.Name = "gridPanel1";
			this.gridPanel1.Row = 3;
			this.gridPanel1.RowsSpanned = 0;
			this.gridPanel1.YOffset = 11;
			// 
			// ShelveFileCreateChangelistDlg
			// 
			this.AcceptButton = this.OkBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.Controls.Add(this.gridLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ShelveFileCreateChangelistDlg";
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridPanel gridPanel1;
		private I18nControls.GridLabel PromptLbl;
		private I18nControls.GridLabel label1;
		private I18nControls.GridTextBox DescriptionTB;
		private I18nControls.GridButton CancelBtn;
		private I18nControls.GridButton OkBtn;
	}
}