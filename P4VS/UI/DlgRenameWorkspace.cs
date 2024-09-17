using System;
using System.Windows.Forms;

namespace Perforce.P4VS
{
    public partial class DlgRenameWorkspace : AutoSizeForm
	{
		public DlgRenameWorkspace(string caption, string prompt, string DefaultValue)
		{
			PreferenceKey = "DlgRenameWorkspace";

			InitializeComponent();

            this.Icon = Images.icon_p4vs_16px;
			this.Text = caption;
			renameWorkspaceLbl.Text = prompt;
            getNewNameLbl.Text = Resources.WorkspacesWindowControl_RenameWorkspaceNewNameLabel;
            warningLbl.Text = Resources.WorlspacesWindowControl_RenameWorkspaceWarningInfo;
            EnableDisableRenameWorkspaceBtn();
		}

		private DlgRenameWorkspace() {}

		public string Value { get { return ValueTB.Text; } }

		public static string Show(string Caption, string prompt, string DefaultValue)
		{
            DlgRenameWorkspace dlg = new DlgRenameWorkspace(Caption, prompt, DefaultValue);

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
				renameWorkspaceBtn.PerformClick();
		}

		private void ValueTB_TextChanged(object sender, EventArgs e)
		{
            EnableDisableRenameWorkspaceBtn();
		}

        private void EnableDisableRenameWorkspaceBtn()
        {
            renameWorkspaceBtn.Enabled = !(string.IsNullOrWhiteSpace(ValueTB.Text));
        }

    }
}
