namespace Perforce.P4VS
{
    partial class QueryEditSyncWarningDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryEditSyncWarningDlg));
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel2 = new Perforce.I18nControls.GridPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.YesBtn = new Perforce.I18nControls.GridButton();
            this.NoBtn = new Perforce.I18nControls.GridButton();
            this.gridLabel1 = new Perforce.I18nControls.GridLabel();
            this.AlwaysSyncCB = new Perforce.I18nControls.GridCheckBox();
            this.NeverSyncCB = new Perforce.I18nControls.GridCheckBox();
            this.WarningLbl = new Perforce.I18nControls.GridLabel();
            this.gridLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridLayoutPanel1
            // 
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.Controls.Add(this.gridPanel2);
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.YesBtn);
            this.gridLayoutPanel1.Controls.Add(this.NoBtn);
            this.gridLayoutPanel1.Controls.Add(this.gridLabel1);
            this.gridLayoutPanel1.Controls.Add(this.AlwaysSyncCB);
            this.gridLayoutPanel1.Controls.Add(this.NeverSyncCB);
            this.gridLayoutPanel1.Controls.Add(this.WarningLbl);
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = false;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // gridPanel2
            // 
            resources.ApplyResources(this.gridPanel2, "gridPanel2");
            this.gridPanel2.CellHeight = 26;
            this.gridPanel2.CellWidth = 401;
            this.gridPanel2.Column = 0;
            this.gridPanel2.ColumnsSpanned = 2;
            this.gridPanel2.Name = "gridPanel2";
            this.gridPanel2.Row = 1;
            this.gridPanel2.RowsSpanned = 0;
            this.gridPanel2.YOffset = 0;
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.CellHeight = 26;
            this.gridPanel1.CellWidth = 169;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 5;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // YesBtn
            // 
            resources.ApplyResources(this.YesBtn, "YesBtn");
            this.YesBtn.CellHeight = 26;
            this.YesBtn.CellWidth = 116;
            this.YesBtn.Column = 1;
            this.YesBtn.ColumnsSpanned = 0;
            this.YesBtn.Name = "YesBtn";
            this.YesBtn.Row = 5;
            this.YesBtn.RowsSpanned = 0;
            this.YesBtn.UseVisualStyleBackColor = true;
            this.YesBtn.YOffset = 0;
            this.YesBtn.Click += new System.EventHandler(this.YesBtn_Click);
            // 
            // NoBtn
            // 
            resources.ApplyResources(this.NoBtn, "NoBtn");
            this.NoBtn.CellHeight = 26;
            this.NoBtn.CellWidth = 116;
            this.NoBtn.Column = 2;
            this.NoBtn.ColumnsSpanned = 0;
            this.NoBtn.Name = "NoBtn";
            this.NoBtn.Row = 5;
            this.NoBtn.RowsSpanned = 0;
            this.NoBtn.UseVisualStyleBackColor = true;
            this.NoBtn.YOffset = 0;
            this.NoBtn.Click += new System.EventHandler(this.NoBtn_Click);
            // 
            // gridLabel1
            // 
            resources.ApplyResources(this.gridLabel1, "gridLabel1");
            this.gridLabel1.CellHeight = 13;
            this.gridLabel1.CellWidth = 401;
            this.gridLabel1.Column = 0;
            this.gridLabel1.ColumnsSpanned = 2;
            this.gridLabel1.Name = "gridLabel1";
            this.gridLabel1.Row = 4;
            this.gridLabel1.RowsSpanned = 0;
            this.gridLabel1.YOffset = 0;
            // 
            // AlwaysSyncCB
            // 
            resources.ApplyResources(this.AlwaysSyncCB, "AlwaysSyncCB");
            this.AlwaysSyncCB.CellHeight = 23;
            this.AlwaysSyncCB.CellWidth = 401;
            this.AlwaysSyncCB.Column = 0;
            this.AlwaysSyncCB.ColumnsSpanned = 2;
            this.AlwaysSyncCB.Name = "AlwaysSyncCB";
            this.AlwaysSyncCB.Row = 3;
            this.AlwaysSyncCB.RowsSpanned = 0;
            this.AlwaysSyncCB.UseVisualStyleBackColor = true;
            this.AlwaysSyncCB.YOffset = 0;
            this.AlwaysSyncCB.CheckedChanged += new System.EventHandler(this.AlwaysSyncCB_CheckedChanged);
            // 
            // NeverSyncCB
            // 
            resources.ApplyResources(this.NeverSyncCB, "NeverSyncCB");
            this.NeverSyncCB.CellHeight = 23;
            this.NeverSyncCB.CellWidth = 401;
            this.NeverSyncCB.Column = 0;
            this.NeverSyncCB.ColumnsSpanned = 2;
            this.NeverSyncCB.Name = "NeverSyncCB";
            this.NeverSyncCB.Row = 2;
            this.NeverSyncCB.RowsSpanned = 0;
            this.NeverSyncCB.UseVisualStyleBackColor = true;
            this.NeverSyncCB.YOffset = 0;
            this.NeverSyncCB.CheckedChanged += new System.EventHandler(this.NeverSyncCB_CheckedChanged);
            // 
            // WarningLbl
            // 
            resources.ApplyResources(this.WarningLbl, "WarningLbl");
            this.WarningLbl.CellHeight = 39;
            this.WarningLbl.CellWidth = 401;
            this.WarningLbl.Column = 0;
            this.WarningLbl.ColumnsSpanned = 2;
            this.WarningLbl.Name = "WarningLbl";
            this.WarningLbl.Row = 0;
            this.WarningLbl.RowsSpanned = 0;
            this.WarningLbl.YOffset = 0;
            // 
            // QueryEditSyncWarningDlg
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.gridLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "QueryEditSyncWarningDlg";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridLabel gridLabel1;
        private I18nControls.GridCheckBox AlwaysSyncCB;
        private I18nControls.GridCheckBox NeverSyncCB;
        private I18nControls.GridLabel WarningLbl;
        private I18nControls.GridButton NoBtn;
        private I18nControls.GridPanel gridPanel1;
        private I18nControls.GridButton YesBtn;
        private I18nControls.GridPanel gridPanel2;
    }
}