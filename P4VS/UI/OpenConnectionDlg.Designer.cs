namespace Perforce.P4VS
{
	partial class OpenConnectionDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenConnectionDlg));
            this.ServerTB = new Perforce.I18nControls.GridTextBox();
            this.WorkspaceTB = new Perforce.I18nControls.GridTextBox();
            this.UserTB = new Perforce.I18nControls.GridTextBox();
            this.label1 = new Perforce.I18nControls.GridLabel();
            this.label2 = new Perforce.I18nControls.GridLabel();
            this.label3 = new Perforce.I18nControls.GridLabel();
            this.label4 = new Perforce.I18nControls.GridLabel();
            this.RecentConnectionsCB = new Perforce.I18nControls.GridComboBox();
            this.ConfigureSandboxBtn = new Perforce.I18nControls.GridButton();
            this.BrowseUserBtn = new Perforce.I18nControls.GridButton();
            this.BrowseWorkspaceBtn = new Perforce.I18nControls.GridButton();
            this.NewUserBtn = new Perforce.I18nControls.GridButton();
            this.NewWorkspaceBtn = new Perforce.I18nControls.GridButton();
            this.CancelBtn = new Perforce.I18nControls.GridButton();
            this.OkBtn = new Perforce.I18nControls.GridButton();
            this.label5 = new Perforce.I18nControls.GridLabel();
            this.HelpBtn = new Perforce.I18nControls.GridButton();
            this.VersionLbl = new Perforce.I18nControls.GridTextBox();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ServerTB
            // 
            resources.ApplyResources(this.ServerTB, "ServerTB");
            this.ServerTB.CellHeight = 32;
            this.ServerTB.CellWidth = 216;
            this.ServerTB.Column = 1;
            this.ServerTB.ColumnsSpanned = 1;
            this.ServerTB.Name = "ServerTB";
            this.ServerTB.Row = 1;
            this.ServerTB.RowsSpanned = 0;
            this.ServerTB.YOffset = 2;
            this.ServerTB.TextChanged += new System.EventHandler(this.ServerTB_TextChanged);
            this.ServerTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ServerTB_KeyDown);
            // 
            // WorkspaceTB
            // 
            resources.ApplyResources(this.WorkspaceTB, "WorkspaceTB");
            this.WorkspaceTB.CellHeight = 32;
            this.WorkspaceTB.CellWidth = 216;
            this.WorkspaceTB.Column = 1;
            this.WorkspaceTB.ColumnsSpanned = 1;
            this.WorkspaceTB.Name = "WorkspaceTB";
            this.WorkspaceTB.Row = 3;
            this.WorkspaceTB.RowsSpanned = 0;
            this.WorkspaceTB.YOffset = 2;
            this.WorkspaceTB.TextChanged += new System.EventHandler(this.WorkspaceTB_TextChanged);
            this.WorkspaceTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WorkspaceTB_KeyDown);
            // 
            // UserTB
            // 
            resources.ApplyResources(this.UserTB, "UserTB");
            this.UserTB.CellHeight = 32;
            this.UserTB.CellWidth = 216;
            this.UserTB.Column = 1;
            this.UserTB.ColumnsSpanned = 1;
            this.UserTB.Name = "UserTB";
            this.UserTB.Row = 2;
            this.UserTB.RowsSpanned = 0;
            this.UserTB.YOffset = 2;
            this.UserTB.TextChanged += new System.EventHandler(this.UserTB_TextChanged);
            this.UserTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UserTB_KeyDown);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.CellHeight = 32;
            this.label1.CellWidth = 71;
            this.label1.Column = 0;
            this.label1.ColumnsSpanned = 0;
            this.label1.Name = "label1";
            this.label1.Row = 1;
            this.label1.RowsSpanned = 0;
            this.label1.YOffset = 5;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.CellHeight = 32;
            this.label2.CellWidth = 71;
            this.label2.Column = 0;
            this.label2.ColumnsSpanned = 0;
            this.label2.Name = "label2";
            this.label2.Row = 2;
            this.label2.RowsSpanned = 0;
            this.label2.YOffset = 5;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.CellHeight = 32;
            this.label3.CellWidth = 71;
            this.label3.Column = 0;
            this.label3.ColumnsSpanned = 0;
            this.label3.Name = "label3";
            this.label3.Row = 3;
            this.label3.RowsSpanned = 0;
            this.label3.YOffset = 5;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.CellHeight = 21;
            this.label4.CellWidth = 284;
            this.label4.Column = 0;
            this.label4.ColumnsSpanned = 0;
            this.label4.Name = "label4";
            this.label4.Row = 0;
            this.label4.RowsSpanned = 0;
            this.label4.YOffset = 4;
            // 
            // RecentConnectionsCB
            // 
            resources.ApplyResources(this.RecentConnectionsCB, "RecentConnectionsCB");
            this.RecentConnectionsCB.CellHeight = 21;
            this.RecentConnectionsCB.CellWidth = 158;
            this.RecentConnectionsCB.Column = 1;
            this.RecentConnectionsCB.ColumnsSpanned = 0;
            this.RecentConnectionsCB.DesignSize = new System.Drawing.Size(0, 0);
            this.RecentConnectionsCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RecentConnectionsCB.FormattingEnabled = true;
            this.RecentConnectionsCB.Name = "RecentConnectionsCB";
            this.RecentConnectionsCB.Row = 0;
            this.RecentConnectionsCB.RowsSpanned = 0;
            this.RecentConnectionsCB.YOffset = 0;
            this.RecentConnectionsCB.DropDown += new System.EventHandler(this.RecentConnectionsCB_DropDown);
            this.RecentConnectionsCB.SelectedIndexChanged += new System.EventHandler(this.RecentConnectionsCB_SelectedIndexChanged);
            // 
            // ConfigureSandboxBtn
            // 
            resources.ApplyResources(this.ConfigureSandboxBtn, "ConfigureSandboxBtn");
            this.ConfigureSandboxBtn.CellHeight = 32;
            this.ConfigureSandboxBtn.CellWidth = 155;
            this.ConfigureSandboxBtn.Column = 3;
            this.ConfigureSandboxBtn.ColumnsSpanned = 1;
            this.ConfigureSandboxBtn.Name = "ConfigureSandboxBtn";
            this.ConfigureSandboxBtn.Row = 1;
            this.ConfigureSandboxBtn.RowsSpanned = 0;
            this.ConfigureSandboxBtn.UseVisualStyleBackColor = true;
            this.ConfigureSandboxBtn.YOffset = 0;
            this.ConfigureSandboxBtn.Click += new System.EventHandler(this.ConfigureSandboxBtn_Click);
            // 
            // BrowseUserBtn
            // 
            resources.ApplyResources(this.BrowseUserBtn, "BrowseUserBtn");
            this.BrowseUserBtn.CellHeight = 32;
            this.BrowseUserBtn.CellWidth = 79;
            this.BrowseUserBtn.Column = 3;
            this.BrowseUserBtn.ColumnsSpanned = 0;
            this.BrowseUserBtn.Name = "BrowseUserBtn";
            this.BrowseUserBtn.Row = 2;
            this.BrowseUserBtn.RowsSpanned = 0;
            this.BrowseUserBtn.UseVisualStyleBackColor = true;
            this.BrowseUserBtn.YOffset = 0;
            this.BrowseUserBtn.Click += new System.EventHandler(this.BrowseUserBtn_Click);
            // 
            // BrowseWorkspaceBtn
            // 
            resources.ApplyResources(this.BrowseWorkspaceBtn, "BrowseWorkspaceBtn");
            this.BrowseWorkspaceBtn.CellHeight = 32;
            this.BrowseWorkspaceBtn.CellWidth = 79;
            this.BrowseWorkspaceBtn.Column = 3;
            this.BrowseWorkspaceBtn.ColumnsSpanned = 0;
            this.BrowseWorkspaceBtn.Name = "BrowseWorkspaceBtn";
            this.BrowseWorkspaceBtn.Row = 3;
            this.BrowseWorkspaceBtn.RowsSpanned = 0;
            this.BrowseWorkspaceBtn.UseVisualStyleBackColor = true;
            this.BrowseWorkspaceBtn.YOffset = 0;
            this.BrowseWorkspaceBtn.Click += new System.EventHandler(this.BrowseWorkspaceBtn_Click);
            // 
            // NewUserBtn
            // 
            resources.ApplyResources(this.NewUserBtn, "NewUserBtn");
            this.NewUserBtn.CellHeight = 32;
            this.NewUserBtn.CellWidth = 76;
            this.NewUserBtn.Column = 4;
            this.NewUserBtn.ColumnsSpanned = 0;
            this.NewUserBtn.Name = "NewUserBtn";
            this.NewUserBtn.Row = 2;
            this.NewUserBtn.RowsSpanned = 0;
            this.NewUserBtn.UseVisualStyleBackColor = true;
            this.NewUserBtn.YOffset = 0;
            this.NewUserBtn.Click += new System.EventHandler(this.NewUserBtn_Click);
            // 
            // NewWorkspaceBtn
            // 
            resources.ApplyResources(this.NewWorkspaceBtn, "NewWorkspaceBtn");
            this.NewWorkspaceBtn.CellHeight = 32;
            this.NewWorkspaceBtn.CellWidth = 76;
            this.NewWorkspaceBtn.Column = 4;
            this.NewWorkspaceBtn.ColumnsSpanned = 0;
            this.NewWorkspaceBtn.Name = "NewWorkspaceBtn";
            this.NewWorkspaceBtn.Row = 3;
            this.NewWorkspaceBtn.RowsSpanned = 0;
            this.NewWorkspaceBtn.UseVisualStyleBackColor = true;
            this.NewWorkspaceBtn.YOffset = 0;
            this.NewWorkspaceBtn.Click += new System.EventHandler(this.NewWorkspaceBtn_Click);
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.CellHeight = 30;
            this.CancelBtn.CellWidth = 79;
            this.CancelBtn.Column = 3;
            this.CancelBtn.ColumnsSpanned = 0;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Row = 4;
            this.CancelBtn.RowsSpanned = 0;
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.YOffset = 0;
            // 
            // OkBtn
            // 
            resources.ApplyResources(this.OkBtn, "OkBtn");
            this.OkBtn.CellHeight = 30;
            this.OkBtn.CellWidth = 79;
            this.OkBtn.Column = 2;
            this.OkBtn.ColumnsSpanned = 0;
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Row = 4;
            this.OkBtn.RowsSpanned = 0;
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.YOffset = 1;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.CellHeight = 30;
            this.label5.CellWidth = 71;
            this.label5.Column = 0;
            this.label5.ColumnsSpanned = 0;
            this.label5.Name = "label5";
            this.label5.Row = 4;
            this.label5.RowsSpanned = 0;
            this.label5.YOffset = 6;
            // 
            // HelpBtn
            // 
            resources.ApplyResources(this.HelpBtn, "HelpBtn");
            this.HelpBtn.CellHeight = 30;
            this.HelpBtn.CellWidth = 76;
            this.HelpBtn.Column = 4;
            this.HelpBtn.ColumnsSpanned = 0;
            this.HelpBtn.Name = "HelpBtn";
            this.HelpBtn.Row = 4;
            this.HelpBtn.RowsSpanned = 0;
            this.HelpBtn.UseVisualStyleBackColor = true;
            this.HelpBtn.YOffset = 0;
            this.HelpBtn.Click += new System.EventHandler(this.HelpBtn_Click);
            // 
            // VersionLbl
            // 
            resources.ApplyResources(this.VersionLbl, "VersionLbl");
            this.VersionLbl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.VersionLbl.CellHeight = 30;
            this.VersionLbl.CellWidth = 137;
            this.VersionLbl.Column = 1;
            this.VersionLbl.ColumnsSpanned = 0;
            this.VersionLbl.Name = "VersionLbl";
            this.VersionLbl.ReadOnly = true;
            this.VersionLbl.Row = 4;
            this.VersionLbl.RowsSpanned = 0;
            this.VersionLbl.YOffset = 6;
            // 
            // gridLayoutPanel1
            // 
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.Controls.Add(this.OkBtn);
            this.gridLayoutPanel1.Controls.Add(this.VersionLbl);
            this.gridLayoutPanel1.Controls.Add(this.HelpBtn);
            this.gridLayoutPanel1.Controls.Add(this.label5);
            this.gridLayoutPanel1.Controls.Add(this.CancelBtn);
            this.gridLayoutPanel1.Controls.Add(this.NewWorkspaceBtn);
            this.gridLayoutPanel1.Controls.Add(this.NewUserBtn);
            this.gridLayoutPanel1.Controls.Add(this.BrowseWorkspaceBtn);
            this.gridLayoutPanel1.Controls.Add(this.BrowseUserBtn);
            this.gridLayoutPanel1.Controls.Add(this.ConfigureSandboxBtn);
            this.gridLayoutPanel1.Controls.Add(this.label3);
            this.gridLayoutPanel1.Controls.Add(this.label2);
            this.gridLayoutPanel1.Controls.Add(this.label1);
            this.gridLayoutPanel1.Controls.Add(this.UserTB);
            this.gridLayoutPanel1.Controls.Add(this.WorkspaceTB);
            this.gridLayoutPanel1.Controls.Add(this.ServerTB);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
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
            this.gridLayoutSubpanel1.CellHeight = 36;
            this.gridLayoutSubpanel1.CellWidth = 442;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 4;
            this.gridLayoutSubpanel1.Controls.Add(this.RecentConnectionsCB);
            this.gridLayoutSubpanel1.Controls.Add(this.label4);
            this.gridLayoutSubpanel1.EnableDesignerGrid = false;
            this.gridLayoutSubpanel1.EnableDesignerLayout = true;
            this.gridLayoutSubpanel1.EnableParentResize = false;
            this.gridLayoutSubpanel1.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel1.MinimumRowHeight = 10;
            this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
            this.gridLayoutSubpanel1.Row = 0;
            this.gridLayoutSubpanel1.RowsSpanned = 0;
            this.gridLayoutSubpanel1.YOffset = 0;
            // 
            // OpenConnectionDlg
            // 
            this.AcceptButton = this.OkBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.gridLayoutPanel1);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OpenConnectionDlg";
            this.TopMost = true;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.HelpBtn_Click);
            this.Load += new System.EventHandler(this.OpenConnectionDlg_Load);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private I18nControls.GridTextBox ServerTB;
		private I18nControls.GridTextBox WorkspaceTB;
		private I18nControls.GridTextBox UserTB;
		private I18nControls.GridLabel label1;
		private I18nControls.GridLabel label2;
		private I18nControls.GridLabel label3;
		private I18nControls.GridLabel label4;
		private I18nControls.GridComboBox RecentConnectionsCB;
		private I18nControls.GridButton ConfigureSandboxBtn;
		private I18nControls.GridButton BrowseUserBtn;
		private I18nControls.GridButton BrowseWorkspaceBtn;
		private I18nControls.GridButton NewUserBtn;
		private I18nControls.GridButton NewWorkspaceBtn;
		private I18nControls.GridButton CancelBtn;
		private I18nControls.GridButton OkBtn;
		private I18nControls.GridLabel label5;
		private I18nControls.GridButton HelpBtn;
		private I18nControls.GridTextBox VersionLbl;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
	}
}

