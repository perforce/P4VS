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
	public class P4ChangeTreeListViewItem : P4ObjectTreeListViewItem
	{
		private P4.Changelist _changeData = null;
		public P4.Changelist ChangeData 
		{
			get { return _changeData; } 
			set
			{
				_changeData = value;

				InitSubitems();
				SelectImagesFromMetaData();
			}
		}

		private SwarmServer.Review _reviewData = null;
		public SwarmServer.Review ReviewData 
		{
			get { return _reviewData; } 
			set
			{
				_reviewData = value;

				InitSubitems();
				SelectImagesFromMetaData();
			}
		}

		public void SetData(P4.Changelist changeData, SwarmServer.Review reviewData)
		{
			_changeData = changeData;
			_reviewData = reviewData;

			FileDataFetched = false;
			ChangeDataFetched = false;

			InitSubitems();
			SelectImagesFromMetaData();
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
		internal bool NeedsResolve { get; set; }

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
			internal set { _files = value; }
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

		public Dictionary<string,string> JobData {
			get
			{
				if ((_changeData.Jobs == null) && (ChangeDataFetched == false))
				{
					FetchChangeData();
				}
				return _changeData.Jobs;
			}
			private set { _changeData.Jobs = value; }
		}
		public IDictionary<string, string> FetchJobData()
		{
			_changeData.Jobs = null;
			ChangeDataFetched = false;
			return JobData;
		}

		//private IList<P4.FileMetaData> _files;
		
		//public bool FileDataFetched  { get; set; }
		//public IList<P4.FileMetaData> FileData {
		//    get
		//    {
		//        if (FileDataFetched == false)
		//        {
		//            P4.Options opts = new P4.Options();

		//            if (ChangeData.Id > 0)
		//            {
		//                opts["-e"] = ChangeData.Id.ToString();
		//            }
		//            else
		//            {
		//                opts["-e"] = "default";
		//                NodeType.Set(nodeType.Default);
		//            }
		//            opts["-Olhp"] = null;
		//            P4.FileSpec fs = new P4.FileSpec(new P4.ClientPath(@"//" + Scm.Workspace + @"/..."), null);
		//            List<P4.FileSpec> lfs = new List<P4.FileSpec>();
		//            lfs.Add(fs);
		//            _files = Scm.GetFileMetaData(lfs, opts);

		//            FileDataFetched = true;
		//        }
		//        return _files;
		//    }
		//    private set { _files = value; }
		//}
		//public IList<P4.FileMetaData> FetchFileData()
		//{
		//    FileDataFetched = false;
		//    return FileData;
		//}

		IList<object> Fields = null;

		public P4ChangeTreeListViewItem(TreeListViewItem parentItem, P4.Changelist changeData,
			 P4ScmProvider scm, IList<object> fields)
			: base()
		{
			ParentItem = parentItem;
			Fields = fields;
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

			if (changeData.Id > 0)
			{
				_reviewData = Scm.Connection.Swarm.IsChangelistAttachedToReview(changeData.Id);
			}
			InitSubitems();
			SelectImagesFromMetaData();
		}

		public P4ChangeTreeListViewItem(TreeListViewItem parentItem, P4.Changelist changeData, SwarmServer.Review reviewData,
			 P4ScmProvider scm, IList<object> fields)
			: base()
		{
			ParentItem = parentItem;
			Fields = fields;
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

			_reviewData = reviewData;

			InitSubitems();
			SelectImagesFromMetaData();
		}

		internal void SelectImagesFromMetaData()
		{
			if (_changeData == null)
			{
				return;
			}
			LeftImageIndices.Clear();
			RightImageIndices.Clear();
			CenterImageIndices.Clear();

			NodeType = nodeType.Changelist;

			// select the center images
			// Center image is based on change type and ownership (ours vs others)
			if (ChangeData.Pending)
			{
				if (Ours)
				{
					ImageIndex = (int)CenterImages.PendingOurs;
					NodeType.Set(nodeType.OurPending);
				}
				else
				{
					ImageIndex = (int)CenterImages.PendingOther;
					NodeType.Set(nodeType.OtherPending);
				}
			}
			else
			{
				ImageIndex = (int)CenterImages.SubmittedChange;
				if (Ours)
				{
					NodeType.Set(nodeType.Our);
				}
				// no flags set for other submitted
			}
			if (NeedsResolve)
			{
				CenterImageIndices.Add((int)CenterImages.PendingResolveBadge);
				NodeType.Set(nodeType.NeedsResolve);
			}
			//Left images
			if (ChangeData.Shelved)
			{
				LeftImageIndices.Add((int)LeftImages.PendingShelvedBadge);
				NodeType.Set(nodeType.HasShelved);
			}
			//Right images
			if ((ReviewData != null) && (ReviewData.id >= 0))
			{
				RightImageIndices.Add((int)RightImages.PendingReviewBadge);
				NodeType.Set(nodeType.UnderReview);
			}
		}

		public enum SubItemFlag
		{
			None = 0,
			ClientName,
			Description,
			Id,
			ModifiedDate,
			OwnerName,
			Pending,
			Shelved,
			Type,
			ReviewId,
			ReviewAuthor,
			ReviewCreated ,
			ReviewDescription,
			ReviewPending,
			ReviewState,//stateLabel,
			ReviewType,
			ReviewUpdated
		}
		private void InitSubitems()
		{
			this.SubItems.Clear();
			for(int idx = 0; idx < Fields.Count; idx++)
			{
				object value = null;
				object field = Fields[idx];
				if (field is SubItemFlag)
				{
					SubItemFlag flag = (SubItemFlag)field;
					switch (flag)
					{
						case SubItemFlag.None:
							value = string.Empty;
							break;
						case SubItemFlag.ClientName:
							value = ChangeData.ClientId;
							break;
						case SubItemFlag.Description:
							value = ChangeData.Description;
							break;
						case SubItemFlag.Id:
							value = ChangeData.Id;
							if (ChangeData.Id <= 0)
							{
								value = Resources.Changelist_Default.ToLower();
							}
							break;
						case SubItemFlag.ModifiedDate:
							value = string.Empty;
							if (ChangeData.Id > 0)
							{
								if (Preferences.LocalSettings.GetBool("P4Date_format", true))
								{
									value = ChangeData.ModifiedDate.ToString("yyyy/MM/dd HH:mm:ss");
								}
								else
								{
									value = ChangeData.ModifiedDate.ToString();
								}
							}
							break;
						case SubItemFlag.OwnerName:
							value = ChangeData.OwnerName;
							break;
						case SubItemFlag.Pending:
							value = ChangeData.Pending;
							break;
						case SubItemFlag.Shelved:
							value = ChangeData.Shelved;
							break;
						case SubItemFlag.Type:
							value = ChangeData.Type.ToString();
							if ((string)value == "None")
							{
								value = string.Empty;
							}
							break;
						case SubItemFlag.ReviewId:
							value = string.Empty;
							if (ReviewData != null)
							{
								value = ReviewData.id;
							}
							break;
						case SubItemFlag.ReviewAuthor:
							value = string.Empty;
							if (ReviewData != null)
							{
								value = ReviewData.author;
							}
							break;
						case SubItemFlag.ReviewCreated:
							value = string.Empty;
							if (ReviewData != null)
							{
								value = ReviewData.created;
							}
							break;
						case SubItemFlag.ReviewDescription:
							value = string.Empty;
							if (ReviewData != null)
							{
								value = ReviewData.description;
							}
							break;
						case SubItemFlag.ReviewPending:
							value = string.Empty;
							if (ReviewData != null)
							{
								value = ReviewData.pending;
							}
							break;
						case SubItemFlag.ReviewState:
							value = string.Empty;
							if (ReviewData != null)
							{
								value = ReviewData.stateLabel;
							}
							break;
						case SubItemFlag.ReviewType:
							value = string.Empty;
							if (ReviewData != null)
							{
								value = ReviewData.type;
							}
								break;
						case SubItemFlag.ReviewUpdated:
							value = string.Empty;
							if (ReviewData != null)
							{
								value = ReviewData.updated;
							} 
							break;
						default:
							value = string.Empty;
							break;
					}
				}
				else
				{
					value = field;
				}
				AddSubitem(value, idx);
			}
		}
	}
}
