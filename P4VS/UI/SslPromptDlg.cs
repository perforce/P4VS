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
	public partial class SslPromptDlg : AutoSizeForm
	{
		public SslPromptDlg()
		{
			PreferenceKey = "SslPromptDlg";

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;

			this.pictureBox1.Image = Images.SslWarning;
		}

		public static string FingerPrint { get; set; }

		private static bool IsHexDigit(char c)
		{
			if (char.IsDigit(c))
			{
				return true;
			}
			switch (c)
			{
				case 'A':
				case 'a':
				case 'B':
				case 'b':
				case 'C':
				case 'c':
				case 'D':
				case 'd':
				case 'E':
				case 'e':
				case 'F':
				case 'f':
					return true;
			}

			return false;
		}
		public static DialogResult ShowFirstContact(string[] msg)
		{
			SslPromptDlg dlg = new SslPromptDlg();

			dlg.WarningTB.Text = string.Empty;
			for (int idx = 0; idx < msg.Length; idx++)
			{
				if (string.IsNullOrEmpty(msg[idx]) == false)
				{
					dlg.WarningTB.Text += msg[idx];

					if (IsHexDigit(msg[idx][0]) && IsHexDigit(msg[idx][1]))
					{
						FingerPrint = msg[idx];
						break;
					}

					dlg.WarningTB.Text += "\r\n";
				}
			}
			dlg.ShowWarningLabel = false;
			dlg.WarningTB.SelectionLength = 0;
			dlg.TrustCB.Focus();

			return dlg.ShowDialog();
		}

		public static DialogResult ShowNewFingerprint(string[] msg)
		{
			SslPromptDlg dlg = new SslPromptDlg();

			dlg.WarningTB.Text = string.Empty;
			//skip first line
			for (int idx = 1; idx < msg.Length; idx++)
			{
				if (string.IsNullOrEmpty(msg[idx]) == false)
				{
					dlg.WarningTB.Text += msg[idx];

					if (IsHexDigit(msg[idx][0]) && IsHexDigit(msg[idx][1]))
					{
						FingerPrint = msg[idx];
						break;
					}

					dlg.WarningTB.Text += "\r\n";
				}
			}
			dlg.ShowWarningLabel = true;
			dlg.WarningTB.SelectionLength = 0;
			dlg.TrustCB.Focus();

			return dlg.ShowDialog();
		}

		public bool ShowWarningLabel
		{
			get { return WarningLbl.Visible; }
			set 
			{ 
				WarningLbl.Visible = value; 
			}
		}
		private void TrustCB_CheckedChanged(object sender, EventArgs e)
		{
			ConnectBtn.Enabled = TrustCB.Checked;
		}
	}
}
