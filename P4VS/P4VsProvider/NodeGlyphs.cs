using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell.Interop;
using System.Threading;
using Microsoft.VisualStudio;
using NLog;
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace Perforce.P4VS
{
    public class NodeGlyphs : BackwardsCompatibleAsyncPackage
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private P4VsProviderService SccService;
        private IVsHierarchy solHier;

        public NodeGlyphs(P4VsProviderService service, IVsHierarchy hier)
        {
            SccService = service;
            solHier = hier;
        }

        /// <summary>
        /// Refreshes the glyphs of the specified hierarchy nodes
        /// </summary>
        public void RefreshSelectedNodesGlyphs()
        {
            RefreshNodesGlyphs(SccService.SelectedNodes, SccService.SelectedFiles);
        }

        /// <summary>
        /// Refreshes the glyphs of the specified hierarchy nodes
        /// </summary>
        public void RefreshNodesGlyphs(IList<VSITEMSELECTION> selectedNodes, IList<string> files)
        {
            RefreshNodesGlyphsArgs args = new RefreshNodesGlyphsArgs(selectedNodes, files);

            if (Preferences.LocalSettings.GetBool("AsyncRefreshNodesGlyphs", false))
            {
                Thread refreshProc = new Thread(new ParameterizedThreadStart(SyncAsyncRefreshNodesGlyphs));
                refreshProc.IsBackground = true;
                refreshProc.Name = "AsyncRefreshNodesGlyphs";

                refreshProc.Start(args);
            }
            else
            {
                SyncAsyncRefreshNodesGlyphs(args);
            }
        }

        internal class RefreshNodesGlyphsArgs
        {
            private RefreshNodesGlyphsArgs() { }
            public RefreshNodesGlyphsArgs(IList<VSITEMSELECTION> _selectedNodes, IList<string> _files)
            {
                selectedNodes = _selectedNodes;
                files = _files;
            }
            public IList<VSITEMSELECTION> selectedNodes;
            public IList<string> files;
        }

        /// <summary>
        /// Refreshes the glyphs of the specified hierarchy nodes
        /// </summary>
        private void SyncAsyncRefreshNodesGlyphs(object parm)
        {
            RefreshNodesGlyphsArgs parms = parm as RefreshNodesGlyphsArgs;

            if (parms == null)
            {
                return;
            }

            IList<VSITEMSELECTION> selectedNodes = parms.selectedNodes;
            IList<string> files = parms.files;

            if (SccService.IsSolutionSelected)
            {
                selectedNodes = null;
            }
            P4VsProvider.Instance.ResetCommandStatus();

            if (!P4VsProvider.Instance.IsThereASolution())
            {
                return;
            }
            // make sure the context menus update for the new state
            P4VsProvider.Instance.ResetCommandStatus();

            if (selectedNodes == null)
            {
                bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
                if (TreatProjectsAsFolders)
                {
                    string slnPath = Path.GetDirectoryName(P4VsProvider.Instance.GetSolutionFileName());
                    slnPath = string.Format("{0}\\...", slnPath);
                    files = new List<string>();
                    files.Add(slnPath);
                }
                else
                {
                    files = P4VsProvider.Instance.GetSolutionFiles();
                }
                selectedNodes = new List<VSITEMSELECTION>();

                IList<IVsHierarchy> controlledProjects = SccService.GetControlledProjects();
                if (controlledProjects != null)
                {
                    foreach (IVsHierarchy node in controlledProjects)
                    {
                        VSITEMSELECTION vsItem;
                        vsItem.itemid = VSConstants.VSITEMID_ROOT;
                        vsItem.pHier = node;
                        selectedNodes.Add(vsItem);
                    }
                }
            }

            if (SccService.ScmProvider != null)
            {
                if ((files != null) && (files.Count > 0))
                {
                    if (Preferences.LocalSettings.GetBool("LazyLoadStatus", false) == false)
                    {
                        SccService.ScmProvider.UpdateFiles(files, true);
                    }
                    foreach (string file in files)
                    {
                        if ((file.EndsWith("\\...")) || (file.EndsWith("\\*")))
                        {
                            //wildcard
                            SccService.ScmProvider.ResetIgnoredFilesMap();
                            break;
                        }
                        SccService.ScmProvider.ResetIgnoredFilesMap(file);
                    }
                }
                else
                {
                    SccService.ScmProvider.ResetIgnoredFilesMap();
                }
            }
            foreach (VSITEMSELECTION vsItemSel in selectedNodes)
            {
                IVsSccProject2 sccProject2 = vsItemSel.pHier as IVsSccProject2;
                if (vsItemSel.itemid == VSConstants.VSITEMID_ROOT)
                {
                    string name = "";
                    if (vsItemSel.pHier!=null)
                    {
                        vsItemSel.pHier.GetCanonicalName(vsItemSel.itemid, out name);
                    }
                    if (sccProject2 == null)
                    {
                        // Note: The solution's hierarchy does not implement IVsSccProject2, IVsSccProject interfaces
                        // It may be a pain to treat the solution as special case everywhere; a possible workaround is 
                        // to implement a solution-wrapper class, that will implement IVsSccProject2, IVsSccProject and
                        // IVsHierarhcy interfaces, and that could be used in provider's code wherever a solution is needed.
                        // This approach could unify the treatment of solution and projects in the provider's code.

                        // Until then, solution is treated as special case
                        string[] rgpszFullPaths = new string[1];
                        rgpszFullPaths[0] = P4VsProvider.Instance.GetSolutionFileName();
                        VsStateIcon[] rgsiGlyphs = new VsStateIcon[1];
                        uint[] rgdwSccStatus = new uint[1];

                        if (SccService.ScmProvider != null)
                        {
                            SccService.ScmProvider.UpdateFileInCache(rgpszFullPaths[0], true);
                        }
                        SccService.GetSccGlyph(1, rgpszFullPaths, rgsiGlyphs, rgdwSccStatus);

                        // Set the solution's glyph directly in the hierarchy
                        solHier.SetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_StateIconIndex, rgsiGlyphs[0]);
                    }
                    else
                    {
                        if ((SccService.UiDispatcher != null) &&
                            (SccService.UiDispatcher.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId))
                        {
                            SccService.UiDispatcher.BeginInvoke(
                                new SccGlyphChangedDelegate(SccGlyphChanged), sccProject2, 0, null, null, null);
                        }
                        else
                        {
                            sccProject2.SccGlyphChanged(0, null, null, null);
                        }
                    }
                }
                else
                {
                    // Refresh all the glyphs in the project; the project will call back GetSccGlyphs() 
                    // with the files for each node that will need new glyph
                    //sccProject2.SccGlyphChanged(0,null,null,null);

                    // It may be easier/faster to simply refresh all the nodes in the project, 
                    // and let the project call back on GetSccGlyphs, but just for the sake of the demo, 
                    // let's refresh ourselves only one node at a time
                    IList<string> sccFiles = P4VsProvider.Instance.GetNodeFiles(sccProject2, vsItemSel.itemid);

                    //// We'll use for the node glyph just the Master file's status (ignoring special files of the node)
                    if (sccFiles.Count > 0)
                    {
                        for (int idx = 0; idx < sccFiles.Count; idx++)
                        {
                            string[] rgpszFullPaths = new string[1];
                            rgpszFullPaths[0] = sccFiles[idx];
                            VsStateIcon[] rgsiGlyphs = new VsStateIcon[1];
                            uint[] rgdwSccStatus = new uint[1];
                            if ((SccService.ScmProvider != null) && (files == null) || (files.Count <= 0))
                            {
                                // update if not already updated
                                SccService.ScmProvider.UpdateFileInCache(sccFiles[idx], true);
                            }
                            SccService.GetSccGlyph(1, rgpszFullPaths, rgsiGlyphs, rgdwSccStatus);

                            uint[] rguiAffectedNodes = new uint[1];
                            if (idx == 0)
                            {
                                rguiAffectedNodes[0] = vsItemSel.itemid;
                                if (sccProject2 != null)
                                {
                                    // Don't async update the node's glyph if the new status is deleted
                                    // The mode might not exist anymore when the update is actually run
                                    if ((SccService.UiDispatcher != null) &&
                                        (SccService.UiDispatcher.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId) &&
                                        ((rgdwSccStatus[0] & (uint)__SccStatus.SCC_STATUS_DELETED) == 0))
                                    {
                                        SccService.UiDispatcher.BeginInvoke(
                                            new SccGlyphChangedDelegate(SccGlyphChanged), sccProject2,
                                                1, rguiAffectedNodes, rgsiGlyphs, rgdwSccStatus);
                                    }
                                    else
                                    {
                                        sccProject2.SccGlyphChanged(1, rguiAffectedNodes, rgsiGlyphs, rgdwSccStatus);
                                    }
                                }
                            }
                            else
                            {
                                uint itemid;
                                int fFound;
                                IVsProject pscp = sccProject2 as IVsProject;

                                VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];
                                if (pscp != null && pscp.IsDocumentInProject(sccFiles[idx], out fFound, prio, out itemid) == VSConstants.S_OK && fFound != 0)
                                {
                                    rguiAffectedNodes[0] = itemid;
                                    // Don't async update the node's glyph if the new status is deleted
                                    // The mode might not exist anymore when the update is actually run
                                    if ((SccService.UiDispatcher != null) &&
                                        (SccService.UiDispatcher.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId) &&
                                        ((rgdwSccStatus[0] & (uint)__SccStatus.SCC_STATUS_DELETED) == 0))
                                    {
                                        SccService.UiDispatcher.BeginInvoke(
                                            new SccGlyphChangedDelegate(SccGlyphChanged), sccProject2,
                                                1, rguiAffectedNodes, rgsiGlyphs, rgdwSccStatus);
                                    }
                                    else
                                    {
                                        sccProject2.SccGlyphChanged(1, rguiAffectedNodes, rgsiGlyphs, rgdwSccStatus);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // could not find files based on the nodes selected, so use the list of files passed in
                        SccService.RefreshProjectGlyphs(files, true);
                    }
                }
            }
        }

        private delegate int SccGlyphChangedDelegate(IVsSccProject2 sccProject2, int cAffectedNodes, uint[] rgitemidAffectedNodes, VsStateIcon[] rgsiNewGlyphs, uint[] rgdwNewSccStatus);

        private int SccGlyphChanged(IVsSccProject2 sccProject2, int cAffectedNodes, uint[] rgitemidAffectedNodes, VsStateIcon[] rgsiNewGlyphs, uint[] rgdwNewSccStatus)
        {
            try
            {
                return sccProject2.SccGlyphChanged(cAffectedNodes, rgitemidAffectedNodes, rgsiNewGlyphs, rgdwNewSccStatus);
            }
            catch { }
            return VSConstants.S_OK;
        }

        public void RefreshFilesAndGlyphs(IList<string> files)
        {
            if (files == null)
            {
                return;
            }

            foreach (string file in files)
            {
                SccService.ScmProvider.ResetIgnoredFilesMap(file);
            }

            // now refresh the selected nodes' glyphs
            SccService.RefreshProjectGlyphs_Int(files, true);
        }

    }
}
