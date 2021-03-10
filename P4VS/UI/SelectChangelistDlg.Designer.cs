namespace Perforce.P4VS
{
	partial class SelectChangelistDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectChangelistDlg));
			this.OkBtn = new Perforce.I18nControls.GridButton();
			this.CancelBtn = new Perforce.I18nControls.GridButton();
			this.ApplyToAllChk = new Perforce.I18nControls.GridCheckBox();
			this.SaveAsBtn = new Perforce.I18nControls.GridButton();
			this.SkipSaveBtn = new Perforce.I18nControls.GridButton();
			this.CheckoutAndSaveBtn = new Perforce.I18nControls.GridButton();
			this.EditFileBtn = new Perforce.I18nControls.GridButton();
			this.CancelEditBtn = new Perforce.I18nControls.GridButton();
			this.submitBtn = new Perforce.I18nControls.GridButton();
			this.msgText = new Perforce.I18nControls.GridLabel();
			this.label1 = new Perforce.I18nControls.GridLabel();
			this.DescriptionTB = new Perforce.I18nControls.GridTextBox();
			this.DontShowAgainCB = new Perforce.I18nControls.GridCheckBox();
			this.ItemsCB = new Perforce.I18nControls.GridComboBox();
			this.PromptLbl = new Perforce.I18nControls.GridLabel();
			this.CheckoutBtn = new Perforce.I18nControls.GridButton();
			this.CancelSaveBtn = new Perforce.I18nControls.GridButton();
			this.saveToChangelistBtn = new Perforce.I18nControls.GridButton();
			this.BtnPanel = new Perforce.I18nControls.GridLayoutSubpanel();
			this.BtnPanelBuffer = new Perforce.I18nControls.GridPanel();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			this.BtnPanel.SuspendLayout();
			this.gridLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
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
			// ApplyToAllChk
			// 
			resources.ApplyResources(this.ApplyToAllChk, "ApplyToAllChk");
			this.ApplyToAllChk.CellHeight = 23;
			this.ApplyToAllChk.CellWidth = 395;
			this.ApplyToAllChk.Column = 0;
			this.ApplyToAllChk.ColumnsSpanned = 0;
			this.ApplyToAllChk.Name = "ApplyToAllChk";
			this.ApplyToAllChk.Row = 5;
			this.ApplyToAllChk.RowsSpanned = 0;
			this.ApplyToAllChk.UseVisualStyleBackColor = true;
			this.ApplyToAllChk.YOffset = 0;
			this.ApplyToAllChk.CheckedChanged += new System.EventHandler(this.ApplyToAllChk_CheckedChanged);
			// 
			// SaveAsBtn
			// 
			resources.ApplyResources(this.SaveAsBtn, "SaveAsBtn");
			this.SaveAsBtn.CellHeight = 23;
			this.SaveAsBtn.CellWidth = 78;
			this.SaveAsBtn.Column = 2;
			this.SaveAsBtn.ColumnsSpanned = 0;
			this.SaveAsBtn.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.SaveAsBtn.Name = "SaveAsBtn";
			this.SaveAsBtn.Row = 0;
			this.SaveAsBtn.RowsSpanned = 0;
			this.SaveAsBtn.UseVisualStyleBackColor = true;
			this.SaveAsBtn.YOffset = 0;
			// 
			// SkipSaveBtn
			// 
			resources.ApplyResources(this.SkipSaveBtn, "SkipSaveBtn");
			this.SkipSaveBtn.CellHeight = 23;
			this.SkipSaveBtn.CellWidth = 98;
			this.SkipSaveBtn.Column = 1;
			this.SkipSaveBtn.ColumnsSpanned = 0;
			this.SkipSaveBtn.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			this.SkipSaveBtn.Name = "SkipSaveBtn";
			this.SkipSaveBtn.Row = 0;
			this.SkipSaveBtn.RowsSpanned = 0;
			this.SkipSaveBtn.UseVisualStyleBackColor = true;
			this.SkipSaveBtn.YOffset = 0;
			// 
			// CheckoutAndSaveBtn
			// 
			resources.ApplyResources(this.CheckoutAndSaveBtn, "CheckoutAndSaveBtn");
			this.CheckoutAndSaveBtn.CellHeight = 0;
			this.CheckoutAndSaveBtn.CellWidth = 0;
			this.CheckoutAndSaveBtn.Column = 0;
			this.CheckoutAndSaveBtn.ColumnsSpanned = 0;
			this.CheckoutAndSaveBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.CheckoutAndSaveBtn.Name = "CheckoutAndSaveBtn";
			this.CheckoutAndSaveBtn.Row = 0;
			this.CheckoutAndSaveBtn.RowsSpanned = 0;
			this.CheckoutAndSaveBtn.UseVisualStyleBackColor = true;
			this.CheckoutAndSaveBtn.YOffset = 0;
			// 
			// EditFileBtn
			// 
			resources.ApplyResources(this.EditFileBtn, "EditFileBtn");
			this.EditFileBtn.CellHeight = 0;
			this.EditFileBtn.CellWidth = 0;
			this.EditFileBtn.Column = 0;
			this.EditFileBtn.ColumnsSpanned = 0;
			this.EditFileBtn.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			this.EditFileBtn.Name = "EditFileBtn";
			this.EditFileBtn.Row = 0;
			this.EditFileBtn.RowsSpanned = 0;
			this.EditFileBtn.UseVisualStyleBackColor = true;
			this.EditFileBtn.YOffset = 0;
			// 
			// CancelEditBtn
			// 
			this.CancelEditBtn.AllowDrop = true;
			resources.ApplyResources(this.CancelEditBtn, "CancelEditBtn");
			this.CancelEditBtn.CellHeight = 0;
			this.CancelEditBtn.CellWidth = 0;
			this.CancelEditBtn.Column = 0;
			this.CancelEditBtn.ColumnsSpanned = 0;
			this.CancelEditBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelEditBtn.Name = "CancelEditBtn";
			this.CancelEditBtn.Row = 0;
			this.CancelEditBtn.RowsSpanned = 0;
			this.CancelEditBtn.UseVisualStyleBackColor = true;
			this.CancelEditBtn.YOffset = 0;
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
			// msgText
			// 
			resources.ApplyResources(this.msgText, "msgText");
			this.msgText.CellHeight = 51;
			this.msgText.CellWidth = 395;
			this.msgText.Column = 0;
			this.msgText.ColumnsSpanned = 0;
			this.msgText.Name = "msgText";
			this.msgText.Row = 0;
			this.msgText.RowsSpanned = 0;
			this.msgText.YOffset = 0;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.CellHeight = 13;
			this.label1.CellWidth = 395;
			this.label1.Column = 0;
			this.label1.ColumnsSpanned = 0;
			this.label1.Name = "label1";
			this.label1.Row = 3;
			this.label1.RowsSpanned = 0;
			this.label1.YOffset = 0;
			// 
			// DescriptionTB
			// 
			this.DescriptionTB.AcceptsReturn = true;
			resources.ApplyResources(this.DescriptionTB, "DescriptionTB");
			this.DescriptionTB.CellHeight = 110;
			this.DescriptionTB.CellWidth = 395;
			this.DescriptionTB.Column = 0;
			this.DescriptionTB.ColumnsSpanned = 0;
			this.DescriptionTB.Name = "DescriptionTB";
			this.DescriptionTB.ReadOnly = true;
			this.DescriptionTB.Row = 4;
			this.DescriptionTB.RowsSpanned = 0;
			this.DescriptionTB.YOffset = 0;
			this.DescriptionTB.TextChanged += new System.EventHandler(this.DescriptionTB_TextChanged);
			// 
			// DontShowAgainCB
			// 
			resources.ApplyResources(this.DontShowAgainCB, "DontShowAgainCB");
			this.DontShowAgainCB.CellHeight = 23;
			this.DontShowAgainCB.CellWidth = 395;
			this.DontShowAgainCB.Column = 0;
			this.DontShowAgainCB.ColumnsSpanned = 0;
			this.DontShowAgainCB.Name = "DontShowAgainCB";
			this.DontShowAgainCB.Row = 6;
			this.DontShowAgainCB.RowsSpanned = 0;
			this.DontShowAgainCB.UseVisualStyleBackColor = true;
			this.DontShowAgainCB.YOffset = 0;
			this.DontShowAgainCB.CheckedChanged += new System.EventHandler(this.DontShowAgainCB_CheckedChanged);
			// 
			// ItemsCB
			// 
			resources.ApplyResources(this.ItemsCB, "ItemsCB");
			this.ItemsCB.CellHeight = 27;
			this.ItemsCB.CellWidth = 395;
			this.ItemsCB.Column = 0;
			this.ItemsCB.ColumnsSpanned = 0;
			this.ItemsCB.DesignSize = new System.Drawing.Size(0, 0);
			this.ItemsCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ItemsCB.FormattingEnabled = true;
			this.ItemsCB.Name = "ItemsCB";
			this.ItemsCB.Row = 2;
			this.ItemsCB.RowsSpanned = 0;
			this.ItemsCB.YOffset = 0;
			this.ItemsCB.SelectedIndexChanged += new System.EventHandler(this.ItemsCB_SelectedIndexChanged);
			// 
			// PromptLbl
			// 
			resources.ApplyResources(this.PromptLbl, "PromptLbl");
			this.PromptLbl.CellHeight = 13;
			this.PromptLbl.CellWidth = 395;
			this.PromptLbl.Column = 0;
			this.PromptLbl.ColumnsSpanned = 0;
			this.PromptLbl.Name = "PromptLbl";
			this.PromptLbl.Row = 1;
			this.PromptLbl.RowsSpanned = 0;
			this.PromptLbl.YOffset = 0;
			// 
			// CheckoutBtn
			// 
			resources.ApplyResources(this.CheckoutBtn, "CheckoutBtn");
			this.CheckoutBtn.CellHeight = 0;
			this.CheckoutBtn.CellWidth = 0;
			this.CheckoutBtn.Column = 0;
			this.CheckoutBtn.ColumnsSpanned = 0;
			this.CheckoutBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.CheckoutBtn.Name = "CheckoutBtn";
			this.CheckoutBtn.Row = 0;
			this.CheckoutBtn.RowsSpanned = 0;
			this.CheckoutBtn.UseVisualStyleBackColor = true;
			this.CheckoutBtn.YOffset = 0;
			// 
			// CancelSaveBtn
			// 
			resources.ApplyResources(this.CancelSaveBtn, "CancelSaveBtn");
			this.CancelSaveBtn.CellHeight = 0;
			this.CancelSaveBtn.CellWidth = 0;
			this.CancelSaveBtn.Column = 0;
			this.CancelSaveBtn.ColumnsSpanned = 0;
			this.CancelSaveBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelSaveBtn.Name = "CancelSaveBtn";
			this.CancelSaveBtn.Row = 0;
			this.CancelSaveBtn.RowsSpanned = 0;
			this.CancelSaveBtn.UseVisualStyleBackColor = true;
			this.CancelSaveBtn.YOffset = 0;
			// 
			// saveToChangelistBtn
			// 
			resources.ApplyResources(this.saveToChangelistBtn, "saveToChangelistBtn");
			this.saveToChangelistBtn.CellHeight = 0;
			this.saveToChangelistBtn.CellWidth = 0;
			this.saveToChangelistBtn.Column = 0;
			this.saveToChangelistBtn.ColumnsSpanned = 0;
			this.saveToChangelistBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.saveToChangelistBtn.Name = "saveToChangelistBtn";
			this.saveToChangelistBtn.Row = 0;
			this.saveToChangelistBtn.RowsSpanned = 0;
			this.saveToChangelistBtn.UseVisualStyleBackColor = true;
			this.saveToChangelistBtn.YOffset = 0;
			// 
			// BtnPanel
			// 
			resources.ApplyResources(this.BtnPanel, "BtnPanel");
			this.BtnPanel.CellHeight = 29;
			this.BtnPanel.CellWidth = 395;
			this.BtnPanel.Column = 0;
			this.BtnPanel.ColumnsSpanned = 0;
			this.BtnPanel.Controls.Add(this.saveToChangelistBtn);
			this.BtnPanel.Controls.Add(this.CheckoutAndSaveBtn);
			this.BtnPanel.Controls.Add(this.SkipSaveBtn);
			this.BtnPanel.Controls.Add(this.SaveAsBtn);
			this.BtnPanel.Controls.Add(this.CancelSaveBtn);
			this.BtnPanel.Controls.Add(this.CheckoutBtn);
			this.BtnPanel.Controls.Add(this.EditFileBtn);
			this.BtnPanel.Controls.Add(this.CancelEditBtn);
			this.BtnPanel.Controls.Add(this.submitBtn);
			this.BtnPanel.Controls.Add(this.OkBtn);
			this.BtnPanel.Controls.Add(this.CancelBtn);
			this.BtnPanel.Controls.Add(this.BtnPanelBuffer);
			this.BtnPanel.EnableDesignerGrid = false;
			this.BtnPanel.EnableDesignerLayout = false;
			this.BtnPanel.EnableParentResize = false;
			this.BtnPanel.MinimumColumnWidth = 0;
			this.BtnPanel.MinimumRowHeight = 0;
			this.BtnPanel.Name = "BtnPanel";
			this.BtnPanel.Row = 7;
			this.BtnPanel.RowsSpanned = 0;
			this.BtnPanel.YOffset = 0;
			// 
			// BtnPanelBuffer
			// 
			resources.ApplyResources(this.BtnPanelBuffer, "BtnPanelBuffer");
			this.BtnPanelBuffer.CellHeight = 23;
			this.BtnPanelBuffer.CellWidth = 213;
			this.BtnPanelBuffer.Column = 0;
			this.BtnPanelBuffer.ColumnsSpanned = 0;
			this.BtnPanelBuffer.Name = "BtnPanelBuffer";
			this.BtnPanelBuffer.Row = 0;
			this.BtnPanelBuffer.RowsSpanned = 0;
			this.BtnPanelBuffer.YOffset = 3;
			// 
			// gridLayoutPanel1
			// 
			this.gridLayoutPanel1.Controls.Add(this.BtnPanel);
			this.gridLayoutPanel1.Controls.Add(this.ApplyToAllChk);
			this.gridLayoutPanel1.Controls.Add(this.msgText);
			this.gridLayoutPanel1.Controls.Add(this.label1);
			this.gridLayoutPanel1.Controls.Add(this.DescriptionTB);
			this.gridLayoutPanel1.Controls.Add(this.DontShowAgainCB);
			this.gridLayoutPanel1.Controls.Add(this.ItemsCB);
			this.gridLayoutPanel1.Controls.Add(this.PromptLbl);
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.EnableDesignerGrid = false;
			this.gridLayoutPanel1.EnableDesignerLayout = false;
			this.gridLayoutPanel1.EnableParentResize = false;
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			this.gridLayoutPanel1.AfterLayoutGrid += new Perforce.I18nControls.GridLayoutPanel.GridLayoutEvent(this.gridLayoutPanel1_AfterLayoutGrid);
			// 
			// SelectChangelistDlg
			// 
			this.AcceptButton = this.OkBtn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelSaveBtn;
			this.Controls.Add(this.gridLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectChangelistDlg";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ChooseDlg_HelpButtonClicked);
			this.BtnPanel.ResumeLayout(false);
			this.BtnPanel.PerformLayout();
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
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
		private I18nControls.GridButton CheckoutBtn;
		private I18nControls.GridButton EditFileBtn;
		private I18nControls.GridButton CancelEditBtn;
		private I18nControls.GridButton CheckoutAndSaveBtn;
		private I18nControls.GridButton CancelSaveBtn;
		private I18nControls.GridButton SaveAsBtn;
		private I18nControls.GridButton SkipSaveBtn;
		private I18nControls.GridButton saveToChangelistBtn;
		private I18nControls.GridButton submitBtn;
		private I18nControls.GridCheckBox ApplyToAllChk;
		private I18nControls.GridLayoutSubpanel BtnPanel;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridPanel BtnPanelBuffer;
	}
}