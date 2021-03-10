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
using Perforce.P4;
using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class DiffDlg : AutoSizeForm
	{
		private System.Windows.Forms.ImageList imageList1;

		public DiffDlg(IList<string> paths, string specifier, string value, P4ScmProvider scm)
		{
			PreferenceKey = "DiffDlg";

			_scm = scm;
			_paths = paths;

            InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;

			if (components == null)
			{
				components = new System.ComponentModel.Container();
			}
            // 
			// imageList1
			// 
			imageList1 = new System.Windows.Forms.ImageList(components);
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.Add("revision_icon_base.png", Images.revision_icon_base);
			this.imageList1.Images.Add("submitted_change_icon.png", Images.submitted_change_icon);
			this.imageList1.Images.Add("datetime.png", Images.datetime);
			this.imageList1.Images.Add("labels_icon.png", Images.labels_icon);
			this.imageList1.Images.Add("clients_icon.png", Images.clients_icon);
			this.imageList1.Images.Add("portrait.png", Images.portrait);
			this.imageList1.Images.Add("client_folder.png", Images.client_folder);
			this.imageList1.Images.Add("depot_folder.png", Images.depot_folder);

			latest1stRBFmt = latest1stRB.Text;
			have1stRBFmt = have1stRB.Text;
			latest2ndRBFmt = latest2ndRB.Text;
			have2ndRBFmt = have2ndRB.Text;

			// dialog called from workspace tool window
			if (specifier == "workspace")
			{
			}

            // dialog called from solution explorer or VS menu
            if (specifier == "revision" ||
                specifier == "explorer")
            {
                int latest = 0;
                int have = 0;

                // set up top path
                path1TB.Text = paths[0];
                P4.FileMetaData file1 = scm.GetFileMetaData(paths[0]);
                if (file1 != null)
                {
                    latest = file1.HeadRev;
                    have = file1.HaveRev;
                    latest1stRB.Text = string.Format(latest1stRBFmt, latest);
                    have1stRB.Text = string.Format(have1stRBFmt, have);
                    specifier1stCB.SelectedIndex = 0;
                    latest1stRB.Checked = true;
                    specifier1stCB.SelectedIndex = 0;
                    if ((string.IsNullOrEmpty(path1ValueTB.Text) == true ||
                        string.IsNullOrEmpty(path1ValueTB.Text) == true) &&
                        (specifier1stRB.Checked == true))
                    {
                        diffBtn.Enabled = false;
                    }
                    else
                    {
                        diffBtn.Enabled = true;
                    }

                    // set up bottom path
                    path2TB.Text = paths[0];
                    latest2ndRB.Text = string.Format(latest2ndRBFmt, latest);
                    have2ndRB.Text = string.Format(have2ndRBFmt, have);
                    specifier2ndCB.SelectedIndex = 0;
                    have2ndRB.Checked = true;
                    if ((string.IsNullOrEmpty(path2ValueTB.Text) == true ||
                        string.IsNullOrEmpty(path2ValueTB.Text) == true) &&
                        (specifier2ndRB.Checked == true))
                    {
                        diffBtn.Enabled = false;
                    }
                    else
                    {
                        diffBtn.Enabled = true;
                    }
                }
                else
                {
                    string msg = string.Format(Resources.DiffDlg_MissingFileError, paths[0]);
                    MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new ArgumentException(msg);
                }
            }
            // dialog called from history
            if (specifier == "history")
            {
                int latest = 0;
                int have = 0;

                // set up top path
                path1TB.Text = paths[0];
                P4.FileMetaData file1 = scm.GetFileMetaData(paths[0]);
                if (file1 != null)
                {
                    latest = file1.HeadRev;
                    have = file1.HaveRev;
                    latest1stRB.Text = string.Format(latest1stRBFmt, latest);
                    have1stRB.Text = string.Format(have1stRBFmt, have);
                    specifier1stCB.SelectedIndex = 0;
                    if (value == "local")
                    {
                        workspace1stRB.Checked = true;
                    }
                    else
                    {
                        specifier1stRB.Checked = true;
                        path1ValueTB.Text = value;
                    }
                    specifier1stCB.SelectedIndex = 0;
                    if ((string.IsNullOrEmpty(path1ValueTB.Text) == true ||
                        string.IsNullOrEmpty(path1ValueTB.Text) == true) &&
                        (specifier1stRB.Checked == true))
                    {
                        diffBtn.Enabled = false;
                    }
                    else
                    {
                        diffBtn.Enabled = true;
                    }

                    // set up bottom path
                    path2TB.Text = paths[0];
                    latest2ndRB.Text = string.Format(latest2ndRBFmt, latest);
                    have2ndRB.Text = string.Format(have2ndRBFmt, have);
                    specifier2ndCB.SelectedIndex = 0;
                    specifier2ndRB.Checked = true;
                    if ((string.IsNullOrEmpty(path2ValueTB.Text) == true ||
                        string.IsNullOrEmpty(path2ValueTB.Text) == true) &&
                        (specifier2ndRB.Checked == true))
                    {
                        diffBtn.Enabled = false;
                    }
                    else
                    {
                        diffBtn.Enabled = true;
                    }
                }
                else
                {
                    string msg = string.Format(Resources.DiffDlg_MissingFileError, paths[0]);
                    MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new ArgumentException(msg);
                }
            }

            // dialog called from submitted changelists tool window
            if (specifier == "changelist")
			{

				// set up top path
				path1TB.Text = string.Empty;
				specifier1stCB.SelectedIndex = 1;
				specifier1stRB.Checked = true;
				path1ValueTB.Text = value;
				diffBtn.Enabled = false;
				
				// set up bottom path
				path2TB.Text = string.Empty;
				specifier2ndCB.SelectedIndex = 0;
				specifier2ndRB.Checked = true;
				diffBtn.Enabled = false;
				
			}

			// dialog called from labels tool window
			if (specifier == "label")
			{
			}
		}

        bool fieldNotEmpty()
        {
            if (((string.IsNullOrWhiteSpace(path2TB.Text) == false &&
                string.IsNullOrWhiteSpace(path1TB.Text) == false) &&
                (specifier1stRB.Checked == false ||
                 string.IsNullOrWhiteSpace(path1ValueTB.Text) == false) &&
                (specifier2ndRB.Checked == false ||
                 string.IsNullOrWhiteSpace(path2ValueTB.Text) == false)))
            {
                return true;
            }
            return false;
        }

		string latest1stRBFmt;
		string have1stRBFmt;
		string latest2ndRBFmt;
		string have2ndRBFmt;

		public string path1 { get { return path1ValueTB.Text; } }
		public string path2 { get { return path2ValueTB.Text; } }

		IList<string> _paths = null;

		private P4ScmProvider _scm { get; set; }

		private P4.Changelist _changelistToAdd = null;

		private P4.Client _workspaceToAdd = null;

		//private P4.Label _labelToAdd = null;

		public static List<P4.FileSpec> Show(IList<string> paths, string specifier, string value, P4ScmProvider scm)
		{
			List<P4.FileSpec> files = new List<P4.FileSpec>();

			DiffDlg dlg = null;
			try
			{
				dlg = new DiffDlg(paths, specifier, value, scm);
			}
			catch 
			{ 
				return null;
			}
			if (dlg.ShowDialog() != DialogResult.Cancel)
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					// get the strings to diff
					string path1 = dlg.path1TB.Text;
					string path2 = dlg.path2TB.Text;

					// create the 2 filespecs
					P4.FileSpec spec1 = new P4.FileSpec();
					P4.FileSpec spec2 = new P4.FileSpec();
					if (path1.StartsWith("//"))
					{
						spec1.DepotPath = new P4.DepotPath(path1);
					}
					else
					{
						spec1.LocalPath = new P4.LocalPath(path1);
					}
					if (path2.StartsWith("//"))
					{
						spec2.DepotPath = new P4.DepotPath(path2);
					}
					else
					{
						spec2.LocalPath = new P4.LocalPath(path2);
					}

					// build spec one
					if (dlg.specifier1stRB.Checked == true)
					{

						// #revision
						if (dlg.specifier1stCB.SelectedIndex == 0)
						{
							if (dlg.path1ValueTB.Text == string.Empty)
							{
								spec1.Version = null;
							}
							else
							{
								try
								{
									int rev1 = Convert.ToInt32(dlg.path1ValueTB.Text);
									spec1.Version = new P4.Revision(rev1);
								}
								catch (Exception)
								{
									MessageBox.Show(string.Format(Resources.DiffDlg_InvalidRevisionSpecifier, dlg.path1ValueTB.Text), 
										Resources.PerforceSCM, MessageBoxButtons.OK);
									Show(paths, specifier, value, scm);
									return files;
								}
							}
						   
						}

						// @workspace
						if (dlg.specifier1stCB.SelectedIndex == 4)
						{
							if (dlg.path1ValueTB.Text == string.Empty)
							{
								spec1.Version = null;
							}
							else
							{
								string ws1 = dlg.path1ValueTB.Text;
								spec1.Version = new P4.ClientNameVersion(ws1);
							}
						}

						// @date/time
						if (dlg.specifier1stCB.SelectedIndex == 2)
						{
							DateTime dt1 = dlg.path1dateTimePicker.Value;
							spec1.Version = new P4.DateTimeVersion(dt1);
						}

						// @changelist
						if (dlg.specifier1stCB.SelectedIndex == 1)
						{
							if (dlg.path1ValueTB.Text == string.Empty)
							{
								spec1.Version = null;
							}
							else
							{
								try
								{
									int cl1 = Convert.ToInt32(dlg.path1ValueTB.Text);
								    spec1.Version = new P4.ChangelistIdVersion(cl1);
								}
								catch (Exception)
								{
									MessageBox.Show(string.Format(Resources.DiffDlg_InvalidChangelistSpecifier, dlg.path1ValueTB.Text), 
										Resources.PerforceSCM, MessageBoxButtons.OK);
									Show(paths, specifier, value, scm);
									return files;
								}
							}
						}

						// @label
						if (dlg.specifier1stCB.SelectedIndex == 3)
						{
							if (dlg.path1ValueTB.Text == string.Empty)
							{
								spec1.Version = null;
							}
							else
							{
								string lb1 = dlg.path1ValueTB.Text;
								spec1.Version = new P4.LabelNameVersion(lb1);
							}
						}

					}

					if (dlg.workspace1stRB.Checked == true)
					{
                      // if diffing with local path, get that from the file metadata
                      // and null out the spec's depot path
                        if (spec1.DepotPath != null)
                        {
                            P4.FileMetaData fmd = scm.GetFileMetaData(spec1.DepotPath.Path);
                            if (fmd != null)
                            {
                                spec1.LocalPath = fmd.LocalPath;
                                spec1.DepotPath = null;
                            }
                        }
					}

					if (dlg.latest1stRB.Checked == true)
					{
						spec1.Version = new P4.HeadRevision();
					}

					if (dlg.have1stRB.Checked == true)
					{
						spec1.Version = new P4.HaveRevision();
					}

					// build spec 2
					if (dlg.specifier2ndRB.Checked == true)
					{

						// #revision
						if (dlg.specifier2ndCB.SelectedIndex == 0)
						{
							if (dlg.path2ValueTB.Text == string.Empty)
							{
								spec2.Version = null;
							}
							else
							{
								try
								{
									int rev2 = Convert.ToInt32(dlg.path2ValueTB.Text);
									spec2.Version = new P4.Revision(rev2);
								}
								catch (Exception)
								{
									MessageBox.Show(string.Format(Resources.DiffDlg_InvalidRevisionSpecifier, dlg.path1ValueTB.Text),
										Resources.PerforceSCM, MessageBoxButtons.OK);
									Show(paths, specifier, value, scm);
									return files;
								}
							}
						}

						// @workspace
						if (dlg.specifier2ndCB.SelectedIndex == 4)
						{
							if (dlg.path2ValueTB.Text == string.Empty)
							{
								spec2.Version = null;
							}
							else
							{
								string ws2 = dlg.path2ValueTB.Text;
								spec2.Version = new P4.ClientNameVersion(ws2);

							}
						}

						// @date/time
						if (dlg.specifier2ndCB.SelectedIndex == 2)
						{
							DateTime dt2 = dlg.path2dateTimePicker.Value;
							spec2.Version = new P4.DateTimeVersion(dt2);
						}

						// @changelist
						if (dlg.specifier2ndCB.SelectedIndex == 1)
						{
							if (dlg.path2ValueTB.Text == string.Empty)
							{
								spec2.Version = null;
							}
							else
							{
								try
								{
									int cl2 = Convert.ToInt32(dlg.path2ValueTB.Text);
									spec2.Version = new P4.ChangelistIdVersion(cl2);
								}
								catch (Exception)
								{
									MessageBox.Show(string.Format(Resources.DiffDlg_InvalidChangelistSpecifier, dlg.path1ValueTB.Text),
										Resources.PerforceSCM, MessageBoxButtons.OK);
									Show(paths, specifier, value, scm);
									return files;
								}
							}
						}

						// @label
						if (dlg.specifier2ndCB.SelectedIndex == 3)
						{
							if (dlg.path2ValueTB.Text == string.Empty)
							{
								spec2.Version = null;
							}
							else
							{
								string lb2 = dlg.path2ValueTB.Text;
								spec2.Version = new P4.LabelNameVersion(lb2);
							}
						}

					}

					if (dlg.workspace2ndRB.Checked == true)
                    {
                        // if diffing with local path, get that from the file metadata
                        // and null out the spec's depot path
                        if (spec2.DepotPath!=null)
                        {
                            P4.FileMetaData fmd = scm.GetFileMetaData(spec2.DepotPath.Path);
                            if (fmd != null)
                            {
                                spec2.LocalPath = fmd.LocalPath;
                                spec2.DepotPath = null;
                            }
                        }
                    }

					if (dlg.latest2ndRB.Checked == true)
					{
						spec2.Version = new P4.HeadRevision();
					}

					if (dlg.have2ndRB.Checked == true)
					{
						spec2.Version = new P4.HaveRevision();
					}

					
					files.Add(spec1);
					files.Add(spec2);

					IList<P4.FileMetaData> checkForValid = scm.GetFileMetaData(files, null);

					if (checkForValid==null||(checkForValid!=null&&checkForValid.Count < 2))
					{
                        if (scm.Connection.Repository.Connection.LastResults.ErrorList != null)
						{
                            P4.P4ClientErrorList invalid = scm.Connection.Repository.Connection.LastResults.ErrorList;
							string message = string.Empty;
							foreach (P4ClientError error in invalid)
							{
								message = message + error.ErrorMessage;
							}
							MessageBox.Show(message);

							Show(paths, specifier, value, scm);
							return files;
						}
						
					}

					if (files != null)
					{
						scm.Diff2Files(files);
					}
					return files;
				}
			}
			return null;
		}

		// Path 1 controls
		private void Path1ValueTB_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode == Keys.Return) && (string.IsNullOrEmpty(path1ValueTB.Text) == false)
				&& (string.IsNullOrEmpty(path1ValueTB.Text)))
				diffBtn.PerformClick();
		}

        private void Path1ValueTB_TextChanged(object sender, EventArgs e)
        {
            diffBtn.Enabled = fieldNotEmpty();
            specifier1stRB.Checked = true;
        }

		//changing the dialog when the selected specifier changes
		private void specifier1stCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			specifier1stRB.Checked = true;

			//revision
			if (specifier1stCB.SelectedIndex == 0)
			{
				path1BrowseBtn.Enabled = false;
				path1BrowseBtn.Visible = false;
				path1RevLbl.Visible = true;
				path1RevLbl.Enabled = true;
				path2RevLbl.Text = Resources.DiffDlg_EnterRevisionNumber;
				path1ValueTB.Enabled = true;
				path1ValueTB.Visible = true;
				path1dateTimePicker.Visible = false;
				path1dateTimePicker.Enabled = false;
			}

			//workspace
			if (specifier1stCB.SelectedIndex == 4)
			{
				path1BrowseBtn.Enabled = true;
				path1BrowseBtn.Visible = true;
				path1RevLbl.Visible = false;
				path1RevLbl.Enabled = false;
				path1ValueTB.Enabled = true;
				path1ValueTB.Visible = true;
				path1dateTimePicker.Visible = false;
				path1dateTimePicker.Enabled = false;
			}

			//date/time
			if (specifier1stCB.SelectedIndex == 2)
			{
				//change textbox to datetime selector
				path1BrowseBtn.Enabled = false;
				path1BrowseBtn.Visible = false;
				path1RevLbl.Visible = false;
				path1RevLbl.Enabled = false;
				path1ValueTB.Enabled = false;
				path1ValueTB.Visible = false;
				path1dateTimePicker.Visible = true;
				path1dateTimePicker.Enabled = true;
			}

			//changelist
			if (specifier1stCB.SelectedIndex == 1)
			{
				path1BrowseBtn.Enabled = true;
				path1BrowseBtn.Visible = true;
				 path1RevLbl.Visible = false;
				path1RevLbl.Enabled = false;
				path1ValueTB.Enabled = true;
				path1ValueTB.Visible = true;
				path1dateTimePicker.Visible = false;
				path1dateTimePicker.Enabled = false;
			}

			//label
			if (specifier1stCB.SelectedIndex == 3)
			{
				path1BrowseBtn.Enabled = true;
				path1BrowseBtn.Visible = true;
				path1RevLbl.Visible = false;
				path1RevLbl.Enabled = false;
				path1ValueTB.Enabled = true;
				path1ValueTB.Visible = true;
				path1dateTimePicker.Visible = false;
				path1dateTimePicker.Enabled = false;
			}
           
                diffBtn.Enabled = fieldNotEmpty();
		}

		// Path 2 controls
		private void Path2ValueTB_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode == Keys.Return) && (string.IsNullOrEmpty(path2ValueTB.Text) == false)
				&& (string.IsNullOrEmpty(path2ValueTB.Text)))
				diffBtn.PerformClick();
		}

		private void Path2ValueTB_TextChanged(object sender, EventArgs e)
		{
                diffBtn.Enabled = fieldNotEmpty();
				specifier2ndRB.Checked = true;
		}

		//changing the dialog when the selected specifier changes
		private void specifier2ndCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			specifier2ndRB.Checked = true;

			//revision
			if (specifier2ndCB.SelectedIndex == 0)
			{
				path2BrowseBtn.Enabled = false;
				path2BrowseBtn.Visible = false;
				path2RevLbl.Visible = true;
				path2RevLbl.Enabled = true;
				path2ValueTB.Enabled = true;
				path2ValueTB.Visible = true;
				path2RevLbl.Text = Resources.DiffDlg_EnterRevisionNumber;
				path2dateTimePicker.Visible = false;
				path2dateTimePicker.Enabled = false;
			}

			//workspace
			if (specifier2ndCB.SelectedIndex == 4)
			{
				path2BrowseBtn.Enabled = true;
				path2BrowseBtn.Visible = true;
				path2RevLbl.Visible = false;
				path2RevLbl.Enabled = false;
				path2ValueTB.Enabled = true;
				path2ValueTB.Visible = true;
				path2dateTimePicker.Visible = false;
				path2dateTimePicker.Enabled = false;
			}

			//date/time
			if (specifier2ndCB.SelectedIndex == 2)
			{
				//change textbox to datetime selector
				path2BrowseBtn.Enabled = false;
				path2BrowseBtn.Visible = false;
				path2RevLbl.Visible = false;
				path2RevLbl.Enabled = false;
				path2ValueTB.Enabled = false;
				path2ValueTB.Visible = false;
				path2dateTimePicker.Visible = true;
				path2dateTimePicker.Enabled = true;
			}

			//changelist
			if (specifier2ndCB.SelectedIndex == 1)
			{
				path2BrowseBtn.Enabled = true;
				path2BrowseBtn.Visible = true;
				path2RevLbl.Visible = false;
				path2RevLbl.Enabled = false;
				path2ValueTB.Enabled = true;
				path2ValueTB.Visible = true;
				path2dateTimePicker.Visible = false;
				path2dateTimePicker.Enabled = false;
			}

			//label
			if (specifier2ndCB.SelectedIndex == 3)
			{
				path2BrowseBtn.Enabled = true;
				path2BrowseBtn.Visible = true;
				path2RevLbl.Visible = false;
				path2RevLbl.Enabled = false;
				path2ValueTB.Enabled = true;
				path2ValueTB.Visible = true;
				path2dateTimePicker.Visible = false;
				path2dateTimePicker.Enabled = false;
			}

            diffBtn.Enabled = fieldNotEmpty();
		}

		private void workspace1stRB_Click(object sender, EventArgs e)
		{
            diffBtn.Enabled = fieldNotEmpty();
		}

		private void latest1stRB_Click(object sender, EventArgs e)
        {
            diffBtn.Enabled = fieldNotEmpty();
        }

		private void have1stRB_Click(object sender, EventArgs e)
        {
            diffBtn.Enabled = fieldNotEmpty();
        }

		private void specifier1stRB_Click(object sender, EventArgs e)
        {
            diffBtn.Enabled = fieldNotEmpty();
        }

		private void workspace2ndRB_Click(object sender, EventArgs e)
        {
            diffBtn.Enabled = fieldNotEmpty();
        }

		private void latest2ndRB_Click(object sender, EventArgs e)
        {
            diffBtn.Enabled = fieldNotEmpty();
        }

		private void have2ndRB_Click(object sender, EventArgs e)
        {
            diffBtn.Enabled = fieldNotEmpty();
        }

		private void specifier2ndRB_Click(object sender, EventArgs e)
        {
            diffBtn.Enabled = fieldNotEmpty();
        }

		private void path1BrowseBtn_Click(object sender, EventArgs e)
		{
			if (specifier1stCB.SelectedIndex == 1) // changelist
			{
				string path = path1TB.Text;
				SubmittedChangelistsBrowserDlg dlg = new SubmittedChangelistsBrowserDlg(_scm,"diff_"+path);

				if (DialogResult.Cancel != dlg.ShowDialog())
				{
					_changelistToAdd = dlg.SelectedChangelist;
                    if (_changelistToAdd != null)
                    {
                        Text = _changelistToAdd.Id.ToString();
                    }
				}
			}

			if (specifier1stCB.SelectedIndex == 3) // label
			{
				LabelsBrowserDlg dlg = new LabelsBrowserDlg();
				if (DialogResult.OK == dlg.ShowDialog(this))
				{
					P4.Label label = dlg.Label;
					if (label != null)
					{
						path1ValueTB.Text = label.Id;
					}
				}
			}

			if (specifier1stCB.SelectedIndex == 4) // workspace
			{
				WorkspacesBrowserDlg dlg = new WorkspacesBrowserDlg(_scm, "diff_against", null,null);

				if (DialogResult.Cancel != dlg.ShowDialog())
				{
					_workspaceToAdd = dlg.SelectedWorkspace;

                    if (_workspaceToAdd != null)
                    {
                        path1ValueTB.Text = _workspaceToAdd.Name.ToString();
                    }
				}
			}
		}

		private void path2BrowseBtn_Click(object sender, EventArgs e)
		{
            if (specifier2ndCB.SelectedIndex == 1) // changelist
            {
                string path = path2TB.Text;
                SubmittedChangelistsBrowserDlg dlg = new SubmittedChangelistsBrowserDlg(_scm, "diff_" + path);

                if (DialogResult.Cancel != dlg.ShowDialog())
                {
                    _changelistToAdd = dlg.SelectedChangelist;
                    if (_changelistToAdd != null)
                    {
                        path2ValueTB.Text = _changelistToAdd.Id.ToString();
                    }
                }
            }

			if (specifier2ndCB.SelectedIndex == 3) // label
			{
				LabelsBrowserDlg dlg = new LabelsBrowserDlg();
				if (DialogResult.OK == dlg.ShowDialog(this))
				{
					P4.Label label = dlg.Label;
					if (label != null)
					{
						path2ValueTB.Text = label.Id;
					}
				}
			}

			if (specifier2ndCB.SelectedIndex == 4) // workspace
			{
				WorkspacesBrowserDlg dlg = new WorkspacesBrowserDlg(_scm, "diff_against",null,null);

				if (DialogResult.Cancel != dlg.ShowDialog())
				{
					_workspaceToAdd = dlg.SelectedWorkspace;
                    if (_workspaceToAdd != null)
                    {
                        path2ValueTB.Text = _workspaceToAdd.Name.ToString();
                    }
				}
			}

		}

		private void path1TB_TextChanged(object sender, EventArgs e)
		{
            diffBtn.Enabled = fieldNotEmpty();
		}

		private void path2TB_TextChanged(object sender, EventArgs e)
		{
            diffBtn.Enabled = fieldNotEmpty();
		}

	 }
}
