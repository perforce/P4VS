using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using Perforce.P4Scm;


namespace Perforce.P4VS
{
	public partial class DlgEditWorkspace : AutoSizeForm
	{
		public DlgEditWorkspace()
		{
			PreferenceKey = "DlgEditWorkspace";

			InitializeComponent();
			this.Icon = Images.workspace;
		}

		public static P4.Client EditWorkspace(P4ScmProvider scm, string workspaceName)
		{
			P4.Client workspace = scm.getClient(workspaceName, null);
			return EditWorkspace(scm, workspace);
		}

		public static P4.Client EditWorkspace(P4ScmProvider scm, string workspaceName, P4.Options options)
		{
			P4.Client workspace = scm.getClient(workspaceName, options);
			return EditWorkspace(scm, workspace);
		}

		public static P4.Client EditWorkspace(P4ScmProvider scm, P4.Client workspace)
		{
            P4.Client updatedClient = null;
            while (updatedClient == null)
            {
                P4.Client editedWorkspace = DlgEditWorkspace.Show(scm, workspace);
                if (editedWorkspace != null)
                {
                    try
                    {
                        updatedClient = scm.createClient(workspace, null);
                        // if the current client is being updated, update the connection with the changed client

                        if (updatedClient != null)
                        {
                            if ((scm != null) && (scm.Connection.Repository != null) &&
                                (scm.Connection.Repository.Connection != null))
                            {
                                if ((scm.Connection.Repository.Connection.Client == null) ||
                                    (updatedClient.Name == scm.Connection.Repository.Connection.Client.Name))
                                {
                                    scm.Connection.Repository.Connection.Client = updatedClient;
                                }
                            }
                            return updatedClient;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Resources.PerforceSCM, MessageBoxButtons.OK);
                    }
                }
                else
                {
                    updatedClient = new P4.Client();
                    return null;
                }
            }
            return updatedClient;
        }
		public static P4.Client Show(P4ScmProvider scm, P4.Client workspace)
		{
			if (workspace==null)
			{
				return null;
			}
			DlgEditWorkspace dlg = new DlgEditWorkspace();
			if (dlg.DialogResult==DialogResult.Cancel)
			{
				return null;
			}

            dlg.Text = string.Format(Resources.DlgEditWorkspace_Title, workspace.Name, scm.Connection.Port, scm.Connection.User);
			

			dlg.workspaceFormNameTB.Text = workspace.Name;

			if (Preferences.LocalSettings.GetBool("P4Date_format", true))
			{
				dlg.workspaceUpdatedFormTB.Text = workspace.Updated.ToString("yyyy/MM/dd HH:mm:ss");
				dlg.workspaceLastAccessedFormTB.Text = workspace.Accessed.ToString("yyyy/MM/dd HH:mm:ss");
			}
			else
			{
				dlg.workspaceUpdatedFormTB.Text = workspace.Updated.ToString();
				dlg.workspaceLastAccessedFormTB.Text = workspace.Accessed.ToString();
			}
			if (workspace.Updated == DateTime.MinValue)
			{
				dlg.workspaceUpdatedFormTB.Text = string.Empty;
			}
			if (workspace.Accessed == DateTime.MinValue)
			{
				dlg.workspaceLastAccessedFormTB.Text = string.Empty;
			}
			dlg.workspaceOwnerNameFormTB.Text = workspace.OwnerName;
			if (workspace.Description != null)
			{
				dlg.workspaceDescriptionFormRTB.Text = workspace.Description;
			}
			dlg.workspaceRootFormTB.Text = workspace.Root;
			if (workspace.AltRoots != null)
			{
				foreach (string altRoot in workspace.AltRoots)
				{
					dlg.workspaceAltRootsFormRTB.AppendText(altRoot +"\r\n");
				}
			}
			if (workspace.Host != null)
			{
				dlg.hostFormTB.Text = workspace.Host;
			}

			string selectedOpts = workspace.SubmitOptions;
			dlg.submitOptionsFormCB.Text = selectedOpts;
			string selectedLineEnd = workspace.LineEnd.ToString();
			dlg.lineEndingsFormCB.Text = selectedLineEnd;

			dlg.workspaceStreamRootFormTB.Enabled = false;
			dlg.workspacesStreamAtChangeFormTB.Enabled = false;
			dlg.workspacesServerIDFormTB.Enabled = false;

			// try to populate the spec with existing entries

			if (workspace.Stream != null)
			{
				dlg.workspaceStreamRootFormTB.Text = workspace.Stream;
				dlg.workspaceStreamRootFormTB.Enabled = true;
			}

			if (workspace.StreamAtChange != null)
			{
				dlg.workspacesStreamAtChangeFormTB.Text = workspace.StreamAtChange;
				dlg.workspacesStreamAtChangeFormTB.Enabled = true;
			}

			if (workspace.ServerID != null)
			{
				dlg.workspacesServerIDFormTB.Text = workspace.ServerID;
				dlg.workspacesServerIDFormTB.Enabled = true;
			}


			List<KeyValuePair<string, string>> clientSpec = scm.GetClientSpec();

			if (clientSpec!=null)
			{
			   foreach(var pair in clientSpec)
				{
					if (pair.Key.Contains("Values2"))
					{
						dlg.lineEndingsFormCB.Items.Clear();
						string lineEnds = pair.Value;
						lineEnds = lineEnds.Remove(0, lineEnds.LastIndexOf(" ") + 1);
						string[] lineEndsArray = lineEnds.Split('/');
						foreach (string lineEnd in lineEndsArray)
						{
							dlg.lineEndingsFormCB.Items.Add(lineEnd);
						}
						selectedLineEnd = workspace.LineEnd.ToString();
						dlg.lineEndingsFormCB.Text = selectedLineEnd;

					}
					if (pair.Value.Contains("Stream line 64 optional"))
					{
						dlg.workspaceStreamRootFormTB.Text = workspace.Stream;
						dlg.workspaceStreamRootFormTB.Enabled = true;
					}
					
					if (pair.Value.Contains("StreamAtChange line 64 optional"))
					{
						dlg.workspacesStreamAtChangeFormTB.Text = workspace.StreamAtChange;
						dlg.workspacesStreamAtChangeFormTB.Enabled = true;
					}
					
					if (pair.Value.Contains("ServerID line 64 always"))
					{
						dlg.workspacesServerIDFormTB.Text = workspace.ServerID;
						dlg.workspacesServerIDFormTB.Enabled = true;
					}
					
				}
				
			}
			
						
			if (workspace.ViewMap != null)
			{
				dlg.workspaceViewMapFormRTB.Text = workspace.ViewMap.ToString();
			}
			dlg.allwriteFormChk.Checked = ((workspace.Options & P4.ClientOption.AllWrite) != 0);
			dlg.clobberFormChk.Checked = ((workspace.Options & P4.ClientOption.Clobber) != 0);
			dlg.compressFormChk.Checked = ((workspace.Options & P4.ClientOption.Compress) != 0);
			dlg.lockedFormChk.Checked = ((workspace.Options & P4.ClientOption.Locked) != 0);
			dlg.modtimeFormChk.Checked = ((workspace.Options & P4.ClientOption.ModTime) != 0);
			dlg.rmdirFormChk.Checked = ((workspace.Options & P4.ClientOption.RmDir) != 0);

			if (dlg.ShowDialog() != DialogResult.Cancel)
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					workspace.Name =dlg.workspaceFormNameTB.Text;
					//workspace.Updated.ToShortDateString() = dlg.workspaceUpdatedFormTB.Text;
					//workspace.Accessed.ToShortDateString() = dlg.workspaceLastAccessedFormTB.Text;
					workspace.OwnerName = dlg.workspaceOwnerNameFormTB.Text;

					workspace.Description = dlg.workspaceDescriptionFormRTB.Text;
					
					workspace.Root = dlg.workspaceRootFormTB.Text;

					string[] alt = Regex.Split(dlg.workspaceAltRootsFormRTB.Text, "\r\n");

					workspace.AltRoots =	alt.ToList();
				
					workspace.Host = dlg.hostFormTB.Text;

					workspace.SubmitOptions = dlg.submitOptionsFormCB.Text;

					workspace.LineEnd = (P4.LineEnd)Enum.Parse(typeof(P4.LineEnd), dlg.lineEndingsFormCB.Text, true);

					string mapLines = dlg.workspaceViewMapFormRTB.Text.Trim();

					List<string> map = new List<string>(mapLines.Split('\n'));

					if (clientSpec != null)
						foreach (var pair in clientSpec)
						{
							if (pair.Value.Contains("Stream line 64 optional"))
							{
								workspace.Stream = dlg.workspaceStreamRootFormTB.Text;
							}

							if (pair.Value.Contains("StreamAtChange line 64 optional"))
							{
								workspace.StreamAtChange = dlg.workspacesStreamAtChangeFormTB.Text;
							}

							if (pair.Value.Contains("ServerID line 64 always"))
							{
								workspace.ServerID = dlg.workspacesServerIDFormTB.Text;
							}

						}

					P4.ViewMap vm = new P4.ViewMap(map);

					workspace.ViewMap = vm;

					workspace.Options = P4.ClientOption.None;
					
					if (dlg.allwriteFormChk.Checked)
					workspace.Options |= P4.ClientOption.AllWrite;

					if (dlg.clobberFormChk.Checked)
					workspace.Options |= P4.ClientOption.Clobber;

					if (dlg.compressFormChk.Checked)
					workspace.Options |= P4.ClientOption.Compress;

					if (dlg.lockedFormChk.Checked)
					workspace.Options |= P4.ClientOption.Locked;

					if (dlg.modtimeFormChk.Checked)
					workspace.Options |= P4.ClientOption.ModTime;

					if (dlg.rmdirFormChk.Checked)
					workspace.Options |= P4.ClientOption.RmDir;
					
					return workspace;
				}

				if (dlg.DialogResult == DialogResult.OK)
				{
					
					return workspace;
				}
			}
			return null;

		}

		private void workspaceFormBrowseBtn_Click(object sender, EventArgs e)
		{
			string selectedFolder;
			FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
			

			openFolderDialog.RootFolder =Environment.SpecialFolder.MyComputer;
			

			if (openFolderDialog.ShowDialog() == DialogResult.OK)
			{
				if ((selectedFolder = openFolderDialog.SelectedPath) != null)
				{
					workspaceRootFormTB.Text = selectedFolder;
					
				}
			}

		}

		private void hostFormTB_KeyDown(object sender, KeyEventArgs e)
		{
				if (e.KeyCode == Keys.Return)
					workspaceFormOKBtn.PerformClick();
		}

		private void submitOptionsFormTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				workspaceFormOKBtn.PerformClick();
		}

		private void lineEndingsFormTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				workspaceFormOKBtn.PerformClick();
		}

		private void workspaceRootFormTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				workspaceFormOKBtn.PerformClick();
		}

		private void workspaceStreamRootFormTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				workspaceFormOKBtn.PerformClick();
		}

		private void workspaceStreamRootFormTB_TextChanged(object sender, EventArgs e)
		{
			if (workspaceStreamRootFormTB.Text.Length > 0)
			{
				workspaceViewMapFormRTB.ReadOnly = true;
				workspaceViewMapFormRTB.Enabled = false;
			}
			else
			{
				workspaceViewMapFormRTB.ReadOnly = false;
				workspaceViewMapFormRTB.Enabled = true;
			}
		}
	}
}
