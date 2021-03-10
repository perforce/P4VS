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
	public partial class RevertWarnDlg : AutoSizeForm
	{
		public bool revertWarn()
		{
			if (Preferences.LocalSettings.ContainsKey("Revert_warn"))
			{
				if ((bool)Preferences.LocalSettings["Revert_warn"] == true)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public RevertWarnDlg(List<string> preview)
		{
			if (revertWarn() == false)
			{
				this.DialogResult = DialogResult.OK;
				return;
			} 
			
			PreferenceKey = "RevertWarnDlg";

			InitializeComponent();

            this.Icon = Images.icon_p4vs_16px;
			foreach (string line in preview)
			{
				ErrorsTB.AppendText(line + "\r\n");
			}
		}

		public static DialogResult ShowDialog(List<string> preview)
		{
			if ((bool)Preferences.LocalSettings["Revert_warn"] == false)
			{
				return DialogResult.OK;
			}
			RevertWarnDlg dlg = new RevertWarnDlg(preview);
			dlg.Show();
			if (dlg.DialogResult == DialogResult.OK)
			{
				return DialogResult.OK;
			}

			return DialogResult.Cancel;
		}

	}

		
}
