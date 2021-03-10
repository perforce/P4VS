namespace Perforce.P4VS
{
	partial class FileAttributesDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileAttributesDlg));
			this.label1 = new System.Windows.Forms.Label();
			this.BaseTypeCB = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.PreserveLocalModTimeCB = new System.Windows.Forms.CheckBox();
			this.AlwaywsWritableCB = new System.Windows.Forms.CheckBox();
			this.ExcecBitSetCB = new System.Windows.Forms.CheckBox();
			this.RcsKeywordExpansionCB = new System.Windows.Forms.CheckBox();
			this.RcsLimitedKeywordExpansionCB = new System.Windows.Forms.CheckBox();
			this.ExclusiveCheckoutsCB = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.TargetChangeListCB = new System.Windows.Forms.ComboBox();
			this.OkBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.AddAtrributesRB = new System.Windows.Forms.RadioButton();
			this.ChangeBaseTypeRB = new System.Windows.Forms.RadioButton();
			this.panel2 = new System.Windows.Forms.Panel();
			this.FullFileRB = new System.Windows.Forms.RadioButton();
			this.RCSDeltasRB = new System.Windows.Forms.RadioButton();
			this.CompressedFilesRB = new System.Windows.Forms.RadioButton();
			this.UseDefaultForTypeRB = new System.Windows.Forms.RadioButton();
			this.divider1GB = new System.Windows.Forms.GroupBox();
			this.divider4GB = new System.Windows.Forms.GroupBox();
			this.divider3GB = new System.Windows.Forms.GroupBox();
			this.divider2GB = new System.Windows.Forms.GroupBox();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.LimitRevsCB = new Perforce.I18nControls.GridCheckBox();
			this.MaxRevsCB = new Perforce.I18nControls.GridComboBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.gridLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// BaseTypeCB
			// 
			this.BaseTypeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.BaseTypeCB.FormattingEnabled = true;
			resources.ApplyResources(this.BaseTypeCB, "BaseTypeCB");
			this.BaseTypeCB.Name = "BaseTypeCB";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// PreserveLocalModTimeCB
			// 
			resources.ApplyResources(this.PreserveLocalModTimeCB, "PreserveLocalModTimeCB");
			this.PreserveLocalModTimeCB.Name = "PreserveLocalModTimeCB";
			this.PreserveLocalModTimeCB.UseVisualStyleBackColor = true;
			// 
			// AlwaywsWritableCB
			// 
			resources.ApplyResources(this.AlwaywsWritableCB, "AlwaywsWritableCB");
			this.AlwaywsWritableCB.Name = "AlwaywsWritableCB";
			this.AlwaywsWritableCB.UseVisualStyleBackColor = true;
			// 
			// ExcecBitSetCB
			// 
			resources.ApplyResources(this.ExcecBitSetCB, "ExcecBitSetCB");
			this.ExcecBitSetCB.Name = "ExcecBitSetCB";
			this.ExcecBitSetCB.UseVisualStyleBackColor = true;
			// 
			// RcsKeywordExpansionCB
			// 
			resources.ApplyResources(this.RcsKeywordExpansionCB, "RcsKeywordExpansionCB");
			this.RcsKeywordExpansionCB.Name = "RcsKeywordExpansionCB";
			this.RcsKeywordExpansionCB.UseVisualStyleBackColor = true;
			this.RcsKeywordExpansionCB.CheckedChanged += new System.EventHandler(this.RcsKeywordExpansionCB_CheckedChanged);
			// 
			// RcsLimitedKeywordExpansionCB
			// 
			resources.ApplyResources(this.RcsLimitedKeywordExpansionCB, "RcsLimitedKeywordExpansionCB");
			this.RcsLimitedKeywordExpansionCB.Name = "RcsLimitedKeywordExpansionCB";
			this.RcsLimitedKeywordExpansionCB.UseVisualStyleBackColor = true;
			this.RcsLimitedKeywordExpansionCB.CheckedChanged += new System.EventHandler(this.RcsLimitedKeywordExpansionCB_CheckedChanged);
			// 
			// ExclusiveCheckoutsCB
			// 
			resources.ApplyResources(this.ExclusiveCheckoutsCB, "ExclusiveCheckoutsCB");
			this.ExclusiveCheckoutsCB.Name = "ExclusiveCheckoutsCB";
			this.ExclusiveCheckoutsCB.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// TargetChangeListCB
			// 
			this.TargetChangeListCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TargetChangeListCB.FormattingEnabled = true;
			resources.ApplyResources(this.TargetChangeListCB, "TargetChangeListCB");
			this.TargetChangeListCB.Name = "TargetChangeListCB";
			// 
			// OkBtn
			// 
			resources.ApplyResources(this.OkBtn, "OkBtn");
			this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkBtn.Name = "OkBtn";
			this.OkBtn.UseVisualStyleBackColor = true;
			// 
			// CancelBtn
			// 
			resources.ApplyResources(this.CancelBtn, "CancelBtn");
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.AddAtrributesRB);
			this.panel1.Controls.Add(this.ChangeBaseTypeRB);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// AddAtrributesRB
			// 
			resources.ApplyResources(this.AddAtrributesRB, "AddAtrributesRB");
			this.AddAtrributesRB.Name = "AddAtrributesRB";
			this.AddAtrributesRB.UseVisualStyleBackColor = true;
			this.AddAtrributesRB.CheckedChanged += new System.EventHandler(this.AddAtrributesRB_CheckedChanged);
			// 
			// ChangeBaseTypeRB
			// 
			resources.ApplyResources(this.ChangeBaseTypeRB, "ChangeBaseTypeRB");
			this.ChangeBaseTypeRB.Name = "ChangeBaseTypeRB";
			this.ChangeBaseTypeRB.TabStop = true;
			this.ChangeBaseTypeRB.UseVisualStyleBackColor = true;
			this.ChangeBaseTypeRB.CheckedChanged += new System.EventHandler(this.ChangeBaseTypeRB_CheckedChanged);
			// 
			// panel2
			// 
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Controls.Add(this.FullFileRB);
			this.panel2.Controls.Add(this.RCSDeltasRB);
			this.panel2.Controls.Add(this.CompressedFilesRB);
			this.panel2.Controls.Add(this.UseDefaultForTypeRB);
			this.panel2.Name = "panel2";
			// 
			// FullFileRB
			// 
			resources.ApplyResources(this.FullFileRB, "FullFileRB");
			this.FullFileRB.Name = "FullFileRB";
			this.FullFileRB.UseVisualStyleBackColor = true;
			// 
			// RCSDeltasRB
			// 
			resources.ApplyResources(this.RCSDeltasRB, "RCSDeltasRB");
			this.RCSDeltasRB.Name = "RCSDeltasRB";
			this.RCSDeltasRB.UseVisualStyleBackColor = true;
			// 
			// CompressedFilesRB
			// 
			resources.ApplyResources(this.CompressedFilesRB, "CompressedFilesRB");
			this.CompressedFilesRB.Name = "CompressedFilesRB";
			this.CompressedFilesRB.UseVisualStyleBackColor = true;
			// 
			// UseDefaultForTypeRB
			// 
			resources.ApplyResources(this.UseDefaultForTypeRB, "UseDefaultForTypeRB");
			this.UseDefaultForTypeRB.Name = "UseDefaultForTypeRB";
			this.UseDefaultForTypeRB.TabStop = true;
			this.UseDefaultForTypeRB.UseVisualStyleBackColor = true;
			// 
			// divider1GB
			// 
			resources.ApplyResources(this.divider1GB, "divider1GB");
			this.divider1GB.BackColor = System.Drawing.SystemColors.Menu;
			this.divider1GB.Name = "divider1GB";
			this.divider1GB.TabStop = false;
			// 
			// divider4GB
			// 
			resources.ApplyResources(this.divider4GB, "divider4GB");
			this.divider4GB.BackColor = System.Drawing.SystemColors.Menu;
			this.divider4GB.Name = "divider4GB";
			this.divider4GB.TabStop = false;
			// 
			// divider3GB
			// 
			resources.ApplyResources(this.divider3GB, "divider3GB");
			this.divider3GB.BackColor = System.Drawing.SystemColors.Menu;
			this.divider3GB.Name = "divider3GB";
			this.divider3GB.TabStop = false;
			// 
			// divider2GB
			// 
			resources.ApplyResources(this.divider2GB, "divider2GB");
			this.divider2GB.BackColor = System.Drawing.SystemColors.Menu;
			this.divider2GB.Name = "divider2GB";
			this.divider2GB.TabStop = false;
			// 
			// gridLayoutPanel1
			// 
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.Controls.Add(this.LimitRevsCB);
			this.gridLayoutPanel1.Controls.Add(this.MaxRevsCB);
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			// 
			// LimitRevsCB
			// 
			resources.ApplyResources(this.LimitRevsCB, "LimitRevsCB");
			this.LimitRevsCB.Column = 0;
			this.LimitRevsCB.ColumnsSpanned = 0;
			this.LimitRevsCB.Name = "LimitRevsCB";
			this.LimitRevsCB.Row = 0;
			this.LimitRevsCB.RowsSpanned = 0;
			this.LimitRevsCB.UseVisualStyleBackColor = true;
			this.LimitRevsCB.YOffset = 2;
			// 
			// MaxRevsCB
			// 
			this.MaxRevsCB.Column = 1;
			this.MaxRevsCB.ColumnsSpanned = 0;
			this.MaxRevsCB.DesignSize = new System.Drawing.Size(69, 21);
			this.MaxRevsCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.MaxRevsCB.FormattingEnabled = true;
			this.MaxRevsCB.Items.AddRange(new object[] {
            resources.GetString("MaxRevsCB.Items"),
            resources.GetString("MaxRevsCB.Items1"),
            resources.GetString("MaxRevsCB.Items2"),
            resources.GetString("MaxRevsCB.Items3"),
            resources.GetString("MaxRevsCB.Items4"),
            resources.GetString("MaxRevsCB.Items5"),
            resources.GetString("MaxRevsCB.Items6"),
            resources.GetString("MaxRevsCB.Items7"),
            resources.GetString("MaxRevsCB.Items8"),
            resources.GetString("MaxRevsCB.Items9"),
            resources.GetString("MaxRevsCB.Items10"),
            resources.GetString("MaxRevsCB.Items11"),
            resources.GetString("MaxRevsCB.Items12"),
            resources.GetString("MaxRevsCB.Items13"),
            resources.GetString("MaxRevsCB.Items14"),
            resources.GetString("MaxRevsCB.Items15")});
			resources.ApplyResources(this.MaxRevsCB, "MaxRevsCB");
			this.MaxRevsCB.Name = "MaxRevsCB";
			this.MaxRevsCB.Row = 0;
			this.MaxRevsCB.RowsSpanned = 0;
			this.MaxRevsCB.YOffset = 0;
			// 
			// FileAttributesDlg
			// 
			this.AcceptButton = this.OkBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.Controls.Add(this.gridLayoutPanel1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.divider1GB);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.BaseTypeCB);
			this.Controls.Add(this.PreserveLocalModTimeCB);
			this.Controls.Add(this.AlwaywsWritableCB);
			this.Controls.Add(this.ExcecBitSetCB);
			this.Controls.Add(this.RcsKeywordExpansionCB);
			this.Controls.Add(this.RcsLimitedKeywordExpansionCB);
			this.Controls.Add(this.ExclusiveCheckoutsCB);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.TargetChangeListCB);
			this.Controls.Add(this.OkBtn);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.divider2GB);
			this.Controls.Add(this.divider3GB);
			this.Controls.Add(this.divider4GB);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Name = "FileAttributesDlg";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox BaseTypeCB;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox PreserveLocalModTimeCB;
		private System.Windows.Forms.CheckBox AlwaywsWritableCB;
		private System.Windows.Forms.CheckBox ExcecBitSetCB;
		private System.Windows.Forms.CheckBox RcsKeywordExpansionCB;
		private System.Windows.Forms.CheckBox RcsLimitedKeywordExpansionCB;
		private System.Windows.Forms.CheckBox ExclusiveCheckoutsCB;
		private System.Windows.Forms.Label label3;
		private Perforce.I18nControls.GridComboBox MaxRevsCB;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox TargetChangeListCB;
		private System.Windows.Forms.Button OkBtn;
		private System.Windows.Forms.Button CancelBtn;
		private Perforce.I18nControls.GridCheckBox LimitRevsCB;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton AddAtrributesRB;
		private System.Windows.Forms.RadioButton ChangeBaseTypeRB;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RadioButton FullFileRB;
		private System.Windows.Forms.RadioButton RCSDeltasRB;
		private System.Windows.Forms.RadioButton CompressedFilesRB;
		private System.Windows.Forms.RadioButton UseDefaultForTypeRB;
		private System.Windows.Forms.GroupBox divider1GB;
		private System.Windows.Forms.GroupBox divider4GB;
		private System.Windows.Forms.GroupBox divider3GB;
		private System.Windows.Forms.GroupBox divider2GB;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
	}
}