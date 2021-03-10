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
	public partial class GetNewWorkspaceStringDlg : AutoSizeForm
	{
		public GetNewWorkspaceStringDlg(string Caption, string prompt, string DefaultValue)
		{
			PreferenceKey = "GetNewWorkspaceStringDlg";

			InitializeComponent();

            this.Icon = Images.icon_p4vs_16px;
			this.Text = Caption;
			PromptLbl.Text = prompt;
			if (DefaultValue != null)
			{
				ValueTB.Text = DefaultValue;
			}
		}

		public string Value { get { return ValueTB.Text; } }

		public static string Show(string Caption, string prompt, string DefaultValue)
		{
			return Show(Caption, prompt, DefaultValue, false);
		}
		public static string Show(string Caption, string prompt, string DefaultValue, bool isPassword)
		{
			GetNewWorkspaceStringDlg dlg = new GetNewWorkspaceStringDlg(Caption, prompt, DefaultValue);

			if (dlg.ShowDialog() != DialogResult.Cancel)
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					return dlg.Value;
				}
			}
			return null;
		}

		private void ValueTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				newOKBtn.PerformClick();
		}

		private void ValueTB_TextChanged(object sender, EventArgs e)
		{
			newOKBtn.Enabled=!(string.IsNullOrEmpty(ValueTB.Text));
		}
	}
}
