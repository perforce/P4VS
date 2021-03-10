using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Perforce.P4;
using NLog;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class ShelveFileDlg : AutoSizeForm
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
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

		private System.Windows.Forms.ImageList ListImages;

		public static void ShelveFiles(IList<string> files, P4ScmProvider Scm, bool ShowUi)
		{
			Dictionary<int, IDictionary<string, P4.FileMetaData>> changeLists =
                new Dictionary<int, IDictionary<string, P4.FileMetaData>>();

		    bool shelvedFiles = false;

			for (int idx = 0; idx < files.Count; idx++)
			{
				int changeListId = -1;

				if (files[idx] != null && (files[idx].EndsWith("...") || files[idx].EndsWith("*")))
				{
					List<P4.FileSpec> fileSpecs = new List<P4.FileSpec>();
					fileSpecs.Add(P4.FileSpec.DepotSpec(files[idx]));
                    IList<P4.File> opened = Scm.Connection.Repository.GetOpenedFiles(fileSpecs, null);
					IList<P4.FileSpec> openedSpecs = new List<P4.FileSpec>();
					foreach (P4.File f in opened)
					{
						if ((f != null) && (f.DepotPath != null))
						{
							openedSpecs.Add(P4.FileSpec.ClientSpec(f.ClientPath.Path));
						}
					}
					IList<P4.FileMetaData> metadata = Scm.ListFileMetaData(null, openedSpecs);
					foreach (P4.FileMetaData f in metadata)
					{
                        if (f != null)
                        {
                            changeListId = f.Change;

                            if (changeLists.ContainsKey(changeListId) == false)
                            {
                                changeLists.Add(changeListId, new Dictionary<string, P4.FileMetaData>());
                            }
                            if (changeLists[changeListId].ContainsKey(f.LocalPath.Path) == false)
                            {
                                changeLists[changeListId].Add(f.LocalPath.Path, f);
                            }
                            if (f.MovedFile != null)
                            {
                                P4.FileMetaData movedFmd = Scm.GetFileMetaData(f.MovedFile.Path);
                                if ((movedFmd != null) && (movedFmd.LocalPath != null) &&
                                    (changeLists[changeListId].ContainsKey(movedFmd.LocalPath.Path) == false))
                                {
                                    changeLists[changeListId].Add(movedFmd.LocalPath.Path, movedFmd);
                                }
                            }
                        }
					}

					continue;
				}
				P4.FileMetaData fmd = null;

				fmd = Scm.Fetch(files[idx]);
				if (fmd == null)
				{
					continue;
				}
				SourceControlStatus status = Scm.GetFileStatus(files[idx]);
				if (status == SourceControlStatus.scsCheckedIn)
				{
					continue;
				}

				changeListId = fmd.Change;

                // describe cl here and see if it is shelved
                if (changeListId > 0)
                {
                    Options opts = new Options();
                    opts["-S"] = null;
                    Changelist isChangeShelved = Scm.GetChangelist(changeListId, opts);

                    if (isChangeShelved.Shelved)
                    {
                        shelvedFiles = true;
                    }
                }

				if (changeLists.ContainsKey(changeListId) == false)
				{
					changeLists.Add(changeListId, new Dictionary<string, P4.FileMetaData>());
				}
				if (changeLists[changeListId].ContainsKey(fmd.LocalPath.Path) == false)
				{
					changeLists[changeListId].Add(fmd.LocalPath.Path, fmd);
				}
				if (fmd.MovedFile != null)
				{
					P4.FileMetaData movedFmd = Scm.GetFileMetaData(fmd.MovedFile.Path);
					if ((movedFmd != null) && (movedFmd.LocalPath != null) &&
						(changeLists[changeListId].ContainsKey(movedFmd.LocalPath.Path) == false))
					{
						changeLists[changeListId].Add(movedFmd.LocalPath.Path, movedFmd);
					}
				}
			}
			bool dlgShown = false;
			foreach (int changeListId in changeLists.Keys)
			{
				ShelveFileDlg dlg = new ShelveFileDlg(Scm);

                if (!(shelvedFiles))
                {
                    dlg.ClearChangelistTB.Checked = false;
                    dlg.ClearChangelistTB.Enabled = false;
                }
				dlg.ChangelistId = changeListId;
				dlg.SelectedFileList = changeLists[changeListId].Values.ToList();
                
				if (dlg.SelectedFileList.Count <= 0)
				{
					continue;
				}
				dlgShown = true;

				DialogResult res = DialogResult.OK;

                dlg.Description = Resources.ShelveFileDlg_FilesShelvedFromDefaultChangelist;
				if (changeListId > 0)
				{
					dlg.Description = string.Format(
						Resources.ShelveFileDlg_FilesShelvedFromNumberedChangelist, changeListId);
				}
				if (ShowUi)
				{
					res = dlg.ShowDialog();
				}
				if (res == DialogResult.Cancel)
				{
					return;
				}
				// If requested delete the existing shelved flags
				if ((changeListId > 0) && (dlg.ReplaceExistingShelvedFiles))
				{
					if (Scm.ShelveFiles(changeListId, null, P4.ShelveFilesCmdFlags.Delete, true, null) == false)
					{
						return;
					}
				}
				P4.ShelveFilesCmdFlags flags = P4.ShelveFilesCmdFlags.Force;
				if (dlg.DontShelveUnchangedFiles)
				{
					flags |= ShelveFilesCmdFlags.LeaveUnchanged;
				}
				// Shelve the files
				if (Scm.ShelveFiles(changeListId, dlg.Description, flags, dlg.SelectedFileList) == false)
				{
					return;
				}

				// If requested, revert the files after shelving

				if (dlg.RevertAfterShelving)
				{
					IList<string> selection = dlg.GetSelectedFiles();
					string[] selected = selection.ToArray();
					Scm.RevertFiles(false, true, null, selected);
				}
				Scm.BroadcastChangelistUpdate(null, new P4ScmProvider.ChangelistUpdateArgs(changeListId, P4ScmProvider.ChangelistUpdateArgs.UpdateType.ContentUpdate));
			}
			if (dlgShown == false)
			{
				MessageBox.Show(Resources.ShelveFileDlg_NoShelveableFilesWarning, Resources.PerforceSCM, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public static void ShelveFiles(IList<P4.FileMetaData> files, P4.Changelist changelist, P4ScmProvider ScmProvider, bool ShowUi)
		{
			ShelveFileDlg dlg = new ShelveFileDlg(ScmProvider);
            bool shelvedFiles = changelist.Shelved;

			dlg.ChangelistId = changelist.Id;
			if ((files != null) && (files.Count > 0))
			{
			    List<P4.FileMetaData> linkedFiles = new List<P4.FileMetaData>();
				foreach (P4.FileMetaData fmd in files)
				{
					if (fmd.MovedFile != null)
					{
						P4.FileMetaData movedFmd = ScmProvider.GetFileMetaData(fmd.MovedFile.Path);
						linkedFiles.Add(movedFmd);
					}
                   
				}
                foreach (P4.FileMetaData fmd in linkedFiles)
                {

                    files.Add(fmd);
                }
			    dlg.SelectedFileList = files;

                
			}
			if (dlg.SelectedFileList.Count <= 0)
			{
				dlg.SelectAllCB.Checked = true;
			}

            if (!(shelvedFiles))
            {
                dlg.ClearChangelistTB.Checked = false;
                dlg.ClearChangelistTB.Enabled = false;
            }
			DialogResult res = DialogResult.OK;

			dlg.Description = Resources.ShelveFileDlg_FilesShelvedFromDefaultChangelist;
			if (changelist.Id > 0)
			{
				dlg.Description = string.Format(
					Resources.ShelveFileDlg_FilesShelvedFromNumberedChangelist, changelist.Id);
			}
			if (ShowUi)
			{
				res = dlg.ShowDialog();
			}
			if (res == DialogResult.Cancel)
			{
				return;
			}
			// If requested delete the existing shelved flags
			if ((changelist.Id > 0) && (dlg.ReplaceExistingShelvedFiles))
			{
				if (ScmProvider.ShelveFiles(changelist.Id, null, P4.ShelveFilesCmdFlags.Delete, true, null) == false)
				{
					return;
				}
			}
			P4.ShelveFilesCmdFlags flags = P4.ShelveFilesCmdFlags.Force;
			if (dlg.DontShelveUnchangedFiles)
			{
				flags |= ShelveFilesCmdFlags.LeaveUnchanged;
			}
			// Shelve the files
			if (ScmProvider.ShelveFiles(changelist.Id, dlg.Description, flags, dlg.SelectedFileList) == false)
			{
				return;
			}

			// If requested, revert the files after shelving

			if (dlg.RevertAfterShelving)
			{
				ScmProvider.RevertFiles(false, true, null, dlg.GetSelectedFiles().ToArray());
			}
			ScmProvider.BroadcastChangelistUpdate(null, new P4ScmProvider.ChangelistUpdateArgs(changelist.Id, P4ScmProvider.ChangelistUpdateArgs.UpdateType.ContentUpdate));
		}
		private ShelveFileDlg()
		{
			PreferenceKey = "ShelveFileDlg";
		}

		public ShelveFileDlg(P4ScmProvider scm)
		{
			PreferenceKey = "ShelveFileDlg";

			_scm = scm;

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
			if (this.components == null)
			{
				this.components = new System.ComponentModel.Container();
			}
			this.ListImages = new System.Windows.Forms.ImageList(this.components);
			// 
			// ListImages
			// 
			ListImages.TransparentColor = System.Drawing.Color.Transparent;
            ListImages.Images.Add("noCheckBox.png", Images.noCheckBox);
            ListImages.Images.Add("CheckBox.png", Images.CheckBox);

		    ChangelistFilesLV.SmallImageList = ListImages;
			ChangelistFilesLV.ListViewItemSorter = (System.Collections.IComparer) new FileListViewSorter();
		}

		public string Description { get; set; }

		public bool RevertAfterShelving
		{
			get { return RevertCB.Checked; }
			set { RevertCB.Checked = value; }
		}
		public bool ReplaceExistingShelvedFiles
		{
			get { return ClearChangelistTB.Checked; }
			set { ClearChangelistTB.Checked = value; }
		}

		public bool DontShelveUnchangedFiles
		{
			get { return DontShelveUnchangeCB.Checked; }
			set { DontShelveUnchangeCB.Checked = value; }
		}

		private P4ScmProvider _scm = null;

		private int _changelistId = 0;

		// map from depot path to list view item
		private IDictionary<string, ListViewItem> listItemMap = null;

		// map from depot path to meta data
		private IDictionary<string, P4.FileMetaData> fileMetaDataMap = null;

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
					PromptLbl.Text = String.Format(_PromptTextTemplate, string.Empty, " " + _changelistId.ToString());
					changeList = _scm.GetChangelist(_changelistId);
				}
				else
				{
					PromptLbl.Text = String.Format(_PromptTextTemplate, Resources.Changelist_Default.ToLower() + " ", string.Empty);
					changeList = new P4.Changelist(0, true); ;
				}


				IList<P4.FileSpec> fileList = null;
				IList<P4.FileMetaData> mdl = null;

				listItemMap = new Dictionary<string, ListViewItem>();
				fileMetaDataMap = new Dictionary<string, P4.FileMetaData>();

				if (_changelistId > 0)
				{
					//it's either an existing numbered list
					if (changeList != null) mdl = changeList.Files;
				}
				else
				{
					// It's the 'default' change list to be turned into a new numbered changelist or
					//  or a new change list to be edited.
					P4.Options options = new P4.GetFileMetaDataCmdOptions(
						P4.GetFileMetadataCmdFlags.Opened,
						null, null, -1, null, "default", null);
                    string wsPath = string.Format("//{0}/...", _scm.Connection.Repository.Connection.Client.Name);
					// get files opened in default changelist
                    mdl = _scm.Connection.Repository.GetFileMetaData(options, new P4.FileSpec(new P4.ClientPath(wsPath), null));
				}

				fileList = new List<P4.FileSpec>();

				if ((mdl != null) && (mdl.Count > 0))
				{
					foreach (P4.FileMetaData file in mdl)
					{
						ListViewItem item = new ListViewItem(string.Empty);		// subitem 0

						P4.FileMetaData fileMd = _scm.GetFileMetaData(file.DepotPath.Path);
						item.Tag = fileMd;
						item.SubItems.Add(file.DepotPath.GetFileName());		// subitem 1
						item.SubItems.Add(file.DepotPath.GetDirectoryName());	// subitem 2
						item.SubItems.Add(file.Action.ToString());				// subitem 3, place holder for pending action 
						item.SubItems.Add(System.IO.Path.GetExtension(file.DepotPath.GetFileName()));// subitem 4
						if (_changelistId > 0)
						{
							// see if file is already shelved in this changelist
							P4.FileSpec lfs = new P4.FileSpec();
							lfs.DepotPath = new P4.DepotPath(file.DepotPath.Path);
							lfs.Version = new P4.ShelvedInChangelistIdVersion(_changelistId);

							P4.FileSpec shelvedfile = _scm.GetFile(null, lfs);

							if (shelvedfile != null)
							{
								item.SubItems.Add("*");
							}
							else
							{
								item.SubItems.Add(string.Empty);
							}
						}

						listItemMap.Add(file.DepotPath.Path, item);
						ChangelistFilesLV.Items.Add(item);

						fileMetaDataMap.Add(file.DepotPath.Path, fileMd);
					}
				}
#if DEBUG_DB
				int i = 0;
				foreach (ListViewItem it in ChangelistFilesLV.Items)
				{
					P4.FileMetaData m = it.Tag as P4.FileMetaData;
					string fp = m.DepotPath.Path;
					logger.Trace("Item {0}: {1}", i++, fp));
				}
#endif
			}
		}

		public IList<P4.FileMetaData> SelectedFileList
		{
			get
			{
				List<P4.FileMetaData> value = new List<P4.FileMetaData>();

				foreach (ListViewItem item in ChangelistFilesLV.Items)
				{
					if (item.Checked)
					{
						value.Add((P4.FileMetaData)item.Tag);
					}
				}
				return value;
			}
			set
			{
				foreach (ListViewItem item in ChangelistFilesLV.Items)
				{
					item.Checked = false;
				}
				foreach (P4.FileMetaData fmd in value)
				{
					if (fileMetaDataMap.ContainsKey(fmd.DepotPath.Path))
					{
						ListViewItem item = listItemMap[fmd.DepotPath.Path];
						item.Checked = true;
					}
				}
			}
		}

		public IList<string> GetSelectedFiles()
		{
			List<string> value = new List<string>();

			foreach (ListViewItem item in ChangelistFilesLV.Items)
			{
				if (item.Checked == true)
				{
					value.Add(((P4.FileMetaData)item.Tag).DepotPath.Path);
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
			if (_changelistId == 0)
			{
				ShelveFileCreateChangelistDlg dlg = new ShelveFileCreateChangelistDlg();

				if (dlg.ShowDialog() == DialogResult.Cancel)
				{
					return;
				}
				Description = dlg.Description;
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void SelectAllCB_CheckedChanged(object sender, EventArgs e)
		{
			// turn off the individual item check handler, so it doesn't try to change the
			// state of the check all check box
			ChangelistFilesLV.ItemChecked += new ItemCheckedEventHandler(ChangelistFilesLV_ItemChecked);
			try
			{
				ShelveBtn.Enabled = SelectAllCB.Checked;
                if (SelectAllCB.Checked)
                {
                    foreach (ListViewItem item in ChangelistFilesLV.Items)
                    {
                        item.Checked = true;
                    }
                }
                else
                {
                    foreach (ListViewItem item in ChangelistFilesLV.Items)
                    {
                        item.Checked = false;
                    }
                }
			}
			finally
			{
				// turn back on the individual item check handler
				ChangelistFilesLV.ItemChecked += new ItemCheckedEventHandler(ChangelistFilesLV_ItemChecked);
			}
		}

		bool inChangelistFilesLV_ItemChecked = false;
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
            
			if (inChangelistFilesLV_ItemChecked == true)
			{
				// don't recurse
				return;
			}
			inChangelistFilesLV_ItemChecked = true;

			// turn off the select all checked event, so it won't effect other controls
			SelectAllCB.CheckedChanged -= new EventHandler(SelectAllCB_CheckedChanged);

			// turn off the item checked event, so it won't recurse for linked files
			ChangelistFilesLV.ItemChecked -= new ItemCheckedEventHandler(ChangelistFilesLV_ItemChecked);
			try
			{
#if DEBUG_DB
				int i = 0;
				foreach (ListViewItem it in ChangelistFilesLV.Items)
				{
				    P4.FileMetaData m = it.Tag as P4.FileMetaData;
				    string fp = m.DepotPath.Path;
				    logger.Trace("Item {0}: {1}", i++, fp));
				}
#endif
				P4.FileMetaData fmd = (P4.FileMetaData)(e.Item.Tag);

				if ((_scm != null) && (fmd.MovedFile != null))
				{
					try
					{
						ListViewItem item = listItemMap[fmd.MovedFile.Path];
						if ((item != null) && (item.Checked != e.Item.Checked))
						{
							item.Checked = e.Item.Checked;
						}
					}
					catch { } // ignore errors
				}

				SelectAllCB.Checked = true;
				ShelveBtn.Enabled = false;
				foreach (ListViewItem item in ChangelistFilesLV.Items)
				{
					ShelveBtn.Enabled |= item.Checked;
					SelectAllCB.Checked &= item.Checked;
				}

			}
			finally
			{
				// turn back on the select all checked event, so it won't effect other controls
				SelectAllCB.CheckedChanged += new EventHandler(SelectAllCB_CheckedChanged);
				// turn back on the individual item check handler
				ChangelistFilesLV.ItemChecked += new ItemCheckedEventHandler(ChangelistFilesLV_ItemChecked);

				inChangelistFilesLV_ItemChecked = false;
			}
		}

		private void ChangelistFilesLV_SelectedIndexChanged(object sender, EventArgs e)
		{
			ShelveBtn.Enabled = ChangelistFilesLV.SelectedItems.Count > 0;
		}

		private void gridLayoutPanel1_AfterLayoutGrid()
		{
			Point l = ChangelistFilesLV.Location;
			l.X += 6;
			l.Y += 7;
			this.SelectAllCB.Location = l;
		}

		//private void ChangelistFilesLV_ItemCheck(object sender, ItemCheckEventArgs e)
		//{
		//    ChangelistFilesLV.ItemCheck -= new ItemCheckEventHandler(ChangelistFilesLV_ItemCheck);
		//    try
		//    {
		//        P4.FileMetaData fmd = (P4.FileMetaData)(ChangelistFilesLV.Items[e.Index].Tag);

		//        if ((_scm != null) && (fmd.MovedFile != null))
		//        {
		//            ListViewItem item = listItemMap[fmd.MovedFile.Path];
		//            if (item != null)
		//            {
		//                item.Checked = (e.NewValue == CheckState.Checked);
		//            }
		//        }
		//    }
		//    finally
		//    {
		//        ChangelistFilesLV.ItemCheck += new ItemCheckEventHandler(ChangelistFilesLV_ItemCheck);
		//    }
		//}
	}
}
