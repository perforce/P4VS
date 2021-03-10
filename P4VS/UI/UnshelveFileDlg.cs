using Perforce.P4Scm;
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
	public partial class UnshelveFileDialog : AutoSizeForm
	{
        private class FileListViewSorter : System.Collections.IComparer
        {
            #region IComparer<ListViewItem> Members

            public int Compare(object xo, object yo)
            {
                ListViewItem x = xo as ListViewItem;
                ListViewItem y = yo as ListViewItem;

                if (x == null)
                {
                    throw new ArgumentNullException("x");
                }

                if (y == null)
                {
                    throw new ArgumentNullException("y");
                }

                if (x.Checked && !y.Checked)
                    return -1;
                if (y.Checked && !x.Checked)
                    return 1;
                return string.Compare(x.SubItems[1].Text, y.SubItems[1].Text);
            }

            #endregion
        }

		private delegate void DeleteShelvedFilesDelegate(P4ScmProvider scm, int changelistId, string[] files);

		private static void DeleteShelvedFilesCallback(P4ScmProvider scm, int changelistId, string[] files)
		{
			P4.ShelveFilesCmdFlags dflags = P4.ShelveFilesCmdFlags.Delete;
			scm.ShelveFiles(changelistId, null, dflags, true, files);
		}

		public static void UnshelveFiles(P4ScmProvider scm, 
					int changelistId, IList<P4.ShelvedFile> selectedShelvedFiles)
		{
			UnshelveFileDialog dlg = new UnshelveFileDialog(scm);
			dlg.ChangelistId = changelistId;

			string description = Resources.UnshelveFileDialog_DefaultChangelistDescription;
			dlg.Description = description;

			if (dlg.ShelvedFilesLV.Items.Count <= 0)
			{
				MessageBox.Show(Resources.UnshelveFileDialog_NoShelvedFilesWarning, Resources.P4VS, 
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			dlg.SelectedFileList = selectedShelvedFiles;

			if (DialogResult.Cancel == dlg.ShowDialog())
			{
				return;
			}
			int targetChangeList = dlg.TargetChangelist;
			if (targetChangeList < 0)
			{
				description = dlg.Description;
			}

			IList<string> selectedFiles = dlg.GetSelectedFiles();
			bool revertFirst = (dlg.RevertBeforeUnshelving);

			P4.UnshelveFilesCmdFlags flags = P4.UnshelveFilesCmdFlags.None;
			if (dlg.RevertBeforeUnshelving)
			{
				scm.RevertFiles(false, true, null, selectedFiles.ToArray());
				revertFirst = true;
			}

			flags = P4.UnshelveFilesCmdFlags.Preview;
			bool success = scm.UnshelveFiles(changelistId, targetChangeList, description, flags, true, revertFirst, selectedFiles.ToArray());
			if (success == false)
			{
				UnshelveFiles(scm, changelistId, selectedShelvedFiles);
				return;
			}

			flags = P4.UnshelveFilesCmdFlags.None;
			if (dlg.OverwriteWritableFiles)
			{
				flags = P4.UnshelveFilesCmdFlags.Force;
			}

			if (dlg.DeleteAfterUnshelve)
			{
				// queue a message to delete the files later because if the unshelve changes some
				// key files like c++ filters, te project will be rleoaded and we might not return
				// to this operation
				DeleteShelvedFilesDelegate d = new DeleteShelvedFilesDelegate(DeleteShelvedFilesCallback);
				scm.SccService.UiDispatcher.BeginInvoke(d, scm, changelistId, selectedFiles.ToArray());
			}
            IList <P4.FileSpec>  files = new List<P4.FileSpec>();
            foreach(string depotPath in selectedFiles)
            {
                P4.FileSpec file = new P4.FileSpec();
                file.DepotPath=new P4.DepotPath(depotPath);
                files.Add(file);
            }
            IList<P4.File> opened =  scm.GetOpenedFiles(files, null);
			scm.UnshelveFiles(changelistId, targetChangeList, description, flags, true, revertFirst, selectedFiles.ToArray());

            // Check for opened files that might need a changelist refresh
            // if the unshelve moves the checked out file to a different
            // changelist. Disregard if no files are checked out that are
            // also being unshelved.
            if (opened!=null)
            {
                int[] changesToRefresh = new int[opened.Count];
                if (opened != null)
                {
                    for (int i = 0; i < opened.Count; i++)
                    {
                        changesToRefresh[i] = opened[i].ChangeId;
                    }
                    changesToRefresh = changesToRefresh.Distinct().ToArray();
                }
                foreach (int changeId in changesToRefresh)
                {
                    scm.BroadcastChangelistUpdate(dlg,
        new P4ScmProvider.ChangelistUpdateArgs(changeId, P4ScmProvider.ChangelistUpdateArgs.UpdateType.Submitted));
                }
            }
            return;
		}
		private UnshelveFileDialog()
		{
			PreferenceKey = "UnshelveFileDialog";
		}

		private System.Windows.Forms.ImageList ListImages;

		public UnshelveFileDialog(P4ScmProvider scm)
		{
			PreferenceKey = "UnshelveFileDialog";

			_scm = scm;
			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
			ListImages = new System.Windows.Forms.ImageList(components);
			// 
			// ListImages
			// 
			ListImages.TransparentColor = System.Drawing.Color.Transparent;
            ListImages.Images.Add("noCheckBox.png", Images.noCheckBox);
            ListImages.Images.Add("CheckBox.png", Images.CheckBox);

            ShelvedFilesLV.SmallImageList = ListImages;
            ShelvedFilesLV.ListViewItemSorter = (System.Collections.IComparer)new FileListViewSorter();

			IList<P4.Changelist> changes = _scm.GetAvailibleChangelists(-1);

			changeMap = new Dictionary<int, P4.Changelist>();

			//add new as the first item in the list 
			TargetChangelistCB.Items.Add(Resources.Changelist_New);
			changeMap[-1] = null;

			//add default as the first item in the list if not already there
			TargetChangelistCB.Items.Add(Resources.Changelist_Default);
			changeMap[0] = null;

			// if there are other existing changelists, add them to the list
			if (changes != null)
			{
				foreach (P4.Changelist item in changes)
				{
					int id = item.Id;
					if ((id != -1) && (id != 0))
					{
						changeMap[id] = item;
						int idx = TargetChangelistCB.Items.Add(SelectChangelistDlg.ChangeListToString(item));
					}
				}
			}
			
			// select the default changelist
			TargetChangelistCB.SelectedIndex = 1;
		}

		private Dictionary<int, P4.Changelist> changeMap;

		public string Description { get; set; }

		public int TargetChangelist
		{
			get 
			{
				if (TargetChangelistCB.SelectedIndex > 1)
				{
					string c= TargetChangelistCB.Text;
					int idx = c.IndexOf(" ");
					c = c.Remove(idx);
					int cID = Convert.ToInt32(c);
					return cID;
				}
				else if (TargetChangelistCB.SelectedIndex == 0)
				{
					return -1;
				}
				return 0;
			}
		}
		public bool RevertBeforeUnshelving
		{
			get { return RevertCB.Checked; }
			set { RevertCB.Checked = value; }
		}
		public bool OverwriteWritableFiles
		{
			get { return OverwriteWritableFilesTB.Checked; }
			set { OverwriteWritableFilesTB.Checked = value; }
		}

		public bool DeleteAfterUnshelve
		{
			get { return DeleteAfterUnshelveCB.Checked; }
			set { DeleteAfterUnshelveCB.Checked = value; }
		}

		private P4ScmProvider _scm = null;

		private int _changelistId = 0;

		// map from depot path to list view item
		private IDictionary<P4.DepotPath, ListViewItem> listItemMap = null;

		// map from depot path to meta data
		private IDictionary<P4.DepotPath, P4.ShelvedFile> shelvedFileDataMap = null;

		private string _PromptTextTemplate = null;

		public int ChangelistId
		{
			get { return _changelistId; }
			set
			{
				_changelistId = value;

				P4.Changelist changeList = null;

				if (_PromptTextTemplate == null)
				{
					_PromptTextTemplate = PromptLbl.Text;
				}
				if (_changelistId > 0)
				{
					PromptLbl.Text = String.Format(_PromptTextTemplate, _changelistId.ToString());
					changeList = _scm.GetChangelist(_changelistId);

					TargetChangelistCB.SelectedIndex = 0;

					string changelistIdStr = string.Format("{0} ", _changelistId);
					for (int idx = 0; idx < TargetChangelistCB.Items.Count; idx++)
					{
						if (TargetChangelistCB.Items[idx].ToString().StartsWith(changelistIdStr))
						{
							TargetChangelistCB.SelectedIndex = idx;
							break;
						}
					}
				}
				else if (_changelistId == 0)
				{
					TargetChangelistCB.SelectedIndex = 1;

					throw new ArgumentOutOfRangeException("ChangelistId", "Must reference a numbered changelist");
				}

				ShelvedFilesLV.Items.Clear();

				if (_changelistId > 0)
				{
					P4.Options describeOptions = new P4.DescribeCmdOptions(P4.DescribeChangelistCmdFlags.Shelved | P4.DescribeChangelistCmdFlags.Omit, -1, -1);
					P4.Changelist sc = _scm.GetChangelist(_changelistId, describeOptions);
					if ((sc != null) && (sc.ShelvedFiles != null) && (sc.ShelvedFiles.Count > 0))
					{
						listItemMap = new Dictionary<P4.DepotPath, ListViewItem>();
						shelvedFileDataMap = new Dictionary<P4.DepotPath, P4.ShelvedFile>();

						foreach (P4.ShelvedFile shelf in sc.ShelvedFiles)
						{
							string fileName = System.IO.Path.GetFileName(shelf.Path.Path.ToString());
							string folder = System.IO.Path.GetDirectoryName(shelf.Path.Path.ToString());
							ListViewItem fileitem = new ListViewItem(string.Empty);
							fileitem.SubItems.Add(fileName);
							fileitem.SubItems.Add(folder);
							fileitem.SubItems.Add(shelf.Action.ToString());

							P4.FileMetaData fmd = _scm.GetFileMetaData(shelf.Path.Path);

							if ((fmd != null) && (fmd.Action != P4.FileAction.None))
							{
								fileitem.SubItems.Add("*");
							}
							else
							{
								fileitem.SubItems.Add(string.Empty);
							}
							//fileitem.ImageIndex = 3;
							fileitem.Tag = shelf;
							ShelvedFilesLV.Items.Add(fileitem);

							listItemMap[shelf.Path] = fileitem;
							shelvedFileDataMap[shelf.Path] = shelf;
						}
					}
					//for (int idx = 2; idx < TargetChangelistCB.Items.Count; idx++)
					//{
					//    TargetChangelistCB.SelectedIndex = 1;
					//    if ((TargetChangelistCB.Items[idx] is int) && 
					//        ((int)TargetChangelistCB.Items[idx] == _changelistId))
					//    {
					//        TargetChangelistCB.SelectedIndex = idx;
					//        break;
					//    }
					//}
				}
				SelectAllCB.Checked = false;
			}
		}

		public IList<P4.ShelvedFile> SelectedFileList
		{
			get
			{
				List<P4.ShelvedFile> value = new List<P4.ShelvedFile>();

				foreach (ListViewItem item in ShelvedFilesLV.Items)
				{
					if (item.Checked)
					{
						value.Add((P4.ShelvedFile)item.Tag);
					}
				}
				return value;
			}
			set
			{
				foreach (ListViewItem item in ShelvedFilesLV.Items)
				{
					item.Checked = false;
				}
				if ((value == null) || (value.Count <= 0))
				{
					return;
				}
				int checkedCnt = 0;
				foreach (P4.ShelvedFile file in value)
				{
					if (file!=null)
					{
						if (listItemMap.ContainsKey(file.Path))
						{
							ListViewItem item = listItemMap[file.Path];
							item.Checked = true;
							checkedCnt++;
						}
					}
				}
				SelectAllCB.Checked =  (ShelvedFilesLV.Items.Count == checkedCnt);
			}
		}

		public IList<string> GetSelectedFiles()
		{
			List<string> value = new List<string>();

			foreach (ListViewItem item in ShelvedFilesLV.Items)
			{
				if (item.Checked == true)
				{
					value.Add(((P4.ShelvedFile)item.Tag).Path.Path);
				}
			}
			if (value.Count > 0)
			{
				return value;
			}
			return null;
		}

		private void CancelBtn_Click(object sender, EventArgs e)
		{

		}

		private void ShelveBtn_Click(object sender, EventArgs e)
		{
			//if (TargetChangelist == 0)
			//{
			//    ShelveFileCreateChangelistDlg dlg = new ShelveFileCreateChangelistDlg();

			//    if (dlg.ShowDialog() == DialogResult.Cancel)
			//    {
			//        return;
			//    }
			//    Description = dlg.Description;
			//}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void SelectAllCB_CheckedChanged(object sender, EventArgs e)
		{
            // turn off the individual item check handler, so it doesn't try to change the
			// state of the check all check box
			ShelvedFilesLV.ItemChecked += new ItemCheckedEventHandler(ChangelistFilesLV_ItemChecked);
			try
			{
				ShelveBtn.Enabled = SelectAllCB.Checked;
                if (SelectAllCB.Checked)
                {
                    foreach (ListViewItem item in ShelvedFilesLV.Items)
                    {
                        item.Checked = true;
                    }

                }
                else
                {
                    foreach (ListViewItem item in ShelvedFilesLV.Items)
                    {
                        item.Checked = false;
                    }
                }
			}
			finally
			{
				// turn back on the individual item check handler
				ShelvedFilesLV.ItemChecked += new ItemCheckedEventHandler(ChangelistFilesLV_ItemChecked);
			}
		}

		private void ChangelistFilesLV_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
            if (e.Item.Checked == true)
            {
                e.Item.ImageIndex = 1;
            }
            else
            {
                e.Item.ImageIndex = 0;
            }
                // turn off the select all checked event, so it won't effect other controls
            SelectAllCB.CheckedChanged -= new EventHandler(SelectAllCB_CheckedChanged);
            try
            {
                SelectAllCB.Checked = true;
                ShelveBtn.Enabled = false;
                foreach (ListViewItem item in ShelvedFilesLV.Items)
                {
                    ShelveBtn.Enabled |= item.Checked;
                    SelectAllCB.Checked &= item.Checked;
                }

            }
            finally
            {
                // turn back on the select all checked event, so it won't effect other controls
                SelectAllCB.CheckedChanged += new EventHandler(SelectAllCB_CheckedChanged);
            }
		}

		private void FileListMnu_Opening(object sender, CancelEventArgs e)
		{
			FLM_ViewMI.Visible = ((ShelvedFilesLV.SelectedItems != null) && (ShelvedFilesLV.SelectedItems.Count > 0));
		}

		private void FileListMnu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Name == "FLM_ViewMI")
			{
                foreach (ListViewItem it in ShelvedFilesLV.SelectedItems)
                {
                    P4.ShelvedFile shelvedFile = it.Tag as P4.ShelvedFile;
                    if (shelvedFile != null)
                    {
                        string file = shelvedFile.Path.ToString();

                        long maxPreviewSize = 1024 * ((long)Preferences.LocalSettings.GetInt("Size_files", 500));
                        long shelvedSize = _scm.GetShelvedFileSize(_changelistId, file);
                        if (shelvedSize > maxPreviewSize)
                        {
                            string size = P4ObjectTreeListViewItem.PrettyPrintFileSize(shelvedSize);
                            string msg = string.Format(Resources.FileExceedsMaxPreviewSizeWarning, size);
                            if (DialogResult.No == MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                            {
                                return;
                            }
                        }

                        P4.FileSpec shelvedSpec = new P4.FileSpec(shelvedFile.Path,
                                                        new P4.ShelvedInChangelistIdVersion(_changelistId));

                        using (TempFile sourceFile = new TempFile(shelvedSpec))
                        {
                            if (_scm.GetFileVersion(sourceFile, shelvedSpec) == null)
                            {
                                MessageBox.Show(Resources.UnshelveFileDialog_CannotGetShelvedFileContentsError,
                                    Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            if (Preferences.LocalSettings.GetBool("OpenShelvedFileInEditor", true))
                            {
                                EnvDTE.DTE dte = P4VsProvider.GetDTE();
                                dte.ItemOperations.OpenFile(sourceFile, null);
                            }
                            else
                            {
                                ShowFileContentsDlg dlg = new ShowFileContentsDlg();

                                dlg.TempFile = sourceFile;
                                dlg.Title = shelvedSpec.ToString();

                                // Show modeless
                                dlg.Show();
                            }
                        }
                    }
                }
			}
		}
	}
}
