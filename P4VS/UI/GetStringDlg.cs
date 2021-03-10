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
	public partial class GetStringDlg : AutoSizeForm
	{
		public GetStringDlg(string Caption, string prompt, string DefaultValue)
		{
			PreferenceKey = "GetStringDlg";

			InitializeComponent();

            this.Icon = Images.icon_p4vs_16px;
			this.Text = Caption;
			PromptLbl.Text = prompt;
			if (DefaultValue != null)
			{
				ValueTB.Text = DefaultValue;
			}
		}

		private GetStringDlg() {}

		public string Value { get { return ValueTB.Text; } }

		public static string Show(string Caption, string prompt, string DefaultValue)
		{
			return Show(Caption, prompt, DefaultValue, false);
		}

		public static string Show(string Caption, string prompt, string DefaultValue, bool isPassword)
		{
			GetStringDlg dlg = new GetStringDlg(Caption, prompt, DefaultValue);

			dlg.ValueTB.UseSystemPasswordChar = isPassword;

			if (isPassword)
			{
				dlg.ShowIcon = true;
				dlg.TopMost = true;
			}
			if (dlg.ShowDialog() != DialogResult.Cancel)
			{
				if (dlg.DialogResult == DialogResult.OK)
				{
					return dlg.Value;
				}
			}
			return null;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (ValueTB.UseSystemPasswordChar)
			{
				//getting a password, so center on the VS Window
				StartPosition = FormStartPosition.CenterParent;
			}
		}

		private void ValueTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				newOKBtn.PerformClick();
		}

        private void GetStringDlg_Load(object sender, EventArgs e)
        {
            if ((ValueTB.UseSystemPasswordChar) && (StartPosition == FormStartPosition.CenterParent))
            {
                // if opening in the default location, move it uup the screen so it can't
                // end up behind the VS initializing progress box, which is also always on top.
                StartPosition = FormStartPosition.Manual;
                Top -= Top / 2;
            }
        }
    }
}
