namespace Perforce.P4VS
{
	partial class SelectChangelistDlg2
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectChangelistDlg2));
			this.PromptLbl = new Perforce.I18nControls.GridLabel();
			this.ItemsCB = new Perforce.I18nControls.GridComboBox();
			this.DontShowAgainCB = new Perforce.I18nControls.GridCheckBox();
			this.DescriptionTB = new Perforce.I18nControls.GridTextBox();
			this.label1 = new Perforce.I18nControls.GridLabel();
			this.msgText = new Perforce.I18nControls.GridLabel();
			this.CancelBtn = new Perforce.I18nControls.GridButton();
			this.OkBtn = new Perforce.I18nControls.GridButton();
			this.saveToChangelistBtn = new Perforce.I18nControls.GridButton();
			this.submitBtn = new Perforce.I18nControls.GridButton();
			this.FilesList = new Perforce.I18nControls.GridDoubleBufferedListView();
			this.ActionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.FileNameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.FilePathClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.NoBtn1 = new Perforce.I18nControls.GridButton();
			this.NoBtn2 = new Perforce.I18nControls.GridButton();
			this.YesBtn2 = new Perforce.I18nControls.GridButton();
			this.YesBtn1 = new Perforce.I18nControls.GridButton();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.BtnBar = new Perforce.I18nControls.GridLayoutSubpanel();
			this.BtnSpacer = new Perforce.I18nControls.GridPanel();
			this.gridLayoutPanel1.SuspendLayout();
			this.BtnBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// PromptLbl
			// 
			resources.ApplyResources(this.PromptLbl, "PromptLbl");
			this.PromptLbl.CellHeight = 13;
			this.PromptLbl.CellWidth = 348;
			this.PromptLbl.Column = 0;
			this.PromptLbl.ColumnsSpanned = 0;
			this.PromptLbl.Name = "PromptLbl";
			this.PromptLbl.Row = 2;
			this.PromptLbl.RowsSpanned = 0;
			this.PromptLbl.YOffset = 0;
			// 
			// ItemsCB
			// 
			resources.ApplyResources(this.ItemsCB, "ItemsCB");
			this.ItemsCB.CellHeight = 27;
			this.ItemsCB.CellWidth = 348;
			this.ItemsCB.Column = 0;
			this.ItemsCB.ColumnsSpanned = 0;
			this.ItemsCB.DesignSize = new System.Drawing.Size(0, 0);
			this.ItemsCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ItemsCB.FormattingEnabled = true;
			this.ItemsCB.Name = "ItemsCB";
			this.ItemsCB.Row = 3;
			this.ItemsCB.RowsSpanned = 0;
			this.ItemsCB.YOffset = 0;
			this.ItemsCB.SelectedIndexChanged += new System.EventHandler(this.ItemsCB_SelectedIndexChanged);
			// 
			// DontShowAgainCB
			// 
			resources.ApplyResources(this.DontShowAgainCB, "DontShowAgainCB");
			this.DontShowAgainCB.CellHeight = 23;
			this.DontShowAgainCB.CellWidth = 348;
			this.DontShowAgainCB.Column = 0;
			this.DontShowAgainCB.ColumnsSpanned = 0;
			this.DontShowAgainCB.Name = "DontShowAgainCB";
			this.DontShowAgainCB.Row = 6;
			this.DontShowAgainCB.RowsSpanned = 0;
			this.DontShowAgainCB.UseVisualStyleBackColor = true;
			this.DontShowAgainCB.YOffset = 0;
			this.DontShowAgainCB.CheckedChanged += new System.EventHandler(this.DontShowAgainCB_CheckedChanged);
			// 
			// DescriptionTB
			// 
			this.DescriptionTB.AcceptsReturn = true;
			resources.ApplyResources(this.DescriptionTB, "DescriptionTB");
			this.DescriptionTB.CellHeight = 70;
			this.DescriptionTB.CellWidth = 348;
			this.DescriptionTB.Column = 0;
			this.DescriptionTB.ColumnsSpanned = 0;
			this.DescriptionTB.Name = "DescriptionTB";
			this.DescriptionTB.ReadOnly = true;
			this.DescriptionTB.Row = 5;
			this.DescriptionTB.RowsSpanned = 0;
			this.DescriptionTB.YOffset = 0;
			this.DescriptionTB.TextChanged += new System.EventHandler(this.DescriptionTB_TextChanged);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.CellHeight = 13;
			this.label1.CellWidth = 348;
			this.label1.Column = 0;
			this.label1.ColumnsSpanned = 0;
			this.label1.Name = "label1";
			this.label1.Row = 4;
			this.label1.RowsSpanned = 0;
			this.label1.YOffset = 0;
			// 
			// msgText
			// 
			resources.ApplyResources(this.msgText, "msgText");
			this.msgText.CellHeight = 33;
			this.msgText.CellWidth = 348;
			this.msgText.Column = 0;
			this.msgText.ColumnsSpanned = 0;
			this.msgText.Name = "msgText";
			this.msgText.Row = 0;
			this.msgText.RowsSpanned = 0;
			this.msgText.YOffset = 0;
			// 
			// CancelBtn
			// 
			resources.ApplyResources(this.CancelBtn, "CancelBtn");
			this.CancelBtn.CellHeight = 0;
			this.CancelBtn.CellWidth = 0;
			this.CancelBtn.Column = 0;
			this.CancelBtn.ColumnsSpanned = 0;
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Row = 0;
			this.CancelBtn.RowsSpanned = 0;
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.YOffset = 0;
			// 
			// OkBtn
			// 
			resources.ApplyResources(this.OkBtn, "OkBtn");
			this.OkBtn.CellHeight = 0;
			this.OkBtn.CellWidth = 0;
			this.OkBtn.Column = 0;
			this.OkBtn.ColumnsSpanned = 0;
			this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkBtn.Name = "OkBtn";
			this.OkBtn.Row = 0;
			this.OkBtn.RowsSpanned = 0;
			this.OkBtn.UseVisualStyleBackColor = true;
			this.OkBtn.YOffset = 0;
			this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
			// 
			// saveToChangelistBtn
			// 
			resources.ApplyResources(this.saveToChangelistBtn, "saveToChangelistBtn");
			this.saveToChangelistBtn.CellHeight = 24;
			this.saveToChangelistBtn.CellWidth = 233;
			this.saveToChangelistBtn.Column = 0;
			this.saveToChangelistBtn.ColumnsSpanned = 0;
			this.saveToChangelistBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.saveToChangelistBtn.Name = "saveToChangelistBtn";
			this.saveToChangelistBtn.Row = 0;
			this.saveToChangelistBtn.RowsSpanned = 0;
			this.saveToChangelistBtn.UseVisualStyleBackColor = true;
			this.saveToChangelistBtn.YOffset = 0;
			// 
			// submitBtn
			// 
			resources.ApplyResources(this.submitBtn, "submitBtn");
			this.submitBtn.CellHeight = 0;
			this.submitBtn.CellWidth = 0;
			this.submitBtn.Column = 0;
			this.submitBtn.ColumnsSpanned = 0;
			this.submitBtn.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.submitBtn.Name = "submitBtn";
			this.submitBtn.Row = 0;
			this.submitBtn.RowsSpanned = 0;
			this.submitBtn.UseVisualStyleBackColor = true;
			this.submitBtn.YOffset = 0;
			// 
			// FilesList
			// 
			this.FilesList.AllowColumnReorder = true;
			resources.ApplyResources(this.FilesList, "FilesList");
			this.FilesList.CellHeight = 117;
			this.FilesList.CellWidth = 348;
			this.FilesList.Column = 0;
			this.FilesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ActionClm,
            this.FileNameClm,
            this.FilePathClm});
			this.FilesList.ColumnsSpanned = 0;
			this.FilesList.FullRowSelect = true;
			this.FilesList.HideActionsColumn = false;
			this.FilesList.Name = "FilesList";
			this.FilesList.OwnerDraw = true;
			this.FilesList.Row = 1;
			this.FilesList.RowsSpanned = 0;
			this.FilesList.UseCompatibleStateImageBehavior = false;
			this.FilesList.View = System.Windows.Forms.View.Details;
			this.FilesList.YOffset = 0;
			// 
			// ActionClm
			// 
			resources.ApplyResources(this.ActionClm, "ActionClm");
			// 
			// FileNameClm
			// 
			resources.ApplyResources(this.FileNameClm, "FileNameClm");
			// 
			// FilePathClm
			// 
			resources.ApplyResources(this.FilePathClm, "FilePathClm");
			// 
			// NoBtn1
			// 
			resources.ApplyResources(this.NoBtn1, "NoBtn1");
			this.NoBtn1.CellHeight = 0;
			this.NoBtn1.CellWidth = 0;
			this.NoBtn1.Column = 0;
			this.NoBtn1.ColumnsSpanned = 0;
			this.NoBtn1.DialogResult = System.Windows.Forms.DialogResult.No;
			this.NoBtn1.Name = "NoBtn1";
			this.NoBtn1.Row = 0;
			this.NoBtn1.RowsSpanned = 0;
			this.NoBtn1.UseVisualStyleBackColor = true;
			this.NoBtn1.YOffset = 0;
			// 
			// NoBtn2
			// 
			resources.ApplyResources(this.NoBtn2, "NoBtn2");
			this.NoBtn2.CellHeight = 0;
			this.NoBtn2.CellWidth = 0;
			this.NoBtn2.Column = 0;
			this.NoBtn2.ColumnsSpanned = 0;
			this.NoBtn2.DialogResult = System.Windows.Forms.DialogResult.No;
			this.NoBtn2.Name = "NoBtn2";
			this.NoBtn2.Row = 0;
			this.NoBtn2.RowsSpanned = 0;
			this.NoBtn2.UseVisualStyleBackColor = true;
			this.NoBtn2.YOffset = 0;
			// 
			// YesBtn2
			// 
			resources.ApplyResources(this.YesBtn2, "YesBtn2");
			this.YesBtn2.CellHeight = 0;
			this.YesBtn2.CellWidth = 0;
			this.YesBtn2.Column = 0;
			this.YesBtn2.ColumnsSpanned = 0;
			this.YesBtn2.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.YesBtn2.Name = "YesBtn2";
			this.YesBtn2.Row = 0;
			this.YesBtn2.RowsSpanned = 0;
			this.YesBtn2.UseVisualStyleBackColor = true;
			this.YesBtn2.YOffset = 0;
			this.YesBtn2.Click += new System.EventHandler(this.OkBtn_Click);
			// 
			// YesBtn1
			// 
			resources.ApplyResources(this.YesBtn1, "YesBtn1");
			this.YesBtn1.CellHeight = 0;
			this.YesBtn1.CellWidth = 0;
			this.YesBtn1.Column = 0;
			this.YesBtn1.ColumnsSpanned = 0;
			this.YesBtn1.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.YesBtn1.Name = "YesBtn1";
			this.YesBtn1.Row = 0;
			this.YesBtn1.RowsSpanned = 0;
			this.YesBtn1.UseVisualStyleBackColor = true;
			this.YesBtn1.YOffset = 0;
			// 
			// gridLayoutPanel1
			// 
			this.gridLayoutPanel1.Controls.Add(this.FilesList);
			this.gridLayoutPanel1.Controls.Add(this.msgText);
			this.gridLayoutPanel1.Controls.Add(this.label1);
			this.gridLayoutPanel1.Controls.Add(this.ItemsCB);
			this.gridLayoutPanel1.Controls.Add(this.PromptLbl);
			this.gridLayoutPanel1.Controls.Add(this.BtnBar);
			this.gridLayoutPanel1.Controls.Add(this.DescriptionTB);
			this.gridLayoutPanel1.Controls.Add(this.DontShowAgainCB);
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.EnableDesignerGrid = false;
			this.gridLayoutPanel1.EnableDesignerLayout = false;
			this.gridLayoutPanel1.EnableParentResize = false;
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			this.gridLayoutPanel1.AfterLayoutGrid += new Perforce.I18nControls.GridLayoutPanel.GridLayoutEvent(this.gridLayoutPanel1_AfterLayoutGrid);
			// 
			// BtnBar
			// 
			resources.ApplyResources(this.BtnBar, "BtnBar");
			this.BtnBar.CellHeight = 31;
			this.BtnBar.CellWidth = 348;
			this.BtnBar.Column = 0;
			this.BtnBar.ColumnsSpanned = 0;
			this.BtnBar.Controls.Add(this.BtnSpacer);
			this.BtnBar.Controls.Add(this.OkBtn);
			this.BtnBar.Controls.Add(this.NoBtn2);
			this.BtnBar.Controls.Add(this.NoBtn1);
			this.BtnBar.Controls.Add(this.YesBtn1);
			this.BtnBar.Controls.Add(this.YesBtn2);
			this.BtnBar.Controls.Add(this.CancelBtn);
			this.BtnBar.Controls.Add(this.saveToChangelistBtn);
			this.BtnBar.EnableDesignerGrid = false;
			this.BtnBar.EnableDesignerLayout = false;
			this.BtnBar.EnableParentResize = false;
			this.BtnBar.MinimumColumnWidth = 10;
			this.BtnBar.MinimumRowHeight = 10;
			this.BtnBar.Name = "BtnBar";
			this.BtnBar.Row = 7;
			this.BtnBar.RowsSpanned = 0;
			this.BtnBar.YOffset = 0;
			// 
			// BtnSpacer
			// 
			resources.ApplyResources(this.BtnSpacer, "BtnSpacer");
			this.BtnSpacer.CellHeight = 0;
			this.BtnSpacer.CellWidth = 0;
			this.BtnSpacer.Column = 0;
			this.BtnSpacer.ColumnsSpanned = 0;
			this.BtnSpacer.Name = "BtnSpacer";
			this.BtnSpacer.Row = 0;
			this.BtnSpacer.RowsSpanned = 0;
			this.BtnSpacer.YOffset = 0;
			// 
			// SelectChangelistDlg2
			// 
			this.AcceptButton = this.OkBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.Controls.Add(this.gridLayoutPanel1);
			this.Controls.Add(this.submitBtn);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectChangelistDlg2";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ChooseDlg_HelpButtonClicked);
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
			this.BtnBar.ResumeLayout(false);
			this.BtnBar.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private I18nControls.GridLabel PromptLbl;
		private I18nControls.GridComboBox ItemsCB;
		private I18nControls.GridCheckBox DontShowAgainCB;
		private I18nControls.GridTextBox DescriptionTB;
		private I18nControls.GridLabel label1;
		private I18nControls.GridLabel msgText;
		private I18nControls.GridButton CancelBtn;
		private I18nControls.GridButton OkBtn;
		private I18nControls.GridButton saveToChangelistBtn;
		private I18nControls.GridButton submitBtn;
		private I18nControls.GridDoubleBufferedListView FilesList;
		private I18nControls.GridButton NoBtn1;
		private I18nControls.GridButton NoBtn2;
		private I18nControls.GridButton YesBtn2;
		private I18nControls.GridButton YesBtn1;
		private System.Windows.Forms.ColumnHeader FileNameClm;
		private System.Windows.Forms.ColumnHeader FilePathClm;
		private System.Windows.Forms.ColumnHeader ActionClm;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridLayoutSubpanel BtnBar;
		private I18nControls.GridPanel BtnSpacer;
	}
}