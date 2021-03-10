using Perforce.P4Scm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IO = System.IO;

namespace Perforce.P4VS
{
	public partial class ResolveFileDlg : AutoSizeForm
	{
		public delegate void UpdateFileStatus();

		public static void ResolveFiles(P4ScmProvider scm,  IList<string> files, bool CmdLineMode,
            UpdateFileStatus refreshUICallback, bool modal)
		{
			ResolveFileDlg dlg = new ResolveFileDlg(scm);

			dlg.Files = files;
			dlg.RefreshUICallback = refreshUICallback;
            if (dlg.Files.Count<1)
            {
                return;
            }

            if (modal)
            {
                dlg.ShowDialog();
            }
            else
            {
                dlg.Show();
            }
		}

		public UpdateFileStatus RefreshUICallback { get; set; }

		private P4ScmProvider _scm = null;

		private string SelectionCountLblTxt = null;

		public ResolveFileDlg(P4ScmProvider scm)
		{
			PreferenceKey = "ResolveFileDlg";

			_scm = scm;

			InitializeComponent();

            this.Icon = Images.icon_p4vs_16px;
			resolveFileInteractiveControl1.Scm = scm;
			resolveFileAutoControl1.Scm = scm;
			resolveFileActionControl1.Scm = scm;

			SelectionCountLblTxt = SelectionCountLbl.Text;

			SelectionCountLbl.Text = string.Format(SelectionCountLblTxt,
				0, (FileList.Items != null) ? FileList.Items.Count : 0);

			resolveFileAutoControl1.Visible = true;
			resolveFileInteractiveControl1.Visible = false;
			resolveFileActionControl1.Visible = false;

			resolveFileAutoControl1.UpdateListView = new InitFileListViewDelegate(InitFileListView);
			resolveFileInteractiveControl1.UpdateListView = new InitFileListViewDelegate(InitFileListView);
			resolveFileActionControl1.UpdateListView = new InitFileListViewDelegate(InitFileListView);

			resolveFileAutoControl1.SelectedMethod = (ResolveFileAutoControl.AutoResolveMethod)Preferences.LocalSettings.Get("DefaultUatoMergeMethod", ResolveFileAutoControl.AutoResolveMethod.Safe);

			MergeBinaryCB.Checked = Preferences.LocalSettings.GetBool("ReolveDialog.MergeBinaryCB.Checked", false);
		}

//		private Dictionary<string, FileListViewItem> itemMap;

		public class FileListViewItem : ListViewItem
		{
			public string TargetFileName;
			public string TargetFolder;
			public string SourceFileName;
			public string SourceFolder;

			public FileListViewItem FirstItem { get; set; }

			public int SubItemIdx { get; private set; }

			public P4.ResolveType _resolveType;
			public P4.ResolveType ResolveType
			{
				get { return _resolveType; }
				set
				{
					_resolveType = value;
					base.SubItems[4].Text = value.ToString();
				}
			}

			//public int SourceDiffCnt { get { return ResolveAnalysis.SourceDiffCnt; } }
			//public int TargetDiffCnt { get { return ResolveAnalysis.TargetDiffCnt; } }
			//public int BothDiffCnt { get { return ResolveAnalysis.CommonDiffCount; } }
			//public int ConflictCnt { get { return ResolveAnalysis.ConflictCount; } }

			public P4.ResolveAnalysis ResolveAnalysis { get; set; }

			private P4.FileSpec _source;
			public P4.FileSpec Source
			{
				get { return _source; }
				set
				{
					_source = value;
					if (value.DepotPath != null)
					{
						SourceFileName = value.DepotPath.GetFileName();
						SourceFolder = value.DepotPath.GetDirectoryName();
					}
					else if (value.LocalPath != null)
					{
						SourceFileName = value.LocalPath.GetFileName();
						SourceFolder = value.LocalPath.GetDirectoryName();
					}
					base.SubItems[2].Text = string.Format("{0}{1}",SourceFileName,(value.Version != null)?value.Version.ToString():string.Empty);
					base.SubItems[3].Text = SourceFolder;
				}
			}
			public FileListViewItem(string path, int subItemIdx)
			{
				Path = path;
				SubItemIdx = subItemIdx;

				FirstItem = null;

				TargetFileName = IO.Path.GetFileName(path);
				TargetFolder = IO.Path.GetDirectoryName(path);

				base.Text = TargetFileName;
				base.SubItems.Add(TargetFolder);
				base.SubItems.Add(string.Empty);
				base.SubItems.Add(string.Empty);
				base.SubItems.Add(string.Empty);
			}
			public string Path
			{
				get {return (string) Tag;}
				private set {Tag = value;}
			}
			public P4.FileResolveRecord ResolveRecord { get; set; }
			public P4.FileMetaData MetaData { get; set; }
		}

		P4.Client.ResolveFileDelegate resolveCallback = null;

		private P4.P4ClientMerge.MergeStatus ResolveCallBackHandler(P4.FileResolveRecord resolveRecord,
			P4.Client.AutoResolveDelegate AutoResolve, string sourcePath, string targetPath, string basePath, string resultsPath)
		{
			return P4.P4ClientMerge.MergeStatus.CMS_SKIP;
		}

		FileListViewItem CurrentItem = null;
		IList<string> _files = null;

		public IList<string> Files
		{
			get
			{
				return _files;
			}
			set
			{
				_files = value;

				InitFileListView();
			}
		}
		private bool bFirstInit = true;

		private void InitFileListView()
		#region single pass initialization of the list
		{
			this.FileList.Items.Clear();
//			itemMap = null;

			if (_files == null)
			{
				return;
			}
//			itemMap = new Dictionary<string, FileListViewItem>();
			resolveCallback = new P4.Client.ResolveFileDelegate(ResolveCallBackHandler);

			P4.Options options = new P4.ResolveCmdOptions(
				P4.ResolveFilesCmdFlags.ForceTextualMerge | P4.ResolveFilesCmdFlags.DisplayBaseFile, -1);

			int itemCnt = 0;

			IList<P4.P4Exception> errors = null;

			IList<string> filesNotListed = new List<string>();

			foreach (string file in _files)
			{
				IList<P4.FileResolveRecord> records = null;
				try
				{
                    records = _scm.Connection.Repository.Connection.Client.ResolveFiles(
					resolveCallback, options, P4.FileSpec.LocalSpec(file));
				}
				catch (P4.P4Exception ex)
				{
					if (errors == null)
					{
						errors = new List<P4.P4Exception>();
					}
					errors.Add(ex);
				}
                P4.P4CommandResult results = _scm.Connection.Repository.Connection.LastResults;

				int subItemIdx = 0;

				FileListViewItem firstItem = null;

				if ((records != null) && (records.Count > 0))
				{
					foreach (P4.FileResolveRecord record in records)
					{
						string path = null;
						if (record.LocalFilePath != null)
						{
							path = record.LocalFilePath.Path;
						}
						else if ((record.FromFileSpec != null) && (record.FromFileSpec.LocalPath != null))
						{
							path = record.FromFileSpec.LocalPath.Path;
						}
						else
						{
							continue;
						}

						CurrentItem = new FileListViewItem(path, subItemIdx);

						if (subItemIdx == 0)
						{
							firstItem = CurrentItem;
						}
						else
						{
							CurrentItem.FirstItem = firstItem;
						}
						subItemIdx++;

						// string key = string.Format("<{0}>{1}", record.ResolveType, path.ToLower());
						//				itemMap.Add(key, CurrentItem);

						this.FileList.Items.Add(CurrentItem);

						CurrentItem.ResolveRecord = record;
						CurrentItem.Source = record.FromFileSpec;
						if (record.ResolveType != P4.ResolveType.None)
						{
							CurrentItem.ResolveType = record.ResolveType;
						}
						else if ((record.Analysis != null) && (record.Analysis.ResolveType != P4.ResolveType.None))
						{
							CurrentItem.ResolveType = record.Analysis.ResolveType ;
						}
						CurrentItem.ResolveAnalysis = record.Analysis;


						CurrentItem = null;
					}
				}
				else
				{
					filesNotListed.Add(file);
				}
			}
			if (errors != null)
			{
				P4ErrorDlg.Show(errors, false);
			}
			// clean out files that do not need to be resolved
			foreach (string file in filesNotListed)
			{
				_files.Remove(file);
			}

			if (RefreshUICallback != null)
			{
				RefreshUICallback();
			}
			if ((_files == null) || (_files.Count <= 0))
			{
				// no files to resolve
				if (bFirstInit)
				{
					MessageBox.Show(Resources.ResolveFileDlg_NoFileToResolveWarning, Resources.P4VS, 
						MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					this.DialogResult = DialogResult.OK;
					Close();
					return;
				}
			}
			bFirstInit = false;

			SelectionCountLbl.Text = string.Format(SelectionCountLblTxt, 0, itemCnt);

			resolveCallback = null;

			AutoResolveRB_CheckedChanged(null, null);
		}
		#endregion
		#region two pass initialization of the list
		/*
		{
			this.FileList.Items.Clear();
			itemMap = null;

			if (_files == null)
			{
				return;
			}
			itemMap = new Dictionary<string, FileListViewItem>();

			//foreach (string val in value)
			//{
			//    FileListViewItem item = new FileListViewItem(val);
			//    itemMap.Add(val.ToLower(), item);
			//    this.FileList.Items.Add(item);
			//}
			P4.Options options = new P4.ResolveCmdOptions(
				P4.ResolveFilesCmdFlags.PreviewOnly | P4.ResolveFilesCmdFlags.DisplayBaseFile, -1);

			IList<P4.FileResolveRecord> records = _scm.Repository.Connection.Client.ResolveFiles(
				P4.FileSpec.LocalSpecList(_files.ToArray()), options);

			P4.P4CommandResult results = _scm.Repository.Connection.LastResults;
			//P4ErrorDlg.Show(results, false);

			if ((records.Count < Files.Count) && (results.ErrorList != null) && (results.ErrorList.Count > 0))
			{
				for (int idx = 0; idx < results.ErrorList.Count; idx++)
				{
					if (results.ErrorList[idx].ErrorCode == P4ClientError.ActionResolve111)
					{
						int idx2 = results.ErrorList[idx].ErrorMessage.IndexOf(" - upgrade");
						string path = results.ErrorList[idx].ErrorMessage.Substring(0, idx2);
						P4.FileMetaData md = _scm.GetFileMetaData(path);

						if (md.Unresolved == false)
						{
							continue;
						}
						P4.FileResolveRecord record = new P4.FileResolveRecord(md.Action, null, md.LocalPath);
						//record.LocalFilePath = md.LocalPath;
						records.Add(record);
					}
				}
			}
			int itemCnt = 0;

			resolveCallback = new P4.Client.ResolveFileDelegate(ResolveCallBackHandler);

			foreach (P4.FileResolveRecord record in records)
			{
				string path = null;
				if (record.LocalFilePath != null)
				{
					path = record.LocalFilePath.Path;
				}
				else if ((record.FromFileSpec != null) && (record.FromFileSpec.LocalPath != null))
				{
					path = record.FromFileSpec.LocalPath.Path;
				}
				else
				{
					continue;
				}

				CurrentItem = new FileListViewItem(path);

				string key = string.Format("<{0}>{1}", record.ResolveType, path.ToLower());
				itemMap.Add(key, CurrentItem);
				this.FileList.Items.Add(CurrentItem);

				CurrentItem.ResolveRecord = record;
				CurrentItem.Source = record.FromFileSpec;
				if (record.ResolveType == P4.ResolveType.Content)
				{
					CurrentItem.ResolveType = record.ResolveType.ToString();
					P4.ResolveFilesCmdFlags flags = P4.ResolveFilesCmdFlags.DisplayBaseFile | P4.ResolveFilesCmdFlags.ForceTextualMerge;

					//switch (record.resolvetype)
					//{
					//    default:
					//    case  p4.resolvetype.content:
					//        flags |= p4.resolvefilescmdflags.filecontentchangesonly;
					//        break;
					//    case  p4.resolvetype.branch:
					//        flags |= p4.resolvefilescmdflags.filebranchingonly;
					//        break;
					//    case  p4.resolvetype.deletions:
					//        flags |= p4.resolvefilescmdflags.filedeletionsonly;
					//        break;
					//    case  p4.resolvetype.filetype:
					//        flags |= p4.resolvefilescmdflags.filetypechangesonly;
					//        break;
					//    case  p4.resolvetype.moverename:
					//        flags |= p4.resolvefilescmdflags.filemovesonly;
					//        break;

					//}
					options = new P4.ResolveCmdOptions(flags, -1);

					IList<P4.FileResolveRecord> records2 =
						_scm.Repository.Connection.Client.ResolveFiles(
								resolveCallback, options,
								new P4.FileSpec[] { P4.FileSpec.LocalSpec(path) });

					results = _scm.Repository.Connection.LastResults;
					P4ErrorDlg.Show(results, false);


					if (CurrentItem == null)
					{
						continue;
					}
					if ((records2 != null) && (records2.Count > 0))
					{
						itemCnt++;
						CurrentItem.ResolveAnalysis = records2[0].Analysis;
					}
					else
					{
						FileList.Items.Remove(CurrentItem);
					}
				}
				else
				{
					CurrentItem.ResolveType = "Action";
				}

				CurrentItem = null;
			}
			SelectionCountLbl.Text = string.Format(SelectionCountLblTxt, 0, itemCnt);

			resolveCallback = null;

			AutoResolveRB_CheckedChanged(null, null);
		}
		 */
		#endregion

		private enum ViewType { Automatic, Interactive, Action };

		//private ViewType CurrentView;

		private void AutoResolveRB_CheckedChanged(object sender, EventArgs e)
		{
			//CurrentView = ViewType.Automatic;

			resolveFileAutoControl1.Visible = true;
			resolveFileInteractiveControl1.Visible = false;
			resolveFileActionControl1.Visible = false;

			FileList.MultiSelect = true;
			foreach (FileListViewItem item in FileList.Items)
			{
				item.Selected = true;
			}
		}

		private void InteractiveResolveRB_CheckedChanged(object sender, EventArgs e)
		{
			FileListViewItem selection = null;
			if ((FileList.SelectedItems != null) && (FileList.SelectedItems.Count > 0))
			{
				selection = (FileListViewItem)FileList.SelectedItems[0];
				if (selection.FirstItem != null)
				{
					selection = selection.FirstItem;
				}

			}
			else
			{
				return;
			}
			if (selection.ResolveType == P4.ResolveType.Content)
			{
				//CurrentView = ViewType.Interactive;

				resolveFileAutoControl1.Visible = false;
				resolveFileInteractiveControl1.Visible = true;
				resolveFileActionControl1.Visible = false;
			}
			else
			{
				//CurrentView = ViewType.Action;

				resolveFileAutoControl1.Visible = false;
				resolveFileInteractiveControl1.Visible = false;
				resolveFileActionControl1.Visible = true;
			}

			//FileListViewItem sel = (FileListViewItem)FileList.Items[0];
			//if ((FileList.SelectedItems!= null) && (FileList.SelectedItems.Count > 0))
			//{
			//    sel = (FileListViewItem) FileList.SelectedItems[0];
			//}
			FileList.MultiSelect = false;
			FileList.SelectedItems.Clear();
			//if (sel != null)
			//{
			//    sel.Selected = true;
			//}
			//bool bFirst = true;
			foreach (FileListViewItem item in FileList.Items)
			{
				item.Selected = false; //bFirst;
				//bFirst = false;
			}
			selection.Selected = true;
		}

		private void FileList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectionCountLblTxt != null)
			{
				SelectionCountLbl.Text = string.Format(SelectionCountLblTxt,
					(FileList.SelectedItems != null) ? FileList.SelectedItems.Count : 0,
					(FileList.Items != null) ? FileList.Items.Count : 0);
			}

            //foreach (FileListViewItem sel in FileList.SelectedItems)
            //{
            //    if ((sel.FirstItem != null) && (sel.FirstItem.Selected == false))
            //    {
            //        sel.Selected = false;
            //        return;
            //    }
            //}
			FileListViewItem selection = null;

			if ((FileList.SelectedItems != null) && (FileList.SelectedItems.Count > 0))
			{
				selection = (FileListViewItem)FileList.SelectedItems[0];
				//if ((FileList.MultiSelect == false) && (selection.FirstItem != null))
				//{
				//    selection.Selected = false;
				//    return;
				//}
				resolveFileInteractiveControl1.SelectedItem = selection;
				resolveFileActionControl1.SelectedItem = selection;
			}
			else
			{
				resolveFileInteractiveControl1.SelectedItem = null;
				resolveFileActionControl1.SelectedItem = null;
			}
			List<FileListViewItem> SelectedItems = null;
			if ((FileList.SelectedItems != null) && (FileList.SelectedItems.Count > 0))
			{
				SelectedItems = new List<FileListViewItem>();
				foreach (ListViewItem item in FileList.SelectedItems)
				{
					SelectedItems.Add(item as FileListViewItem);
				}
			}
			resolveFileAutoControl1.SelectedItems = SelectedItems;
			if ((InteractiveResolveRB.Checked) && (selection != null))
			{
				if (selection.ResolveType == P4.ResolveType.Content)
				{
					//CurrentView = ViewType.Interactive;

					resolveFileAutoControl1.Visible = false;
					resolveFileInteractiveControl1.Visible = true;
					resolveFileActionControl1.Visible = false;
				}
				else
				{
					//CurrentView = ViewType.Action;

					resolveFileAutoControl1.Visible = false;
					resolveFileInteractiveControl1.Visible = false;
					resolveFileActionControl1.Visible = true;
				}
			}
		}

		private void MergeBinaryCB_CheckedChanged(object sender, EventArgs e)
		{
			resolveFileAutoControl1.MergeBinaryAsText = MergeBinaryCB.Checked;
			resolveFileInteractiveControl1.MergeBinaryAsText = MergeBinaryCB.Checked;

			Preferences.LocalSettings["ReolveDialog.MergeBinaryCB.Checked"] = MergeBinaryCB.Checked;
		}

		public delegate void InitFileListViewDelegate();

		private void ResolveFileDlg_FormClosed(object sender, FormClosedEventArgs e)
		{
			resolveFileInteractiveControl1.Cleanup();
            resolveFileAutoControl1.CleanUp();
            resolveFileActionControl1.Cleanup();
        }
	}
}
