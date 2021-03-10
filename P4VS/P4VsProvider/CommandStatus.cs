using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NLog;
using Perforce.P4Scm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Perforce.P4VS
{
    public abstract class CommandStatus : PersistSolutionProps
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

//        private P4VsProviderService SccService;
//        private P4ScmProvider CurrentScm;

        private bool _isThereASolution = false;


//        public CommandStatus(P4VsProviderService service, P4ScmProvider scm)
//        {
//            SccService = service;
//            CurrentScm = scm;
//        }

        

        private bool CommandsStatusUpdated = false;

        private bool cmdf_FileStatusIsLazyLoaded = false;

        OLECMDF cmdf_icmdAddToSourceControl = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdCheckin = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdCheckout = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdCheckoutProject = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdCheckoutSolution = OLECMDF.OLECMDF_SUPPORTED;
        //OLECMDF cmdf_icmdUseSccOffline = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdP4V = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdTimeLapse = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdRevGraph = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdStreamGraph = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdRevert = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdReconcile = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdRevertUnchanged = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdLock = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdUnlock = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdChangeFileType = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdMoveToChangelist = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdAddToIgnoreList = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdRemoveFromIgnoreList = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdEditIgnoreList = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdResolve = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdSync = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdSyncHead = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdDiffVsHave = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdDiffVsAny = OLECMDF.OLECMDF_SUPPORTED;
        //OLECMDF cmdf_icmdAddProjectToSCC = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdShowHistory = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdShelve = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdScmMerge = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdScmCopy = OLECMDF.OLECMDF_SUPPORTED;
        OLECMDF cmdf_icmdPublish = OLECMDF.OLECMDF_SUPPORTED;


        public void ResetCommandStatus()
        {
            CommandsStatusUpdated = false;
        }

        private void GetCommandStatus()
        {
            try
            {
                _isThereASolution = IsThereASolution();

                cmdf_FileStatusIsLazyLoaded = QueryIsLazyLoad();
                // Process our Commands
                cmdf_icmdAddToSourceControl = QueryStatus_icmdAddToSourceControl();
                cmdf_icmdCheckin = QueryStatus_icmdCheckin();
                cmdf_icmdCheckout = QueryStatus_icmdCheckout();
                cmdf_icmdCheckoutProject = QueryStatus_icmdCheckoutProject();
                cmdf_icmdCheckoutSolution = QueryStatus_icmdCheckoutSolution();
                //cmdf_icmdUseSccOffline = QueryStatus_icmdUseSccOffline();
                //cmdf_icmdUseSccOffline = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdP4V = QueryStatus_icmdP4V();
                cmdf_icmdTimeLapse = QueryStatus_icmdTimeLapse();
                cmdf_icmdRevGraph = QueryStatus_icmdRevGraph();
                cmdf_icmdStreamGraph = QueryStatus_icmdStreamGraph();
                cmdf_icmdRevert = QueryStatus_icmdRevert();
                cmdf_icmdReconcile = QueryStatus_icmdReconcile();
                cmdf_icmdRevertUnchanged = QueryStatus_icmdRevertUnchanged();
                cmdf_icmdLock = QueryStatus_icmdLock();
                cmdf_icmdUnlock = QueryStatus_icmdUnlock();
                cmdf_icmdChangeFileType = QueryStatus_icmdChangeFileType();
                cmdf_icmdMoveToChangelist = QueryStatus_icmdMoveToChangelist();
                cmdf_icmdAddToIgnoreList = QueryStatus_icmdAddToIgnoreList();
                cmdf_icmdRemoveFromIgnoreList = QueryStatus_icmdRemoveFromIgnoreList();
                cmdf_icmdEditIgnoreList = QueryStatus_icmdEditIgnoreList();
                cmdf_icmdResolve = QueryStatus_icmdResolve();
                cmdf_icmdSync = QueryStatus_icmdSync();
                cmdf_icmdSyncHead = QueryStatus_icmdSyncHead();
                cmdf_icmdDiffVsHave = QueryStatus_icmdDiffVsHave();
                cmdf_icmdDiffVsAny = QueryStatus_icmdDiffVsAny();
                //			cmdf_icmdAddProjectToSCC = QueryStatus_icmdAddProjectToSCC();
                cmdf_icmdShowHistory = QueryStatus_icmdShowHistory();
                cmdf_icmdShelve = QueryStatus_icmdShelve();
                cmdf_icmdScmMerge = QueryStatus_icmdScmMerge();
                cmdf_icmdScmCopy = QueryStatus_icmdScmCopy();
                cmdf_icmdPublish = QueryStatus_icmdAddToSourceControl();

                CommandsStatusUpdated = true;
            }
            catch
            {
                cmdf_FileStatusIsLazyLoaded = false;

                cmdf_icmdAddToSourceControl = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdCheckin = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdCheckout = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdCheckoutProject = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdCheckoutSolution = OLECMDF.OLECMDF_INVISIBLE;
                //cmdf_icmdUseSccOffline = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdP4V = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdTimeLapse = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdRevGraph = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdStreamGraph = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdReconcile = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdRevert = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdRevertUnchanged = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdLock = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdUnlock = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdChangeFileType = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdMoveToChangelist = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdAddToIgnoreList = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdRemoveFromIgnoreList = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdEditIgnoreList = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdResolve = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdSync = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdSyncHead = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdDiffVsHave = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdDiffVsAny = OLECMDF.OLECMDF_INVISIBLE;
                //			cmdf_icmdAddProjectToSCC = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdShowHistory = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdShelve = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdScmMerge = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdScmCopy = OLECMDF.OLECMDF_INVISIBLE;
                cmdf_icmdPublish = OLECMDF.OLECMDF_INVISIBLE;

                CommandsStatusUpdated = false;
            }
        }

        public void SetOleCmdText(IntPtr pCmdText, string text)
        {
            if (text == null)
            {
                return;
            }
            OLECMDTEXT CmdText = (OLECMDTEXT)Marshal.PtrToStructure(pCmdText, typeof(OLECMDTEXT));

            char[] buffer = null;
            if (text != null)
            {
                buffer = text.ToCharArray();
            }
            else
            {
                buffer = new char[] { ' ' };
            }
            IntPtr pText = (IntPtr)((long)pCmdText + (long)Marshal.OffsetOf(typeof(OLECMDTEXT), "rgwz"));

            IntPtr pCwActual = (IntPtr)((long)pCmdText + (long)Marshal.OffsetOf(typeof(OLECMDTEXT), "cwActual"));

            // The max chars we copy is our string, or one less than the buffer size,

            // since we need a null at the end.

            int maxChars = (int)Math.Min(CmdText.cwBuf - 1, buffer.Length);

            Marshal.Copy(buffer, 0, pText, maxChars);

            // append a null

            Marshal.WriteInt16((IntPtr)((long)pText + (long)maxChars * 2), (Int16)0);

            // write out the length + null char

            Marshal.WriteInt32(pCwActual, maxChars + 1);
        }


        /// <summary>
        /// The shell call this function to know if a menu item should be visible and
        /// if it should be enabled/disabled.
        /// Note that this function will only be called when an instance of this editor
        /// is open.
        /// </summary>
        /// <param name="guidCmdGroup">Guid describing which set of command the current command(s) belong to</param>
        /// <param name="cCmds">Number of command which status are being asked for</param>
        /// <param name="prgCmds">Information for each command</param>
        /// <param name="pCmdText">Used to dynamically change the command text</param>
        /// <returns>HRESULT</returns>
        public int QueryStatus(ref Guid guidCmdGroup, uint cCmds, OLECMD[] prgCmds, System.IntPtr pCmdText)
        {
            //change the text of the checkout command 
            OLECMDTEXT cmdtxtStructure = (OLECMDTEXT)Marshal.PtrToStructure(pCmdText, typeof(OLECMDTEXT));

            Debug.Assert(cCmds == 1, "Multiple commands");
            Debug.Assert(prgCmds != null, "NULL argument");

            if ((prgCmds == null))
                return VSConstants.E_INVALIDARG;

            // Filter out commands that are not defined by this package
            if (guidCmdGroup != GuidList.guidP4VsProviderCmdSet)
            {
                return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED); ;
            }
            // First set the text for the menu item regardless of eventual state
            switch (prgCmds[0].cmdID)
            {
                case CommandId.icmdScmRefresh:
                    SetOleCmdText(pCmdText, Resources.icmdScmRefresh);
                    break;

                case CommandId.icmdAddToSourceControl:
                    SetOleCmdText(pCmdText, Resources.icmdAddToSourceControl);
                    break;

                case CommandId.icmdCheckin:
                    SetOleCmdText(pCmdText, Resources.icmdCheckin);
                    break;

                case CommandId.icmdCheckout:
                    if (cmdtxtStructure.cmdtextf == (uint)OLECMDTEXTF.OLECMDTEXTF_NAME)
                    {
                        {
                            IList<VSITEMSELECTION> nodes = SccService.SelectedNodes;
                            string Name = string.Empty;

                            if (nodes != null)
                            {
                                if (SccService.IsSolutionSelected)
                                {

                                    Name = GetSolutionFileName();
                                    SetOleCmdText(pCmdText,
                                        string.Format(Resources.icmdCheckout, Path.GetFileName(Name)));
                                }
                                else if (SccService.IsaControlledProjectSelected)
                                {
                                    if (nodes.Count == 1)
                                    {
                                        IVsProject pscp = nodes[0].pHier as IVsProject;
                                        if (pscp != null)
                                        {
                                            pscp.GetMkDocument(nodes[0].itemid, out Name);
                                            SetOleCmdText(pCmdText,
                                                string.Format(Resources.icmdCheckout, Path.GetFileName(Name)));
                                        }
                                    }

                                    else
                                    {
                                        SetOleCmdText(pCmdText, Resources.icmdCheckoutProjectFiles);
                                    }
                                }
                                else
                                {
                                    if (nodes.Count == 1)
                                    {
                                        IVsProject pscp = nodes[0].pHier as IVsProject;
                                        if (pscp != null)
                                        {
                                            pscp.GetMkDocument(nodes[0].itemid, out Name);
                                            SetOleCmdText(pCmdText,
                                                string.Format(Resources.icmdCheckout, Path.GetFileName(Name)));
                                        }
                                    }

                                    else
                                    {
                                        SetOleCmdText(pCmdText, Resources.icmdCheckoutSelectedFiles);
                                    }
                                }
                            }
                        }
                    }
                    break;

                case CommandId.icmdCheckoutProject:
                    SetOleCmdText(pCmdText, Resources.icmdCheckoutProject);
                    break;

                case CommandId.icmdCheckoutSolution:
                    SetOleCmdText(pCmdText, Resources.icmdCheckoutSolution);
                    break;

                case CommandId.icmdP4V:
                    SetOleCmdText(pCmdText, Resources.icmdP4V);
                    break;

                case CommandId.icmdTimeLapse:
                    SetOleCmdText(pCmdText, Resources.icmdTimeLapse);
                    break;

                case CommandId.icmdRevGraph:
                    SetOleCmdText(pCmdText, Resources.icmdRevGraph);
                    break;

                case CommandId.icmdStreamGraph:
                    SetOleCmdText(pCmdText, Resources.icmdStreamGraph);
                    break;

                case CommandId.icmdReconcile:
                    SetOleCmdText(pCmdText, Resources.icmdReconcile);
                    break;

                case CommandId.icmdRevert:
                    SetOleCmdText(pCmdText, Resources.icmdRevert);
                    break;

                case CommandId.icmdRevertUnchanged:
                    SetOleCmdText(pCmdText, Resources.icmdRevertUnchanged);
                    break;

                case CommandId.icmdLock:
                    SetOleCmdText(pCmdText, Resources.icmdLock);
                    break;

                case CommandId.icmdUnlock:
                    SetOleCmdText(pCmdText, Resources.icmdUnlock);
                    break;

                case CommandId.icmdChangeFileType:
                    SetOleCmdText(pCmdText, Resources.icmdChangeFileType);
                    break;

                case CommandId.icmdMoveToChangelist:
                    SetOleCmdText(pCmdText, Resources.icmdMoveToChangelist);
                    break;

                case CommandId.icmdAddToIgnoreList:
                    SetOleCmdText(pCmdText, Resources.icmdAddToIgnoreList);
                    break;

                case CommandId.icmdRemoveFromIgnoreList:
                    SetOleCmdText(pCmdText, Resources.icmdRemoveFromIgnoreList);
                    break;

                case CommandId.icmdEditIgnoreList:
                    SetOleCmdText(pCmdText, Resources.icmdEditIgnoreList);
                    break;

                case CommandId.icmdResolve:
                    SetOleCmdText(pCmdText, Resources.icmdResolve);
                    break;

                case CommandId.icmdSync:
                    SetOleCmdText(pCmdText, Resources.icmdSync);
                    break;

                case CommandId.icmdSyncHead:
                    SetOleCmdText(pCmdText, Resources.icmdSyncHead);
                    break;

                case CommandId.icmdDiffVsHave:
                    SetOleCmdText(pCmdText, Resources.icmdDiffVsHave);
                    break;

                case CommandId.icmdDiffVsAny:
                    SetOleCmdText(pCmdText, Resources.icmdDiffVsAny);
                    break;

                case CommandId.icmdShowHistory:
                    SetOleCmdText(pCmdText, Resources.icmdShowHistory);
                    break;

                case CommandId.icmdShelve:
                    SetOleCmdText(pCmdText, Resources.icmdShelve);
                    break;

                case CommandId.icmdScmMerge:
                    SetOleCmdText(pCmdText, Resources.icmdScmMerge);
                    break;

                case CommandId.icmdScmCopy:
                    SetOleCmdText(pCmdText, Resources.icmdScmCopy);
                    break;

                case CommandId.icmdViewWorkspaceToolWindow:
                    SetOleCmdText(pCmdText, Resources.icmdViewWorkspaceToolWindow);
                    break;

                case CommandId.icmdViewHistoryToolWindow:
                    SetOleCmdText(pCmdText, Resources.icmdViewHistoryToolWindow);
                    break;

                case CommandId.icmdViewJobsToolWindow:
                    SetOleCmdText(pCmdText, Resources.icmdViewJobsToolWindow);
                    break;

                case CommandId.icmdViewLabelsToolWindow:
                    SetOleCmdText(pCmdText, Resources.icmdViewLabelsToolWindow);
                    break;

                //case CommandId.icmdViewSwarmToolWindow:
                //    SetOleCmdText(pCmdText, Resources.icmdViewSwarmToolWindow);
                //    break;

                //case CommandId.icmdViewReviewsToolWindow:
                //    SetOleCmdText(pCmdText, Resources.icmdViewReviewsToolWindow);
                //    break;

                case CommandId.icmdViewStreamsToolWindow:
                    SetOleCmdText(pCmdText, Resources.icmdViewStreamsToolWindow);
                    break;

                case CommandId.icmdViewSubmittedChangelistsToolWindow:
                    SetOleCmdText(pCmdText, Resources.icmdViewSubmittedChangelistsToolWindow);
                    break;

                case CommandId.icmdViewPendingChangelistsToolWindow:
                    SetOleCmdText(pCmdText, Resources.icmdViewPendingChangelistsToolWindow);
                    break;

                case CommandId.icmdFileOpenInDepot:
                    SetOleCmdText(pCmdText, Resources.icmdFileOpenInDepot);
                    break;

                case CommandId.icmdOpenConnection:
                    SetOleCmdText(pCmdText, Resources.icmdOpenConnection);
                    break;

                case CommandId.imnuFileSourceControlMenu:
                    SetOleCmdText(pCmdText, Resources.imnuFileSourceControlMenu);
                    break;
                case CommandId.imnuManageMenu:
                    SetOleCmdText(pCmdText, Resources.imnuManageMenu);
                    break;
                case CommandId.imnuRevisionsMenu:
                    SetOleCmdText(pCmdText, Resources.imnuRevisionsMenu);
                    break;
                case CommandId.imnuCopyMergeMenu:
                    SetOleCmdText(pCmdText, Resources.imnuCopyMergeMenu);
                    break;
                case CommandId.imnuDiffMenu:
                    SetOleCmdText(pCmdText, Resources.imnuDiffMenu);
                    break;
                case CommandId.imnuViewsMenu:
                    SetOleCmdText(pCmdText, Resources.imnuViewsMenu);
                    break;
                case CommandId.icmdP4VSHelp:
                    SetOleCmdText(pCmdText, Resources.icmdP4VSHelp);
                    break;
                case CommandId.icmdP4VSSystemInfo:
                    SetOleCmdText(pCmdText, Resources.icmdP4VSSystemInfo);
                    break;
                case CommandId.icmdPublish:
                    SetOleCmdText(pCmdText, Resources.icmdPublish);
                    break;
                case CommandId.icmdCloseConnection:
                    SetOleCmdText(pCmdText, Resources.icmdCloseConnection);
                    break;
                case CommandId.cmdidCancelActiveCommand:
                    SetOleCmdText(pCmdText, Resources.cmdidCancelActiveCommand);
                    break;
                case CommandId.cmdidCurrentStream:
                    if ((SccService != null) && (SccService.ScmProvider != null) && (SccService.ScmProvider.Connected) &&
                        (SccService.ScmProvider.Connection.Repository != null) && (SccService.ScmProvider.Connection.Repository.Connection != null) &&
                        (SccService.ScmProvider.Connection.Repository.Connection.Client != null) &&
                        (SccService.ScmProvider.Connection.Repository.Connection.Client.Stream != null))
                    {
                        string curStream = SccService.ScmProvider.Connection.Repository.Connection.Client.Stream;
                        SetOleCmdText(pCmdText, curStream);
                    }
                    else
                    {
                        prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_INVISIBLE | OLECMDF.OLECMDF_DEFHIDEONCTXTMENU);
                        SetOleCmdText(pCmdText, " ");
                        return VSConstants.S_OK;
                    }
                    break;
                default:
                    break;
            }

            OLECMDF cmdf = OLECMDF.OLECMDF_SUPPORTED;

            if ((prgCmds[0].cmdID ==CommandId.icmdPublish))
            {
                prgCmds[0].cmdf = (uint)(cmdf | OLECMDF.OLECMDF_ENABLED);

                return VSConstants.S_OK;
            }

            if ((prgCmds[0].cmdID == CommandId.cmdidConnectionDropDownCombo) ||
                (prgCmds[0].cmdID == CommandId.cmdidConnectionDropDownComboGetList))
            {
                prgCmds[0].cmdf = (uint)(cmdf | OLECMDF.OLECMDF_ENABLED);

                return VSConstants.S_OK;
            }
            if ((prgCmds[0].cmdID == CommandId.cmdidActiveChangelistDropDownCombo) ||
                (prgCmds[0].cmdID == CommandId.cmdidActiveChangelistDropDownComboGetList))
            {
                if ((CurrentScm != null) && CurrentScm.Connected)
                {
                    prgCmds[0].cmdf = (uint)(cmdf | OLECMDF.OLECMDF_ENABLED);
                }
                return VSConstants.S_OK;
            }

            if (CommandId.icmdP4VSHelp == prgCmds[0].cmdID)
            {
                prgCmds[0].cmdf = (uint)(cmdf | OLECMDF.OLECMDF_ENABLED);

                return VSConstants.S_OK;
            }

            if (CommandId.icmdP4VSSystemInfo == prgCmds[0].cmdID)
            {
                prgCmds[0].cmdf = (uint)(cmdf | OLECMDF.OLECMDF_ENABLED);

                return VSConstants.S_OK;
            }

            if ((SccService.ScmProvider == null) || (SccService.ScmProvider.Connection.Disconnected))
            {
                if ((CommandId.icmdFileOpenInDepot != prgCmds[0].cmdID) && (CommandId.icmdOpenConnection != prgCmds[0].cmdID))
                {
                    prgCmds[0].cmdf = (uint)(cmdf | OLECMDF.OLECMDF_INVISIBLE);

                    return VSConstants.S_OK;
                }
            }
            else
            {
                if (CommandId.icmdCloseConnection == prgCmds[0].cmdID)
                {
                    prgCmds[0].cmdf = (uint)(cmdf | OLECMDF.OLECMDF_ENABLED);

                    return VSConstants.S_OK;
                }
            }

            // All source control commands needs to be hidden and disabled when the provider is not active
            if (!SccService.Active)
            {
                cmdf = cmdf | OLECMDF.OLECMDF_INVISIBLE;
                cmdf = cmdf & ~(OLECMDF.OLECMDF_ENABLED);

                prgCmds[0].cmdf = (uint)cmdf;
                return VSConstants.S_OK;
            }
            bool lazyLoad = Preferences.LocalSettings.GetBool("LazyLoadStatus", false) && Preferences.LocalSettings.GetBool("LazyLoadFullMenu", false) && QueryIsLazyLoad();
            if (SccService.IsSolutionSelected || SccService.IsaControlledProjectSelected || lazyLoad)
            {
                if ((prgCmds[0].cmdID == CommandId.icmdStreamGraph) ||
                    (prgCmds[0].cmdID == CommandId.icmdScmMerge) ||
                    (prgCmds[0].cmdID == CommandId.icmdScmCopy))
                {
                    if (ClientStream.getStream() != null)
                    {
                        cmdf |= OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED;
                    }
                    else
                    {
                        cmdf |= OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE;
                    }
                }
                else if ((prgCmds[0].cmdID == CommandId.icmdAddToIgnoreList) ||
                    (prgCmds[0].cmdID == CommandId.icmdRemoveFromIgnoreList) ||
                    (prgCmds[0].cmdID == CommandId.icmdEditIgnoreList))
                {
                    if ((SccService == null) || (SccService.ScmProvider == null)
                        || (!P4ScmProvider.P4IgnoreSet) || (SccService.ScmProvider.ServerVersion < Versions.V12_1))
                    {
                        cmdf |= OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE;
                    }
                    else
                    {
                        cmdf |= OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED;
                    }
                }
                else
                {
                    cmdf |= OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED;
                }
                prgCmds[0].cmdf = (uint)cmdf;

                return VSConstants.S_OK;
            }

            if (!CommandsStatusUpdated)
            {
                GetCommandStatus();
            }

            bool isThereASolution = IsThereASolution();
            // Process our Commands
            switch (prgCmds[0].cmdID)
            {
                case CommandId.icmdScmRefresh:
                    if (isThereASolution)
                    {
                        cmdf |= OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED;
                    }
                    else
                    {
                        cmdf |= OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE;
                    }
                    break;

                case CommandId.icmdAddToSourceControl:
                    cmdf |= cmdf_icmdAddToSourceControl;
                    break;

                case CommandId.icmdCheckin:
                    cmdf |= cmdf_icmdCheckin;
                    break;

                case CommandId.icmdCheckout:
                    cmdf |= cmdf_icmdCheckout;
                    break;

                case CommandId.icmdCheckoutProject:
                    cmdf |= cmdf_icmdCheckoutProject;
                    break;

                case CommandId.icmdCheckoutSolution:
                    cmdf |= cmdf_icmdCheckoutSolution;
                    break;

                //case CommandId.icmdUseSccOffline:
                //    cmdf |= cmdf_icmdUseSccOffline;
                //    break;

                case CommandId.icmdP4V:
                    cmdf |= cmdf_icmdP4V;
                    break;

                case CommandId.icmdTimeLapse:
                    cmdf |= cmdf_icmdTimeLapse;
                    break;

                case CommandId.icmdRevGraph:
                    cmdf |= cmdf_icmdRevGraph;
                    break;

                case CommandId.icmdStreamGraph:
                    cmdf |= cmdf_icmdStreamGraph;
                    break;

                case CommandId.icmdReconcile:
                    cmdf |= cmdf_icmdReconcile;
                    break;

                case CommandId.icmdRevert:
                    cmdf |= cmdf_icmdRevert;
                    break;

                case CommandId.icmdRevertUnchanged:
                    cmdf |= cmdf_icmdRevertUnchanged;
                    break;

                case CommandId.icmdLock:
                    cmdf |= cmdf_icmdLock;
                    break;

                case CommandId.icmdUnlock:
                    cmdf |= cmdf_icmdUnlock;
                    break;

                case CommandId.icmdChangeFileType:
                    cmdf |= cmdf_icmdChangeFileType;
                    break;

                case CommandId.icmdMoveToChangelist:
                    cmdf |= cmdf_icmdMoveToChangelist;
                    break;

                case CommandId.icmdAddToIgnoreList:
                    cmdf |= cmdf_icmdAddToIgnoreList;
                    break;

                case CommandId.icmdRemoveFromIgnoreList:
                    cmdf |= cmdf_icmdRemoveFromIgnoreList;
                    break;

                case CommandId.icmdEditIgnoreList:
                    cmdf |= cmdf_icmdEditIgnoreList;
                    break;

                case CommandId.icmdResolve:
                    cmdf |= cmdf_icmdResolve;
                    break;

                case CommandId.icmdSync:
                    cmdf |= cmdf_icmdSync;
                    break;

                case CommandId.icmdSyncHead:
                    cmdf |= cmdf_icmdSyncHead;
                    break;

                case CommandId.icmdDiffVsHave:
                    cmdf |= cmdf_icmdDiffVsHave;
                    break;

                case CommandId.icmdDiffVsAny:
                    cmdf |= cmdf_icmdDiffVsAny;
                    break;

                //case CommandId.icmdAddProjectToSCC:
                //    cmdf |= cmdf_icmdAddProjectToSCC;
                //    break;

                case CommandId.icmdShowHistory:
                    cmdf |= cmdf_icmdShowHistory;
                    break;

                case CommandId.icmdShelve:
                    cmdf |= cmdf_icmdShelve;
                    break;

                //case CommandId.icmdScmAttributes:
                //    cmdf |= cmdf_icmdScmAttributes;
                //    break;

                case CommandId.icmdScmMerge:
                    cmdf |= cmdf_icmdScmMerge;
                    break;

                case CommandId.icmdScmCopy:
                    cmdf |= cmdf_icmdScmCopy;
                    break;

                case CommandId.cmdidCancelActiveCommand:
                    //	cmdf |= OLECMDF.OLECMDF_ENABLED;
                    if (KeepAliveMonitor.IsCommandRunning)
                    {
                        cmdf |= OLECMDF.OLECMDF_ENABLED;
                    }
                    break;

                case CommandId.cmdidCurrentStream:
                    if (string.IsNullOrEmpty(ClientStream.getStream()) == false)
                    {
                        cmdf |= OLECMDF.OLECMDF_ENABLED;
                    }
                    break;

                // These commands are always enabled when the provider is active

                case CommandId.icmdViewWorkspaceToolWindow:
                case CommandId.icmdViewHistoryToolWindow:
                case CommandId.icmdViewJobsToolWindow:
                //case CommandId.icmdToolWindowToolbarCommand:
                //case CommandId.icmdViewP4ToolWindow:
                case CommandId.icmdViewSubmittedChangelistsToolWindow:
                case CommandId.icmdViewPendingChangelistsToolWindow:
                case CommandId.icmdViewLabelsToolWindow:
                //				case CommandId.icmdViewSwarmToolWindow:
                //              case CommandId.icmdViewStreamsToolWindow:
                case CommandId.icmdFileOpenInDepot:
                case CommandId.icmdOpenConnection:
                case CommandId.icmdPublish:
                    cmdf |= OLECMDF.OLECMDF_ENABLED;
                    break;

                //case CommandId.icmdViewReviewsToolWindow:
                //case CommandId.icmdViewSwarmToolWindow:
                //    if (Preferences.LocalSettings.GetBool("EnableSwarm", false))
                //    {
                //        cmdf |= OLECMDF.OLECMDF_ENABLED;
                //    }
                //    else
                //    {
                //        cmdf |= OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE;
                //    }
                //    break;

                case CommandId.icmdViewStreamsToolWindow:
                    cmdf |= OLECMDF.OLECMDF_ENABLED;
                    break;

                default:
                    return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
            }

            prgCmds[0].cmdf = (uint)cmdf;

            return VSConstants.S_OK;
        }

        bool QueryIsLazyLoad()
        {
            if (!_isThereASolution)
            {
                return false;
            }

            IList<string> files = SccService.SelectedFiles;
            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.Test(SourceControlStatus.scsUnknown))
                    {
                        return true;
                    }
                }
            }
            return false;
        }




        OLECMDF QueryStatus_icmdCheckin()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;
            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status == SourceControlStatus.scsCheckedIn)
                    {
                        continue;
                    }

                    if (status.TestAny(SourceControlStatus.scsCheckedOut |
                        SourceControlStatus.scsIntegrated | SourceControlStatus.scsMoved))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdCheckout()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;
            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.TestNone(SourceControlStatus.scsUncontrolled | SourceControlStatus.scsCheckedOut |
                        SourceControlStatus.scsMoved | SourceControlStatus.scsNotOnDisk |
                        SourceControlStatus.scsDeletedAtHead))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdCheckoutProject()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;
            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.TestNone(SourceControlStatus.scsUncontrolled | SourceControlStatus.scsCheckedOut |
                        SourceControlStatus.scsMoved | SourceControlStatus.scsNotOnDisk |
                        SourceControlStatus.scsDeletedAtHead))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdCheckoutSolution()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;
            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.TestNone(SourceControlStatus.scsUncontrolled | SourceControlStatus.scsCheckedOut |
                        SourceControlStatus.scsMoved | SourceControlStatus.scsNotOnDisk |
                        SourceControlStatus.scsDeletedAtHead))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdAddToSourceControl()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<VSITEMSELECTION> sel = SccService.SelectedNodes;
            bool isSolutionSelected = SccService.IsSolutionSelected;
            Hashtable hash = SccService.SelectedHierarchies;

            // The command is enabled when the solution is selected and is uncontrolled yet
            // or when an uncontrolled project is selected
            if (isSolutionSelected)
            {
                SourceControlStatus status = SccService.GetFileStatus(GetSolutionFileName());
                if ((!SccService.IsProjectControlled(null)) || (status == SourceControlStatus.scsUncontrolled))
                {
                    return OLECMDF.OLECMDF_ENABLED;
                }
            }
            else
            {
                if (hash != null)
                {
                    foreach (IVsHierarchy pHier in hash.Keys)
                    {
                        if (!SccService.IsProjectControlled(pHier))
                        {
                            return OLECMDF.OLECMDF_ENABLED;
                        }
                    }
                }
                IList<string> files = SccService.SelectedFiles;
                if (files != null)
                {
                    foreach (string file in files)
                    {
                        SourceControlStatus status = SccService.GetFileStatus(file);
                        if (status.TestAny(SourceControlStatus.scsUncontrolled | SourceControlStatus.scsDeletedAtHead))
                        {
                            if (status.TestNone(SourceControlStatus.scsMarkedAdd | SourceControlStatus.scsIgnored))
                            {
                                return OLECMDF.OLECMDF_ENABLED;
                            }
                        }
                    }
                }
            }

            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdReconcile()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            // as long as there is a solution,
            // make reconcile visible
            return OLECMDF.OLECMDF_ENABLED;
        }
        OLECMDF QueryStatus_icmdRevert()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.Test(SourceControlStatus.scsCheckedIn))
                    {
                        continue;
                    }

                    if (status.TestAny(SourceControlStatus.scsCheckedOut |
                        SourceControlStatus.scsNeedsResolve |
                        SourceControlStatus.scsIntegrated |
                        SourceControlStatus.scsMoved) &&
                        status.TestNone(SourceControlStatus.scsBranched))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdRevertUnchanged()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);

                    if (status.TestAny(SourceControlStatus.scsCheckedOut |
                                        SourceControlStatus.scsNeedsResolve) &&
                        status.TestNone(SourceControlStatus.scsMarkedAdd |
                                        SourceControlStatus.scsBranched |
                                        SourceControlStatus.scsIntegrated |
                                        SourceControlStatus.scsMoved))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdLock()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status == SourceControlStatus.scsCheckedIn)
                    {
                        continue;
                    }

                    if (status.TestAny(SourceControlStatus.scsIntegrated |
                                        SourceControlStatus.scsCheckedOut) &&
                        status.TestNone(SourceControlStatus.scsMarkedAdd |
                                        SourceControlStatus.scsMoved |
                                        SourceControlStatus.scsBranched |
                                        SourceControlStatus.scsLocked))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdUnlock()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status == SourceControlStatus.scsCheckedIn)
                    {
                        continue;
                    }

                    if (status.TestAny(SourceControlStatus.scsIntegrated |
                                        SourceControlStatus.scsCheckedOut) &&
                        status.Test(SourceControlStatus.scsLockedSelf))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdChangeFileType()
        {
            if ((!_isThereASolution) || (SccService.SelectedFiles == null) || (SccService.SelectedFiles.Count <= 0))
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.TestNone(SourceControlStatus.scsUncontrolled | SourceControlStatusFlags.scsNotOnDisk))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdMoveToChangelist()
        {
            if ((!_isThereASolution) || (SccService.SelectedFiles == null) || (SccService.SelectedFiles.Count <= 0))
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.TestAny(SourceControlStatus.scsCheckedOut | SourceControlStatus.scsIntegrated |
                        SourceControlStatusFlags.scsMarkedAdd | SourceControlStatusFlags.scsMarkedDelete |
                        SourceControlStatusFlags.scsMoved))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdAddToIgnoreList()
        {
            if ((!_isThereASolution) || (SccService.SelectedFiles == null) ||
                (SccService.IsaControlledProjectSelected) || (SccService.IsSolutionSelected) ||
                (SccService.SelectedFiles.Count <= 0) || (P4ScmProvider.P4IgnoreSet == false) ||
                (SccService.ScmProvider.ServerVersion < Versions.V12_1))
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.Test(SourceControlStatus.scsUncontrolled) && status.TestNone(SourceControlStatus.scsIgnored))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdRemoveFromIgnoreList()
        {
            if ((!_isThereASolution) || (SccService.SelectedFiles == null) ||
                (SccService.IsaControlledProjectSelected) || (SccService.IsSolutionSelected) ||
                (SccService.SelectedFiles.Count <= 0) || (P4ScmProvider.P4IgnoreSet == false) ||
                (SccService.ScmProvider.ServerVersion < Versions.V12_1))
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }
            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.TestAll(SourceControlStatus.scsUncontrolled | SourceControlStatus.scsIgnored))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdEditIgnoreList()
        {
            if ((!_isThereASolution) || (SccService.SelectedFiles == null) ||
                (SccService.SelectedFiles.Count <= 0) || (P4ScmProvider.P4IgnoreSet == false) ||
                (SccService.ScmProvider.ServerVersion < Versions.V12_1))
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }
            return OLECMDF.OLECMDF_ENABLED;
        }

        OLECMDF QueryStatus_icmdResolve()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.Test(SourceControlStatus.scsNeedsResolve))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdSync()
        {
            if ((!_isThereASolution) || (SccService.SelectedFiles == null) || (SccService.SelectedFiles.Count <= 0))
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.TestNone(SourceControlStatus.scsUncontrolled |
                                        SourceControlStatus.scsMoved |
                                        SourceControlStatus.scsMarkedAdd |
                                        SourceControlStatus.scsBranched) |
                        status.Test(SourceControlStatus.scsNotOnDisk))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdSyncHead()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
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

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);

                    if (file.EndsWith("\\..."))
                    {
                        // it's a directory wildcard
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                    if (status.TestAny(SourceControlStatus.scsDeletedAtHead))
                    {
                        continue;
                    }

                    if (status.TestAny(SourceControlStatus.scsStale | SourceControlStatusFlags.scsNotOnDisk))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdDiffVsHave()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.TestNone(SourceControlStatus.scsUncontrolled |
                                            SourceControlStatus.scsMarkedAdd |
                                            SourceControlStatus.scsBranched |
                                            SourceControlStatusFlags.scsNotOnDisk))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdDiffVsAny()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status == SourceControlStatus.scsUncontrolled)
                    {
                        return OLECMDF.OLECMDF_INVISIBLE;
                    }

                    return OLECMDF.OLECMDF_ENABLED;
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdShowHistory()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files == null)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.TestNone(SourceControlStatus.scsUncontrolled |
                                        SourceControlStatusFlags.scsNotOnDisk |
                                        SourceControlStatus.scsMoved |
                                        SourceControlStatus.scsMarkedAdd |
                                        SourceControlStatus.scsBranched))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdShelve()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;

            if (files == null)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            if (files != null)
            {
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.TestAny(SourceControlStatus.scsCheckedOut |
                                        SourceControlStatus.scsIntegrated |
                                        SourceControlStatusFlags.scsMarkedAdd |
                                        SourceControlStatus.scsBranched |
                                        SourceControlStatusFlags.scsNeedsResolve |
                                        SourceControlStatus.scsMoved))
                    {
                        return OLECMDF.OLECMDF_ENABLED;
                    }
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdScmMerge()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;
            P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
            P4ScmProvider scm = P4VSService.ScmProvider;
            bool controlled = false;
            foreach (string file in files)
            {
                P4.FileMetaData fmd = scm.Fetch(file);
                if (fmd != null)
                {
                    controlled = true;
                    break;
                }
            }
            if (controlled == false)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            if ((ClientStream.exists().Equals(true)) &&
                (ClientStream.getStream() != null))
            {
                return OLECMDF.OLECMDF_ENABLED;
            }

            else
                return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdScmCopy()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;
            P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
            P4ScmProvider scm = P4VSService.ScmProvider;
            bool controlled = false;
            foreach (string file in files)
            {
                P4.FileMetaData fmd = scm.Fetch(file);
                if (fmd != null)
                {
                    controlled = true;
                    break;
                }
            }
            if (controlled == false)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }
            if (files == SccService.GetControlledProjects())
                ClientStream.exists();

            if ((ClientStream.exists().Equals(true)) &&
                (ClientStream.getStream() != null))
            {
                return OLECMDF.OLECMDF_ENABLED;
            }

            else
                return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdP4V()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;
            P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
            P4ScmProvider scm = P4VSService.ScmProvider;
            bool controlled = false;
            foreach (string file in files)
            {
                P4.FileMetaData fmd = scm.Fetch(file);
                if (fmd != null)
                {
                    controlled = true;
                    break;
                }
            }
            if (controlled == false)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            P4V.exists();

            if ((P4V.exists().Equals(true)) &&
                (P4V.version() >= 2010.2) &&
                (files != null) &&
                (files.Count > 0))
            {
                return OLECMDF.OLECMDF_ENABLED;
            }

            else
                return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdTimeLapse()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;
            P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
            P4ScmProvider scm = P4VSService.ScmProvider;
            bool controlled = false;
            foreach (string file in files)
            {
                P4.FileMetaData fmd = scm.Fetch(file);
                if (fmd != null)
                {
                    controlled = true;
                    break;
                }
            }
            if (controlled == false)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            if ((P4V.exists() == false) || (files == null) || (files.Count <= 0))
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            foreach (string file in files)
            {
                SourceControlStatus status = SccService.GetFileStatus(file);
                if (status.TestNone(SourceControlStatusFlags.scsMarkedAdd |
                                        SourceControlStatus.scsBranched |
                                        SourceControlStatus.scsMoved))
                {
                    return OLECMDF.OLECMDF_ENABLED;
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdRevGraph()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;
            P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
            P4ScmProvider scm = P4VSService.ScmProvider;
            bool controlled = false;
            foreach (string file in files)
            {
                P4.FileMetaData fmd = scm.Fetch(file);
                if (fmd != null)
                {
                    controlled = true;
                    break;
                }
            }
            if (controlled == false)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            if ((P4V.exists() == false) || (files == null) || (files.Count <= 0))
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            foreach (string file in files)
            {
                SourceControlStatus status = SccService.GetFileStatus(file);
                if (status.TestNone(SourceControlStatusFlags.scsMarkedAdd |
                                        SourceControlStatus.scsBranched |
                                        SourceControlStatus.scsMoved))
                {
                    return OLECMDF.OLECMDF_ENABLED;
                }
            }
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        OLECMDF QueryStatus_icmdStreamGraph()
        {
            if (!_isThereASolution)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            IList<string> files = SccService.SelectedFiles;
            P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));
            P4ScmProvider scm = P4VSService.ScmProvider;
            bool controlled = false;
            foreach (string file in files)
            {
                P4.FileMetaData fmd = scm.Fetch(file);
                if (fmd != null)
                {
                    controlled = true;
                    break;
                }
            }
            if (controlled == false)
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }

            if ((P4V.exists() == false) ||
                (ClientStream.exists() == false) ||
                (ClientStream.getStream() == null) ||
                (P4V.version() < 2011.1))
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }
            return OLECMDF.OLECMDF_ENABLED;
        }

    }
}
