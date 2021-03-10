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
	public partial class SubmittedChangelistsBrowserDlg : AutoSizeForm
	{
		public SubmittedChangelistsBrowserDlg(P4ScmProvider scm, string sender)
		{
			Scm = scm;
			InitializeComponent();
            this.Icon = Images.submitted;
            submittedChangelistsToolWindowControl1.Scm = scm;
			submittedChangelistsToolWindowControl1.filterBtn.Enabled=true;
			submittedChangelistsToolWindowControl1.fromBrowser = true;
			if (sender.Contains("diff_"))
			{
				string path = sender.Replace("diff_", "");
				submittedChangelistsToolWindowControl1.pathCB.Text = path;
			}
		}
	

		private SubmittedChangelistsBrowserDlg()
		{
			InitializeComponent();
            this.Icon = Images.submitted;
        }

		public P4.Changelist SelectedChangelist
		{
			get { return submittedChangelistsToolWindowControl1.SelectedChangelist; }
			//private set; 
		}

		public P4ScmProvider Scm { get; private set; }

        private void submittedChangelistsToolWindowControl1_TreeListViewDoubleClicked(object sender, MouseEventArgs e)
        {
            if (SelectedChangelist != null)
            {
                AcceptButton.PerformClick();
            }
        }

	}

   
}

