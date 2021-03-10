using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Perforce.P4;
using Perforce.SwarmApi;
using NLog;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class CreateSwarmReviewDlg : AutoSizeForm
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
		private class FileListViewSorter : System.Collections.IComparer
		{
			#region IComparer<ListViewItem> Members

			public int Compare(object xo, object yo)
			{
				ListViewItem x = xo as ListViewItem;
				ListViewItem y = yo as ListViewItem;

				if (x == null)
				{
					throw new ArgumentNullException("x");
				}

				if (y == null)
				{
					throw new ArgumentNullException("y");
				}

				return string.Compare(x.SubItems[0].Text, y.SubItems[0].Text);
			}

			#endregion
		}

		private class ReviewerSorter : System.Collections.IComparer
		{
			#region IComparer<ListViewItem> Members

			public int Compare(object xo, object yo)
			{
				ListViewItem x = xo as ListViewItem;
				ListViewItem y = yo as ListViewItem;

				if (x == null)
				{
					throw new ArgumentNullException("x");
				}

				if (y == null)
				{
					throw new ArgumentNullException("y");
				}

				return string.Compare(x.SubItems[1].Text, y.SubItems[1].Text);
			}

			#endregion
		}

		public static void LaunchSwarm(int reviewId)
		{
			//string url = string.Format()
			//System.Diagnostics.Process p = new System.Diagnostics.Process();

			//System.Diagnostics.ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo()
		
		}
		private System.Windows.Forms.ImageList ListImages;

		public static bool RequestReview(P4ScmProvider Scm, int changeId)//, bool pending)
		{
			CreateSwarmReviewDlg dlg = new CreateSwarmReviewDlg(Scm, "CreateSwarmReviewDlg");

			dlg.Option1Text = Resources.CreateSwarmReviewDlg_RevertAfterShelving;
			dlg.Option1Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_RevertAfterShelving", false);
			dlg.Option2Text = Resources.CreateSwarmReviewDlg_ClearShelfBefore;
			dlg.Option2Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_ClearShelfBefore", false);
			dlg.Option3Text = Resources.CreateSwarmReviewDlg_DontShelveUnchangedFiles;
			dlg.Option3Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_DontShelveUnchangedFiles", false);
			dlg.Option4Text = Resources.CreateSwarmReviewDlg_OpenReviewInSwarm;
			dlg.Option4Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_OpenReviewInSwarm", false);
			dlg.OptionLblPnl.Visible = true;
			try
			{
				dlg.ChangelistId = changeId;
				dlg.Option1CB.Visible = dlg._pending;
				dlg.Option2CB.Visible = dlg._shelved;
				dlg.Option2CB.Enabled = dlg._shelved;
				dlg.Option3CB.Visible = dlg._pending;
			}
			catch { return false; }
            dlg.gridLayoutSubpanel1.Visible = false;
			dlg.UpdateReviewLbl.Visible = false;
			dlg.UpdateReviewTB.Visible = false;
			dlg.BrowseReviewBtn.Visible = false;
			dlg.ViewReviewDescBtn.Visible = false;
			dlg.OKBtn.Enabled = true;

			if (dlg._pending)
			{
				dlg.PromptLbl.Text = Resources.CreateSwarmReviewDlg_CreateFromPendingPrompt;
			}
			else
			{
				dlg.PromptLbl.Text = Resources.CreateSwarmReviewDlg_CreateFromSubmittedPrompt;
			}

			dlg.Text = Resources.CreateSwarmReviewDlg_CreateReview;
            dlg.OKBtn.Text = Resources.CreateSwarmReviewDlg_RequestReviewBtn;
            if (changeId <= 0)
			{
				// must enter a change description to enable OK
				dlg.OKBtn.Enabled = false;
			}

			DialogResult dlgResult = dlg.ShowDialog();

			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_RevertAfterShelving", dlg.Option1Checked);
			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_ClearShelfBefore", dlg.Option2Checked);
			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_DontShelveUnchangedFiles", dlg.Option3Checked);
			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_OpenReviewInSwarm", dlg.Option4Checked);

			if (dlgResult == DialogResult.Cancel)
			{
				return false;
			}
			int change = changeId;

			string description = dlg.Description;
			if (string.IsNullOrEmpty(description) ||
				(string.Compare(description.Trim(), Resources.DefaultChangeListDescription, true) == 0))
			{
				description = Resources.CreateSwarmReviewDlg_DefaultChangelistDescription;
			}
			P4.Changelist newChange = null;
			try
			{
				if (change <= 0)
				{
					// default, so move files to an new change list
					newChange = Scm.MoveFilesToChangeList(-1, description, P4.FileSpec.DepotSpecList(dlg.GetFiles()));
					change = newChange.Id;
				}
				if (dlg._pending == true)
				{
					if ((dlg.Option2Checked) && (change > 0) && (dlg._shelved))
					{
						// Clear the shelve;
						if (Scm.ShelveFiles(change, null, P4.ShelveFilesCmdFlags.Delete, true, null) == false)
						{
							return false;
						}
					}
					if ((dlg.FileList != null) && (dlg.FileList.Count > 0))
					{
						P4.ShelveFilesCmdFlags flags = P4.ShelveFilesCmdFlags.Force;
						if (dlg.Option3Checked)
						{
							flags |= ShelveFilesCmdFlags.LeaveUnchanged;
						}
						// need to shelve the files for the review
						if (Scm.ShelveFiles(change, description, flags, dlg.FileList) == false)
						{
							return false;
						}
					}
					if (dlg.Option1Checked)
					{
						// Delete the opened files in the changelist
						Scm.RevertFiles(false, true, null, dlg.GetFiles());
					}
				}
			}
			catch (P4Exception ex)
			{
				P4ErrorDlg.Show(ex);
				return false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
            SwarmServer sw = Scm.Connection.Swarm.GetSwarmServer();

			if (sw != null)
			{
				SwarmApi.Options options = new SwarmApi.Options();
				options["description"] = new JSONParser.JSONStringField(description);
				if (dlg.reviewersGridView.RowCount>0) 
				{
                    if (Scm.Connection.Swarm.SwarmAPI7)
                    {
                        // users: required or optional
                        IList<string> reviewers = dlg.GetReviewers(ReviewerType.optional);
                        options["reviewers[]"] = new JSONParser.JSONArray(reviewers);
                        reviewers = dlg.GetReviewers(ReviewerType.reqired);
                        options["requiredReviewers[]"] = new JSONParser.JSONArray(reviewers);

                        // keep an index of all group reviewers
                        int totalReviewerGroups = 0;

                        IList<string> groups = new List<string>();



                        // groups: all members optional
                        reviewers = dlg.GetReviewers(ReviewerType.optionalGroup);
                        for (int i = 0; i < reviewers.Count; i++)
                        {
                            options["reviewerGroups[" + totalReviewerGroups + "][name]"] =
                                new JSONParser.JSONStringField(reviewers[i]);
                            totalReviewerGroups++;
                        }
                        // groups: one members required
                        reviewers = dlg.GetReviewers(ReviewerType.oneRequiredInGroup);
                        for (int i = 0; i < reviewers.Count; i++)
                        {
                            options["reviewerGroups[" + totalReviewerGroups + "][name]"] =
                                new JSONParser.JSONStringField(reviewers[i]);
                            options["reviewerGroups[" + totalReviewerGroups + "][required]"] =
                                new JSONParser.JSONStringField("true");
                            options["reviewerGroups[" + totalReviewerGroups + "][quorum]"] =
                                new JSONParser.JSONStringField("1");
                            totalReviewerGroups++;
                        }
                        // groups: all members required
                        reviewers = dlg.GetReviewers(ReviewerType.requiredGroup);
                        for (int i = 0; i < reviewers.Count; i++)
                        {
                            options["reviewerGroups[" + totalReviewerGroups + "][name]"] =
                                new JSONParser.JSONStringField(reviewers[i]);
                            options["reviewerGroups[" + totalReviewerGroups + "][required]"] =
                                new JSONParser.JSONStringField("true");
                            totalReviewerGroups++;
                        }

                    }
                    else if (Scm.Connection.Swarm.SwarmAPI1_1)
					{
						IList<string> reviewers = dlg.GetReviewers(ReviewerType.optional);
						options["reviewers"] = new JSONParser.JSONArray(reviewers);
						reviewers = dlg.GetReviewers(ReviewerType.reqired);
						options["requiredReviewers"] = new JSONParser.JSONArray(reviewers);
					}
					else
					{
						IList<string> reviewers = dlg.GetReviewers();
						options["reviewers"] = new JSONParser.JSONArray(reviewers);
					}
				}
                SwarmServer.Review reveiw = null;
                try
                {
                    if (Scm.Connection.Swarm.SwarmAPI7)
                    {
                        reveiw = sw.CreateReview7(change, options);
                    }
                    else if (Scm.Connection.Swarm.SwarmAPI1_1)
					{
						reveiw = sw.CreateReview1_1(change, options);
					}
					else
					{
						reveiw = sw.CreateReview(change, options);
					}
					if (reveiw != null)
					{
						if (dlg.Option4Checked)
						{
							sw.ShowReviewInBrowser(reveiw.id);
						}
						else
						{
							string msg = string.Format(Resources.CreateSwarmReviewDlg_ReviewCreated,
								reveiw.id, reveiw.description);
							MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
						if (newChange != null)
						{
							P4ScmProvider.ChangelistUpdateArgs args =
								new P4ScmProvider.ChangelistUpdateArgs(newChange.Id, P4ScmProvider.ChangelistUpdateArgs.UpdateType.Add);
							Scm.BroadcastChangelistUpdate(dlg, args);
						}
					}
				}
				catch (Exception ex)
				{
                    if (reveiw == null && ex.InnerException != null)
                    {
                        P4ErrorDlg.Show(Resources.CreateSwarmReviewDlg_ReviewCouldNotBeCreated +
                             "\r\n" + ex.InnerException.Message, false, false);
                    }
                    else
                    {
                        P4ErrorDlg.Show(ex.Message, false, false);
                    }
                }
			}
			return true;
		}

		public static bool UpdateReview(P4ScmProvider Scm, int changeId)//, bool pending)
		{
			CreateSwarmReviewDlg dlg = new CreateSwarmReviewDlg(Scm, "UpdateSwarmReviewDlg");

			dlg.Option1Text = Resources.CreateSwarmReviewDlg_RevertAfterShelving;
			dlg.Option1Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_RevertAfterShelving", false);
			dlg.Option2Text = Resources.CreateSwarmReviewDlg_ClearShelfBefore;
			dlg.Option2Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_ClearShelfBefore", false);
			dlg.Option3Text = Resources.CreateSwarmReviewDlg_DontShelveUnchangedFiles;
			dlg.Option3Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_DontShelveUnchangedFiles", false);
			dlg.Option4Text = Resources.CreateSwarmReviewDlg_OpenReviewInSwarm;
			dlg.Option4Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_OpenReviewInSwarm", false);
			dlg.OptionLblPnl.Visible = true;

			dlg.ReviewersLbl.Visible = false;
			dlg.reviewersGridView.Visible = false;
			dlg.UserLbl.Visible = false;
			dlg.UserTB.Visible = false;
			dlg.AddUserBtn.Visible = false;
			dlg.BrowseUserBtn.Visible = false;
			dlg.DeleteBtn.Visible = false;
			dlg.AddREviewerGLSP.Visible = false;

            dlg.groupLbl.Visible = false;
            dlg.GroupTB.Visible = false;
            dlg.AddGroupBtn.Visible = false;
            dlg.BrowseGroupBtn.Visible = false;
            dlg.AddREviewerGLSP2.Visible = false;
            dlg.gridPanel2.Visible = false;

            dlg.DescriptionTB.Enabled = false;

            dlg.Text = Resources.CreateSwarmReviewDlg_UpdateReview;
            dlg.OKBtn.Text = Resources.CreateSwarmReviewDlg_UpdateReviewBtn;
            dlg.updatingReview = true;

			try
			{
				dlg.ChangelistId = changeId;
				dlg.Option1CB.Visible = dlg._pending;
				dlg.Option2CB.Visible = dlg._shelved;
				dlg.Option2CB.Enabled = dlg._shelved;
				dlg.Option3CB.Visible = dlg._pending;
			}
			catch { return false; }

			if (dlg._pending)
			{
				dlg.PromptLbl.Text = Resources.CreateSwarmReviewDlg_UpdateFromPendingPrompt;
			}
			else
			{
				dlg.PromptLbl.Text = Resources.CreateSwarmReviewDlg_UpdateFromSubmittedPrompt1;
			}
			//if (changeId <= 0)
			//{
			//    // must enter a change description to enable OK
			//    dlg.OKBtn.Enabled = false;
			//}
			DialogResult dlgResult = dlg.ShowDialog();

			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_RevertAfterShelving", dlg.Option1Checked);
			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_ClearShelfBefore", dlg.Option2Checked);
			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_DontShelveUnchangedFiles", dlg.Option3Checked);
			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_OpenReviewInSwarm", dlg.Option4Checked);

			if (dlgResult == DialogResult.Cancel)
			{
				return false;
			}
			int change = changeId;

			string description = dlg.Description;
			if (string.IsNullOrEmpty(description) ||
				(string.Compare(description.Trim(), Resources.DefaultChangeListDescription, true) == 0))
			{
				description = Resources.CreateSwarmReviewDlg_DefaultChangelistDescription;
			}
			P4.Changelist newChange = null;
			try
			{
				if (change <= 0)
				{
					// default, so move files to an new change list
					newChange = Scm.MoveFilesToChangeList(-1, description, P4.FileSpec.DepotSpecList(dlg.GetFiles()));
					change = newChange.Id;
				}
				if ((dlg._pending == true) && (dlg._shelved == false))
				{
					if ((dlg.Option2Checked) && (change > 0) && (dlg._shelved))
					{
						// Clear the shelve;
						if (Scm.ShelveFiles(change, null, P4.ShelveFilesCmdFlags.Delete, true, null) == false)
						{
							return false;
						}
					}
					P4.ShelveFilesCmdFlags flags = P4.ShelveFilesCmdFlags.Force;
					if (dlg.Option3Checked)
					{
						flags |= ShelveFilesCmdFlags.LeaveUnchanged;
					}
					// need to shelve the files for the review
					if (Scm.ShelveFiles(change, description, flags, dlg.FileList) == false)
					{
						return false;
					}
					if (dlg.Option1Checked)
					{
						// Delete the opened files in the changelist
						Scm.RevertFiles(false, true, null, dlg.GetFiles());
					}
				}
			}
			catch (P4Exception ex)
			{
				P4ErrorDlg.Show(ex);
				return false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
            SwarmServer sw = Scm.Connection.Swarm.GetSwarmServer();

			if (sw != null)
			{
				SwarmApi.Options options = new SwarmApi.Options();
				options["description"] = new JSONParser.JSONStringField(description);
                if (dlg.reviewersGridView.RowCount>0)
                {
                    if (Scm.Connection.Swarm.SwarmAPI1_1)
                    {
                        IList<string> reviewers = dlg.GetReviewers(ReviewerType.optional);
                        options["reviewers"] = new JSONParser.JSONArray(reviewers);
                        reviewers = dlg.GetReviewers(ReviewerType.reqired);
                        options["requiredReviewers"] = new JSONParser.JSONArray(reviewers);
                    }
                    else
                    {
                        IList<string> reviewers = dlg.GetReviewers();
                        options["reviewers"] = new JSONParser.JSONArray(reviewers);
                    }
                }
                try
				{
					SwarmServer.Review review = null;
                    if (Scm.Connection.Swarm.SwarmAPI1_1)
					{
						review = sw.AddChangeToReview1_1(_selectedReviewID, change);
					}
					else
					{
						review = sw.AddChangeToReview(_selectedReviewID, change);
					}
					if (review != null)
					{
						if (dlg.Option4Checked)
						{
							sw.ShowReviewInBrowser(review.id);
						}
						else
						{
							string msg = string.Format(Resources.CreateSwarmReviewDlg_ReviewUpdated,
								review.id, review.description);
							MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
						if (newChange != null)
						{
							P4ScmProvider.ChangelistUpdateArgs args =
								new P4ScmProvider.ChangelistUpdateArgs(newChange.Id, P4ScmProvider.ChangelistUpdateArgs.UpdateType.Add);
							Scm.BroadcastChangelistUpdate(dlg, args);
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			return true;
		}

		private string ReviewDescription = null;

		// Shelve open files on a changelist that is attached to a review to refresh the review
		public static bool RefreshReview(P4ScmProvider Scm, int changeId, int reviewId, string reviewDescription)//, bool pending)
		{
			CreateSwarmReviewDlg dlg = new CreateSwarmReviewDlg(Scm, "UpdateSwarmReviewDlg");

			dlg.ReviewDescription = reviewDescription;

			dlg.Option1Text = Resources.CreateSwarmReviewDlg_RevertAfterShelving;
			dlg.Option1Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_RevertAfterShelving", false);
			dlg.Option2Text = Resources.CreateSwarmReviewDlg_ClearShelfBefore;
			dlg.Option2Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_ClearShelfBefore", false);
			dlg.Option3Text = Resources.CreateSwarmReviewDlg_DontShelveUnchangedFiles;
			dlg.Option3Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_DontShelveUnchangedFiles", false);
			dlg.Option4Text = Resources.CreateSwarmReviewDlg_OpenReviewInSwarm;
			dlg.Option4Checked = Preferences.LocalSettings.GetBool("CreateSwarmReviewDlg_OpenReviewInSwarm", false);
			dlg.OptionLblPnl.Visible = true;

			dlg.ReviewersLbl.Visible = false;
			dlg.reviewersGridView.Visible = false;
			dlg.UserLbl.Visible = false;
			dlg.UserTB.Visible = false;
			dlg.AddUserBtn.Visible = false;
			dlg.BrowseUserBtn.Visible = false;
			dlg.DeleteBtn.Visible = false;
			dlg.AddREviewerGLSP.Visible = false;

			dlg.UpdateReviewLbl.Visible = false;
			dlg.UpdateReviewTB.Visible = false;
			dlg.BrowseReviewBtn.Visible = false;
			dlg.ViewReviewDescBtn.Visible = false;

			dlg.DescriptionTB.Enabled = false;
			dlg.OKBtn.Enabled = true;

			dlg.updatingReview = true;

			dlg.Text = string.Format(Resources.CreateSwarmReviewDlg_RefreshReview, reviewId);
			try
			{
				dlg.ChangelistId = changeId;
				dlg.Option1CB.Visible = dlg._pending;
				dlg.Option2CB.Visible = dlg._shelved;
				dlg.Option2CB.Enabled = dlg._shelved;
				dlg.Option3CB.Visible = dlg._pending;
			}
			catch { return false; }
			if (changeId <= 0)
			{
				// must enter a change description to enable OK
				dlg.OKBtn.Enabled = false;
			}
			dlg.PromptLbl.Text = String.Format(Resources.CreateSwarmReviewDlg_RefreshFromPendingPrompt, changeId);

			DialogResult dlgResult = dlg.ShowDialog();

			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_RevertAfterShelving", dlg.Option1Checked);
			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_ClearShelfBefore", dlg.Option2Checked);
			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_DontShelveUnchangedFiles", dlg.Option3Checked);
			Preferences.LocalSettings.Set("CreateSwarmReviewDlg_OpenReviewInSwarm", dlg.Option4Checked);

			if (dlgResult == DialogResult.Cancel)
			{
				return false;
			}
			try
			{
				int change = changeId;

				string description = dlg.Description;
				if (string.IsNullOrEmpty(description) ||
					(string.Compare(description.Trim(), Resources.DefaultChangeListDescription, true) == 0))
				{
					description = Resources.CreateSwarmReviewDlg_DefaultChangelistDescription;
				}
				if (change <= 0)
				{
					// default, so move files to an new change list
					P4.Changelist newChange = Scm.MoveFilesToChangeList(-1, dlg.Description, P4.FileSpec.DepotSpecList(dlg.GetFiles()));
					change = newChange.Id;
				}
				if ((dlg._pending == true) && (dlg._shelved == false))
				{
					if ((dlg.Option2Checked) && (change > 0) && (dlg._shelved))
					{
						// Clear the shelve;
						if (Scm.ShelveFiles(change, null, P4.ShelveFilesCmdFlags.Delete, true, null) == false)
						{
							return false;
						}
					}
					P4.ShelveFilesCmdFlags flags = P4.ShelveFilesCmdFlags.Force;
					if (dlg.Option3Checked)
					{
						flags |= ShelveFilesCmdFlags.LeaveUnchanged;
					}
					// need to shelve the files for the review
					if (Scm.ShelveFiles(change, description, flags, dlg.FileList) == false)
					{
						return false;
					}
					if (dlg.Option1Checked)
					{
						// Delete the opened files in the changelist
						Scm.RevertFiles(false, true, null, dlg.GetFiles());
					}
				}
				if (dlg.Option4Checked)
				{
                    SwarmServer sw = Scm.Connection.Swarm.GetSwarmServer();

					if (sw != null)
					{
						sw.ShowReviewInBrowser(reviewId);
					} 
					return true;
				}
			}
			catch (P4Exception ex)
			{
				P4ErrorDlg.Show(ex);
				return false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			return true;
		}

		private bool Swarm1_1 = false;

		public CreateSwarmReviewDlg(P4ScmProvider scm, string _preferenceKey)
		{
			PreferenceKey = _preferenceKey;

			_scm = scm;

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
			if (this.components == null)
			{
				this.components = new System.ComponentModel.Container();
			}
			this.ListImages = new System.Windows.Forms.ImageList(this.components);
			// 
			// ListImages
			// 
			ListImages.TransparentColor = System.Drawing.Color.Transparent;
            ListImages.Images.Add("noCheckBox.png", Images.noCheckBox);
            ListImages.Images.Add("CheckBox.png", Images.CheckBox);
            ListImages.Images.Add("users_icon.png", Images.users_icon);
            ListImages.Images.Add("groups_icon.png", Images.groups_icon);

            //ReviewersLV.SmallImageList = ListImages;

            ChangelistFilesLV.ListViewItemSorter = (System.Collections.IComparer) new FileListViewSorter();

            Swarm1_1 = scm.Connection.Swarm.SwarmAPI1_1;
			if (!Swarm1_1)
			{
				// remove the required reviewers column
				reviewersGridView.Columns.RemoveAt(0);
			}
		}

		public string Description
		{
			get { return DescriptionTB.Text; }
			set { DescriptionTB.Text = value; }
		}

		private P4ScmProvider _scm = null;

		private int _changelistId = 0;
		bool _pending = false;
		bool _shelved = false;

		// map from depot path to list view item
		private IDictionary<string, ListViewItem> listItemMap = null;

		// map from depot path to meta data
		private IDictionary<string, P4.FileMetaData> fileMetaDataMap = null;

		private P4.Changelist changelist = null;

		private bool updatingReview = false;

		public int ChangelistId
		{
			get { return _changelistId; }
			set
			{
				IDictionary<string, ShelvedFile> ShelvedFilesTable = null;

				_changelistId = value;

				ReplaceShelvedLbl.Visible = false;

				if (_changelistId > 0)
				{
					changelist = _scm.GetChangelist(value);
					if (changelist.Shelved)
					{
						P4.Changelist change2 = _scm.GetChangelist(value, new P4.DescribeCmdOptions(P4.DescribeChangelistCmdFlags.Shelved | P4.DescribeChangelistCmdFlags.Omit, -1, -1));
						changelist.ShelvedFiles = change2.ShelvedFiles;

						ShelvedFilesTable = new Dictionary<string, ShelvedFile>();
						foreach (ShelvedFile sf in changelist.ShelvedFiles)
						{
							ShelvedFilesTable.Add(sf.Path.Path, sf);
						}
					}
					else
					{
						AlreadyShelvedFilesLbl.Visible = false;
						ShelvedFilesLV.Visible = false;
					}
					_pending = changelist.Pending;
					if (updatingReview)
					{
						if (string.IsNullOrEmpty(ReviewDescription))
						{
							Description = string.Empty;
						}
						else
						{
							Description = ReviewDescription;
						}
					}
					else
					{
						Description = changelist.Description;
					}
				}
				else
				{
					// no shelved files in default cl
					AlreadyShelvedFilesLbl.Visible = false;
					ShelvedFilesLV.Visible = false;

					if (updatingReview)
					{
						Description = string.Empty;
					}
					else
					{
						Description = Resources.DefaultChangeListDescription;
					}
					changelist = _scm.Connection.Repository.NewChangelist();
                    // It's the 'default' change list to be turned into a new numbered changelist or
                    //  or a new change list to be edited.
                    P4.Options options = new P4.GetFileMetaDataCmdOptions(
						P4.GetFileMetadataCmdFlags.Opened,
						null, null, -1, null, "default", null);
                    string wsPath = string.Format("//{0}/...", _scm.Connection.Repository.Connection.Client.Name);
					// get files opened in default changelist
                    changelist.Files = _scm.Connection.Repository.GetFileMetaData(options, new P4.FileSpec(new P4.ClientPath(wsPath), null));

					_pending = true;
				}

				IList<P4.FileSpec> fileList = null;
				IList<P4.FileMetaData> mdl = null;

				listItemMap = new Dictionary<string, ListViewItem>();
				fileMetaDataMap = new Dictionary<string, P4.FileMetaData>();

				if (_changelistId > 0)
				{
					//it's an existing numbered list
					if ((changelist != null) && (changelist.Files.Count > 0))
					{
						mdl = changelist.Files;
					}
				}
				else
				{
					// It's the 'default' change list to be turned into a new numbered changelist or
					//  or a new change list to be edited.
					P4.Options options = new P4.GetFileMetaDataCmdOptions(
						P4.GetFileMetadataCmdFlags.Opened,
						null, null, -1, null, "default", null);
                    string wsPath = string.Format("//{0}/...", _scm.Connection.Repository.Connection.Client.Name);
					// get files opened in default changelist
                    mdl = _scm.Connection.Repository.GetFileMetaData(options, new P4.FileSpec(new P4.ClientPath(wsPath), null));
				}

				fileList = new List<P4.FileSpec>();
				if ((mdl != null) && (mdl.Count > 0))
				{
					foreach (P4.FileMetaData file in mdl)
					{
						ListViewItem item = new ListViewItem(file.DepotPath.GetFileName());		// subitem 0

						P4.FileMetaData fileMd = _scm.GetFileMetaData(file.DepotPath.Path);
						item.Tag = fileMd;
						item.SubItems.Add(file.DepotPath.GetDirectoryName());	// subitem 1
						item.SubItems.Add(file.Action.ToString());				// subitem 2, place holder for pending action 
						item.SubItems.Add(System.IO.Path.GetExtension(file.DepotPath.GetFileName()));// subitem 3
						if (_changelistId > 0)
						{
							P4.ShelvedFile shelvedfile = null;
							if ((ShelvedFilesTable != null) && (ShelvedFilesTable.ContainsKey(file.DepotPath.Path)))
							{
								shelvedfile = ShelvedFilesTable[file.DepotPath.Path];
							}
							if (shelvedfile != null)
							{
								ReplaceShelvedLbl.Visible = true;
								item.SubItems.Add(shelvedfile.Action.ToString() + "*");
							}
							else
							{
								item.SubItems.Add(string.Empty);
							}
						}
						listItemMap.Add(file.DepotPath.Path, item);
						ChangelistFilesLV.Items.Add(item);
						fileMetaDataMap.Add(file.DepotPath.Path, fileMd);
					}
					if (_pending == false)
					{
						ShelvedFileActionClm.Width = 0;
					}
				}
				else
				{
					PromptLbl.Visible = false;
					ChangelistFilesLV.Visible = false;
				}
				// now list the files that are shelved in the changelist
                if ((ShelvedFilesTable != null) && (ShelvedFilesTable.Count > 0))
                {
                    foreach (P4.ShelvedFile file in ShelvedFilesTable.Values)
                    {
                        ListViewItem item = new ListViewItem(file.Path.GetFileName());		// subitem 0
                        item.Tag = null;
                        item.SubItems.Add(file.Path.GetDirectoryName());		// subitem 1
                        item.SubItems.Add(file.Action.ToString());
                        if (fileMetaDataMap.ContainsKey(file.Path.Path))
                        {
                            item.SubItems.Add("*");
                        }
                        else
                        {
                            item.SubItems.Add(string.Empty);
                        }
                        ShelvedFilesLV.Items.Add(item);
                    }
                }
                else if ((changelist != null) && (changelist.ShelvedFiles != null) && (changelist.ShelvedFiles.Count > 0))
                {
                    _shelved = true;
                    foreach (P4.ShelvedFile file in changelist.ShelvedFiles)
                    {
                        ListViewItem item = new ListViewItem(file.Path.GetFileName());		// subitem 0
                        P4.FileMetaData fileMd = _scm.GetFileMetaData(file.Path.Path);
                        item.Tag = fileMd;
                        item.SubItems.Add(file.Path.GetDirectoryName());	// subitem 1
                        item.SubItems.Add(file.Action.ToString());				// subitem 2, place holder for pending action 
                        item.SubItems.Add(System.IO.Path.GetExtension(file.Path.GetFileName()));// subitem 3
                        listItemMap.Add(file.Path.Path, item);
                        ChangelistFilesLV.Items.Add(item);
                        fileMetaDataMap.Add(file.Path.Path, fileMd);
                    }
                    if (_pending == false)
                    {
                        ShelvedFileActionClm.Width = 0;
                    }
                }
#if DEBUG_DB
				int i = 0;
				foreach (ListViewItem it in ChangelistFilesLV.Items)
				{
					P4.FileMetaData m = it.Tag as P4.FileMetaData;
					string fp = m.DepotPath.Path;
					logger.Trace("Item {0}: {1}", i++, fp);
				}
#endif
			}
		}

		public IList<P4.FileMetaData> FileList
		{
			get
			{
				List<P4.FileMetaData> value = new List<P4.FileMetaData>();

				foreach (ListViewItem item in ChangelistFilesLV.Items)
				{
					if (item.Tag != null)
					{
						value.Add((P4.FileMetaData)item.Tag);
					}
				}
				if (value.Count <= 0)
				{
					return null;
				}
				return value;
			}
		}

		public string[] GetFiles()
		{
			List<string> value = new List<string>();

			foreach (ListViewItem item in ChangelistFilesLV.Items)
			{
				if (item.Tag != null)
				{
					value.Add(((P4.FileMetaData)item.Tag).DepotPath.Path);
				}
			}
			if (value.Count > 0)
			{
				return value.ToArray();
			}
			return null;
		}


        

        private void BrowseGroupBtn_Click(object sender, EventArgs e)
        {
            GroupsBrowserDlg dlg = new GroupsBrowserDlg(_scm, null);

            if ((DialogResult.Cancel != dlg.ShowDialog()) && (dlg.SelectedGroup.Id != null))
            {
                GroupTB.TextChanged -= new EventHandler(GroupTB_TextChanged);
                GroupTB.Text = dlg.SelectedGroup.Id;
                GroupTB.Tag = dlg.SelectedGroup;

                // no dupes
                bool notDupe = !ReviewerMap.ContainsKey(GroupTB.Text.Trim());
                AddGroupBtn.Enabled = notDupe;

                GroupTB.TextChanged += new EventHandler(GroupTB_TextChanged);

                //if (dlg.ClosedByDoubleClick)
                //{
                if (notDupe)
                {
                    // last value was accepted
                    AddGroupBtn_Click(sender, e);
                }
                //}
            }
        }
        private void BrowseReviewerBtn_Click(object sender, EventArgs e)
		{
			UsersBrowserDlg dlg = new UsersBrowserDlg(_scm, null);

			if ((DialogResult.Cancel != dlg.ShowDialog()) && (dlg.SelectedUser.Id != null))
			{
				UserTB.TextChanged -= new EventHandler(ReviewerTB_TextChanged);
				UserTB.Text = dlg.SelectedUser.Id;
				UserTB.Tag = dlg.SelectedUser;

				// no dupes
				bool notDupe = !ReviewerMap.ContainsKey(UserTB.Text.Trim());
				AddUserBtn.Enabled = notDupe;

				UserTB.TextChanged += new EventHandler(ReviewerTB_TextChanged);

				//if (dlg.ClosedByDoubleClick)
				//{
				if (notDupe)
				{
					// last value was accepted
					AddReviewerBtn_Click(sender, e);
				}
				//}
			}
		}

        public enum ReviewerType { all, reqired, optional, optionalGroup,
            requiredGroup, oneRequiredInGroup };

        public IList<string> GetReviewers()
        {
            return GetReviewers(ReviewerType.all);
        }

        public IList<string> GetReviewers(ReviewerType type)
		{
            if (ReviewerMap.Count <= 0)
			{
				return null;
			}
			List<string> value = new List<string>();
			foreach (DataGridViewRow dr in reviewersGridView.Rows)
			{
                switch (type)
                {
                    case ReviewerType.all:
                        value.Add(dr.Cells[1].Value.ToString());
                        break;
                    case ReviewerType.reqired:
                        if (dr.Cells[3].Value.ToString().ToLower().Contains("required") &&
                           !(string.IsNullOrEmpty(dr.Cells[2].Value.ToString())))
                        {
                            value.Add(dr.Cells[1].Value.ToString());
                        }
                        break;
                    case ReviewerType.optional:
                        if (dr.Cells[3].Value.ToString().ToLower().Contains("optional") &&
                           !(string.IsNullOrEmpty(dr.Cells[2].Value.ToString())))
                        {
                            value.Add(dr.Cells[1].Value.ToString());
                        }
                        break;
                    case ReviewerType.requiredGroup:
                        if (dr.Cells[3].Value.ToString().ToLower().Contains("are required"))
                        {
                            value.Add(dr.Cells[1].Value.ToString());
                        }
                        break;
                    case ReviewerType.oneRequiredInGroup:
                        if (dr.Cells[3].Value.ToString().ToLower().Contains("is required"))
                        {
                            value.Add(dr.Cells[1].Value.ToString());
                        }
                        break;
                    case ReviewerType.optionalGroup:
                        if (dr.Cells[3].Value.ToString().ToLower().Contains("are optional")||
                            dr.Cells[3].Value.ToString().ToLower().Contains("members optional"))
                        {
                            value.Add(dr.Cells[1].Value.ToString());
                        }
                        break;
                        //"All members optional",
                        //"All group members\' reviews are optional",
                        //"All members required",
                        //"All group members\' reviews are required",
                        //"One review required",
                        //"Only one group member\'s review is required",
                        //"Optional",
                        //"Required"});

                }
			}
			return value;
		}

        Dictionary<string, object> ReviewerMap = new Dictionary<string, object>();

		private void AddReviewerBtn_Click(object sender, EventArgs e)
		{
			P4.User user = null;
			if (UserTB.Tag != null)
			{
				user = (P4.User)UserTB.Tag;
			}
			else
			{
				user = _scm.GetUser(UserTB.Text.Trim());
				if (user == null)
				{
                    MessageBox.Show(Resources.CreateSwarmReviewDlg_CantFindUser, Resources.P4VS,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    UserTB.Clear();
                    UserTB.Tag = null;
                    return;
				}
			}
            ReviewerMap[UserTB.Text.Trim()] = user;

            reviewersGridView.Rows.Add(Images.users_icon, user.Id, user.FullName,
                "Optional");

            reviewersGridView.Rows[reviewersGridView.Rows.Count - 1].Tag = user;
            reviewersGridView.Rows[reviewersGridView.Rows.Count - 1].Selected = true;

            UserTB.Clear();
            UserTB.Tag = null;

            DataGridViewComboBoxCell cell =
                    (DataGridViewComboBoxCell)reviewersGridView.
                    Rows[reviewersGridView.SelectedRows[0].Index].Cells[3];

            cell.Items.Add("Optional");
            cell.Items.Add("Required");

            cell.Value = cell.Items[0];
        }

        private void AddGroupBtn_Click(object sender, EventArgs e)
        {
            P4.Group group = null;
            if (GroupTB.Tag != null)
            {
                group = (P4.Group)GroupTB.Tag;
            }
            else
            {
                List<string> name = new List<string>();
                name.Add(GroupTB.Text.Trim());
                GroupsCmdOptions opts = new GroupsCmdOptions(GroupsCmdFlags.None, 1);
                IList<Group> groups = _scm.GetGroups(name,opts);
                if (groups == null || groups.Count < 1)
                {
                    MessageBox.Show(Resources.CreateSwarmReviewDlg_CantFindGroup, Resources.P4VS,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    GroupTB.Clear();
                    GroupTB.Tag = null;
                    return;
                }
                group = groups[0];
            }
            ReviewerMap[GroupTB.Text.Trim()] = group;

            reviewersGridView.Rows.Add(Images.groups_icon, group.Id, string.Empty,
                "All members optional");

            reviewersGridView.Rows[reviewersGridView.Rows.Count - 1].Tag = group;
            reviewersGridView.Rows[reviewersGridView.Rows.Count - 1].Selected=true;
            GroupTB.Clear();
            GroupTB.Tag = null;

            DataGridViewComboBoxCell cell =
                    (DataGridViewComboBoxCell)reviewersGridView.
                    Rows[reviewersGridView.SelectedRows[0].Index].Cells[3];

            cell.Items.Add("All members optional");
            cell.Items.Add("All members required");
            cell.Items.Add("One review required");

            cell.Value = cell.Items[0];
            
        }
        private void DeleteReviewerBtn_Click(object sender, EventArgs e)
		{
            if ((reviewersGridView.SelectedRows == null) || (reviewersGridView.SelectedRows.Count <= 0))
            {
                return;
            }
            int idx = reviewersGridView.SelectedRows[0].Index;
            if (idx < 0)
            {
                return;
            }
            P4.User user = reviewersGridView.Rows[idx].Tag as P4.User;
            if (user != null)
            {
                ReviewerMap.Remove(user.Id);
            }
            P4.Group group = reviewersGridView.Rows[idx].Tag as P4.Group;
            if (group != null)
            {
                ReviewerMap.Remove(group.Id);
            }
            reviewersGridView.Rows.RemoveAt(idx);
        }

		private void ReviewersLV_SelectedIndexChanged(object sender, EventArgs e)
		{
            DeleteBtn.Enabled = reviewersGridView.SelectedRows != null || reviewersGridView.SelectedRows.Count > 0;
        }

        private void ReviewerTB_TextChanged(object sender, EventArgs e)
		{
			UserTB.Tag = null; // hand edited
			if (string.IsNullOrWhiteSpace(UserTB.Text))
			{
				AddUserBtn.Enabled = false;
				return;
			}
			if (ReviewerMap.ContainsKey(UserTB.Text.Trim()))
			{
				// no dupes
				AddUserBtn.Enabled = false;
				return;
			}
			AddUserBtn.Enabled = true;
		}

        private void GroupTB_TextChanged(object sender, EventArgs e)
        {
            GroupTB.Tag = null; // hand edited
            if (string.IsNullOrWhiteSpace(GroupTB.Text))
            {
                AddGroupBtn.Enabled = false;
                return;
            }
            if (ReviewerMap.ContainsKey(GroupTB.Text.Trim()))
            {
                // no dupes
                AddGroupBtn.Enabled = false;
                return;
            }
            AddGroupBtn.Enabled = true;
        }

        //private void ReviewerTB_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Return)
        //    {
        //        if (AddReviewerBtn.Enabled)
        //        {
        //            // last value was accepted
        //            AddReviewerBtn_Click(sender, e);
        //        }
        //        e.Handled = true;
        //    }
        //}

        private void CreateSwarmReviewDlg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (UserTB.Focused)
                {
                    if (AddUserBtn.Enabled)
                    {
                        // last value was accepted
                        AddReviewerBtn_Click(sender, e);
                    }
                    e.Handled = true;
                }
                if (GroupTB.Focused)
                {
                    if (AddGroupBtn.Enabled)
                    {
                        // last value was accepted
                        AddGroupBtn_Click(sender, e);
                    }
                    e.Handled = true;
                }
            }
        }

        public string Option1Text
		{
			set
			{
				Option1CB.Visible = (string.IsNullOrEmpty(value) != true);
				Option1CB.Text = value;
			}
			get
			{
				return Option1CB.Text;
			}
		}

		public bool Option1Checked
		{
			set { Option1CB.Checked = value; }
			get { return Option1CB.Checked; }
		}

		public string Option2Text
		{
			set
			{
				Option2CB.Visible = (string.IsNullOrEmpty(value) != true);
				Option2CB.Text = value;
			}

		}

		public bool Option2Checked
		{
			set { Option2CB.Checked = value; }
			get { return Option2CB.Checked; }
		}

		public bool Option3Checked
		{
			set { Option3CB.Checked = value; }
			get { return Option3CB.Checked; }
		}

		public string Option3Text
		{
			set
			{
				Option3CB.Visible = (string.IsNullOrEmpty(value) != true);
				Option3CB.Text = value;
			}

		}

		public bool Option4Checked
		{
			set { Option4CB.Checked = value; }
			get { return Option4CB.Checked; }
		}

		public string Option4Text
		{
			set
			{
				Option4CB.Visible = (string.IsNullOrEmpty(value) != true);
				Option4CB.Text = value;
			}

		}
		private void EnableOKBtn()
		{
			if (string.IsNullOrEmpty(DescriptionTB.Text) ||
				(string.Compare(DescriptionTB.Text.Trim(),Resources.DefaultChangeListDescription, true) == 0))
			{
				OKBtn.Enabled = false;
				return;
			}
			OKBtn.Enabled = true;
		}

		private void DescriptinCB_TextChanged(object sender, EventArgs e)
		{
			EnableOKBtn();
		}

		private static int _selectedReviewID = -1;

		private void BrowseReviewBtn_Click(object sender, EventArgs e)
		{
			SwarmReviewsBrowserDlg dlg = new SwarmReviewsBrowserDlg(_scm);
			if (DialogResult.OK == dlg.ShowDialog())
			{
				SwarmServer.Review SelectedReview = dlg.SelectedReview;

				_selectedReviewID = SelectedReview.id;
				UpdateReviewTB.Text = _selectedReviewID.ToString();

				foreach (SwarmServer.Review.Participant r in SelectedReview.participants.Values)
				{
					//ListViewItem it = new ListViewItem(new string[] { r., user.Id, user.FullName });
					//it.Checked = true;
					//it.Tag = user;
					//ReviewersLV.Items.Add(it);
				}
				DescriptionTB.Text = SelectedReview.description;
			}
		}

		private void ViewReviewDescBtn_Click(object sender, EventArgs e)
		{
			string idStr = UpdateReviewTB.Text;
			if (string.IsNullOrEmpty(idStr))
			{
				return;
			}
			int id = -1;
			int.TryParse(idStr, out id);
			if (id < 0)
			{
				return;
			}
			try
			{
                SwarmServer sw = _scm.Connection.Swarm.GetSwarmServer();
				SwarmServer.Review review = sw.GetReview(id);

				if (review != null)
				{
					Description = review.description;
				}
			}
			catch (SwarmServer.SwarmException ex)
			{
				OKBtn.Enabled = false;
				P4ErrorDlg.Show(ex, false, false);
			}
		}

		private void UpdateReviewTB_TextChanged(object sender, EventArgs e)
		{
			if (updatingReview && string.IsNullOrEmpty(UpdateReviewTB.Text))
			{
				OKBtn.Enabled = false;
				ViewReviewDescBtn.Enabled = false;
				return;
			}
			else if (updatingReview && !string.IsNullOrEmpty(UpdateReviewTB.Text))
			{
				int id = -1;
				if (int.TryParse(UpdateReviewTB.Text, out id))
				{
					_selectedReviewID = id;
				}
			}
			OKBtn.Enabled = true;
			ViewReviewDescBtn.Enabled = updatingReview;
		}

		private void DescriptionTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.A)
			{
				DescriptionTB.SelectionStart = 0;
				DescriptionTB.SelectionLength = DescriptionTB.Text.Length;
			}
		}

        private void reviewersGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;                  // no image in the header
            if (e.ColumnIndex == 0)
            {
                e.PaintBackground(e.ClipBounds, true);  // allow highlighting
                e.PaintContent(e.ClipBounds);

                
                int y = e.CellBounds.Bottom - 18;         // font height
                string user = reviewersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value.ToString();
                e.Graphics.DrawString(user, reviewersGridView.Font, Brushes.Black, e.CellBounds.Left + 24, y);
                e.Handled = true;                        // done with the image column 
            }
        }

        private void reviewersGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (reviewersGridView.CurrentCell.ColumnIndex == 3)
            {
                e.Handled = true;
                DataGridViewCell cell = reviewersGridView.Rows[0].Cells[0];
                reviewersGridView.CurrentCell = cell;
            }
        }

        private void reviewersGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel=true;
        }

        private void reviewersGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)reviewersGridView.CurrentCell;

            // It is a group
            if (cell.Items.Count > 2)
            {
                
                string selected = cell.FormattedValue.ToString();
                cell.Items.Clear();
                cell.Items.Add("All group members' reviews are optional");
                cell.Items.Add("All group members' reviews are required");
                cell.Items.Add("Only one group member's review is required");
                
                if (selected.Contains("optional"))
                {
                    cell.Value = cell.Items[0];
                }
                else if (selected.Contains("members required"))
                {
                    cell.Value = cell.Items[1];
                }
                else if (selected.Contains("review required"))
                {
                    cell.Value = cell.Items[2];
                }
            }
            else
            {
                string selected = cell.FormattedValue.ToString();
                cell.Items.Clear();
                cell.Items.Add("Optional");
                cell.Items.Add("Required");
            }
            //"All members optional",
            //"All group members\' reviews are optional",
            //"All members required",
            //"All group members\' reviews are required",
            //"One review required",
            //"Only one group member\'s review is required",
            //"Optional",
            //"Required"
        }

        private void reviewersGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox)
            {
                ComboBox CB = (ComboBox)e.Control;
                CB.DrawMode = DrawMode.OwnerDrawFixed;
                CB.DrawItem += combobox_DrawItem;
            }
        }

       
        private void combobox_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            string s = "";
            Brush br = SystemBrushes.WindowText;
            Brush brBack;
            Rectangle rDraw;
            bool bSelected = e.State == DrawItemState.Selected;
            bool bValue = e.State == DrawItemState.ComboBoxEdit;

            rDraw = e.Bounds;
            rDraw.Inflate(-1, -1);

            if (bSelected & !bValue)
            {
                brBack = Brushes.LightBlue;
                g.FillRectangle(Brushes.LightBlue, rDraw);
                g.DrawRectangle(Pens.Blue, rDraw);
            }
            else
            {
                brBack = Brushes.White;
                g.FillRectangle(brBack, e.Bounds);
            }

            try
            {
                s = ((ComboBox)sender).Items[e.Index].ToString();
            }
            catch
            {
                s = "";
            }

            int x, y;

            x = e.Bounds.Left + 20;
            y = e.Bounds.Top + 1;

            string selected = reviewersGridView.CurrentCell.Value.ToString();
            Font f = reviewersGridView.Font;
            if (s==selected)
            {
                e.Graphics.DrawImage(Images.CheckMark, new Point(e.Bounds.Left, e.Bounds.Top));
                f = new Font(new FontFamily("Microsoft Sans Serif"), 8.25f, FontStyle.Bold);
            }
            g.DrawString(s, f, Brushes.Black, x, y);
        }



        private void reviewersGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (reviewersGridView.CurrentCell.ColumnIndex==3)
            {
                reviewersGridView.CurrentCell.ReadOnly = false;
            }
        }

        private void reviewersGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (reviewersGridView.CurrentCell!=null &&
                reviewersGridView.CurrentCell.IsInEditMode &&
                e.ColumnIndex==3 &&
                reviewersGridView.CurrentRow.Cells[2].Value.ToString()=="")
            {
                try
                {
                    DataGridViewComboBoxCell cell =
    (DataGridViewComboBoxCell)reviewersGridView.CurrentCell;

                    cell.Items.Clear();
                    cell.Items.Add("All members optional");
                    cell.Items.Add("All members required");
                    cell.Items.Add("One review required");

                    if (reviewersGridView.CurrentCell.EditedFormattedValue.ToString().Contains("are required"))
                    {
                        cell.Items.RemoveAt(1);
                        cell.Items.Insert(0, "All members required");
                    }
                    if (reviewersGridView.CurrentCell.EditedFormattedValue.ToString().Contains("is required"))
                    {
                        cell.Items.RemoveAt(2);
                        cell.Items.Insert(0, "One review required");
                    }
                }
                catch{}
                
                reviewersGridView.EndEdit();
                reviewersGridView.CurrentCell.ReadOnly = true;
            }
        }
    }
}
