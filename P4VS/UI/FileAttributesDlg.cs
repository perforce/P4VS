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
	public partial class FileAttributesDlg : AutoSizeForm
	{
		public static void ChangeFileType(P4ScmProvider scm, IList<string> paths)
		{
			FileAttributesDlg dlg = new FileAttributesDlg(scm);

			int changelistId = 0;

			IList<string> CheckedinFiles = new List<string>();
			IList<string> CheckedoutFiles = new List<string>();

			IList<P4.FileMetaData> fmd = scm.GetFileMetaData(paths, null);
			P4.FileType ft = null;
			if (fmd != null)
			{
				foreach (P4.FileMetaData md in fmd)
				{
					if (ft == null)
					{
						ft = md.Type;
						if (md.Change > 0)
						{
							changelistId = md.Change;
						}
					}
					else
					{
						// only keep the base type if all the same
						if (ft.BaseType != md.Type.BaseType)
						{
							ft.BaseType = P4.BaseFileType.Unspecified;
						}
						// only keep fags that are the same
						ft.Modifiers &= md.Type.Modifiers;
						if (changelistId != md.Change)
						{
							changelistId = 0;
						}
					}
					if (md.Action == P4.FileAction.None)
					{
						CheckedinFiles.Add(md.LocalPath.Path);
					}
					else
					{
						CheckedoutFiles.Add(md.LocalPath.Path);
					}
				}
			}

			dlg.FileType = ft;
			dlg.TargetChangelistId = changelistId;

			if (dlg.ShowDialog() == DialogResult.Cancel)
			{
				return;
			}
			int targetChangelist = dlg.TargetChangelistId;
			P4.Options options = null;

			if (targetChangelist < 0)
			{
				// if selected, create a new numbered changelist with the files.

				P4.Changelist change = scm.Connection.Repository.NewChangelist();
                change.Description = "<P4VS Change FileType>";
                change.ClientId = scm.Connection.Repository.Connection.Client.Name;
                change.OwnerName = scm.Connection.Repository.Connection.UserName;
				change.Files = null; // new List<P4.FileMetaData>();

                P4.Changelist newChange = scm.Connection.Repository.CreateChangelist(change);

				targetChangelist = newChange.Id;
			}

			if (CheckedoutFiles.Count > 0)
			{
				options = new P4.ReopenCmdOptions(targetChangelist, dlg.FileType);
				if ((options.ContainsKey("-t")) && (string.IsNullOrEmpty(options["-t"]) == false))
				{
					scm.ReopenFiles(P4.FileSpec.LocalSpecList(CheckedoutFiles), options);
				}
			}

			if (CheckedinFiles.Count > 0)
			{
				options = new P4.EditCmdOptions(P4.EditFilesCmdFlags.None, targetChangelist, dlg.FileType);

				if ((options.ContainsKey("-t")) && (string.IsNullOrEmpty(options["-t"]) == false))
				{
					scm.EditFiles(P4.FileSpec.LocalSpecList(CheckedinFiles), options);
				}
			}
            scm.BroadcastChangelistUpdate(null, new P4ScmProvider.ChangelistUpdateArgs(targetChangelist,
                            P4ScmProvider.ChangelistUpdateArgs.UpdateType.Edit));
        }

		private class ChangelistItem
		{
			public ChangelistItem(P4.Changelist _change)
			{
				change = _change;
			}
			public P4.Changelist change;

			public override string ToString()
			{
			    string line = change.Description;
			    line = line.Replace('\r', ' ');
			    line = line.Replace('\n', ' ');
			    return string.Format("{0} {1}", change.Id, line);
			}

		    public static implicit operator P4.Changelist(ChangelistItem it) 
			{
				return it.change;
			}
		}
		public FileAttributesDlg(P4ScmProvider scm)
		{
			_scm = scm;

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
			BaseTypeCB.Items.Clear();

            //P4VsOutputWindow.AppendMessage("Server version: {0}", _scm.ServerVersion.ToString());

			BaseTypeCB.Items.Add(P4.BaseFileType.Text);
			BaseTypeCB.Items.Add(P4.BaseFileType.Binary);
			BaseTypeCB.Items.Add(P4.BaseFileType.Symlink);
			BaseTypeCB.Items.Add(P4.BaseFileType.Apple);
			BaseTypeCB.Items.Add(P4.BaseFileType.Resource);
			BaseTypeCB.Items.Add(P4.BaseFileType.Unicode);
			BaseTypeCB.Items.Add(P4.BaseFileType.UTF16);
            if (_scm.ServerVersion >= Versions.V15_2)
                BaseTypeCB.Items.Add(P4.BaseFileType.UTF8);

			MaxRevsCB.Items.Clear();

			MaxRevsCB.Items.Add(1);
			MaxRevsCB.Items.Add(2);
			MaxRevsCB.Items.Add(3);
			MaxRevsCB.Items.Add(4);
			MaxRevsCB.Items.Add(5);
			MaxRevsCB.Items.Add(6);
			MaxRevsCB.Items.Add(7);
			MaxRevsCB.Items.Add(8);
			MaxRevsCB.Items.Add(9);
			MaxRevsCB.Items.Add(10);
			MaxRevsCB.Items.Add(16);
			MaxRevsCB.Items.Add(32);
			MaxRevsCB.Items.Add(64);
			MaxRevsCB.Items.Add(128);
			MaxRevsCB.Items.Add(256);
			MaxRevsCB.Items.Add(512);

			IList<P4.Changelist> changelists = _scm.GetAvailibleChangelists(-1);

			TargetChangeListCB.Items.Clear();

			TargetChangeListCB.Items.Add(Resources.Changelist_New);
			TargetChangeListCB.Items.Add(Resources.Changelist_Default);

			if (changelists != null)
			{
				foreach (P4.Changelist change in changelists)
				{
					TargetChangeListCB.Items.Add(new ChangelistItem(change));
				}
			}
		}

		P4ScmProvider _scm = null;

		P4.FileType FileType
		{
			get
			{
				P4.FileType value = null;
				P4.BaseFileType bft = P4.BaseFileType.Unspecified;

				if (ChangeBaseTypeRB.Checked)
				{
					bft = (P4.BaseFileType)BaseTypeCB.SelectedItem;
				}

				P4.FileTypeModifier mods = P4.FileTypeModifier.None;

				if (PreserveLocalModTimeCB.Checked)
				{
					mods |= P4.FileTypeModifier.ModTime;
				}
				if (AlwaywsWritableCB.Checked)
				{
					mods |= P4.FileTypeModifier.Writable;
				}
				if (ExcecBitSetCB.Checked)
				{
					mods |= P4.FileTypeModifier.Exec;
				}
				if (RcsKeywordExpansionCB.Checked)
				{
					mods |= P4.FileTypeModifier.KeywordsAll;
				}
				if (RcsLimitedKeywordExpansionCB.Checked)
				{
					mods |= P4.FileTypeModifier.KeywordsLimited;
				}
				if (ExclusiveCheckoutsCB.Checked)
				{
					mods |= P4.FileTypeModifier.ExclusiveOpen;
				}

				//if (UseDefaultForTypeRB.Checked)
				//{
				//    mods |= P4.FileTypeModifier.;
				//}
				if (CompressedFilesRB.Checked)
				{
					mods |= P4.FileTypeModifier.CompressedFiles;
				}
				if (RCSDeltasRB.Checked)
				{
					mods |= P4.FileTypeModifier.RCSDeltaFiles;
				}
				if (FullFileRB.Checked)
				{
					mods |= P4.FileTypeModifier.FullRevisions;
				}
				if (LimitRevsCB.Checked)
				{
					int revs = (int)MaxRevsCB.SelectedItem;
					if (revs > 1)
					{
						mods |= P4.FileTypeModifier.NRevsOnly;
						value = new P4.FileType(bft, mods, revs);
					}
					else
					{
						mods |= P4.FileTypeModifier.HeadrevOnly;
					}
				}
				if (value == null)
				{
					value = new P4.FileType(bft, mods);
					if (string.IsNullOrEmpty(value.ToString()))
					{
						bft = (P4.BaseFileType)BaseTypeCB.SelectedItem;
						value = new P4.FileType(bft, mods);
					}
				}
				return value;
			}
			set
			{
				if (value.BaseType == P4.BaseFileType.Unspecified)
				{
					ChangeBaseTypeRB.Checked = false;
					AddAtrributesRB.Checked = true;
					BaseTypeCB.Enabled = false;
					BaseTypeCB.SelectedItem = P4.BaseFileType.Text;
				}
				else
				{
					AddAtrributesRB.Checked = false;
					ChangeBaseTypeRB.Checked = true;
					BaseTypeCB.SelectedItem = value.BaseType;
				}
				PreserveLocalModTimeCB.Checked = ((value.Modifiers & P4.FileTypeModifier.ModTime) != 0);

				AlwaywsWritableCB.Checked = ((value.Modifiers & P4.FileTypeModifier.Writable) != 0);

				ExcecBitSetCB.Checked = ((value.Modifiers & P4.FileTypeModifier.Exec) != 0);

				RcsKeywordExpansionCB.Checked = ((value.Modifiers & P4.FileTypeModifier.KeywordsAll) != 0);

				RcsLimitedKeywordExpansionCB.Checked = ((value.Modifiers & P4.FileTypeModifier.KeywordsLimited) != 0);

				ExclusiveCheckoutsCB.Checked = ((value.Modifiers & P4.FileTypeModifier.ExclusiveOpen) != 0);

				UseDefaultForTypeRB.Checked = true;
				CompressedFilesRB.Checked = ((value.Modifiers & P4.FileTypeModifier.CompressedFiles) != 0);
				RCSDeltasRB.Checked = ((value.Modifiers & P4.FileTypeModifier.RCSDeltaFiles) != 0);
				FullFileRB.Checked = ((value.Modifiers & P4.FileTypeModifier.FullRevisions) != 0);

				MaxRevsCB.SelectedItem = 1;
				LimitRevsCB.Checked = ((value.Modifiers & (P4.FileTypeModifier.HeadrevOnly | P4.FileTypeModifier.NRevsOnly)) != 0);
				
				if (value.StoredRevs > 1)
				{
					MaxRevsCB.SelectedItem = value.StoredRevs;
				}
			}
		}

		public int TargetChangelistId
		{
			get 
			{
				if (TargetChangeListCB.SelectedIndex > 1)
				{
					return ((ChangelistItem) TargetChangeListCB.SelectedItem).change.Id;
				}
				if (TargetChangeListCB.SelectedIndex == 1)
				{
					return 0;
				}
				return -1;
			}
			set
			{
				if (value == -1)
				{
					TargetChangeListCB.SelectedIndex = 0;
				}
				else if (value == 0)
				{
					TargetChangeListCB.SelectedIndex = 1;
				}
				else
				{
					for (int idx = 0; idx < TargetChangeListCB.Items.Count; idx++)
					{
						if (TargetChangeListCB.Items[idx].ToString().StartsWith(value.ToString()))
						{
							TargetChangeListCB.SelectedIndex = idx;
							return;
						}
					}
					TargetChangeListCB.SelectedIndex = 1;
				}
			}
		}
		private void ChangeBaseTypeRB_CheckedChanged(object sender, EventArgs e)
		{
			UseDefaultForTypeRB.Text = Resources.FileAttributesDlg_UseDefaultForTypeRB_Text_1;
			BaseTypeCB.Enabled = true;
		}

		private void AddAtrributesRB_CheckedChanged(object sender, EventArgs e)
		{
			UseDefaultForTypeRB.Text = Resources.FileAttributesDlg_UseDefaultForTypeRB_Text_2;
			BaseTypeCB.Enabled = false;
		}

		private void RcsKeywordExpansionCB_CheckedChanged(object sender, EventArgs e)
		{
			if (RcsKeywordExpansionCB.Checked)
			{
				RcsLimitedKeywordExpansionCB.Checked = false;
			}
		}

		private void RcsLimitedKeywordExpansionCB_CheckedChanged(object sender, EventArgs e)
		{
			if (RcsLimitedKeywordExpansionCB.Checked)
			{
				RcsKeywordExpansionCB.Checked = false;
			}
		}


    }
}
