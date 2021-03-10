namespace Perforce.P4VS
{
	partial class ChooseStringDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseStringDlg));
			this.ItemsLB = new System.Windows.Forms.ListBox();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.OkBtn = new System.Windows.Forms.Button();
			this.PromptLbl = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// ItemsLB
			// 
			resources.ApplyResources(this.ItemsLB, "ItemsLB");
			this.ItemsLB.FormattingEnabled = true;
			this.ItemsLB.Name = "ItemsLB";
			this.ItemsLB.SelectedIndexChanged += new System.EventHandler(this.ItemsLB_SelectedIndexChanged);
			this.ItemsLB.DoubleClick += new System.EventHandler(this.ItemsLB_DoubleClick);
			// 
			// CancelBtn
			// 
			resources.ApplyResources(this.CancelBtn, "CancelBtn");
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.UseVisualStyleBackColor = true;
			// 
			// OkBtn
			// 
			resources.ApplyResources(this.OkBtn, "OkBtn");
			this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkBtn.Name = "OkBtn";
			this.OkBtn.UseVisualStyleBackColor = true;
			// 
			// PromptLbl
			// 
			resources.ApplyResources(this.PromptLbl, "PromptLbl");
			this.PromptLbl.Name = "PromptLbl";
			// 
			// ChooseStringDlg
			// 
			this.AcceptButton = this.OkBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.Controls.Add(this.PromptLbl);
			this.Controls.Add(this.OkBtn);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.ItemsLB);
			this.HelpButton = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChooseStringDlg";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ChooseDlg_HelpButtonClicked);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox ItemsLB;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.Button OkBtn;
		private System.Windows.Forms.Label PromptLbl;
	}
}
