using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CenterImages = Perforce.P4VS.P4ObjectTreeListView.CenterImages;
using LeftImages = Perforce.P4VS.P4ObjectTreeListView.LeftImages;
using RightImages = Perforce.P4VS.P4ObjectTreeListView.RightImages;

namespace Perforce.P4VS
{
	public class P4ShelvedFileTreeListViewItem : P4ObjectTreeListViewItem
	{
		private P4.ShelvedFile _fileData = null;
		public P4.ShelvedFile FileData 
		{
			get { return _fileData; } 
			set
			{
				_fileData = value;

				InitSubitems(Fields);
				SelectImagesFromMetaData();
			}
		}

		IList<object> Fields = null;

		//public string FileName { get; set; }
		//public string FileFolder { get; set; }

		public P4ShelvedFileTreeListViewItem(TreeListViewItem parentItem, P4.ShelvedFile fileData, IList<object> fields)
			:base()
		{
			ParentItem = parentItem;
			Fields = fields;
			FileData = fileData;

			Tag = fileData;

			//FileName = fileData.LocalPath.GetFileName();
			//FileFolder = fileData.LocalPath.GetFileName();
		}

		private void SelectImagesFromMetaData()
		{
			if (FileData == null || FileData.Path == null)
			{
				return;
			}

			// first check if it is a directory
			if (FileData.Path.Path.EndsWith("/..."))
			{
				ImageIndex = (int) CenterImages.FolderIcon;
			}
			else
			{
				ImageIndex = (int)CenterImages.ShelveBase;
				switch (FileData.Action)
				{
					case P4.FileAction.Add:
						ImageIndex = (int)CenterImages.ShelveAdd;
						break;
					case P4.FileAction.Branch:
						ImageIndex = (int)CenterImages.ShelveBranch;
						break;
					case P4.FileAction.Delete:
						ImageIndex = (int)CenterImages.ShelveDelete;
						break;
					case P4.FileAction.Edit:
						ImageIndex = (int)CenterImages.ShelveEdit;
						break;
					case P4.FileAction.Integrate:
						ImageIndex = (int)CenterImages.ShelveIntegrate;
						break;
					case P4.FileAction.MoveAdd:
						ImageIndex = (int)CenterImages.ShelveMoveAdd;
						break;
					case P4.FileAction.MoveDelete:
						ImageIndex = (int)CenterImages.ShelveMoveDelete;
						break;
					case P4.FileAction.Purge:
						ImageIndex = (int)CenterImages.ShelvePurge;
						break;
					default:
						ImageIndex = (int)CenterImages.ShelveBase;
						break;
				}
			}
		}

		public enum SubItemFlag
		{
			None = 0,
			Path,
			FileName,
			Folder,
			Action,
			Digest,
			Revision,
			Size,
			Type,
			FileExtension
		}

		private void InitSubitems(IList<object> fields)
		{
			this.SubItems.Clear();
			for (int idx = 0; idx < fields.Count; idx++)
			{
				object value = null;
				object field = fields[idx];
				if (field is SubItemFlag)
				{
					SubItemFlag flag = (SubItemFlag)field;
					switch (flag)
					{
						case SubItemFlag.None:
							value = string.Empty;
							break;
						case SubItemFlag.Path:
							value = FileData.Path.Path;
							break;
						case SubItemFlag.FileName:
							value = FileData.Path.GetFileName();
							break;
						case SubItemFlag.Folder:
							value = FileData.Path.GetDirectoryName();
							break;
						case SubItemFlag.Type:
							value = FileData.Type;
							break;
						case SubItemFlag.Revision:
							if (FileData.Revision >= 0)
							{
								value = string.Format("#{0}", FileData.Revision);
							}
							else
							{
								value = "#0";
							}
							break;
						case SubItemFlag.Action:
							value = FileData.Action;
							break;
						case SubItemFlag.Digest:
							value = FileData.Digest;
							break;
						case SubItemFlag.Size:
							if (FileData.Size >= 0)
							{
								value = PrettyPrintFileSize(FileData.Size);
							}
							else
							{
								value = string.Empty;
							}
							break;
						case SubItemFlag.FileExtension:
							value = System.IO.Path.GetExtension(FileData.Path.GetFileName());
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
