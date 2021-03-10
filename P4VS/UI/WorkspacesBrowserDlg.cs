using Perforce.P4Scm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.P4VS
{
	public partial class WorkspacesBrowserDlg : AutoSizeForm
	{
        public WorkspacesBrowserDlg(P4ScmProvider scm, string sender,
            string integAction, IList<P4.Client> streamsWorkspaces)
		{
            PreferenceKey = "WorkspacesBrowserDlg";
            Scm = scm;
			InitializeComponent();
            this.Icon = Images.workspace;
			workspaceToolWindowControl1.Scm = scm;
			if (sender.Contains("//"))
			{
                workspaceToolWindowControl1.integAction = integAction;
				workspaceToolWindowControl1.stream = sender;
                workspaceToolWindowControl1.streamsWorkspaces = streamsWorkspaces;
			}
			if (sender == "connection")
			{
                workspaceToolWindowControl1.sentOwner = scm.Connection.User;
				workspaceToolWindowControl1.hostOnly = true;
			}
			if (sender == "get_revision")
			{
                workspaceToolWindowControl1.sentOwner = scm.Connection.User;
				workspaceToolWindowControl1.hostOnly = false;
			}
			if (sender == "diff_against")
			{
                workspaceToolWindowControl1.sentOwner = scm.Connection.User;
				workspaceToolWindowControl1.hostOnly = false;
			}
		}

		private WorkspacesBrowserDlg()
		{
            PreferenceKey = "WorkspacesBrowserDlg";
            InitializeComponent();
            this.Icon = Images.workspace;
		}

	    public P4.Client SelectedWorkspace
		{
			get { return workspaceToolWindowControl1.SelectedWorkspace; }
			//private set; 
		}

		public P4ScmProvider Scm { get; private set; }

		public string stream { get; private set; }

        private void workspaceToolWindowControl1_TreeListViewDoubleClicked(object sender, MouseEventArgs e)
        {
            if (SelectedWorkspace != null)
            {
                AcceptButton.PerformClick();
            }
        }
	}
}
