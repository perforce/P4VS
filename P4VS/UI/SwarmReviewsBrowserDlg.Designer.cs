namespace Perforce.P4VS
{
	partial class SwarmReviewsBrowserDlg
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
			this.swarmReviewsToolWindowControl1 = new Perforce.P4VS.SwarmReviewsToolWindowControl();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.gridPanel1 = new Perforce.I18nControls.GridPanel();
			this.OKBtn = new Perforce.I18nControls.GridButton();
			this.CancelBtn = new Perforce.I18nControls.GridButton();
			this.gridLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// swarmReviewsToolWindowControl1
			// 
			this.swarmReviewsToolWindowControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.swarmReviewsToolWindowControl1.BackColor = System.Drawing.SystemColors.Window;
			this.swarmReviewsToolWindowControl1.Location = new System.Drawing.Point(0, -2);
			this.swarmReviewsToolWindowControl1.Name = "swarmReviewsToolWindowControl1";
			this.swarmReviewsToolWindowControl1.Scm = null;
			this.swarmReviewsToolWindowControl1.Size = new System.Drawing.Size(284, 232);
			this.swarmReviewsToolWindowControl1.TabIndex = 0;
			// 
			// gridLayoutPanel1
			// 
			this.gridLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
			this.gridLayoutPanel1.Controls.Add(this.OKBtn);
			this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
			this.gridLayoutPanel1.EnableDesignerGrid = false;
			this.gridLayoutPanel1.EnableDesignerLayout = false;
			this.gridLayoutPanel1.EnableParentResize = false;
			this.gridLayoutPanel1.Location = new System.Drawing.Point(0, 236);
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			this.gridLayoutPanel1.Size = new System.Drawing.Size(284, 29);
			this.gridLayoutPanel1.TabIndex = 1;
			// 
			// gridPanel1
			// 
			this.gridPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.gridPanel1.CellHeight = 0;
			this.gridPanel1.CellWidth = 0;
			this.gridPanel1.Column = 0;
			this.gridPanel1.ColumnsSpanned = 0;
			this.gridPanel1.Location = new System.Drawing.Point(0, 0);
			this.gridPanel1.Name = "gridPanel1";
			this.gridPanel1.Row = 0;
			this.gridPanel1.RowsSpanned = 0;
			this.gridPanel1.Size = new System.Drawing.Size(110, 20);
			this.gridPanel1.TabIndex = 2;
			this.gridPanel1.YOffset = 0;
			// 
			// OKBtn
			// 
			this.OKBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.OKBtn.CellHeight = 0;
			this.OKBtn.CellWidth = 0;
			this.OKBtn.Column = 1;
			this.OKBtn.ColumnsSpanned = 0;
			this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKBtn.Location = new System.Drawing.Point(116, 0);
			this.OKBtn.Name = "OKBtn";
			this.OKBtn.Row = 0;
			this.OKBtn.RowsSpanned = 0;
			this.OKBtn.Size = new System.Drawing.Size(75, 23);
			this.OKBtn.TabIndex = 1;
			this.OKBtn.Text = "OK";
			this.OKBtn.UseVisualStyleBackColor = true;
			this.OKBtn.YOffset = 0;
			// 
			// CancelBtn
			// 
			this.CancelBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.CancelBtn.CellHeight = 0;
			this.CancelBtn.CellWidth = 0;
			this.CancelBtn.Column = 2;
			this.CancelBtn.ColumnsSpanned = 0;
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(197, 0);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Row = 0;
			this.CancelBtn.RowsSpanned = 0;
			this.CancelBtn.Size = new System.Drawing.Size(75, 23);
			this.CancelBtn.TabIndex = 0;
			this.CancelBtn.Text = "Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.YOffset = 0;
			// 
			// SwarmReviewsBrowserDlg
			// 
			this.AcceptButton = this.OKBtn;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.gridLayoutPanel1);
			this.Controls.Add(this.swarmReviewsToolWindowControl1);
			this.Name = "SwarmReviewsBrowserDlg";
			this.Text = "SwarmReviewsBrowserDlg";
			this.gridLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SwarmReviewsToolWindowControl swarmReviewsToolWindowControl1;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridPanel gridPanel1;
		private I18nControls.GridButton OKBtn;
		private I18nControls.GridButton CancelBtn;
	}
}