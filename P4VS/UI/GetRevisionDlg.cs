using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Perforce.P4;
using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class GetRevisionDlg : AutoSizeForm
	{

		public GetRevisionDlg(IList<P4.FileSpec> files, string specifier, string value, P4ScmProvider scm, bool isSolutionIncluded)
		{
			PreferenceKey = "GetRevisionDlg";

			_scm = scm;
			_files = files;
			_isSolutionIncluded = isSolutionIncluded;

			InitializeComponent();

            this.Icon = Images.icon_p4vs_16px;
			// adding a header with header style set to none so that this
			// behaves as a single column list.
			ColumnHeader h = new ColumnHeader();
			h.Width = -2;
			filesListView.Columns.Add(h);
			IList<P4.FileMetaData> mdl = new List<P4.FileMetaData>();
            if (files != null)
            {
                foreach (FileSpec fs in files)
                {
                    // if LocalPath is null, it is likely that this was
                    // launched from somewhere only providing depot paths
                    // in that case, assume all LocalPaths for files will
                    // be null and break.
                    if (fs.LocalPath==null)
                    {
                        break;
                    }
                    if (fs.LocalPath.Path.EndsWith("..."))
                    {
                        mdl.Add(fs);
                    }
                    else
                    {
                        FileMetaData fmd = _scm.GetCachedFile(fs.LocalPath.Path);
                        if (fmd == null)
                        {
                            fmd = _scm.UpdateFileInCache(fs.LocalPath.Path, true);
                        }
                        // if it is still null, it is likely not in Perforce
                        if (fmd != null)
                        {
                            mdl.Add(fmd);
                        }
                    }
                }
                if (mdl.Count==0)
                {
                    mdl = scm.GetFileMetaData(files, null);
                }
            }
            IList<object> fields = new List<object>();
			fields.Add(P4FileTreeListViewItem.SubItemFlag.DepotFullPath);

			if ((mdl != null) && (mdl.Count > 0))
			{
				foreach (P4.FileMetaData file in mdl)
				{
					P4FileTreeListViewItem item = new P4FileTreeListViewItem(null, file, fields);
					P4FileTreeListViewItem dupe = (P4FileTreeListViewItem)filesListView.FindItemWithText(item.Text);
					if (dupe !=item)
					{
						item.Tag = file;
						item.FileData = file;

						filesListView.Items.Add(item);
					}
					
				}
			}
			
			// dialog called from workspace tool window
			if (specifier == "workspace")
			{
				specifierCB.SelectedIndex = 4;
				specifierRB.Checked = true;
				if (value != null)
				{
					ValueTB.Text = value;
				}
			}

			// dialog called from solution explorer or VS menu
			if (specifier == "revision" ||
				specifier == "explorer")
			{
				specifierCB.SelectedIndex = 0;
				getLatestRB.Checked = true;
			}

			// dialog called from submitted changelists tool window
			if (specifier == "changelist")
			{
				specifierCB.SelectedIndex = 1;
				specifierRB.Checked = true;
				changelistFilesChk.Enabled = true;
				if (value != null)
				{
					ValueTB.Text = value;
				}
			}

			// dialog called from labels tool window
			if (specifier == "label")
			{
				specifierCB.SelectedIndex = 3;
				specifierRB.Checked = true;
				removeChk.Enabled = true;
				if (value != null)
				{
					ValueTB.Text = value;
				}
			}
		}

		public string Value { get { return ValueTB.Text; } }

		IList<P4.FileSpec> _files = null;

		private P4ScmProvider _scm { get; set; }

		private P4.Changelist _changelistToAdd = null;

		//private P4.Client _workspaceToAdd = null;

		//private P4.Label _labelToAdd = null;

		private bool _isSolutionIncluded = false;

		public static string Show(IList<P4.FileSpec> files, string specifier, string value, P4ScmProvider scm, bool isSolutionIncluded)
		{
			GetRevisionDlg dlg = new GetRevisionDlg(files, specifier, value, scm, isSolutionIncluded);

			if (dlg.ShowDialog() != DialogResult.Cancel)
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					return dlg.Value;
				}
                if (dlg.DialogResult == DialogResult.Retry)
                {
                    Show(files,specifier,value,scm,isSolutionIncluded);
                }
			}
			return null;
		}

		private void ValueTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				newOKBtn.PerformClick();
		}

		private void ValueTB_TextChanged(object sender, EventArgs e)
		{
			specifierRB.Checked = true;
			if (string.IsNullOrEmpty(ValueTB.Text))
			{
				newOKBtn.Enabled = false;
			}
			else
			{
				newOKBtn.Enabled = true;
			}
		}

		//changing the dialog when the selected specifier changes
		private void specifierCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			specifierRB.Checked = true;

			//revision
			if (specifierCB.SelectedIndex == 0)
			{
				browseBtn.Enabled = false;
				browseBtn.Visible = false;
				enterRevLbl.Visible = true;
				enterRevLbl.Enabled = true;
				enterRevLbl.Text = Resources.DiffDlg_EnterRevisionNumber;
				changelistFilesChk.Enabled = false;
				removeChk.Enabled = false;
				ValueTB.Enabled = true;
				ValueTB.Visible = true;
				dateTimePicker.Visible = false;
				dateTimePicker.Enabled = false;
				if (string.IsNullOrEmpty(ValueTB.Text))
				{
					newOKBtn.Enabled = false;
				}
			}

			//workspace
			if (specifierCB.SelectedIndex == 4)
			{
				browseBtn.Enabled = true;
				browseBtn.Visible = true;
				changelistFilesChk.Enabled = false;
				removeChk.Enabled = false;
				enterRevLbl.Visible = false;
				enterRevLbl.Enabled = false;
				ValueTB.Enabled = true;
				ValueTB.Visible = true;
				dateTimePicker.Visible = false;
				dateTimePicker.Enabled = false;
				if (string.IsNullOrEmpty(ValueTB.Text))
				{
					newOKBtn.Enabled = false;
				}
			}

			//date/time
			if (specifierCB.SelectedIndex == 2)
			{
				//change textbox to datetime selector
				browseBtn.Enabled = false;
				browseBtn.Visible = false;
				changelistFilesChk.Enabled = false;
				removeChk.Enabled = false;
				enterRevLbl.Visible = false;
				enterRevLbl.Enabled = false;
				ValueTB.Enabled = false;
				ValueTB.Visible = false;
				dateTimePicker.Visible = true;
				dateTimePicker.Enabled = true;
				if (specifierRB.Checked==true)
				{
					newOKBtn.Enabled = true;
				}
			}

			//changelist
			if (specifierCB.SelectedIndex == 1)
			{
				browseBtn.Enabled = true;
				browseBtn.Visible = true;
				changelistFilesChk.Enabled = true;
				removeChk.Enabled = false;
				enterRevLbl.Visible = false;
				enterRevLbl.Enabled = false;
				ValueTB.Enabled = true;
				ValueTB.Visible = true;
				dateTimePicker.Visible = false;
				dateTimePicker.Enabled = false;
				if (string.IsNullOrEmpty(ValueTB.Text))
				{
					newOKBtn.Enabled = false;
				}
			}

			//label
			if (specifierCB.SelectedIndex == 3)
			{
				browseBtn.Enabled = true;
				browseBtn.Visible = true;
				removeChk.Enabled = true;
				changelistFilesChk.Enabled = false;
				enterRevLbl.Visible = false;
				enterRevLbl.Enabled = false;
				enterRevLbl.Text = Resources.DiffDlg_EnterLabelName;
				ValueTB.Enabled = true;
				ValueTB.Visible = true;
				dateTimePicker.Visible = false;
				dateTimePicker.Enabled = false;
				if (string.IsNullOrEmpty(ValueTB.Text))
				{
					newOKBtn.Enabled = false;
				}
			}

		}

		// Get Revision button click
		private void newOKBtn_Click(object sender, EventArgs e)
		{
			bool updateSolution = false;
			IVsSolution solution = null;
			string path = _scm.SolutionFile;

			IList<IVsHierarchy> projectsUpdating = new List<IVsHierarchy>();
			IList<string> projectsClosed = new List<string>();
			IList<Guid> projectsGuidsClosed = new List<Guid>();

			if (_isSolutionIncluded)
			{
				// Figure out a better test

				//updateSolution = _scm.SyncFile(
				//    new P4.SyncFilesCmdOptions(P4.SyncFilesCmdFlags.Preview, 1),
				//    path);

				updateSolution = true;
			}
			if (updateSolution)
			{
				if (DialogResult.Cancel == MessageBox.Show(
                    Resources.GetRevisionDlg_UpdatingSlnWarninig,
					Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
				{
					return;
				}
				solution = (IVsSolution)P4VsProvider.Instance.GetService(typeof(SVsSolution));

				P4VsProvider.Instance.SuppressConnection = true;

				if (VSConstants.S_OK != solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, null, 0))
				{
					MessageBox.Show(Resources.Error_ClosingSolution, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			else if (P4VsProvider.Instance.SccService.SelectedNodes != null)
			{
				bool needToAsk = true;
				foreach (VSITEMSELECTION pItem in P4VsProvider.Instance.SccService.SelectedNodes)
				{
					if ((pItem.itemid == VSConstants.VSITEMID_ROOT) && (P4VsProvider.Instance.SccService.IsProjectControlled(pItem.pHier)))
					{
#if VS2008
						if (DialogResult.Cancel == MessageBox.Show(Resources.GetRevisionDlg_UpdatingProjWarninig,
							Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
						{
							return;
						}
						updateSolution = true;

						solution = (IVsSolution)GetService(typeof(SVsSolution));

						P4VsProvider.Instance.SuppressConnection = true;

						if (VSConstants.S_OK != solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, null, 0))
						{
							MessageBox.Show(Resources.Error_ClosingSolution, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
							return;
						}
						break;
#else
						IVsProject3 pProj = pItem.pHier as IVsProject3;
						if (pProj != null)
						{
							if (needToAsk == true)
							{
								if (DialogResult.Cancel == MessageBox.Show(Resources.GetRevisionDlg_UpdatingProjWarninig,
								   Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
								{
									return;
								}
								needToAsk = false;
							}
							projectsUpdating.Add(pItem.pHier);

							if (solution == null)
							{
								solution = (IVsSolution)P4VsProvider.Instance.GetService(typeof(SVsSolution));
							}
							Guid projGuid;
							solution.GetGuidOfProject(pItem.pHier, out projGuid);
							projectsGuidsClosed.Add(projGuid);

							if (VSConstants.S_OK != solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, pItem.pHier, 0))
							{
								MessageBox.Show(Resources.Error_ClosingProject, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
								return;
							}
						}
#endif
					}
				}
			}
			List<string> refreshItems = new List<string>();

			try
			{
				//change this to a better global validator
				int test = 0;
				bool num = int.TryParse(ValueTB.Text, out test);
				if (specifierRB.Checked == true &&
					num == false &&
					specifierCB.SelectedIndex == 0 &&
					ValueTB.Text != "head" &&
					ValueTB.Text != "have" &&
					ValueTB.Text != "none"
					)
				{
					MessageBox.Show(string.Format(Resources.GetRevisionDlg_InvalidRevisionSpecifierWarninig, ValueTB.Text));
					ValueTB.Text = string.Empty;
					return;
				}
				////////
				_files = new List<P4.FileSpec>();

				foreach (ListViewItem item in filesListView.Items)
				{
					P4.FileSpec file = new P4.FileSpec();
					P4.FileMetaData fmd = new P4.FileMetaData();
					if (item.Text.EndsWith("/..."))
					{
						fmd.DepotPath = new P4.DepotPath(item.Text);
					}
					else
					{
						fmd = _scm.GetFileMetaData(item.Text);
					}

					if (fmd.LocalPath != null)
					{
						file.LocalPath = fmd.LocalPath;
					}
					if (fmd.ClientPath != null)
					{
						file.ClientPath = fmd.ClientPath;
					}
					if (fmd.DepotPath != null)
					{
						file.DepotPath = fmd.DepotPath;
					}
					_files.Add(file);
					refreshItems.Add(item.Text.ToString());
				}

				P4.SyncFilesCmdFlags flags = new P4.SyncFilesCmdFlags();
				flags = P4.SyncFilesCmdFlags.None;
				if (forceChk.Checked == true)
				{
					flags = P4.SyncFilesCmdFlags.Force;
				}

				P4.Options options = new P4.Options(flags, -1);

				// set up the list of paths with specifiers that we will sync to
				IList<P4.FileSpec> files = new List<P4.FileSpec>();
				//int idx = 0;

				if (specifierRB.Checked == true)
				{

					// #revision
					if (specifierCB.SelectedIndex == 0)
					{
						bool disconnectAfterSync = false;
						int number = 0;
						bool isnum = int.TryParse(Value, out number);
						if (isnum == true)
						{
							if (number == 0)
							{
								if (_isSolutionIncluded)
								{
									if (DialogResult.Cancel == MessageBox.Show(	Resources.GetRevisionDlg_RemovingSlnWarninig,
										Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
									{
										this.DialogResult = DialogResult.Cancel;
										return;
									}
									disconnectAfterSync = true;
								}
								else
								{
									if (DialogResult.Cancel == MessageBox.Show(Resources.GetRevisionDlg_RemovingProjWarninig,
										Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
									{
										this.DialogResult = DialogResult.Cancel;
										return;
									}
								}
							}
						}
						foreach (P4.FileSpec file in _files)
						{
							if (isnum == true)
                            {
								// skip meta data check for rev 0 there is none, but we dtill want to allow
								// this as a way to remove the local copy of the file, but still keep checking
								// to make sure other bad revisions are not fetched such as file that was moved
								if (number != 0)
								{
									IList<P4.FileSpec> fs = new List<FileSpec>();
									file.Version = new P4.Revision(number);
									fs.Add(file);
									IList<P4.FileMetaData> fmd = _scm.GetFileMetaData(fs, null);
									if (fmd == null)
									{
                                        if (_scm.Connection.Repository.Connection.LastResults.ErrorList != null)
										{
											if (_files.Count == 1)
											{
                                                P4ErrorDlg.Show(_scm.Connection.Repository.Connection.LastResults, false);
											}
                                            P4VsOutputWindow.AppendMessage(_scm.Connection.Repository.Connection.LastResults.ErrorList[0].ToString());
										}
										continue;
									}
								}
								file.Version = new P4.Revision(number);
							}
							else if (Value == "head")
							{
								file.Version = new P4.HeadRevision();
							}
							else if (Value == "have")
							{
								file.Version = new P4.HaveRevision();
							}
							else if (Value == "none")
							{
								file.Version = new P4.NoneRevision();
							}
							files.Add(file);
						}
                        if (files.Count>0)
                        {
                            _scm.SyncFiles(options, files);
                        }
						if (_scm.Connection.Repository.Connection.LastResults.ErrorList != null)
						{
							IList<P4.P4ClientError> errors = _scm.Connection.Repository.Connection.LastResults.ErrorList.ToList();
							foreach (P4.P4ClientError error in errors)
							{
								if (error.SeverityLevel == P4.ErrorSeverity.E_FAILED |
									error.SeverityLevel == P4.ErrorSeverity.E_FATAL)
								{
									P4ErrorDlg.Show(_scm.Connection.Repository.Connection.LastResults, false);
									return;
								}

							}
							if (_scm.SccService == null)
							{
								return;
							}
							this.DialogResult = DialogResult.OK;

						}
						else
						{
							if (_scm.SccService == null)
							{
								return;
							}
							this.DialogResult = DialogResult.OK;
						}
						if (disconnectAfterSync)
						{
							P4VsProvider.Instance.SccService.Dispose();
						}
					}

					// @workspace
					if (specifierCB.SelectedIndex == 4)
					{
						foreach (P4.FileSpec file in _files)
						{
							file.Version = new P4.ClientNameVersion(Value.ToString());
							files.Add(file);
						}
						_scm.SyncFiles(options, files);
						if (_scm.Connection.Repository.Connection.LastResults.ErrorList != null)
						{
							IList<P4.P4ClientError> errors = _scm.Connection.Repository.Connection.LastResults.ErrorList.ToList();
							foreach (P4.P4ClientError error in errors)
							{
								if (error.SeverityLevel == P4.ErrorSeverity.E_FAILED |
									error.SeverityLevel == P4.ErrorSeverity.E_FATAL)
								{
									P4ErrorDlg.Show(_scm.Connection.Repository.Connection.LastResults, false);
									return;
								}
							}
							this.DialogResult = DialogResult.OK;

						}
						else
						{
							this.DialogResult = DialogResult.OK;
						}
					}

					// @date/time
					if (specifierCB.SelectedIndex == 2)
					{
						DateTime value = dateTimePicker.Value;

						string timestamp = value.Date.ToShortDateString() + ":" + value.TimeOfDay.ToString();

						foreach (P4.FileSpec file in _files)
						{
							file.Version = new P4.DateTimeVersion(value);
							files.Add(file);
						}
						_scm.SyncFiles(options, _files);
						if (_scm.Connection.Repository.Connection.LastResults.ErrorList != null)
						{
							IList<P4.P4ClientError> errors = _scm.Connection.Repository.Connection.LastResults.ErrorList.ToList();
							foreach (P4.P4ClientError error in errors)
							{
								if (error.SeverityLevel == P4.ErrorSeverity.E_FAILED |
									error.SeverityLevel == P4.ErrorSeverity.E_FATAL)
								{
									P4ErrorDlg.Show(_scm.Connection.Repository.Connection.LastResults, false);
									return;
								}
							}
							this.DialogResult = DialogResult.OK;

						}
						else
						{
							this.DialogResult = DialogResult.OK;
						}
					}

					// @changelist
					if (specifierCB.SelectedIndex == 1)
					{
						if (changelistFilesChk.Checked == true)
						{
							foreach (P4.FileSpec file in _files)
							{
								file.Version = new P4.VersionRange(new P4.ChangelistIdVersion(Convert.ToInt16(Value)),
									new P4.ChangelistIdVersion(Convert.ToInt32(Value)));
								files.Add(file);
							}
						}
						else
						{
							foreach (P4.FileSpec file in _files)
							{
								file.Version = new P4.ChangelistIdVersion(Convert.ToInt32(Value));
								files.Add(file);
							}
						}
						_scm.SyncFiles(options, files);
						if (_scm.Connection.Repository.Connection.LastResults.ErrorList != null)
						{
							IList<P4.P4ClientError> errors = _scm.Connection.Repository.Connection.LastResults.ErrorList.ToList();
							foreach (P4.P4ClientError error in errors)
							{
								if (error.SeverityLevel == P4.ErrorSeverity.E_FAILED |
									error.SeverityLevel == P4.ErrorSeverity.E_FATAL)
								{
									P4ErrorDlg.Show(_scm.Connection.Repository.Connection.LastResults, false);
									return;
								}
							}
							this.DialogResult = DialogResult.OK;

						}
						else
						{
							this.DialogResult = DialogResult.OK;
						}
					}

					// @label
					if (specifierCB.SelectedIndex == 3)
					{
						if (removeChk.Checked == true)
						{
							foreach (P4.FileSpec file in _files)
							{
								file.Version = new P4.LabelNameVersion(Value.ToString());
								files.Add(file);
							}
						}
						else
						{
							foreach (P4.FileSpec file in _files)
							{
								file.Version = new P4.VersionRange(new P4.LabelNameVersion(Value.ToString()),
								new P4.LabelNameVersion(Value.ToString()));
								files.Add(file);
							}
						}
						_scm.SyncFiles(options, files);
						if (_scm.Connection.Repository.Connection.LastResults.ErrorList != null)
						{
							IList<P4.P4ClientError> errors = _scm.Connection.Repository.Connection.LastResults.ErrorList.ToList();
							foreach (P4.P4ClientError error in errors)
							{
								if (error.SeverityLevel == P4.ErrorSeverity.E_FAILED |
									error.SeverityLevel == P4.ErrorSeverity.E_FATAL)
								{
									P4ErrorDlg.Show(_scm.Connection.Repository.Connection.LastResults, false);
									return;
								}
							}
							this.DialogResult = DialogResult.OK;

						}
						else
						{
							this.DialogResult = DialogResult.OK;
						}
					}

				}

				else if (getLatestRB.Checked == true)
				{
					files = _files;
					_scm.SyncFiles(options, files);
					if (_scm.Connection.Repository.Connection.LastResults.ErrorList != null)
					{
						IList<P4.P4ClientError> errors = _scm.Connection.Repository.Connection.LastResults.ErrorList.ToList();
						foreach (P4.P4ClientError error in errors)
						{
							if (error.SeverityLevel == P4.ErrorSeverity.E_FAILED |
								error.SeverityLevel == P4.ErrorSeverity.E_FATAL)
							{
								P4ErrorDlg.Show(_scm.Connection.Repository.Connection.LastResults, false);
								return;
							}
						}
						this.DialogResult = DialogResult.OK;

					}
					else
					{
						this.DialogResult = DialogResult.OK;
					}
				}
			}
			finally
			{
			    if (updateSolution)
				{
				    if (path != null)
					{
						solution.OpenSolutionFile(0, path);
					}
				    if (_scm.SccService != null) _scm.SccService.ResetSelection();
				}
#if !VS2008
				else if (projectsUpdating.Count > 0)
				{
					foreach (Guid projGuid in projectsGuidsClosed)
					{
					    Guid rProjGuid = projGuid;

					    var vsSolution4 = (IVsSolution4) solution;
					    if (vsSolution4 != null)
					    {
					        int res = vsSolution4.ReloadProject(ref rProjGuid);
					    }
					}
				}
#endif
				// now refresh the selected nodes' glyphs
			    if (_scm.SccService != null) _scm.SccService.UpdateProjectGlyphs(refreshItems, true);
			}
		}

		private void filesListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (filesListView.SelectedItems.Count > 0)
			{
				removeBtn.Enabled = true;
			}
			else
			{
				removeBtn.Enabled = false;
			}
		}

		private void removeBtn_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in filesListView.SelectedItems)
			{
				item.Remove();
			}
			newOKBtn.Enabled = filesListView.Items.Count > 0;
		}

		private void addBtn_Click(object sender, EventArgs e)
		{
		    try
		    {
                DepotPathDlg dlg = new DepotPathDlg(_scm.SccService, Resources.GetRevisionDlg_AddFilesFoldersPrompt, false);
		   

			if (DialogResult.OK == dlg.ShowDialog())
			{
				string depotPath = null;
				if (dlg.SelectedFile != null)
				{
					depotPath = dlg.SelectedFile;
				}

				P4.FileMetaData f = new P4.FileMetaData();
				f.DepotPath = new P4.DepotPath(depotPath);
				IList<P4.FileMetaData> fmd = null;
				IList<object> fields = new List<object>();
				fields.Add(P4FileTreeListViewItem.SubItemFlag.DepotFullPath);
				P4FileTreeListViewItem lvi = new P4FileTreeListViewItem(null,f,fields);

                    if (depotPath != null && depotPath.EndsWith("/..."))
                    {
                        try
                        {
                            P4.Options options = new P4.Options();
                            //options["Op"] = null;
                            options["-m"] = "1";

                            fmd = _scm.GetFileMetaData(options, depotPath);
                        }
                        catch (P4Exception ex)
                        {
                            if (!(ex.ErrorCode == P4ClientError.MsgDb_NotUnderRoot))
                            {
                                MessageBox.Show(ex.Message, Resources.P4VS);
                                return;
                            }
                        }



                        //lvi.Text = depotPath;


                        foreach (ListViewItem item in filesListView.Items)
                        {
                            if (item.Text.Equals(lvi.Text))
                            {

                                return;
                            }
                        }

                        filesListView.Items.Add(lvi);

                    }

                    else
                    {
                        P4.FileMetaData file = null;
                        {
                            try
                            {
                                file = _scm.GetFileMetaData(depotPath);
                            }
                            catch (P4.P4Exception ex)
                            {
                                if ((ex.ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
                                    (ex.ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient))
                                {

                                }
                                else
                                {
                                    MessageBox.Show(ex.Message, Resources.P4VS);
                                    //LogExeption(ex);

                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, Resources.P4VS);
                                return;
                            }

                            if (file != null && file.LocalPath != null)
                            {
                                lvi.Text = file.LocalPath.Path;
                            }
                            else
                            {
                                if (file != null && file.DepotPath != null)
                                {
                                    lvi.Text = file.DepotPath.Path;
                                }
                            }


                            foreach (ListViewItem item in filesListView.Items)
                            {
                                if (item.Text.Equals(lvi.Text))
                                {

                                    return;
                                }
                            }

                            filesListView.Items.Add(lvi);

                        }
                    }
				filesListView.Refresh();

				newOKBtn.Enabled = filesListView.Items.Count > 0;
			}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.P4VS);
                this.DialogResult=DialogResult.Cancel;
            }
		}

		private void browseBtn_Click(object sender, EventArgs e)
		{
			if (specifierCB.SelectedIndex == 1) // changelist
			{
				SubmittedChangelistsBrowserDlg dlg = new SubmittedChangelistsBrowserDlg(_scm, "get_revision");

				if (DialogResult.Cancel != dlg.ShowDialog())
				{
					if (dlg.SelectedChangelist!=null)
					{
						_changelistToAdd = dlg.SelectedChangelist;
						ValueTB.Text = _changelistToAdd.Id.ToString();
					}
				}
			}

			if (specifierCB.SelectedIndex == 3) // label
			{
				LabelsBrowserDlg dlg = new LabelsBrowserDlg();
				if (DialogResult.OK == dlg.ShowDialog(this))
				{
					P4.Label label = dlg.Label;
					if (label != null)
					{
						ValueTB.Text = label.Id;
					}
				}
			}

			if (specifierCB.SelectedIndex == 4) // workspace
			{
				WorkspacesBrowserDlg dlg = new WorkspacesBrowserDlg(_scm, "get_revision",null,null);

				if (DialogResult.Cancel != dlg.ShowDialog())
				{
					if (dlg.SelectedWorkspace != null)
					{
						ValueTB.Text = dlg.SelectedWorkspace.Name.ToString();
					}
				}
			}
		}

		private void filesListView_Resize(object sender, EventArgs e)
		{
			filesListView.BeginUpdate();
            if (filesListView.Columns.Count > 0 &&
                filesListView.Columns[0] != null)
            {
                filesListView.Columns[0].Width = filesListView.Width;
            }
			filesListView.EndUpdate();
		}

		private void specifierRB_CheckedChanged(object sender, EventArgs e)
		{
			if (specifierRB.Checked == true)
			{
				if (string.IsNullOrEmpty(ValueTB.Text))
				{
					newOKBtn.Enabled = false;
				}
				else if (filesListView.Items.Count !=0)
				{
					newOKBtn.Enabled = true;
				}
				if (specifierCB.SelectedIndex == 1)
				{
					changelistFilesChk.Enabled = true;
				}
				if (specifierCB.SelectedIndex == 3)
				{
					removeChk.Enabled = true;
				}
				if (specifierCB.SelectedIndex == 2)
				{
					newOKBtn.Enabled = true;
				}
			}
		}

		private void ValueTB_EnabledChanged(object sender, EventArgs e)
		{
			if (filesListView.Items.Count < 1)
			{
				if (ValueTB.Enabled == false)
				{
					newOKBtn.Enabled = true;
				}
				if ((ValueTB.Enabled == true && string.IsNullOrEmpty(ValueTB.Text) == false) ||
					(specifierRB.Checked == false))
				{
					newOKBtn.Enabled = true;
				}
			}
		}

		private void filesListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (filesListView.Items.Count < 1)
			{
				newOKBtn.Enabled = false;
			}
			else
			{
				newOKBtn.Enabled = true;
			}
		}

		private void getLatestRB_CheckedChanged(object sender, EventArgs e)
		{
			if (getLatestRB.Checked == true && filesListView.Items.Count < 1)
			{
				newOKBtn.Enabled = false;
			}
			if (getLatestRB.Checked == true)
			{
				removeChk.Enabled = false;
				changelistFilesChk.Enabled = false;
				if (filesListView.Items.Count > 0)
				{
					newOKBtn.Enabled = true;
				}
			}
		}

		private void GetRevisionDlg_Load(object sender, EventArgs e)
		{
			newOKBtn.Enabled = true;
		}

        private void newOKBtn_EnabledChanged(object sender, EventArgs e)
        {
            if (filesListView.Items.Count<1)
            {
                newOKBtn.Enabled = false;
            }
            if ((specifierRB.Checked)&&(specifierCB.SelectedIndex!=2)&&(ValueTB.Text.Length<1))
            {
                newOKBtn.Enabled = false;
            }
        }

 
	}
}
