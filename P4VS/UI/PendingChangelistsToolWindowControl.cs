
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
using System.Linq;
using System.Threading;
using Perforce.P4;
using Perforce.SwarmApi;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.Text;

using Microsoft.VisualStudio.Shell.Interop;
using Label = System.Windows.Forms.Label;
using nodeTypeFlags = Perforce.P4VS.P4ObjectTreeListViewItem.nodeTypeFlags;
using nodeType = Perforce.P4VS.P4ObjectTreeListViewItem.nodeType;

namespace Perforce.P4VS
{
	using nodeType = P4ChangeTreeListViewItem.nodeType;
    using Perforce.P4Scm;

	/// <summary>
	/// Summary description for P4ToolWindowControl.
	/// </summary>
	public class PendingChangelistsToolWindowControl : P4ToolWindowControlBase
	{
		List<ColumnHeader> DefaultListColumns = null; //submittedTreeListView.Columns;

		private object[] PendingTLVIFields
		{
			get
			{
				if ((Scm != null) && (Scm.Connection.Swarm.SwarmEnabled))
				{
					return PendingTLVIFieldsSwarm;
				}
				return PendingTLVIFieldsNoSwarm;
			}
		}
		private static object[] PendingTLVIFieldsNoSwarm = new object[] { 
					P4ChangeTreeListViewItem.SubItemFlag.Id, 
					P4ChangeTreeListViewItem.SubItemFlag.ModifiedDate, 
					P4ChangeTreeListViewItem.SubItemFlag.ClientName, 
					P4ChangeTreeListViewItem.SubItemFlag.OwnerName, 
					P4ChangeTreeListViewItem.SubItemFlag.Type, 
					P4ChangeTreeListViewItem.SubItemFlag.Description};
		private static object[] PendingTLVIFieldsSwarm = new object[] { 
					P4ChangeTreeListViewItem.SubItemFlag.Id, 
					P4ChangeTreeListViewItem.SubItemFlag.ReviewId, 
					P4ChangeTreeListViewItem.SubItemFlag.ReviewState, 
					P4ChangeTreeListViewItem.SubItemFlag.ModifiedDate, 
					P4ChangeTreeListViewItem.SubItemFlag.ClientName, 
					P4ChangeTreeListViewItem.SubItemFlag.OwnerName, 
					P4ChangeTreeListViewItem.SubItemFlag.Type, 
					P4ChangeTreeListViewItem.SubItemFlag.Description};

		private P4ScmProvider _scm { get; set; }
		private string PathFilterText { get; set; }
		private string UserFilterText { get; set; }
		private string WorkspaceFilterText { get; set; }
		private bool selectionChangedByLoad = false;

		private SplitContainer splitContainer1;
		private I18nControls.GridP4ObjectTreeListView pendingTreeListView;
		private I18nControls.GridLabel filesLbl;
		private I18nControls.GridLabel jobsLbl;
		private I18nControls.GridTextBox descriptionTB;
		private I18nControls.GridLabel descriptionLbl;
		private I18nControls.GridTextBox filesTB;
		private I18nControls.GridTextBox jobsTB;
		private ContextMenuStrip changelistContextMenuStrip;
		private ToolStripMenuItem submitTSMI;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem revertUnchangedFilesTMSI;
		private ToolStripMenuItem revertFilesTMSI;
		private ToolStripMenuItem resolveFilesTMSI;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem unshelveFilesTMSI;
		private ToolStripMenuItem deleteShelvedFilesTMSI;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem newPendingChangelistTMSI;
		private ToolStripMenuItem editPendingChangelistTMSI;
		private ToolStripMenuItem deletePendingChangelistTMSI;
		private ToolStripMenuItem changeOwnershipTMSI;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripMenuItem refreshPendingChangelistListTMSI;
		private ToolStripMenuItem refreshPendingChangelistTMSI;
		private ToolStripMenuItem moveFilesToAnTMSI;
		private ToolStripMenuItem removeJobTMSI;
		private ToolStripMenuItem revertTMSI;
		private ToolStripMenuItem moveToAnotherChangelistTMSI;
		private ToolStripMenuItem shelveTSMI;
		private ToolStripMenuItem shelveFilesTSMI;
		private ToolStripMenuItem openShelvedFileTSMI; 
		private ToolStripSeparator toolStripSeparator6;
		private ToolStripMenuItem diffAgainstTSMI;
		private ToolStripSeparator toolStripSeparator5;
		private ToolStripMenuItem changeFiletypeTSMI;
		private ToolStripMenuItem unshelveTMSI;
		private ToolStripMenuItem deleteTSMI;
		private ToolStripMenuItem lockTSMI;
		private I18nControls.GridLabel typeLbl;
		private I18nControls.GridLabel userLbl2;
		private I18nControls.GridLabel statusLbl;
		private I18nControls.GridGroupBox dividerGB;
		private I18nControls.GridButton filterBtn;
		private I18nControls.GridLabel matchesLbl;
		private I18nControls.GridLabel workspaceLbl;
		private I18nControls.GridLabel userLbl;
		private I18nControls.GridLabel pathLbl;
		private UI.ThreadMonitorControl threadMonitorControl1;
		private ListViewColumnSorter lvwColumnSorter;

		//private ImageList imageList1;
		//private ImageList FileCenterImageList;
		//private ImageList FileLeftImageList;
		//private ImageList FileRightImageList;
		private ToolStripMenuItem resolveTMSI;
		private ToolStripMenuItem diffAgainstHaveTMSI;
		private ToolStripMenuItem diffAgainstHavesTMSI;
		internal I18nControls.GridFilterComboBox workspaceCB;
        internal I18nControls.GridFilterComboBox userCB;
        internal I18nControls.GridFilterComboBox pathCB;
		private ToolStripMenuItem submitShelvedFilesTMSI;
		private ToolStripMenuItem diffAgainstSourceRevTMSI;
		private ToolStripMenuItem diffAgainstWorkspaceFileTMSI;
		private I18nControls.GridLayoutPanel gridLayoutPanel2;
		private I18nControls.GridTextBox workspaceTB2;
		private I18nControls.GridTextBox dateTB;
		private I18nControls.GridLabel workspaceLbl2;
		private I18nControls.GridLabel dateLbl;
		private I18nControls.GridTextBox changeTB;
		private I18nControls.GridLabel changeLbl;
		private I18nControls.GridTextBox typeTB;
		private I18nControls.GridTextBox statusTB;
		private I18nControls.GridTextBox UserTB2;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private ToolStripSeparator SwarmReviewTSS;
		private ToolStripMenuItem RequestReviewTSMI;
		private ToolStripMenuItem UpdateReviewTSMI;
		private ToolStripMenuItem openReviewInSwarmTSMI;

		public new P4ScmProvider Scm
		{
			get
            {
                return _scm;
            }
			set
			{
				// clear registration with the old scm
				if (_scm != null)
				{
					_scm.ChangelistUpdated -= changeUpdated;
				}
				_scm = value;

				if (_scm != null)
				{
					_scm.ChangelistUpdated += changeUpdated;
				}
			}
		}

		//private MRUList _recentPendingPaths = null;
		//private MRUList _recentPendingUsers = null;
		//private MRUList _recentPendingWorkspaces = null;

		private object SyncRoot = new object();

		private delegate void PendingTreeListViewDelegate();
		private delegate void PendingTreeListViewItemDelegate(TreeListViewItem item);

		private delegate void setStringPropertyDelegate(string str);

		private delegate void setFilterBtnDelegate(bool filter);

		private void setFilterBtnBool(bool enabled)
		{
			this.filterBtn.Enabled=enabled;
		}

		private void setPendingMatchesLblText(string matches)
		{
			this.matchesLbl.Text = matches;
		}

		private delegate void ReplacePendingTreeListViewItemDelegate(int index, TreeListViewItem item);

		private void replacePendingListViewItem(int index, TreeListViewItem item)
		{
			pendingTreeListView.Nodes.Remove(item);
		}

		private IContainer components;

		//private enum CenterImage : int
		//{
		//    depot_icon = 0,
		//    file_depot_icon = 1,
		//    file_changed_icon = 2,
		//    file_local_icon = 3,
		//    file_notInClient_icon = 4,
		//    file_unmapped_icon = 5,
		//    pending_icon = 6,
		//    pending_other = 7,
		//    pending_resolve = 8,
		//    resolve_badge = 9,
		//    virtual_file_badge = 10,
		//    virtual_folder_badge = 11,
		//    delete_move_center_badge = 12,
		//    delete_center_badge = 13,
		//    pending_icon_shelved = 14,
		//    pending_other_shelved = 15,
		//    pending_resolve_shelved = 16,
		//    jobs_icon = 17,
		//    shelve_icon_add = 18,
		//    shelve_icon_archive = 19,
		//    shelve_icon_base = 20,
		//    shelve_icon_branch = 21,
		//    shelve_icon_delete = 22,
		//    shelve_icon_edit = 23,
		//    shelve_icon_integrate = 24,
		//    shelve_icon_moveadd = 25,
		//    shelve_icon_movedelete = 26,
		//    shelve_icon_purge = 27,
		//    pending_icon_shelved_review = 28,
		//    pending_other_shelved_review = 29

		//}

		public PendingChangelistsToolWindowControl()
		{
			PreferenceKey = "PendingChangelistsToolWindowControl";
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			base.Initialize();

			// if the mru lists have not been loaded, see if the old preference exists
			if (Preferences.LocalSettings != null)
			{
				if ((pathCB.mruLoaded == false) && (Preferences.LocalSettings.ContainsKey("RecentPendingPaths")))
				{
					MRUList value = (MRUList) Preferences.LocalSettings["RecentPendingPaths"];
					if (value != null)
					{
						pathCB.mruValues = value.Clone();
						pathCB.mruLoaded = true;
					}
				}
				if ((userCB.mruLoaded == false) && (Preferences.LocalSettings.ContainsKey("RecentPendingUsers")))
				{
					MRUList value = (MRUList) Preferences.LocalSettings["RecentPendingUsers"];
					if (value != null)
					{
						userCB.mruValues = value.Clone();
						userCB.mruLoaded = true;
					}
				}
				if ((workspaceCB.mruLoaded == false) &&
					(Preferences.LocalSettings.ContainsKey("RecentPendingWorkspaces")))
				{
					MRUList value = (MRUList) Preferences.LocalSettings["RecentPendingWorkspaces"];
					if (value != null)
					{
						workspaceCB.mruValues = value.Clone();
						workspaceCB.mruLoaded = true;
					}
				}
			}
			//imageList1 = new System.Windows.Forms.ImageList(this.components);
			//FileCenterImageList = new System.Windows.Forms.ImageList(this.components);
			//FileLeftImageList = new System.Windows.Forms.ImageList(this.components);
			//FileRightImageList = new System.Windows.Forms.ImageList(this.components);

			// 
			// FileCenterImageList
			// 
			//FileCenterImageList.TransparentColor = System.Drawing.Color.White;
			//FileCenterImageList.Images.Add("depot_icon.png", Images.depot_icon );//0
			//FileCenterImageList.Images.Add("file_depot_icon.png", Images.file_depot_icon );//1
			//FileCenterImageList.Images.Add("file_changed_icon.png", Images.file_changed_icon );//2
			//FileCenterImageList.Images.Add("file_local_icon.png", Images.file_local_icon );//3
			//FileCenterImageList.Images.Add("file_notInClient_icon.png", Images.file_notInClient_icon );//4
			//FileCenterImageList.Images.Add("file_unmapped_icon.png", Images.file_unmapped_icon );//5
			//FileCenterImageList.Images.Add("pending_icon.png", Images.pending_icon );//6
			//FileCenterImageList.Images.Add("pending_other.png", Images.pending_other );//7
			//FileCenterImageList.Images.Add("pending_resolve.png", Images.pending_resolve );//8
			//FileCenterImageList.Images.Add("resolve_badge.png", Images.resolve_badge );//9
			//FileCenterImageList.Images.Add("virtual_file_badge.png", Images.virtual_file_badge );//10
			//FileCenterImageList.Images.Add("virtual_folder_badge.png", Images.virtual_folder_badge );//11
			//FileCenterImageList.Images.Add("delete_move_center_badge.png", Images.delete_move_center_badge );//12
			//FileCenterImageList.Images.Add("delete_center_badge.png", Images.delete_center_badge );//13
			//FileCenterImageList.Images.Add("pending_icon_shelved.png", Images.pending_icon_shelved );//14
			//FileCenterImageList.Images.Add("pending_other_shelved.png", Images.pending_other_shelved );//15
			//FileCenterImageList.Images.Add("pending_resolve_shelved.png", Images.pending_resolve_shelved );//16
			//FileCenterImageList.Images.Add("jobs_icon.png", Images.jobs_icon );//17
			//FileCenterImageList.Images.Add("shelve_icon_add.png", Images.shelve_icon_add );//18
			//FileCenterImageList.Images.Add("shelve_icon_archive.png", Images.shelve_icon_archive );//19
			//FileCenterImageList.Images.Add("shelve_icon_base.png", Images.shelve_icon_base );//20
			//FileCenterImageList.Images.Add("shelve_icon_branch.png", Images.shelve_icon_branch );//21
			//FileCenterImageList.Images.Add("shelve_icon_delete.png", Images.shelve_icon_delete );//22
			//FileCenterImageList.Images.Add("shelve_icon_edit.png", Images.shelve_icon_edit );//23
			//FileCenterImageList.Images.Add("shelve_icon_integrate.png", Images.shelve_icon_integrate );//24
			//FileCenterImageList.Images.Add("shelve_icon_moveadd.png", Images.shelve_icon_moveadd );//25
			//FileCenterImageList.Images.Add("shelve_icon_movedelete.png", Images.shelve_icon_movedelete );//26
			//FileCenterImageList.Images.Add("shelve_icon_purge.png", Images.shelve_icon_purge );//27
			//FileCenterImageList.Images.Add("pending_icon_shelved_review.png", Images.pending_icon_shelved_review);//28
			//FileCenterImageList.Images.Add("pending_other_shelved_review.png", Images.pending_other_shelved_review);//29
			// 
			// imageList1
			// 
			//imageList1.TransparentColor = System.Drawing.Color.Transparent;
			//imageList1.Images.Add("pending_icon.png", Images.pending_icon );
			//imageList1.Images.Add("pending_icon_shelved.png", Images.pending_icon_shelved );
			//imageList1.Images.Add("pending_other.png", Images.pending_other );
			//imageList1.Images.Add("pending_other_shelved.png", Images.pending_other_shelved );
			//imageList1.Images.Add("pending_resolve.png", Images.pending_resolve );
			//imageList1.Images.Add("pending_resolve_shelved.png", Images.pending_resolve_shelved );
			//imageList1.Images.Add("portrait.png", Images.portrait );
			//imageList1.Images.Add("shelve_icon_base.png", Images.shelve_icon_base );
			//imageList1.Images.Add("jobs_icon.png", Images.jobs_icon );
			// 
			// FileLeftImageList
			// 
			//FileLeftImageList.TransparentColor = System.Drawing.Color.White;
			//FileLeftImageList.Images.Add("edit_badge.png", Images.edit_badge );
			//FileLeftImageList.Images.Add("add_badge.png", Images.add_badge );
			//FileLeftImageList.Images.Add("delete_badge.png", Images.delete_badge );
			//FileLeftImageList.Images.Add("lock_badge.png", Images.lock_badge );
			//FileLeftImageList.Images.Add("branch_badge.png", Images.branch_badge );
			//FileLeftImageList.Images.Add("add_move_badge.png", Images.add_move_badge );
			//FileLeftImageList.Images.Add("delete_move_badge.png", Images.delete_move_badge);
			//FileLeftImageList.Images.Add("pending_shelved_badge.png", Images.pending_shelved_badge);
			// 
			// FileRightImageList
			// 
			//FileRightImageList.TransparentColor = System.Drawing.Color.White;
			//FileRightImageList.Images.Add("head_dot_badge.png", Images.head_dot_badge );
			//FileRightImageList.Images.Add("out_of_sync_badge.png", Images.out_of_sync_badge );
			//FileRightImageList.Images.Add("editO_badge.png", Images.editO_badge );
			//FileRightImageList.Images.Add("addO_badge.png", Images.addO_badge );
			//FileRightImageList.Images.Add("deleteO_badge.png", Images.deleteO_badge );
			//FileRightImageList.Images.Add("branchO_badge.png", Images.branchO_badge );
			//FileRightImageList.Images.Add("delete_moveO_badge.png", Images.delete_moveO_badge );
			//FileRightImageList.Images.Add("add_moveO_badge.png", Images.add_moveO_badge );
			//FileRightImageList.Images.Add("lock_other_badge.png", Images.lock_other_badge );

			//pendingTreeListView.LargeImageList = FileCenterImageList;
			//pendingTreeListView.SmallImageList = FileCenterImageList;
			
			// Create an instance of a ListView column sorter and assign it 
			// to the ListView control.
			lvwColumnSorter = new ListViewColumnSorter();
			this.pendingTreeListView.ListViewItemSorter = lvwColumnSorter;
			PathFilterText = pathCB.Text;
			UserFilterText = userCB.Text;
			WorkspaceFilterText = workspaceCB.Text;
			threadMonitorControl1.Visible = false;

			//newConection = new P4VsProvider.NewConnectionDelegate(OnNewConnection);
			P4VsProvider.NewConnection += newConection;

			changeUpdated = new P4ScmProvider.ChangelistUpdatedDelegate(onChangelistUpdated);

			checkConnection();
#if VS2012
            if (!DesignMode)
            {
                base.InitThemeManager();
            }
#endif
        }

        public PendingChangelistsToolWindowControl(P4ScmProvider scm)
			:base (scm)
		{
			// clear registration with the old scm
			_scm.ChangelistUpdated -= changeUpdated;

			Scm = scm;
			PreferenceKey = "PendingChangelistsToolWindowControl";

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			base.Initialize();
			
			newConection = new P4VsProvider.NewConnectionDelegate(OnNewConnection);
			P4VsProvider.NewConnection += newConection;

			changeUpdated = new P4ScmProvider.ChangelistUpdatedDelegate(onChangelistUpdated);
			scm.ChangelistUpdated += changeUpdated;

			checkConnection();
#if VS2012
            if (!DesignMode)
            {
                base.InitThemeManager();
            }
#endif
        }

        P4VsProvider.NewConnectionDelegate newConection;
		P4ScmProvider.ChangelistUpdatedDelegate changeUpdated; 


		public override void OnNewConnection(P4ScmProvider newScm)
		{
			Scm = newScm;

			// refilter;
			this.pendingTreeListView.Nodes.Clear();
			
			clearDetails();

			filterBtn.Enabled = (Scm != null) && Scm.Connected;
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

				//if (userCB.Text == ""&&
				//    !(userCB.mruValues.Contains("")))
				//{
				//    selectionChangedByLoad = true;
				//    userCB.Text = Scm.User;
				//}

				//if (workspaceCB.Text == ""&&
				//    !(workspaceCB.mruValues.Contains("")))
				//{
				//    selectionChangedByLoad = true;
				//    workspaceCB.Text = Scm.Workspace;
				//}
				

				Scm.ChangelistUpdated += changeUpdated;
			}
			
			filterBtn_Click(null, null);
		}

		public void onChangelistUpdated(object Sender, P4ScmProvider.ChangelistUpdateArgs Args)
		{
				ListViewItem toRefresh = new ListViewItem();
				if (Args.ChanglistId == 0)
				{
					toRefresh = pendingTreeListView.FindItemWithText("default");
					if (toRefresh != null)
					{
						refreshChangelistObject(toRefresh as TreeListViewItem);
						return;
					}
					return;
				}

				toRefresh = pendingTreeListView.FindItemWithText(Args.ChanglistId.ToString());
				if (toRefresh != null)
				{
					refreshChangelistObject(toRefresh as TreeListViewItem);
					return;
				}
				else
				{
					P4.FileSpec path = new P4.FileSpec();
					string workspaceName = workspaceCB.Text;
					string user = userCB.Text;
					if (string.IsNullOrEmpty(pathCB.Text))
					{ path = null; }
					else
					{
						if (pathCB.Text.StartsWith("//"))
						{ path.DepotPath = new P4.DepotPath(pathCB.Text); }
						else
						{ path.LocalPath = new P4.LocalPath(pathCB.Text); }
					}

					IList<P4.Changelist> filteredList = Scm.GetChangelists(P4.ChangesCmdFlags.None, workspaceName, -1, P4.ChangeListStatus.Pending, user, path);
					if (filteredList != null)
					{
						foreach (P4.Changelist change in filteredList)
						{
							if (change.Id == Args.ChanglistId)
							{
								addChangelistItem(Args.ChanglistId);
								return;
							}
						}
					}
					return;
				}
		}

		public int addChangelistItem(int ChangeId)
		{
			P4.Changelist addedChange = Scm.GetChangelist(ChangeId);
            SwarmApi.SwarmServer.Review review = Scm.Connection.Swarm.IsChangelistAttachedToReview(addedChange.Id);
			P4ChangeTreeListViewItem itemToAdd = new P4ChangeTreeListViewItem(null, addedChange, review, Scm, PendingTLVIFields);

			itemToAdd.ChildNodes.Add(new TreeListViewItem());
			if (pendingTreeListView.InvokeRequired)
			{
				pendingTreeListView.Invoke(new PendingTreeListViewItemDelegate(this.pendingTreeListView.Nodes.Add), itemToAdd);
				pendingTreeListView.Invoke(new PendingTreeListViewDelegate(this.pendingTreeListView.BuildTreeList));
			}
			else
			{
				this.pendingTreeListView.Nodes.Add(itemToAdd);
				this.pendingTreeListView.BuildTreeList();
			}
            string itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches,
                                        pendingTreeListView.Nodes.Count);
            if (this.matchesLbl.InvokeRequired)
            {
                matchesLbl.Invoke(new setStringPropertyDelegate(setPendingMatchesLblText), itemCountStr);
            }
            else
            {
                matchesLbl.Text = itemCountStr;
            }
            return addedChange.Id;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PendingChangelistsToolWindowControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gridLayoutPanel2 = new Perforce.I18nControls.GridLayoutPanel();
            this.threadMonitorControl1 = new Perforce.P4VS.UI.ThreadMonitorControl();
            this.dividerGB = new Perforce.I18nControls.GridGroupBox();
            this.pathLbl = new Perforce.I18nControls.GridLabel();
            this.pendingTreeListView = new Perforce.I18nControls.GridP4ObjectTreeListView();
            this.changelistContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.submitTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.revertUnchangedFilesTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.revertFilesTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.revertTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.resolveFilesTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.moveFilesToAnTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToAnotherChangelistTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.resolveTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.shelveFilesTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.shelveTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.submitShelvedFilesTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.unshelveFilesTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.unshelveTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteShelvedFilesTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.openShelvedFileTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.diffAgainstHaveTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.diffAgainstHavesTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.diffAgainstTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.diffAgainstSourceRevTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.diffAgainstWorkspaceFileTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.changeFiletypeTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.lockTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.RequestReviewTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.UpdateReviewTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.openReviewInSwarmTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.SwarmReviewTSS = new System.Windows.Forms.ToolStripSeparator();
            this.newPendingChangelistTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.editPendingChangelistTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePendingChangelistTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.changeOwnershipTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshPendingChangelistListTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshPendingChangelistTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.removeJobTMSI = new System.Windows.Forms.ToolStripMenuItem();
            this.workspaceCB = new Perforce.I18nControls.GridFilterComboBox();
            this.userLbl = new Perforce.I18nControls.GridLabel();
            this.pathCB = new Perforce.I18nControls.GridFilterComboBox();
            this.filterBtn = new Perforce.I18nControls.GridButton();
            this.workspaceLbl = new Perforce.I18nControls.GridLabel();
            this.userCB = new Perforce.I18nControls.GridFilterComboBox();
            this.matchesLbl = new Perforce.I18nControls.GridLabel();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.typeTB = new Perforce.I18nControls.GridTextBox();
            this.changeLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceTB2 = new Perforce.I18nControls.GridTextBox();
            this.dateLbl = new Perforce.I18nControls.GridLabel();
            this.statusTB = new Perforce.I18nControls.GridTextBox();
            this.changeTB = new Perforce.I18nControls.GridTextBox();
            this.UserTB2 = new Perforce.I18nControls.GridTextBox();
            this.statusLbl = new Perforce.I18nControls.GridLabel();
            this.dateTB = new Perforce.I18nControls.GridTextBox();
            this.filesLbl = new Perforce.I18nControls.GridLabel();
            this.filesTB = new Perforce.I18nControls.GridTextBox();
            this.userLbl2 = new Perforce.I18nControls.GridLabel();
            this.jobsTB = new Perforce.I18nControls.GridTextBox();
            this.jobsLbl = new Perforce.I18nControls.GridLabel();
            this.workspaceLbl2 = new Perforce.I18nControls.GridLabel();
            this.descriptionTB = new Perforce.I18nControls.GridTextBox();
            this.descriptionLbl = new Perforce.I18nControls.GridLabel();
            this.typeLbl = new Perforce.I18nControls.GridLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gridLayoutPanel2.SuspendLayout();
            this.changelistContextMenuStrip.SuspendLayout();
            this.gridLayoutPanel1.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.gridLayoutPanel2);
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainer1.Panel2.Controls.Add(this.gridLayoutPanel1);
            this.splitContainer1.TabStop = false;
            // 
            // gridLayoutPanel2
            // 
            this.gridLayoutPanel2.Controls.Add(this.threadMonitorControl1);
            this.gridLayoutPanel2.Controls.Add(this.dividerGB);
            this.gridLayoutPanel2.Controls.Add(this.pathLbl);
            this.gridLayoutPanel2.Controls.Add(this.pendingTreeListView);
            this.gridLayoutPanel2.Controls.Add(this.workspaceCB);
            this.gridLayoutPanel2.Controls.Add(this.userLbl);
            this.gridLayoutPanel2.Controls.Add(this.pathCB);
            this.gridLayoutPanel2.Controls.Add(this.filterBtn);
            this.gridLayoutPanel2.Controls.Add(this.workspaceLbl);
            this.gridLayoutPanel2.Controls.Add(this.userCB);
            this.gridLayoutPanel2.Controls.Add(this.matchesLbl);
            resources.ApplyResources(this.gridLayoutPanel2, "gridLayoutPanel2");
            this.gridLayoutPanel2.EnableDesignerGrid = false;
            this.gridLayoutPanel2.EnableDesignerLayout = false;
            this.gridLayoutPanel2.EnableParentResize = false;
            this.gridLayoutPanel2.MinimumColumnWidth = 10;
            this.gridLayoutPanel2.MinimumRowHeight = 10;
            this.gridLayoutPanel2.Name = "gridLayoutPanel2";
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
            // dividerGB
            // 
            resources.ApplyResources(this.dividerGB, "dividerGB");
            this.dividerGB.CellHeight = 56;
            this.dividerGB.CellWidth = 10;
            this.dividerGB.Column = 4;
            this.dividerGB.ColumnsSpanned = 0;
            this.dividerGB.Name = "dividerGB";
            this.dividerGB.Row = 0;
            this.dividerGB.RowsSpanned = 1;
            this.dividerGB.TabStop = false;
            this.dividerGB.YOffset = 2;
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
            // pendingTreeListView
            // 
            this.pendingTreeListView._maxLineOffset = 0;
            this.pendingTreeListView.ActionColumn = -1;
            this.pendingTreeListView.AllowColumnReorder = true;
            resources.ApplyResources(this.pendingTreeListView, "pendingTreeListView");
            this.pendingTreeListView.CellHeight = 104;
            this.pendingTreeListView.CellWidth = 614;
            this.pendingTreeListView.Column = 0;
            this.pendingTreeListView.ColumnsSpanned = 5;
            this.pendingTreeListView.ContextMenuStrip = this.changelistContextMenuStrip;
            this.pendingTreeListView.EnableIconOverlays = true;
            this.pendingTreeListView.EnableSorting = true;
            this.pendingTreeListView.FullRowSelect = true;
            this.pendingTreeListView.MultiSelectConditions = Perforce.P4VS.TreeListView.MultiSelectCondition.SameClass;
            this.pendingTreeListView.Name = "pendingTreeListView";
            this.pendingTreeListView.OverlayOffset = 3;
            this.pendingTreeListView.RootCheckBoxes = false;
            this.pendingTreeListView.Row = 2;
            this.pendingTreeListView.RowsSpanned = 0;
            this.pendingTreeListView.ScrollPosition = 0;
            this.pendingTreeListView.TabStop = false;
            this.pendingTreeListView.TreeView = true;
            this.pendingTreeListView.UseClassicImageList = false;
            this.pendingTreeListView.UseCompatibleStateImageBehavior = false;
            this.pendingTreeListView.View = System.Windows.Forms.View.Details;
            this.pendingTreeListView.YOffset = 0;
            this.pendingTreeListView.onMaxScroll += new System.Windows.Forms.ScrollEventHandler(this.pendingTreeListView_onMaxScroll);
            this.pendingTreeListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.pendingTreeListView_ColumnClick);
            this.pendingTreeListView.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.pendingTreeListView_ColumnReordered);
            this.pendingTreeListView.SelectedIndexChanged += new System.EventHandler(this.pendingTreeListView_SelectedIndexChanged);
            this.pendingTreeListView.Click += new System.EventHandler(this.pendingTreeListView_Click);
            this.pendingTreeListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pendingTreeListView_MouseDoubleClick);
            // 
            // changelistContextMenuStrip
            // 
            this.changelistContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.submitTSMI,
            this.toolStripSeparator1,
            this.revertUnchangedFilesTMSI,
            this.revertFilesTMSI,
            this.revertTMSI,
            this.resolveFilesTMSI,
            this.moveFilesToAnTMSI,
            this.moveToAnotherChangelistTMSI,
            this.resolveTMSI,
            this.toolStripSeparator2,
            this.shelveFilesTSMI,
            this.shelveTSMI,
            this.submitShelvedFilesTMSI,
            this.unshelveFilesTMSI,
            this.unshelveTMSI,
            this.deleteShelvedFilesTMSI,
            this.openShelvedFileTSMI,
            this.deleteTSMI,
            this.toolStripSeparator3,
            this.diffAgainstHaveTMSI,
            this.diffAgainstHavesTMSI,
            this.diffAgainstTSMI,
            this.diffAgainstSourceRevTMSI,
            this.diffAgainstWorkspaceFileTMSI,
            this.toolStripSeparator4,
            this.changeFiletypeTSMI,
            this.lockTSMI,
            this.toolStripSeparator5,
            this.RequestReviewTSMI,
            this.UpdateReviewTSMI,
            this.openReviewInSwarmTSMI,
            this.SwarmReviewTSS,
            this.newPendingChangelistTMSI,
            this.editPendingChangelistTMSI,
            this.deletePendingChangelistTMSI,
            this.changeOwnershipTMSI,
            this.toolStripSeparator6,
            this.refreshPendingChangelistListTMSI,
            this.refreshPendingChangelistTMSI,
            this.removeJobTMSI});
            this.changelistContextMenuStrip.Name = "changelistContextMenuStrip";
            resources.ApplyResources(this.changelistContextMenuStrip, "changelistContextMenuStrip");
            this.changelistContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.changelistContextMenuStrip_Opening);
            // 
            // submitTSMI
            // 
            resources.ApplyResources(this.submitTSMI, "submitTSMI");
            this.submitTSMI.Name = "submitTSMI";
            this.submitTSMI.Click += new System.EventHandler(this.submitToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.toolStripSeparator1.ForeColor = System.Drawing.Color.Black;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // revertUnchangedFilesTMSI
            // 
            this.revertUnchangedFilesTMSI.Name = "revertUnchangedFilesTMSI";
            resources.ApplyResources(this.revertUnchangedFilesTMSI, "revertUnchangedFilesTMSI");
            this.revertUnchangedFilesTMSI.Click += new System.EventHandler(this.revertUnchangedFilesTMSI_Click);
            // 
            // revertFilesTMSI
            // 
            resources.ApplyResources(this.revertFilesTMSI, "revertFilesTMSI");
            this.revertFilesTMSI.Name = "revertFilesTMSI";
            this.revertFilesTMSI.Click += new System.EventHandler(this.revertFilesToolStripMenuItem_Click);
            // 
            // revertTMSI
            // 
            resources.ApplyResources(this.revertTMSI, "revertTMSI");
            this.revertTMSI.Name = "revertTMSI";
            this.revertTMSI.Click += new System.EventHandler(this.revertToolStripMenuItem_Click);
            // 
            // resolveFilesTMSI
            // 
            resources.ApplyResources(this.resolveFilesTMSI, "resolveFilesTMSI");
            this.resolveFilesTMSI.Name = "resolveFilesTMSI";
            this.resolveFilesTMSI.Click += new System.EventHandler(this.resolveFilesToolStripMenuItem_Click);
            // 
            // moveFilesToAnTMSI
            // 
            resources.ApplyResources(this.moveFilesToAnTMSI, "moveFilesToAnTMSI");
            this.moveFilesToAnTMSI.Name = "moveFilesToAnTMSI";
            this.moveFilesToAnTMSI.Click += new System.EventHandler(this.moveFilesToAnToolStripMenuItem_Click);
            // 
            // moveToAnotherChangelistTMSI
            // 
            resources.ApplyResources(this.moveToAnotherChangelistTMSI, "moveToAnotherChangelistTMSI");
            this.moveToAnotherChangelistTMSI.Name = "moveToAnotherChangelistTMSI";
            this.moveToAnotherChangelistTMSI.Click += new System.EventHandler(this.moveToAnotherChangelistToolStripMenuItem_Click);
            // 
            // resolveTMSI
            // 
            this.resolveTMSI.Name = "resolveTMSI";
            resources.ApplyResources(this.resolveTMSI, "resolveTMSI");
            this.resolveTMSI.Click += new System.EventHandler(this.resolveToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // shelveFilesTSMI
            // 
            resources.ApplyResources(this.shelveFilesTSMI, "shelveFilesTSMI");
            this.shelveFilesTSMI.Name = "shelveFilesTSMI";
            this.shelveFilesTSMI.Click += new System.EventHandler(this.shelveFilesToolStripMenuItem_Click);
            // 
            // shelveTSMI
            // 
            resources.ApplyResources(this.shelveTSMI, "shelveTSMI");
            this.shelveTSMI.Name = "shelveTSMI";
            this.shelveTSMI.Click += new System.EventHandler(this.shelveToolStripMenuItem_Click);
            // 
            // submitShelvedFilesTMSI
            // 
            this.submitShelvedFilesTMSI.Name = "submitShelvedFilesTMSI";
            resources.ApplyResources(this.submitShelvedFilesTMSI, "submitShelvedFilesTMSI");
            this.submitShelvedFilesTMSI.Click += new System.EventHandler(this.submitShelvedFilesToolStripMenuItem_Click);
            // 
            // unshelveFilesTMSI
            // 
            resources.ApplyResources(this.unshelveFilesTMSI, "unshelveFilesTMSI");
            this.unshelveFilesTMSI.Name = "unshelveFilesTMSI";
            this.unshelveFilesTMSI.Click += new System.EventHandler(this.unshelveFilesToolStripMenuItem_Click);
            // 
            // unshelveTMSI
            // 
            resources.ApplyResources(this.unshelveTMSI, "unshelveTMSI");
            this.unshelveTMSI.Name = "unshelveTMSI";
            this.unshelveTMSI.Click += new System.EventHandler(this.unshelveToolStripMenuItem_Click);
            // 
            // deleteShelvedFilesTMSI
            // 
            resources.ApplyResources(this.deleteShelvedFilesTMSI, "deleteShelvedFilesTMSI");
            this.deleteShelvedFilesTMSI.Name = "deleteShelvedFilesTMSI";
            this.deleteShelvedFilesTMSI.Click += new System.EventHandler(this.deleteShelvedFilesToolStripMenuItem_Click);
            // 
            // openShelvedFileTSMI
            // 
            resources.ApplyResources(this.openShelvedFileTSMI, "openShelvedFileTSMI");
            this.openShelvedFileTSMI.Name = "openShelvedFileTSMI";
            this.openShelvedFileTSMI.Click += new System.EventHandler(this.openShelvedFileToolStripMenuItem_Click);
            // 
            // deleteTSMI
            // 
            resources.ApplyResources(this.deleteTSMI, "deleteTSMI");
            this.deleteTSMI.Name = "deleteTSMI";
            this.deleteTSMI.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // diffAgainstHaveTMSI
            // 
            this.diffAgainstHaveTMSI.Name = "diffAgainstHaveTMSI";
            resources.ApplyResources(this.diffAgainstHaveTMSI, "diffAgainstHaveTMSI");
            this.diffAgainstHaveTMSI.Click += new System.EventHandler(this.diffAgainstHaveToolStripMenuItem_Click);
            // 
            // diffAgainstHavesTMSI
            // 
            this.diffAgainstHavesTMSI.Name = "diffAgainstHavesTMSI";
            resources.ApplyResources(this.diffAgainstHavesTMSI, "diffAgainstHavesTMSI");
            this.diffAgainstHavesTMSI.Click += new System.EventHandler(this.diffAgainstHavesToolStripMenuItem_Click);
            // 
            // diffAgainstTSMI
            // 
            resources.ApplyResources(this.diffAgainstTSMI, "diffAgainstTSMI");
            this.diffAgainstTSMI.Name = "diffAgainstTSMI";
            this.diffAgainstTSMI.Click += new System.EventHandler(this.diffAgainstToolStripMenuItem_Click);
            // 
            // diffAgainstSourceRevTMSI
            // 
            this.diffAgainstSourceRevTMSI.Name = "diffAgainstSourceRevTMSI";
            resources.ApplyResources(this.diffAgainstSourceRevTMSI, "diffAgainstSourceRevTMSI");
            this.diffAgainstSourceRevTMSI.Click += new System.EventHandler(this.diffAgainstSourceRevToolStripMenuItem_Click);
            // 
            // diffAgainstWorkspaceFileTMSI
            // 
            this.diffAgainstWorkspaceFileTMSI.Name = "diffAgainstWorkspaceFileTMSI";
            resources.ApplyResources(this.diffAgainstWorkspaceFileTMSI, "diffAgainstWorkspaceFileTMSI");
            this.diffAgainstWorkspaceFileTMSI.Click += new System.EventHandler(this.diffAgainstWorkspaceFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // changeFiletypeTSMI
            // 
            resources.ApplyResources(this.changeFiletypeTSMI, "changeFiletypeTSMI");
            this.changeFiletypeTSMI.Name = "changeFiletypeTSMI";
            this.changeFiletypeTSMI.Click += new System.EventHandler(this.changeFiletypeToolStripMenuItem_Click);
            // 
            // lockTSMI
            // 
            resources.ApplyResources(this.lockTSMI, "lockTSMI");
            this.lockTSMI.Name = "lockTSMI";
            this.lockTSMI.Click += new System.EventHandler(this.lockToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // RequestReviewTSMI
            // 
            resources.ApplyResources(this.RequestReviewTSMI, "RequestReviewTSMI");
            this.RequestReviewTSMI.Name = "RequestReviewTSMI";
            this.RequestReviewTSMI.Click += new System.EventHandler(this.RequestReviewTSMI_Click);
            // 
            // UpdateReviewTSMI
            // 
            resources.ApplyResources(this.UpdateReviewTSMI, "UpdateReviewTSMI");
            this.UpdateReviewTSMI.Name = "UpdateReviewTSMI";
            this.UpdateReviewTSMI.Click += new System.EventHandler(this.UpdateReviewTSMI_Click);
            // 
            // openReviewInSwarmTSMI
            // 
            resources.ApplyResources(this.openReviewInSwarmTSMI, "openReviewInSwarmTSMI");
            this.openReviewInSwarmTSMI.Name = "openReviewInSwarmTSMI";
            this.openReviewInSwarmTSMI.Click += new System.EventHandler(this.openReviewInSwarmToolStripMenuItem_Click);
            // 
            // SwarmReviewTSS
            // 
            this.SwarmReviewTSS.Name = "SwarmReviewTSS";
            resources.ApplyResources(this.SwarmReviewTSS, "SwarmReviewTSS");
            // 
            // newPendingChangelistTMSI
            // 
            this.newPendingChangelistTMSI.Name = "newPendingChangelistTMSI";
            resources.ApplyResources(this.newPendingChangelistTMSI, "newPendingChangelistTMSI");
            this.newPendingChangelistTMSI.Click += new System.EventHandler(this.newPendingChangelistToolStripMenuItem_Click);
            // 
            // editPendingChangelistTMSI
            // 
            resources.ApplyResources(this.editPendingChangelistTMSI, "editPendingChangelistTMSI");
            this.editPendingChangelistTMSI.Name = "editPendingChangelistTMSI";
            this.editPendingChangelistTMSI.Click += new System.EventHandler(this.editPendingChangelistToolStripMenuItem_Click);
            // 
            // deletePendingChangelistTMSI
            // 
            resources.ApplyResources(this.deletePendingChangelistTMSI, "deletePendingChangelistTMSI");
            this.deletePendingChangelistTMSI.Name = "deletePendingChangelistTMSI";
            this.deletePendingChangelistTMSI.Click += new System.EventHandler(this.deletePendingChangelistToolStripMenuItem_Click);
            // 
            // changeOwnershipTMSI
            // 
            resources.ApplyResources(this.changeOwnershipTMSI, "changeOwnershipTMSI");
            this.changeOwnershipTMSI.Name = "changeOwnershipTMSI";
            this.changeOwnershipTMSI.Click += new System.EventHandler(this.changeOwnershipToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // refreshPendingChangelistListTMSI
            // 
            this.refreshPendingChangelistListTMSI.Name = "refreshPendingChangelistListTMSI";
            resources.ApplyResources(this.refreshPendingChangelistListTMSI, "refreshPendingChangelistListTMSI");
            this.refreshPendingChangelistListTMSI.Click += new System.EventHandler(this.refreshPendingChangelistListToolStripMenuItem_Click);
            // 
            // refreshPendingChangelistTMSI
            // 
            resources.ApplyResources(this.refreshPendingChangelistTMSI, "refreshPendingChangelistTMSI");
            this.refreshPendingChangelistTMSI.Name = "refreshPendingChangelistTMSI";
            this.refreshPendingChangelistTMSI.Click += new System.EventHandler(this.refreshPendingChangelistToolStripMenuItem_Click);
            // 
            // removeJobTMSI
            // 
            resources.ApplyResources(this.removeJobTMSI, "removeJobTMSI");
            this.removeJobTMSI.Name = "removeJobTMSI";
            this.removeJobTMSI.Click += new System.EventHandler(this.removeJobToolStripMenuItem_Click);
            // 
            // workspaceCB
            // 
            resources.ApplyResources(this.workspaceCB, "workspaceCB");
            this.workspaceCB.CellHeight = 27;
            this.workspaceCB.CellWidth = 194;
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
            // pathCB
            // 
            resources.ApplyResources(this.pathCB, "pathCB");
            this.pathCB.CellHeight = 29;
            this.pathCB.CellWidth = 460;
            this.pathCB.Column = 1;
            this.pathCB.ColumnsSpanned = 2;
            this.pathCB.FormattingEnabled = true;
            this.pathCB.Name = "pathCB";
            this.pathCB.Row = 0;
            this.pathCB.RowsSpanned = 0;
            this.pathCB.YOffset = 1;
            this.pathCB.DropDown += new System.EventHandler(this.pathCB_DropDown);
            this.pathCB.SelectedIndexChanged += new System.EventHandler(this.pathCB_SelectedIndexChanged);
            this.pathCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pathCB_KeyDown);
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
            // userCB
            // 
            resources.ApplyResources(this.userCB, "userCB");
            this.userCB.CellHeight = 27;
            this.userCB.CellWidth = 195;
            this.userCB.Column = 1;
            this.userCB.ColumnsSpanned = 0;
            this.userCB.FormattingEnabled = true;
            this.userCB.Name = "userCB";
            this.userCB.Row = 1;
            this.userCB.RowsSpanned = 0;
            this.userCB.YOffset = 0;
            this.userCB.DropDown += new System.EventHandler(this.userCB_DropDown);
            this.userCB.SelectedIndexChanged += new System.EventHandler(this.userCB_SelectedIndexChanged);
            this.userCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userCB_KeyDown);
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
            this.matchesLbl.TextChanged += new System.EventHandler(this.matchesLbl_TextChanged);
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.typeTB);
            this.gridLayoutPanel1.Controls.Add(this.changeLbl);
            this.gridLayoutPanel1.Controls.Add(this.workspaceTB2);
            this.gridLayoutPanel1.Controls.Add(this.dateLbl);
            this.gridLayoutPanel1.Controls.Add(this.statusTB);
            this.gridLayoutPanel1.Controls.Add(this.changeTB);
            this.gridLayoutPanel1.Controls.Add(this.UserTB2);
            this.gridLayoutPanel1.Controls.Add(this.statusLbl);
            this.gridLayoutPanel1.Controls.Add(this.dateTB);
            this.gridLayoutPanel1.Controls.Add(this.filesLbl);
            this.gridLayoutPanel1.Controls.Add(this.filesTB);
            this.gridLayoutPanel1.Controls.Add(this.userLbl2);
            this.gridLayoutPanel1.Controls.Add(this.jobsTB);
            this.gridLayoutPanel1.Controls.Add(this.jobsLbl);
            this.gridLayoutPanel1.Controls.Add(this.workspaceLbl2);
            this.gridLayoutPanel1.Controls.Add(this.descriptionTB);
            this.gridLayoutPanel1.Controls.Add(this.descriptionLbl);
            this.gridLayoutPanel1.Controls.Add(this.typeLbl);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = false;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // typeTB
            // 
            resources.ApplyResources(this.typeTB, "typeTB");
            this.typeTB.CellHeight = 26;
            this.typeTB.CellWidth = 248;
            this.typeTB.Column = 3;
            this.typeTB.ColumnsSpanned = 0;
            this.typeTB.Name = "typeTB";
            this.typeTB.ReadOnly = true;
            this.typeTB.Row = 2;
            this.typeTB.RowsSpanned = 0;
            this.typeTB.YOffset = 0;
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
            this.changeLbl.YOffset = 3;
            // 
            // workspaceTB2
            // 
            resources.ApplyResources(this.workspaceTB2, "workspaceTB2");
            this.workspaceTB2.CellHeight = 26;
            this.workspaceTB2.CellWidth = 249;
            this.workspaceTB2.Column = 1;
            this.workspaceTB2.ColumnsSpanned = 0;
            this.workspaceTB2.Name = "workspaceTB2";
            this.workspaceTB2.ReadOnly = true;
            this.workspaceTB2.Row = 2;
            this.workspaceTB2.RowsSpanned = 0;
            this.workspaceTB2.YOffset = 0;
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
            this.dateLbl.YOffset = 3;
            // 
            // statusTB
            // 
            resources.ApplyResources(this.statusTB, "statusTB");
            this.statusTB.CellHeight = 26;
            this.statusTB.CellWidth = 248;
            this.statusTB.Column = 3;
            this.statusTB.ColumnsSpanned = 0;
            this.statusTB.Name = "statusTB";
            this.statusTB.ReadOnly = true;
            this.statusTB.Row = 1;
            this.statusTB.RowsSpanned = 0;
            this.statusTB.YOffset = 0;
            // 
            // changeTB
            // 
            resources.ApplyResources(this.changeTB, "changeTB");
            this.changeTB.CellHeight = 26;
            this.changeTB.CellWidth = 249;
            this.changeTB.Column = 1;
            this.changeTB.ColumnsSpanned = 0;
            this.changeTB.Name = "changeTB";
            this.changeTB.ReadOnly = true;
            this.changeTB.Row = 0;
            this.changeTB.RowsSpanned = 0;
            this.changeTB.YOffset = 0;
            // 
            // UserTB2
            // 
            resources.ApplyResources(this.UserTB2, "UserTB2");
            this.UserTB2.CellHeight = 26;
            this.UserTB2.CellWidth = 248;
            this.UserTB2.Column = 3;
            this.UserTB2.ColumnsSpanned = 0;
            this.UserTB2.Name = "UserTB2";
            this.UserTB2.ReadOnly = true;
            this.UserTB2.Row = 0;
            this.UserTB2.RowsSpanned = 0;
            this.UserTB2.YOffset = 0;
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
            this.statusLbl.YOffset = 3;
            // 
            // dateTB
            // 
            resources.ApplyResources(this.dateTB, "dateTB");
            this.dateTB.CellHeight = 26;
            this.dateTB.CellWidth = 249;
            this.dateTB.Column = 1;
            this.dateTB.ColumnsSpanned = 0;
            this.dateTB.Name = "dateTB";
            this.dateTB.ReadOnly = true;
            this.dateTB.Row = 1;
            this.dateTB.RowsSpanned = 0;
            this.dateTB.YOffset = 0;
            // 
            // filesLbl
            // 
            resources.ApplyResources(this.filesLbl, "filesLbl");
            this.filesLbl.CellHeight = 60;
            this.filesLbl.CellWidth = 71;
            this.filesLbl.Column = 0;
            this.filesLbl.ColumnsSpanned = 0;
            this.filesLbl.Name = "filesLbl";
            this.filesLbl.Row = 5;
            this.filesLbl.RowsSpanned = 0;
            this.filesLbl.YOffset = 0;
            // 
            // filesTB
            // 
            resources.ApplyResources(this.filesTB, "filesTB");
            this.filesTB.CellHeight = 60;
            this.filesTB.CellWidth = 543;
            this.filesTB.Column = 1;
            this.filesTB.ColumnsSpanned = 2;
            this.filesTB.Name = "filesTB";
            this.filesTB.ReadOnly = true;
            this.filesTB.Row = 5;
            this.filesTB.RowsSpanned = 0;
            this.filesTB.YOffset = 6;
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
            this.userLbl2.YOffset = 3;
            // 
            // jobsTB
            // 
            resources.ApplyResources(this.jobsTB, "jobsTB");
            this.jobsTB.CellHeight = 60;
            this.jobsTB.CellWidth = 543;
            this.jobsTB.Column = 1;
            this.jobsTB.ColumnsSpanned = 2;
            this.jobsTB.Name = "jobsTB";
            this.jobsTB.ReadOnly = true;
            this.jobsTB.Row = 4;
            this.jobsTB.RowsSpanned = 0;
            this.jobsTB.YOffset = 6;
            // 
            // jobsLbl
            // 
            resources.ApplyResources(this.jobsLbl, "jobsLbl");
            this.jobsLbl.CellHeight = 60;
            this.jobsLbl.CellWidth = 71;
            this.jobsLbl.Column = 0;
            this.jobsLbl.ColumnsSpanned = 0;
            this.jobsLbl.Name = "jobsLbl";
            this.jobsLbl.Row = 4;
            this.jobsLbl.RowsSpanned = 0;
            this.jobsLbl.YOffset = 0;
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
            this.workspaceLbl2.YOffset = 3;
            // 
            // descriptionTB
            // 
            resources.ApplyResources(this.descriptionTB, "descriptionTB");
            this.descriptionTB.CellHeight = 62;
            this.descriptionTB.CellWidth = 543;
            this.descriptionTB.Column = 1;
            this.descriptionTB.ColumnsSpanned = 2;
            this.descriptionTB.Name = "descriptionTB";
            this.descriptionTB.ReadOnly = true;
            this.descriptionTB.Row = 3;
            this.descriptionTB.RowsSpanned = 0;
            this.descriptionTB.YOffset = 6;
            // 
            // descriptionLbl
            // 
            resources.ApplyResources(this.descriptionLbl, "descriptionLbl");
            this.descriptionLbl.CellHeight = 62;
            this.descriptionLbl.CellWidth = 71;
            this.descriptionLbl.Column = 0;
            this.descriptionLbl.ColumnsSpanned = 0;
            this.descriptionLbl.Name = "descriptionLbl";
            this.descriptionLbl.Row = 3;
            this.descriptionLbl.RowsSpanned = 0;
            this.descriptionLbl.YOffset = 0;
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
            this.typeLbl.YOffset = 3;
            // 
            // PendingChangelistsToolWindowControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PendingChangelistsToolWindowControl";
            this.Load += new System.EventHandler(this.PendingChangelistsToolWindowControl_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gridLayoutPanel2.ResumeLayout(false);
            this.gridLayoutPanel2.PerformLayout();
            this.changelistContextMenuStrip.ResumeLayout(false);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
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

		bool ContextMenuInitialized = false;

		void UpdateMenuItem(ToolStripMenuItem item, bool state)
		{
			item.Enabled = state;
			item.Visible = state;
		}

		private void clearContext()
		{
			ContextMenuInitialized = false;

			UpdateMenuItem(submitTSMI, false);
			toolStripSeparator1.Visible = false;

			UpdateMenuItem(revertUnchangedFilesTMSI, false);

			UpdateMenuItem(revertFilesTMSI, false);
			UpdateMenuItem(revertTMSI, false);
			UpdateMenuItem(resolveFilesTMSI, false);
			UpdateMenuItem(moveFilesToAnTMSI, false);
			UpdateMenuItem(moveToAnotherChangelistTMSI, false);
			UpdateMenuItem(resolveTMSI, false);

			toolStripSeparator2.Visible = false;

			UpdateMenuItem(shelveFilesTSMI, false);
			UpdateMenuItem(shelveTSMI, false);
			UpdateMenuItem(submitShelvedFilesTMSI, false);
			UpdateMenuItem(unshelveFilesTMSI, false);
			UpdateMenuItem(unshelveTMSI, false);
			UpdateMenuItem(deleteShelvedFilesTMSI, false);
			UpdateMenuItem(openShelvedFileTSMI, false);
			UpdateMenuItem(deleteTSMI, false);

			toolStripSeparator3.Visible = false;

			UpdateMenuItem(diffAgainstHaveTMSI, false);
			UpdateMenuItem(diffAgainstTSMI, false);
			UpdateMenuItem(diffAgainstHavesTMSI, false);
			// need this enabled to launch with shortcut key
			// when context menu is closed?
			//diffAgainstSourceRevToolStripMenuItem.Enabled = false;
			diffAgainstSourceRevTMSI.Visible = false;
			UpdateMenuItem(diffAgainstWorkspaceFileTMSI, false);

			toolStripSeparator4.Visible = false;

			UpdateMenuItem(changeFiletypeTSMI, false);
			UpdateMenuItem(lockTSMI, false);

			toolStripSeparator5.Visible = false;

			UpdateMenuItem(newPendingChangelistTMSI, false);
			UpdateMenuItem(editPendingChangelistTMSI, false);
			UpdateMenuItem(deletePendingChangelistTMSI, false);
			UpdateMenuItem(changeOwnershipTMSI, false);

			toolStripSeparator6.Visible = false;

			UpdateMenuItem(refreshPendingChangelistListTMSI, false);
			UpdateMenuItem(refreshPendingChangelistTMSI, false);
			UpdateMenuItem(removeJobTMSI, false);

			SwarmReviewTSS.Visible = false;

			UpdateMenuItem(RequestReviewTSMI, false);
			UpdateMenuItem(UpdateReviewTSMI, false);
			UpdateMenuItem(openReviewInSwarmTSMI, false);
		}

		private void refreshPendingList()
		{
            // update the column headers to reflect whether or not the server is Swarm enabled

            pendingTreeListView.BeginUpdate();

            pendingTreeListView.Columns.Clear();
			int idx = 0;
			foreach (ColumnHeader h in DefaultListColumns)
			{
				// columns 1 and 2 are review id and state, so not needed if no swarm
                if (((Scm != null) && Scm.Connection.isSwarmEnabled()) || ((idx != 1) && (idx != 2)))
				{
					pendingTreeListView.Columns.Add(h);
				}
				idx++;
			}
            pendingTreeListView.EndUpdate();

            filterBtn.Enabled = false;
			pendingTreeListView.Enabled = false;
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
				pendingTreeListView.Enabled = true;
				return;
			}
			PathFilterText = pathCB.Text;
			UserFilterText = userCB.Text;
			WorkspaceFilterText = workspaceCB.Text;
			threadMonitorControl1.Visible = false;

			if (Scm != null)
			{
				//if (Scm.SwarmEnabled == false)
				//{
				//    // hide the review status if not hooked up to a swarm server
				//    pendingTreeListView.Columns[1].Width = 0;
				//    pendingTreeListView.Columns[2].Width = 0;
				//}
				//else
				//{
				//    if (pendingTreeListView.Columns[1].Width <= 0)
				//    {
				//        pendingTreeListView.Columns[1].Width = 80;
				//    }
				//    if (pendingTreeListView.Columns[2].Width <= 0)
				//    {
				//        pendingTreeListView.Columns[2].Width = 80;
				//    }
				//}
				FillInListProc = new Thread(new ThreadStart(AsyncPopulateListView));
				FillInListProc.IsBackground = true;

				FillInListProc.Start();
			}
			pendingTreeListView.Enabled = true;
			return;

		}

        Dictionary<int, P4ChangeTreeListViewItem> ChangeItemMap = null;

        private void AsyncPopulateListView()
		{
			if (maxItems == 0)
			{
				maxItems = -1;
			}

			bool threadAborted = false;
			if (pendingTreeListView.InvokeRequired)
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
						if (Scm == null)
						{
							P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
							if (P4VSService != null) Scm = P4VSService.ScmProvider;
						}

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
						{
							path = null;
						}

						else
						{
							if (PathFilterText.StartsWith("//"))
							{
								path.DepotPath = new P4.DepotPath(PathFilterText);
							}
							else
							{
								path.LocalPath = new P4.LocalPath(PathFilterText);
							}
						}

						if (string.IsNullOrEmpty(WorkspaceFilterText))
						{
							workspaceName = null;
						}
						else
						{
							workspaceName = WorkspaceFilterText;
						}

						if (string.IsNullOrEmpty(UserFilterText))
						{
							user = null;
						}
						else
						{
							user = UserFilterText;
						}

						IList<P4.Changelist> fullList = new List<P4.Changelist>();
						P4.Options opts = new P4.Options();

						// get the default changelist for the current client even if empty
                        if ((path == null) && (user == Scm.Connection.User || user == null) &&
                            (workspaceName == Scm.Connection.Workspace || workspaceName == null))
						{
							P4.Changelist current = Scm.Connection.Repository.NewChangelist();
                            current.ClientId = Scm.Connection.Workspace;
                            current.OwnerName = Scm.Connection.User;
							current.Shelved = false;
							current.Type = ChangeListType.None;
							current.Description = Resources.DefaultChangeListDescription;
							fullList.Add(current);
						}

						opts["-c"] = "default";
						opts["-a"] = null;
						if (workspaceName != null)
						{
							opts["-C"] = workspaceName;
						}
						if (user != null && Scm.ServerVersion >= Versions.V9_1)
						{
							opts["-u"] = user;
						}

						IList<P4.FileSpec> fileFilters = null;
						if (path != null)
						{
							P4.DepotPath dps = null;
							if (path.DepotPath != null)
							{
								dps = new P4.DepotPath(path.DepotPath.Path);
							}
							P4.ClientPath cps = null;
							if (path.ClientPath != null)
							{
								cps = new P4.ClientPath(path.ClientPath.Path);
							}
							P4.LocalPath lps = null;
							if (path.LocalPath != null)
							{
								lps = new P4.LocalPath(path.LocalPath.Path);
							}
							P4.FileSpec fileFilter = new P4.FileSpec(dps, cps, lps, null);
							fileFilters = new List<P4.FileSpec>();
							fileFilters.Add(fileFilter);
						}

						bool foundDupe = false;
						IList<P4.File> opened = Scm.GetOpenedFiles(fileFilters, opts);
						if (opened != null)
						{
							foreach (P4.File file in opened)
							{
								if (user != null && file.User != user)
								{
									continue;
								}
								if (fullList.Count > 0)
								{
									foreach (P4.Changelist d in fullList)
									{
										if (d.ClientId == file.Client && d.OwnerName == file.User)
										{
											foundDupe = true;
											break;
										}
									}
									if (foundDupe == true)
									{
										foundDupe = false;
										continue;
									}
								}

								if (fullList.Count == maxItems)
								{
									break;
								}
								P4.Changelist dfault = Scm.Connection.Repository.NewChangelist();
                                dfault.ClientId = file.Client;
								dfault.OwnerName = file.User;
								dfault.Shelved = false;
								dfault.Type = ChangeListType.None;
								dfault.Description = string.Empty;

                                if (dfault.ClientId == Scm.Connection.Workspace)
								{
									dfault.Description = Resources.DefaultChangeListDescription;
								}

								fullList.Add(dfault);
							}
						}

						int remainingItems = -1;

						if (maxItems != -1)
						{
							remainingItems = maxItems - fullList.Count;
						}

						if (remainingItems != 0)
						{
							// get numbered changes
							IList<P4.Changelist> changelists = Scm.GetChangelists(flags, workspaceName, remainingItems,
																				  P4.ChangeListStatus.Pending, user, path);
							if (changelists != null)
							{
								foreach (P4.Changelist change in changelists)
								{
									fullList.Add(change);
								}
							}
						}

                        // check to see if scm has disconected by this point

                        if(Scm.Connected)
                        {
                            if ((fullList == null) || (fullList.Count <= 0))
                            {
                                TreeListViewItem it = new TreeListViewItem(null, Resources.JobsToolWindowControl_NoItemsAvailable, true);

                                if (pendingTreeListView.InvokeRequired)
                                {
                                    pendingTreeListView.Invoke(
                                        new PendingTreeListViewItemDelegate(this.pendingTreeListView.Nodes.Add), it);
                                    this.matchesLbl.Invoke(new setStringPropertyDelegate(setPendingMatchesLblText), Resources.JobsToolWindowControl_NoMatches);
                                    pendingTreeListView.Invoke(
                                        new PendingTreeListViewDelegate(this.pendingTreeListView.BuildTreeList));
                                }
                                else
                                {
                                    this.pendingTreeListView.Nodes.Add(it);
                                    this.matchesLbl.Text = Resources.JobsToolWindowControl_NoMatches;
                                    this.pendingTreeListView.BuildTreeList();
                                }

                                return;
                            }

                            threadMonitorControl1.Value = 0;
                            //threadMonitorControl1.Show(FillInListProc, fullList.Count);


                            P4ChangeTreeListViewItem[] items = new P4ChangeTreeListViewItem[fullList.Count];
                            int cnt = 0;
                            int stepCnt = 25;
                            if (fullList.Count >= 260)
                            {
                                stepCnt = fullList.Count / 10;
                            }

                            int sleepTime = 300;
                            try
                            {
                                Dictionary<int, SwarmApi.SwarmServer.Review> ChangeReviewMap = null;

                                if (Scm.Connection.Swarm.SwarmEnabled)
                                {
                                    ChangeReviewMap = new Dictionary<int, SwarmApi.SwarmServer.Review>();

                                    foreach (P4.Changelist change in fullList)
                                    {
                                        if (change.Id > 0)
                                        {
                                            ChangeReviewMap.Add(change.Id, null);
                                        }
                                    }
                                    // make sure was more than the default cl in the list
                                    if (ChangeReviewMap.Count > 0)
                                    {
                                        Scm.Connection.Swarm.IsChangelistAttachedToReview(ChangeReviewMap);
                                    }
                                    else
                                    {
                                        ChangeReviewMap = null;
                                    }
                                }

                                threadMonitorControl1.CancelPressed = false;

                                if (ChangeItemMap == null)
                                {
                                    ChangeItemMap = new Dictionary<int, P4ChangeTreeListViewItem>();
                                }

                                for (int idx = 0; idx < fullList.Count; idx++)
                                {
                                    P4.Changelist changelist = fullList[idx];

                                    SwarmApi.SwarmServer.Review review = null;
                                    if ((ChangeReviewMap != null) && (ChangeReviewMap.ContainsKey(changelist.Id)))
                                    {
                                        review = ChangeReviewMap[changelist.Id];
                                    }

                                    items[idx] = new P4ChangeTreeListViewItem(null, changelist, review, Scm, PendingTLVIFields);
                                    ChangeItemMap[changelist.Id] = items[idx];

                                    // determine the pending changelist icon
                                    items[idx].ChildNodes.Add(new TreeListViewItem());

                                    if (pendingTreeListView.InvokeRequired)
                                    {
                                        if (idx < pendingTreeListView.Items.Count)
                                        {
                                            pendingTreeListView.Invoke(
                                                new ReplacePendingTreeListViewItemDelegate(replacePendingListViewItem), idx, items[idx]);

                                        }
                                        else
                                        {
                                            pendingTreeListView.Invoke(
                                                new PendingTreeListViewItemDelegate(this.pendingTreeListView.Nodes.Add), items[idx]);
                                        }

                                    }
                                    else
                                    {
                                        if (idx < pendingTreeListView.Items.Count)
                                        {
                                            pendingTreeListView.Items[idx] = items[idx];
                                        }
                                        else
                                        {
                                            this.pendingTreeListView.Nodes.Add(items[idx]);
                                        }
                                    }
                                    ++cnt;
                                    if ((cnt % stepCnt) == 0)
                                    {
                                        threadMonitorControl1.Value = cnt;

                                        sleepTime = Math.Max(250, (cnt * 3) / 2);
                                        Thread.Sleep(sleepTime); // yield to let the progress bar update
                                    }
                                    if ((!pendingTreeListView.Updating) && (cnt > pendingTreeListView.PageSize + 1))
                                    {
                                        threadMonitorControl1.Show(fullList.Count);

                                        if (pendingTreeListView.InvokeRequired)
                                        {
                                            pendingTreeListView.Invoke(
                                                new PendingTreeListViewDelegate(pendingTreeListView.BeginUpdate));
                                        }
                                        else
                                        {
                                            pendingTreeListView.BeginUpdate();
                                        }
                                    }
                                    else
                                    {
                                        if (pendingTreeListView.InvokeRequired)
                                        {
                                            pendingTreeListView.Invoke(
                                                new PendingTreeListViewDelegate(this.pendingTreeListView.BuildTreeList));
                                        }
                                        else
                                        {
                                            this.pendingTreeListView.BuildTreeList();
                                        }
                                    }
                                    if (threadMonitorControl1.CancelPressed)
                                    {
                                        break;
                                    }
                                }

                                pendingTreeListView.BeforeExpand += new TreeListViewEvent(before);
                            }
                            finally
                            {
                                try
                                {
                                    if (!pendingTreeListView.IsDisposed)
                                    {
                                        if (pendingTreeListView.InvokeRequired)
                                        {
                                            pendingTreeListView.Invoke(new PendingTreeListViewDelegate(pendingTreeListView.EndUpdate));
                                        }
                                        else
                                        {
                                            pendingTreeListView.EndUpdate();
                                        }
                                        if (pendingTreeListView.InvokeRequired)
                                        {
                                            pendingTreeListView.Invoke(
                                                new PendingTreeListViewDelegate(this.pendingTreeListView.BuildTreeList));
                                        }
                                        else
                                        {
                                            this.pendingTreeListView.BuildTreeList();
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        else
                        {
                            // return with no list and
                            // no connection
                            if (this.matchesLbl.InvokeRequired)
                            {
                                this.matchesLbl.Invoke(new setStringPropertyDelegate(setPendingMatchesLblText),
                                    Resources.JobsToolWindowControl_NoConnection);
                            }
                            else
                            {
                                this.matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
                            }

                            return;
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
						threadMonitorControl1.Hide();
						FillInListProc = null;

						if (!threadAborted || threadMonitorControl1.CancelPressed)
						{
							//user canceled, not aborted by new request
							string itemCountStr = Resources.JobsToolWindowControl_1Match;
							if (pendingTreeListView.Nodes.Count > 1)
							{
								if (pendingTreeListView.Items.Count == maxItems)
								{
									itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches,
										pendingTreeListView.Nodes.Count+"+");
								}
								else
								{
									itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches,
										pendingTreeListView.Nodes.Count);
								}
							}

							if (this.matchesLbl.InvokeRequired)
							{
								this.matchesLbl.Invoke(new setStringPropertyDelegate(setPendingMatchesLblText), itemCountStr);
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
				if (!pendingTreeListView.IsDisposed)
				{
					if (pendingTreeListView.InvokeRequired)
					{
						this.filterBtn.Invoke(new setFilterBtnDelegate(setFilterBtnBool),
                            ((Scm != null) && (Scm.Connected)));
					}
					else
					{
						filterBtn.Enabled = ((Scm != null) && (Scm.Connected));
					}
				}
			}
		}
		Thread FillInListProc = null;

		private int maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);

		internal void filterBtn_Click(object sender, EventArgs e)
		{
			if (pendingTreeListView.InvokeRequired)
			{
				pendingTreeListView.Invoke(new PendingTreeListViewDelegate(this.pendingTreeListView.Items.Clear));
				pendingTreeListView.Invoke(new PendingTreeListViewDelegate(this.pendingTreeListView.Nodes.Clear));
			}
			else
			{
				pendingTreeListView.Items.Clear();
				pendingTreeListView.Nodes.Clear();
			}

		   maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
			if (maxItems == 0)
			{
				maxItems = -1;
			}
			refreshPendingList();
		}


		public int refreshChangelistObject(TreeListViewItem tlvi)
		{
			P4ChangeTreeListViewItem item = tlvi as P4ChangeTreeListViewItem;
			if (item==null)
			{
				return 0;
			}

            item.FileData = item.FetchFileData();
            item.NeedsResolve = false;

            if (item.FileData!=null)
            {
                foreach (FileMetaData file in item.FileData)
                {
                    if (file.Unresolved)
                    {
                        item.NeedsResolve = true;
                        break;
                    }
                }
            }

            P4.Changelist changelist = item.ChangeData;
			SwarmApi.SwarmServer.Review review = item.ReviewData;
			if (changelist != null)
			{
				if (changelist.Id > 0)
				{
					changelist = Scm.GetChangelist(changelist.Id);
					if (changelist != null)// no longer pending
					{
						if (changelist.Pending == false) // no longer pending
						{
							changelist = null;
						}
						else
						{
                            // see if it was attached to a review as part of the update
                            // adding a try catch in the edge case that a swarm config
                            // or connection was broken prior to the refresh
                            try
                            {
                                review = Scm.Connection.Swarm.IsChangelistAttachedToReview(changelist.Id);
                            }
                            catch
                            {
                                // do nothing, error messages have already been raised
                                // where needed, refresh fails
                            }
                        }
					}
				}
			}
			if (changelist != null)
			{
				try
				{
					bool expanded = tlvi.Expanded;
					pendingTreeListView.BeginUpdate();
					if (expanded)
					{
						pendingTreeListView.CollapseNode(item);
					}
					//P4ChangeTreeListViewItem itemToAdd = new P4ChangeTreeListViewItem(null, changelist, review, Scm, PendingTLVIFields);
					//int childIdx = pendingTreeListView.Items.IndexOf(item);
					//pendingTreeListView.Nodes.RemoveAt(childIdx);
					//pendingTreeListView.Items.Insert(childIdx, itemToAdd);
					item.SetData(changelist, review);
                    updateDetails(changelist);
                    if (expanded)
					{
						expandChangelistObject(item);
						pendingTreeListView.ExpandNode(item);
					}
					return 0;
				}
				finally
				{
					pendingTreeListView.EndUpdate();
                    // we changed the data for the selected changelist, 
                    // so pretend the selection changed to force update of contxt menus
                    pendingTreeListView_SelectedIndexChanged(this, null);
				}
			}
			else
			{
				try
				{
					pendingTreeListView.BeginUpdate();
                    pendingTreeListView.CollapseNode(item);
					pendingTreeListView.Nodes.Remove(item);
					int childIdx = pendingTreeListView.Items.IndexOf(item);
					if (childIdx >= 0)
					{
						pendingTreeListView.Items.RemoveAt(childIdx);
					}
					return 0;
				}
				finally
				{
					pendingTreeListView.EndUpdate();
                    string itemCountStr = Resources.JobsToolWindowControl_1Match;
                    if (pendingTreeListView.Nodes.Count > 1)
                    {
                        if (pendingTreeListView.Items.Count == maxItems)
                        {
                            itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches,
                                pendingTreeListView.Nodes.Count + "+");
                        }
                        else
                        {
                            itemCountStr = string.Format(Resources.JobsToolWindowControl_nMatches,
                                pendingTreeListView.Nodes.Count);
                        }
                    }

                    if (this.matchesLbl.InvokeRequired)
                    {
                        this.matchesLbl.Invoke(new setStringPropertyDelegate(setPendingMatchesLblText), itemCountStr);
                    }
                    else
                    {
                        this.matchesLbl.Text = itemCountStr;
                    }
                    Scm.BroadcastChangelistUpdate(null, new P4ScmProvider.ChangelistUpdateArgs(item.ChangeData.Id,
P4ScmProvider.ChangelistUpdateArgs.UpdateType.Delete));
                    P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
                    P4VSService.UpdateChangesStatusBar(-1);
                }
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
			P4.Options opts = null;
			if (change.Id > 0 && Scm.ServerVersion >= Versions.V9_2)
			{
				opts = new P4.Options();
				opts["-s"] = null;
			}
			P4.Changelist changeInfo = Scm.GetChangelist(change.Id, opts);

			if (changeInfo == null)
			{
				return;
			}

			if (changeInfo.Id > 0)
			{ 
				changeTB.Text = changeInfo.Id.ToString();

				DateTime local = changeInfo.ModifiedDate;

				// we need a pref for local time, until then, don't do this:
				//local = TimeZone.CurrentTimeZone.ToLocalTime(local);

				if (Preferences.LocalSettings.GetBool("P4Date_format", true))
				{
					dateTB.Text = local.ToString("yyyy/MM/dd HH:mm:ss");
				}
				else
				{
					dateTB.Text = local.ToString();
				}
			}
			else
			{ 
				changeTB.Text = Resources.Changelist_Default; 
			}
			changeTB.Refresh();

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
			{ statusTB.Text = "pending"; }
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
					sb.AppendLine(file.DepotPath.Path.ToString());
				}
				filesTB.Text = sb.ToString();
			}
			filesTB.Refresh();
		}

		public int expandChangelistObject(TreeListViewItem item)
		{
			int maxFiles = Preferences.LocalSettings.GetInt("Number_files", 1000);

			P4ChangeTreeListViewItem change = item as P4ChangeTreeListViewItem;
			if (change != null)
			{
				change.ChildNodes.Clear();

				if (change.FileData != null)
				{
					if (change.FileData.Count <= maxFiles)
					{
						foreach (P4.FileMetaData f in change.FileData)
						{
							P4FileTreeListViewItem fileNode = new P4FileTreeListViewItem(change, f, new List<object> { P4FileTreeListViewItem.SubItemFlag.DepotFullPath });
							fileNode.FullLine = true;
                            if (fileNode.FileData.HaveRev>=0)
                            {
                                fileNode.Text += " #" + fileNode.FileData.HaveRev + "/" + fileNode.FileData.HeadRev;
                            }
                            fileNode.Text += " <" + f.Type.ToString() + ">";
                            change.ChildNodes.Add(fileNode);
						}
					}
					else
					{
						string msg = string.Format(Resources.PendingChangelistsToolWindowControl_FileCount, change.FileData.Count); ;
						P4ObjectTreeListViewItem message = new P4ObjectTreeListViewItem(change, msg, true);
						change.ChildNodes.Add(message);
					}
				}
				if (change.ShelvedFileData != null && !change.Text.Contains("Shelved"))
				{
					if (change.ShelvedFileData.Count <= maxFiles)
					{
                        Changelist cl = Scm.Connection.Repository.NewChangelist();
                        cl.ShelvedFiles = change.ShelvedFileData;
                        P4ShelvedFolderTreeListViewItem shelveFolder = new P4ShelvedFolderTreeListViewItem(change,cl, Scm);
                        shelveFolder.Text = "Shelved Files (" + cl.ShelvedFiles.Count + ")";
                        change.ChildNodes.Add(shelveFolder);

                        foreach (P4.ShelvedFile s in change.ShelvedFileData)
                        {
                            P4ShelvedFileTreeListViewItem fileNode = new P4ShelvedFileTreeListViewItem(change, s, new List<object> { P4ShelvedFileTreeListViewItem.SubItemFlag.Path });
                            fileNode.FullLine = true;
                            shelveFolder.ChildNodes.Add(fileNode);
                        }
                    }

					else
					{
						string msg = string.Format(Resources.PendingChangelistsToolWindowControl_PendingFileCount, change.ChangeData.ShelvedFiles.Count); ;
						P4ObjectTreeListViewItem message = new P4ObjectTreeListViewItem(change, msg, true);
						change.ChildNodes.Add(message);
					}
				}
                if (change.JobData != null && change.JobData.Count > 0)
				{
					foreach (string jobId in change.JobData.Keys)
					{
						P4JobTreeListViewItem jobNode = new P4JobTreeListViewItem(change, jobId);
						change.ChildNodes.Add(jobNode);
					}
				}

			}
			return 0;
		}
		public void before(object sender, TreeListViewEventArgs args)
		{
			expandChangelistObject(args.Node);
            expandShelvedFolderObject(args.Node);
		}

		public void after(object sender, TreeListViewEventArgs args)
		{
			//MessageBox.Show("after expand");
		}

        public int expandShelvedFolderObject(TreeListViewItem item)
        {
            int maxFiles = Preferences.LocalSettings.GetInt("Number_files", 1000);

            P4ShelvedFolderTreeListViewItem shelvedFolder = item as P4ShelvedFolderTreeListViewItem;

            if (shelvedFolder != null)
            {
                shelvedFolder.ChildNodes.Clear();

                if (shelvedFolder.ShelvedFileData != null)
                {
                    if (shelvedFolder.ShelvedFileData.Count <= maxFiles)
                    {
                        foreach (P4.ShelvedFile s in shelvedFolder.ShelvedFileData)
                        {
                            P4ShelvedFileTreeListViewItem fileNode = new P4ShelvedFileTreeListViewItem(shelvedFolder, s, new List<object> { P4ShelvedFileTreeListViewItem.SubItemFlag.Path });
                            fileNode.FullLine = true;
                            if (s.Revision > 0)
                            {
                                fileNode.Text += " #" + s.Revision;
                            }
                            fileNode.Text += " <" + s.Type.ToString() + ">";
                            shelvedFolder.ChildNodes.Add(fileNode);
                        }
                    }
                    else
                    {
                        string msg = string.Format(Resources.PendingChangelistsToolWindowControl_PendingFileCount, shelvedFolder.ChangeData.ShelvedFiles.Count); ;
                        P4ObjectTreeListViewItem message = new P4ObjectTreeListViewItem(shelvedFolder, msg, true);
                        shelvedFolder.ChildNodes.Add(message);
                    }
                }
            }
            return 0;
        }

        private void PendingChangelistsToolWindowControl_Load(object sender, EventArgs e)
		{
			// store the full column header list for later use
			DefaultListColumns = new List<ColumnHeader>();

			pendingTreeListView.Columns.Add(Resources.PendingChangelistsToolWindowControl_ClmTitleChange, 120);
			pendingTreeListView.Columns.Add(Resources.PendingChangelistsToolWindowControl_ClmTitleReviewId, 130);
			pendingTreeListView.Columns.Add(Resources.PendingChangelistsToolWindowControl_ClmTitleReviewStatus, 130);
			pendingTreeListView.Columns.Add(Resources.PendingChangelistsToolWindowControl_ClmTitleDate, 130);
			pendingTreeListView.Columns.Add(Resources.PendingChangelistsToolWindowControl_ClmTitleWorkspace, 180);
			pendingTreeListView.Columns.Add(Resources.PendingChangelistsToolWindowControl_ClmTitleUser, 80);
			pendingTreeListView.Columns.Add(Resources.PendingChangelistsToolWindowControl_ClmTitleAccessType, 80);
			pendingTreeListView.Columns.Add(Resources.PendingChangelistsToolWindowControl_ClmTitleDescription, 100);
			pendingTreeListView.Columns[7].Width = -2;

			foreach (ColumnHeader h in pendingTreeListView.Columns)
			{
				DefaultListColumns.Add(h);
			}

			matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;

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

				//if (userCB.Text == ""&&
				//    !(userCB.mruValues.Contains("")))
				//{
				//    selectionChangedByLoad = true;
				//    userCB.Text = Scm.User;
				//}

				//if (workspaceCB.Text == ""&&
				//    !(workspaceCB.mruValues.Contains("")))
				//{
				//    selectionChangedByLoad = true;
				//    workspaceCB.Text = Scm.Workspace;
				//}


				Scm.ChangelistUpdated += changeUpdated;
			}

			refreshPendingList();
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
			//selectionChangedByLoad = true;
			//userCB.Text = Scm.Repository.Connection.UserName;
			//selectionChangedByLoad = true;
			//workspaceCB.Text = Scm.Repository.Connection.Client.Name;
		}

		public P4ObjectTreeListViewItem currentselection = null;
		public P4ChangeTreeListViewItem currentChangelist = null;

		//private void setDetails(P4.Changelist change)
		//{
		//    changeTB.Text = change.Id.ToString();	
		//    dateTB.Text = change.ModifiedDate.ToShortDateString() + " " + change.ModifiedDate.ToShortTimeString();
		//    workspaceTB2.Text = change.ClientId;
		//    descriptionTB.Text = change.Description;
		//    UserTB2.Text = change.OwnerName;
		//    statusTB.Text =  change.Pending?"Pending":"Submitted";
		//    typeTB.Text = change.Type.ToString();
		//    jobsTB.Text = string.Empty;
		//    if (change.Jobs != null)
		//    {
		//        foreach (string jobId in change.Jobs.Keys)
		//        {
		//            jobsTB.Text += jobId + "\r\n";
		//        }
		//    }
		//    filesTB.Text = string.Empty;
		//    if (change.Files != null)
		//    {
		//        foreach (FileMetaData file in change.Files)
		//        {
		//            filesTB.Text += file.DepotPath.Path + "\r\n";
		//        }
		//    }
		//}


		private void pendingTreeListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			ContextMenuInitialized = false;

			if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
			{
				if (currentselection == pendingTreeListView.SelectedItems[0] as P4ObjectTreeListViewItem)
				{
					return;
				}
			}
			checkConnection();

            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count <= 0)
            {
                // only show the refresh and new options
                clearContext();
				newPendingChangelistTMSI.Enabled = true;
				newPendingChangelistTMSI.Visible = true;
				toolStripSeparator6.Visible = true;
				refreshPendingChangelistListTMSI.Enabled = true;
				refreshPendingChangelistListTMSI.Visible = true;
				return;
			}
            currentselection = pendingTreeListView.SelectedItems[0] as P4ObjectTreeListViewItem;
			P4ChangeTreeListViewItem selectedChange = currentselection as P4ChangeTreeListViewItem;
			if (selectedChange == null)
			{
				// wasn't a changelist selected
				selectedChange = currentselection.ParentItem as P4ChangeTreeListViewItem;
			}
			if (selectedChange == null)
			{
				// only show the refresh and new options
				clearContext();
				newPendingChangelistTMSI.Enabled = true;
				newPendingChangelistTMSI.Visible = true;
				toolStripSeparator6.Visible = true;
				refreshPendingChangelistListTMSI.Enabled = true;
				refreshPendingChangelistListTMSI.Visible = true;
				return;
			}
			if ((currentChangelist == null) || (selectedChange == null) || (selectedChange.ChangeData.Id != currentChangelist.ChangeData.Id))
			{
				currentChangelist = selectedChange;
				// new change selected, fill in the details
				clearDetails();
				if (selectedChange != null)
				{
					updateDetails(selectedChange.ChangeData);
				}
			}
		}

		// submit on changelist or file object
		private void submitToolStripMenuItem_Click(object sender, EventArgs e)
		{
            EnvDTE.DTE dte2;
            dte2 = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));

            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                P4ObjectTreeListViewItem item = pendingTreeListView.SelectedItems[0] as P4ObjectTreeListViewItem;
                if (item == null)
                {
                    // shouldn't happen, get me out of here
                    return;
                }
                // all the selected items are guarenteed to be the same type
                if (item is P4FileTreeListViewItem)
                {
                    List<string> files = new List<string>();
                    // dealing with one or more files that are selected
                    foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                    {
                        P4FileTreeListViewItem fileItem = it as P4FileTreeListViewItem;
                        if (fileItem == null)
                        {
                            continue;
                        }
                        files.Add(fileItem.FileData.LocalPath.Path);
                    }
                    // only do the save all after the list of files is
                    // created from the selected items
                    dte2.ExecuteCommand("File.SaveAll");

                    Dictionary<int, IDictionary<string, P4.FileMetaData>> changeLists = new Dictionary<int, IDictionary<string, P4.FileMetaData>>();
                    if (SubmitDlg.SubmitFiles(files, Scm, false, null, ref changeLists))
                    {
                        foreach (int id in changeLists.Keys)
                        {
                            P4ChangeTreeListViewItem changedItem = null;
                            if (id > 0)
                            {
                                if (ChangeItemMap.ContainsKey(id))
                                {
                                    changedItem = ChangeItemMap[id];
                                }
                            }
                            else if (ChangeItemMap.ContainsKey(0))
                            {
                                changedItem = ChangeItemMap[0];
                            }
                            else if (ChangeItemMap.ContainsKey(-1))
                            {
                                changedItem = ChangeItemMap[-1];
                            }
                            if (changedItem != null)
                            {
                                refreshChangelistObject(changedItem);
                            }
                        }
                    }
                }
                else if ((item is P4ChangeTreeListViewItem) && (item.NodeType.TestAll(nodeTypeFlags.Changelist | nodeTypeFlags.Pending | nodeTypeFlags.Our)))
                {
                    dte2.ExecuteCommand("File.SaveAll");
                    // dealing with one or more pending changelists that are selected
                    foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                    {
                        P4ChangeTreeListViewItem changeItem = it as P4ChangeTreeListViewItem;
                        if (changeItem == null)
                        {
                            continue;
                        }
                        if (SubmitDlg.SubmitPendingChanglist(changeItem.ChangeData, null, Scm))
                        {
                            refreshChangelistObject(changeItem);
                        }
                        else
                        {
                            // user canceled
                            break;
                        }
                    }
                }
            }
        }

		// Changelist objects
		// revert unchanged on changelist object
		private void revertUnchangedFilesTMSI_Click(object sender, EventArgs e)
		{
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4.Changelist changelist = it.Tag as P4.Changelist;
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        continue;
                    }
                    TreeListViewItem selected = pendingTreeListView.SelectedItems[0] as TreeListViewItem;

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

                    List<string> files = new List<string>();

                    changelist = Scm.GetChangelist(changelist.Id);
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        continue;
                    }
                    foreach (P4.FileMetaData fmd in changelist.Files)
                    {
                        files.Add(fmd.DepotPath.Path);
                    }

                    P4.Options opts = new P4.Options();
                    opts["-a"] = null;
                    string path = "//" + Scm.Connection.Workspace + "/...";
                    Scm.RevertFilesInChangelist(opts, changelist, path);

                    refreshChangelistObject(selected);
                    Scm.SccService.UpdateProjectGlyphs(files, true);
                }
            }
		}

        // revert files on changelist object
        private void revertFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4.Changelist changelist = it.Tag as P4.Changelist;
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }
                    TreeListViewItem selected = pendingTreeListView.SelectedItems[0] as TreeListViewItem;

                    if (Scm == null)
                    {
                        // still null
                        return;
                    }

                    if (changelist.Id <= 0)
                    {
                        changelist = Scm.GetChangelist(0, null);
                        if (changelist == null)
                        {
                            MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                                Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            refreshChangelistObject(selected);
                            return;
                        }
                    }
                    P4.Options opts = new P4.Options();
                    opts["-a"] = null;
                    opts["-c"] = changelist.Id.ToString();

                    if (changelist.Id <= 0)
                    {
                        opts["-c"] = "default";
                    }
                    string path = "//" + Scm.Connection.Workspace + "/...";
                    try
                    {
                        Scm.Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(path), opts);
                    }
                    catch (Exception ex)
                    {
                        // If the error is because the repository is now null, it means
                        // the connection was closed in the middle of a command, so ignore it.
                        if (Scm.Connection.Repository != null)
                        {
                            Scm.ShowException(ex);
                        }
                        return;
                    }
                    List<string> files = new List<string>();

                    P4.P4CommandResult results = Scm.Connection.Repository.Connection.LastResults;
                    if (results.TaggedOutput != null)
                    {
                        foreach (P4.TaggedObject obj in results.TaggedOutput)
                        {
                            if (obj.ContainsKey("depotFile"))
                            {
                                files.Add(obj["depotFile"].ToString());
                            }
                            int idx = 0;
                            string key = String.Format("depotFile{0}", idx);
                            if (obj.ContainsKey(key))
                            {
                                while (obj.ContainsKey(key))
                                {
                                    files.Add(obj[key].ToString());
                                    idx++;
                                    key = String.Format("depotFile{0}", idx);
                                }
                            }
                        }
                    }

                    opts = new P4.Options();
                    opts["-n"] = null;
                    opts["-c"] = changelist.Id.ToString();
                    if (changelist.Id <= 0)
                    {
                        opts["-c"] = "default";
                    }
                    Scm.Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(path), opts);
                    List<string> preview = new List<string>();
                    results = Scm.Connection.Repository.Connection.LastResults;

                    if (results.TaggedOutput != null)
                    {
                        foreach (P4.TaggedObject obj in results.TaggedOutput)
                        {
                            if (obj.ContainsKey("depotFile"))
                            {
                                preview.Add(obj["depotFile"].ToString());
                                files.Add(obj["depotFile"].ToString());
                            }
                            int idx = 0;
                            string key = String.Format("depotFile{0}", idx);
                            if (obj.ContainsKey(key))
                            {
                                while (obj.ContainsKey(key))
                                {
                                    preview.Add(obj[key].ToString());
                                    files.Add(obj[key].ToString());
                                    idx++;
                                    key = String.Format("depotFile{0}", idx);
                                }
                            }
                        }
                    }

                    if (preview.Count > 0)
                    {
                        RevertWarnDlg dlg = new RevertWarnDlg(preview);
                        if (revertWarn() == true)
                        {
                            dlg.ShowDialog();
                        }

                        if (dlg.DialogResult == DialogResult.OK)
                        {
                            opts = new P4.Options();
                            Scm.RevertFilesInChangelist(opts, changelist, path);
                            //Scm.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(path), opts);
                        }
                    }
                    refreshChangelistObject(selected);
                    if (files == null)
                    {
                        foreach (P4.FileMetaData fmd in changelist.Files)
                        {
                            files.Add(fmd.DepotPath.Path);
                        }
                    }
                    Scm.SccService.UpdateProjectGlyphs(files, true);
                }
            }
        }

		public bool revertWarn()
		{
			if (Preferences.LocalSettings.ContainsKey("Revert_warn"))
			{
				if ((bool)Preferences.LocalSettings["Revert_warn"] == true)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		// diff against have revisions on changelist object
		private void diffAgainstHavesToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                IList<string> filesToDiff = new List<string>();
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4.Changelist changelist = it.Tag as P4.Changelist;
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                                        Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }
                    TreeListViewItem selected = pendingTreeListView.SelectedItems[0] as TreeListViewItem;



                    string change = changelist.Id.ToString();
                    if (change == "0" || change == "-1")
                    {
                        change = "default";
                    }

                    P4.Options opts = new P4.Options();
                    opts["-e"] = change;
                    //opts["-Olhp"] = null;
                    opts["-Ro"] = null;

                    P4.FileSpec fs = new P4.FileSpec(new P4.DepotPath(@"//..."), null);
                    List<P4.FileSpec> dfs = new List<P4.FileSpec>();
                    dfs.Add(fs);
                    IList<P4.FileMetaData> diffableFiles = Scm.GetFileMetaData(dfs, opts);

                    foreach (P4.FileMetaData data in diffableFiles)
                    {
                        if (data.LocalPath != null && data.Action != FileAction.Delete &&
                            data.Action != FileAction.MoveDelete &&
                            data.Action != FileAction.DeleteInto &&
                            data.Action != FileAction.DeleteFrom &&
                             data.Action != FileAction.Add &&
                             data.Action != FileAction.Branch)
                        {
                            string file = data.LocalPath.Path;
                            filesToDiff.Add(file);
                        }
                    }
                }
                if (filesToDiff.Count > 0)
                {
                    _scm.DiffFiles(filesToDiff.ToArray());
                }
            }
		}

        // resolve files on changelist object
        private void resolveFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                IList<string> filesToResolve = new List<string>();
                IList<string> filesToRefresh = new List<string>();
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4.Changelist changelist = it.Tag as P4.Changelist;
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }
                    TreeListViewItem selected = pendingTreeListView.SelectedItems[0] as TreeListViewItem;

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

                    string change = changelist.Id.ToString();
                    if (change == "0" || change == "-1")
                    {
                        change = "default";
                    }

                    P4.Options opts = new P4.Options();
                    opts["-e"] = change;
                    opts["-Olhp"] = null;
                    opts["-Rcu"] = null;

                    P4.FileSpec fs = new P4.FileSpec(new P4.ClientPath(@"//" + Scm.Connection.Workspace + @"/..."), null);
                    List<P4.FileSpec> lfs = new List<P4.FileSpec>();
                    lfs.Add(fs);
                    IList<P4.FileMetaData> resolveableFiles = Scm.GetFileMetaData(lfs, opts);

                    if (resolveableFiles != null && resolveableFiles.Count > 0)
                    {
                        foreach (P4.FileMetaData data in resolveableFiles)
                        {
                            if (data.LocalPath != null)
                            {
                                string file = data.LocalPath.Path;
                                filesToResolve.Add(file);
                                filesToRefresh.Add(file);
                            }
                        }
                    }
                }

                if (filesToResolve.Count > 0)
                {
                    ResolveFileDlg.ResolveFiles(Scm, filesToResolve, false, null, true);
                    Scm.SccService.UpdateProjectGlyphs(filesToRefresh, true);
                }
                // refresh all the selected changelists
                foreach (P4ChangeTreeListViewItem it in pendingTreeListView.SelectedItems)
                {
                    refreshChangelistObject(it);
                }
            }
        }

        // shelve files on changelist object
        private void shelveFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4.Changelist changelist = it.Tag as P4.Changelist;
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }

                    IList<P4.FileMetaData> files = new List<P4.FileMetaData>();
                    foreach (P4.FileMetaData fmd in changelist.Files)
                    {
                        files.Add(fmd);
                    }

                    ShelveFileDlg.ShelveFiles(files, changelist, Scm, true);

                    if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
                    {
                        TreeListViewItem selected = pendingTreeListView.SelectedItems[0] as TreeListViewItem;
                        if (selected != null)
                        {
                            refreshChangelistObject(selected);
                        }

                    }
                    else
                    {
                        refreshPendingList();
                    }
                    IList<string> localPaths = new List<string>();
                    foreach (P4.FileMetaData md in files)
                    {
                        if (md.LocalPath != null)
                        {
                            localPaths.Add(md.LocalPath.Path.ToString());
                        }
                    }
                    Scm.SccService.UpdateProjectGlyphs(localPaths, true);
                }
            }
        }

        // unshelve files on changelist object
        private void unshelveFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4.Changelist changelist = it.Tag as P4.Changelist;
                    if (changelist.Id == -1)
                    {
                        changelist = currentChangelist.Tag as P4.Changelist;
                    }
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }
                    P4.Options opts = new P4.Options();
                    opts["-S"] = null;
                    changelist = Scm.GetChangelist(changelist.Id, opts);
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }
                    UnshelveFileDialog.UnshelveFiles(Scm, changelist.Id, changelist.ShelvedFiles);
                    if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
                    {
                        TreeListViewItem selected = pendingTreeListView.SelectedItems[0] as TreeListViewItem;
                        if (selected != null)
                        {
                            refreshChangelistObject(selected);
                        }

                    }
                    else
                    {
                        refreshPendingList();
                    }

                    if (changelist.ShelvedFiles != null)
                    {
                        List<string> filesToRefresh = new List<string>();
                        foreach (P4.ShelvedFile sf in changelist.ShelvedFiles)
                        {
                            filesToRefresh.Add(sf.Path.Path);
                        }
                        Scm.SccService.UpdateProjectGlyphs(filesToRefresh, true);
                    }
                }
            }
        }

        // delete shelved files on changelist object
        private void deleteShelvedFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {  
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
                { 
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4.Changelist changelist = it.Tag as P4.Changelist;
                    if (changelist.Id == -1)
                    {
                        changelist = currentChangelist.Tag as P4.Changelist;
                    }
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }
                    P4.Options opts = new P4.Options();
                    opts["-S"] = null;
                    changelist = Scm.GetChangelist(changelist.Id, opts);
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }
                    P4.ShelveFilesCmdFlags dflags = P4.ShelveFilesCmdFlags.Delete;

                    // this is all of the files in the changelist, so we do not need a list of files
                    //IList<string> shelved = new List<string>();
                    //foreach (P4.ShelvedFile shelve in changelist.ShelvedFiles)
                    //{
                    //    shelved.Add(shelve.Path.Path);
                    //}

                    string prompt = string.Format(Resources.PendingChangelistsToolWindowControl_DeleteShelvedFileWarning, changelist.ShelvedFiles[0].Path.Path);
                    if (changelist.ShelvedFiles.Count > 1)
                    {
                        prompt = Resources.PendingChangelistsToolWindowControl_DeleteShelvedFilesWarning;
                    }
                    string caption = Resources.P4VS;

                    if (DialogResult.No != MessageBox.Show(prompt, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        // this is all of the files in the changelist, so we do not need a list of files
                        Scm.ShelveFiles(changelist.Id, null, dflags, true, null);

                        //Scm.ShelveFiles(changelist.Id, null, dflags, true, shelved.ToArray());
                    }
                    TreeListViewItem selected;
                    if (pendingTreeListView.SelectedItems.Count < 1)
                    {
                        selected = currentChangelist as TreeListViewItem;
                    }
                    else
                    {
                        selected = pendingTreeListView.SelectedItems[0] as TreeListViewItem;
                    }

                    refreshChangelistObject(selected);
                }
            }
        }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

		// open shelved file on shelved file object
		private void openShelvedFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                long maxPreviewSize = 1024 * Preferences.LocalSettings.GetInt("Size_files", 500);
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4.ShelvedFile selectedFile = it.Tag as P4.ShelvedFile;
                    if (selectedFile == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }
                    if (selectedFile.Action == FileAction.Delete)
                    {
                        continue;
                    }
                    int _changelistId = -1;
                    TreeListViewItem parent = null;
                    P4.Changelist changelist = null;
                    TreeListViewItem item = pendingTreeListView.SelectedItems[0] as TreeListViewItem;
                    if (item != null)
                    {
                        parent = item.ParentItem.ParentItem;
                    }
                    if (parent != null)
                    {
                        changelist = parent.Tag as P4.Changelist;
                    }
                    if (changelist != null)
                    {
                        _changelistId = changelist.Id;
                    }
                    if (_changelistId <= 0)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }
                    P4.FileSpec fs = FileSpec.DepotSpec(selectedFile.Path.Path);
                    fs.Version = new P4.ShelvedInChangelistIdVersion(_changelistId);

                    long shelvedSize = _scm.GetShelvedFileSize(_changelistId, selectedFile.Path.Path);
                    if (shelvedSize > maxPreviewSize)
                    {
                        string size = P4ObjectTreeListViewItem.PrettyPrintFileSize(shelvedSize);
                        string msg = string.Format(Resources.FileExceedsMaxPreviewSizeWarning, size);
                        if (DialogResult.No == MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        {
                            continue;
                        }
                    }

                    using (TempFile shelvedFile = new TempFile(fs))
                    {
                        if (_scm.GetFileVersion(shelvedFile, fs) == null)
                        {
                            shelvedFile.Dispose();
                            continue;
                        }

                        shelvedFile.ReadOnly = true;

                        if (Preferences.LocalSettings.GetBool("OpenShelvedFileInEditor", true))
                        {
                            EnvDTE.DTE dte = P4VsProvider.GetDTE();

                            string ext = System.IO.Path.GetExtension(shelvedFile);

                            if ((ext.EndsWith("proj", StringComparison.OrdinalIgnoreCase) ||
                                (string.Compare(ext, "sln", true) == 0)))
                            {
                                //
                                //ShowFileContentsDlg dlg = new ShowFileContentsDlg();

                                //dlg.TempFile = shelvedFile;
                                //dlg.Title = selectedFile.ToString();

                                //// Show modeless
                                //dlg.Show();
                                dte.ItemOperations.OpenFile(shelvedFile, "{7651A703-06E5-11D1-8EBD-00A0C90F26EA}" /*EnvDTE.Constants.vsViewKindTextView*/);
                            }
                            else
                            {
                                dte.ItemOperations.OpenFile(shelvedFile, null);// "{7651A701-06E5-11D1-8EBD-00A0C90F26EA}" /*EnvDTE.Constants.vsViewKindCode*/);
                            }
                        }

                        else
                        {
                            ShowFileContentsDlg dlg = new ShowFileContentsDlg();

                            dlg.TempFile = shelvedFile;
                            dlg.Title = selectedFile.ToString();

                            // Show modeless
                            dlg.Show();
                        }
                    }
                }
            }
		}

		void newPendingChangelistDialogClosed(DialogResult result, int changelistId)
		{
			if (result != DialogResult.Cancel)
			{
				refreshPendingList();
			}
		}

		// new pending changelist on changelist object
		private void newPendingChangelistToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Scm == null)
			{
				Scm = new P4ScmProvider(null);
			}
			if (Scm == null)
			{
				// still null
				return;
			}
			try
			{
				PendingChangelistDlg dlg = new PendingChangelistDlg(Scm);

				dlg.ChangeListId = -1;

				P4.ServerMetaData smd =Scm.GetServerMetaData();

				dlg.Text = string.Format(Resources.PendingChangelistsToolWindowControl_PendingChangelistDlgCaption,
                    smd.Address.Uri, Scm.Connection.Repository.Connection.UserName);
                dlg.UserText = Scm.Connection.Repository.Connection.UserName;

                dlg.WorkspaceText = Scm.Connection.Repository.Connection.Client.Name;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    refreshPendingList();
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

		Dictionary<int, TreeListViewItem> ItemsBeingEdited = new Dictionary<int, TreeListViewItem>();

		void editPendingChangelistDialogClosed(DialogResult result, int changelistId)
		{
			try
			{
				TreeListViewItem item = ItemsBeingEdited[changelistId];

				refreshChangelistObject(item);

				if (result == DialogResult.Cancel)
				{
					return;
				}
				if (changelistId == -1)
				{
					refreshPendingList();
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

		// edit pending changelist on changelist object
		private void editPendingChangelistToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Scm == null)
			{
				Scm = new P4ScmProvider(null);
			}
			if (Scm == null)
			{
				// still null
				return;
			}
			if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
			{
                try
                {
                    foreach (P4ChangeTreeListViewItem it in pendingTreeListView.SelectedItems)
                    {
                        P4.Changelist change = it.Tag as P4.Changelist;
                        if (change == null)
                        {
                            MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                                Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            refreshChangelistObject((P4ChangeTreeListViewItem)pendingTreeListView.SelectedItems[0]);
                            return;
                        }
                        P4ChangeTreeListViewItem item = pendingTreeListView.SelectedItems[0] as P4ChangeTreeListViewItem;
                        string changeText = change.Id.ToString();
                        if (change.Id == 0)
                        {
                            changeText = Resources.Changelist_Default; //"Default"
                        }
                        if (change.Id == -1)
                        {
                            changeText = Resources.Changelist_New; //"New"
                        }
                        PendingChangelistDlg dlg = new PendingChangelistDlg(Scm);
                        P4.ServerMetaData smd = Scm.GetServerMetaData();
                        dlg.Text = string.Format(Resources.PendingChangelistsToolWindowControl_PendingChangelistDlgTitle,
                            changeText, smd.Address.Uri, Scm.Connection.Repository.Connection.UserName);
                        dlg.ChangeListId = change.Id;

                        ItemsBeingEdited[change.Id] = item;

                        dlg.OnDialogClosed =
                            new PendingChangelistDlg.PendingChangelistDialogCloseDelegate(editPendingChangelistDialogClosed);

                        dlg.ShowDialog();

                        // refresh the changelist
                        refreshChangelistObject(it);
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
		}

        // delete pending changelist on changelist object
        private void deletePendingChangelistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4.Changelist changelist = it.Tag as P4.Changelist;
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }

                    TreeListViewItem selected = pendingTreeListView.SelectedItems[0] as TreeListViewItem;
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
                    string prompt = string.Empty;
                    string caption = string.Empty;
                    P4.Options opts = new P4.Options();

                    if (Scm.ServerVersion >= Versions.V9_2)
                    {
                        opts["-S"] = null;
                        changelist = Scm.GetChangelist(changelist.Id, opts);

                        if (changelist == null)
                        {
                            MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                                Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            refreshChangelistObject(selected);
                            return;
                        }
                        if (changelist.Shelved == true)
                        {
                            prompt = string.Format(Resources.PendingChangelistsToolWindowControl_DeleteChangelistContainsShelvedFilesWarning1,
                                changelist.Id);
                            caption = Resources.P4VS;

                            if (DialogResult.No != MessageBox.Show(prompt, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                IList<string> shelved = new List<string>();
                                foreach (P4.ShelvedFile shelve in changelist.ShelvedFiles)
                                {
                                    shelved.Add(shelve.Path.Path);
                                }
                                P4.ShelveFilesCmdFlags dflags = P4.ShelveFilesCmdFlags.Delete;
                                Scm.ShelveFiles(changelist.Id, null, dflags, true, shelved.ToArray());
                                refreshChangelistObject(selected);
                            }
                            else
                            {
                                return;
                            }
                        }
                    }

                    prompt = string.Format(Resources.PendingChangelistsToolWindowControl_DeletePendingChangelistWarning, changelist.Id);
                    caption = Resources.PendingChangelistsToolWindowControl_DeletePendingChangelistCaption;

                    if (((changelist.Files.Count == 0) && (changelist.Shelved == false)) || (DialogResult.No != MessageBox.Show(prompt, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question)))
                    {
                        opts = new P4.Options();
                        Scm.DeleteChangelist(changelist);

                        if (selected != null) selected.Selected = false;
                        pendingTreeListView.Refresh();
                        refreshPendingList();
                        refreshChangelistObject(selected);
                    }
                }
            }
        }

        private void changeOwnershipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4.Changelist changelist = it.Tag as P4.Changelist;

                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }

                    TreeListViewItem selected = pendingTreeListView.SelectedItems[0] as TreeListViewItem;
                    string sentUser = Scm.Connection.User;
                    string sentWorkspace = Scm.Connection.Workspace;
                    string cl = changelist.Id.ToString();
                    IList<string> newOwner = ChangeOwnerDlg.Show(sentUser, sentWorkspace, cl, Scm);

                    if (newOwner != null)
                    {
                        changelist = Scm.GetChangelist(changelist.Id);
                        if (changelist == null)
                        {
                            MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                                Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            refreshChangelistObject(selected);
                            return;
                        }
                        changelist.OwnerName = newOwner[0];
                        changelist.ClientId = newOwner[1];
                        Scm.SaveChangelist(changelist, null);
                    }
                    refreshChangelistObject(selected);
                }
            }
        }

		// move all files in a changelist on changelist object
		private void moveFilesToAnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Scm == null)
			{
				P4VsProviderService P4VSService = (P4VsProviderService) GetService(typeof (P4VsProviderService));
				if (P4VSService != null) Scm = P4VSService.ScmProvider;
			}
			if (Scm == null)
			{
				//still null
				return;
			}

			IList<P4.Changelist> changes = Scm.GetAvailibleChangelists(-1);
			Changelist getDesc = Scm.GetChangelist(-1);
			string newChangeDescription = getDesc.Description;

			int changeListId = SelectChangelistDlg.ShowChooseChangelistMoveFilesInChangelist(
				Resources.PendingChangelistsToolWindowControl_SelectChangelistDlgCaption2, 
				changes, ref newChangeDescription, Scm);

			if (changeListId <= -1)
			{
				// user hit cancel or new changelist creation failed
				return;
			}

            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4.Changelist changelist = it.Tag as P4.Changelist;

                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
                        return;
                    }

                    P4.Options opts = new P4.Options();
                    if (changelist.Id > 0)
                    {
                        opts["-c"] = changelist.Id.ToString();
                    }
                    else
                    {
                        opts["-c"] = "default";
                    }
                    IList<P4.File> files = Scm.GetOpenedFiles(null, opts);

                    if (files != null && files.Count != 0)
                    {
                        List<string> solutionExp = new List<string>();
                        IList<P4.FileSpec> fs = new List<P4.FileSpec>();
                        opts = new P4.Options();
                        if (changeListId > 0)
                        {
                            opts["-c"] = changeListId.ToString();
                        }
                        else
                        {
                            opts["-c"] = "default";
                        }
                        foreach (P4.File file in files)
                        {
                            P4.FileSpec f = new P4.FileSpec(file.DepotPath, file.ClientPath, null, file.Version);
                            fs.Add(f);
                            solutionExp.Add(file.DepotPath.Path);
                        }

                        Scm.ReopenFiles(fs, opts);
                        if (Scm.SccService != null)
                        {
                            Scm.SccService.UpdateProjectGlyphs(solutionExp, true);
                        }
                    }

                    else
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_NoFilesToMoveWarning,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    TreeListViewItem fromItem = pendingTreeListView.SelectedItems[0] as TreeListViewItem;
                    if (fromItem != null)
                    {
                        refreshChangelistObject(fromItem);
                    }

                    refreshPendingList();
                    if (opts["-c"] != null)
                    {
                        TreeListViewItem toItem = pendingTreeListView.FindItemWithText(opts["-c"]) as TreeListViewItem;
                        if (toItem != null)
                        {
                            refreshChangelistObject(toItem);
                        }
                        else
                        {
                            Scm.BroadcastChangelistUpdate(null, new P4ScmProvider.ChangelistUpdateArgs(changeListId,
                            P4ScmProvider.ChangelistUpdateArgs.UpdateType.ContentUpdate));
                        }
                    }
                }
            }
		}

		// refresh pending changelist list on changelist object
		private void refreshPendingChangelistListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			filterBtn_Click(null, null);
		}

		// refresh individual pending changelist on changelist object
		private void refreshPendingChangelistToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    TreeListViewItem selected = it as P4ChangeTreeListViewItem;
                    if (it is P4ShelvedFolderTreeListViewItem)
                    {
                        P4ShelvedFolderTreeListViewItem folder = it as P4ShelvedFolderTreeListViewItem;
                        selected = folder.ParentItem;
                    }
                    refreshChangelistObject(selected);
                }
            }
		}

		// File object
		// revert on file object
		private void revertToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
			{
				P4FileTreeListViewItem selected = pendingTreeListView.SelectedItems[0] as P4FileTreeListViewItem;
				if (selected != null)
				{
					TreeListViewItem parent = selected.ParentItem;
					P4.Changelist changelist = parent.Tag as P4.Changelist;
					if (changelist == null)
					{
						MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
										Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
						refreshChangelistObject((TreeListViewItem)pendingTreeListView.SelectedItems[0]);
						return;
					}
					if (Scm == null)
					{
						// still null
						return;
					}

                    IList<P4ChangeTreeListViewItem> changesToRefresh =
                        new List<P4ChangeTreeListViewItem>();
					IList<string> toRevert = new List<string>();
					//IList<string> files = new List<string>();
					foreach (P4FileTreeListViewItem selectedFile in pendingTreeListView.SelectedItems)
					{
						if (selectedFile.FileData!=null)
						{
							toRevert.Add(selectedFile.FileData.DepotPath.Path);
                            changesToRefresh.Add(selectedFile.ParentItem as P4ChangeTreeListViewItem);
						}

					}

					// revert the unchanged files without a preview
					P4.Options opts = new P4.Options();
					Scm.RevertFiles(true, true, opts, toRevert.ToArray());

					opts["-n"] = null;

					// get a preview list of the files about to be reverted
					List<string> preview = Scm.PreviewRevertFiles(opts, null, true,toRevert.ToArray());

					opts = new P4.Options();
					if (preview != null && preview.Count > 0)
					{
						RevertWarnDlg dlg = new RevertWarnDlg(preview);
						if (Preferences.LocalSettings.GetBool("Revert_warn", true))
						{
							if (dlg.ShowDialog() != DialogResult.Cancel)
							{
								Scm.RevertFiles(false, true, opts, toRevert.ToArray());
							}
						}

						else
							if (toRevert.Count > 0)
							{
								Scm.RevertFiles(false, true, opts, toRevert.ToArray());
							}
					}
                    foreach(P4ChangeTreeListViewItem change in changesToRefresh)
                    {
                        refreshChangelistObject(change);
                    }
                    if (Scm.SccService != null)
					{
						Scm.SccService.UpdateProjectGlyphs(toRevert, true);
					}
				}
			}
		}

		// move to another changelist on file(s) object(s)
		private void moveToAnotherChangelistToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Scm == null)
			{
				P4VsProviderService P4VSService = (P4VsProviderService) GetService(typeof (P4VsProviderService));
				if (P4VSService != null) Scm = P4VSService.ScmProvider;
			}
			if (Scm == null)
			{
				//still null
				return;
			}

			IList<P4.Changelist> changes = Scm.GetAvailibleChangelists(-1);
			Changelist getDesc = Scm.GetChangelist(-1);
			string newChangeDescription = getDesc.Description;
			int changeListId = SelectChangelistDlg.ShowChooseChangelistMoveFilesInChangelist(
						Resources.PendingChangelistsToolWindowControl_SelectChangelistDlgCaption, 
						changes, ref newChangeDescription, Scm);

			if (changeListId <= -1)
			{
				// user hit cancel or new changelist creation failed
				return;
			}
            // keep track of the changelist items that will need to be updated in the tree list view
            Dictionary<int, P4ChangeTreeListViewItem> affectedChanges = new Dictionary<int, P4ChangeTreeListViewItem>();
            P4ChangeTreeListViewItem toItem = null;
            if (changeListId > 0)
            {
                toItem = pendingTreeListView.FindItemWithText(changeListId.ToString()) as P4ChangeTreeListViewItem;
            }
            else
            {
                toItem = pendingTreeListView.FindItemWithText("default") as P4ChangeTreeListViewItem;
            }
            affectedChanges.Add(changeListId, toItem);

            List<string> solutionExp = new List<string>();
			IList<P4.FileSpec> files = new List<P4.FileSpec>();
			if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
			{
				foreach (P4FileTreeListViewItem selected in pendingTreeListView.SelectedItems)
				{
					Type t = selected.GetType();
					if (t.FullName != "Perforce.P4VS.P4FileTreeListViewItem")
					{
						continue;
					}
                    P4.FileSpec file = new P4.FileSpec(new P4.DepotPath(selected.FileData.DepotPath.Path), null);
                    P4.FileSpec otherFile = new FileSpec();
					files.Add(file);
					solutionExp.Add(file.DepotPath.Path);
					P4.FileMetaData fmd = Scm.GetFileMetaData(file.DepotPath.Path);
					if (fmd.Action == FileAction.MoveAdd && fmd.MovedFile != null)
					{
						solutionExp.Add(fmd.MovedFile.Path);
						otherFile.DepotPath = new DepotPath(fmd.MovedFile.Path);
						files.Add(otherFile);
					}
					if (fmd.Action == FileAction.MoveDelete && fmd.MovedFile != null)
					{
						solutionExp.Add(fmd.MovedFile.Path);
						otherFile.DepotPath = new DepotPath(fmd.MovedFile.Path);
						files.Add(otherFile);
					}
                    P4ChangeTreeListViewItem fromItem = selected.ParentItem as P4ChangeTreeListViewItem;
                    if ((fromItem != null) && (fromItem.ChangeData != null) && 
                        (affectedChanges.ContainsKey(fromItem.ChangeData.Id) == false))
                    {
                        affectedChanges.Add(fromItem.ChangeData.Id, fromItem);
                    }
                }
            }

			P4.Options opts = new P4.Options();

			if (files.Count != 0)
			{
				opts["-c"] = changeListId.ToString();
				if (changeListId == 0)
				{
					opts["-c"] = "default";
				}
				Scm.ReopenFiles(files, opts);
				if (Scm.SccService!=null)
				{
				Scm.SccService.UpdateProjectGlyphs(solutionExp, true);
			}
			}
			else
			{
				MessageBox.Show("no files to move");
			}
            foreach (int id in affectedChanges.Keys)
            {
                P4ChangeTreeListViewItem item = affectedChanges[id];
                if (item != null)
                {
                    refreshChangelistObject(item);
                }
                else
                {
                    Scm.BroadcastChangelistUpdate(null, new P4ScmProvider.ChangelistUpdateArgs(id,
                    P4ScmProvider.ChangelistUpdateArgs.UpdateType.ContentUpdate));
                }
            }
		}

		// shelve on file object
        private void shelveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                Dictionary<int, P4ChangeTreeListViewItem> changes = new Dictionary<int, P4ChangeTreeListViewItem>();
                Dictionary<int, List<P4FileTreeListViewItem>> changeFiles = new Dictionary<int, List<P4FileTreeListViewItem>>();

                // sort the selected files by their parent change list
                foreach (P4FileTreeListViewItem file in pendingTreeListView.SelectedItems)
                {
                    P4ChangeTreeListViewItem parent = file.ParentItem as P4ChangeTreeListViewItem;
                    P4.Changelist changelist = parent.Tag as P4.Changelist;
                    if (changelist == null)
                    {
                        MessageBox.Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                                        Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    List<P4FileTreeListViewItem> fileList = null;
                    if (changes.ContainsKey(changelist.Id))
                    {
                        fileList = changeFiles[changelist.Id];
                    }
                    else
                    {
                        // haven't seen the changelist yet
                        fileList = new List<P4FileTreeListViewItem>();
                        changes[changelist.Id] = parent;
                        changeFiles[changelist.Id] = fileList;
                    }
                    fileList.Add(file);
                }
                foreach (int parentId in changes.Keys)
                {
                    P4ChangeTreeListViewItem parent = changes[parentId];
                    if (parent != null)
                    {
                        P4.Changelist changelist = parent.Tag as P4.Changelist;
                        if (changelist == null)
                        {
                            MessageBox.
                                Show(Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
                                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        IList<P4.FileMetaData> filesToShelve = new List<P4.FileMetaData>();

                        foreach (P4FileTreeListViewItem selectedFile in changeFiles[parentId])
                        {
                            filesToShelve.Add(selectedFile.Tag as P4.FileMetaData);
                        }
                        ShelveFileDlg.ShelveFiles(filesToShelve, changelist, Scm, true);
                        refreshChangelistObject(parent);
                    }
                }
            }
        }

		// resolve on file object
		private void resolveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
			{
				IList<string> toResolve = new List<string>();
                // Need to keep a second list. The toResolve list
                // gets emptied when the files are resolved.
                IList<string> toRefresh = new List<string>();

                foreach (P4FileTreeListViewItem selectedFile in pendingTreeListView.SelectedItems)
                {
                    Type t = selectedFile.GetType();
                    if (t.FullName == "Perforce.P4VS.P4FileTreeListViewItem")
                    {
                        P4.FileMetaData fmd = selectedFile.Tag as P4.FileMetaData;
                        if (fmd != null) fmd = Scm.GetFileMetaData(fmd.DepotPath.Path);
                        if (fmd != null) toResolve.Add(fmd.LocalPath.Path);
                        if (fmd != null) toRefresh.Add(fmd.LocalPath.Path);
                    }
                }
                if (toResolve.Count > 0)
				{
                    ResolveFileDlg.ResolveFiles(Scm, toResolve, false, null, true);
                    Scm.SccService.UpdateProjectGlyphs(toRefresh, true);
                }
				P4FileTreeListViewItem selected = pendingTreeListView.SelectedItems[0] as P4FileTreeListViewItem;
				if (selected != null)
				{
                    P4ChangeTreeListViewItem parentChange = selected.ParentItem as P4ChangeTreeListViewItem;
                    refreshChangelistObject(parentChange);
				}
			}
		}

		// diff against have on file object
        private void diffAgainstHaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                IList<string> files = new List<string>();
                foreach (P4FileTreeListViewItem selected in pendingTreeListView.SelectedItems)
                {
                    P4.FileMetaData fmd = selected.Tag as P4.FileMetaData;
                    if (fmd != null) fmd = _scm.GetFileMetaData(fmd.DepotPath.Path);
                    if (fmd != null)
                    {
                        string file = fmd.LocalPath.Path;
                        files.Add(file);
                    }
                }
                _scm.DiffFiles(files.ToArray());
            }
        }

		// diff against on file object
        private void diffAgainstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                IList<string> files = new List<string>();
                foreach (P4FileTreeListViewItem selected in pendingTreeListView.SelectedItems)
                {
                    if (selected != null) files.Add(selected.FileData.DepotPath.Path);
                }
                IList<P4.FileSpec> result = DiffDlg.Show(files, "explorer", null, Scm);
            }
        }

		// diff against source revision on shelved file object
		private void diffAgainstSourceRevToolStripMenuItem_Click(object sender, EventArgs e)
		{
		   List<FileSpec> files=new List<FileSpec>();
			foreach (TreeListViewItem tlvi in pendingTreeListView.SelectedItems)
			{
				Changelist change = tlvi.Tag as Changelist;
				if (change != null)
				{
					diffAgainstHavesToolStripMenuItem_Click(sender,e);
					return;
				}
				FileMetaData file = tlvi.Tag as FileMetaData;
				if (file != null)
				{
					diffAgainstHaveToolStripMenuItem_Click(sender, e);
					return;
				}
				ShelvedFile shelve = tlvi.Tag as ShelvedFile;
				if (shelve != null)
				{
                    try
                    {
                        Changelist parent = tlvi.ParentItem.ParentItem.Tag as Changelist;
                        FileMetaData fmd = Scm.GetFileMetaData(shelve.Path.Path);
                        FileSpec fs1 = new FileSpec(fmd.DepotPath, null, null, new Revision(fmd.HaveRev));
                        FileSpec fs2 = new FileSpec(fmd.DepotPath, null, null,
                            new ShelvedInChangelistIdVersion(parent.Id));
                        files.Add(fs1);
                        files.Add(fs2);
                    }
                    catch (Exception ex)
                    {
                        P4ErrorDlg.Show(ex.Message, false, false);
                    }
                }
				Scm.Diff2Files(files);
			}
		}

		// diff against workspace file on shelved file object
		private void diffAgainstWorkspaceFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<FileSpec> files = new List<FileSpec>();
			foreach (TreeListViewItem tlvi in pendingTreeListView.SelectedItems)
			{
				ShelvedFile shelve = tlvi.Tag as ShelvedFile;
				if (shelve != null)
				{
					Changelist parent = tlvi.ParentItem.ParentItem.Tag as Changelist;
					FileMetaData fmd = Scm.GetFileMetaData(shelve.Path.Path);
					FileSpec fs1 = new FileSpec(fmd.DepotPath, null, null,
						new ShelvedInChangelistIdVersion(parent.Id));
					FileSpec fs2 = new FileSpec(null, null, fmd.LocalPath, null);
					files.Add(fs1);
					files.Add(fs2);
				}
				Scm.Diff2Files(files);
			}
		}

		// change filetype on file object
		private void changeFiletypeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
			{
				IList<string> toChangeFiletype = new List<string>();
				foreach (P4FileTreeListViewItem selectedFile in pendingTreeListView.SelectedItems)
				{
					if (selectedFile is Perforce.P4VS.P4FileTreeListViewItem)
					{
						P4.FileMetaData fmd = selectedFile.Tag as P4.FileMetaData;
						if (fmd != null) fmd = Scm.GetFileMetaData(fmd.DepotPath.Path);
						if (fmd != null) toChangeFiletype.Add(fmd.ClientPath.Path);
					}
				}
				if (toChangeFiletype.Count > 0)
				{
					FileAttributesDlg.ChangeFileType(Scm, toChangeFiletype);
					Scm.SccService.UpdateProjectGlyphs(toChangeFiletype, true);
				}
				P4FileTreeListViewItem selected = pendingTreeListView.SelectedItems[0] as P4FileTreeListViewItem;
				if (selected != null)
				{
					TreeListViewItem parentChange = selected.ParentItem;
					refreshChangelistObject(parentChange);
				}
			}
		}

		// lock on file object
		private void lockToolStripMenuItem_Click(object sender, EventArgs e)
		{
            Dictionary<int, TreeListViewItem> AffectedChanges = new Dictionary<int, TreeListViewItem>();

			if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
			{
				IList<string> toLock = new List<string>();
				IList<string> toUnlock = new List<string>();
				foreach (P4FileTreeListViewItem selected in pendingTreeListView.SelectedItems)
				{
                    if ((selected != null) && (selected is Perforce.P4VS.P4FileTreeListViewItem))
					{
                        P4FileTreeListViewItem selectedFile = selected as Perforce.P4VS.P4FileTreeListViewItem;
						if (lockTSMI.Text == 
							Resources.PendingChangelistsToolWindowControl_MenuItemLock)
						{
							P4.FileMetaData fmd = selectedFile.Tag as P4.FileMetaData;
							if (fmd != null) fmd = Scm.GetFileMetaData(fmd.DepotPath.Path);
							if (fmd != null) toLock.Add(fmd.ClientPath.Path);
						}
						if (lockTSMI.Text ==
							Resources.PendingChangelistsToolWindowControl_MenuItemUnlock)
						{
							P4.FileMetaData fmd = selectedFile.Tag as P4.FileMetaData;
							if (fmd != null) fmd = Scm.GetFileMetaData(fmd.DepotPath.Path);
							if (fmd != null) toUnlock.Add(fmd.ClientPath.Path);
						}
                        TreeListViewItem parentChange = selected.ParentItem;
                        P4.Changelist change = parentChange.Tag as P4.Changelist;
                        if ((change != null) && (AffectedChanges.ContainsKey(change.Id)== false))
                        {
                            AffectedChanges.Add(change.Id, parentChange);
                        }
					}
				}
                try
                {
                    if (toLock.Count > 0)
                    {
                        Scm.LockFiles(toLock.ToArray());
                        Scm.SccService.UpdateProjectGlyphs(toLock, true);
                    }
                    if (toUnlock.Count > 0)
                    {
                        Scm.UnlockFiles(toUnlock.ToArray());
                        Scm.SccService.UpdateProjectGlyphs(toUnlock, true);
                    }
                    foreach (TreeListViewItem parentChange in AffectedChanges.Values)
                    {
                        refreshChangelistObject(parentChange);
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
        }

        // Shelved file object
        // unshelve on shelved file object
        private void unshelveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                Dictionary<int, List<P4ShelvedFileTreeListViewItem>> itemMap = new Dictionary<int, List<P4ShelvedFileTreeListViewItem>>();

                foreach (ListViewItem it in pendingTreeListView.SelectedItems)
                {
                    P4ShelvedFileTreeListViewItem selected = it as P4ShelvedFileTreeListViewItem;
                    if (selected != null)
                    {
                        TreeListViewItem parent = selected.ParentItem.ParentItem;
                        try
                        {
                            P4.Changelist changelist = parent.Tag as P4.Changelist;
                            if (itemMap.ContainsKey(changelist.Id) == false)
                            {
                                //new changelist
                                List<P4ShelvedFileTreeListViewItem> list = new List<P4ShelvedFileTreeListViewItem>();
                                list.Add(selected);
                                itemMap.Add(changelist.Id, list);
                            }
                            else
                            {
                                itemMap[changelist.Id].Add(selected);
                            }
                        }
                        catch (Exception ex)
                        {
                            P4ErrorDlg.Show(ex.Message, false, false);
                        }
                    }
                }
                // now show a dialog for each changelist with items to unshelve
                foreach(int changeId in itemMap.Keys)
                {
                    IList<P4.ShelvedFile> filesToUnshelve = new List<P4.ShelvedFile>();

                    foreach (P4ShelvedFileTreeListViewItem selectedFile in itemMap[changeId])
                    {
                        P4.ShelvedFile sf = new P4.ShelvedFile();
                        sf.Path = new P4.DepotPath(selectedFile.FileData.Path.Path);
                        filesToUnshelve.Add(sf);
                    }

                    UnshelveFileDialog.UnshelveFiles(Scm, changeId, filesToUnshelve);

                    refreshChangelistObject(itemMap[changeId][0].ParentItem.ParentItem);
                }
            }
        }

		// delete shelved file on sheleved file object
		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                Dictionary<int, TreeListViewItem> parentMap = new Dictionary<int, TreeListViewItem>();
                Dictionary<int, IList<string>> shelved = new Dictionary<int, IList<string>>();
                TreeListViewItem selected = pendingTreeListView.SelectedItems[0] as TreeListViewItem;
                string first = null;
                string second = null;

                if (selected != null)
                {
                    P4.ShelveFilesCmdFlags dflags = P4.ShelveFilesCmdFlags.Delete;

                    foreach (P4ObjectTreeListViewItem selectedFile in pendingTreeListView.SelectedItems)
                    {
                        if (selectedFile is P4ShelvedFileTreeListViewItem) //this could be 18 -27
                        {
                            P4ShelvedFileTreeListViewItem sf = selectedFile as P4ShelvedFileTreeListViewItem;
                            if (sf == null)
                            {
                                continue;
                            }

                            TreeListViewItem parent = selectedFile.ParentItem.ParentItem;
                            P4.Changelist change = parent.Tag as P4.Changelist;
                            if ((parentMap.ContainsKey(change.Id) == false) && (parent != null))
                            {
                                parentMap.Add(change.Id, parent);
                            }
                            if (shelved.ContainsKey(change.Id) == false)
                            {
                                shelved.Add(change.Id, new List<string>());
                            }
                            shelved[change.Id].Add(sf.FileData.Path.Path);
                            if (first == null)
                            {
                                first = selectedFile.Text;
                            }
                            else if (second == null)
                            {
                                second = selectedFile.Text;
                            }
                            if (sf.FileData.Action == FileAction.MoveAdd || sf.FileData.Action == FileAction.MoveDelete)
                            {
                                IList<P4.FileSpec> fs = new List<P4.FileSpec>();
                                P4.FileSpec f = new P4.FileSpec();
                                f.DepotPath = new P4.DepotPath(sf.FileData.Path.Path);
                                f.Version = new P4.ShelvedInChangelistIdVersion(change.Id);
                                fs.Add(f);

                                IList<P4.FileMetaData> move = Scm.GetFileMetaData(fs, null);
                                if (move != null)
                                {
                                    foreach (P4.FileMetaData fmd in move)
                                    {
                                        if (fmd.MovedFile != null)
                                        {
                                            shelved[change.Id].Add(fmd.MovedFile.Path);
                                            if (first == null)
                                            {
                                                first = selectedFile.Text;
                                            }
                                            else if (second == null)
                                            {
                                                second = selectedFile.Text;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    string prompt = string.Format(Resources.PendingChangelistsToolWindowControl_DeleteShelvedFileWarning,
                                                  first);
                    if (shelved.Count == 2)
                    {
                        prompt = string.Format(Resources.PendingChangelistsToolWindowControl_DeleteShelved2FilesWarning,
                                               first, second);
                    }
                    if (shelved.Count > 2)
                    {
                        prompt = Resources.PendingChangelistsToolWindowControl_DeleteShelvedFilesWarning;
                    }

                    string caption = Resources.P4VS;

                    if (DialogResult.No != MessageBox.Show(prompt, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        foreach (int changeId in parentMap.Keys)
                        {
                            TreeListViewItem parent = parentMap[changeId];
                            Scm.ShelveFiles(changeId, null, dflags, true, shelved[changeId].ToArray());
                            refreshChangelistObject(parent);
                        }
                    }
                }
            }
		}

		// Job object
		// remove job from changelist on job object
		private void removeJobToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
            {
                foreach (TreeListViewItem selected in pendingTreeListView.SelectedItems)
                {
                    P4.Job job = new P4.Job();
                    job.Id = pendingTreeListView.SelectedItems[0].Text;

                    if (selected != null)
                    {
                        int cl = Convert.ToInt32(selected.ParentItem.Text);

                        P4.Connection con = Scm.Connection.Repository.Connection;
                        P4.Changelist change = new P4.Changelist(cl, true);
                        change.initialize(con);

                        P4.Options opts = new P4.Options(P4.FixJobsCmdFlags.Delete, -1, null);
                        try
                        {
                            IList<P4.Fix> fixes = change.FixJobs(opts, job);
                        }
                        catch (Exception ex)
                        {
                            Scm.ShowException(ex);
                        }
                    }

                    P4.P4CommandResult result = Scm.Connection.Repository.Connection.LastResults;
                    if (result.Success == true)
                    {
                        if (selected != null)
                        {
                            selected.Remove();
                            P4ChangeTreeListViewItem change = selected.ParentItem as P4ChangeTreeListViewItem;
                            change.JobData.Remove(job.Id);
                        }
                    }
                }
            }
			return;
		}

		private void pendingTreeListView_onMaxScroll(object sender, ScrollEventArgs e)
		{
			if ((maxItems > 0) && (pendingTreeListView.Nodes.Count >= maxItems))
			{
				maxItems += (int)Preferences.LocalSettings.GetInt("Number_specs", 100); ;
				refreshPendingList();
			}
		}

		private void filterBtn_EnabledChanged(object sender, EventArgs e)
		{
			if ((filterBtn.Enabled == false)&&((Scm==null)||
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

		private void pendingTreeListView_Click(object sender, EventArgs e)
		{
			checkConnection();
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
				pendingTreeListView.Nodes.Clear();
				pendingTreeListView.BuildTreeList();
				changelistContextMenuStrip.Enabled = false;
				matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;
				return;
			}
			filterBtn.Enabled = true;
			changelistContextMenuStrip.Enabled = true;
		}

		private void changelistContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			if (ContextMenuInitialized == true)
			{
				// already setup
				return;
			}
			if ((currentselection == null) || (currentChangelist == null) || (pendingTreeListView.SelectedItems.Count <= 0))
			{
				// no changelist or item in a changelist has been selected
				clearContext();
				UpdateMenuItem(newPendingChangelistTMSI, true);
				toolStripSeparator6.Visible = true;
				UpdateMenuItem(refreshPendingChangelistListTMSI, true);
				return;
			}
            bool isMultiSelect = pendingTreeListView.SelectedItems.Count>1;

            bool isShelveFolder = false;
            bool isPending = false;
            bool isFile = false;
            bool isOurLock = false;
            bool isOtherLock = false;
            bool fileNeedsResolve = false;
            bool isShelved = false;
            bool isShelvedDelete = false;
            bool isJob = false;
			if (currentChangelist.Pending && (currentselection is P4ChangeTreeListViewItem))
			{
				isPending = true;
			}
			else if (currentselection is P4FileTreeListViewItem)
			{
				isFile = true;
                isOurLock = ((P4FileTreeListViewItem)currentselection).FileData.OurLock;
                isOtherLock = ((P4FileTreeListViewItem)currentselection).FileData.OtherLock;
                fileNeedsResolve = ((P4FileTreeListViewItem)currentselection).FileData.Unresolved;
            }
			else if (currentselection is P4ShelvedFileTreeListViewItem)
			{
				isShelved = true;
                P4ShelvedFileTreeListViewItem shelvedSelection = currentselection as P4ShelvedFileTreeListViewItem;
                if (shelvedSelection != null)
                {
                    P4.ShelvedFile sf = shelvedSelection.Tag as P4.ShelvedFile;
                    if ((sf == null) || (sf.Action == FileAction.Delete))
                    {
                        isShelvedDelete = true;
                    }
                }
			}
            else if (currentselection is P4ShelvedFolderTreeListViewItem)
            {
                isShelveFolder = true;
            }
			else if (currentselection is P4JobTreeListViewItem)
			{
				isJob = true;
			}
			bool ours = false;
			bool hasShelved = false;
			bool needsReolved = false;
			bool underReview = false;
			bool hasFilesOpen = false;
			bool isSameOwner = false;
			bool isDefault = false;
			bool hasJobs = false;

			if (currentChangelist != null)
			{
				ours = currentChangelist.NodeType.Test(nodeTypeFlags.Our);
				hasShelved = currentChangelist.NodeType.Test(nodeTypeFlags.HasShelved);
				needsReolved = currentChangelist.NodeType.Test(nodeTypeFlags.NeedsResolve);
				underReview = currentChangelist.NodeType.Test(nodeTypeFlags.UnderReview);
				hasFilesOpen = (currentChangelist.FileData != null) && (currentChangelist.FileData.Count > 0);
                isSameOwner = currentChangelist.ChangeData.OwnerName == Scm.Connection.User;
				isDefault = currentChangelist.ChangeData.Id <= 0;
				hasJobs = (currentChangelist.JobData != null) && (currentChangelist.JobData.Count > 0);
			}
            bool swarmEnabled = Scm.Connection.Swarm.SwarmEnabled;

			bool ourOpen= ours && isPending && hasFilesOpen;

			UpdateMenuItem(submitTSMI, ourOpen | isFile);

			toolStripSeparator1.Visible = ourOpen | isFile;

			UpdateMenuItem(revertUnchangedFilesTMSI, ourOpen); // revert all unchanged files in a changelist
			UpdateMenuItem(revertFilesTMSI, ourOpen); // revert all files in a changelist
			UpdateMenuItem(revertTMSI, isFile);// revert the selected file(s)
			UpdateMenuItem(resolveFilesTMSI, ourOpen && needsReolved); 
			UpdateMenuItem(moveFilesToAnTMSI, ourOpen);// move all files inn a change
			UpdateMenuItem(moveToAnotherChangelistTMSI, isFile); // move the selected file(s)
            UpdateMenuItem(resolveTMSI, fileNeedsResolve);

			toolStripSeparator2.Visible = ourOpen | isFile;

			UpdateMenuItem(shelveFilesTSMI, ourOpen); // shelve all files in our changelist
			UpdateMenuItem(shelveTSMI, isFile); // shelve the selected file(s)
			UpdateMenuItem(submitShelvedFilesTMSI, (isPending||isShelveFolder) && isSameOwner && hasShelved && !hasFilesOpen); // submit all shelved files in our changelist
			UpdateMenuItem(unshelveFilesTMSI, (isPending || isShelveFolder) && hasShelved); // unshelve all shelved files in a changelist
			UpdateMenuItem(unshelveTMSI, isShelved); // shelve the selected file(s)
			UpdateMenuItem(deleteShelvedFilesTMSI, (isPending || isShelveFolder) &&  ours && hasShelved); // delete all shelved files in our changelist
			UpdateMenuItem(openShelvedFileTSMI, isShelved && !isShelvedDelete);  // open the selected shelved file in a changelist
			UpdateMenuItem(deleteTSMI, ours && isShelved);// delete the selected shelved file(s)

			toolStripSeparator3.Visible = isFile | hasShelved;

			UpdateMenuItem(diffAgainstHaveTMSI, isFile); // diff against have the selected file(s)
			UpdateMenuItem(diffAgainstTSMI, isFile); // diff the selected file(s)
			UpdateMenuItem(diffAgainstHavesTMSI, ourOpen); // diff against have all files in our changelist
			// need this enabled to launch with shortcut key
			// when context menu is closed?
			//diffAgainstSourceRevToolStripMenuItem.Enabled = false;
			diffAgainstSourceRevTMSI.Visible = isShelved; // diff the selected shelved file(s) against source rev
			UpdateMenuItem(diffAgainstWorkspaceFileTMSI, isShelved); // diff the selected shelved file(s) against workspace file

			toolStripSeparator4.Visible = isFile | isShelved | ourOpen ;

			UpdateMenuItem(changeFiletypeTSMI, isFile); // change type on the selected file(s)
			UpdateMenuItem(lockTSMI, isFile && ! isOtherLock); // lock the selected file(s)
            if (isFile)
            {
                if (isOurLock)
                {
                    lockTSMI.Text = Resources.PendingChangelistsToolWindowControl_MenuItemUnlock;
                }
                else
                {
					lockTSMI.Text = Resources.PendingChangelistsToolWindowControl_MenuItemLock;
                }
            }
			toolStripSeparator5.Visible = isFile;

			UpdateMenuItem(newPendingChangelistTMSI, !isShelveFolder);
            UpdateMenuItem(editPendingChangelistTMSI, isPending && ours);
            if (isMultiSelect)
            {
                editPendingChangelistTMSI.Text = Resources.PendingChangelistsToolWindowControl_MenuItemEditSelectedPendingChangelists;
            }
            else
            {
                editPendingChangelistTMSI.Text = string.Format(
                    Resources.PendingChangelistsToolWindowControl_MenuItemEditPendingChangelist,
                    pendingTreeListView.SelectedItems[0].Text);
            }
            UpdateMenuItem(deletePendingChangelistTMSI, isPending && ours && !hasJobs && !isDefault && !hasFilesOpen);
            if (isMultiSelect)
            {
                deletePendingChangelistTMSI.Text = Resources.PendingChangelistsToolWindowControl_MenuItemDeleteSelectedPendingChangelists;
            }
            else
            {
                deletePendingChangelistTMSI.Text = string.Format(
                    Resources.PendingChangelistsToolWindowControl_MenuItemDeletePendingChangelist,
                    pendingTreeListView.SelectedItems[0].Text);
            }
            UpdateMenuItem(changeOwnershipTMSI, isPending && !isDefault);

			toolStripSeparator6.Visible = !isShelveFolder;

			UpdateMenuItem(refreshPendingChangelistListTMSI, !isShelveFolder);
			UpdateMenuItem(refreshPendingChangelistTMSI, (isPending || isShelveFolder));
            if(isShelveFolder)
            {
                refreshPendingChangelistTMSI.Text = string.Format(
                   Resources.PendingChangelistsToolWindowControl_MenuItemRefreshPendingChangelist,
                   currentChangelist.Text);
            }
            else if (isMultiSelect)
            {
                refreshPendingChangelistTMSI.Text = Resources.PendingChangelistsToolWindowControl_MenuItemRefreshSelectedPendingChangelists;
            }
            else
            {
                refreshPendingChangelistTMSI.Text = string.Format(
                    Resources.PendingChangelistsToolWindowControl_MenuItemRefreshPendingChangelist,
                    pendingTreeListView.SelectedItems[0].Text);
            }
            UpdateMenuItem(removeJobTMSI, isJob);

            bool showRequestReview = swarmEnabled && isPending && ours && (hasShelved || hasFilesOpen) && !underReview && !isMultiSelect;
			UpdateMenuItem(RequestReviewTSMI, showRequestReview);
			RequestReviewTSMI.Image = showRequestReview?Images.swarm_contextmenu16_16:null;
            bool showUpdateReview = swarmEnabled && !isMultiSelect && isPending && ours && (((hasShelved || hasFilesOpen) && !underReview) || hasFilesOpen);
			UpdateMenuItem(UpdateReviewTSMI, showUpdateReview);
			if (showUpdateReview)
			{
				if (underReview)
				{
					UpdateReviewTSMI.Text = string.Format(Resources.PendingChangelistsToolWindowControl_MenuItemUpdateReview, currentChangelist.ReviewData.id);
				}
				else
				{
					UpdateReviewTSMI.Text = Resources.PendingChangelistsToolWindowControl_MenuItemUpdateReview2;
				}
			}
			UpdateReviewTSMI.Image = (showUpdateReview && !showRequestReview) ? Images.swarm_contextmenu16_16 : null;
            bool showOpenReview = isPending && underReview && !isMultiSelect;
			UpdateMenuItem(openReviewInSwarmTSMI, showOpenReview);
			openReviewInSwarmTSMI.Image = (showOpenReview && !(showRequestReview | showUpdateReview)) ? Images.swarm_contextmenu16_16 : null;

			if (showOpenReview)
			{
				openReviewInSwarmTSMI.Text = string.Format(Resources.PendingChangelistsToolWindowControl_MenuItemOpenSwarmReview, currentChangelist.ReviewData.id);
			}
			SwarmReviewTSS.Visible = showRequestReview || showUpdateReview || showOpenReview;

			ContextMenuInitialized = true;
		}

		private void pendingTreeListView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
		{
			if (e.NewDisplayIndex == 0 | e.OldDisplayIndex == 0)
			{
				e.Cancel = true;
			}
		}

        private void pendingTreeListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            P4ObjectTreeListView list = sender as P4ObjectTreeListView;
            if (list != null)
            {
                P4FileTreeListViewItem file = list.FocusedItem as P4FileTreeListViewItem;
                if (file != null)
                {
                    P4.FileMetaData fmd = Scm.GetFileMetaData(file.FileData.DepotPath.Path);
                    if (fmd != null && fmd.LocalPath != null)
                    {
                        EnvDTE.DTE dte2;
                        dte2 = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
                        if (System.IO.File.Exists(fmd.LocalPath.Path))
                        {
                            try
                            {
                                if (dte2 != null) dte2.ItemOperations.OpenFile(fmd.LocalPath.Path, null);
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
                    P4ShelvedFileTreeListViewItem ptLvi = list.FocusedItem as P4ShelvedFileTreeListViewItem;
                    if (ptLvi != null)
                    {
                        ShelvedFile shelved = ptLvi.FileData;
                        if (shelved != null)
                        {
                            P4ChangeTreeListViewItem parent = ptLvi.ParentItem.ParentItem as P4ChangeTreeListViewItem;
                            if (parent != null)
                            {
                                P4.Changelist change = parent.ChangeData;

                                P4.FileSpec fs = FileSpec.DepotSpec(shelved.Path.Path);

                                IList<P4.FileMetaData> fmd = _scm.GetFileMetaData(null, fs);
                                if ((fmd != null) && (fmd.Count > 0) && (fmd[0] != null))
                                {
                                    if (fmd[0].FileSize > Preferences.LocalSettings.GetInt("Size_files", 500))
                                    {
                                        return;
                                    }
                                }

                                if (change != null)
                                {
                                    long maxPreviewSize = 1024 * Preferences.LocalSettings.GetInt("Size_files", 500);
                                    long shelvedSize = _scm.GetShelvedFileSize(change.Id, shelved.Path.Path);
                                    if (shelvedSize > maxPreviewSize)
                                    {
                                        string size = P4ObjectTreeListViewItem.PrettyPrintFileSize(shelvedSize);
                                        string msg = string.Format(Resources.FileExceedsMaxPreviewSizeWarning, size);
                                        if (DialogResult.No == MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                                        {
                                            return;
                                        }
                                    }
                                    fs.Version = new P4.ShelvedInChangelistIdVersion(change.Id);

                                    using (TempFile shelvedFile = new TempFile(fs))
                                    {
                                        try
                                        {
                                            if (_scm.GetFileVersion(shelvedFile, fs) == null)
                                            {
                                                shelvedFile.Dispose();
                                                return;
                                            }

                                            if (Preferences.LocalSettings.GetBool("OpenShelvedFileInEditor", true))
                                            {
                                                EnvDTE.DTE dte = P4VsProvider.GetDTE();
                                                if (System.IO.File.Exists(shelvedFile))
                                                {
                                                    dte.ItemOperations.OpenFile(shelvedFile, null);
                                                }
                                            }
                                            else
                                            {
                                                ShowFileContentsDlg dlg = new ShowFileContentsDlg();
                                                string shelvedChange = "";
                                                dlg.TempFile = shelvedFile;
                                                if (change != null)
                                                {
                                                    shelvedChange = "@=" + change.Id.ToString();
                                                }
                                                dlg.Title = shelved.Path.Path + shelvedChange;

                                                // Show modeless
                                                dlg.Show();
                                            }
                                        }


                                        catch (Exception)
                                        {
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

		private void pendingTreeListView_ColumnClick(object sender, ColumnClickEventArgs e)
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
				pendingTreeListView.Columns[lvwColumnSorter.SortColumn].Text =
					pendingTreeListView.Columns[lvwColumnSorter.SortColumn].Text;

				// Set the column number that is to be sorted; default to ascending.

				lvwColumnSorter.SortColumn = e.Column;
				lvwColumnSorter.Order = SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			this.pendingTreeListView.Sort();
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
			//if (selectionChangedByLoad == false)
			//{

			//    if (userCB.Text == Resources.FilterComboBox_BrowseBtn)
			//    {
			//        // browse for user
			//        UsersBrowserDlg dlg = new UsersBrowserDlg(_scm, null);

			//        dlg.TopMost = true; ;

			//        if (DialogResult.Cancel != dlg.ShowDialog())
			//        {
			//        selectionChangedByLoad = true;
			//            if (dlg.SelectedUser.Id != null)
			//            {
			//                userCB.Text = dlg.SelectedUser.Id;
			//                selectionChangedByLoad = false;
			//                filterBtn.PerformClick();
			//                return;
			//            }

			//            userCB.Text = "";
			//            selectionChangedByLoad = false;
			//            return;
			//        }
			//    }
			//    filterBtn.PerformClick();
			//    selectionChangedByLoad = false;
			//}
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
			//if (selectionChangedByLoad == false)
			//{

			//    if (pathCB.Text == Resources.FilterComboBox_BrowseBtn)
			//    {
			//        // browse for path
			//        DepotPathDlg dlg = new DepotPathDlg(_scm.SccService, Resources.GetRevisionDlg_AddFilesFoldersPrompt,
			//                                            false);

			//        if (DialogResult.OK == dlg.ShowDialog())
			//        {
			//            selectionChangedByLoad = true;
			//            if (dlg.SelectedFile != null)
			//            {
			//                pathCB.Text = dlg.SelectedFile;
			//                selectionChangedByLoad = false;
			//                filterBtn.PerformClick();
			//                return;
			//            }

			//            pathCB.Text = "";
			//            selectionChangedByLoad = false;
			//            return;
			//        }
			//    }
			//    filterBtn.PerformClick();
			//    selectionChangedByLoad = false;
			//}
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

		private void submitShelvedFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (pendingTreeListView.SelectedItems != null && pendingTreeListView.SelectedItems.Count > 0)
			{
				P4.Changelist changelist = null;
				TreeListViewItem child = pendingTreeListView.SelectedItems[0] as TreeListViewItem;
				Dictionary<int, IList<P4.FileMetaData>> selectedFileMap = new Dictionary<int, IList<P4.FileMetaData>>();
				Dictionary<int, P4.Changelist> selectedChangelistMap = new Dictionary<int, P4.Changelist>();

				foreach (P4ObjectTreeListViewItem item in pendingTreeListView.SelectedItems)
				{
                    string prompt = string.Empty;
					if (item is Perforce.P4VS.P4FileTreeListViewItem)
					{
						// It's a file
						return;
					}
					P4ChangeTreeListViewItem changeItem = item as P4ChangeTreeListViewItem;
                    if (changeItem==null)
                    {
                        changeItem = item.ParentItem as P4ChangeTreeListViewItem;
                    }
                    if (changeItem!= null && changeItem.NodeType.TestAll(nodeTypeFlags.Pending | nodeTypeFlags.Our))
					{
						// It's a change list that can be submitted
						changelist = (P4.Changelist) changeItem.Tag;
						if (changelist == null)
						{
							MessageBox.Show(
								Resources.PendingChangelistsToolWindowControl_SelectedChangelistDeletedWarning,
								Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
							refreshChangelistObject(item);
							return;
						}
                        if (changelist.Files.Count > 0)
                        {
                            // In some cases the changelist is not getting refreshed
                            // after a shelve/revert. This leaves file(s) in the
                            // item.ChangeData which will not allow the submit.
                            refreshChangelistObject(changeItem);
                            changeItem.Tag = changeItem.ChangeData;
                            changelist = (P4.Changelist)changeItem.Tag;
                        }
                        if (changelist.Files.Count > 0)
						{
							prompt =
								string.Format(
									Resources.
										PendingChangelistsToolWindowControl_SubmitShelvedChangelistContainsFilesWarning,
									changelist.Id);
							DialogResult res = SubmitShelvedWarningDlg.Show(prompt);
							if (res == DialogResult.Cancel)
							{
								return;
							}

							if (res == DialogResult.Yes)
							{
								// move
								moveFilesToAnTMSI.PerformClick();
							}

							if (res == DialogResult.OK)
							{
								// revert
								revertFilesTMSI.PerformClick();
							}
						}
						// submit the shelved change
						Scm.SubmitShelvedChangelist(changelist.Id);
					}
				}

				foreach (int Id in selectedFileMap.Keys)
				{
					SubmitDlg.SubmitPendingChanglist(selectedChangelistMap[Id], selectedFileMap[Id], Scm);
				}

				if (child != null && child.ParentItem == null)
				{
					if (pendingTreeListView.SelectedItems.Count > 0)
					{
						if (pendingTreeListView.SelectedItems[0] is Perforce.P4VS.P4FileTreeListViewItem)
						{
							P4.FileMetaData file = pendingTreeListView.SelectedItems[0].Tag as P4.FileMetaData;
							if (file != null) child = pendingTreeListView.FindItemWithText(file.Change.ToString()) as TreeListViewItem;
						}

						refreshChangelistObject(child);
					}
				}
				else
				{
					if (child != null) refreshChangelistObject(child.ParentItem);
				}
			}
		}

		private void pathCB_DropDown(object sender, EventArgs e)
		{
			//pathCB.Items.Remove(Resources.FilterComboBox_BrowseBtn);

			//if ((Scm == null) || !(Scm.Connected))
			//{
			//    pathCB.Items.Clear();
			//    return;
			//}
			//pathCB.Items.Insert(0, Resources.FilterComboBox_BrowseBtn);

		}

		private void userCB_DropDown(object sender, EventArgs e)
		{
			//userCB.Items.Remove(Resources.FilterComboBox_BrowseBtn);

			//if ((Scm == null) || !(Scm.Connected))
			//{
			//    userCB.Items.Clear();
			//    return;
			//}
			//userCB.Items.Insert(0, Resources.FilterComboBox_BrowseBtn);

		}

		private void matchesLbl_SizeChanged(object sender, EventArgs e)
		{
			gridLayoutPanel2.LayoutGrid();
		}

		private void RequestReviewTSMI_Click(object sender, EventArgs e)
		{
			P4.Changelist changelist = pendingTreeListView.SelectedItems[0].Tag as P4.Changelist;
			if (changelist != null)
			{
				CreateSwarmReviewDlg.RequestReview(Scm, changelist.Id);
			}
			refreshChangelistObject(pendingTreeListView.SelectedItems[0] as TreeListViewItem);
        }

		private void UpdateReviewTSMI_Click(object sender, EventArgs e)
		{
			P4ChangeTreeListViewItem li = pendingTreeListView.SelectedItems[0] as P4ChangeTreeListViewItem;
			if (li != null)
			{
				if (li.ReviewData != null)
				{
					P4.Changelist changelist = li.Tag as P4.Changelist;
					CreateSwarmReviewDlg.RefreshReview(Scm, changelist.Id, li.ReviewData.id, li.ReviewData.description);
				}
				else
				{
					P4.Changelist changelist = li.Tag as P4.Changelist;
					CreateSwarmReviewDlg.UpdateReview(Scm, changelist.Id);
				}
			}
            refreshChangelistObject(li);
		}

		private void openReviewInSwarmToolStripMenuItem_Click(object sender, EventArgs e)
		{
			P4ChangeTreeListViewItem li = pendingTreeListView.SelectedItems[0] as P4ChangeTreeListViewItem;
			if ((li != null) && (li.ReviewData != null))
			{
                SwarmServer sw = Scm.Connection.Swarm.GetSwarmServer();
				sw.ShowReviewInBrowser(li.ReviewData.id);
			}
		}
        private void matchesLbl_TextChanged(object sender, EventArgs e)
        {
            if (matchesLbl.Text.Contains("matches") ||
                matchesLbl.Text.Contains("no connection"))
            {
                if (Scm != null && Scm.SccService != null)
                {
                    Scm.SccService.PendingChangeCount = 0;
                }
            }
        }
    }
}
