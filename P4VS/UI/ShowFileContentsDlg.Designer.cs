namespace Perforce.P4VS
{
	partial class ShowFileContentsDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowFileContentsDlg));
			this.FileContents = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// FileContents
			// 
			this.FileContents.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.FileContents, "FileContents");
			this.FileContents.Name = "FileContents";
			this.FileContents.ReadOnly = true;
			// 
			// ShowFileContentsDlg
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.FileContents);
			this.Name = "ShowFileContentsDlg";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShowFileContentsDlg_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox FileContents;
	}
}