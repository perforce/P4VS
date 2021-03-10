namespace Perforce.P4VS
{
	partial class GetRevisionDlg
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetRevisionDlg));
			this.gridPanel2 = new Perforce.I18nControls.GridPanel();
			this.ValueTB = new System.Windows.Forms.TextBox();
			this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.gridPanel3 = new Perforce.I18nControls.GridPanel();
			this.gridPanel1 = new Perforce.I18nControls.GridPanel();
			this.enterRevLbl = new System.Windows.Forms.Label();
			this.browseBtn = new System.Windows.Forms.Button();
			this.instructionsLbl = new Perforce.I18nControls.GridLabel();
			this.filesListView = new Perforce.I18nControls.GridP4ObjectTreeListView();
			this.addBtn = new Perforce.I18nControls.GridButton();
			this.removeBtn = new Perforce.I18nControls.GridButton();
			this.specifierCB = new Perforce.I18nControls.GridComboBox();
			this.specifierRB = new Perforce.I18nControls.GridRadioButton();
			this.getLatestRB = new Perforce.I18nControls.GridRadioButton();
			this.removeChk = new Perforce.I18nControls.GridCheckBox();
			this.changelistFilesChk = new Perforce.I18nControls.GridCheckBox();
			this.forceChk = new Perforce.I18nControls.GridCheckBox();
			this.newCancelBtn = new Perforce.I18nControls.GridButton();
			this.newOKBtn = new Perforce.I18nControls.GridButton();
			this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
			this.optionsLbl = new Perforce.I18nControls.GridLabel();
			this.optionsGB = new Perforce.I18nControls.GridGroupBox();
			this.gridPanel2.SuspendLayout();
			this.gridLayoutPanel1.SuspendLayout();
			this.gridPanel1.SuspendLayout();
			this.gridLayoutSubpanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridPanel2
			// 
			resources.ApplyResources(this.gridPanel2, "gridPanel2");
			this.gridPanel2.Column = 2;
			this.gridPanel2.ColumnsSpanned = 0;
			this.gridPanel2.Controls.Add(this.ValueTB);
			this.gridPanel2.Controls.Add(this.dateTimePicker);
			this.gridPanel2.Name = "gridPanel2";
			this.gridPanel2.Row = 5;
			this.gridPanel2.RowsSpanned = 0;
			this.gridPanel2.YOffset = 1;
			// 
			// ValueTB
			// 
			resources.ApplyResources(this.ValueTB, "ValueTB");
			this.ValueTB.Name = "ValueTB";
			this.ValueTB.EnabledChanged += new System.EventHandler(this.ValueTB_EnabledChanged);
			this.ValueTB.TextChanged += new System.EventHandler(this.ValueTB_TextChanged);
			this.ValueTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ValueTB_KeyDown);
			// 
			// dateTimePicker
			// 
			resources.ApplyResources(this.dateTimePicker, "dateTimePicker");
			this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker.Name = "dateTimePicker";
			// 
			// gridLayoutPanel1
			// 
			this.gridLayoutPanel1.Controls.Add(this.gridPanel3);
			this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
			this.gridLayoutPanel1.Controls.Add(this.instructionsLbl);
			this.gridLayoutPanel1.Controls.Add(this.filesListView);
			this.gridLayoutPanel1.Controls.Add(this.addBtn);
			this.gridLayoutPanel1.Controls.Add(this.removeBtn);
			this.gridLayoutPanel1.Controls.Add(this.specifierCB);
			this.gridLayoutPanel1.Controls.Add(this.specifierRB);
			this.gridLayoutPanel1.Controls.Add(this.getLatestRB);
			this.gridLayoutPanel1.Controls.Add(this.removeChk);
			this.gridLayoutPanel1.Controls.Add(this.changelistFilesChk);
			this.gridLayoutPanel1.Controls.Add(this.forceChk);
			this.gridLayoutPanel1.Controls.Add(this.newCancelBtn);
			this.gridLayoutPanel1.Controls.Add(this.newOKBtn);
			this.gridLayoutPanel1.Controls.Add(this.gridPanel2);
			this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.EnableDesignerGrid = true;
			this.gridLayoutPanel1.EnableDesignerLayout = true;
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			// 
			// gridPanel3
			// 
			resources.ApplyResources(this.gridPanel3, "gridPanel3");
			this.gridPanel3.Column = 3;
			this.gridPanel3.ColumnsSpanned = 0;
			this.gridPanel3.Name = "gridPanel3";
			this.gridPanel3.Row = 3;
			this.gridPanel3.RowsSpanned = 0;
			this.gridPanel3.YOffset = 0;
			// 
			// gridPanel1
			// 
			this.gridPanel1.Column = 3;
			this.gridPanel1.ColumnsSpanned = 0;
			this.gridPanel1.Controls.Add(this.enterRevLbl);
			this.gridPanel1.Controls.Add(this.browseBtn);
			resources.ApplyResources(this.gridPanel1, "gridPanel1");
			this.gridPanel1.Name = "gridPanel1";
			this.gridPanel1.Row = 5;
			this.gridPanel1.RowsSpanned = 0;
			this.gridPanel1.YOffset = 0;
			// 
			// enterRevLbl
			// 
			resources.ApplyResources(this.enterRevLbl, "enterRevLbl");
			this.enterRevLbl.Name = "enterRevLbl";
			// 
			// browseBtn
			// 
			resources.ApplyResources(this.browseBtn, "browseBtn");
			this.browseBtn.Name = "browseBtn";
			this.browseBtn.UseVisualStyleBackColor = true;
			this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
			// 
			// instructionsLbl
			// 
			resources.ApplyResources(this.instructionsLbl, "instructionsLbl");
			this.instructionsLbl.Column = 0;
			this.instructionsLbl.ColumnsSpanned = 3;
			this.instructionsLbl.Name = "instructionsLbl";
			this.instructionsLbl.Row = 0;
			this.instructionsLbl.RowsSpanned = 0;
			this.instructionsLbl.YOffset = 0;
			// 
			// filesListView
			// 
			this.filesListView._maxLineOffset = 0;
			this.filesListView.ActionColumn = -1;
			resources.ApplyResources(this.filesListView, "filesListView");
			this.filesListView.AllowColumnReorder = true;
			this.filesListView.Column = 0;
			this.filesListView.ColumnsSpanned = 2;
			this.filesListView.EnableIconOverlays = true;
			this.filesListView.EnableSorting = true;
			this.filesListView.FullRowSelect = true;
			this.filesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.filesListView.Name = "filesListView";
			this.filesListView.OverlayOffset = 3;
			this.filesListView.RootCheckBoxes = false;
			this.filesListView.Row = 1;
			this.filesListView.RowsSpanned = 2;
			this.filesListView.ScrollPosition = 0;
			this.filesListView.ShowGroups = false;
			this.filesListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.filesListView.TreeView = false;
			this.filesListView.UseClassicImageList = false;
			this.filesListView.UseCompatibleStateImageBehavior = false;
			this.filesListView.View = System.Windows.Forms.View.Details;
			this.filesListView.YOffset = 0;
			this.filesListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.filesListView_ItemSelectionChanged);
			this.filesListView.SelectedIndexChanged += new System.EventHandler(this.filesListView_SelectedIndexChanged);
			this.filesListView.Resize += new System.EventHandler(this.filesListView_Resize);
			// 
			// addBtn
			// 
			this.addBtn.Column = 3;
			this.addBtn.ColumnsSpanned = 0;
			resources.ApplyResources(this.addBtn, "addBtn");
			this.addBtn.Name = "addBtn";
			this.addBtn.Row = 1;
			this.addBtn.RowsSpanned = 0;
			this.addBtn.UseVisualStyleBackColor = true;
			this.addBtn.YOffset = 0;
			this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
			// 
			// removeBtn
			// 
			this.removeBtn.Column = 3;
			this.removeBtn.ColumnsSpanned = 0;
			resources.ApplyResources(this.removeBtn, "removeBtn");
			this.removeBtn.Name = "removeBtn";
			this.removeBtn.Row = 2;
			this.removeBtn.RowsSpanned = 0;
			this.removeBtn.UseVisualStyleBackColor = true;
			this.removeBtn.YOffset = 0;
			this.removeBtn.Click += new System.EventHandler(this.removeBtn_Click);
			// 
			// specifierCB
			// 
			resources.ApplyResources(this.specifierCB, "specifierCB");
			this.specifierCB.AutoCompleteCustomSource.AddRange(new string[] {
            resources.GetString("specifierCB.AutoCompleteCustomSource"),
            resources.GetString("specifierCB.AutoCompleteCustomSource1"),
            resources.GetString("specifierCB.AutoCompleteCustomSource2"),
            resources.GetString("specifierCB.AutoCompleteCustomSource3"),
            resources.GetString("specifierCB.AutoCompleteCustomSource4")});
			this.specifierCB.Column = 1;
			this.specifierCB.ColumnsSpanned = 0;
			this.specifierCB.DesignSize = new System.Drawing.Size(0, 0);
			this.specifierCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.specifierCB.FormattingEnabled = true;
			this.specifierCB.Items.AddRange(new object[] {
            resources.GetString("specifierCB.Items"),
            resources.GetString("specifierCB.Items1"),
            resources.GetString("specifierCB.Items2"),
            resources.GetString("specifierCB.Items3"),
            resources.GetString("specifierCB.Items4")});
			this.specifierCB.Name = "specifierCB";
			this.specifierCB.Row = 5;
			this.specifierCB.RowsSpanned = 0;
			this.specifierCB.YOffset = 1;
			this.specifierCB.SelectedIndexChanged += new System.EventHandler(this.specifierCB_SelectedIndexChanged);
			// 
			// specifierRB
			// 
			resources.ApplyResources(this.specifierRB, "specifierRB");
			this.specifierRB.Column = 0;
			this.specifierRB.ColumnsSpanned = 0;
			this.specifierRB.Name = "specifierRB";
			this.specifierRB.Row = 5;
			this.specifierRB.RowsSpanned = 0;
			this.specifierRB.TabStop = true;
			this.specifierRB.UseVisualStyleBackColor = true;
			this.specifierRB.YOffset = 3;
			this.specifierRB.CheckedChanged += new System.EventHandler(this.specifierRB_CheckedChanged);
			// 
			// getLatestRB
			// 
			resources.ApplyResources(this.getLatestRB, "getLatestRB");
			this.getLatestRB.Column = 0;
			this.getLatestRB.ColumnsSpanned = 3;
			this.getLatestRB.Name = "getLatestRB";
			this.getLatestRB.Row = 4;
			this.getLatestRB.RowsSpanned = 0;
			this.getLatestRB.TabStop = true;
			this.getLatestRB.UseVisualStyleBackColor = true;
			this.getLatestRB.YOffset = 0;
			this.getLatestRB.CheckedChanged += new System.EventHandler(this.getLatestRB_CheckedChanged);
			// 
			// removeChk
			// 
			resources.ApplyResources(this.removeChk, "removeChk");
			this.removeChk.Column = 0;
			this.removeChk.ColumnsSpanned = 3;
			this.removeChk.Name = "removeChk";
			this.removeChk.Row = 9;
			this.removeChk.RowsSpanned = 0;
			this.removeChk.UseVisualStyleBackColor = true;
			this.removeChk.YOffset = 0;
			// 
			// changelistFilesChk
			// 
			resources.ApplyResources(this.changelistFilesChk, "changelistFilesChk");
			this.changelistFilesChk.Column = 0;
			this.changelistFilesChk.ColumnsSpanned = 3;
			this.changelistFilesChk.Name = "changelistFilesChk";
			this.changelistFilesChk.Row = 8;
			this.changelistFilesChk.RowsSpanned = 0;
			this.changelistFilesChk.UseVisualStyleBackColor = true;
			this.changelistFilesChk.YOffset = 0;
			// 
			// forceChk
			// 
			resources.ApplyResources(this.forceChk, "forceChk");
			this.forceChk.Column = 0;
			this.forceChk.ColumnsSpanned = 3;
			this.forceChk.Name = "forceChk";
			this.forceChk.Row = 7;
			this.forceChk.RowsSpanned = 0;
			this.forceChk.UseVisualStyleBackColor = true;
			this.forceChk.YOffset = 0;
			// 
			// newCancelBtn
			// 
			this.newCancelBtn.Column = 3;
			this.newCancelBtn.ColumnsSpanned = 0;
			this.newCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.newCancelBtn, "newCancelBtn");
			this.newCancelBtn.Name = "newCancelBtn";
			this.newCancelBtn.Row = 10;
			this.newCancelBtn.RowsSpanned = 0;
			this.newCancelBtn.UseVisualStyleBackColor = true;
			this.newCancelBtn.YOffset = 0;
			// 
			// newOKBtn
			// 
			resources.ApplyResources(this.newOKBtn, "newOKBtn");
			this.newOKBtn.Column = 2;
			this.newOKBtn.ColumnsSpanned = 0;
			this.newOKBtn.Name = "newOKBtn";
			this.newOKBtn.Row = 10;
			this.newOKBtn.RowsSpanned = 0;
			this.newOKBtn.UseVisualStyleBackColor = true;
			this.newOKBtn.YOffset = 0;
			this.newOKBtn.EnabledChanged += new System.EventHandler(this.newOKBtn_EnabledChanged);
			this.newOKBtn.Click += new System.EventHandler(this.newOKBtn_Click);
			// 
			// gridLayoutSubpanel1
			// 
			resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
			this.gridLayoutSubpanel1.Column = 0;
			this.gridLayoutSubpanel1.ColumnsSpanned = 3;
			this.gridLayoutSubpanel1.Controls.Add(this.optionsLbl);
			this.gridLayoutSubpanel1.Controls.Add(this.optionsGB);
			this.gridLayoutSubpanel1.EnableDesignerGrid = false;
			this.gridLayoutSubpanel1.EnableDesignerLayout = false;
			this.gridLayoutSubpanel1.MinimumColumnWidth = 10;
			this.gridLayoutSubpanel1.MinimumRowHeight = 10;
			this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
			this.gridLayoutSubpanel1.Row = 6;
			this.gridLayoutSubpanel1.RowsSpanned = 0;
			this.gridLayoutSubpanel1.YOffset = 0;
			// 
			// optionsLbl
			// 
			resources.ApplyResources(this.optionsLbl, "optionsLbl");
			this.optionsLbl.Column = 0;
			this.optionsLbl.ColumnsSpanned = 0;
			this.optionsLbl.Name = "optionsLbl";
			this.optionsLbl.Row = 0;
			this.optionsLbl.RowsSpanned = 0;
			this.optionsLbl.YOffset = 0;
			// 
			// optionsGB
			// 
			resources.ApplyResources(this.optionsGB, "optionsGB");
			this.optionsGB.Column = 1;
			this.optionsGB.ColumnsSpanned = 0;
			this.optionsGB.Name = "optionsGB";
			this.optionsGB.Row = 0;
			this.optionsGB.RowsSpanned = 0;
			this.optionsGB.TabStop = false;
			this.optionsGB.YOffset = 9;
			// 
			// GetRevisionDlg
			// 
			this.AcceptButton = this.newOKBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.newCancelBtn;
			this.Controls.Add(this.gridLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GetRevisionDlg";
			this.Load += new System.EventHandler(this.GetRevisionDlg_Load);
			this.gridPanel2.ResumeLayout(false);
			this.gridPanel2.PerformLayout();
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
			this.gridPanel1.ResumeLayout(false);
			this.gridPanel1.PerformLayout();
			this.gridLayoutSubpanel1.ResumeLayout(false);
			this.gridLayoutSubpanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox ValueTB;
		private System.Windows.Forms.DateTimePicker dateTimePicker;
		private System.Windows.Forms.Label enterRevLbl;
		private System.Windows.Forms.Button browseBtn;
		private I18nControls.GridPanel gridPanel2;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridPanel gridPanel1;
		private I18nControls.GridButton newOKBtn;
		private I18nControls.GridButton newCancelBtn;
		private I18nControls.GridCheckBox forceChk;
		private I18nControls.GridCheckBox changelistFilesChk;
		private I18nControls.GridCheckBox removeChk;
		private I18nControls.GridRadioButton getLatestRB;
		private I18nControls.GridRadioButton specifierRB;
		private I18nControls.GridComboBox specifierCB;
		private I18nControls.GridButton removeBtn;
		private I18nControls.GridButton addBtn;
		private I18nControls.GridP4ObjectTreeListView filesListView;
		private I18nControls.GridLabel instructionsLbl;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
		private I18nControls.GridLabel optionsLbl;
		private I18nControls.GridGroupBox optionsGB;
		private I18nControls.GridPanel gridPanel3;

	}
}