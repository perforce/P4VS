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
	public partial class ResolveFileAutoControl : UserControl
	{
		public enum AutoMode {}
		public ResolveFileAutoControl()
		{
			InitializeComponent();
		}

		public bool MergeBinaryAsText { get; set; }

		public string[] SourceFiles { get; set; }

		public P4ScmProvider Scm { get; set; }

		private IList<ResolveFileDlg.FileListViewItem> _selectedItems = null;
		public IList<ResolveFileDlg.FileListViewItem> SelectedItems 
{ 
			get { return _selectedItems;} 
			set
			{
				_selectedItems = value;
				AutoResolveBtn.Enabled = value != null;
			}
		}


		public enum AutoResolveMethod 
		{
			Safe, SafeMerge, Source, Target, MergeWithConflicts
		}
		public AutoResolveMethod SelectedMethod
		{
			get
			{
				if (AutoSafeMergeRB.Checked)
					return AutoResolveMethod.SafeMerge;
				if (AcceptSourceRB.Checked)
					return AutoResolveMethod.Source;
				if (AcceptTargetRB.Checked)
					return AutoResolveMethod.Target;
				if (AutoMergeConflictsRB.Checked)
					return AutoResolveMethod.MergeWithConflicts;
				return AutoResolveMethod.Safe;
			}
			set
			{
				SafeAutomaticRB.Checked = value == AutoResolveMethod.Safe;
				AutoSafeMergeRB.Checked = value == AutoResolveMethod.SafeMerge;
				AcceptSourceRB.Checked = value == AutoResolveMethod.Source;
				AcceptTargetRB.Checked = value == AutoResolveMethod.Target;
				AutoMergeConflictsRB.Checked = value == AutoResolveMethod.MergeWithConflicts;
			}
		}

		public ResolveFileDlg.InitFileListViewDelegate UpdateListView { get; set; }

		private void AutoResolveBtn_Click(object sender, EventArgs e)
		{
			if (SelectedItems == null)
			{
				return;
			}

			AutoResolveMethod method = SelectedMethod;

			P4.ResolveFilesCmdFlags flags = P4.ResolveFilesCmdFlags.None;
			if (MergeBinaryAsText)
			{
				flags |= P4.ResolveFilesCmdFlags.ForceTextualMerge;
			}
			switch (method)
			{
				default:
				case AutoResolveMethod.Safe:
					flags |= P4.ResolveFilesCmdFlags.AutomaticSafeMode;
					break;
				case AutoResolveMethod.SafeMerge:
					flags |= P4.ResolveFilesCmdFlags.AutomaticMergeMode;
					break;
				case AutoResolveMethod.Source:
					flags |= P4.ResolveFilesCmdFlags.AutomaticTheirsMode;
					break;
				case AutoResolveMethod.Target:
					flags |= P4.ResolveFilesCmdFlags.AutomaticYoursMode;
					break;
				case AutoResolveMethod.MergeWithConflicts:
					flags |= P4.ResolveFilesCmdFlags.AutomaticForceMergeMode;
					break;
			}
			P4.Options options = new P4.ResolveCmdOptions(flags, -1);

			//Dictionary<string, ResolveFileDlg.FileListViewItem> itemMap = new Dictionary<string, ResolveFileDlg.FileListViewItem>();

			int failedResolves = 0;
			foreach (ResolveFileDlg.FileListViewItem item in SelectedItems)
			{
				try
				{
					IList<P4.FileResolveRecord> records =
                            Scm.Connection.Repository.Connection.Client.ResolveFiles(
									null, options, item.ResolveRecord.LocalFilePath);

					//bool resolveFailed = true;
					//if (records != null)
					//{
					//    foreach (P4.FileResolveRecord r in records)
					//    {
					//        if (r.Action != P4.FileAction.None)
					//        {
					//            resolveFailed = false;
					//            break;
					//        }
					//    }
					//}
					//if (resolveFailed)
					//{
					//    failedResolves++;
					//}
					P4.P4CommandResult results = null;
					P4.FileMetaData newMd = Scm.GetFileMetaData(null, item.ResolveRecord.LocalFilePath.Path, out results);
					if (newMd.Unresolved)
					{
						failedResolves++;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, Resources.PerforceSCM, MessageBoxButtons.OK);
					return;
				}
			}
			if (UpdateListView != null)
			{
				UpdateListView();
			}
			if (failedResolves > 0)
			{
				string msg = string.Format(Resources.ResolveFileAutoControl_FilesNotResolvedWarning, failedResolves);
				MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void SetAutoDefaultBtn_Click(object sender, EventArgs e)
		{
			Preferences.LocalSettings["DefaultUatoMergeMethod"] = SelectedMethod;
		}

        public void CleanUp()
        {
        }
	}
}
