
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

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

using Perforce.SwarmApi;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	/// <summary>
	/// Summary description for P4ToolWindowControl.
	/// </summary>
	public class SubmittedChangelistsToolWindowControl : P4ToolWindowControlBase
	{
        private I18nControls.GridLayoutPanel panel1;
		private SplitContainer splitContainer1;
        private I18nControls.GridP4ObjectTreeListView submittedTreeListView;
		private I18nControls.GridLayoutPanel panel2;
        private I18nControls.GridLabel filesLbl;
        private I18nControls.GridLabel jobsLbl;
        private I18nControls.GridTextBox descriptionTB;
        private I18nControls.GridLabel descriptionLbl;
        private I18nControls.GridTextBox filesTB;
        private I18nControls.GridTextBox jobsTB;
		private ContextMenuStrip changelistContextMenuStrip;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem getRevisionToolStripMenuItem;
		private ToolStripMenuItem diffAgainstToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem editSubmittedChangelistToolStripMenuItem;
		private ToolStripMenuItem refreshSubmittedChangelistListToolStripMenuItem;
		private ToolStripMenuItem refreshSubmittedChangelistToolStripMenuItem;
        private ToolStripMenuItem removeJobToolStripMenuItem;
        private I18nControls.GridTextBox typeTB;
        private I18nControls.GridTextBox statusTB;
        private I18nControls.GridTextBox UserTB2;
        private I18nControls.GridLabel typeLbl;
        private I18nControls.GridLabel userLbl2;
        private I18nControls.GridLabel statusLbl;
        private I18nControls.GridTextBox workspaceTB2;
        private I18nControls.GridTextBox dateTB;
        private I18nControls.GridLabel workspaceLbl2;
        private I18nControls.GridLabel dateLbl;
        private I18nControls.GridTextBox changeTB;
        private I18nControls.GridLabel changeLbl;
        private UI.ThreadMonitorControl threadMonitorControl1;
        private I18nControls.GridLabel workspaceLbl;
        private I18nControls.GridLabel userLbl;
        private I18nControls.GridLabel pathLbl;
		private ColumnHeader ClmHdr_Change;
		private ColumnHeader ClmHdr_Date;
		private ColumnHeader ClmHdr_Workspace;
		private ColumnHeader ClmHdr_User;
		private ColumnHeader ClmHdr_AccessType;
		private ColumnHeader ClmHdr_Description;
        private ListViewColumnSorter lvwColumnSorter;

		private IContainer components;
        private I18nControls.GridFilterComboBox workspaceCB;
        private I18nControls.GridFilterComboBox userCB;
        internal I18nControls.GridFilterComboBox pathCB;

        //private MRUList _recentSubmittedPaths = null;
        //private MRUList _recentSubmittedUsers = null;
        //private MRUList _recentSubmittedWorkspaces = null;

		public bool fromBrowser { get; set; }

		//private ImageList imageList1;
        public I18nControls.GridButton filterBtn;
        private I18nControls.GridGroupBox gridGroupBox1;
        private I18nControls.GridLabel matchesLbl;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem requestReviewToolStripMenuItem;
		private ToolStripMenuItem updateReviewToolStripMenuItem;
		private ColumnHeader ClmHdr_ReviewStatus;
		private ToolStripMenuItem openReviewInSwarmToolStripMenuItem;
		private ColumnHeader ClmHdr_ReviewId;
	    private bool selectionChangedByLoad = false;
		private ToolStripMenuItem fileHistoryStripMenuItem;
		private ToolStripMenuItem timeLapseViewtoolStripMenuItem;
		List<ColumnHeader> DefaultListColumns = null; //submittedTreeListView.Columns;

		public SubmittedChangelistsToolWindowControl()
		{
            PreferenceKey = "SubmittedChangelistsToolWindowControl";
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
            base.Initialize();

            // if the mru lists have not been loaded, see if the old preference exists
            if (Preferences.LocalSettings != null)
            {
                if ((pathCB.mruLoaded == false) && (Preferences.LocalSettings.ContainsKey("RecentSubmittedPaths")))
                {
                    MRUList value = (MRUList) Preferences.LocalSettings["RecentSubmittedPaths"];
                    if (value != null)
                    {
                        pathCB.mruValues = value.Clone();
                        pathCB.mruLoaded = true;
                    }
                }
                if ((userCB.mruLoaded == false) && (Preferences.LocalSettings.ContainsKey("RecentSubmittedUsers")))
                {
                    MRUList value = (MRUList) Preferences.LocalSettings["RecentSubmittedUsers"];
                    if (value != null)
                    {
                        userCB.mruValues = value.Clone();
                        userCB.mruLoaded = true;
                    }
                }
                if ((workspaceCB.mruLoaded == false) &&
                    (Preferences.LocalSettings.ContainsKey("RecentSubmittedWorkspaces")))
                {
                    MRUList value = (MRUList) Preferences.LocalSettings["RecentSubmittedWorkspaces"];
                    if (value != null)
                    {
                        workspaceCB.mruValues = value.Clone();
                        workspaceCB.mruLoaded = true;
                    }
                }
            }
            //this.imageList1 = new System.Windows.Forms.ImageList(this.components);

			// 
			// imageList1
			// 
            //this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            //this.imageList1.Images.Add("submitted_change_icon.png", Images.submitted_change_icon);
            //this.imageList1.Images.Add("portrait.png", Images.portrait);
            //this.imageList1.Images.Add("jobs_icon.png", Images.jobs_icon);
            //this.imageList1.Images.Add("shelve_icon_add.png", Images.shelve_icon_add);
            //this.imageList1.Images.Add("shelve_icon_archive.png", Images.shelve_icon_archive);
            //this.imageList1.Images.Add("shelve_icon_base.png", Images.shelve_icon_base);
            //this.imageList1.Images.Add("shelve_icon_branch.png", Images.shelve_icon_branch);
            //this.imageList1.Images.Add("shelve_icon_delete.png", Images.shelve_icon_delete);
            //this.imageList1.Images.Add("shelve_icon_edit.png", Images.shelve_icon_edit);
            //this.imageList1.Images.Add("shelve_icon_integrate.png", Images.shelve_icon_integrate);
            //this.imageList1.Images.Add("shelve_icon_moveadd.png", Images.shelve_icon_moveadd);
            //this.imageList1.Images.Add("shelve_icon_movedelete.png", Images.shelve_icon_movedelete);
            //this.imageList1.Images.Add("shelve_icon_purge.png", Images.shelve_icon_purge);

            //this.submittedTreeListView.LargeImageList = this.imageList1;
            //this.submittedTreeListView.SmallImageList = this.imageList1;
			
			// Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            this.submittedTreeListView.ListViewItemSorter = lvwColumnSorter;

			if (Scm != null)
			{
                userCB.Text = Scm.Connection.User;
                workspaceCB.Text = Scm.Connection.Workspace;
			}

			PathFilterText = pathCB.Text;
			UserFilterText = userCB.Text;
			WorkspaceFilterText = workspaceCB.Text;
			threadMonitorControl1.Visible = false;

			// store the full colum header list for later use
			DefaultListColumns = new List<ColumnHeader>();
			foreach (ColumnHeader h in submittedTreeListView.Columns)
			{
				DefaultListColumns.Add(h);
			}

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

        public SubmittedChangelistsToolWindowControl(P4ScmProvider scm)
            :base(scm)
		{
			Scm = scm;
            PreferenceKey = "SubmittedChangelistsToolWindowControl";
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
            base.Initialize();

            //this.imageList1 = new System.Windows.Forms.ImageList(this.components);

			// 
			// imageList1
			// 
            //this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            //this.imageList1.Images.Add("submitted_change_icon.png", Images.submitted_change_icon);//0
            //this.imageList1.Images.Add("portrait.png", Images.portrait);//1
            //this.imageList1.Images.Add("jobs_icon.png", Images.jobs_icon);//2
            //this.imageList1.Images.Add("shelve_icon_add.png", Images.shelve_icon_add);//3
            //this.imageList1.Images.Add("shelve_icon_archive.png", Images.shelve_icon_archive);//4
            //this.imageList1.Images.Add("shelve_icon_base.png", Images.shelve_icon_base);//5
            //this.imageList1.Images.Add("shelve_icon_branch.png", Images.shelve_icon_branch);//6
            //this.imageList1.Images.Add("shelve_icon_delete.png", Images.shelve_icon_delete);//7
            //this.imageList1.Images.Add("shelve_icon_edit.png", Images.shelve_icon_edit);//8
            //this.imageList1.Images.Add("shelve_icon_integrate.png", Images.shelve_icon_integrate);//9
            //this.imageList1.Images.Add("shelve_icon_moveadd.png", Images.shelve_icon_moveadd);//10
            //this.imageList1.Images.Add("shelve_icon_movedelete.png", Images.shelve_icon_movedelete);//11
            //this.imageList1.Images.Add("shelve_icon_purge.png", Images.shelve_icon_purge);//12
            //this.imageList1.Images.Add("submitted_change_review_icon.png", Images.submitted_change_review_icon);//13

            //this.submittedTreeListView.LargeImageList = this.imageList1;
            //this.submittedTreeListView.SmallImageList = this.imageList1;

			PathFilterText = pathCB.Text;
			UserFilterText = userCB.Text;
			WorkspaceFilterText = workspaceCB.Text;
			threadMonitorControl1.Visible = false;

			newConection = new P4VsProvider.NewConnectionDelegate(OnNewConnection);
			P4VsProvider.NewConnection += newConection;

            //_recentSubmittedPaths = (MRUList)Preferences.LocalSettings["RecentSubmittedPaths"];
            //if (_recentSubmittedPaths != null)
            //{

            //    foreach (string path in _recentSubmittedPaths)
            //    {
            //        if (path != null)
            //        {
            //            pathCB.Items.Add(path);
            //        }
            //    }
            //}

            //_recentSubmittedUsers = (MRUList)Preferences.LocalSettings["RecentSubmittedUsers"];
            //if (_recentSubmittedUsers != null)
            //{

            //    foreach (string user in _recentSubmittedUsers)
            //    {
            //        if (user != null)
            //        {
            //            userCB.Items.Add(user);
            //        }
            //    }
            //}

            //_recentSubmittedWorkspaces = (MRUList)Preferences.LocalSettings["RecentSubmittedWorkspaces"];
            //if (_recentSubmittedWorkspaces != null)
            //{

            //    foreach (string workspace in _recentSubmittedWorkspaces)
            //    {
            //        if (workspace != null)
            //        {
            //            workspaceCB.Items.Add(workspace);
            //        }
            //    }
            //}

			// store the full colum header list for later use
			DefaultListColumns = new List<ColumnHeader>();
			foreach(ColumnHeader h in submittedTreeListView.Columns)
			{
				DefaultListColumns.Add(h);
			}
			//if (Scm != null)
			//{
			//    submittedTreeListView.Columns.Clear();
			//    bool swarmEnabled = Scm.SwarmEnabled;
			//    foreach (ColumnHeader h in DefaultListColumns)
			//    {
			//        submittedTreeListView.Columns.Add(h);
			//    }
			//    if (swarmEnabled == false)
			//    {
			//        int idx = submittedTreeListView.Columns.IndexOf(ClmHdr_ReviewId);
			//        if (idx >= 0)
			//        {
			//            submittedTreeListView.Columns.RemoveAt(idx);
			//        }
			//        idx = submittedTreeListView.Columns.IndexOf(ClmHdr_ReviewStatus);
			//        if (idx >= 0)
			//        {
			//            submittedTreeListView.Columns.RemoveAt(idx);
			//        }
			//    }
			//}
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
			this.submittedTreeListView.Nodes.Clear();
			clearDetails();
			maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
			if (maxItems == 0)
			{
				maxItems = -1;
			}

			filterBtn.Enabled = (Scm != null) && Scm.Connected;
			if (Scm != null)
			{
				if (userCB.Text == "" &&
					userCB.mruValues[1] == null)
				{
					selectionChangedByLoad = true;
                    userCB.Text = Scm.Connection.User;
				}

				if (workspaceCB.Text == "" &&
					workspaceCB.mruValues[1] == null)
				{
					selectionChangedByLoad = true;
                    workspaceCB.Text = Scm.Connection.Workspace;
				}

				//if (userCB.Text == "" &&
				//    !(userCB.mruValues.Contains("")))
				//{
				//   selectionChangedByLoad = true;
				//   userCB.Text = Scm.User;
				//}

				//if (workspaceCB.Text == "" &&
				//    !(workspaceCB.mruValues.Contains("")))
				//{
				//    selectionChangedByLoad = true;
				//    workspaceCB.Text = Scm.Workspace;
				//}
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubmittedChangelistsToolWindowControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.submittedTreeListView = new Perforce.I18nControls.GridP4ObjectTreeListView();
            this.ClmHdr_Change = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_ReviewId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_ReviewStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_Date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_Workspace = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_User = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_AccessType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ClmHdr_Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.changelistContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.getRevisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diffAgainstToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.requestReviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateReviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openReviewInSwarmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.editSubmittedChangelistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshSubmittedChangelistListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshSubmittedChangelistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeJobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.threadMonitorControl1 = new Perforce.P4VS.UI.ThreadMonitorControl();
            this.gridGroupBox1 = new Perforce.I18nControls.GridGroupBox();
            this.matchesLbl = new Perforce.I18nControls.GridLabel();
            this.filterBtn = new Perforce.I18nControls.GridButton();
            this.workspaceCB = new Perforce.I18nControls.GridFilterComboBox();
            this.userCB = new Perforce.I18nControls.GridFilterComboBox();
            this.workspaceLbl = new Perforce.I18nControls.GridLabel();
            this.pathCB = new Perforce.I18nControls.GridFilterComboBox();
            this.userLbl = new Perforce.I18nControls.GridLabel();
            this.pathLbl = new Perforce.I18nControls.GridLabel();
            this.panel2 = new Perforce.I18nControls.GridLayoutPanel();
            this.typeTB = new Perforce.I18nControls.GridTextBox();
            this.workspaceTB2 = new Perforce.I18nControls.GridTextBox();
            this.statusTB = new Perforce.I18nControls.GridTextBox();
            this.UserTB2 = new Perforce.I18nControls.GridTextBox();
            this.dateTB = new Perforce.I18nControls.GridTextBox();
            this.typeLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceLbl2 = new Perforce.I18nControls.GridLabel();
            this.userLbl2 = new Perforce.I18nControls.GridLabel();
            this.statusLbl = new Perforce.I18nControls.GridLabel();
            this.filesTB = new Perforce.I18nControls.GridTextBox();
            this.dateLbl = new Perforce.I18nControls.GridLabel();
            this.jobsTB = new Perforce.I18nControls.GridTextBox();
            this.changeTB = new Perforce.I18nControls.GridTextBox();
            this.filesLbl = new Perforce.I18nControls.GridLabel();
            this.changeLbl = new Perforce.I18nControls.GridLabel();
            this.jobsLbl = new Perforce.I18nControls.GridLabel();
            this.descriptionTB = new Perforce.I18nControls.GridTextBox();
            this.descriptionLbl = new Perforce.I18nControls.GridLabel();
            this.timeLapseViewtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileHistoryStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.changelistContextMenuStrip.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.TabStop = false;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.submittedTreeListView);
            this.panel1.Controls.Add(this.threadMonitorControl1);
            this.panel1.Controls.Add(this.gridGroupBox1);
            this.panel1.Controls.Add(this.matchesLbl);
            this.panel1.Controls.Add(this.filterBtn);
            this.panel1.Controls.Add(this.workspaceCB);
            this.panel1.Controls.Add(this.userCB);
            this.panel1.Controls.Add(this.workspaceLbl);
            this.panel1.Controls.Add(this.pathCB);
            this.panel1.Controls.Add(this.userLbl);
            this.panel1.Controls.Add(this.pathLbl);
            this.panel1.EnableDesignerGrid = false;
            this.panel1.EnableDesignerLayout = false;
            this.panel1.EnableParentResize = false;
            this.panel1.MinimumColumnWidth = 10;
            this.panel1.MinimumRowHeight = 10;
            this.panel1.Name = "panel1";
            // 
            // submittedTreeListView
            // 
            this.submittedTreeListView._maxLineOffset = 0;
            this.submittedTreeListView.ActionColumn = -1;
            this.submittedTreeListView.AllowColumnReorder = true;
            resources.ApplyResources(this.submittedTreeListView, "submittedTreeListView");
            this.submittedTreeListView.CellHeight = 123;
            this.submittedTreeListView.CellWidth = 644;
            this.submittedTreeListView.Column = 0;
            this.submittedTreeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ClmHdr_Change,
            this.ClmHdr_ReviewId,
            this.ClmHdr_ReviewStatus,
            this.ClmHdr_Date,
            this.ClmHdr_Workspace,
            this.ClmHdr_User,
            this.ClmHdr_AccessType,
            this.ClmHdr_Description});
            this.submittedTreeListView.ColumnsSpanned = 5;
            this.submittedTreeListView.ContextMenuStrip = this.changelistContextMenuStrip;
            this.submittedTreeListView.EnableIconOverlays = true;
            this.submittedTreeListView.EnableSorting = true;
            this.submittedTreeListView.FullRowSelect = true;
            this.submittedTreeListView.HideSelection = false;
            this.submittedTreeListView.MultiSelect = false;
            this.submittedTreeListView.MultiSelectConditions = Perforce.P4VS.TreeListView.MultiSelectCondition.none;
            this.submittedTreeListView.Name = "submittedTreeListView";
            this.submittedTreeListView.OverlayOffset = 3;
            this.submittedTreeListView.OwnerDraw = true;
            this.submittedTreeListView.RootCheckBoxes = false;
            this.submittedTreeListView.Row = 2;
            this.submittedTreeListView.RowsSpanned = 0;
            this.submittedTreeListView.ScrollPosition = 0;
            this.submittedTreeListView.TabStop = false;
            this.submittedTreeListView.TreeView = true;
            this.submittedTreeListView.UseClassicImageList = false;
            this.submittedTreeListView.UseCompatibleStateImageBehavior = false;
            this.submittedTreeListView.View = System.Windows.Forms.View.Details;
            this.submittedTreeListView.YOffset = 0;
            this.submittedTreeListView.onMaxScroll += new System.Windows.Forms.ScrollEventHandler(this.submittedTreeListView_onMaxScroll);
            this.submittedTreeListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.submittedTreeListView_ColumnClick);
            this.submittedTreeListView.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.submittedTreeListView_ColumnReordered);
            this.submittedTreeListView.SelectedIndexChanged += new System.EventHandler(this.submittedTreeListView_SelectedIndexChanged);
            this.submittedTreeListView.Click += new System.EventHandler(this.submittedTreeListView_Click);
            this.submittedTreeListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.submittedTreeListView_MouseDoubleClick);
            // 
            // ClmHdr_Change
            // 
            resources.ApplyResources(this.ClmHdr_Change, "ClmHdr_Change");
            // 
            // ClmHdr_ReviewId
            // 
            resources.ApplyResources(this.ClmHdr_ReviewId, "ClmHdr_ReviewId");
            // 
            // ClmHdr_ReviewStatus
            // 
            resources.ApplyResources(this.ClmHdr_ReviewStatus, "ClmHdr_ReviewStatus");
            // 
            // ClmHdr_Date
            // 
            resources.ApplyResources(this.ClmHdr_Date, "ClmHdr_Date");
            // 
            // ClmHdr_Workspace
            // 
            resources.ApplyResources(this.ClmHdr_Workspace, "ClmHdr_Workspace");
            // 
            // ClmHdr_User
            // 
            resources.ApplyResources(this.ClmHdr_User, "ClmHdr_User");
            // 
            // ClmHdr_AccessType
            // 
            resources.ApplyResources(this.ClmHdr_AccessType, "ClmHdr_AccessType");
            // 
            // ClmHdr_Description
            // 
            resources.ApplyResources(this.ClmHdr_Description, "ClmHdr_Description");
            // 
            // changelistContextMenuStrip
            // 
            this.changelistContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getRevisionToolStripMenuItem,
            this.diffAgainstToolStripMenuItem,
            this.fileHistoryStripMenuItem,
            this.timeLapseViewtoolStripMenuItem,
            this.toolStripSeparator1,
            this.requestReviewToolStripMenuItem,
            this.updateReviewToolStripMenuItem,
            this.openReviewInSwarmToolStripMenuItem,
            this.toolStripSeparator2,
            this.editSubmittedChangelistToolStripMenuItem,
            this.toolStripSeparator3,
            this.refreshSubmittedChangelistListToolStripMenuItem,
            this.refreshSubmittedChangelistToolStripMenuItem,
            this.removeJobToolStripMenuItem});
            this.changelistContextMenuStrip.Name = "changelistContextMenuStrip";
            resources.ApplyResources(this.changelistContextMenuStrip, "changelistContextMenuStrip");
            this.changelistContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.changelistContextMenuStrip_Opening);
            // 
            // getRevisionToolStripMenuItem
            // 
            resources.ApplyResources(this.getRevisionToolStripMenuItem, "getRevisionToolStripMenuItem");
            this.getRevisionToolStripMenuItem.Name = "getRevisionToolStripMenuItem";
            this.getRevisionToolStripMenuItem.Click += new System.EventHandler(this.getRevisionToolStripMenuItem_Click);
            // 
            // diffAgainstToolStripMenuItem
            // 
            resources.ApplyResources(this.diffAgainstToolStripMenuItem, "diffAgainstToolStripMenuItem");
            this.diffAgainstToolStripMenuItem.Name = "diffAgainstToolStripMenuItem";
            this.diffAgainstToolStripMenuItem.Click += new System.EventHandler(this.diffAgainstToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.toolStripSeparator1.ForeColor = System.Drawing.Color.Black;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // requestReviewToolStripMenuItem
            // 
            this.requestReviewToolStripMenuItem.Name = "requestReviewToolStripMenuItem";
            resources.ApplyResources(this.requestReviewToolStripMenuItem, "requestReviewToolStripMenuItem");
            this.requestReviewToolStripMenuItem.Click += new System.EventHandler(this.requestReviewToolStripMenuItem_Click);
            // 
            // updateReviewToolStripMenuItem
            // 
            this.updateReviewToolStripMenuItem.Name = "updateReviewToolStripMenuItem";
            resources.ApplyResources(this.updateReviewToolStripMenuItem, "updateReviewToolStripMenuItem");
            this.updateReviewToolStripMenuItem.Click += new System.EventHandler(this.updateReviewToolStripMenuItem_Click);
            // 
            // openReviewInSwarmToolStripMenuItem
            // 
            this.openReviewInSwarmToolStripMenuItem.Name = "openReviewInSwarmToolStripMenuItem";
            resources.ApplyResources(this.openReviewInSwarmToolStripMenuItem, "openReviewInSwarmToolStripMenuItem");
            this.openReviewInSwarmToolStripMenuItem.Click += new System.EventHandler(this.openReviewInSwarmToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // editSubmittedChangelistToolStripMenuItem
            // 
            resources.ApplyResources(this.editSubmittedChangelistToolStripMenuItem, "editSubmittedChangelistToolStripMenuItem");
            this.editSubmittedChangelistToolStripMenuItem.Name = "editSubmittedChangelistToolStripMenuItem";
            this.editSubmittedChangelistToolStripMenuItem.Click += new System.EventHandler(this.editSubmittedChangelistToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // refreshSubmittedChangelistListToolStripMenuItem
            // 
            resources.ApplyResources(this.refreshSubmittedChangelistListToolStripMenuItem, "refreshSubmittedChangelistListToolStripMenuItem");
            this.refreshSubmittedChangelistListToolStripMenuItem.Name = "refreshSubmittedChangelistListToolStripMenuItem";
            this.refreshSubmittedChangelistListToolStripMenuItem.Click += new System.EventHandler(this.refreshSubmittedChangelistListToolStripMenuItem_Click);
            // 
            // refreshSubmittedChangelistToolStripMenuItem
            // 
            resources.ApplyResources(this.refreshSubmittedChangelistToolStripMenuItem, "refreshSubmittedChangelistToolStripMenuItem");
            this.refreshSubmittedChangelistToolStripMenuItem.Name = "refreshSubmittedChangelistToolStripMenuItem";
            this.refreshSubmittedChangelistToolStripMenuItem.Click += new System.EventHandler(this.refreshSubmittedChangelistToolStripMenuItem_Click);
            // 
            // removeJobToolStripMenuItem
            // 
            resources.ApplyResources(this.removeJobToolStripMenuItem, "removeJobToolStripMenuItem");
            this.removeJobToolStripMenuItem.Name = "removeJobToolStripMenuItem";
            this.removeJobToolStripMenuItem.Click += new System.EventHandler(this.removeJobToolStripMenuItem_Click);
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
            // gridGroupBox1
            // 
            resources.ApplyResources(this.gridGroupBox1, "gridGroupBox1");
            this.gridGroupBox1.CellHeight = 56;
            this.gridGroupBox1.CellWidth = 10;
            this.gridGroupBox1.Column = 4;
            this.gridGroupBox1.ColumnsSpanned = 0;
            this.gridGroupBox1.Name = "gridGroupBox1";
            this.gridGroupBox1.Row = 0;
            this.gridGroupBox1.RowsSpanned = 1;
            this.gridGroupBox1.TabStop = false;
            this.gridGroupBox1.YOffset = 0;
            // 
            // matchesLbl
            // 
            resources.ApplyResources(this.matchesLbl, "matchesLbl");
            this.matchesLbl.CellHeight = 27;
            this.matchesLbl.CellWidth = 81;
            this.matchesLbl.Column = 5;
            this.matchesLbl.ColumnsSpanned = 0;
            this.matchesLbl.Name = "matchesLbl";
            this.matchesLbl.Row = 1;
            this.matchesLbl.RowsSpanned = 0;
            this.matchesLbl.YOffset = 4;
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
            this.filterBtn.Click += new System.EventHandler(this.filterBtn_Click);
            // 
            // workspaceCB
            // 
            resources.ApplyResources(this.workspaceCB, "workspaceCB");
            this.workspaceCB.CellHeight = 27;
            this.workspaceCB.CellWidth = 209;
            this.workspaceCB.Column = 3;
            this.workspaceCB.ColumnsSpanned = 0;
            this.workspaceCB.FormattingEnabled = true;
            this.workspaceCB.Name = "workspaceCB";
            this.workspaceCB.Row = 1;
            this.workspaceCB.RowsSpanned = 0;
            this.workspaceCB.YOffset = 0;
            this.workspaceCB.SelectedIndexChanged += new System.EventHandler(this.workspaceCB_SelectedIndexChanged);
            this.workspaceCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.workspaceCB_KeyDown);
            // 
            // userCB
            // 
            resources.ApplyResources(this.userCB, "userCB");
            this.userCB.CellHeight = 27;
            this.userCB.CellWidth = 210;
            this.userCB.Column = 1;
            this.userCB.ColumnsSpanned = 0;
            this.userCB.FormattingEnabled = true;
            this.userCB.Name = "userCB";
            this.userCB.Row = 1;
            this.userCB.RowsSpanned = 0;
            this.userCB.YOffset = 0;
            this.userCB.SelectedIndexChanged += new System.EventHandler(this.userCB_SelectedIndexChanged);
            this.userCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userCB_KeyDown);
            // 
            // workspaceLbl
            // 
            resources.ApplyResources(this.workspaceLbl, "workspaceLbl");
            this.workspaceLbl.CellHeight = 27;
            this.workspaceLbl.CellWidth = 71;
            this.workspaceLbl.Column = 2;
            this.workspaceLbl.ColumnsSpanned = 0;
            this.workspaceLbl.Name = "workspaceLbl";
            this.workspaceLbl.Row = 1;
            this.workspaceLbl.RowsSpanned = 0;
            this.workspaceLbl.YOffset = 4;
            // 
            // pathCB
            // 
            resources.ApplyResources(this.pathCB, "pathCB");
            this.pathCB.CellHeight = 29;
            this.pathCB.CellWidth = 490;
            this.pathCB.Column = 1;
            this.pathCB.ColumnsSpanned = 2;
            this.pathCB.FormattingEnabled = true;
            this.pathCB.Name = "pathCB";
            this.pathCB.Row = 0;
            this.pathCB.RowsSpanned = 0;
            this.pathCB.YOffset = 1;
            this.pathCB.SelectedIndexChanged += new System.EventHandler(this.pathCB_SelectedIndexChanged);
            this.pathCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pathCB_KeyDown);
            // 
            // userLbl
            // 
            resources.ApplyResources(this.userLbl, "userLbl");
            this.userLbl.CellHeight = 27;
            this.userLbl.CellWidth = 63;
            this.userLbl.Column = 0;
            this.userLbl.ColumnsSpanned = 0;
            this.userLbl.Name = "userLbl";
            this.userLbl.Row = 1;
            this.userLbl.RowsSpanned = 0;
            this.userLbl.YOffset = 4;
            // 
            // pathLbl
            // 
            resources.ApplyResources(this.pathLbl, "pathLbl");
            this.pathLbl.CellHeight = 29;
            this.pathLbl.CellWidth = 63;
            this.pathLbl.Column = 0;
            this.pathLbl.ColumnsSpanned = 0;
            this.pathLbl.Name = "pathLbl";
            this.pathLbl.Row = 0;
            this.pathLbl.RowsSpanned = 0;
            this.pathLbl.YOffset = 5;
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel2.Controls.Add(this.typeTB);
            this.panel2.Controls.Add(this.workspaceTB2);
            this.panel2.Controls.Add(this.statusTB);
            this.panel2.Controls.Add(this.UserTB2);
            this.panel2.Controls.Add(this.dateTB);
            this.panel2.Controls.Add(this.typeLbl);
            this.panel2.Controls.Add(this.workspaceLbl2);
            this.panel2.Controls.Add(this.userLbl2);
            this.panel2.Controls.Add(this.statusLbl);
            this.panel2.Controls.Add(this.filesTB);
            this.panel2.Controls.Add(this.dateLbl);
            this.panel2.Controls.Add(this.jobsTB);
            this.panel2.Controls.Add(this.changeTB);
            this.panel2.Controls.Add(this.filesLbl);
            this.panel2.Controls.Add(this.changeLbl);
            this.panel2.Controls.Add(this.jobsLbl);
            this.panel2.Controls.Add(this.descriptionTB);
            this.panel2.Controls.Add(this.descriptionLbl);
            this.panel2.EnableDesignerGrid = false;
            this.panel2.EnableDesignerLayout = false;
            this.panel2.EnableParentResize = false;
            this.panel2.MinimumColumnWidth = 10;
            this.panel2.MinimumRowHeight = 10;
            this.panel2.Name = "panel2";
            // 
            // typeTB
            // 
            resources.ApplyResources(this.typeTB, "typeTB");
            this.typeTB.CellHeight = 26;
            this.typeTB.CellWidth = 263;
            this.typeTB.Column = 3;
            this.typeTB.ColumnsSpanned = 0;
            this.typeTB.Name = "typeTB";
            this.typeTB.ReadOnly = true;
            this.typeTB.Row = 2;
            this.typeTB.RowsSpanned = 0;
            this.typeTB.YOffset = 3;
            // 
            // workspaceTB2
            // 
            resources.ApplyResources(this.workspaceTB2, "workspaceTB2");
            this.workspaceTB2.CellHeight = 26;
            this.workspaceTB2.CellWidth = 264;
            this.workspaceTB2.Column = 1;
            this.workspaceTB2.ColumnsSpanned = 0;
            this.workspaceTB2.Name = "workspaceTB2";
            this.workspaceTB2.ReadOnly = true;
            this.workspaceTB2.Row = 2;
            this.workspaceTB2.RowsSpanned = 0;
            this.workspaceTB2.YOffset = 3;
            // 
            // statusTB
            // 
            resources.ApplyResources(this.statusTB, "statusTB");
            this.statusTB.CellHeight = 26;
            this.statusTB.CellWidth = 263;
            this.statusTB.Column = 3;
            this.statusTB.ColumnsSpanned = 0;
            this.statusTB.Name = "statusTB";
            this.statusTB.ReadOnly = true;
            this.statusTB.Row = 1;
            this.statusTB.RowsSpanned = 0;
            this.statusTB.YOffset = 3;
            // 
            // UserTB2
            // 
            resources.ApplyResources(this.UserTB2, "UserTB2");
            this.UserTB2.CellHeight = 26;
            this.UserTB2.CellWidth = 263;
            this.UserTB2.Column = 3;
            this.UserTB2.ColumnsSpanned = 0;
            this.UserTB2.Name = "UserTB2";
            this.UserTB2.ReadOnly = true;
            this.UserTB2.Row = 0;
            this.UserTB2.RowsSpanned = 0;
            this.UserTB2.YOffset = 3;
            // 
            // dateTB
            // 
            resources.ApplyResources(this.dateTB, "dateTB");
            this.dateTB.CellHeight = 26;
            this.dateTB.CellWidth = 264;
            this.dateTB.Column = 1;
            this.dateTB.ColumnsSpanned = 0;
            this.dateTB.Name = "dateTB";
            this.dateTB.ReadOnly = true;
            this.dateTB.Row = 1;
            this.dateTB.RowsSpanned = 0;
            this.dateTB.YOffset = 3;
            // 
            // typeLbl
            // 
            resources.ApplyResources(this.typeLbl, "typeLbl");
            this.typeLbl.CellHeight = 26;
            this.typeLbl.CellWidth = 46;
            this.typeLbl.Column = 2;
            this.typeLbl.ColumnsSpanned = 0;
            this.typeLbl.Name = "typeLbl";
            this.typeLbl.Row = 2;
            this.typeLbl.RowsSpanned = 0;
            this.typeLbl.YOffset = 0;
            // 
            // workspaceLbl2
            // 
            resources.ApplyResources(this.workspaceLbl2, "workspaceLbl2");
            this.workspaceLbl2.CellHeight = 26;
            this.workspaceLbl2.CellWidth = 71;
            this.workspaceLbl2.Column = 0;
            this.workspaceLbl2.ColumnsSpanned = 0;
            this.workspaceLbl2.Name = "workspaceLbl2";
            this.workspaceLbl2.Row = 2;
            this.workspaceLbl2.RowsSpanned = 0;
            this.workspaceLbl2.YOffset = 6;
            // 
            // userLbl2
            // 
            resources.ApplyResources(this.userLbl2, "userLbl2");
            this.userLbl2.CellHeight = 26;
            this.userLbl2.CellWidth = 46;
            this.userLbl2.Column = 2;
            this.userLbl2.ColumnsSpanned = 0;
            this.userLbl2.Name = "userLbl2";
            this.userLbl2.Row = 0;
            this.userLbl2.RowsSpanned = 0;
            this.userLbl2.YOffset = 0;
            // 
            // statusLbl
            // 
            resources.ApplyResources(this.statusLbl, "statusLbl");
            this.statusLbl.CellHeight = 26;
            this.statusLbl.CellWidth = 46;
            this.statusLbl.Column = 2;
            this.statusLbl.ColumnsSpanned = 0;
            this.statusLbl.Name = "statusLbl";
            this.statusLbl.Row = 1;
            this.statusLbl.RowsSpanned = 0;
            this.statusLbl.YOffset = 0;
            // 
            // filesTB
            // 
            resources.ApplyResources(this.filesTB, "filesTB");
            this.filesTB.CellHeight = 73;
            this.filesTB.CellWidth = 573;
            this.filesTB.Column = 1;
            this.filesTB.ColumnsSpanned = 2;
            this.filesTB.Name = "filesTB";
            this.filesTB.ReadOnly = true;
            this.filesTB.Row = 5;
            this.filesTB.RowsSpanned = 0;
            this.filesTB.YOffset = 0;
            // 
            // dateLbl
            // 
            resources.ApplyResources(this.dateLbl, "dateLbl");
            this.dateLbl.CellHeight = 26;
            this.dateLbl.CellWidth = 71;
            this.dateLbl.Column = 0;
            this.dateLbl.ColumnsSpanned = 0;
            this.dateLbl.Name = "dateLbl";
            this.dateLbl.Row = 1;
            this.dateLbl.RowsSpanned = 0;
            this.dateLbl.YOffset = 6;
            // 
            // jobsTB
            // 
            resources.ApplyResources(this.jobsTB, "jobsTB");
            this.jobsTB.CellHeight = 73;
            this.jobsTB.CellWidth = 573;
            this.jobsTB.Column = 1;
            this.jobsTB.ColumnsSpanned = 2;
            this.jobsTB.Name = "jobsTB";
            this.jobsTB.ReadOnly = true;
            this.jobsTB.Row = 4;
            this.jobsTB.RowsSpanned = 0;
            this.jobsTB.YOffset = 0;
            // 
            // changeTB
            // 
            resources.ApplyResources(this.changeTB, "changeTB");
            this.changeTB.CellHeight = 26;
            this.changeTB.CellWidth = 264;
            this.changeTB.Column = 1;
            this.changeTB.ColumnsSpanned = 0;
            this.changeTB.Name = "changeTB";
            this.changeTB.ReadOnly = true;
            this.changeTB.Row = 0;
            this.changeTB.RowsSpanned = 0;
            this.changeTB.YOffset = 3;
            // 
            // filesLbl
            // 
            resources.ApplyResources(this.filesLbl, "filesLbl");
            this.filesLbl.CellHeight = 73;
            this.filesLbl.CellWidth = 71;
            this.filesLbl.Column = 0;
            this.filesLbl.ColumnsSpanned = 0;
            this.filesLbl.Name = "filesLbl";
            this.filesLbl.Row = 5;
            this.filesLbl.RowsSpanned = 0;
            this.filesLbl.YOffset = 0;
            // 
            // changeLbl
            // 
            resources.ApplyResources(this.changeLbl, "changeLbl");
            this.changeLbl.CellHeight = 26;
            this.changeLbl.CellWidth = 71;
            this.changeLbl.Column = 0;
            this.changeLbl.ColumnsSpanned = 0;
            this.changeLbl.Name = "changeLbl";
            this.changeLbl.Row = 0;
            this.changeLbl.RowsSpanned = 0;
            this.changeLbl.YOffset = 6;
            // 
            // jobsLbl
            // 
            resources.ApplyResources(this.jobsLbl, "jobsLbl");
            this.jobsLbl.CellHeight = 73;
            this.jobsLbl.CellWidth = 71;
            this.jobsLbl.Column = 0;
            this.jobsLbl.ColumnsSpanned = 0;
            this.jobsLbl.Name = "jobsLbl";
            this.jobsLbl.Row = 4;
            this.jobsLbl.RowsSpanned = 0;
            this.jobsLbl.YOffset = 0;
            // 
            // descriptionTB
            // 
            resources.ApplyResources(this.descriptionTB, "descriptionTB");
            this.descriptionTB.CellHeight = 73;
            this.descriptionTB.CellWidth = 573;
            this.descriptionTB.Column = 1;
            this.descriptionTB.ColumnsSpanned = 2;
            this.descriptionTB.Name = "descriptionTB";
            this.descriptionTB.ReadOnly = true;
            this.descriptionTB.Row = 3;
            this.descriptionTB.RowsSpanned = 0;
            this.descriptionTB.YOffset = 0;
            // 
            // descriptionLbl
            // 
            resources.ApplyResources(this.descriptionLbl, "descriptionLbl");
            this.descriptionLbl.CellHeight = 73;
            this.descriptionLbl.CellWidth = 71;
            this.descriptionLbl.Column = 0;
            this.descriptionLbl.ColumnsSpanned = 0;
            this.descriptionLbl.Name = "descriptionLbl";
            this.descriptionLbl.Row = 3;
            this.descriptionLbl.RowsSpanned = 0;
            this.descriptionLbl.YOffset = 0;
            // 
            // timeLapseViewtoolStripMenuItem
            // 
            this.timeLapseViewtoolStripMenuItem.Name = "timeLapseViewtoolStripMenuItem";
            resources.ApplyResources(this.timeLapseViewtoolStripMenuItem, "timeLapseViewtoolStripMenuItem");
            this.timeLapseViewtoolStripMenuItem.Click += new System.EventHandler(this.timeLapseViewtoolStripMenuItem_Click);
            // 
            // fileHistoryStripMenuItem
            // 
            this.fileHistoryStripMenuItem.Name = "fileHistoryStripMenuItem";
            resources.ApplyResources(this.fileHistoryStripMenuItem, "fileHistoryStripMenuItem");
            this.fileHistoryStripMenuItem.Click += new System.EventHandler(this.fileHistoryStripMenuItem_Click);
            // 
            // SubmittedChangelistsToolWindowControl
            // 
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitContainer1);
            this.Name = "SubmittedChangelistsToolWindowControl";
            this.Load += new System.EventHandler(this.SubmittedChangelistsToolWindowControl_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.changelistContextMenuStrip.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void clearDetails()
		{
			changeTB.Text = string.Empty;
			changeTB.Refresh();
			dateTB.Text = string.Empty;
			dateTB.Refresh();
			workspaceTB2.Text = string.Empty;
			workspaceTB2.Refresh();
			descriptionTB.Text = string.Empty;
			descriptionTB.Refresh();
			UserTB2.Text = string.Empty;
			UserTB2.Refresh();
			statusTB.Text = string.Empty;
			statusTB.Refresh();
			statusTB.Text = string.Empty;
			statusTB.Refresh();
			typeTB.Text = string.Empty;
			jobsTB.Text = string.Empty;
			jobsTB.Refresh();
			filesTB.Text = string.Empty;
			filesTB.Refresh();
		}
		private object[] SubmittedTLVIFields
		{
			get
			{
                if ((Scm != null) && (Scm.Connection.Swarm.SwarmEnabled))			
				{ 
					return SubmittedTLVIFieldsSwarm; 
				}
				return SubmittedTLVIFieldsNoSwarm;
			}
		}
		private static object[] SubmittedTLVIFieldsNoSwarm = new object[] { 
					P4ChangeTreeListViewItem.SubItemFlag.Id, 
					P4ChangeTreeListViewItem.SubItemFlag.ModifiedDate, 
					P4ChangeTreeListViewItem.SubItemFlag.ClientName, 
					P4ChangeTreeListViewItem.SubItemFlag.OwnerName, 
					P4ChangeTreeListViewItem.SubItemFlag.Type, 
					P4ChangeTreeListViewItem.SubItemFlag.Description};
		private static object[] SubmittedTLVIFieldsSwarm = new object[] { 
					P4ChangeTreeListViewItem.SubItemFlag.Id, 
					P4ChangeTreeListViewItem.SubItemFlag.ReviewId, 
					P4ChangeTreeListViewItem.SubItemFlag.ReviewState, 
					P4ChangeTreeListViewItem.SubItemFlag.ModifiedDate, 
					P4ChangeTreeListViewItem.SubItemFlag.ClientName, 
					P4ChangeTreeListViewItem.SubItemFlag.OwnerName, 
					P4ChangeTreeListViewItem.SubItemFlag.Type, 
					P4ChangeTreeListViewItem.SubItemFlag.Description};

		public new P4ScmProvider Scm { get; set; }

		private string PathFilterText { get; set; }
		private string UserFilterText { get; set; }
		private string WorkspaceFilterText { get; set; }

		private object SyncRoot = new object();

		private delegate void PendingTreeListViewDelegate();
		private delegate void PendingTreeListViewItemDelegate(TreeListViewItem item);

		public P4.Changelist SelectedChangelist { get; private set; }

		private delegate void SubmittedTreeListViewSetBoolDelegate(bool state);
		private delegate void SubmittedTreeListViewDelegate();
		private delegate void SubmittedTreeListViewItemDelegate(TreeListViewItem item);
		private delegate ListViewItem SubmittedTreeListViewItemDelegate2(TreeListViewItem item);
		private delegate void setStringPropertyDelegate(string str);

		private void setSubmittedMatchesLblText(string matches)
		{
			this.matchesLbl.Text = matches;
		}

        private delegate void ReplaceSubmittedTreeListViewItemDelegate(int index, TreeListViewItem item);

        private void replaceSubmittedListViewItem(int index, TreeListViewItem item)
        {
            submittedTreeListView.Nodes.Remove(item);
            //submittedTreeListView.Nodes.Insert(index, item);
        }

		public int addChangelistItem(int ChangeId)
		{
			P4.Changelist addedChange = Scm.GetChangelist(ChangeId);

			P4ChangeTreeListViewItem itemToAdd = new P4ChangeTreeListViewItem(null, addedChange, Scm, SubmittedTLVIFields);

			itemToAdd.ChildNodes.Add(new TreeListViewItem());
			if (submittedTreeListView.InvokeRequired)
			{
				submittedTreeListView.Invoke(new PendingTreeListViewItemDelegate(this.submittedTreeListView.Nodes.Add), itemToAdd);
				submittedTreeListView.Invoke(new PendingTreeListViewDelegate(this.submittedTreeListView.BuildTreeList));
			}
			else
			{
				this.submittedTreeListView.Nodes.Add(itemToAdd);
				this.submittedTreeListView.BuildTreeList();
			}
			return addedChange.Id;
		}

		private void AsyncPopulateListView()
		{
			bool threadAborted = false;
            if (submittedTreeListView.InvokeRequired)
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

                    P4.ChangesCmdFlags flags = P4.ChangesCmdFlags.FullDescription;
					P4.FileSpec path = new P4.FileSpec();
					string workspaceName = null;
					string user = null;

					if (string.IsNullOrEmpty(PathFilterText))
					{ path = null; }

					else
					{
						if (PathFilterText.StartsWith("//"))
						{ path.DepotPath = new P4.DepotPath(PathFilterText); }
						else
						{ path.LocalPath = new P4.LocalPath(PathFilterText); }
					}

					if (string.IsNullOrEmpty(WorkspaceFilterText))
					{ workspaceName = null; }
					else
					{ workspaceName = WorkspaceFilterText; }

					if (string.IsNullOrEmpty(UserFilterText))
					{ user = null; }
					else
					{ user = UserFilterText; }

                    if (maxItems == 0)
                    {
                        maxItems = -1;
                    }

					IList<P4.Changelist> changelists = Scm.GetChangelists(flags, workspaceName, maxItems, P4.ChangeListStatus.Submitted, user, path);

					TreeListViewItem it = new TreeListViewItem();
					if ((changelists == null) || (changelists.Count <= 0))
					{
						it = new TreeListViewItem(null, Resources.JobsToolWindowControl_NoItemsAvailable, true);

                        if (submittedTreeListView.InvokeRequired)
						{
                            submittedTreeListView.Invoke(new SubmittedChangelistsToolWindowControl.SubmittedTreeListViewDelegate(this.submittedTreeListView.Items.Clear));
                            submittedTreeListView.Invoke(new SubmittedChangelistsToolWindowControl.SubmittedTreeListViewDelegate(this.submittedTreeListView.Nodes.Clear));
							submittedTreeListView.Invoke(new SubmittedTreeListViewItemDelegate(this.submittedTreeListView.Nodes.Add), it);
							this.matchesLbl.Invoke(new setStringPropertyDelegate(setSubmittedMatchesLblText), 
								Resources.JobsToolWindowControl_NoMatches);
							submittedTreeListView.Invoke(new SubmittedTreeListViewDelegate(this.submittedTreeListView.BuildTreeList));
						}
						else
						{
                            submittedTreeListView.Items.Clear();
                            submittedTreeListView.Nodes.Clear();
							this.submittedTreeListView.Nodes.Add(it);
							this.matchesLbl.Text = "no matches";
							this.submittedTreeListView.BuildTreeList();
						}

                        return;
					}
					threadMonitorControl1.Value = 0;
					//threadMonitorControl1.Show(FillInListProc, changelists.Count);

					int cnt = 0;
					//int stepCnt = 25;
					//if (changelists.Count >= 2600)
					//{
					//    stepCnt = changelists.Count / 100;
					//}
					int stepCnt = 25;
					if (changelists.Count >= 260)
					{
						stepCnt = changelists.Count / 10;
					}
					if (stepCnt > 1000)
					{
						stepCnt = 1000;
					}

					int sleepTime = 300;
                        try
                        {
                            Dictionary<int, SwarmApi.SwarmServer.Review> ChangeReviewMap = null;

                            if (Scm.Connection.Swarm.SwarmEnabled)
                            {
                                ChangeReviewMap = new Dictionary<int, SwarmApi.SwarmServer.Review>();

                                foreach (P4.Changelist change in changelists)
                                {
                                    ChangeReviewMap.Add(change.Id, null);
                                }
                                Scm.Connection.Swarm.IsChangelistAttachedToReview(ChangeReviewMap);
                            }
                            //if (submittedTreeListView.InvokeRequired)
                            //{
                            //    submittedTreeListView.Invoke(new SubmittedTreeListViewDelegate(submittedTreeListView.BeginUpdate));
                            //}
                            //else
                            //{
                            //    submittedTreeListView.BeginUpdate();
                            //}
                            threadMonitorControl1.CancelPressed = false;

                            for (int idx = 0; idx < changelists.Count; idx++)
                            {
                                P4.Changelist changelist = changelists[idx];

                                SwarmApi.SwarmServer.Review review = null;
                                if ((ChangeReviewMap != null) && (ChangeReviewMap.ContainsKey(changelist.Id)))
                                {
                                    review = ChangeReviewMap[changelist.Id];
                                }
                                P4ChangeTreeListViewItem item = new P4ChangeTreeListViewItem(null, changelist, review, Scm, SubmittedTLVIFields);

                                item.ChildNodes.Add(new P4ObjectTreeListViewItem());

                                if (submittedTreeListView.InvokeRequired)
                                {
                                    if (idx < submittedTreeListView.Items.Count)
                                    {

                                        submittedTreeListView.Invoke(
                                            new ReplaceSubmittedTreeListViewItemDelegate(replaceSubmittedListViewItem),
                                            idx, item);

                                    }
                                    else
                                    {
                                        submittedTreeListView.Invoke(new SubmittedTreeListViewItemDelegate(this.submittedTreeListView.Nodes.Add), item);
                                        submittedTreeListView.Invoke(new SubmittedTreeListViewItemDelegate2(this.submittedTreeListView.Items.Add), item);
                                    }
                                }
                                else
                                {
                                    if (idx < submittedTreeListView.Items.Count)
                                    {
                                        submittedTreeListView.Items[idx] = item;
                                    }
                                    else
                                    {
                                        this.submittedTreeListView.Nodes.Add(item);
                                        this.submittedTreeListView.Items.Add(item);
                                    }
                                }
                                ++cnt;
                                if ((cnt % stepCnt) == 0)
                                {
                                    threadMonitorControl1.Value = cnt;

                                    sleepTime = Math.Min(250, (cnt * 3) / 2);
                                    Thread.Sleep(sleepTime); // yield to let the progress bar update
                                }
                                if ((!submittedTreeListView.Updating) && (cnt > submittedTreeListView.PageSize + 1))
                                {
                                    threadMonitorControl1.Show(changelists.Count);

                                    if (submittedTreeListView.InvokeRequired)
                                    {
                                        submittedTreeListView.Invoke(new SubmittedTreeListViewDelegate(submittedTreeListView.BeginUpdate));
                                    }
                                    else
                                    {
                                        submittedTreeListView.BeginUpdate();
                                    }
                                }
                                if (threadMonitorControl1.CancelPressed)
                                {
                                    break;
                                }
                            }

                            if (submittedTreeListView.InvokeRequired)
                            {
                                submittedTreeListView.Invoke(new SubmittedTreeListViewDelegate(this.submittedTreeListView.BuildTreeList));
                            }
                            else
                            {
                                this.submittedTreeListView.BuildTreeList();
                            }
                        }
                        finally
                        {
                            try
                            {
                                if (submittedTreeListView != null && submittedTreeListView.IsHandleCreated && !submittedTreeListView.IsDisposed)
                                {
                                    if (submittedTreeListView.InvokeRequired)
                                    {
                                        submittedTreeListView.Invoke(new SubmittedTreeListViewDelegate(submittedTreeListView.EndUpdate));
                                    }
                                    else
                                    {
                                        submittedTreeListView.EndUpdate();
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
						if (submittedTreeListView.Nodes.Count > 1)
						{
							itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches,
                                submittedTreeListView.Nodes.Count);
                            if (submittedTreeListView.Items.Count == maxItems)
                            {
                                itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches,
                                    submittedTreeListView.Nodes.Count + "+");
                            }
                            else
                            {
                                itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches,
                                    submittedTreeListView.Nodes.Count);
                            }
						}

						if (this.matchesLbl.InvokeRequired)
						{
							this.matchesLbl.Invoke(new setStringPropertyDelegate(setSubmittedMatchesLblText), itemCountStr);
						}
						else
						{
							this.matchesLbl.Text = itemCountStr;
						}
						//if (this.filterBtn.InvokeRequired)
						//{
						//    this.filterBtn.Invoke(new SubmittedTreeListViewSetBoolDelegate(this.setFilterButtonEnabled), true);
						//}
						//else
						//{
						//    this.filterBtn.Enabled = true;
						//}		
					}
	
					threadMonitorControl1.Hide();
					FillInListProc = null;
			}
				catch { }
			}
            }
            finally
            {
                if (submittedTreeListView.InvokeRequired)
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
		

		private void refreshSubmittedList()
		{
			// update the column headers to reflect whether or not the server is Swarm enabled
			submittedTreeListView.Columns.Clear();
			foreach (ColumnHeader h in DefaultListColumns)
			{
				submittedTreeListView.Columns.Add(h);
			}
            if ((Scm == null) || (Scm.Connection.isSwarmEnabled() == false))
			{
				int idx = submittedTreeListView.Columns.IndexOf(ClmHdr_ReviewId);
				if (idx >= 0)
				{
					submittedTreeListView.Columns.RemoveAt(idx);
				}
				idx = submittedTreeListView.Columns.IndexOf(ClmHdr_ReviewStatus);
				if (idx >= 0)
				{
					submittedTreeListView.Columns.RemoveAt(idx);
				}
			}

		    filterBtn.Enabled = false;
		    submittedTreeListView.Enabled = false;
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
                submittedTreeListView.Enabled = true;
				return;
			}
			PathFilterText = pathCB.Text;
			UserFilterText = userCB.Text;
			WorkspaceFilterText = workspaceCB.Text;
			threadMonitorControl1.Visible = false;

            if (Scm!=null)
            {
				//if (Scm.SwarmEnabled == false)
				//{
				//    // hide the review status if not hooked up to a swarm server
				//    submittedTreeListView.Columns[1].Width = 0;
				//    submittedTreeListView.Columns[2].Width = 0;
				//}
				//else
				//{
				//    if (submittedTreeListView.Columns[1].Width <= 0)
				//    {
				//        submittedTreeListView.Columns[1].Width = 80;
				//    }
				//    if (submittedTreeListView.Columns[2].Width <= 0)
				//    {
				//        submittedTreeListView.Columns[2].Width = 80;
				//    }
				//}
				FillInListProc = new Thread(new ThreadStart(AsyncPopulateListView));
                FillInListProc.IsBackground = true;

                FillInListProc.Start();
            }

            submittedTreeListView.Enabled = true;
            filterBtn.Enabled = true;

    		submittedTreeListView.BeforeExpand += new TreeListViewEvent(before);

            SaveControlSettings();

			return;

		}

		private int _maxItems = -1;
		private int maxItems
		{
			get
			{
				if (_maxItems == -1)
				{
					if (this.DesignMode)
					{
						_maxItems = 100;
					}
					else
					{
						_maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
					}
				}
				return _maxItems;
			}
			set { _maxItems = value; }
		}

		private void filterBtn_Click(object sender, EventArgs e)
		{
            if (submittedTreeListView.InvokeRequired)
            {
                submittedTreeListView.Invoke(new SubmittedChangelistsToolWindowControl.SubmittedTreeListViewDelegate(this.submittedTreeListView.Items.Clear));
                submittedTreeListView.Invoke(new SubmittedChangelistsToolWindowControl.SubmittedTreeListViewDelegate(this.submittedTreeListView.Nodes.Clear));
            }
            else
            {
                submittedTreeListView.Items.Clear();
                submittedTreeListView.Nodes.Clear();
            }

			maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
			if (maxItems == 0)
			{
				maxItems = -1;
			}
			refreshSubmittedList();
		}

		public int refreshChangelistObject(TreeListViewItem item)
		{
			P4.Changelist changelist = (P4.Changelist)item.Tag;
			changelist = Scm.GetChangelist(changelist.Id);
			item.Tag = changelist;
			string type = changelist.Type.ToString();
			if (type == "None") { type = string.Empty; }
			string description = changelist.Description.ToString();

			item.SubItems[4].Text = type;
			item.SubItems[5].Text = description;
			updateDetails(changelist);
            // if Swarm is enabled, also refresh those fields
            if (Scm.Connection.Swarm.SwarmEnabled)
            {
                UpdateSwarmFields(item as P4ChangeTreeListViewItem);
            }
            try
			{
				bool isExpanded = item.Expanded;
				submittedTreeListView.BeginUpdate();
				if (isExpanded)
				{
					submittedTreeListView.CollapseNode(item);
				}
				int val = expandChangelistObject(item);
				if (isExpanded)
				{
					submittedTreeListView.ExpandNode(item);
				}
				return val;
			}
			finally
			{
				submittedTreeListView.EndUpdate();
			}
		}

		public int expandChangelistObject(TreeListViewItem item)
		{
			if (Scm == null)
			{
				// still null
				return 0;
			}
            
			item.ChildNodes.Clear();

            int maxFiles = Preferences.LocalSettings.GetInt("Number_files", 1000);

            TreeListViewItem newChildItem = null;
			P4.Changelist changeInfo = Scm.Connection.Repository.NewChangelist();

            // submitted changelist object
            if (item is P4ChangeTreeListViewItem)
			{
				int cl = Convert.ToInt32(item.Text);
				P4.Options opts = new P4.Options();
				opts["-s"] = null;
				changeInfo = Scm.GetChangelist(cl, opts);

				IList<P4.FileMetaData> files = changeInfo.Files;

                if (changeInfo.Files != null && changeInfo.Files.Count > 0)
                {
                    if (files.Count <= maxFiles)
                    {
                        foreach (P4.FileMetaData file in files)
                        {
                            newChildItem = new P4FileTreeListViewItem(item, string.Format("{0}#{1}", file.DepotPath.Path, file.HeadRev), file);
                            //((P4FileTreeListViewItem)newChildItem).FileData = file;
                            item.ChildNodes.Add(newChildItem);
                        }
                    }
                    else
                    {
                        string msg = string.Format(Resources.PendingChangelistsToolWindowControl_FileCount, files.Count); ;
                        P4ObjectTreeListViewItem message = new P4ObjectTreeListViewItem(item, msg, true);
                        item.ChildNodes.Add(message);
                    }
                }
				if (changeInfo.Jobs != null && changeInfo.Jobs.Count > 0)
				{
					foreach (string job in changeInfo.Jobs.Keys)
					{
                        newChildItem = new P4JobTreeListViewItem(item, job);
                        item.ChildNodes.Add(newChildItem);
					}
				}
//				submittedTreeListView.BuildTreeList();
			}
			return 0;
		}

		public void before(object sender, TreeListViewEventArgs args)
		{
			expandChangelistObject(args.Node);
		}

		private void getRevisionToolStripMenuItem_Click(object sender, EventArgs e)
		{
             if (submittedTreeListView.SelectedItems != null && submittedTreeListView.SelectedItems.Count > 0)
             {
                 TreeListViewItem selected = submittedTreeListView.SelectedItems[0] as TreeListViewItem;
                 if (selected != null)
                 {
                     P4.Changelist change = (P4.Changelist)selected.Tag;
                     change = Scm.GetChangelist(change.Id, null);
                     IList<P4.FileSpec> files = new List<P4.FileSpec>();
                     foreach (P4.FileMetaData fmd in change.Files)
                     {
                         P4.FileSpec file = new P4.FileSpec();
                         file.DepotPath = fmd.DepotPath;
                         files.Add(file);
                     }
                     bool isSolutionIncluded = false;
                     foreach (P4.FileSpec file in files)
                     {
                         if (file != null && file.LocalPath != null && file.LocalPath.Path != null &&
                             file.LocalPath.Path == Scm.SolutionFile)
                         {
                             isSolutionIncluded = true;
                             break;
                         }
                         else if ((file != null && file.LocalPath != null && file.LocalPath.Path != null) &&
                                  (file.LocalPath.Path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) ||
                                   file.LocalPath.Path.EndsWith(".jsproj", StringComparison.OrdinalIgnoreCase) ||
                                   file.LocalPath.Path.EndsWith(".vcxproj", StringComparison.OrdinalIgnoreCase) ||
                                   file.LocalPath.Path.EndsWith(".vcxproj.filters", StringComparison.OrdinalIgnoreCase)))
                         {
                             isSolutionIncluded = true;
                             break;
                         }
                     }
                     GetRevisionDlg dlg = new GetRevisionDlg(files, "changelist", change.Id.ToString(), Scm,
                                                             isSolutionIncluded);
                     dlg.ShowDialog();
                 }
             }
		}

        private void diffAgainstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (submittedTreeListView.SelectedItems != null && submittedTreeListView.SelectedItems.Count > 0)
            {
                TreeListViewItem selected = submittedTreeListView.SelectedItems[0] as TreeListViewItem;
                if (selected != null)
                {
                    P4.FileMetaData fmd = (P4.FileMetaData)selected.Tag;
                    P4.FileSpec fileSelectedRev = new P4.FileSpec();
                    fileSelectedRev.DepotPath = fmd.DepotPath;
                    fileSelectedRev.Version = new P4.Revision(fmd.HeadRev);
                    P4.FileSpec filePreviousRev = new P4.FileSpec();
                    filePreviousRev.DepotPath = fmd.DepotPath;
                    filePreviousRev.Version = new P4.Revision(fmd.HeadRev - 1);
                    IList<P4.FileSpec> files = new List<P4.FileSpec>();
                    files.Add(filePreviousRev);
                    files.Add(fileSelectedRev);
                    if (files != null)
                        Scm.Diff2Files(files);
                }
            }
        }

	    private void editSubmittedChangelistToolStripMenuItem_Click(object sender, EventArgs e)
		{
			{
				if (Scm == null)
				{
					// still null
					return;
				}
				if ((submittedTreeListView.SelectedItems != null) && (submittedTreeListView.SelectedItems.Count >= 1))
				{
					try
					{
						P4.Changelist change = submittedTreeListView.SelectedItems[0].Tag as P4.Changelist;
						TreeListViewItem selected = submittedTreeListView.SelectedItems[0] as TreeListViewItem;
						SubmittedChangelistDlg dlg = new SubmittedChangelistDlg(Scm);
					    if (change != null)
					    {
					        string changeText = change.Id.ToString();
					        if (change.Id < 1)
					        {
					            changeText = Resources.Changelist_Default;
					        }
					        P4.ServerMetaData smd = Scm.GetServerMetaData();
					        dlg.Text = string.Format(Resources.SubmittedChangelistsToolWindowControl_SubmittedChangelistDlgCaption,
					                                 changeText, smd.Address.Uri, Scm.Connection.Repository.Connection.UserName);
					    }
					    if (change != null) dlg.ChangeListId = change.Id;

					    if (dlg.ShowDialog() == DialogResult.Cancel)
						{
							refreshChangelistObject(selected);
							return;
						}

						refreshChangelistObject(selected);
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
			}
		}

        private void refreshSubmittedChangelistListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterBtn_Click(null, null);
        }

	    private void refreshSubmittedChangelistToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (submittedTreeListView.SelectedItems!=null && submittedTreeListView.SelectedItems.Count>0)
		    {
		        TreeListViewItem selected = submittedTreeListView.SelectedItems[0] as TreeListViewItem;
		        refreshChangelistObject(selected);
		    }
		}

		private void removeJobToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (submittedTreeListView.SelectedItems != null && submittedTreeListView.SelectedItems.Count > 0)
            {
				P4.Job job = new P4.Job();
				job.Id = submittedTreeListView.SelectedItems[0].Text;

				TreeListViewItem selected = submittedTreeListView.SelectedItems[0] as TreeListViewItem;
                if (selected != null)
                {
                    int cl = Convert.ToInt32(selected.ParentItem.Text);

                    P4.Connection con = Scm.Connection.Repository.Connection;
                    P4.Changelist change = new P4.Changelist(cl, true);
                    change.initialize(con);

                    P4.Options opts = new P4.Options(P4.FixJobsCmdFlags.Delete, -1, null);
                    IList<P4.Fix> fixes = change.FixJobs(opts, job);
                }

                P4.P4CommandResult result = Scm.Connection.Repository.Connection.LastResults;
				if (result.Success == true)
				{
				    if (selected != null) selected.Remove();
				}
			}
			return;
		}


        private void SubmittedChangelistsToolWindowControl_Load(object sender, EventArgs e)
        {
			submittedTreeListView.Columns[submittedTreeListView.Columns.Count-1].Width = -2;
            matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;

            if (Scm != null)
            {
                if (userCB.Text == "" &&
                    userCB.mruValues[1]==null)
                {
                    selectionChangedByLoad = true;
                    userCB.Text = Scm.Connection.User;
                }

                if (workspaceCB.Text == "" &&
                    workspaceCB.mruValues[1]==null)
                {
                    selectionChangedByLoad = true;
                    workspaceCB.Text = Scm.Connection.Workspace;
                }

                //if (userCB.Text == "" &&
                //    !(userCB.mruValues.Contains("")))
                //{
                //    selectionChangedByLoad = true;
                //    userCB.Text = Scm.User;
                //}

                //if (workspaceCB.Text == "" &&
                //    !(workspaceCB.mruValues.Contains("")))
                //{
                //    selectionChangedByLoad = true;
                //    workspaceCB.Text = Scm.Workspace;
                //}
            }
            refreshSubmittedList();
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
            //userCB.Text = Scm.Connection.Repository.Connection.UserName;
            //workspaceCB.Text = Scm.Connection.Repository.Connection.Client.Name;
            //filterBtn_Click(null, null);
		}

		private void submittedTreeListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			checkConnection();

			P4.Changelist change = null;

            if (submittedTreeListView.SelectedItems != null && submittedTreeListView.SelectedItems.Count > 0)
            {
				P4ChangeTreeListViewItem selected = submittedTreeListView.SelectedItems[0] as P4ChangeTreeListViewItem;
				string id = null;

				if (selected != null)
				{
					change = selected.ChangeData;
					id = selected.Text;
				}
				else
				{
					P4ObjectTreeListViewItem child = submittedTreeListView.SelectedItems[0] as P4ObjectTreeListViewItem;
					if ((child != null) && (child.ParentItem != null))
				    {
						selected = child.ParentItem as P4ChangeTreeListViewItem;
						change = selected.ChangeData;
						id = child.ParentItem.Text;
				    }
				}
                SelectedChangelist = change;

				if (changeTB.Text == id)
				{ 
					return; 
				}
				updateDetails(change);
			}
			
		}

		public void updateDetails(P4.Changelist change)
		{
			if (change == null)
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
			opts["-s"] = null;
			P4.Changelist changeInfo = Scm.GetChangelist(change.Id, opts);
			if (changeInfo == null)
			{
				return;
			}

			if (changeInfo.Id > 0)
			{ changeTB.Text = changeInfo.Id.ToString(); }
			else
			{ changeTB.Text = Resources.Changelist_Default; }
			changeTB.Refresh();

            DateTime local = changeInfo.ModifiedDate;

            // need a pref for local time, until then, don't do this:
            //local = TimeZone.CurrentTimeZone.ToLocalTime(local);

			if (Preferences.LocalSettings.GetBool("P4Date_format", true))
			{
				dateTB.Text = local.ToString("yyyy/MM/dd HH:mm:ss");
			}
			else
			{
				dateTB.Text = local.ToString();
			}

			dateTB.Refresh();
			workspaceTB2.Text = changeInfo.ClientId;
			workspaceTB2.Refresh();
			descriptionTB.Text = changeInfo.Description;
			descriptionTB.Refresh();
			UserTB2.Text = changeInfo.OwnerName;
			UserTB2.Refresh();

			if (changeInfo.Id <= 0)
            { statusTB.Text = "default"; }
			else
			{ statusTB.Text = "submitted"; }
			statusTB.Refresh();

			if (changeInfo.Id > 0)
			{ typeTB.Text = changeInfo.Type.ToString(); }
			else
			{ typeTB.Text = string.Empty; }
			typeTB.Refresh();

			if (changeInfo.Jobs != null)
			{
				foreach (string job in changeInfo.Jobs.Keys)
				{
					jobsTB.Text += job + "\r\n";
				}
			}
			jobsTB.Refresh();

			if (changeInfo.Files != null)
			{
                StringBuilder sb = new StringBuilder(changeInfo.Files.Count * 260);
                foreach (P4.FileMetaData file in changeInfo.Files)
                {
                    sb.AppendLine(string.Format("{0}#{1}", file.DepotPath.Path, file.HeadRev));
                }
                filesTB.Text = sb.ToString();
			}
			filesTB.Refresh();
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
				submittedTreeListView.Nodes.Clear();
				submittedTreeListView.BuildTreeList();
				changelistContextMenuStrip.Enabled = false;
				matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
				matchesLbl.Refresh();
				return;
			}
			filterBtn.Enabled = true;
			changelistContextMenuStrip.Enabled = true;
		}

		#region IDisposable Members

		public new void Dispose()
		{
			P4VsProvider.NewConnection -= newConection;
			base.Dispose();
		}

		#endregion

		private void submittedTreeListView_onMaxScroll(object sender, ScrollEventArgs e)
		{
			if ((maxItems > 0) && (submittedTreeListView.Nodes.Count >= maxItems))
			{
				maxItems += (int)Preferences.LocalSettings.GetInt("Number_specs", 100); ;
				refreshSubmittedList();
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

		private void submittedTreeListView_Click(object sender, EventArgs e)
		{
			checkConnection();
		}

		private void changelistContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			if (fromBrowser)
			{
				e.Cancel = true;
			}
			else
			{
				//clear the menu
				getRevisionToolStripMenuItem.Visible = false;
				diffAgainstToolStripMenuItem.Visible = false;
                fileHistoryStripMenuItem.Visible = false;
                timeLapseViewtoolStripMenuItem.Visible = false;

				toolStripSeparator1.Visible = false;

				requestReviewToolStripMenuItem.Visible = false;
				updateReviewToolStripMenuItem.Visible = false;
				openReviewInSwarmToolStripMenuItem.Visible = false;

				toolStripSeparator2.Visible = false;

				editSubmittedChangelistToolStripMenuItem.Visible = false;

				toolStripSeparator3.Visible = false;

				refreshSubmittedChangelistListToolStripMenuItem.Visible = false;
				refreshSubmittedChangelistToolStripMenuItem.Visible = false;
				removeJobToolStripMenuItem.Visible = false;

				if ((submittedTreeListView.SelectedItems == null) || (submittedTreeListView.SelectedItems.Count <= 0))
				{
					// nothing selected
					refreshSubmittedChangelistListToolStripMenuItem.Visible = true;
					refreshSubmittedChangelistListToolStripMenuItem.Enabled = true;
					return;
				}
				P4JobTreeListViewItem job = submittedTreeListView.SelectedItems[0] as P4JobTreeListViewItem;
				if (job != null)
				{
					//job selected
					removeJobToolStripMenuItem.Visible = true;
					removeJobToolStripMenuItem.Enabled = true;
					return;
				}
				P4FileTreeListViewItem file = submittedTreeListView.SelectedItems[0] as P4FileTreeListViewItem;
				if (file != null)
				{
					//file selected
					P4.FileMetaData fmd = file.Tag as P4.FileMetaData;
					if ((fmd.HeadRev < 2) || (fmd.Action == P4.FileAction.Delete || fmd.Action == P4.FileAction.MoveDelete
						|| fmd.Action == P4.FileAction.Purge))
					{
						e.Cancel = true;
					}
					else
					{
						diffAgainstToolStripMenuItem.Visible = true;
						diffAgainstToolStripMenuItem.Enabled = true;
                        fileHistoryStripMenuItem.Visible = true;
                        fileHistoryStripMenuItem.Enabled = true;
                        timeLapseViewtoolStripMenuItem.Visible = true;
                        timeLapseViewtoolStripMenuItem.Enabled = true;
                    }
                    return;
				}

				P4ChangeTreeListViewItem change = submittedTreeListView.SelectedItems[0] as P4ChangeTreeListViewItem;

				if (change == null)
				{
					//shouldn't happen but....
					return;
				}
				getRevisionToolStripMenuItem.Visible = true;
				getRevisionToolStripMenuItem.Enabled = true;

				toolStripSeparator1.Visible = true;

                bool swarmEnabled = Scm.Connection.Swarm.SwarmEnabled;
				if (swarmEnabled)
				{
					bool isAttachedToReview = change.ReviewData != null;

					if (isAttachedToReview)
					{
                        // already attached to review, so can only open that review
                        openReviewInSwarmToolStripMenuItem.Image = Images.swarm_contextmenu16_16;
                        openReviewInSwarmToolStripMenuItem.Visible = true;
						openReviewInSwarmToolStripMenuItem.Text = string.Format(Resources.PendingChangelistsToolWindowControl_MenuItemOpenSwarmReview, change.ReviewData.id);
					}
					else
					{
						// not already attached to review, so can request anew or update a current review
						requestReviewToolStripMenuItem.Visible = true;
                        requestReviewToolStripMenuItem.Image = Images.swarm_contextmenu16_16;
                        updateReviewToolStripMenuItem.Visible = true;
					}

					toolStripSeparator2.Visible = true;
				}
				editSubmittedChangelistToolStripMenuItem.Visible = true;
				editSubmittedChangelistToolStripMenuItem.Enabled = true;
				editSubmittedChangelistToolStripMenuItem.Text = string.Format(Resources.SubmittedChangelistsToolWindowControl_EditSubmittedChangelistMenuItemText, change.ChangeData.Id);

				toolStripSeparator3.Visible = true;

				refreshSubmittedChangelistListToolStripMenuItem.Visible = true;
				refreshSubmittedChangelistListToolStripMenuItem.Enabled = true;
				refreshSubmittedChangelistToolStripMenuItem.Visible = true;
				refreshSubmittedChangelistToolStripMenuItem.Enabled = true;
				refreshSubmittedChangelistToolStripMenuItem.Text = string.Format(Resources.SubmittedChangelistsToolWindowControl_RefreshSubmittedChangelistMenuItemText1, change.ChangeData.Id);
			}
		}

        private void submittedTreeListView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            if (e.NewDisplayIndex == 0 | e.OldDisplayIndex == 0)
            {
                e.Cancel = true;
            }
        }

        private void submittedTreeListView_ColumnClick(object sender, ColumnClickEventArgs e)
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
                submittedTreeListView.Columns[lvwColumnSorter.SortColumn].Text =
                    submittedTreeListView.Columns[lvwColumnSorter.SortColumn].Text;

                // Set the column number that is to be sorted; default to ascending.

                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.submittedTreeListView.Sort();
        }

        public event MouseEventHandler TreeListViewDoubleClicked;

        private void submittedTreeListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (TreeListViewDoubleClicked!=null)
            {
                TreeListViewDoubleClicked(sender, e);
            }
        }

        private void workspaceCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectionChangedByLoad == false)
            {
                filterBtn.PerformClick();
            }
            selectionChangedByLoad = false;
        }

        private void workspaceCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                filterBtn.PerformClick();
        }

        private void userCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectionChangedByLoad == false)
            {
                filterBtn.PerformClick();
            }
            selectionChangedByLoad = false;
        }

        private void userCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                filterBtn.PerformClick();
        }

        private void pathCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectionChangedByLoad == false)
            {
                filterBtn.PerformClick();
            }
            selectionChangedByLoad = false;
        }

        private void pathCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                filterBtn.PerformClick();
        }

		private void UpdateSwarmFields(P4ChangeTreeListViewItem li)
		{
			int id = li.ChangeData.Id;
            // adding a try catch in the edge case that a swarm config
            // or connection was broken prior to the refresh
            try
            {
                SwarmApi.SwarmServer.Review r = Scm.Connection.Swarm.IsChangelistAttachedToReview(id);
                if (r == null)
                {
                    li.SubItems[1].Text = string.Empty;
                    li.SubItems[2].Text = string.Empty;
                }
                else
                {
                    li.SubItems[1].Text = r.id.ToString();
                    li.SubItems[2].Text = r.stateLabel;
                }
            }
            catch
            {
                // do nothing, error messages have already been raised
                // where needed, refresh fails
            }
		}

		private void requestReviewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			P4ChangeTreeListViewItem tlvi = submittedTreeListView.SelectedItems[0] as P4ChangeTreeListViewItem;
			if (tlvi != null)
			{
				CreateSwarmReviewDlg.RequestReview(Scm, tlvi.ChangeData.Id);
				UpdateSwarmFields(tlvi);
			}
		}

		private void updateReviewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			P4ChangeTreeListViewItem li = submittedTreeListView.SelectedItems[0] as P4ChangeTreeListViewItem;
			if (li != null)
			{
				if (li.ReviewData != null)
				{
				P4.Changelist changelist = li.Tag as P4.Changelist;
					CreateSwarmReviewDlg.RefreshReview(Scm, li.ChangeData.Id, li.ReviewData.id, li.ReviewData.description);
				}
				else
				{
					CreateSwarmReviewDlg.UpdateReview(Scm, li.ChangeData.Id);
				}
				UpdateSwarmFields(li);
			}
		}

		private void openReviewInSwarmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			P4ChangeTreeListViewItem li = submittedTreeListView.SelectedItems[0] as P4ChangeTreeListViewItem;
			if ((li != null) && (li.ReviewData != null))
			{
                SwarmServer sw = Scm.Connection.Swarm.GetSwarmServer();
				sw.ShowReviewInBrowser(li.ReviewData.id);
			}
		}

        private void timeLapseViewtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (submittedTreeListView.SelectedItems != null)
            {
                TreeListViewItem selected = submittedTreeListView.SelectedItems[0] as TreeListViewItem;
                if (selected != null)
                {
                    P4.FileMetaData fmd = (P4.FileMetaData)selected.Tag;
                    Scm.SccService.ScmProvider.LaunchTimeLapseView(fmd.DepotPath.Path);
                }
            }
        }

        private void fileHistoryStripMenuItem_Click(object sender, EventArgs e)
        {
            if (submittedTreeListView.SelectedItems != null)
            {
                TreeListViewItem selected = submittedTreeListView.SelectedItems[0] as TreeListViewItem;
                if (selected != null)
                {
                    P4.FileMetaData fmd = (P4.FileMetaData)selected.Tag;
                    IList<string> files = new List<string>();
                    files.Add(fmd.DepotPath.Path);
                    Scm.SccService._P4VsProvider.P4VsViewHistoryToolWindowExt(files);
                }
            }
        }
    }
}
