namespace Perforce.P4VS
{
	partial class GetNewWorkspaceStringDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetNewWorkspaceStringDlg));
			this.PromptLbl = new System.Windows.Forms.Label();
			this.ValueTB = new System.Windows.Forms.TextBox();
			this.newOKBtn = new System.Windows.Forms.Button();
			this.newCancelBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// PromptLbl
			// 
			resources.ApplyResources(this.PromptLbl, "PromptLbl");
			this.PromptLbl.Name = "PromptLbl";
			// 
			// ValueTB
			// 
			resources.ApplyResources(this.ValueTB, "ValueTB");
			this.ValueTB.Name = "ValueTB";
			this.ValueTB.TextChanged += new System.EventHandler(this.ValueTB_TextChanged);
			this.ValueTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ValueTB_KeyDown);
			// 
			// newOKBtn
			// 
			this.newOKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.newOKBtn, "newOKBtn");
			this.newOKBtn.Name = "newOKBtn";
			this.newOKBtn.UseVisualStyleBackColor = true;
			// 
			// newCancelBtn
			// 
			this.newCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.newCancelBtn, "newCancelBtn");
			this.newCancelBtn.Name = "newCancelBtn";
			this.newCancelBtn.UseVisualStyleBackColor = true;
			// 
			// GetNewWorkspaceStringDlg
			// 
			this.AcceptButton = this.newOKBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.newCancelBtn;
			this.Controls.Add(this.newCancelBtn);
			this.Controls.Add(this.newOKBtn);
			this.Controls.Add(this.ValueTB);
			this.Controls.Add(this.PromptLbl);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GetNewWorkspaceStringDlg";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label PromptLbl;
		private System.Windows.Forms.TextBox ValueTB;
		private System.Windows.Forms.Button newOKBtn;
		private System.Windows.Forms.Button newCancelBtn;
	}
}