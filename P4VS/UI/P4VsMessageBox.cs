using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System;
using System.Windows.Forms;
using Microsoft.VisualStudio;

namespace Perforce.P4VS
{
    public static class P4VsMessageBox
    {
        public static DialogResult Show(string message, string title = null, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            if (string.IsNullOrEmpty(title))
            {
                title = Resources.P4VS;
            }

            OLEMSGBUTTON msgBtn = OLEMSGBUTTON.OLEMSGBUTTON_OK;
            OLEMSGDEFBUTTON msgDefBtn = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
            OLEMSGICON msgIcon = OLEMSGICON.OLEMSGICON_NOICON;

            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    msgBtn = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                    break;
                case MessageBoxButtons.OKCancel:
                    msgBtn = OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL;
                    break;
                case MessageBoxButtons.YesNo:
                    msgBtn = OLEMSGBUTTON.OLEMSGBUTTON_YESNO;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    msgBtn = OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL;
                    break;
                case MessageBoxButtons.RetryCancel:
                    msgBtn = OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL;
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    msgBtn = OLEMSGBUTTON.OLEMSGBUTTON_ABORTRETRYIGNORE;
                    break;
            }

            switch (icon)
            {
                case MessageBoxIcon.Error:
                    msgIcon = OLEMSGICON.OLEMSGICON_CRITICAL;
                    break;
                case MessageBoxIcon.Question:
                    msgIcon = OLEMSGICON.OLEMSGICON_QUERY;
                    break;
                case MessageBoxIcon.Warning:
                    msgIcon = OLEMSGICON.OLEMSGICON_WARNING;
                    break;
                case MessageBoxIcon.Information:
                    msgIcon = OLEMSGICON.OLEMSGICON_INFO;
                    break;
            }

            IVsUIShell uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            
            if (uiShell != null)
            {
                Guid clsid = Guid.Empty;
                int result;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                           0,
                           ref clsid,
                           title,
                           message,
                           string.Empty,
                           0,
                           msgBtn,
                           msgDefBtn,
                           msgIcon,
                           0,
                           out result));

                switch (result)
                {
                    case 1: return DialogResult.OK;
                    case 2: return DialogResult.Cancel;
                    case 3: return DialogResult.Abort;
                    case 4: return DialogResult.Retry;
                    case 5: return DialogResult.Ignore;
                    case 6: return DialogResult.Yes;
                    case 7: return DialogResult.No;
                    default: return DialogResult.None;
                }
            }
            else
            {
                return MessageBox.Show(message, title, buttons, icon);
            }
        }
    }
}
