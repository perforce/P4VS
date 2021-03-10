namespace Perforce.P4VS
{
	partial class ChangeOwnerDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeOwnerDlg));
			this.PromptLbl = new Perforce.I18nControls.GridLabel();
			this.newOKBtn = new Perforce.I18nControls.GridButton();
			this.newCancelBtn = new Perforce.I18nControls.GridButton();
			this.userLbl = new Perforce.I18nControls.GridLabel();
			this.workspaceLbl = new Perforce.I18nControls.GridLabel();
			this.userTB = new Perforce.I18nControls.GridTextBox();
			this.workspaceTB = new Perforce.I18nControls.GridTextBox();
			this.userBrowseBtn = new Perforce.I18nControls.GridButton();
			this.workspaceBrowseBtn = new Perforce.I18nControls.GridButton();
			this.gridControl1 = new Perforce.I18nControls.GridLayoutPanel();
			this.gridControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// PromptLbl
			// 
			resources.ApplyResources(this.PromptLbl, "PromptLbl");
			this.PromptLbl.Column = 0;
			this.PromptLbl.ColumnsSpanned = 0;
			this.PromptLbl.Name = "PromptLbl";
			this.PromptLbl.Row = 0;
			this.PromptLbl.RowsSpanned = 0;
			this.PromptLbl.YOffset = 0;
			// 
			// newOKBtn
			// 
			resources.ApplyResources(this.newOKBtn, "newOKBtn");
			this.newOKBtn.Column = 1;
			this.newOKBtn.ColumnsSpanned = 0;
			this.newOKBtn.Name = "newOKBtn";
			this.newOKBtn.Row = 2;
			this.newOKBtn.RowsSpanned = 0;
			this.newOKBtn.UseVisualStyleBackColor = true;
			this.newOKBtn.YOffset = 0;
			this.newOKBtn.Click += new System.EventHandler(this.newOKBtn_Click);
			// 
			// newCancelBtn
			// 
			this.newCancelBtn.Column = 2;
			this.newCancelBtn.ColumnsSpanned = 0;
			this.newCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.newCancelBtn, "newCancelBtn");
			this.newCancelBtn.Name = "newCancelBtn";
			this.newCancelBtn.Row = 2;
			this.newCancelBtn.RowsSpanned = 0;
			this.newCancelBtn.UseVisualStyleBackColor = true;
			this.newCancelBtn.YOffset = 0;
			// 
			// userLbl
			// 
			resources.ApplyResources(this.userLbl, "userLbl");
			this.userLbl.Column = 0;
			this.userLbl.ColumnsSpanned = 0;
			this.userLbl.Name = "userLbl";
			this.userLbl.Row = 0;
			this.userLbl.RowsSpanned = 0;
			this.userLbl.YOffset = 0;
			// 
			// workspaceLbl
			// 
			resources.ApplyResources(this.workspaceLbl, "workspaceLbl");
			this.workspaceLbl.Column = 0;
			this.workspaceLbl.ColumnsSpanned = 0;
			this.workspaceLbl.Name = "workspaceLbl";
			this.workspaceLbl.Row = 1;
			this.workspaceLbl.RowsSpanned = 0;
			this.workspaceLbl.YOffset = 0;
			// 
			// userTB
			// 
			resources.ApplyResources(this.userTB, "userTB");
			this.userTB.Column = 1;
			this.userTB.ColumnsSpanned = 0;
			this.userTB.Name = "userTB";
			this.userTB.Row = 0;
			this.userTB.RowsSpanned = 0;
			this.userTB.YOffset = 0;
			// 
			// workspaceTB
			// 
			resources.ApplyResources(this.workspaceTB, "workspaceTB");
			this.workspaceTB.Column = 1;
			this.workspaceTB.ColumnsSpanned = 0;
			this.workspaceTB.Name = "workspaceTB";
			this.workspaceTB.Row = 1;
			this.workspaceTB.RowsSpanned = 0;
			this.workspaceTB.YOffset = 0;
			// 
			// userBrowseBtn
			// 
			this.userBrowseBtn.Column = 2;
			this.userBrowseBtn.ColumnsSpanned = 0;
			resources.ApplyResources(this.userBrowseBtn, "userBrowseBtn");
			this.userBrowseBtn.Name = "userBrowseBtn";
			this.userBrowseBtn.Row = 0;
			this.userBrowseBtn.RowsSpanned = 0;
			this.userBrowseBtn.UseVisualStyleBackColor = true;
			this.userBrowseBtn.YOffset = 0;
			this.userBrowseBtn.Click += new System.EventHandler(this.userBrowseBtn_Click);
			// 
			// workspaceBrowseBtn
			// 
			this.workspaceBrowseBtn.Column = 2;
			this.workspaceBrowseBtn.ColumnsSpanned = 0;
			resources.ApplyResources(this.workspaceBrowseBtn, "workspaceBrowseBtn");
			this.workspaceBrowseBtn.Name = "workspaceBrowseBtn";
			this.workspaceBrowseBtn.Row = 1;
			this.workspaceBrowseBtn.RowsSpanned = 0;
			this.workspaceBrowseBtn.UseVisualStyleBackColor = true;
			this.workspaceBrowseBtn.YOffset = 0;
			this.workspaceBrowseBtn.Click += new System.EventHandler(this.workspaceBrowseBtn_Click);
			// 
			// gridControl1
			// 
			resources.ApplyResources(this.gridControl1, "gridControl1");
			this.gridControl1.Controls.Add(this.workspaceBrowseBtn);
			this.gridControl1.Controls.Add(this.userBrowseBtn);
			this.gridControl1.Controls.Add(this.workspaceTB);
			this.gridControl1.Controls.Add(this.userTB);
			this.gridControl1.Controls.Add(this.workspaceLbl);
			this.gridControl1.Controls.Add(this.userLbl);
			this.gridControl1.Controls.Add(this.newCancelBtn);
			this.gridControl1.Controls.Add(this.newOKBtn);
			this.gridControl1.Name = "gridControl1";
			// 
			// ChangeOwnerDlg
			// 
			this.AcceptButton = this.newOKBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.newCancelBtn;
			this.Controls.Add(this.PromptLbl);
			this.Controls.Add(this.gridControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChangeOwnerDlg";
			this.gridControl1.ResumeLayout(false);
			this.gridControl1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private I18nControls.GridLabel PromptLbl;
		private I18nControls.GridButton newOKBtn;
		private I18nControls.GridButton newCancelBtn;
		private I18nControls.GridLabel userLbl;
		private I18nControls.GridLabel workspaceLbl;
		private I18nControls.GridTextBox userTB;
		private I18nControls.GridTextBox workspaceTB;
		private I18nControls.GridButton userBrowseBtn;
		private I18nControls.GridButton workspaceBrowseBtn;
		private I18nControls.GridLayoutPanel gridControl1;
	}
}