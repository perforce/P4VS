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
	public partial class ResolveFileInteractiveControl : UserControl
	{
		string CommonDiffsLblFormat = "{0}";

		public System.Windows.Forms.ImageList ButtonImages;

		public ResolveFileInteractiveControl()
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

			this.pictureBox1.Image = Images.ResolveFlow;
			this.pictureBox2.Image = Images.ResolveCommon;

			this.AdditionalActionsBtn.ImageList = this.ButtonImages;

			CommonDiffsLblFormat = CommonDiffsLbl.Text;
		}

		private P4ScmProvider _scm = null;
		public P4ScmProvider Scm 
		{
			get { return _scm; }
			set
			{
				_scm = value;
				if (_scm != null)
				{
					RunMergeToolBtn.Enabled = _scm.P4MergeExists();
					AADiffMI.Enabled = RunMergeToolBtn.Enabled;
					if (RunMergeToolBtn.Enabled == false)
					{
						RunMergeToolLbl.Text = RunMergeToolLbl.Text +
							Resources.ResolveFileInteractiveControl_DiffToolNotFoundWarning;
					}
				}
			}
		}

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
					switch (value.ResolveAnalysis.SuggestedAction)
					{
						case P4.P4ClientMerge.MergeStatus.CMS_THEIRS:
							RecomendationText = Resources.ResolveFileInteractiveControl_SuggestedAction_at;
							ReasonText = Resources.ResolveFileInteractiveControl_SuggestedReason_at;
							break;
						case P4.P4ClientMerge.MergeStatus.CMS_YOURS:
							RecomendationText = Resources.ResolveFileInteractiveControl_SuggestedAction_ay;
							ReasonText = Resources.ResolveFileInteractiveControl_SuggestedReason_ay;
							break;
						case P4.P4ClientMerge.MergeStatus.CMS_MERGED:
							RecomendationText = Resources.ResolveFileInteractiveControl_SuggestedAction_am;
							ReasonText = Resources.ResolveFileInteractiveControl_SuggestedReason_am;
							break;
						case P4.P4ClientMerge.MergeStatus.CMS_EDIT:
							RecomendationText = Resources.ResolveFileInteractiveControl_SuggestedAction_ae;
							ReasonText = Resources.ResolveFileInteractiveControl_SuggestedReason_ae;
							break;
						case P4.P4ClientMerge.MergeStatus.CMS_SKIP:
							RecomendationText = Resources.ResolveFileInteractiveControl_SuggestedAction_e;
							ReasonText = Resources.ResolveFileInteractiveControl_SuggestedReason_e;
							break;
						//case P4.P4ClientMerge.MergeStatus.CMS_EDIT:
						//    RecomendationText = Resources.SuggestedAction_e;
						//    ReasonText = Resources.SuggestedReason_e;
						//    break;
						//case "a":
						default:
							RecomendationText = value.ResolveAnalysis.SuggestedAction.ToString();
							ReasonText = Resources.ResolveFileInteractiveControl_SuggestedReason_default;
							break;
					}

					bool hasBaseFile = ((value.ResolveRecord != null) && (value.ResolveRecord.BaseFileSpec != null));
					bool hasToFile = ((value.ResolveRecord != null) && (value.ResolveRecord.LocalFilePath != null));
					bool hasFromFile = ((value.ResolveRecord != null) && (value.ResolveRecord.FromFileSpec != null));
					
					if (hasBaseFile)
					{
					    if (value.ResolveRecord != null) BaseFileText = value.ResolveRecord.BaseFileSpec.ToString();
					}
					else
					{
						BaseFileText = string.Empty;
					}

				    if (value.ResolveRecord != null)
				        AADiffMI.Enabled = RunMergeToolBtn.Enabled && (value.ResolveRecord.ResolveType == P4.ResolveType.Content);

				    AADiffSourceVsTargetMI.Enabled = hasFromFile && hasToFile;
					AADiffBaseVsSourceMI.Enabled = hasBaseFile && hasFromFile;
					AADiffBaseVsTargetMI.Enabled = hasBaseFile && hasToFile;
				    if (value.ResolveRecord != null)
				    {
				        AADiffBaseVsMergedMI.Enabled = hasBaseFile && (value.ResolveRecord.ResolveType == P4.ResolveType.Content);
				        AADiffSourceVsMergedMI.Enabled = hasFromFile && (value.ResolveRecord.ResolveType == P4.ResolveType.Content);
				        AADiffTargetVsMergedMI.Enabled = hasToFile && (value.ResolveRecord.ResolveType == P4.ResolveType.Content);
				    }

				    SourceDifferencesText = value.ResolveAnalysis.SourceDiffCnt.ToString();
					TargetDifferencesText = value.ResolveAnalysis.TargetDiffCnt.ToString();
					ConflictsText = value.ResolveAnalysis.ConflictCount.ToString();
					if (value.ResolveAnalysis.CommonDiffCount > 0)
					{
						CommonDiffsLbl.Visible = true;
						pictureBox2.Visible = true;
						CommonDiffsText = string.Format(CommonDiffsLblFormat, value.ResolveAnalysis.CommonDiffCount);
					}
					else
					{
						CommonDiffsLbl.Visible = false;
						pictureBox2.Visible = false;
					}
				}
				else
				{
					_selectedItem = null;

					RecomendationText = string.Empty;
					ReasonText = string.Empty;
					BaseFileText = string.Empty;
					SourceDifferencesText = string.Empty;
					TargetDifferencesText = string.Empty;
					ConflictsText = string.Empty;
					CommonDiffsText = string.Empty;
					CommonDiffsLbl.Visible = false;
					pictureBox2.Visible = false;

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
		public string BaseFileText
		{
			get { return BaseFileLbl.Text; }
			set { BaseFileLbl.Text = value; }
		}
		public string SourceDifferencesText
		{
			get { return SourceDifferencesLbl.Text; }
			set { SourceDifferencesLbl.Text = value; }
		}
		public string TargetDifferencesText
		{
			get { return TargetDifferencesLbl.Text; }
			set { TargetDifferencesLbl.Text = value; }
		}
		public string ConflictsText
		{
			get { return ConflictsLbl.Text; }
			set { ConflictsLbl.Text = value; }
		}
		public string CommonDiffsText
		{
			get { return CommonDiffsLbl.Text; }
			set { CommonDiffsLbl.Text = value; }
		}

		public ResolveFileDlg.InitFileListViewDelegate UpdateListView { get; set; }

		private void AcceptSourceBtn_Click(object sender, EventArgs e)
		{
			P4.ResolveFilesCmdFlags flags = P4.ResolveFilesCmdFlags.FileContentChangesOnly;
			if (MergeBinaryAsText)
			{
				flags |= P4.ResolveFilesCmdFlags.ForceTextualMerge;
			}
			P4.Options options = new P4.ResolveCmdOptions(flags, -1);

			//FirstTry = true;

			P4.Client.ResolveFileDelegate resolveCallback = new P4.Client.ResolveFileDelegate(MergeCallBackHandler);

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
			P4.ResolveFilesCmdFlags flags = P4.ResolveFilesCmdFlags.FileContentChangesOnly;
			if (MergeBinaryAsText)
			{
				flags |= P4.ResolveFilesCmdFlags.ForceTextualMerge;
			}
			P4.Options options = new P4.ResolveCmdOptions(flags, -1);

			//FirstTry = true;

			P4.Client.ResolveFileDelegate resolveCallback = new P4.Client.ResolveFileDelegate(MergeCallBackHandler);

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

		private void AcceptMergedBtn_Click(object sender, EventArgs e)
		{
			P4.ResolveFilesCmdFlags flags = P4.ResolveFilesCmdFlags.FileContentChangesOnly;
			if (MergeBinaryAsText)
			{
				flags |= P4.ResolveFilesCmdFlags.ForceTextualMerge;
			}
			P4.Options options = new P4.ResolveCmdOptions(flags, -1);

			//FirstTry = true;

			P4.Client.ResolveFileDelegate resolveCallback = new P4.Client.ResolveFileDelegate(MergeCallBackHandler);

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

		P4.P4ClientMerge.MergeStatus ResolveResult = P4.P4ClientMerge.MergeStatus.CMS_SKIP;

		private P4.P4ClientMerge.MergeStatus MergeCallBackHandler(P4.FileResolveRecord resolveRecord,
			P4.Client.AutoResolveDelegate AutoResolve, string sourcePath, string targetPath, string basePath, string resultsPath)
		{
			if (ResolveResult == P4.P4ClientMerge.MergeStatus.CMS_EDIT)
			{
				if (resolveRecord.Analysis.ResolveType == P4.ResolveType.Content)
				{
					while (true)
					{
						Scm.MergeFiles(
							sourcePath, _selectedItem.ResolveRecord.FromFileSpec.ToString(),
							targetPath, _selectedItem.ResolveRecord.LocalFilePath.ToString(),
							basePath, _selectedItem.ResolveRecord.BaseFileSpec.ToString(),
							resultsPath);

						DialogResult answer = MessageBox.Show(Resources.ResolveFileInteractiveControl_ResolveSaveChanges, Resources.PerforceSCM, MessageBoxButtons.YesNoCancel);

						if (answer == DialogResult.Cancel)
						{
							// user wants to cancel
							return P4.P4ClientMerge.MergeStatus.CMS_SKIP;
						}
						if (answer == DialogResult.No)
						{
							// user wants to try again
							continue;
						}
						// user wants to keep merged file
						return P4.P4ClientMerge.MergeStatus.CMS_MERGED;
					}
				}
			}
			return ResolveResult;
		}

		//bool FirstTry;
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
			P4.ResolveFilesCmdFlags flags = P4.ResolveFilesCmdFlags.FileContentChangesOnly;
			if (MergeBinaryAsText)
			{
				flags |= P4.ResolveFilesCmdFlags.ForceTextualMerge;
			}
			P4.Options options = new P4.ResolveCmdOptions(flags, -1);

			//FirstTry = true;

			P4.Client.ResolveFileDelegate resolveCallback =	new P4.Client.ResolveFileDelegate(MergeCallBackHandler);

			try
			{
				ResolveResult = P4.P4ClientMerge.MergeStatus.CMS_EDIT;

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
			if ((sourceFile == null) || (!System.IO.File.Exists(sourceFile)))
			{
				sourceFile = new TempFile(_selectedItem.ResolveRecord.FromFileSpec);
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					sourceFile.Dispose();
					sourceFile = null;
					return;
				}
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
				dlg.Title = _selectedItem.ResolveRecord.FromFileSpec.ToString();

				// Show modeless
				dlg.Show();
			}
		}

		private void AAOpenTargetMI_Click(object sender, EventArgs e)
		{
			if (Preferences.LocalSettings.GetBool("OpenShelvedFileInEditor", true))
			{
				EnvDTE.DTE dte = P4VsProvider.GetDTE();
				dte.ItemOperations.OpenFile(_selectedItem.ResolveRecord.LocalFilePath.Path, null);
			}
			else
			{
				ShowFileContentsDlg dlg = new ShowFileContentsDlg();

				dlg.FilePath = _selectedItem.ResolveRecord.LocalFilePath.Path;
				dlg.Title = _selectedItem.ResolveRecord.FromFileSpec.ToString();

				// Show modeless
				dlg.Show();
			}
		}

		private void AAOpenMergedMI_Click(object sender, EventArgs e)
		{
			if ((sourceFile == null) || (!System.IO.File.Exists(sourceFile)))
			{
				sourceFile = new TempFile(_selectedItem.ResolveRecord.FromFileSpec);
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					sourceFile.Dispose();
					sourceFile = null;
					return;
				}
			}

			if ((baseFile == null) || (!System.IO.File.Exists(baseFile)))
			{
				baseFile = new TempFile(_selectedItem.ResolveRecord.BaseFileSpec);
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					baseFile = null;
					return;
				}
			}
			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			if ((mergedFile == null) || (!System.IO.File.Exists(mergedFile)))
			{
				string mergedFileName = string.Format("{0}(merged {1}[{2}]){3}",
					System.IO.Path.GetFileNameWithoutExtension(_selectedItem.ResolveRecord.LocalFilePath.Path),
					((P4.VersionRange)_selectedItem.ResolveRecord.FromFileSpec.Version).Upper,
					_selectedItem.ResolveRecord.BaseFileSpec.Version,
					System.IO.Path.GetExtension(_selectedItem.ResolveRecord.LocalFilePath.Path));

				mergedFile = new TempFile(mergedFileName);

				if (Scm.MergeLocalFiles(baseFile, sourceFile, targetFile, mergedFile) == false)
				{
					MessageBox.Show(Resources.ResolveFileInteractiveControl_CannotCreateMergedFile, Resources.PerforceSCM, MessageBoxButtons.OK);
					mergedFile.Dispose();
					mergedFile = null;
					return;
				}
			}
			if (Preferences.LocalSettings.GetBool("OpenShelvedFileInEditor", true))
			{
				EnvDTE.DTE dte = P4VsProvider.GetDTE();
				dte.ItemOperations.OpenFile(mergedFile, null);
			}
			else
			{
				ShowFileContentsDlg dlg = new ShowFileContentsDlg();

				dlg.TempFile = mergedFile;
				dlg.Title = string.Format(Resources.ResolveFileInteractiveControl_MergeOfFiles,
											_selectedItem.ResolveRecord.FromFileSpec.ToString(),
											_selectedItem.ResolveRecord.LocalFilePath.Path);

				// Show modeless
				dlg.Show();
			}
		}
		private void AADiffSourceVsTargetMI_Click(object sender, EventArgs e)
		{
			if ((sourceFile == null) || (!System.IO.File.Exists(sourceFile)))
			{
				sourceFile = new TempFile(_selectedItem.ResolveRecord.FromFileSpec);
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					sourceFile.Dispose();
					sourceFile = null;
					return;
				}
			}
			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			Scm.Diff2Files(sourceFile,
							string.Format(Resources.ResolveFileInteractiveControl_SourceFile,
											_selectedItem.ResolveRecord.FromFileSpec),
							targetFile,
							string.Format(Resources.ResolveFileInteractiveControl_TargetFile,
											targetFile)
							);
		}

		private void AADiffBaseVsSourceMI_Click(object sender, EventArgs e)
		{
			if ((sourceFile == null) || (!System.IO.File.Exists(sourceFile)))
			{
				sourceFile = new TempFile(_selectedItem.ResolveRecord.FromFileSpec);
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					sourceFile.Dispose();
					sourceFile = null;
					return;
				}
			}

			if ((baseFile == null) || (!System.IO.File.Exists(baseFile)))
			{
				baseFile = new TempFile(_selectedItem.ResolveRecord.BaseFileSpec);
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					baseFile.Dispose();
					baseFile = null;
					return;
				}
			}

			Scm.Diff2Files(	baseFile,
							string.Format(Resources.ResolveFileInteractiveControl_BaseFile, 
											_selectedItem.ResolveRecord.BaseFileSpec),
							sourceFile, 
							string.Format(Resources.ResolveFileInteractiveControl_SourceFile, 
											_selectedItem.ResolveRecord.FromFileSpec)
							);
		}

		private void AADiffBaseVsTargetMI_Click(object sender, EventArgs e)
		{
			baseFile = new TempFile(_selectedItem.ResolveRecord.BaseFileSpec);
			{
				baseFile = new TempFile(_selectedItem.ResolveRecord.BaseFileSpec);
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					baseFile.Dispose();
					baseFile = null;
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
			if ((sourceFile == null) || (!System.IO.File.Exists(sourceFile)))
			{
				sourceFile = new TempFile(_selectedItem.ResolveRecord.FromFileSpec);
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					sourceFile.Dispose();
					sourceFile = null;
					return;
				}
			}

			if ((baseFile == null) || (!System.IO.File.Exists(baseFile)))
			{
				baseFile = new TempFile(_selectedItem.ResolveRecord.BaseFileSpec);
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					baseFile.Dispose();
					baseFile = null;
					return;
				}
			}

			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			if ((mergedFile == null) || (!System.IO.File.Exists(mergedFile)))
			{
				string mergedFileName = string.Format("{0}(merged {1}[{2}]){3}",
					System.IO.Path.GetFileNameWithoutExtension(_selectedItem.ResolveRecord.LocalFilePath.Path),
					((P4.VersionRange)_selectedItem.ResolveRecord.FromFileSpec.Version).Upper,
					_selectedItem.ResolveRecord.BaseFileSpec.Version,
					System.IO.Path.GetExtension(_selectedItem.ResolveRecord.LocalFilePath.Path));

				mergedFile = new TempFile(mergedFileName);

				if (Scm.MergeLocalFiles(baseFile, sourceFile, targetFile, mergedFile) == false)
				{
					MessageBox.Show(Resources.ResolveFileInteractiveControl_CannotCreateMergedFile, Resources.PerforceSCM, MessageBoxButtons.OK);
					mergedFile.Dispose();
					mergedFile = null;
					return;
				}
			}
			Scm.Diff2Files(baseFile,
							string.Format(Resources.ResolveFileInteractiveControl_BaseFile, 
											baseFile.ToString()),
							mergedFile,
							Resources.ResolveFileInteractiveControl_MergedSourceTarget);
		}
		private void AADiffSourceVsMergedMI_Click(object sender, EventArgs e)
		{
			if ((sourceFile == null) || (!System.IO.File.Exists(sourceFile)))
			{
				sourceFile = new TempFile(_selectedItem.ResolveRecord.FromFileSpec);
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					sourceFile.Dispose();
					sourceFile = null;
					return;
				}
			}

			if ((baseFile == null) || (!System.IO.File.Exists(baseFile)))
			{
				baseFile = new TempFile(_selectedItem.ResolveRecord.BaseFileSpec);
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					baseFile.Dispose();
					baseFile = null;
					return;
				}
			}

			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			if ((mergedFile == null) || (!System.IO.File.Exists(mergedFile)))
			{
				string mergedFileName = string.Format("{0}(merged {1}[{2}]){3}",
					System.IO.Path.GetFileNameWithoutExtension(_selectedItem.ResolveRecord.LocalFilePath.Path),
					((P4.VersionRange)_selectedItem.ResolveRecord.FromFileSpec.Version).Upper,
					_selectedItem.ResolveRecord.BaseFileSpec.Version,
					System.IO.Path.GetExtension(_selectedItem.ResolveRecord.LocalFilePath.Path));

				mergedFile = new TempFile(mergedFileName);

				if (Scm.MergeLocalFiles(baseFile, sourceFile, targetFile, mergedFile) == false)
				{
					MessageBox.Show(Resources.ResolveFileInteractiveControl_CannotCreateMergedFile, Resources.PerforceSCM, MessageBoxButtons.OK);
					mergedFile.Dispose();
					mergedFile = null;
					return;
				}
			}
			Scm.Diff2Files(sourceFile,
							string.Format(Resources.ResolveFileInteractiveControl_SourceFile, 
											_selectedItem.ResolveRecord.FromFileSpec),
							mergedFile,
							Resources.ResolveFileInteractiveControl_MergedSourceTarget);
		}

		private void AADiffTargetVsMergedMI_Click(object sender, EventArgs e)
		{
			if ((sourceFile == null) || (!System.IO.File.Exists(sourceFile)))
			{
				sourceFile = new TempFile(_selectedItem.ResolveRecord.FromFileSpec);
				if (Scm.GetFileVersion(sourceFile, _selectedItem.ResolveRecord.FromFileSpec) == null)
				{
					sourceFile.Dispose();
					sourceFile = null;
					return;
				}
			}

			if ((baseFile == null) || (!System.IO.File.Exists(baseFile)))
			{
				baseFile = new TempFile(_selectedItem.ResolveRecord.BaseFileSpec);
				if (Scm.GetFileVersion(baseFile, _selectedItem.ResolveRecord.BaseFileSpec) == null)
				{
					baseFile.Dispose();
					baseFile = null;
					return;
				}
			}

			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;

			if ((mergedFile == null) || (!System.IO.File.Exists(mergedFile)))
			{
				string mergedFileName = string.Format("{0}(merged {1}[{2}]){3}",
					System.IO.Path.GetFileNameWithoutExtension(_selectedItem.ResolveRecord.LocalFilePath.Path),
					((P4.VersionRange)_selectedItem.ResolveRecord.FromFileSpec.Version).Upper,
					_selectedItem.ResolveRecord.BaseFileSpec.Version,
					System.IO.Path.GetExtension(_selectedItem.ResolveRecord.LocalFilePath.Path));

				mergedFile = new TempFile(mergedFileName);

				if (Scm.MergeLocalFiles(baseFile, sourceFile, targetFile, mergedFile) == false)
				{
					MessageBox.Show(Resources.ResolveFileInteractiveControl_CannotCreateMergedFile, Resources.PerforceSCM, MessageBoxButtons.OK);
					mergedFile.Dispose();
					mergedFile = null;
					return;
				}
			}
			Scm.Diff2Files(	targetFile,
							string.Format(Resources.ResolveFileInteractiveControl_TargetFile, 
											targetFile),
							mergedFile,
							Resources.ResolveFileInteractiveControl_MergedSourceTarget);
		}
		private delegate void ShowDialogDelegate();

		private void AAHistorySourceMI_Click(object sender, EventArgs e)
		{
			string sourceFile = _selectedItem.ResolveRecord.FromFileSpec.DepotPath.Path;
			FileHistoryDialog dlg = new FileHistoryDialog(Scm);
			dlg.Files = new List<string> { sourceFile };
			if (dlg.InvokeRequired)
			{
				dlg.Invoke(new ShowDialogDelegate(dlg.Show));
			}
			else
			{
				dlg.Show();
			}
		}

		private void AAHistoryTargetMI_Click(object sender, EventArgs e)
		{
			string targetFile = _selectedItem.ResolveRecord.LocalFilePath.Path;
			FileHistoryDialog dlg = new FileHistoryDialog(Scm);
			dlg.Files = new List<string> { targetFile };
			if (dlg.InvokeRequired)
			{
				dlg.Invoke(new ShowDialogDelegate(dlg.Show));
			}
			else
			{
				dlg.Show();
			}
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
