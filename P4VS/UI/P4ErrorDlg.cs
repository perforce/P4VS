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
	public partial class P4ErrorDlg : AutoSizeForm
	{
		private P4ErrorDlg(P4.P4CommandResult results, bool showCancel)
		{
			PreferenceKey = "P4ErrorDlg";

			InitializeComponent();

			_defCancelBtnLocation = CancelBtn.Location;
			_defOKBtnLocation = OkBtn.Location;

            this.Icon = Images.icon_p4vs_16px;
			Results = results;
			ShowCancelBtn = showCancel;
		}

		private P4ErrorDlg(P4.P4Exception ex, bool showCancel)
		{
			PreferenceKey = "P4ErrorDlg";

			InitializeComponent();

			_defCancelBtnLocation = CancelBtn.Location;
			_defOKBtnLocation = OkBtn.Location;

			this.Icon = Images.icon_p4vs_16px;
			ShowCancelBtn = showCancel;

			ErrorsTB.Text = string.Empty;
			if (string.IsNullOrEmpty(ex.CmdLine) == false)
			{
				ErrorsTB.Text += ex.CmdLine + "\r\n\r\n";
			}
			ErrorsTB.Text += Resources.P4ErrorDlg_Errors + "\r\n";

			P4.P4Exception currentError = ex;
			while (currentError != null)
			{
				if (!(ErrorsTB.Text.Contains(currentError.Message)))
				{
					ErrorsTB.Text += currentError.Message;
					ErrorsTB.Text += "\r\n";

				}
				currentError = currentError.NextError;
			}
			if (ex.Details != null)
			{
				ErrorsTB.Text += "\r\n";
				ErrorsTB.Text += Resources.P4ErrorDlg_Details + "\r\n";
				foreach (P4.P4ClientInfoMessage info in ex.Details)
				{
					ErrorsTB.Text += info.Message;
					ErrorsTB.Text += "\r\n";
				}
			}
			PromptLbl.Text = Resources.P4ErrorDlg_PromptLabel;
		}

		private P4ErrorDlg(SwarmApi.SwarmServer.SwarmException ex, bool showCancel)
		{
			PreferenceKey = "P4ErrorDlg";

			InitializeComponent();

			_defCancelBtnLocation = CancelBtn.Location;
			_defOKBtnLocation = OkBtn.Location;

			this.Icon = Images.icon_p4vs_16px;
			ShowCancelBtn = showCancel;

			ErrorsTB.Text = string.Empty;
			ErrorsTB.Text += Resources.P4ErrorDlg_Errors + "\r\n";
			if (ex.Message.Contains("Not Found"))
			{
				ErrorsTB.Text += Resources.P4ErrorDlg_ReviewNotFound + "\r\n";
			}
			else
			{
				ErrorsTB.Text += ex.Message.Trim('{', '}', ' ') + "\r\n";
				if (ex.InnerException != null)
				{
					ErrorsTB.Text += ex.InnerException.Message.Trim('{', '}', ' ') + "\r\n";
				}
			}
			PromptLbl.Text = Resources.P4ErrorDlg_SwarmPromptLabel;
		}

		private P4ErrorDlg(string errorMsg, bool showCancel)
		{
			PreferenceKey = "P4ErrorDlg";

			InitializeComponent();

			_defCancelBtnLocation = CancelBtn.Location;
			_defOKBtnLocation = OkBtn.Location;

			this.Icon = Images.icon_p4vs_16px;
			ShowCancelBtn = showCancel;

			ErrorsTB.Text = errorMsg;

			PromptLbl.Text = Resources.P4ErrorDlg_PromptLabel;
		}

		private P4ErrorDlg(IList<P4.P4Exception> errors, bool showCancel)
		{
			PreferenceKey = "P4ErrorDlg";

			InitializeComponent();

			_defCancelBtnLocation = CancelBtn.Location;
			_defOKBtnLocation = OkBtn.Location;

            this.Icon = Images.icon_p4vs_16px;
			ShowCancelBtn = showCancel;

			ErrorsTB.Text = string.Empty;

			if (string.IsNullOrEmpty(errors[0].CmdLine) == false)
			{
				ErrorsTB.Text += errors[0].CmdLine + "\r\n\r\n";
			}

			ErrorsTB.Text += Resources.P4ErrorDlg_Errors + "\r\n";

			foreach (P4.P4Exception ex in errors)
			{
				P4.P4Exception currentError = ex;
				while (currentError != null)
				{
					ErrorsTB.Text += currentError.Message;
					ErrorsTB.Text += "\r\n";
					currentError = currentError.NextError;
				}
				if (ex.Details != null)
				{
					ErrorsTB.Text += "\r\n";
					ErrorsTB.Text += Resources.P4ErrorDlg_Details + "\r\n";
					foreach (P4.P4ClientInfoMessage info in ex.Details)
					{
						ErrorsTB.Text += info.Message;
						ErrorsTB.Text += "\r\n";
					}
				}
			}
			PromptLbl.Text = Resources.P4ErrorDlg_PromptLabel;
		}

		public P4ErrorDlg()
		{
			InitializeComponent();

			_defCancelBtnLocation = CancelBtn.Location;
			_defOKBtnLocation = OkBtn.Location;
		}

		public static DialogResult Show(P4.P4CommandResult results, bool showCancel)
		{
			if ((results == null) || (results.ErrorList == null) || (results.ErrorList.Count <= 0))
			{
				return DialogResult.Ignore;
			}
			int numWarnings = 0;
			List<string> WarningMsgs = new List<string>();

			foreach (P4.P4ClientError error in results.ErrorList)
			{
				if (error.SeverityLevel <= P4.ErrorSeverity.E_WARN)
				{
					numWarnings++;
				}
				WarningMsgs.Add(string.Format("{0}: {1}", error.SeverityLevel, error.ErrorMessage));
			}
			if ((results.ErrorList.Count - numWarnings) <= 0)
			{
				if (WarningMsgs.Count > 0)
				{
					// error dlialog will not display, so list any warning in the output pane
					foreach (string msg in WarningMsgs)
					{
						P4VsOutputWindow.AppendMessage(msg);
					}
				} 
				return DialogResult.Ignore;
			}

			P4ErrorDlg dlg = new P4ErrorDlg(results, showCancel);
			dlg.PromptLbl.Text = Resources.P4ErrorDlg_PromptLabel;
			dlg.TopMost = true;
			return dlg.ShowDialog();
		}

		public static DialogResult Show(P4.P4Exception ex)
		{
			return Show(ex, false, false);
		}

        public static DialogResult Show(P4.P4Exception ex, bool showCancel, bool suppressUI)
        {
            P4VsProviderService p4vs = (P4VsProviderService)P4VsProvider.Service(typeof(P4VsProviderService));
            if ((suppressUI) && (ex.ErrorLevel < P4.ErrorSeverity.E_FATAL))
            {
                return DialogResult.OK;
            }

            if (p4vs.ScmProvider!=null&&p4vs.ScmProvider.Connection!=null)
            {
                if (p4vs.ScmProvider.Connection.isLoggedIn() == false)
                {
                    MessageBox.Show(Resources.P4ErrorDlg_LoggedOutMsg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return DialogResult.Cancel;
                }
            }

            if (ex is P4.P4CommandCanceledException)
            {
                // this command was canceled by the user, so no need to show an error.
                return DialogResult.OK;
            }
            
            P4ErrorDlg dlg = new P4ErrorDlg(ex, showCancel);

            return dlg.ShowDialog();
        }

		public static DialogResult Show(string errorMsg, bool showCancel, bool suppressUI)
		{
			if (suppressUI)
			{
				return DialogResult.OK;
			}
			P4ErrorDlg dlg = new P4ErrorDlg(errorMsg, showCancel);

			return dlg.ShowDialog();
		}

		public static DialogResult Show(SwarmApi.SwarmServer.SwarmException ex, bool showCancel, bool suppressUI)
		{
			if (suppressUI)
			{
				return DialogResult.OK;
			}
			P4ErrorDlg dlg = new P4ErrorDlg(ex, showCancel);

			return dlg.ShowDialog();
		}

		public static DialogResult Show(IList<P4.P4Exception> errors, bool showCancel)
		{
			P4ErrorDlg dlg = new P4ErrorDlg(errors, showCancel);

			return dlg.ShowDialog();
		}

		private new DialogResult ShowDialog()
		{
			return base.ShowDialog();
		}

		private new DialogResult ShowDialog(IWin32Window wnd)
		{
			return base.ShowDialog(wnd);
		}

		private Point _defCancelBtnLocation;
		private Point _defOKBtnLocation;

		public bool ShowCancelBtn
		{
			get { return CancelBtn.Visible; }
			set
			{
				//if (value == CancelBtn.Visible)
				//{
				//    //not changing so return;
				//    return;
				//}
				CancelBtn.Visible = value;

				// If hiding the cancel button, move the OK button over to where the cancel button was
				if (value == false)
				{
					OkBtn.Location = _defOKBtnLocation;
				}
				else
				{
					OkBtn.Location = _defCancelBtnLocation;
				}
			}
		}

		public P4.P4CommandResult Results
		{
			set
			{
				PromptLbl.Text += string.Empty;

				if (string.IsNullOrEmpty(value.Cmd) == false)
				{
					PromptLbl.Text += value.Cmd;
					if (value.CmdArgs != null)
					{
						for (int idx = 0; idx < value.CmdArgs.Length; idx++)
						{
							PromptLbl.Text += " " + value.CmdArgs[idx];
						}
					}
					PromptLbl.Text += "\r\n\r\n";
				}

				ErrorsTB.Text += Resources.P4ErrorDlg_Errors + "\r\n";
				foreach (P4.P4ClientError error in value.ErrorList)
				{
					ErrorsTB.Text += error.ToString();
					ErrorsTB.Text += "\r\n";
				}
				if (value.InfoOutput != null)
				{
					ErrorsTB.Text += Resources.P4ErrorDlg_Details + "\r\n";
					ErrorsTB.Text += "\r\n";
					foreach (P4.P4ClientInfoMessage info in value.InfoOutput)
					{
						ErrorsTB.Text += info.Message;
						ErrorsTB.Text += "\r\n";
					}
				}
				PromptLbl.Text = Resources.P4ErrorDlg_PromptLabel;
			}
		}

	}
}
