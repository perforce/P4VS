
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
using Perforce.P4;
using Perforce.P4VS.UI;
using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Label = System.Windows.Forms.Label;
using System.Linq;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	/// <summary>
	/// Summary description for P4ToolWindowControl.
	/// </summary>
	public class JobsToolWindowControl : P4ToolWindowControlBase
	{
		private Perforce.I18nControls.GridLayoutSubpanel jobsPanel1;
		private Perforce.I18nControls.GridSplitContainer splitContainer1;
		private Perforce.I18nControls.GridButton filterBtn;
		private Perforce.I18nControls.GridLabel pathLbl;
		private Perforce.I18nControls.GridLabel keywordsLbl;
		private Perforce.I18nControls.GridLabel matchesLbl;
        private P4ObjectTreeListView jobsListView;
		private Label jobLbl;
		private TextBox jobTB;
		private Panel detailsPanel;
		private Perforce.I18nControls.GridGroupBox dividerGB;
		private ContextMenuStrip jobsContextMenuStrip;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem refreshJobListToolStripMenuItem;
		private ToolStripMenuItem refreshJobToolStripMenuItem;
		private UI.ThreadMonitorControl threadMonitorControl1;
		
		public string KeywordsText { get; set; }
		public string PathText { get; set; }
        private bool selectionChangedByLoad = false;
		public bool fromBrowser { get; set; }

		string refreshJobToolStripMenuItemFmt;
		string editJobToolStripMenuItemFmt;
        private ToolStripMenuItem newJobToolStripMenuItem;
        private ToolStripMenuItem editJobToolStripMenuItem;
		internal Perforce.I18nControls.GridFilterComboBox keywordsCB;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
        internal Perforce.I18nControls.GridFilterComboBox pathCB;

        private delegate void setFilterBtnDelegate(bool filter);

        private void setFilterBtnBool(bool enabled)
        {
            this.filterBtn.Enabled = enabled;
        }

        private IContainer components;

		private ImageList imageList1;

		public JobsToolWindowControl()
        {
			PreferenceKey = "JobsToolWindowControl";

			GotJobFields = false;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			base.Initialize();

			// if the mru lists have not been loaded, see if the old preference exists
            if (Preferences.LocalSettings != null)
            {
                if ((pathCB.mruLoaded == false) && (Preferences.LocalSettings.ContainsKey("RecentJobsPaths")))
                {
                    MRUList value = (MRUList) Preferences.LocalSettings["RecentJobsPaths"];
                    if (value != null)
                    {
                        pathCB.mruValues = value.Clone();
                        pathCB.mruLoaded = true;
                    }
                }
                if ((keywordsCB.mruLoaded == false) && (Preferences.LocalSettings.ContainsKey("RecentJobsKeywords")))
                {
                    MRUList value = (MRUList) Preferences.LocalSettings["RecentJobsKeywords"];
                    if (value != null)
                    {
                        keywordsCB.mruValues = value.Clone();
                        keywordsCB.mruLoaded = true;
                    }
                }
            }

		    imageList1 = new System.Windows.Forms.ImageList(this.components);
			// 
			// imageList1
			// 
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.Add("jobs_icon.png", Images.jobs_icon);

			this.jobsListView.LargeImageList = this.imageList1;
			this.jobsListView.SmallImageList = this.imageList1;
			
			KeywordsText = keywordsCB.Text;
			PathText = pathCB.Text;
			threadMonitorControl1.Visible = false;

			refreshJobToolStripMenuItemFmt = refreshJobToolStripMenuItem.Text;
			editJobToolStripMenuItemFmt = editJobToolStripMenuItem.Text;

            P4VsProvider.NewConnection += newConnection;
#if VS2012
            if (!DesignMode)
            {
                base.InitThemeManager();
            }
#endif
            checkConnection();
		}

		public JobsToolWindowControl(P4ScmProvider scm)
			:base (scm)
		{
			PreferenceKey = "JobsToolWindowControl";

			GotJobFields = false;
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			base.Initialize();

			//newConection = new P4VsProvider.NewConnectionDelegate(OnNewConnection);
			//P4VsProvider.NewConnection += newConection;
#if VS2012
            if (!DesignMode)
            {
                base.InitThemeManager();
            }
#endif
            checkConnection();
		}

#pragma warning disable 649 // Not clear if we can remove this even though it seems not used.
		P4VsProvider.NewConnectionDelegate newConnection;
#pragma warning restore 649

        public P4.FormSpec jobspec { get; set; }

		public override void OnNewConnection(P4ScmProvider newScm)
		{
            Scm = newScm;
            clearList();
            clearDetails();

            if ((Scm == null) || (Scm.Connection.Disconnected))
            {
                matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
                filterBtn.Enabled = false;
                jobsListView.Enabled = true;
                return;
            }

            P4.Options opts = new P4.Options();
            opts["-o"] = null;
            jobspec = Scm.Connection.Repository.GetFormSpec(opts, "job");
            IList<P4.Job> jobFields = null;
            try
            {
                jobFields = Scm.getJobs(P4.JobsCmdFlags.None, null, 1, null);
            }
            
            catch
            {
                matchesLbl.Text = Resources.JobsToolWindowControl_NoMatches;
                jobsListView.Enabled = true;
                filterBtn.Enabled = (Scm != null) && Scm.Connected;
                return;
            }

            if (jobFields!=null&& jobFields.Count>0)
            {
            GetJobFields(jobFields[0]);
            }
            

		    filterBtn.Enabled = (Scm != null) && Scm.Connected;

			refreshJobsList();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JobsToolWindowControl));
			this.splitContainer1 = new Perforce.I18nControls.GridSplitContainer();
			this.threadMonitorControl1 = new Perforce.P4VS.UI.ThreadMonitorControl();
			this.jobsListView = new Perforce.P4VS.P4ObjectTreeListView();
			this.jobsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.newJobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editJobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.refreshJobListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshJobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.detailsPanel = new System.Windows.Forms.Panel();
			this.jobTB = new System.Windows.Forms.TextBox();
			this.jobLbl = new System.Windows.Forms.Label();
			this.jobsPanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
			this.pathCB = new Perforce.I18nControls.GridFilterComboBox();
			this.keywordsCB = new Perforce.I18nControls.GridFilterComboBox();
			this.dividerGB = new Perforce.I18nControls.GridGroupBox();
			this.matchesLbl = new Perforce.I18nControls.GridLabel();
			this.filterBtn = new Perforce.I18nControls.GridButton();
			this.pathLbl = new Perforce.I18nControls.GridLabel();
			this.keywordsLbl = new Perforce.I18nControls.GridLabel();
			this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.jobsContextMenuStrip.SuspendLayout();
			this.detailsPanel.SuspendLayout();
			this.jobsPanel1.SuspendLayout();
			this.gridLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.BackColor = System.Drawing.SystemColors.ScrollBar;
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer1.Column = 0;
			this.splitContainer1.ColumnsSpanned = 0;
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitContainer1.Panel1.Controls.Add(this.threadMonitorControl1);
			this.splitContainer1.Panel1.Controls.Add(this.jobsListView);
			// 
			// splitContainer1.Panel2
			// 
			resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
			this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitContainer1.Panel2.Controls.Add(this.detailsPanel);
			this.splitContainer1.Row = 1;
			this.splitContainer1.RowsSpanned = 0;
			this.splitContainer1.YOffset = 0;
			// 
			// threadMonitorControl1
			// 
			this.threadMonitorControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.threadMonitorControl1.CancelPressed = false;
			resources.ApplyResources(this.threadMonitorControl1, "threadMonitorControl1");
			this.threadMonitorControl1.Maximum = 100;
			this.threadMonitorControl1.Name = "threadMonitorControl1";
			this.threadMonitorControl1.Step = 1;
			this.threadMonitorControl1.Value = 50;
			// 
			// jobsListView
			// 
			this.jobsListView._maxLineOffset = 0;
			this.jobsListView.ActionColumn = -1;
			this.jobsListView.AllowColumnReorder = true;
			this.jobsListView.ContextMenuStrip = this.jobsContextMenuStrip;
			resources.ApplyResources(this.jobsListView, "jobsListView");
			this.jobsListView.EnableIconOverlays = false;
			this.jobsListView.EnableSorting = true;
			this.jobsListView.FullRowSelect = true;
			this.jobsListView.GridLines = true;
			this.jobsListView.Name = "jobsListView";
			this.jobsListView.OverlayOffset = 0;
			this.jobsListView.RootCheckBoxes = false;
			this.jobsListView.ScrollPosition = 0;
			this.jobsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.jobsListView.TreeView = false;
			this.jobsListView.UseClassicImageList = false;
			this.jobsListView.UseCompatibleStateImageBehavior = false;
			this.jobsListView.View = System.Windows.Forms.View.Details;
			this.jobsListView.onMaxScroll += new System.Windows.Forms.ScrollEventHandler(this.jobsListView_onMaxScroll);
			this.jobsListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.jobsListView_ColumnClick);
			this.jobsListView.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.jobsListView_ColumnReordered);
			this.jobsListView.SelectedIndexChanged += new System.EventHandler(this.jobsListView_SelectedIndexChanged);
			// 
			// jobsContextMenuStrip
			// 
			this.jobsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newJobToolStripMenuItem,
            this.editJobToolStripMenuItem,
            this.toolStripSeparator1,
            this.refreshJobListToolStripMenuItem,
            this.refreshJobToolStripMenuItem});
			this.jobsContextMenuStrip.Name = "changelistContextMenuStrip";
			resources.ApplyResources(this.jobsContextMenuStrip, "jobsContextMenuStrip");
			this.jobsContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.jobsContextMenuStrip_Opening);
			// 
			// newJobToolStripMenuItem
			// 
			resources.ApplyResources(this.newJobToolStripMenuItem, "newJobToolStripMenuItem");
			this.newJobToolStripMenuItem.Name = "newJobToolStripMenuItem";
			this.newJobToolStripMenuItem.Click += new System.EventHandler(this.newJobToolStripMenuItem_Click);
			// 
			// editJobToolStripMenuItem
			// 
			resources.ApplyResources(this.editJobToolStripMenuItem, "editJobToolStripMenuItem");
			this.editJobToolStripMenuItem.Name = "editJobToolStripMenuItem";
			this.editJobToolStripMenuItem.Click += new System.EventHandler(this.editJobToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.toolStripSeparator1.ForeColor = System.Drawing.Color.Black;
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// refreshJobListToolStripMenuItem
			// 
			resources.ApplyResources(this.refreshJobListToolStripMenuItem, "refreshJobListToolStripMenuItem");
			this.refreshJobListToolStripMenuItem.Name = "refreshJobListToolStripMenuItem";
			this.refreshJobListToolStripMenuItem.Click += new System.EventHandler(this.refreshJobListToolStripMenuItem_Click);
			// 
			// refreshJobToolStripMenuItem
			// 
			resources.ApplyResources(this.refreshJobToolStripMenuItem, "refreshJobToolStripMenuItem");
			this.refreshJobToolStripMenuItem.Name = "refreshJobToolStripMenuItem";
			this.refreshJobToolStripMenuItem.Click += new System.EventHandler(this.refreshJobToolStripMenuItem_Click);
			// 
			// detailsPanel
			// 
			resources.ApplyResources(this.detailsPanel, "detailsPanel");
			this.detailsPanel.BackColor = System.Drawing.SystemColors.ControlLight;
			this.detailsPanel.Controls.Add(this.jobTB);
			this.detailsPanel.Controls.Add(this.jobLbl);
			this.detailsPanel.Name = "detailsPanel";
			// 
			// jobTB
			// 
			resources.ApplyResources(this.jobTB, "jobTB");
			this.jobTB.Name = "jobTB";
			this.jobTB.ReadOnly = true;
			// 
			// jobLbl
			// 
			resources.ApplyResources(this.jobLbl, "jobLbl");
			this.jobLbl.Name = "jobLbl";
			// 
			// jobsPanel1
			// 
			resources.ApplyResources(this.jobsPanel1, "jobsPanel1");
			this.jobsPanel1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.jobsPanel1.Column = 0;
			this.jobsPanel1.ColumnsSpanned = 0;
			this.jobsPanel1.Controls.Add(this.pathCB);
			this.jobsPanel1.Controls.Add(this.keywordsCB);
			this.jobsPanel1.Controls.Add(this.dividerGB);
			this.jobsPanel1.Controls.Add(this.matchesLbl);
			this.jobsPanel1.Controls.Add(this.filterBtn);
			this.jobsPanel1.Controls.Add(this.pathLbl);
			this.jobsPanel1.Controls.Add(this.keywordsLbl);
			this.jobsPanel1.EnableDesignerGrid = true;
			this.jobsPanel1.EnableDesignerLayout = true;
			this.jobsPanel1.MinimumColumnWidth = 10;
			this.jobsPanel1.MinimumRowHeight = 10;
			this.jobsPanel1.Name = "jobsPanel1";
			this.jobsPanel1.Row = 0;
			this.jobsPanel1.RowsSpanned = 0;
			this.jobsPanel1.YOffset = 0;
			// 
			// pathCB
			// 
			resources.ApplyResources(this.pathCB, "pathCB");
			this.pathCB.Column = 1;
			this.pathCB.ColumnsSpanned = 0;
			this.pathCB.FormattingEnabled = true;
			this.pathCB.Name = "pathCB";
			this.pathCB.Row = 1;
			this.pathCB.RowsSpanned = 0;
			this.pathCB.YOffset = 0;
			this.pathCB.SelectedIndexChanged += new System.EventHandler(this.pathCB_SelectedIndexChanged);
			this.pathCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pathCB_KeyDown);
			// 
			// keywordsCB
			// 
			resources.ApplyResources(this.keywordsCB, "keywordsCB");
			this.keywordsCB.Column = 1;
			this.keywordsCB.ColumnsSpanned = 0;
			this.keywordsCB.FormattingEnabled = true;
			this.keywordsCB.Name = "keywordsCB";
			this.keywordsCB.Row = 0;
			this.keywordsCB.RowsSpanned = 0;
			this.keywordsCB.YOffset = 2;
			this.keywordsCB.SelectedIndexChanged += new System.EventHandler(this.keywordsCB_SelectedIndexChanged);
			this.keywordsCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keywordsCB_KeyDown);
			// 
			// dividerGB
			// 
			resources.ApplyResources(this.dividerGB, "dividerGB");
			this.dividerGB.Column = 2;
			this.dividerGB.ColumnsSpanned = 0;
			this.dividerGB.Name = "dividerGB";
			this.dividerGB.Row = 0;
			this.dividerGB.RowsSpanned = 1;
			this.dividerGB.TabStop = false;
			this.dividerGB.YOffset = 0;
			// 
			// matchesLbl
			// 
			resources.ApplyResources(this.matchesLbl, "matchesLbl");
			this.matchesLbl.AutoEllipsis = true;
			this.matchesLbl.Column = 3;
			this.matchesLbl.ColumnsSpanned = 0;
			this.matchesLbl.Name = "matchesLbl";
			this.matchesLbl.Row = 1;
			this.matchesLbl.RowsSpanned = 0;
			this.matchesLbl.YOffset = 4;
			// 
			// filterBtn
			// 
			resources.ApplyResources(this.filterBtn, "filterBtn");
			this.filterBtn.Column = 3;
			this.filterBtn.ColumnsSpanned = 0;
			this.filterBtn.Name = "filterBtn";
			this.filterBtn.Row = 0;
			this.filterBtn.RowsSpanned = 0;
			this.filterBtn.UseVisualStyleBackColor = true;
			this.filterBtn.YOffset = 1;
			this.filterBtn.EnabledChanged += new System.EventHandler(this.filterBtn_EnabledChanged);
			this.filterBtn.Click += new System.EventHandler(this.filterBtn_Click);
			// 
			// pathLbl
			// 
			resources.ApplyResources(this.pathLbl, "pathLbl");
			this.pathLbl.Column = 0;
			this.pathLbl.ColumnsSpanned = 0;
			this.pathLbl.Name = "pathLbl";
			this.pathLbl.Row = 1;
			this.pathLbl.RowsSpanned = 0;
			this.pathLbl.YOffset = 4;
			// 
			// keywordsLbl
			// 
			resources.ApplyResources(this.keywordsLbl, "keywordsLbl");
			this.keywordsLbl.Column = 0;
			this.keywordsLbl.ColumnsSpanned = 0;
			this.keywordsLbl.Name = "keywordsLbl";
			this.keywordsLbl.Row = 0;
			this.keywordsLbl.RowsSpanned = 0;
			this.keywordsLbl.YOffset = 6;
			// 
			// gridLayoutPanel1
			// 
			this.gridLayoutPanel1.Controls.Add(this.jobsPanel1);
			this.gridLayoutPanel1.Controls.Add(this.splitContainer1);
			resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
			this.gridLayoutPanel1.EnableDesignerGrid = false;
			this.gridLayoutPanel1.EnableDesignerLayout = false;
			this.gridLayoutPanel1.MinimumColumnWidth = 10;
			this.gridLayoutPanel1.MinimumRowHeight = 10;
			this.gridLayoutPanel1.Name = "gridLayoutPanel1";
			// 
			// JobsToolWindowControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Controls.Add(this.gridLayoutPanel1);
			this.MinimumSize = new System.Drawing.Size(583, 572);
			this.Name = "JobsToolWindowControl";
			this.Load += new System.EventHandler(this.JobsToolWindowControl_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.jobsContextMenuStrip.ResumeLayout(false);
			this.detailsPanel.ResumeLayout(false);
			this.detailsPanel.PerformLayout();
			this.jobsPanel1.ResumeLayout(false);
			this.jobsPanel1.PerformLayout();
			this.gridLayoutPanel1.ResumeLayout(false);
			this.gridLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private object SyncRoot = new object();

		private void clearDetails()
		{
			foreach (Control ctrl in detailsPanel.Controls)
				if (ctrl is TextBox)
				{
					ctrl.Text = string.Empty;
				}
		}

        private void clearList()
        {
            if (jobsListView.InvokeRequired)
            {
                jobsListView.Invoke(new JobsTreeListViewDelegate(this.jobsListView.Items.Clear));

            }
            else
            {
                jobsListView.Items.Clear();
            }
        }

        private void clearColumns()
        {
            if (jobsListView.InvokeRequired)
            {
                jobsListView.Invoke(new JobsTreeListViewDelegate(this.jobsListView.Columns.Clear));

            }
            else
            {
                jobsListView.Columns.Clear();
            }
        }

        public ConnectionData ConnectionInfo { get; private set; }

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
            maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
			if (maxItems == 0)
			{
				maxItems = -1;
			}

            if (jobsListView.InvokeRequired)
            {
                jobsListView.Invoke(new JobsTreeListViewDelegate(this.jobsListView.Items.Clear));
                jobsListView.Invoke(new JobsTreeListViewDelegate(this.jobsListView.Nodes.Clear));

            }
            else
            {
                jobsListView.Items.Clear();
                jobsListView.Nodes.Clear();
            }

			refreshJobsList();
		}

		public P4.Job SelectedJob { get; private set; }

		public IList<P4.Job> SelectedJobs { get; private set; }

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
                clearList();
                clearDetails();
                jobsListView.BuildTreeList();
                jobsContextMenuStrip.Enabled = false;
                matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
				return;
			}
			filterBtn.Enabled = true;
			jobsContextMenuStrip.Enabled = true;
		}

		public void refreshJobsList()
		{
            filterBtn.Enabled = false;
            jobsListView.Enabled = false;
            checkConnection();
			clearDetails();

            if (FillInListProc != null)
			{
				if (FillInListProc.IsAlive)
				{
					threadMonitorControl1.CancelPressed = true;
					FillInListProc.Join(1000);
				}
				threadMonitorControl1.Hide();
				FillInListProc = null;
			}

            if ((Scm == null) || (Scm.Connection.Disconnected))
			{
				matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
                filterBtn.Enabled = false;
                jobsListView.Enabled = true;
                return;
			}

			KeywordsText = keywordsCB.Text;
			PathText = pathCB.Text;
			threadMonitorControl1.Visible = false;

            if (Scm != null)
            {
                FillInListProc = new Thread(new ThreadStart(AsyncPopulateListView));
                FillInListProc.IsBackground = true;

                FillInListProc.Start();
            }
		    jobsListView.Enabled = true;

            SaveControlSettings();

            return;
		}

        private delegate void JobsTreeListViewDelegate();
        private delegate void JobsTreeListViewItemDelegate(TreeListViewItem item);
        private delegate ListViewItem JobsTreeListViewItemDelegate2(TreeListViewItem item);
        private delegate void ReplaceJobsTreeListViewItemDelegate(int index,TreeListViewItem item);

        private void replaceJobListViewItem (int index, TreeListViewItem item)
        {
            jobsListView.Items[index] = item;
        }

        private delegate void setStringPropertyDelegate(string str);

        private void setJobMatchesLblText(string matches)
        {
            this.matchesLbl.Text = matches;
        }

        private delegate void setBoolPropertyDelegate(bool b);

        private void setJobListViewEnabled(bool enabled)
        {
            jobsListView.Enabled = enabled;
        }

        private void AsyncPopulateListView()
        {
            bool threadAborted = false;
            if (filterBtn.IsHandleCreated)
            {
                if (filterBtn.InvokeRequired)
                {
                    this.filterBtn.BeginInvoke(new setFilterBtnDelegate(setFilterBtnBool), false);
                }
                else
                {
                    filterBtn.Enabled = false;
                }
            }
            try
            {
                lock (SyncRoot)
                {
                    try
                    {
                        if (Scm == null)
                        {
                            // still null
                            return;
                        }

                        Scm.Connection.Repository.Connection.getP4Server().ProgramName = "P4VS";
                        Scm.Connection.Repository.Connection.getP4Server().ProgramVersion = Versions.product();

                        P4.JobsCmdFlags flags = P4.JobsCmdFlags.None;
                        string keywords = null;
                        KeywordsText = KeywordsText.Trim();
                        if (KeywordsText.Length > 0)
                        {
                            keywords = KeywordsText;
                        }

                        PathText = PathText.Trim();

                        P4.FileSpec path = new P4.FileSpec();
                        if (string.IsNullOrEmpty(PathText))
                        {
                            path = null;
                        }

                        else
                        {
                            if (PathText.StartsWith("//"))
                            {
                                path.DepotPath = new P4.DepotPath(PathText);
                            }
                            else
                            {
                                path.LocalPath = new P4.LocalPath(PathText);
                            }
                        }

                        if (maxItems == 0)
                        {
                            maxItems = -1;
                        }
                        IList<P4.Job> jobList = null;
                        try
                        {
                            jobList = Scm.getJobs(flags, keywords, maxItems, path);
                        }
                        catch (ThreadAbortException)
                        {
                            threadAborted = true;
                            Thread.ResetAbort();
                        }
                        TreeListViewItem it = new TreeListViewItem();
                        if ((jobList == null) || (jobList.Count <= 0))
                        {
                            string noItemsAvailable = Resources.JobsToolWindowControl_NoItemsAvailable;
                            it = new TreeListViewItem(null, noItemsAvailable, true);

                            if (jobsListView.InvokeRequired)
                            {
                                jobsListView.Invoke(new JobsTreeListViewDelegate(this.jobsListView.Items.Clear));
                                jobsListView.Invoke(new JobsTreeListViewItemDelegate2(this.jobsListView.Items.Add), it);
                                matchesLbl.Invoke(new setStringPropertyDelegate(setJobMatchesLblText),
                                                  Resources.JobsToolWindowControl_NoMatches);
                                this.filterBtn.Invoke(new setFilterBtnDelegate(setFilterBtnBool), ((Scm != null) && (Scm.Connected)));
                            }
                            else
                            {
                                jobsListView.Items.Clear();
                                this.jobsListView.Items.Add(it);
                                this.matchesLbl.Text = Resources.JobsToolWindowControl_NoMatches;
                                filterBtn.Enabled = ((Scm != null) && (Scm.Connected));
                            }
                            return;
                            
                        }
                        threadMonitorControl1.Value = 0;
                        threadMonitorControl1.CancelPressed = false;

                        int cnt = 0;

                        try
                        {
                            if (jobsListView.InvokeRequired)
                            {
                                jobsListView.Invoke(new JobsTreeListViewDelegate(jobsListView.BeginUpdate));
                                jobsListView.Invoke(new setBoolPropertyDelegate(setJobListViewEnabled), false);

                            }
                            else
                            {
                                jobsListView.BeginUpdate();
                                jobsListView.Enabled = false;
                            }
                            threadMonitorControl1.Show(jobList.Count);
                            for (int idx = 0; idx < jobList.Count; idx++)
                            {
                                ++cnt;
                                threadMonitorControl1.Value = cnt;

                                it = new TreeListViewItem(null, " " + jobList[idx].Id, false);
                                it.ImageIndex = 0;

                                it.Tag = jobList[idx];

                                for (int idx2 = 1; idx2 < jobsListView.Columns.Count; idx2++)
                                {
                                    string key = jobsListView.Columns[idx2].Text;
                                    if (jobList[idx].ContainsKey(key))
                                    {
                                        foreach (KeyValuePair<string, object> obj in jobList[idx])
                                        {
                                            if (obj.Key.Equals(key))
                                            {

                                                if ((jobspec.GetSpecFieldDataType(jobspec, obj.Key.ToString()) ==
                                                     P4.SpecFieldDataType.Date)
                                                    && p4Date() == false)
                                                {
                                                    DateTime d;
                                                    DateTime.TryParse(obj.Value.ToString(), out d);
                                                    it.SubItems.Add(d.ToString("MM/d/yyyy h:mm:ss tt"));
                                                }
                                                else
                                                {
                                                    it.SubItems.Add(obj.Value.ToString());
                                                }
                                            }
                                            if (threadMonitorControl1.CancelPressed)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        it.SubItems.Add(string.Empty);
                                    }
                                    if (threadMonitorControl1.CancelPressed)
                                    {
                                        break;
                                    }
                                }

                                if (jobsListView.InvokeRequired)
                                {
                                    if (idx < jobsListView.Items.Count)
                                    {

                                        jobsListView.Invoke(
                                            new ReplaceJobsTreeListViewItemDelegate(replaceJobListViewItem), idx, it);
                                    }
                                    else
                                    {
                                        jobsListView.Invoke(
                                            new JobsTreeListViewItemDelegate2(this.jobsListView.Items.Add),
                                            it);
                                    }
                                }
                                else
                                {
                                    if (idx < jobsListView.Items.Count)
                                    {
                                        jobsListView.Items[idx] = it;
                                    }
                                    else
                                    {
                                        this.jobsListView.Items.Add(it);
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
                            if (jobsListView.InvokeRequired)
                            {
                                jobsListView.Invoke(new JobsTreeListViewDelegate(jobsListView.EndUpdate));
                                jobsListView.Invoke(new setBoolPropertyDelegate(setJobListViewEnabled), true);
                            }
                            else
                            {
                                jobsListView.EndUpdate();
                                jobsListView.Enabled = true;
                            }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        threadAborted = true;
                        Thread.ResetAbort();
                    }

                    try
                    {
                        threadMonitorControl1.Hide();
                        FillInListProc = null;

                        if (!threadAborted || threadMonitorControl1.CancelPressed)
                        {
                            //user canceled, not aborted by new request
                            string itemCountStr = Resources.JobsToolWindowControl_1Match;
                            if (jobsListView.Items.Count > 1)
                            {
                                if (jobsListView.Items.Count == maxItems)
                                {
                                    itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches,
                                                                 jobsListView.Items.Count + "+");
                                }
                                else
                                {
                                    itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches,
                                                                 jobsListView.Items.Count);
                                }
                            }

                            if (this.matchesLbl.InvokeRequired)
                            {
                                this.matchesLbl.BeginInvoke(new setStringPropertyDelegate(setJobMatchesLblText), itemCountStr);
                            }
                            else
                            {
                                this.matchesLbl.Text = itemCountStr;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            finally
            {
                if (jobsListView.InvokeRequired)
                {
                    this.filterBtn.BeginInvoke(new setFilterBtnDelegate(setFilterBtnBool), ((Scm != null) && (Scm.Connected)));
                }
                else
                {
                    filterBtn.Enabled = ((Scm != null) && (Scm.Connected));
                }
            }
        }

	    Thread FillInListProc = null;

		 private void JobsToolWindowControl_Load(object sender, EventArgs e)
		{

			this.matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
			this.jobsListView.OwnerDraw = true;
            if (Scm != null)
            {
                OnNewConnection(Scm);
                //refreshJobsList();

            }

           
		 }

		 public static bool GotJobFields {get; set;}
		 public void GetJobFields(P4.Job job)
		 {
			 if (GotJobFields == true)
			 {
				 return;
			 }

             clearColumns();
             clearList();
             clearDetails();
			 
			 detailsPanel.Controls.Clear();

			 string spec = string.Empty;
			 if (job.ContainsKey("specdef"))
			 {
				 spec = job["specdef"].ToString();
				 spec = spec.TrimEnd(';');
			 }
			 else
			 {
				 return;
			 }

			 string[] fields = Regex.Split(spec, ";;");

			 int singleline = 0;
			 foreach (string field in fields)
			 {
				 if (field.Contains("type:text") ||
					 field.Contains("type:bulk"))
				 {
					 continue;
				 }
				 singleline++;
			 }

			 int leftCol = (singleline + 1)/2;
			 int leftcount = 0; //job is there
			 int rightcount = 0;
			 int bottomcount = 0;
			 int idx = 0; // Job: is 0
			 int first = 0;
			 int last = 0;
			 //int y = 0;
			 Label[] labels = new Label[fields.Length];
			 TextBox[] textboxes = new TextBox[fields.Length];
			 foreach (string field in fields)
			 {
				 if (field.Contains("type:text") ||
					 field.Contains("type:bulk"))
				 {
					 labels[idx] = new Label();
					 first = 0;
					 last = field.IndexOf(";");
					 string fieldname = field.Substring(first, last - first);
					 labels[idx].Text = fieldname + ":";
					 jobsListView.Columns.Add(fieldname,80);
					 Point location = jobLbl.Location;
					 location.Y = location.Y + (60*(leftCol + bottomcount));
					 if (bottomcount > 0)
					 {
						 location.Y = location.Y + 25;
					 }
					 labels[idx].Location = location;
					 labels[idx].Name = fieldname + "Lbl";
					 Size labelsize = labels[idx].Size;
					 int labelx = labelsize.Width;

					 textboxes[idx] = new TextBox();
					 location.X += (labelx + 8);
					 textboxes[idx].Name = fieldname + "TB";
					 textboxes[idx].Location = location;
					 textboxes[idx].Height = 60;
					 textboxes[idx].Width = 400;
					 textboxes[idx].Multiline = true;
					 textboxes[idx].ReadOnly = true;
					 textboxes[idx].ScrollBars = ScrollBars.Vertical;
					 textboxes[idx].WordWrap = true;

					 bottomcount++;
					 idx++;
				 }

				 else
				 {

					 {
						 Point location = new Point();
						 labels[idx] = new Label();
						 first = 0;
						 last = field.IndexOf(";");
						 string fieldname = field.Substring(first, last - first);
						 labels[idx].Text = fieldname + ":";
						 jobsListView.Columns.Add(fieldname, 80);
						 if (leftcount < leftCol)
						 {
							 location = jobLbl.Location;
							 location.Y += (60*leftcount);
							 leftcount++;
						 }
						 else
						 {
							 location = jobLbl.Location;
							 location.Y += (60*rightcount);
							 int indent = (jobLbl.Width + jobTB.Width + 60);
							 location.X = location.X + indent;
							 rightcount++;
						 }
						 labels[idx].Location = location;
						 labels[idx].Name = fieldname + "Lbl";
						 Size labelsize = labels[idx].Size;
						 int labelx = labelsize.Width;

						 textboxes[idx] = new TextBox();
						 int tab = labels[idx].Width + 8;
						 location.X = labels[idx].Location.X + tab;
						 textboxes[idx].Name = fieldname + "TB";
						 textboxes[idx].Location = location;
						 textboxes[idx].Height = 15;
						 textboxes[idx].Width = 100;
						 textboxes[idx].ReadOnly = true;

						 idx++;
					 }
				 }

			 }

			 detailsPanel.Controls.AddRange(labels);
			 detailsPanel.Controls.AddRange(textboxes);

			 GotJobFields = true;

		 }

		 private void jobsListView_SelectedIndexChanged(object sender, EventArgs e)
		 {
			clearDetails();

			if (jobsListView.SelectedItems.Count >0 && jobsListView.SelectedItems[0].ImageIndex ==-1)
			{
				refreshJobListToolStripMenuItem.Enabled = false;
				refreshJobListToolStripMenuItem.Visible = false;
				refreshJobToolStripMenuItem.Enabled = false;
				refreshJobToolStripMenuItem.Visible = false;

				SelectedJob = null;
				SelectedJobs = null;

				return;
			}

			if (jobsListView.SelectedItems.Count  == 0)
			{
				newJobToolStripMenuItem.Enabled = true;
				newJobToolStripMenuItem.Visible = true;
				refreshJobListToolStripMenuItem.Enabled = true;
				refreshJobListToolStripMenuItem.Visible = true;
				refreshJobToolStripMenuItem.Enabled = false;
				refreshJobToolStripMenuItem.Visible = false;
				editJobToolStripMenuItem.Enabled = false;
				editJobToolStripMenuItem.Visible = false;

				SelectedJob = null;
				SelectedJobs = null;

				return;
			}

			if (jobsListView.SelectedItems.Count > 0 && jobsListView.SelectedItems[0].ImageIndex == 0)
			{
				newJobToolStripMenuItem.Enabled = true;
				newJobToolStripMenuItem.Visible = true;
				refreshJobListToolStripMenuItem.Enabled = true;
				refreshJobListToolStripMenuItem.Visible = true;
				refreshJobToolStripMenuItem.Enabled = true;
				refreshJobToolStripMenuItem.Visible = true;
				editJobToolStripMenuItem.Enabled = true;
				editJobToolStripMenuItem.Visible = true;

				P4.Job job = (P4.Job)jobsListView.SelectedItems[0].Tag;
				SelectedJob = job;

				if (job != null)
				{
					editJobToolStripMenuItem.Text = string.Format(editJobToolStripMenuItemFmt, job.Id);
					refreshJobToolStripMenuItem.Text = string.Format(refreshJobToolStripMenuItemFmt, job.Id);
					refreshJobObject(job);
				}
				SelectedJobs = new List<P4.Job>();
				// should this be TreeListViewItem?
				foreach (ListViewItem item in jobsListView.SelectedItems)
				{
					if (item.ImageIndex == 0)
					{
						SelectedJobs.Add((P4.Job)item.Tag);
					}
				}
			}
		 }

		 public bool p4Date()
		 {
			 if (Preferences.LocalSettings.ContainsKey("P4Date_format"))
			 {
				 if ((bool)Preferences.LocalSettings["P4Date_format"] == true)
				 {
					 return true;
				 }
			 }
			 return false;
		 }

		 public bool isDate(string value)
		 {
			 DateTime date;
			 return DateTime.TryParse(value, out date);
		 }

		private void refreshJobObject(P4.Job job)
		 {
             if (jobsListView.Items.Count > 0&&job.Id!="new")
             {
                 ListViewItem jobItem = new ListViewItem();
                 job = Scm.getJob(job.Id);
                 if (job != null)
                 {

                     jobItem = jobsListView.FindItemWithText(" " + job.Id, true, 0);
                     if (jobItem != null)
                     {
                         jobItem.SubItems.Clear();
                         jobItem.Text = " " + job.Id;
                     }

                     foreach (SpecField field in jobspec.Fields)
                     {
                         if ((field.Name.Equals("specdef")) ||
                             (field.Name.Equals("func")))
                         { continue; }

                         Control[] texBox = detailsPanel.Controls.Find(field.Name.ToString() + "TB", true);

                         if (texBox != null && texBox.Length > 0)
                         {
                             if ((jobspec.GetSpecFieldDataType(jobspec, field.Name.ToString()) == P4.SpecFieldDataType.Date)
                                 && p4Date() == false)
                             {
                                 DateTime d;
                                 DateTime.TryParse(job[field.Name].ToString(), out d);
                                 texBox[0].Text = d.ToString("MM/d/yyyy h:mm:ss tt");
                                 jobItem.SubItems.Add(d.ToString("MM/d/yyyy h:mm:ss tt"));
                             }
                             else
                             {
                                 object val = null;
                                 job.TryGetValue(field.Name, out val);
                                 if (val == null)
                                 {
                                     val = "";
                                 }
                                 texBox[0].Text = val.ToString().Replace("\n","\r\n");

                                 if (field.Code!=101)
                                 {
                                     if (jobItem != null)
                                     {
                                         jobItem.SubItems.Add(val.ToString());
                                     }
                                 }
                             }
                         }
                     }
                 }
             }
		 }


		 private void refreshJobListToolStripMenuItem_Click(object sender, EventArgs e)
		 {
             if (maxItems > (int)Preferences.LocalSettings.GetInt("Number_specs", 100))
             {
                 refreshJobsList();
             }
             else
             {
                 maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
                 refreshJobsList();
             }
		 }

		 private void refreshJobToolStripMenuItem_Click(object sender, EventArgs e)
		 {
			 if (jobsListView.SelectedItems != null && jobsListView.SelectedItems.Count > 0)
			 {
				 P4.Job job = (P4.Job)jobsListView.SelectedItems[0].Tag;
				 refreshJobObject(job);
			 }
		 }

		 private void keywordsCB_KeyDown(object sender, KeyEventArgs e)
		 {
			 if (e.KeyCode == Keys.Return)
				 filterBtn.PerformClick();
		 }

		 private void jobsListView_onMaxScroll(object sender, ScrollEventArgs e)
		 {
			 if ((maxItems > 0) && (jobsListView.Items.Count >= maxItems))
			 {
				 maxItems += (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
				 refreshJobsList();
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

		 private void jobsContextMenuStrip_Opening(object sender, CancelEventArgs e)
		 {
			 if (fromBrowser)
			 {
				 e.Cancel = true;
			 }
             else if (Scm == null || Scm.Connection.Disconnected == true)
             {
                 refreshJobToolStripMenuItem.Enabled = false;
                 refreshJobToolStripMenuItem.Visible = false;
                 editJobToolStripMenuItem.Enabled = false;
                 editJobToolStripMenuItem.Visible = false;
                 refreshJobListToolStripMenuItem.Enabled = false;
                 refreshJobListToolStripMenuItem.Visible = true;
                 newJobToolStripMenuItem.Enabled = false;
                 newJobToolStripMenuItem.Visible = true;
                 return;
             }
             else
			 {
				 e.Cancel = false;
				 if (jobsListView.SelectedItems.Count ==0)
				 {
					 refreshJobToolStripMenuItem.Enabled = false;
					 refreshJobToolStripMenuItem.Visible = false;
					 editJobToolStripMenuItem.Enabled = false;
					 editJobToolStripMenuItem.Visible = false;
                     refreshJobListToolStripMenuItem.Enabled = true;
                     refreshJobListToolStripMenuItem.Visible = true;
				     newJobToolStripMenuItem.Enabled = true;
				     newJobToolStripMenuItem.Visible = true;
				 }
				 else
				 {
					 refreshJobToolStripMenuItem.Enabled = true;
					 refreshJobToolStripMenuItem.Visible = true;
					 editJobToolStripMenuItem.Enabled = true;
					 editJobToolStripMenuItem.Visible = true;
				 }
			 }
		 }

		 private void jobsListView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
		 {
             //if (e.NewDisplayIndex==0|e.OldDisplayIndex==0)
             //{
             //    e.Cancel=true;
             //}
		 }

		 private void jobsListView_ColumnClick(object sender, ColumnClickEventArgs e)
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
             //    jobsListView.Columns[lvwColumnSorter.SortColumn].Text =
             //        jobsListView.Columns[lvwColumnSorter.SortColumn].Text;

             //    // Set the column number that is to be sorted; default to ascending.
				 
             //    lvwColumnSorter.SortColumn = e.Column;
             //    lvwColumnSorter.Order = SortOrder.Ascending;
             //}

             //// Perform the sort with these new sort options.
             //this.jobsListView.Sort();
		 }

		 private void newJobToolStripMenuItem_Click(object sender, EventArgs e)
		 {
			 P4.Options opts = new P4.Options();
			 opts["-o"] = null;
             jobspec = Scm.Connection.Repository.GetFormSpec(opts, "job");
			 P4.Job job = new Job();

			 job = Scm.getJob("");
		   
			 job = DlgEditJob.Show(Scm, job);
			 if (job != null)
			 {
				 refreshJobsList();
			 }
		 }

         private void editJobToolStripMenuItem_Click(object sender, EventArgs e)
         {
             if (jobsListView.SelectedItems != null && jobsListView.SelectedItems.Count > 0)
             {
                 P4.Job job = (P4.Job) jobsListView.SelectedItems[0].Tag;
                 DlgEditJob.Show(Scm,job);
				 if (job != null)
				 {
					 refreshJobObject(job);
				 }
             }
         }

         private void keywordsCB_SelectedIndexChanged(object sender, EventArgs e)
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

         private void pathCB_SelectedIndexChanged(object sender, EventArgs e)
         {
             if (selectionChangedByLoad == false)
             {
                 filterBtn.PerformClick();
             }
             selectionChangedByLoad = false;
         }

         //public void updateFilterComboBoxes()
         //{
         //    // load any MRUlists that may have changed in another tool window
         //    // of the same kind
         //    if (Preferences.LocalSettings.ContainsKey(PreferenceKey +"_pathCB"))
         //    {
         //        MRUList value = (MRUList)Preferences.LocalSettings[PreferenceKey + "_pathCB"];
         //        if (value != null)
         //        {
         //            pathCB.mruValues = value.Clone();
         //            pathCB.mruLoaded = true;
         //        }
         //    }
         //    if (Preferences.LocalSettings.ContainsKey(PreferenceKey + "_keywordsCB"))
         //    {
         //        MRUList value = (MRUList)Preferences.LocalSettings[PreferenceKey + "_keywordsCB"];
         //        if (value != null)
         //        {
         //            keywordsCB.mruValues = value.Clone();
         //            keywordsCB.mruLoaded = true;
         //        }
         //    }
         //}
     
	}
}
