namespace Perforce.P4VS
{
	partial class KeepAliveDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeepAliveDlg));
			this.CommandLine = new System.Windows.Forms.TextBox();
			this.CancelCommandBtn = new System.Windows.Forms.Button();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.label1 = new System.Windows.Forms.Label();
			this.CancelingCmdLbl = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// CommandLine
			// 
			resources.ApplyResources(this.CommandLine, "CommandLine");
			this.CommandLine.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.CommandLine.Name = "CommandLine";
			this.CommandLine.ReadOnly = true;
			this.CommandLine.TabStop = false;
			// 
			// CancelCommandBtn
			// 
			resources.ApplyResources(this.CancelCommandBtn, "CancelCommandBtn");
			this.CancelCommandBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelCommandBtn.Name = "CancelCommandBtn";
			this.CancelCommandBtn.UseVisualStyleBackColor = true;
			this.CancelCommandBtn.Click += new System.EventHandler(this.button1_Click);
			// 
			// progressBar1
			// 
			resources.ApplyResources(this.progressBar1, "progressBar1");
			this.progressBar1.MarqueeAnimationSpeed = 10;
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// CancelingCmdLbl
			// 
			resources.ApplyResources(this.CancelingCmdLbl, "CancelingCmdLbl");
			this.CancelingCmdLbl.BackColor = System.Drawing.SystemColors.Window;
			this.CancelingCmdLbl.ForeColor = System.Drawing.Color.Red;
			this.CancelingCmdLbl.Name = "CancelingCmdLbl";
			// 
			// KeepAliveDlg
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelCommandBtn;
			this.ControlBox = false;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CancelCommandBtn);
			this.Controls.Add(this.CommandLine);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.CancelingCmdLbl);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "KeepAliveDlg";
			this.Shown += new System.EventHandler(this.KeepAliveDlg_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox CommandLine;
		private System.Windows.Forms.Button CancelCommandBtn;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label CancelingCmdLbl;
	}
}