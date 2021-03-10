using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

using Perforce;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class SelectChangelistDlg2 : SelectChangelistDlgBase
	{
		public const int Checkout = 1;
		public const int EditInMemory = 2;
		public const int Cancel = 3;
		public const int SaveAs = 4;
		public const int Ignore = 5;

		IList<string> Files { get; set; }

		public bool HideActionsColumn { get; set; }

		public static int ShowChooseChangelistYesNo(string prompt, IList<string> files, IList<P4.Changelist> items, ref string NewChangeDescription)
		{
            if (P4VsProvider.Instance.ChangeLists.ActiveChangeList == -2)
            {
                Preferences.LocalSettings["PromptForChanglist"] = true;
            }
            if (Preferences.LocalSettings.GetBool("PromptForChanglist", true))
			{
				SelectChangelistDlg2 dlg = new SelectChangelistDlg2(prompt, files, items);

				dlg.NewChangelistDescription = NewChangeDescription;

				dlg.OkBtn.Visible = false;
				dlg.CancelBtn.Visible = false;

				dlg.saveToChangelistBtn.Visible = false;
				dlg.submitBtn.Visible = false;

				dlg.YesBtn1.Visible = true;
				dlg.NoBtn1.Visible = true;

				dlg.YesBtn2.Visible = false;
				dlg.NoBtn2.Visible = false;

				dlg.BtnBar.Controls.Clear();
				dlg.BtnBar.Controls.Add(dlg.BtnSpacer);
				dlg.BtnBar.Controls.Add(dlg.NoBtn1);
				dlg.BtnBar.Controls.Add(dlg.YesBtn1);

				dlg.YesBtn1.Column = 1;
				dlg.NoBtn1.Column = 2;
				dlg.BtnBar.InitializeGrid(true);

				DialogResult res = dlg.ShowDialog();
				if ((res == DialogResult.No) || (res == DialogResult.Cancel))
				{
					CurrentChangeList = -2;
					return CurrentChangeList;
				}
				int changelistId = dlg.Result;
				if (changelistId == -1)
				{
					NewChangeDescription = dlg.DescriptionTB.Text;
					//					CurrentChangeDescription = NewChangeDescription;
				}
				CurrentChangeListLastUse = DateTime.Now;
				CurrentChangeList = changelistId;
				return changelistId;
			}
			else
			{
                return P4VsProvider.Instance.ChangeLists.ActiveChangeList;
			}
		}

		public static int ShowChooseChangelistActions(string prompt, IList<string> files, IList<string> actions, IList<P4.Changelist> items, ref string NewChangeDescription)
		{
			if (Preferences.LocalSettings.GetBool("PromptForChanglist", true))
			{
				if (ActiveChangeList > -1)
				{
					CurrentChangeListLastUse = DateTime.Now;
					//					NewChangeDescription = CurrentChangeDescription;
					return CurrentChangeList;
				}
				SelectChangelistDlg2 dlg = new SelectChangelistDlg2(prompt, files, actions, items);

				dlg.NewChangelistDescription = NewChangeDescription;

				dlg.OkBtn.Visible = true;
				dlg.CancelBtn.Visible = true;

				dlg.saveToChangelistBtn.Visible = false;
				dlg.submitBtn.Visible = false;

				dlg.YesBtn1.Visible = false;
				dlg.NoBtn1.Visible = false;

				dlg.YesBtn2.Visible = false;
				dlg.NoBtn2.Visible = false;

				dlg.BtnBar.Controls.Clear();
				dlg.BtnBar.Controls.Add(dlg.BtnSpacer);
				dlg.BtnBar.Controls.Add(dlg.OkBtn);
				dlg.BtnBar.Controls.Add(dlg.CancelBtn);

				dlg.OkBtn.Column = 1;
				dlg.CancelBtn.Column = 2;
				dlg.BtnBar.InitializeGrid(true);

				if (dlg.ShowDialog() == DialogResult.Cancel)
				{
					CurrentChangeList = -2;
					return CurrentChangeList;
				}
				int changelistId = dlg.Result;
				if (changelistId == -1)
				{
					NewChangeDescription = dlg.DescriptionTB.Text;
					//					CurrentChangeDescription = NewChangeDescription;
				}
				CurrentChangeListLastUse = DateTime.Now;
				CurrentChangeList = changelistId;
				return changelistId;
			}
			else
			{
				return P4VsProvider.Instance.ChangeLists.ActiveChangeList;
			}
		}

		public static int ShowChooseChangelistYesNoCancel(string prompt, IList<string> files, IList<P4.Changelist> items, ref string NewChangeDescription)
		{
			//if ((items == null) || (items.Count <= 0))
			//{
			//    return 0;
			//}
			if (ActiveChangeList > -1)
			{
				CurrentChangeListLastUse = DateTime.Now;
				//					NewChangeDescription = CurrentChangeDescription;
				return CurrentChangeList;
			}
			SelectChangelistDlg2 dlg = new SelectChangelistDlg2(prompt, files, items);

			dlg.NewChangelistDescription = NewChangeDescription;

			dlg.OkBtn.Visible = false;
			dlg.CancelBtn.Visible = true;

			dlg.saveToChangelistBtn.Visible = false;
			dlg.submitBtn.Visible = false;

			dlg.YesBtn1.Visible = false;
			dlg.NoBtn1.Visible = false;

			dlg.YesBtn2.Visible = true;
			dlg.NoBtn2.Visible = true;

			dlg.DontShowAgainCB.Visible = false;

			dlg.BtnBar.Controls.Clear();
			dlg.BtnBar.Controls.Add(dlg.BtnSpacer);
			dlg.BtnBar.Controls.Add(dlg.YesBtn2);
			dlg.BtnBar.Controls.Add(dlg.NoBtn2);
			dlg.BtnBar.Controls.Add(dlg.CancelBtn);

			dlg.YesBtn2.Column = 1;
			dlg.NoBtn2.Column = 2;
			dlg.CancelBtn.Column = 3;
			dlg.BtnBar.InitializeGrid(true);

			DialogResult res = dlg.ShowDialog();
			if (res == DialogResult.Cancel)
			{
				CurrentChangeList = -3;
				return CurrentChangeList;
			}
			if (res == DialogResult.No)
			{
				CurrentChangeList = -2;
				return CurrentChangeList;
			}
			int changelistId = dlg.Result;
			if (changelistId == -1)
			{
				NewChangeDescription = dlg.DescriptionTB.Text;
				//					CurrentChangeDescription = NewChangeDescription;
			}
			CurrentChangeListLastUse = DateTime.Now;
			CurrentChangeList = changelistId;
			return changelistId;
		}

		public static int ShowChooseChangelistMove(string prompt, IList<string> files, IList<P4.Changelist> items, ref string NewChangeDescription, P4ScmProvider scm)
		{

			if (ActiveChangeList > -1)
			{
				CurrentChangeListLastUse = DateTime.Now;
				//					NewChangeDescription = CurrentChangeDescription;
				return CurrentChangeList;
			}
			SelectChangelistDlg2 dlg = new SelectChangelistDlg2(prompt, files, items);

			dlg.DontShowAgainCB.Enabled = false;
			dlg.DontShowAgainCB.Visible = false;

			dlg.saveToChangelistBtn.Visible = false;
			dlg.submitBtn.Visible = false;

			dlg.NewChangelistDescription = NewChangeDescription;

			dlg.OkBtn.Visible = true;
			dlg.CancelBtn.Visible = true;

			dlg.YesBtn1.Visible = false;
			dlg.NoBtn1.Visible = false;
			dlg.YesBtn2.Visible = false;
			dlg.NoBtn2.Visible = false;

			dlg.BtnBar.Controls.Clear();
			dlg.BtnBar.Controls.Add(dlg.BtnSpacer);
			dlg.BtnBar.Controls.Add(dlg.OkBtn);
			dlg.BtnBar.Controls.Add(dlg.CancelBtn);

			dlg.OkBtn.Column = 1;
			dlg.CancelBtn.Column = 2;
			dlg.BtnBar.InitializeGrid(true);

			if (dlg.ShowDialog() == DialogResult.Cancel)
			{
				CurrentChangeList = -2;
				return CurrentChangeList;
			}
			int changelistId = dlg.Result;
			if (changelistId == -1)
			{
				P4.Changelist newChange = scm.Connection.Repository.NewChangelist();
                // Make sure files is empty. If default has files in it, a new changelist
                // will automatically get those files.
                newChange.Files = new List<P4.FileMetaData>();
                newChange.Description = dlg.DescriptionTB.Text;
				newChange = scm.SaveChangelist(newChange, null);
                if (newChange!=null)
                {
                    return newChange.Id;			
                }
                else
                {
                    return changelistId;			
                }
			}
			CurrentChangeListLastUse = DateTime.Now;
			CurrentChangeList = changelistId;
			return changelistId;

		}

		public static int ShowChooseChangelistSubmit(string prompt, IList<string> files, IList<P4.Changelist> items, ref string NewChangeDescription, out string action)
		{
			action = null;

			//if ((items == null) || (items.Count <= 0))
			//{
			//    return 0;
			//}
			if (Preferences.LocalSettings.GetBool("PromptForChanglist", true))
			{
				SelectChangelistDlg2 dlg = new SelectChangelistDlg2(prompt, files, items);
				
				dlg.NewChangelistDescription = NewChangeDescription;
				
				dlg.saveToChangelistBtn.Enabled = true;
				dlg.saveToChangelistBtn.Visible = true;
				dlg.submitBtn.Enabled = true;
				dlg.submitBtn.Visible = true;
				dlg.CancelBtn.Visible = true;

				dlg.YesBtn1.Visible = false;
				dlg.NoBtn1.Visible = false;
				dlg.YesBtn2.Visible = false;
				dlg.NoBtn2.Visible = false;

				dlg.OkBtn.Visible = false;

				dlg.BtnBar.Controls.Clear();
				dlg.BtnBar.Controls.Add(dlg.BtnSpacer);
				dlg.BtnBar.Controls.Add(dlg.saveToChangelistBtn);
				dlg.BtnBar.Controls.Add(dlg.submitBtn);
				dlg.BtnBar.Controls.Add(dlg.CancelBtn);

				dlg.saveToChangelistBtn.Column = 1;
				dlg.submitBtn.Column = 2;
				dlg.CancelBtn.Column = 2;
				dlg.BtnBar.InitializeGrid(true);

				if (dlg.ShowDialog() == DialogResult.Cancel)
				{
					return -2;
				}

				if (dlg.DialogResult == DialogResult.OK)
				{
					// Save to Changelist
					return dlg.Result;
				}

				if (dlg.DialogResult == DialogResult.Yes)
				{
					// Submit...
					action = "submit";
					return dlg.Result;
				}

				int changelistId = dlg.Result;
				if (changelistId == -1)
				{
					NewChangeDescription = dlg.DescriptionTB.Text;
				}
				return changelistId;
			}
			else
			{
                return P4VsProvider.Instance.ChangeLists.ActiveChangeList;
			}
		}

		private Dictionary<int, P4.Changelist> changeMap;

		private void Init(string prompt, IList<string> files, IList<string> actions, IList<P4.Changelist> items)
		{
			PreferenceKey = "SelectChangelistDlg2";

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
			NewChangelistDescription = Resources.SelectChangelistDlg_DefaultNewChangeDesc;

			//PromptLbl.Text = prompt;
			msgText.Text = prompt;

			changeMap = new Dictionary<int, P4.Changelist>();

			//add new as the first item in the list 
			ItemsCB.Items.Add(Resources.Changelist_New);
			changeMap[-1] = null;

			//add default as the first item in the list if not already there
			ItemsCB.Items.Add(Resources.Changelist_Default);
			changeMap[-0] = null;

			if ((items != null) && (items.Count > 0))
			{
				foreach (P4.Changelist item in items)
				{
					int id = item.Id;
					if ((id != -1) && (id != 0))
					{
						changeMap[id] = item;
						int idx = ItemsCB.Items.Add(SelectChangelistDlg.ChangeListToString(item));
					}
				}
			}

			Files = files;

			HideActionsColumn = (actions == null);

			if ((files != null) && (files.Count > 0))
			{
				for (int idx = 0; idx < files.Count; idx++)
				{
					string action = string.Empty;
					if ((actions != null) && (idx < actions.Count) && (actions[idx] != null))
					{
						action = actions[idx];
					}
					if (files[idx] != null)
					{
						string fileName = Path.GetFileName(files[idx]);
						string path = Path.GetDirectoryName(files[idx]);
						ListViewItem item = new ListViewItem(new string[] { action, fileName, path });

						FilesList.Items.Add(item);
					}
				}
			}
			// select the default changelist
			ItemsCB.SelectedIndex = 1;

			OkBtn.Enabled = ItemsCB.SelectedIndex >= 0;
		}
		public SelectChangelistDlg2(string prompt, IList<string> files, IList<string> actions, IList<P4.Changelist> items)
		{
			Init(prompt, files, actions, items);
		}
		public SelectChangelistDlg2(string prompt, IList<string> files, IList<P4.Changelist> items)
		{
			Init(prompt, files, null, items);
		}

		public int Result
		{
			get 
			{
				if (ItemsCB.SelectedIndex >= 2)
				{
					string[] parts = ItemsCB.Text.Split(' ');
					int changeId = -1;
					if ((parts.Length > 0) && (int.TryParse(parts[0], out changeId)))
					{
						return changeId;
					}
					return -1;
				}
				else if (ItemsCB.SelectedIndex == 0)
				{
					return -1;
				}
				// otherwise it's the default
				return 0;
			}
		}

		private void ChooseDlg_HelpButtonClicked(object sender, CancelEventArgs e)
		{

		}

		private void DontShowAgainCB_CheckedChanged(object sender, EventArgs e)
		{
			Preferences.LocalSettings["PromptForChanglist"] = !DontShowAgainCB.Checked;
		}

		//int newChangelistId = -1;

		private void OkBtn_Click(object sender, EventArgs e)
		{
			if (ItemsCB.SelectedIndex == 0)
			{
				// create a new changelist
			}
		}

		public string NewChangelistDescription { get; set; }

		private void ItemsCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool oldVal = DescriptionTB.ReadOnly;
			DescriptionTB.ReadOnly = ItemsCB.SelectedIndex != 0;

			if (oldVal != DescriptionTB.ReadOnly)
			{
				// changed to/from 'New'
				if (oldVal)
				{
					// was read only, so now description for new changelist
					DescriptionTB.Text = NewChangelistDescription;
				}
				else
				{
					// 'New' is no longer selected, so save the description for new
					// in case the user comes back to it.
					NewChangelistDescription = DescriptionTB.Text;
				}
			}
			int changelistId = 0;

			if (ItemsCB.SelectedIndex > 1)
			{
				int idx = ((string) ItemsCB.SelectedItem).IndexOf(' ');
				string str = ((string) ItemsCB.SelectedItem).Substring(0,idx);

				changelistId = 0;
				int.TryParse(str, out changelistId);
			}
			else if (ItemsCB.SelectedIndex == 1)
			{
				changelistId = 0;
			}
			else
			{
				changelistId = -1;
			}

			if (changeMap[changelistId] != null)
			{
				DescriptionTB.Text = changeMap[changelistId].Description;
			}
			else if (changelistId == 0)
			{
				DescriptionTB.Text = string.Empty;
			}

			OkBtn.Enabled = ItemsCB.SelectedIndex >= 0;

			if (!DescriptionTB.ReadOnly)
			{
				OkBtn.Enabled &= (DescriptionTB.Text.Length > 0 &&
                    DescriptionTB.Text.Trim() != Resources.DefaultChangeListDescription);
            }
            if (OkBtn.Enabled)
            {
                label1.ForeColor = SystemColors.ControlText;
            }
            else
            {
                label1.ForeColor = Color.Red;
			}
		}

		private void DescriptionTB_TextChanged(object sender, EventArgs e)
		{
			OkBtn.Enabled = ItemsCB.SelectedIndex >= 0;

			if (!DescriptionTB.ReadOnly)
			{
                OkBtn.Enabled &= (DescriptionTB.Text.Length > 0 &&
                    DescriptionTB.Text.Trim() != Resources.DefaultChangeListDescription);
            }
            if (OkBtn.Enabled)
            {
                label1.ForeColor = SystemColors.ControlText;
            }
            else
            {
                label1.ForeColor = Color.Red;
            }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (HideActionsColumn)
			{
			    this.FilesList.HideActionsColumn = HideActionsColumn;
				ActionClm.Width = 0;
			}
			else
			{
				string prefKey = "SelectChangelistDlg2_FilesList_Column_0_Width";
				int width = Preferences.LocalSettings.GetInt(prefKey, 60);
				if (width > 0)
				{
					ActionClm.Width = width;
				}
				else
				{
					ActionClm.Width = 60;
				}
			}
		}

		private bool LayoutBtnBarOnce = true;

		private void gridLayoutPanel1_AfterLayoutGrid()
		{
			if (LayoutBtnBarOnce)
			{
				BtnBar.LayoutGrid();
				LayoutBtnBarOnce = false;
			}
		}
	}
}
