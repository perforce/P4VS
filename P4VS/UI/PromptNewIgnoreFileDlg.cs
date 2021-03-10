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
	public partial class PromptNewIgnoreFileDlg : Form
	{
		public new static bool Show()
		{
			if (Preferences.LocalSettings.GetBool("PromptOnNewIgnoreFile", true))
			{
				PromptNewIgnoreFileDlg dlg = new PromptNewIgnoreFileDlg();

				return dlg.ShowDialog() == DialogResult.OK;
			}
			return true;
		}

		public PromptNewIgnoreFileDlg()
		{
			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
		}

		private void OkBtn_Click(object sender, EventArgs e)
		{
			Preferences.LocalSettings["PromptOnNewIgnoreFile"] = !BugMeNotChk.Checked;
		}
	}
}
