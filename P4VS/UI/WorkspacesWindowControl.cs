
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
using System.Threading;
using System.Net;
using Perforce.P4;
using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Label = System.Windows.Forms.Label;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
    /// <summary>
    /// Summary description for P4ToolWindowControl.
    /// </summary>
    public class WorkspaceToolWindowControl : P4ToolWindowControlBase
    {
        public enum nodeType
        {
            None, ClassicWorkspace, StreamsWorkspace
        }
        private class workspacesTreeListViewItem : TreeListViewItem
        {
            public workspacesTreeListViewItem() { }
            public workspacesTreeListViewItem(TreeListViewItem parentItem, string itemText, bool fullLine, nodeType t)
                : base(parentItem, itemText, fullLine) { NodeType = t; }
            public workspacesTreeListViewItem(TreeListViewItem parentItem, string[] items, nodeType t)
                : base(parentItem, items) { NodeType = t; }
            public nodeType NodeType { get; set; }
        }
        public ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem createWorkspaceFromToolStripMenuItem;
        private ToolStripMenuItem editWorkspaceToolStripMenuItem;
        private ToolStripMenuItem deleteWorkspaceToolStripMenuItem;
        private ToolStripMenuItem refreshWorkspaceToolStripMenuItem;
        private ToolStripMenuItem getRevisionToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem newWorkspaceToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem refreshWorkspaceListToolStripMenuItem;

        private I18nControls.GridGroupBox dividerGB;
        private I18nControls.GridLabel workspaceMatchesLbl;
        private I18nControls.GridButton filterBtn;
        private I18nControls.GridCheckBox nameCaseFilterChk;
        private I18nControls.GridCheckBox workspaceHostFilterChk;
        private I18nControls.GridLabel streamVarLbl;
        private I18nControls.GridLabel ownerCBLbl;
        private I18nControls.GridLabel nameContainsLbl;
        private SplitContainer splitContainer1;
        private I18nControls.GridTreeListView workspacesTreeListView;
        private ColumnHeader workspace;
        private ColumnHeader dateModified;
        private ColumnHeader lastAccessed;
        private ColumnHeader owner;
        private ColumnHeader host;
        private ColumnHeader root;
        private ColumnHeader streamRoot;
        private ColumnHeader description;
        private UI.ThreadMonitorControl threadMonitorControl1;
        private I18nControls.GridLayoutPanel panel1;
        private I18nControls.GridTextBox descriptionTB;
        private I18nControls.GridLabel streamLbl;
        private I18nControls.GridLabel altRootsLbl;
        private I18nControls.GridLabel optionsLbl;
        private I18nControls.GridLabel workspaceRootLbl;
        private I18nControls.GridLabel workspaceStreamRootLbl;
        private I18nControls.GridLabel rootLbl;
        private I18nControls.GridLabel workspaceHostNameLbl;
        private I18nControls.GridLabel viewLbl;
        private I18nControls.GridLabel workspaceSubmitOptionsLbl;
        private I18nControls.GridLabel workspaceLastAccessedLbl;
        private I18nControls.GridLabel OwnerLbl;
        private I18nControls.GridLabel workspaceNameLbl;
        private I18nControls.GridLabel hostLbl;
        private I18nControls.GridLabel descriptionLbl;
        private I18nControls.GridLabel workspaceLineEndingsLbl;
        private I18nControls.GridLabel lineEndingsLbl;
        private I18nControls.GridLabel submitOptionsLbl;
        private I18nControls.GridLabel workspaceOwnerNameLbl;
        private I18nControls.GridLabel workspaceUpdatedLbl;
        private I18nControls.GridLabel lastAccessedLbl;
        private I18nControls.GridLabel dateModifiedLbl;
        private I18nControls.GridLabel workspaceLbl;
        private I18nControls.GridLabel severIDLbl;
        private I18nControls.GridLabel workspaceSeverIDLbl;
        private I18nControls.GridLabel streamAtChangeLbl;
        private I18nControls.GridLabel workspaceStreamAtChangeLbl;
        private ColumnHeader streamAtChange;
        private ColumnHeader serverID;
        private I18nControls.GridTextBox altRootsTB;
        private ListViewColumnSorter lvwColumnSorter;
        private I18nControls.GridTextBox viewTB;

        public new P4ScmProvider Scm { get; set; }
        public string stream { get; set; }
        public string integAction { get; set; }
        public IList<P4.Client> streamsWorkspaces { get; set; }
        public bool hostOnly { get; set; }
        public string sentOwner { get; set; }
        public string NameFilterText { get; set; }
        public string OwnerCBText { get; set; }
        public P4.Client SelectedWorkspace { get; private set; }

        //private MRUList _recentWorkspacesOwners = null;
        //private MRUList _recentWorkspacesNames = null;

        private IContainer components;
        private I18nControls.GridFilterComboBox ownerCB;
        private I18nControls.GridFilterComboBox nameContainsCB;
        private bool selectionChangedByLoad = false;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridLayoutPanel gridLayoutPanel2;
        private I18nControls.GridLayoutSubpanel panel6;
        private I18nControls.GridCheckBox rmdirChk;
        private I18nControls.GridCheckBox modtimeChk;
        private I18nControls.GridCheckBox lockedChk;
        private I18nControls.GridCheckBox compressChk;
        private I18nControls.GridCheckBox clobberChk;
        private I18nControls.GridCheckBox allwriteChk;
        private ImageList imageList1;

        public WorkspaceToolWindowControl()
        {
            PreferenceKey = "WorkspacesToolWindowControl";
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            base.Initialize();

            // if the mru lists have not been loaded, see if the old preference exists
            if (Preferences.LocalSettings != null)
            {
                if ((ownerCB.mruLoaded == false) && (Preferences.LocalSettings.ContainsKey("RecentWorkspacesOwners")))
                {
                    MRUList value = (MRUList)Preferences.LocalSettings["RecentWorkspacesOwners"];
                    if (value != null)
                    {
                        ownerCB.mruValues = value.Clone();
                        ownerCB.mruLoaded = true;
                    }
                }
                if ((nameContainsCB.mruLoaded == false) && (Preferences.LocalSettings.ContainsKey("RecentWorkspacesNames")))
                {
                    MRUList value = (MRUList)Preferences.LocalSettings["RecentWorkspacesNames"];
                    if (value != null)
                    {
                        nameContainsCB.mruValues = value.Clone();
                        nameContainsCB.mruLoaded = true;
                    }
                }

            }
            imageList1 = new System.Windows.Forms.ImageList(components);
            // 
            // imageList1
            // 
            imageList1.TransparentColor = System.Drawing.Color.Transparent;
            imageList1.Images.Add("clients_icon.png", Images.clients_icon);
            imageList1.Images.Add("stream_clients_icon.png", Images.stream_clients_icon);

            workspacesTreeListView.LargeImageList = imageList1;
            workspacesTreeListView.SmallImageList = imageList1;

            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            this.workspacesTreeListView.ListViewItemSorter = lvwColumnSorter;

            if (hostOnly == true)
            {
                this.workspaceHostFilterChk.Checked = true;
                this.workspaceHostFilterChk.Enabled = true;
            }
            if (sentOwner != null)
            {
                this.ownerCB.Text = sentOwner;
            }
            // might not need these assigned here
            NameFilterText = nameContainsCB.Text;
            OwnerCBText = ownerCB.Text;
            threadMonitorControl1.Visible = false;

            //newConection = new P4VsProvider.NewConnectionDelegate(OnNewConnection);
            P4VsProvider.NewConnection += newConection;
#if VS2012
            if (!DesignMode)
            {
                base.InitThemeManager();
            }
#endif
            checkConnection();
        }

        public WorkspaceToolWindowControl(P4ScmProvider scm)
            : base(scm)
        {
            PreferenceKey = "WorkspacesToolWindowControl";
            Scm = scm;
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            base.Initialize();

            imageList1 = new System.Windows.Forms.ImageList(components);
            // 
            // imageList1
            // 
            imageList1.TransparentColor = System.Drawing.Color.Transparent;
            imageList1.Images.Add("clients_icon.png", Images.clients_icon);
            imageList1.Images.Add("stream_clients_icon.png", Images.stream_clients_icon);

            workspacesTreeListView.LargeImageList = imageList1;
            workspacesTreeListView.SmallImageList = imageList1;

            newConection = new P4VsProvider.NewConnectionDelegate(OnNewConnection);
            P4VsProvider.NewConnection += newConection;
#if VS2012
            if (!DesignMode)
            {
                base.InitThemeManager();
            }
#endif
            checkConnection();
        }

        private delegate void setFilterBtnDelegate(bool filter);

        private void setFilterBtnBool(bool enabled)
        {
            this.filterBtn.Enabled = enabled;
        }

        private P4VsProvider.NewConnectionDelegate newConection;

        public override void OnNewConnection(P4ScmProvider newScm)
        {
            Scm = newScm;

            // refilter;
            this.workspacesTreeListView.Items.Clear();
            clearDetails();

            filterBtn.Enabled = (Scm != null) && Scm.Connected;
            if (Scm != null)
            {
                if (ownerCB.Text == "" &&
                 ownerCB.mruValues[1] == null)
                {
                    ownerCB.Text = Scm.Connection.User;
                }

                //if (ownerCB.Text == "" &&
                //     !(ownerCB.mruValues.Contains("")))
                //{
                //    ownerCB.Text = Scm.User;
                //}

                if (Scm.ServerVersion < Versions.V11_1)
                {
                    nameCaseFilterChk.Enabled = false;
                    nameCaseFilterChk.Checked = true;
                }
                else
                {
                    nameCaseFilterChk.Enabled = true;
                    nameCaseFilterChk.Checked = false;
                }
            }
            refreshWorkspacesList();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkspaceToolWindowControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.threadMonitorControl1 = new Perforce.P4VS.UI.ThreadMonitorControl();
            this.gridLayoutPanel2 = new Perforce.I18nControls.GridLayoutPanel();
            this.workspacesTreeListView = new Perforce.I18nControls.GridTreeListView();
            this.workspace = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dateModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lastAccessed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.owner = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.host = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.root = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.streamRoot = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.streamAtChange = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.serverID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.getRevisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.newWorkspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createWorkspaceFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editWorkspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteWorkspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshWorkspaceListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshWorkspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ownerCB = new Perforce.I18nControls.GridFilterComboBox();
            this.dividerGB = new Perforce.I18nControls.GridGroupBox();
            this.nameCaseFilterChk = new Perforce.I18nControls.GridCheckBox();
            this.nameContainsCB = new Perforce.I18nControls.GridFilterComboBox();
            this.workspaceHostFilterChk = new Perforce.I18nControls.GridCheckBox();
            this.filterBtn = new Perforce.I18nControls.GridButton();
            this.streamVarLbl = new Perforce.I18nControls.GridLabel();
            this.ownerCBLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceMatchesLbl = new Perforce.I18nControls.GridLabel();
            this.nameContainsLbl = new Perforce.I18nControls.GridLabel();
            this.panel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.panel6 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.rmdirChk = new Perforce.I18nControls.GridCheckBox();
            this.modtimeChk = new Perforce.I18nControls.GridCheckBox();
            this.lockedChk = new Perforce.I18nControls.GridCheckBox();
            this.compressChk = new Perforce.I18nControls.GridCheckBox();
            this.clobberChk = new Perforce.I18nControls.GridCheckBox();
            this.allwriteChk = new Perforce.I18nControls.GridCheckBox();
            this.viewTB = new Perforce.I18nControls.GridTextBox();
            this.altRootsTB = new Perforce.I18nControls.GridTextBox();
            this.severIDLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceSeverIDLbl = new Perforce.I18nControls.GridLabel();
            this.streamAtChangeLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceStreamAtChangeLbl = new Perforce.I18nControls.GridLabel();
            this.descriptionTB = new Perforce.I18nControls.GridTextBox();
            this.streamLbl = new Perforce.I18nControls.GridLabel();
            this.altRootsLbl = new Perforce.I18nControls.GridLabel();
            this.optionsLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceRootLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceStreamRootLbl = new Perforce.I18nControls.GridLabel();
            this.rootLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceHostNameLbl = new Perforce.I18nControls.GridLabel();
            this.viewLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceSubmitOptionsLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceLastAccessedLbl = new Perforce.I18nControls.GridLabel();
            this.OwnerLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceNameLbl = new Perforce.I18nControls.GridLabel();
            this.hostLbl = new Perforce.I18nControls.GridLabel();
            this.descriptionLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceLineEndingsLbl = new Perforce.I18nControls.GridLabel();
            this.lineEndingsLbl = new Perforce.I18nControls.GridLabel();
            this.submitOptionsLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceOwnerNameLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceUpdatedLbl = new Perforce.I18nControls.GridLabel();
            this.lastAccessedLbl = new Perforce.I18nControls.GridLabel();
            this.dateModifiedLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceLbl = new Perforce.I18nControls.GridLabel();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gridLayoutPanel2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel6.SuspendLayout();
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
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainer1.Panel1.Controls.Add(this.threadMonitorControl1);
            this.splitContainer1.Panel1.Controls.Add(this.gridLayoutPanel2);
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.TabStop = false;
            // 
            // threadMonitorControl1
            // 
            this.threadMonitorControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.threadMonitorControl1.CancelPressed = false;
            resources.ApplyResources(this.threadMonitorControl1, "threadMonitorControl1");
            this.threadMonitorControl1.Maximum = 100;
            this.threadMonitorControl1.Name = "threadMonitorControl1";
            this.threadMonitorControl1.Step = 1;
            this.threadMonitorControl1.TabStop = false;
            this.threadMonitorControl1.Value = 50;
            // 
            // gridLayoutPanel2
            // 
            this.gridLayoutPanel2.Controls.Add(this.workspacesTreeListView);
            this.gridLayoutPanel2.Controls.Add(this.ownerCB);
            this.gridLayoutPanel2.Controls.Add(this.dividerGB);
            this.gridLayoutPanel2.Controls.Add(this.nameCaseFilterChk);
            this.gridLayoutPanel2.Controls.Add(this.nameContainsCB);
            this.gridLayoutPanel2.Controls.Add(this.workspaceHostFilterChk);
            this.gridLayoutPanel2.Controls.Add(this.filterBtn);
            this.gridLayoutPanel2.Controls.Add(this.streamVarLbl);
            this.gridLayoutPanel2.Controls.Add(this.ownerCBLbl);
            this.gridLayoutPanel2.Controls.Add(this.workspaceMatchesLbl);
            this.gridLayoutPanel2.Controls.Add(this.nameContainsLbl);
            resources.ApplyResources(this.gridLayoutPanel2, "gridLayoutPanel2");
            this.gridLayoutPanel2.EnableDesignerGrid = false;
            this.gridLayoutPanel2.EnableDesignerLayout = true;
            this.gridLayoutPanel2.EnableParentResize = false;
            this.gridLayoutPanel2.MinimumColumnWidth = 10;
            this.gridLayoutPanel2.MinimumRowHeight = 10;
            this.gridLayoutPanel2.Name = "gridLayoutPanel2";
            // 
            // workspacesTreeListView
            // 
            this.workspacesTreeListView._maxLineOffset = 0;
            this.workspacesTreeListView.ActionColumn = -1;
            this.workspacesTreeListView.AllowColumnReorder = true;
            resources.ApplyResources(this.workspacesTreeListView, "workspacesTreeListView");
            this.workspacesTreeListView.CellHeight = 148;
            this.workspacesTreeListView.CellWidth = 576;
            this.workspacesTreeListView.Column = 0;
            this.workspacesTreeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.workspace,
            this.dateModified,
            this.lastAccessed,
            this.owner,
            this.host,
            this.root,
            this.streamRoot,
            this.streamAtChange,
            this.serverID,
            this.description});
            this.workspacesTreeListView.ColumnsSpanned = 5;
            this.workspacesTreeListView.ContextMenuStrip = this.contextMenuStrip1;
            this.workspacesTreeListView.EnableIconOverlays = false;
            this.workspacesTreeListView.EnableSorting = true;
            this.workspacesTreeListView.FullRowSelect = true;
            this.workspacesTreeListView.GridLines = true;
            this.workspacesTreeListView.MultiSelect = false;
            this.workspacesTreeListView.MultiSelectConditions = Perforce.P4VS.TreeListView.MultiSelectCondition.none;
            this.workspacesTreeListView.Name = "workspacesTreeListView";
            this.workspacesTreeListView.OverlayOffset = 0;
            this.workspacesTreeListView.RootCheckBoxes = false;
            this.workspacesTreeListView.Row = 3;
            this.workspacesTreeListView.RowsSpanned = 0;
            this.workspacesTreeListView.ScrollPosition = 0;
            this.workspacesTreeListView.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.workspacesTreeListView.TreeView = false;
            this.workspacesTreeListView.UseClassicImageList = false;
            this.workspacesTreeListView.UseCompatibleStateImageBehavior = false;
            this.workspacesTreeListView.View = System.Windows.Forms.View.Details;
            this.workspacesTreeListView.YOffset = 0;
            this.workspacesTreeListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.workspacesTreeListView_ColumnClick);
            this.workspacesTreeListView.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.workspacesTreeListView_ColumnReordered);
            this.workspacesTreeListView.SelectedIndexChanged += new System.EventHandler(this.workspacesTreeListView_SelectedIndexChanged);
            this.workspacesTreeListView.Click += new System.EventHandler(this.workspacesTreeListView_Click);
            this.workspacesTreeListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.workspacesTreeListView_MouseDoubleClick);
            // 
            // workspace
            // 
            resources.ApplyResources(this.workspace, "workspace");
            // 
            // dateModified
            // 
            resources.ApplyResources(this.dateModified, "dateModified");
            // 
            // lastAccessed
            // 
            resources.ApplyResources(this.lastAccessed, "lastAccessed");
            // 
            // owner
            // 
            resources.ApplyResources(this.owner, "owner");
            // 
            // host
            // 
            resources.ApplyResources(this.host, "host");
            // 
            // root
            // 
            resources.ApplyResources(this.root, "root");
            // 
            // streamRoot
            // 
            resources.ApplyResources(this.streamRoot, "streamRoot");
            // 
            // streamAtChange
            // 
            resources.ApplyResources(this.streamAtChange, "streamAtChange");
            // 
            // serverID
            // 
            resources.ApplyResources(this.serverID, "serverID");
            // 
            // description
            // 
            resources.ApplyResources(this.description, "description");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getRevisionToolStripMenuItem,
            this.toolStripSeparator1,
            this.newWorkspaceToolStripMenuItem,
            this.createWorkspaceFromToolStripMenuItem,
            this.editWorkspaceToolStripMenuItem,
            this.deleteWorkspaceToolStripMenuItem,
            this.toolStripSeparator2,
            this.refreshWorkspaceListToolStripMenuItem,
            this.refreshWorkspaceToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // getRevisionToolStripMenuItem
            // 
            this.getRevisionToolStripMenuItem.Name = "getRevisionToolStripMenuItem";
            resources.ApplyResources(this.getRevisionToolStripMenuItem, "getRevisionToolStripMenuItem");
            this.getRevisionToolStripMenuItem.Click += new System.EventHandler(this.getRevisionToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // newWorkspaceToolStripMenuItem
            // 
            this.newWorkspaceToolStripMenuItem.Name = "newWorkspaceToolStripMenuItem";
            resources.ApplyResources(this.newWorkspaceToolStripMenuItem, "newWorkspaceToolStripMenuItem");
            this.newWorkspaceToolStripMenuItem.Click += new System.EventHandler(this.newWorkspaceToolStripMenuItem_Click);
            // 
            // createWorkspaceFromToolStripMenuItem
            // 
            this.createWorkspaceFromToolStripMenuItem.Name = "createWorkspaceFromToolStripMenuItem";
            resources.ApplyResources(this.createWorkspaceFromToolStripMenuItem, "createWorkspaceFromToolStripMenuItem");
            this.createWorkspaceFromToolStripMenuItem.Click += new System.EventHandler(this.createWorkspaceFromToolStripMenuItem_Click);
            // 
            // editWorkspaceToolStripMenuItem
            // 
            this.editWorkspaceToolStripMenuItem.Name = "editWorkspaceToolStripMenuItem";
            resources.ApplyResources(this.editWorkspaceToolStripMenuItem, "editWorkspaceToolStripMenuItem");
            this.editWorkspaceToolStripMenuItem.Click += new System.EventHandler(this.editWorkspaceToolStripMenuItem_Click);
            // 
            // deleteWorkspaceToolStripMenuItem
            // 
            this.deleteWorkspaceToolStripMenuItem.Name = "deleteWorkspaceToolStripMenuItem";
            resources.ApplyResources(this.deleteWorkspaceToolStripMenuItem, "deleteWorkspaceToolStripMenuItem");
            this.deleteWorkspaceToolStripMenuItem.Click += new System.EventHandler(this.deleteWorkspaceToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // refreshWorkspaceListToolStripMenuItem
            // 
            this.refreshWorkspaceListToolStripMenuItem.Name = "refreshWorkspaceListToolStripMenuItem";
            resources.ApplyResources(this.refreshWorkspaceListToolStripMenuItem, "refreshWorkspaceListToolStripMenuItem");
            this.refreshWorkspaceListToolStripMenuItem.Click += new System.EventHandler(this.refreshWorkspaceListToolStripMenuItem_Click);
            // 
            // refreshWorkspaceToolStripMenuItem
            // 
            this.refreshWorkspaceToolStripMenuItem.Name = "refreshWorkspaceToolStripMenuItem";
            resources.ApplyResources(this.refreshWorkspaceToolStripMenuItem, "refreshWorkspaceToolStripMenuItem");
            this.refreshWorkspaceToolStripMenuItem.Click += new System.EventHandler(this.refreshWorkspaceToolStripMenuItem_Click);
            // 
            // ownerCB
            // 
            resources.ApplyResources(this.ownerCB, "ownerCB");
            this.ownerCB.CellHeight = 29;
            this.ownerCB.CellWidth = 137;
            this.ownerCB.Column = 1;
            this.ownerCB.ColumnsSpanned = 0;
            this.ownerCB.FormattingEnabled = true;
            this.ownerCB.Name = "ownerCB";
            this.ownerCB.Row = 0;
            this.ownerCB.RowsSpanned = 0;
            this.ownerCB.YOffset = 1;
            this.ownerCB.SelectedIndexChanged += new System.EventHandler(this.ownerCB_SelectedIndexChanged);
            this.ownerCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OwnerCB_KeyDown);
            // 
            // dividerGB
            // 
            resources.ApplyResources(this.dividerGB, "dividerGB");
            this.dividerGB.CellHeight = 65;
            this.dividerGB.CellWidth = 10;
            this.dividerGB.Column = 4;
            this.dividerGB.ColumnsSpanned = 0;
            this.dividerGB.Name = "dividerGB";
            this.dividerGB.Row = 0;
            this.dividerGB.RowsSpanned = 2;
            this.dividerGB.TabStop = false;
            this.dividerGB.YOffset = 0;
            // 
            // nameCaseFilterChk
            // 
            resources.ApplyResources(this.nameCaseFilterChk, "nameCaseFilterChk");
            this.nameCaseFilterChk.CellHeight = 23;
            this.nameCaseFilterChk.CellWidth = 214;
            this.nameCaseFilterChk.Column = 3;
            this.nameCaseFilterChk.ColumnsSpanned = 0;
            this.nameCaseFilterChk.Name = "nameCaseFilterChk";
            this.nameCaseFilterChk.Row = 1;
            this.nameCaseFilterChk.RowsSpanned = 0;
            this.nameCaseFilterChk.UseVisualStyleBackColor = true;
            this.nameCaseFilterChk.YOffset = 3;
            // 
            // nameContainsCB
            // 
            resources.ApplyResources(this.nameContainsCB, "nameContainsCB");
            this.nameContainsCB.CellHeight = 29;
            this.nameContainsCB.CellWidth = 214;
            this.nameContainsCB.Column = 3;
            this.nameContainsCB.ColumnsSpanned = 0;
            this.nameContainsCB.FormattingEnabled = true;
            this.nameContainsCB.Name = "nameContainsCB";
            this.nameContainsCB.Row = 0;
            this.nameContainsCB.RowsSpanned = 0;
            this.nameContainsCB.YOffset = 1;
            this.nameContainsCB.SelectedIndexChanged += new System.EventHandler(this.nameContainsCB_SelectedIndexChanged);
            this.nameContainsCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NameFilterCB_KeyDown);
            // 
            // workspaceHostFilterChk
            // 
            resources.ApplyResources(this.workspaceHostFilterChk, "workspaceHostFilterChk");
            this.workspaceHostFilterChk.CellHeight = 23;
            this.workspaceHostFilterChk.CellWidth = 271;
            this.workspaceHostFilterChk.Column = 0;
            this.workspaceHostFilterChk.ColumnsSpanned = 2;
            this.workspaceHostFilterChk.Name = "workspaceHostFilterChk";
            this.workspaceHostFilterChk.Row = 1;
            this.workspaceHostFilterChk.RowsSpanned = 0;
            this.workspaceHostFilterChk.UseVisualStyleBackColor = true;
            this.workspaceHostFilterChk.YOffset = 3;
            // 
            // filterBtn
            // 
            resources.ApplyResources(this.filterBtn, "filterBtn");
            this.filterBtn.CellHeight = 29;
            this.filterBtn.CellWidth = 81;
            this.filterBtn.Column = 5;
            this.filterBtn.ColumnsSpanned = 0;
            this.filterBtn.Name = "filterBtn";
            this.filterBtn.Row = 0;
            this.filterBtn.RowsSpanned = 0;
            this.filterBtn.UseVisualStyleBackColor = true;
            this.filterBtn.YOffset = 0;
            this.filterBtn.EnabledChanged += new System.EventHandler(this.filterBtn_EnabledChanged);
            this.filterBtn.Click += new System.EventHandler(this.filterBtn_Click);
            // 
            // streamVarLbl
            // 
            resources.ApplyResources(this.streamVarLbl, "streamVarLbl");
            this.streamVarLbl.CellHeight = 13;
            this.streamVarLbl.CellWidth = 214;
            this.streamVarLbl.Column = 3;
            this.streamVarLbl.ColumnsSpanned = 0;
            this.streamVarLbl.Name = "streamVarLbl";
            this.streamVarLbl.Row = 2;
            this.streamVarLbl.RowsSpanned = 0;
            this.streamVarLbl.YOffset = 0;
            // 
            // ownerCBLbl
            // 
            resources.ApplyResources(this.ownerCBLbl, "ownerCBLbl");
            this.ownerCBLbl.CellHeight = 29;
            this.ownerCBLbl.CellWidth = 47;
            this.ownerCBLbl.Column = 0;
            this.ownerCBLbl.ColumnsSpanned = 0;
            this.ownerCBLbl.Name = "ownerCBLbl";
            this.ownerCBLbl.Row = 0;
            this.ownerCBLbl.RowsSpanned = 0;
            this.ownerCBLbl.YOffset = 5;
            // 
            // workspaceMatchesLbl
            // 
            resources.ApplyResources(this.workspaceMatchesLbl, "workspaceMatchesLbl");
            this.workspaceMatchesLbl.AutoEllipsis = true;
            this.workspaceMatchesLbl.CellHeight = 23;
            this.workspaceMatchesLbl.CellWidth = 81;
            this.workspaceMatchesLbl.Column = 5;
            this.workspaceMatchesLbl.ColumnsSpanned = 0;
            this.workspaceMatchesLbl.Name = "workspaceMatchesLbl";
            this.workspaceMatchesLbl.Row = 1;
            this.workspaceMatchesLbl.RowsSpanned = 0;
            this.workspaceMatchesLbl.YOffset = 0;
            // 
            // nameContainsLbl
            // 
            resources.ApplyResources(this.nameContainsLbl, "nameContainsLbl");
            this.nameContainsLbl.CellHeight = 29;
            this.nameContainsLbl.CellWidth = 87;
            this.nameContainsLbl.Column = 2;
            this.nameContainsLbl.ColumnsSpanned = 0;
            this.nameContainsLbl.Name = "nameContainsLbl";
            this.nameContainsLbl.Row = 0;
            this.nameContainsLbl.RowsSpanned = 0;
            this.nameContainsLbl.YOffset = 5;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.viewTB);
            this.panel1.Controls.Add(this.altRootsTB);
            this.panel1.Controls.Add(this.severIDLbl);
            this.panel1.Controls.Add(this.workspaceSeverIDLbl);
            this.panel1.Controls.Add(this.streamAtChangeLbl);
            this.panel1.Controls.Add(this.workspaceStreamAtChangeLbl);
            this.panel1.Controls.Add(this.descriptionTB);
            this.panel1.Controls.Add(this.streamLbl);
            this.panel1.Controls.Add(this.altRootsLbl);
            this.panel1.Controls.Add(this.optionsLbl);
            this.panel1.Controls.Add(this.workspaceRootLbl);
            this.panel1.Controls.Add(this.workspaceStreamRootLbl);
            this.panel1.Controls.Add(this.rootLbl);
            this.panel1.Controls.Add(this.workspaceHostNameLbl);
            this.panel1.Controls.Add(this.viewLbl);
            this.panel1.Controls.Add(this.workspaceSubmitOptionsLbl);
            this.panel1.Controls.Add(this.workspaceLastAccessedLbl);
            this.panel1.Controls.Add(this.OwnerLbl);
            this.panel1.Controls.Add(this.workspaceNameLbl);
            this.panel1.Controls.Add(this.hostLbl);
            this.panel1.Controls.Add(this.descriptionLbl);
            this.panel1.Controls.Add(this.workspaceLineEndingsLbl);
            this.panel1.Controls.Add(this.lineEndingsLbl);
            this.panel1.Controls.Add(this.submitOptionsLbl);
            this.panel1.Controls.Add(this.workspaceOwnerNameLbl);
            this.panel1.Controls.Add(this.workspaceUpdatedLbl);
            this.panel1.Controls.Add(this.lastAccessedLbl);
            this.panel1.Controls.Add(this.dateModifiedLbl);
            this.panel1.Controls.Add(this.workspaceLbl);
            this.panel1.EnableDesignerGrid = false;
            this.panel1.EnableDesignerLayout = true;
            this.panel1.EnableParentResize = false;
            this.panel1.MinimumColumnWidth = 10;
            this.panel1.MinimumRowHeight = 10;
            this.panel1.Name = "panel1";
            // 
            // panel6
            // 
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.CellHeight = 36;
            this.panel6.CellWidth = 454;
            this.panel6.Column = 1;
            this.panel6.ColumnsSpanned = 2;
            this.panel6.Controls.Add(this.rmdirChk);
            this.panel6.Controls.Add(this.modtimeChk);
            this.panel6.Controls.Add(this.lockedChk);
            this.panel6.Controls.Add(this.compressChk);
            this.panel6.Controls.Add(this.clobberChk);
            this.panel6.Controls.Add(this.allwriteChk);
            this.panel6.EnableDesignerGrid = false;
            this.panel6.EnableDesignerLayout = true;
            this.panel6.EnableParentResize = false;
            this.panel6.MinimumColumnWidth = 10;
            this.panel6.MinimumRowHeight = 10;
            this.panel6.Name = "panel6";
            this.panel6.Row = 7;
            this.panel6.RowsSpanned = 0;
            this.panel6.YOffset = 0;
            // 
            // rmdirChk
            // 
            resources.ApplyResources(this.rmdirChk, "rmdirChk");
            this.rmdirChk.CellHeight = 26;
            this.rmdirChk.CellWidth = 77;
            this.rmdirChk.Column = 5;
            this.rmdirChk.ColumnsSpanned = 0;
            this.rmdirChk.Name = "rmdirChk";
            this.rmdirChk.Row = 0;
            this.rmdirChk.RowsSpanned = 0;
            this.rmdirChk.TabStop = false;
            this.rmdirChk.UseVisualStyleBackColor = true;
            this.rmdirChk.YOffset = 0;
            this.rmdirChk.CheckedChanged += new System.EventHandler(this.rmdirChk_CheckedChanged);
            // 
            // modtimeChk
            // 
            resources.ApplyResources(this.modtimeChk, "modtimeChk");
            this.modtimeChk.CellHeight = 26;
            this.modtimeChk.CellWidth = 77;
            this.modtimeChk.Column = 4;
            this.modtimeChk.ColumnsSpanned = 0;
            this.modtimeChk.Name = "modtimeChk";
            this.modtimeChk.Row = 0;
            this.modtimeChk.RowsSpanned = 0;
            this.modtimeChk.TabStop = false;
            this.modtimeChk.UseVisualStyleBackColor = true;
            this.modtimeChk.YOffset = 0;
            this.modtimeChk.CheckedChanged += new System.EventHandler(this.modtimeChk_CheckedChanged);
            // 
            // lockedChk
            // 
            resources.ApplyResources(this.lockedChk, "lockedChk");
            this.lockedChk.CellHeight = 26;
            this.lockedChk.CellWidth = 77;
            this.lockedChk.Column = 3;
            this.lockedChk.ColumnsSpanned = 0;
            this.lockedChk.Name = "lockedChk";
            this.lockedChk.Row = 0;
            this.lockedChk.RowsSpanned = 0;
            this.lockedChk.TabStop = false;
            this.lockedChk.UseVisualStyleBackColor = true;
            this.lockedChk.YOffset = 0;
            this.lockedChk.CheckedChanged += new System.EventHandler(this.lockedChk_CheckedChanged);
            // 
            // compressChk
            // 
            resources.ApplyResources(this.compressChk, "compressChk");
            this.compressChk.CellHeight = 26;
            this.compressChk.CellWidth = 77;
            this.compressChk.Column = 2;
            this.compressChk.ColumnsSpanned = 0;
            this.compressChk.Name = "compressChk";
            this.compressChk.Row = 0;
            this.compressChk.RowsSpanned = 0;
            this.compressChk.TabStop = false;
            this.compressChk.UseVisualStyleBackColor = true;
            this.compressChk.YOffset = 0;
            this.compressChk.CheckedChanged += new System.EventHandler(this.compressChk_CheckedChanged);
            // 
            // clobberChk
            // 
            resources.ApplyResources(this.clobberChk, "clobberChk");
            this.clobberChk.CellHeight = 26;
            this.clobberChk.CellWidth = 77;
            this.clobberChk.Column = 1;
            this.clobberChk.ColumnsSpanned = 0;
            this.clobberChk.Name = "clobberChk";
            this.clobberChk.Row = 0;
            this.clobberChk.RowsSpanned = 0;
            this.clobberChk.TabStop = false;
            this.clobberChk.UseVisualStyleBackColor = true;
            this.clobberChk.YOffset = 0;
            this.clobberChk.CheckedChanged += new System.EventHandler(this.clobberChk_CheckedChanged);
            // 
            // allwriteChk
            // 
            resources.ApplyResources(this.allwriteChk, "allwriteChk");
            this.allwriteChk.CellHeight = 26;
            this.allwriteChk.CellWidth = 77;
            this.allwriteChk.Column = 0;
            this.allwriteChk.ColumnsSpanned = 0;
            this.allwriteChk.Name = "allwriteChk";
            this.allwriteChk.Row = 0;
            this.allwriteChk.RowsSpanned = 0;
            this.allwriteChk.TabStop = false;
            this.allwriteChk.UseVisualStyleBackColor = true;
            this.allwriteChk.YOffset = 0;
            this.allwriteChk.CheckedChanged += new System.EventHandler(this.allwriteChk_CheckedChanged);
            // 
            // viewTB
            // 
            resources.ApplyResources(this.viewTB, "viewTB");
            this.viewTB.CellHeight = 210;
            this.viewTB.CellWidth = 454;
            this.viewTB.Column = 1;
            this.viewTB.ColumnsSpanned = 2;
            this.viewTB.Name = "viewTB";
            this.viewTB.ReadOnly = true;
            this.viewTB.Row = 11;
            this.viewTB.RowsSpanned = 0;
            this.viewTB.TabStop = false;
            this.viewTB.YOffset = 9;
            // 
            // altRootsTB
            // 
            resources.ApplyResources(this.altRootsTB, "altRootsTB");
            this.altRootsTB.CellHeight = 45;
            this.altRootsTB.CellWidth = 454;
            this.altRootsTB.Column = 1;
            this.altRootsTB.ColumnsSpanned = 2;
            this.altRootsTB.Name = "altRootsTB";
            this.altRootsTB.ReadOnly = true;
            this.altRootsTB.Row = 6;
            this.altRootsTB.RowsSpanned = 0;
            this.altRootsTB.TabStop = false;
            this.altRootsTB.YOffset = 3;
            // 
            // severIDLbl
            // 
            resources.ApplyResources(this.severIDLbl, "severIDLbl");
            this.severIDLbl.CellHeight = 13;
            this.severIDLbl.CellWidth = 102;
            this.severIDLbl.Column = 0;
            this.severIDLbl.ColumnsSpanned = 0;
            this.severIDLbl.Name = "severIDLbl";
            this.severIDLbl.Row = 10;
            this.severIDLbl.RowsSpanned = 0;
            this.severIDLbl.YOffset = 0;
            // 
            // workspaceSeverIDLbl
            // 
            resources.ApplyResources(this.workspaceSeverIDLbl, "workspaceSeverIDLbl");
            this.workspaceSeverIDLbl.CellHeight = 13;
            this.workspaceSeverIDLbl.CellWidth = 454;
            this.workspaceSeverIDLbl.Column = 1;
            this.workspaceSeverIDLbl.ColumnsSpanned = 2;
            this.workspaceSeverIDLbl.Name = "workspaceSeverIDLbl";
            this.workspaceSeverIDLbl.Row = 10;
            this.workspaceSeverIDLbl.RowsSpanned = 0;
            this.workspaceSeverIDLbl.YOffset = 0;
            // 
            // streamAtChangeLbl
            // 
            resources.ApplyResources(this.streamAtChangeLbl, "streamAtChangeLbl");
            this.streamAtChangeLbl.CellHeight = 13;
            this.streamAtChangeLbl.CellWidth = 102;
            this.streamAtChangeLbl.Column = 0;
            this.streamAtChangeLbl.ColumnsSpanned = 0;
            this.streamAtChangeLbl.Name = "streamAtChangeLbl";
            this.streamAtChangeLbl.Row = 9;
            this.streamAtChangeLbl.RowsSpanned = 0;
            this.streamAtChangeLbl.YOffset = 0;
            // 
            // workspaceStreamAtChangeLbl
            // 
            resources.ApplyResources(this.workspaceStreamAtChangeLbl, "workspaceStreamAtChangeLbl");
            this.workspaceStreamAtChangeLbl.CellHeight = 13;
            this.workspaceStreamAtChangeLbl.CellWidth = 454;
            this.workspaceStreamAtChangeLbl.Column = 1;
            this.workspaceStreamAtChangeLbl.ColumnsSpanned = 2;
            this.workspaceStreamAtChangeLbl.Name = "workspaceStreamAtChangeLbl";
            this.workspaceStreamAtChangeLbl.Row = 9;
            this.workspaceStreamAtChangeLbl.RowsSpanned = 0;
            this.workspaceStreamAtChangeLbl.YOffset = 0;
            // 
            // descriptionTB
            // 
            resources.ApplyResources(this.descriptionTB, "descriptionTB");
            this.descriptionTB.CellHeight = 42;
            this.descriptionTB.CellWidth = 454;
            this.descriptionTB.Column = 1;
            this.descriptionTB.ColumnsSpanned = 2;
            this.descriptionTB.Name = "descriptionTB";
            this.descriptionTB.ReadOnly = true;
            this.descriptionTB.Row = 4;
            this.descriptionTB.RowsSpanned = 0;
            this.descriptionTB.TabStop = false;
            this.descriptionTB.YOffset = 3;
            // 
            // streamLbl
            // 
            resources.ApplyResources(this.streamLbl, "streamLbl");
            this.streamLbl.CellHeight = 13;
            this.streamLbl.CellWidth = 102;
            this.streamLbl.Column = 0;
            this.streamLbl.ColumnsSpanned = 0;
            this.streamLbl.Name = "streamLbl";
            this.streamLbl.Row = 8;
            this.streamLbl.RowsSpanned = 0;
            this.streamLbl.YOffset = 0;
            // 
            // altRootsLbl
            // 
            resources.ApplyResources(this.altRootsLbl, "altRootsLbl");
            this.altRootsLbl.CellHeight = 45;
            this.altRootsLbl.CellWidth = 102;
            this.altRootsLbl.Column = 0;
            this.altRootsLbl.ColumnsSpanned = 0;
            this.altRootsLbl.Name = "altRootsLbl";
            this.altRootsLbl.Row = 6;
            this.altRootsLbl.RowsSpanned = 0;
            this.altRootsLbl.YOffset = 0;
            // 
            // optionsLbl
            // 
            resources.ApplyResources(this.optionsLbl, "optionsLbl");
            this.optionsLbl.CellHeight = 36;
            this.optionsLbl.CellWidth = 102;
            this.optionsLbl.Column = 0;
            this.optionsLbl.ColumnsSpanned = 0;
            this.optionsLbl.Name = "optionsLbl";
            this.optionsLbl.Row = 7;
            this.optionsLbl.RowsSpanned = 0;
            this.optionsLbl.YOffset = 8;
            // 
            // workspaceRootLbl
            // 
            resources.ApplyResources(this.workspaceRootLbl, "workspaceRootLbl");
            this.workspaceRootLbl.CellHeight = 13;
            this.workspaceRootLbl.CellWidth = 454;
            this.workspaceRootLbl.Column = 1;
            this.workspaceRootLbl.ColumnsSpanned = 2;
            this.workspaceRootLbl.Name = "workspaceRootLbl";
            this.workspaceRootLbl.Row = 5;
            this.workspaceRootLbl.RowsSpanned = 0;
            this.workspaceRootLbl.YOffset = 0;
            // 
            // workspaceStreamRootLbl
            // 
            resources.ApplyResources(this.workspaceStreamRootLbl, "workspaceStreamRootLbl");
            this.workspaceStreamRootLbl.CellHeight = 13;
            this.workspaceStreamRootLbl.CellWidth = 185;
            this.workspaceStreamRootLbl.Column = 1;
            this.workspaceStreamRootLbl.ColumnsSpanned = 0;
            this.workspaceStreamRootLbl.Name = "workspaceStreamRootLbl";
            this.workspaceStreamRootLbl.Row = 8;
            this.workspaceStreamRootLbl.RowsSpanned = 0;
            this.workspaceStreamRootLbl.YOffset = 0;
            // 
            // rootLbl
            // 
            resources.ApplyResources(this.rootLbl, "rootLbl");
            this.rootLbl.CellHeight = 13;
            this.rootLbl.CellWidth = 102;
            this.rootLbl.Column = 0;
            this.rootLbl.ColumnsSpanned = 0;
            this.rootLbl.Name = "rootLbl";
            this.rootLbl.Row = 5;
            this.rootLbl.RowsSpanned = 0;
            this.rootLbl.YOffset = 0;
            // 
            // workspaceHostNameLbl
            // 
            resources.ApplyResources(this.workspaceHostNameLbl, "workspaceHostNameLbl");
            this.workspaceHostNameLbl.AutoEllipsis = true;
            this.workspaceHostNameLbl.CellHeight = 13;
            this.workspaceHostNameLbl.CellWidth = 185;
            this.workspaceHostNameLbl.Column = 3;
            this.workspaceHostNameLbl.ColumnsSpanned = 0;
            this.workspaceHostNameLbl.Name = "workspaceHostNameLbl";
            this.workspaceHostNameLbl.Row = 0;
            this.workspaceHostNameLbl.RowsSpanned = 0;
            this.workspaceHostNameLbl.YOffset = 0;
            // 
            // viewLbl
            // 
            resources.ApplyResources(this.viewLbl, "viewLbl");
            this.viewLbl.CellHeight = 210;
            this.viewLbl.CellWidth = 102;
            this.viewLbl.Column = 0;
            this.viewLbl.ColumnsSpanned = 0;
            this.viewLbl.Name = "viewLbl";
            this.viewLbl.Row = 11;
            this.viewLbl.RowsSpanned = 0;
            this.viewLbl.YOffset = 0;
            // 
            // workspaceSubmitOptionsLbl
            // 
            resources.ApplyResources(this.workspaceSubmitOptionsLbl, "workspaceSubmitOptionsLbl");
            this.workspaceSubmitOptionsLbl.AutoEllipsis = true;
            this.workspaceSubmitOptionsLbl.CellHeight = 13;
            this.workspaceSubmitOptionsLbl.CellWidth = 185;
            this.workspaceSubmitOptionsLbl.Column = 3;
            this.workspaceSubmitOptionsLbl.ColumnsSpanned = 0;
            this.workspaceSubmitOptionsLbl.Name = "workspaceSubmitOptionsLbl";
            this.workspaceSubmitOptionsLbl.Row = 1;
            this.workspaceSubmitOptionsLbl.RowsSpanned = 0;
            this.workspaceSubmitOptionsLbl.YOffset = 0;
            // 
            // workspaceLastAccessedLbl
            // 
            resources.ApplyResources(this.workspaceLastAccessedLbl, "workspaceLastAccessedLbl");
            this.workspaceLastAccessedLbl.AutoEllipsis = true;
            this.workspaceLastAccessedLbl.CellHeight = 13;
            this.workspaceLastAccessedLbl.CellWidth = 185;
            this.workspaceLastAccessedLbl.Column = 1;
            this.workspaceLastAccessedLbl.ColumnsSpanned = 0;
            this.workspaceLastAccessedLbl.Name = "workspaceLastAccessedLbl";
            this.workspaceLastAccessedLbl.Row = 2;
            this.workspaceLastAccessedLbl.RowsSpanned = 0;
            this.workspaceLastAccessedLbl.YOffset = 0;
            // 
            // OwnerLbl
            // 
            resources.ApplyResources(this.OwnerLbl, "OwnerLbl");
            this.OwnerLbl.CellHeight = 13;
            this.OwnerLbl.CellWidth = 102;
            this.OwnerLbl.Column = 0;
            this.OwnerLbl.ColumnsSpanned = 0;
            this.OwnerLbl.Name = "OwnerLbl";
            this.OwnerLbl.Row = 3;
            this.OwnerLbl.RowsSpanned = 0;
            this.OwnerLbl.YOffset = 0;
            // 
            // workspaceNameLbl
            // 
            resources.ApplyResources(this.workspaceNameLbl, "workspaceNameLbl");
            this.workspaceNameLbl.AutoEllipsis = true;
            this.workspaceNameLbl.CellHeight = 13;
            this.workspaceNameLbl.CellWidth = 185;
            this.workspaceNameLbl.Column = 1;
            this.workspaceNameLbl.ColumnsSpanned = 0;
            this.workspaceNameLbl.Name = "workspaceNameLbl";
            this.workspaceNameLbl.Row = 0;
            this.workspaceNameLbl.RowsSpanned = 0;
            this.workspaceNameLbl.YOffset = 0;
            // 
            // hostLbl
            // 
            resources.ApplyResources(this.hostLbl, "hostLbl");
            this.hostLbl.CellHeight = 13;
            this.hostLbl.CellWidth = 84;
            this.hostLbl.Column = 2;
            this.hostLbl.ColumnsSpanned = 0;
            this.hostLbl.Name = "hostLbl";
            this.hostLbl.Row = 0;
            this.hostLbl.RowsSpanned = 0;
            this.hostLbl.YOffset = 0;
            // 
            // descriptionLbl
            // 
            resources.ApplyResources(this.descriptionLbl, "descriptionLbl");
            this.descriptionLbl.CellHeight = 42;
            this.descriptionLbl.CellWidth = 102;
            this.descriptionLbl.Column = 0;
            this.descriptionLbl.ColumnsSpanned = 0;
            this.descriptionLbl.Name = "descriptionLbl";
            this.descriptionLbl.Row = 4;
            this.descriptionLbl.RowsSpanned = 0;
            this.descriptionLbl.YOffset = 0;
            // 
            // workspaceLineEndingsLbl
            // 
            resources.ApplyResources(this.workspaceLineEndingsLbl, "workspaceLineEndingsLbl");
            this.workspaceLineEndingsLbl.CellHeight = 13;
            this.workspaceLineEndingsLbl.CellWidth = 185;
            this.workspaceLineEndingsLbl.Column = 3;
            this.workspaceLineEndingsLbl.ColumnsSpanned = 0;
            this.workspaceLineEndingsLbl.Name = "workspaceLineEndingsLbl";
            this.workspaceLineEndingsLbl.Row = 2;
            this.workspaceLineEndingsLbl.RowsSpanned = 0;
            this.workspaceLineEndingsLbl.YOffset = 0;
            // 
            // lineEndingsLbl
            // 
            resources.ApplyResources(this.lineEndingsLbl, "lineEndingsLbl");
            this.lineEndingsLbl.CellHeight = 13;
            this.lineEndingsLbl.CellWidth = 84;
            this.lineEndingsLbl.Column = 2;
            this.lineEndingsLbl.ColumnsSpanned = 0;
            this.lineEndingsLbl.Name = "lineEndingsLbl";
            this.lineEndingsLbl.Row = 2;
            this.lineEndingsLbl.RowsSpanned = 0;
            this.lineEndingsLbl.YOffset = 0;
            // 
            // submitOptionsLbl
            // 
            resources.ApplyResources(this.submitOptionsLbl, "submitOptionsLbl");
            this.submitOptionsLbl.CellHeight = 13;
            this.submitOptionsLbl.CellWidth = 84;
            this.submitOptionsLbl.Column = 2;
            this.submitOptionsLbl.ColumnsSpanned = 0;
            this.submitOptionsLbl.Name = "submitOptionsLbl";
            this.submitOptionsLbl.Row = 1;
            this.submitOptionsLbl.RowsSpanned = 0;
            this.submitOptionsLbl.YOffset = 0;
            // 
            // workspaceOwnerNameLbl
            // 
            resources.ApplyResources(this.workspaceOwnerNameLbl, "workspaceOwnerNameLbl");
            this.workspaceOwnerNameLbl.AutoEllipsis = true;
            this.workspaceOwnerNameLbl.CellHeight = 13;
            this.workspaceOwnerNameLbl.CellWidth = 185;
            this.workspaceOwnerNameLbl.Column = 1;
            this.workspaceOwnerNameLbl.ColumnsSpanned = 0;
            this.workspaceOwnerNameLbl.Name = "workspaceOwnerNameLbl";
            this.workspaceOwnerNameLbl.Row = 3;
            this.workspaceOwnerNameLbl.RowsSpanned = 0;
            this.workspaceOwnerNameLbl.YOffset = 0;
            // 
            // workspaceUpdatedLbl
            // 
            resources.ApplyResources(this.workspaceUpdatedLbl, "workspaceUpdatedLbl");
            this.workspaceUpdatedLbl.AutoEllipsis = true;
            this.workspaceUpdatedLbl.CellHeight = 13;
            this.workspaceUpdatedLbl.CellWidth = 185;
            this.workspaceUpdatedLbl.Column = 1;
            this.workspaceUpdatedLbl.ColumnsSpanned = 0;
            this.workspaceUpdatedLbl.Name = "workspaceUpdatedLbl";
            this.workspaceUpdatedLbl.Row = 1;
            this.workspaceUpdatedLbl.RowsSpanned = 0;
            this.workspaceUpdatedLbl.YOffset = 0;
            // 
            // lastAccessedLbl
            // 
            resources.ApplyResources(this.lastAccessedLbl, "lastAccessedLbl");
            this.lastAccessedLbl.CellHeight = 13;
            this.lastAccessedLbl.CellWidth = 102;
            this.lastAccessedLbl.Column = 0;
            this.lastAccessedLbl.ColumnsSpanned = 0;
            this.lastAccessedLbl.Name = "lastAccessedLbl";
            this.lastAccessedLbl.Row = 2;
            this.lastAccessedLbl.RowsSpanned = 0;
            this.lastAccessedLbl.YOffset = 0;
            // 
            // dateModifiedLbl
            // 
            resources.ApplyResources(this.dateModifiedLbl, "dateModifiedLbl");
            this.dateModifiedLbl.CellHeight = 13;
            this.dateModifiedLbl.CellWidth = 102;
            this.dateModifiedLbl.Column = 0;
            this.dateModifiedLbl.ColumnsSpanned = 0;
            this.dateModifiedLbl.Name = "dateModifiedLbl";
            this.dateModifiedLbl.Row = 1;
            this.dateModifiedLbl.RowsSpanned = 0;
            this.dateModifiedLbl.YOffset = 0;
            // 
            // workspaceLbl
            // 
            resources.ApplyResources(this.workspaceLbl, "workspaceLbl");
            this.workspaceLbl.CellHeight = 13;
            this.workspaceLbl.CellWidth = 102;
            this.workspaceLbl.Column = 0;
            this.workspaceLbl.ColumnsSpanned = 0;
            this.workspaceLbl.Name = "workspaceLbl";
            this.workspaceLbl.Row = 0;
            this.workspaceLbl.RowsSpanned = 0;
            this.workspaceLbl.YOffset = 0;
            // 
            // gridLayoutPanel1
            // 
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = false;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // WorkspaceToolWindowControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.gridLayoutPanel1);
            this.Name = "WorkspaceToolWindowControl";
            this.Load += new System.EventHandler(this.WorkspacesWindowControl_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gridLayoutPanel2.ResumeLayout(false);
            this.gridLayoutPanel2.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private object SyncRoot = new object();

        private void clearDetails()
        {
            //lock (SyncRoot)
            //{
            try
            {
                this.workspaceNameLbl.Text = string.Empty;
                this.workspaceUpdatedLbl.Text = string.Empty;
                this.workspaceLastAccessedLbl.Text = string.Empty;
                this.workspaceOwnerNameLbl.Text = string.Empty;
                this.descriptionTB.Text = string.Empty;
                this.workspaceRootLbl.Text = string.Empty;
                this.altRootsTB.Text = string.Empty;
                this.allwriteChk.Checked = false;
                this.clobberChk.Checked = false;
                this.compressChk.Checked = false;
                this.lockedChk.Checked = false;
                this.modtimeChk.Checked = false;
                this.rmdirChk.Checked = false;
                this.workspaceStreamRootLbl.Text = string.Empty;
                this.workspaceStreamAtChangeLbl.Text = string.Empty;
                this.workspaceSeverIDLbl.Text = string.Empty;
                this.viewTB.Text = string.Empty;
                this.workspaceHostNameLbl.Text = string.Empty;
                this.workspaceSubmitOptionsLbl.Text = string.Empty;
                this.workspaceLineEndingsLbl.Text = string.Empty;
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            //}
        }

        private delegate void workspacesTreeListViewDelegate();

        private delegate void WorkspacesTreeListViewItemDelegate(TreeListViewItem item);
        private delegate ListViewItem WorkspacesTreeListViewItemDelegate2(TreeListViewItem item);
        private delegate void setStringPropertyDelegate(string str);

        private void setWorkspaceMatchesLblText(string matches)
        {
            this.workspaceMatchesLbl.Text = matches;
        }

        private void AsyncPopulateListView()
        {
            bool threadAborted = false;
            if (workspacesTreeListView.InvokeRequired)
            {
                this.filterBtn.Invoke(new setFilterBtnDelegate(setFilterBtnBool), false);
            }
            else
            {
                filterBtn.Enabled = false;
            }
            try
            {
                lock (SyncRoot)
                {

                    try
                    {
                        if (workspacesTreeListView.InvokeRequired)
                        {
                            workspacesTreeListView.Invoke(new workspacesTreeListViewDelegate(this.workspacesTreeListView.Items.Clear));
                            workspacesTreeListView.Invoke(new workspacesTreeListViewDelegate(this.workspacesTreeListView.Nodes.Clear));
                        }
                        else
                        {
                            workspacesTreeListView.Items.Clear();
                            workspacesTreeListView.Nodes.Clear();
                        }

                        if ((Scm == null) || (Scm.Connection.Disconnected))
                        {
                            return;
                        }

                        Scm.Connection.Repository.Connection.getP4Server().ProgramName = "P4VS";
                        Scm.Connection.Repository.Connection.getP4Server().ProgramVersion = Versions.product();

                        string stream = this.stream;
                        string nameFilter = string.Empty;
                        if ((NameFilterText != null) && (NameFilterText != string.Empty))
                        {
                            nameFilter = "*" + NameFilterText + "*";
                        }
                        P4.ClientsCmdFlags flags = P4.ClientsCmdFlags.None;
                        if (nameCaseFilterChk.Checked == false)
                        {
                            flags = P4.ClientsCmdFlags.IgnoreCase;
                        }

                        int max = -1;

                        IList<P4.Client> workspaces = new List<P4.Client>();
                        if (streamsWorkspaces == null || streamsWorkspaces.Count < 1)
                        {
                            workspaces = Scm.getClients(flags, OwnerCBText, nameFilter, max, stream);
                        }
                        else
                        {
                            workspaces = streamsWorkspaces;
                        }

                        TreeListViewItem it = new TreeListViewItem();
                        if ((workspaces == null) || (workspaces.Count <= 0))
                        {
                            it = new TreeListViewItem(null, Resources.JobsToolWindowControl_NoItemsAvailable, true);

                            if (workspacesTreeListView.InvokeRequired)
                            {
                                workspacesTreeListView.Invoke(new WorkspacesTreeListViewItemDelegate(this.workspacesTreeListView.Nodes.Add), it);
                                workspacesTreeListView.Invoke(
                                    new WorkspacesTreeListViewItemDelegate(this.workspacesTreeListView.Nodes.Add), it);
                                this.workspaceMatchesLbl.Invoke(
                                    new setStringPropertyDelegate(setWorkspaceMatchesLblText), Resources.JobsToolWindowControl_NoMatches);
                            }
                            else
                            {
                                this.workspacesTreeListView.Items.Add(it);
                                this.workspaceMatchesLbl.Text = Resources.JobsToolWindowControl_NoMatches;
                            }

                            return;
                        }

                        threadMonitorControl1.Value = 0;
                        threadMonitorControl1.Show(workspaces.Count);

                        int cnt = 0;

                        for (int idx = 0; idx < workspaces.Count; idx++)
                        {
                            P4.Client client = workspaces[idx];
                            if ((workspaceHostFilterChk.Checked == true) &&
                                (client.Host != string.Empty) &&
                                (string.Equals(client.Host.ToString(),
                                               Dns.GetHostName(),
                                               StringComparison.CurrentCultureIgnoreCase)) == false)
                            {
                                continue;
                            }

                            // use these to build new lvi
                            string workspace = string.Empty;
                            string dateModified = string.Empty;
                            string lastAccessed = string.Empty;
                            string owner = string.Empty;
                            string host = string.Empty;
                            string root = string.Empty;
                            string streamRoot = string.Empty;
                            string streamAtChange = string.Empty;
                            string serverID = string.Empty;
                            string description = string.Empty;
                            int image = 0;

                            workspace = " " + client.Name;

                            DateTime access = client.Accessed;
                            DateTime update = client.Updated;
                            if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                            {
                                dateModified = update.ToString("yyyy/MM/dd HH:mm:ss");
                                lastAccessed = access.ToString("yyyy/MM/dd HH:mm:ss");
                            }
                            else
                            {
                                dateModified = update.ToString();
                                lastAccessed = access.ToString();
                            }

                            owner = client.OwnerName;
                            host = client.Host;
                            root = client.Root;
                            if (client.Stream == null)
                            {
                                image = 0;
                            }
                            else
                            {
                                if (client.Stream.Contains("@"))
                                {
                                    streamRoot = client.Stream.Substring(0, client.Stream.IndexOf("@"));
                                    int idx2 = client.Stream.IndexOf("@") + 1;
                                    streamAtChange = client.Stream.Substring(idx2);
                                }
                                else
                                {
                                    streamRoot = client.Stream;
                                }

                                image = 1;
                            }

                            if (client.StreamAtChange != null)
                            {
                                streamAtChange = client.StreamAtChange;
                            }

                            if (client.ServerID != null)
                            {
                                serverID = client.ServerID;
                            }
                            description = client.Description;
                            string[] fields = {
                                                  workspace, dateModified,
                                                  lastAccessed, owner, host, root, streamRoot,
                                                  streamAtChange, serverID, description
                                              };

                            it = new TreeListViewItem(null, fields, image);
                            it.Tag = client;

                            if (workspacesTreeListView.InvokeRequired)
                            {
                                workspacesTreeListView.Invoke(
                                    new WorkspacesTreeListViewItemDelegate(this.workspacesTreeListView.Nodes.Add), it);
                                workspacesTreeListView.Invoke(
                                    new WorkspacesTreeListViewItemDelegate2(this.workspacesTreeListView.Items.Add), it);
                            }
                            else
                            {
                                this.workspacesTreeListView.Items.Add(it);
                            }

                            threadMonitorControl1.Value = ++cnt;

                            if (threadMonitorControl1.CancelPressed)
                            {
                                break;
                            }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        threadAborted = true;
                        Thread.ResetAbort();
                    }
                    finally
                    {
                    }
                    try
                    {
                        if (!threadAborted || threadMonitorControl1.CancelPressed)
                        {
                            //user canceled, not aborted by new request
                            string itemCountStr = Resources.JobsToolWindowControl_NoMatches;
                            if (workspacesTreeListView.Items.Count == 1)
                            {
                                itemCountStr = Resources.JobsToolWindowControl_1Match;
                            }
                            if (workspacesTreeListView.Items.Count > 1)
                            {
                                itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches, workspacesTreeListView.Items.Count);
                            }

                            if (this.workspaceMatchesLbl.InvokeRequired)
                            {
                                this.workspaceMatchesLbl.Invoke(
                                    new setStringPropertyDelegate(setWorkspaceMatchesLblText), itemCountStr);
                            }
                            else
                            {
                                this.workspaceMatchesLbl.Text = itemCountStr;
                            }
                        }
                        FillInListProc = null;
                        threadMonitorControl1.Hide();
                    }
                    catch
                    {
                    }
                }
            }
            finally
            {
                if (workspacesTreeListView.InvokeRequired)
                {
                    this.filterBtn.Invoke(new setFilterBtnDelegate(setFilterBtnBool), true);
                }
                else
                {
                    filterBtn.Enabled = ((Scm != null) && (Scm.Connected));
                }
            }
        }

        private Thread FillInListProc = null;

        private P4.Client clientInfo = null;

        private void workspacesTreeListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkConnection();
            getRevisionToolStripMenuItem.Enabled = true;
            getRevisionToolStripMenuItem.Visible = true;
            toolStripSeparator1.Visible = true;
            newWorkspaceToolStripMenuItem.Enabled = true;
            newWorkspaceToolStripMenuItem.Visible = true;
            createWorkspaceFromToolStripMenuItem.Enabled = true;
            createWorkspaceFromToolStripMenuItem.Visible = true;
            editWorkspaceToolStripMenuItem.Enabled = true;
            editWorkspaceToolStripMenuItem.Visible = true;
            deleteWorkspaceToolStripMenuItem.Enabled = true;
            deleteWorkspaceToolStripMenuItem.Visible = true;
            toolStripSeparator2.Visible = true;
            refreshWorkspaceListToolStripMenuItem.Enabled = true;
            refreshWorkspaceListToolStripMenuItem.Visible = true;
            refreshWorkspaceToolStripMenuItem.Enabled = true;
            refreshWorkspaceToolStripMenuItem.Visible = true;


            if (workspacesTreeListView.SelectedItems.Count <= 0 ||
                workspacesTreeListView.SelectedItems[0].Text.Contains(Resources.JobsToolWindowControl_NoItemsAvailable))
            {
                clearDetails();
                getRevisionToolStripMenuItem.Enabled = false;
                getRevisionToolStripMenuItem.Visible = false;
                toolStripSeparator1.Visible = false;
                newWorkspaceToolStripMenuItem.Enabled = true;
                newWorkspaceToolStripMenuItem.Visible = true;
                createWorkspaceFromToolStripMenuItem.Enabled = false;
                createWorkspaceFromToolStripMenuItem.Visible = false;
                editWorkspaceToolStripMenuItem.Enabled = false;
                editWorkspaceToolStripMenuItem.Visible = false;
                deleteWorkspaceToolStripMenuItem.Enabled = false;
                deleteWorkspaceToolStripMenuItem.Visible = false;
                toolStripSeparator2.Visible = true;
                refreshWorkspaceListToolStripMenuItem.Enabled = true;
                refreshWorkspaceListToolStripMenuItem.Visible = true;
                refreshWorkspaceToolStripMenuItem.Enabled = false;
                refreshWorkspaceToolStripMenuItem.Visible = false;
                return;
            }
            clearDetails();
            P4.Client client = (P4.Client)workspacesTreeListView.SelectedItems[0].Tag;

            SelectedWorkspace = client;
            if (client.Stream == null)
            {
                createWorkspaceFromToolStripMenuItem.Enabled = true;
                createWorkspaceFromToolStripMenuItem.Visible = true;
                createWorkspaceFromToolStripMenuItem.Text = String.Format(
                    Resources.WorkspacesWindowControl_CreateWorkspaceFromMenuItem_Text, client.Name);
            }
            else
            {
                createWorkspaceFromToolStripMenuItem.Enabled = false;
                createWorkspaceFromToolStripMenuItem.Visible = false;
            }
            editWorkspaceToolStripMenuItem.Text = String.Format(
                    Resources.WorkspacesWindowControl_EditWorkspaceMenuItem_Text, client.Name);
            deleteWorkspaceToolStripMenuItem.Text = String.Format(
                    Resources.WorkspacesWindowControl_DeleteWorkspaceMenuItem_Text, client.Name);
            refreshWorkspaceToolStripMenuItem.Text = String.Format(
                    Resources.WorkspacesWindowControl_RefreshWorkspaceMenuItem_Text, client.Name);

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

            clientInfo = Scm.getClient(client.Name, null);

            try
            {
                workspaceNameLbl.Text = clientInfo.Name;

                DateTime access = client.Accessed;
                DateTime update = client.Updated;
                if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                {
                    workspaceUpdatedLbl.Text = update.ToString("yyyy/MM/dd HH:mm:ss");
                    workspaceLastAccessedLbl.Text = access.ToString("yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                    workspaceUpdatedLbl.Text = clientInfo.Updated.ToString();
                    workspaceLastAccessedLbl.Text = clientInfo.Accessed.ToString();
                }

                workspaceOwnerNameLbl.Text = clientInfo.OwnerName;
                if (clientInfo.Description != null)
                {
                    descriptionTB.Text = clientInfo.Description;
                }
                workspaceRootLbl.Text = clientInfo.Root;
                if (clientInfo.AltRoots != null)
                {
                    foreach (string root in clientInfo.AltRoots)
                    {
                        altRootsTB.AppendText(root + "\r\n");
                    }
                }
                if (clientInfo.Host != null)
                {
                    workspaceHostNameLbl.Text = clientInfo.Host;
                }
                workspaceRootLbl.Text = clientInfo.Root;
                workspaceSubmitOptionsLbl.Text = clientInfo.SubmitOptions;
                workspaceLineEndingsLbl.Text = clientInfo.LineEnd.ToString();

                workspaceStreamRootLbl.Text = clientInfo.Stream;
                workspaceStreamAtChangeLbl.Text = clientInfo.StreamAtChange;
                workspaceSeverIDLbl.Text = clientInfo.ServerID;

                if (clientInfo.ViewMap != null)
                {
                    string type = string.Empty;
                    foreach (P4.MapEntry entry in clientInfo.ViewMap)
                    {
                        if (entry.Type == P4.MapType.Exclude)
                        {
                            type = "-";
                        }
                        if (entry.Type == P4.MapType.Overlay)
                        {
                            type = "+";
                        }
                        viewTB.AppendText(type + entry.Left.Path.ToString() + " " + entry.Right.Path.ToString() + "\r\n");
                    }
                }
                allwriteChk.Checked = ((clientInfo.Options & P4.ClientOption.AllWrite) != 0);
                clobberChk.Checked = ((clientInfo.Options & P4.ClientOption.Clobber) != 0);
                compressChk.Checked = ((clientInfo.Options & P4.ClientOption.Compress) != 0);
                lockedChk.Checked = ((clientInfo.Options & P4.ClientOption.Locked) != 0);
                modtimeChk.Checked = ((clientInfo.Options & P4.ClientOption.ModTime) != 0);
                rmdirChk.Checked = ((clientInfo.Options & P4.ClientOption.RmDir) != 0);
            }
            catch
            {
            }
            ;
        }

        private void refreshWorkspaceItem(P4.Client workspace)
        {
            if (workspace != null)
            {

                workspace = Scm.getClient(workspace.Name, null);
                ListViewItem itemForRefresh = new ListViewItem();
                if (workspace != null)
                {
                    itemForRefresh = workspacesTreeListView.FindItemWithText(" " + workspace.Name, true, 0);

                    if (itemForRefresh != null)
                    {
                        itemForRefresh.Selected = true;
                        // use these to build new lvi
                        string client = string.Empty;
                        string dateModified = string.Empty;
                        string lastAccessed = string.Empty;
                        string owner = string.Empty;
                        string host = string.Empty;
                        string root = string.Empty;
                        string streamRoot = string.Empty;
                        string streamAtChange = string.Empty;
                        string serverID = string.Empty;
                        string description = string.Empty;

                        if (workspace.Name != null)
                        {
                            client = " " + workspace.Name;
                        }

                        if (workspace.Updated != null)
                        {
                            if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                            {
                                dateModified = workspace.Updated.ToString("yyyy/MM/dd HH:mm:ss");
                            }
                            else
                            {
                                dateModified = workspace.Updated.ToString();
                            }
                        }

                        if (workspace.Accessed != null)
                        {
                            if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                            {
                                lastAccessed = workspace.Accessed.ToString("yyyy/MM/dd HH:mm:ss");
                            }
                            else
                            {
                                lastAccessed = workspace.Accessed.ToString();
                            }
                        }

                        if (workspace.OwnerName != null)
                        {
                            owner = workspace.OwnerName;
                        }

                        if (workspace.Host != null)
                        {
                            host = workspace.Host;
                        }

                        if (workspace.Root != null)
                        {
                            root = workspace.Root;
                        }

                        if (workspace.Stream != null)
                        {
                            if (workspace.Stream.Contains("@"))
                            {
                                streamRoot = workspace.Stream.Substring(0, workspace.Stream.IndexOf("@"));
                                int idx2 = workspace.Stream.IndexOf("@") + 1;
                                streamAtChange = workspace.Stream.Substring(idx2);
                            }
                            else
                            {
                                streamRoot = workspace.Stream;
                            }
                        }

                        if (workspace.StreamAtChange != null)
                        {
                            streamAtChange = workspace.StreamAtChange;
                        }

                        if (workspace.ServerID != null)
                        {
                            serverID = workspace.ServerID;
                        }

                        if (workspace.Description != null)
                        {
                            description = workspace.Description;
                        }

                        string[] fields = {
                                              dateModified,
                                              lastAccessed, owner, host, root, streamRoot,
                                              streamAtChange, serverID, description
                                          };

                        itemForRefresh.SubItems.Clear();
                        itemForRefresh.Text = client;
                        itemForRefresh.SubItems.AddRange(fields);
                        itemForRefresh.Tag = workspace;
                        workspacesTreeListView_SelectedIndexChanged(null, null);
                    }
                }
            }


        }

        private void addWorkspaceItem(string WorkspaceId)
        {
            P4.Client client = Scm.getClient(WorkspaceId, null);
            if (client != null)
            {
                // use these to build new lvi
                string workspace = string.Empty;
                string dateModified = string.Empty;
                string lastAccessed = string.Empty;
                string owner = string.Empty;
                string host = string.Empty;
                string root = string.Empty;
                string streamRoot = string.Empty;
                string streamAtChange = string.Empty;
                string serverID = string.Empty;
                string description = string.Empty;
                int image = 0;

                workspace = " " + client.Name;
                if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                {
                    dateModified = client.Updated.ToString("yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                    dateModified = client.Updated.ToString();
                }
                if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                {
                    lastAccessed = client.Accessed.ToString("yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                    lastAccessed = client.Accessed.ToString();
                }

                owner = client.OwnerName;
                host = client.Host;
                root = client.Root;
                if (client.Stream == null)
                {
                    image = 0;
                }
                else
                {
                    if (client.Stream.Contains("@"))
                    {
                        streamRoot = client.Stream.Substring(0, client.Stream.IndexOf("@"));
                        int idx2 = client.Stream.IndexOf("@") + 1;
                        streamAtChange = client.Stream.Substring(idx2);
                    }
                    else
                    {
                        streamRoot = client.Stream;
                    }

                    image = 1;
                }

                if (client.StreamAtChange != null)
                {
                    streamAtChange = client.StreamAtChange;
                }

                if (client.ServerID != null)
                {
                    serverID = client.ServerID;
                }
                description = client.Description;
                string[] fields = {
                                      workspace, dateModified,
                                      lastAccessed, owner, host, root, streamRoot,
                                      streamAtChange, serverID, description
                                  };

                TreeListViewItem it = new TreeListViewItem(null, fields, image);
                it.Tag = client;

                if (workspacesTreeListView.InvokeRequired)
                {
                    workspacesTreeListView.Invoke(new WorkspacesTreeListViewItemDelegate(this.workspacesTreeListView.Nodes.Add), it);
                }
                else
                {
                    this.workspacesTreeListView.Items.Add(it);
                }
            }
        }

        private void refreshWorkspacesList()
        {
            checkConnection();
            this.workspacesTreeListView.Items.Clear();
            clearDetails();

            if (FillInListProc != null)
            {
                if (FillInListProc.IsAlive)
                {
                    threadMonitorControl1.CancelPressed = true;
                    //FillInListProc.Abort();
                    FillInListProc.Join(1000);
                }
                threadMonitorControl1.Hide();
                FillInListProc = null;
            }
            //lock (SyncRoot)
            //{
            //    this.workspacesTreeListView.Items.Clear();
            //    clearDetails();
            //}
            if ((Scm == null) || (Scm.Connection.Disconnected))
            {
                workspaceMatchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
                return;
            }

            NameFilterText = nameContainsCB.Text;
            OwnerCBText = ownerCB.Text;
            threadMonitorControl1.Visible = false;

            FillInListProc = new Thread(new ThreadStart(AsyncPopulateListView));
            FillInListProc.IsBackground = true;

            FillInListProc.Start();

            SaveControlSettings();

            return;

        }

        private void WorkspacesWindowControl_Load(object sender, EventArgs e)
        {
            clearDetails();
            if (stream != null)
            {
                streamVarLbl.Text = stream;
                streamVarLbl.Visible = false;
                if (integAction == "merge")
                {
                    ownerCBLbl.Text = string.Format(Resources.WorkspacesWindowControl_SelectAWorkspaceForCopying, "Merging", stream);
                }
                else
                {
                    ownerCBLbl.Text = string.Format(Resources.WorkspacesWindowControl_SelectAWorkspaceForCopying, "Copying", stream);
                }
                //ownerCB.Text = Scm.User.ToString();
                ownerCB.Visible = false;
                nameContainsLbl.Visible = false;
                nameContainsCB.Visible = false;
                workspaceHostFilterChk.Checked = true;
                workspaceHostFilterChk.Visible = false;
                nameCaseFilterChk.Visible = false;
                workspaceMatchesLbl.Visible = false;
                filterBtn.Visible = false;
                contextMenuStrip1.Enabled = false;
                contextMenuStrip1.Visible = false;
                dividerGB.Enabled = false;
                dividerGB.Visible = false;
            }

            if ((hostOnly == true) && (sentOwner != null))
            {
                //ownerCB.Text = Scm.User.ToString();
                workspaceHostFilterChk.Checked = true;
                workspaceHostFilterChk.Enabled = true;
                contextMenuStrip1.Enabled = false;
                contextMenuStrip1.Visible = false;
            }

            if ((hostOnly == false) && (sentOwner != null))
            {
                //ownerCB.Text = Scm.User.ToString();
                contextMenuStrip1.Enabled = false;
                contextMenuStrip1.Visible = false;
            }

            this.Refresh();
            this.description.Width = -2;

            if (Scm != null)
            {
                if (ownerCB.Text == "" &&
                 ownerCB.mruValues[1] == null)
                {
                    ownerCB.Text = Scm.Connection.User;
                }

                //if (ownerCB.Text == ""&&
                //    !(ownerCB.mruValues.Contains("")))
                //{
                //    selectionChangedByLoad = true;
                //    ownerCB.Text = Scm.User;
                //}

                if (Scm.ServerVersion < Versions.V11_1)
                {
                    nameCaseFilterChk.Enabled = false;
                    nameCaseFilterChk.Checked = true;
                }
                else
                {
                    nameCaseFilterChk.Enabled = true;
                    nameCaseFilterChk.Checked = false;
                }
            }

            refreshWorkspacesList();
        }

        private void OwnerCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                filterBtn.PerformClick();
        }

        private void NameFilterCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                filterBtn.PerformClick();
        }

        //New Workspace... context menu click
        private void newWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newName = GetStringDlg.Show(Resources.WorkspacesWindowControl_NewWorkspace,
                Resources.WorkspacesWindowControl_EnterNameForNewWorkspace, null);
            if ((newName != null) && (newName != string.Empty))
            {
                if (newName.Contains(" "))
                {
                    MessageBox.Show(Resources.NewUserDlg_NameContainsSpacesWarning);
                }

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

                IList<P4.Client> checkExisting = Scm.getClients(P4.ClientsCmdFlags.None, null, newName, 1, null);
                if (checkExisting == null)
                {
                    P4.Client clientInfo = Scm.getClient(newName, null);

                    if (clientInfo != null)
                    {
                        // adjust root here based on users dir
                        string root = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        int idx = root.LastIndexOf(@"\");
                        root = root.Remove(idx + 1);
                        root += newName;
                        clientInfo.Root = root;
                        P4.Client workspace = DlgEditWorkspace.EditWorkspace(Scm, clientInfo.Name);

                        if (workspace != null)
                        {
                            refreshWorkspaceItem(workspace);
                        }
                    }
                    else
                    {
                        newWorkspaceToolStripMenuItem_Click(null, null);
                    }

                }
                else
                {
                    MessageBox.Show(
                        string.Format(Resources.WorkspacesWindowControl_WorkspaceExistsWarning, newName),
                        Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    newWorkspaceToolStripMenuItem_Click(null, null);
                }

            }
            else
            {
                if (newName == string.Empty)
                {
                    MessageBox.Show(Resources.WorkspacesWindowControl_EmptyNameWarning,
                        Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    newWorkspaceToolStripMenuItem_Click(null, null);
                }
            }
            refreshWorkspacesList();
        }

        //Create Workspace from ''... context menu click
        private void createWorkspaceFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            P4.Client client = (P4.Client)workspacesTreeListView.SelectedItems[0].Tag;

            string newName = GetStringDlg.Show(Resources.WorkspacesWindowControl_NewWorkspace,
                Resources.WorkspacesWindowControl_EnterNameForNewWorkspace, null);
            if ((newName != null) && (newName != string.Empty))
            {
                if (newName.Contains(" "))
                {
                    MessageBox.Show(Resources.NewUserDlg_NameContainsSpacesWarning);
                }

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

                IList<P4.Client> checkExisting = Scm.getClients(P4.ClientsCmdFlags.None, null, newName, 10, null);
                if (checkExisting == null)
                {
                    P4.Client workspace = new P4.Client();
                    P4.Options options = new P4.Options(P4.ClientCmdFlags.None, client.Name, null, -1);
                    options["-t"] = client.Name;

                    workspace = DlgEditWorkspace.EditWorkspace(Scm, newName, options);

                    if (workspace != null)
                    {
                        P4.Client updateClient = Scm.createClient(workspace, null);
                        refreshWorkspaceItem(workspace);
                    }
                    else
                    {
                        createWorkspaceFromToolStripMenuItem_Click(null, null);
                    }

                }
                else
                {
                    // do a foreach check here to confirm that the name == newName and 
                    // does not just contain part of it.
                    MessageBox.Show(String.Format(Resources.WorkspacesWindowControl_WorkspaceExistsWarning, newName),
                        Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    createWorkspaceFromToolStripMenuItem_Click(null, null);
                }

            }
            else
            {
                if (newName == string.Empty)
                {
                    MessageBox.Show(Resources.WorkspacesWindowControl_EmptyNameWarning,
                        Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    createWorkspaceFromToolStripMenuItem_Click(null, null);
                }
            }
            refreshWorkspacesList();
        }

        //Edit Workspace ''... context menu click
        private void editWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            P4.Client client = (P4.Client)workspacesTreeListView.SelectedItems[0].Tag;

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
            DlgEditWorkspace.EditWorkspace(Scm, client.Name);
            refreshWorkspaceItem(client);
        }

        //Delete Workspace ''... context menu click
        private void deleteWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            P4.Client client = (P4.Client)workspacesTreeListView.SelectedItems[0].Tag;

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


            string prompt = string.Format(Resources.WorkspacesWindowControl_DeleteWorkspaceWarning, client.Name);
            string caption = Resources.WorkspacesWindowControl_DeleteWorkspaceCaption;

            if (DialogResult.No != MessageBox.Show(prompt, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                Scm.deleteClient(client, null);
                workspacesTreeListView.SelectedItems.Clear();
                workspacesTreeListView.SelectedIndices.Clear();
                workspacesTreeListView_SelectedIndexChanged(null, null);

                refreshWorkspacesList();
            }


        }

        //Refresh Workspace List context menu click
        private void refreshWorkspaceListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refreshWorkspacesList();
        }

        //Refresh Workspace ''... context menu click
        private void refreshWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            workspacesTreeListView_SelectedIndexChanged(null, null);
        }

        //Get Revision... context menu click
        private void getRevisionToolStripMenuItem_Click(object sender, EventArgs e)
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

            P4.Client client = (P4.Client)workspacesTreeListView.SelectedItems[0].Tag;
            string value = client.Name;

            GetRevisionDlg.Show(null, "workspace", value, Scm, true);
        }

        private void filterBtn_Click(object sender, EventArgs e)
        {
            refreshWorkspacesList();
        }

        private void checkConnection()
        {
            if (Scm == null)
            {
                Scm = P4VsProvider.CurrentScm;
            }

            if ((Scm == null) || (Scm.Connection.Disconnected))
            {
                // still null
                filterBtn.Enabled = false;
                clearDetails();
                workspacesTreeListView.Items.Clear();
                contextMenuStrip1.Enabled = false;
                setWorkspaceMatchesLblText(Resources.JobsToolWindowControl_NoConnection);
                return;
            }
            filterBtn.Enabled = true;
            contextMenuStrip1.Enabled = true;
        }

        private void filterBtn_EnabledChanged(object sender, EventArgs e)
        {
            if ((filterBtn.Enabled == false) && ((Scm == null) ||
                                                 (Scm.Connection.Disconnected)))
            {
                workspaceMatchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
            }
            else if ((filterBtn.Enabled == false) && (Scm != null) &&
                     (Scm.Connected))
            {
                workspaceMatchesLbl.Text = "";
            }
        }

        private void workspacesTreeListView_Click(object sender, EventArgs e)
        {
            checkConnection();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (sentOwner != null)
            {
                e.Cancel = true;
                return;
            }
            if (workspacesTreeListView.SelectedItems.Count == 0 ||
                workspacesTreeListView.SelectedItems[0].Text.Contains(Resources.JobsToolWindowControl_NoItemsAvailable))
            {
                e.Cancel = false;
                workspacesTreeListView_SelectedIndexChanged(null, null);
            }
            if ((SelectedWorkspace == null) || (SelectedWorkspace.OwnerName != Scm.Connection.User))
            {
                deleteWorkspaceToolStripMenuItem.Visible = false;
                deleteWorkspaceToolStripMenuItem.Enabled = false;
            }
        }

        private void workspacesTreeListView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            if (e.NewDisplayIndex == 0 | e.OldDisplayIndex == 0)
            {
                e.Cancel = true;
            }
        }

        private void workspacesTreeListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // set the header text to what it already is to force a
                // redraw of the previously selected column header.
                workspacesTreeListView.Columns[lvwColumnSorter.SortColumn].Text =
                    workspacesTreeListView.Columns[lvwColumnSorter.SortColumn].Text;

                // Set the column number that is to be sorted; default to ascending.

                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.workspacesTreeListView.Sort();
        }

        public event MouseEventHandler TreeListViewDoubleClicked;


        private void workspacesTreeListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (TreeListViewDoubleClicked != null)
            {
                TreeListViewDoubleClicked(sender, e);
            }
        }

        private void nameContainsCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectionChangedByLoad == false)
            {
                filterBtn.PerformClick();
            }
            selectionChangedByLoad = false;
        }

        private void ownerCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectionChangedByLoad == false)
            {
                filterBtn.PerformClick();
            }
            selectionChangedByLoad = false;
        }

        bool in_allwriteChk_CheckedChanged = false;

        // can't make check boxes disabled, or they won't obey theming, so lock their values to the correct ones
        private void allwriteChk_CheckedChanged(object sender, EventArgs e)
        {
            if (in_allwriteChk_CheckedChanged)
            {
                return;
            }
            try
            {
                in_allwriteChk_CheckedChanged = true;

                if ((clientInfo == null) && (allwriteChk.Checked != false))
                {
                    allwriteChk.Checked = false;
                }
                else if ((clientInfo != null) && (allwriteChk.Checked != ((clientInfo.Options & P4.ClientOption.AllWrite) != 0)))
                {
                    allwriteChk.Checked = ((clientInfo.Options & P4.ClientOption.AllWrite) != 0);
                }
            }
            finally
            {
                in_allwriteChk_CheckedChanged = false;
            }
        }

        bool in_clobberChk_CheckedChanged = false;

        private void clobberChk_CheckedChanged(object sender, EventArgs e)
        {
            if (in_clobberChk_CheckedChanged)
            {
                return;
            }
            try
            {
                in_clobberChk_CheckedChanged = true;

                if ((clientInfo == null) && (clobberChk.Checked != false))
                {
                    clobberChk.Checked = false;
                }
                else if ((clientInfo != null) && (clobberChk.Checked != ((clientInfo.Options & P4.ClientOption.Clobber) != 0)))
                {
                    clobberChk.Checked = ((clientInfo.Options & P4.ClientOption.Clobber) != 0);
                }
            }
            finally
            {
                in_clobberChk_CheckedChanged = false;
            }
        }

        bool in_compressChk_CheckedChanged = false;

        private void compressChk_CheckedChanged(object sender, EventArgs e)
        {
            if (in_compressChk_CheckedChanged)
            {
                return;
            }
            try
            {
                in_compressChk_CheckedChanged = true;

                if ((clientInfo == null) && (compressChk.Checked != false))
                {
                    compressChk.Checked = false;
                }
                else if ((clientInfo != null) && (compressChk.Checked != ((clientInfo.Options & P4.ClientOption.Compress) != 0)))
                {
                    compressChk.Checked = ((clientInfo.Options & P4.ClientOption.Compress) != 0);
                }
            }
            finally
            {
                in_compressChk_CheckedChanged = false;
            }
        }

        bool in_lockedChk_CheckedChanged = false;

        private void lockedChk_CheckedChanged(object sender, EventArgs e)
        {
            if (in_lockedChk_CheckedChanged)
            {
                return;
            }
            try
            {
                in_lockedChk_CheckedChanged = true;

                if ((clientInfo == null) && (lockedChk.Checked != false))
                {
                    lockedChk.Checked = false;
                }
                else if ((clientInfo != null) && (lockedChk.Checked != ((clientInfo.Options & P4.ClientOption.Locked) != 0)))
                {
                    lockedChk.Checked = ((clientInfo.Options & P4.ClientOption.Locked) != 0);
                }
            }
            finally
            {
                in_lockedChk_CheckedChanged = false;
            }
        }

        bool in_modtimeChk_CheckedChanged = false;

        private void modtimeChk_CheckedChanged(object sender, EventArgs e)
        {
            if (in_modtimeChk_CheckedChanged)
            {
                return;
            }
            try
            {
                in_modtimeChk_CheckedChanged = true;

                if ((clientInfo == null) && (modtimeChk.Checked != false))
                {
                    modtimeChk.Checked = false;
                }
                else if ((clientInfo != null) && (modtimeChk.Checked != ((clientInfo.Options & P4.ClientOption.ModTime) != 0)))
                {
                    modtimeChk.Checked = ((clientInfo.Options & P4.ClientOption.ModTime) != 0);
                }
            }
            finally
            {
                in_modtimeChk_CheckedChanged = false;
            }
        }

        bool in_rmdirChk_CheckedChanged = false;

        private void rmdirChk_CheckedChanged(object sender, EventArgs e)
        {
            if (in_rmdirChk_CheckedChanged)
            {
                return;
            }
            try
            {
                in_rmdirChk_CheckedChanged = true;

                if ((clientInfo == null) && (rmdirChk.Checked != false))
                {
                    rmdirChk.Checked = false;
                }
                else if ((clientInfo != null) && (rmdirChk.Checked != ((clientInfo.Options & P4.ClientOption.RmDir) != 0)))
                {
                    rmdirChk.Checked = ((clientInfo.Options & P4.ClientOption.RmDir) != 0);
                }

            }
            finally
            {
                in_rmdirChk_CheckedChanged = false;
            }
        }
    }
}