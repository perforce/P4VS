using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Perforce.P4Scm;
using NLog;
using System.Net;
using Perforce.P4;
using EnvDTE80;
using EnvDTE;

namespace Perforce.P4VS
{
    public abstract class Action : Checkout
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private void LogFunctionCall(string functionName)
        {
            logger.Trace("Fn {0}", functionName);
        }

        public void P4VsRevertUnchanged(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
            IList<string> files = null;
            if (TreatProjectsAsFolders)
            {
                files = SccService.SelectedFilesFolders;
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            IList<string> revertedFiles = SccService.RevertFiles(true, files);

            // now refresh the selected nodes' glyphs
            Glyphs.RefreshFilesAndGlyphs(revertedFiles);
        }

        public void P4VsLock(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
            IList<string> files = null;
            if (TreatProjectsAsFolders)
            {
                files = SccService.SelectedFilesFolders;
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            SccService.LockFiles(files);

            // now refresh the selected nodes' glyphs
            Glyphs.RefreshNodesGlyphs(selectedNodes, files);
        }

        public void P4VsUnlock(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
            IList<string> files = null;
            if (TreatProjectsAsFolders)
            {
                files = SccService.SelectedFilesFolders;
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            SccService.UnlockFiles(files);

            // now refresh the selected nodes' glyphs
            Glyphs.RefreshNodesGlyphs(selectedNodes, files);
        }

        public void P4VsChangeFileType(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
            IList<string> files = null;
            if (TreatProjectsAsFolders)
            {
                files = SccService.SelectedFilesFolders;
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            SccService.ChangeFileType(files);

            // now refresh the selected nodes' glyphs
            Glyphs.RefreshNodesGlyphs(selectedNodes, files);
        }

        public void P4VsMoveToChangelist(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
            IList<string> files = null;
            if (TreatProjectsAsFolders)
            {
                files = SccService.SelectedFilesFolders;
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            SccService.MoveToChangelist(files);

            // now refresh the selected nodes' glyphs
            Glyphs.RefreshNodesGlyphs(selectedNodes, files);
        }

        public void P4VsAddToIgnoreList(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution() || !P4ScmProvider.P4IgnoreSet)
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<string> files = null;
            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            if (SccService.IsSolutionSelected)
            {
                files = new List<string>();
                files.Add(GetSolutionFileName());
            }
            else if (SccService.IsaControlledProjectSelected)
            {
                files = new List<string>();
                foreach (VSITEMSELECTION node in selectedNodes)
                {
                    if (((node.itemid == VSConstants.VSITEMID_ROOT)) && (SccService.IsProjectControlled(node.pHier)))
                    {
                        IVsSccProject2 proj = node.pHier as IVsSccProject2;
                        if (proj != null)
                        {
                            files.Add(GetProjectFileName(proj));
                        }
                    }
                }
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            SccService.AddToIgnoreList(files, false, false);
            SccService.ScmProvider.Connection.Repository.Connection.ReleaseConnection();
            Glyphs.RefreshNodesGlyphs(selectedNodes, files);
        }

        public void P4VsRemoveFromIgnoreList(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution() || !P4ScmProvider.P4IgnoreSet)
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<string> files = null;
            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            if (SccService.IsSolutionSelected)
            {
                files = new List<string>();
                files.Add(GetSolutionFileName());
            }
            else if (SccService.IsaControlledProjectSelected)
            {
                files = new List<string>();
                foreach (VSITEMSELECTION node in selectedNodes)
                {
                    if (((node.itemid == VSConstants.VSITEMID_ROOT)) && (SccService.IsProjectControlled(node.pHier)))
                    {
                        IVsSccProject2 proj = node.pHier as IVsSccProject2;
                        if (proj != null)
                        {
                            files.Add(GetProjectFileName(proj));
                        }
                    }
                }
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            SccService.AddToIgnoreList(files, true, false);
            SccService.ScmProvider.Connection.Repository.Connection.ReleaseConnection();
            Glyphs.RefreshNodesGlyphs(selectedNodes, files);
        }

        public void P4VsEditIgnoreList(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution() || !P4ScmProvider.P4IgnoreSet)
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<string> files = null;
            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            if (SccService.IsSolutionSelected)
            {
                files = new List<string>();
                files.Add(GetSolutionFileName());
            }
            else if (SccService.IsaControlledProjectSelected)
            {
                files = new List<string>();
                foreach (VSITEMSELECTION node in selectedNodes)
                {
                    if (((node.itemid == VSConstants.VSITEMID_ROOT)) && (SccService.IsProjectControlled(node.pHier)))
                    {
                        IVsSccProject2 proj = node.pHier as IVsSccProject2;
                        if (proj != null)
                        {
                            files.Add(GetProjectFileName(proj));
                        }
                    }
                }
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            SccService.AddToIgnoreList(files, false, true);
            SccService.ScmProvider.Connection.Repository.Connection.ReleaseConnection();
            Glyphs.RefreshNodesGlyphs(selectedNodes, files);
        }

        public void P4VsResolve(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
            IList<string> selected = null;
            if (TreatProjectsAsFolders)
            {
                selected = SccService.SelectedFilesFolders;
            }
            else
            {
                selected = SccService.SelectedFiles;
            }

            SccService.ResolveFiles(selected);
            Glyphs.RefreshNodesGlyphs(selectedNodes, selected);
        }

        public void P4VsSync(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
            IList<string> selected = null;
            if (TreatProjectsAsFolders)
            {
                selected = SccService.SelectedFilesFolders;
            }
            else
            {
                selected = SccService.SelectedFiles;
            }

            IList<P4.FileSpec> files = new List<P4.FileSpec>();
            foreach (string s in selected)
            {
                P4.FileSpec file = new P4.FileSpec();
                file.LocalPath = new P4.LocalPath(s);
                if (file.LocalPath.Path.EndsWith("..."))
                {
                    IList<P4.FileSpec> folderpath = 
                        SccService.ScmProvider.Connection.Repository.Connection.Client.GetClientFileMappings(file);
                    files.Add(folderpath[0]);
                }
                else
                {
                    files.Add(file);
                }
            }

            GetRevisionDlg.Show(files, "explorer", string.Empty,
                SccService.ScmProvider, SccService.IsSolutionSelected);

            // now refresh the selected nodes' glyphs
            Glyphs.RefreshNodesGlyphs(selectedNodes, selected);
        }

        public void P4VsSyncHead(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
            IList<string> files = null;
            if (TreatProjectsAsFolders)
            {
                files = SccService.SelectedFilesFolders;
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            for (int idx = 0; idx < files.Count; idx++)
            {
                string file = files[idx];

                if (file.EndsWith("\\..."))
                {
                    continue;
                }
                P4.FileMetaData fmd = null;

                if (string.IsNullOrEmpty(file) == false)
                {
                    fmd = SccService.ScmProvider.Fetch(file);
                }

                if ((fmd == null) || (fmd.HeadAction == P4.FileAction.Delete))
                {
                    files.RemoveAt(idx--);
                    continue;
                }
            }
            if (files.Count <= 0)
            {
                MessageBox.Show(Resources.P4ScmProvider_SelectedFileDeletedAtHeadRevisionWarning,
                    Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                return;
            }
            bool updateSolution = false;
            IVsSolution solution = null;
            string path = SccService.ScmProvider.SolutionFile;

            IList<IVsHierarchy> projectsUpdating = new List<IVsHierarchy>();
            IList<string> projectsClosed = new List<string>();
            IList<Guid> projectsGuidsClosed = new List<Guid>();

            IList<string> filesChanged = null;

            if (SccService.IsSolutionSelected)
            {

                updateSolution = SccService.ScmProvider.SyncFile(
                    new P4.SyncFilesCmdOptions(P4.SyncFilesCmdFlags.Preview, 1),
                    path);
            }
            if (updateSolution)
            {
                if (DialogResult.Cancel == MessageBox.Show(
                    Resources.P4VsProvider_GetLatestReplaceSlnWarning,
                    Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    return;
                }
                solution = (IVsSolution)GetService(typeof(SVsSolution));

                SuppressConnection = true;

                if (VSConstants.S_OK != solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, null, 0))
                {
                    MessageBox.Show(Resources.P4VsProvider_ErrorClosingSolution, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (SccService.SelectedHierarchies != null)
            {
                bool needToAsk = true;
                foreach (VSITEMSELECTION pItem in P4VsProvider.Instance.SccService.SelectedNodes)
                {
                    if ((pItem.itemid == VSConstants.VSITEMID_ROOT) && (P4VsProvider.Instance.SccService.IsProjectControlled(pItem.pHier)))
                    {
                        IVsProject3 pProj = pItem.pHier as IVsProject3;
                        string projFile;
                        if (pProj != null)
                        {
                            pProj.GetMkDocument(VSConstants.VSITEMID_ROOT, out projFile);

                            if (string.IsNullOrEmpty(projFile) == false)
                            {
                                bool updateProject = SccService.ScmProvider.SyncFile(
                                                        new P4.SyncFilesCmdOptions(P4.SyncFilesCmdFlags.Preview, 1),
                                                        projFile);
                                if (updateProject)
                                {
#if VS2008
								if (DialogResult.Cancel == MessageBox.Show(
									   Resources.P4VsProvider_GetLatestReplacProjWarning,
									   Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
								{
									return;
								}
								updateSolution = true;

								solution = (IVsSolution)GetService(typeof(SVsSolution));

								SuppressConnection = true;

								if (VSConstants.S_OK != solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, null, 0))
								{
									MessageBox.Show(Resources.P4VsProvider_ErrorClosingProject, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
									return;
								}
								break;
#else
                                    if (needToAsk == true)
                                    {
                                        if (DialogResult.Cancel == MessageBox.Show(
                                           Resources.P4VsProvider_GetLatestReplacProjWarning,
                                           Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                                        {
                                            return;
                                        }
                                        needToAsk = false;
                                    }
                                    projectsUpdating.Add(pItem.pHier);
                                    projectsClosed.Add(projFile);

                                    if (solution == null)
                                    {
                                        solution = (IVsSolution)GetService(typeof(SVsSolution));
                                    }
                                    Guid projGuid;
                                    solution.GetGuidOfProject(pItem.pHier, out projGuid);
                                    projectsGuidsClosed.Add(projGuid);

                                    if (VSConstants.S_OK != solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, pItem.pHier, 0))
                                    {
                                        MessageBox.Show(Resources.P4VsProvider_ErrorClosingProject, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
#endif
                                }
                            }
                        }
                    }
                }
            }
            try
            {
                bool success = SccService.SyncFiles(out filesChanged, null, files);
                if (!success || (filesChanged == null) || (filesChanged.Count <= 0))
                {
                    return;
                }
            }
            catch (P4.P4Exception ex)
            {
                P4ErrorDlg.Show(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (updateSolution)
                {
                    if (path != null)
                    {
                        int res = solution.OpenSolutionFile(0, path);

                        IVsUIShell shell = (IVsUIShell)GetService(typeof(IVsUIShell));
                        shell.ReportErrorInfo(res);

                        SccService.ResetSelection();
                    }
                }
#if !VS2008
                else if (projectsUpdating.Count > 0)
                {
                    foreach (Guid projGuid in projectsGuidsClosed)
                    {
                        Guid rProjGuid = projGuid;

                        var vsSolution4 = (IVsSolution4)solution;
                        if (vsSolution4 != null)
                        {
                            int res = vsSolution4.ReloadProject(ref rProjGuid);

                            IVsUIShell shell = (IVsUIShell)GetService(typeof(IVsUIShell));
                            shell.ReportErrorInfo(res);
                        }
                    }
                }
#endif
                Glyphs.RefreshFilesAndGlyphs(filesChanged);
            }
        }

        public void P4VsDiffVsHave(object sender, EventArgs e)
        {
            SaveAll();

            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;
            IList<string> files = SccService.SelectedFiles;

            if (SccService.IsSolutionSelected)
            {
                files = new List<string>();
                files.Add(GetSolutionFileName());
            }
            else if (SccService.IsaControlledProjectSelected)
            {
                files = new List<string>();
                foreach (VSITEMSELECTION node in selectedNodes)
                {
                    if (((node.itemid == VSConstants.VSITEMID_ROOT)) && (SccService.IsProjectControlled(node.pHier)))
                    {
                        IVsSccProject2 proj = node.pHier as IVsSccProject2;
                        if (proj != null)
                        {
                            files.Add(GetProjectFileName(proj));
                        }
                    }
                }
            }
            SccService.DiffFiles(files);
        }

        public void P4VsDiffVsAny(object sender, EventArgs e)
        {
            SaveAll();

            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;
            IList<string> files = SccService.SelectedFiles;

            if (SccService.IsSolutionSelected)
            {
                files = new List<string>();
                files.Add(GetSolutionFileName());
            }

            P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
            P4ScmProvider scm = P4VSService.ScmProvider;

            IList<P4.FileSpec> result = DiffDlg.Show(files, "explorer", "latest", scm);
        }

        public void P4VsShowHistory(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;
            IList<string> files = SccService.SelectedFiles;

            SccHistoryToolWindow window = (SccHistoryToolWindow)this.FindToolWindow(typeof(SccHistoryToolWindow), 0, true);

            window.control.Files = files;

            P4VsViewHistoryToolWindow(null, null);
        }

        public void P4VsShelve(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
            IList<string> files = null;
            if (TreatProjectsAsFolders)
            {
                files = SccService.SelectedFilesFolders;
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            if (files != null)
            {
                SccService.ShelveFiles(files);
            }

            Glyphs.RefreshNodesGlyphs(selectedNodes, files);
        }

        public void P4VsScmMerge(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;
            IList<string> files = SccService.SelectedFiles;

            SccService.Merge(files);

            // now refresh the selected nodes' glyphs
            Glyphs.RefreshNodesGlyphs(selectedNodes, files);
        }

        public void P4VsScmCopy(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;
            IList<string> files = SccService.SelectedFiles;

            SccService.Copy(files);

            // now refresh the selected nodes' glyphs
            Glyphs.RefreshNodesGlyphs(selectedNodes, files);

        }

        public void P4VsScmPublish(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);

            SccService.AddNewSolutionToSourceControl();

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;
            IList<string> files = SccService.SelectedFiles;

            // now refresh the selected nodes' glyphs
            Glyphs.RefreshNodesGlyphs(selectedNodes, files);

            // return to P4VsProvider to update the shell with
            // solution added status so that the status bar
            // will change
            SccService.SlnAddedToSourceControl();
        }

        // Menu commands to bring up different parts of P4V
        public void P4VsP4V(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            IList<string> sel = SccService.SelectedFiles;
            if (sel != null && sel.Count > 0)
            {
                SccService.ScmProvider.LaunchP4V(sel[0]);
            }
        }

        public void P4VsTimeLapse(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            IList<string> sel = SccService.SelectedFiles;
            if (sel != null && sel.Count > 0)
            {
                SccService.ScmProvider.LaunchTimeLapseView(sel[0]);
            }
        }

        public void P4VsRevGraph(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            IList<string> sel = SccService.SelectedFiles;
            if (sel != null && sel.Count > 0)
            {
                SccService.ScmProvider.LaunchRevisionGraphView(sel[0]);
            }
        }

        public void P4VsStreamGraph(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            SccService.ScmProvider.LaunchStreamGraph();
        }

        // The function can be used to bring back the provider's toolwindow if it was previously closed
        public void P4VsViewWorkspaceToolWindow(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            ToolWindowPane window = this.FindToolWindow(typeof(WorkspaceToolWindow), 0, true);
            IVsWindowFrame windowFrame = null;
            if (window != null && window.Frame != null)
            {
                windowFrame = (IVsWindowFrame)window.Frame;
            }
            if (windowFrame != null)
            {
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
            }
        }

        // The function can be used to bring back the provider's toolwindow if it was previously closed
        public void P4VsViewHistoryToolWindow(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            SccHistoryToolWindow window = (SccHistoryToolWindow)this.FindToolWindow(typeof(SccHistoryToolWindow), 0, true);
            IVsWindowFrame windowFrame = null;
            if (window != null && window.Frame != null)
            {
                windowFrame = (IVsWindowFrame)window.Frame;
            }
            if (windowFrame != null)
            {
                ErrorHandler.ThrowOnFailure(windowFrame.Show());

                IList<string> files = P4VsProvider.Instance.GetSelectedFilesInControlledProjects();
               
                if ((files != null) && (files.Count > 0))
                {
                    window.control.Files = files;
                }
            }
        }

        public void P4VsViewHistoryToolWindowExt(IList<string> sentFiles = null)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            SccHistoryToolWindow window = (SccHistoryToolWindow)this.FindToolWindow(typeof(SccHistoryToolWindow), 0, true);
            IVsWindowFrame windowFrame = null;
            if (window != null && window.Frame != null)
            {
                windowFrame = (IVsWindowFrame)window.Frame;
            }
            if (windowFrame != null)
            {
                ErrorHandler.ThrowOnFailure(windowFrame.Show());

                IList<string> files = P4VsProvider.Instance.GetSelectedFilesInControlledProjects();
                if (sentFiles != null)
                { files = sentFiles; }

                if ((files != null) && (files.Count > 0))
                {
                    window.control.Files = files;
                }
            }
        }

        // The function can be used to bring back the provider's toolwindow if it was previously closed
        public void P4VsViewJobsToolWindow(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            JobsToolWindow window = (JobsToolWindow)this.FindToolWindow(typeof(JobsToolWindow), 0, true);
            IVsWindowFrame windowFrame = null;
            if (window != null && window.Frame != null)
            {
                windowFrame = (IVsWindowFrame)window.Frame;
            }
            if (windowFrame != null)
            {
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
            }
        }

        // The function can be used to bring back the provider's toolwindow if it was previously closed
        public void P4VsViewSubmittedChangelistsToolWindow(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            SubmittedChangelistsToolWindow window = (SubmittedChangelistsToolWindow)this.FindToolWindow(typeof(SubmittedChangelistsToolWindow), 0, true);
            IVsWindowFrame windowFrame = null;
            if (window != null && window.Frame != null)
            {
                windowFrame = (IVsWindowFrame)window.Frame;
            }
            if (windowFrame != null)
            {
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
                if (window._scm == null)
                {
                    window._scm = SccService.ScmProvider;
                    window.initData();
                }
            }
        }

        // The function can be used to bring back the provider's toolwindow if it was previously closed
        public void P4VsViewPendingChangelistsToolWindow(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            PendingChangelistsToolWindow window = (PendingChangelistsToolWindow)this.FindToolWindow(typeof(PendingChangelistsToolWindow), 0, true);
            IVsWindowFrame windowFrame = null;

            // the status bar button click sends (null,null)
            // set the user and workspace to the current connection
            // that is what the changes count will be based off of.
            if(sender==null)
            {
                window.control.userCB.Text = window._scm.Connection.User;
                window.control.workspaceCB.Text = window._scm.Connection.Workspace;
                window.control.pathCB.Text = "";
                window.control.filterBtn_Click(null, null);
            }

            if (window != null && window.Frame != null)
            {
                windowFrame = (IVsWindowFrame)window.Frame;
            }
            if (windowFrame != null)
            {
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
                if (window._scm == null)
                {
                    window._scm = SccService.ScmProvider;
                    window.initData();
                }
            }
        }

        // The function can be used to bring back the provider's toolwindow if it was previously closed
        public void P4VsViewLabelsToolWindow(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            LabelsToolWindow window = (LabelsToolWindow)this.FindToolWindow(typeof(LabelsToolWindow), 0, true);
            IVsWindowFrame windowFrame = null;
            if (window != null && window.Frame != null)
            {
                windowFrame = (IVsWindowFrame)window.Frame;
            }
            if (windowFrame != null)
            {
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
            }
        }

        // The function can be used to bring back the provider's toolwindow if it was previously closed
        public void P4VsViewSwarmToolWindow(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            SwarmToolWindow window = (SwarmToolWindow)this.FindToolWindow(typeof(SwarmToolWindow), 0, true);
            IVsWindowFrame windowFrame = null;
            if (window != null && window.Frame != null)
            {
                windowFrame = (IVsWindowFrame)window.Frame;
            }
            if (windowFrame != null)
            {
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
            }
        }

        // The function can be used to bring back the provider's toolwindow if it was previously closed
        public void P4VsViewReviewsToolWindow(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            SwarmReviewsToolWindow window = (SwarmReviewsToolWindow)this.FindToolWindow(typeof(SwarmReviewsToolWindow), 0, true);
            IVsWindowFrame windowFrame = null;
            if (window != null && window.Frame != null)
            {
                windowFrame = (IVsWindowFrame)window.Frame;
            }
            if (windowFrame != null)
            {
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
            }
        }

        // The function can be used to bring back the provider's toolwindow if it was previously closed
        public void P4VsViewStreamsToolWindow(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            StreamsToolWindow window = (StreamsToolWindow)this.FindToolWindow(typeof(StreamsToolWindow), 0, true);
            IVsWindowFrame windowFrame = null;
            if (window != null && window.Frame != null)
            {
                windowFrame = (IVsWindowFrame)window.Frame;
            }
            if (windowFrame != null)
            {
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
            }
        }

        // The function can be used to bring back the provider's toolwindow if it was previously closed
        public void P4VsP4VSHelp(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion; // 2011.1.0.0 is the format
            version = version.Remove(0, 2);
            string[] versionSplit = version.Split('.');
            string relDir = "r" + versionSplit[0] + "." + versionSplit[1];
            try
            {
                bool pageExists = false;
                string helpPath = @"https://www.perforce.com/perforce/doc.current/manuals/p4vs/index.html";
                string versionPath = helpPath.Replace("doc.current", relDir);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(versionPath);
                request.Method = WebRequestMethods.Http.Head;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    pageExists = response.StatusCode == HttpStatusCode.OK;
                }
                catch
                {
                    // likely got a 404 here, page does not exist
                    // this will happen if the html help has not been
                    // pushed to web for the assembly's version. For
                    // example, builds from main.
                }
                if (pageExists)
                {
                    Help.ShowHelp(null, versionPath, null);
                }
                else
                {
                    Help.ShowHelp(null, helpPath, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // The function can be used to bring back the provider's toolwindow if it was previously closed
        public void P4VsP4VSAbout(object sender, EventArgs e)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string assemblyPath = assembly.CodeBase;
                string resourceName = "LICENSE.txt";

                string licensePath = Path.Combine(Path.GetDirectoryName(assemblyPath), resourceName);
                if (licensePath.StartsWith("file:\\"))
                {
                    licensePath = licensePath.Substring(6);
                }

                string license = System.IO.File.ReadAllText(licensePath);

                LicenseDlg dlg = new LicenseDlg();
                dlg.licenseTB.Text = license;
                dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public Dictionary<string, string> knownInfoKeys()
        {
            Dictionary<string, string> keysToLabels =
            new Dictionary<string, string>();
            keysToLabels.Add("userName", "User name: ");
            keysToLabels.Add("clientName", "Workspace name: ");
            keysToLabels.Add("clientHost", "Workspace host: ");
            keysToLabels.Add("clientRoot", "Workspace root: ");
            keysToLabels.Add("clientCwd", "Current directory: ");
            keysToLabels.Add("clientAddress", "Workspace address: ");
            keysToLabels.Add("unicode", "Unicode: ");
            keysToLabels.Add("charset", "Charset: ");
            keysToLabels.Add("security", "Security: ");
            keysToLabels.Add("serverName", "Server Name: ");
            keysToLabels.Add("serverDescription", "Server description: ");
            keysToLabels.Add("serverAddress", "Server address: ");
            keysToLabels.Add("serverRoot", "Server root: ");
            keysToLabels.Add("serverDate", "Server date: ");
            keysToLabels.Add("serverUptime", "Server uptime: ");
            keysToLabels.Add("serverVersion", "Server version: ");
            keysToLabels.Add("authServer", "Authorization server: ");
            keysToLabels.Add("changeServer", "Changelist server: ");
            keysToLabels.Add("serverLicense", "Server license: ");
            keysToLabels.Add("serverLicense-ip", "Server license IP: ");
            keysToLabels.Add("password", "Password: ");
            keysToLabels.Add("monitor", "Monitor: ");
            keysToLabels.Add("proxyVersion", "Proxy Version: ");
            keysToLabels.Add("brokerAddress", "Broker Address: ");
            keysToLabels.Add("brokerVersion", "Broker Version: ");
            keysToLabels.Add("caseHandling", "Server case handling: ");
            keysToLabels.Add("minClient", "Minimum Client Level: ");
            keysToLabels.Add("minClientMessage", "Message text for clients that are too old: ");
            return keysToLabels;
        }
        public string p4Info()
        {
            string smd = null;
            if (SccService != null && SccService.ScmProvider != null)
            {
                P4Command cmd = new P4Command(SccService.ScmProvider.Connection.Repository.Connection,
                    "info", true, null);
                P4CommandResult results = cmd.Run();
                if ((results.Success) && (results.TaggedOutput != null))
                {
                    TaggedObject to = results.TaggedOutput[0];
                    string val;
                    string label;
                    foreach (string key in knownInfoKeys().Keys)
                    {
                        to.TryGetValue(key, out val);
                        if (!string.IsNullOrEmpty(val))
                        {
                            knownInfoKeys().TryGetValue(key, out label);
                            smd += label + val + "\r";
                            to.Remove(key);
                        }
                    }
                    foreach (string key in to.Keys)
                    {
                        to.TryGetValue(key, out val);
                        smd += key + ": " + val + "\r";
                    }
                }
            }
            return smd;
        }

        public string p4VSprefs()
        {
            string prefs = "";
            object setting = "";

            // Connection options
            Preferences.LocalSettings.TryGetValue("ConnectPreference", out setting);
            switch (setting)
            {
                case (int)ConnectionPreference.UseEnvironment:
                    setting = ConnectionPreference.UseEnvironment.ToString();
                    break;
                case (int)ConnectionPreference.ShowDialog:
                    setting = ConnectionPreference.ShowDialog.ToString();
                    break;
                case (int)ConnectionPreference.UseRecent:
                    setting = ConnectionPreference.UseRecent.ToString();
                    break;
                default:
                case (int)ConnectionPreference.UseSolution:
                    setting = ConnectionPreference.UseSolution.ToString();
                    break;
            }
            prefs += "ConnectPreference: " + setting + "\n";

            prefs += "Use_IP: " + Preferences.LocalSettings.GetBool("Use_IP", false).ToString() + "\n";
            prefs += "Auto_logoff: " + Preferences.LocalSettings.GetBool("Auto_logoff", false).ToString() + "\n";

            // Data retrieval options

            prefs += "Update_status: " + Preferences.LocalSettings.GetInt("Update_status", 5).ToString() + "\n";
            prefs += "Number_files: " + Preferences.LocalSettings.GetInt("Number_files", 1000).ToString() + "\n";
            prefs += "Size_files: " + Preferences.LocalSettings.GetInt("Size_files", 500).ToString() + "\n";
            prefs += "Number_specs: " + Preferences.LocalSettings.GetInt("Number_specs", 100).ToString() + "\n";
            prefs += "AutoUpdateFileData: " + Preferences.LocalSettings.GetBool("AutoUpdateFileData", false).ToString() + "\n";

            prefs += "TreatProjectsAsFolders: " + Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", false).ToString() + "\n";
            prefs += "PreloadScmCache: " + Preferences.LocalSettings.GetBool("PreloadScmCache", true).ToString() + "\n";
            prefs += "LazyLoadStatus: " + Preferences.LocalSettings.GetBool("LazyLoadStatus", false).ToString() + "\n";
            prefs += "LazyLoadFullMenu: " + Preferences.LocalSettings.GetBool("LazyLoadFullMenu", false).ToString() + "\n";

            prefs += "DisableParallelSync: " + Preferences.LocalSettings.GetBool("DisableParallelSync", false).ToString() + "\n";
            prefs += "DisableParallelSubmit: " + Preferences.LocalSettings.GetBool("DisableParallelSubmit", false).ToString() + "\n";
            prefs += "DisableParallelShelve: " + Preferences.LocalSettings.GetBool("DisableParallelShelve", false).ToString() + "\n";

            // Data retrieval options
            setting = "";
            Preferences.LocalSettings.TryGetValue("P4Diff_path", out setting);
            prefs += "Diff application: " + setting + "\n";
            setting = "";
            Preferences.LocalSettings.TryGetValue("Diff_args", out setting);
            prefs += "Diff args: " + setting + "\n";
            setting = "";
            Preferences.LocalSettings.TryGetValue("P4Merge_path", out setting);
            prefs += "Merge application: " + setting + "\n";
            setting = "";
            Preferences.LocalSettings.TryGetValue("Merge_args", out setting);
            prefs += "Merge args: " + setting + "\n";

            prefs += "Launch_Swarm_Browser: " + Preferences.LocalSettings.GetBool("Launch_Swarm_Browser", false).ToString() + "\n";

            // General options
            prefs += "P4Date_format: " + Preferences.LocalSettings.GetBool("P4Date_format", true).ToString() + "\n";

            prefs += "Revert_warn: " + Preferences.LocalSettings.GetBool("Revert_warn", true).ToString() + "\n";
            prefs += "Checkout_lock: " + Preferences.LocalSettings.GetBool("Checkout_lock", false).ToString() + "\n";
            prefs += "PromptForChanglist: " + Preferences.LocalSettings.GetBool("PromptForChanglist", true).ToString() + "\n";
            prefs += "CheckoutWriteable: " + Preferences.LocalSettings.GetBool("CheckoutWriteable", false).ToString() + "\n";
            prefs += "PromptForDelete: " + Preferences.LocalSettings.GetBool("PromptForDelete", true).ToString() + "\n";
            prefs += "Auto_Add: " + Preferences.LocalSettings.GetBool("Auto_Add", true).ToString() + "\n";
            prefs += "Update_Project: " + Preferences.LocalSettings.GetBool("Update_Project", true).ToString() + "\n";
            prefs += "OpenShelvedFileInEditor: " + Preferences.LocalSettings.GetBool("OpenShelvedFileInEditor", true).ToString() + "\n";

            prefs += "QueryEditNeverSync: " + Preferences.LocalSettings.GetBool("QueryEditNeverSync", false).ToString() + "\n";
            prefs += "QueryEditAlwaysSync: " + Preferences.LocalSettings.GetBool("QueryEditAlwaysSync", false).ToString() + "\n";

            prefs += "TagSolutionProjectFiles: " + Preferences.LocalSettings.GetBool("TagSolutionProjectFiles", false).ToString() + "\n";
            prefs += "SetProjectFileLocation: " + Preferences.LocalSettings.GetBool("SetProjectFileLocation", true).ToString() + "\n";
            prefs += "WarnSlnWorkspace: " + Preferences.LocalSettings.GetBool("WarnSlnWorkspace", true).ToString() + "\n";
            prefs += "PromptSlnSync: " + Preferences.LocalSettings.GetBool("PromptSlnSync", true).ToString() + "\n";

            // Ignore options
            bool ignoreEnabled = P4ScmProvider.P4Ignore != null;
            prefs += "IgnoreEnabled: " + ignoreEnabled.ToString() + "\n";
            prefs += "PreferedIgnoreFileName: " + Preferences.LocalSettings.GetString("PreferedIgnoreFileName", ".p4ignore.txt").ToString() + "\n";

            prefs += "AddIgnoreFileToSolutuion: " + Preferences.LocalSettings.GetBool("AddIgnoreFileToSolutuion", true).ToString() + "\n";
            prefs += "PromptOnNewIgnoreFile: " + Preferences.LocalSettings.GetBool("PromptOnNewIgnoreFile", true).ToString() + "\n";
            prefs += "IgnoreIgnoredFiles: " + Preferences.LocalSettings.GetBool("IgnoreIgnoredFiles", true).ToString() + "\n";
            prefs += "MarkIgnoreFileForAdd: " + Preferences.LocalSettings.GetBool("MarkIgnoreFileForAdd", false).ToString() + "\n";

            // Logging options
            prefs += "Log_reporting: " + Preferences.LocalSettings.GetBool("Log_reporting", false).ToString() + "\n";
            prefs += "Log_command: " + Preferences.LocalSettings.GetBool("Log_command", false).ToString() + "\n";
            prefs += "Log_save: " + Preferences.LocalSettings.GetBool("Log_save", false).ToString() + "\n";
            prefs += "Log_path: " + Preferences.LocalSettings.GetString("Log_path", null) + "\n";
            prefs += "Log_size: " + Preferences.LocalSettings.GetInt("Log_size", 50).ToString() + "\n";
            prefs += "\n";

            return prefs;
        }
        public void P4VsP4VSSystemInfo(object sender, EventArgs e)
        {
            SystemInfoDlg sysInfo = new SystemInfoDlg();
            // Show p4info (if connected), Application info,
            // Swarm Configuration (if configured), Computer info,
            // and P4VS preference settings.

            string smd = p4Info();

            System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph();

            // p4info and Swarm configuration
            if (smd!=null)
            {
                paragraph.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run("Connection: " + "\r")));
                paragraph.Inlines.Add(new System.Windows.Documents.Run(smd + "\n"));

                if (SccService.ScmProvider.Connection.Swarm.SwarmEnabled != false)
                {
                    paragraph.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run("Swarm Configuration:" + "\r")));
                    paragraph.Inlines.Add(new System.Windows.Documents.Run(SccService.ScmProvider.Connection.Swarm.SwarmVersion.version + "\n"));
                }
                paragraph.Inlines.Add(new System.Windows.Documents.Run("\n"));
            }
            else
            {
                paragraph.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run("No Connection" + "\n" + "\n")));
            }

            // Application and preferences
            paragraph.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run("Application:" + "\r")));

            // Get an instance of the currently running Visual Studio IDE
            // for VS edition and version info
            try
            {
                DTE2 dte2;
                dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
                string vSApp = dte2.FileName;

                if (System.IO.File.Exists(vSApp))
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(vSApp);
                    paragraph.Inlines.Add(new System.Windows.Documents.Run("Visual Studio " + dte2.Edition + " " + fileVersionInfo.ProductVersion + "\r"));
                }
            }
            catch (Exception)
            {
                // if there is any kind of error getting DTE or information from
                // it, just catch it and move on
            }
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.ProductVersion;
            paragraph.Inlines.Add(new System.Windows.Documents.Run("P4VS " + version + "\r"));

            string prefs = p4VSprefs();
            paragraph.Inlines.Add(new System.Windows.Documents.Run(prefs));

            // Computer 
            paragraph.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run("Computer:" + "\r")));

            string subKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion";
            Microsoft.Win32.RegistryKey rkey = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey skey = rkey.OpenSubKey(subKey);
            string name = skey.GetValue("ProductName").ToString();
            string bit = "";
            bit = Environment.Is64BitOperatingSystem ? "64 - bit" : "32 - bit";
            paragraph.Inlines.Add(new System.Windows.Documents.Run("OS name: " + name + ", " + bit + "\n"));
            paragraph.Inlines.Add(new System.Windows.Documents.Run("Version: " + Environment.OSVersion.Version + "\n"));
            paragraph.Inlines.Add(new System.Windows.Documents.Run("System name: " + SystemInformation.ComputerName.ToString() + "\n"));

            for (int i=0;i< Environment.ProcessorCount; i++)
            {
                Microsoft.Win32.RegistryKey processor_name =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0",
                    Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree);   //This registry entry contains entry for processor info.
                if (processor_name != null)
                {
                    if (processor_name.GetValue("ProcessorNameString") != null)
                    {
                        paragraph.Inlines.Add(new System.Windows.Documents.Run("Processor: " + processor_name.GetValue("ProcessorNameString") + "\n"));
                    }
                }
            }


            Microsoft.VisualBasic.Devices.ComputerInfo ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
            paragraph.Inlines.Add(new System.Windows.Documents.Run("Total physical memory: " + string.Concat(Math.Round(Convert.ToDouble(ci.TotalPhysicalMemory) / Math.Pow(1024, 3), 2), " GB") + "\n"));
            paragraph.Inlines.Add(new System.Windows.Documents.Run("Available physical memory: " + string.Concat(Math.Round(Convert.ToDouble(ci.AvailablePhysicalMemory) / Math.Pow(1024, 3), 2), " GB") + "\n"));
            paragraph.Inlines.Add(new System.Windows.Documents.Run("Total virtual memory: " + string.Concat(Math.Round(Convert.ToDouble(ci.TotalVirtualMemory) / Math.Pow(1024, 3), 2), " GB") + "\n"));
            paragraph.Inlines.Add(new System.Windows.Documents.Run("Available virtual memory: " + string.Concat(Math.Round(Convert.ToDouble(ci.AvailableVirtualMemory) / Math.Pow(1024, 3), 2), " GB")));

            sysInfo.systemInfoTB.Document = new System.Windows.Documents.FlowDocument(paragraph);
            sysInfo.ShowDialog();

            return;
        }
        public void P4VsCheckin(object sender, EventArgs e)
        {
            SaveAll();

            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }
            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> nodes = SccService.SelectedNodes;
            IList<string> sel = SccService.SelectedFiles;

            if (SccService.CheckinFiles(sel))
            {
                // something was submitted, now refresh the selected nodes' glyphs
                Glyphs.RefreshNodesGlyphs(nodes, sel);
            }
        }

        public void P4VsCheckout(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }
            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            IList<VSITEMSELECTION> nodes = SccService.SelectedNodes;
            IList<string> sel = new List<string>();

            if (SccService.IsSolutionSelected)
            {
                sel.Add(GetSolutionFileName());
            }
            else if (SccService.IsaControlledProjectSelected)
            {
                foreach (VSITEMSELECTION node in nodes)
                {
                    if (((node.itemid == VSConstants.VSITEMID_ROOT)) && (SccService.IsProjectControlled(node.pHier)))
                    {
                        IVsSccProject2 proj = node.pHier as IVsSccProject2;
                        if (proj != null)
                        {
                            sel.Add(GetProjectFileName(proj));
                        }
                    }
                }
            }
            else
            {
                sel = SccService.SelectedFiles;
            }
            if (sel != null && sel.Count > 0)
            {
                P4VsScmRefresh(sender, e);
                CheckoutResource(sel);
            }
        }

        public void P4VsCheckoutEntireProjectOrSolution(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", false);
            IList<string> files = null;
            if (TreatProjectsAsFolders)
            {
                files = SccService.SelectedFilesFolders;
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            if (files != null && files.Count > 0)
            {
                CheckoutResource(files);
            }
            Glyphs.RefreshFilesAndGlyphs(SccService.SelectedFiles);
        }

        public void P4VsScmRefresh(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                Debug.Assert(false, "The command should have been disabled");
                return;
            }

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            if (SccService.IsSolutionSelected)
            {
                Glyphs.RefreshNodesGlyphs(null, null);
            }
            else
            {
                bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", false);
                if (TreatProjectsAsFolders && (SccService.IsSolutionSelected || SccService.IsaControlledProjectSelected))
                {
                    IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;
                    IList<string> files = SccService.SelectedFilesFolders;

                    // now refresh the selected nodes' glyphs
                    Glyphs.RefreshNodesGlyphs(selectedNodes, files);
                }
                else
                {
                    IList<string> files = null;
                    files = SccService.SelectedFiles;
                    Glyphs.RefreshFilesAndGlyphs(files);
                }
            }
            // make sure the context menus update for the new state
            ResetCommandStatus();
        }

        public void P4VsAddToSourceControl(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                Debug.Assert(false, "The command should have been disabled");
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            string solutionFile = GetSolutionFileName();

            IList<VSITEMSELECTION> sel = GetSelectedNodes();

            bool isSolutionSelected = false;
            Hashtable hash = GetSelectedHierarchies(ref sel, out isSolutionSelected);

            Hashtable hashUncontrolledProjects = new Hashtable();
            if (isSolutionSelected)
            {
                SolutionHasDirtyProps = true;

                if (SccService.IsProjectControlled(null) == false)
                {
                    SccService.RegisterSccProject(null, solutionFile, "", "", ProviderName);
                }
                if (string.IsNullOrEmpty(SccService.ScmProvider.SolutionFile))
                {
                    SccService.ScmProvider.SolutionFile = solutionFile;
                }
                // When the solution is selected, all the uncontrolled projects in the solution will be added to scc
                hash = GetLoadedControllableProjectsEnum();
            }

            foreach (IVsHierarchy pHier in hash.Keys)
            {
                if (!SccService.IsProjectControlled(pHier))
                {
                    hashUncontrolledProjects[pHier] = true;
                }
            }
            bool forceSeletionChange = false;
            if (hashUncontrolledProjects.Count > 0)
            {
                forceSeletionChange = true;
                SccService.AddProjectsToSourceControl(ref hashUncontrolledProjects, isSolutionSelected);
            }
            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            IList<string> files = SccService.SelectedFiles;

            // if the solution is one of the selected nodes, make sure it's in the list
            if ((isSolutionSelected) && (solutionFile != null))
            {
                bool hasSolutionfile = false;
                if (files != null)
                {
                    foreach (string file in files)
                    {
                        if (file.ToLower() == solutionFile.ToLower())
                        {
                            hasSolutionfile = true;
                            break;
                        }
                    }
                }
                if (hasSolutionfile == false)
                {
                    if (files != null) files.Add(SccService.ScmProvider.SolutionFile);
                }
            }
            int changeListId = 0;

            string newChangeDescription = Resources.P4VsProvider_AddFilesDefaultChangelistDescription;
            string prompt = Resources.P4VsProvider_AddFilesPrompt;

            if (files.Count == 1)
            {
                newChangeDescription = Resources.P4VsProvider_AddFileDefaultChangelistDescription;
                prompt = Resources.P4VsProvider_AddFilePrompt;
            }

            IList<P4.Changelist> changes = SccService.ScmProvider.GetAvailibleChangelists(-1);
            changeListId = SelectChangelistDlg.ShowChooseChangelist(prompt, changes, ref newChangeDescription);

            if (changeListId <= -2)
            {
                // user hit 'No'
                return;
            }

            bool notInWorkspace = true;
            //bool notUnderClientRoot = false;
            while (notInWorkspace)
            {
                try
                {
                    changeListId = SccService.AddFilesToSourceControl(files, changeListId, newChangeDescription);
                    P4.P4CommandResult results = SccService.ScmProvider.Connection.Repository.Connection.LastResults;
                    if ((results != null) && (results.ErrorList != null) && (results.ErrorList.Count > 0) &&
                        ((results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
                        (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient) ||
                        (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_IntegMovedUnmapped) ||
                        (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_ExVIEW) ||
                        (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_ExVIEW2)))
                    {
                        if ((results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
                                (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient))
                        {
                            MessageBox.Show(Resources.P4VsProvider_CantAddSlnClientRootError,
                                Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        notInWorkspace = false;
                        P4ErrorDlg.Show(results, false);
                    }
                }
                catch (P4.P4Exception ex)
                {
                    if ((ex.ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
                            (ex.ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient))
                    {
                        MessageBox.Show(Resources.P4VsProvider_CantAddSlnClientRootError,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!((ex.ErrorCode == P4.P4ClientError.MsgDm_IntegMovedUnmapped) ||
                        (ex.ErrorCode == P4.P4ClientError.MsgDm_ExVIEW) ||
                        (ex.ErrorCode == P4.P4ClientError.MsgDm_ExVIEW2)))
                    {
                        notInWorkspace = false;
                        MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK);
                    }
                }
                if (notInWorkspace)
                {
                    if(Preferences.LocalSettings.GetBool("WarnSlnWorkspace", true))
                    {
                        if (DialogResult.No == MessageBox.Show(Resources.P4VsProvider_AddSlnWantUpdateClientMapQuestion,
                        Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            return;
                        }
                        P4.Client client = SccService.ScmProvider.getClient(SccService.ScmProvider.Connection.Repository.Connection.Client.Name, null);
                        if (DlgEditWorkspace.EditWorkspace(SccService.ScmProvider, client) == null)
                        {
                            return;
                        }
                    }
                }
            }
            SelectChangelistDlg.CurrentChangeList = changeListId;

            // fake a selected item change to re-init the list of selected files
            if (forceSeletionChange)
            {
#if !VS2012
                if (P4VsProviderService.SolutionExplorer == null)
                {
                    P4VsProviderService.SolutionExplorer = new P4SolutionExplorer();
                    P4VsProviderService.SolutionExplorer.Capture();
                }
#endif
                SccService.GetSelection();
            }
            // now refresh the selected nodes' glyphs
            Glyphs.RefreshNodesGlyphs(selectedNodes, files);
        }

        public void P4VsReconcileFiles(object sender, EventArgs e)
        {
            IVsSolution sol = (IVsSolution)GetService(typeof(SVsSolution));
            sol.GetSolutionInfo(out string slnDir, out string slnFile,
                    out string userOptsFile);
            List<P4.FileSpec> list = new List<P4.FileSpec>();
            P4.FileSpec fs = new P4.FileSpec();
            fs.LocalPath = new P4.LocalPath(slnDir + @"...");
            list.Add(fs);

            IList<P4.FileSpec> delList = new List<P4.FileSpec>();
            IList<P4.FileSpec> addList = new List<P4.FileSpec>();
            IList<P4.FileSpec> editList = new List<P4.FileSpec>();
            IList<P4.FileSpec> recList = new List<P4.FileSpec>();

            // this will get files under the selection
            // not using for now, maybe add as a preference
            //IList<string> slnFiles = P4VsProvider.Instance.GetSelectedFilesInControlledProjects();

            // this will get files under the sln, no matter what
            // the selection is, but the solution file itself is
            // not included, so add it at the end.
            IList<string> slnFiles = P4VsProvider.Instance.GetSolutionFiles();
            slnFiles.Add(slnFile);

            P4.Options sFlags = new P4.Options(P4.ReconcileFilesCmdFlags.DeletedLocally, -1);
            delList = SccService.ScmProvider.ReconcileStatus(list, sFlags);

            // for adds, check for files that are part of the solution
            // and considered to be source controlled items by
            // Visual Studio (otherwise the reconcile will return .dll's,
            // .cache files and other items not to check in.
            sFlags = new P4.Options(P4.ReconcileFilesCmdFlags.NotControlled |
                P4.ReconcileFilesCmdFlags.ModTimeCheck, -1);
            recList = SccService.ScmProvider.ReconcileStatus(list, sFlags);
            if (recList != null)
            {
                foreach (P4.FileSpec f in recList)
                {
                    if (slnFiles.Contains(f.LocalPath.ToString()))
                    {
                        addList.Add(f);
                    }
                }
            }

            sFlags = new P4.Options(P4.ReconcileFilesCmdFlags.ModifiedOutside, -1);
            editList = SccService.ScmProvider.ReconcileStatus(list, sFlags);

            // don't even bother fetching available changelists
            // if there are no files to reconcile
            // TODO change this to a p4vs dlg
            if (addList == null || addList.Count == 0 &&
                delList == null &&
                editList == null)
            {
                MessageBox.Show("no files to reconcile");
                return;
            }

            IList<P4.Changelist> changelists =
                SccService.ScmProvider.GetChangelists(P4.ChangesCmdFlags.None,
                SccService.ScmProvider.Connection.Workspace, 0, P4.ChangeListStatus.Pending,
                SccService.ScmProvider.Connection.User, null);

            ReconcileDlg dlg = new ReconcileDlg(addList, delList, editList, changelists,
                SccService);
            dlg.ShowDialog();
        }

        public void P4VsRevert(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (!IsThereASolution())
            {
                return;
            }

            CheckLazyLoadStatus();

            // processing menu commands so obviously no longer loading solution
            SccService.ScmProvider.LoadingSolution = false;

            // Before reverting files, make sure all in-memory edits have been committed to disk 
            // by forcing a save of the solution. Ideally, only the files to be checked in should be saved...
            // If the files are not saved, it can result in a situation where in memory changes are 
            // not backed out by the revert.
            IVsSolution sol = (IVsSolution)GetService(typeof(SVsSolution));
            if (sol.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty, null, 0) != VSConstants.S_OK)
            {
                // If saving the files failed, don't continue with the revert
                return;
            }

            IList<VSITEMSELECTION> selectedNodes = SccService.SelectedNodes;

            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
            IList<string> files = null;
            if (TreatProjectsAsFolders)
            {
                files = SccService.SelectedFilesFolders;
            }
            else
            {
                files = SccService.SelectedFiles;
            }

            bool updateSolution = false;
            IVsSolution solution = null;
            string solutionPath = SccService.ScmProvider.SolutionFile;
            P4.P4CommandResult results = null;

            IList<IVsHierarchy> projectsUpdating = new List<IVsHierarchy>();
            IList<string> projectsClosed = new List<string>();
            IList<Guid> projectsGuidsClosed = new List<Guid>();

            IList<string> revertedFiles = null;

            if (SccService.IsSolutionSelected)
            {
                try
                {
                    SccService.ScmProvider.Connection.Repository.Connection.Client.RevertFiles(
                        new P4.RevertCmdOptions(P4.RevertFilesCmdFlags.Preview, -1),
                        P4.FileSpec.LocalSpec(solutionPath));

                    results = SccService.ScmProvider.Connection.Repository.Connection.LastResults;

                    if (results.Success == false)
                    {
                        P4ErrorDlg.Show(results, false);
                        return;
                    }
                    updateSolution = results.TaggedOutput != null &&
                                        results.TaggedOutput.Count > 0;
                }
                catch (P4.P4Exception ex)
                {
                    P4ErrorDlg.Show(ex);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (updateSolution)
            {
                if (DialogResult.Cancel == MessageBox.Show(
                    Resources.P4VsProvider_RevertReplaceSlnWarning,
                    Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    return;
                }
                solution = (IVsSolution)GetService(typeof(SVsSolution));

                SuppressConnection = true;

                if (VSConstants.S_OK != solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, null, 0))
                {
                    MessageBox.Show(Resources.P4VsProvider_ErrorClosingSolution, Resources.P4VS,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (SccService.SelectedHierarchies != null)
            {
                bool needToAsk = true;
                foreach (VSITEMSELECTION pItem in SccService.SelectedNodes)
                {
                    if ((pItem.itemid == VSConstants.VSITEMID_ROOT) && (SccService.IsProjectControlled(pItem.pHier)))
                    {
                        IVsProject3 pProj = pItem.pHier as IVsProject3;
                        string projFile;
                        if (pProj != null)
                        {
                            pProj.GetMkDocument(VSConstants.VSITEMID_ROOT, out projFile);
                            if (projFile != null)
                            {
                                SccService.ScmProvider.Connection.Repository.Connection.Client.RevertFiles(
                                        new P4.RevertCmdOptions(P4.RevertFilesCmdFlags.Preview, -1),
                                        P4.FileSpec.LocalSpec(projFile));


                                results = SccService.ScmProvider.Connection.Repository.Connection.LastResults;

                                if (results.Success == false)
                                {
                                    P4ErrorDlg.Show(results, false);
                                    return;
                                }
                                bool updateProject = results.TaggedOutput != null &&
                                                            results.TaggedOutput.Count > 0;
                                if (updateProject)
                                {
#if VS2008
								if (DialogResult.Cancel == MessageBox.Show(
										Resources.P4VsProvider_RevertReplacProjWarning,
										Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
								{
									return;
								}
								updateSolution = true;

								solution = (IVsSolution)GetService(typeof(SVsSolution));

								SuppressConnection = true;

								if (VSConstants.S_OK != solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, null, 0))
								{
										MessageBox.Show(Resources.P4VsProvider_ErrorClosingProject, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
									return;
								}
								break;
#else
                                    if (needToAsk == true)
                                    {
                                        if (DialogResult.Cancel == MessageBox.Show(
                                           Resources.P4VsProvider_RevertReplacProjWarning,
                                           Resources.P4VS, MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                                        {
                                            return;
                                        }
                                        needToAsk = false;
                                    }
                                    projectsUpdating.Add(pItem.pHier);
                                    projectsClosed.Add(projFile);

                                    if (solution == null)
                                    {
                                        solution = (IVsSolution)GetService(typeof(SVsSolution));
                                    }
                                    Guid projGuid;
                                    solution.GetGuidOfProject(pItem.pHier, out projGuid);
                                    projectsGuidsClosed.Add(projGuid);

                                    if (VSConstants.S_OK != solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, pItem.pHier, 0))
                                    {
                                        MessageBox.Show(Resources.P4VsProvider_ErrorClosingProject, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
#endif
                                }
                            }

                        }
                    }
                }
            }
            try
            {
                // revert the unchanged files without a preview
                P4.Options opts = new P4.Options();
                revertedFiles = SccService.ScmProvider.RevertFiles(true, false, opts, files.ToArray());
                Glyphs.RefreshFilesAndGlyphs(revertedFiles);

                opts["-n"] = null;

                // get a preview list of the files about to be reverted
                List<string> preview = SccService.ScmProvider.PreviewRevertFiles(opts, null, false, files.ToArray());

                opts = new P4.Options();
                if (preview != null && preview.Count > 0)
                {
                    RevertWarnDlg dlg = new RevertWarnDlg(preview);
                    if (Preferences.LocalSettings.GetBool("Revert_warn", true))
                    {
                        if (dlg.ShowDialog() != DialogResult.Cancel)
                        {
                            revertedFiles = SccService.ScmProvider.RevertFiles(false, false, opts, files.ToArray());
                            Glyphs.RefreshFilesAndGlyphs(revertedFiles);
                        }
                    }

                    else
                        if (files.Count > 0)
                        {
                            revertedFiles = SccService.ScmProvider.RevertFiles(false, false, opts, files.ToArray());
                            Glyphs.RefreshFilesAndGlyphs(revertedFiles);
                        }
                }
            }
            finally
            {
                if (updateSolution)
                {
                    if (solutionPath != null)
                    {
                        int res = solution.OpenSolutionFile(0, solutionPath);

                        IVsUIShell shell = (IVsUIShell)GetService(typeof(IVsUIShell));
                        shell.ReportErrorInfo(res);
                    }
                }
#if !VS2008
                else if (projectsUpdating.Count > 0)
                {
                    foreach (Guid projGuid in projectsGuidsClosed)
                    {
                        Guid rProjGuid = projGuid;

                        var vsSolution4 = (IVsSolution4)solution;
                        if (vsSolution4 != null)
                        {
                            int res = vsSolution4.ReloadProject(ref rProjGuid);
                            IVsUIShell shell = (IVsUIShell)GetService(typeof(IVsUIShell));
                            shell.ReportErrorInfo(res);
                        }
                    }
                }
#endif
            }
        }

        // Used to open a connection to a Perforce depot
        public void P4VsOpenConnection(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            OpenConnection(null, null, null);
        }

        // Used to open a connection to a Perforce depot
        public void P4VsCloseConnection(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            CloseConnection();
        }

        // Used to open a solution or project in the Perforce depot
        public void P4VsFileOpenInDepot(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            DepotPathDlg dlg = new DepotPathDlg(SccService, null, true);

            if (DialogResult.OK == dlg.ShowDialog())
            {
                SuppressConnection = true;
                LastConnectionInfo = dlg.ConnectionInfo;

                string depotPath = dlg.SelectedFile;

                bool inWorkspace = false;
                P4.FileMetaData fmd = null;
                try
                {
                    fmd = SccService.ScmProvider.GetFileMetaData(depotPath);

                    if (fmd == null)
                    {
                        MessageBox.Show(Resources.P4VsProvider_SlnDoesNotExistError,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    P4.P4CommandResult results = SccService.ScmProvider.Connection.Repository.Connection.LastResults;

                    if ((results.ErrorList != null) && (results.ErrorList.Count > 0))
                    {
                        if ((results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
                            (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient))
                        {
                            MessageBox.Show(Resources.P4VsProvider_CantOpenSlnClientRootError,
                                Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if ((fmd.LocalPath != null) || (fmd.ClientPath != null))
                    {
                        inWorkspace = true;
                    }

                    if (fmd.Action == P4.FileAction.Delete)
                    {
                        MessageBox.Show(Resources.P4VsProvider_CantOpenMarkedForDeleteSlnError,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error); return;
                    }
                }
                catch (P4.P4Exception ex)
                {
                    if ((ex.ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
                        (ex.ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient))
                    {
                        MessageBox.Show(Resources.P4VsProvider_CantOpenSlnClientRootError,
                            Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if ((ex.ErrorCode == P4.P4ClientError.MsgDm_IntegMovedUnmapped) ||
                        (ex.ErrorCode == P4.P4ClientError.MsgDm_ExVIEW) ||
                        (ex.ErrorCode == P4.P4ClientError.MsgDm_ExVIEW2))
                    {
                        string msg = ex.Message;
                        MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                while (inWorkspace == false)
                {
                    if (DialogResult.No == MessageBox.Show(
                        Resources.P4VsProvider_OpenSlnWantUpdateClientMapQuestion,
                        Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        return;
                    }
                    P4.Client client = SccService.ScmProvider.getClient(SccService.ScmProvider.Connection.Repository.Connection.Client.Name, null);
                    if (DlgEditWorkspace.EditWorkspace(SccService.ScmProvider, client) == null)
                    {
                        return;
                    }
                    fmd = SccService.ScmProvider.GetFileMetaData(depotPath);
                    if (fmd.LocalPath != null)
                    {
                        inWorkspace = true;
                    }
                }

                string path = null;
                string folder = null;
                if (fmd.LocalPath != null)
                {
                    path = fmd.LocalPath.Path;
                    folder = Path.GetDirectoryName(path);
                }
                else
                {
                    // if the local path is not returned from GetFileMetaData(), the client root must not exist on this machine,
                    // so try syncing the depot path
                    P4.ServerMetaData smd = SccService.ScmProvider.Connection.Repository.Server.Metadata;

                    path = fmd.DepotPath.Path;
                    folder = Path.GetDirectoryName(path);
                }
                if ((fmd.LocalPath == null) || (fmd.HaveRev <= 0))
                {
                    if ((fmd.HaveRev <= 0) &&
                        (DialogResult.No == MessageBox.Show(
                            Resources.P4VsProvider_OpenSlnWantSyncClientQuestion,
                            Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Question)))
                    {
                        return;
                    }
                    string solutionPath = string.Format("{0}\\...", folder);

                    P4.SyncFilesCmdOptions options = new P4.SyncFilesCmdOptions(P4.SyncFilesCmdFlags.Force, -1);
                    if (SccService.ScmProvider.SyncFiles(options, solutionPath) == false)
                    {
                        return;
                    }

                    //update the solution file meta data after the sync
                    fmd = SccService.ScmProvider.GetFileMetaData(depotPath);
                    if (fmd.LocalPath != null)
                    {
                        path = fmd.LocalPath.Path;
                        folder = Path.GetDirectoryName(path);
                    }
                }
                if ((fmd.HaveRev > 0) && !string.IsNullOrEmpty(path) && (!System.IO.File.Exists(path)))
                {
                    if (fmd.Action != P4.FileAction.None)
                    {
                        if (DialogResult.No == MessageBox.Show(
                            Resources.P4VsProvider_OpenSlnRevertDeletedButCheckoutQuestion,
                            Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            return;
                        }
                        SccService.ScmProvider.RevertFiles(false, false, null, path);
                    }
                    else if (DialogResult.No == MessageBox.Show(
                        Resources.P4VsProvider_OpenSlnWantSyncClientQuestion,
                        Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        return;
                    }
                    string solutionPath = string.Format("{0}\\...", folder);

                    IList<P4.File> opened = SccService.ScmProvider.GetOpenedFiles(
                        P4.FileSpec.LocalSpecList(solutionPath),
                        new P4.GetOpenedFilesOptions(P4.GetOpenedFilesCmdFlags.None, null,
                            SccService.ScmProvider.Connection.Workspace, SccService.ScmProvider.Connection.User, 1));

                    if ((opened != null) && (DialogResult.Yes == MessageBox.Show(
                        Resources.P4VsProvider_OpenSlnRevertCheckedoutDeletedFilesQuestion,
                        Resources.P4VS, MessageBoxButtons.YesNo, MessageBoxIcon.Question)))
                    {
                        P4.RevertCmdOptions revertOpts = new P4.RevertCmdOptions(
                            P4.RevertFilesCmdFlags.None, -1);

                        SccService.ScmProvider.RevertFiles(false, true, null, solutionPath);

                    }
                    P4.SyncFilesCmdOptions options = new P4.SyncFilesCmdOptions(P4.SyncFilesCmdFlags.Force, -1);
                    SccService.ScmProvider.SyncFiles(options, solutionPath);
                    //need to fetch the  file(s)
                }

                IVsSolution solution = (IVsSolution)GetService(typeof(SVsSolution));

                if (path != null && path.EndsWith("sln"))
                {
                    int res = solution.OpenSolutionFile(0, path);

                    IVsUIShell shell = (IVsUIShell)GetService(typeof(IVsUIShell));
                    shell.ReportErrorInfo(res);
                }

                else
                {
                    string dir;
                    string file;
                    string fileOpts;
                    solution.GetSolutionInfo(out dir, out file, out fileOpts);
                    // if there is no solution open, do not attempt to close it
                    if (file != null)
                    {
                        if (VSConstants.S_OK != solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, null, 0))
                        {
                            MessageBox.Show(Resources.P4VsProvider_ErrorClosingSolution, Resources.P4VS,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    Guid gnull = Guid.Empty;
                    Guid gInterface = Guid.Empty;
                    IntPtr pProj = IntPtr.Zero;

                    ErrorHandler.ThrowOnFailure(solution.CreateProject(ref gnull, path, null, null, (uint)__VSCREATEPROJFLAGS.CPF_OPENFILE, ref gInterface, out pProj));

                }
            }
        }



        private string[] ConnectionDropDownComboChoices = { Resources.ConnectionDropDownCombo_NoConnection };
        private int mruConnectionsCount = 0;

        // DropDownCombo
        //	 a DROPDOWNCOMBO does not let the user type into the combo box; they can only pick from the list.
        //   The string value of the element selected is returned.
        //	 For example, this type of combo could be used for the "Solution Configurations" on the "Standard" toolbar.
        //
        //   A DropDownCombo box requires two commands:
        //     One command (cmdidMyCombo) is used to ask for the current value of the combo box and to 
        //     set the new value when the user makes a choice in the combo box.
        //
        //     The second command (cmdidMyComboGetList) is used to retrieve this list of choices for the combo box.
        public void P4VsConnectionDropDownCombo(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if ((e == null) || (e == EventArgs.Empty))
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException("Null or empty arguments", "e")); // force an exception to be thrown
            }

            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            string newChoice = eventArgs.InValue as string;
            IntPtr vOut = eventArgs.OutValue;

            if (vOut != IntPtr.Zero && newChoice != null)
            {
                throw (new ArgumentException("Bad In/Out arguments")); // force an exception to be thrown
            }
            else if (vOut != IntPtr.Zero)
            {
                // when vOut is non-NULL, the IDE is requesting the current value for the combo
                Marshal.GetNativeVariantForObject(this.currentConnectionDropDownComboChoice, vOut);
            }

            else if (newChoice != null)
            {
                if (newChoice == Resources.ConnectionDropDownCombo_NoConnection)
                {
                    currentConnectionDropDownComboChoice = Resources.ConnectionDropDownCombo_NoConnection;

                    return;
                }
                if (newChoice == Resources.ConnectionDropDownCombo_CloseConnection)
                {
                    CloseConnection();
                    currentConnectionDropDownComboChoice = Resources.ConnectionDropDownCombo_NoConnection;
                    return;
                }
                // new value was selected or typed in
                // see if it is one of our items
                bool validInput = false;
                int indexInput = -1;
                for (indexInput = 0; indexInput < ConnectionDropDownComboChoices.Length; indexInput++)
                {
                    if (String.Compare(ConnectionDropDownComboChoices[indexInput], newChoice, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        validInput = true;
                        break;
                    }
                }

                if (validInput)
                {
                    MRUList recentConnections = (MRUList)Preferences.LocalSettings["RecentConnections"];
                    if (indexInput < mruConnectionsCount && recentConnections!=null)
                    {

                        ConnectionData cd = recentConnections[indexInput] as ConnectionData;
                        if (cd != null)
                        {
                            OpenConnection(cd.ServerPort, cd.UserName, cd.Workspace);
                        }
                        // 'Add' it to the mru list to move it to the top (most recent) position
                        recentConnections.Add(cd);
                    }
                    else
                    {
                        if (indexInput == mruConnectionsCount)
                        {
                            // new connection
                            OpenConnection(null, null, null);

                            recentConnections = (MRUList)Preferences.LocalSettings["RecentConnections"];

                            if (CurrentScm != null && Connection.CurrentScm.Connected &&
                                recentConnections != null)
                            {
                                currentConnectionDropDownComboChoice = recentConnections[0].ToString();
                            }
                            else
                            {
                                currentConnectionDropDownComboChoice = Resources.ConnectionDropDownCombo_NoConnection;
                            }
                        }
                        else
                        {
                            CloseConnection();
                            currentConnectionDropDownComboChoice = Resources.ConnectionDropDownCombo_NoConnection;
                        }
                    }
                }
                else
                {
                    throw (new ArgumentException("Not a valid connection")); // force an exception to be thrown
                }
            }
            else
            {
                // We should never get here
                throw (new ArgumentException("Bad In argument")); // force an exception to be thrown
            }
        }

        // A DropDownCombo box requires two commands:
        //    This command is used to retrieve this list of choices for the combo box.
        // 
        // Normally IOleCommandTarget::QueryStatus is used to determine the state of a command, e.g.
        // enable vs. disable, shown vs. hidden, etc. The QueryStatus method does not have any way to 
        // control the statue of a combo box, e.g. what list of items should be shown and what is the 
        // current value. In order to communicate this information actually IOleCommandTarget::Exec
        // is used with a non-NULL varOut parameter. You can think of these Exec calls as extended 
        // QueryStatus calls. There are two pieces of information needed for a combo, thus it takes
        // two commands to retrieve this information. The main command id for the command is used to 
        // retrieve the current value and the second command is used to retrieve the full list of 
        // choices to be displayed as an array of strings.
        public void P4VsConnectionDropDownComboGetList(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if ((null == e) || (e == EventArgs.Empty))
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException("Null arguments", "e")); // force an exception to be thrown
            }

            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                object inParam = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (inParam != null)
                {
                    throw (new ArgumentException("Bad In argument")); // force an exception to be thrown
                }
                else if (vOut != IntPtr.Zero)
                {
                    MRUList recentConnections = (MRUList)Preferences.LocalSettings["RecentConnections"];
                    if (recentConnections != null)
                    {
                        mruConnectionsCount = 0;
                        foreach (ConnectionData con in recentConnections)
                        {
                            if (con != null)
                            {
                                mruConnectionsCount++;
                            }
                        }
                        int count = mruConnectionsCount + 1;
                        if ((CurrentScm != null) && CurrentScm.Connected)
                        {
                            count++;
                        }

                        ConnectionDropDownComboChoices = new string[count];
                        int idx = 0;

                        foreach (ConnectionData con in recentConnections)
                        {
                            if (con != null)
                            {
                                ConnectionDropDownComboChoices[idx++] = con.ToString();
                            }
                        }
                        ConnectionDropDownComboChoices[idx++] = Resources.ConnectionDropDownCombo_NewConnection;
                        if ((CurrentScm != null) && CurrentScm.Connected)
                        {
                            ConnectionDropDownComboChoices[idx++] = Resources.ConnectionDropDownCombo_CloseConnection;
                        }
                    }
                    else
                    {
                        int count = 1;
                        if ((CurrentScm != null) && CurrentScm.Connected)
                        {
                            count++;
                        }
                        ConnectionDropDownComboChoices = new string[count];
                        ConnectionDropDownComboChoices[0] = Resources.ConnectionDropDownCombo_NewConnection;
                        if ((CurrentScm != null) && CurrentScm.Connected)
                        {
                            ConnectionDropDownComboChoices[1] = Resources.ConnectionDropDownCombo_CloseConnection;
                        }
                    }
                    Marshal.GetNativeVariantForObject(ConnectionDropDownComboChoices, vOut);
                }
                else
                {
                    throw (new ArgumentException("Bad Out argument")); // force an exception to be thrown
                }
            }
        }

        // DropDownCombo
        //	 a DROPDOWNCOMBO does not let the user type into the combo box; they can only pick from the list.
        //   The string value of the element selected is returned.
        //	 For example, this type of combo could be used for the "Solution Configurations" on the "Standard" toolbar.
        //
        //   A DropDownCombo box requires two commands:
        //     One command (cmdidMyCombo) is used to ask for the current value of the combo box and to 
        //     set the new value when the user makes a choice in the combo box.
        //
        //     The second command (cmdidMyComboGetList) is used to retrieve this list of choices for the combo box.
        public void P4VsActiveChangelistDropDownCombo(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if ((e == null) || (e == EventArgs.Empty))
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException("Null or empty arguments", "e")); // force an exception to be thrown
            }

            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            string newChoice = eventArgs.InValue as string;
            if ((eventArgs.InValue != null) && (newChoice == null) && (eventArgs.InValue is int))
            {
                int newVal = (int)eventArgs.InValue;
                if (newVal > 0)
                {
                    newChoice = newVal.ToString();
                }
                else if (newVal == 0)
                {
                    newChoice = Resources.Changelist_Default;
                }
                else
                {
                    newChoice = Resources.Changelist_New;
                }
            }

            IntPtr vOut = eventArgs.OutValue;

            if (vOut != IntPtr.Zero && newChoice != null)
            {
                throw (new ArgumentException("Bad In/Out arguments")); // force an exception to be thrown
            }
            else if (vOut != IntPtr.Zero)
            {
                // when vOut is non-NULL, the IDE is requesting the current value for the combo
                Marshal.GetNativeVariantForObject(ChangeLists.ActiveChangeListComboChoice, vOut);
            }

            else if (newChoice != null)
            {
                // new value was selected or typed in
                // see if it is one of our items
                bool validInput = false;
                int indexInput = -1;
                int val = -1;

                bool promptPreferenceSet = Preferences.LocalSettings.GetBool("PromptForChanglist", true);

                // if disconnected, the changelist dropdown is disabled, so it
                // goes back to default.
                if (promptPreferenceSet&& SccService.ScmProvider.Connection.Disconnected)
                {
                    newChoice = Resources.NoActiveChangelist;
                }
                if (newChoice == Resources.NoActiveChangelist)
                {
                    Preferences.LocalSettings.Set("PromptForChanglist", true);
                    val = -2;
                }
                else if (newChoice == Resources.NewChangelist)
                {
                    Preferences.LocalSettings.Set("PromptForChanglist", false);

#if true
                    ShelveFileCreateChangelistDlg dlg = new ShelveFileCreateChangelistDlg();
                    dlg.Prompt = Resources.NewChangeListDlgPrompt;
                    if (DialogResult.Cancel == dlg.ShowDialog())
                    {
                        return;
                    }
                    string description = dlg.Description;
                    P4.Changelist change = SccService.ScmProvider.Connection.Repository.NewChangelist();

                    change.Files = null;

                    change.Description = description;
#else
					P4ScmProvider Scm = SccService.ScmProvider;

					PendingChangelistDlg dlg = new PendingChangelistDlg(Scm);

					dlg.ChangeListId = -1;

					P4.ServerMetaData smd = Scm.GetServerMetaData();

					dlg.Text = string.Format(Resources.PendingChangelistsToolWindowControl_PendingChangelistDlgCaption,
						smd.Address.Uri, Scm.Repository.Connection.UserName);
					dlg.UserText = Scm.Repository.Connection.UserName;

					dlg.WorkspaceText = Scm.Repository.Connection.Client.Name;

					//dlg2.OnDialogClosed =
					//    new PendingChangelistDlg.PendingChangelistDialogCloseDelegate(newPendingChangelistDialogClosed);

					if (DialogResult.Cancel == dlg.ShowDialog())
					{
						return;
					}
					P4.Changelist change = dlg.ChangeList;
#endif
                    P4.Changelist newChange = SccService.ScmProvider.SaveChangelist(change, null);

                    val = newChange.Id;

                    SccService.ScmProvider.BroadcastChangelistUpdate(this,
                        new P4ScmProvider.ChangelistUpdateArgs(val, P4ScmProvider.ChangelistUpdateArgs.UpdateType.Add));

                    string d = newChange.Description;
                    d = d.Replace('\r', ' ');
                    d = d.Replace('\n', ' ');
                    newChoice = string.Format("{0} {1}", val, d);
                    ChangeLists.addActiveChangeListComboChoicesMap(val, d);
                }
                else if (newChoice == Resources.Changelist_Default)
                {
                    Preferences.LocalSettings.Set("PromptForChanglist", false);
                    val = 0;
                }
                else
                {
                    for (indexInput = 0; indexInput < ChangeLists.ActiveChangeListComboChoices.Length; indexInput++)
                    {
                        if (String.Compare(ChangeLists.ActiveChangeListComboChoices[indexInput], newChoice, StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            int breakIdx = newChoice.IndexOf(' ');
                            if (breakIdx >= 0)
                            {
                                string changeId = newChoice.Substring(0, breakIdx);
                                int.TryParse(changeId, out val);
                                validInput = true;
                            }
                            break;
                        }
                        else if (ChangeLists.ActiveChangeListComboChoices[indexInput].StartsWith(newChoice))
                        {
                            // just add the changlist number with out the description
                            newChoice = ChangeLists.ActiveChangeListComboChoices[indexInput];
                            validInput = true;
                            break;
                        }
                    }
                    if (validInput == false)
                    {
                        throw (new ArgumentException("Not a valid changelist")); // force an exception to be thrown
                    }
                    Preferences.LocalSettings.Set("PromptForChanglist", false);
                }
                ChangeLists.SetActiveChangeList(val);

                if ((SccService != null) && (SccService.ScmProvider != null) && (SccService.ScmProvider.Connection.Repository != null) &&
                    (SccService.ScmProvider.Connection.Repository.Connection != null) && (SccService.ScmProvider.Connection.Repository.Connection.Server != null) &&
                    (SccService.ScmProvider.Connection.Repository.Connection.Server.Address != null) && (SccService.ScmProvider.Connection.Repository.Connection.Server.Address.Uri != null))
                {
                    string key = "ActiveChangelist_" + SccService.ScmProvider.Connection.Repository.Connection.Server.Address.Uri.Replace(':', '_') +
                       "_" + SccService.ScmProvider.Connection.User + "_" + SccService.ScmProvider.Connection.Workspace;
                    Preferences.LocalSettings.Set(key, ChangeLists.ActiveChangeListComboChoice);

                    if (Preferences.LocalSettings.GetBool("SetEnvironmentVars", true))
                    {
                        if (ChangeLists.ActiveChangeList == 0)
                        {
                            Environment.SetEnvironmentVariable("P4VS_ACTIVE_CHANGELIST", "default");
                        }
                        else
                        {
                            Environment.SetEnvironmentVariable("P4VS_ACTIVE_CHANGELIST", ChangeLists.ActiveChangeList.ToString());
                        }
                    }
                }
            }
            else
            {
                // We should never get here
                throw (new ArgumentException("Bad In argument")); // force an exception to be thrown
            }
        }

        // A DropDownCombo box requires two commands:
        //    This command is used to retrieve this list of choices for the combo box.
        // 
        // Normally IOleCommandTarget::QueryStatus is used to determine the state of a command, e.g.
        // enable vs. disable, shown vs. hidden, etc. The QueryStatus method does not have any way to 
        // control the statue of a combo box, e.g. what list of items should be shown and what is the 
        // current value. In order to communicate this information actually IOleCommandTarget::Exec
        // is used with a non-NULL varOut parameter. You can think of these Exec calls as extended 
        // QueryStatus calls. There are two pieces of information needed for a combo, thus it takes
        // two commands to retrieve this information. The main command id for the command is used to 
        // retrieve the current value and the second command is used to retrieve the full list of 
        // choices to be displayed as an array of strings.
        public void P4VsActiveChangelistDropDownComboGetList(object sender, EventArgs e)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if ((null == e) || (e == EventArgs.Empty))
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException("Null arguments", "e")); // force an exception to be thrown
            }

            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                object inParam = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (inParam != null)
                {
                    throw (new ArgumentException("Bad In argument")); // force an exception to be thrown
                }
                else if (vOut != IntPtr.Zero)
                {
                    ChangeLists = new ActiveChangeListCombo(SccService); //PAUL InitActiveChangeListComboChoicesMap();

                    Marshal.GetNativeVariantForObject(ChangeLists.ActiveChangeListComboChoices, vOut);
                }
                else
                {
                    throw (new ArgumentException("Bad Out argument")); // force an exception to be thrown
                }
            }
        }

        public void SaveAll()
        {
            EnvDTE.DTE dte2;
            dte2 = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
            dte2.ExecuteCommand("File.SaveAll");
        }

    }
}
