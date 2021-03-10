using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class P4IgnorePreferencesControl : UserControl
	{
		//public P4ScmProvider Scm { get; set; }

		public P4IgnorePreferencesControl()
		{
			P4IgnorePreferencesControl.Instance = this;

			InitializeComponent();

			P4VsProvider.NewConnection += newConection;
		}

		//protected override bool ProcessKeyPreview(ref Message m)
		//{
		//    return true;
		//}
		private void enableIgnoreChk_CheckedChanged(object sender, EventArgs e)
		{
			nameLbl.Enabled = enableIgnoreChk.Checked;
			pathTB.Enabled = enableIgnoreChk.Checked;

			LocalIgnoreChk.Enabled = enableIgnoreChk.Checked && (Scm != null);
			if (!LocalIgnoreChk.Enabled)
			{
				LocalIgnoreChk.Checked = false;
			}
		}

		// The parent page, use to persist data
		private P4IgnorePreferences _customPage;

		public P4IgnorePreferences OptionsPage
		{
			set
			{
				_customPage = value;
			}
		}

		public static P4IgnorePreferencesControl Instance { get; private set; }
		public static P4ScmProvider Scm
		{
			get
			{
				if ((P4VsProvider.Instance != null) &&
					(P4VsProvider.Instance.SccService != null) &&
					(P4VsProvider.Instance.SccService.ScmProvider != null) &&
					(P4VsProvider.Instance.SccService.ScmProvider.Connected))
				{
					return P4VsProvider.Instance.SccService.ScmProvider;
				}
				return null;
			}
		}
		P4VsProvider.NewConnectionDelegate newConection = null;

		public void OnNewConnection(P4ScmProvider newScm)
		{
			if (Scm != null)
			{
				LocalIgnoreChk.Checked = Scm.EnforceLocalIgnore;
				LocalIgnoreChk.Enabled = true;
			}
			else
			{
				LocalIgnoreChk.Checked = false;
				LocalIgnoreChk.Enabled = false;
			}
			if (LocalIgnoreChk.Checked)
			{
				LocalIgnoreChk.Visible=true;
				if (P4DataRetrievalPreferencesControl.Instance != null)
				{
					P4DataRetrievalPreferencesControl.Instance.UpdateIntervalEnabled = false;
				}
			}
		}


		private void P4IgnorePreferencesControl_Load(object sender, EventArgs e)
		{
			//Scm = P4VsProvider.Instance.sccService.ScmProvider;

			pathTB.Text = Preferences.LocalSettings.GetString("PreferedIgnoreFileName", ".p4ignore.txt");

			enableIgnoreChk.Enabled = true;
			enableIgnoreChk.Checked = P4ScmProvider.P4IgnoreSet;

			if (enableIgnoreChk.Checked)
			{
				pathTB.Text = P4ScmProvider.P4Ignore;
			}
			addToSolutionChk.Checked =
				Preferences.LocalSettings.GetBool("AddIgnoreFileToSolutuion", true);
			promptOnNewChk.Checked =
				Preferences.LocalSettings.GetBool("PromptOnNewIgnoreFile", true);
			ignoreIgnoredFilesChk.Checked =
				Preferences.LocalSettings.GetBool("IgnoreIgnoredFiles", true);
			if (ignoreIgnoredFilesChk.Checked)
			{
				// mutually exclusive
				markIgnoreFileForAddChk.Checked = false;
			}
			else
			{
				markIgnoreFileForAddChk.Checked =
				   Preferences.LocalSettings.GetBool("MarkIgnoreFileForAdd", false);
			}
			if (Scm != null)
			{
				LocalIgnoreChk.Checked = Scm.EnforceLocalIgnore;
				LocalIgnoreChk.Enabled = true;
			}
			else
			{
				LocalIgnoreChk.Checked = false;
				LocalIgnoreChk.Enabled = false;
			}
			if (LocalIgnoreChk.Checked)
			{
				LocalIgnoreChk.Visible=true;
				if (P4DataRetrievalPreferencesControl.Instance != null)
				{
					P4DataRetrievalPreferencesControl.Instance.UpdateIntervalEnabled = false;
				}
			}
		}

		public void OnApply()
		{
			string p4Ignore = P4ScmProvider.P4Ignore;
			if ((enableIgnoreChk.Checked) && (pathTB.Text != p4Ignore))
			{
				if (string.IsNullOrEmpty(pathTB.Text))
				{
					pathTB.Text = Preferences.LocalSettings.GetString("PreferedIgnoreFileName", ".p4ignore.txt");
				}
				if (string.IsNullOrEmpty(pathTB.Text)) // still empty or null
				{
					if (string.IsNullOrEmpty(p4Ignore) == false)
					{
						pathTB.Text = p4Ignore;
					}
					else
					{
						pathTB.Text = ".p4ignore.txt";
					}
					// setting is null or empty, so remove it from the preferences
					Preferences.LocalSettings.Remove("PreferedIgnoreFileName");
				}
				if (pathTB.Text != p4Ignore)
				{
					P4ScmProvider.P4Ignore = pathTB.Text;
				}
				Preferences.LocalSettings["PreferedIgnoreFileName"] = pathTB.Text;
			}
			else if ((!enableIgnoreChk.Checked) && (!string.IsNullOrEmpty(p4Ignore)))
			{
				P4ScmProvider.P4Ignore = null;
			}

			Preferences.LocalSettings["PromptOnNewIgnoreFile"] = promptOnNewChk.Checked;
			Preferences.LocalSettings["AddIgnoreFileToSolutuion"] = addToSolutionChk.Checked;
			Preferences.LocalSettings["IgnoreIgnoredFiles"] = ignoreIgnoredFilesChk.Checked;
			if (ignoreIgnoredFilesChk.Checked)
			{
				Preferences.LocalSettings["MarkIgnoreFileForAdd"] = false;
			}
			else
			{
				Preferences.LocalSettings["MarkIgnoreFileForAdd"] = markIgnoreFileForAddChk.Checked;
			}
			if (Scm != null)
			{
				Scm.EnforceLocalIgnore = LocalIgnoreChk.Checked;

				if (enableIgnoreChk.Checked && Scm != null && !string.IsNullOrEmpty(Scm.Connection.WorkspaceRoot))
				{
                    string root = Scm.Connection.WorkspaceRoot;
					string prompt = null;
					if (LocalIgnoreChk.Checked)
					{
						prompt = Resources.P4IgnorePreferencesControl_PromptForIgnoreListAdd;
					}
                    else
                    {
                        string rootIgnoreFile = Path.Combine(root, pathTB.Text);
                        if (File.Exists(rootIgnoreFile))
                        {
                            prompt = Resources.P4IgnorePreferencesControl_PromptForIgnoreListRemove;
                        }
                    }
					if ((prompt != null) && 
                        (DialogResult.Yes == MessageBox.Show(prompt, Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Question)))
					{
                        string ignoreStr = System.IO.Path.Combine(root, "*.*");
						P4VsProvider.Instance.SccService.AddToIgnoreList(!LocalIgnoreChk.Checked, false, ignoreStr);
					}
				}
			}
		}

		private void ignoreIgnoredFilesChk_CheckedChanged(object sender, EventArgs e)
		{
			markIgnoreFileForAddChk.Enabled = !ignoreIgnoredFilesChk.Checked;
			markIgnoreFileForAddChk.Checked = false;
		}

		private void pathTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			if (e.KeyChar == ':' || e.KeyChar == '/' || e.KeyChar == '\\' ||
				e.KeyChar == '*' || e.KeyChar == '?' || e.KeyChar == '"' ||
				e.KeyChar == '<' || e.KeyChar == '>' || e.KeyChar == '|')

				e.Handled = true;
		}

		private void panel1_MouseClick(object sender, MouseEventArgs e)
		{
			if ((Control.ModifierKeys & Keys.Control) != 0)
			{
				LocalIgnoreChk.Visible = !LocalIgnoreChk.Visible;
			}
		}
		public bool LocalIgnoreChkChecked { get { return LocalIgnoreChk.Checked; } }

		private void LocalIgnoreChk_CheckedChanged(object sender, EventArgs e)
		{
			if (P4DataRetrievalPreferencesControl.Instance != null)
			{
				P4DataRetrievalPreferencesControl.Instance.UpdateIntervalEnabled = !LocalIgnoreChk.Checked;
			}
		}
	}
}
