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
	public partial class FileListWarningDlg : AutoSizeForm
	{
		public enum WarningStyle { Locked, GetLatest, UpdateSln };

		public FileListWarningDlg(WarningStyle buttons)
		{
			PreferenceKey = string.Format("LockedFilesWarningDlg_{0}",buttons);

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
			DontGetLatestBtn.Visible = (buttons == WarningStyle.GetLatest || buttons == WarningStyle.UpdateSln);
			GetLatestBtn.Visible = (buttons == WarningStyle.GetLatest || buttons == WarningStyle.UpdateSln);
            OkBtn.Visible = (buttons == WarningStyle.Locked);

			if (buttons == WarningStyle.GetLatest)
			{
				Prompt = Resources.WarningStyle_GetLatest_Prompt;
				ReasonClmLbl = null;
			}
            else if (buttons == WarningStyle.UpdateSln)
            {
                Prompt = Resources.WarningStyle_GetLatestSln_Prompt;
                ReasonClmLbl = null;
            }
            else
			{
				Prompt = Resources.WarningStyle_Locked_Prompt;
				ReasonClmLbl = Resources.WarningStyle_Locked_WarningClmLbl;
			}
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

		public string ReasonClmLbl
		{
			get
			{
				if (ReasonClm.Width > 0)
				{
					return ReasonClm.Text;
				}
				return null;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					if (FilesList.Columns.Contains(ReasonClm))
					{
						FilesList.Columns.Remove(ReasonClm);
					}
					//ReasonClm.Width = 0;
					ReasonClm.Text = string.Empty;
					//ReasonClm.AutoResize(ColumnHeaderAutoResizeStyle.None);
				}
				else
				{
					if (FilesList.Columns.Contains(ReasonClm) == false)
					{
						FilesList.Columns.Add(ReasonClm);
					}
					//ReasonClm.Width = -2;
					ReasonClm.Text = value;
					//ReasonClm.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
				}
			}
		}

		public static DialogResult Show(IList<string> files, IList<string> Reason, WarningStyle style)
		{
			FileListWarningDlg dlg = new FileListWarningDlg(style);

			for (int idx = 0; idx < files.Count; idx++)
			{
				string fileName = Path.GetFileName(files[idx]);
				string path = Path.GetDirectoryName(files[idx]);
				string locked = string.Empty;
				if ((Reason != null) && (idx < Reason.Count))
				{
					locked = Reason[idx];
				}
				ListViewItem item = new ListViewItem(new string[] { fileName, path, locked });

				dlg.FilesList.Items.Add(item);
			}

			return dlg.ShowDialog();
		}

		private void DontGetLatestBtn_Click(object sender, EventArgs e)
		{

		}
	}
}
