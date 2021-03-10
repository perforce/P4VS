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
    public partial class QueryEditSyncWarningDlg : Form
    {
        private QueryEditSyncWarningDlg()
        {
            InitializeComponent();
        }

        public static DialogResult Show(string FileName)
        {
           if(Preferences.LocalSettings.GetBool("QueryEditNeverSync",false))
           {
               return DialogResult.No;
           }
           if (Preferences.LocalSettings.GetBool("QueryEditAlwaysSync", false))
           {
               return DialogResult.Yes;
           }
            
            QueryEditSyncWarningDlg dlg = new QueryEditSyncWarningDlg();

            string msg = string.Format(dlg.WarningLbl.Text, FileName);

            dlg.WarningLbl.Text = msg;

            return dlg.ShowDialog();       
        }

		private bool _inCheckChanged = false;

        private void NeverSyncCB_CheckedChanged(object sender, EventArgs e)
        {
			if (_inCheckChanged)
			{
				// inside another invocation
				return;
			}
			try
			{
				_inCheckChanged = true;
				if (NeverSyncCB.Checked)
				{
					AlwaysSyncCB.Checked = false;
					YesBtn.Enabled = false;
					NoBtn.Enabled = true;
				}
				else
				{
                    YesBtn.Enabled = true;
                    NoBtn.Enabled = true;
                }
			}
			finally
			{
				_inCheckChanged = false;
			}
        }

		private void AlwaysSyncCB_CheckedChanged(object sender, EventArgs e)
		{
			if (_inCheckChanged)
			{
				// inside another invocation
				return;
			}
			try
			{
				_inCheckChanged = true;
				if (AlwaysSyncCB.Checked)
				{
					NeverSyncCB.Checked = false;
					YesBtn.Enabled = true;
					NoBtn.Enabled = false;
				}
				else
				{
                    YesBtn.Enabled = true;
                    NoBtn.Enabled = true;
                }
			}
			finally
			{
				_inCheckChanged = false;
			}
		}

        private void YesBtn_Click(object sender, EventArgs e)
        {
            Preferences.LocalSettings["QueryEditAlwaysSync"] = AlwaysSyncCB.Checked;
            Preferences.LocalSettings["QueryEditNeverSync"] = NeverSyncCB.Checked;

            DialogResult = DialogResult.Yes;
            Close();
        }
        private void NoBtn_Click(object sender, EventArgs e)
        {
            Preferences.LocalSettings["QueryEditAlwaysSync"] = AlwaysSyncCB.Checked;
            Preferences.LocalSettings["QueryEditNeverSync"] = NeverSyncCB.Checked;

			DialogResult = DialogResult.No;
            Close();
        }
    }
}
