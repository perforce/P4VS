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
	public partial class JobsBrowserDlg : AutoSizeForm
	{
        public JobsBrowserDlg(P4ScmProvider scm)
        {
            Scm = scm;
            InitializeComponent();
            this.Icon = Images.job;
            jobsToolWindowControl1.Scm = scm;
            jobsToolWindowControl1.refreshJobsList();
            jobsToolWindowControl1.fromBrowser = true;
        }

	    private JobsBrowserDlg()
		{
			InitializeComponent();
            this.Icon = Images.job;
		}

		public P4.Job SelectedJob
		{
			get { return jobsToolWindowControl1.SelectedJob; }
			//private set; 
		}

		public IList<P4.Job> SelectedJobs
		{
			get { return jobsToolWindowControl1.SelectedJobs; }
			//private set; 
		}

		public P4ScmProvider Scm { get; private set; }
	}
}
