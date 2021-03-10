namespace Perforce.I18nControls
{
	partial class LabeledControl
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
			this.mLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// mLabel
			// 
			this.mLabel.AutoSize = true;
			this.mLabel.Location = new System.Drawing.Point(0, 3);
			this.mLabel.Name = "mLabel";
			this.mLabel.Size = new System.Drawing.Size(0, 13);
			this.mLabel.TabIndex = 0;
			this.mLabel.SizeChanged += new System.EventHandler(this.mLabel_SizeChanged);
			// 
			// LabeledControl
			// 
			if (this.DesignMode)
			{
				this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			}
			this.Controls.Add(this.mLabel);
			this.Name = "LabeledTextBox";
			this.Size = new System.Drawing.Size(227, 21);
			this.SizeChanged += new System.EventHandler(this.LabeledControl_SizeChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label mLabel;
	}
}
