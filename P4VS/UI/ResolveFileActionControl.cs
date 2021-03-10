using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class ResolveFileActionControl : UserControl
	{
		public System.Windows.Forms.ImageList ButtonImages;

		public ResolveFileActionControl()
		{
			InitializeComponent();

			this.ButtonImages = new System.Windows.Forms.ImageList(this.components);
			// 
			// ButtonImages
			// 
			this.ButtonImages.TransparentColor = System.Drawing.Color.White;
			this.ButtonImages.Images.Add("TriangleDownGray.png", Images.TriangleDownGray);
			this.ButtonImages.Images.Add("TriangleDownRed.png", Images.TriangleDownRed);
			this.ButtonImages.Images.Add("TriangleRightGray.png", Images.TriangleRightGray);
			this.ButtonImages.Images.Add("TriangleRightRed.png", Images.TriangleRightRed);

			this.AdditionalActionsBtn.ImageList = this.ButtonImages;
		}
        ~ResolveFileActionControl()
        {
            if (sourceFile != null)
            {
                sourceFile.Dispose();
                sourceFile = null;
            }
            if (baseFile != null)
            {
                baseFile.Dispose();
                baseFile = null;
            }
            if (mergedFile != null)
            {
                mergedFile.Dispose();
                mergedFile = null;
            }
        }


		public P4ScmProvider Scm { get; set; }

		private ResolveFileDlg.FileListViewItem _selectedItem;
		public ResolveFileDlg.FileListViewItem SelectedItem 
		{
			get { return _selectedItem; } 
			set
			{
				_selectedItem = value;
				AcceptMergedBtn.Enabled = value != null;// && value.ConflictCnt == 0;

				AcceptSourceBtn.Enabled = value != null;
				AcceptTargetBtn.Enabled = value != null;
				AcceptTargetBtn.Enabled = value != null;
				AdditionalActionsBtn.Enabled = value != null;

				if (value != null)
				{
					AcceptMergedLbl.Visible = (value.ResolveAnalysis.Options & P4.ResolveOptions.Merge) != 0;
					AcceptMergedBtn.Visible = (value.ResolveAnalysis.Options & P4.ResolveOptions.Merge) != 0;

					if (value.ResolveAnalysis.SuggestedAction == P4.P4ClientMerge.MergeStatus.CMS_THEIRS)
					{
							RecomendationText = Resources.ResolveFileInteractiveControl_SuggestedAction_at;
					}
					else if (value.ResolveAnalysis.SuggestedAction == P4.P4ClientMerge.MergeStatus.CMS_YOURS)
					{
						RecomendationText = Resources.ResolveFileInteractiveControl_SuggestedAction_ay;
					}
					else if (value.ResolveAnalysis.SuggestedAction == P4.P4ClientMerge.MergeStatus.CMS_MERGED)
					{
						RecomendationText = Resources.ResolveFileInteractiveControl_SuggestedAction_ae;
					}

					switch (value.ResolveType)
					{
						case P4.ResolveType.Attribute:
							if (value.ResolveAnalysis.SuggestedAction == P4.P4ClientMerge.MergeStatus.CMS_THEIRS)
							{
								ReasonText = string.Format(Resources.ResolveFileActionControl_AttributeReason, Resources.ResolveFileInteractiveControl_Resolve_SourceFile);
							}
							else if (value.ResolveAnalysis.SuggestedAction == P4.P4ClientMerge.MergeStatus.CMS_YOURS)
							{
								ReasonText = string.Format(Resources.ResolveFileActionControl_AttributeReason, Resources.ResolveFileInteractiveControl_Resolve_TargetFile);
							}
							else if (value.ResolveAnalysis.SuggestedAction == P4.P4ClientMerge.MergeStatus.CMS_MERGED)
							{
								ReasonText = string.Format(Resources.ResolveFileActionControl_AttributeReason, Resources.ResolveFileActionControl_Resolve_BothFiles);
							}

							AcceptSourceLbl.Text = Resources.ResolveFileActionControl_AttributeAcceptSourceLabel;
							AcceptTargetLbl.Text = Resources.ResolveFileActionControl_AttributeAcceptTargetLabel;
							AcceptMergedLbl.Text = Resources.ResolveFileActionControl_AttributeAcceptMergedLabel;

							break;
						case P4.ResolveType.Branch:
							ReasonText = Resources.ResolveFileActionControl_BranchActionReason;

							AcceptSourceLbl.Text = Resources.ResolveFileActionControl_BranchAcceptSourceLabel;
							AcceptTargetLbl.Text = Resources.ResolveFileActionControl_BranchAcceptTargetLabel;
							AcceptMergedLbl.Text = string.Empty;

							break;
						case P4.ResolveType.Delete:
							ReasonText = Resources.ResolveFileActionControl_DeleteActionReason;

							AcceptSourceLbl.Text = Resources.ResolveFileActionControl_DeleteAcceptSourceLabel;
							AcceptTargetLbl.Text = Resources.ResolveFileActionControl_DeleteAcceptTargetLabel;
							AcceptMergedLbl.Text = string.Empty;


							break;
						case P4.ResolveType.Filetype:
							ReasonText = Resources.ResolveFileActionControl_FileTypeResolveReason;

							AcceptSourceLbl.Text = Resources.ResolveFileActionControl_FileTypeAcceptSourceLabel;
							AcceptTargetLbl.Text = Resources.ResolveFileActionControl_FileTypeAcceptTargetLabel;
							AcceptMergedLbl.Text = Resources.ResolveFileActionControl_FileTypeAcceptMergedLabel;

							break;
						case P4.ResolveType.Move:
							if (value.ResolveAnalysis.SuggestedAction == P4.P4ClientMerge.MergeStatus.CMS_THEIRS)
							{
								ReasonText = Resources.ResolveFileActionControl_RenameActionReasonSource;
							}
							else 
							{
								ReasonText = Resources.ResolveFileActionControl_RenameActionReasonTarget;
							}

							AcceptSourceLbl.Text = Resources.ResolveFileActionControl_MoveAcceptSourceLabel;
							AcceptTargetLbl.Text = Resources.ResolveFileActionControl_MoveAcceptTargetLabel;
							AcceptMergedLbl.Text = Resources.ResolveFileActionControl_MoveAcceptMergedLabel;

							break;
						// not an action resolve
						default:
					RecomendationText = string.Empty;
					ReasonText = string.Empty;
							break;
					}
					if (string.IsNullOrEmpty(value.ResolveAnalysis.MergeAction) == false)
					{
						AcceptMergedLbl.Text += ": " + value.ResolveAnalysis.MergeAction;
					}
					if (string.IsNullOrEmpty(value.ResolveAnalysis.YoursAction) == false)
					{
						AcceptTargetLbl.Text += ": " + value.ResolveAnalysis.YoursAction;
					}
					if (string.IsNullOrEmpty(value.ResolveAnalysis.TheirsAction) == false)
					{
						AcceptSourceLbl.Text += ": " + value.ResolveAnalysis.TheirsAction;
					}
				}
				else
				{
					_selectedItem = null;

					RecomendationText = string.Empty;
					ReasonText = string.Empty;
				}
			}
		}

		public bool MergeBinaryAsText { get; set; }

		public string RecomendationText
		{
			get { return RecomendationLbl.Text; }
			set { RecomendationLbl.Text = value; }
		}
		public string ReasonText
		{
			get { return ReasonLbl.Text; }
			set { ReasonLbl.Text = value; }
		}

		public ResolveFileDlg.InitFileListViewDelegate UpdateListView { get; set; }

		P4.P4ClientMerge.MergeStatus ResolveResult = P4.P4ClientMerge.MergeStatus.CMS_SKIP;

		private P4.P4ClientMerge.MergeStatus ResolveCallBackHandler(P4.FileResolveRecord resolveRecord,
			P4.Client.AutoResolveDelegate AutoResolve, string sourcePath, string targetPath, string basePath, string resultsPath)
		{
			//if (ResolveResult == P4.P4ClientMerge.MergeStatus.CMS_EDIT)
			//{
			//    if (resolveRecord.Analysis.ResolveType == P4.ResolveType.Content)
			//    {
			//        while (true)
			//        {
			//            Scm.MergeFiles(
			//                sourcePath, _selectedItem.ResolveRecord.FromFileSpec.ToString(),
			//                targetPath, _selectedItem.ResolveRecord.LocalFilePath.ToString(),
			//                basePath, _selectedItem.ResolveRecord.BaseFileSpec.ToString(),
			//                resultsPath);

			//            DialogResult answer = MessageBox.Show(Resources.ResolveSaveChanges, Resources.PerforceSCM, MessageBoxButtons.YesNoCancel);

			//            if (answer == DialogResult.Cancel)
			//            {
			//                // user wants to cancel
			//                return P4.P4ClientMerge.MergeStatus.CMS_SKIP;
			//            }
			//            if (answer == DialogResult.No)
			//            {
			//                // user wants to try again
			//                continue;
			//            }
			//            // user wants to keep merged file
			//            return P4.P4ClientMerge.MergeStatus.CMS_MERGED;
			//        }
			//    }
			//}
			return ResolveResult;
		}

		private P4.ResolveFilesCmdFlags GetResolveLimitFlag()
		{
			switch (_selectedItem._resolveType)
			{
				case P4.ResolveType.Attribute:
					return P4.ResolveFilesCmdFlags.FileAttributesOnly;

				case P4.ResolveType.Branch:
					return P4.ResolveFilesCmdFlags.FileBranchingOnly;

				case P4.ResolveType.Content:
					return P4.ResolveFilesCmdFlags.FileContentChangesOnly;

				case P4.ResolveType.Delete:
					return P4.ResolveFilesCmdFlags.FileDeletionsOnly;

				case P4.ResolveType.Filetype:
					return P4.ResolveFilesCmdFlags.FileTypeChangesOnly;

				case P4.ResolveType.Move:
					return P4.ResolveFilesCmdFlags.FileMovesOnly;

				default:
					return P4.ResolveFilesCmdFlags.None;
			}
		}

		private void AcceptSourceBtn_Click(object sender, EventArgs e)
		{
			P4.ResolveFilesCmdFlags flags = GetResolveLimitFlag();
			if (MergeBinaryAsText)
			{
				flags |= P4.ResolveFilesCmdFlags.ForceTextualMerge;
			}
			P4.Options options = new P4.ResolveCmdOptions(flags, -1);

			//FirstTry = true;

			P4.Client.ResolveFileDelegate resolveCallback = new P4.Client.ResolveFileDelegate(ResolveCallBackHandler);

			try
			{
				ResolveResult = P4.P4ClientMerge.MergeStatus.CMS_THEIRS;

                IList<P4.FileResolveRecord> records = Scm.Connection.Repository.Connection.Client.ResolveFiles(
					 resolveCallback, options, _selectedItem.ResolveRecord.LocalFilePath);

				foreach (P4.FileResolveRecord r in records)
				{
					if (r.Action != P4.FileAction.None)
					{
						if (UpdateListView != null)
						{
							UpdateListView();
						}
						break;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Resources.PerforceSCM, MessageBoxButtons.OK);
				return;
			}
		}

		private void AcceptTargetBtn_Click(object sender, EventArgs e)
		{
			P4.ResolveFilesCmdFlags flags = GetResolveLimitFlag();
			if (MergeBinaryAsText)
			{
				flags |= P4.ResolveFilesCmdFlags.ForceTextualMerge;
			}
			P4.Options options = new P4.ResolveCmdOptions(flags, -1);

			//FirstTry = true;

			P4.Client.ResolveFileDelegate resolveCallback = new P4.Client.ResolveFileDelegate(ResolveCallBackHandler);

			try
			{
				ResolveResult = P4.P4ClientMerge.MergeStatus.CMS_YOURS;

                IList<P4.FileResolveRecord> records = Scm.Connection.Repository.Connection.Client.ResolveFiles(
					 resolveCallback, options, _selectedItem.ResolveRecord.LocalFilePath);

				if ((records != null) && (records.Count > 0))
				{
					foreach (P4.FileResolveRecord r in records)
					{
						if (r.Action != P4.FileAction.None)
						{
							if (UpdateListView != null)
							{
								UpdateListView();
							}
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Resources.PerforceSCM, MessageBoxButtons.OK);
				return;
			}
		}

		//bool FirstTry;

		private void AcceptMergedBtn_Click(object sender, EventArgs e)
		{
			P4.ResolveFilesCmdFlags flags = GetResolveLimitFlag();
			if (MergeBinaryAsText)
			{
				flags |= P4.ResolveFilesCmdFlags.ForceTextualMerge;
			}
			P4.Options options = new P4.ResolveCmdOptions(flags, -1);

			//FirstTry = true;

			P4.Client.ResolveFileDelegate resolveCallback = new P4.Client.ResolveFileDelegate(ResolveCallBackHandler);

			try
			{
				ResolveResult = P4.P4ClientMerge.MergeStatus.CMS_MERGED;

                IList<P4.FileResolveRecord> records = Scm.Connection.Repository.Connection.Client.ResolveFiles(
					 resolveCallback, options, _selectedItem.ResolveRecord.LocalFilePath);

				foreach (P4.FileResolveRecord r in records)
				{
					if (r.Action != P4.FileAction.None)
					{
						if (UpdateListView != null)
						{
							UpdateListView();
						}
						break;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Resources.PerforceSCM, MessageBoxButtons.OK);
				return;
			}
		}

		//String ResolvePromptHandler(String msg, bool displayText)
		//{
		//    if (FirstTry)
		//    {
		//        FirstTry = false;
		//        // invoke the merge tool
		//        return "m";
		//    }
		//    DialogResult answer = MessageBox.Show(Resources.ResolveSaveChanges, Resources.PerforceSCM, MessageBoxButtons.YesNoCancel);

		//    if (answer == DialogResult.Cancel)
		//    {
		//        // user wants to cancel
		//        return "s";
		//    }
		//    if (answer == DialogResult.No)
		//    {
		//        // user wants to cancel
		//        return "m";
		//    }
		//    // user wants to keep merged file
		//    return "ae";
		//}

		private void RunMergeToolBtn_Click(object sender, EventArgs e)
		{
			P4.ResolveFilesCmdFlags flags = GetResolveLimitFlag();
			if (MergeBinaryAsText)
			{
				flags |= P4.ResolveFilesCmdFlags.ForceTextualMerge;
			}
			P4.Options options = new P4.ResolveCmdOptions(flags, -1);

			//FirstTry = true;

			P4.Client.ResolveFileDelegate resolver = new P4.Client.ResolveFileDelegate(ResolveCallBackHandler);
			try
			{
                IList<P4.FileResolveRecord> records = Scm.Connection.Repository.Connection.Client.ResolveFiles(
					 resolver, options, _selectedItem.ResolveRecord.FromFileSpec);

				foreach (P4.FileResolveRecord r in records)
				{
					if (r.Action != P4.FileAction.None)
					{
						UpdateListView();
						break;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Resources.PerforceSCM, MessageBoxButtons.OK);
				return;
			}
		}

		private void AdditionalActionsBtn_Click(object sender, EventArgs e)
		{
			AdditionalActionsBtn.ContextMenuStrip.Show(AdditionalActionsBtn, 
				new Point(0,AdditionalActionsBtn.Size.Height));
		}

		// Remember:
		// Target == Yours
		// Source == Theirs
		private void AdditionalActionsCtxMnu_Opening(object sender, CancelEventArgs e)
		{
		}
		TempFile sourceFile = null;
		TempFile baseFile = null;
		TempFile mergedFile = null;

		private void AAOpenSourceMI_Click(object sender, EventArgs e)
		{
			if (sourceFile == null)
			{
				sourceFile = new TempFile();
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					return;
				}
			}

			ShowFileContentsDlg dlg = new ShowFileContentsDlg();

			dlg.TempFile = sourceFile;
			dlg.Title = _selectedItem.ResolveRecord.FromFileSpec.ToString();

			// Show modeless
			dlg.Show();
		}

		private void AAOpenTargetMI_Click(object sender, EventArgs e)
		{
			ShowFileContentsDlg dlg = new ShowFileContentsDlg();

			dlg.FilePath = _selectedItem.ResolveRecord.LocalFilePath.Path;
			dlg.Title = _selectedItem.ResolveRecord.FromFileSpec.ToString();

			// Show modeless
			dlg.Show();
		}

		private void AAOpenMergedMI_Click(object sender, EventArgs e)
		{
			if (sourceFile == null)
			{
				sourceFile = new TempFile();
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					return;
				}
			}

			if (baseFile == null)
			{
				baseFile = new TempFile();
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					return;
				}
			}

			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			if (mergedFile == null)
			{
				mergedFile = new TempFile();

				if (Scm.MergeLocalFiles(baseFile, sourceFile, targetFile, mergedFile) == false)
				{
					return;
				}
			}
			ShowFileContentsDlg dlg = new ShowFileContentsDlg();

			dlg.TempFile = mergedFile;
			dlg.Title = string.Format(Resources.ResolveFileInteractiveControl_MergeOfFiles,
				_selectedItem.ResolveRecord.FromFileSpec.ToString(),
				_selectedItem.ResolveRecord.LocalFilePath.Path);

			// Show modeless
			dlg.Show();
		}
		private void AADiffSourceVsTargetMI_Click(object sender, EventArgs e)
		{
			if (sourceFile == null)
			{
				sourceFile = new TempFile();
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					return;
				}
			}

			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			Scm.Diff2Files(	sourceFile, 
							string.Format(Resources.ResolveFileInteractiveControl_SourceFile, 
											_selectedItem.ResolveRecord.FromFileSpec),
							targetFile,
							string.Format(Resources.ResolveFileInteractiveControl_TargetFile, 
											targetFile)
				);
		}

		private void AADiffBaseVsSourceMI_Click(object sender, EventArgs e)
		{
			if (sourceFile == null)
			{
				sourceFile = new TempFile();
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					return;
				}
			}

			if (baseFile == null)
			{
				baseFile = new TempFile();
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					return;
				}
			}

			Scm.Diff2Files(	baseFile,
							string.Format(Resources.ResolveFileInteractiveControl_BaseFile, _selectedItem.ResolveRecord.BaseFileSpec),
							sourceFile,
							string.Format(Resources.ResolveFileInteractiveControl_SourceFile, 
											_selectedItem.ResolveRecord.FromFileSpec)
							);
		}

		private void AADiffBaseVsTargetMI_Click(object sender, EventArgs e)
		{
			if (baseFile == null)
			{
				baseFile = new TempFile();
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					return;
				}
			}

			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			Scm.Diff2Files(baseFile,
							string.Format(Resources.ResolveFileInteractiveControl_SourceFile, 
											_selectedItem.ResolveRecord.BaseFileSpec),
							targetFile,
							string.Format(Resources.ResolveFileInteractiveControl_TargetFile, 
											targetFile)
				);
		}

		private void AADiffBaseVsMergedMI_Click(object sender, EventArgs e)
		{
			if (sourceFile == null)
			{
				sourceFile = new TempFile();
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					return;
				}
			}

			if (baseFile == null)
			{
				baseFile = new TempFile();
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					return;
				}
			}

			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			if (mergedFile == null)
			{
				mergedFile = new TempFile();

				if (Scm.MergeLocalFiles(baseFile, sourceFile, targetFile, mergedFile) == false)
				{
					return;
				}
			}
			Scm.Diff2Files(baseFile,
							string.Format("Base ({0})", baseFile.ToString()),
							mergedFile,
							"Merged Source/Target");
		}
		private void AADiffSourceVsMergedMI_Click(object sender, EventArgs e)
		{
			if (sourceFile == null)
			{
				sourceFile = new TempFile();
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					return;
				}
			}

			if (baseFile == null)
			{
				baseFile = new TempFile();
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					return;
				}
			}

			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			if (mergedFile == null)
			{
				mergedFile = new TempFile();

				if (Scm.MergeLocalFiles(baseFile, sourceFile, targetFile, mergedFile) == false)
				{
					return;
				}
			}
			Scm.Diff2Files(sourceFile,
							string.Format(Resources.ResolveFileInteractiveControl_SourceFile,
											_selectedItem.ResolveRecord.FromFileSpec),
							mergedFile,
							"Merged Source/Target");
		}

		private void AADiffTargetVsMergedMI_Click(object sender, EventArgs e)
		{
			if (sourceFile == null)
			{
				sourceFile = new TempFile();
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					return;
				}
			}

			if (baseFile == null)
			{
				baseFile = new TempFile();
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					return;
				}
			}

			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			if (mergedFile == null)
			{
				mergedFile = new TempFile();

				if (Scm.MergeLocalFiles(baseFile, sourceFile, targetFile, mergedFile) == false)
				{
					return;
				}
			}
			Scm.Diff2Files(	targetFile,
							string.Format(Resources.ResolveFileInteractiveControl_TargetFile, targetFile),
							mergedFile,
							Resources.ResolveFileInteractiveControl_MergedSourceTarget);
		}

		private void AAHistorySourceMI_Click(object sender, EventArgs e)
		{
			string sourceFile = _selectedItem.ResolveRecord.FromFileSpec.DepotPath.Path;
			FileHistoryDialog dlg = new FileHistoryDialog(Scm);
			dlg.Files = new List<string> { sourceFile };
			dlg.Show();
		}

		private void AAHistoryTargetMI_Click(object sender, EventArgs e)
		{
			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;
			FileHistoryDialog dlg = new FileHistoryDialog(Scm);
			dlg.Files = new List<string> { targetFile };
			dlg.Show();
		}

		private void AATimeLapseSourceMI_Click(object sender, EventArgs e)
		{
			string sourceFile = _selectedItem.ResolveRecord.FromFileSpec.DepotPath.Path;

			Scm.LaunchTimeLapseView(sourceFile);
		}

		private void AATimeLapseTargetMI_Click(object sender, EventArgs e)
		{
			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			Scm.LaunchTimeLapseView(targetFile);
		}

		private void AARevisionGraphSourceMI_Click(object sender, EventArgs e)
		{
			string sourceFile = _selectedItem.ResolveRecord.FromFileSpec.DepotPath.Path;

			Scm.LaunchRevisionGraphView(sourceFile);
		}

		private void AARevisionGraphTargetMI_Click(object sender, EventArgs e)
		{
			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			Scm.LaunchRevisionGraphView(targetFile);
		}

        public void Cleanup()
        {
            if (sourceFile != null)
            {
                sourceFile.Dispose();
                sourceFile = null;
            }
            if (baseFile != null)
            {
                baseFile.Dispose();
                baseFile = null;
            }
            if (mergedFile != null)
            {
                mergedFile.Dispose();
                mergedFile = null;
            }
        }
    }
}
