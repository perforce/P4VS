using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EnvDTE;

using Perforce.P4;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class PendingChangelistDlg : AutoSizeForm
	{
		private ImageList ShelvedFileIcons;
        private ImageList JobIcon;
        private ImageList CheckboxIcons;


		private PendingChangelistDlg() 
		{
			PreferenceKey = "PendingChangelistDlg";

            InitializeComponent();
			//this.slidingPanelContainer1.DlgParent = this;
		    this.Icon = Images.pending;

			ShelvedFileIcons = new System.Windows.Forms.ImageList(this.components);
			// 
			// ShelvedFileIcons
			// 
			this.ShelvedFileIcons.TransparentColor = System.Drawing.Color.White;
			this.ShelvedFileIcons.Images.Add("shelve_icon_base", Images.shelve_icon_base);
			this.ShelvedFileIcons.Images.Add("shelve_icon_edit", Images.shelve_icon_edit);
			this.ShelvedFileIcons.Images.Add("shelve_icon_add", Images.shelve_icon_add);
			this.ShelvedFileIcons.Images.Add("shelve_icon_delete", Images.shelve_icon_delete);
			this.ShelvedFileIcons.Images.Add("shelve_icon_branch", Images.shelve_icon_branch);
			this.ShelvedFileIcons.Images.Add("shelve_icon_integrate", Images.shelve_icon_integrate);
			this.ShelvedFileIcons.Images.Add("shelve_icon_moveadd", Images.shelve_icon_moveadd);
			this.ShelvedFileIcons.Images.Add("shelve_icon_movedelete", Images.shelve_icon_movedelete);
			this.ShelvedFileIcons.Images.Add("shelve_icon_archive", Images.shelve_icon_archive);
			this.ShelvedFileIcons.Images.Add("shelve_icon_purge", Images.shelve_icon_purge);
            
            JobIcon = new System.Windows.Forms.ImageList(this.components);
            // 
            // JobIcon
            // 
            this.JobIcon.Images.Add("jobs_icon", Images.jobs_icon);
            //JobsListLV.SmallImageList = JobIcon;

            CheckboxIcons = new System.Windows.Forms.ImageList(this.components);
            // 
            // CheckboxIcons
            // 
            CheckboxIcons.TransparentColor = System.Drawing.Color.Transparent;
            CheckboxIcons.Images.Add("noCheckBox.png", Images.noCheckBox);
            CheckboxIcons.Images.Add("CheckBox.png", Images.CheckBox);
            JobsListLV.SmallImageList = CheckboxIcons;

			//ButtonTitles
			JobsPanel.ButtonText = Resources.PendingChangelistDlg_JobsPanel_ButtonText;
			DescriptionPanel.ButtonText = Resources.PendingChangelistDlg_DescriptionPanel_ButtonText;
			FilesPanel.ButtonText = Resources.PendingChangelistDlg_FilesPanel_ButtonText;
			ShelvedFilesPanel.ButtonText = Resources.PendingChangelistDlg_ShelvedFilesPanel_ButtonText;

			this.SelectedClm.Text = Resources.EmptyString;

			ShelvedFilesLV.SmallImageList = this.ShelvedFileIcons;
		}

		public PendingChangelistDlg(P4ScmProvider scm)
		{
			PreferenceKey = "PendingChangelistDlg";

			_scm = scm;

            InitializeComponent();
			//this.slidingPanelContainer1.DlgParent = this;
		    this.Icon = Images.pending;

			ShelvedFileIcons = new System.Windows.Forms.ImageList(this.components);
			// 
			// ShelvedFileIcons
			// 
			this.ShelvedFileIcons.TransparentColor = System.Drawing.Color.White;
			this.ShelvedFileIcons.Images.Add("shelve_icon_base", Images.shelve_icon_base);
			this.ShelvedFileIcons.Images.Add("shelve_icon_edit", Images.shelve_icon_edit);
			this.ShelvedFileIcons.Images.Add("shelve_icon_add", Images.shelve_icon_add);
			this.ShelvedFileIcons.Images.Add("shelve_icon_delete", Images.shelve_icon_delete);
			this.ShelvedFileIcons.Images.Add("shelve_icon_branch", Images.shelve_icon_branch);
			this.ShelvedFileIcons.Images.Add("shelve_icon_integrate", Images.shelve_icon_integrate);
			this.ShelvedFileIcons.Images.Add("shelve_icon_moveadd", Images.shelve_icon_moveadd);
			this.ShelvedFileIcons.Images.Add("shelve_icon_movedelete", Images.shelve_icon_movedelete);
			this.ShelvedFileIcons.Images.Add("shelve_icon_archive", Images.shelve_icon_archive);
			this.ShelvedFileIcons.Images.Add("shelve_icon_purge", Images.shelve_icon_purge);

            JobIcon = new System.Windows.Forms.ImageList(this.components);
            // 
            // JobIcon
            // 
            this.JobIcon.Images.Add("jobs_icon", Images.jobs_icon);
            //JobsListLV.SmallImageList = JobIcon;

            CheckboxIcons = new System.Windows.Forms.ImageList(this.components);
            // 
            // CheckboxIcons
            // 
            CheckboxIcons.TransparentColor = System.Drawing.Color.Transparent;
            CheckboxIcons.Images.Add("noCheckBox.png", Images.noCheckBox);
            CheckboxIcons.Images.Add("CheckBox.png", Images.CheckBox);
            JobsListLV.SmallImageList = CheckboxIcons;

			//ButtonTitles
			JobsPanel.ButtonText = Resources.PendingChangelistDlg_JobsPanel_ButtonText;
			DescriptionPanel.ButtonText = Resources.PendingChangelistDlg_DescriptionPanel_ButtonText;
			FilesPanel.ButtonText = Resources.PendingChangelistDlg_FilesPanel_ButtonText;
			ShelvedFilesPanel.ButtonText = Resources.PendingChangelistDlg_ShelvedFilesPanel_ButtonText;

			this.SelectedClm.Text = Resources.EmptyString;

			ShelvedFilesLV.SmallImageList = this.ShelvedFileIcons;

			//DetailsPanel.Collapsed = Preferences.LocalSettings.GetBool("DetailsPanel.Collapsed",false);
			DescriptionPanel.Collapsed = Preferences.LocalSettings.GetBool("DescriptionPanel.Collapsed",false);
			if (DescriptionPanel.Collapsed)
			{
				DescriptionPanel.LayoutPanel();
			}
			FilesPanel.Collapsed = Preferences.LocalSettings.GetBool("FilesPanel.Collapsed", false);
			if (FilesPanel.Collapsed)
			{
				FilesPanel.LayoutPanel();
			}
			ShelvedFilesPanel.Collapsed = Preferences.LocalSettings.GetBool("ShelvedFilesPanel.Collapsed", false);
			if (ShelvedFilesPanel.Collapsed)
			{
				ShelvedFilesPanel.LayoutPanel();
			}
			JobsPanel.Collapsed = Preferences.LocalSettings.GetBool("JobsPanel.Collapsed", false);
			if (JobsPanel.Collapsed)
			{
				JobsPanel.LayoutPanel();
			}
			
			slidingPanelContainer1.LayoutPanel();
			slidingPanelContainer1.Refresh();
		}


		public delegate void PendingChangelistDialogCloseDelegate(DialogResult result, int changelistId);

		public PendingChangelistDialogCloseDelegate OnDialogClosed { get; set; } 

		public string Description
		{
			get { return DescriptionTB.Text; }
			set
			{
				DescriptionTB.Text = value;
				DescriptionTB_TextChanged(null, null);
			}
		}

		private P4ScmProvider _scm { get; set; }

		// map from depot path to list view item
		private IDictionary<string, P4FileTreeListViewItem> listItemMap = null;

		//// map from depot path to file spec
		//private IDictionary<string, P4.FileSpec> fileSpecMap = null;

		// map from depot path to meta data
		private IDictionary<string, P4.FileMetaData> fileMetaDataMap = null;

		private int _changelistId;
		public int ChangeListId
		{
			get { return _changelistId; }
			set
			{
				_changelistId = value;

				if (ChangeListId > 0)
				{
					P4.Changelist c = _scm.GetChangelist(ChangeListId);
					if (c == null)
					{
						throw new ApplicationException(Resources.PendingChangelistDlg_CantCreateChanglistError);
					}

					ChangeList = c;
				}
				else
				{
                    P4.Changelist c = _scm.Connection.Repository.NewChangelist();
					if (c == null)
					{
						throw new ApplicationException(Resources.PendingChangelistDlg_CantCreateChanglistError);
					}

					ChangeList = c;

					if (_changeList.Description.Trim() == Resources.DefaultChangeListDescription)
					{
						_changeList.Description = string.Empty;
					}
				}
			}
		}

		public string UserText
		{
			set { UserTB.Text = value; }
			get { return UserTB.Text; }
		}
		public string WorkspaceText
		{
			set { WorkspaceTB.Text = value; }
			get { return WorkspaceTB.Text; }
		}

		P4.Changelist _changeList = null;

		public P4.Changelist ChangeList 
		{
			get { return _changeList; } 
			set
			{
				if (value == null)
				{
					return;
				}
				_changeList = value;
				_changelistId = _changeList.Id;

				if (_changelistId < 0)
				{
					ChangelistTB.Text = Resources.Changelist_New;
				}
				else if (_changelistId == 0)
				{
					ChangelistTB.Text = Resources.Changelist_Default;
				}
				else
				{
					ChangelistTB.Text = _changelistId.ToString();
				}

                if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                {
                    DateTime local = _changeList.ModifiedDate;

                    // we need a pref for local time, until then, don't do this:
                    //local = TimeZone.CurrentTimeZone.ToLocalTime(local);
                    DateTB.Text = local.ToString("yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                    DateTime local = _changeList.ModifiedDate;

                    // we need a pref for local time, until then, don't do this:
                    //local = TimeZone.CurrentTimeZone.ToLocalTime(local);
                    DateTB.Text = string.Format("{0} {1}", local.ToShortDateString(),
                                                        local.ToShortTimeString());
                }

				UserTB.Text = _changeList.OwnerName;
				
				if (_changelistId <= 0)
				{
					DateTB.Text = string.Empty;
				}
				WorkspaceTB.Text = _changeList.ClientId;

				RestrictAccessCB.Checked = (_changeList.Type == ChangeListType.Restricted);

                if (_changeList.Description.Trim() == Resources.DefaultChangeListDescription)
                {
                    _changeList.Description = string.Empty;
                }
                DescriptionTB.Text = _changeList.Description;
                DescriptionTB_TextChanged(null, null);

                FileListLV.Items.Clear();
                FileFlatList.Items.Clear();

				listItemMap = new Dictionary<string, P4FileTreeListViewItem>();

				fileMetaDataMap = new Dictionary<string, P4.FileMetaData>();

				IList<P4.FileSpec> fileList = null;
				IList<P4.FileMetaData> mdl = null;

				if (ChangeListId > 0)
				{
					//it's either an existing numbered list
					mdl = _changeList.Files;
				}
				else
				{
					// It's the 'default' change list to be turned into a new numbered changelist or
					//  or a new change list to be edited.

					// removed shelved section here.
					
					P4.Options options = new P4.GetFileMetaDataCmdOptions(
						P4.GetFileMetadataCmdFlags.Opened,
						null, null, -1, null, "default", null);
                    string wsPath = string.Format("//{0}/...", _scm.Connection.Repository.Connection.Client.Name);
					// get files opened in default changelist
                    mdl = _scm.Connection.Repository.GetFileMetaData(options, new P4.FileSpec(new P4.ClientPath(wsPath), null));
				}

                int maxFiles = Preferences.LocalSettings.GetInt("Number_files", 1000);

                if ((mdl != null) && (mdl.Count > 0))
                {
                    P4FileTreeListViewItem[] newItems = new P4FileTreeListViewItem[mdl.Count];

                    fileList = new List<P4.FileSpec>(mdl.Count);
                    listItemMap = new Dictionary<string, P4FileTreeListViewItem>(mdl.Count);

                    fileMetaDataMap = new Dictionary<string, P4.FileMetaData>(mdl.Count);
                    //if (mdl.Count > maxFiles)
                    //{
                    //    FileListLV.Visible = false;
                    //    FileFlatList.Visible = true;
                    //}
                    //else
                    //{
                        FileListLV.Visible = true;
                        FileFlatList.Visible = false;

                        IList<object> fields = new List<object>();
                        fields.Add(P4FileTreeListViewItem.SubItemFlag.None);
                        fields.Add(P4FileTreeListViewItem.SubItemFlag.DepotFileName);
                        fields.Add(P4FileTreeListViewItem.SubItemFlag.DepotDepotFolder);
                        fields.Add(P4FileTreeListViewItem.SubItemFlag.ResolvedStatus);
                        fields.Add(P4FileTreeListViewItem.SubItemFlag.FileExtension);
                        fields.Add(P4FileTreeListViewItem.SubItemFlag.Action);

                    int idx = 0;
                        foreach (P4.FileMetaData file in mdl)
                        {
                            P4.FileSpec fs = (P4.FileSpec)file;
                            if ((fs.Version is P4.Revision) && (((P4.Revision)fs.Version).Rev == 0))
                            {
                                fs.Version = null;
                            }
                            fileList.Add(fs);

                            P4FileTreeListViewItem item = new P4FileTreeListViewItem(null, file, fields);
                            item.Tag = file;

                            listItemMap.Add(file.DepotPath.Path, item);
                            //fileSpecMap.Add(file.DepotPath.Path, file);
                            newItems[idx++] = item;
                        }
                        FileListLV.Items.AddRange(newItems);

                        IList<P4.FileMetaData> fileMetaData = _scm.GetFileMetaData(fileList, null);
                        if ((fileList != null) && (fileList.Count > fileMetaData.Count))
                        {
                            //could not get the metadata for one of the files
                            //P4.P4CommandResult r = Repository.Connection.LastResults;
                            //P4ErrorDlg.Show(r.ErrorList, false);
                        }
                    foreach (P4.FileMetaData file in fileMetaData)
                    {
                        fileMetaDataMap.Add(file.DepotPath.Path, file);

                        string path = file.DepotPath.Path;
                        P4FileTreeListViewItem item = listItemMap[path];

                        item.FileData = file;
                        item.Checked = true;
                    }
                        SelectAllFilesCB.Checked = true;
                    //}
                }
				ShelvedFilesLV.Items.Clear();

				if (_scm.ServerVersion >= Versions.V9_2 && ChangeList.Id > 0)
				{
					//if ()

					P4.Options describeOptions =
						new P4.DescribeCmdOptions(P4.DescribeChangelistCmdFlags.Shelved | P4.DescribeChangelistCmdFlags.Omit, -1, -1);
					P4.Changelist sc = _scm.GetChangelist(_changelistId, describeOptions);
					if ((sc != null) && (sc.ShelvedFiles != null) && (sc.ShelvedFiles.Count > 0))
					{
                        ListViewItem[] newItems = new ListViewItem[sc.ShelvedFiles.Count];
                        int idx =0;

						foreach (P4.ShelvedFile shelf in sc.ShelvedFiles)
						{
							string fileName = System.IO.Path.GetFileName(shelf.Path.Path.ToString());
						    string folder = shelf.Path.Path.Remove(shelf.Path.Path.LastIndexOf("/"));
							ListViewItem fileitem = new ListViewItem(fileName);
							fileitem.SubItems.Add(shelf.Action.ToString());
							fileitem.SubItems.Add(folder);
							//fileitem.ImageIndex = 3;
							fileitem.Tag = shelf;

                            //this.ShelvedFileIcons.TransparentColor = System.Drawing.Color.White;
                            //this.ShelvedFileIcons.Images.Add("shelve_icon_base", Images.shelve_icon_base);
                            //this.ShelvedFileIcons.Images.Add("shelve_icon_edit", Images.shelve_icon_edit);
                            //this.ShelvedFileIcons.Images.Add("shelve_icon_add", Images.shelve_icon_add);
                            //this.ShelvedFileIcons.Images.Add("shelve_icon_delete", Images.shelve_icon_delete);
                            //this.ShelvedFileIcons.Images.Add("shelve_icon_branch", Images.shelve_icon_branch);
                            //this.ShelvedFileIcons.Images.Add("shelve_icon_integrate", Images.shelve_icon_integrate);
                            //this.ShelvedFileIcons.Images.Add("shelve_icon_moveadd", Images.shelve_icon_moveadd);
                            //this.ShelvedFileIcons.Images.Add("shelve_icon_movedelete", Images.shelve_icon_movedelete);
                            //this.ShelvedFileIcons.Images.Add("shelve_icon_archive", Images.shelve_icon_archive);
                            //this.ShelvedFileIcons.Images.Add("shelve_icon_purge", Images.shelve_icon_purge);


							switch (shelf.Action)
							{
								case P4.FileAction.Edit:
									fileitem.ImageKey = "shelve_icon_edit";
							        fileitem.ImageIndex = 0;
									break;
								case P4.FileAction.Add:
									fileitem.ImageKey = "shelve_icon_add";
                                    fileitem.ImageIndex = 0;
                                    break;
                                case P4.FileAction.Delete:
									fileitem.ImageKey = "shelve_icon_delete";
                                    fileitem.ImageIndex = 0;
                                    break;
                                case P4.FileAction.Branch:
									fileitem.ImageKey = "shelve_icon_branch";
                                    fileitem.ImageIndex = 0;
                                    break;
                                case P4.FileAction.Integrate:
									fileitem.ImageKey = "shelve_icon_integrate";
                                    fileitem.ImageIndex = 0;
                                    break;
                                case P4.FileAction.MoveAdd:
									fileitem.ImageKey = "shelve_icon_moveadd";
                                    fileitem.ImageIndex = 0;
                                    break;
                                case P4.FileAction.MoveDelete:
									fileitem.ImageKey = "shelve_icon_movedelete";
                                    fileitem.ImageIndex = 0;
                                    break;
                                case P4.FileAction.Purge:
									fileitem.ImageKey = "shelve_icon_purge";
                                    fileitem.ImageIndex = 0;
                                    break;
                                default:
									fileitem.ImageKey = "shelve_icon_base";
                                    fileitem.ImageIndex = 0;
                                    break;
                            }
                            newItems[idx++] = fileitem;
						}
						ShelvedFilesLV.Items.AddRange(newItems);
					}
				}
				else
				{
					ShelvedFilesPanel.CollapsedHeight = 0;
					ShelvedFilesPanel.Collapsed = true;
					ShelvedFilesPanel.Collapse();
					ShelvedFilesPanel.LayoutPanel();
				}
				JobsListLV.Items.Clear();

				//IList<P4.Job> jobs = null;
				if ((_changeList.Jobs != null) && (_changeList.Jobs.Count > 0))
				{
                    ListViewItem[] newItems = new ListViewItem[_changeList.Jobs.Count];
                    int idx = 0;
					foreach (string JobId in _changeList.Jobs.Keys)
					{
						P4.Job job = _scm.getJob(JobId);
						//jobs.Add(job);

						ListViewItem item = new ListViewItem(job.Id);
						item.Tag = job;
						if (job.ContainsKey("Status") && job["Status"] is string)
						{
							item.SubItems.Add(job["Status"] as string);
						}
						else
						{
							item.SubItems.Add(string.Empty);
						}
						if (job.ContainsKey("Description") && job["Description"] is string)
						{
							item.SubItems.Add(job["Description"] as string);
						}
						else
						{
							item.SubItems.Add(string.Empty);
						}
						if (ChangeListId > 0)
						{
							item.Checked = true;
						}
                        item.ImageKey = "jobs_icon";
					    item.ImageIndex = 0;

                        newItems[idx++]= item;
					}
						JobsListLV.Items.AddRange(newItems);
                }
			}
		}

		public IList<P4.ShelvedFile> ShelvedFiles
		{
			get
			{
				List<P4.ShelvedFile> value = new List<P4.ShelvedFile>();

				foreach (ListViewItem item in ShelvedFilesLV.Items)
				{
					value.Add((P4.ShelvedFile)item.Tag);
				}
				return value;
			}
		}


		public IList<P4.FileMetaData> FileList
		{
			get
			{
				List<P4.FileMetaData> value = new List<P4.FileMetaData>();

				foreach (ListViewItem item in FileListLV.Items)
				{
					value.Add((P4.FileMetaData)item.Tag);
				}
				return value;
			}
		}

		public IList<P4.FileMetaData> SelectedFileList
		{
			get
			{
				return FileListLV.SelectedFileList;
			}
			set
			{
				FileListLV.SelectedFileList = value;
			}
		}

		public IList<P4.FileMetaData> UnselectedFileList
		{
			get
			{
				return FileListLV.UnselectedFileList;
			}
		}

		public IList<P4.ShelvedFile> SelectedShelvedFileList
		{
			get
			{
				if (ShelvedFilesLV.SelectedItems.Count <= 0)
				{
					return null;
				}

				IList<P4.ShelvedFile> value = new List<P4.ShelvedFile>();

				foreach (ListViewItem item in ShelvedFilesLV.SelectedItems)
				{
					P4.ShelvedFile sv = item.Tag as P4.ShelvedFile;
					value.Add(sv);
				} 
				if (value.Count == 0)
				{
					return null;
				}
				return value;
			}
		}

		public IList<string> GetSelectedShelvedFiles()
		{
			if (ShelvedFilesLV.SelectedItems.Count <= 0)
			{
				return null;
			}
			List<string> value = new List<string>();

			foreach (ListViewItem item in ShelvedFilesLV.SelectedItems)
			{
				P4.ShelvedFile sv = item.Tag as P4.ShelvedFile;
				if (sv != null) value.Add(sv.Path.Path);
			}
			if (value.Count == 0)
			{
				return null;
			}
			return value;
		}

		public IList<P4.Job> JobList
		{
			get
			{
				List<P4.Job> value = new List<P4.Job>();

				foreach (ListViewItem item in JobsListLV.Items)
				{
					value.Add((P4.Job)item.Tag);
				}
				if (value.Count <= 0)
				{
					return null;
				}
				return value;
			}
			set
			{
				JobsListLV.Items.Clear();
				foreach (P4.Job job in value)
				{
					ListViewItem item = new ListViewItem(job.Id);
					item.Tag = job;
					if (job.ContainsKey("Status") && job["Status"] is string)
					{
						item.SubItems.Add(job["Status"] as string);
					}
					else
					{
						item.SubItems.Add(string.Empty);
					}
					if (job.ContainsKey("Description") && job["Description"] is string)
					{
						item.SubItems.Add(job["Description"] as string);
					}
					else
					{
						item.SubItems.Add(string.Empty);
					}
					item.Checked = true;
                    item.ImageKey = "jobs_icon";
                    item.ImageIndex = 0;
                    JobsListLV.Items.Add(item);
				}
			}
		}

		public IList<P4.Job> SelectedJobList
		{
			get
			{
				List<P4.Job> value = new List<P4.Job>();

				foreach (ListViewItem item in JobsListLV.Items)
				{
					if (item.Checked)
					{
						value.Add((P4.Job)item.Tag);
					}
				}
				if (value.Count <= 0)
				{
					return null;
				}
				return value;
			}
		}

		public IList<P4.Job> UnselectedJobList
		{
			get
			{
				List<P4.Job> value = new List<P4.Job>();

				foreach (ListViewItem item in JobsListLV.Items)
				{
					if (item.Checked == false)
					{
						value.Add((P4.Job)item.Tag);
					}
				}
				if (value.Count <= 0)
				{
					return null;
				}
				return value;
			}
		}

		private void DescriptionTB_TextChanged(object sender, EventArgs e)
		{
			DescriptionPanel.ShowAlert = ((DescriptionTB.Text.Length <= 0) ||
				(DescriptionTB.Text.Trim() == Resources.DefaultChangeListDescription));

			OkBtn.Enabled = DescriptionPanel.ShowAlert == false;
		}


		private void JobTB_TextChanged(object sender, EventArgs e)
		{
			AddJobBtn.Enabled = (JobTB.Text.Length > 0);
		}

		private bool JobInList(P4.Job job)
		{
			foreach (ListViewItem item in JobsListLV.Items)
			{
				P4.Job j = (P4.Job)item.Tag;
				if (j.Id == job.Id)
				{
					return true;
				}
			}
			return false;
		}

		private void AddJobBtn_Click(object sender, EventArgs e)
		{
			P4.Job job = null;
			try
			{
				string filter = string.Format("Job={0}", JobTB.Text);
				IList<P4.Job> shortList = _scm.getJobs(P4.JobsCmdFlags.None, filter, 1, null);
				if (shortList != null && shortList.Count == 1)
				{
					job = shortList[0];
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "P4VS Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if ((job == null) || (JobInList(job)))
			{
				string msg;
				if (job == null)
				{
					msg = string.Format(Resources.PendingChangelistDlg_JobDoesntExistError, JobTB.Text);
				}
				else
				{
					msg = string.Format(Resources.PendingChangelistDlg_JobAlreadyLinkedError, JobTB.Text);
				}
				MessageBox.Show(msg, Resources.PerforceSCM, MessageBoxButtons.OK, MessageBoxIcon.Error);
				// still null or it's in the list already;
				return;
			}
			ListViewItem item = new ListViewItem(job.Id);
			item.Tag = job;
			if (job.ContainsKey("Status") && job["Status"] is string)
			{
				item.SubItems.Add(job["Status"] as string);
			}
			else
			{
				item.SubItems.Add(string.Empty);
			}
			if (job.ContainsKey("Description") && job["Description"] is string)
			{
				item.SubItems.Add(job["Description"] as string);
			}
			else
			{
				item.SubItems.Add(string.Empty);
			}
			item.Checked = true;
            item.ImageKey = "jobs_icon";
            item.ImageIndex = 0;
			JobsListLV.Items.Add(item);
		}

		private void BrowseJobsBtn_Click(object sender, EventArgs e)
		{
			JobsBrowserDlg dlg = new JobsBrowserDlg(_scm);

			//dlg.scm =

			if (DialogResult.Cancel != dlg.ShowDialog())
			{
				IList<P4.Job> newJobsToAdd = dlg.SelectedJobs;

				if ((newJobsToAdd != null) && (newJobsToAdd.Count > 0))
				{
					foreach (P4.Job job in newJobsToAdd)
					{
						ListViewItem item = new ListViewItem(job.Id);
						item.Tag = job;

						if ((job == null) || (JobInList(job)))
						{
							// it's null or in the list already;
							return;
						}

						if (job.ContainsKey("Status") && job["Status"] is string)
						{
							item.SubItems.Add(job["Status"] as string);
						}
						else
						{
							item.SubItems.Add(string.Empty);
						}
						if (job.ContainsKey("Description") && job["Description"] is string)
						{
							item.SubItems.Add(job["Description"] as string);
						}
						else
						{
							item.SubItems.Add(string.Empty);
						}
						item.Checked = true;
                        item.ImageKey = "jobs_icon";
                        item.ImageIndex = 0;
						JobsListLV.Items.Add(item);
					}
				}
			}
		}

		private P4.Changelist SaveNumberedChangelist()
		{
			P4.Changelist newChange = null;

			if (ChangeList.Id <= 0)
			{
				newChange = _scm.Connection.Repository.NewChangelist();
            }
			else
			{
				newChange = new P4.Changelist(_changelistId, true);
			}

			newChange.ClientId = WorkspaceTB.Text;
			newChange.OwnerName = UserTB.Text;
			newChange.Description = DescriptionTB.Text;

			List<KeyValuePair<string, string>> changeSpec = _scm.GetChangeSpec();
			if (changeSpec != null)
			{

				foreach (var pair in changeSpec)
				{
					if (pair.Value.Contains("Type select 10 optional"))
					{
						newChange.Type = ChangeListType.Public;
						if (RestrictAccessCB.Checked)
						{
							newChange.Type = ChangeListType.Restricted;
						}
					}

				}

			}

			IList<P4.Job> jobs = SelectedJobList;
			newChange.Jobs = null;
			if (jobs !=null && jobs.Count > 0)
			{
				newChange.Jobs = new Dictionary<string, string>();
				foreach (P4.Job job in jobs)
				{
					newChange.Jobs[job.Id] = null;
				}
			}

			IList<P4.FileMetaData> files = SelectedFileList;
			newChange.Files = files;
            newChange = _scm.SaveChangelist(newChange, null);


			return newChange;
		}

		private void ApplyBtn_Click(object sender, EventArgs e)
		{
			SaveNumberedChangelist();
			this.DialogResult = DialogResult.OK;
		}

		private void OkBtn_Click(object sender, EventArgs e)
		{
			//Preferences.LocalSettings["DetailsPanel.Collapsed"] = DetailsPanel.Collapsed;
			Preferences.LocalSettings["DescriptionPanel.Collapsed"] = DescriptionPanel.Collapsed;
			Preferences.LocalSettings["FilesPanel.Collapsed"] = FilesPanel.Collapsed;
			Preferences.LocalSettings["ShelvedFilesPanel.Collapsed"] = ShelvedFilesPanel.Collapsed;
			Preferences.LocalSettings["JobsPanel.Collapsed"] = JobsPanel.Collapsed;

            P4.Changelist change = null;
            change= SaveNumberedChangelist();

            if (change != null)
            {
                IDictionary<int, object> fileMetaDataMap = new Dictionary<int, object>();
                fileMetaDataMap.Add(0, null);
                fileMetaDataMap.Add(change.Id, null);
                _scm.BroadcastChangelistUpdates(fileMetaDataMap, P4ScmProvider.ChangelistUpdateArgs.UpdateType.Add);
                DialogResult = DialogResult.OK;
                Close();
            }
            return;
		}


		private void CancelBtn_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		#region Drag/Drop support in files list
		private void FileListLV_DragDrop(object sender, DragEventArgs e)
		{

		}

		private void FileListLV_DragEnter(object sender, DragEventArgs e)
		{

		}

		private void FileListLV_DragLeave(object sender, EventArgs e)
		{

		}

		private void FileListLV_DragOver(object sender, DragEventArgs e)
		{

		}

		private void FileListLV_GiveFeedback(object sender, GiveFeedbackEventArgs e)
		{

		}

		private void FileListLV_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{

		}
		#endregion

		private void SelectAllFilesCB_CheckedChanged(object sender, EventArgs e)
		{
			// turn off the individual item check handler, so it doesn't try to change the
			// state of the check all check box
			FileListLV.ItemChecked -= new ItemCheckedEventHandler(FileListLV_ItemChecked);
			try
			{
				foreach (P4FileTreeListViewItem item in FileListLV.Items)
				{
					item.Checked = SelectAllFilesCB.Checked;
				}
			}
			finally
			{
				FileListLV.Invalidate();
				// turn back on the individual item check handler
				FileListLV.ItemChecked += new ItemCheckedEventHandler(FileListLV_ItemChecked);
			}
		}

		private void FileListLV_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			// turn off the select all checked event, so it won't effect other controls
			SelectAllFilesCB.CheckedChanged -= new EventHandler(SelectAllFilesCB_CheckedChanged);

			try
			{
				if (((P4FileTreeListViewItem)e.Item).Checked == false)
				{
					// uncheck select all
					SelectAllFilesCB.Checked = false;
				}
				else
				{
					foreach (P4FileTreeListViewItem item in FileListLV.Items)
					{
						if (item.Checked == false)
						{
							// if any single one is unchecked, turn off check all
							SelectAllFilesCB.Checked = false;
							return;
						}
					}
					// all must be checked, so check check all
					SelectAllFilesCB.Checked = true;

				}
			}
			finally
			{
				// turn back on the select all checked event, so it won't effect other controls
				SelectAllFilesCB.CheckedChanged += new EventHandler(SelectAllFilesCB_CheckedChanged);
			}
		}

		private void SelectAllJobsCB_CheckedChanged(object sender, EventArgs e)
		{
			// turn off the individual item check handler, so it doesn't try to change the
			// state of the check all check box
			JobsListLV.ItemChecked += new ItemCheckedEventHandler(JobsListLV_ItemChecked);
			try
			{
                if (SelectAllJobsCB.Checked)
				foreach (ListViewItem item in JobsListLV.Items)
				{
				    item.Checked = true;
				}
                else
                {
                    foreach (ListViewItem item in JobsListLV.Items)
                    {
                        item.Checked = false;
                    }
                }
			}
			finally
			{
				// turn back on the individual item check handler
				JobsListLV.ItemChecked += new ItemCheckedEventHandler(JobsListLV_ItemChecked);
			}
		}

		private void JobsListLV_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
            if (e.Item.Checked == true)
            {
                e.Item.ImageIndex = 1;
            }
            else
            {
                e.Item.ImageIndex = 0;
            }
			// turn off the select all checked event, so it won't effect other controls
			SelectAllJobsCB.CheckedChanged -= new EventHandler(SelectAllJobsCB_CheckedChanged);
			try
			{
				if (e.Item.Checked == false)
				{
					// uncheck select all
					SelectAllJobsCB.Checked = false;
				}
				else
				{
					foreach (ListViewItem item in JobsListLV.Items)
					{
                        if (item != null && item.Checked == false)
                        {
							// if any single one is unchecked, turn off check all
							SelectAllJobsCB.Checked = false;
							return;
						}
					}
					// all must be checked, so check check all
					SelectAllJobsCB.Checked = true;

				}
			}
			finally
			{
				// turn back on the select all checked event, so it won't effect other controls
				SelectAllJobsCB.CheckedChanged += new EventHandler(SelectAllJobsCB_CheckedChanged);
			}
		}

		private void UnshelveSFCM_Click(object sender, EventArgs e)
		{
			IList<ShelvedFile> SelectedShelvedFiles = this.SelectedShelvedFileList;
			if (SelectedShelvedFiles == null)
			{
				return;
			}
			UnshelveFileDialog.UnshelveFiles(_scm, _changelistId, SelectedShelvedFiles);
            ChangeList = _scm.GetChangelist(_changelistId);
		}

		private void DeleteShelvedFileSFCM_Click(object sender, EventArgs e)
		{
			IList<string> SelectedShelvedFiles = this.GetSelectedShelvedFiles();
			if (SelectedShelvedFiles == null)
			{
				return;
			}
			P4.ShelveFilesCmdFlags dflags = P4.ShelveFilesCmdFlags.Delete;
			_scm.ShelveFiles(_changelistId, null, dflags, true, SelectedShelvedFiles.ToArray());
            ChangeList = _scm.GetChangelist(_changelistId);
		}

		private void OpenSFCM_Click(object sender, EventArgs e)
		{
			IList<ShelvedFile> SelectedShelvedFiles = this.SelectedShelvedFileList;
			if (SelectedShelvedFiles.Count <= 0)
			{
				return;
			}
			ShelvedFile selectedFile = SelectedShelvedFiles[0];

			P4.FileSpec fs = FileSpec.DepotSpec(selectedFile.Path.Path);
			fs.Version = new P4.ShelvedInChangelistIdVersion(_changelistId);

            long maxPreviewSize = 1024 * ((long) Preferences.LocalSettings.GetInt("Size_files", 500));
            long shelvedSize = _scm.GetShelvedFileSize(_changelistId, selectedFile.Path.Path);
            if (shelvedSize > maxPreviewSize)
            {
                string size = P4ObjectTreeListViewItem.PrettyPrintFileSize(shelvedSize);
                string msg = string.Format(Resources.FileExceedsMaxPreviewSizeWarning, size);
                if (DialogResult.No == MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    return;
                }
            }
            using (TempFile shelvedFile = new TempFile(fs))
			{
				if (_scm.GetFileVersion(shelvedFile, fs) == null)
				{
					shelvedFile.Dispose();
					return;
				}

				if (Preferences.LocalSettings.GetBool("OpenShelvedFileInEditor", true))
				{
					EnvDTE.DTE dte = P4VsProvider.GetDTE();
					dte.ItemOperations.OpenFile(shelvedFile, null);
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

        // select all with ctrl-a
        private void DescriptionTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                DescriptionTB.SelectionStart = 0;
                DescriptionTB.SelectionLength = DescriptionTB.Text.Length;
            }
        }

		private void PendingChangelistDlg_FormClosed(object sender, FormClosedEventArgs e)
		{
			OnDialogClosed(DialogResult, _changelistId);
		}

        private void FilesContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            DiffvsHaveMI.Visible = false;
            ResolveMI.Visible = false;
            bool menusToShow = false;
            if ((FileListLV.SelectedItems != null) && (FileListLV.SelectedItems.Count > 0))
            {
                foreach (ListViewItem it in FileListLV.SelectedItems)
                {
                    if (it.SubItems[3].Text == Resources.P4FileTreeListViewItem_Unresolved)
                    {
                        ResolveMI.Visible = true;
                        menusToShow = true;
                        break;
                    }
                }
                foreach (ListViewItem it in FileListLV.SelectedItems)
                {
                    FileMetaData fmd = it.Tag as FileMetaData;
                    if (fmd != null && fmd.Action != FileAction.DeleteFrom &&
                        fmd.Action != FileAction.DeleteInto &&
                        fmd.Action != FileAction.MoveDelete &&
                        fmd.Action != FileAction.Delete &&
                        fmd.Action != FileAction.Add &&
                        fmd.Action != FileAction.Branch)
                    {
                        DiffvsHaveMI.Visible = true;
                        menusToShow = true;
                        break;
                    }
                }
                if (!menusToShow)
                {
                    e.Cancel = true;
                }
            }
        }

        private void FilesContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            FilesContextMenuStrip.Close();
            IList<string> files = new List<string>();
            IList<string> filesToRefresh = new List<string>();
            string menuItemId = (string)e.ClickedItem.Tag;
            switch (menuItemId)
            {
                case "DiffVsHave":
                    {
                        foreach (ListViewItem it in FileListLV.SelectedItems)
                        {
                            P4.FileMetaData fmd = it.Tag as P4.FileMetaData;
                            if (fmd != null) fmd = _scm.GetFileMetaData(fmd.DepotPath.Path);
                            if (fmd != null && fmd.Action != FileAction.DeleteFrom &&
                                    fmd.Action != FileAction.DeleteInto &&
                                    fmd.Action != FileAction.MoveDelete &&
                                    fmd.Action != FileAction.Delete &&
                                    fmd.Action != FileAction.Add &&
                                    fmd.Action != FileAction.Branch)
                            {
                                if (fmd.LocalPath != null)
                                {
                                    string file = fmd.LocalPath.Path;
                                    files.Add(file);
                                }
                            }
                        }
                        if (files != null&&files.Count>0)
                        {
                            _scm.DiffFiles(files.ToArray());
                        }
                        break;
                    }
                    
                case "Resolve":
                    {
                        foreach (ListViewItem it in FileListLV.SelectedItems)
                        {
                            P4.FileMetaData fmd = it.Tag as P4.FileMetaData;
                            if (fmd != null)
                            {
                                if (fmd.LocalPath==null && fmd.DepotPath!=null)
                                {
                                    fmd= _scm.GetFileMetaData(fmd.DepotPath.Path);
                                }

                                if (fmd.LocalPath != null)
                                {
                                    string file = fmd.LocalPath.Path;

                                    files.Add(file);
                                    filesToRefresh.Add(file);
                                }
                            }
                        }
                        if (files != null && files.Count > 0)
                        {
                            ResolveFileDlg.ResolveFiles(_scm, files, false, null,true);
                        }
                        // if any of the files were resolved
                        // refresh the changelist
                        if(files.Count< filesToRefresh.Count)
                        {
                            this.ChangeList= _scm.GetChangelist(ChangeListId);
                            _scm.SccService.UpdateProjectGlyphs(filesToRefresh, true);
                        }
                        break;
                    }
            }
        }

        private void ShelvedFilesLV_DoubleClick(object sender, EventArgs e)
		{
			if ((ShelvedFilesLV.SelectedItems == null) || (ShelvedFilesLV.SelectedItems.Count<=0))
			{
				return;
			}
			OpenSFCM_Click(sender, e);
		}

        private void DiffAgainstWorkspaceSFCM_Click(object sender, EventArgs e)
        {
            List<FileSpec> files = new List<FileSpec>();
            foreach (ListViewItem tlvi in ShelvedFilesLV.SelectedItems)
            {
                ShelvedFile shelve = tlvi.Tag as ShelvedFile;
                if (shelve != null)
                {
                    FileMetaData fmd = _scm.GetFileMetaData(shelve.Path.Path);
                    FileSpec fs1 = new FileSpec(fmd.DepotPath, null, null, new ShelvedInChangelistIdVersion(fmd.Change));
                    FileSpec fs2 = new FileSpec(null, null, fmd.LocalPath, null);
                    files.Add(fs1);
                    files.Add(fs2);
                }
                _scm.Diff2Files(files);
            }
        }

        private void DiffAgainstSourceSFCM_Click(object sender, EventArgs e)
        {
            List<FileSpec> files = new List<FileSpec>();
            foreach (ListViewItem tlvi in ShelvedFilesLV.SelectedItems)
            {
                ShelvedFile shelve = tlvi.Tag as ShelvedFile;
                if (shelve != null)
                {
                    FileMetaData fmd = _scm.GetFileMetaData(shelve.Path.Path);
                    FileSpec fs1 = new FileSpec(fmd.DepotPath, null, null, new Revision(fmd.HaveRev));
                    FileSpec fs2 = new FileSpec(fmd.DepotPath, null, null, new ShelvedInChangelistIdVersion(fmd.Change));
                    files.Add(fs1);
                    files.Add(fs2);
                }
                _scm.Diff2Files(files);
            }
        }

		private void gridLayoutPanel2_AfterLayoutGrid()
		{
			Point l = JobsListLV.Location;
			l.X += 6;
			l.Y += 7;
			this.SelectAllJobsCB.Location = l;

			l = FileListLV.Location;
			l.X += 4;
			l.Y += 7;
			this.SelectAllFilesCB.Location = l;
		}
	}
}
