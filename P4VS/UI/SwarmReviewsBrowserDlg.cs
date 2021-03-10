using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Perforce.SwarmApi;
using Perforce.P4;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class SwarmReviewsBrowserDlg : AutoSizeForm
	{
		public SwarmReviewsBrowserDlg(P4ScmProvider scm)
		{
			InitializeComponent();

			swarmReviewsToolWindowControl1.Scm = scm;

			swarmReviewsToolWindowControl1.ReviewsLVDoubleClicked += new EventHandler(swarmReviewsToolWindowControl1_ReviewsLV_DoubleClick);
		}

		public SwarmServer.Review SelectedReview
		{
			get { return swarmReviewsToolWindowControl1.SelectedReview; }
		}

		private void swarmReviewsToolWindowControl1_ReviewsLV_DoubleClick(object sender, EventArgs e)
		{
			if (SelectedReview != null)
			{
				AcceptButton.PerformClick();
			}
		}
	}
}
