namespace Perforce.P4VS
{
	partial class ShelveFileDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShelveFileDlg));
			this.dividerGB = new Perforce.I18nControls.GridGroupBox();
			this.label1 = new Perforce.I18nControls.GridLabel();
			this.ClearChangelistTB = new Perforce.I18nControls.GridCheckBox();
			this.SelectAllCB = new System.Windows.Forms.CheckBox();
			this.ShelveBtn = new Perforce.I18nControls.GridButton();
			this.CancelBtn = new Perforce.I18nControls.GridButton();
			this.RevertCB = new Perforce.I18nControls.GridCheckBox();
			this.label2 = new Perforce.I18nControls.GridLabel();
			this.ChangelistFilesLV = new Perforce.I18nControls.GridDoubleBufferedListView();
			this.CheckBoxClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ShelvedFileNameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ShelvedFileFolderClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ShelvedFileActionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ShelvedFileTypeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ShelvedFileShelvedClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.PromptLbl = new Perforce.I18nControls.GridLabel();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.gridPanel1 = new Perforce.I18nControls.GridPanel();
			this.DontShelveUnchangeCB = new Perforce.I18nControls.GridCheckBox();
			this.gridLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dividerGB
			// 
			resources.ApplyResources(this.dividerGB, "dividerGB");
			this.dividerGB.BackColor = System.Drawing.SystemColors.Menu;
			this.dividerGB.CellHeight = 30;
			this.dividerGB.CellWidth = 402;
			this.dividerGB.Column = 1;
			this.dividerGB.ColumnsSpanned = 2;
			this.dividerGB.Name = "dividerGB";
			this.dividerGB.Row = 3;
			this.dividerGB.RowsSpanned = 0;
			this.dividerGB.TabStop = false;
			this.dividerGB.YOffset = 5;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.CellHeight = 30;
			this.label1.CellWidth = 52;
			this.label1.Column = 0;
			this.label1.ColumnsSpanned = 0;
			this.label1.Name = "label1";
			this.label1.Row = 3;
			this.label1.RowsSpanned = 0;
			this.label1.YOffset = 0;
			// 
			// ClearChangelistTB
			// 
			resources.ApplyResources(this.ClearChangelistTB, "ClearChangelistTB");
			this.ClearChangelistTB.CellHeight = 23;
			this.ClearChangelistTB.CellWidth = 454;
			this.ClearChangelistTB.Column = 0;
			this.ClearChangelistTB.ColumnsSpanned = 3;
			this.ClearChangelistTB.Name = "ClearChangelistTB";
			this.ClearChangelistTB.Row = 5;
			this.ClearChangelistTB.RowsSpanned = 0;
			this.ClearChangelistTB.UseVisualStyleBackColor = true;
			this.ClearChangelistTB.YOffset = 0;
			// 
			// SelectAllCB
			// 
			resources.ApplyResources(this.SelectAllCB, "SelectAllCB");
			this.SelectAllCB.Name = "SelectAllCB";
			this.SelectAllCB.UseVisualStyleBackColor = true;
			this.SelectAllCB.CheckedChanged += new System.EventHandler(this.SelectAllCB_CheckedChanged);
			// 
			// ShelveBtn
			// 
			resources.ApplyResources(this.ShelveBtn, "ShelveBtn");
			this.ShelveBtn.CellHeight = 29;
			this.ShelveBtn.CellWidth = 81;
			this.ShelveBtn.Column = 2;
			this.ShelveBtn.ColumnsSpanned = 0;
			this.ShelveBtn.Name = "ShelveBtn";
			this.ShelveBtn.Row = 7;
			this.ShelveBtn.RowsSpanned = 0;
			this.ShelveBtn.UseVisualStyleBackColor = true;
			this.ShelveBtn.YOffset = 0;
			this.ShelveBtn.Click += new System.EventHandler(this.ShelveBtn_Click);
			// 
			// CancelBtn
			// 
			resources.ApplyResources(this.CancelBtn, "CancelBtn");
			this.CancelBtn.CellHeight = 29;
			this.CancelBtn.CellWidth = 81;
			this.CancelBtn.Column = 3;
			this.CancelBtn.ColumnsSpanned = 0;
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Row = 7;
			this.CancelBtn.RowsSpanned = 0;
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.YOffset = 0;
			this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
			// 
			// RevertCB
			// 
			resources.ApplyResources(this.RevertCB, "RevertCB");
			this.RevertCB.CellHeight = 23;
			this.RevertCB.CellWidth = 454;
			this.RevertCB.Column = 0;
			this.RevertCB.ColumnsSpanned = 3;
			this.RevertCB.Name = "RevertCB";
			this.RevertCB.Row = 4;
			this.RevertCB.RowsSpanned = 0;
			this.RevertCB.UseVisualStyleBackColor = true;
			this.RevertCB.YOffset = 0;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.AutoEllipsis = true;
			this.label2.CellHeight = 13;
			this.label2.CellWidth = 454;
			this.label2.Column = 0;
			this.label2.ColumnsSpanned = 3;
			this.label2.Name = "label2";
			this.label2.Row = 2;
			this.label2.RowsSpanned = 0;
			this.label2.YOffset = 0;
			// 
			// ChangelistFilesLV
			// 
			this.ChangelistFilesLV.AllowColumnReorder = true;
			resources.ApplyResources(this.ChangelistFilesLV, "ChangelistFilesLV");
			this.ChangelistFilesLV.CellHeight = 118;
			this.ChangelistFilesLV.CellWidth = 454;
			this.ChangelistFilesLV.CheckBoxes = true;
			this.ChangelistFilesLV.Column = 0;
			this.ChangelistFilesLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CheckBoxClm,
            this.ShelvedFileNameClm,
            this.ShelvedFileFolderClm,
            this.ShelvedFileActionClm,
            this.ShelvedFileTypeClm,
            this.ShelvedFileShelvedClm});
			this.ChangelistFilesLV.ColumnsSpanned = 3;
			this.ChangelistFilesLV.FullRowSelect = true;
			this.ChangelistFilesLV.GridLines = true;
			this.ChangelistFilesLV.HideActionsColumn = false;
			this.ChangelistFilesLV.Name = "ChangelistFilesLV";
			this.ChangelistFilesLV.OwnerDraw = true;
			this.ChangelistFilesLV.Row = 1;
			this.ChangelistFilesLV.RowsSpanned = 0;
			this.ChangelistFilesLV.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.ChangelistFilesLV.UseCompatibleStateImageBehavior = false;
			this.ChangelistFilesLV.View = System.Windows.Forms.View.Details;
			this.ChangelistFilesLV.YOffset = 0;
			this.ChangelistFilesLV.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ChangelistFilesLV_ItemChecked);
			this.ChangelistFilesLV.SelectedIndexChanged += new System.EventHandler(this.ChangelistFilesLV_SelectedIndexChanged);
			// 
			// CheckBoxClm
			// 
			resources.ApplyResources(this.CheckBoxClm, "CheckBoxClm");
			// 
			// ShelvedFileNameClm
			// 
			resources.ApplyResources(this.ShelvedFileNameClm, "ShelvedFileNameClm");
			// 
			// ShelvedFileFolderClm
			// 
			resources.ApplyResources(this.ShelvedFileFolderClm, "ShelvedFileFolderClm");
			// 
			// ShelvedFileActionClm
			// 
			resources.ApplyResources(this.ShelvedFileActionClm, "ShelvedFileActionClm");
			// 
			// ShelvedFileTypeClm
			// 
			resources.ApplyResources(this.ShelvedFileTypeClm, "ShelvedFileTypeClm");
			// 
			// ShelvedFileShelvedClm
			// 
			resources.ApplyResources(this.ShelvedFileShelvedClm, "ShelvedFileShelvedClm");
			// 
			// PromptLbl
			// 
			resources.ApplyResources(this.PromptLbl, "PromptLbl");
			this.PromptLbl.CellHeight = 13;
			this.PromptLbl.CellWidth = 454;
			this.PromptLbl.Column = 0;
			this.PromptLbl.ColumnsSpanned = 3;
			this.PromptLbl.Name = "PromptLbl";
			this.PromptLbl.Row = 0;
			this.PromptLbl.RowsSpanned = 0;
			this.PromptLbl.YOffset = 0;
			// 
			// gridLayoutPanel1
			// 
			this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
			this.gridLayoutPanel1.Controls.Add(this.dividerGB);
			this.gridLayoutPanel1.Controls.Add(this.label1);
			this.gridLayoutPanel1.Controls.Add(this.ShelveBtn);
			this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
			this.gridLayoutPanel1.Controls.Add(this.RevertCB);
			this.gridLayoutPanel1.Controls.Add(this.label2);
			this.gridLayoutPanel1.Controls.Add(this.ChangelistFilesLV);
			this.gridLayoutPanel1.Controls.Add(this.PromptLbl);
			this.gridLayoutPanel1.Controls.Add(this.DontShelveUnchangeCB);
			this.gridLayoutPanel1.Controls.Add(this.ClearChangelistTB);
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.EnableDesignerGrid = false;
			this.gridLayoutPanel1.EnableDesignerLayout = true;
			this.gridLayoutPanel1.EnableParentResize = false;
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			this.gridLayoutPanel1.AfterLayoutGrid += new Perforce.I18nControls.GridLayoutPanel.GridLayoutEvent(this.gridLayoutPanel1_AfterLayoutGrid);
			// 
			// gridPanel1
			// 
			resources.ApplyResources(this.gridPanel1, "gridPanel1");
			this.gridPanel1.CellHeight = 29;
			this.gridPanel1.CellWidth = 240;
			this.gridPanel1.Column = 1;
			this.gridPanel1.ColumnsSpanned = 0;
			this.gridPanel1.Name = "gridPanel1";
			this.gridPanel1.Row = 7;
			this.gridPanel1.RowsSpanned = 0;
			this.gridPanel1.YOffset = 0;
			// 
			// DontShelveUnchangeCB
			// 
			resources.ApplyResources(this.DontShelveUnchangeCB, "DontShelveUnchangeCB");
			this.DontShelveUnchangeCB.CellHeight = 23;
			this.DontShelveUnchangeCB.CellWidth = 454;
			this.DontShelveUnchangeCB.Column = 0;
			this.DontShelveUnchangeCB.ColumnsSpanned = 3;
			this.DontShelveUnchangeCB.Name = "DontShelveUnchangeCB";
			this.DontShelveUnchangeCB.Row = 6;
			this.DontShelveUnchangeCB.RowsSpanned = 0;
			this.DontShelveUnchangeCB.UseVisualStyleBackColor = true;
			this.DontShelveUnchangeCB.YOffset = 0;
			// 
			// ShelveFileDlg
			// 
			this.AcceptButton = this.ShelveBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.Controls.Add(this.SelectAllCB);
			this.Controls.Add(this.gridLayoutPanel1);
			this.DoubleBuffered = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ShelveFileDlg";
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private I18nControls.GridLabel PromptLbl;
		private I18nControls.GridDoubleBufferedListView ChangelistFilesLV;
		private System.Windows.Forms.ColumnHeader ShelvedFileNameClm;
		private System.Windows.Forms.ColumnHeader ShelvedFileActionClm;
		private System.Windows.Forms.ColumnHeader ShelvedFileFolderClm;
		private I18nControls.GridLabel label2;
		private I18nControls.GridCheckBox RevertCB;
		private I18nControls.GridButton CancelBtn;
		private I18nControls.GridButton ShelveBtn;
		private System.Windows.Forms.ColumnHeader ShelvedFileTypeClm;
		private System.Windows.Forms.ColumnHeader ShelvedFileShelvedClm;
		private System.Windows.Forms.CheckBox SelectAllCB;
		private I18nControls.GridCheckBox ClearChangelistTB;
		private I18nControls.GridLabel label1;
		private System.Windows.Forms.ColumnHeader CheckBoxClm;
		private I18nControls.GridGroupBox dividerGB;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridPanel gridPanel1;
		private I18nControls.GridCheckBox DontShelveUnchangeCB;
	}
}