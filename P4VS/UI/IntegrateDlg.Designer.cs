namespace Perforce.P4VS
{
    partial class IntegrateDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IntegrateDlg));
            this.actionBtn = new Perforce.I18nControls.GridButton();
            this.cancelBtn = new Perforce.I18nControls.GridButton();
            this.previewBtn = new Perforce.I18nControls.GridButton();
            this.methodLbl = new Perforce.I18nControls.GridLabel();
            this.sourceLbl = new Perforce.I18nControls.GridLabel();
            this.targetLbl = new Perforce.I18nControls.GridLabel();
            this.methodTB = new Perforce.I18nControls.GridTextBox();
            this.targetTB = new Perforce.I18nControls.GridTextBox();
            this.previewTB = new Perforce.I18nControls.GridTextBox();
            this.sourceCB = new Perforce.I18nControls.GridComboBox();
            this.sourceTB = new Perforce.I18nControls.GridTextBox();
            this.addToPendingRB = new Perforce.I18nControls.GridRadioButton();
            this.submitCopiedRB = new Perforce.I18nControls.GridRadioButton();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel2 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.gridPanel3 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutSubpanel2 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridPanel2.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.gridLayoutSubpanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // actionBtn
            // 
            resources.ApplyResources(this.actionBtn, "actionBtn");
            this.actionBtn.CellHeight = 24;
            this.actionBtn.CellWidth = 72;
            this.actionBtn.Column = 1;
            this.actionBtn.ColumnsSpanned = 0;
            this.actionBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.actionBtn.Name = "actionBtn";
            this.actionBtn.Row = 0;
            this.actionBtn.RowsSpanned = 0;
            this.actionBtn.UseVisualStyleBackColor = true;
            this.actionBtn.YOffset = 0;
            this.actionBtn.Click += new System.EventHandler(this.actionBtn_Click);
            // 
            // cancelBtn
            // 
            resources.ApplyResources(this.cancelBtn, "cancelBtn");
            this.cancelBtn.CellHeight = 24;
            this.cancelBtn.CellWidth = 69;
            this.cancelBtn.Column = 3;
            this.cancelBtn.ColumnsSpanned = 0;
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Row = 0;
            this.cancelBtn.RowsSpanned = 0;
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.YOffset = 0;
            // 
            // previewBtn
            // 
            resources.ApplyResources(this.previewBtn, "previewBtn");
            this.previewBtn.CellHeight = 24;
            this.previewBtn.CellWidth = 72;
            this.previewBtn.Column = 2;
            this.previewBtn.ColumnsSpanned = 0;
            this.previewBtn.Name = "previewBtn";
            this.previewBtn.Row = 0;
            this.previewBtn.RowsSpanned = 0;
            this.previewBtn.UseVisualStyleBackColor = true;
            this.previewBtn.YOffset = 0;
            this.previewBtn.Click += new System.EventHandler(this.previewBtn_Click);
            // 
            // methodLbl
            // 
            resources.ApplyResources(this.methodLbl, "methodLbl");
            this.methodLbl.CellHeight = 26;
            this.methodLbl.CellWidth = 86;
            this.methodLbl.Column = 0;
            this.methodLbl.ColumnsSpanned = 0;
            this.methodLbl.Name = "methodLbl";
            this.methodLbl.Row = 0;
            this.methodLbl.RowsSpanned = 0;
            this.methodLbl.YOffset = 3;
            // 
            // sourceLbl
            // 
            resources.ApplyResources(this.sourceLbl, "sourceLbl");
            this.sourceLbl.CellHeight = 26;
            this.sourceLbl.CellWidth = 86;
            this.sourceLbl.Column = 0;
            this.sourceLbl.ColumnsSpanned = 0;
            this.sourceLbl.Name = "sourceLbl";
            this.sourceLbl.Row = 1;
            this.sourceLbl.RowsSpanned = 0;
            this.sourceLbl.YOffset = 3;
            // 
            // targetLbl
            // 
            resources.ApplyResources(this.targetLbl, "targetLbl");
            this.targetLbl.CellHeight = 26;
            this.targetLbl.CellWidth = 86;
            this.targetLbl.Column = 0;
            this.targetLbl.ColumnsSpanned = 0;
            this.targetLbl.Name = "targetLbl";
            this.targetLbl.Row = 2;
            this.targetLbl.RowsSpanned = 0;
            this.targetLbl.YOffset = 3;
            // 
            // methodTB
            // 
            resources.ApplyResources(this.methodTB, "methodTB");
            this.methodTB.CellHeight = 26;
            this.methodTB.CellWidth = 430;
            this.methodTB.Column = 1;
            this.methodTB.ColumnsSpanned = 0;
            this.methodTB.Name = "methodTB";
            this.methodTB.ReadOnly = true;
            this.methodTB.Row = 0;
            this.methodTB.RowsSpanned = 0;
            this.methodTB.YOffset = 0;
            // 
            // targetTB
            // 
            resources.ApplyResources(this.targetTB, "targetTB");
            this.targetTB.CellHeight = 26;
            this.targetTB.CellWidth = 430;
            this.targetTB.Column = 1;
            this.targetTB.ColumnsSpanned = 0;
            this.targetTB.Name = "targetTB";
            this.targetTB.ReadOnly = true;
            this.targetTB.Row = 2;
            this.targetTB.RowsSpanned = 0;
            this.targetTB.YOffset = 0;
            // 
            // previewTB
            // 
            resources.ApplyResources(this.previewTB, "previewTB");
            this.previewTB.CellHeight = 255;
            this.previewTB.CellWidth = 516;
            this.previewTB.Column = 0;
            this.previewTB.ColumnsSpanned = 1;
            this.previewTB.Name = "previewTB";
            this.previewTB.ReadOnly = true;
            this.previewTB.Row = 5;
            this.previewTB.RowsSpanned = 0;
            this.previewTB.YOffset = 0;
            // 
            // sourceCB
            // 
            this.sourceCB.CellHeight = 0;
            this.sourceCB.CellWidth = 0;
            this.sourceCB.Column = 1;
            this.sourceCB.ColumnsSpanned = 0;
            this.sourceCB.DesignSize = new System.Drawing.Size(0, 0);
            resources.ApplyResources(this.sourceCB, "sourceCB");
            this.sourceCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sourceCB.FormattingEnabled = true;
            this.sourceCB.Name = "sourceCB";
            this.sourceCB.Row = 0;
            this.sourceCB.RowsSpanned = 0;
            this.sourceCB.YOffset = 0;
            this.sourceCB.SelectedIndexChanged += new System.EventHandler(this.sourceCB_SelectedIndexChanged);
            // 
            // sourceTB
            // 
            this.sourceTB.CellHeight = 0;
            this.sourceTB.CellWidth = 0;
            this.sourceTB.Column = 0;
            this.sourceTB.ColumnsSpanned = 0;
            resources.ApplyResources(this.sourceTB, "sourceTB");
            this.sourceTB.Name = "sourceTB";
            this.sourceTB.ReadOnly = true;
            this.sourceTB.Row = 0;
            this.sourceTB.RowsSpanned = 0;
            this.sourceTB.YOffset = 0;
            // 
            // addToPendingRB
            // 
            resources.ApplyResources(this.addToPendingRB, "addToPendingRB");
            this.addToPendingRB.CellHeight = 17;
            this.addToPendingRB.CellWidth = 172;
            this.addToPendingRB.Column = 0;
            this.addToPendingRB.ColumnsSpanned = 0;
            this.addToPendingRB.Name = "addToPendingRB";
            this.addToPendingRB.Row = 0;
            this.addToPendingRB.RowsSpanned = 0;
            this.addToPendingRB.UseVisualStyleBackColor = true;
            this.addToPendingRB.YOffset = 0;
            this.addToPendingRB.CheckedChanged += new System.EventHandler(this.addToPendingRB_CheckedChanged);
            // 
            // submitCopiedRB
            // 
            resources.ApplyResources(this.submitCopiedRB, "submitCopiedRB");
            this.submitCopiedRB.CellHeight = 17;
            this.submitCopiedRB.CellWidth = 179;
            this.submitCopiedRB.Column = 1;
            this.submitCopiedRB.ColumnsSpanned = 0;
            this.submitCopiedRB.Name = "submitCopiedRB";
            this.submitCopiedRB.Row = 0;
            this.submitCopiedRB.RowsSpanned = 0;
            this.submitCopiedRB.TabStop = true;
            this.submitCopiedRB.UseVisualStyleBackColor = true;
            this.submitCopiedRB.YOffset = 0;
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.gridPanel2);
            this.gridLayoutPanel1.Controls.Add(this.previewTB);
            this.gridLayoutPanel1.Controls.Add(this.targetTB);
            this.gridLayoutPanel1.Controls.Add(this.methodTB);
            this.gridLayoutPanel1.Controls.Add(this.targetLbl);
            this.gridLayoutPanel1.Controls.Add(this.sourceLbl);
            this.gridLayoutPanel1.Controls.Add(this.methodLbl);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel2);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // gridPanel2
            // 
            resources.ApplyResources(this.gridPanel2, "gridPanel2");
            this.gridPanel2.CellHeight = 26;
            this.gridPanel2.CellWidth = 430;
            this.gridPanel2.Column = 1;
            this.gridPanel2.ColumnsSpanned = 0;
            this.gridPanel2.Controls.Add(this.sourceTB);
            this.gridPanel2.Controls.Add(this.sourceCB);
            this.gridPanel2.Name = "gridPanel2";
            this.gridPanel2.Row = 1;
            this.gridPanel2.RowsSpanned = 0;
            this.gridPanel2.YOffset = 0;
            // 
            // gridLayoutSubpanel1
            // 
            this.gridLayoutSubpanel1.CellHeight = 23;
            this.gridLayoutSubpanel1.CellWidth = 516;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 1;
            this.gridLayoutSubpanel1.Controls.Add(this.gridPanel3);
            this.gridLayoutSubpanel1.Controls.Add(this.addToPendingRB);
            this.gridLayoutSubpanel1.Controls.Add(this.submitCopiedRB);
            this.gridLayoutSubpanel1.EnableDesignerGrid = false;
            this.gridLayoutSubpanel1.EnableDesignerLayout = true;
            this.gridLayoutSubpanel1.EnableParentResize = false;
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel1.MinimumRowHeight = 10;
            this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
            this.gridLayoutSubpanel1.Row = 3;
            this.gridLayoutSubpanel1.RowsSpanned = 0;
            this.gridLayoutSubpanel1.YOffset = 0;
            // 
            // gridPanel3
            // 
            resources.ApplyResources(this.gridPanel3, "gridPanel3");
            this.gridPanel3.CellHeight = 17;
            this.gridPanel3.CellWidth = 147;
            this.gridPanel3.Column = 2;
            this.gridPanel3.ColumnsSpanned = 0;
            this.gridPanel3.Name = "gridPanel3";
            this.gridPanel3.Row = 0;
            this.gridPanel3.RowsSpanned = 0;
            this.gridPanel3.YOffset = 3;
            // 
            // gridLayoutSubpanel2
            // 
            resources.ApplyResources(this.gridLayoutSubpanel2, "gridLayoutSubpanel2");
            this.gridLayoutSubpanel2.CellHeight = 31;
            this.gridLayoutSubpanel2.CellWidth = 516;
            this.gridLayoutSubpanel2.Column = 0;
            this.gridLayoutSubpanel2.ColumnsSpanned = 1;
            this.gridLayoutSubpanel2.Controls.Add(this.gridPanel1);
            this.gridLayoutSubpanel2.Controls.Add(this.actionBtn);
            this.gridLayoutSubpanel2.Controls.Add(this.cancelBtn);
            this.gridLayoutSubpanel2.Controls.Add(this.previewBtn);
            this.gridLayoutSubpanel2.EnableDesignerGrid = false;
            this.gridLayoutSubpanel2.EnableDesignerLayout = true;
            this.gridLayoutSubpanel2.EnableParentResize = false;
            this.gridLayoutSubpanel2.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel2.MinimumRowHeight = 10;
            this.gridLayoutSubpanel2.Name = "gridLayoutSubpanel2";
            this.gridLayoutSubpanel2.Row = 4;
            this.gridLayoutSubpanel2.RowsSpanned = 0;
            this.gridLayoutSubpanel2.YOffset = 0;
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.CellHeight = 24;
            this.gridPanel1.CellWidth = 297;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 0;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 2;
            // 
            // IntegrateDlg
            // 
            this.AcceptButton = this.actionBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.Controls.Add(this.gridLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IntegrateDlg";
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.gridPanel2.ResumeLayout(false);
            this.gridPanel2.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.gridLayoutSubpanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

		private I18nControls.GridButton actionBtn;
		private I18nControls.GridButton cancelBtn;
		private I18nControls.GridButton previewBtn;
		private I18nControls.GridLabel methodLbl;
		private I18nControls.GridLabel sourceLbl;
		private I18nControls.GridLabel targetLbl;
		private I18nControls.GridTextBox methodTB;
		private I18nControls.GridTextBox targetTB;
		private I18nControls.GridTextBox previewTB;
		private I18nControls.GridComboBox sourceCB;
		private I18nControls.GridTextBox sourceTB;
		private I18nControls.GridRadioButton addToPendingRB;
		private I18nControls.GridRadioButton submitCopiedRB;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel2;
		private I18nControls.GridPanel gridPanel1;
		private I18nControls.GridPanel gridPanel2;
		private I18nControls.GridPanel gridPanel3;
    }
}