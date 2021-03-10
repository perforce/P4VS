using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class P4ObjectTreeListView : TreeListView
	{
		public enum CenterImages : int
		{
			DepotIcon = 0,
			FileDepotIcon,
			FileChangedIcon,
			FileLocalIcon,
			FileNotInClientIcon,
			FileUnmappedIcon,
			PendingIcon,
			PendingOtherIcon,
			PendingResolveIcon,
			ResolveBadge,
			VirtualFileBadge,
			VirtualFolderBadge,
			DeleteMoveBadge,
			DeleteBadge,
			FolderIcon,
            SubmittedIcon,
            JobsIcon,
            RevisionAdd,
            RevisionArchive,
            RevisionBranch,
            RevisionDelete,
            RevisionEdit,
            RevisionIntegrate,
            RevisionMoveAdd,
            RevisionMoveDelete,
            RevisionPurge,
			PendingOurs,
			PendingOther,
			PendingResolveBadge,
			ShelveAdd,
			ShelveArchive,
			ShelveBase,
			ShelveBranch,
			ShelveDelete,
			ShelveEdit,
			ShelveIntegrate,
			ShelveMoveAdd,
			ShelveMoveDelete,
			ShelvePurge,
			Job,
			SubmittedChange,
            Portrait,
            ShelvedFolder
		}
		public enum LeftImages : int
		{
			EditBadge = 0,
			AddBadge,
			DeleteBadge,
			LockBadge,
			BranchBadge,
			AddMoveBadge,
			DeleteMoveBadge,
			IntegrateBadge,
			PendingShelvedBadge
		}
		public enum RightImages : int
		{
			HeadBadge,
			OutOfSyncBadge,
			EditOtherBadge,
			AddOtherBadge,
			DeleteOtherBadge,
			BranchOtherBadge,
			DeleteMoveOtherBadge,
			AddMoveOtherBadge,
			LockOtherBadge,
			IntegrateOtherBadge,
			PendingReviewBadge
		}


		private System.Windows.Forms.ImageList FileCenterImageList;
		private System.Windows.Forms.ImageList FileLeftImageList;
		private System.Windows.Forms.ImageList FileRightImageList;


		public P4ObjectTreeListView()
			: base()
		{
			InitializeComponent();

			this.FileCenterImageList = new System.Windows.Forms.ImageList(this.components);
			this.FileLeftImageList = new System.Windows.Forms.ImageList(this.components);
			this.FileRightImageList = new System.Windows.Forms.ImageList(this.components);
			// 
			// FileCenterImageList
			// 
			this.FileCenterImageList.TransparentColor = System.Drawing.Color.White;

			this.FileCenterImageList.Images.Add("depot_icon.png", Images.depot_icon);
			this.FileCenterImageList.Images.Add("file_depot_icon.png", Images.file_depot_icon);
			this.FileCenterImageList.Images.Add("file_changed_icon.png", Images.file_changed_icon);
			this.FileCenterImageList.Images.Add("file_local_icon.png", Images.file_local_icon);
			this.FileCenterImageList.Images.Add("file_notInClient_icon.png", Images.file_notInClient_icon);
			this.FileCenterImageList.Images.Add("file_unmapped_icon.png", Images.file_unmapped_icon);
			this.FileCenterImageList.Images.Add("pending_icon.png", Images.pending_icon);
			this.FileCenterImageList.Images.Add("pending_other.png", Images.pending_other);
			this.FileCenterImageList.Images.Add("pending_resolve.png", Images.pending_resolve);
			this.FileCenterImageList.Images.Add("resolve_badge.png", Images.resolve_badge);
			this.FileCenterImageList.Images.Add("virtual_file_badge.png", Images.virtual_file_badge);
			this.FileCenterImageList.Images.Add("virtual_folder_badge.png", Images.virtual_folder_badge);
			this.FileCenterImageList.Images.Add("delete_move_center_badge.png", Images.delete_move_center_badge);
			this.FileCenterImageList.Images.Add("delete_center_badge.png", Images.delete_center_badge);
			this.FileCenterImageList.Images.Add("folder.png", Images.folder);
            this.FileCenterImageList.Images.Add("submitted_change_icon.png", Images.submitted_change_icon);
            this.FileCenterImageList.Images.Add("jobs_icon.png", Images.jobs_icon);
            this.FileCenterImageList.Images.Add("revision_icon_add.png", Images.revision_icon_add);
            this.FileCenterImageList.Images.Add("revision_icon_archive.png", Images.revision_icon_archive);
            this.FileCenterImageList.Images.Add("revision_icon_branch.png", Images.revision_icon_branch);
            this.FileCenterImageList.Images.Add("revision_icon_delete.png", Images.revision_icon_delete);
            this.FileCenterImageList.Images.Add("revision_icon_edit.png", Images.revision_icon_edit);
            this.FileCenterImageList.Images.Add("revision_icon_integrate.png", Images.revision_icon_integrate);
            this.FileCenterImageList.Images.Add("revision_icon_moveadd.png", Images.revision_icon_moveadd);
            this.FileCenterImageList.Images.Add("revision_icon_movedelete.png", Images.revision_icon_movedelete);
			this.FileCenterImageList.Images.Add("revision_icon_purge.png", Images.revision_icon_purge);
			this.FileCenterImageList.Images.Add("pending.png", Images.pending_icon);
			this.FileCenterImageList.Images.Add("pending_other.png", Images.pending_other);
			this.FileCenterImageList.Images.Add("pending_resolve_badge.png", Images.pending_resolve_badge);
			this.FileCenterImageList.Images.Add("shelve_icon_add.png", Images.shelve_icon_add);
			this.FileCenterImageList.Images.Add("shelve_icon_archive.png", Images.shelve_icon_archive);
			this.FileCenterImageList.Images.Add("shelve_icon_base.png", Images.shelve_icon_base);
			this.FileCenterImageList.Images.Add("shelve_icon_branch.png", Images.shelve_icon_branch);
			this.FileCenterImageList.Images.Add("shelve_icon_delete.png", Images.shelve_icon_delete);
			this.FileCenterImageList.Images.Add("shelve_icon_edit.png", Images.shelve_icon_edit);
			this.FileCenterImageList.Images.Add("shelve_icon_integrate.png", Images.shelve_icon_integrate);
			this.FileCenterImageList.Images.Add("shelve_icon_moveadd.png", Images.shelve_icon_moveadd);
			this.FileCenterImageList.Images.Add("shelve_icon_movedelete.png", Images.shelve_icon_movedelete);
			this.FileCenterImageList.Images.Add("shelve_icon_purge.png", Images.shelve_icon_purge);
			this.FileCenterImageList.Images.Add("jobs_icon.png", Images.jobs_icon);
			this.FileCenterImageList.Images.Add("submitted_change_icon.png", Images.submitted_change_icon);
            this.FileCenterImageList.Images.Add("portrait.png", Images.portrait);
            this.FileCenterImageList.Images.Add("shelve_icon_folder.png", Images.shelve_icon_folder);
            // 
            // FileLeftImageList
            // 
            this.FileLeftImageList.TransparentColor = System.Drawing.Color.White;
			this.FileLeftImageList.Images.Add("edit_badge.png", Images.edit_badge);
			this.FileLeftImageList.Images.Add("add_badge.png", Images.add_badge);
			this.FileLeftImageList.Images.Add("delete_badge.png", Images.delete_badge);
			this.FileLeftImageList.Images.Add("lock_badge.png", Images.lock_badge);
			this.FileLeftImageList.Images.Add("branch_badge.png", Images.branch_badge);
			this.FileLeftImageList.Images.Add("add_move_badge.png", Images.add_move_badge);
			this.FileLeftImageList.Images.Add("delete_move_badge.png", Images.delete_move_badge);
			this.FileLeftImageList.Images.Add("integrate_badge.png", Images.integrate_badge);
			this.FileLeftImageList.Images.Add("pending_shelved_badge.png", Images.pending_shelved_badge);
			// 
			// FileRightImageList
			// 
			this.FileRightImageList.TransparentColor = System.Drawing.Color.White;
			this.FileRightImageList.Images.Add("head_dot_badge.png", Images.head_dot_badge);
			this.FileRightImageList.Images.Add("out_of_sync_badge.png", Images.out_of_sync_badge);
			this.FileRightImageList.Images.Add("editO_badge.png", Images.editO_badge);
			this.FileRightImageList.Images.Add("addO_badge.png", Images.addO_badge);
			this.FileRightImageList.Images.Add("deleteO_badge.png", Images.deleteO_badge);
			this.FileRightImageList.Images.Add("branchO_badge.png", Images.branchO_badge);
			this.FileRightImageList.Images.Add("delete_moveO_badge.png", Images.delete_moveO_badge);
			this.FileRightImageList.Images.Add("add_moveO_badge.png", Images.add_moveO_badge);
			this.FileRightImageList.Images.Add("lock_other_badge.png", Images.lock_other_badge);
			this.FileRightImageList.Images.Add("integrateO_badge.png", Images.integrateO_badge);
			this.FileRightImageList.Images.Add("pending_review_badge.png", Images.pending_review_badge);
			this.ResumeLayout(false);

			SmallImageList = FileCenterImageList;
			LeftImageList = FileLeftImageList;
			RightImageList = FileRightImageList;
		}

		public void InitView(P4ScmProvider _scm,  IList<string> files, IList<object> fields)
		{
			listItemMap = new Dictionary<string, P4ObjectTreeListViewItem>();

			IList<P4.FileSpec> fs = P4.FileSpec.LocalSpecList(files.ToArray());

			Files = _scm.GetFileMetaData(fs, null);
			Fields = fields;

			foreach (P4.FileMetaData file in Files)
			{
				P4FileTreeListViewItem it = new P4FileTreeListViewItem(null, file, (IList<object>)Fields);
				listItemMap.Add(file.DepotPath.Path, it);
				this.Items.Add(it);
			}

		}

		public void InitView(IList<P4.FileMetaData> files, IList<object> fields)
		{
			Files = files;
			Fields = fields;

			foreach (P4.FileMetaData file in Files)
			{
				this.Items.Add(new P4FileTreeListViewItem(null, file, (IList<object>)Fields));
			}

		}

		IList<P4.FileMetaData> Files { get; set; }

		IList<object> Fields { get; set; }

		IDictionary<string, P4ObjectTreeListViewItem> listItemMap;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<P4.FileMetaData> SelectedFileList
		{
			get
			{
				List<P4.FileMetaData> value = new List<P4.FileMetaData>();

				if (CheckBoxes)
				{
					foreach (P4ObjectTreeListViewItem item in Items)
					{
						if ((item is P4FileTreeListViewItem) && (item.Checked))
						{
							value.Add(((P4FileTreeListViewItem)item).FileData);
						}
					}
				}
				else
				{
					foreach (P4ObjectTreeListViewItem item in SelectedItems)
					{
						if (item is P4FileTreeListViewItem)
						{
							value.Add(((P4FileTreeListViewItem)item).FileData);
						}
					}
				}
				return value;
			}
			set
			{
				if (CheckBoxes)
				{
					foreach (P4ObjectTreeListViewItem item in Items)
					{
						item.Checked = false;
					}
					foreach (P4.FileMetaData file in value)
					{
						if (listItemMap.ContainsKey(file.DepotPath.Path))
						{
							P4ObjectTreeListViewItem item = listItemMap[file.DepotPath.Path];
							item.Checked = true;
						}
					}
				}
				else
				{
					SelectedItems.Clear();
					foreach (P4.FileMetaData file in value)
					{
						if (listItemMap.ContainsKey(file.DepotPath.Path))
						{
							P4ObjectTreeListViewItem item = listItemMap[file.DepotPath.Path];
							item.Selected = true;
						}
					}
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<P4.FileMetaData> UnselectedFileList
		{
			get
			{
				List<P4.FileMetaData> value = new List<P4.FileMetaData>();

				if (CheckBoxes)
				{
					foreach (P4ObjectTreeListViewItem item in Items)
					{
						if ((item is P4FileTreeListViewItem) && (item.Checked == false))
						{
							value.Add(((P4FileTreeListViewItem)item).FileData);
						}
					}
				}
				return value;
			}
		}

	}
}
