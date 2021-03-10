namespace Perforce.P4VS
{
	partial class DiffDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiffDlg));
			this.path1ValueTB = new System.Windows.Forms.TextBox();
			this.diffBtn = new Perforce.I18nControls.GridButton();
			this.cancelBtn = new Perforce.I18nControls.GridButton();
			this.specifier1stCB = new Perforce.I18nControls.GridComboBox();
			this.panel3 = new Perforce.I18nControls.GridPanel();
			this.specifier2ndRB = new System.Windows.Forms.RadioButton();
			this.have2ndRB = new System.Windows.Forms.RadioButton();
			this.workspace2ndRB = new System.Windows.Forms.RadioButton();
			this.latest2ndRB = new System.Windows.Forms.RadioButton();
			this.panel2 = new Perforce.I18nControls.GridPanel();
			this.specifier1stRB = new System.Windows.Forms.RadioButton();
			this.latest1stRB = new System.Windows.Forms.RadioButton();
			this.have1stRB = new System.Windows.Forms.RadioButton();
			this.workspace1stRB = new System.Windows.Forms.RadioButton();
			this.specifier2ndCB = new Perforce.I18nControls.GridComboBox();
			this.path2RevLbl = new System.Windows.Forms.Label();
			this.path2BrowseBtn = new System.Windows.Forms.Button();
			this.path2ValueTB = new System.Windows.Forms.TextBox();
			this.path2TB = new Perforce.I18nControls.GridTextBox();
			this.path1TB = new Perforce.I18nControls.GridTextBox();
			this.path2Lbl = new Perforce.I18nControls.GridLabel();
			this.path1RevLbl = new System.Windows.Forms.Label();
			this.path1BrowseBtn = new System.Windows.Forms.Button();
			this.path1Lbl = new Perforce.I18nControls.GridLabel();
			this.path1dateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.path2dateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.gridPanel1_1 = new Perforce.I18nControls.GridPanel();
			this.gridPanel1_2 = new Perforce.I18nControls.GridPanel();
			this.gridPanel2_1 = new Perforce.I18nControls.GridPanel();
			this.gridPanel2_2 = new Perforce.I18nControls.GridPanel();
			this.panel3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.gridLayoutPanel1.SuspendLayout();
			this.gridPanel1_1.SuspendLayout();
			this.gridPanel1_2.SuspendLayout();
			this.gridPanel2_1.SuspendLayout();
			this.gridPanel2_2.SuspendLayout();
			this.SuspendLayout();
			// 
			// path1ValueTB
			// 
			resources.ApplyResources(this.path1ValueTB, "path1ValueTB");
			this.path1ValueTB.Name = "path1ValueTB";
			this.path1ValueTB.TextChanged += new System.EventHandler(this.Path1ValueTB_TextChanged);
			// 
			// diffBtn
			// 
			resources.ApplyResources(this.diffBtn, "diffBtn");
			this.diffBtn.Column = 3;
			this.diffBtn.ColumnsSpanned = 0;
			this.diffBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.diffBtn.Name = "diffBtn";
			this.diffBtn.Row = 6;
			this.diffBtn.RowsSpanned = 0;
			this.diffBtn.UseVisualStyleBackColor = true;
			this.diffBtn.YOffset = 0;
			// 
			// cancelBtn
			// 
			resources.ApplyResources(this.cancelBtn, "cancelBtn");
			this.cancelBtn.Column = 4;
			this.cancelBtn.ColumnsSpanned = 0;
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Row = 6;
			this.cancelBtn.RowsSpanned = 0;
			this.cancelBtn.UseVisualStyleBackColor = true;
			this.cancelBtn.YOffset = 0;
			// 
			// specifier1stCB
			// 
			this.specifier1stCB.AutoCompleteCustomSource.AddRange(new string[] {
            resources.GetString("specifier1stCB.AutoCompleteCustomSource"),
            resources.GetString("specifier1stCB.AutoCompleteCustomSource1"),
            resources.GetString("specifier1stCB.AutoCompleteCustomSource2"),
            resources.GetString("specifier1stCB.AutoCompleteCustomSource3"),
            resources.GetString("specifier1stCB.AutoCompleteCustomSource4")});
			this.specifier1stCB.Column = 2;
			this.specifier1stCB.ColumnsSpanned = 0;
			this.specifier1stCB.DesignSize = new System.Drawing.Size(0, 0);
			this.specifier1stCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.specifier1stCB.FormattingEnabled = true;
			this.specifier1stCB.Items.AddRange(new object[] {
            resources.GetString("specifier1stCB.Items"),
            resources.GetString("specifier1stCB.Items1"),
            resources.GetString("specifier1stCB.Items2"),
            resources.GetString("specifier1stCB.Items3"),
            resources.GetString("specifier1stCB.Items4")});
			resources.ApplyResources(this.specifier1stCB, "specifier1stCB");
			this.specifier1stCB.Name = "specifier1stCB";
			this.specifier1stCB.Row = 2;
			this.specifier1stCB.RowsSpanned = 0;
			this.specifier1stCB.YOffset = 2;
			this.specifier1stCB.SelectedIndexChanged += new System.EventHandler(this.specifier1stCB_SelectedIndexChanged);
			// 
			// panel3
			// 
			this.panel3.Column = 1;
			this.panel3.ColumnsSpanned = 3;
			this.panel3.Controls.Add(this.specifier2ndRB);
			this.panel3.Controls.Add(this.have2ndRB);
			this.panel3.Controls.Add(this.workspace2ndRB);
			this.panel3.Controls.Add(this.latest2ndRB);
			resources.ApplyResources(this.panel3, "panel3");
			this.panel3.Name = "panel3";
			this.panel3.Row = 4;
			this.panel3.RowsSpanned = 0;
			this.panel3.YOffset = 0;
			// 
			// specifier2ndRB
			// 
			resources.ApplyResources(this.specifier2ndRB, "specifier2ndRB");
			this.specifier2ndRB.Name = "specifier2ndRB";
			this.specifier2ndRB.TabStop = true;
			this.specifier2ndRB.UseVisualStyleBackColor = true;
			this.specifier2ndRB.Click += new System.EventHandler(this.specifier2ndRB_Click);
			// 
			// have2ndRB
			// 
			resources.ApplyResources(this.have2ndRB, "have2ndRB");
			this.have2ndRB.Name = "have2ndRB";
			this.have2ndRB.TabStop = true;
			this.have2ndRB.UseVisualStyleBackColor = true;
			this.have2ndRB.Click += new System.EventHandler(this.have2ndRB_Click);
			// 
			// workspace2ndRB
			// 
			resources.ApplyResources(this.workspace2ndRB, "workspace2ndRB");
			this.workspace2ndRB.Name = "workspace2ndRB";
			this.workspace2ndRB.TabStop = true;
			this.workspace2ndRB.UseVisualStyleBackColor = true;
			this.workspace2ndRB.Click += new System.EventHandler(this.workspace2ndRB_Click);
			// 
			// latest2ndRB
			// 
			resources.ApplyResources(this.latest2ndRB, "latest2ndRB");
			this.latest2ndRB.Name = "latest2ndRB";
			this.latest2ndRB.TabStop = true;
			this.latest2ndRB.UseVisualStyleBackColor = true;
			this.latest2ndRB.Click += new System.EventHandler(this.latest2ndRB_Click);
			// 
			// panel2
			// 
			this.panel2.Column = 1;
			this.panel2.ColumnsSpanned = 3;
			this.panel2.Controls.Add(this.specifier1stRB);
			this.panel2.Controls.Add(this.latest1stRB);
			this.panel2.Controls.Add(this.have1stRB);
			this.panel2.Controls.Add(this.workspace1stRB);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			this.panel2.Row = 1;
			this.panel2.RowsSpanned = 0;
			this.panel2.YOffset = 0;
			// 
			// specifier1stRB
			// 
			resources.ApplyResources(this.specifier1stRB, "specifier1stRB");
			this.specifier1stRB.Name = "specifier1stRB";
			this.specifier1stRB.TabStop = true;
			this.specifier1stRB.UseVisualStyleBackColor = true;
			this.specifier1stRB.Click += new System.EventHandler(this.specifier1stRB_Click);
			// 
			// latest1stRB
			// 
			resources.ApplyResources(this.latest1stRB, "latest1stRB");
			this.latest1stRB.Name = "latest1stRB";
			this.latest1stRB.TabStop = true;
			this.latest1stRB.UseVisualStyleBackColor = true;
			this.latest1stRB.Click += new System.EventHandler(this.latest1stRB_Click);
			// 
			// have1stRB
			// 
			resources.ApplyResources(this.have1stRB, "have1stRB");
			this.have1stRB.Name = "have1stRB";
			this.have1stRB.TabStop = true;
			this.have1stRB.UseVisualStyleBackColor = true;
			this.have1stRB.Click += new System.EventHandler(this.have1stRB_Click);
			// 
			// workspace1stRB
			// 
			resources.ApplyResources(this.workspace1stRB, "workspace1stRB");
			this.workspace1stRB.Name = "workspace1stRB";
			this.workspace1stRB.TabStop = true;
			this.workspace1stRB.UseVisualStyleBackColor = true;
			this.workspace1stRB.Click += new System.EventHandler(this.workspace1stRB_Click);
			// 
			// specifier2ndCB
			// 
			this.specifier2ndCB.AutoCompleteCustomSource.AddRange(new string[] {
            resources.GetString("specifier2ndCB.AutoCompleteCustomSource"),
            resources.GetString("specifier2ndCB.AutoCompleteCustomSource1"),
            resources.GetString("specifier2ndCB.AutoCompleteCustomSource2"),
            resources.GetString("specifier2ndCB.AutoCompleteCustomSource3"),
            resources.GetString("specifier2ndCB.AutoCompleteCustomSource4")});
			this.specifier2ndCB.Column = 2;
			this.specifier2ndCB.ColumnsSpanned = 0;
			this.specifier2ndCB.DesignSize = new System.Drawing.Size(0, 0);
			this.specifier2ndCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.specifier2ndCB.FormattingEnabled = true;
			this.specifier2ndCB.Items.AddRange(new object[] {
            resources.GetString("specifier2ndCB.Items"),
            resources.GetString("specifier2ndCB.Items1"),
            resources.GetString("specifier2ndCB.Items2"),
            resources.GetString("specifier2ndCB.Items3"),
            resources.GetString("specifier2ndCB.Items4")});
			resources.ApplyResources(this.specifier2ndCB, "specifier2ndCB");
			this.specifier2ndCB.Name = "specifier2ndCB";
			this.specifier2ndCB.Row = 5;
			this.specifier2ndCB.RowsSpanned = 0;
			this.specifier2ndCB.YOffset = 2;
			this.specifier2ndCB.SelectedIndexChanged += new System.EventHandler(this.specifier2ndCB_SelectedIndexChanged);
			// 
			// path2RevLbl
			// 
			resources.ApplyResources(this.path2RevLbl, "path2RevLbl");
			this.path2RevLbl.Name = "path2RevLbl";
			// 
			// path2BrowseBtn
			// 
			resources.ApplyResources(this.path2BrowseBtn, "path2BrowseBtn");
			this.path2BrowseBtn.Name = "path2BrowseBtn";
			this.path2BrowseBtn.UseVisualStyleBackColor = true;
			this.path2BrowseBtn.Click += new System.EventHandler(this.path2BrowseBtn_Click);
			// 
			// path2ValueTB
			// 
			resources.ApplyResources(this.path2ValueTB, "path2ValueTB");
			this.path2ValueTB.Name = "path2ValueTB";
			this.path2ValueTB.TextChanged += new System.EventHandler(this.Path2ValueTB_TextChanged);
			// 
			// path2TB
			// 
			resources.ApplyResources(this.path2TB, "path2TB");
			this.path2TB.Column = 1;
			this.path2TB.ColumnsSpanned = 3;
			this.path2TB.Name = "path2TB";
			this.path2TB.Row = 3;
			this.path2TB.RowsSpanned = 0;
			this.path2TB.YOffset = 9;
			this.path2TB.TextChanged += new System.EventHandler(this.path2TB_TextChanged);
			// 
			// path1TB
			// 
			resources.ApplyResources(this.path1TB, "path1TB");
			this.path1TB.Column = 1;
			this.path1TB.ColumnsSpanned = 3;
			this.path1TB.Name = "path1TB";
			this.path1TB.Row = 0;
			this.path1TB.RowsSpanned = 0;
			this.path1TB.YOffset = 9;
			this.path1TB.TextChanged += new System.EventHandler(this.path1TB_TextChanged);
			// 
			// path2Lbl
			// 
			resources.ApplyResources(this.path2Lbl, "path2Lbl");
			this.path2Lbl.Column = 0;
			this.path2Lbl.ColumnsSpanned = 0;
			this.path2Lbl.Name = "path2Lbl";
			this.path2Lbl.Row = 3;
			this.path2Lbl.RowsSpanned = 0;
			this.path2Lbl.YOffset = 0;
			// 
			// path1RevLbl
			// 
			resources.ApplyResources(this.path1RevLbl, "path1RevLbl");
			this.path1RevLbl.Name = "path1RevLbl";
			// 
			// path1BrowseBtn
			// 
			resources.ApplyResources(this.path1BrowseBtn, "path1BrowseBtn");
			this.path1BrowseBtn.Name = "path1BrowseBtn";
			this.path1BrowseBtn.UseVisualStyleBackColor = true;
			this.path1BrowseBtn.Click += new System.EventHandler(this.path1BrowseBtn_Click);
			// 
			// path1Lbl
			// 
			resources.ApplyResources(this.path1Lbl, "path1Lbl");
			this.path1Lbl.Column = 0;
			this.path1Lbl.ColumnsSpanned = 0;
			this.path1Lbl.Name = "path1Lbl";
			this.path1Lbl.Row = 0;
			this.path1Lbl.RowsSpanned = 0;
			this.path1Lbl.YOffset = 0;
			// 
			// path1dateTimePicker
			// 
			resources.ApplyResources(this.path1dateTimePicker, "path1dateTimePicker");
			this.path1dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.path1dateTimePicker.Name = "path1dateTimePicker";
			// 
			// path2dateTimePicker
			// 
			resources.ApplyResources(this.path2dateTimePicker, "path2dateTimePicker");
			this.path2dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.path2dateTimePicker.Name = "path2dateTimePicker";
			// 
			// gridLayoutPanel1
			// 
			this.gridLayoutPanel1.Controls.Add(this.gridPanel1_1);
			this.gridLayoutPanel1.Controls.Add(this.panel3);
			this.gridLayoutPanel1.Controls.Add(this.panel2);
			this.gridLayoutPanel1.Controls.Add(this.cancelBtn);
			this.gridLayoutPanel1.Controls.Add(this.diffBtn);
			this.gridLayoutPanel1.Controls.Add(this.specifier2ndCB);
			this.gridLayoutPanel1.Controls.Add(this.path2TB);
			this.gridLayoutPanel1.Controls.Add(this.path1TB);
			this.gridLayoutPanel1.Controls.Add(this.specifier1stCB);
			this.gridLayoutPanel1.Controls.Add(this.path2Lbl);
			this.gridLayoutPanel1.Controls.Add(this.path1Lbl);
			this.gridLayoutPanel1.Controls.Add(this.gridPanel1_2);
			this.gridLayoutPanel1.Controls.Add(this.gridPanel2_1);
			this.gridLayoutPanel1.Controls.Add(this.gridPanel2_2);
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.EnableDesignerGrid = true;
			this.gridLayoutPanel1.EnableDesignerLayout = true;
			this.gridLayoutPanel1.MinimumColumnWidth = 30;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			// 
			// gridPanel1_1
			// 
			resources.ApplyResources(this.gridPanel1_1, "gridPanel1_1");
			this.gridPanel1_1.Column = 3;
			this.gridPanel1_1.ColumnsSpanned = 0;
			this.gridPanel1_1.Controls.Add(this.path1dateTimePicker);
			this.gridPanel1_1.Controls.Add(this.path1ValueTB);
			this.gridPanel1_1.Name = "gridPanel1_1";
			this.gridPanel1_1.Row = 2;
			this.gridPanel1_1.RowsSpanned = 0;
			this.gridPanel1_1.YOffset = 2;
			// 
			// gridPanel1_2
			// 
			resources.ApplyResources(this.gridPanel1_2, "gridPanel1_2");
			this.gridPanel1_2.Column = 4;
			this.gridPanel1_2.ColumnsSpanned = 0;
			this.gridPanel1_2.Controls.Add(this.path1BrowseBtn);
			this.gridPanel1_2.Controls.Add(this.path1RevLbl);
			this.gridPanel1_2.Name = "gridPanel1_2";
			this.gridPanel1_2.Row = 2;
			this.gridPanel1_2.RowsSpanned = 0;
			this.gridPanel1_2.YOffset = 0;
			// 
			// gridPanel2_1
			// 
			resources.ApplyResources(this.gridPanel2_1, "gridPanel2_1");
			this.gridPanel2_1.Column = 3;
			this.gridPanel2_1.ColumnsSpanned = 0;
			this.gridPanel2_1.Controls.Add(this.path2ValueTB);
			this.gridPanel2_1.Controls.Add(this.path2dateTimePicker);
			this.gridPanel2_1.Name = "gridPanel2_1";
			this.gridPanel2_1.Row = 5;
			this.gridPanel2_1.RowsSpanned = 0;
			this.gridPanel2_1.YOffset = 2;
			// 
			// gridPanel2_2
			// 
			resources.ApplyResources(this.gridPanel2_2, "gridPanel2_2");
			this.gridPanel2_2.Column = 4;
			this.gridPanel2_2.ColumnsSpanned = 0;
			this.gridPanel2_2.Controls.Add(this.path2BrowseBtn);
			this.gridPanel2_2.Controls.Add(this.path2RevLbl);
			this.gridPanel2_2.Name = "gridPanel2_2";
			this.gridPanel2_2.Row = 5;
			this.gridPanel2_2.RowsSpanned = 0;
			this.gridPanel2_2.YOffset = 0;
			// 
			// DiffDlg
			// 
			this.AcceptButton = this.diffBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelBtn;
			this.Controls.Add(this.gridLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DiffDlg";
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
			this.gridPanel1_1.ResumeLayout(false);
			this.gridPanel1_1.PerformLayout();
			this.gridPanel1_2.ResumeLayout(false);
			this.gridPanel1_2.PerformLayout();
			this.gridPanel2_1.ResumeLayout(false);
			this.gridPanel2_1.PerformLayout();
			this.gridPanel2_2.ResumeLayout(false);
			this.gridPanel2_2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox path1ValueTB;
		private I18nControls.GridButton diffBtn;
		private I18nControls.GridButton cancelBtn;
		private I18nControls.GridComboBox specifier1stCB;
		private I18nControls.GridLabel path1Lbl;
		private System.Windows.Forms.Button path1BrowseBtn;
		private System.Windows.Forms.Label path1RevLbl;
        private System.Windows.Forms.DateTimePicker path1dateTimePicker;
		private I18nControls.GridComboBox specifier2ndCB;
        private System.Windows.Forms.Label path2RevLbl;
        private System.Windows.Forms.DateTimePicker path2dateTimePicker;
        private System.Windows.Forms.Button path2BrowseBtn;
		private System.Windows.Forms.TextBox path2ValueTB;
		private I18nControls.GridTextBox path2TB;
		private I18nControls.GridTextBox path1TB;
		private I18nControls.GridLabel path2Lbl;
		private I18nControls.GridPanel panel3;
		private System.Windows.Forms.RadioButton specifier2ndRB;
		private System.Windows.Forms.RadioButton have2ndRB;
		private System.Windows.Forms.RadioButton workspace2ndRB;
		private System.Windows.Forms.RadioButton latest2ndRB;
		private I18nControls.GridPanel panel2;
		private System.Windows.Forms.RadioButton specifier1stRB;
		private System.Windows.Forms.RadioButton latest1stRB;
		private System.Windows.Forms.RadioButton have1stRB;
		private System.Windows.Forms.RadioButton workspace1stRB;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridPanel gridPanel1_1;
		private I18nControls.GridPanel gridPanel1_2;
		private I18nControls.GridPanel gridPanel2_1;
		private I18nControls.GridPanel gridPanel2_2;
	}
}