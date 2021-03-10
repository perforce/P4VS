using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Perforce.I18nControls;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class ChangeOwnerDlg : AutoSizeForm
	{
		public ChangeOwnerDlg(string sentUser, string sentWorkspace, string changeslist, P4ScmProvider scm)
		{
			PreferenceKey = "ChangeOwnerDlg";

			InitializeComponent();
			//gridControl1.InitializeGrid();

			_scm = scm;
            this.Icon = Images.icon_p4vs_16px;
			promptText = PromptLbl.Text;
			PromptLbl.Text = string.Format(promptText, changeslist);
			userTB.Text = sentUser;
			workspaceTB.Text = sentWorkspace;
			
		}

		private string promptText;

		private P4ScmProvider _scm { get; set; }

		public string user { get; set; }
		public string workspace { get; set; }

		public static IList<string> Show(string sentUser, string sentWorkspace, string changeslist, P4ScmProvider scm)
		{
			ChangeOwnerDlg dlg = new ChangeOwnerDlg(sentUser, sentWorkspace, changeslist, scm);

			if (dlg.ShowDialog() != DialogResult.Cancel)
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					IList<string> result = new List<string>();
					string user = dlg.userTB.Text;
					string workspace = dlg.workspaceTB.Text;
					result.Add(user);
					result.Add(workspace);
					return result;
				}
			}
			return null;
		}

		private void userBrowseBtn_Click(object sender, EventArgs e)
		{
			UsersBrowserDlg dlg = new UsersBrowserDlg(_scm, null);
			if (DialogResult.Cancel != dlg.ShowDialog())
			{
				userTB.Text = dlg.SelectedUser.Id.ToString();
			}
		}

		private void workspaceBrowseBtn_Click(object sender, EventArgs e)
		{
			WorkspacesBrowserDlg dlg = new WorkspacesBrowserDlg(_scm, "get_revision", null,null);

			if (DialogResult.Cancel != dlg.ShowDialog())
			{
                if (dlg.SelectedWorkspace != null)
                {
                    workspaceTB.Text = dlg.SelectedWorkspace.Name.ToString();
                }
			}
		}

		private void newOKBtn_Click(object sender, EventArgs e)
		{
			string user = userTB.Text;
			string workspace = workspaceTB.Text;
			//validate these both here
			bool validated = true;
			IList<string> users = new List<string>();
			users.Add(user);
			IList<P4.User> u = _scm.GetUsers(users, null);
			if (u == null)
			{
				string msg = string.Format(Resources.ChangeOwnerDlg_UserError, user);
				MessageBox.Show(msg);
				validated = false;
			}
			P4.ClientsCmdFlags flags= P4.ClientsCmdFlags.None;
			IList<P4.Client> c = _scm.getClients(flags, null, workspace, -1, null);
			if (c==null)
			{
				string msg = string.Format(Resources.ChangeOwnerDlg_UserError, user);
				MessageBox.Show(msg);
				validated = false;
			}
			if (validated == true)
			{
				this.DialogResult = DialogResult.OK;
			}
		}
	}
}
