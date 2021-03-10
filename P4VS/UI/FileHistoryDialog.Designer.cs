namespace Perforce.P4VS
{
	partial class FileHistoryDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileHistoryDialog));
			this.sccHistoryToolWindowControl1 = new Perforce.P4VS.SccHistoryToolWindowControl();
			this.SuspendLayout();
			// 
			// sccHistoryToolWindowControl1
			// 
			this.sccHistoryToolWindowControl1.BackColor = System.Drawing.SystemColors.Window;
			this.sccHistoryToolWindowControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.sccHistoryToolWindowControl1, "sccHistoryToolWindowControl1");
			this.sccHistoryToolWindowControl1.Files = null;
			this.sccHistoryToolWindowControl1.Name = "sccHistoryToolWindowControl1";
			this.sccHistoryToolWindowControl1.Scm = null;
			// 
			// FileHistoryDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.sccHistoryToolWindowControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FileHistoryDialog";
			this.ResumeLayout(false);

		}

		#endregion

		private SccHistoryToolWindowControl sccHistoryToolWindowControl1;

	}
}