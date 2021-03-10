using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.P4VS
{
	public partial class LabelsToolWindowFilesControl : UserControl
	{
		public LabelsToolWindowFilesControl()
		{
			InitializeComponent();
		}

		public bool ShowWorkspaceFilesList
		{
			get { return !LabeledRevisionsRB.Checked; }
		}

		public LabelsToolWindowControl ParentControl { get; set; }

		public TreeListView LabeledRevisionsList
		{
			get { return LabeledRevisionsLV; }
		}

		public TreeListView WorkspaceFilesList
		{
			get { return WorkspaceFilesLV; }
		}
        bool inSwapViews = false;
        private void SwapViews()
        {
            if (inSwapViews)
            {
                return;
            }
            inSwapViews = true;
            try
            {
                LabeledRevisionsLV.Visible = LabeledRevisionsRB.Checked;
                WorkspaceFilesLV.Visible = !LabeledRevisionsRB.Checked;

                if (ParentControl != null)
                {
                    ParentControl.setFiles();
                }
            }
            finally
            {
                inSwapViews = false;
            }
        }
		private void LabeledRevisionsRB_CheckedChanged(object sender, EventArgs e)
		{
			SwapViews();
		}

		private void WorkspaceVersionRB_CheckedChanged(object sender, EventArgs e)
		{
			SwapViews();
		}

		public int maxItems;

		private void WorkspaceFilesLV_onMaxScroll(object sender, ScrollEventArgs e)
		{
			if ((maxItems > 0) && (WorkspaceFilesLV.Items.Count >= maxItems))
			{
				maxItems += (int)Preferences.LocalSettings.GetInt("Number_specs", 100);

				if (ParentControl != null)
				{
					ParentControl.setFiles();
				}
			}
		}

		private void LabeledRevisionsLV_onMaxScroll(object sender, ScrollEventArgs e)
		{
			if ((maxItems > 0) && (LabeledRevisionsLV.Items.Count >= maxItems))
			{
				maxItems += (int)Preferences.LocalSettings.GetInt("Number_specs", 100);

				if (ParentControl != null)
				{
					ParentControl.setFiles();
				}
			}
		}
	}
}
