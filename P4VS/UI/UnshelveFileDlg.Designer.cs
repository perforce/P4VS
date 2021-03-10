namespace Perforce.P4VS
{
	partial class UnshelveFileDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnshelveFileDialog));
            this.PromptLbl = new Perforce.I18nControls.GridLabel();
            this.ShelvedFilesLV = new Perforce.I18nControls.GridDoubleBufferedListView();
            this.CheckBoxClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ShelvedFileNameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ShelvedFileFolderClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ShelvedFileActionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ShelvedFileCheckedOutClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FileListMnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.FLM_ViewMI = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new Perforce.I18nControls.GridLabel();
            this.RevertCB = new Perforce.I18nControls.GridCheckBox();
            this.CancelBtn = new Perforce.I18nControls.GridButton();
            this.ShelveBtn = new Perforce.I18nControls.GridButton();
            this.SelectAllCB = new System.Windows.Forms.CheckBox();
            this.OverwriteWritableFilesTB = new Perforce.I18nControls.GridCheckBox();
            this.label1 = new Perforce.I18nControls.GridLabel();
            this.TargetChangelistCB = new Perforce.I18nControls.GridComboBox();
            this.DeleteAfterUnshelveCB = new Perforce.I18nControls.GridCheckBox();
            this.dividerGB = new Perforce.I18nControls.GridGroupBox();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.FileListMnu.SuspendLayout();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.SuspendLayout();
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
            // ShelvedFilesLV
            // 
            this.ShelvedFilesLV.AllowColumnReorder = true;
            resources.ApplyResources(this.ShelvedFilesLV, "ShelvedFilesLV");
            this.ShelvedFilesLV.CellHeight = 86;
            this.ShelvedFilesLV.CellWidth = 454;
            this.ShelvedFilesLV.CheckBoxes = true;
            this.ShelvedFilesLV.Column = 0;
            this.ShelvedFilesLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CheckBoxClm,
            this.ShelvedFileNameClm,
            this.ShelvedFileFolderClm,
            this.ShelvedFileActionClm,
            this.ShelvedFileCheckedOutClm});
            this.ShelvedFilesLV.ColumnsSpanned = 3;
            this.ShelvedFilesLV.ContextMenuStrip = this.FileListMnu;
            this.ShelvedFilesLV.FullRowSelect = true;
            this.ShelvedFilesLV.GridLines = true;
            this.ShelvedFilesLV.HideActionsColumn = false;
            this.ShelvedFilesLV.Name = "ShelvedFilesLV";
            this.ShelvedFilesLV.OwnerDraw = true;
            this.ShelvedFilesLV.Row = 1;
            this.ShelvedFilesLV.RowsSpanned = 0;
            this.ShelvedFilesLV.UseCompatibleStateImageBehavior = false;
            this.ShelvedFilesLV.View = System.Windows.Forms.View.Details;
            this.ShelvedFilesLV.YOffset = 0;
            this.ShelvedFilesLV.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ChangelistFilesLV_ItemChecked);
            // 
            // CheckBoxClm
            // 
            this.CheckBoxClm.Text = global::Perforce.P4VS.Resources.ResolveFileInteractiveControl_SuggestedAction_default;
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
            // ShelvedFileCheckedOutClm
            // 
            resources.ApplyResources(this.ShelvedFileCheckedOutClm, "ShelvedFileCheckedOutClm");
            // 
            // FileListMnu
            // 
            this.FileListMnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FLM_ViewMI});
            this.FileListMnu.Name = "FileListMnu";
            resources.ApplyResources(this.FileListMnu, "FileListMnu");
            this.FileListMnu.Opening += new System.ComponentModel.CancelEventHandler(this.FileListMnu_Opening);
            this.FileListMnu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.FileListMnu_ItemClicked);
            // 
            // FLM_ViewMI
            // 
            this.FLM_ViewMI.Name = "FLM_ViewMI";
            resources.ApplyResources(this.FLM_ViewMI, "FLM_ViewMI");
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.AutoEllipsis = true;
            this.label2.CellHeight = 27;
            this.label2.CellWidth = 277;
            this.label2.Column = 0;
            this.label2.ColumnsSpanned = 0;
            this.label2.Name = "label2";
            this.label2.Row = 2;
            this.label2.RowsSpanned = 0;
            this.label2.YOffset = 4;
            // 
            // RevertCB
            // 
            resources.ApplyResources(this.RevertCB, "RevertCB");
            this.RevertCB.CellHeight = 23;
            this.RevertCB.CellWidth = 277;
            this.RevertCB.Column = 0;
            this.RevertCB.ColumnsSpanned = 3;
            this.RevertCB.Name = "RevertCB";
            this.RevertCB.Row = 5;
            this.RevertCB.RowsSpanned = 0;
            this.RevertCB.UseVisualStyleBackColor = true;
            this.RevertCB.YOffset = 0;
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
            // SelectAllCB
            // 
            resources.ApplyResources(this.SelectAllCB, "SelectAllCB");
            this.SelectAllCB.Name = "SelectAllCB";
            this.SelectAllCB.UseVisualStyleBackColor = true;
            this.SelectAllCB.CheckedChanged += new System.EventHandler(this.SelectAllCB_CheckedChanged);
            // 
            // OverwriteWritableFilesTB
            // 
            resources.ApplyResources(this.OverwriteWritableFilesTB, "OverwriteWritableFilesTB");
            this.OverwriteWritableFilesTB.CellHeight = 23;
            this.OverwriteWritableFilesTB.CellWidth = 277;
            this.OverwriteWritableFilesTB.Column = 0;
            this.OverwriteWritableFilesTB.ColumnsSpanned = 3;
            this.OverwriteWritableFilesTB.Name = "OverwriteWritableFilesTB";
            this.OverwriteWritableFilesTB.Row = 6;
            this.OverwriteWritableFilesTB.RowsSpanned = 0;
            this.OverwriteWritableFilesTB.UseVisualStyleBackColor = true;
            this.OverwriteWritableFilesTB.YOffset = 0;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.CellHeight = 13;
            this.label1.CellWidth = 52;
            this.label1.Column = 0;
            this.label1.ColumnsSpanned = 0;
            this.label1.Name = "label1";
            this.label1.Row = 0;
            this.label1.RowsSpanned = 0;
            this.label1.YOffset = 0;
            // 
            // TargetChangelistCB
            // 
            resources.ApplyResources(this.TargetChangelistCB, "TargetChangelistCB");
            this.TargetChangelistCB.CellHeight = 27;
            this.TargetChangelistCB.CellWidth = 177;
            this.TargetChangelistCB.Column = 1;
            this.TargetChangelistCB.ColumnsSpanned = 2;
            this.TargetChangelistCB.DesignSize = new System.Drawing.Size(0, 0);
            this.TargetChangelistCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TargetChangelistCB.FormattingEnabled = true;
            this.TargetChangelistCB.Name = "TargetChangelistCB";
            this.TargetChangelistCB.Row = 2;
            this.TargetChangelistCB.RowsSpanned = 0;
            this.TargetChangelistCB.YOffset = 0;
            // 
            // DeleteAfterUnshelveCB
            // 
            resources.ApplyResources(this.DeleteAfterUnshelveCB, "DeleteAfterUnshelveCB");
            this.DeleteAfterUnshelveCB.CellHeight = 23;
            this.DeleteAfterUnshelveCB.CellWidth = 277;
            this.DeleteAfterUnshelveCB.Column = 0;
            this.DeleteAfterUnshelveCB.ColumnsSpanned = 3;
            this.DeleteAfterUnshelveCB.Name = "DeleteAfterUnshelveCB";
            this.DeleteAfterUnshelveCB.Row = 4;
            this.DeleteAfterUnshelveCB.RowsSpanned = 0;
            this.DeleteAfterUnshelveCB.UseVisualStyleBackColor = true;
            this.DeleteAfterUnshelveCB.YOffset = 0;
            // 
            // dividerGB
            // 
            resources.ApplyResources(this.dividerGB, "dividerGB");
            this.dividerGB.BackColor = System.Drawing.SystemColors.Menu;
            this.dividerGB.CellHeight = 13;
            this.dividerGB.CellWidth = 396;
            this.dividerGB.Column = 1;
            this.dividerGB.ColumnsSpanned = 0;
            this.dividerGB.Name = "dividerGB";
            this.dividerGB.Row = 0;
            this.dividerGB.RowsSpanned = 0;
            this.dividerGB.TabStop = false;
            this.dividerGB.YOffset = 5;
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.PromptLbl);
            this.gridLayoutPanel1.Controls.Add(this.ShelvedFilesLV);
            this.gridLayoutPanel1.Controls.Add(this.DeleteAfterUnshelveCB);
            this.gridLayoutPanel1.Controls.Add(this.label2);
            this.gridLayoutPanel1.Controls.Add(this.TargetChangelistCB);
            this.gridLayoutPanel1.Controls.Add(this.RevertCB);
            this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
            this.gridLayoutPanel1.Controls.Add(this.OverwriteWritableFilesTB);
            this.gridLayoutPanel1.Controls.Add(this.ShelveBtn);
            this.gridLayoutPanel1.Controls.Add(this.SelectAllCB);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.CellHeight = 29;
            this.gridPanel1.CellWidth = 15;
            this.gridPanel1.Column = 1;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 7;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.CellHeight = 19;
            this.gridLayoutSubpanel1.CellWidth = 454;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 3;
            this.gridLayoutSubpanel1.Controls.Add(this.label1);
            this.gridLayoutSubpanel1.Controls.Add(this.dividerGB);
            this.gridLayoutSubpanel1.EnableDesignerGrid = false;
            this.gridLayoutSubpanel1.EnableDesignerLayout = true;
            this.gridLayoutSubpanel1.EnableParentResize = false;
            this.gridLayoutSubpanel1.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel1.MinimumRowHeight = 10;
            this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
            this.gridLayoutSubpanel1.Row = 3;
            this.gridLayoutSubpanel1.RowsSpanned = 0;
            this.gridLayoutSubpanel1.YOffset = 0;
            // 
            // UnshelveFileDialog
            // 
            this.AcceptButton = this.ShelveBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.gridLayoutPanel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UnshelveFileDialog";
            this.FileListMnu.ResumeLayout(false);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private I18nControls.GridLabel PromptLbl;
        private I18nControls.GridDoubleBufferedListView ShelvedFilesLV;
		private System.Windows.Forms.ColumnHeader ShelvedFileNameClm;
		private System.Windows.Forms.ColumnHeader ShelvedFileActionClm;
		private System.Windows.Forms.ColumnHeader ShelvedFileFolderClm;
        private I18nControls.GridLabel label2;
        private I18nControls.GridCheckBox RevertCB;
        private I18nControls.GridButton CancelBtn;
        private I18nControls.GridButton ShelveBtn;
		private System.Windows.Forms.ColumnHeader ShelvedFileCheckedOutClm;
		private System.Windows.Forms.CheckBox SelectAllCB;
        private I18nControls.GridCheckBox OverwriteWritableFilesTB;
        private I18nControls.GridLabel label1;
		private System.Windows.Forms.ColumnHeader CheckBoxClm;
        private I18nControls.GridComboBox TargetChangelistCB;
        private I18nControls.GridCheckBox DeleteAfterUnshelveCB;
        private I18nControls.GridGroupBox dividerGB;
		private System.Windows.Forms.ContextMenuStrip FileListMnu;
		private System.Windows.Forms.ToolStripMenuItem FLM_ViewMI;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
        private I18nControls.GridPanel gridPanel1;
	}
}