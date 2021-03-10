
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

using Microsoft.VisualStudio.Shell.Interop;

using Perforce;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
    /// <summary>
    /// Summary description for SccProviderToolWindowControl.
    /// </summary>
    public class SccHistoryToolWindowControl : P4ToolWindowControlBase
    {
        public ImageList CustomGlyphs;
        private SplitContainer splitContainer1;
        private TreeListView HistoryListView;
        private ColumnHeader Rev;
        private ColumnHeader ChangeList;
        private ColumnHeader DateTime;
        private ColumnHeader User;
        private ColumnHeader Client;
        private ColumnHeader Description;
        private I18nControls.GridTextBox RevisionTB;
        private I18nControls.GridLabel label1;
        private I18nControls.GridButton ShowDetailsBtn;
        private SccHistoryDetailsControl DetailsTab;
        private I18nControls.GridButton ShowLabelsBtn;
        private I18nControls.GridButton ShowIntegrationsBtn;
        private SccHistoryIntegrationsControl IntegrationsTab;
        private SccHistoryLabelsControl LabelsTab;
        private I18nControls.GridGroupBox dividerGB;
        private UI.ThreadMonitorControl ThreadMonitor;
        private IContainer components;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridPanel gridPanel1;
        private I18nControls.GridPanel gridPanel2;
        private ContextMenuStrip HistoryListViewMenu;
        private ToolStripMenuItem GetThisVersionTSMI;
        private ToolStripMenuItem ViewThisRevisionTSMI;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem DiffVersusPreviousRevisionTSMI;
        private ToolStripMenuItem DiffAgainstWorkspaceVersionTSMI;
        private ToolStripMenuItem DiffAgainstTSMI;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem ViewChangelistTSMI;
        private ToolStripMenuItem revisionGraphToolStripMenuItem;
        private ToolStripMenuItem timelapseViewToolStripMenuItem;
        private ToolStripMenuItem diffSelectedToolStripMenuItem;
        private I18nControls.GridButton mRefreshBtn;
        //private ImageList ButtonImages;

        public SccHistoryToolWindowControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            //this.ButtonImages = new System.Windows.Forms.ImageList(this.components);
            // 
            // ButtonImages
            // 
            //this.ButtonImages.TransparentColor = System.Drawing.Color.White;
            //this.ButtonImages.Images.Add("TriangleDownGray.png", Images.TriangleDownGray);
            //this.ButtonImages.Images.Add("TriangleDownRed.png", Images.TriangleDownRed);
            //this.ButtonImages.Images.Add("TriangleRightGray.png", Images.TriangleRightGray);
            //this.ButtonImages.Images.Add("TriangleRightRed.png", Images.TriangleRightRed);

            //this.ShowLabelsBtn.ImageList = this.ButtonImages;
            this.ShowLabelsBtn.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.ShowLabelsBtn.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ShowLabelsBtn.ImageList.Images.Add("TriangleDownGray.png", Images.TriangleDownGray);
            this.ShowLabelsBtn.ImageList.Images.Add("TriangleDownRed.png", Images.TriangleDownRed);
            this.ShowLabelsBtn.ImageList.Images.Add("TriangleRightGray.png", Images.TriangleRightGray);
            this.ShowLabelsBtn.ImageList.Images.Add("TriangleRightRed.png", Images.TriangleRightRed);

            //this.ShowIntegrationsBtn.ImageList = this.ButtonImages;
            this.ShowIntegrationsBtn.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.ShowIntegrationsBtn.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ShowIntegrationsBtn.ImageList.Images.Add("TriangleDownGray.png", Images.TriangleDownGray);
            this.ShowIntegrationsBtn.ImageList.Images.Add("TriangleDownRed.png", Images.TriangleDownRed);
            this.ShowIntegrationsBtn.ImageList.Images.Add("TriangleRightGray.png", Images.TriangleRightGray);
            this.ShowIntegrationsBtn.ImageList.Images.Add("TriangleRightRed.png", Images.TriangleRightRed);

            //this.ShowDetailsBtn.ImageList = this.ButtonImages;
            this.ShowDetailsBtn.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.ShowDetailsBtn.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ShowDetailsBtn.ImageList.Images.Add("TriangleDownGray.png", Images.TriangleDownGray);
            this.ShowDetailsBtn.ImageList.Images.Add("TriangleDownRed.png", Images.TriangleDownRed);
            this.ShowDetailsBtn.ImageList.Images.Add("TriangleRightGray.png", Images.TriangleRightGray);
            this.ShowDetailsBtn.ImageList.Images.Add("TriangleRightRed.png", Images.TriangleRightRed);

            ActiveButton = ShowDetailsBtn;
            ActiveTab = DetailsTab;

            ThreadMonitor.Visible = false;

            newConection = new P4VsProvider.NewConnectionDelegate(OnNewConnection);
            P4VsProvider.NewConnection += newConection;
#if VS2012
            if (!DesignMode)
            {
                base.InitThemeManager();
            }
#endif
        }

        P4VsProvider.NewConnectionDelegate newConection;

        public override void OnNewConnection(P4ScmProvider newScm)
        {
            Scm = newScm;

            // refilter;
            this.HistoryListView.Items.Clear();
        }


        /// <summary> 
        /// Let this control process the mnemonics.
        /// </summary>
        protected override bool ProcessDialogChar(char charCode)
        {
            // If we're the top-level form or control, we need to do the mnemonic handling
            if (charCode != ' ' && ProcessMnemonic(charCode))
            {
                return true;
            }
            return base.ProcessDialogChar(charCode);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private new void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SccHistoryToolWindowControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ThreadMonitor = new Perforce.P4VS.UI.ThreadMonitorControl();
            this.HistoryListView = new Perforce.P4VS.TreeListView();
            this.Rev = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ChangeList = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.User = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Client = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.HistoryListViewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.GetThisVersionTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewThisRevisionTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.revisionGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timelapseViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diffSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DiffVersusPreviousRevisionTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DiffAgainstWorkspaceVersionTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DiffAgainstTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ViewChangelistTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.mRefreshBtn = new Perforce.I18nControls.GridButton();
            this.gridPanel2 = new Perforce.I18nControls.GridPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.IntegrationsTab = new Perforce.P4VS.SccHistoryIntegrationsControl();
            this.DetailsTab = new Perforce.P4VS.SccHistoryDetailsControl();
            this.LabelsTab = new Perforce.P4VS.SccHistoryLabelsControl();
            this.ShowLabelsBtn = new Perforce.I18nControls.GridButton();
            this.dividerGB = new Perforce.I18nControls.GridGroupBox();
            this.ShowIntegrationsBtn = new Perforce.I18nControls.GridButton();
            this.RevisionTB = new Perforce.I18nControls.GridTextBox();
            this.ShowDetailsBtn = new Perforce.I18nControls.GridButton();
            this.label1 = new Perforce.I18nControls.GridLabel();
            this.CustomGlyphs = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.HistoryListViewMenu.SuspendLayout();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ThreadMonitor);
            this.splitContainer1.Panel1.Controls.Add(this.HistoryListView);
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainer1.Panel2.Controls.Add(this.gridLayoutPanel1);
            this.splitContainer1.TabStop = false;
            // 
            // ThreadMonitor
            // 
            resources.ApplyResources(this.ThreadMonitor, "ThreadMonitor");
            this.ThreadMonitor.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ThreadMonitor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ThreadMonitor.CancelPressed = false;
            this.ThreadMonitor.Maximum = 100;
            this.ThreadMonitor.Name = "ThreadMonitor";
            this.ThreadMonitor.Step = 1;
            this.ThreadMonitor.TabStop = false;
            this.ThreadMonitor.Value = 50;
            // 
            // HistoryListView
            // 
            this.HistoryListView._maxLineOffset = 0;
            this.HistoryListView.ActionColumn = -1;
            this.HistoryListView.AllowColumnReorder = true;
            this.HistoryListView.AllowDrop = true;
            this.HistoryListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HistoryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Rev,
            this.ChangeList,
            this.DateTime,
            this.User,
            this.Client,
            this.Description});
            this.HistoryListView.ContextMenuStrip = this.HistoryListViewMenu;
            resources.ApplyResources(this.HistoryListView, "HistoryListView");
            this.HistoryListView.EnableIconOverlays = false;
            this.HistoryListView.EnableSorting = true;
            this.HistoryListView.FullRowSelect = true;
            this.HistoryListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.HistoryListView.MultiSelectConditions = Perforce.P4VS.TreeListView.MultiSelectCondition.none;
            this.HistoryListView.Name = "HistoryListView";
            this.HistoryListView.OverlayOffset = 0;
            this.HistoryListView.OwnerDraw = true;
            this.HistoryListView.RootCheckBoxes = false;
            this.HistoryListView.ScrollPosition = 0;
            this.HistoryListView.ShowGroups = false;
            this.HistoryListView.TreeView = true;
            this.HistoryListView.UseClassicImageList = false;
            this.HistoryListView.UseCompatibleStateImageBehavior = false;
            this.HistoryListView.View = System.Windows.Forms.View.Details;
            this.HistoryListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.HistoryListView_ItemDrag);
            this.HistoryListView.SelectedIndexChanged += new System.EventHandler(this.HistoryListView_SelectedIndexChanged);
            this.HistoryListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.HistoryListView_DragDrop);
            this.HistoryListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.HistoryListView_DragEnter);
            this.HistoryListView.Enter += new System.EventHandler(this.HistoryListView_Enter);
            this.HistoryListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.HistoryListView_MouseDoubleClick);
            // 
            // Rev
            // 
            resources.ApplyResources(this.Rev, "Rev");
            // 
            // ChangeList
            // 
            resources.ApplyResources(this.ChangeList, "ChangeList");
            // 
            // DateTime
            // 
            resources.ApplyResources(this.DateTime, "DateTime");
            // 
            // User
            // 
            resources.ApplyResources(this.User, "User");
            // 
            // Client
            // 
            resources.ApplyResources(this.Client, "Client");
            // 
            // Description
            // 
            resources.ApplyResources(this.Description, "Description");
            // 
            // HistoryListViewMenu
            // 
            this.HistoryListViewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GetThisVersionTSMI,
            this.ViewThisRevisionTSMI,
            this.toolStripSeparator1,
            this.revisionGraphToolStripMenuItem,
            this.timelapseViewToolStripMenuItem,
            this.diffSelectedToolStripMenuItem,
            this.DiffVersusPreviousRevisionTSMI,
            this.DiffAgainstWorkspaceVersionTSMI,
            this.DiffAgainstTSMI,
            this.toolStripSeparator2,
            this.ViewChangelistTSMI});
            this.HistoryListViewMenu.Name = "HistoryListViewMenu";
            resources.ApplyResources(this.HistoryListViewMenu, "HistoryListViewMenu");
            this.HistoryListViewMenu.Opening += new System.ComponentModel.CancelEventHandler(this.HistoryListViewMenu_Opening);
            // 
            // GetThisVersionTSMI
            // 
            this.GetThisVersionTSMI.Name = "GetThisVersionTSMI";
            resources.ApplyResources(this.GetThisVersionTSMI, "GetThisVersionTSMI");
            this.GetThisVersionTSMI.Click += new System.EventHandler(this.GetThisVersionTSMI_Click);
            // 
            // ViewThisRevisionTSMI
            // 
            this.ViewThisRevisionTSMI.Name = "ViewThisRevisionTSMI";
            resources.ApplyResources(this.ViewThisRevisionTSMI, "ViewThisRevisionTSMI");
            this.ViewThisRevisionTSMI.Click += new System.EventHandler(this.ViewThisRevisionTSMI_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // revisionGraphToolStripMenuItem
            // 
            this.revisionGraphToolStripMenuItem.Name = "revisionGraphToolStripMenuItem";
            resources.ApplyResources(this.revisionGraphToolStripMenuItem, "revisionGraphToolStripMenuItem");
            this.revisionGraphToolStripMenuItem.Click += new System.EventHandler(this.revisionGraphToolStripMenuItem_Click);
            // 
            // timelapseViewToolStripMenuItem
            // 
            this.timelapseViewToolStripMenuItem.Name = "timelapseViewToolStripMenuItem";
            resources.ApplyResources(this.timelapseViewToolStripMenuItem, "timelapseViewToolStripMenuItem");
            this.timelapseViewToolStripMenuItem.Click += new System.EventHandler(this.timelapseViewToolStripMenuItem_Click);
            // 
            // diffSelectedToolStripMenuItem
            // 
            this.diffSelectedToolStripMenuItem.Name = "diffSelectedToolStripMenuItem";
            resources.ApplyResources(this.diffSelectedToolStripMenuItem, "diffSelectedToolStripMenuItem");
            this.diffSelectedToolStripMenuItem.Click += new System.EventHandler(this.diffSelectedToolStripMenuItem_Click);
            // 
            // DiffVersusPreviousRevisionTSMI
            // 
            this.DiffVersusPreviousRevisionTSMI.Name = "DiffVersusPreviousRevisionTSMI";
            resources.ApplyResources(this.DiffVersusPreviousRevisionTSMI, "DiffVersusPreviousRevisionTSMI");
            this.DiffVersusPreviousRevisionTSMI.Click += new System.EventHandler(this.DiffVersusPreviousRevisionTSMI_Click);
            // 
            // DiffAgainstWorkspaceVersionTSMI
            // 
            this.DiffAgainstWorkspaceVersionTSMI.Name = "DiffAgainstWorkspaceVersionTSMI";
            resources.ApplyResources(this.DiffAgainstWorkspaceVersionTSMI, "DiffAgainstWorkspaceVersionTSMI");
            this.DiffAgainstWorkspaceVersionTSMI.Click += new System.EventHandler(this.DiffAgainstWorkspaceVersionTSMI_Click);
            // 
            // DiffAgainstTSMI
            // 
            this.DiffAgainstTSMI.Name = "DiffAgainstTSMI";
            resources.ApplyResources(this.DiffAgainstTSMI, "DiffAgainstTSMI");
            this.DiffAgainstTSMI.Click += new System.EventHandler(this.DiffAgainstTSMI_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // ViewChangelistTSMI
            // 
            this.ViewChangelistTSMI.Name = "ViewChangelistTSMI";
            resources.ApplyResources(this.ViewChangelistTSMI, "ViewChangelistTSMI");
            this.ViewChangelistTSMI.Click += new System.EventHandler(this.ViewChangelistTSMI_Click);
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.mRefreshBtn);
            this.gridLayoutPanel1.Controls.Add(this.gridPanel2);
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.ShowLabelsBtn);
            this.gridLayoutPanel1.Controls.Add(this.dividerGB);
            this.gridLayoutPanel1.Controls.Add(this.ShowIntegrationsBtn);
            this.gridLayoutPanel1.Controls.Add(this.RevisionTB);
            this.gridLayoutPanel1.Controls.Add(this.ShowDetailsBtn);
            this.gridLayoutPanel1.Controls.Add(this.label1);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // mRefreshBtn
            // 
            this.mRefreshBtn.CellHeight = 33;
            this.mRefreshBtn.CellWidth = 28;
            this.mRefreshBtn.Column = 4;
            this.mRefreshBtn.ColumnsSpanned = 0;
            resources.ApplyResources(this.mRefreshBtn, "mRefreshBtn");
            this.mRefreshBtn.Image = global::Perforce.P4VS.Images.refresh;
            this.mRefreshBtn.Name = "mRefreshBtn";
            this.mRefreshBtn.Row = 0;
            this.mRefreshBtn.RowsSpanned = 0;
            this.mRefreshBtn.TabStop = false;
            this.mRefreshBtn.UseVisualStyleBackColor = true;
            this.mRefreshBtn.YOffset = 3;
            this.mRefreshBtn.Click += new System.EventHandler(this.mRefreshBtn_Click);
            // 
            // gridPanel2
            // 
            resources.ApplyResources(this.gridPanel2, "gridPanel2");
            this.gridPanel2.CellHeight = 75;
            this.gridPanel2.CellWidth = 133;
            this.gridPanel2.Column = 0;
            this.gridPanel2.ColumnsSpanned = 0;
            this.gridPanel2.Name = "gridPanel2";
            this.gridPanel2.Row = 3;
            this.gridPanel2.RowsSpanned = 0;
            this.gridPanel2.YOffset = 0;
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.CellHeight = 141;
            this.gridPanel1.CellWidth = 384;
            this.gridPanel1.Column = 2;
            this.gridPanel1.ColumnsSpanned = 2;
            this.gridPanel1.Controls.Add(this.IntegrationsTab);
            this.gridPanel1.Controls.Add(this.DetailsTab);
            this.gridPanel1.Controls.Add(this.LabelsTab);
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 1;
            this.gridPanel1.RowsSpanned = 2;
            this.gridPanel1.YOffset = 0;
            // 
            // IntegrationsTab
            // 
            this.IntegrationsTab.BackColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.IntegrationsTab, "IntegrationsTab");
            this.IntegrationsTab.Name = "IntegrationsTab";
            this.IntegrationsTab.RevisionDetail = null;
            this.IntegrationsTab.TabStop = false;
            // 
            // DetailsTab
            // 
            this.DetailsTab.BackColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.DetailsTab, "DetailsTab");
            this.DetailsTab.Name = "DetailsTab";
            this.DetailsTab.RevisionDetail = null;
            this.DetailsTab.TabStop = false;
            // 
            // LabelsTab
            // 
            this.LabelsTab.BackColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.LabelsTab, "LabelsTab");
            this.LabelsTab.File = null;
            this.LabelsTab.Name = "LabelsTab";
            this.LabelsTab.Scm = null;
            this.LabelsTab.TabStop = false;
            // 
            // ShowLabelsBtn
            // 
            this.ShowLabelsBtn.CellHeight = 33;
            this.ShowLabelsBtn.CellWidth = 133;
            this.ShowLabelsBtn.Column = 0;
            this.ShowLabelsBtn.ColumnsSpanned = 0;
            this.ShowLabelsBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.ShowLabelsBtn, "ShowLabelsBtn");
            this.ShowLabelsBtn.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ShowLabelsBtn.Name = "ShowLabelsBtn";
            this.ShowLabelsBtn.Row = 2;
            this.ShowLabelsBtn.RowsSpanned = 0;
            this.ShowLabelsBtn.UseVisualStyleBackColor = true;
            this.ShowLabelsBtn.YOffset = 0;
            this.ShowLabelsBtn.Click += new System.EventHandler(this.ShowLabelsBtn_Click);
            // 
            // dividerGB
            // 
            resources.ApplyResources(this.dividerGB, "dividerGB");
            this.dividerGB.BackColor = System.Drawing.SystemColors.Menu;
            this.dividerGB.CellHeight = 174;
            this.dividerGB.CellWidth = 10;
            this.dividerGB.Column = 1;
            this.dividerGB.ColumnsSpanned = 0;
            this.dividerGB.Name = "dividerGB";
            this.dividerGB.Row = 0;
            this.dividerGB.RowsSpanned = 4;
            this.dividerGB.TabStop = false;
            this.dividerGB.YOffset = 0;
            // 
            // ShowIntegrationsBtn
            // 
            this.ShowIntegrationsBtn.CellHeight = 33;
            this.ShowIntegrationsBtn.CellWidth = 133;
            this.ShowIntegrationsBtn.Column = 0;
            this.ShowIntegrationsBtn.ColumnsSpanned = 0;
            this.ShowIntegrationsBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.ShowIntegrationsBtn, "ShowIntegrationsBtn");
            this.ShowIntegrationsBtn.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ShowIntegrationsBtn.Name = "ShowIntegrationsBtn";
            this.ShowIntegrationsBtn.Row = 1;
            this.ShowIntegrationsBtn.RowsSpanned = 0;
            this.ShowIntegrationsBtn.UseVisualStyleBackColor = true;
            this.ShowIntegrationsBtn.YOffset = 0;
            this.ShowIntegrationsBtn.Click += new System.EventHandler(this.ShowIntegrationsBtn_Click);
            // 
            // RevisionTB
            // 
            resources.ApplyResources(this.RevisionTB, "RevisionTB");
            this.RevisionTB.CellHeight = 33;
            this.RevisionTB.CellWidth = 299;
            this.RevisionTB.Column = 3;
            this.RevisionTB.ColumnsSpanned = 0;
            this.RevisionTB.Name = "RevisionTB";
            this.RevisionTB.ReadOnly = true;
            this.RevisionTB.Row = 0;
            this.RevisionTB.RowsSpanned = 0;
            this.RevisionTB.TabStop = false;
            this.RevisionTB.YOffset = 3;
            // 
            // ShowDetailsBtn
            // 
            this.ShowDetailsBtn.CellHeight = 33;
            this.ShowDetailsBtn.CellWidth = 133;
            this.ShowDetailsBtn.Column = 0;
            this.ShowDetailsBtn.ColumnsSpanned = 0;
            this.ShowDetailsBtn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.ShowDetailsBtn, "ShowDetailsBtn");
            this.ShowDetailsBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ShowDetailsBtn.Name = "ShowDetailsBtn";
            this.ShowDetailsBtn.Row = 0;
            this.ShowDetailsBtn.RowsSpanned = 0;
            this.ShowDetailsBtn.UseVisualStyleBackColor = true;
            this.ShowDetailsBtn.YOffset = 0;
            this.ShowDetailsBtn.Click += new System.EventHandler(this.ShowDetailsBtn_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.CellHeight = 33;
            this.label1.CellWidth = 57;
            this.label1.Column = 2;
            this.label1.ColumnsSpanned = 0;
            this.label1.Name = "label1";
            this.label1.Row = 0;
            this.label1.RowsSpanned = 0;
            this.label1.YOffset = 7;
            // 
            // CustomGlyphs
            // 
            this.CustomGlyphs.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("CustomGlyphs.ImageStream")));
            this.CustomGlyphs.TransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.CustomGlyphs.Images.SetKeyName(0, "Glyph_1.png");
            this.CustomGlyphs.Images.SetKeyName(1, "Glyph_2.png");
            this.CustomGlyphs.Images.SetKeyName(2, "Glyph_3.png");
            this.CustomGlyphs.Images.SetKeyName(3, "Glyph_4.png");
            // 
            // SccHistoryToolWindowControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitContainer1);
            this.Name = "SccHistoryToolWindowControl";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.HistoryListViewMenu.ResumeLayout(false);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.gridPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private object SyncRoot = new object();

        public void ClearFiles()
        {
            Files = new List<string>();
            this.HistoryListView.Items.Clear();
        }

        public IList<string> _files = null;

        private delegate void HistoryListViewDelegate();
        private delegate void HistoryListViewItemDelegate(TreeListViewItem item);

        private void AsyncPopulateTreeListView(object parm)
        {
            lock (SyncRoot)
            {
                //bool threadAborted = false;
                try
                {
                    if ((Scm == null) || Scm.Connection.Disconnected)
                    {
                        return;
                    }

                    Scm.Connection.Repository.Connection.getP4Server().ProgramName = "P4VS";
                    Scm.Connection.Repository.Connection.getP4Server().ProgramVersion = Versions.product();

                    if (HistoryListView.InvokeRequired)
                    {
                        HistoryListView.Invoke(new HistoryListViewDelegate(this.HistoryListView.Nodes.Clear));
                        HistoryListView.Invoke(new HistoryListViewDelegate(this.HistoryListView.Items.Clear));
                    }
                    else
                    {
                        this.HistoryListView.Nodes.Clear();
                        this.HistoryListView.Items.Clear();
                    }
                    string[] value = parm as string[];

                    ThreadMonitor.Value = 0;

                    if (value == null)
                    {
                        //ThreadMonitor.Show(FillInListProc, 0);
                        _files = null;
                    }
                    else
                    {
                        ThreadMonitor.Show(value.Length);
                        _files = new List<string>();

                        int cnt = 0;
                        nodeCount = value.Length;
                        foreach (string file in value)
                        {
                            if (ThreadMonitor.CancelPressed)
                            {
                                break;
                            }
                            AddFile(file);
                            ThreadMonitor.Value = ++cnt;
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    //threadAborted = true;
                    Thread.ResetAbort();
                }
                catch (Exception ex)
                {
                    if (!ThreadMonitor.CancelPressed)
                    {
                        P4ErrorDlg.Show(ex.Message, false, false);
                    }
                }
                finally
                {
                    //if (!threadAborted || ThreadMonitor.CancelPressed)
                    //{
                    ThreadMonitor.Hide();
                    FillInListProc = null;
                    //}
                }
            }
        }
        Thread FillInListProc = null;

        public IList<string> Files
        {
            set
            {
                if (Scm == null)
                {
                    P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
                    if (P4VSService != null)
                    {
                        Scm = P4VSService.ScmProvider;
                    }
                }

                if (Scm == null)
                {
                    // still null
                    return;
                }

                if ((FillInListProc != null) && (FillInListProc.IsAlive))
                {
                    // will call abort on the thread
                    ThreadMonitor.CancelPressed = true;

                    //FillInListProc.Abort();

                    // might have been nulled out if the proc completed asynchronously 
                    if (FillInListProc != null)
                    {
                        FillInListProc.Join(1000);
                    }
                    //ThreadMonitor.Hide();
                    //FillInListProc = null;
                }
                DetailsTab.RevisionDetail = null;
                IntegrationsTab.RevisionDetail = null;
                LabelsTab.File = null;

                FillInListProc = new Thread(new ParameterizedThreadStart(AsyncPopulateTreeListView));
                FillInListProc.IsBackground = true;

                // need to copy the list, otherwise we've got a live reference to the current selection in
                // solution explorer, so if the user clicks on a new selection while filling in the list view,
                // the contents of the list will change and we'll get an enumeration error.
                string[] _value = new string[value.Count];
                value.CopyTo(_value, 0);

                FillInListProc.Start(_value);
            }
            get { return _files; }
        }

        public new P4ScmProvider Scm { get; set; }

        public void AddFile(string file)
        {
            if (Scm == null)
            {
                P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
                if (P4VSService != null)
                {
                    Scm = P4VSService.ScmProvider;
                }
            }

            if (Scm == null)
            {
                // still null
                return;
            }

            IList<P4.FileHistory> log = Scm.GetFileHistory(file);

            if ((log == null) || (log.Count <= 0))
            {
                return;
            }

            _files.Add(file);
            TreeListViewItem fileItem = new TreeListViewItem(null, file, true);
            fileItem.Tag = null;
            for (int idx = 0; idx < log.Count; idx++)
            {
                P4.FileHistory entry = log[idx];

                DateTime local = entry.Date;
                string date = "";
                // we need a pref for local time, until then, don't do this:
                //DateTime local = TimeZone.CurrentTimeZone.ToLocalTime(entry.Date);
                if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                {
                    date = local.ToString("yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                    date = local.ToString();
                }
                String[] fields = { entry.Revision.ToString(), entry.ChangelistId.ToString(), date, entry.UserName, entry.ClientName, entry.Description };
                TreeListViewItem revItem = new TreeListViewItem(fileItem, fields, 0);
                //this.HistoryListView.Nodes.Add(revItem);
                fileItem.ChildNodes.Add(revItem);
                revItem.Tag = entry;
                //if (entry.IntegrationSummaries.Count > 0)
                //{
                //    foreach (P4.RevisionIntegrationSummary rev in entry.IntegrationSummaries)
                //    {
                //        TreeListViewItem subItem = new TreeListViewItem(rev.ToString(), true);
                //        revItem.ChildNodes.Add(subItem);
                //    }
                //}
            }

            if (!HistoryListView.IsDisposed)
            {
                if (HistoryListView.InvokeRequired)
                {
                    HistoryListView.Invoke(new HistoryListViewItemDelegate(this.HistoryListView.Nodes.Add), fileItem);
                    HistoryListView.Invoke(new HistoryListViewDelegate(this.HistoryListView.BuildTreeList));
                }
                else
                {
                    this.HistoryListView.Nodes.Add(fileItem);
                    this.HistoryListView.BuildTreeList();
                }
                if (nodeCount == 1)
                {
                    if (HistoryListView.InvokeRequired)
                    {
                        HistoryListView.Invoke(new HistoryListViewItemDelegate(this.HistoryListView.ExpandNode),
                            HistoryListView.Nodes[0]);
                    }
                    else
                    {
                        HistoryListView.ExpandNode(HistoryListView.Nodes[0]);
                    }
                }
            }
        }

        private int nodeCount = 0;
        private P4.FileHistory revisionDetail = null;
        private P4.FileSpec revisionSpec = null;

        private void HistoryListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Scm == null)
            {
                P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
                if (P4VSService != null)
                {
                    Scm = P4VSService.ScmProvider;
                }
            }

            if ((HistoryListView.SelectedItems != null) && (HistoryListView.SelectedItems.Count > 0))
            {
                P4.FileHistory newRevisionDetail = (P4.FileHistory)HistoryListView.SelectedItems[0].Tag;

                if (revisionDetail == newRevisionDetail)
                {
                    return;
                }
                revisionDetail = newRevisionDetail;
            }
            else
            {
                return;
            }

            if (revisionDetail != null)
            {
                RevisionTB.Text = string.Format("{0}#{1}",
                                            revisionDetail.DepotPath.Path,
                                            revisionDetail.Revision.ToString());

                revisionSpec = new P4.FileSpec(revisionDetail.DepotPath,
                    new P4.VersionRange(revisionDetail.Revision, revisionDetail.Revision));
            }
            else
            {
                RevisionTB.Text = string.Empty;
            }
            DetailsTab.RevisionDetail = revisionDetail;
            if (ActiveTab == DetailsTab)
            {
                DetailsTab.Invalidate(true);
            }
            IntegrationsTab.RevisionDetail = revisionDetail;
            if (ActiveTab == IntegrationsTab)
            {
                IntegrationsTab.Invalidate(true);
            }

            if (LabelsTab.Scm == null)
            {
                LabelsTab.Scm = Scm;
            }
            if ((LabelsTab.Scm != null) && (ActiveTab == LabelsTab))
            {
                LabelsTab.File = revisionSpec;
                LabelsTab.Invalidate(true);
            }
            else
            {
                LabelsTab.File = null;
                if (ActiveTab == LabelsTab)
                {
                    LabelsTab.Invalidate(true);
                }
            }
            splitContainer1.Panel2.Invalidate(true);
        }

        private Button ActiveButton = null; //ShowDetailsBtn;
        private UserControl ActiveTab = null; //DetailsTab;

        private void SwitchTab(Button NewButton, UserControl NewTab)
        {
            if (ActiveButton.Text == NewButton.Text)
            {
                // no change
                return;
            }
            ActiveButton.ImageIndex = 2;
            ActiveButton.ForeColor = SystemColors.GrayText;

            ActiveTab.Visible = false;
            //ActiveTab.SendToBack();

            ActiveButton = NewButton;
            ActiveTab = NewTab;

            ActiveButton.ImageIndex = 0;
#if VS2012
            ActiveButton.ForeColor = ThemeMgr.ForeColor;
#else
            ActiveButton.ForeColor = SystemColors.ControlText;
#endif

            //ActiveTab.Visible = true;
            //ActiveTab.BringToFront();
        }

        private void ShowDetailsBtn_Click(object sender, EventArgs e)
        {
            SwitchTab(ShowDetailsBtn, DetailsTab);
            DetailsTab.Visible = true;
        }

        private void ShowIntegrationsBtn_Click(object sender, EventArgs e)
        {
            SwitchTab(ShowIntegrationsBtn, IntegrationsTab);
            IntegrationsTab.Visible = true;
        }

        private void ShowLabelsBtn_Click(object sender, EventArgs e)
        {
            SwitchTab(ShowLabelsBtn, LabelsTab);
            LabelsTab.Visible = true;

            if ((LabelsTab.Scm != null) && (revisionSpec != null) && (LabelsTab.Visible))
            {
                LabelsTab.File = revisionSpec;
                if (ActiveTab == LabelsTab)
                {
                    LabelsTab.Invalidate(true);
                    LabelsTab.Focus();
                }
            }
        }

        private void HistoryListView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                object data = e.Data.GetData(DataFormats.FileDrop);
            }
            else if (e.Data.GetDataPresent(DataFormats.Text) ||
               e.Data.GetDataPresent(DataFormats.UnicodeText) ||
               e.Data.GetDataPresent(DataFormats.OemText))
            {
                string fs = e.Data.GetData(DataFormats.UnicodeText, true) as string;

                P4VsOutputWindow.AppendMessage(string.Format("DragDrop: {0}", fs));

                P4.FileSpec fs1 = null;
                if (fs != null && fs.StartsWith("DepotSpec?"))
                {
                    string[] parts = fs.Split('?');

                    int rev = 0;
                    if (int.TryParse(parts[2], out rev))
                    {
                        fs1 = P4.FileSpec.DepotSpec(parts[1], rev);
                    }
                    else if (parts[2] == "head")
                    {
                        fs1 = P4.FileSpec.DepotSpec(parts[1]);
                        fs1.Version = new P4.HeadRevision();
                    }
                    else if (parts[2] == "have")
                    {
                        fs1 = P4.FileSpec.DepotSpec(parts[1], rev);
                        fs1.Version = new P4.HaveRevision();
                    }
                }
                else if (fs != null && fs.StartsWith("LocalSpec?"))
                {
                    string[] parts = fs.Split('?');

                    int rev = 0;
                    if (int.TryParse(parts[2], out rev))
                    {
                        fs1 = P4.FileSpec.LocalSpec(parts[1], rev);
                    }
                    else if ((parts.Length <= 2) || (string.IsNullOrEmpty(parts[2])))
                    {
                        fs1 = P4.FileSpec.LocalSpec(parts[1]);
                    }
                    else if (parts[2] == "head")
                    {
                        fs1 = P4.FileSpec.LocalSpec(parts[1]);
                        fs1.Version = new P4.HeadRevision();
                    }
                    else if (parts[2] == "have")
                    {
                        fs1 = P4.FileSpec.LocalSpec(parts[1], rev);
                        fs1.Version = new P4.HaveRevision();
                    }
                }
                P4.FileSpec fs2 = null;
                Point clientCoor = HistoryListView.PointToClient(new Point(e.X, e.Y));
                TreeListViewItem tlvi = HistoryListView.GetItemAt(clientCoor.X, clientCoor.Y) as TreeListViewItem;
                if (tlvi != null)
                {
                    if (tlvi.ParentItem != null)
                    {
                        // it's a revision line
                        P4.FileHistory entry = tlvi.Tag as P4.FileHistory;

                        if (entry != null) fs2 = P4.FileSpec.DepotSpec(entry.DepotPath.Path, entry.Revision);
                    }
                    else
                    {
                        fs2 = P4.FileSpec.DepotSpec(tlvi.Text);
                        fs2.Version = new P4.HeadRevision();
                    }
                }
                if ((fs1 != null) && (fs2 != null))
                {
                    if ((fs1.Version != null) && (fs2.Version != null))
                    {
                        // versioned files, so always display most recent on the right
                        // assume fs2 is most recent
                        P4.FileSpec left = fs1;
                        P4.FileSpec right = fs2;

                        if (fs1.Version is P4.HeadRevision)
                        {
                            left = fs2;
                            right = fs1;
                        }
                        else if ((fs1.Version is P4.Revision) && (fs2.Version is P4.Revision) &&
                            (((P4.Revision)fs1.Version).Rev > ((P4.Revision)fs2.Version).Rev))
                        {
                            left = fs2;
                            right = fs1;
                        }
                        Scm.Diff2Files(left, null, right, null);
                    }
                    else
                    {
                        Scm.Diff2Files(fs1, null, fs2, null);
                    }
                }
                e.Effect = DragDropEffects.Link;
                return;
            }
            e.Effect = DragDropEffects.None;
        }

        private void HistoryListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) ||
               e.Data.GetDataPresent(DataFormats.UnicodeText) ||
               e.Data.GetDataPresent(DataFormats.OemText))
            {
                string fs = e.Data.GetData(DataFormats.UnicodeText, true) as string;

                P4VsOutputWindow.AppendMessage(string.Format("DragEnter: {0}", fs));

                if (fs != null && ((fs.StartsWith("DepotSpec?")) || (fs.StartsWith("LocalSpec?"))))
                {
                    e.Effect = DragDropEffects.Link;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void HistoryListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Item is TreeListViewItem)
            {
                TreeListViewItem tlvi = e.Item as TreeListViewItem;
                if (tlvi != null)
                {
                    if (tlvi.ParentItem != null)
                    {
                        // it's a revision line
                        P4.FileHistory entry = tlvi.Tag as P4.FileHistory;

                        if (entry != null)
                        {
                            P4VsOutputWindow.AppendMessage(string.Format("Starting drag/drop, FileSpec:{0}#{1}", entry.DepotPath, entry.Revision));

                            DoDragDrop(string.Format("DepotSpec?{0}?{1}", entry.DepotPath, entry.Revision),
                                       DragDropEffects.Link);
                        }
                    }
                    else
                    {
                        P4VsOutputWindow.AppendMessage(string.Format("Starting drag/drop, LocalSpec:{0}", tlvi.Text));

                        DoDragDrop(string.Format("LocalSpec?{0}?", tlvi.Text),
                                    DragDropEffects.Link);
                    }
                }
            }
            else
            {
                System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
                messageBoxCS.AppendFormat("{0} = {1}", "Button", e.Button);
                messageBoxCS.AppendLine();
                messageBoxCS.AppendFormat("{0} = {1}", "Item", e.Item);
                messageBoxCS.AppendLine();
                MessageBox.Show(messageBoxCS.ToString(), "ItemDrag Event");
            }
        }

        private void HistoryListViewMenu_Opening(object sender, CancelEventArgs e)
        {
            GetThisVersionTSMI.Enabled = false;
            GetThisVersionTSMI.Visible = false;

            ViewThisRevisionTSMI.Enabled = false;
            ViewThisRevisionTSMI.Visible = false;

            toolStripSeparator1.Visible = false;

            diffSelectedToolStripMenuItem.Enabled = false;
            diffSelectedToolStripMenuItem.Visible = false;

            DiffVersusPreviousRevisionTSMI.Enabled = false;
            DiffVersusPreviousRevisionTSMI.Visible = false;

            DiffAgainstWorkspaceVersionTSMI.Enabled = false;
            DiffAgainstWorkspaceVersionTSMI.Visible = false;

            DiffAgainstTSMI.Enabled = false;
            DiffAgainstTSMI.Visible = false;

            toolStripSeparator2.Visible = false;

            ViewChangelistTSMI.Enabled = false;
            ViewChangelistTSMI.Visible = false;

            if ((HistoryListView.SelectedItems == null) || (HistoryListView.SelectedItems.Count <= 0))
            {
                //nothing selected
                return;
            }
            TreeListViewItem tlvi = HistoryListView.SelectedItems[0] as TreeListViewItem;
            if (tlvi == null)
            {
                //bad selection
                return;
            }
            if (tlvi.ParentItem != null)
            {
                P4.FileHistory entry = tlvi.Tag as P4.FileHistory;
                if (entry == null)
                {
                    //bad selection
                    return;
                }
                int rev = entry.Revision;

                // its a specific revision under a file, so enabel all items
                GetThisVersionTSMI.Enabled = true;
                GetThisVersionTSMI.Visible = true;

                ViewThisRevisionTSMI.Enabled = true;
                ViewThisRevisionTSMI.Visible = true;

                toolStripSeparator1.Visible = true;

                DiffVersusPreviousRevisionTSMI.Enabled = (rev > 1);
                DiffVersusPreviousRevisionTSMI.Visible = (rev > 1);

                DiffAgainstWorkspaceVersionTSMI.Enabled = true;
                DiffAgainstWorkspaceVersionTSMI.Visible = true;

                DiffAgainstTSMI.Enabled = true;
                DiffAgainstTSMI.Visible = true;

                toolStripSeparator2.Visible = true;

                ViewChangelistTSMI.Enabled = true;
                ViewChangelistTSMI.Visible = true;
            }
            else
            {
                // It's a file item
                // its a specific revision under a file, so enabel all items
                GetThisVersionTSMI.Enabled = false;
                GetThisVersionTSMI.Visible = false;

                ViewThisRevisionTSMI.Enabled = false;
                ViewThisRevisionTSMI.Visible = false;

                toolStripSeparator1.Visible = false;

                DiffVersusPreviousRevisionTSMI.Enabled = false;
                DiffVersusPreviousRevisionTSMI.Visible = false;

                DiffAgainstWorkspaceVersionTSMI.Enabled = true;
                DiffAgainstWorkspaceVersionTSMI.Visible = true;

                DiffAgainstTSMI.Enabled = true;
                DiffAgainstTSMI.Visible = true;

                toolStripSeparator2.Visible = false;

                ViewChangelistTSMI.Enabled = false;
                ViewChangelistTSMI.Visible = false;
            }
            // finally, check if 2 revision items are selected
            // to allow "Diff Selected"
            if (HistoryListView.SelectedItems.Count == 2)
            {
                P4.FileHistory firstSelection = (P4.FileHistory)HistoryListView.SelectedItems[0].Tag;
                P4.FileHistory secondSelection = (P4.FileHistory)HistoryListView.SelectedItems[1].Tag;
                if (firstSelection!=null&& secondSelection!=null)
                {
                    diffSelectedToolStripMenuItem.Enabled = true;
                    diffSelectedToolStripMenuItem.Visible = true;
                }
            }
        }

        private void GetThisVersionTSMI_Click(object sender, EventArgs e)
        {
            //if ((HistoryListView.SelectedItems == null) || (HistoryListView.SelectedItems.COunt <=1))
            //{
            //    return;
            //}
            TreeListViewItem tlvi = HistoryListView.SelectedItems[0] as TreeListViewItem;
            if (tlvi == null)
            {
                //bad selection
                return;
            }
            P4.FileHistory entry = null;
            if (tlvi.ParentItem != null)
            {
                entry = tlvi.Tag as P4.FileHistory;
                tlvi = tlvi.ParentItem;
            }
            else
            {
                return;
            }
            if (entry == null)
            {
                return;
            }

            string file = tlvi.Text;
            P4VsProvider.Instance.CheckLazyLoadStatus();

            P4.FileMetaData fmd = null;

            if (string.IsNullOrEmpty(file) == false)
            {
                fmd = Scm.Fetch(file);
            }

            if (fmd == null)
            {
                return;
            }

            try
            {
                List<P4.FileSpec> files = new List<P4.FileSpec>();
                files.Add(new P4.FileSpec(null, null, new P4.LocalPath(file), new P4.Revision(entry.Revision)));
                bool success = Scm.SyncFiles(null, files);
            }
            catch (P4.P4Exception ex)
            {
                P4ErrorDlg.Show(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                List<string> files = new List<string>();
                files.Add(file);
                P4VsProvider.Instance.Glyphs.RefreshFilesAndGlyphs(files);
            }

        }

        private void ViewThisRevisionTSMI_Click(object sender, EventArgs e)
        {
            TreeListViewItem tlvi = HistoryListView.SelectedItems[0] as TreeListViewItem;
            if (tlvi == null)
            {
                //bad selection
                return;
            }

            P4.FileHistory entry = null;
            string file = null;
            string title = null;

            if (tlvi.ParentItem != null)
            {
                entry = tlvi.Tag as P4.FileHistory;
                title = entry.DepotPath.Path + "#" + entry.Revision.ToString();
                file = tlvi.ParentItem.Text;
            }
            else
            {
                return;
            }
            if (entry == null)
            {
                return;
            }

            P4.FileSpec fs = new P4.FileSpec(null, null, new P4.LocalPath(file), new P4.Revision(entry.Revision));

            //string tmpFileName = string.Format("{0}#{1}{2}", Path.GetFileNameWithoutExtension(file),
            //    entry.Revision, Path.GetExtension(file));
            using (TempFile sourceFile = new TempFile(fs))
            {
                if (Scm.GetFileVersion(sourceFile, new P4.FileSpec(null, null, new P4.LocalPath(file), new P4.Revision(entry.Revision))) == null)
                {
                    return;
                }

                if (Preferences.LocalSettings.GetBool("OpenShelvedFileInEditor", true))
                {
                    EnvDTE.DTE dte = P4VsProvider.GetDTE();
                    if (System.IO.File.Exists(sourceFile))
                    {
                        dte.ItemOperations.OpenFile(sourceFile, null);
                    }
                }
                else
                {
                    ShowFileContentsDlg dlg = new ShowFileContentsDlg();

                    dlg.TempFile = sourceFile;
                    dlg.Title = title;

                    // Show modeless
                    dlg.Show();
                }
            }
        }

        private void DiffVersusPreviousRevisionTSMI_Click(object sender, EventArgs e)
        {
            TreeListViewItem tlvi = HistoryListView.SelectedItems[0] as TreeListViewItem;
            if (tlvi == null)
            {
                //bad selection
                return;
            }

            P4.FileHistory entry = null;
            string file = null;

            if (tlvi.ParentItem != null)
            {
                entry = tlvi.Tag as P4.FileHistory;
                file = tlvi.ParentItem.Text;
            }
            else
            {
                return;
            }
            if ((entry == null) || (entry.Revision < 1)) // no rev or first rev
            {
                return;
            }

            P4.FileSpec left = P4.FileSpec.LocalSpec(file, entry.Revision - 1);
            P4.FileSpec right = P4.FileSpec.LocalSpec(file, entry.Revision);

            if ((left != null) && (right != null))
            {
                Scm.Diff2Files(left, Path.GetFileName(left.ToString()), right, Path.GetFileName(right.ToString()));
            }
        }

        private void DiffAgainstWorkspaceVersionTSMI_Click(object sender, EventArgs e)
        {
            TreeListViewItem tlvi = HistoryListView.SelectedItems[0] as TreeListViewItem;
            if (tlvi == null)
            {
                //bad selection
                return;
            }

            P4.FileHistory entry = null;
            string file = null;

            if (tlvi.ParentItem != null)
            {
                entry = tlvi.Tag as P4.FileHistory;
                file = tlvi.ParentItem.Text;
            }
            if ((entry != null) && (entry.Revision >= 1))
            {
                P4.FileSpec right = P4.FileSpec.LocalSpec(file, entry.Revision);

                if ((file != null) && (right != null))
                {
                    Scm.Diff2Files(file, file, right, Path.GetFileName(right.ToString()));
                }
            }
            else
            {
                if (tlvi.ParentItem != null)
                {
                    //should only be invoked on a root item
                    return;
                }
                file = tlvi.Text;
                P4.FileSpec right = P4.FileSpec.LocalSpec(file, new P4.HeadRevision());
                Scm.Diff2Files(file, file, right, Path.GetFileName(right.ToString()));
            }
        }

        private void DiffAgainstTSMI_Click(object sender, EventArgs e)
        {
            TreeListViewItem tlvi = HistoryListView.SelectedItems[0] as TreeListViewItem;
            if (tlvi == null)
            {
                //bad selection
                return;
            }

            P4.FileHistory entry = null;
            string file = null;

            if (tlvi.ParentItem != null)
            {
                // clicked on a revision
                entry = tlvi.Tag as P4.FileHistory;
                file = tlvi.ParentItem.Text;
            }
            else
            {
                file = tlvi.Text;
            }
            List<string> files = new List<string>();
            files.Add(file);

            if (entry != null)
            {
                IList<P4.FileSpec> result = DiffDlg.Show(files, "history", entry.Revision.ToString(), Scm);
            }
            else
            {
                IList<P4.FileSpec> result = DiffDlg.Show(files, "history", "local", Scm);
            }
        }

        private void ViewChangelistTSMI_Click(object sender, EventArgs e)
        {
            if (Scm == null)
            {
                // still null
                return;
            }
            TreeListViewItem tlvi = HistoryListView.SelectedItems[0] as TreeListViewItem;
            if (tlvi == null)
            {
                //bad selection
                return;
            }

            P4.FileHistory entry = null;
            string file = null;

            if (tlvi.ParentItem != null)
            {
                // clicked on a revision
                entry = tlvi.Tag as P4.FileHistory;
                file = tlvi.ParentItem.Text;
            }
            else
            {
                file = tlvi.Text;
            }
            try
            {
                SubmittedChangelistDlg dlg = new SubmittedChangelistDlg(Scm, true);
                if (entry != null)
                {
                    string changeText = entry.ChangelistId.ToString();
                    if (entry.ChangelistId < 1)
                    {
                        changeText = Resources.Changelist_Default;
                    }
                    P4.ServerMetaData smd = Scm.GetServerMetaData();
                    dlg.Text = string.Format(Resources.SubmittedChangelistsToolWindowControl_SubmittedChangelistDlgCaption,
                                             changeText, smd.Address.Uri, Scm.Connection.Repository.Connection.UserName);
                    dlg.ChangeListId = entry.ChangelistId;
                    dlg.ShowDialog();
                }
            }
            catch (P4.P4Exception ex)
            {
                P4ErrorDlg.Show(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mRefreshBtn_Click(object sender, EventArgs e)
        {
            HistoryListView.BeginUpdate();
            try
            {
                IDictionary<string, bool> expanded = new Dictionary<string, bool>();
                for (int idx = 0; idx < HistoryListView.Nodes.Count; idx++)
                {
                    if (HistoryListView.Nodes[idx].Expanded)
                    {
                        expanded.Add(HistoryListView.Nodes[idx].Text, true);
                    }
                }
                string[] _value = new string[_files.Count];
                _files.CopyTo(_value, 0);

                AsyncPopulateTreeListView(_value);
                if (expanded.Count > 0)
                {
                    for (int idx = 0; idx < HistoryListView.Nodes.Count; idx++)
                    {
                        if (expanded.ContainsKey(HistoryListView.Nodes[idx].Text))
                        {
                            HistoryListView.ExpandNode(HistoryListView.Nodes[idx]);
                        }
                    }
                    HistoryListView.BuildTreeList();
                }
            }
            finally
            {
                HistoryListView.EndUpdate();
            }
        }

        private void HistoryListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            P4.FileHistory newRevisionDetail = null;
            if ((HistoryListView.SelectedItems != null) && (HistoryListView.SelectedItems.Count > 0))
            {
                newRevisionDetail = (P4.FileHistory)HistoryListView.SelectedItems[0].Tag;
            }

            if (newRevisionDetail == null)
            {
                EnvDTE.DTE dte2;
                dte2 = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
                if ((HistoryListView.SelectedItems != null) && (HistoryListView.SelectedItems.Count > 0))
                {
                    if (System.IO.File.Exists(HistoryListView.SelectedItems[0].Text))
                    {
                        try
                        {
                            if (dte2 != null) dte2.ItemOperations.OpenFile(HistoryListView.SelectedItems[0].Text, null);
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                ViewThisRevisionTSMI_Click(sender, e);
            }
        }

        private void timelapseViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            P4.FileHistory newRevisionDetail = null;
            if ((HistoryListView.SelectedItems != null) && (HistoryListView.SelectedItems.Count > 0))
            {
                newRevisionDetail = (P4.FileHistory)HistoryListView.SelectedItems[0].Tag;
            }

            if (newRevisionDetail == null)
            {
                if ((HistoryListView.SelectedItems != null) && (HistoryListView.SelectedItems.Count > 0))
                {
                    if (System.IO.File.Exists(HistoryListView.SelectedItems[0].Text))
                    {
                        Scm.LaunchTimeLapseView(HistoryListView.SelectedItems[0].Text);
                    }
                }
            }
            else
            {
                Scm.LaunchTimeLapseView(newRevisionDetail.DepotPath.Path +
                    "#" + newRevisionDetail.Revision.ToString());
            }

        }

        private void revisionGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            P4.FileHistory newRevisionDetail = null;
            if ((HistoryListView.SelectedItems != null) && (HistoryListView.SelectedItems.Count > 0))
            {
                newRevisionDetail = (P4.FileHistory)HistoryListView.SelectedItems[0].Tag;
            }

            if (newRevisionDetail == null)
            {
                if ((HistoryListView.SelectedItems != null) && (HistoryListView.SelectedItems.Count > 0))
                {
                    if (System.IO.File.Exists(HistoryListView.SelectedItems[0].Text))
                    {
                        Scm.LaunchRevisionGraphView(HistoryListView.SelectedItems[0].Text);
                    }
                }
            }
            else
            {
                Scm.LaunchRevisionGraphView(newRevisionDetail.DepotPath.Path +
                    "#" + newRevisionDetail.Revision.ToString());
            }
        }

        private void diffSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            P4.FileHistory firstSelection = (P4.FileHistory)HistoryListView.SelectedItems[0].Tag;
            P4.FileHistory secondSelection = (P4.FileHistory)HistoryListView.SelectedItems[1].Tag;
            P4.FileSpec fs1 = null;
            P4.FileSpec fs2 = null;

            if (firstSelection != null)
            {
                fs1 = P4.FileSpec.DepotSpec(firstSelection.DepotPath.Path, firstSelection.Revision);
            }
            if (secondSelection != null)
            {
                fs2 = P4.FileSpec.DepotSpec(secondSelection.DepotPath.Path, secondSelection.Revision);
            }
            Scm.Diff2Files(fs1, null, fs2, null);
        }

        private void HistoryListView_Enter(object sender, EventArgs e)
        {
            if (HistoryListView.Items != null && HistoryListView.Items.Count > 0)
            {
                if (HistoryListView.SelectedItems == null || HistoryListView.SelectedItems.Count < 1)
                {
                    HistoryListView.Items[0].Selected = true;
                }
            }
        }
    }
}



