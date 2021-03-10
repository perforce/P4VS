namespace Perforce.P4VS.UI
{
	partial class ThreadMonitorControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreadMonitorControl));
			this.ProgressBar = new System.Windows.Forms.ProgressBar();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ProgressBar
			// 
			resources.ApplyResources(this.ProgressBar, "ProgressBar");
			this.ProgressBar.Name = "ProgressBar";
			this.ProgressBar.Step = 1;
			this.ProgressBar.Value = 50;
			// 
			// CancelBtn
			// 
			resources.ApplyResources(this.CancelBtn, "CancelBtn");
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
			// 
			// ThreadMonitorControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.ProgressBar);
			this.Name = "ThreadMonitorControl";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar ProgressBar;
		private System.Windows.Forms.Button CancelBtn;
	}
}
