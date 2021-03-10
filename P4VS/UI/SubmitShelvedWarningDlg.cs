using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Perforce.P4VS
{
	public partial class SubmitShelvedWarningDlg : AutoSizeForm
	{
		public enum WarningStyle { Locked, GetLatest };

		public SubmitShelvedWarningDlg()
		{
			//PreferenceKey = string.Format("LockedFilesWarningDlg_{0}",buttons);

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
				Prompt = Resources.WarningStyle_Locked_Prompt;
			
		}

        public static DialogResult Show(string prompt)
        {
            SubmitShelvedWarningDlg dlg = new SubmitShelvedWarningDlg();

            dlg.Prompt = prompt;

            return dlg.ShowDialog();
        }
		public string Prompt
		{
			get
			{
				return PromptLbl.Text;
			}
			set
			{
				PromptLbl.Text = value;
			}
		}

	}
}
