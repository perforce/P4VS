using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Perforce;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class SelectChangelistDlg : SelectChangelistDlgBase
	{
		public const int None = 0;
		public const int Checkout = 1;
		public const int EditInMemory = 2;
		public const int Cancel = 3;
		public const int SaveAs = 4;
		public const int Ignore = 5;

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
        public static int ShowChooseChangelist(string prompt, IList<P4.Changelist> items, ref string NewChangeDescription)
		{
			//if ((items == null) || (items.Count <= 0))
			//{
			//    return 0;
			//}
            if (Preferences.LocalSettings.GetBool("PromptForChanglist", true) || (P4VsProvider.Instance.ChangeLists.ActiveChangeList == -2))
			{
				if (ActiveChangeList > -1)
				{
					CurrentChangeListLastUse = DateTime.Now;
//					NewChangeDescription = CurrentChangeDescription;
					return CurrentChangeList;
				}
				SelectChangelistDlg dlg = new SelectChangelistDlg(prompt, items);

				dlg.NewChangelistDescription = NewChangeDescription;

				dlg.OkBtn.Visible = true;
				dlg.CancelBtn.Visible = true;

                dlg.CancelButton = dlg.CancelBtn;

				dlg.saveToChangelistBtn.Visible = false;
				dlg.submitBtn.Visible = false;

				dlg.CheckoutAndSaveBtn.Visible = false;
				dlg.SkipSaveBtn.Visible = false;
				dlg.SaveAsBtn.Visible = false;
				dlg.CancelSaveBtn.Visible = false;

				dlg.CancelEditBtn.Visible = false;
				dlg.EditFileBtn.Visible = false;
				dlg.CheckoutBtn.Visible = false;

				dlg.BtnPanel.Controls.Clear();
				dlg.BtnPanel.Controls.Add(dlg.BtnPanelBuffer);
				//dlg.BtnPanel.Controls.Add(dlg.CancelEditBtn);
				dlg.BtnPanel.Controls.Add(dlg.CancelBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CancelSaveBtn);
				//dlg.BtnPanel.Controls.Add(dlg.SkipSaveBtn);
				//dlg.BtnPanel.Controls.Add(dlg.SaveAsBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CheckoutAndSaveBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CheckoutBtn);
				//dlg.BtnPanel.Controls.Add(dlg.EditFileBtn);
				//dlg.BtnPanel.Controls.Add(dlg.submitBtn);
				dlg.BtnPanel.Controls.Add(dlg.OkBtn);
				//dlg.BtnPanel.Controls.Add(dlg.saveToChangelistBtn);

				dlg.OkBtn.Column = 1;
				dlg.CancelBtn.Column = 2;
				dlg.BtnPanel.InitializeGrid(true);

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

		public static int ShowChooseChangelistMove(string prompt, IList<P4.Changelist> items, ref string NewChangeDescription,P4ScmProvider scm)
		{
            if (Preferences.LocalSettings.GetBool("PromptForChanglist", true))
            {
                if (ActiveChangeList > -1)
                {
                    CurrentChangeListLastUse = DateTime.Now;
                    //					NewChangeDescription = CurrentChangeDescription;
                    return CurrentChangeList;
                }
                SelectChangelistDlg dlg = new SelectChangelistDlg(prompt, items);

                dlg.DontShowAgainCB.Enabled = false;
                dlg.DontShowAgainCB.Visible = false;

                dlg.saveToChangelistBtn.Visible = false;
                dlg.submitBtn.Visible = false;

                dlg.NewChangelistDescription = NewChangeDescription;

                dlg.OkBtn.Visible = true;
                dlg.CancelBtn.Visible = true;

                dlg.CancelButton = dlg.CancelBtn;

                dlg.CheckoutAndSaveBtn.Visible = false;
                dlg.SkipSaveBtn.Visible = false;
                dlg.SaveAsBtn.Visible = false;
                dlg.CancelSaveBtn.Visible = false;

                dlg.CancelEditBtn.Visible = false;
                dlg.EditFileBtn.Visible = false;
                dlg.CheckoutBtn.Visible = false;

                dlg.BtnPanel.Controls.Clear();
                dlg.BtnPanel.Controls.Add(dlg.BtnPanelBuffer);
                //dlg.BtnPanel.Controls.Add(dlg.CancelEditBtn);
                dlg.BtnPanel.Controls.Add(dlg.CancelBtn);
                //dlg.BtnPanel.Controls.Add(dlg.CancelSaveBtn);
                //dlg.BtnPanel.Controls.Add(dlg.SkipSaveBtn);
                //dlg.BtnPanel.Controls.Add(dlg.SaveAsBtn);
                //dlg.BtnPanel.Controls.Add(dlg.CheckoutAndSaveBtn);
                //dlg.BtnPanel.Controls.Add(dlg.CheckoutBtn);
                //dlg.BtnPanel.Controls.Add(dlg.EditFileBtn);
                //dlg.BtnPanel.Controls.Add(dlg.submitBtn);
                dlg.BtnPanel.Controls.Add(dlg.OkBtn);
                //dlg.BtnPanel.Controls.Add(dlg.saveToChangelistBtn);

                dlg.OkBtn.Column = 1;
                dlg.CancelBtn.Column = 2;
                dlg.BtnPanel.InitializeGrid(true);

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
                    newChange.Files = null;

                    newChange.Description = dlg.DescriptionTB.Text;
                    newChange = scm.SaveChangelist(newChange, null);
                    return newChange.Id;
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

        public static int ShowChooseChangelistMoveFilesInChangelist(string prompt, IList<P4.Changelist> items, ref string NewChangeDescription, P4ScmProvider scm)
        {
            if (ActiveChangeList > -1)
            {
                CurrentChangeListLastUse = DateTime.Now;
                //					NewChangeDescription = CurrentChangeDescription;
                return CurrentChangeList;
            }
            SelectChangelistDlg dlg = new SelectChangelistDlg(prompt, items);

            dlg.DontShowAgainCB.Enabled = false;
            dlg.DontShowAgainCB.Visible = false;

            dlg.saveToChangelistBtn.Visible = false;
            dlg.submitBtn.Visible = false;

            dlg.NewChangelistDescription = NewChangeDescription;

            dlg.OkBtn.Visible = true;
            dlg.CancelBtn.Visible = true;

            dlg.CancelButton = dlg.CancelBtn;

            dlg.CheckoutAndSaveBtn.Visible = false;
            dlg.SkipSaveBtn.Visible = false;
            dlg.SaveAsBtn.Visible = false;
            dlg.CancelSaveBtn.Visible = false;

            dlg.CancelEditBtn.Visible = false;
            dlg.EditFileBtn.Visible = false;
            dlg.CheckoutBtn.Visible = false;

			dlg.BtnPanel.Controls.Clear();
			dlg.BtnPanel.Controls.Add(dlg.BtnPanelBuffer);
			//dlg.BtnPanel.Controls.Add(dlg.CancelEditBtn);
			dlg.BtnPanel.Controls.Add(dlg.CancelBtn);
			//dlg.BtnPanel.Controls.Add(dlg.CancelSaveBtn);
			//dlg.BtnPanel.Controls.Add(dlg.SkipSaveBtn);
			//dlg.BtnPanel.Controls.Add(dlg.SaveAsBtn);
			//dlg.BtnPanel.Controls.Add(dlg.CheckoutAndSaveBtn);
			//dlg.BtnPanel.Controls.Add(dlg.CheckoutBtn);
			//dlg.BtnPanel.Controls.Add(dlg.EditFileBtn);
			//dlg.BtnPanel.Controls.Add(dlg.submitBtn);
			dlg.BtnPanel.Controls.Add(dlg.OkBtn);
			//dlg.BtnPanel.Controls.Add(dlg.saveToChangelistBtn);

			dlg.OkBtn.Column = 1;
			dlg.CancelBtn.Column = 2;
			dlg.BtnPanel.InitializeGrid(true);

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
                if (newChange != null)
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


	    public static P4.Changelist ShowChooseChangelistSubmit(string prompt, IList<P4.Changelist> items,
            ref string NewChangeDescription, out string action, bool streams,bool integ, P4ScmProvider scm)
		{
			action = null;
            P4.Changelist changelist = scm.Connection.Repository.NewChangelist();
            // Make sure files is empty. If default has files in it, a new changelist
            // will automatically get those files.
            changelist.Files = new List<P4.FileMetaData>();
            if (Preferences.LocalSettings.GetBool("PromptForChanglist", true)||
                integ==true)
			{
				SelectChangelistDlg dlg = new SelectChangelistDlg(prompt, items);

				dlg.saveToChangelistBtn.Enabled = true;
				dlg.saveToChangelistBtn.Visible = true;
				dlg.submitBtn.Enabled = true;
				dlg.submitBtn.Visible = true;

				if (streams)
				{
					dlg.CancelBtn.Visible = false;
					dlg.CancelBtn.Enabled = false;
					dlg.ControlBox = false;

					dlg.BtnPanel.Controls.Clear();
					dlg.BtnPanel.Controls.Add(dlg.BtnPanelBuffer);
					dlg.BtnPanel.Controls.Add(dlg.saveToChangelistBtn);
					dlg.BtnPanel.Controls.Add(dlg.submitBtn);

					dlg.saveToChangelistBtn.Column = 1;
					dlg.submitBtn.Column = 2;
					dlg.submitBtn.Column = 3;
					dlg.BtnPanel.InitializeGrid(true);
				}
				else
				{
					dlg.CancelBtn.Visible = true;

					dlg.BtnPanel.Controls.Clear();
					dlg.BtnPanel.Controls.Add(dlg.BtnPanelBuffer);
					dlg.BtnPanel.Controls.Add(dlg.saveToChangelistBtn);
					dlg.BtnPanel.Controls.Add(dlg.submitBtn);
					dlg.BtnPanel.Controls.Add(dlg.CancelBtn);

					dlg.saveToChangelistBtn.Column = 1;
					dlg.submitBtn.Column = 2;
					dlg.submitBtn.Column = 3;
					dlg.CancelBtn.Column = 4;
					dlg.BtnPanel.InitializeGrid(true);
				}
				dlg.NewChangelistDescription = NewChangeDescription;
				
                dlg.CancelButton = dlg.CancelBtn;

				dlg.OkBtn.Visible = false;

				dlg.CheckoutAndSaveBtn.Visible = false;
				dlg.SkipSaveBtn.Visible = false;
				dlg.SaveAsBtn.Visible = false;
				dlg.CancelSaveBtn.Visible = false;

				dlg.CancelEditBtn.Visible = false;
				dlg.EditFileBtn.Visible = false;
				dlg.CheckoutBtn.Visible = false;

				//dlg.BtnPanel.Controls.Add(dlg.CancelEditBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CancelBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CancelSaveBtn);
				//dlg.BtnPanel.Controls.Add(dlg.SkipSaveBtn);
				//dlg.BtnPanel.Controls.Add(dlg.SaveAsBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CheckoutAndSaveBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CheckoutBtn);
				//dlg.BtnPanel.Controls.Add(dlg.EditFileBtn);

                if (dlg.ShowDialog() == DialogResult.Cancel)
                {
                    return null;
                }

                if (dlg.DialogResult == DialogResult.OK)
                {
                    // Save to Changelist
                    changelist.Description = dlg.DescriptionTB.Text;
                    return changelist;
                }

                if (dlg.DialogResult == DialogResult.Yes)
                {
                    // Submit...
                    action = "submit";
                    changelist.Description = dlg.DescriptionTB.Text;
                    return changelist;
                }

                int changelistId = dlg.Result;
                if (changelistId == -1)
                {
                    changelist.Description = dlg.DescriptionTB.Text;
                }
                return changelist;
            }
            else
            {
                changelist = P4VsProvider.CurrentScm.GetChangelist(P4VsProvider.Instance.ChangeLists.ActiveChangeList, null);
                return changelist;
            }
		}

		private static int LastQueryEditResult = None;
		public static int LastQueryEditChangelistId = 0;
		private static string LastQueryEditChangeDesc = Resources.SelectChangelistDlg_DefaultNewChangeDesc;
		private static DateTime LastQueryEditTime = DateTime.MinValue;

		public static int ShowQueryEdit(string fileName, IList<P4.Changelist> items, bool allowInMemeoryEdit, out int changelistId, out string NewChangeDescription)
		{
			NewChangeDescription = null;
            changelistId = P4VsProvider.Instance.ChangeLists.ActiveChangeList;

			if (DateTime.Now - LastQueryEditTime < TimeSpan.FromSeconds(2))
			{
				changelistId = LastQueryEditChangelistId;
				if (changelistId == -1)
				{
					NewChangeDescription = LastQueryEditChangeDesc;
				}
				LastQueryEditTime = DateTime.Now;
				return LastQueryEditResult;
			}

			if (Preferences.LocalSettings.GetBool("PromptForChanglist", true))
			{
				string prompt = string.Format(
					Resources.SelectChangelistDlg_QueryEditOrSavePrompt,
					System.IO.Path.GetFileName(fileName));
				SelectChangelistDlg dlg = new SelectChangelistDlg(prompt, items);

				dlg.saveToChangelistBtn.Visible = false;
				dlg.submitBtn.Visible = false;
				
				dlg.OkBtn.Visible = false;
				dlg.CancelBtn.Visible = false;

				dlg.CheckoutAndSaveBtn.Visible = false;
				dlg.SkipSaveBtn.Visible = false;
				dlg.SaveAsBtn.Visible = false;
				dlg.CancelSaveBtn.Visible = false;

				dlg.CancelEditBtn.Visible = true;

                dlg.CancelButton = dlg.CancelEditBtn;

				dlg.EditFileBtn.Visible = true;
				dlg.CheckoutBtn.Visible = true;
				dlg.ShowApplyToAllChk = true;
				dlg.ApplyToAllChk.Text = Resources.SelectChangelistDlg_ApplyToAllFilesEditLabel;

				dlg.EditFileBtn.Enabled = allowInMemeoryEdit;

				dlg.BtnPanel.Controls.Clear();
				dlg.BtnPanel.Controls.Add(dlg.BtnPanelBuffer);
				dlg.BtnPanel.Controls.Add(dlg.CheckoutBtn);
				dlg.BtnPanel.Controls.Add(dlg.EditFileBtn);
				dlg.BtnPanel.Controls.Add(dlg.CancelEditBtn);

				//dlg.BtnPanel.Controls.Add(dlg.saveToChangelistBtn);
				//dlg.BtnPanel.Controls.Add(dlg.submitBtn);
				//dlg.BtnPanel.Controls.Add(dlg.OkBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CancelBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CancelSaveBtn);
				//dlg.BtnPanel.Controls.Add(dlg.SkipSaveBtn);
				//dlg.BtnPanel.Controls.Add(dlg.SaveAsBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CheckoutAndSaveBtn);

				dlg.CheckoutBtn.Column = 1;
				dlg.EditFileBtn.Column = 2;
				dlg.CancelEditBtn.Column = 3;
				dlg.BtnPanel.InitializeGrid(true);

                dlg.ItemsCB.Select();

                DialogResult result = dlg.ShowDialog();

				if (dlg.ApplyToAllChk.Checked)
				{
					LastQueryEditTime = DateTime.Now;
				}

				if (result == DialogResult.Cancel)
				{
					LastQueryEditResult = Cancel;
					return Cancel;
				}
				if (result == DialogResult.Ignore)
				{
					LastQueryEditResult = EditInMemory;
					return EditInMemory;
				}
				changelistId = dlg.Result;
				LastQueryEditChangelistId = changelistId;
				if (changelistId == -1)
				{
					NewChangeDescription = dlg.DescriptionTB.Text;
					LastQueryEditChangeDesc = NewChangeDescription;
				}
				LastQueryEditResult = Checkout;
				return Checkout;
			}
			else
			{
				return Checkout;
			}
		}

		private static int LastQuerySaveResult = None;
		public static int LastQuerySaveChangelistId = 0;
		private static string LastQuerySaveChangeDesc = Resources.SelectChangelistDlg_DefaultNewChangeDesc;
		private static DateTime LastQuerySaveTime = DateTime.MinValue;

		public static int ShowQuerySave(string fileName, IList<P4.Changelist> items, out int changelistId, out string NewChangeDescription)
		{
			NewChangeDescription = null;
            changelistId = P4VsProvider.Instance.ChangeLists.ActiveChangeList;

			//if ((items == null) || (items.Count <= 0))
			//{
			//    return Checkout;
			//}
			if (DateTime.Now - LastQuerySaveTime < TimeSpan.FromSeconds(2))
			{
				changelistId = LastQuerySaveChangelistId;
				if (changelistId == -1)
				{
					NewChangeDescription = LastQuerySaveChangeDesc;
				}
				LastQuerySaveTime = DateTime.Now;
				return LastQuerySaveResult;
			}
			if (Preferences.LocalSettings.GetBool("PromptForChanglist", true))
			{
				string prompt = string.Format(
					Resources.SelectChangelistDlg_QueryEditOrSavePrompt,
					System.IO.Path.GetFileName(fileName));
				SelectChangelistDlg dlg = new SelectChangelistDlg(prompt, items);

				dlg.saveToChangelistBtn.Visible = false;
				dlg.submitBtn.Visible = false;

				dlg.OkBtn.Visible = false;
				dlg.CancelBtn.Visible = false;

				dlg.CheckoutAndSaveBtn.Visible = true;
				dlg.SkipSaveBtn.Visible = true;
				dlg.SaveAsBtn.Visible = true;
				dlg.CancelSaveBtn.Visible = true;

                dlg.CancelButton = dlg.CancelSaveBtn;

				dlg.ShowApplyToAllChk = true;
				dlg.ApplyToAllChk.Text = Resources.SelectChangelistDlg_ApplyToAllFilesSaveLabel;

				dlg.CancelEditBtn.Visible = false;
				dlg.EditFileBtn.Visible = false;
				dlg.CheckoutBtn.Visible = false;

				dlg.BtnPanel.Controls.Clear();
				dlg.BtnPanel.Controls.Add(dlg.BtnPanelBuffer);
				dlg.BtnPanel.Controls.Add(dlg.CheckoutAndSaveBtn);
				dlg.BtnPanel.Controls.Add(dlg.SkipSaveBtn);
				dlg.BtnPanel.Controls.Add(dlg.SaveAsBtn);
				dlg.BtnPanel.Controls.Add(dlg.CancelSaveBtn);

				//dlg.BtnPanel.Controls.Add(dlg.CheckoutBtn);
				//dlg.BtnPanel.Controls.Add(dlg.EditFileBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CancelEditBtn);
				//dlg.BtnPanel.Controls.Add(dlg.saveToChangelistBtn);
				//dlg.BtnPanel.Controls.Add(dlg.submitBtn);
				//dlg.BtnPanel.Controls.Add(dlg.OkBtn);
				//dlg.BtnPanel.Controls.Add(dlg.CancelBtn);

				dlg.CheckoutAndSaveBtn.Column = 1;
				dlg.SkipSaveBtn.Column = 2;
				dlg.SaveAsBtn.Column = 3;
				dlg.CancelSaveBtn.Column = 4;
				dlg.BtnPanel.InitializeGrid(true);

				DialogResult result = dlg.ShowDialog();

				if (dlg.ApplyToAllChk.Checked)
				{
					LastQuerySaveTime = DateTime.Now;
				}

				if (result == DialogResult.Cancel)
				{
					LastQuerySaveResult = Cancel;
					return Cancel;
				}
				else if (result == DialogResult.Ignore)
				{
					LastQuerySaveResult = Ignore;
					return Ignore;
				}
				else if (result == DialogResult.Retry)
				{
					LastQuerySaveResult = SaveAs;
					return SaveAs;
				}
				changelistId = dlg.Result;
				LastQuerySaveChangelistId = changelistId;
				if (changelistId == -1)
				{
					NewChangeDescription = dlg.DescriptionTB.Text;
					LastQuerySaveChangeDesc = NewChangeDescription;
				}
				LastQuerySaveResult = Checkout;
				return Checkout;
			}
			else
			{
				return Checkout;
			}
		}
		/*
		 * CheckoutAndSaveBtn
		 * SkipSaveBtn
		 * SaveAsBtn
		 * CancelSaveBtn
		 */

		private Dictionary<int, P4.Changelist> changeMap;

		public SelectChangelistDlg(string prompt, IList<P4.Changelist> items)
		{
			PreferenceKey = "SelectChangelistDlg";

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
			changeMap[0] = null;

			if ((items != null) && (items.Count > 0))
			{
				foreach (P4.Changelist item in items)
				{
					int id = item.Id;
					if ((id != -1) && (id != 0))
					{
						changeMap[id] = item;
						int idx = ItemsCB.Items.Add(ChangeListToString(item));
					}
				}
			}
			// select the default changelist
			ItemsCB.SelectedIndex = 1;

			OkBtn.Enabled = ItemsCB.SelectedIndex >= 0;
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

		public static string ChangeListToString(P4.Changelist change)
		{
		    string line = change.Description;
		    line = line.Replace('\r', ' ');
            line = line.Replace('\n', ' ');
			return string.Format("{0} {1}", change.Id, line);
		}

		private void ChooseDlg_HelpButtonClicked(object sender, CancelEventArgs e)
		{

		}

		private void DontShowAgainCB_CheckedChanged(object sender, EventArgs e)
		{
			Preferences.LocalSettings["PromptForChanglist"] = !DontShowAgainCB.Checked;
		}

		//int newChangelistId = 0;

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
                int idx = ((string)ItemsCB.SelectedItem).IndexOf(' ');
                string str = ((string)ItemsCB.SelectedItem).Substring(0, idx);

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
            if
                (OkBtn.Enabled)
            {
                label1.ForeColor = SystemColors.ControlText;
            }
            else
            {
                label1.ForeColor = Color.Red;
            }
		}

		public bool ShowApplyToAllChk
		{
			get { return ApplyToAllChk.Visible; }
			set
			{
				ApplyToAllChk.Visible = value;
				if ((value) && (DontShowAgainCB.Location.Y == ApplyToAllChk.Location.Y))
				{
					DescriptionTB.Size = new Size(DescriptionTB.Size.Width, DescriptionTB.Size.Height - 20);
					DontShowAgainCB.Location = new Point(DontShowAgainCB.Location.X, DontShowAgainCB.Location.Y - 20);
				}
				else if ((value == false) && (DontShowAgainCB.Location.Y != ApplyToAllChk.Location.Y))
				{
					DescriptionTB.Size = new Size(DescriptionTB.Size.Width, DescriptionTB.Size.Height + 20);
					DontShowAgainCB.Location = new Point(DontShowAgainCB.Location.X, ApplyToAllChk.Location.Y);
				}
			}
		}

		private void ApplyToAllChk_CheckedChanged(object sender, EventArgs e)
		{
			SaveAsBtn.Enabled = ApplyToAllChk.Checked == false;
		}

		private bool LayoutBtnPanelOnce = true;

		private void gridLayoutPanel1_AfterLayoutGrid()
		{
			if (LayoutBtnPanelOnce)
			{
				BtnPanel.LayoutGrid();
				LayoutBtnPanelOnce = false;
			}
		}
	}
}
