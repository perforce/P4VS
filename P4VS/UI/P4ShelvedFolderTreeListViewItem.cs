using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Perforce.SwarmApi;

using CenterImages = Perforce.P4VS.P4ObjectTreeListView.CenterImages;
using LeftImages = Perforce.P4VS.P4ObjectTreeListView.LeftImages;
using RightImages = Perforce.P4VS.P4ObjectTreeListView.RightImages;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public class P4ShelvedFolderTreeListViewItem : P4ObjectTreeListViewItem
	{
		private P4.Changelist _changeData = null;
		public P4.Changelist ChangeData 
		{
			get { return _changeData; } 
			set
			{
				_changeData = value;

                ImageIndex = (int)CenterImages.ShelvedFolder;
            }
		}
		public void SetData(P4.Changelist changeData, SwarmServer.Review reviewData)
		{
			_changeData = changeData;
			//_reviewData = reviewData;

			FileDataFetched = false;
			ChangeDataFetched = false;

            ImageIndex = (int)CenterImages.ShelvedFolder;
        }

		public bool Ours { get; private set; }
		public bool Pending { 
			get
			{
				if (Tag != null)
				{
					P4.Changelist change = Tag as P4.Changelist;
					if (change != null)
					{
						return change.Pending;
					}
				}
				return false;
			}
		}
		bool NeedsResolve { get; set; }

		public string Workspace { get; set; }
		public P4ScmProvider Scm { get; set; }

		private IList<P4.FileMetaData> _files;
		
		public bool FileDataFetched  { get; set; }
		public IList<P4.FileMetaData> FileData {
			get
			{
				if (FileDataFetched == false)
				{
					P4.Options opts = new P4.Options();

					if (ChangeData.Id > 0)
					{
						opts["-e"] = ChangeData.Id.ToString();
					}
					else
					{
						opts["-e"] = "default";
						NodeType.Set(nodeType.Default);
					}
					opts["-Olhp"] = null;
					opts["-Ro"] = null;
					P4.FileSpec fs = new P4.FileSpec(new P4.ClientPath(@"//" + Scm.Connection.Workspace + @"/..."), null);
					List<P4.FileSpec> lfs = new List<P4.FileSpec>();
					lfs.Add(fs);
					_files = Scm.GetFileMetaData((IList<P4.FileSpec>)lfs, opts);

					//List<string> paths = new List<string>();
					//foreach (P4.FileMetaData md in _changeData.Files)
					//{
					//    if (md.DepotPath != null)
					//    {
					//        paths.Add(md.DepotPath.Path);
					//    }
					//    _files = Scm.GetFileMetaData(paths);
					//}
					FileDataFetched = true;
				}
				return _files;
			}
			private set { _files = value; }
		}
		public IList<P4.FileMetaData> FetchFileData()
		{
			FileDataFetched = false;
			return FileData;
		}
		
		public bool ChangeDataFetched  { get; set; }
		public void FetchChangeData()
		{
			if (ChangeDataFetched == false)
			{
                P4.Options describeOptions = new P4.ChangeCmdOptions(P4.ChangeCmdFlags.IncludeJobs);
                if (_changeData.Id > 0)
                {
                    describeOptions = new P4.DescribeCmdOptions(P4.DescribeChangelistCmdFlags.Shelved | P4.DescribeChangelistCmdFlags.Omit, -1, -1);
                }
                P4.Changelist sc = Scm.GetChangelist(_changeData.Id, describeOptions);
				if (sc != null)
				{
					if ((sc.ShelvedFiles != null) && (sc.ShelvedFiles.Count > 0))
					{
						_changeData.ShelvedFiles = sc.ShelvedFiles;
					}
					else
					{
						_changeData.ShelvedFiles = null;
					}
					if ((sc.Jobs != null) && (sc.Jobs.Count > 0))
					{
						_changeData.Jobs = sc.Jobs;
					}
					else
					{
						_changeData.Jobs = null;
					}
				}
				ChangeDataFetched = true;
			}
		}
		public IList<P4.ShelvedFile> ShelvedFileData {
			get
			{
				if ((_changeData.ShelvedFiles == null) && (ChangeDataFetched == false))
				{
					FetchChangeData();
				}
				return _changeData.ShelvedFiles;
			}
			private set { _changeData.ShelvedFiles = value; }
		}

		public IList<P4.ShelvedFile> FetchShelvedFileData()
		{
			_changeData.ShelvedFiles = null;
			ChangeDataFetched = false;
			return ShelvedFileData;
		}

		public P4ShelvedFolderTreeListViewItem(TreeListViewItem parentItem, P4.Changelist changeData,
			 P4ScmProvider scm)
			: base()
		{
			ParentItem = parentItem;
			NodeType = nodeType.Pending; // default
			Scm = scm;
			Tag = changeData;

			Ours = (changeData.ClientId == Scm.Connection.Workspace);

			if (Ours)
			{
				P4.Options opts = new P4.Options();

				if (changeData.Id > 0)
				{
					opts["-e"] = changeData.Id.ToString();
				}
				else
				{
					opts["-e"] = "default";
					NodeType.Set(nodeType.Default);
				}
				opts["-Ru"] = null;
				opts["-m"] = "1";
				P4.FileSpec fs = new P4.FileSpec(new P4.ClientPath(@"//" + Scm.Connection.Workspace + @"/..."), null);
				List<P4.FileSpec> lfs = new List<P4.FileSpec>();
				lfs.Add(fs);
				IList<P4.FileMetaData> unresolved = Scm.GetFileMetaData(lfs, opts);

				if ((unresolved != null) && (unresolved.Count > 0))
				{
					NeedsResolve = true;
				}
			}

			_changeData = changeData; // don't call InitSubitems() or SelectImagesFromMetaData() yet

            ImageIndex = (int)CenterImages.ShelvedFolder;
        }
	}
}
