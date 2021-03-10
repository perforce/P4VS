namespace Perforce.P4VS
{
	partial class PromptNewIgnoreFileDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PromptNewIgnoreFileDlg));
			this.label1 = new Perforce.I18nControls.GridLabel();
			this.label2 = new Perforce.I18nControls.GridLabel();
			this.BugMeNotChk = new Perforce.I18nControls.GridCheckBox();
			this.CancelBtn = new Perforce.I18nControls.GridButton();
			this.OkBtn = new Perforce.I18nControls.GridButton();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.gridPanel2 = new Perforce.I18nControls.GridPanel();
			this.gridPanel1 = new Perforce.I18nControls.GridPanel();
			this.gridLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Column = 0;
			this.label1.ColumnsSpanned = 2;
			this.label1.Name = "label1";
			this.label1.Row = 0;
			this.label1.RowsSpanned = 0;
			this.label1.YOffset = 0;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Column = 0;
			this.label2.ColumnsSpanned = 2;
			this.label2.Name = "label2";
			this.label2.Row = 1;
			this.label2.RowsSpanned = 0;
			this.label2.YOffset = 0;
			// 
			// BugMeNotChk
			// 
			resources.ApplyResources(this.BugMeNotChk, "BugMeNotChk");
			this.BugMeNotChk.Column = 0;
			this.BugMeNotChk.ColumnsSpanned = 2;
			this.BugMeNotChk.Name = "BugMeNotChk";
			this.BugMeNotChk.Row = 3;
			this.BugMeNotChk.RowsSpanned = 0;
			this.BugMeNotChk.UseVisualStyleBackColor = true;
			this.BugMeNotChk.YOffset = 0;
			// 
			// CancelBtn
			// 
			resources.ApplyResources(this.CancelBtn, "CancelBtn");
			this.CancelBtn.Column = 1;
			this.CancelBtn.ColumnsSpanned = 0;
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Row = 4;
			this.CancelBtn.RowsSpanned = 0;
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.YOffset = 5;
			// 
			// OkBtn
			// 
			resources.ApplyResources(this.OkBtn, "OkBtn");
			this.OkBtn.Column = 2;
			this.OkBtn.ColumnsSpanned = 0;
			this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkBtn.Name = "OkBtn";
			this.OkBtn.Row = 4;
			this.OkBtn.RowsSpanned = 0;
			this.OkBtn.UseVisualStyleBackColor = true;
			this.OkBtn.YOffset = 5;
			this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
			// 
			// gridLayoutPanel1
			// 
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.Controls.Add(this.BugMeNotChk);
			this.gridLayoutPanel1.Controls.Add(this.gridPanel2);
			this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
			this.gridLayoutPanel1.Controls.Add(this.OkBtn);
			this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
			this.gridLayoutPanel1.Controls.Add(this.label2);
			this.gridLayoutPanel1.Controls.Add(this.label1);
			this.gridLayoutPanel1.EnableDesignerGrid = false;
			this.gridLayoutPanel1.EnableDesignerLayout = true;
			this.gridLayoutPanel1.EnableParentResize = true;
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			// 
			// gridPanel2
			// 
			resources.ApplyResources(this.gridPanel2, "gridPanel2");
			this.gridPanel2.Column = 0;
			this.gridPanel2.ColumnsSpanned = 0;
			this.gridPanel2.Name = "gridPanel2";
			this.gridPanel2.Row = 2;
			this.gridPanel2.RowsSpanned = 0;
			this.gridPanel2.YOffset = 0;
			// 
			// gridPanel1
			// 
			resources.ApplyResources(this.gridPanel1, "gridPanel1");
			this.gridPanel1.Column = 0;
			this.gridPanel1.ColumnsSpanned = 0;
			this.gridPanel1.Name = "gridPanel1";
			this.gridPanel1.Row = 4;
			this.gridPanel1.RowsSpanned = 0;
			this.gridPanel1.YOffset = 0;
			// 
			// PromptNewIgnoreFileDlg
			// 
			this.AcceptButton = this.OkBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.Controls.Add(this.gridLayoutPanel1);
			this.Name = "PromptNewIgnoreFileDlg";
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private I18nControls.GridLabel label1;
		private I18nControls.GridLabel label2;
		private I18nControls.GridCheckBox BugMeNotChk;
		private I18nControls.GridButton CancelBtn;
		private I18nControls.GridButton OkBtn;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridPanel gridPanel2;
		private I18nControls.GridPanel gridPanel1;
	}
}