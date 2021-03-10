using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Perforce.P4;
using NLog;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class SubmitDlg : AutoSizeForm
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
		private class FileListViewSorter : ColumnSorter
		{
			#region IComparer<ListViewItem> Members

			public override int Compare(object xo, object yo)
			{
				try
				{
					int result = 0;

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

					if (SortColumn == 0)
					{
						if (x.Checked && !y.Checked)
							result = -1;
						else if (y.Checked && !x.Checked)
							result = 1;
						else
							result = string.Compare(x.SubItems[1].Text, y.SubItems[1].Text);
					}
					else
					{
						result = string.Compare(x.SubItems[SortColumn].Text, y.SubItems[SortColumn].Text);
					}
					if (Order == SortOrder.Ascending)
					{
						// Ascending sort is selected, return normal result of compare operation
						return result;
					}
					else if (Order == SortOrder.Descending)
					{
						// Descending sort is selected, return negative result of compare operation
						return (-result);
					}
					else
					{
						// Return '0' to indicate they are equal
						return 0;
					}
				}
				catch (Exception ex)
				{
					FileLogger.LogException("FileListViewSorter.Compare", ex);
					return 0;
				}
			}

			#endregion
		}

		private System.Windows.Forms.ImageList ListImages;

        public static bool SubmitFiles(IList<string> files, P4ScmProvider Scm, bool inCommandLineMode,
            string description)
        {
			Dictionary<int, IDictionary<string, P4.FileMetaData>> changeLists = new Dictionary<int, IDictionary<string, P4.FileMetaData>>();
            return SubmitFiles(files, Scm, inCommandLineMode, description, ref changeLists);
        }
        public static bool SubmitFiles(IList<string> files, P4ScmProvider Scm, bool inCommandLineMode,
            string description, ref Dictionary<int, IDictionary<string, P4.FileMetaData>> changeLists)
		{
			bool result = false; // at least one submit dialog was not canceled

			bool dlgShown = false;
			changeLists = new Dictionary<int, IDictionary<string, P4.FileMetaData>>();

			string wsRoot = null;
            if (Scm != null && !string.IsNullOrEmpty(Scm.Connection.WorkspaceRoot))
			{
                wsRoot = System.IO.Path.GetFullPath(Scm.Connection.WorkspaceRoot).ToLower();
			}

			for (int idx = 0; idx < files.Count; idx++)
			{
				if (files[idx] == null)
				{
					continue;
				}
				P4.FileMetaData fmd = null;
				if ((Scm.Connection.WorkspaceRoot != "null") && (files[idx].ToLower().StartsWith(wsRoot) == false))
				{
					// not in workspace, so skip this file
					continue;
				}
				fmd = Scm.Fetch(files[idx]);

				if ((fmd == null) || (fmd.Action == P4.FileAction.None))
				{
                        continue;
				}
				int changeListId = fmd.Change;

				if (changeLists.ContainsKey(changeListId) == false)
				{
					changeLists.Add(changeListId, new Dictionary<string, P4.FileMetaData>());
				}
				if (changeLists[changeListId].ContainsKey(fmd.DepotPath.Path) == false)
				{
					// haven't already seen this file
					changeLists[changeListId].Add(fmd.DepotPath.Path, fmd);
					if (fmd.MovedFile != null)
					{
						P4.FileMetaData movedFmd = Scm.GetFileMetaData(fmd.MovedFile.Path);
						if ((movedFmd != null) && (movedFmd.LocalPath != null) &&
							(changeLists[changeListId].ContainsKey(movedFmd.DepotPath.Path) == false))
						{
							files.Add(movedFmd.LocalPath.Path);
						}
					}
				}
			}
			foreach (int changeListId in changeLists.Keys)
			{
				P4.Changelist changelist = null;
				SubmitDlg dlg = new SubmitDlg(Scm);

				if (changeListId <= 0)
				{
                    changelist = Scm.Connection.Repository.NewChangelist();
					dlg.Text = Resources.SubmitDlg_SubmitFilesTitle;

					if (changelist.Description.Trim() == Resources.DefaultChangeListDescription)
					{
						changelist.Description = string.Empty;
					}
					//dlg.Description = string.Empty;
				}
				else
				{
                    changelist = Scm.Connection.Repository.GetChangelist(changeListId);
					dlg.Text = string.Format(Resources.SubmitDlg_SubmitChangelistTitle, changeListId); 
				}

                if (description!=null)
                {
                    dlg.Description = description;
                }
                else
                {
				    dlg.Description = changelist.Description;
                }

				dlg.ChangeListId = changeListId;
				if (dlg.ChangeListId == -3)
				{
					// no files to list
					continue;
				}

				dlg.SelectedFileList = changeLists[changeListId].Values.ToList();
				if ((changeLists[changeListId].Values != null) && (changeLists[changeListId].Values.Count > 0) && (dlg.SelectedFileCount <= 0))
				{
					// no files selected from list
					continue;
				}
				bool reopenFiles = dlg.CheckOutAfterSubmit;
				IList<P4.Job> jobs = null;

				P4.ClientSubmitOptions submitOptins = dlg.SubmitOptions;

				if (changelist != null)
				{
					jobs = null;
					if (changelist.Jobs != null)
					{
						jobs = new List<P4.Job>();
						foreach (string jobId in changelist.Jobs.Keys)
						{
							P4.Job job = Scm.getJob(jobId);
							if (job != null)
							{
								jobs.Add(job);
							}
						}
					}
					dlg.JobList = jobs;
				}
				DialogResult res = DialogResult.OK;

				if (inCommandLineMode == false)
				{
					res = dlg.ShowDialog();
				}
				else
				{
					dlg.Description = Resources.SubmitDlg_DefaultChangelistDescription;
				}

				// dialog box has been shown
				dlgShown = true;

				if (res == DialogResult.Cancel)
				{
                    // refresh the default changelist, it is possible that the Cancel
                    // is a Save
                    if (changelist.Id <=0)
                    {
                        Scm.BroadcastChangelistUpdate(dlg,
new P4ScmProvider.ChangelistUpdateArgs(0, P4ScmProvider.ChangelistUpdateArgs.UpdateType.Submitted));
                    }
                    continue;
				}
				else
				{
					result = true;
				}
				if (changeListId > 0)
				{
					// Attach/Remove jobs to existing changelists so there status is update on submit
					// need to do it before the submit so any previously attached jobs that have been
					// removed are not updated.

					jobs = dlg.SelectedJobList;

					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.None, changelist.Id, dlg.JobStatusOnSubmit);
						try
						{
							changelist.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}

					jobs = dlg.UnselectedJobList;
					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, changelist.Id, null);
						try
						{
							changelist.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}

					jobs = dlg.DeletedJobs;
					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, changelist.Id, null);
						try
						{
							changelist.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}
				}

				int newChangeId = -2;
				if (dlg.AllFilesSelected)
				{
					//all files have been selected, so submit entire chnagelist

					P4.Changelist newChangelist = null;
					//changeListId = dlg.ChangeListId;
					if (changeListId > 0)
					{
						// use the existing changelist
						if (changelist != null)
						{
							newChangelist = changelist;
						}
						else
						{
							newChangelist = Scm.GetChangelist(changeListId);
						}
					}
					else
					{
                        // default or new changelist, so create it
                        newChangelist = Scm.Connection.Repository.NewChangelist();

						newChangelist.Description = dlg.Description;
						newChangelist.Files = changelist.Files;

						newChangelist = Scm.SaveChangelist(newChangelist, null);
					}

					jobs = dlg.SelectedJobList;
					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.None, newChangelist.Id, dlg.JobStatusOnSubmit);
						try
						{
							newChangelist.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}

					jobs = dlg.UnselectedJobList;
					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newChangelist.Id, null);
						try
						{
							newChangelist.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}

					jobs = dlg.DeletedJobs;
					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newChangelist.Id, null);
						try
						{
							newChangelist.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}
					newChangeId = Scm.SubmitChangelist(newChangelist.Id, dlg.Description, dlg.SubmitOptions, dlg.CheckOutAfterSubmit);
					files = dlg.GetSelectedFiles();
					Scm.UpdateFiles(files, true);
					Scm.OnCacheFilesUpdated(files, true);
					if (changeListId != newChangeId)
					{
						Scm.BroadcastChangelistUpdate(dlg,
						   new P4ScmProvider.ChangelistUpdateArgs(changeListId, P4ScmProvider.ChangelistUpdateArgs.UpdateType.Submitted));
					}
				}
				else
				{
					// Only some files have been selected

					if (changeListId > 0)
					{
						// checking in some files in a numbered changelist,
						// so create a move the files that have not been 
						// selected to the default changelist.

						IList<string> unselectedFiles = dlg.GetUnselectedFiles();
						Scm.MoveFilesToChangeList(0, null, unselectedFiles.ToArray());
						// update 
						Scm.UpdateFiles(unselectedFiles, true);
						Scm.OnCacheFilesUpdated(unselectedFiles, true);

						// submit the pared down changelist
						newChangeId = Scm.SubmitChangelist(changeListId, dlg.Description, dlg.SubmitOptions, dlg.CheckOutAfterSubmit);
						files = dlg.GetSelectedFiles();
						Scm.UpdateFiles(files, true);
						Scm.OnCacheFilesUpdated(files, true);
					}
					else
					{
						// if submitting some files in the default changelist, submit them in a 
						//  new changelist

						P4.Changelist newChangelist = null;
						if (changeListId > 0)
						{
							// use the existing changelist
							if (changelist != null)
							{
								newChangelist = changelist;
							}
							else
							{
								newChangelist = Scm.GetChangelist(changeListId);
							}
						}
						else
						{
							// default or new changelist, so create it
							newChangelist = Scm.Connection.Repository.NewChangelist();

                            newChangelist.Description = dlg.Description;
							newChangelist.Files = dlg.SelectedFileList;

							newChangelist = Scm.SaveChangelist(newChangelist, null);

						}

						jobs = dlg.SelectedJobList;
						if ((jobs != null) && (jobs.Count > 0))
						{
							P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.None, newChangelist.Id, dlg.JobStatusOnSubmit);
							try
							{
								newChangelist.FixJobs(jobs, fixOptions);
							}
							catch { } // ignore errors
						}

						jobs = dlg.UnselectedJobList;
						if ((jobs != null) && (jobs.Count > 0))
						{
							P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newChangelist.Id, null);
							try
							{
								newChangelist.FixJobs(jobs, fixOptions);
							}
							catch { } // ignore errors
						}

						jobs = dlg.DeletedJobs;
						if ((jobs != null) && (jobs.Count > 0))
						{
							P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newChangelist.Id, null);
							try
							{
								newChangelist.FixJobs(jobs, fixOptions);
							}
							catch { } // ignore errors
						}
						newChangeId = Scm.SubmitChangelist(newChangelist.Id, dlg.Description, dlg.SubmitOptions, dlg.CheckOutAfterSubmit);
						files = dlg.GetSelectedFiles();
						Scm.UpdateFiles(files, true);
						Scm.OnCacheFilesUpdated(files, true);
                        // refresh the default changelist that the files were removed from
                        Scm.BroadcastChangelistUpdate(dlg,
   new P4ScmProvider.ChangelistUpdateArgs(changeListId, P4ScmProvider.ChangelistUpdateArgs.UpdateType.Submitted));
                    }
                }
				if ((changeListId <= 0) && (newChangeId > 0))
				{
					// Attach jobs to the newly created changelist so there status is update on submit
                    P4.Changelist newchange = Scm.Connection.Repository.GetChangelist(newChangeId);
					jobs = dlg.SelectedJobList;

					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.None, newchange.Id, dlg.JobStatusOnSubmit);
						try
						{
							newchange.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}

					jobs = dlg.UnselectedJobList;
					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newchange.Id, null);
						try
						{
							newchange.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}

					jobs = dlg.DeletedJobs;
					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newchange.Id, null);
						try
						{
							newchange.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}
				}
			}
			if (dlgShown == false)
			{
				MessageBox.Show(Resources.SubmitDlg_NoFilesToSubmitWarning, Resources.PerforceSCM,
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return result;
		}

		public static bool SubmitPendingChanglist(P4.Changelist changelist,
			IList<P4.FileMetaData> selectedFiles, P4ScmProvider Scm)
		{
			Dictionary<string, P4.FileMetaData> fileMap = new Dictionary<string, P4.FileMetaData>();
			if (selectedFiles == null || selectedFiles.Count < 1)
			{
				changelist = Scm.GetChangelist(changelist.Id);
				selectedFiles = changelist.Files;
			}
			for (int idx = 0; idx < selectedFiles.Count; idx++)
			{
				P4.FileMetaData fmd = selectedFiles[idx];

				if (fmd == null)
					continue;

				int changeListId = fmd.Change;

				if (fileMap.ContainsKey(fmd.DepotPath.Path) == false)
				{
					// haven't already seen this file
					fileMap.Add(fmd.DepotPath.Path, fmd);
					if (fmd.MovedFile != null)
					{
						P4.FileMetaData movedFmd = Scm.GetFileMetaData(fmd.MovedFile.Path);
						if ((movedFmd != null) && (movedFmd.LocalPath != null) &&
							(fileMap.ContainsKey(movedFmd.DepotPath.Path) == false))
						{
							selectedFiles.Add(movedFmd);
						}
					}
				}
			}

			string id = changelist.Id.ToString();
			if (changelist.Id <= 0)
			{
				id = Resources.Changelist_Default;
			}
			SubmitDlg dlg = new SubmitDlg(Scm);

            changelist = Scm.Connection.Repository.GetChangelist(changelist.Id);
			dlg.Text = string.Format(Resources.SubmitDlg_SubmitPendingChanglistTitle, id);

            if (changelist.Description.Trim() == Resources.DefaultChangeListDescription)
            {
                changelist.Description = string.Empty;
            }
            dlg.Description = changelist.Description;

			dlg.ChangeListId = changelist.Id;

			//dlg.SelectedFileList = new List<P4.FileMetaData>();

			dlg.SelectedFileList = selectedFiles;

			//dlg.changelistFileList = null;

			IList<P4.Job> jobs = new List<P4.Job>();
			if (changelist.Jobs != null)
			{
				foreach (string jobId in changelist.Jobs.Keys)
				{
					P4.Job job = Scm.getJob(jobId);
					if (job != null)
					{
						jobs.Add(job);
					}
				}
			}
			dlg.JobList = jobs;

			DialogResult res = DialogResult.OK;

			res = dlg.ShowDialog();

			int newChangelistId = -2;

            if (res == DialogResult.Cancel)
            {
                // refresh the default changelist, it is possible that the Cancel
                // is a Save
                if (changelist.Id <= 0)
                {
                    Scm.BroadcastChangelistUpdate(dlg,
new P4ScmProvider.ChangelistUpdateArgs(0, P4ScmProvider.ChangelistUpdateArgs.UpdateType.Submitted));
                }
                return false;
            }
			// Attach/Remove jobs to existing changelists so there status is update on submit
			// need to do it before the submit so any previously attached jobs that have been
			// removed are not updated.

			jobs = dlg.SelectedJobList;

			if (changelist.Id > 0)
			{
				if ((jobs != null) && (jobs.Count > 0))
				{
					P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.None, changelist.Id, dlg.JobStatusOnSubmit);
					try
					{
						changelist.FixJobs(jobs, fixOptions);
					}
					catch { } // ignore errors
				}

				jobs = dlg.UnselectedJobList;
				if ((jobs != null) && (jobs.Count > 0))
				{
					P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, changelist.Id, null);
					try
					{
						changelist.FixJobs(jobs, fixOptions);
					}
					catch { } // ignore errors
				}

				jobs = dlg.DeletedJobs;
				if ((jobs != null) && (jobs.Count > 0))
				{
					P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, changelist.Id, null);
					try
					{
						changelist.FixJobs(jobs, fixOptions);
					}
					catch { } // ignore errors
				}
			}
			int newChangeId = -2;
			if (dlg.AllFilesSelected)
			{
				//all files have been selected, so submit entire changelist

				P4.Changelist newChangelist = Scm.Connection.Repository.NewChangelist();
                int changeListId = dlg.ChangeListId;
				if (changeListId > 0)
				{
					// use the existing changelist
					if (changelist != null)
					{
						newChangelist = changelist;
					}
					else
					{
						newChangelist = Scm.GetChangelist(changeListId);
					}
				}
				else
				{
					// default or new changelist, so create it

					newChangelist = Scm.Connection.Repository.NewChangelist();
                    newChangelist.Description = dlg.Description;
					newChangelist.Files = dlg.SelectedFileList;

					newChangelist = Scm.SaveChangelist(newChangelist, null);

					// if new changelist comes back null, re-launch the submit dialog
					if (newChangelist == null)
					{
						SubmitPendingChanglist(changelist, selectedFiles, Scm);
					}
				}

				jobs = dlg.SelectedJobList;
				if ((jobs != null) && (jobs.Count > 0))
				{
					P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.None, newChangelist.Id, dlg.JobStatusOnSubmit);
					try
					{
						newChangelist.FixJobs(jobs, fixOptions);
					}
					catch { } // ignore errors
				}

				jobs = dlg.UnselectedJobList;
				if ((jobs != null) && (jobs.Count > 0))
				{
					P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newChangelist.Id, null);
					try
					{
						newChangelist.FixJobs(jobs, fixOptions);
					}
					catch { } // ignore errors
				}

				jobs = dlg.DeletedJobs;
				if ((jobs != null) && (jobs.Count > 0))
				{
					P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newChangelist.Id, null);
					try
					{
						newChangelist.FixJobs(jobs, fixOptions);
					}
					catch { } // ignore errors
				}
				if (newChangelist != null)
				{
					newChangeId = Scm.SubmitChangelist(newChangelist.Id, dlg.Description, dlg.SubmitOptions, dlg.CheckOutAfterSubmit);
				}
			}
			else
			{
				// Only some files have been selected

				if (changelist.Id > 0)
				{
					// checking in some files in a numbered changelist,
					// so create a move the files that have not been 
					// selected to the default changelist.

					IList<string> unselectedFiles = dlg.GetUnselectedFiles();
					Scm.MoveFilesToChangeList(0, null, unselectedFiles.ToArray());

					// submit the pared down changelist
					newChangelistId = Scm.SubmitChangelist(changelist.Id, dlg.Description, dlg.SubmitOptions, dlg.CheckOutAfterSubmit);
				}
				else
				{
					// submitting some files in the default changelist, so must submit them in a 
					//  new changelist

					P4.Changelist newChangelist = Scm.Connection.Repository.NewChangelist();
                    newChangelist.Description = dlg.Description;
					newChangelist.Files = dlg.SelectedFileList;

					newChangelist = Scm.SaveChangelist(newChangelist, null);

					jobs = dlg.SelectedJobList;
					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.None, newChangelist.Id, dlg.JobStatusOnSubmit);
						try
						{
							newChangelist.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}

					jobs = dlg.UnselectedJobList;
					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newChangelist.Id, null);
						try
						{
							newChangelist.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}

					jobs = dlg.DeletedJobs;
					if ((jobs != null) && (jobs.Count > 0))
					{
						P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newChangelist.Id, null);
						try
						{
							newChangelist.FixJobs(jobs, fixOptions);
						}
						catch { } // ignore errors
					}

					newChangeId = Scm.SubmitChangelist(newChangelist.Id, dlg.Description, dlg.SubmitOptions, dlg.CheckOutAfterSubmit);
				}
			}
			if ((changelist.Id <= 0) && (newChangelistId > 0))
			{
				// Attach jobs to the newly created changelist so there status is update on submit
                P4.Changelist newchange = Scm.Connection.Repository.GetChangelist(newChangelistId);
				jobs = dlg.SelectedJobList;

				if ((jobs != null) && (jobs.Count > 0))
				{
					P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.None, newchange.Id, dlg.JobStatusOnSubmit);
					try
					{
						newchange.FixJobs(jobs, fixOptions);
					}
					catch { } // ignore errors
				}

				jobs = dlg.UnselectedJobList;
				if ((jobs != null) && (jobs.Count > 0))
				{
					P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newchange.Id, null);
					try
					{
						newchange.FixJobs(jobs, fixOptions);
					}
					catch { } // ignore errors
				}

				jobs = dlg.DeletedJobs;
				if ((jobs != null) && (jobs.Count > 0))
				{
					P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, newchange.Id, null);
					try
					{
						newchange.FixJobs(jobs, fixOptions);
					}
					catch { } // ignore errors
				}
			}
			Scm.SccService.UpdateProjectGlyphs(dlg.GetSelectedFiles(), true);
            return true;
		}

		public enum SubmitOptionsIndex { LeaveUnchanged = 0, RevertUnchanged, SubmitAll };

		public static Dictionary<SubmitOptionsIndex, P4.SubmitType> SubmitOptionsByIndex =
			new Dictionary<SubmitOptionsIndex, P4.SubmitType>() { 
				{SubmitOptionsIndex.LeaveUnchanged, P4.SubmitType.LeaveUnchanged},
				{SubmitOptionsIndex.RevertUnchanged, P4.SubmitType.RevertUnchanged},
				{SubmitOptionsIndex.SubmitAll, P4.SubmitType.SubmitUnchanged}
			};

		public static Dictionary<P4.SubmitType, SubmitOptionsIndex> SubmitOptionsIndexByType =
			new Dictionary<P4.SubmitType, SubmitOptionsIndex>() { 
				{P4.SubmitType.LeaveUnchanged, SubmitOptionsIndex.LeaveUnchanged},
				{P4.SubmitType.RevertUnchanged, SubmitOptionsIndex.RevertUnchanged},
				{P4.SubmitType.SubmitUnchanged, SubmitOptionsIndex.SubmitAll}
			};

		public ImageList ButtonImages;
		public ImageList JobIcon;

		public SubmitDlg(P4ScmProvider scm)
		{
			PreferenceKey = "SubmitDlg";

			_scm = scm;

			InitializeComponent();
			this.Icon = Images.pending;
			this.ButtonImages = new System.Windows.Forms.ImageList(this.components);
			// 
			// ButtonImages
			// 
			ButtonImages.TransparentColor = System.Drawing.Color.White;
			ButtonImages.Images.Add("TriangleDownGray.png", Images.TriangleDownGray);
			ButtonImages.Images.Add("TriangleDownRed.png", Images.TriangleDownRed);
			ButtonImages.Images.Add("TriangleRightGray.png", Images.TriangleRightGray);
			ButtonImages.Images.Add("TriangleRightRed.png", Images.TriangleRightRed);

			this.JobIcon = new ImageList(components);
			// 
			// JobImage
			// 
			ButtonImages.TransparentColor = System.Drawing.Color.White;
			JobIcon.Images.Add("jobs_icon", Images.jobs_icon);

			//ShowJobListBtn.ImageList = ButtonImages;
			//ShowFileListBtn.ImageList = ButtonImages;
			//ShowDescriptionBtn.ImageList = ButtonImages;

			JobsListLV.SmallImageList = JobIcon;

			//ButtonTitles
			DescriptionPanel.ButtonText = Resources.SubmitDlg_DescriptionPanelBtnText;
			FileListPanel.ButtonText = Resources.SubmitDlg_FileListPanelBtnText;
			JobListPanel.ButtonText = Resources.SubmitDlg_JobListPanelBtnText;

            this.resolveMsgImg.Image = new Bitmap(Images.pending_resolve);

            this.ListImages = new System.Windows.Forms.ImageList(this.components);
			// 
			// ListImages
			// 
			ListImages.TransparentColor = System.Drawing.Color.Transparent;
			ListImages.Images.Add("noCheckBox.png", Images.noCheckBox);
			ListImages.Images.Add("CheckBox.png", Images.CheckBox);

			FileListLV.SmallImageList = ListImages;

			//FileListLV.ListViewItemSorter = (System.Collections.IComparer)new FileListViewSorter();

			//LayoutDialog();

			string DefaultJobStatus = Preferences.LocalSettings.GetString("SubmitDlg.JobStatusCB.Selected", "closed");

			IList<string> jobValues = JobStatusValues;
			if (jobValues != null)
			{
				int select = -1;
				JobStatusCB.Items.Clear();
				foreach (string s in jobValues)
				{
					int idx = JobStatusCB.Items.Add(s);
					if (s == DefaultJobStatus)
					{
						select = idx;
					}
				}
				JobStatusCB.SelectedIndex = select;
			}
			else
			{
				JobStatusCB.SelectedIndex = 2;
			}

            P4.Client client = scm.Connection.Repository.Connection.Client;

			//SubmitFilesCB.SelectedIndex = Preferences.LocalSettings.GetInt("SubmitDlg.SubmitFilesCB.SelectedIndex", 2);
			SubmitFilesCB.SelectedIndex = (int)SubmitOptionsIndexByType[client.SubmitOptions.SubmitType];

			//DetailsPanel.Collapsed = Preferences.LocalSettings.GetBool("DetailsPanel.Collapsed",false);
			DescriptionPanel.Collapsed = Preferences.LocalSettings.GetBool("DescriptionPanel.Collapsed", false);
			if (DescriptionPanel.Collapsed)
			{
				DescriptionPanel.LayoutPanel();
			}
			FileListPanel.Collapsed = Preferences.LocalSettings.GetBool("FilesPanel.Collapsed", false);
			if (FileListPanel.Collapsed)
			{
				FileListPanel.LayoutPanel();
			}
			JobListPanel.Collapsed = Preferences.LocalSettings.GetBool("JobsPanel.Collapsed", false);
			if (JobListPanel.Collapsed)
			{
				JobListPanel.LayoutPanel();
			}

			//CheckOutAfterCB.Checked = Preferences.LocalSettings.GetBool("SubmitDlg.CheckOutAfterCB.Checked", false);
			CheckOutAfterCB.Checked = client.SubmitOptions.Reopen;

			FileListLV.ColumnSorter = new FileListViewSorter();
			FileListLV.ColumnSorter.SortColumn = 0;
			FileListLV.ColumnSorter.Order = SortOrder.Ascending;
		}

		private static IList<string> _jobStatusValues = null;
		private static string _jobStatusServer = null;

		public IList<string> JobStatusValues
		{
			get
			{
                if ((_jobStatusValues != null) && (_jobStatusServer != null) && (_jobStatusServer == _scm.Connection.Port))
				{
					return _jobStatusValues;
				}
				// remember the server we read the job status list from, so 
				// that if the user logs into another server, we can detect 
				// that and read a new set of status values.

                _jobStatusServer = _scm.Connection.Port;

				IList<P4.Job> jobList = _scm.getJobs(P4.JobsCmdFlags.None, null, 1, null);

				if ((jobList == null) || (jobList.Count < 1))
					return null;

				string spec = string.Empty;
				if (jobList[0].ContainsKey("specdef"))
				{
					spec = jobList[0]["specdef"].ToString();
					spec = spec.TrimEnd(';');
				}
				string[] fields = spec.Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);

				//				Dictionary<string, string[]> FieldDetails = new Dictionary<string, string[]> ();
				foreach (string field in fields)
				{
					string[] details = field.Split(';');

					if (details[0].ToLower() == "status")
					{
						// status field def
						for (int idx = 1; idx < details.Length; idx++)
						{
							if (details[idx].StartsWith("val:"))
							{
								_jobStatusValues = details[idx].Substring(4).Split('/');

								if (_scm.ServerVersion >= Versions.V8_2)
								{
									_jobStatusValues = new List<string>(_jobStatusValues);

									_jobStatusValues.Insert(0, "same");
								}
								return _jobStatusValues;
							}
						}
					}
				}
				return _jobStatusValues;
			}
		}
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

		private int _changeListId;
		public int ChangeListId
		{
			get { return _changeListId; }
			set
			{
				_changeListId = value;

				FileListLV.Items.Clear();

				listItemMap = new Dictionary<string, ListViewItem>();
				//fileSpecMap = new Dictionary<string, P4.FileSpec>();
				fileMetaDataMap = new Dictionary<string, P4.FileMetaData>();

				IList<P4.FileSpec> fileList = null;
				IList<P4.FileMetaData> mdl = null;
				if (ChangeListId > 0)
				{
					ChangeList = _scm.Connection.Repository.GetChangelist(ChangeListId);

					P4.Options options = new P4.GetFileMetaDataCmdOptions(
						P4.GetFileMetadataCmdFlags.Opened,
						null, null, -1, null, ChangeListId.ToString(), null);
					string wsPath = string.Format("//{0}/...", _scm.Connection.Repository.Connection.Client.Name);
					// get files opened in default changelist
					mdl = _scm.Connection.Repository.GetFileMetaData(options, new P4.FileSpec(new P4.ClientPath(wsPath), null));
				}
				else
				{
					P4.Options options = new P4.GetFileMetaDataCmdOptions(
						P4.GetFileMetadataCmdFlags.Opened,
						null, null, -1, null, "default", null);
					string wsPath = string.Format("//{0}/...", _scm.Connection.Repository.Connection.Client.Name);
					// get files opened in default changelist
					mdl = _scm.Connection.Repository.GetFileMetaData(options, new P4.FileSpec(new P4.ClientPath(wsPath), null));
				}

				if ((mdl == null) || (mdl.Count <= 0))
				{
					_changeListId = -3;
					return;
				}
				fileList = new List<P4.FileSpec>();

				if (mdl != null)
				{
					foreach (P4.FileMetaData file in mdl)
					{
						P4.FileSpec fs = (P4.FileSpec)file;
						if (fs == null)
						{
							continue;
						}

						if (((fs.Version is P4.Revision) && (((P4.Revision)fs.Version).Rev == 0)) ||
							(file.Action == P4.FileAction.MoveAdd) ||
							(file.Action == P4.FileAction.MoveDelete) ||
							(file.Action == P4.FileAction.Add))
						{
							fs.Version = null;
						}
						fileList.Add(fs);

						ListViewItem item = new ListViewItem(string.Empty);// (idx.ToString()); idx++;
						item.Tag = file;
						item.SubItems.Add(file.DepotPath.GetFileName());
						item.SubItems.Add(file.DepotPath.GetDirectoryName());

						string resolvedStat = string.Empty;
						if (file.Resolved)
						{
							resolvedStat = Resources.P4FileTreeListViewItem_Resolved;
						}
						else if (file.Unresolved)
						{
							resolvedStat = Resources.P4FileTreeListViewItem_Unresolved;
						}
						item.SubItems.Add(resolvedStat); // resolve status

						item.SubItems.Add(System.IO.Path.GetExtension(file.DepotPath.GetFileName()));

						item.SubItems.Add(file.Action.ToString()); // pending action

						if (listItemMap.ContainsKey(file.DepotPath.Path.ToLower()))
						{
							continue;
						}

						listItemMap.Add(file.DepotPath.Path.ToLower(), item);
						FileListLV.Items.Add(item);
						fileMetaDataMap.Add(file.DepotPath.Path.ToLower(), file);
					}
				}
			}
		}

		public P4.Changelist ChangeList { get; private set; }

		// map from depot path to list view item
		private IDictionary<string, ListViewItem> listItemMap = null;

		//// map from depot path to file spec
		//private IDictionary<string, P4.FileSpec> fileSpecMap = null;

		// map from depot path to meta data
		private IDictionary<string, P4.FileMetaData> fileMetaDataMap = null;

        private void RefreshFileList(ListView.SelectedListViewItemCollection files)
        {
            foreach (ListViewItem file in files)
            {
                IList<P4.FileMetaData> mdl = new List<P4.FileMetaData>();
                mdl.Add(file.Tag as P4.FileMetaData);
                if (mdl != null)
                {
                    mdl = _scm.Connection.Repository.GetFileMetaData(null,
                    new FileSpec(new DepotPath(mdl[0].DepotPath.Path), null, null, null));
                    if (mdl != null)
                    {
                        file.Tag = mdl[0];
                        string resolvedStat = string.Empty;
                        if (mdl[0].Resolved)
                        {
                            resolvedStat = Resources.P4FileTreeListViewItem_Resolved;
                        }
                        else if (mdl[0].Unresolved)
                        {
                            resolvedStat = Resources.P4FileTreeListViewItem_Unresolved;
                        }
                        file.SubItems[3].Text = resolvedStat;
                        bool isChecked = file.Checked;
                        int index = file.Index;
                        FileListLV.Items.RemoveAt(index);
                        FileListLV.Items.Insert(index, file);
                        FileListLV.Items[file.Index].Checked = isChecked;
                    }
                }
            }
            
        }

		public IList<P4.FileSpec> FileList
		{
			get
			{
				List<P4.FileSpec> value = new List<P4.FileSpec>();

				foreach (ListViewItem item in FileListLV.Items)
				{
					value.Add(fileMetaDataMap[((P4.FileMetaData)item.Tag).DepotPath.Path.ToLower()]);
				}
				return value;
			}
			//set
			//{
			//    foreach (P4.FileMetaData file in value)
			//    {
			//        ListViewItem item = new ListViewItem(file.GetFileName());
			//        item.Tag = file;
			//        item.SubItems.Add(file.GetDirectoryName());
			//        string resolvedStat = string.Empty;
			//        if (file.Resolved)
			//        {
			//            resolvedStat = "Resolved";
			//        }
			//        if (file.Unresolved)
			//        {
			//            resolvedStat = "Unresolved";
			//        }
			//        item.SubItems.Add(resolvedStat);
			//        item.SubItems.Add(System.IO.Path.GetExtension(file.GetFileName()));
			//        item.SubItems.Add(file.Action.ToString());

			//        fileMetaDataMap.Add(file.DepotPath.Path, item);
			//        FileListLV.Items.Add(item);
			//    }
			//}
		}

		public bool AllFilesSelected
		{
			get
			{
				return FileListLV.Items.Count == FileListLV.CheckedItems.Count;
			}
		}
		private int _selectedFileCount = 0;
		public int SelectedFileCount
		{
			get
			{
				return _selectedFileCount;
			}
		}
		public IList<P4.FileMetaData> SelectedFileList
		{
			get
			{
				List<P4.FileMetaData> value = new List<P4.FileMetaData>();

				foreach (ListViewItem item in FileListLV.CheckedItems)
				{
					value.Add((P4.FileMetaData)item.Tag);
				}
				if (value.Count > 0)
				{
					return value;
				}
				return null;
			}
			set
			{
				try
				{
					if (FileListLV_ItemCheckedHandler != null)
					{
						this.FileListLV.ItemChecked -= FileListLV_ItemCheckedHandler;
					}
					_selectedFileCount = 0;
					//TODO find a better way to select all
					if (value == null)
					{
						_selectedFileCount = FileListLV.Items.Count;
						foreach (ListViewItem item in FileListLV.Items)
						{
							item.Checked = true;
							//item.ImageIndex = 1;
						}
						return;
					}

					foreach (ListViewItem item in FileListLV.Items)
					{
						item.Checked = false;
						item.ImageIndex = 0;
					}
					foreach (P4.FileMetaData file in value)
					{
						if (listItemMap.ContainsKey(file.DepotPath.Path.ToLower()))
						{
							ListViewItem item = listItemMap[file.DepotPath.Path.ToLower()];
							item.Checked = true;
							//item.ImageIndex = 1;
							_selectedFileCount++;
						}
					}
				}
				finally
				{
					if (FileListLV_ItemCheckedHandler != null)
					{
						this.FileListLV.ItemChecked += FileListLV_ItemCheckedHandler;
					}
					SetButtonState();
				}
			}
		}

		public IList<string> GetSelectedFiles()
		{
			List<string> value = new List<string>();

			foreach (ListViewItem item in FileListLV.CheckedItems)
			{
				value.Add(((P4.FileMetaData)item.Tag).LocalPath.Path);
			}
			if (value.Count > 0)
			{
				return value;
			}
			return null;
		}

		public IList<string> GetUnselectedFiles()
		{
			List<string> value = new List<string>();

			foreach (ListViewItem item in FileListLV.Items)
			{
                if (item.Checked==false)
                {
                    value.Add(((P4.FileMetaData)item.Tag).LocalPath.Path);
                }
			}
			if (value.Count > 0)
			{
				return value;
			}
			return null;
		}

		public SubmitOptionsIndex OnSubmit { get; private set; }

		public P4.ClientSubmitOptions SubmitOptions
		{
			get
			{
				switch (OnSubmit)
				{
					case SubmitOptionsIndex.LeaveUnchanged:
						return new P4.ClientSubmitOptions(false, P4.SubmitType.LeaveUnchanged);
					case SubmitOptionsIndex.RevertUnchanged:
						return new P4.ClientSubmitOptions(false, P4.SubmitType.RevertUnchanged);
					default:
					case SubmitOptionsIndex.SubmitAll:
						return new P4.ClientSubmitOptions(false, P4.SubmitType.SubmitUnchanged);
				}
			}
		}
		public bool CheckOutAfterSubmit
		{
			get { return CheckOutAfterCB.Checked; }
			set { CheckOutAfterCB.Checked = value; }
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
				return value;
			}
			set
			{
				JobsListLV.Items.Clear();
				if (value != null)
				{
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
						if (ChangeListId > 0)
						{
							item.Checked = true;
						}
						item.ImageKey = "jobs_icon";
						item.ImageIndex = 0;
						JobsListLV.Items.Add(item);
					}
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
				return value;
			}
		}

		public IList<P4.Job> DeletedJobs { get; private set; }

		#region Layout

		//private bool HideDescription = false;
		//private bool HideFileList = false;
		//private bool HideJobList = false;

		//private void LayoutDialog()
		//{
		//    if (this.DesignMode)
		//    {
		//        return;
		//    }
		//    this.SuspendLayout();

		//    int totalPanelHeight = this.ClientSize.Height - 40; // 40 is for bottom buttons
		//    int descriptionHeight = 40; // hidden size of a panel
		//    int fileListHeight = 40; // hidden size of a panel
		//    int jobListHeight = 40; // hidden size of a panel

		//    if ((HideDescription == false) && (HideFileList == false) && (HideJobList == false))
		//    {
		//        descriptionHeight = (totalPanelHeight * 3) / 10;
		//        fileListHeight = (totalPanelHeight * 4) / 10;
				//jobListHeight = (totalPanelHeight * 3) / 10;
		//    }
		//    else if ((HideDescription == false) && (HideFileList == false) && (HideJobList == true))
		//    {
		//        jobListHeight = 40;
		//        totalPanelHeight -= 40; //-40 for collapsed panel
		//        descriptionHeight = (totalPanelHeight * 4) / 10;
				//fileListHeight = (totalPanelHeight * 6) / 10;
		//        //jobListHeight = (totalPanelHeight * 3) / 10;
		//    }
		//    else if ((HideDescription == false) && (HideFileList == true) && (HideJobList == false))
		//    {
		//        fileListHeight = 40;
		//        totalPanelHeight -= 40; //-40 for collapsed panel
		//        descriptionHeight = totalPanelHeight / 2;
		//        //fileListHeight = (totalPanelHeight * 6) / 10;
		//        jobListHeight = totalPanelHeight / 2;
		//    }
		//    else if ((HideDescription == false) && (HideFileList == true) && (HideJobList == true))
		//    {
		//        fileListHeight = 40;
		//        jobListHeight = 40;
		//        descriptionHeight = totalPanelHeight - 80;
		//    }
		//    if ((HideDescription == true) && (HideFileList == false) && (HideJobList == false))
		//    {
		//        descriptionHeight = 40;
		//        totalPanelHeight -= 40; //-40 for collapsed panel
		//        fileListHeight = (totalPanelHeight * 6) / 10;
		//        jobListHeight = (totalPanelHeight * 4) / 10;
		//    }
		//    else if ((HideDescription == true) && (HideFileList == false) && (HideJobList == true))
		//    {
		//        descriptionHeight = 40;
		//        jobListHeight = 40;
		//        fileListHeight = totalPanelHeight - 80;
		//    }
		//    else if ((HideDescription == true) && (HideFileList == true) && (HideJobList == false))
		//    {
		//        descriptionHeight = 40;
		//        fileListHeight = 40;
		//        jobListHeight = totalPanelHeight - 80;
		//    }
		//    else if ((HideDescription == true) && (HideFileList == true) && (HideJobList == true))
		//    {
		//        descriptionHeight = 40;
		//        fileListHeight = 40;
		//        jobListHeight = 40;
		//    }

		//    DescritionPanel.Location = new Point(0, 0);
		//    DescritionPanel.Size = new System.Drawing.Size(this.ClientSize.Width, descriptionHeight);
		//    FileListPanel.Location = new Point(0, descriptionHeight);
		//    FileListPanel.Size = new System.Drawing.Size(this.ClientSize.Width, fileListHeight);
		//    JobListPanel.Location = new Point(0, descriptionHeight + fileListHeight);
		//    JobListPanel.Size = new System.Drawing.Size(this.ClientSize.Width, jobListHeight);

		//    DescriptionTB.Visible = !HideDescription;
		//    if (DescriptionTB.Text.Length > 0)
		//    {
		//        ShowDescriptionBtn.ForeColor = SystemColors.ControlText;
		//        ShowDescriptionBtn.ImageIndex = HideDescription ? 2 : 0;
		//    }
		//    else
		//    {
		//        ShowDescriptionBtn.ForeColor = Color.Red;
		//        ShowDescriptionBtn.ImageIndex = HideDescription ? 3 : 1;
		//    }

		//    FileListLV.Visible = !HideFileList;
		//    SelectAllFileListChk.Visible = !HideFileList;
		//    CheckOutAfterCB.Visible = !HideFileList;
		//    SubmitFilesCB.Visible = !HideFileList;
		//    OnSubmitLbl.Visible = !HideFileList;

		//    JobsListLV.Visible = !HideJobList;
		//    BrowseJobsBtn.Visible = !HideJobList;
		//    AddJobBtn.Visible = !HideJobList;
		//    JobTB.Visible = !HideJobList;
		//    JobLbl.Visible = !HideJobList;
		//    JobStatusLbl.Visible = !HideJobList;
		//    JobStatusCB.Visible = !HideJobList;

		//    this.ResumeLayout(false);
		//}

		//private void SubmitDlg_SizeChanged(object sender, EventArgs e)
		//{
		//    LayoutDialog();
		//}

		//private void SubmitDlg_Resize(object sender, EventArgs e)
		//{
		//    LayoutDialog();
		//}

		//private void ShowDescriptionBtn_Click(object sender, EventArgs e)
		//{
		//    HideDescription = !HideDescription;
		//    ShowDescriptionBtn.ImageIndex = HideDescription ? 2 : 0;
		//    LayoutDialog();
		//}

		//private void ShowFileListBtn_Click(object sender, EventArgs e)
		//{
		//    HideFileList = !HideFileList;
		//    ShowFileListBtn.ImageIndex = HideFileList ? 2 : 0;
		//    LayoutDialog();
		//}

		//private void ShowJobListBtn_Click(object sender, EventArgs e)
		//{
		//    HideJobList = !HideJobList;
		//    ShowJobListBtn.ImageIndex = HideJobList ? 2 : 0;
		//    LayoutDialog();
		//}
		#endregion

		private void DescriptionTB_TextChanged(object sender, EventArgs e)
		{
			DescriptionPanel.ShowAlert = ((DescriptionTB.Text.Length <= 0) ||
				(DescriptionTB.Text.Trim() == Resources.DefaultChangeListDescription));

			SetButtonState();
		}

		private void CancelBtn_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void SaveBtn_Click(object sender, EventArgs e)
		{
			P4.Changelist change = ChangeList;
			if (ChangeListId <= 0)
			{
				change = _scm.Connection.Repository.NewChangelist();
            }
			else if (change == null)
			{
				change = _scm.GetChangelist(ChangeListId);
			}
			change.Description = Description;
			change.Files = SelectedFileList;
			change.Jobs = new Dictionary<string, string>();
			foreach (P4.Job job in JobList)
			{
				change.Jobs[job.Id] = null;
			}
			change.OwnerName = _scm.Connection.Repository.Connection.UserName;

			P4.Changelist savedChange = null;
			savedChange = _scm.SaveChangelist(change, null);

			if (savedChange == null)
			{
				return;
			}
			IDictionary<int, object> fileMetaDataMap = new Dictionary<int, object>();

			fileMetaDataMap.Add(savedChange.Id, null);

			_scm.BroadcastChangelistUpdates(fileMetaDataMap, P4ScmProvider.ChangelistUpdateArgs.UpdateType.Add);
            IList<string> files = GetSelectedFiles();
            _scm.UpdateFiles(files, true);
            _scm.OnCacheFilesUpdated(files, true);
			DialogResult = DialogResult.Cancel;

			this.Close();
		}

		private void SubmitBtn_Click(object sender, EventArgs e)
		{
			Preferences.LocalSettings["SubmitDlg.JobStatusCB.Selected"] = JobStatusCB.Text;
			//Preferences.LocalSettings["SubmitDlg.SubmitFilesCB.SelectedIndex"] = SubmitFilesCB.SelectedIndex;
			//Preferences.LocalSettings["SubmitDlg.CheckOutAfterCB.Checked"] = CheckOutAfterCB.Checked;

			this.Close();
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
				string filter = string.Format("Job={0}", JobTB.Text.Trim());
				IList<P4.Job> shortList = _scm.getJobs(P4.JobsCmdFlags.None, filter, 1, null);
				if (shortList != null && shortList.Count == 1)
				{
					job = shortList[0];
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

		public string JobStatusOnSubmit
		{
			get { return JobStatusCB.Text; }
		}
		private void JobTB_TextChanged(object sender, EventArgs e)
		{
			AddJobBtn.Enabled = (JobTB.Text.Length > 0);
		}

		private void SubmitFilesCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnSubmit = (SubmitOptionsIndex)SubmitFilesCB.SelectedIndex;
		}

		private void JobListMnu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if ((string)e.ClickedItem.Tag == "DeleteJob")
			{
				IList<ListViewItem> toDelete = new List<ListViewItem>();
				foreach (ListViewItem item in JobsListLV.SelectedItems)
				{
					toDelete.Add(item);
				}
				foreach (ListViewItem item in toDelete)
				{
					if (DeletedJobs == null)
					{
						DeletedJobs = new List<P4.Job>();
					}
					DeletedJobs.Add((P4.Job)item.Tag);
					JobsListLV.Items.Remove(item);
				}
			}
		}

		private void SubmitDlg_Activated(object sender, EventArgs e)
		{
			DescriptionTB.SelectAll();
			DescriptionTB.Focus();
		}

		private void DescriptionTB_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode == Keys.A) && (ModifierKeys == Keys.Control))
			{
				DescriptionTB.SelectAll();

				e.SuppressKeyPress = true;
			}
		}

		private void JobListMnu_Opening(object sender, CancelEventArgs e)
		{
			JLM_DeleteJobMI.Visible = ((JobsListLV.SelectedItems != null) && (JobsListLV.SelectedItems.Count > 0));
		}

		private void FileListMenu_Opening(object sender, CancelEventArgs e)
		{
			e.Cancel = false;
			FLM_DiffVsHaveMI.Visible = false;
			FLM_ResolveMI.Visible = false;
			bool menusToShow = false;
			if ((FileListLV.SelectedItems != null) && (FileListLV.SelectedItems.Count > 0))
			{
				foreach (ListViewItem it in FileListLV.SelectedItems)
				{
					if (it.SubItems[3].Text == Resources.P4FileTreeListViewItem_Unresolved)
					{
						FLM_ResolveMI.Visible = true;
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
						FLM_DiffVsHaveMI.Visible = true;
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

		private void FileListMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			FileListMenu.Close();
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
						if (files != null && files.Count > 0)
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
								string file = fmd.LocalPath.Path;
								files.Add(file);
                                filesToRefresh.Add(file);
							}
						}
						if (files != null && files.Count > 0)
						{
                            ResolveFileDlg.ResolveFiles(_scm, files, false, null, true);

                            // update the files list in the submit dialog
                            RefreshFileList(FileListLV.SelectedItems);
                            _scm.SccService.UpdateProjectGlyphs(filesToRefresh, true);
                            _scm.SccService.ScmProvider.BroadcastChangelistUpdate(null,
                                new P4ScmProvider.ChangelistUpdateArgs(ChangeListId,
                                P4ScmProvider.ChangelistUpdateArgs.UpdateType.ContentUpdate));
                        }
						break;
					}
			}
		}

		System.Windows.Forms.ItemCheckedEventHandler FileListLV_ItemCheckedHandler = null;
		System.EventHandler SelectAllFileListChk_CheckedChangedHandler = null;

		bool inFileListLV_ItemChecked = false;

		private void FileListLV_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (inFileListLV_ItemChecked == true)
			{
				// don't recurse
				return;
			}
			inFileListLV_ItemChecked = true;
			try
			{
#if DEBUG_DB
				int i = 0;
				foreach (ListViewItem it in FileListLV.Items)
				{
					P4.FileMetaData m = it.Tag as P4.FileMetaData;
					string fp = m.DepotPath.Path;
					logger.Trace("Item {0}: {1}", i++, fp));
				}
#endif
				P4.FileMetaData fmd = (P4.FileMetaData)(e.Item.Tag);

				if ((_scm != null) && (fmd.MovedFile != null))
				{
					try
					{
						ListViewItem item = listItemMap[fmd.MovedFile.Path.ToLower()];
						if ((item != null) && (item.Checked = e.Item.Checked))
						{
							item.Checked = e.Item.Checked;
						}
					}
					catch { } // ignore errors
				}
			}
			finally
			{
				inFileListLV_ItemChecked = false;
			}

			// turn off the select all checked event, so it won't effect other controls
			SelectAllFileListChk.CheckedChanged -= SelectAllFileListChk_CheckedChangedHandler;
			try
			{
				if (e.Item.Checked == false)
				{
					// uncheck select all
					SelectAllFileListChk.Checked = false;
				}
				else
				{
					// if all checked (checked item count same as item count), check check all
					SelectAllFileListChk.Checked = (FileListLV.Items.Count == FileListLV.CheckedItems.Count);
				}
			}
			finally
			{
				// turn back on the select all checked event, so it won't effect other controls
				SelectAllFileListChk.CheckedChanged += SelectAllFileListChk_CheckedChangedHandler;
			}

			SetButtonState();
		}

		private void SelectAllFileListChk_CheckedChanged(object sender, EventArgs e)
		{
			// turn off the individual item check handler, so it doesn't try to change the
			// state of the check all check box
			FileListLV.ItemChecked -= FileListLV_ItemCheckedHandler;

			bool newState = SelectAllFileListChk.Checked;
			try
			{
				foreach (ListViewItem item in FileListLV.Items)
				{
					item.Checked = newState;
					//item.ImageIndex = newState ? 1 : 0;
				}
			}
			finally
			{
				// turn back on the individual item check handler
				FileListLV.ItemChecked += FileListLV_ItemCheckedHandler;
			}
			SetButtonState();
		}

        bool ResolveNeeded()
        {
            foreach(ListViewItem file in FileListLV.CheckedItems)
            {
                FileMetaData fmd =file.Tag as FileMetaData;
                if (fmd.Unresolved)
                {
                    return true;
                }
            }
            return false;
        }
		private void SetButtonState()
		{
            bool needsresolve = ResolveNeeded();
            if (needsresolve)
            {
                resolveWarningLbl.Enabled = true;
                resolveWarningLbl.Visible = true;
                resolveMsgImg.Enabled = true;
                resolveMsgImg.Visible = true;
                gridLayoutSubpanel1.Visible = true;
                gridLayoutPanel3.LayoutGrid();
            }
            else
            {
                resolveWarningLbl.Enabled = false;
                resolveWarningLbl.Visible = false;
                resolveMsgImg.Enabled = false;
                resolveMsgImg.Visible = false;
                gridLayoutSubpanel1.Visible = false;
                gridLayoutPanel3.LayoutGrid();
            }

            SubmitBtn.Enabled = (!needsresolve) && (FileListLV.CheckedItems.Count > 0) && (DescriptionTB.Text.Length > 0) &&
				(DescriptionTB.Text != Resources.DefaultChangeListDescription);
			SaveBtn.Enabled = (DescriptionTB.Text.Length > 0) &&
				(DescriptionTB.Text != Resources.DefaultChangeListDescription); ;
		}

		private void SubmitDlg_Shown(object sender, EventArgs e)
		{
			FileListLV_ItemCheckedHandler = new System.Windows.Forms.ItemCheckedEventHandler(this.FileListLV_ItemChecked);
			FileListLV.ItemChecked += FileListLV_ItemCheckedHandler;

			SelectAllFileListChk_CheckedChangedHandler = new EventHandler(SelectAllFileListChk_CheckedChanged);
			SelectAllFileListChk.CheckedChanged += SelectAllFileListChk_CheckedChangedHandler;

			SelectAllFileListChk.Checked = FileListLV.Items.Count == FileListLV.CheckedItems.Count;
		}

		private void SubmitDlg_FormClosing(object sender, FormClosingEventArgs e)
		{
			Preferences.LocalSettings["DescriptionPanel.Collapsed"] = DescriptionPanel.Collapsed;
			Preferences.LocalSettings["FilesPanel.Collapsed"] = FileListPanel.Collapsed;
			Preferences.LocalSettings["JobsPanel.Collapsed"] = JobListPanel.Collapsed;
		}

        private void gridLayoutPanel3_AfterLayoutGrid()
        {
            Point l = FileListLV.Location;
            l.X+=6;
            l.Y+=7;

            SelectAllFileListChk.Location = l;
        }

        private void FileListLV_Enter(object sender, EventArgs e)
        {
            if (FileListLV.Items !=null&&FileListLV.Items.Count>0)
            {
                if (FileListLV.SelectedItems==null || FileListLV.SelectedItems.Count<1)
                {
                    FileListLV.Items[0].Selected = true;
                }
            }
        }

        private void JobsListLV_Enter(object sender, EventArgs e)
        {
            if (JobsListLV.Items != null && JobsListLV.Items.Count > 0)
            {
                if (JobsListLV.SelectedItems == null || JobsListLV.SelectedItems.Count < 1)
                {
                    JobsListLV.Items[0].Selected = true;
                }
            }
        }
    }
}
