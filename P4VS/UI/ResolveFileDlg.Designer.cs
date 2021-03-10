namespace Perforce.P4VS
{
	partial class ResolveFileDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResolveFileDlg));
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.gridPanel2 = new Perforce.I18nControls.GridPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.InteractiveResolveRB = new Perforce.I18nControls.GridRadioButton();
            this.AutoResolveRB = new Perforce.I18nControls.GridRadioButton();
            this.MergeBinaryCB = new Perforce.I18nControls.GridCheckBox();
            this.SelectionCountLbl = new Perforce.I18nControls.GridLabel();
            this.FileList = new Perforce.I18nControls.GridDoubleBufferedListView();
            this.TargetFileClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TargetFolderClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SourceFileClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SourceFolderCLm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ResolveTypeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new Perforce.I18nControls.GridLabel();
            this.panel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.groupBox2 = new Perforce.I18nControls.GridGroupBox();
            this.groupBox1 = new Perforce.I18nControls.GridGroupBox();
            this.label1 = new Perforce.I18nControls.GridLabel();
            this.resolveFileActionControl1 = new Perforce.P4VS.ResolveFileActionControl();
            this.resolveFileInteractiveControl1 = new Perforce.P4VS.ResolveFileInteractiveControl();
            this.resolveFileAutoControl1 = new Perforce.P4VS.ResolveFileAutoControl();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridLayoutPanel1
            // 
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
            this.gridLayoutPanel1.Controls.Add(this.MergeBinaryCB);
            this.gridLayoutPanel1.Controls.Add(this.SelectionCountLbl);
            this.gridLayoutPanel1.Controls.Add(this.FileList);
            this.gridLayoutPanel1.Controls.Add(this.label2);
            this.gridLayoutPanel1.Controls.Add(this.panel1);
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.CellHeight = 25;
            this.gridLayoutSubpanel1.CellWidth = 507;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 0;
            this.gridLayoutSubpanel1.Controls.Add(this.gridPanel2);
            this.gridLayoutSubpanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutSubpanel1.Controls.Add(this.InteractiveResolveRB);
            this.gridLayoutSubpanel1.Controls.Add(this.AutoResolveRB);
            this.gridLayoutSubpanel1.EnableDesignerGrid = false;
            this.gridLayoutSubpanel1.EnableDesignerLayout = true;
            this.gridLayoutSubpanel1.EnableParentResize = false;
            this.gridLayoutSubpanel1.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel1.MinimumRowHeight = 10;
            this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
            this.gridLayoutSubpanel1.Row = 1;
            this.gridLayoutSubpanel1.RowsSpanned = 0;
            this.gridLayoutSubpanel1.YOffset = 0;
            // 
            // gridPanel2
            // 
            resources.ApplyResources(this.gridPanel2, "gridPanel2");
            this.gridPanel2.CellHeight = 24;
            this.gridPanel2.CellWidth = 127;
            this.gridPanel2.Column = 3;
            this.gridPanel2.ColumnsSpanned = 0;
            this.gridPanel2.Name = "gridPanel2";
            this.gridPanel2.Row = 0;
            this.gridPanel2.RowsSpanned = 0;
            this.gridPanel2.YOffset = 0;
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.CellHeight = 24;
            this.gridPanel1.CellWidth = 21;
            this.gridPanel1.Column = 0;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 0;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // InteractiveResolveRB
            // 
            resources.ApplyResources(this.InteractiveResolveRB, "InteractiveResolveRB");
            this.InteractiveResolveRB.CellHeight = 24;
            this.InteractiveResolveRB.CellWidth = 210;
            this.InteractiveResolveRB.Column = 2;
            this.InteractiveResolveRB.ColumnsSpanned = 0;
            this.InteractiveResolveRB.Name = "InteractiveResolveRB";
            this.InteractiveResolveRB.Row = 0;
            this.InteractiveResolveRB.RowsSpanned = 0;
            this.InteractiveResolveRB.UseVisualStyleBackColor = true;
            this.InteractiveResolveRB.YOffset = 0;
            this.InteractiveResolveRB.CheckedChanged += new System.EventHandler(this.InteractiveResolveRB_CheckedChanged);
            // 
            // AutoResolveRB
            // 
            resources.ApplyResources(this.AutoResolveRB, "AutoResolveRB");
            this.AutoResolveRB.CellHeight = 24;
            this.AutoResolveRB.CellWidth = 149;
            this.AutoResolveRB.Checked = true;
            this.AutoResolveRB.Column = 1;
            this.AutoResolveRB.ColumnsSpanned = 0;
            this.AutoResolveRB.Name = "AutoResolveRB";
            this.AutoResolveRB.Row = 0;
            this.AutoResolveRB.RowsSpanned = 0;
            this.AutoResolveRB.TabStop = true;
            this.AutoResolveRB.UseVisualStyleBackColor = true;
            this.AutoResolveRB.YOffset = 0;
            this.AutoResolveRB.CheckedChanged += new System.EventHandler(this.AutoResolveRB_CheckedChanged);
            // 
            // MergeBinaryCB
            // 
            resources.ApplyResources(this.MergeBinaryCB, "MergeBinaryCB");
            this.MergeBinaryCB.CellHeight = 23;
            this.MergeBinaryCB.CellWidth = 507;
            this.MergeBinaryCB.Column = 0;
            this.MergeBinaryCB.ColumnsSpanned = 0;
            this.MergeBinaryCB.Name = "MergeBinaryCB";
            this.MergeBinaryCB.Row = 5;
            this.MergeBinaryCB.RowsSpanned = 0;
            this.MergeBinaryCB.UseVisualStyleBackColor = true;
            this.MergeBinaryCB.YOffset = 0;
            this.MergeBinaryCB.CheckedChanged += new System.EventHandler(this.MergeBinaryCB_CheckedChanged);
            // 
            // SelectionCountLbl
            // 
            resources.ApplyResources(this.SelectionCountLbl, "SelectionCountLbl");
            this.SelectionCountLbl.CellHeight = 18;
            this.SelectionCountLbl.CellWidth = 507;
            this.SelectionCountLbl.Column = 0;
            this.SelectionCountLbl.ColumnsSpanned = 0;
            this.SelectionCountLbl.Name = "SelectionCountLbl";
            this.SelectionCountLbl.Row = 3;
            this.SelectionCountLbl.RowsSpanned = 0;
            this.SelectionCountLbl.YOffset = 0;
            // 
            // FileList
            // 
            this.FileList.AllowColumnReorder = true;
            resources.ApplyResources(this.FileList, "FileList");
            this.FileList.CellHeight = 143;
            this.FileList.CellWidth = 507;
            this.FileList.Column = 0;
            this.FileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TargetFileClm,
            this.TargetFolderClm,
            this.SourceFileClm,
            this.SourceFolderCLm,
            this.ResolveTypeClm});
            this.FileList.ColumnsSpanned = 0;
            this.FileList.FullRowSelect = true;
            this.FileList.HideActionsColumn = false;
            this.FileList.HideSelection = false;
            this.FileList.Name = "FileList";
            this.FileList.OwnerDraw = true;
            this.FileList.Row = 4;
            this.FileList.RowsSpanned = 0;
            this.FileList.UseCompatibleStateImageBehavior = false;
            this.FileList.View = System.Windows.Forms.View.Details;
            this.FileList.YOffset = 0;
            this.FileList.SelectedIndexChanged += new System.EventHandler(this.FileList_SelectedIndexChanged);
            // 
            // TargetFileClm
            // 
            resources.ApplyResources(this.TargetFileClm, "TargetFileClm");
            // 
            // TargetFolderClm
            // 
            resources.ApplyResources(this.TargetFolderClm, "TargetFolderClm");
            // 
            // SourceFileClm
            // 
            resources.ApplyResources(this.SourceFileClm, "SourceFileClm");
            // 
            // SourceFolderCLm
            // 
            resources.ApplyResources(this.SourceFolderCLm, "SourceFolderCLm");
            // 
            // ResolveTypeClm
            // 
            resources.ApplyResources(this.ResolveTypeClm, "ResolveTypeClm");
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.CellHeight = 23;
            this.label2.CellWidth = 507;
            this.label2.Column = 0;
            this.label2.ColumnsSpanned = 0;
            this.label2.Name = "label2";
            this.label2.Row = 2;
            this.label2.RowsSpanned = 0;
            this.label2.YOffset = 0;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.CellHeight = 32;
            this.panel1.CellWidth = 507;
            this.panel1.Column = 0;
            this.panel1.ColumnsSpanned = 0;
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.EnableDesignerGrid = false;
            this.panel1.EnableDesignerLayout = true;
            this.panel1.EnableParentResize = false;
            this.panel1.MinimumColumnWidth = 10;
            this.panel1.MinimumRowHeight = 10;
            this.panel1.Name = "panel1";
            this.panel1.Row = 0;
            this.panel1.RowsSpanned = 0;
            this.panel1.YOffset = 0;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.groupBox2.CellHeight = 22;
            this.groupBox2.CellWidth = 414;
            this.groupBox2.Column = 1;
            this.groupBox2.ColumnsSpanned = 0;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Row = 0;
            this.groupBox2.RowsSpanned = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.YOffset = 6;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.groupBox1.CellHeight = 0;
            this.groupBox1.CellWidth = 0;
            this.groupBox1.Column = 0;
            this.groupBox1.ColumnsSpanned = 0;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Row = 0;
            this.groupBox1.RowsSpanned = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.YOffset = 0;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.CellHeight = 22;
            this.label1.CellWidth = 93;
            this.label1.Column = 0;
            this.label1.ColumnsSpanned = 0;
            this.label1.Name = "label1";
            this.label1.Row = 0;
            this.label1.RowsSpanned = 0;
            this.label1.YOffset = 0;
            // 
            // resolveFileActionControl1
            // 
            resources.ApplyResources(this.resolveFileActionControl1, "resolveFileActionControl1");
            this.resolveFileActionControl1.MergeBinaryAsText = false;
            this.resolveFileActionControl1.Name = "resolveFileActionControl1";
            this.resolveFileActionControl1.ReasonText = global::Perforce.P4VS.Resources.EmptyString;
            this.resolveFileActionControl1.RecomendationText = global::Perforce.P4VS.Resources.EmptyString;
            this.resolveFileActionControl1.Scm = null;
            this.resolveFileActionControl1.SelectedItem = null;
            this.resolveFileActionControl1.UpdateListView = null;
            // 
            // resolveFileInteractiveControl1
            // 
            resources.ApplyResources(this.resolveFileInteractiveControl1, "resolveFileInteractiveControl1");
            this.resolveFileInteractiveControl1.BaseFileText = global::Perforce.P4VS.Resources.EmptyString;
            this.resolveFileInteractiveControl1.CommonDiffsText = global::Perforce.P4VS.Resources.EmptyString;
            this.resolveFileInteractiveControl1.ConflictsText = global::Perforce.P4VS.Resources.EmptyString;
            this.resolveFileInteractiveControl1.MergeBinaryAsText = false;
            this.resolveFileInteractiveControl1.Name = "resolveFileInteractiveControl1";
            this.resolveFileInteractiveControl1.ReasonText = global::Perforce.P4VS.Resources.EmptyString;
            this.resolveFileInteractiveControl1.RecomendationText = global::Perforce.P4VS.Resources.EmptyString;
            this.resolveFileInteractiveControl1.Scm = null;
            this.resolveFileInteractiveControl1.SelectedItem = null;
            this.resolveFileInteractiveControl1.SourceDifferencesText = "{0}";
            this.resolveFileInteractiveControl1.TargetDifferencesText = "{0}";
            this.resolveFileInteractiveControl1.UpdateListView = null;
            // 
            // resolveFileAutoControl1
            // 
            resources.ApplyResources(this.resolveFileAutoControl1, "resolveFileAutoControl1");
            this.resolveFileAutoControl1.MergeBinaryAsText = false;
            this.resolveFileAutoControl1.Name = "resolveFileAutoControl1";
            this.resolveFileAutoControl1.Scm = null;
            this.resolveFileAutoControl1.SelectedItems = null;
            this.resolveFileAutoControl1.SelectedMethod = Perforce.P4VS.ResolveFileAutoControl.AutoResolveMethod.Safe;
            this.resolveFileAutoControl1.SourceFiles = null;
            this.resolveFileAutoControl1.UpdateListView = null;
            // 
            // ResolveFileDlg
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridLayoutPanel1);
            this.Controls.Add(this.resolveFileInteractiveControl1);
            this.Controls.Add(this.resolveFileAutoControl1);
            this.Controls.Add(this.resolveFileActionControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ResolveFileDlg";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ResolveFileDlg_FormClosed);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private I18nControls.GridLayoutSubpanel panel1;
		private I18nControls.GridRadioButton InteractiveResolveRB;
		private I18nControls.GridRadioButton AutoResolveRB;
		private I18nControls.GridLabel label1;
		private ResolveFileAutoControl resolveFileAutoControl1;
		private ResolveFileInteractiveControl resolveFileInteractiveControl1;
		private I18nControls.GridLabel label2;
		private I18nControls.GridDoubleBufferedListView FileList;
		private I18nControls.GridLabel SelectionCountLbl;
		private I18nControls.GridCheckBox MergeBinaryCB;
		private System.Windows.Forms.ColumnHeader TargetFileClm;
		private System.Windows.Forms.ColumnHeader TargetFolderClm;
		private System.Windows.Forms.ColumnHeader SourceFileClm;
		private System.Windows.Forms.ColumnHeader SourceFolderCLm;
		private System.Windows.Forms.ColumnHeader ResolveTypeClm;
		private ResolveFileActionControl resolveFileActionControl1;
		private I18nControls.GridGroupBox groupBox1;
		private I18nControls.GridGroupBox groupBox2;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
		private I18nControls.GridPanel gridPanel1;
		private I18nControls.GridPanel gridPanel2;
	}
}