
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
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;
using System.Net;
using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.Linq;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
    /// <summary>
    /// Summary description for P4ToolWindowControl.
    /// </summary>
    public class StreamsToolWindowControl : P4ToolWindowControlBase
    {
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private SplitContainer splitContainer1;
        private I18nControls.GridTreeListView streamsTreeListView;
        private ContextMenuStrip streamContextMenuStrip;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem mergeToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem refreshStreamsListToolStripMenuItem;
        private ToolStripMenuItem refreshStreamToolStripMenuItem;
        private UI.ThreadMonitorControl threadMonitorControl1;
        private I18nControls.GridGroupBox dividerGB;
        public I18nControls.GridButton filterBtn;
        private I18nControls.GridLabel matchesLbl;
        private I18nControls.GridLabel nameFilterLbl;
        private I18nControls.GridLabel ownerFilterLbl;
        private I18nControls.GridLabel depotFilterLbl;
        private ColumnHeader ClmHdr_Stream;
        private ColumnHeader ClmHdr_StreamRoot;
        private ColumnHeader ClmHdr_Parent;
        private ColumnHeader ClmHdr_Owner;
        private ColumnHeader ClmHdr_ParentRoot;
        private ColumnHeader ClmHdr_Type;
        private ListViewColumnSorter lvwColumnSorter;

        private IContainer components;
        private I18nControls.GridFilterComboBox nameFilterCB;
        private I18nControls.GridFilterComboBox ownerFilterCB;
        private I18nControls.GridComboBox depotFilterCB;

        private MRUList _recentStreamOwners = null;
        private MRUList _recentParentStreams = null;
        private MRUList _recentStreamNames = null;

        public bool fromBrowser { get; set; }

        private ImageList streamsImageList;
        private ColumnHeader ClmHdr_Description;
        private ColumnHeader ClmHdr_SubmitAccess;
        private ColumnHeader ClmHdr_Locked;
        private I18nControls.GridLabel typeFilterLbl;
        private I18nControls.GridLabel parentFilterLbl;
        private I18nControls.GridFilterComboBox parentFilterCB;
        private I18nControls.GridComboBox typeFilterCB;
        private ColumnHeader ClmHdr_PendingAction;
        private bool selectionChangedByLoad = false;
        private string lastDepotFilter = null;
        private I18nControls.GridLayoutSubpanel gridLayoutPanel2;
        private I18nControls.GridTextBox remappedTB;
        private I18nControls.GridTextBox pathsTB;
        private I18nControls.GridLabel optionsLbl;
        private I18nControls.GridPanel gridLayoutSubpanel1;
        private I18nControls.GridCheckBox fromParentChk;
        private I18nControls.GridCheckBox toParentChk;
        private I18nControls.GridCheckBox lockedChk;
        private I18nControls.GridCheckBox submittingChk;
        private I18nControls.GridLabel descriptionLbl;
        private I18nControls.GridTextBox descriptionTB;
        private I18nControls.GridLabel pathsLbl;
        private I18nControls.GridTextBox typeTB;
        private I18nControls.GridTextBox parentTB;
        private I18nControls.GridTextBox streamRootTB;
        private I18nControls.GridLabel typeLbl;
        private I18nControls.GridLabel streamRootLbl;
        private I18nControls.GridLabel parentLbl;
        private I18nControls.GridLabel ownerLbl;
        private I18nControls.GridTextBox ownerTB;
        private I18nControls.GridTextBox accessedTB;
        private I18nControls.GridTextBox modifiedTB;
        private I18nControls.GridLabel accessedLbl;
        private I18nControls.GridLabel dateLbl;
        private I18nControls.GridTextBox streamTB;
        private I18nControls.GridLabel streamLbl;
        private I18nControls.GridTextBox viewTB;
        private I18nControls.GridTextBox ignoredTB;
        private I18nControls.GridLabel viewLbl;
        private I18nControls.GridLabel ignoredLbl;
        private I18nControls.GridLabel remappedLbl;
        private string lastTypeFilter = null;
        public StreamsToolWindowControl()
        {
            PreferenceKey = "StreamsToolWindowControl";
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            base.Initialize();

            // if the mru lists have not been loaded, see if the old preference exists
            if (Preferences.LocalSettings != null)
            {
                if ((ownerFilterCB.mruLoaded == false) && (Preferences.LocalSettings.ContainsKey("RecentStreamOwners")))
                {
                    MRUList value = (MRUList)Preferences.LocalSettings["RecentStreamOwners"];
                    if (value != null)
                    {
                        ownerFilterCB.mruValues = value.Clone();
                        ownerFilterCB.mruLoaded = true;
                    }
                }
                if ((parentFilterCB.mruLoaded == false) && (Preferences.LocalSettings.ContainsKey("RecentParentStreams")))
                {
                    MRUList value = (MRUList)Preferences.LocalSettings["RecentParentStreams"];
                    if (value != null)
                    {
                        parentFilterCB.mruValues = value.Clone();
                        parentFilterCB.mruLoaded = true;
                    }
                }

                if ((nameFilterCB.mruLoaded == false) &&
                    (Preferences.LocalSettings.ContainsKey("RecentStreamNames")))
                {
                    MRUList value = (MRUList)Preferences.LocalSettings["RecentStreamNames"];
                    if (value != null)
                    {
                        nameFilterCB.mruValues = value.Clone();
                        nameFilterCB.mruLoaded = true;
                    }
                }
            }

            this.streamsImageList = new System.Windows.Forms.ImageList(this.components);

            // 
            // streamsImageList
            // 
            this.streamsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.streamsImageList.Images.Add("stream_development.png", Images.stream_development_icon);
            this.streamsImageList.Images.Add("stream_icon.png", Images.stream_icon);
            this.streamsImageList.Images.Add("stream_mainline.png", Images.stream_mainline_icon);
            this.streamsImageList.Images.Add("stream_merge_available.png", Images.stream_merge_available_icon);
            this.streamsImageList.Images.Add("stream_merge_available_reversed.png", Images.stream_merge_available_reversed_icon);
            this.streamsImageList.Images.Add("stream_merge_none.png", Images.stream_merge_none_icon);
            this.streamsImageList.Images.Add("stream_merge_none_reversed.png", Images.stream_merge_none_reversed_icon);
            this.streamsImageList.Images.Add("stream_promote_available.png", Images.stream_promote_available_icon);
            this.streamsImageList.Images.Add("stream_promote_available_reversed.png", Images.stream_promote_available_reversed_icon);
            this.streamsImageList.Images.Add("stream_promote_conflict.png", Images.stream_promote_conflict_icon);
            this.streamsImageList.Images.Add("stream_promote_conflict_reversed.png", Images.stream_promote_conflict_reversed_icon);
            this.streamsImageList.Images.Add("stream_promote_none.png", Images.stream_promote_none_icon);
            this.streamsImageList.Images.Add("stream_promote_none_reversed.png", Images.stream_promote_none_reversed_icon);
            this.streamsImageList.Images.Add("stream_release.png", Images.stream_release_icon);
            this.streamsImageList.Images.Add("stream_task.png", Images.stream_task_icon);
            this.streamsImageList.Images.Add("stream_unknown.png", Images.stream_unknown_icon);
            this.streamsImageList.Images.Add("stream_virtual.png", Images.stream_virtual_icon);

            this.streamsTreeListView.LargeImageList = this.streamsImageList;
            this.streamsTreeListView.SmallImageList = this.streamsImageList;


            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            this.streamsTreeListView.ListViewItemSorter = lvwColumnSorter;

            if (Scm != null)
            {
                ownerFilterCB.Text = Scm.Connection.User;
            }

            DepotFilterText = depotFilterCB.Text;
            OwnerFilterText = ownerFilterCB.Text;
            ParentFilterText = parentFilterCB.Text;
            NameFilterText = nameFilterCB.Text;
            TypeFilterText = typeFilterCB.Text;

            threadMonitorControl1.Visible = false;

            //newConection = new P4VsProvider.NewConnectionDelegate(OnNewConnection);
            P4VsProvider.NewConnection += newConection;

            checkConnection();
#if VS2012
            if (!DesignMode)
            {
                base.InitThemeManager();
            }
#endif
        }

        public StreamsToolWindowControl(P4ScmProvider scm)
            : base(scm)
        {
            Scm = scm;
            PreferenceKey = "StreamsToolWindowControl";
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            base.Initialize();

            this.streamsImageList = new System.Windows.Forms.ImageList(this.components);

            // 
            // streamsImageList
            // 
            this.streamsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.streamsImageList.Images.Add("stream_development.png", Images.stream_development_icon);
            this.streamsImageList.Images.Add("stream_icon.png", Images.stream_icon);
            this.streamsImageList.Images.Add("stream_mainline.png", Images.stream_mainline_icon);
            this.streamsImageList.Images.Add("stream_merge_available.png", Images.stream_merge_available_icon);
            this.streamsImageList.Images.Add("stream_merge_available_reversed.png", Images.stream_merge_available_reversed_icon);
            this.streamsImageList.Images.Add("stream_merge_none.png", Images.stream_merge_none_icon);
            this.streamsImageList.Images.Add("stream_merge_none_reversed.png", Images.stream_merge_none_reversed_icon);
            this.streamsImageList.Images.Add("stream_promote_available.png", Images.stream_promote_available_icon);
            this.streamsImageList.Images.Add("stream_promote_available_reversed.png", Images.stream_promote_available_reversed_icon);
            this.streamsImageList.Images.Add("stream_promote_conflict.png", Images.stream_promote_conflict_icon);
            this.streamsImageList.Images.Add("stream_promote_conflict_reversed.png", Images.stream_promote_conflict_reversed_icon);
            this.streamsImageList.Images.Add("stream_promote_none.png", Images.stream_promote_none_icon);
            this.streamsImageList.Images.Add("stream_promote_none_reversed.png", Images.stream_promote_none_reversed_icon);
            this.streamsImageList.Images.Add("stream_release.png", Images.stream_release_icon);
            this.streamsImageList.Images.Add("stream_task.png", Images.stream_task_icon);
            this.streamsImageList.Images.Add("stream_unknown.png", Images.stream_unknown_icon);
            this.streamsImageList.Images.Add("stream_virtual.png", Images.stream_virtual_icon);

            this.streamsTreeListView.LargeImageList = this.streamsImageList;
            this.streamsTreeListView.SmallImageList = this.streamsImageList;

            DepotFilterText = depotFilterCB.Text;
            OwnerFilterText = ownerFilterCB.Text;
            ParentFilterText = parentFilterCB.Text;
            NameFilterText = nameFilterCB.Text;
            TypeFilterText = typeFilterCB.Text;

            threadMonitorControl1.Visible = false;

            newConection = new P4VsProvider.NewConnectionDelegate(OnNewConnection);
            P4VsProvider.NewConnection += newConection;

            _recentStreamOwners = (MRUList)Preferences.LocalSettings["RecentStreamOwners"];
            if (_recentStreamOwners != null)
            {

                foreach (string owner in _recentStreamOwners)
                {
                    if (owner != null)
                    {
                        ownerFilterCB.Items.Add(owner);
                    }
                }
            }

            _recentParentStreams = (MRUList)Preferences.LocalSettings["RecentParentStreams"];
            if (_recentParentStreams != null)
            {

                foreach (string parent in _recentParentStreams)
                {
                    if (parent != null)
                    {
                        parentFilterCB.Items.Add(parent);
                    }
                }
            }

            _recentStreamNames = (MRUList)Preferences.LocalSettings["RecentStreamNames"];
            if (_recentStreamNames != null)
            {

                foreach (string name in _recentStreamNames)
                {
                    if (name != null)
                    {
                        nameFilterCB.Items.Add(name);
                    }
                }
            }

            checkConnection();
#if VS2012
            if (!DesignMode)
            {
                base.InitThemeManager();
            }
#endif
        }

        private delegate void setFilterBtnDelegate(bool filter);

        private void setFilterBtnBool(bool enabled)
        {
            this.filterBtn.Enabled = enabled;
        }

        P4VsProvider.NewConnectionDelegate newConection;

        public override void OnNewConnection(P4ScmProvider newScm)
        {
            Scm = newScm;

            // refilter;
            this.streamsTreeListView.Nodes.Clear();
            clearDetails();
            clearFilters();
            maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
            if (maxItems == 0)
            {
                maxItems = -1;
            }

            populateFilters();
            filterBtn_Click(null, null);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StreamsToolWindowControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.dividerGB = new Perforce.I18nControls.GridGroupBox();
            this.typeFilterCB = new Perforce.I18nControls.GridComboBox();
            this.filterBtn = new Perforce.I18nControls.GridButton();
            this.matchesLbl = new Perforce.I18nControls.GridLabel();
            this.nameFilterCB = new Perforce.I18nControls.GridFilterComboBox();
            this.typeFilterLbl = new Perforce.I18nControls.GridLabel();
            this.parentFilterCB = new Perforce.I18nControls.GridFilterComboBox();
            this.nameFilterLbl = new Perforce.I18nControls.GridLabel();
            this.ownerFilterCB = new Perforce.I18nControls.GridFilterComboBox();
            this.parentFilterLbl = new Perforce.I18nControls.GridLabel();
            this.depotFilterCB = new Perforce.I18nControls.GridComboBox();
            this.ownerFilterLbl = new Perforce.I18nControls.GridLabel();
            this.depotFilterLbl = new Perforce.I18nControls.GridLabel();
            this.streamsTreeListView = new Perforce.I18nControls.GridTreeListView();
            this.ClmHdr_Stream = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_PendingAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_StreamRoot = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_Parent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_Owner = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_ParentRoot = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_SubmitAccess = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_Locked = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.streamContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mergeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshStreamsListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshStreamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.threadMonitorControl1 = new Perforce.P4VS.UI.ThreadMonitorControl();
            this.gridLayoutPanel2 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.typeTB = new Perforce.I18nControls.GridTextBox();
            this.ownerLbl = new Perforce.I18nControls.GridLabel();
            this.parentTB = new Perforce.I18nControls.GridTextBox();
            this.remappedTB = new Perforce.I18nControls.GridTextBox();
            this.streamRootTB = new Perforce.I18nControls.GridTextBox();
            this.ownerTB = new Perforce.I18nControls.GridTextBox();
            this.typeLbl = new Perforce.I18nControls.GridLabel();
            this.pathsTB = new Perforce.I18nControls.GridTextBox();
            this.streamRootLbl = new Perforce.I18nControls.GridLabel();
            this.accessedTB = new Perforce.I18nControls.GridTextBox();
            this.parentLbl = new Perforce.I18nControls.GridLabel();
            this.optionsLbl = new Perforce.I18nControls.GridLabel();
            this.modifiedTB = new Perforce.I18nControls.GridTextBox();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridPanel();
            this.fromParentChk = new Perforce.I18nControls.GridCheckBox();
            this.toParentChk = new Perforce.I18nControls.GridCheckBox();
            this.lockedChk = new Perforce.I18nControls.GridCheckBox();
            this.submittingChk = new Perforce.I18nControls.GridCheckBox();
            this.accessedLbl = new Perforce.I18nControls.GridLabel();
            this.descriptionLbl = new Perforce.I18nControls.GridLabel();
            this.dateLbl = new Perforce.I18nControls.GridLabel();
            this.descriptionTB = new Perforce.I18nControls.GridTextBox();
            this.streamTB = new Perforce.I18nControls.GridTextBox();
            this.pathsLbl = new Perforce.I18nControls.GridLabel();
            this.streamLbl = new Perforce.I18nControls.GridLabel();
            this.viewTB = new Perforce.I18nControls.GridTextBox();
            this.ignoredTB = new Perforce.I18nControls.GridTextBox();
            this.viewLbl = new Perforce.I18nControls.GridLabel();
            this.ignoredLbl = new Perforce.I18nControls.GridLabel();
            this.remappedLbl = new Perforce.I18nControls.GridLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gridLayoutPanel1.SuspendLayout();
            this.streamContextMenuStrip.SuspendLayout();
            this.gridLayoutPanel2.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.gridLayoutPanel1);
            this.splitContainer1.Panel1.Controls.Add(this.threadMonitorControl1);
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainer1.Panel2.Controls.Add(this.gridLayoutPanel2);
            this.splitContainer1.TabStop = false;
            // 
            // gridLayoutPanel1
            // 
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.Controls.Add(this.dividerGB);
            this.gridLayoutPanel1.Controls.Add(this.typeFilterCB);
            this.gridLayoutPanel1.Controls.Add(this.filterBtn);
            this.gridLayoutPanel1.Controls.Add(this.matchesLbl);
            this.gridLayoutPanel1.Controls.Add(this.nameFilterCB);
            this.gridLayoutPanel1.Controls.Add(this.typeFilterLbl);
            this.gridLayoutPanel1.Controls.Add(this.parentFilterCB);
            this.gridLayoutPanel1.Controls.Add(this.nameFilterLbl);
            this.gridLayoutPanel1.Controls.Add(this.ownerFilterCB);
            this.gridLayoutPanel1.Controls.Add(this.parentFilterLbl);
            this.gridLayoutPanel1.Controls.Add(this.depotFilterCB);
            this.gridLayoutPanel1.Controls.Add(this.ownerFilterLbl);
            this.gridLayoutPanel1.Controls.Add(this.depotFilterLbl);
            this.gridLayoutPanel1.Controls.Add(this.streamsTreeListView);
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // dividerGB
            // 
            resources.ApplyResources(this.dividerGB, "dividerGB");
            this.dividerGB.CellHeight = 83;
            this.dividerGB.CellWidth = 10;
            this.dividerGB.Column = 4;
            this.dividerGB.ColumnsSpanned = 0;
            this.dividerGB.Name = "dividerGB";
            this.dividerGB.Row = 0;
            this.dividerGB.RowsSpanned = 2;
            this.dividerGB.TabStop = false;
            this.dividerGB.YOffset = 0;
            // 
            // typeFilterCB
            // 
            resources.ApplyResources(this.typeFilterCB, "typeFilterCB");
            this.typeFilterCB.CellHeight = 27;
            this.typeFilterCB.CellWidth = 222;
            this.typeFilterCB.Column = 3;
            this.typeFilterCB.ColumnsSpanned = 0;
            this.typeFilterCB.DesignSize = new System.Drawing.Size(0, 0);
            this.typeFilterCB.FormattingEnabled = true;
            this.typeFilterCB.Name = "typeFilterCB";
            this.typeFilterCB.Row = 2;
            this.typeFilterCB.RowsSpanned = 0;
            this.typeFilterCB.YOffset = 0;
            this.typeFilterCB.SelectedIndexChanged += new System.EventHandler(this.typeFilterCB_SelectedIndexChanged);
            this.typeFilterCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.typeFilterCB_KeyDown);
            this.typeFilterCB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.typeFilterCB_KeyPress);
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
            // matchesLbl
            // 
            resources.ApplyResources(this.matchesLbl, "matchesLbl");
            this.matchesLbl.AutoEllipsis = true;
            this.matchesLbl.CellHeight = 27;
            this.matchesLbl.CellWidth = 81;
            this.matchesLbl.Column = 5;
            this.matchesLbl.ColumnsSpanned = 0;
            this.matchesLbl.Name = "matchesLbl";
            this.matchesLbl.Row = 1;
            this.matchesLbl.RowsSpanned = 0;
            this.matchesLbl.YOffset = 4;
            this.matchesLbl.SizeChanged += new System.EventHandler(this.matchesLbl_SizeChanged);
            // 
            // nameFilterCB
            // 
            resources.ApplyResources(this.nameFilterCB, "nameFilterCB");
            this.nameFilterCB.CellHeight = 27;
            this.nameFilterCB.CellWidth = 222;
            this.nameFilterCB.Column = 3;
            this.nameFilterCB.ColumnsSpanned = 0;
            this.nameFilterCB.FormattingEnabled = true;
            this.nameFilterCB.Name = "nameFilterCB";
            this.nameFilterCB.Row = 1;
            this.nameFilterCB.RowsSpanned = 0;
            this.nameFilterCB.YOffset = 0;
            this.nameFilterCB.SelectedIndexChanged += new System.EventHandler(this.nameFilterCB_SelectedIndexChanged);
            this.nameFilterCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nameFilterCB_KeyDown);
            // 
            // typeFilterLbl
            // 
            resources.ApplyResources(this.typeFilterLbl, "typeFilterLbl");
            this.typeFilterLbl.CellHeight = 27;
            this.typeFilterLbl.CellWidth = 44;
            this.typeFilterLbl.Column = 2;
            this.typeFilterLbl.ColumnsSpanned = 0;
            this.typeFilterLbl.Name = "typeFilterLbl";
            this.typeFilterLbl.Row = 2;
            this.typeFilterLbl.RowsSpanned = 0;
            this.typeFilterLbl.YOffset = 4;
            // 
            // parentFilterCB
            // 
            resources.ApplyResources(this.parentFilterCB, "parentFilterCB");
            this.parentFilterCB.CellHeight = 27;
            this.parentFilterCB.CellWidth = 222;
            this.parentFilterCB.Column = 1;
            this.parentFilterCB.ColumnsSpanned = 0;
            this.parentFilterCB.FormattingEnabled = true;
            this.parentFilterCB.Name = "parentFilterCB";
            this.parentFilterCB.Row = 2;
            this.parentFilterCB.RowsSpanned = 0;
            this.parentFilterCB.YOffset = 0;
            this.parentFilterCB.SelectedIndexChanged += new System.EventHandler(this.parentFilterCB_SelectedIndexChanged);
            this.parentFilterCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.parentFilterCB_KeyDown);
            // 
            // nameFilterLbl
            // 
            resources.ApplyResources(this.nameFilterLbl, "nameFilterLbl");
            this.nameFilterLbl.CellHeight = 27;
            this.nameFilterLbl.CellWidth = 44;
            this.nameFilterLbl.Column = 2;
            this.nameFilterLbl.ColumnsSpanned = 0;
            this.nameFilterLbl.Name = "nameFilterLbl";
            this.nameFilterLbl.Row = 1;
            this.nameFilterLbl.RowsSpanned = 0;
            this.nameFilterLbl.YOffset = 4;
            // 
            // ownerFilterCB
            // 
            resources.ApplyResources(this.ownerFilterCB, "ownerFilterCB");
            this.ownerFilterCB.CellHeight = 27;
            this.ownerFilterCB.CellWidth = 222;
            this.ownerFilterCB.Column = 1;
            this.ownerFilterCB.ColumnsSpanned = 0;
            this.ownerFilterCB.FormattingEnabled = true;
            this.ownerFilterCB.Name = "ownerFilterCB";
            this.ownerFilterCB.Row = 1;
            this.ownerFilterCB.RowsSpanned = 0;
            this.ownerFilterCB.YOffset = 0;
            this.ownerFilterCB.SelectedIndexChanged += new System.EventHandler(this.ownerFilterCB_SelectedIndexChanged);
            this.ownerFilterCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ownerFilterCB_KeyDown);
            // 
            // parentFilterLbl
            // 
            resources.ApplyResources(this.parentFilterLbl, "parentFilterLbl");
            this.parentFilterLbl.CellHeight = 27;
            this.parentFilterLbl.CellWidth = 47;
            this.parentFilterLbl.Column = 0;
            this.parentFilterLbl.ColumnsSpanned = 0;
            this.parentFilterLbl.Name = "parentFilterLbl";
            this.parentFilterLbl.Row = 2;
            this.parentFilterLbl.RowsSpanned = 0;
            this.parentFilterLbl.YOffset = 4;
            // 
            // depotFilterCB
            // 
            resources.ApplyResources(this.depotFilterCB, "depotFilterCB");
            this.depotFilterCB.CellHeight = 29;
            this.depotFilterCB.CellWidth = 488;
            this.depotFilterCB.Column = 1;
            this.depotFilterCB.ColumnsSpanned = 2;
            this.depotFilterCB.DesignSize = new System.Drawing.Size(0, 0);
            this.depotFilterCB.DropDownHeight = 150;
            this.depotFilterCB.FormattingEnabled = true;
            this.depotFilterCB.Name = "depotFilterCB";
            this.depotFilterCB.Row = 0;
            this.depotFilterCB.RowsSpanned = 0;
            this.depotFilterCB.YOffset = 1;
            this.depotFilterCB.SelectedIndexChanged += new System.EventHandler(this.depotFilterCB_SelectedIndexChanged);
            this.depotFilterCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.depotFilterCB_KeyDown);
            this.depotFilterCB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.depotFilterCB_KeyPress);
            // 
            // ownerFilterLbl
            // 
            resources.ApplyResources(this.ownerFilterLbl, "ownerFilterLbl");
            this.ownerFilterLbl.CellHeight = 27;
            this.ownerFilterLbl.CellWidth = 47;
            this.ownerFilterLbl.Column = 0;
            this.ownerFilterLbl.ColumnsSpanned = 0;
            this.ownerFilterLbl.Name = "ownerFilterLbl";
            this.ownerFilterLbl.Row = 1;
            this.ownerFilterLbl.RowsSpanned = 0;
            this.ownerFilterLbl.YOffset = 4;
            // 
            // depotFilterLbl
            // 
            resources.ApplyResources(this.depotFilterLbl, "depotFilterLbl");
            this.depotFilterLbl.CellHeight = 29;
            this.depotFilterLbl.CellWidth = 47;
            this.depotFilterLbl.Column = 0;
            this.depotFilterLbl.ColumnsSpanned = 0;
            this.depotFilterLbl.Name = "depotFilterLbl";
            this.depotFilterLbl.Row = 0;
            this.depotFilterLbl.RowsSpanned = 0;
            this.depotFilterLbl.YOffset = 5;
            // 
            // streamsTreeListView
            // 
            this.streamsTreeListView._maxLineOffset = 0;
            this.streamsTreeListView.ActionColumn = 1;
            this.streamsTreeListView.AllowColumnReorder = true;
            resources.ApplyResources(this.streamsTreeListView, "streamsTreeListView");
            this.streamsTreeListView.CellHeight = 151;
            this.streamsTreeListView.CellWidth = 626;
            this.streamsTreeListView.Column = 0;
            this.streamsTreeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ClmHdr_Stream,
            this.ClmHdr_PendingAction,
            this.ClmHdr_StreamRoot,
            this.ClmHdr_Parent,
            this.ClmHdr_Owner,
            this.ClmHdr_ParentRoot,
            this.ClmHdr_Type,
            this.ClmHdr_Description,
            this.ClmHdr_SubmitAccess,
            this.ClmHdr_Locked});
            this.streamsTreeListView.ColumnsSpanned = 5;
            this.streamsTreeListView.ContextMenuStrip = this.streamContextMenuStrip;
            this.streamsTreeListView.EnableIconOverlays = false;
            this.streamsTreeListView.EnableSorting = false;
            this.streamsTreeListView.FullRowSelect = true;
            this.streamsTreeListView.MultiSelect = false;
            this.streamsTreeListView.MultiSelectConditions = Perforce.P4VS.TreeListView.MultiSelectCondition.none;
            this.streamsTreeListView.Name = "streamsTreeListView";
            this.streamsTreeListView.OverlayOffset = 0;
            this.streamsTreeListView.OwnerDraw = true;
            this.streamsTreeListView.RootCheckBoxes = false;
            this.streamsTreeListView.Row = 3;
            this.streamsTreeListView.RowsSpanned = 0;
            this.streamsTreeListView.ScrollPosition = 0;
            this.streamsTreeListView.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.streamsTreeListView.TabStop = false;
            this.streamsTreeListView.TreeView = true;
            this.streamsTreeListView.UseClassicImageList = false;
            this.streamsTreeListView.UseCompatibleStateImageBehavior = false;
            this.streamsTreeListView.View = System.Windows.Forms.View.Details;
            this.streamsTreeListView.YOffset = 0;
            this.streamsTreeListView.onMaxScroll += new System.Windows.Forms.ScrollEventHandler(this.streamsTreeListView_onMaxScroll);
            this.streamsTreeListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.streamsTreeListView_ColumnClick);
            this.streamsTreeListView.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.streamsTreeListView_ColumnReordered);
            this.streamsTreeListView.SelectedIndexChanged += new System.EventHandler(this.streamsTreeListView_SelectedIndexChanged);
            this.streamsTreeListView.Click += new System.EventHandler(this.streamsTreeListView_Click);
            this.streamsTreeListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.streamsTreeListView_MouseDoubleClick);
            // 
            // ClmHdr_Stream
            // 
            resources.ApplyResources(this.ClmHdr_Stream, "ClmHdr_Stream");
            // 
            // ClmHdr_PendingAction
            // 
            resources.ApplyResources(this.ClmHdr_PendingAction, "ClmHdr_PendingAction");
            // 
            // ClmHdr_StreamRoot
            // 
            resources.ApplyResources(this.ClmHdr_StreamRoot, "ClmHdr_StreamRoot");
            // 
            // ClmHdr_Parent
            // 
            resources.ApplyResources(this.ClmHdr_Parent, "ClmHdr_Parent");
            // 
            // ClmHdr_Owner
            // 
            resources.ApplyResources(this.ClmHdr_Owner, "ClmHdr_Owner");
            // 
            // ClmHdr_ParentRoot
            // 
            resources.ApplyResources(this.ClmHdr_ParentRoot, "ClmHdr_ParentRoot");
            // 
            // ClmHdr_Type
            // 
            resources.ApplyResources(this.ClmHdr_Type, "ClmHdr_Type");
            // 
            // ClmHdr_Description
            // 
            resources.ApplyResources(this.ClmHdr_Description, "ClmHdr_Description");
            // 
            // ClmHdr_SubmitAccess
            // 
            resources.ApplyResources(this.ClmHdr_SubmitAccess, "ClmHdr_SubmitAccess");
            // 
            // ClmHdr_Locked
            // 
            resources.ApplyResources(this.ClmHdr_Locked, "ClmHdr_Locked");
            // 
            // streamContextMenuStrip
            // 
            this.streamContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mergeToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.toolStripSeparator1,
            this.refreshStreamsListToolStripMenuItem,
            this.refreshStreamToolStripMenuItem});
            this.streamContextMenuStrip.Name = "changelistContextMenuStrip";
            resources.ApplyResources(this.streamContextMenuStrip, "streamContextMenuStrip");
            this.streamContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.streamsContextMenuStrip_Opening);
            // 
            // mergeToolStripMenuItem
            // 
            this.mergeToolStripMenuItem.Name = "mergeToolStripMenuItem";
            resources.ApplyResources(this.mergeToolStripMenuItem, "mergeToolStripMenuItem");
            this.mergeToolStripMenuItem.Click += new System.EventHandler(this.mergeToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.toolStripSeparator1.ForeColor = System.Drawing.Color.Black;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // refreshStreamsListToolStripMenuItem
            // 
            this.refreshStreamsListToolStripMenuItem.Name = "refreshStreamsListToolStripMenuItem";
            resources.ApplyResources(this.refreshStreamsListToolStripMenuItem, "refreshStreamsListToolStripMenuItem");
            this.refreshStreamsListToolStripMenuItem.Click += new System.EventHandler(this.refreshStreamsListToolStripMenuItem_Click);
            // 
            // refreshStreamToolStripMenuItem
            // 
            this.refreshStreamToolStripMenuItem.Name = "refreshStreamToolStripMenuItem";
            resources.ApplyResources(this.refreshStreamToolStripMenuItem, "refreshStreamToolStripMenuItem");
            this.refreshStreamToolStripMenuItem.Click += new System.EventHandler(this.refreshStreamToolStripMenuItem_Click);
            // 
            // threadMonitorControl1
            // 
            this.threadMonitorControl1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.threadMonitorControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.threadMonitorControl1.CancelPressed = false;
            resources.ApplyResources(this.threadMonitorControl1, "threadMonitorControl1");
            this.threadMonitorControl1.Maximum = 100;
            this.threadMonitorControl1.Name = "threadMonitorControl1";
            this.threadMonitorControl1.Step = 1;
            this.threadMonitorControl1.Value = 50;
            // 
            // gridLayoutPanel2
            // 
            resources.ApplyResources(this.gridLayoutPanel2, "gridLayoutPanel2");
            this.gridLayoutPanel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gridLayoutPanel2.CellHeight = 0;
            this.gridLayoutPanel2.CellWidth = 0;
            this.gridLayoutPanel2.Column = 0;
            this.gridLayoutPanel2.ColumnsSpanned = 0;
            this.gridLayoutPanel2.Controls.Add(this.typeTB);
            this.gridLayoutPanel2.Controls.Add(this.ownerLbl);
            this.gridLayoutPanel2.Controls.Add(this.parentTB);
            this.gridLayoutPanel2.Controls.Add(this.remappedTB);
            this.gridLayoutPanel2.Controls.Add(this.streamRootTB);
            this.gridLayoutPanel2.Controls.Add(this.ownerTB);
            this.gridLayoutPanel2.Controls.Add(this.typeLbl);
            this.gridLayoutPanel2.Controls.Add(this.pathsTB);
            this.gridLayoutPanel2.Controls.Add(this.streamRootLbl);
            this.gridLayoutPanel2.Controls.Add(this.accessedTB);
            this.gridLayoutPanel2.Controls.Add(this.parentLbl);
            this.gridLayoutPanel2.Controls.Add(this.optionsLbl);
            this.gridLayoutPanel2.Controls.Add(this.modifiedTB);
            this.gridLayoutPanel2.Controls.Add(this.gridLayoutSubpanel1);
            this.gridLayoutPanel2.Controls.Add(this.accessedLbl);
            this.gridLayoutPanel2.Controls.Add(this.descriptionLbl);
            this.gridLayoutPanel2.Controls.Add(this.dateLbl);
            this.gridLayoutPanel2.Controls.Add(this.descriptionTB);
            this.gridLayoutPanel2.Controls.Add(this.streamTB);
            this.gridLayoutPanel2.Controls.Add(this.pathsLbl);
            this.gridLayoutPanel2.Controls.Add(this.streamLbl);
            this.gridLayoutPanel2.Controls.Add(this.viewTB);
            this.gridLayoutPanel2.Controls.Add(this.ignoredTB);
            this.gridLayoutPanel2.Controls.Add(this.viewLbl);
            this.gridLayoutPanel2.Controls.Add(this.ignoredLbl);
            this.gridLayoutPanel2.Controls.Add(this.remappedLbl);
            this.gridLayoutPanel2.EnableDesignerGrid = false;
            this.gridLayoutPanel2.EnableDesignerLayout = true;
            this.gridLayoutPanel2.EnableParentResize = false;
            this.gridLayoutPanel2.MinimumColumnWidth = 10;
            this.gridLayoutPanel2.MinimumRowHeight = 10;
            this.gridLayoutPanel2.Name = "gridLayoutPanel2";
            this.gridLayoutPanel2.Row = 0;
            this.gridLayoutPanel2.RowsSpanned = 0;
            this.gridLayoutPanel2.YOffset = 0;
            // 
            // typeTB
            // 
            resources.ApplyResources(this.typeTB, "typeTB");
            this.typeTB.CellHeight = 26;
            this.typeTB.CellWidth = 231;
            this.typeTB.Column = 3;
            this.typeTB.ColumnsSpanned = 0;
            this.typeTB.Name = "typeTB";
            this.typeTB.ReadOnly = true;
            this.typeTB.Row = 2;
            this.typeTB.RowsSpanned = 0;
            this.typeTB.YOffset = 0;
            // 
            // ownerLbl
            // 
            resources.ApplyResources(this.ownerLbl, "ownerLbl");
            this.ownerLbl.CellHeight = 26;
            this.ownerLbl.CellWidth = 88;
            this.ownerLbl.Column = 0;
            this.ownerLbl.ColumnsSpanned = 0;
            this.ownerLbl.Name = "ownerLbl";
            this.ownerLbl.Row = 3;
            this.ownerLbl.RowsSpanned = 0;
            this.ownerLbl.YOffset = 3;
            // 
            // parentTB
            // 
            resources.ApplyResources(this.parentTB, "parentTB");
            this.parentTB.CellHeight = 26;
            this.parentTB.CellWidth = 231;
            this.parentTB.Column = 3;
            this.parentTB.ColumnsSpanned = 0;
            this.parentTB.Name = "parentTB";
            this.parentTB.ReadOnly = true;
            this.parentTB.Row = 1;
            this.parentTB.RowsSpanned = 0;
            this.parentTB.YOffset = 0;
            // 
            // remappedTB
            // 
            resources.ApplyResources(this.remappedTB, "remappedTB");
            this.remappedTB.CellHeight = 40;
            this.remappedTB.CellWidth = 538;
            this.remappedTB.Column = 1;
            this.remappedTB.ColumnsSpanned = 2;
            this.remappedTB.Name = "remappedTB";
            this.remappedTB.ReadOnly = true;
            this.remappedTB.Row = 7;
            this.remappedTB.RowsSpanned = 0;
            this.remappedTB.YOffset = 0;
            // 
            // streamRootTB
            // 
            resources.ApplyResources(this.streamRootTB, "streamRootTB");
            this.streamRootTB.CellHeight = 26;
            this.streamRootTB.CellWidth = 231;
            this.streamRootTB.Column = 3;
            this.streamRootTB.ColumnsSpanned = 0;
            this.streamRootTB.Name = "streamRootTB";
            this.streamRootTB.ReadOnly = true;
            this.streamRootTB.Row = 0;
            this.streamRootTB.RowsSpanned = 0;
            this.streamRootTB.YOffset = 0;
            // 
            // ownerTB
            // 
            resources.ApplyResources(this.ownerTB, "ownerTB");
            this.ownerTB.CellHeight = 26;
            this.ownerTB.CellWidth = 232;
            this.ownerTB.Column = 1;
            this.ownerTB.ColumnsSpanned = 0;
            this.ownerTB.Name = "ownerTB";
            this.ownerTB.ReadOnly = true;
            this.ownerTB.Row = 3;
            this.ownerTB.RowsSpanned = 0;
            this.ownerTB.YOffset = 0;
            // 
            // typeLbl
            // 
            resources.ApplyResources(this.typeLbl, "typeLbl");
            this.typeLbl.CellHeight = 26;
            this.typeLbl.CellWidth = 75;
            this.typeLbl.Column = 2;
            this.typeLbl.ColumnsSpanned = 0;
            this.typeLbl.Name = "typeLbl";
            this.typeLbl.Row = 2;
            this.typeLbl.RowsSpanned = 0;
            this.typeLbl.YOffset = 3;
            // 
            // pathsTB
            // 
            resources.ApplyResources(this.pathsTB, "pathsTB");
            this.pathsTB.CellHeight = 40;
            this.pathsTB.CellWidth = 538;
            this.pathsTB.Column = 1;
            this.pathsTB.ColumnsSpanned = 2;
            this.pathsTB.Name = "pathsTB";
            this.pathsTB.ReadOnly = true;
            this.pathsTB.Row = 6;
            this.pathsTB.RowsSpanned = 0;
            this.pathsTB.YOffset = 0;
            // 
            // streamRootLbl
            // 
            resources.ApplyResources(this.streamRootLbl, "streamRootLbl");
            this.streamRootLbl.CellHeight = 26;
            this.streamRootLbl.CellWidth = 75;
            this.streamRootLbl.Column = 2;
            this.streamRootLbl.ColumnsSpanned = 0;
            this.streamRootLbl.Name = "streamRootLbl";
            this.streamRootLbl.Row = 0;
            this.streamRootLbl.RowsSpanned = 0;
            this.streamRootLbl.YOffset = 3;
            // 
            // accessedTB
            // 
            resources.ApplyResources(this.accessedTB, "accessedTB");
            this.accessedTB.CellHeight = 26;
            this.accessedTB.CellWidth = 232;
            this.accessedTB.Column = 1;
            this.accessedTB.ColumnsSpanned = 0;
            this.accessedTB.Name = "accessedTB";
            this.accessedTB.ReadOnly = true;
            this.accessedTB.Row = 2;
            this.accessedTB.RowsSpanned = 0;
            this.accessedTB.YOffset = 0;
            // 
            // parentLbl
            // 
            resources.ApplyResources(this.parentLbl, "parentLbl");
            this.parentLbl.CellHeight = 26;
            this.parentLbl.CellWidth = 75;
            this.parentLbl.Column = 2;
            this.parentLbl.ColumnsSpanned = 0;
            this.parentLbl.Name = "parentLbl";
            this.parentLbl.Row = 1;
            this.parentLbl.RowsSpanned = 0;
            this.parentLbl.YOffset = 3;
            // 
            // optionsLbl
            // 
            resources.ApplyResources(this.optionsLbl, "optionsLbl");
            this.optionsLbl.CellHeight = 90;
            this.optionsLbl.CellWidth = 88;
            this.optionsLbl.Column = 0;
            this.optionsLbl.ColumnsSpanned = 0;
            this.optionsLbl.Name = "optionsLbl";
            this.optionsLbl.Row = 5;
            this.optionsLbl.RowsSpanned = 0;
            this.optionsLbl.YOffset = 1;
            // 
            // modifiedTB
            // 
            resources.ApplyResources(this.modifiedTB, "modifiedTB");
            this.modifiedTB.CellHeight = 26;
            this.modifiedTB.CellWidth = 232;
            this.modifiedTB.Column = 1;
            this.modifiedTB.ColumnsSpanned = 0;
            this.modifiedTB.Name = "modifiedTB";
            this.modifiedTB.ReadOnly = true;
            this.modifiedTB.Row = 1;
            this.modifiedTB.RowsSpanned = 0;
            this.modifiedTB.YOffset = 0;
            // 
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.CellHeight = 90;
            this.gridLayoutSubpanel1.CellWidth = 538;
            this.gridLayoutSubpanel1.Column = 1;
            this.gridLayoutSubpanel1.ColumnsSpanned = 2;
            this.gridLayoutSubpanel1.Controls.Add(this.fromParentChk);
            this.gridLayoutSubpanel1.Controls.Add(this.toParentChk);
            this.gridLayoutSubpanel1.Controls.Add(this.lockedChk);
            this.gridLayoutSubpanel1.Controls.Add(this.submittingChk);
            this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
            this.gridLayoutSubpanel1.Row = 5;
            this.gridLayoutSubpanel1.RowsSpanned = 0;
            this.gridLayoutSubpanel1.YOffset = 0;
            // 
            // fromParentChk
            // 
            resources.ApplyResources(this.fromParentChk, "fromParentChk");
            this.fromParentChk.CellHeight = 0;
            this.fromParentChk.CellWidth = 0;
            this.fromParentChk.Column = 0;
            this.fromParentChk.ColumnsSpanned = 0;
            this.fromParentChk.Name = "fromParentChk";
            this.fromParentChk.Row = 3;
            this.fromParentChk.RowsSpanned = 0;
            this.fromParentChk.UseVisualStyleBackColor = true;
            this.fromParentChk.YOffset = 0;
            this.fromParentChk.CheckedChanged += new System.EventHandler(this.fromParentChk_CheckedChanged);
            // 
            // toParentChk
            // 
            resources.ApplyResources(this.toParentChk, "toParentChk");
            this.toParentChk.CellHeight = 0;
            this.toParentChk.CellWidth = 0;
            this.toParentChk.Column = 0;
            this.toParentChk.ColumnsSpanned = 0;
            this.toParentChk.Name = "toParentChk";
            this.toParentChk.Row = 2;
            this.toParentChk.RowsSpanned = 0;
            this.toParentChk.UseVisualStyleBackColor = true;
            this.toParentChk.YOffset = 0;
            this.toParentChk.CheckedChanged += new System.EventHandler(this.toParentChk_CheckedChanged);
            // 
            // lockedChk
            // 
            resources.ApplyResources(this.lockedChk, "lockedChk");
            this.lockedChk.CellHeight = 0;
            this.lockedChk.CellWidth = 0;
            this.lockedChk.Column = 0;
            this.lockedChk.ColumnsSpanned = 0;
            this.lockedChk.Name = "lockedChk";
            this.lockedChk.Row = 1;
            this.lockedChk.RowsSpanned = 0;
            this.lockedChk.UseVisualStyleBackColor = true;
            this.lockedChk.YOffset = 0;
            this.lockedChk.CheckedChanged += new System.EventHandler(this.lockedChk_CheckedChanged);
            // 
            // submittingChk
            // 
            resources.ApplyResources(this.submittingChk, "submittingChk");
            this.submittingChk.CellHeight = 0;
            this.submittingChk.CellWidth = 0;
            this.submittingChk.Column = 0;
            this.submittingChk.ColumnsSpanned = 0;
            this.submittingChk.Name = "submittingChk";
            this.submittingChk.Row = 0;
            this.submittingChk.RowsSpanned = 0;
            this.submittingChk.UseVisualStyleBackColor = true;
            this.submittingChk.YOffset = 0;
            this.submittingChk.CheckedChanged += new System.EventHandler(this.submittingChk_CheckedChanged);
            // 
            // accessedLbl
            // 
            resources.ApplyResources(this.accessedLbl, "accessedLbl");
            this.accessedLbl.CellHeight = 26;
            this.accessedLbl.CellWidth = 88;
            this.accessedLbl.Column = 0;
            this.accessedLbl.ColumnsSpanned = 0;
            this.accessedLbl.Name = "accessedLbl";
            this.accessedLbl.Row = 2;
            this.accessedLbl.RowsSpanned = 0;
            this.accessedLbl.YOffset = 3;
            // 
            // descriptionLbl
            // 
            resources.ApplyResources(this.descriptionLbl, "descriptionLbl");
            this.descriptionLbl.CellHeight = 43;
            this.descriptionLbl.CellWidth = 88;
            this.descriptionLbl.Column = 0;
            this.descriptionLbl.ColumnsSpanned = 0;
            this.descriptionLbl.Name = "descriptionLbl";
            this.descriptionLbl.Row = 4;
            this.descriptionLbl.RowsSpanned = 0;
            this.descriptionLbl.YOffset = 0;
            // 
            // dateLbl
            // 
            resources.ApplyResources(this.dateLbl, "dateLbl");
            this.dateLbl.CellHeight = 26;
            this.dateLbl.CellWidth = 88;
            this.dateLbl.Column = 0;
            this.dateLbl.ColumnsSpanned = 0;
            this.dateLbl.Name = "dateLbl";
            this.dateLbl.Row = 1;
            this.dateLbl.RowsSpanned = 0;
            this.dateLbl.YOffset = 3;
            // 
            // descriptionTB
            // 
            resources.ApplyResources(this.descriptionTB, "descriptionTB");
            this.descriptionTB.CellHeight = 43;
            this.descriptionTB.CellWidth = 538;
            this.descriptionTB.Column = 1;
            this.descriptionTB.ColumnsSpanned = 2;
            this.descriptionTB.Name = "descriptionTB";
            this.descriptionTB.ReadOnly = true;
            this.descriptionTB.Row = 4;
            this.descriptionTB.RowsSpanned = 0;
            this.descriptionTB.YOffset = 0;
            // 
            // streamTB
            // 
            resources.ApplyResources(this.streamTB, "streamTB");
            this.streamTB.CellHeight = 26;
            this.streamTB.CellWidth = 232;
            this.streamTB.Column = 1;
            this.streamTB.ColumnsSpanned = 0;
            this.streamTB.Name = "streamTB";
            this.streamTB.ReadOnly = true;
            this.streamTB.Row = 0;
            this.streamTB.RowsSpanned = 0;
            this.streamTB.YOffset = 0;
            // 
            // pathsLbl
            // 
            resources.ApplyResources(this.pathsLbl, "pathsLbl");
            this.pathsLbl.CellHeight = 40;
            this.pathsLbl.CellWidth = 88;
            this.pathsLbl.Column = 0;
            this.pathsLbl.ColumnsSpanned = 0;
            this.pathsLbl.Name = "pathsLbl";
            this.pathsLbl.Row = 6;
            this.pathsLbl.RowsSpanned = 0;
            this.pathsLbl.YOffset = 0;
            // 
            // streamLbl
            // 
            resources.ApplyResources(this.streamLbl, "streamLbl");
            this.streamLbl.CellHeight = 26;
            this.streamLbl.CellWidth = 88;
            this.streamLbl.Column = 0;
            this.streamLbl.ColumnsSpanned = 0;
            this.streamLbl.Name = "streamLbl";
            this.streamLbl.Row = 0;
            this.streamLbl.RowsSpanned = 0;
            this.streamLbl.YOffset = 3;
            // 
            // viewTB
            // 
            resources.ApplyResources(this.viewTB, "viewTB");
            this.viewTB.CellHeight = 40;
            this.viewTB.CellWidth = 538;
            this.viewTB.Column = 1;
            this.viewTB.ColumnsSpanned = 2;
            this.viewTB.Name = "viewTB";
            this.viewTB.ReadOnly = true;
            this.viewTB.Row = 9;
            this.viewTB.RowsSpanned = 0;
            this.viewTB.YOffset = 0;
            // 
            // ignoredTB
            // 
            resources.ApplyResources(this.ignoredTB, "ignoredTB");
            this.ignoredTB.CellHeight = 40;
            this.ignoredTB.CellWidth = 538;
            this.ignoredTB.Column = 1;
            this.ignoredTB.ColumnsSpanned = 2;
            this.ignoredTB.Name = "ignoredTB";
            this.ignoredTB.ReadOnly = true;
            this.ignoredTB.Row = 8;
            this.ignoredTB.RowsSpanned = 0;
            this.ignoredTB.YOffset = 0;
            // 
            // viewLbl
            // 
            resources.ApplyResources(this.viewLbl, "viewLbl");
            this.viewLbl.CellHeight = 40;
            this.viewLbl.CellWidth = 395;
            this.viewLbl.Column = 0;
            this.viewLbl.ColumnsSpanned = 2;
            this.viewLbl.Name = "viewLbl";
            this.viewLbl.Row = 9;
            this.viewLbl.RowsSpanned = 0;
            this.viewLbl.YOffset = 0;
            // 
            // ignoredLbl
            // 
            resources.ApplyResources(this.ignoredLbl, "ignoredLbl");
            this.ignoredLbl.CellHeight = 40;
            this.ignoredLbl.CellWidth = 88;
            this.ignoredLbl.Column = 0;
            this.ignoredLbl.ColumnsSpanned = 0;
            this.ignoredLbl.Name = "ignoredLbl";
            this.ignoredLbl.Row = 8;
            this.ignoredLbl.RowsSpanned = 0;
            this.ignoredLbl.YOffset = 0;
            // 
            // remappedLbl
            // 
            resources.ApplyResources(this.remappedLbl, "remappedLbl");
            this.remappedLbl.CellHeight = 40;
            this.remappedLbl.CellWidth = 88;
            this.remappedLbl.Column = 0;
            this.remappedLbl.ColumnsSpanned = 0;
            this.remappedLbl.Name = "remappedLbl";
            this.remappedLbl.Row = 7;
            this.remappedLbl.RowsSpanned = 0;
            this.remappedLbl.YOffset = 0;
            // 
            // StreamsToolWindowControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitContainer1);
            this.Name = "StreamsToolWindowControl";
            this.Load += new System.EventHandler(this.StreamsToolWindowControl_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.streamContextMenuStrip.ResumeLayout(false);
            this.gridLayoutPanel2.ResumeLayout(false);
            this.gridLayoutPanel2.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private void clearDetails()
        {
            streamTB.Text = string.Empty;
            streamTB.Refresh();
            modifiedTB.Text = string.Empty;
            modifiedTB.Refresh();
            accessedTB.Text = string.Empty;
            accessedTB.Refresh();
            ownerTB.Text = string.Empty;
            ownerTB.Refresh();
            streamRootTB.Text = string.Empty;
            streamRootTB.Refresh();
            parentTB.Text = string.Empty;
            parentTB.Refresh();
            typeTB.Text = string.Empty;
            typeTB.Refresh();
            descriptionTB.Text = string.Empty;
            descriptionTB.Refresh();
            pathsTB.Text = string.Empty;
            pathsTB.Refresh();
            remappedTB.Text = string.Empty;
            remappedTB.Refresh();
            ignoredTB.Text = string.Empty;
            ignoredTB.Refresh();
            viewTB.Text = string.Empty;
            viewTB.Refresh();
        }

        private void clearFilters()
        {
            depotFilterCB.Items.Clear();
            ownerFilterCB.Items.Clear();
            nameFilterCB.Items.Clear();
            parentFilterCB.Items.Clear();
            typeFilterCB.Items.Clear();
        }

        public new P4ScmProvider Scm { get; set; }

        private string DepotFilterText { get; set; }
        private string OwnerFilterText { get; set; }
        private string ParentFilterText { get; set; }
        private string NameFilterText { get; set; }
        private string TypeFilterText { get; set; }

        private object SyncRoot = new object();

        public P4.Stream SelectedStream { get; private set; }

        private delegate void StreamsTreeListViewSetBoolDelegate(bool state);
        private delegate void StreamsTreeListViewDelegate();
        private delegate void StreamsTreeListViewItemDelegate(TreeListViewItem item);
        private delegate ListViewItem StreamsTreeListViewItemDelegate2(TreeListViewItem item);
        private delegate void setStringPropertyDelegate(string str);

        private void setStreamsMatchesLblText(string matches)
        {
            this.matchesLbl.Text = matches;
        }

        private delegate void ReplaceStreamsTreeListViewItemDelegate(int index, TreeListViewItem item);

        private void replaceStreamsListViewItem(int index, TreeListViewItem item)
        {
            streamsTreeListView.Nodes.Remove(item);
            //streamsTreeListView.Nodes.Insert(index, item);
        }

        private TreeListViewItem createStreamTreeListViewItem(P4.Stream stream, bool inFilteredView)
        {
            string streamName = stream.Name;
            string streamRoot = stream.Id;
            string parentRoot = stream.Parent.Path;
            string streamParent = string.Empty;
            if (parentRoot != "none")
            {
                streamParent = parentRoot.Remove(parentRoot.IndexOf('/'),
                    (parentRoot.LastIndexOf('/') + 1));
            }
            string streamOwner = stream.OwnerName.ToString();
            string streamType = stream.Type.ToString();
            string description = stream.Description;
            string submitAccess = "Owner";
            if ((stream.Options & P4.StreamOption.OwnerSubmit) == 0)
            {
                submitAccess = "All";
            }
            string locked = string.Empty;
            if ((stream.Options & P4.StreamOption.Locked) != 0)
            {
                locked = " X";
            }

            TreeListViewItem item = new TreeListViewItem();
            item = new TreeListViewItem(null, new string[] { streamName, String.Empty,streamRoot,
                                    streamParent, streamOwner, parentRoot, streamType, description,submitAccess,locked }
                                );

            if (!inFilteredView)
            {
                item.ForeColor = Color.Gray;
            }

            item.ImageIndex = 15;

            if (stream.Type == P4.StreamType.Mainline)
            {
                item.ImageIndex = 2;
            }
            if (stream.Type == P4.StreamType.Release)
            {
                item.ImageIndex = 13;
            }
            if (stream.Type == P4.StreamType.Development)
            {
                item.ImageIndex = 0;
            }
            if (stream.Type == P4.StreamType.Task)
            {
                item.ImageIndex = 14;
            }
            if (stream.Type == P4.StreamType.Virtual)
            {
                item.ImageIndex = 16;
            }
            item.Tag = stream;
            item.action1 = -1;
            item.action2 = -1;

            if (stream.Type != P4.StreamType.Virtual)
            {
                P4.Options options = new P4.Options();
                options["-a"] = null;
                P4.StreamMetaData integData = Scm.GetStreamMetaData(stream, options);
                if (integData != null)
                {
                    item.firmer = integData.FirmerThanParent;
                    if (integData.IntegFromParent)
                    {
                        if (integData.IntegFromParentHow ==
                            P4.StreamMetaData.IntegAction.Merge)
                        {
                            item.action1 = 3; // merge available
                        }
                        if (integData.IntegFromParentHow ==
                            P4.StreamMetaData.IntegAction.Copy)
                        {
                            item.action2 = 7; // promote available
                        }

                    }
                    if (!(integData.IntegFromParent) && integData.ChangeFlowsFromParent
                         && integData.Parent.Path != "none")
                    {
                        if (item.firmer)
                        {
                            item.action2 = 11; // promote not available
                        }
                        else
                        {
                            item.action1 = 5; // merge not available
                        }
                    }
                    if (integData.IntegToParent)
                    {
                        if (integData.IntegToParentHow ==
                            P4.StreamMetaData.IntegAction.Merge)
                        {
                            item.action1 = 3; // merge available
                        }
                        if (integData.IntegToParentHow ==
                           P4.StreamMetaData.IntegAction.Copy)
                        {
                            item.action2 = 7; // promote available
                        }
                    }
                    if (!(integData.IntegToParent) && integData.ChangeFlowsToParent
                        && integData.Parent.Path != "none")
                    {
                        if (item.firmer)
                        {
                            item.action1 = 5; // merge not available
                        }
                        else
                        {
                            item.action2 = 11; // promote not available
                        }
                    }
                    if (item.action1 == 3 && item.action2 == 7)
                    {
                        item.action2 = 9;// promote available after merge
                    }
                }
            }

            return item;
        }

        private void AsyncPopulateListView()
        {
            int streamsCount = 0;
            bool threadAborted = false;
            if (streamsTreeListView.InvokeRequired)
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
                        if ((Scm == null) || (Scm.Connection.Disconnected))
                        {
                            return;
                        }

                        Scm.Connection.Repository.Connection.getP4Server().ProgramName = "P4VS";
                        Scm.Connection.Repository.Connection.getP4Server().ProgramVersion = Versions.product();

                        string depot = null;
                        string owner = null;
                        string parent = null;
                        string name = null;
                        string type = null;
                        P4.FileSpec depotPath = new P4.FileSpec();
                        string filter = null;

                        if (string.IsNullOrEmpty(DepotFilterText))
                        { depotPath = null; }
                        else
                        {
                            depot = "//" + DepotFilterText + "/...";
                            depotPath = new P4.FileSpec(new P4.DepotPath(depot),
                                null, null, null);
                        }

                        if (string.IsNullOrEmpty(OwnerFilterText))
                        { owner = null; }
                        else
                        {
                            owner = OwnerFilterText;
                            if (filter != null)
                            {
                                filter = filter + " & Owner=" + owner;
                            }
                            else
                            {
                                filter = "Owner=" + owner;
                            }
                        }

                        if (string.IsNullOrEmpty(ParentFilterText))
                        { parent = null; }
                        else
                        {
                            parent = ParentFilterText;
                            if (filter != null)
                            {
                                filter = filter + " & Parent=" + parent;
                            }
                            else
                            {
                                filter = "Parent=" + parent;
                            }
                        }

                        if (string.IsNullOrEmpty(NameFilterText))
                        { name = null; }
                        else
                        {
                            name = NameFilterText;
                            if (filter != null)
                            {
                                filter = filter + " & Name=" + name;
                            }
                            else
                            {
                                filter = "Name=" + name;
                            }
                        }

                        if (string.IsNullOrEmpty(TypeFilterText))
                        { type = null; }
                        else
                        {
                            type = TypeFilterText;
                            if (filter != null)
                            {
                                filter = filter + " & Type=" + type;
                            }
                            else
                            {
                                filter = "Type=" + type;
                            }
                        }

                        if (maxItems == 0)
                        {
                            maxItems = -1;
                        }


                        if (streamsTreeListView.InvokeRequired)
                        {
                            streamsTreeListView.Invoke(new StreamsToolWindowControl.StreamsTreeListViewDelegate(this.streamsTreeListView.Items.Clear));
                            streamsTreeListView.Invoke(new StreamsToolWindowControl.StreamsTreeListViewDelegate(this.streamsTreeListView.Nodes.Clear));
                        }
                        else
                        {
                            streamsTreeListView.Items.Clear();
                            streamsTreeListView.Nodes.Clear();
                        }

                        // add filter items here
                        P4.Options options = new P4.Options(P4.StreamsCmdFlags.None,
                            filter, null, depot, 0);
                        IList<P4.Stream> streams = Scm.GetStreams(options, depotPath);

                        TreeListViewItem it = new TreeListViewItem();
                        if ((streams == null) || (streams.Count <= 0))
                        {
                            it = new TreeListViewItem(null, Resources.JobsToolWindowControl_NoItemsAvailable, true);

                            if (streamsTreeListView.InvokeRequired)
                            {
                                streamsTreeListView.Invoke(new StreamsToolWindowControl.StreamsTreeListViewDelegate(this.streamsTreeListView.Items.Clear));
                                streamsTreeListView.Invoke(new StreamsToolWindowControl.StreamsTreeListViewDelegate(this.streamsTreeListView.Nodes.Clear));
                                streamsTreeListView.Invoke(new StreamsTreeListViewItemDelegate(this.streamsTreeListView.Nodes.Add), it);
                                this.matchesLbl.Invoke(new setStringPropertyDelegate(setStreamsMatchesLblText),
                                    Resources.JobsToolWindowControl_NoMatches);
                                streamsTreeListView.Invoke(new StreamsTreeListViewDelegate(this.streamsTreeListView.BuildTreeList));
                            }
                            else
                            {
                                streamsTreeListView.Items.Clear();
                                streamsTreeListView.Nodes.Clear();
                                this.streamsTreeListView.Nodes.Add(it);
                                this.matchesLbl.Text = Resources.JobsToolWindowControl_NoMatches;
                                this.streamsTreeListView.BuildTreeList();
                            }

                            return;
                        }

                        SortedDictionary<string, TreeListViewItem> streamTree = new SortedDictionary<string, TreeListViewItem>();
                        Dictionary<string, TreeListViewItem> addedParents = new Dictionary<string, TreeListViewItem>();
                        Dictionary<string, P4.Stream> initialStreamsList = new Dictionary<string, P4.Stream>();

                        List<P4.Stream> hierarchyStreams = new List<P4.Stream>();
                        int i = 0;
                        threadMonitorControl1.Show(streams.Count * 2);
                        foreach (P4.Stream stream in streams)
                        {
                            if (threadMonitorControl1.CancelPressed)
                            {
                                break;
                            }
                            hierarchyStreams.Add(stream);
                            initialStreamsList.Add(stream.Id, stream);
                            if (stream.Parent.Path != "none")
                            {
                                P4.Stream nextParent = new P4.Stream();
                                bool isParent = false;
                                string thisParent = string.Empty;
                                nextParent = stream;
                                while (isParent == false)
                                {
                                    thisParent = nextParent.Parent.Path;
                                    nextParent = Scm.GetStream(thisParent, null, null);
                                    hierarchyStreams.Add(nextParent);
                                    if (nextParent.Parent.Path == "none")
                                    {
                                        isParent = true;
                                    }
                                }
                            }
                            i++;
                            threadMonitorControl1.Value = i;
                        }

                        foreach (P4.Stream stream in hierarchyStreams)
                        {
                            if (threadMonitorControl1.CancelPressed)
                            {
                                break;
                            }
                            bool InfilteredView = false;
                            if (initialStreamsList.ContainsKey(stream.Id))
                            {
                                InfilteredView = true;
                            }

                            TreeListViewItem createdItem = createStreamTreeListViewItem(stream, InfilteredView);
                            string[] path = new string[1];
                            path[0] = stream.Parent.Path;

                            if (path[0].StartsWith("//"))
                            {
                                TreeListViewItem parentitem = new TreeListViewItem(null, path);
                                createdItem.ParentItem = parentitem;
                            }
                            if (streamTree.ContainsKey(stream.Id))
                            {
                                continue;
                            }
                            streamTree.Add(stream.Id, createdItem);
                            i++;
                            threadMonitorControl1.Value = i;
                            if (InfilteredView)
                            {
                                streamsCount++;
                            }
                        }

                        foreach (KeyValuePair<string, TreeListViewItem> tlvi in streamTree)
                        {
                            TreeListViewItem currentItem = tlvi.Value;
                            // if the item is a mainline or parentless task do nothing
                            if (currentItem.ParentItem == null)
                            {
                                continue;
                            }
                            // or, add the item to its parent
                            else
                            {
                                P4.Stream currentChild = currentItem.Tag as P4.Stream;
                                bool InfilteredView = false;
                                if (initialStreamsList.ContainsKey(currentChild.Id))
                                {
                                    InfilteredView = true;
                                }
                                // if the parent is in the dictionary, find it and add the
                                // item to it.
                                if (streamTree.ContainsKey(currentChild.Parent.Path))
                                {
                                    TreeListViewItem thisParent = new TreeListViewItem();
                                    streamTree.TryGetValue(currentChild.Parent.Path, out thisParent);
                                    currentItem.ParentItem = thisParent;
                                    if (addedParents.ContainsKey(currentChild.Parent.Path))
                                    {
                                        addedParents.Remove(currentChild.Parent.Path);
                                    }
                                    thisParent.ChildNodes.Add(currentItem);
                                    addedParents.Add(currentChild.Parent.Path, thisParent);

                                }
                                // otherwise, fetch the parent, build a new item and
                                // add that item to a new dictionary, adding the child
                                // item to it.
                                else
                                {
                                    P4.Stream fetchedParent = Scm.GetStream(currentChild.Parent.Path, null, null);
                                    TreeListViewItem createdItem = createStreamTreeListViewItem(fetchedParent, InfilteredView);
                                    createdItem.ChildNodes.Add(currentItem);
                                    addedParents.Add(fetchedParent.Name, createdItem);
                                }
                            }
                        }

                        // then combine the dictionaries
                        foreach (KeyValuePair<string, TreeListViewItem> tlvi in addedParents)
                        {
                            streamTree[tlvi.Key] = tlvi.Value;
                        }


                        threadMonitorControl1.Value = 0;

                        int cnt = 0;
                        //int stepCnt = 25;
                        //if (changelists.Count >= 2600)
                        //{
                        //    stepCnt = changelists.Count / 100;
                        //}
                        int stepCnt = 25;
                        if (streams.Count >= 260)
                        {
                            stepCnt = streams.Count / 10;
                        }
                        if (stepCnt > 1000)
                        {
                            stepCnt = 1000;
                        }

                        int sleepTime = 300;
                        try
                        {
                            threadMonitorControl1.CancelPressed = false;

                            TreeListViewItem[] listForDisplay = streamTree.Values.ToArray();


                            for (int idx = 0; idx < streamTree.Count; idx++)
                            {
                                TreeListViewItem item = listForDisplay[idx];
                                if (item.ChildNodes != null && item.ChildNodes.Count > 0)
                                {
                                    item.Expand();
                                    item.Collapse();
                                }
                                if (item.ParentItem != null)
                                {
                                    continue;
                                }

                                if (streamsTreeListView.InvokeRequired)
                                {
                                    streamsTreeListView.Invoke(new StreamsTreeListViewItemDelegate(this.streamsTreeListView.Nodes.Add), item);
                                    streamsTreeListView.Invoke(new StreamsTreeListViewItemDelegate2(this.streamsTreeListView.Items.Add), item);
                                }
                                else
                                {
                                    this.streamsTreeListView.Nodes.Add(item);
                                    this.streamsTreeListView.Items.Add(item);
                                }

                                ++cnt;
                                if ((cnt % stepCnt) == 0)
                                {
                                    threadMonitorControl1.Value = cnt;

                                    sleepTime = Math.Min(250, (cnt * 3) / 2);
                                    Thread.Sleep(sleepTime); // yield to let the progress bar update
                                }
                                if ((!streamsTreeListView.Updating) && (cnt > streamsTreeListView.PageSize + 1))
                                {
                                    threadMonitorControl1.Show(streams.Count);

                                    if (streamsTreeListView.InvokeRequired)
                                    {
                                        streamsTreeListView.Invoke(new StreamsTreeListViewDelegate(streamsTreeListView.BeginUpdate));
                                    }
                                    else
                                    {
                                        streamsTreeListView.BeginUpdate();
                                    }
                                }
                                if (threadMonitorControl1.CancelPressed)
                                {
                                    break;
                                }


                            }
                        }
                        finally
                        {
                            try
                            {
                                if (streamsTreeListView != null && streamsTreeListView.IsHandleCreated && !streamsTreeListView.IsDisposed)
                                {
                                    if (streamsTreeListView.InvokeRequired)
                                    {
                                        streamsTreeListView.Invoke(new StreamsTreeListViewDelegate(streamsTreeListView.EndUpdate));
                                    }
                                    else
                                    {
                                        streamsTreeListView.EndUpdate();
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        threadAborted = true;
                        Thread.ResetAbort();
                    }
                    catch
                    {
                    }
                    finally
                    {
                    }
                    try
                    {
                        if (!threadAborted || threadMonitorControl1.CancelPressed)
                        {
                            //user canceled, not aborted by new request
                            string itemCountStr = Resources.JobsToolWindowControl_1Match;
                            if (streamsCount > 1)
                            {
                                itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches,
                                    streamsCount);
                            }
                            if (streamsCount == 0)
                            {
                                itemCountStr = Resources.JobsToolWindowControl_NoMatches;
                            }
                            if (this.matchesLbl.InvokeRequired)
                            {
                                this.matchesLbl.Invoke(new setStringPropertyDelegate(setStreamsMatchesLblText), itemCountStr);
                            }
                            else
                            {
                                this.matchesLbl.Text = itemCountStr;
                            }
                        }

                        threadMonitorControl1.Hide();
                        FillInListProc = null;
                    }
                    catch { }
                }
            }
            finally
            {
                if (streamsTreeListView.InvokeRequired)
                {
                    this.filterBtn.Invoke(new setFilterBtnDelegate(setFilterBtnBool), true);
                }
                else
                {
                    filterBtn.Enabled = ((Scm != null) && (Scm.Connected));
                }
            }
        }
        Thread FillInListProc = null;

        //public void setFilterButtonEnabled(bool state)
        //{
        //    filterBtn.Enabled = state;
        //}


        private void refreshStreamsList()
        {
            filterBtn.Enabled = false;
            streamsTreeListView.Enabled = false;
            checkConnection();

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

            clearDetails();

            if ((Scm == null) || (Scm.Connection.Disconnected))
            {
                matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
                filterBtn.Enabled = false;
                streamsTreeListView.Enabled = true;
                return;
            }
            DepotFilterText = depotFilterCB.Text;
            OwnerFilterText = ownerFilterCB.Text;
            ParentFilterText = parentFilterCB.Text;
            NameFilterText = nameFilterCB.Text;
            TypeFilterText = typeFilterCB.Text;

            threadMonitorControl1.Visible = false;

            if (Scm != null)
            {
                FillInListProc = new Thread(new ThreadStart(AsyncPopulateListView));
                FillInListProc.IsBackground = true;

                FillInListProc.Start();
            }

            streamsTreeListView.Enabled = true;
            filterBtn.Enabled = true;

            streamsTreeListView.BeforeExpand += new TreeListViewEvent(before);

            SaveControlSettings();

            return;

        }

        private int maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);

        private void filterBtn_Click(object sender, EventArgs e)
        {
            if (streamsTreeListView.InvokeRequired)
            {
                streamsTreeListView.Invoke(new StreamsToolWindowControl.StreamsTreeListViewDelegate(this.streamsTreeListView.Items.Clear));
                streamsTreeListView.Invoke(new StreamsToolWindowControl.StreamsTreeListViewDelegate(this.streamsTreeListView.Nodes.Clear));
            }
            else
            {
                streamsTreeListView.Items.Clear();
                streamsTreeListView.Nodes.Clear();
            }

            maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
            if (maxItems == 0)
            {
                maxItems = -1;
            }
            refreshStreamsList();
        }

        public int refreshStreamObject(TreeListViewItem item)
        {
            List<TreeListViewItem> childNodes = new List<TreeListViewItem>();
            foreach (TreeListViewItem child in item.ChildNodes)
            {
                childNodes.Add(child);
            }
            int indent = item.IndentCount;
            int index = item.Index;
            int treeStateImage = item.treeStateImageIndex;
            bool isExpanded = item.Expanded;
            P4.Stream stream = item.Tag as P4.Stream;
            stream = Scm.GetStream(stream.Id, null, null);
            TreeListViewItem parent = item.ParentItem;

            item = createStreamTreeListViewItem(stream, true);
            item.ChildNodes = childNodes;
            item.Expanded = isExpanded;
            item.treeStateImageIndex = treeStateImage;
            item.IndentCount = indent;
            item.ParentItem = parent;
            updateDetails(stream);

            if (index > -1)
            {
                if (item.ParentItem != null)
                {
                    streamsTreeListView.Items.RemoveAt(index);
                    streamsTreeListView.Items.Insert(index, item);
                    item.ParentItem.ChildNodes.Remove(item);
                    item.ParentItem.ChildNodes.Add(item);
                }
                else
                {
                    streamsTreeListView.Items.RemoveAt(index);
                    streamsTreeListView.Items.Insert(index, item);
                }
            }

            try
            {
                isExpanded = item.Expanded;
                streamsTreeListView.BeginUpdate();
                if (isExpanded)
                {
                    item.Collapse();
                }
                int val = expandStreamObject(item);
                if (isExpanded)
                {
                    item.Expand();
                }
                return val;
            }
            finally
            {
                streamsTreeListView.EndUpdate();
            }
        }

        public int expandStreamObject(TreeListViewItem item)
        {
            if (Scm == null)
            {
                // still null
                return 0;
            }
            return 0;
        }

        public void before(object sender, TreeListViewEventArgs args)
        {
            expandStreamObject(args.Node);
        }

        private void refreshStreamsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (maxItems > (int)Preferences.LocalSettings.GetInt("Number_specs", 100))
            {
                refreshStreamsList();
            }
            else
            {
                maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
                refreshStreamsList();
            }
        }

        private void refreshStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (streamsTreeListView.SelectedItems != null && streamsTreeListView.SelectedItems.Count > 0)
            {
                TreeListViewItem selected = streamsTreeListView.SelectedItems[0] as TreeListViewItem;
                refreshStreamObject(selected);
            }
        }

        private void StreamsToolWindowControl_Load(object sender, EventArgs e)
        {
            streamsTreeListView.Columns[5].Width = -2;
            matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;

            if (Scm != null)
            {
                populateFilters();
            }
            refreshStreamsList();

            refreshStreamsList();

        }

        private void populateFilters()
        {
            _recentStreamOwners = (MRUList)Preferences.LocalSettings["RecentStreamOwners"];
            if (_recentStreamOwners != null)
            {

                foreach (string owner in _recentStreamOwners)
                {
                    if (owner != null)
                    {
                        ownerFilterCB.Items.Add(owner);
                    }
                }
            }

            _recentParentStreams = (MRUList)Preferences.LocalSettings["RecentParentStreams"];
            if (_recentParentStreams != null)
            {

                foreach (string parent in _recentParentStreams)
                {
                    if (parent != null)
                    {
                        parentFilterCB.Items.Add(parent);
                    }
                }
            }

            _recentStreamNames = (MRUList)Preferences.LocalSettings["RecentStreamNames"];
            if (_recentStreamNames != null)
            {

                foreach (string name in _recentStreamNames)
                {
                    if (name != null)
                    {
                        nameFilterCB.Items.Add(name);
                    }
                }
            }

            filterBtn.Enabled = (Scm != null) && Scm.Connected;
            if (Scm != null)
            {
                if (ownerFilterCB.Text == "" &&
                    ownerFilterCB.mruValues[1] == null)
                {
                    selectionChangedByLoad = true;
                    ownerFilterCB.Text = Scm.Connection.User;
                }
                IList<P4.Depot> depots = Scm.GetDepots();

                if (depots != null && depots.Count > 0)
                {
					foreach (P4.Depot depot in depots)
					{
						if (depot.Type == P4.DepotType.Stream)
						{
							depotFilterCB.Items.Add(depot.Id);
						}
					}

					string pref = string.Format("{0}_{1}", PreferenceKey, "last_depot_filter");

                    if (Preferences.LocalSettings.ContainsKey(pref))
                    {
                        lastDepotFilter = Preferences.LocalSettings[pref].ToString();
                    }

                    bool savedDepotFilterLoaded = false;

                    if (lastDepotFilter != null)
                    {
                        foreach (object item in depotFilterCB.Items)
                        {
                            if (item.ToString() == lastDepotFilter)
                            {
                                selectionChangedByLoad = true;
                                depotFilterCB.Text = item.ToString();
                                savedDepotFilterLoaded = true;
                                break;
                            }
                        }
                        if (savedDepotFilterLoaded == false)
                        {
                            selectionChangedByLoad = true;
                            if ((depotFilterCB.Items != null) && (depotFilterCB.Items.Count > 0))
                            {
                                depotFilterCB.Text = depotFilterCB.Items[0].ToString();
                            }
                        }
                    }
                    else if ((depotFilterCB.Items != null) && (depotFilterCB.Items.Count > 0))
                    {
                        depotFilterCB.Text = depotFilterCB.Items[0].ToString();
                    }
                }


                foreach (P4.StreamType streamType in Enum.GetValues(typeof(P4.StreamType)))
                {
                    string dropdownItem = streamType.ToString().ToLower();
                    typeFilterCB.Items.Add(dropdownItem);
                }
                typeFilterCB.Items.Add(string.Empty);

                if (typeFilterCB.Items.Count > 0)
                {
                    string pref = string.Format("{0}_{1}", PreferenceKey, "last_type_filter");

                    if (Preferences.LocalSettings.ContainsKey(pref))
                    {
                        lastTypeFilter = Preferences.LocalSettings[pref].ToString();
                    }
                    bool savedTypeFilterLoaded = false;

                    if (lastTypeFilter != null)
                    {
                        foreach (object item in typeFilterCB.Items)
                        {
                            if (item.ToString() == lastTypeFilter)
                            {
                                selectionChangedByLoad = true;
                                typeFilterCB.Text = item.ToString();
                                savedTypeFilterLoaded = true;
                                break;
                            }
                        }
                        if (savedTypeFilterLoaded == false)
                        {
                            selectionChangedByLoad = true;
                            if ((depotFilterCB.Items != null) && (depotFilterCB.Items.Count > 0))
                            {
                                depotFilterCB.Text = depotFilterCB.Items[0].ToString();
                            }
                        }
                    }
                }

            }
        }
        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (streamsTreeListView.SelectedItems != null && streamsTreeListView.SelectedItems.Count > 0)
            {
                bool changedWS = false;
                P4.Client originalWorkspace = Scm.getClient(Scm.Connection.Workspace, null);
                List<string> source = new List<string>();
                string target = string.Empty;
                TreeListViewItem selected = streamsTreeListView.SelectedItems[0] as TreeListViewItem;
                P4.Stream stream = selected.Tag as P4.Stream;
                P4.Options options = new P4.Options();
                options["-a"] = null;
                P4.StreamMetaData integData = Scm.GetStreamMetaData(stream, options);

                if (integData == null)
                {
                    return;
                }

                selected.firmer = integData.FirmerThanParent;
                if (integData.IntegFromParent)
                {
                    if (integData.IntegFromParentHow ==
                        P4.StreamMetaData.IntegAction.Merge)
                    {
                        source.Add(stream.Parent.Path);
                        target = stream.Id;
                    }
                }
                if (integData.IntegToParent)
                {
                    if (integData.IntegToParentHow ==
                        P4.StreamMetaData.IntegAction.Merge)
                    {
                        source.Add(stream.Id);
                        target = stream.Parent.Path;
                    }
                }
                if (target == null || target == string.Empty)
                {
                    return;
                }

                // before launching the merge dlg, check to see if the user is in the
                // correct stream workspace
                string streamName = string.Empty;

                P4.Client workspace = originalWorkspace;
                if (workspace.Stream != target)
                {
                    // let the user know they either need to switch to or create an appropriate
                    // workspace to complete the merge operation

                    IList<P4.Client> workspaces = Scm.getClients(P4.ClientsCmdFlags.None,
                        Scm.Connection.User, null, -1, target);
                    IList<P4.Client> validWorkspaces = new List<P4.Client>();

                    if (workspaces != null)
                    {
                        foreach (P4.Client client in workspaces)
                        {
                            if ((client.Host == string.Empty) ||
                (string.Equals(client.Host.ToString(),
                               Dns.GetHostName(),
                               StringComparison.CurrentCultureIgnoreCase)) == true)
                            {
                                validWorkspaces.Add(client);
                            }
                        }
                    }

                    if (validWorkspaces == null || validWorkspaces.Count < 1)
                    {
                        // set the user up to create a workspace
                        StreamsWorkspaceSwitchCreate dlg = new StreamsWorkspaceSwitchCreate(
                            Resources.StreamWorkspaceCreateTitle, string.Empty,
                            Resources.StreamWorkspaceCreateText, "merge");
                        if (DialogResult.Cancel != dlg.ShowDialog())
                        {
                            P4.Client createdWS = newWorkspace(target);
                            if (createdWS != null)
                            {
                                // switch the workspace and change connection
                                Scm.Connection.Workspace = createdWS.Name;
                                Scm.Connection.Connect(true, null);
                                changedWS = true;
                            }
                            else { return; }
                        }
                        else
                        {
                            return;
                        }
                    }
                    if (validWorkspaces != null && validWorkspaces.Count == 1)
                    {
                        //prompt the user to change to the only available workspace
                        StreamsWorkspaceSwitchCreate dlg = new StreamsWorkspaceSwitchCreate(
                            Resources.StreamWorkspaceSwitchTitle, validWorkspaces[0].Name,
                            Resources.StreamWorkspaceSwitchText, "merge");
                        if (DialogResult.Cancel != dlg.ShowDialog())
                        {
                            // switch the workspace and change connection
                            Scm.Connection.Workspace = validWorkspaces[0].Name;
                            Scm.Connection.Connect(true, null);
                            changedWS = true;
                        }
                        else { return; }
                    }
                    if (validWorkspaces != null && validWorkspaces.Count > 1)
                    {
                        // set up the user to select workspace from browser
                        WorkspacesBrowserDlg dlg = new WorkspacesBrowserDlg(Scm, target, "merge", validWorkspaces);

                        if (DialogResult.Cancel != dlg.ShowDialog())
                        {
                            if ((dlg.SelectedWorkspace != null) && (dlg.SelectedWorkspace.Name != null))
                            {
                                // switch the workspace and change connection
                                Scm.Connection.Workspace = dlg.SelectedWorkspace.Name;
                                Scm.Connection.Connect(true, null);
                                changedWS = true;
                            }
                            else { return; }
                        }
                        else { return; }
                    }
                }

                IList<P4.FileSpec> merge = IntegrateDlg.Show(source, target, "merge", Scm);

                // if the merge was successful, offer to submit the files or
                // save them in a numbered pending changelist

                // THIS IS ALL DONE IN THE INTEG ACTION BUTTON NOW

                //if (merge != null && merge.Count > 0)
                //{
                //    IList<string> files = new List<string>();
                //    foreach(P4.FileSpec f in merge)
                //    {
                //        files.Add(f.DepotPath.Path);
                //    }

                //    SubmitDlg.SubmitFiles(files, Scm, false);
                //}

                if (changedWS)
                {
                    // switch back to the original workspace and change connection
                    Scm.Connection.Workspace = originalWorkspace.Name;
                    Scm.Connection.Connect(true, null);
                    changedWS = false;
                }
                // refresh the selected item to update the merge/copy icons
                refreshStreamObject(selected);
            }

        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (streamsTreeListView.SelectedItems != null && streamsTreeListView.SelectedItems.Count > 0)
            {
                bool changedWS = false;
                P4.Client originalWorkspace = Scm.getClient(Scm.Connection.Workspace, null);
                List<string> source = new List<string>();
                string target = string.Empty;
                TreeListViewItem selected = streamsTreeListView.SelectedItems[0] as TreeListViewItem;
                P4.Stream stream = selected.Tag as P4.Stream;
                P4.Options options = new P4.Options();
                options["-a"] = null;
                P4.StreamMetaData integData = Scm.GetStreamMetaData(stream, options);

                if (integData == null)
                {
                    return;
                }

                selected.firmer = integData.FirmerThanParent;
                if (integData.IntegFromParent)
                {
                    if (integData.IntegFromParentHow ==
                        P4.StreamMetaData.IntegAction.Copy)
                    {
                        source.Add(stream.Parent.Path);
                        target = stream.Id;
                    }
                }
                if (integData.IntegToParent)
                {
                    if (integData.IntegToParentHow ==
                        P4.StreamMetaData.IntegAction.Copy)
                    {
                        source.Add(stream.Id);
                        target = stream.Parent.Path;
                    }
                }
                if (target == null || target == string.Empty)
                {
                    return;
                }
                // before launching the copy dlg, check to see if the user is in the
                // correct stream workspace
                string streamName = string.Empty;

                P4.Client workspace = originalWorkspace;
                if (workspace.Stream != target)
                {
                    // let the user know they either need to switch to or create an appropriate
                    // workspace to complete the copy operation

                    IList<P4.Client> workspaces = Scm.getClients(P4.ClientsCmdFlags.None,
                        Scm.Connection.User, null, -1, target);

                    IList<P4.Client> validWorkspaces = new List<P4.Client>();

                    if (workspaces != null)
                    {
                        foreach (P4.Client client in workspaces)
                        {
                            if ((client.Host == string.Empty) ||
                (string.Equals(client.Host.ToString(),
                               Dns.GetHostName(),
                               StringComparison.CurrentCultureIgnoreCase)) == true)
                            {
                                validWorkspaces.Add(client);
                            }
                        }
                    }

                    if (validWorkspaces == null || validWorkspaces.Count < 1)
                    {
                        // set the user up to create a workspace
                        StreamsWorkspaceSwitchCreate dlg = new StreamsWorkspaceSwitchCreate(
                            Resources.StreamWorkspaceCreateTitle, string.Empty,
                            Resources.StreamWorkspaceCreateText, "copy");
                        if (DialogResult.Cancel != dlg.ShowDialog())
                        {
                            P4.Client createdWS = newWorkspace(target);
                            if (createdWS != null)
                            {
                                // switch the workspace and change connection
                                Scm.Connection.Workspace = createdWS.Name;
                                Scm.Connection.Connect(true, null);
                                changedWS = true;
                            }
                            else { return; }
                        }
                        else { return; }
                    }
                    if (validWorkspaces != null && validWorkspaces.Count == 1)
                    {
                        //prompt the user to change to the only available workspace
                        StreamsWorkspaceSwitchCreate dlg = new StreamsWorkspaceSwitchCreate(
                            Resources.StreamWorkspaceSwitchTitle, validWorkspaces[0].Name,
                            Resources.StreamWorkspaceSwitchText, "copy");
                        if (DialogResult.Cancel != dlg.ShowDialog())
                        {
                            // switch the workspace and change connection
                            Scm.Connection.Workspace = validWorkspaces[0].Name;
                            Scm.Connection.Connect(true, null);
                            changedWS = true;
                        }
                        else { return; }
                    }
                    if (validWorkspaces != null && validWorkspaces.Count > 1)
                    {
                        // set up the user to select workspace from browser
                        WorkspacesBrowserDlg dlg = new WorkspacesBrowserDlg(Scm, target, "copy", validWorkspaces);

                        if (DialogResult.Cancel != dlg.ShowDialog())
                        {
                            if ((dlg.SelectedWorkspace != null) && (dlg.SelectedWorkspace.Name != null))
                            {
                                // switch the workspace and change connection
                                Scm.Connection.Workspace = dlg.SelectedWorkspace.Name;
                                Scm.Connection.Connect(true, null);
                                changedWS = true;
                            }
                            else { return; }
                        }
                        else { return; }
                    }
                }

                IList<P4.FileSpec> copy = IntegrateDlg.Show(source, target, "copy", Scm);

                // if the copy was successful, offer to submit the files or
                // save them in a numbered pending changelist

                // THIS IS ALL DONE IN THE INTEG ACTION BUTTON NOW

                //if (copy != null && copy.Count > 0)
                //{
                //    IList<string> files = new List<string>();
                //    foreach (P4.FileSpec f in copy)
                //    {
                //        files.Add(f.DepotPath.Path);
                //    }

                //    SubmitDlg.SubmitFiles(files, Scm, false);
                //}

                if (changedWS)
                {
                    // switch back to the original workspace and change connection
                    Scm.Connection.Workspace = originalWorkspace.Name;
                    Scm.Connection.Connect(true, null);
                    changedWS = false;
                }
                // refresh the selected item to update the merge/copy icons
                refreshStreamObject(selected);
            }
        }

        // create new streams workspace
        private P4.Client newWorkspace(string stream)
        {
            string newName = GetStringDlg.Show(Resources.WorkspacesWindowControl_NewWorkspace,
                Resources.WorkspacesWindowControl_EnterNameForNewWorkspace, null);
            if ((newName != null) && (newName != string.Empty))
            {
                if (newName.Contains(" "))
                {
                    MessageBox.Show(Resources.NewUserDlg_NameContainsSpacesWarning);
                    return newWorkspace(stream);
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
                        // add the stream
                        clientInfo.Stream = stream;
                        P4.Client workspace = DlgEditWorkspace.EditWorkspace(Scm, clientInfo);

                        if (workspace != null)
                        {
                            return workspace;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    MessageBox.Show(
                        string.Format(Resources.WorkspacesWindowControl_WorkspaceExistsWarning, newName),
                        Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return newWorkspace(stream);
                }

            }
            else
            {
                if (newName == string.Empty)
                {
                    MessageBox.Show(Resources.WorkspacesWindowControl_EmptyNameWarning,
                        Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return newWorkspace(stream);
                }
                return null;
            }
        }

        public void initData()
        {
            if (Scm == null)
            {
                P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
                if (P4VSService != null) Scm = P4VSService.ScmProvider;
            }
            if (Scm == null)
            {
                // still null
                return;
            }
            //userCB.Text = Scm.Repository.Connection.UserName;
            //workspaceCB.Text = Scm.Repository.Connection.Client.Name;
            //filterBtn_Click(null, null);
        }

        private void streamsTreeListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkConnection();

            //if (streamsTreeListView.SelectedItems != null && streamsTreeListView.SelectedItems.Count > 0)
            //{
            //    TreeListViewItem whatItem = streamsTreeListView.SelectedItems[0] as TreeListViewItem;
            //    if (whatItem != null)
            //    {
            //        P4.Stream stream = streamsTreeListView.SelectedItems[0].Tag as P4.Stream;
            //        SelectedStream = stream;
            //    }
            //}

            P4.Stream stream = new P4.Stream();

            if (streamsTreeListView.SelectedItems != null && streamsTreeListView.SelectedItems.Count > 0)
            {
                TreeListViewItem selected = streamsTreeListView.SelectedItems[0] as TreeListViewItem;
                string id = null;

                if (selected != null)
                {
                    stream = selected.Tag as P4.Stream;
                    id = selected.Text;
                }

                if (streamTB.Text == id)
                { return; }
                updateDetails(stream);
            }

        }

        private P4.Stream streamInfo = null;

        public void updateDetails(P4.Stream stream)
        {
            if (stream == null)
            {
                return;
            }
            clearDetails();

            if (Scm == null)
            {
                // still null
                return;
            }
            P4.Options opts = new P4.Options();
            opts["-v"] = null;
            opts["-o"] = null;
            streamInfo = Scm.GetStream(stream.Id, null, opts);
            if (streamInfo == null)
            {
                return;
            }

            if (streamInfo.Name != null)
            { streamTB.Text = streamInfo.Name; }

            streamTB.Refresh();

            if (Preferences.LocalSettings.GetBool("P4Date_format", true))
            {
                modifiedTB.Text = streamInfo.Updated.ToString("yyyy/MM/dd HH:mm:ss");
                accessedTB.Text = streamInfo.Accessed.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                modifiedTB.Text = streamInfo.Updated.ToString();
                accessedTB.Text = streamInfo.Accessed.ToString();
            }

            modifiedTB.Refresh();
            accessedTB.Refresh();

            ownerTB.Text = streamInfo.OwnerName;
            ownerTB.Refresh();

            streamRootTB.Text = streamInfo.Id;
            streamRootTB.Refresh();

            parentTB.Text = streamInfo.Parent.Path;
            parentTB.Refresh();

            typeTB.Text = streamInfo.Type.ToString();
            typeTB.Refresh();

            descriptionTB.Text = streamInfo.Description;
            descriptionTB.Refresh();


            submittingChk.Checked = ((streamInfo.Options & P4.StreamOption.OwnerSubmit) != 0);

            lockedChk.Checked = ((streamInfo.Options & P4.StreamOption.Locked) != 0);

            toParentChk.Checked = ((streamInfo.Options & P4.StreamOption.NoToParent) == 0);

            fromParentChk.Checked = ((streamInfo.Options & P4.StreamOption.NoFromParent) == 0);

            if (streamInfo.Paths != null)
            {
                string type = string.Empty;
                foreach (P4.MapEntry entry in streamInfo.Paths)
                {
                    if (entry.Type == P4.MapType.StreamPathExclude)
                    {
                        type = "exclude";
                    }
                    if (entry.Type == P4.MapType.Share)
                    {
                        type = "share";
                    }
                    if (entry.Type == P4.MapType.Import)
                    {
                        type = "import";
                    }
                    if (entry.Type == P4.MapType.Isolate)
                    {
                        type = "isolate";
                    }
                    pathsTB.AppendText(type + " " + entry.Left.Path.ToString() + " " + entry.Right.Path.ToString() + "\r\n");
                }
            }

            if (streamInfo.Remapped != null)
            {
                string type = string.Empty;
                foreach (P4.MapEntry entry in streamInfo.Remapped)
                {
                    if (entry.Type == P4.MapType.Exclude)
                    {
                        type = "-";
                    }
                    if (entry.Type == P4.MapType.Overlay)
                    {
                        type = "+";
                    }
                    remappedTB.AppendText(type + entry.Left.Path.ToString() + " " + entry.Right.Path.ToString() + "\r\n");
                }
            }

            if (streamInfo.Ignored != null)
            {
                string type = string.Empty;
                foreach (P4.MapEntry entry in streamInfo.Ignored)
                {
                    if (entry.Type == P4.MapType.Exclude)
                    {
                        type = "-";
                    }
                    if (entry.Type == P4.MapType.Overlay)
                    {
                        type = "+";
                    }
                    ignoredTB.AppendText(type + entry.Left.Path.ToString() + " " + entry.Right.Path.ToString() + "\r\n");
                }
            }

            if (streamInfo.View != null)
            {
                string type = string.Empty;
                foreach (P4.MapEntry entry in streamInfo.View)
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

            string test = Scm.Connection.Repository.Connection.LastResults.TaggedOutput.ToString();



            //accessedTB.Text = changeInfo.ClientId;
            //remappedTB.Text = changeInfo.Description;
            //remappedTB.Refresh();
            //streamRootTB.Text = changeInfo.OwnerName;
            //streamRootTB.Refresh();

            //if (changeInfo.Id <= 0)
            //{ parentTB.Text = "default"; }
            //else
            //{ parentTB.Text = "submitted"; }
            //parentTB.Refresh();

            //if (changeInfo.Id > 0)
            //{ typeTB.Text = changeInfo.Type.ToString(); }
            //else
            //{ typeTB.Text = string.Empty; }
            //typeTB.Refresh();

            //if (changeInfo.Jobs != null)
            //{
            //    foreach (string job in changeInfo.Jobs.Keys)
            //    {
            //        ignoredTB.Text += job + "\r\n";
            //    }
            //}
            //ignoredTB.Refresh();

            //if (changeInfo.Files != null)
            //{
            //    StringBuilder sb = new StringBuilder(changeInfo.Files.Count * 260);
            //    foreach (P4.FileMetaData file in changeInfo.Files)
            //    {
            //        sb.AppendLine(string.Format("{0}#{1}", file.DepotPath.Path, file.HeadRev));
            //    }
            //    viewTB.Text = sb.ToString();
            //}
            //viewTB.Refresh();
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
                streamsTreeListView.Nodes.Clear();
                streamsTreeListView.BuildTreeList();
                streamContextMenuStrip.Enabled = false;
                matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
                matchesLbl.Refresh();
                return;
            }
            filterBtn.Enabled = true;
            streamContextMenuStrip.Enabled = true;
        }

        #region IDisposable Members

        public new void Dispose()
        {
            P4VsProvider.NewConnection -= newConection;
            base.Dispose();
        }

        #endregion

        private void streamsTreeListView_onMaxScroll(object sender, ScrollEventArgs e)
        {
            if ((maxItems > 0) && (streamsTreeListView.Nodes.Count >= maxItems))
            {
                maxItems += (int)Preferences.LocalSettings.GetInt("Number_specs", 100); ;
                refreshStreamsList();
            }
        }

        private void filterBtn_EnabledChanged(object sender, EventArgs e)
        {
            if ((filterBtn.Enabled == false) && ((Scm == null) ||
                (Scm.Connection.Disconnected)))
            {
                matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
            }
            else if ((filterBtn.Enabled == false) && (Scm != null) &&
                (Scm.Connected))
            {
                matchesLbl.Text = "";
            }
        }

        private void streamsTreeListView_Click(object sender, EventArgs e)
        {
            checkConnection();
        }

        private void streamsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            mergeToolStripMenuItem.Enabled = false;
            mergeToolStripMenuItem.Visible = false;
            copyToolStripMenuItem.Enabled = false;
            copyToolStripMenuItem.Visible = false;
            refreshStreamToolStripMenuItem.Enabled = false;
            refreshStreamToolStripMenuItem.Visible = false;
            refreshStreamsListToolStripMenuItem.Visible = false;
            refreshStreamsListToolStripMenuItem.Enabled = false;
            if (fromBrowser)
            {
                e.Cancel = true;
            }
            else
            {
                if (streamsTreeListView.SelectedItems != null && streamsTreeListView.SelectedItems.Count > 0)
                {
                    TreeListViewItem selected = streamsTreeListView.SelectedItems[0]
                        as TreeListViewItem;
                    P4.Stream stream = selected.Tag as P4.Stream;
                    if (selected.action1 == 3)
                    {
                        if (selected.firmer)
                        {
                            mergeToolStripMenuItem.Text =
                                string.Format(Resources.StreamsToolWindow_MergeToParentStreamMenuText, stream.Name);
                        }
                        else
                        {
                            mergeToolStripMenuItem.Text =
                                string.Format(Resources.StreamsToolWindow_MergeFromParentStreamMenuText, stream.Name);
                        }
                        mergeToolStripMenuItem.Enabled = true;
                        mergeToolStripMenuItem.Visible = true;
                    }
                    else
                    {
                        mergeToolStripMenuItem.Text =
                            string.Format(Resources.StreamsToolWindow_NothingToMergeMenuText, stream.Name);
                        mergeToolStripMenuItem.Enabled = false;
                        mergeToolStripMenuItem.Visible = true;
                    }

                    if (selected.action2 == 7)
                    {
                        if (selected.firmer)
                        {
                            copyToolStripMenuItem.Text =
                                string.Format(Resources.StreamsToolWindow_CopyFromParentStreamMenuText, stream.Name);
                        }
                        else
                        {
                            copyToolStripMenuItem.Text =
                                string.Format(Resources.StreamsToolWindow_CopyToParentStreamMenuText, stream.Name);
                        }
                        copyToolStripMenuItem.Enabled = true;
                        copyToolStripMenuItem.Visible = true;
                    }
                    else if (selected.action2 == 9)
                    {
                        copyToolStripMenuItem.Text =
                            string.Format(Resources.StreamsToolWindow_MergeBeforeCopyStreamMenuText, stream.Name);
                        copyToolStripMenuItem.Enabled = false;
                        copyToolStripMenuItem.Visible = true;
                    }
                    else
                    {
                        copyToolStripMenuItem.Text =
                            string.Format(Resources.StreamsToolWindow_NothingToCopyMenuText, stream.Name);
                        copyToolStripMenuItem.Enabled = false;
                        copyToolStripMenuItem.Visible = true;
                    }

                    refreshStreamToolStripMenuItem.Text =
                            string.Format(Resources.StreamsToolWindow_RefreshStreamMenuText, stream.Name);
                    refreshStreamToolStripMenuItem.Enabled = true;
                    refreshStreamToolStripMenuItem.Visible = true;
                    refreshStreamsListToolStripMenuItem.Visible = true;
                    refreshStreamsListToolStripMenuItem.Enabled = true;
                }
                refreshStreamsListToolStripMenuItem.Visible = true;
                refreshStreamsListToolStripMenuItem.Enabled = true;
                e.Cancel = false;
            }
        }

        private void streamsTreeListView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            if (e.NewDisplayIndex == 0 | e.OldDisplayIndex == 0)
            {
                e.Cancel = true;
            }
        }

        private void streamsTreeListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //// Determine if clicked column is already the column that is being sorted.
            //if (e.Column == lvwColumnSorter.SortColumn)
            //{
            //    // Reverse the current sort direction for this column.
            //    if (lvwColumnSorter.Order == SortOrder.Ascending)
            //    {
            //        lvwColumnSorter.Order = SortOrder.Descending;
            //    }
            //    else
            //    {
            //        lvwColumnSorter.Order = SortOrder.Ascending;
            //    }
            //}
            //else
            //{
            //    // set the header text to what it already is to force a
            //    // redraw of the previously selected column header.
            //    streamsTreeListView.Columns[lvwColumnSorter.SortColumn].Text =
            //        streamsTreeListView.Columns[lvwColumnSorter.SortColumn].Text;

            //    // Set the column number that is to be sorted; default to ascending.

            //    lvwColumnSorter.SortColumn = e.Column;
            //    lvwColumnSorter.Order = SortOrder.Ascending;
            //}

            //// Perform the sort with these new sort options.
            //this.streamsTreeListView.Sort();
        }

        public event MouseEventHandler TreeListViewDoubleClicked;

        private void streamsTreeListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (TreeListViewDoubleClicked != null)
            {
                TreeListViewDoubleClicked(sender, e);
            }
        }

        private void depotFilterCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectionChangedByLoad == false)
            {
                filterBtn.PerformClick();
            }
            if (PreferenceKey != null)
            {
                string pref = string.Format("{0}_{1}", PreferenceKey, "last_depot_filter");
                Preferences.LocalSettings[pref] = depotFilterCB.Text;
            }
            selectionChangedByLoad = false;
        }

        private void depotFilterCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                filterBtn.PerformClick();
        }

        private void depotFilterCB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar == (char)Keys.Return))
            {
                e.Handled = true;
            }
        }

        private void ownerFilterCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectionChangedByLoad == false)
            {
                filterBtn.PerformClick();
            }
            selectionChangedByLoad = false;
        }

        private void ownerFilterCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                filterBtn.PerformClick();
        }

        private void nameFilterCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectionChangedByLoad == false)
            {
                filterBtn.PerformClick();
            }
            selectionChangedByLoad = false;
        }

        private void nameFilterCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                filterBtn.PerformClick();
        }

        private void parentFilterCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectionChangedByLoad == false)
            {
                filterBtn.PerformClick();
            }
            selectionChangedByLoad = false;
        }

        private void parentFilterCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                filterBtn.PerformClick();
        }

        private void typeFilterCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectionChangedByLoad == false)
            {
                filterBtn.PerformClick();
            }
            if (PreferenceKey != null)
            {
                string pref = string.Format("{0}_{1}", PreferenceKey, "last_type_filter");
                Preferences.LocalSettings[pref] = typeFilterCB.Text;
            }

            selectionChangedByLoad = false;
        }

        private void typeFilterCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                filterBtn.PerformClick();
        }

        private void typeFilterCB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar == (char)Keys.Return))
            {
                e.Handled = true;
            }
        }

        private void matchesLbl_SizeChanged(object sender, EventArgs e)
        {
            gridLayoutPanel1.LayoutGrid();
        }

        bool in_submittingChk = false;

        private void submittingChk_CheckedChanged(object sender, EventArgs e)
        {
            if (in_submittingChk)
            {
                return;
            }
            try
            {
                in_submittingChk = true;

                if ((streamInfo == null) && (submittingChk.Checked != false))
                {
                    submittingChk.Checked = false;
                }
                else if ((streamInfo != null) && (submittingChk.Checked != ((streamInfo.Options & P4.StreamOption.OwnerSubmit) != 0)))
                {
                    submittingChk.Checked = ((streamInfo.Options & P4.StreamOption.OwnerSubmit) != 0);
                }

            }
            finally
            {
                in_submittingChk = false;
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

                if ((streamInfo == null) && (lockedChk.Checked != false))
                {
                    lockedChk.Checked = false;
                }
                else if ((streamInfo != null) && (lockedChk.Checked != ((streamInfo.Options & P4.StreamOption.Locked) != 0)))
                {
                    lockedChk.Checked = ((streamInfo.Options & P4.StreamOption.Locked) != 0);
                }

            }
            finally
            {
                in_lockedChk_CheckedChanged = false;
            }
        }

        bool in_toParentChk_CheckedChanged = false;

        private void toParentChk_CheckedChanged(object sender, EventArgs e)
        {
            if (in_toParentChk_CheckedChanged)
            {
                return;
            }
            try
            {
                in_toParentChk_CheckedChanged = true;

                if ((streamInfo == null) && (toParentChk.Checked != false))
                {
                    toParentChk.Checked = false;
                }
                else if ((streamInfo != null) && (toParentChk.Checked != ((streamInfo.Options & P4.StreamOption.NoToParent) != 0)))
                {
                    toParentChk.Checked = ((streamInfo.Options & P4.StreamOption.NoToParent) != 0);
                }

            }
            finally
            {
                in_toParentChk_CheckedChanged = false;
            }
        }

        bool in_fromParentChk_CheckedChanged = false;

        private void fromParentChk_CheckedChanged(object sender, EventArgs e)
        {
            if (in_fromParentChk_CheckedChanged)
            {
                return;
            }
            try
            {
                in_fromParentChk_CheckedChanged = true;

                if ((streamInfo == null) && (fromParentChk.Checked != false))
                {
                    fromParentChk.Checked = false;
                }
                else if ((streamInfo != null) && (fromParentChk.Checked != ((streamInfo.Options & P4.StreamOption.NoFromParent) != 0)))
                {
                    fromParentChk.Checked = ((streamInfo.Options & P4.StreamOption.NoFromParent) != 0);
                }

            }
            finally
            {
                in_fromParentChk_CheckedChanged = false;
            }
        }
    }
}
