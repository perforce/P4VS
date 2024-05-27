/*******************************************************************************

Copyright (c) 2011 Perforce Software, Inc.  All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1.  Redistributions of source code must retain the above copyright
	notice, this list of conditions and the following disclaimer.

2.  Redistributions in binary form must reproduce the above copyright
	notice, this list of conditions and the following disclaimer in the
	documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL PERFORCE SOFTWARE, INC. BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*******************************************************************************/

/*******************************************************************************
 * Name		: P4VsProviderService.cs
 *
 * Author	: Duncan Barbee <dbarbee@perforce.com>
 *
 * Description	: Implementation of P4VS Source Control Provider Service
 *
 ******************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using NLog;
using System.Windows.Forms;
using ScmFile = Perforce.P4.FileMetaData;
using FileMap = System.Collections.Generic.Dictionary<string, Perforce.P4.FileMetaData>;
using Perforce.P4Scm;
using EnvDTE;

namespace Perforce.P4VS
{
    [Guid("93C6B80C-A9E4-4F63-A605-51E7FCB9F906")]
    public partial class P4VsProviderService :
        IVsSccProvider,             // Required for provider registration with source control manager
        IVsSccManager2,             // Base source control functionality interface
        IVsSccManagerTooltip,       // Provide tooltips for source control items
        IVsSolutionEvents,          // We'll register for solution events, these are useful for source control
        IVsSolutionEvents2,
        IVsSccControlNewSolution,   // Adds a newly created solution automatically to source control.
        IVsQueryEditQuerySave2,     // Required to allow editing of controlled files 
        IVsTrackProjectDocumentsEvents2,  // Usefull to track project changes (add, renames, deletes, etc)
        IVsTrackProjectDocumentsEvents4,  // Usefull to track project changes (add, renames, deletes, etc)
        IVsSccGlyphs,               // Allows full customization of Source Control Glyphs - 32 bit builds
        IVsSccGlyphs2,              // Allows full customization of Source Control Glyphs - 64 bit builds
        IVsSelectionEvents,         // Notifies registered VSPackages of changes to the current selection, element value, or command UI context.
        IVsSccSolution,             // Interface that raises events related to a loaded solution and Scc.
        IVsSccPublish,              // Supports publishing of source code that is in a local repository.
        IDisposable,
        EnvDTE.SourceControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        // Whether the provider is active or not
        private bool active = false;
        // The service and source control provider
        internal P4VsProvider _P4VsProvider = null;
        // The cookie for solution events 
        private uint vsSolutionEventsCookie;
        // The cookie for project document events
        private uint tpdTrackProjectDocumentsCookie;
        // The cookie for monitoring selection events
        private uint tpdMonitorSelectionCookie;
        // The cookie for receiving broadcast message events
        //private uint tpdBroadcastMessageCookie;
#if VS2012
        Microsoft.VisualStudio.PlatformUI.ThemeChangedEventHandler ThemeChangedHandler = null;
#endif

        // The list of controlled projects hierarchies
        private Dictionary<IVsHierarchy, FileMap> controlledProjects = new Dictionary<IVsHierarchy, FileMap>();

        // Variable tracking whether the currently loading solution is controlled (during solution load or merge)
        private string _loadingControlledSolutionLocation = "";
        // The location of the currently controlled solution
        private string solutionLocation;

        // The list of files approved for in-memory edit
        private Hashtable approvedForInMemoryEdit = new Hashtable();

        private static int lastAccessedIndex = 0;

#if !VS2012
		internal static P4SolutionExplorer SolutionExplorer = null;
#endif

        private Dispatcher _uiDispatcher;
        public Dispatcher UiDispatcher { get { return _uiDispatcher; } }

        private P4ScmProvider _scmProvider = null;
        public P4ScmProvider ScmProvider
        {
            get
            {
                return _scmProvider;
            }
            internal set
            {
                _scmProvider = value;
            }
        }

        private void LogFunctionCall(string functionName)
        {
            logger.Trace("Fn {0}", functionName);
        }

        #region P4VsProvider Service initialization/unitialization

        public bool InitEventSubscriptions()
        {
            try
            {
                bool success = true;

                // Subscribe to solution events
                IVsSolution sol = (IVsSolution)_P4VsProvider.GetService(typeof(SVsSolution));
                sol.AdviseSolutionEvents(this, out vsSolutionEventsCookie);
                success &= VSConstants.VSCOOKIE_NIL != vsSolutionEventsCookie;

                // Subscribe to project documents
                IVsTrackProjectDocuments2 tpdService = (IVsTrackProjectDocuments2)_P4VsProvider.GetService(typeof(SVsTrackProjectDocuments));
                tpdService.AdviseTrackProjectDocumentsEvents(this, out tpdTrackProjectDocumentsCookie);
                success &= VSConstants.VSCOOKIE_NIL != tpdTrackProjectDocumentsCookie;

                // Subscribe to selection events
                IVsMonitorSelection MonitorSelectionService = (IVsMonitorSelection)_P4VsProvider.GetService(typeof(SVsShellMonitorSelection));
                MonitorSelectionService.AdviseSelectionEvents(this, out tpdMonitorSelectionCookie);
                success &= VSConstants.VSCOOKIE_NIL != tpdMonitorSelectionCookie;

                //// Subscribe to Nuget package manager events.
                //this.PackageInstalling += new VsPackageEventHandler(OnPackageInstalling);

#if VS2012
                ThemeChangedHandler = new ThemeChangedEventHandler(OnThemeChanged);
                VSColorTheme.ThemeChanged += ThemeChangedHandler;
#endif
                _uiDispatcher = Dispatcher.FromThread(System.Threading.Thread.CurrentThread);

                return success;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "InitEventSubscriptions");
                return false;
            }
        }

        public void DisposeEventSubscriptions()
        {
            // Unregister from receiving solution events
            if (VSConstants.VSCOOKIE_NIL != vsSolutionEventsCookie)
            {
                IVsSolution sol = (IVsSolution)_P4VsProvider.GetService(typeof(SVsSolution));
                sol.UnadviseSolutionEvents(vsSolutionEventsCookie);
                vsSolutionEventsCookie = VSConstants.VSCOOKIE_NIL;
            }

            // Unregister from receiving project documents
            if (VSConstants.VSCOOKIE_NIL != tpdTrackProjectDocumentsCookie)
            {
                IVsTrackProjectDocuments2 tpdService = (IVsTrackProjectDocuments2)_P4VsProvider.GetService(typeof(SVsTrackProjectDocuments));
                tpdService.UnadviseTrackProjectDocumentsEvents(tpdTrackProjectDocumentsCookie);
                tpdTrackProjectDocumentsCookie = VSConstants.VSCOOKIE_NIL;
            }

            // Unregister from receiving selection events
            if (VSConstants.VSCOOKIE_NIL != tpdMonitorSelectionCookie)
            {
                IVsMonitorSelection MonitorSelectionService = (IVsMonitorSelection)_P4VsProvider.GetService(typeof(SVsShellMonitorSelection));
                MonitorSelectionService.UnadviseSelectionEvents(tpdMonitorSelectionCookie);
                tpdMonitorSelectionCookie = VSConstants.VSCOOKIE_NIL;
            }
#if VS2012
            if (ThemeChangedHandler != null)
            {
                VSColorTheme.ThemeChanged -= ThemeChangedHandler;
            }
#endif
        }
        public bool RefreshEventSubscriptions()
        {
            DisposeEventSubscriptions();
            if ((ScmProvider != null) && (ScmProvider.Connected))
            {
                return InitEventSubscriptions();
            }
            return false;
        }

        public P4VsProviderService(P4VsProvider P4VsProvider)
        {
            Debug.Assert(null != P4VsProvider);
            logger.Trace("P4VsProviderService constructor");
            _P4VsProvider = P4VsProvider;

            bool success = InitEventSubscriptions();
            Debug.Assert(success);
        }

        public void Dispose()
        {
            DisposeEventSubscriptions();

            if (ScmProvider != null)
            {
                P4VsProvider.BroadcastNewConnection(null);
                ScmProvider.Dispose();
                ScmProvider = null;
            }
        }

        #endregion

        //--------------------------------------------------------------------------------
        // IVsSccPublish specific functions
        //--------------------------------------------------------------------------------
        #region IVsSccPublish interface functions



        async System.Threading.Tasks.Task IVsSccPublish.BeginPublishWorkflowAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            AddNewSolutionToSourceControl();
            IVsUIShell uiShell = (IVsUIShell)ScmProvider.SccService;
        }
        #endregion

        public event EventHandler AddedToSourceControl;

        public void SlnAddedToSourceControl()
        {
            AddedToSourceControl?.Invoke(this, EventArgs.Empty);
        }
        //--------------------------------------------------------------------------------
        // IVsSccProvider specific functions
        //--------------------------------------------------------------------------------
        #region IVsP4VsProvider interface functions

        // Called by the scc manager when the provider is activated. 
        // Make visible and enable if necessary scc related menu commands
        public int SetActive()
        {
            logger.Trace("Provider set active");

            active = true;
            _P4VsProvider.OnActiveStateChange();

            return VSConstants.S_OK;
        }

        // Called by the scc manager when the provider is deactivated. 
        // Hides and disable scc related menu commands
        public int SetInactive()
        {
            logger.Trace("Provider set inactive");

            active = false;
            _P4VsProvider.OnActiveStateChange();

            return VSConstants.S_OK;
        }

        public int AnyItemsUnderSourceControl(out int pfResult)
        {
            if ((!active) || (ScmProvider == null) || (ScmProvider.Connection.Disconnected))
            {
                pfResult = 0;
            }
            else
            {
                // Although the parameter is an int, it's in reality a BOOL value, so let's return 0/1 values
                pfResult = (ScmProvider.Count != 0) ? 1 : 0;
            }
            return VSConstants.S_OK;
        }

        #endregion

        //--------------------------------------------------------------------------------
        // IVsSccManager2 specific functions
        //--------------------------------------------------------------------------------
        #region IVsSccManager2 interface functions

        public int BrowseForProject(out string pbstrDirectory, out int pfOK)
        {
            // Obsolete method
            pbstrDirectory = null;
            pfOK = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int CancelAfterBrowseForProject()
        {
            // Obsolete method
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Returns whether the source control provider is fully installed
        /// </summary>
        public int IsInstalled(out int pbInstalled)
        {
            // All source control packages should always return S_OK and set pbInstalled to nonzero
            pbInstalled = 1;
            return VSConstants.S_OK;
        }

        //  		scsCheckedIn = 0x0000,
        //  		scsStale = 0x0001,
        //  		scsOtherCheckedOut = 0x0002,
        //  		scsCheckedOut = 0x0004,
        //  		scsLockedSelf = 0x0008,  \
        //										sccLocked
        //  		scsLockedOther = 0x0010, /
        //
        //  0	checked in
        //	1	checked in + stale
        //  2	other check out
        //  3	other check out + stale
        //  4	self check out
        //  5	self check out + stale
        //  6	self check out + other check out
        //  7	self check out + other check out + stale
        //  8	locked (can't be set without other check our or self check out)
        //  9	locked + stale (can't be set without other check our or self check out)
        //  10	locked + other check out (show as other check out)
        //  11  locked + other check out + stale (show as other check out + stale)
        //  12	locked + self check out (show as self check out)
        //  13	locked + self check out + stale (show as self check out + stale)
        //  14	locked + other check out + self check out
        //  15	locked + other check out + self check out + stale
        //
        //                 0  1  2  3   4  5  6  7   8   9   10  11  12  13  14  15
#if VS2010 || VS2012
        int[] glyphMap = { 1, 3, 6, 12, 2, 4, 9, 11, -1, -1, 22, 28, 18, 20, 10, 8 };
#else
		//                 0  1  2  3   4  5  6  7   8   9  10  11  12 13 14  15
		int[] glyphMap = { 1, 3, 6, 12, 2, 4, 9, 11, -1, -1, 6, 12, 2, 4, 10, 8 };
#endif
        /// <summary>
        /// Provide source control icons for the specified files and returns scc status of files
        /// </summary>
        /// <returns>The method returns S_OK if at least one of the files is controlled, S_FALSE if none of them are</returns>
        public int GetSccGlyph([In] int cFiles, [In] string[] rgpszFullPaths, [Out] VsStateIcon[] rgsiGlyphs, [Out] uint[] rgdwSccStatus)
        {
#if SHOW_ALL_GLYPHS
			String fileName = Path.GetFileName(rgpszFullPaths[0]);
			if (fileName.StartsWith("STATEICON_"))
			{
				string icon = Path.GetFileNameWithoutExtension(fileName);
				switch (icon)
				{
					case "STATEICON_NOSTATEICON":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_NOSTATEICON;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_NOTCONTROLLED;
						}
						break;
					case "STATEICON_CHECKEDIN":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_CHECKEDIN;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_CONTROLLED;
						}
						break;
					case "STATEICON_CHECKEDOUT":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_CHECKEDOUT;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)(__SccStatus.SCC_STATUS_CHECKEDOUT | SCC_STATUS_OUTBYUSER);
						}
						break;
					case "STATEICON_ORPHANED":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_ORPHANED;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_DELETED;
						}
						break;
					case "STATEICON_EDITABLE":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_EDITABLE;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_MODIFIED;
						}
						break;
					case "STATEICON_BLANK":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_BLANK;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_NOMERGE;
						}
						break;
					case "STATEICON_READONLY":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_READONLY;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_LOCKED;
						}
						break;
					case "STATEICON_DISABLED":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_DISABLED;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_PINNED;
						}
						break;
					case "STATEICON_CHECKEDOUTEXCLUSIVE":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_CHECKEDOUTEXCLUSIVE;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)(__SccStatus.SCC_STATUS_CHECKEDOUT | __SccStatus.SCC_STATUS_LOCKED | SCC_STATUS_OUTBYUSER);
						}
						break;
					case "STATEICON_CHECKEDOUTSHAREDOTHER":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_CHECKEDOUTSHAREDOTHER;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_CHECKEDOUT | (uint)__SccStatus.SCC_STATUS_SHARED;
						}
						break;
					case "STATEICON_CHECKEDOUTEXCLUSIVEOTHER":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_CHECKEDOUTEXCLUSIVEOTHER;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_CHECKEDOUT | (uint)__SccStatus.SCC_STATUS_LOCKED;
						}
						break;
					case "STATEICON_EXCLUDEDFROMSCC":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_EXCLUDEDFROMSCC;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_NOTCONTROLLED;
						}
						break;
					case "STATEICON_12":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_MAXINDEX;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_NOTCONTROLLED;
						}
						break;
					case "STATEICON_13":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_MAXINDEX + 1;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_NOTCONTROLLED;
						}
						break;
					case "STATEICON_14":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_MAXINDEX + 2;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_NOTCONTROLLED;
						}
						break;
					case "STATEICON_15":
						rgsiGlyphs[0] = VsStateIcon.STATEICON_MAXINDEX + 3;
						if (rgdwSccStatus != null)
						{
							rgdwSccStatus[0] = (uint)__SccStatus.SCC_STATUS_NOTCONTROLLED;
						}
						break;
				}
				return VSConstants.S_OK;
			}
#endif
            int ret = VSConstants.S_FALSE;

            for (int idx = 0; idx < cFiles; idx++)
            {
                bool LazyLoad = Preferences.LocalSettings.GetBool("LazyLoadStatus", true);

                if (LazyLoad && (ScmProvider != null) && (ScmProvider.Contains(rgpszFullPaths[idx]) == false))
                {
                    // We are Lazy loading, connected to a server, and don't have cached data for this file
                    rgsiGlyphs[idx] = (VsStateIcon)(37 + 16);
                    if (rgdwSccStatus != null)
                    {
                        rgdwSccStatus[idx] = (uint)__SccStatus.SCC_STATUS_NOTCONTROLLED;
                    }
                    continue;
                }

                // Return the icons and the status. While the status is a combination a flags, we'll return just values 
                // with one bit set, to make life easier for GetSccGlyphsFromStatus
                SourceControlStatus status = GetFileStatus(rgpszFullPaths[idx]);

                if (status != null && status.TestNone(SourceControlStatus.scsUncontrolled | SourceControlStatus.scsIgnored))
                {
                    if (status.Test(SourceControlStatus.scsNeedsResolve))
                    {
                        rgsiGlyphs[idx] = (VsStateIcon)13;
                        if (rgdwSccStatus != null)
                        {
                            rgdwSccStatus[idx] = (uint)__SccStatus.SCC_STATUS_NOMERGE;
                        }
                    }
                    else if (status.TestAny(SourceControlStatus.scsMarkedAdd | SourceControlStatus.scsBranched | SourceControlStatus.scsMoved))
                    {
                        rgsiGlyphs[idx] = (VsStateIcon)14;
                        if (rgdwSccStatus != null)
                        {
                            rgdwSccStatus[idx] = (uint)__SccStatus.SCC_STATUS_RESERVED_2;
                        }
                    }
                    else if (status.Test(SourceControlStatus.scsMarkedDelete))
                    {
                        rgsiGlyphs[idx] = (VsStateIcon)15;
                        if (rgdwSccStatus != null)
                        {
                            rgdwSccStatus[idx] = (uint)__SccStatus.SCC_STATUS_DELETED;
                        }
                    }
                    else if (status.Test(SourceControlStatus.scsDeletedAtHead))
                    {
                        rgsiGlyphs[idx] = (VsStateIcon)3;
                        if (rgdwSccStatus != null)
                        {
                            unchecked
                            {
                                rgdwSccStatus[idx] = (uint)__SccStatus.SCC_STATUS_INVALID;
                            }
                        }
                    }
                    else if (status.Test(SourceControlStatus.scsNotOnDisk))
                    {
                        rgsiGlyphs[idx] = 0;
                        if (rgdwSccStatus != null)
                        {
                            unchecked
                            {
                                rgdwSccStatus[idx] = (uint)__SccStatus.SCC_STATUS_INVALID;
                            }
                        }
                    }
                    else
                    {
                        int glyphIdx = status & 0x0007;

                        // displaying integrated status trumps displaying locked status
                        if (status.Test(SourceControlStatus.scsLocked) && status.TestNone(SourceControlStatus.scsIntegrated))
                        {
                            glyphIdx += 8;
                        }

                        rgsiGlyphs[idx] = (VsStateIcon)glyphMap[glyphIdx];

                        if (status.Test(SourceControlStatus.scsIntegrated) && (glyphIdx <= 3))
                        {
                            logger.Trace("hit a integrated file");
                            rgsiGlyphs[idx] += 32;
                        }

                        if (rgdwSccStatus != null)
                        {
                            __SccStatus stat = __SccStatus.SCC_STATUS_NOTCONTROLLED;

                            if (status.TestAll(SourceControlStatus.scsOutMultiple))
                            {
                                stat |= __SccStatus.SCC_STATUS_OUTMULTIPLE;
                            }
                            if (status.TestAny(SourceControlStatus.scsCheckedOut | SourceControlStatus.scsIntegrated))
                            {
                                stat |= __SccStatus.SCC_STATUS_CHECKEDOUT;
                            }
                            if (status.Test(SourceControlStatus.scsOtherCheckedOut))
                            {
                                stat |= __SccStatus.SCC_STATUS_OUTOTHER | __SccStatus.SCC_STATUS_OUTBYUSER;
                            }
                            if (status.Test(SourceControlStatus.scsLocked))
                            {
                                stat |= __SccStatus.SCC_STATUS_OUTEXCLUSIVE;
                            }
                            if (status.Test(SourceControlStatus.scsStale))
                                stat |= __SccStatus.SCC_STATUS_OUTOFDATE;

                            rgdwSccStatus[idx] = (uint)stat;
                        }
                    }
#if VS2010 || VS2012
                    if (BaseImageIdx != 0) unchecked
                        {
                            rgsiGlyphs[idx] = (rgsiGlyphs[idx] + 16);
                        }
#endif
                }
                else
                {
                    var anyControlledProject = AnyControlledProjectsContainingFile(rgpszFullPaths[idx]);
                    if (anyControlledProject)
					{
                        // If the file is not controlled, but is member of a controlled project, report the item as checked out (same as source control in VS2003 did)
                        // If the provider wants to have special icons for "pending add" files, the IVsSccGlyphs interface needs to be supported
                        rgsiGlyphs[idx] = VsStateIcon.STATEICON_DISABLED;

                        if (status != null && status.TestAll(SourceControlStatus.scsIgnored))
                        {
                            rgsiGlyphs[idx] = (VsStateIcon)21;
                        }
#if VS2010 || VS2012
                        if (BaseImageIdx != 0) unchecked
                            {
                                rgsiGlyphs[idx] = (rgsiGlyphs[idx] + 16);
                            }
#endif
                        if (rgdwSccStatus != null)
                        {
                            rgdwSccStatus[idx] = (uint)__SccStatus.SCC_STATUS_NOTCONTROLLED;
                        }
                    }
                    else
                    {
                        // This is an uncontrolled file, return a blank scc glyph for it
                        rgsiGlyphs[idx] = VsStateIcon.STATEICON_BLANK;
                        if (rgdwSccStatus != null)
                        {
                            rgdwSccStatus[idx] = (uint)__SccStatus.SCC_STATUS_NOTCONTROLLED;
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Determines the corresponding scc status glyph to display, given a combination of scc status flags
        /// </summary>
        public int GetSccGlyphFromStatus([In] uint dwSccStatus, [Out] VsStateIcon[] psiGlyph)
        {
            psiGlyph[0] = VsStateIcon.STATEICON_BLANK;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// One of the most important methods in a source control provider, is called by projects 
        /// that are under source control when they are first opened to register project settings
        /// </summary>
        public int RegisterSccProject([In] IVsSccProject2 pscp2Project, [In] string pszSccProjectName, [In] string pszSccAuxPath, [In] string pszSccLocalPath, [In] string pszProvider)
        {
            if (pszProvider.CompareTo(_P4VsProvider.ProviderName) != 0)
            {
                // If the provider name controlling this project is not our provider, the user may be adding to a 
                // solution controlled by this provider an existing project controlled by some other provider.
                // We'll deny the registration with scc in such case.
                //string projName = pscp2Project.

                IVsHierarchy hierProject = (IVsHierarchy)pscp2Project;
                object projName = null;
                int hr = hierProject.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_Name, out projName);
                string msg = string.Format(Resources.P4VsProviderService_UnderOtherSCMWarning, projName);
                if (DialogResult.Yes != MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.YesNo))
                {
                    return VSConstants.E_FAIL;
                }
                if (Preferences.LocalSettings.GetBool("TagSolutionProjectFiles", false))
                {
                    pscp2Project.SetSccLocation(string.Empty, string.Empty, string.Empty, _P4VsProvider.ProviderName);
                }
                else
                {
                    pscp2Project.SetSccLocation(string.Empty, string.Empty, string.Empty, string.Empty);
                }
            }
            if (pscp2Project == null)
            {
                // Manual registration with source control of the solution, from OnAfterOpenSolution
                logger.Trace("Solution {0} is registering with source control - {1}, {2}, {3}, {4}",
                    _P4VsProvider.GetSolutionFileName(), pszSccProjectName, pszSccAuxPath, pszSccLocalPath, pszProvider);

                IVsHierarchy solHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
                string solutionFile = _P4VsProvider.GetSolutionFileName();

                if (_scmProvider == null)
                {
                    _scmProvider = new P4ScmProvider(solutionFile, this);
                    _scmProvider.LoadingSolution = !string.IsNullOrEmpty(LoadingControlledSolutionLocation);
                }
                controlledProjects[solHier] = new FileMap();
            }
            else
            {
                logger.Trace("Project {0} is registering with source control - {1}, {2}, {3}, {4}",
                    _P4VsProvider.GetProjectFileName(pscp2Project), pszSccProjectName, pszSccAuxPath, pszSccLocalPath, pszProvider);

                // Add the project to the list of controlled projects
                IVsHierarchy hierProject = (IVsHierarchy)pscp2Project;
                controlledProjects[hierProject] = new FileMap();
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called by projects registered with the source control portion of the environment before they are closed. 
        /// </summary>
        public int UnregisterSccProject([In] IVsSccProject2 pscp2Project)
        {
            // Get the project's hierarchy
            IVsHierarchy hierProject = null;
            if (pscp2Project == null)
            {
                // If the project's pointer is null, it must be the solution calling to unregister, from OnBeforeCloseSolution
                logger.Trace("Solution {0} is unregistering with source control.", _P4VsProvider.GetSolutionFileName());
                hierProject = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));

                if (ScmProvider != null)
                {
                    ScmProvider.ClearCache();
                }
            }
            else
            {
                logger.Trace("Project {0} is unregistering with source control.", _P4VsProvider.GetProjectFileName(pscp2Project));
                hierProject = (IVsHierarchy)pscp2Project;
            }

            // Remove the project from the list of controlled projects
            if ((controlledProjects.ContainsKey(hierProject)) && (controlledProjects[hierProject] != null))
            {
                controlledProjects.Remove(hierProject);
                return VSConstants.S_OK;
            }
            else
            {
                return VSConstants.S_FALSE;
            }
        }

        #endregion

        //--------------------------------------------------------------------------------
        // IVsSccManagerTooltip specific functions
        //--------------------------------------------------------------------------------
        #region IVsSccManagerTooltip interface functions

        /// <summary>
        /// Called by solution explorer to provide tooltips for items. Returns a text describing the source control status of the item.
        /// </summary>
        public int GetGlyphTipText([In] IVsHierarchy phierHierarchy, [In] uint itemidNode, out string pbstrTooltipText)
        {
            // Initialize output parameters
            pbstrTooltipText = "";

            if ((ScmProvider == null) || (ScmProvider.Connection.Disconnected))
            {
                return VSConstants.S_OK;
            }

            IList<string> files = _P4VsProvider.GetNodeFiles(phierHierarchy, itemidNode);
            if (files.Count == 0)
            {
                return VSConstants.S_OK;
            }

            // Return the glyph text based on the first file of node (the master file)
            pbstrTooltipText = ScmProvider.GetToolTip(files[0]);

            return VSConstants.S_OK;
        }

        #endregion

        //--------------------------------------------------------------------------------
        // IVsSolutionEvents and IVsSolutionEvents2 specific functions
        //--------------------------------------------------------------------------------
        #region IVsSolutionEvents interface functions

        public int OnAfterCloseSolution([In] Object pUnkReserved)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            // Reset all source-control-related data now that solution is closed
            controlledProjects.Clear();
            _P4VsProvider.SolutionHasDirtyProps = false;
            LoadingControlledSolutionLocation = "";
            solutionLocation = "";
            approvedForInMemoryEdit.Clear();

            if (_P4VsProvider.SuppressConnection == false)
            {
                P4VsProvider.BroadcastNewConnection(null);
                if (ScmProvider != null)
                {
                    ScmProvider.Dispose();
                    ScmProvider = null;
                }
            }
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject([In] IVsHierarchy pStubHierarchy, [In] IVsHierarchy pRealHierarchy)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            AddCOntrolledProject(pRealHierarchy);

            string projectFile = _P4VsProvider.GetProjectFileName((IVsSccProject2)pRealHierarchy);

            RefreshProjectGlyphs(projectFile);

            ResetSelection();

            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject([In] IVsHierarchy pHierarchy, [In] int fAdded)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if ((ScmProvider == null) || (ScmProvider.Connection.Disconnected))
            {
                return VSConstants.S_OK;
            }
            if (CreatingNewSolution)
            {
                return VSConstants.S_OK;
            }
            if ((ScmProvider.LoadingSolution == false) && (fAdded == 1))
            {
                // If a solution folder is added to the solution after the solution is added to scc, 
                //  we need to control that folder
                if (_P4VsProvider.IsSolutionFolderProject(pHierarchy))
                {
                    IVsHierarchy solHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
                    if (IsProjectControlled(solHier))
                    {
                        // Register this solution folder using the same location as the solution
                        IVsSccProject2 pSccProject = (IVsSccProject2)pHierarchy;
                        RegisterSccProject(pSccProject, solutionLocation, "", "", _P4VsProvider.ProviderName);

                        // We'll also need to refresh the solution folders glyphs to reflect the controlled state
                        IList<VSITEMSELECTION> nodes = new List<VSITEMSELECTION>();

                        VSITEMSELECTION vsItem;
                        vsItem.itemid = VSConstants.VSITEMID_ROOT;
                        vsItem.pHier = pHierarchy;
                        nodes.Add(vsItem);

                        _P4VsProvider.Glyphs.RefreshNodesGlyphs(nodes, null);
                    }
                }
                else
                {
                    // this is a new project that has been added to the solution
                    IVsSccProject2 pSccProject2 = pHierarchy as IVsSccProject2;

                    if (pSccProject2 != null)
                    {
                        RegisterSccProject(pSccProject2, solutionLocation, "", "", _P4VsProvider.ProviderName);

                        // check to see if the project file is already in the cache
                        string projectFile = _P4VsProvider.GetProjectFileName(pSccProject2);
                        if ((ScmProvider.isFileIgnored(projectFile) == false) && (ScmProvider.IsFileCached(projectFile) == false))
                        {
                            bool TreatProjectsAsFolders = Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true);
                            bool LazyLoad = Preferences.LocalSettings.GetBool("LazyLoadStatus", true);
                            IList<string> projectFiles = null;

                            bool addit = true;
                            if (TreatProjectsAsFolders)
                            {
                                string path = Path.GetDirectoryName(projectFile);
                                path = path + "\\...";
                                projectFiles = new List<string>();
                                projectFiles.Add(path);
                                ScmProvider.UpdateFiles(projectFiles, true);

                                addit = ((ScmProvider != null) && ScmProvider.isFileIgnored(projectFile) == false) && (ScmProvider.IsFileCached(projectFile) == false);
                            }
                            if (addit)
                            {
                                projectFiles = _P4VsProvider.GetProjectFiles(pSccProject2);
                                if ((TreatProjectsAsFolders == false) && (LazyLoad == false))
                                {
                                    //if not already done,...
                                    ScmProvider.UpdateFiles(projectFiles, true);
                                }
                                bool autoAdd = Preferences.LocalSettings.GetBool("Auto_Add", true);

                                if ((autoAdd) && (LazyLoad == false))
                                {
                                    IList<string> addedFiles = new List<string>();

                                    foreach (string file in projectFiles)
                                    {
                                        if (ScmProvider.IsFileCached(file))
                                        {
                                            SourceControlStatus fileStautus = ScmProvider.GetFileStatus(file);
                                            if (fileStautus.Test(SourceControlStatusFlags.scsUncontrolled))
                                            {
                                                // only add uncontrolled files
                                                addedFiles.Add(file);
                                            }
                                        }
                                        else if ((ScmProvider.Connection.Repository != null) && (ScmProvider.Connection.Repository.Connection != null) &&
                                            (ScmProvider.Connection.Repository.Connection.Client != null) &&
                                            (ScmProvider.Connection.WorkspaceRoot != "null"))
                                        {
                                            string wsRoot = ScmProvider.Connection.WorkspaceRoot.Replace("/", "\\");

                                            if (file.StartsWith(wsRoot, StringComparison.OrdinalIgnoreCase))
                                            {
                                                // it's in the local workspace, so if it's not in the cache by now, 
                                                // it's a new file
                                                addedFiles.Add(file);
                                            }
                                        }
                                    }

                                    if (addedFiles.Count > 0)
                                    {
                                        // In non-UI mode just add the files using the default changelist
                                        int changeListId = 0;
                                        string newChangeDescription = null;
                                        if (_P4VsProvider.InCommandLineMode() == false)
                                        {
                                            IList<P4.Changelist> changes = ScmProvider.GetAvailibleChangelists(-1);
                                            changeListId = SelectChangelistDlg2.ShowChooseChangelistYesNo(Resources.P4VsProviderService_QueryAddFilesQuestion, addedFiles, changes, ref newChangeDescription);
                                            if (changeListId <= -2)
                                            {
                                                // user hit 'No'
                                                return VSConstants.S_OK;
                                            }

                                            if (string.IsNullOrEmpty(newChangeDescription))
                                            {
                                                newChangeDescription = Resources.P4VsProvider_AddFilesDefaultChangelistDescription;
                                            }

                                            changeListId = ScmProvider.AddFiles(changeListId, newChangeDescription, addedFiles.ToArray());
                                            if (changeListId < -1)
                                            {
                                                // the files could not be  added, so unregister the project
                                                UnregisterSccProject(pSccProject2);
                                                return VSConstants.S_OK;
                                            }
                                            SelectChangelistDlg.CurrentChangeList = changeListId;
                                        }
                                        ScmProvider.UpdateFiles(addedFiles, true);
                                    }
                                }
                            }
                            // Now that we know which files belong to this project, iterate the project files
                            if (LazyLoad == false)
                            {
                                RefreshProjectGlyphs(projectFiles, true);
                            }
                        }
                    }
                }
            }
            return VSConstants.S_OK;
        }

        public bool AsyncLoad = false;
        public string port = null;
        public string user = null;
        public string workspace = null;

        public int OnAfterOpenSolution([In] Object pUnkReserved, [In] int fNewSolution)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (CreatingNewSolution)
            {
                CreatingNewSolution = false;
                return VSConstants.S_OK;
            }
            string solutionFile = _P4VsProvider.GetSolutionFileName();

            if (string.IsNullOrEmpty(solutionFile))
            {
                // no solution file active
                return VSConstants.S_OK;
            }

            bool lazyLoading = Preferences.LocalSettings.GetBool("LazyLoadStatus", false);
            bool RegisterSolution = lazyLoading;
            // This event is fired last by the shell when opening a solution.
            // By this time, we have already loaded the solution persistence data from the PreLoad section
            // the controlled projects should be opened and registered with source control
            if ((LoadingControlledSolutionLocation.Length > 0) || (controlledProjects.Count > 0) || AsyncLoad)
            {
                if ((ScmProvider == null) || ScmProvider.Connection.Disconnected)
                {
                    _P4VsProvider.ConnectToScm(port, user, workspace);
                }

                if ((ScmProvider != null) && (ScmProvider.Connected))
                {
                    ScmProvider.SolutionFile = solutionFile;

                    if (string.IsNullOrEmpty(ScmProvider.SolutionFile))
                    {
                        // no solution file active
                        return VSConstants.S_OK;
                    }
                    if (lazyLoading && (_scmProvider.Contains(ScmProvider.SolutionFile) == false))
                    {
                        _scmProvider.AddFileToCache(ScmProvider.SolutionFile);
                    }
                    SourceControlStatus solutionFileScmStatus = _scmProvider.GetFileStatus(ScmProvider.SolutionFile);
                    if (((solutionFileScmStatus & SourceControlStatus.scsStale) != 0) &&
                        Preferences.LocalSettings.GetBool("PromptSlnSync", true))
                    {
                        IList<string> files = new List<string>();
                        files.Add(solutionFile);
                        DialogResult answer = FileListWarningDlg.Show(files, null, FileListWarningDlg.WarningStyle.UpdateSln);
                        if (answer == DialogResult.OK)
                        {
                            ScmProvider.SccService.SyncFiles(null, files);
                        }
                    }
                    if (SourceControlStatus.scsUncontrolled == solutionFileScmStatus)
                    {
                        P4.P4CommandResult results = null;
                        P4.FileMetaData fmd = _scmProvider.GetFileMetaData(null, _P4VsProvider.GetSolutionFileName(), out results);

                        if ((results != null) && (results.ErrorList != null) && (results.ErrorList.Count > 0) &&
                            ((results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
                            (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient) ||
                            (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_IntegMovedUnmapped) ||
                            (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_ExVIEW) ||
                            (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_ExVIEW2)))
                        {
                            if (Preferences.LocalSettings.GetBool("WarnSlnWorkspace", true))
                            {
                                if ((results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
(results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient))
                                {
                                    MessageBox.Show(
                                        Resources.P4VsProviderService_CantAddSlnClientRootError,
                                        Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                }
                                else
                                {
                                    MessageBox.Show(
    Resources.P4VsProviderService_AddSlnWantUpdateClientMapWarning,
    Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Question);
                                }
                            }
                        }
                        else
                        {
                            // this solution file is marked as controlled (loadingControlledSolutionLocation is not empty)
                            // or it contains controlled projects, but the solution file is not checked into the depot,
                            // so ask the user if he wants to add the solution file to the depot.
                            string fileName = Path.GetFileName(ScmProvider.SolutionFile);
                            if (fileName != null)
                            {
                                if (Preferences.LocalSettings.GetBool("Auto_Add", false) && !_P4VsProvider.InCommandLineMode())
                                {
                                    string newChangeDescription = null;
                                    string prompt = string.Format(Resources.P4VsProviderService_MarkAddSlnFileQuestion, fileName);
                                    IList<P4.Changelist> changes = ScmProvider.GetAvailibleChangelists(-1);

                                    int changeListId = SelectChangelistDlg.ShowChooseChangelist(prompt, changes, ref newChangeDescription);
                                    if (string.IsNullOrEmpty(newChangeDescription))
                                    {
                                        newChangeDescription = Resources.P4VsProvider_AddFilesDefaultChangelistDescription;
                                    }

                                    if (changeListId > -2)
                                    {
                                        changeListId = ScmProvider.AddFiles(changeListId, newChangeDescription, ScmProvider.SolutionFile);
                                        SelectChangelistDlg.CurrentChangeList = changeListId;

                                    }
                                    _P4VsProvider.SolutionHasDirtyProps = true;

                                    RegisterSolution = true;
                                }
                            }
                        }
                    }

                    else
                    {
                        if (string.IsNullOrEmpty(LoadingControlledSolutionLocation))
                        {
                            // solution file is in the depot, but it is not marked as being controlled
                            _P4VsProvider.SolutionHasDirtyProps = true;
                        }
                        RegisterSolution = true;
                    }
                }
            }
            else if ((ScmProvider != null) && (ScmProvider.Connected))
            {
                ScmProvider.SolutionFile = solutionFile;
                //Preferences.LocalSettings.GetBool("PromptSlnSync", true);
                SourceControlStatus solutionFileScmStatus = _scmProvider.GetFileStatus(ScmProvider.SolutionFile);
                if (((solutionFileScmStatus & SourceControlStatus.scsStale) != 0) &&
                    Preferences.LocalSettings.GetBool("PromptSlnSync", true))
                {
                    IList<string> files = new List<string>();
                    files.Add(solutionFile);
                    DialogResult answer = FileListWarningDlg.Show(files, null, FileListWarningDlg.WarningStyle.UpdateSln);
                    if (answer == DialogResult.OK)
                    {
                        ScmProvider.SccService.SyncFiles(null, files);
                    }
                }
                if (SourceControlStatus.scsUncontrolled != solutionFileScmStatus)
                {
                    if (SourceControlStatus.scsCheckedIn == solutionFileScmStatus)
                    {
                        int changeListId = 0;
                        string newChangeDescription = null;
                        if (!_P4VsProvider.InCommandLineMode())
                        {
                            IList<P4.Changelist> changes = ScmProvider.GetAvailibleChangelists(-1);

                            changeListId = -1;
                            string prompt = Resources.P4VsProviderService_RegisterSlnFileQuestion;
                            changeListId = SelectChangelistDlg2.ShowChooseChangelistYesNo(prompt,
                                new List<string> { ScmProvider.SolutionFile }, changes, ref newChangeDescription);
                        }
                        if (changeListId >= -1)
                        {
                            ScmProvider.CheckoutFiles(0, newChangeDescription, ScmProvider.SolutionFile);
                            // solution file is in the depot, but it is not marked as being controlled
                            _P4VsProvider.SolutionHasDirtyProps = true;

                            RegisterSolution = true;
                        }
                    }
                    else
                    {
                        // solution file is in the depot, but it is not marked as being controlled
                        _P4VsProvider.SolutionHasDirtyProps = true;

                        RegisterSolution = true;
                    }
                }
                else
                {
                    P4.P4CommandResult results = null;
                    P4.FileMetaData fmd = _scmProvider.GetFileMetaData(null, solutionFile, out results);

                    if ((results != null) && (results.ErrorList != null) && (results.ErrorList.Count > 0) &&
                        ((results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
                        (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient) ||
                        (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_IntegMovedUnmapped) ||
                        (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_ExVIEW) ||
                        (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_ExVIEW2)))
                    {
                        if (Preferences.LocalSettings.GetBool("WarnSlnWorkspace", true))
                        {
                            if ((results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
                            (results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient))
                            {
                                MessageBox.Show(
                                    Resources.P4VsProviderService_CantAddSlnClientRootError,
                                    Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show(Resources.P4VsProviderService_AddSlnWantUpdateClientMapWarning,
Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        // solution file is in the current workspace but not checked into the depo
                        _P4VsProvider.SolutionHasDirtyProps = true;

                        RegisterSolution = true;
                    }
                }
            }
            if (RegisterSolution)
            {
                // We'll also need to refresh the solution glyphs to reflect the controlled state
                IList<VSITEMSELECTION> nodes = new List<VSITEMSELECTION>();

                // If the solution was controlled, now it is time to register the solution hierarchy with source control, too.
                // Note that solution is not calling RegisterSccProject(), the scc package will do this call as it knows the source control location
                RegisterSccProject(null, LoadingControlledSolutionLocation, "", "", _P4VsProvider.ProviderName);

                VSITEMSELECTION vsItem;
                vsItem.itemid = VSConstants.VSITEMID_ROOT;
                vsItem.pHier = null;
                nodes.Add(vsItem);

                // Also, solution folders won't call RegisterSccProject, so we have to enumerate them and register them with scc once the solution is controlled
                Hashtable enumSolFolders = _P4VsProvider.GetSolutionFoldersEnum();
                foreach (IVsHierarchy pHier in enumSolFolders.Keys)
                {
                    // Register this solution folder using the same location as the solution
                    string folderName;
                    pHier.GetCanonicalName(VSConstants.VSITEMID_ROOT, out folderName);

                    IVsSccProject2 pSccProject = pHier as IVsSccProject2;
                    if (pSccProject != null)
                    {
                        RegisterSccProject(pSccProject, LoadingControlledSolutionLocation, "", "", _P4VsProvider.ProviderName);
                        vsItem.itemid = VSConstants.VSITEMID_ROOT;
                        vsItem.pHier = pHier;
                        nodes.Add(vsItem);
                    }
                    else
                    {
                        FileLogger.LogMessage(3, "P4VsServiceProvider",
                            string.Format("Can't register solution folder, {0}", folderName));
                    }
                }

                Hashtable hashUncontrolledProjects = new Hashtable();

                // When the solution is selected, all the uncontrolled projects in the solution will be added to scc
                Hashtable hash = _P4VsProvider.GetLoadedControllableProjectsEnum();

                foreach (IVsHierarchy pHier in hash.Keys)
                {
                    if (!IsProjectControlled(pHier))
                    {
                        hashUncontrolledProjects[pHier] = true;
                    }
                }
                if (hashUncontrolledProjects.Count > 0)
                {
                    foreach (IVsHierarchy pHier in hashUncontrolledProjects.Keys)
                    {
                        IVsSccProject2 sccProject2 = (IVsSccProject2)pHier;
                        IList<string> files = _P4VsProvider.GetProjectFiles(sccProject2);

                        RegisterSccProject(sccProject2, LoadingControlledSolutionLocation, "", "", _P4VsProvider.ProviderName);

                        RefreshProjectGlyphs(files, true);
                    }
                }

                // Refresh the glyphs now for solution and solution folders
                _P4VsProvider.Glyphs.RefreshNodesGlyphs(nodes, null);
            }

            solutionLocation = LoadingControlledSolutionLocation;

            // reset the flag now that solution open completed
            LoadingControlledSolutionLocation = "";

            _P4VsProvider.ClientStream.clearExists();

#if !VS2012
			if (SolutionExplorer != null)
			{
				SolutionExplorer.Dispose();
			}
			SolutionExplorer = new P4SolutionExplorer();
			SolutionExplorer.Capture();

#endif
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject([In] IVsHierarchy pHierarchy, [In] int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution([In] Object pUnkReserved)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            // Since we registered the solution with source control from OnAfterOpenSolution, it would be nice to unregister it, too, when it gets closed.
            // Also, unregister the solution folders
            Hashtable enumSolFolders = _P4VsProvider.GetSolutionFoldersEnum();
            foreach (IVsHierarchy pHier in enumSolFolders.Keys)
            {
                IVsSccProject2 pSccProject = pHier as IVsSccProject2;
                if (pSccProject != null)
                {
                    UnregisterSccProject(pSccProject);
                }
            }

            UnregisterSccProject(null);

            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject([In] IVsHierarchy pRealHierarchy, [In] IVsHierarchy pStubHierarchy)
        {
            if (controlledProjects.ContainsKey(pRealHierarchy))
            {
                controlledProjects.Remove(pRealHierarchy);
            }
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject([In] IVsHierarchy pHierarchy, [In] int fRemoving, [In] ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution([In] Object pUnkReserved, [In] ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject([In] IVsHierarchy pRealHierarchy, [In] ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterMergeSolution([In] Object pUnkReserved)
        {
            // reset the flag now that solutions were merged and the merged solution completed opening
            LoadingControlledSolutionLocation = "";

            return VSConstants.S_OK;
        }

        #endregion

        //--------------------------------------------------------------------------------
        // IVsQueryEditQuerySave2 specific functions
        //--------------------------------------------------------------------------------
        #region IVsQueryEditQuerySave2 interface functions

        public int BeginQuerySaveBatch()
        {
            return VSConstants.S_OK;
        }

        public int EndQuerySaveBatch()
        {
            return VSConstants.S_OK;
        }

        public int DeclareReloadableFile([In] string pszMkDocument, [In] uint rgf, [In] VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo)
        {
            return VSConstants.S_OK;
        }

        public int DeclareUnreloadableFile([In] string pszMkDocument, [In] uint rgf, [In] VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo)
        {
            return VSConstants.S_OK;
        }

        public int IsReloadable([In] string pszMkDocument, out int pbResult)
        {
            // Since we're not tracking which files are reloadable and which not, consider everything reloadable
            pbResult = 1;
            return VSConstants.S_OK;
        }

        public int OnAfterSaveUnreloadableFile([In] string pszMkDocument, [In] uint rgf, [In] VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo)
        {
            return VSConstants.S_OK;
        }

        DateTime projectQueryEditTime = DateTime.MinValue;
        /// <summary>
        /// Called by projects and editors before modifying a file
        /// The function allows the source control systems to take the necessary actions (checkout, flip attributes)
        /// to make the file writable in order to allow the edit to continue
        ///
        /// There are a lot of cases to deal with during QueryEdit/QuerySave. 
        /// - called in command line mode, when UI cannot be displayed
        /// - called during builds, when save shouldn't probably be allowed
        /// - called during projects migration, when projects are not open and not registered yet with source control
        /// - checking out files may bring new versions from vss database which may be reloaded and the user may lose in-memory changes; some other files may not be reloadable
        /// - not all editors call QueryEdit when they modify the file the first time (buggy editors!), and the files may be already dirty in memory when QueryEdit is called
        /// - files on disk may be modified outside IDE and may have attributes incorrect for their scc status
        /// - checkouts may fail
        /// The sample provider won't deal with all these situations, but a real source control provider should!
        /// </summary>
        public int QueryEditFiles([In] uint rgfQueryEdit, [In] int cFiles, [In] string[] rgpszMkDocuments, [In] uint[] rgrgf, [In] VSQEQS_FILE_ATTRIBUTE_DATA[] rgFileInfo, out uint pfEditVerdict, out uint prgfMoreInfo)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);

            // Initialize output variables
            pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
            prgfMoreInfo = 0;
            bool notAtHead = false;

            // Check if this is triggered on a project file and if so, is it the 2nd + time
            // it has been called in less than 20 seconds. If so, return.
            if (rgpszMkDocuments.Length > 0 && rgpszMkDocuments[0].EndsWith("proj", StringComparison.OrdinalIgnoreCase))
            {
                TimeSpan sinceLastProjectQuery = DateTime.Now.Subtract(projectQueryEditTime);
                if (sinceLastProjectQuery.TotalSeconds < 20)
                {
                    return VSConstants.S_OK;
                }
                projectQueryEditTime = DateTime.Now;
            }

            List<string> rgpszMkDocumentsList = new List<string>();
            rgpszMkDocumentsList = rgpszMkDocuments.ToList();

            RefreshProjectGlyphs(rgpszMkDocumentsList, true);
            // If C++ project file in list, include .filters file as well.
            foreach (string rgpszMkDocument in rgpszMkDocumentsList)
            {
                if (rgpszMkDocument.EndsWith(".vcxproj", StringComparison.OrdinalIgnoreCase))
                {
                    string filterFile = rgpszMkDocument + ".filters";
                    if (File.Exists(filterFile))
                    {
                        rgpszMkDocumentsList.Add(filterFile);
                        cFiles++;
                        break;
                    }
                }
            }

            if ((ScmProvider == null) || (ScmProvider.Connection.Disconnected))
            {
                return VSConstants.S_OK;
            }
            // In non-UI mode just allow the edit, because the user cannot be asked what to do with the file
            if (_P4VsProvider.InCommandLineMode())
            {
                return VSConstants.S_OK;
            }
            // make sure we were sent files to check
            if (cFiles == 0)
            {
                //no files to check
                return VSConstants.S_OK;
            }
            try
            {
                bool allowInMemeoryEdit =
                    ((rgfQueryEdit & (uint)tagVSQueryEditFlags.QEF_DisallowInMemoryEdits) == 0);

                uint fEditVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                uint fMoreInfo = 0;

                // work with all files, even if not in the solution
                for (int iFile = 0; iFile < cFiles; iFile++)
                {
                    // Because of the way we calculate the status, it is not possible to have a 
                    // checked in file that is writable on disk, or a checked out file that is read-only on disk
                    // A source control provider would need to deal with those situations, too
                    SourceControlStatus status = GetFileStatus(rgpszMkDocumentsList[iFile]);
                    bool fileExists = File.Exists(rgpszMkDocumentsList[iFile]);
                    bool isFileReadOnly = false;
                    if (fileExists)
                    {
                        isFileReadOnly = ((File.GetAttributes(rgpszMkDocumentsList[iFile]) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);

                        if (isFileReadOnly && ((rgfQueryEdit & (uint)tagVSQueryEditFlags.QEF_ReportOnly) == 0) &&
                            status.TestAny(SourceControlStatusFlags.scsUncontrolled | SourceControlStatusFlags.scsUnknown) && (Preferences.LocalSettings.GetBool("LazyLoadStatus", false)))
                        {
                            // we are lazy loading status file is read only but  shows as uncontrolled, 
                            //  so we likely haven't loaded the status, so do so now
                            _scmProvider.UpdateFileInCache(rgpszMkDocumentsList[iFile], true);
                            status = GetFileStatus(rgpszMkDocumentsList[iFile]);
                        }
                    }
                    if (isFileReadOnly && status.Test(SourceControlStatusFlags.scsCheckedOut))
                    {
                        _scmProvider.UpdateFileInCache(rgpszMkDocumentsList[iFile], false);
                        status = GetFileStatus(rgpszMkDocumentsList[iFile]);
                    }
                    // Allow the edits if the file does not exist or is writable
                    if ((!fileExists || !isFileReadOnly) &&
                        status.TestNone(SourceControlStatus.scsIntegrated) &&
                        ((!Preferences.LocalSettings.GetBool("CheckoutWriteable", false)) ||
                        ((rgfQueryEdit & (uint)tagVSQueryEditFlags.QEF_SilentMode) == 0)))
                    {
                        fEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
                    }
                    else if ((rgfQueryEdit & (uint)tagVSQueryEditFlags.QEF_ReportOnly) != 0)
                    {
                        if ((status.Test(SourceControlStatus.scsCheckedOut)) ||
                            approvedForInMemoryEdit.ContainsKey(rgpszMkDocumentsList[iFile].ToLower()))
                        {
                            fEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
                        }
                    }
                    else
                    {
                        // If the IDE asks about a file that was already approved for in-memory edit, allow the edit without asking the user again
                        if (approvedForInMemoryEdit.ContainsKey(rgpszMkDocumentsList[iFile].ToLower()))
                        {
                            fEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
                            fMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_InMemoryEdit);
                        }
                        else
                        {
                            // refresh the file status before checkout in case it is
                            // at an older revision.
                            _scmProvider.UpdateFileInCache(rgpszMkDocumentsList[iFile], true);
                            status = GetFileStatus(rgpszMkDocumentsList[iFile]);

                            if (status.TestAll(SourceControlStatusFlags.scsStale))
                            {
                                notAtHead = true;
                            }
                            if (status.Test(SourceControlStatus.scsCheckedIn))
                            {
                                // not checked out

                                //                        if (allowInMemeoryEdit && (rgpszMkDocumentsList[iFile] != null) &&
                                //                             (rgpszMkDocumentsList[iFile].EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) ||
                                //                             rgpszMkDocumentsList[iFile].EndsWith(".vcxproj", StringComparison.OrdinalIgnoreCase) ||
                                //                             rgpszMkDocumentsList[iFile].EndsWith(".vcxproj.filters", StringComparison.OrdinalIgnoreCase)))
                                //                        {
                                //                            // project file, approve for in memory edit so as not to continuously pester the user
                                //                            //  User will be prompted to checkout the file if it needs to be saved
                                //                            fEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
                                //                            fMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_InMemoryEdit);
                                //                            // Add the file to the list of files approved for edit, so if the IDE asks again about this file, we'll allow the edit without asking the user again
                                //                            // UNDONE: Currently, a file gets removed from _approvedForInMemoryEdit list only when the solution is closed. Consider intercepting the 
                                //                            // IVsRunningDocTableEvents.OnAfterSave/OnAfterSaveAll interface and removing the file from the approved list after it gets saved once.

                                //                            // Don't allow the in memory edit if the project is being edited
                                //                            // for changes to nuget packages or packages.config Those edits
                                //                            // are not saved in a way that would trigger a check out.
                                //                            approvedForInMemoryEdit[rgpszMkDocumentsList[iFile].ToLower()] = true;

                                //                        }
                                //else if ((rgfQueryEdit & (uint)tagVSQueryEditFlags.QEF_ReportOnly) != 0)
                                if ((rgfQueryEdit & (uint)tagVSQueryEditFlags.QEF_ReportOnly) != 0)
                                {
                                    fMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_EditNotPossible | tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc);
                                }
                                else
                                {
                                    int changeList = 0;
                                    string newChangelistDescription = null;

                                    int answer = SelectChangelistDlg.Checkout;

                                    if ((rgfQueryEdit & (uint)tagVSQueryEditFlags.QEF_SilentMode) != 0)
                                    {
                                        // When called in silent mode, attempt the checkout
                                        // (The alternative is to deny the edit and return QER_NoisyPromptRequired and expect for a non-silent call)
                                        fEditVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                                        fMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc | tagVSQueryEditResultFlags.QER_NoisyPromptRequired);
                                    }

                                    // check for exclusively opened file
                                    if (ScmProvider.IsFileExclusiveOpened(rgpszMkDocumentsList[iFile]))
                                    {
                                        // Show the error and set EditNotOK for this file. Do not show
                                        // the select changelist dialog.
                                        P4ErrorDlg.Show(Resources.P4VsProvider_ExclusiveOpenError, false, false);
                                        answer = SelectChangelistDlg.Cancel;
                                        pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                                    }
                                    else
                                    {
                                        IList<P4.Changelist> changes = ScmProvider.GetAvailibleChangelists(-1);
                                        answer = SelectChangelistDlg.ShowQueryEdit(rgpszMkDocumentsList[iFile], changes, allowInMemeoryEdit, out changeList, out newChangelistDescription);
                                    }

                                    if (answer == SelectChangelistDlg.Checkout)
                                    {
                                        if (notAtHead)
                                        {
                                            if (DialogResult.Yes ==
                                                QueryEditSyncWarningDlg.Show(Path.GetFileName(rgpszMkDocumentsList[iFile])))
                                            {
                                                // user wants out of date file synced
                                                ScmProvider.SyncFiles(null, new string[] { rgpszMkDocumentsList[iFile] });
                                            }
                                        }
                                        // Checkout the file, and since it cannot fail, allow the edit

                                        int changeId = CheckoutFileAndRefreshProjectGlyphs(rgpszMkDocumentsList[iFile], changeList, newChangelistDescription);
                                        if (changeList == -1)
                                        {
                                            // a new changelist was created, so update the last changelist in the SelectChangelistDlg
                                            SelectChangelistDlg.LastQueryEditChangelistId = changeId;
                                        }
                                        fEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
                                        fMoreInfo = (uint)tagVSQueryEditResultFlags.QER_MaybeCheckedout;
                                        // Do not forget to set QER_Changed if the content of the file on disk changes during the query edit
                                        // Do not forget to set QER_Reloaded if the source control reloads the file from disk after such changing checkout.
                                    }
                                    else if (answer == SelectChangelistDlg.EditInMemory)
                                    {
                                        // Allow edit in memory
                                        fEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
                                        fMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_InMemoryEdit);
                                        // Add the file to the list of files approved for edit, so if the IDE asks again about this file, we'll allow the edit without asking the user again
                                        // UNDONE: Currently, a file gets removed from _approvedForInMemoryEdit list only when the solution is closed. Consider intercepting the 
                                        // IVsRunningDocTableEvents.OnAfterSave/OnAfterSaveAll interface and removing the file from the approved list after it gets saved once.
                                        approvedForInMemoryEdit[rgpszMkDocumentsList[iFile].ToLower()] = true;
                                    }
                                    else
                                    {
                                        fEditVerdict = (uint)tagVSQueryEditResult.QER_NoEdit_UserCanceled;
                                        fMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc |
                                            tagVSQueryEditResultFlags.QER_CheckoutCanceledOrFailed);
                                        break;
                                    }
                                }
                                // It's a bit unfortunate that we have to return only one set of flags for all the files involved in the operation
                                // The edit can continue if all the files were approved for edit
                                prgfMoreInfo |= fMoreInfo;
                                pfEditVerdict |= fEditVerdict;
                            }
                            else if (status.TestAny(SourceControlStatus.scsCheckedOut | SourceControlStatus.scsUncontrolled | SourceControlStatus.scsIgnored))
                            {
                                if (fileExists && isFileReadOnly)
                                {
                                    if ((rgfQueryEdit & (uint)tagVSQueryEditFlags.QEF_ReportOnly) != 0)
                                    {
                                        fMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_EditNotPossible | tagVSQueryEditResultFlags.QER_ReadOnlyNotUnderScc);
                                    }
                                    else
                                    {
                                        bool fChangeAttribute = false;
                                        if ((rgfQueryEdit & (uint)tagVSQueryEditFlags.QEF_SilentMode) != 0)
                                        {
                                            // When called in silent mode, deny the edit and return QER_NoisyPromptRequired and expect for a non-silent call)
                                            // (The alternative is to silently make the file writable and accept the edit)
                                            fMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_EditNotPossible | tagVSQueryEditResultFlags.QER_ReadOnlyNotUnderScc | tagVSQueryEditResultFlags.QER_NoisyPromptRequired);
                                        }
                                        else
                                        {
                                            // This is a controlled file, warn the user
                                            IVsUIShell uiShell = (IVsUIShell)_P4VsProvider.GetService(typeof(SVsUIShell));
                                            Guid clsid = Guid.Empty;
                                            int result = VSConstants.S_OK;
                                            string messageText = Resources.QEQS_EditUncontrolledReadOnly;
                                            string messageCaption = Resources.ProviderName;
                                            if (uiShell.ShowMessageBox(0, ref clsid,
                                                                messageCaption,
                                                                String.Format(CultureInfo.CurrentUICulture, messageText, rgpszMkDocumentsList[iFile]),
                                                                string.Empty,
                                                                0,
                                                                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                                                                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                                                                OLEMSGICON.OLEMSGICON_QUERY,
                                                                0,        // false = application modal; true would make it system modal
                                                                out result) == VSConstants.S_OK
                                                && result == (int)DialogResult.Yes)
                                            {
                                                fChangeAttribute = true;
                                            }
                                        }

                                        if (fChangeAttribute)
                                        {
                                            // Make the file writable and allow the edit
                                            File.SetAttributes(rgpszMkDocumentsList[iFile], FileAttributes.Normal);
                                            fEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
                                        }
                                    }
                                }
                                else
                                {
                                    fEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
                                }
                                // It's a bit unfortunate that we have to return only one set of flags for all the files involved in the operation
                                // The edit can continue if all the files were approved for edit
                                prgfMoreInfo |= fMoreInfo;
                                pfEditVerdict |= fEditVerdict;
                            }
                        }
                    }
                }
                // It's a bit unfortunate that we have to return only one set of flags for all the files involved in the operation
                // The edit can continue if all the files were approved for edit
                prgfMoreInfo |= fMoreInfo;

                // Mask QER_InMemoryEdit.
                prgfMoreInfo &= ~(uint)(tagVSQueryEditResultFlags.QER_InMemoryEdit);

                pfEditVerdict |= fEditVerdict;
            }
            catch (Exception)
            {
                // If an exception was caught, do not allow the edit
                pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                prgfMoreInfo = (uint)tagVSQueryEditResultFlags.QER_EditNotPossible;
            }
            // Maybe don't need to update glyphs here. 
            // If they were checked out, they were already updated
#if DEBUG
            if (pfEditVerdict == (uint)tagVSQueryEditResult.QER_EditOK)
            {
                string msg = string.Format("Edit approved for {0}", rgpszMkDocumentsList[0]);
                P4VsOutputWindow.AppendMessage(msg);
            }
            else
            {
                string msg = string.Format("Edit denied for {0}", rgpszMkDocumentsList[0]);
                P4VsOutputWindow.AppendMessage(msg);
            }
#endif
            return VSConstants.S_OK;
        }


        /// <summary>
        /// Called by editors and projects before saving the files
        /// The function allows the source control systems to take the necessary actions (checkout, flip attributes)
        /// to make the file writable in order to allow the file saving to continue
        /// </summary>
        public int QuerySaveFile([In] string pszMkDocument, [In] uint rgf, [In] VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo, out uint pdwQSResult)
        {
            // Delegate to the other QuerySave function
            string[] rgszDocuements = new string[1];
            uint[] rgrgf = new uint[1];
            rgszDocuements[0] = pszMkDocument;
            rgrgf[0] = rgf;
            return QuerySaveFiles(((uint)tagVSQuerySaveFlags.QSF_DefaultOperation), 1, rgszDocuements, rgrgf, pFileInfo, out pdwQSResult);
        }

        /// <summary>
        /// Called by editors and projects before saving the files
        /// The function allows the source control systems to take the necessary actions (checkout, flip attributes)
        /// to make the file writable in order to allow the file saving to continue
        /// </summary>
        public int QuerySaveFiles([In] uint rgfQuerySave, [In] int cFiles, [In] string[] rgpszMkDocuments, [In] uint[] rgrgf, [In] VSQEQS_FILE_ATTRIBUTE_DATA[] rgFileInfo, out uint pdwQSResult)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            // Initialize output variables
            // It's a bit unfortunate that we have to return only one set of flags for all the files involved in the operation
            // The last file will win setting this flag
            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;

            if ((ScmProvider == null) || (ScmProvider.Connection.Disconnected))
            {
                return VSConstants.S_OK;
            }

            // make sure we were sent files to check
            if (cFiles == 0)
            {
                //no files to check
                return VSConstants.S_OK;
            }

            // In non-UI mode attempt to silently flip the attributes of files or check them out 
            // and allow the save, because the user cannot be asked what to do with the file
            if (_P4VsProvider.InCommandLineMode())
            {
                rgfQuerySave = rgfQuerySave | (uint)tagVSQuerySaveFlags.QSF_SilentMode;
            }
            try
            {
                for (int iFile = 0; iFile < cFiles; iFile++)
                {
                    SourceControlStatus status = GetFileStatus(rgpszMkDocuments[iFile]);
                    bool fileExists = File.Exists(rgpszMkDocuments[iFile]);
                    bool isFileReadOnly = false;
                    if (fileExists)
                    {
                        isFileReadOnly = ((File.GetAttributes(rgpszMkDocuments[iFile]) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
                        if (isFileReadOnly && status.Test(SourceControlStatusFlags.scsUncontrolled) &&
                            (Preferences.LocalSettings.GetBool("LazyLoadStatus", false)))
                        {
                            // we are lazy loading status file is read only but shows as uncontrolled, 
                            //  so we likely haven't loaded the status, so do so now
                            _scmProvider.UpdateFileInCache(rgpszMkDocuments[iFile], true);
                            status = GetFileStatus(rgpszMkDocuments[iFile]);
                        }
                    }

                    if (status.Test(SourceControlStatus.scsCheckedIn))
                    {
                        // If the preference is set, prompt or auto-checkout on save
                        // of writeable file (likely writable by allwrite or +w).
                        // To do that, skip this check that would allow the save
                        // without checkout.
                        if (!Preferences.LocalSettings.GetBool("CheckoutWriteable", false))
                        {
                            // Allow the edits if the file does not exist or is writable
                            if ((!fileExists || !isFileReadOnly) && status.TestNone(SourceControlStatus.scsIntegrated))
                            {
                                pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
                                break;
                            }
                        }
                        int changeList = 0;
                        string newChangelistDescription = null;

                        int answer = SelectChangelistDlg.Checkout;

                        if ((rgfQuerySave & (uint)tagVSQuerySaveFlags.QSF_SilentMode) != 0)
                        {
                            // When called in silent mode, attempt the checkout
                            // (The alternative is to deny the save, return QSR_NoSave_NoisyPromptRequired and expect for a non-silent call)
                            answer = SelectChangelistDlg.Checkout;
                        }
                        else
                        {
                            IList<P4.Changelist> changes = ScmProvider.GetAvailibleChangelists(-1);
                            answer = SelectChangelistDlg.ShowQuerySave(rgpszMkDocuments[iFile], changes, out changeList, out newChangelistDescription);
                        }

                        switch (answer)
                        {
                            case SelectChangelistDlg.Checkout:
                                // Checkout the file, and since it cannot fail, allow the save to continue
                                int changeId = CheckoutFileAndRefreshProjectGlyphs(rgpszMkDocuments[iFile], changeList, newChangelistDescription);
                                if (changeList == -1)
                                {
                                    // this was a new changelist so update the last changelist in the selectchangelist dialog
                                    SelectChangelistDlg.LastQuerySaveChangelistId = changeId;
                                }
                                pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
                                break;
                            case SelectChangelistDlg.SaveAs:
                                pdwQSResult = (uint)tagVSQuerySaveResult.QSR_ForceSaveAs;
                                break;
                            case SelectChangelistDlg.Ignore:
                                pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_Continue;
                                break;
                            default:
                                pdwQSResult = SelectChangelistDlg.Cancel;
                                break;
                        }
                    }
                    else if (status.Test(SourceControlStatus.scsCheckedOut))
                    {
                        if (fileExists && isFileReadOnly)
                        {
                            // something is not right for it to be checked out
                            // and read only, refresh the file and try this
                            // whole process again
                            RefreshProjectGlyphs(rgpszMkDocuments[iFile]);
                            QuerySaveFiles(rgfQuerySave, cFiles, rgpszMkDocuments, rgrgf, rgFileInfo, out pdwQSResult);
                            break;
                        }
                        // Allow the save now 
                        pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // If an exception was caught, do not allow the save
                pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_Cancel;
            }
            // Maybe don't need to update glyphs here. 
            // If it was checked out, the glyph was already updated. Otherwise,
            // this is just a local save, so it likely did not effect the scm state 

#if DEBUG
            if (pdwQSResult == (uint)tagVSQuerySaveResult.QSR_SaveOK)
            {
                string msg = string.Format("Save approved for {0}", rgpszMkDocuments[0]);
                P4VsOutputWindow.AppendMessage(msg);
            }
            else
            {
                string msg = string.Format("Save denied for {0}", rgpszMkDocuments[0]);
                P4VsOutputWindow.AppendMessage(msg);
            }
#endif
            return VSConstants.S_OK;
        }

        #endregion

        //--------------------------------------------------------------------------------
        // IVsTrackProjectDocumentsEvents2 specific functions
        //--------------------------------------------------------------------------------
        #region IVsTrackProjectDocumentsEvents2 interface functions

        public int OnQueryAddFiles([In] IVsProject pProject, [In] int cFiles, [In] string[] rgpszMkDocuments, [In] VSQUERYADDFILEFLAGS[] rgFlags, [Out] VSQUERYADDFILERESULTS[] pSummaryResult, [Out] VSQUERYADDFILERESULTS[] rgResults)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            // make sure we were sent files to check
            if (cFiles == 0)
            {
                //no files to check
                return VSConstants.S_OK;
            }
            try
            {
                for (int idx = 0; idx < cFiles; idx++)
                {
                    if ((ScmProvider != null) && (ScmProvider.Connected))
                    {
                        ScmProvider.PreloadFile(rgpszMkDocuments[idx]);
                    }
                    if (rgResults != null)
                    {
                        rgResults[idx] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK;
                    }
                    deletedOrExcludedFiles.Remove(rgpszMkDocuments[idx]);
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogException("P4VsProviderService.OnQueryAddFiles", ex);
            }
            pSummaryResult[0] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Implement this function to update the project scc glyphs when the items are added to the project.
        /// If a project doesn't call GetSccGlyphs as they should do (as solution folder do), this will update correctly the glyphs when the project is controled
        /// </summary>
        public int OnAfterAddFilesEx([In] int cProjects, [In] int cFiles, [In] IVsProject[] rgpProjects, [In] int[] rgFirstIndices, [In] string[] rgpszMkDocuments, [In] VSADDFILEFLAGS[] rgFlags)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            // make sure we were sent files to check
            if (cFiles == 0)
            {
                //no files to check
                return VSConstants.S_OK;
            }
            bool autoAdd = Preferences.LocalSettings.GetBool("Auto_Add", true);
            if ((autoAdd == false) || (ScmProvider == null) || (ScmProvider.Connection.Disconnected))
            {
                return VSConstants.S_OK;
            }
            try
            {
                string ignoreFileName = P4ScmProvider.P4Ignore;

                // Start by iterating through all projects calling this function
                for (int iProject = 0; iProject < cProjects; iProject++)
                {
                    IVsSccProject2 sccProject = rgpProjects[iProject] as IVsSccProject2;
                    IVsHierarchy pHier = rgpProjects[iProject] as IVsHierarchy;

                    // If the project is not controllable, or is not controlled, skip it
                    if (sccProject == null || !IsProjectControlled(pHier))
                    {
                        continue;
                    }

                    // Files in this project are in rgszMkOldNames, rgszMkNewNames arrays starting with iProjectFilesStart index and ending at iNextProjecFilesStart-1
                    int iProjectFilesStart = rgFirstIndices[iProject];
                    int iNextProjecFilesStart = cFiles;
                    if (iProject < cProjects - 1)
                    {
                        iNextProjecFilesStart = rgFirstIndices[iProject + 1];
                    }

                    // In non-UI mode just add the files using the default changelist
                    int changeListId = 0;
                    string newChangeDescription = null;
                    IList<string> addedFiles = null;
                    if (_P4VsProvider.InCommandLineMode() == false)
                    {
                        addedFiles = new List<string>();
                        for (int iFile = iProjectFilesStart; iFile < iNextProjecFilesStart; iFile++)
                        {
                            if (Path.GetFileName(rgpszMkDocuments[iFile]) == ignoreFileName)
                            {
                                // ignore files are handled by their own code
                                continue;
                            }

                            // check for file opened in IDE but not part of solution or projects
                            IVsExternalFilesManager vsExternalFilesManager =
                (IVsExternalFilesManager)_P4VsProvider.GetService(typeof(SVsExternalFilesManager));

                            try
                            {
                                vsExternalFilesManager.GetExternalFilesProject(out IVsProject vsProject);
                                vsProject.IsDocumentInProject(rgpszMkDocuments[iFile], out int pfFound,
                                    new VSDOCUMENTPRIORITY[1], out uint pitemid);
                                if (Convert.ToBoolean(pfFound))
                                {
                                    // skip file that is in the hidden, Miscellaneous
                                    // Files project
                                    continue;
                                }
                            }
                            catch
                            {
                                // likely that the IVsExternalFilesManager was unable to be obtained,
                                // or the Miscellaneous Files project was not returned.
                            }

                            SourceControlStatus fileStatus = ScmProvider.GetFileStatus(rgpszMkDocuments[iFile]);
                            logger.Trace("File: " + rgpszMkDocuments[iFile].ToString() + " status: " + fileStatus.GetType().ToString());

                            if ((rgpszMkDocuments[iFile].StartsWith(ScmProvider.Connection.WorkspaceRoot.Replace("/", "\\"),
                                StringComparison.CurrentCultureIgnoreCase))
                                && (fileStatus.Test(SourceControlStatusFlags.scsUncontrolled)
                                && fileStatus.TestNone(SourceControlStatus.scsIgnored))
                                || fileStatus.Test(SourceControlStatusFlags.scsDeletedAtHead))
                            {
                                // only add uncontrolled files or files that need to be
                                // re-added because they were deleted at head rev.
                                logger.Trace("Adding file for p4 add: " + rgpszMkDocuments[iFile].ToString() + " status: " + fileStatus.GetType());
                                addedFiles.Add(rgpszMkDocuments[iFile]);
                            }
                        }
                        if (addedFiles.Count <= 0)
                        {
                            return VSConstants.S_OK;
                        }
                        IList<P4.Changelist> changes = ScmProvider.GetAvailibleChangelists(-1);

                        string prompt = Resources.P4VsProviderService_QueryAddFilesQuestion;
                        changeListId = SelectChangelistDlg2.ShowChooseChangelistYesNo(prompt, addedFiles, changes, ref newChangeDescription);
                        if (string.IsNullOrEmpty(newChangeDescription))
                        {
                            newChangeDescription = Resources.P4VsProvider_AddFilesDefaultChangelistDescription;
                        }

                        if (changeListId <= -2)
                        {
                            // user hit 'No'
                            return VSConstants.S_OK;
                        }

                        changeListId = ScmProvider.AddFiles(changeListId, newChangeDescription, addedFiles.ToArray());
                        if (changeListId <= -2)
                        {
                            // there was an error adding the file, so abort
                            break;
                        }
                        SelectChangelistDlg.CurrentChangeList = changeListId;
                    }

                    // Now that we know which files belong to this project, iterate the project files
                    ScmProvider.UpdateFiles(addedFiles, true);
                    RefreshProjectGlyphs(addedFiles, true);
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogException("P4VsProviderService.OnAfterAddFilesEx", ex);
            }
            return VSConstants.S_OK;
        }

        public int OnQueryAddDirectories([In] IVsProject pProject, [In] int cDirectories, [In] string[] rgpszMkDocuments, [In] VSQUERYADDDIRECTORYFLAGS[] rgFlags, [Out] VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, [Out] VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterAddDirectoriesEx([In] int cProjects, [In] int cDirectories, [In] IVsProject[] rgpProjects, [In] int[] rgFirstIndices, [In] string[] rgpszMkDocuments, [In] VSADDDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        private IDictionary<string, int> FilesApprovedForDelete = new Dictionary<string, int>();

        public List<string> deletedOrExcludedFiles = new List<string>();
        /// <summary>
        /// Implement OnQueryRemoveFiles event to warn the user when he's deleting controlled files.
        /// The user gets the chance to cancel the file removal.
        /// </summary>
        public int OnQueryRemoveFiles([In] IVsProject pProject, [In] int cFiles, [In] string[] rgpszMkDocuments, [In] VSQUERYREMOVEFILEFLAGS[] rgFlags, [Out] VSQUERYREMOVEFILERESULTS[] pSummaryResult, [Out] VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            // don't prevent deletion of files, deal with them post-delete
            // in OnAfterRemoveFiles
            return VSConstants.E_NOTIMPL;
        }

        public void OnAfterRemoveFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgFlags)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);

            // if the file has only been removed from the project (not deleted)
            // do nothing and return
            foreach (uint flag in rgFlags)
            {
                if (flag >= 16)
                    return;
            }

            // if the user has disabled this prompt, just return
            if (!Preferences.LocalSettings.GetBool("PromptForDelete", true))
            {
                return;
            }

            // make sure we were sent files to check
            if (cFiles == 0)
            {
                //no files to check
                return;
            }
            // also make sure there is a valid provider and connection
            if ((ScmProvider == null) || ScmProvider.Connection == null || (ScmProvider.Connection.Disconnected))
            {
                return;
            }
            try
            {
                int changeListId = -1;
                string newChangeDescription = null;
                List<string> deleteFiles = new List<string>();
                List<string> revertFiles = new List<string>();
                List<string> revertThenDeleteFiles = new List<string>();
                List<string> selectedFiles = new List<string>();

                foreach (string file in rgpszMkDocuments)
                {
                    // If the file was not deleted and not part of the controlled project,
                    // it is likely a file that is under the workspace root, opened in
                    // the current instance of Visual Studio, but not deleted. This event
                    // was triggered by the closing of that file's tab in Visual Studio.
                    // Continue.
                    if (File.Exists(file) &&
                        !_P4VsProvider.GetSelectedFilesInControlledProjects().Contains(file))
                    {
                        continue;
                    }

                    // if the file is not in the workspace and workspace root is not set to "null", continue
                    if ( !ScmProvider.Connection.WorkspaceRoot.Equals("null") && !file.StartsWith(ScmProvider.Connection.WorkspaceRoot, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    SourceControlStatus status = ScmProvider.GetFileStatus(file);

                    // if the file is not controlled, continue
                    if (status == SourceControlStatus.scsUncontrolled)
                    {
                        continue;
                    }

                    if (status == SourceControlStatus.scsCheckedOut ||
                        status == SourceControlStatus.scsOutMultiple)
                    {
                        revertThenDeleteFiles.Add(file);
                    }

                    P4.FileMetaData fmd = _scmProvider.GetFileMetaData(file);

                    // check the current source control status of the file to
                    // determine which list it belongs in
                    if (fmd == null)
                    {
                        continue;
                    }
                    if (fmd.Action == P4.FileAction.Add || fmd.Action == P4.FileAction.MoveAdd)
                    {
                        revertFiles.Add(file);
                    }
                    else
                    {
                        deleteFiles.Add(file);
                    }
                }

                selectedFiles.AddRange(deleteFiles);
                selectedFiles.AddRange(revertFiles);

                // at this point, all files have been evaluated. If there
                // are none in the list, return
                if (selectedFiles.Count == 0)
                {
                    return;
                }

                string prompt = Resources.P4VsProviderService_QueryDeleteFileQuestion;
                if (deleteFiles.Count >= 2)
                {
                    prompt = Resources.P4VsProviderService_QueryDeleteFilesQuestion;
                }
                if (revertFiles.Count > 0)
                {
                    prompt += Resources.P4VsProviderService_QueryDeleteFilesRevertAddNote;
                }

                IList<P4.Changelist> changes = ScmProvider.GetAvailibleChangelists(-1);
                changeListId = SelectChangelistDlg2.ShowChooseChangelistYesNo(prompt, selectedFiles, changes, ref newChangeDescription);

                if (changeListId <= -3)
                {
                    // user hit cancel
                    return;
                }
                if (changeListId == -2)
                {
                    // user hit no
                    return;
                }
                else
                {
                    if (changeListId == -1)
                    {
                        // create new changelist
                        if (string.IsNullOrEmpty(newChangeDescription))
                        {
                            newChangeDescription = Resources.P4VsProvider_CheckoutFilesDefaultChangelistDescription;
                        }

                        P4.Changelist change = ScmProvider.Connection.Repository.NewChangelist();
                        // Make sure files is empty. If default has files in it, a new changelist
                        // will automatically get those files.
                        change.Files = new List<P4.FileMetaData>();

                        change.Description = newChangeDescription;
                        change = ScmProvider.SaveChangelist(change, null);
                        changeListId = change.Id;
                    }
                }

                deletedOrExcludedFiles.AddRange(selectedFiles);

                revertFiles.AddRange(revertThenDeleteFiles);

                if (revertFiles.Count > 0)
                {
                    ScmProvider.RevertFiles(false, false, null, revertFiles.ToArray());
                }

                if (deleteFiles.Count > 0)
                {
                    ScmProvider.DeleteFiles(changeListId, newChangeDescription, deleteFiles.ToArray());
                }

                // Update file state here or files that were deleted and are re-added with the same
                // name never get added to p4.
                ScmProvider.UpdateFiles(selectedFiles, true);
            }
            catch (Exception ex)
            {
                FileLogger.LogException("P4VsProviderService.OnAfterRemoveFiles", ex);
            }

            return;

        }
        public int OnAfterRemoveFiles([In] int cProjects, [In] int cFiles, [In] IVsProject[] rgpProjects, [In] int[] rgFirstIndices, [In] string[] rgpszMkDocuments, [In] VSREMOVEFILEFLAGS[] rgFlags)
        {
            // removing this in favor of OnAfterRemoveFilesEx
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryRemoveDirectories([In] IVsProject pProject, [In] int cDirectories, [In] string[] rgpszMkDocuments, [In] VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, [Out] VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, [Out] VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterRemoveDirectories([In] int cProjects, [In] int cDirectories, [In] IVsProject[] rgpProjects, [In] int[] rgFirstIndices, [In] string[] rgpszMkDocuments, [In] VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryRenameFiles([In] IVsProject pProject, [In] int cFiles, [In] string[] rgszMkOldNames, [In] string[] rgszMkNewNames, [In] VSQUERYRENAMEFILEFLAGS[] rgFlags, [Out] VSQUERYRENAMEFILERESULTS[] pSummaryResult, [Out] VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            // make sure we were sent files to check
            if (cFiles == 0)
            {
                //no files to check
                return VSConstants.S_OK;
            }
            if ((ScmProvider == null) || (ScmProvider.Connection.Disconnected))
            {
                return VSConstants.S_OK;
            }
            try
            {
                if ((Preferences.LocalSettings.GetBool("LazyLoadStatus", false)) && (ScmProvider.Contains(rgszMkOldNames[0]) == false))
                {
                    ScmProvider.AddFileToCache(rgszMkOldNames[0]);
                }
                P4.FileMetaData fmd = ScmProvider.Fetch(rgszMkOldNames[0]);

                // to get the project file path, though we are not using it yet.
                IVsSccProject2 p = (IVsSccProject2)pProject;
                string project = _P4VsProvider.GetProjectFileName(p);

                P4.ServerMetaData info = ScmProvider.GetServerMetaData();

                if (ScmProvider.ServerVersion < Versions.V9_2 || info.MoveEnabled == false)
                {
                    if (fmd != null && fmd.Action != P4.FileAction.None)
                    {
                        MessageBox.Show(Resources.P4VsProviderService_QueryRenameCheckedinWarning);
                        pSummaryResult[0] = VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameNotOK;
                        rgResults = pSummaryResult;
                    }

                    if (fmd != null && fmd.Action == P4.FileAction.None && (fmd.HeadAction == P4.FileAction.Delete ||
                        fmd.HeadAction == P4.FileAction.DeleteInto || fmd.HeadAction == P4.FileAction.DeleteFrom ||
                        fmd.HeadAction == P4.FileAction.MoveDelete))
                    {
                        MessageBox.Show(Resources.P4VsProviderService_QueryRenameDeletedWarning);
                        pSummaryResult[0] = VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameNotOK;
                        rgResults = pSummaryResult;
                    }
                }

                else if (fmd != null && fmd.Action == P4.FileAction.None && (fmd.HeadAction != P4.FileAction.Delete &&
                    fmd.HeadAction != P4.FileAction.DeleteInto && fmd.HeadAction != P4.FileAction.DeleteFrom &&
                    fmd.HeadAction != P4.FileAction.MoveDelete))
                {
                    // just go ahead and check it out to the default changelist
                    // make sure to check out all files
                    for (int idx = 0; idx < rgszMkOldNames.Count(); idx++)
                    {

                        CheckoutFile(rgszMkOldNames[idx], 0, Resources.P4VsProviderService_QueryRenameDefaultChangelistDecription);
                    }
                }

                if (fmd != null && fmd.Action == P4.FileAction.None && (fmd.HeadAction == P4.FileAction.Delete ||
                    fmd.HeadAction == P4.FileAction.DeleteInto || fmd.HeadAction == P4.FileAction.DeleteFrom ||
                    fmd.HeadAction == P4.FileAction.MoveDelete))
                {
                    MessageBox.Show(Resources.P4VsProviderService_QueryRenameDeletedWarning);
                    pSummaryResult[0] = VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameNotOK;
                    rgResults = pSummaryResult;
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogException("P4VsProviderService.OnQueryRenameFiles", ex);
            }
            return VSConstants.S_OK;
        }

        private delegate bool CheckInFilesDelegate(IList<string> files);

        /// <summary>
        /// Implement OnAfterRenameFiles event to rename a file in the source control store when it gets renamed in the project
        /// Also, rename the store if the project itself is renamed
        /// </summary>
        public int OnAfterRenameFiles([In] int cProjects, [In] int cFiles, [In] IVsProject[] rgpProjects, [In] int[] rgFirstIndices, [In] string[] rgszMkOldNames, [In] string[] rgszMkNewNames, [In] VSRENAMEFILEFLAGS[] rgFlags)
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            // make sure we were sent files to check
            if (cFiles == 0)
            {
                //no files to check
                return VSConstants.S_OK;
            }
            if ((ScmProvider == null) || (ScmProvider.Connection.Disconnected))
            {
                return VSConstants.S_OK;
            }
            try
            {
                // Start by iterating through all projects calling this function
                for (int iProject = 0; iProject < cProjects; iProject++)
                {
                    IVsSccProject2 sccProject = rgpProjects[iProject] as IVsSccProject2;
                    IVsHierarchy pHier = rgpProjects[iProject] as IVsHierarchy;

                    // If the project is not controllable, or is not controlled, skip it
                    if (sccProject == null || !IsProjectControlled(pHier))
                    {
                        continue;
                    }

                    string projectName = null;
                    if (sccProject == null)
                    {
                        // This is the solution calling
                        pHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
                        projectName = _P4VsProvider.GetSolutionFileName();
                    }
                    else
                    {
                        if (sccProject == null)
                        {
                            // It is a project that doesn't support source control, in which case it should be ignored
                            continue;
                        }

                        projectName = _P4VsProvider.GetProjectFileName(sccProject);
                    }

                    // Files in this project are in rgszMkOldNames, rgszMkNewNames arrays starting with iProjectFilesStart index and ending at iNextProjecFilesStart-1
                    int iProjectFilesStart = rgFirstIndices[iProject];
                    int iNextProjecFilesStart = cFiles;
                    if (iProject < cProjects - 1)
                    {
                        iNextProjecFilesStart = rgFirstIndices[iProject + 1];
                    }
                    if (ScmProvider != null)
                    {
                        int changeListId = 0;
                        string newChangeDescription = null;

                        P4.FileMetaData fmd = ScmProvider.Fetch(rgszMkOldNames[0]);
                        if (fmd == null)
                        {
                            continue;
                        }

                        // is it a move?
                        if (Path.GetFileName(rgszMkOldNames[0]) == Path.GetFileName(rgszMkNewNames[0]))
                        {
                            int count = rgszMkNewNames.Count();
                            string summary = string.Format(
                                Resources.P4VsProviderService_AfterMoveDefaultSummary,
                                count);
                            if (count == 1)
                            {
                                summary = string.Format(
                                    Resources.P4VsProviderService_AfterMove1FileSummary,
                                    rgszMkOldNames[0], rgszMkNewNames[0]);
                                newChangeDescription = string.Format(
                                    Resources.P4VsProviderService_AfterMove1FileSummary,
                                    rgszMkOldNames[0], rgszMkNewNames[0]);
                            }
                            else
                            {
                                for (int movedFiles = 0; movedFiles < rgszMkNewNames.Count(); movedFiles++)
                                {
                                    newChangeDescription = newChangeDescription + string.Format(
                                    Resources.P4VsProviderService_AfterMove1FileSummary,
                                    rgszMkOldNames[movedFiles], rgszMkNewNames[movedFiles]) + "\n\t";
                                }
                            }


                            IList<P4.Changelist> changes = ScmProvider.GetAvailibleChangelists(-1);
                            changeListId = SelectChangelistDlg.ShowChooseChangelistMove(summary, changes, ref newChangeDescription, _scmProvider);

                            if (changeListId == -2)
                            {
                                // user hit 'No'
                                return VSConstants.S_OK;
                            }

                            P4.Changelist changeToSubmit = ScmProvider.Connection.Repository.NewChangelist();
                            if (changeListId == -1)
                            {
                                changeToSubmit.Description = newChangeDescription;
                                changeToSubmit = ScmProvider.SaveChangelist(changeToSubmit, null);
                                changeListId = changeToSubmit.Id;
                            }
                            if (changeListId > 0)
                            {
                                changeToSubmit = ScmProvider.GetChangelist(changeListId);
                                changeToSubmit.Description = newChangeDescription;
                                changeToSubmit = ScmProvider.SaveChangelist(changeToSubmit, null);
                                changeListId = changeToSubmit.Id;
                            }

                            ScmProvider.RenameFile(rgszMkOldNames, rgszMkNewNames, changeListId);
                            ScmProvider.UpdateFiles(rgszMkOldNames, true);
                            ScmProvider.UpdateFiles(rgszMkNewNames, true);
                        }

                        // is it a rename?
                        if (Path.GetFileName(rgszMkOldNames[0]) != Path.GetFileName(rgszMkNewNames[0]))
                        {
                            IList<P4.Changelist> changes = ScmProvider.GetAvailibleChangelists(-1);

                            newChangeDescription = string.Format(Resources.P4VsProviderService_AfterRenameChangeDescription,
                                rgszMkOldNames[0], rgszMkNewNames[0]);

                            string summary = string.Format(
                                Resources.P4VsProviderService_AfterRenameSummary,
                                rgszMkOldNames[0], rgszMkNewNames[0]);

                            changeListId = SelectChangelistDlg.ShowChooseChangelistMove(summary, changes, ref newChangeDescription, _scmProvider);

                            if (changeListId == -2)
                            {
                                // user hit 'No'
                                return VSConstants.S_OK;
                            }

                            P4.Changelist changeToSubmit = ScmProvider.Connection.Repository.NewChangelist();
                            if (changeListId == -1)
                            {
                                changeToSubmit.Description = newChangeDescription;
                                changeToSubmit = ScmProvider.SaveChangelist(changeToSubmit, null);
                                changeListId = changeToSubmit.Id;
                            }
                            if (changeListId > 0)
                            {
                                changeToSubmit = ScmProvider.GetChangelist(changeListId);
                                changeToSubmit = ScmProvider.SaveChangelist(changeToSubmit, null);
                                changeListId = changeToSubmit.Id;
                            }

                            ScmProvider.RenameFile(rgszMkOldNames, rgszMkNewNames, changeListId);
                            ScmProvider.UpdateFiles(rgszMkOldNames, true);
                            ScmProvider.UpdateFiles(rgszMkNewNames, true);
                        }

                        GetSelection();

                        // And refresh the solution explorer glyphs because we affected the source control status of this file
                        // Note that by now, the project should already know about the new file name being part of its hierarchy
                        for (int newToRefresh = 0; newToRefresh < rgszMkNewNames.Count(); newToRefresh++)
                        {
                            IList<VSITEMSELECTION> nodes = GetControlledProjectsContainingFile(rgszMkNewNames[newToRefresh]);
                            _P4VsProvider.Glyphs.RefreshNodesGlyphs(nodes, null);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogException("P4VsProviderService.OnAfterRenameFiles", ex);
            }

            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories([In] IVsProject pProject, [In] int cDirs, [In] string[] rgszMkOldNames, [In] string[] rgszMkNewNames, [In] VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, [Out] VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, [Out] VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterRenameDirectories([In] int cProjects, [In] int cDirs, [In] IVsProject[] rgpProjects, [In] int[] rgFirstIndices, [In] string[] rgszMkOldNames, [In] string[] rgszMkNewNames, [In] VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterSccStatusChanged([In] int cProjects, [In] int cFiles, [In] IVsProject[] rgpProjects, [In] int[] rgFirstIndices, [In] string[] rgpszMkDocuments, [In] uint[] rgdwSccStatus)
        {
            return VSConstants.E_NOTIMPL;
        }
        #endregion

        #region Files and Project Management Functions

        /// <summary>
        /// Returns whether this source control provider is the active scc provider.
        /// </summary>
        public bool Active
        {
            get { return active; }
        }

        /// <summary>
        /// Variable containing the solution location in source control if the solution being loaded is controlled
        /// </summary>
        public string LoadingControlledSolutionLocation
        {
            get { return _loadingControlledSolutionLocation; }
            set
            {
                _loadingControlledSolutionLocation = value;

                if (ScmProvider != null)
                {
                    ScmProvider.LoadingSolution = !string.IsNullOrEmpty(_loadingControlledSolutionLocation);
                }
            }
        }

        /// <summary>
        /// Checks whether the specified project or solution (pHier==null) is under source control
        /// </summary>
        /// <returns>True if project is controlled.</returns>
        public bool IsProjectControlled(IVsHierarchy pHier)
        {
            if (pHier == null)
            {
                // this is solution, get the solution hierarchy
                pHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
            }

            return controlledProjects.ContainsKey(pHier);
        }

        /// <summary>
        /// Checks whether the specified project or solution (pHier==null) is offline
        /// </summary>
        /// <returns>True if project is offline.</returns>
        public bool IsProjectOffline(IVsHierarchy pHier)
        {
            if (pHier == null)
            {
                // this is solution, get the solution hierarchy
                pHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
            }

            return false;
        }

        public void AddFilesToProject(FileMap projectMap, IList<string> files)
        {
            ScmProvider.AddFilesToCache(files.ToArray());
            foreach (string file in files)
            {
                ScmFile f = ScmProvider.AddFileToCache(file);
                projectMap[file] = f;
            }
        }

        public bool AddCOntrolledProject(IVsHierarchy pHier)
        {
            if (controlledProjects.ContainsKey(pHier) == false)
            {
                controlledProjects[pHier] = new FileMap();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the specified projects and solution to source control
        /// </summary>
        public void AddProjectsToSourceControl(ref Hashtable hashUncontrolledProjects, bool addSolutionToSourceControl)
        {
            // A real source control provider will ask the user for a location where the projects will be controlled
            // From the user input it should create up to 4 strings that will pass them to the projects to persist, 
            // so next time the project is open from disk, it will callback source control package, and the package
            // could use the 4 binding strings to identify the correct database location of the project files.
            foreach (IVsHierarchy pHier in hashUncontrolledProjects.Keys)
            {
                IVsSccProject2 sccProject2 = (IVsSccProject2)pHier;
                if (Preferences.LocalSettings.GetBool("TagSolutionProjectFiles", false))
                {
                    sccProject2.SetSccLocation(string.Empty, string.Empty, string.Empty, _P4VsProvider.ProviderName);
                }
                else
                {
                    sccProject2.SetSccLocation(string.Empty, string.Empty, string.Empty, string.Empty);//, _P4VsProvider.ProviderName);
                }
                // Add the newly controlled projects now to the list of controlled projects in this solution
                controlledProjects[pHier] = new FileMap();
            }

            // Also, if the solution was selected to be added to scc, write in the solution properties the controlled status
            if (addSolutionToSourceControl)
            {
                IVsHierarchy solHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
                controlledProjects[solHier] = new FileMap();
                _P4VsProvider.SolutionHasDirtyProps = true;
            }

            // Now save all the modified files
            IVsSolution sol = (IVsSolution)_P4VsProvider.GetService(typeof(SVsSolution));
            sol.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty, null, 0);

            // Add now the solution and project files to the source control database
            foreach (IVsHierarchy pHier in hashUncontrolledProjects.Keys)
            {
                IVsSccProject2 sccProject2 = (IVsSccProject2)pHier;
                IList<string> files = _P4VsProvider.GetProjectFiles(sccProject2);
                AddFilesToProject(controlledProjects[pHier], files);
            }

            // If adding solution to source control, create a storage for the solution, too
            if (addSolutionToSourceControl)
            {
                IVsHierarchy solHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
                IList<string> files = new List<string>();
                string solutionFile = _P4VsProvider.GetSolutionFileName();
                files.Add(solutionFile);
                AddFilesToProject(controlledProjects[solHier], files);
            }

            // For all the projects added to source control, refresh their source control glyphs
            IList<VSITEMSELECTION> nodes = new List<VSITEMSELECTION>();
            foreach (IVsHierarchy pHier in hashUncontrolledProjects.Keys)
            {
                VSITEMSELECTION vsItem;
                vsItem.itemid = VSConstants.VSITEMID_ROOT;
                vsItem.pHier = pHier;
                nodes.Add(vsItem);
            }

            // Also, add the solution if necessary to the list of glyphs to refresh
            if (addSolutionToSourceControl)
            {
                VSITEMSELECTION vsItem;
                vsItem.itemid = VSConstants.VSITEMID_ROOT;
                vsItem.pHier = null;
                nodes.Add(vsItem);
            }

            _P4VsProvider.Glyphs.RefreshNodesGlyphs(nodes, null);
            // Raise the event to inform the shell that the solution was added to Source Control
            AddedToSourceControl?.Invoke(this, EventArgs.Empty);
        }

        // The following methods are not very efficient
        // A good source control provider should maintain maps to identify faster to which project does a file belong
        // and check only the status of the files in that project; or simply, query one common storage about the file status

        /// <summary>
        /// Returns the source control status of the specified file
        /// </summary>
        public SourceControlStatus GetFileStatus(string filename)
        {
            if (ScmProvider == null)
            {
                return SourceControlStatus.scsUncontrolled;
            }
            return ScmProvider.GetFileStatus(filename);
        }

        /// <summary>
        /// Adds the specified file to source control; the file must be part of a controlled project
        /// </summary>
        public void AddFileToSourceControl(string file, int changelistID, string newChangeDescription)
        {
            // Get all controlled projects containing this file
            IList<VSITEMSELECTION> nodes = GetControlledProjectsContainingFile(file);
            foreach (VSITEMSELECTION vsItem in nodes)
            {
                if (ScmProvider != null)
                {
                    ScmProvider.AddFiles(changelistID, newChangeDescription, file);
                    break;
                }
            }
        }

        /// <summary>
        /// Adds the specified files to source control; the files must be part of a controlled project
        /// </summary>
        public int AddFilesToSourceControl(IList<string> files, int changelistID, string newChangeDescription)
        {
            if (ScmProvider == null)
            {
                return -2;
            }

            IList<string> filesToAdd = new List<string>();

            foreach (string file in files)
            {
                // Get all controlled projects containing this file
                IList<VSITEMSELECTION> nodes = GetControlledProjectsContainingFile(file);
                if (nodes.Count > 0)
                {
                    // in at least one controlled project
                    filesToAdd.Add(file);
                }
            }
            if (ScmProvider != null)
            {
                return ScmProvider.AddFiles(changelistID, newChangeDescription, filesToAdd.ToArray());
            }
            return -2;
        }

        /// <summary>
        /// Checks in the specified file
        /// </summary>
        public void CheckinFile(int changeListID, string Description, string file)
        {
            // Before checking in files, make sure all in-memory edits have been committed to disk 
            // by forcing a save of the solution. Ideally, only the files to be checked in should be saved...
            IVsSolution sol = (IVsSolution)_P4VsProvider.GetService(typeof(SVsSolution));
            if (sol.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty, null, 0) != VSConstants.S_OK)
            {
                // If saving the files failed, don't continue with the checkin
                return;
            }

            // if not in the default changelist, move to the default list for checkin
            if (changeListID > 0)
            {
                ScmProvider.MoveFilesToChangeList(0, Description, new string[] { file });
            }
            SourceControlStatus status = ScmProvider.GetFileStatus(file);
            if (status != SourceControlStatus.scsUncontrolled)
            {
                ScmProvider.SubmitFiles(Description, null, false, file);
                return;
            }
        }

        /// <summary>
        /// Checks in the specified files
        /// </summary>
        public bool CheckinFiles(IList<string> files)
        {
            // Before checking in files, make sure all in-memory edits have been commited to disk 
            // by forcing a save of the solution. Ideally, only the files to be checked in should be saved...
            IVsSolution sol = (IVsSolution)_P4VsProvider.GetService(typeof(SVsSolution));
            if (sol.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty, null, 0) != VSConstants.S_OK)
            {
                // If saving the files failed, don't continue with the checkin
                return false;
            }

            return SubmitDlg.SubmitFiles(files, ScmProvider, _P4VsProvider.InCommandLineMode(), null);
        }

        /// <summary>
        /// Checkout the specified file from source control
        /// </summary>
        public int CheckoutFile(string file, int changelistID, string newChangeDescription)
        {
            SourceControlStatus status = ScmProvider.GetFileStatus(file);
            if (status != SourceControlStatus.scsCheckedOut)
            {
                return ScmProvider.CheckoutFiles(changelistID, newChangeDescription, file);
            }
            return -2;
        }

        /// <summary>
        /// Checkout the specified file from source control
        /// </summary>
        public int CheckoutFiles(IList<string> files, int changelistID, string newChangeDescription)
        {
            return ScmProvider.CheckoutFiles(changelistID, newChangeDescription, files.ToArray());
        }

        /// <summary>
        /// Lock the specified file from source control
        /// </summary>
        public IList<string> RevertFiles(bool unchangedOnly, IList<string> files)
        {
            // Before reverting files, make sure all in-memory edits have been committed to disk 
            // by forcing a save of the solution. Ideally, only the files to be checked in should be saved...
            // If the files are not saved, it can result in a situation where in memeory changes are 
            // not backed out by the revert.
            IVsSolution sol = (IVsSolution)_P4VsProvider.GetService(typeof(SVsSolution));
            if (sol.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty, null, 0) != VSConstants.S_OK)
            {
                // If saving the files failed, don't continue with the checkin
                return null;
            }
            return ScmProvider.RevertFiles(unchangedOnly, false, null, files.ToArray());
        }

        /// <summary>
        /// Unlock the specified file from source control
        /// </summary>
        public void LockFiles(IList<string> files)
        {
            ScmProvider.LockFiles(files.ToArray());
        }

        /// <summary>
        /// Checkout the specified file from source control
        /// </summary>
        public void UnlockFiles(IList<string> files)
        {
            ScmProvider.UnlockFiles(files.ToArray());
        }

        public void ChangeFileType(IList<string> files)
        {
            FileAttributesDlg.ChangeFileType(ScmProvider, files);
        }

        public void MoveToChangelist(IList<string> files)
        {
            P4.Changelist getDesc = ScmProvider.GetChangelist(-1);
            string newChangeDescription = getDesc.Description;

            IList<P4.Changelist> changes = ScmProvider.GetAvailibleChangelists(-1);
            int newChangeId = SelectChangelistDlg2.ShowChooseChangelistMove(Resources.MoveToChangelistPrompt,
                files, changes, ref newChangeDescription, ScmProvider);

            if (newChangeId > -1)
            {
                IList<P4.FileMetaData> mdl = ScmProvider.GetFileMetaData(files, null);

                if ((mdl != null) && (mdl.Count > 0))
                {
                    foreach (P4.FileMetaData fmd in mdl)
                    {
                        if (fmd.MovedFile != null)
                        {
                            P4.FileMetaData movedFmd = ScmProvider.GetFileMetaData(fmd.MovedFile.Path);
                            files.Add(movedFmd.LocalPath.Path);
                        }
                    }
                }
                ScmProvider.MoveFilesToChangeList(newChangeId, newChangeDescription, files.ToArray());
            }
        }

        private class IgnoreFileData
        {
            public IList<string> Files = null;
            public string Folder = null;
            public string Path = null;
            public string FirstFile = null;
            public bool Exists;
            public SourceControlStatus Status = null;
        }

        System.Text.Encoding _ignoreFileEncoding = null;
        System.Text.Encoding IgnoreFileEncoding
        {
            get
            {
                if (_ignoreFileEncoding == null)
                {
                    if (ScmProvider.Connection.Repository.Connection.Server.Metadata.UnicodeEnabled)
                    {
                        _ignoreFileEncoding = System.Text.Encoding.UTF8;
                    }
                    else
                    {
                        _ignoreFileEncoding = System.Text.Encoding.Default;
                    }
                }
                return _ignoreFileEncoding;
            }
        }
        public void AddToIgnoreList(bool remove, bool editOnly, params string[] files)
        {
            AddToIgnoreList(new List<string>(files), remove, editOnly);
        }

        public void AddToIgnoreList(IList<string> files, bool remove, bool editOnly)
        {
            bool markIgnoreFilesForAdd = Preferences.LocalSettings.GetBool("MarkIgnoreFileForAdd", false);
            bool addIgnoreFilesToSolution = Preferences.LocalSettings.GetBool("AddIgnoreFileToSolutuion", true);
            bool promptForChangeList = Preferences.LocalSettings.GetBool("PromptForChanglist", true);
            bool promptForNewIgnoreFile = Preferences.LocalSettings.GetBool("PromptOnNewIgnoreFile", true);
            bool IgnoreIgnoredFiles = Preferences.LocalSettings.GetBool("IgnoreIgnoredFiles", true);
            string ignoreFileName = P4ScmProvider.P4Ignore;

            _ignoreFileEncoding = null;

            int changeListId = -3;
            string newChangeDescription = null;

            Dictionary<string, IgnoreFileData> IgnoreFilesFolderMap = new Dictionary<string, IgnoreFileData>();
            List<string> Files = new List<string>();
            List<string> Actions = new List<string>();

            List<string> FilesToAdd = new List<string>();
            List<string> FilesToEdit = new List<string>();

            bool needChooseChangeDialog = false;
            bool needCreatePromptDialog = false;

            foreach (string file in files)
            {
                string folder = Path.GetDirectoryName(file);
                string fileName = Path.GetFileName(file);

                if (IgnoreFilesFolderMap.ContainsKey(folder) == false)
                {
                    IgnoreFileData ifd = new IgnoreFileData();
                    ifd.Files = new List<string>();
                    ifd.Folder = folder;

                    IgnoreFilesFolderMap[folder] = ifd;

                    string ignoreFile = Path.Combine(folder, ignoreFileName);

                    bool exists = File.Exists(ignoreFile);
                    SourceControlStatus status = SourceControlStatus.scsUncontrolled;
                    if (exists)
                    {
                        status = _scmProvider.GetFileStatus(ignoreFile);
                        if (status.TestAny(SourceControlStatus.scsCheckedIn))
                        {
                            FilesToEdit.Add(ignoreFile);
                            Files.Add(ignoreFile);
                            Actions.Add(Resources.AddToIgnoreList_CheckOut);
                        }
                    }
                    else
                    {
                        FilesToAdd.Add(ignoreFile);
                        Files.Add(ignoreFile);
                        Actions.Add(Resources.AddToIgnoreList_Add);
                    }
                    IgnoreFilesFolderMap[folder].Path = ignoreFile;
                    IgnoreFilesFolderMap[folder].Exists = exists;
                    IgnoreFilesFolderMap[folder].Status = status;
                    IgnoreFilesFolderMap[folder].FirstFile = file;

                    if (needCreatePromptDialog == false)
                    {
                        needCreatePromptDialog |= promptForNewIgnoreFile && !exists;
                    }
                    if (promptForChangeList && !needChooseChangeDialog)
                    {
                        needChooseChangeDialog |= IgnoreFilesFolderMap[folder].Status.TestAny(SourceControlStatus.scsCheckedIn);
                        needChooseChangeDialog |= markIgnoreFilesForAdd && !IgnoreFilesFolderMap[folder].Exists;
                    }
                }
                IgnoreFilesFolderMap[folder].Files.Add(fileName);

                _scmProvider.ResetIgnoredFilesMap(file);
            }
            if (needChooseChangeDialog)
            {
                if (_P4VsProvider.InCommandLineMode() == false)
                {
                    IList<P4.Changelist> changes = _scmProvider.GetAvailibleChangelists(-1);
                    changeListId = SelectChangelistDlg2.ShowChooseChangelistActions(Resources.P4VsProvider_EditIgnoreFilesPrompt, Files, Actions, changes, ref newChangeDescription);

                    if (changeListId <= -2)
                    {
                        return;
                    }
                }
                else
                {
                    changeListId = 0;
                }
                if ((changeListId == -1) && (string.IsNullOrEmpty(newChangeDescription)))
                {
                    newChangeDescription = Resources.P4VsProvider_AddFilesDefaultChangelistDescription;
                }

                if ((FilesToEdit != null) && (FilesToEdit.Count > 0))
                {
                    _scmProvider.CheckoutFiles(changeListId, newChangeDescription, FilesToEdit.ToArray());
                }

                if ((FilesToAdd != null) && (FilesToAdd.Count > 0))
                {
                    foreach (string file in FilesToAdd)
                    {
                        using (StreamWriter sw = new StreamWriter(file, false, IgnoreFileEncoding))
                        {
                            sw.Write(Resources.AddToIgnoreList_TemplateFile);
                        }
                    }
                    _scmProvider.AddFiles(changeListId, newChangeDescription, FilesToAdd.ToArray());
                }
            }
            else if (needCreatePromptDialog)    // only prompt for permission to create if we haven't already asked
            {                                   // the user to choose a change list to checkout / add an ignore list
                if (_P4VsProvider.InCommandLineMode() == false)
                {
                    if (PromptNewIgnoreFileDlg.Show() == false)
                    {
                        return;
                    }
                }
            }
            foreach (string folder in IgnoreFilesFolderMap.Keys)
            {
                IgnoreFileData ignoreFile = IgnoreFilesFolderMap[folder];

                bool exists = ignoreFile.Exists;

                if (exists == false)
                {
                    // create a new ignore file for this directory

                    // release the connection so the ignored file will be read after it is written
                    ScmProvider.Connection.Repository.Connection.ReleaseConnection();
                    using (StreamWriter sw = new StreamWriter(ignoreFile.Path, false, IgnoreFileEncoding))
                    {
                        sw.Write(Resources.AddToIgnoreList_TemplateFile);
                        sw.WriteLine("#");
                        sw.WriteLine(string.Format(Resources.IgnoreFileCreatedFor, ignoreFile.Folder));
                        sw.WriteLine("#");
                        if (IgnoreIgnoredFiles)
                        {
                            sw.WriteLine(ignoreFileName);
                        }
                        if (!editOnly)
                        {
                            foreach (string file in ignoreFile.Files)
                            {
                                if (remove)
                                {
                                    sw.Write("!");
                                }
                                sw.WriteLine(file);
                            }
                        }
                    }
                    if (addIgnoreFilesToSolution)
                    {
                        IList<VSITEMSELECTION> nodes = GetControlledProjectsContainingFile(ignoreFile.FirstFile);

                        if ((nodes != null) && (nodes.Count > 0))
                        {
                            VSADDRESULT[] addResults = new VSADDRESULT[1];
                            int idx = 0;
                            IVsProject2 pProject = null;
                            while ((pProject == null) && (idx < nodes.Count))
                            {
                                pProject = nodes[idx++].pHier as IVsProject2;
                            }
                            if (pProject != null)
                            {

                                pProject.AddItem(VSConstants.VSITEMID_ROOT,
                                                    VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE,
                                                    ignoreFileName,
                                                    1,
                                                    new string[] { ignoreFile.Path },
                                                    IntPtr.Zero,
                                                    addResults);
                            }
                        }
                    }
                }
                else if (!editOnly)
                {
                    Dictionary<string, bool> FileMap = new Dictionary<string, bool>();
                    List<string> FileLines = new List<string>();

                    string contents = null;

                    using (StreamReader sr = new StreamReader(ignoreFile.Path))
                    {
                        contents = sr.ReadToEnd();
                    }
                    string[] lines = null;
                    if (string.IsNullOrEmpty(contents) == false)
                    {
                        lines = contents.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    if ((lines != null) && (lines.Length > 0))
                    {
                        foreach (string l in lines)
                        {
                            string line = l.Trim();
                            if (line[0] == '!')
                            {
                                string file = line.Substring(1);
                                FileMap.Add(file, true);
                                FileLines.Add(file);
                            }
                            else
                            {
                                //don't add comments to the file map
                                if (line[0] != '#')
                                {
                                    FileMap.Add(line, false);
                                }
                                FileLines.Add(line);
                            }
                        }
                    }
                    if (ignoreFile.Files.Count > 0)
                    {
                        // files to add or remove

                        foreach (string file in ignoreFile.Files)
                        {
                            if (FileMap.ContainsKey(file))
                            {
                                //already in the file
                                if (remove != FileMap[file])
                                {
                                    // remove and exclude disagree, so remove from ignore list
                                    FileMap.Remove(file);
                                    FileLines.Remove(file);
                                }
                            }
                            else
                            {
                                // not in file so add it
                                FileMap.Add(file, remove);
                                FileLines.Add(file);
                            }
                        }
                    }
                    // release the connection so the ignored file will be reread after it is written
                    ScmProvider.Connection.Repository.Connection.ReleaseConnection();
                    if (File.Exists(ignoreFile.Path))
                    {
                        File.Delete(ignoreFile.Path);
                    }
                    using (StreamWriter sw = new StreamWriter(ignoreFile.Path, false, IgnoreFileEncoding))
                    {
                        foreach (string line in FileLines)
                        {
                            if (FileMap.ContainsKey(line) && FileMap[line])
                            {
                                sw.Write("!");
                            }
                            sw.WriteLine(line);
                        }
                    }
                }

                if (editOnly)
                {
                    EnvDTE.DTE dte = P4VsProvider.GetDTE();
                    dte.ItemOperations.OpenFile(ignoreFile.Path, null);
                }
                if ((!ignoreFile.Exists) || ignoreFile.Status.TestAny(SourceControlStatus.scsCheckedIn))
                {
                    ScmProvider.Connection.Repository.Connection.ReleaseConnection();
                    ScmProvider.UpdateFileInCache(ignoreFile.Path, true);
                    RefreshProjectGlyphs(ignoreFile.Path);
                }
            }
        }
        public void ResolveFiles(IList<string> files)
        {
            ResolveFileDlg.UpdateFileStatus cb = new ResolveFileDlg.UpdateFileStatus(_P4VsProvider.Glyphs.RefreshSelectedNodesGlyphs);
            ResolveFileDlg.ResolveFiles(ScmProvider, files, _P4VsProvider.InCommandLineMode(), cb, false);
        }
        /// <summary>
        /// Checkout the specified file from source control
        /// </summary>
        public void DeleteFiles(IList<string> files, int changelistID, string newChangeDescription)
        {
            ScmProvider.DeleteFiles(changelistID, newChangeDescription, files.ToArray());
        }

        /// <summary>
        /// Diff the specified files in source control
        /// </summary>
        public void DiffFiles(IList<string> files)
        {
            ScmProvider.DiffFiles(files.ToArray());
        }

        /// <summary>
        /// Diff the specified files in source control
        /// </summary>
        public void Diff2Files(IList<P4.FileSpec> files)
        {
            ScmProvider.Diff2Files(files);
        }

        /// <summary>
        /// Sync the specified file(s) to the head revision from source control
        /// </summary>
        public bool SyncFiles(P4.Options options, IList<string> files)
        {
            return ScmProvider.SyncFiles(null, files.ToArray());
        }

        /// <summary>
        /// Sync the specified file(s) to the head revision from source control
        /// </summary>
        public bool SyncFiles(out IList<string> fileshanged, P4.Options options, IList<string> files)
        {
            return ScmProvider.SyncFiles(out fileshanged, null, files.ToArray());
        }

        /// <summary>
        /// Shelves the specified files
        /// </summary>
        public void ShelveFiles(IList<string> files)
        {
            // Before checking in files, make sure all in-memory edits have been commited to disk 
            // by forcing a save of the solution. Ideally, only the files to be checked in should be saved...
            IVsSolution sol = (IVsSolution)_P4VsProvider.GetService(typeof(SVsSolution));
            if (sol.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty, null, 0) != VSConstants.S_OK)
            {
                // If saving the files failed, don't continue with the checkin
                return;
            }

            ShelveFileDlg.ShelveFiles(files, ScmProvider, (_P4VsProvider.InCommandLineMode() == false));
        }


        /// <summary>
        /// Unshelves the specified files
        /// </summary>
        public void UnshelveFiles(IList<string> files)
        {
        }

        /// <summary>
        /// Merges the specified files
        /// </summary>
        public int Merge(IList<string> files)
        {
            List<string> source = new List<string>();
            string target = null;

            P4.Client current = ScmProvider.Connection.Repository.Connection.Client;
            target = current.Stream;
            if (target != null)
            {
                P4.Stream stream = ScmProvider.GetStream(target, null, null);
                //need to convert the list here
                //case 1
                if (stream.Type == P4.StreamType.Development ||
                    stream.Type == P4.StreamType.Task)
                {
                    if (!(stream.Options == P4.StreamOption.NoFromParent))
                    {
                        if (stream.Parent.Path != "none")
                        {
                            source.Add(stream.Parent.Path);
                        }
                    }
                    P4.Options opts = new P4.Options();
                    opts["-F"] = "Parent=" + stream.Id;
                    IList<P4.Stream> releaseChildren = ScmProvider.GetStreams(opts, null);
                    if (releaseChildren != null)
                    {
                        foreach (P4.Stream releaseChild in releaseChildren)
                        {
                            if (releaseChild.FirmerThanParent == "true" && releaseChild.ChangeFlowsToParent == "true")
                            {
                                source.Add(releaseChild.Id);
                            }
                        }
                    }
                }
                //case 2, 3 and 4
                if ((stream.Type == P4.StreamType.Release) ||
                    (stream.Type == P4.StreamType.Mainline) ||
                    (stream.Type == P4.StreamType.Virtual))
                {
                    P4.Options opts = new P4.Options();
                    opts["-F"] = "Parent=" + stream.Id;
                    IList<P4.Stream> releaseChildren = ScmProvider.GetStreams(opts, null);
                    if (releaseChildren != null)
                    {
                        foreach (P4.Stream releaseChild in releaseChildren)
                        {
                            if (releaseChild.FirmerThanParent == "true" && releaseChild.ChangeFlowsToParent == "true")
                            {
                                source.Add(releaseChild.Id);
                            }
                        }
                    }
                }

                if (source != null && source.Count > 0)
                {
                    IList<P4.FileSpec> merge = IntegrateDlg.Show(source, target, "merge", ScmProvider);

                    if (merge != null)
                    {
                        return VSConstants.S_OK;
                    }
                }
                else
                {
                    MessageBox.Show(string.Format(Resources.P4VsProviderService_MergeNoPreferedStreamsWarning,
                        target));
                }
            }
            else
            {
                MessageBox.Show(Resources.P4VsProviderService_MergeNeedDedicatedStreamWarning);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Copies the specified files
        /// </summary>
        public int Copy(IList<string> files)
        {
            List<string> source = new List<string>();
            string target = null;

            P4.Client current = ScmProvider.Connection.Repository.Connection.Client;
            target = current.Stream;
            if (target != null)
            {
                P4.Stream stream = ScmProvider.GetStream(target, null, null);
                //need to convert the list here
                //case 1, 3, and 4
                if ((stream.Type == P4.StreamType.Development) ||
                    (stream.Type == P4.StreamType.Mainline) ||
                    (stream.Type == P4.StreamType.Virtual))
                {
                    P4.Options opts = new P4.Options();
                    opts["-F"] = "Parent=" + stream.Id + " & Type=development";
                    IList<P4.Stream> devChildren = ScmProvider.GetStreams(opts, null);
                    if (devChildren != null)
                    {
                        foreach (P4.Stream devChild in devChildren)
                        {
                            if (devChild.FirmerThanParent == "false" && devChild.ChangeFlowsToParent == "true")
                            {
                                source.Add(devChild.Id);
                            }
                        }
                    }
                    opts = new P4.Options();
                    opts["-F"] = "Parent=" + stream.Id + " & Type=task";
                    IList<P4.Stream> taskChildren = ScmProvider.GetStreams(opts, null);
                    if (taskChildren != null)
                    {
                        foreach (P4.Stream taskChild in taskChildren)
                        {
                            if (taskChild.FirmerThanParent == "false" && taskChild.ChangeFlowsToParent == "true")
                            {
                                source.Add(taskChild.Id);
                            }
                        }
                    }
                }

                //case 2
                if (stream.Type == P4.StreamType.Release)
                {
                    if (!(stream.Options == P4.StreamOption.NoFromParent))
                    {
                        source.Add(stream.Parent.Path);
                    }
                    P4.Options opts = new P4.Options();
                    opts["-F"] = "Parent=" + stream.Id;
                    IList<P4.Stream> devChildren = ScmProvider.GetStreams(opts, null);
                    if (devChildren != null)
                    {
                        foreach (P4.Stream devChild in devChildren)
                        {
                            if (devChild.FirmerThanParent == "false" && devChild.ChangeFlowsToParent == "true")
                            {
                                source.Add(devChild.Id);
                            }
                        }
                    }
                }

                if (source != null && source.Count > 0)
                {
                    IList<P4.FileSpec> copy = IntegrateDlg.Show(source, target, "copy", ScmProvider);

                    if (copy != null)
                    {
                        return VSConstants.S_OK;
                    }
                }
                else
                {
                    MessageBox.Show(string.Format(Resources.P4VsProviderService_CopyNoPreferedStreamsWarning,
                        target));
                }
            }
            else
            {
                MessageBox.Show(Resources.P4VsProviderService_CopyNeedDedicatedStreamWarning);
            }

            return VSConstants.S_OK;
        }

        public IList<IVsHierarchy> GetControlledProjects()
        {
            IList<IVsHierarchy> value = new List<IVsHierarchy>();
            foreach (IVsHierarchy node in controlledProjects.Keys)
            {
                value.Add(node);
            }
            if (value.Count > 0)
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Returns a list of controlled projects containing the specified file
        /// </summary>
        public IList<VSITEMSELECTION> GetControlledProjectsContainingFile(string file)
        {
            // Accumulate all the controlled projects that contain this file
            IList<VSITEMSELECTION> nodes = new List<VSITEMSELECTION>();

            // bail immediately if the path ends with a ... wildcard
            if (file.EndsWith("..."))
            {
                return nodes;
            }

            foreach (IVsHierarchy pHier in controlledProjects.Keys)
            {
                IVsHierarchy solHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
                if (solHier == pHier)
                {
                    // This is the solution
                    if (_P4VsProvider.IsThereASolution())
                    {
                        if (file.ToLower().CompareTo(_P4VsProvider.GetSolutionFileName().ToLower()) == 0)
                        {
                            VSITEMSELECTION vsItem;
                            vsItem.itemid = VSConstants.VSITEMID_ROOT;
                            vsItem.pHier = null;
                            nodes.Add(vsItem);
                        }
                    }
                }
                else
                {
                    IVsProject2 pProject = pHier as IVsProject2;
                    // See if the file is member of this project
                    // Caveat: the IsDocumentInProject function is expensive for certain project types, 
                    // you may want to limit its usage by creating your own maps of file2project or folder2project
                    int fFound;
                    uint itemid;
                    VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];
                    if (pProject != null && pProject.IsDocumentInProject(file, out fFound, prio, out itemid) == VSConstants.S_OK && fFound != 0)
                    {
                        VSITEMSELECTION vsItem;
                        vsItem.itemid = itemid;
                        vsItem.pHier = pHier;
                        nodes.Add(vsItem);
                    }
                    else
                    {
                        IVsSccProject2 sccPrj2 = pHier as IVsSccProject2;
                        if (sccPrj2 != null)
                        {
                            string prjFile = _P4VsProvider.GetProjectFileName(sccPrj2);
                            if (prjFile != null)
                            {
                                if (file.ToLower().CompareTo(prjFile.ToLower()) == 0)
                                {
                                    VSITEMSELECTION vsItem;
                                    vsItem.itemid = VSConstants.VSITEMID_ROOT;
                                    vsItem.pHier = null;
                                    nodes.Add(vsItem);
                                }
                            }
                        }
                    }
                }
            }

            return nodes;
        }

		/// <summary>
		/// This function returns true in case the file is present in any of the project
		/// It is an optimized modification to the function GetControlledProjectsContainingFile
		/// </summary>
		private bool AnyControlledProjectsContainingFile(string file)
		{
			// bail immediately if the path ends with a ... wildcard
			if (file.EndsWith("..."))
			{
				return false;
			}

			var controlledProjectKeys = controlledProjects.Keys.ToArray();
			var totalProjects = controlledProjectKeys.Length;

			// It is high likely that this file would also be present in the same project
			// in which last file checked was present
			// So check all projects, but just start with last project checked
			// And as this function can get called via different thread, static lastAccessedIndex is received into local variable
			var projectToStartSearching = lastAccessedIndex;

			for (int i = 0; i < totalProjects; i = IncrementCounters(i, totalProjects))
			{
				// To circle back to first project once already searched in the last project
				// "% totalProjects" is used
				var currentProject = (projectToStartSearching + i) % totalProjects;
				IVsHierarchy pHier = controlledProjectKeys[currentProject];

				IVsHierarchy solHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
				if (solHier == pHier)
				{
					// This is the solution
					if (_P4VsProvider.IsThereASolution())
					{
						if (file.ToLower().CompareTo(_P4VsProvider.GetSolutionFileName().ToLower()) == 0)
						{
							VSITEMSELECTION vsItem;
							vsItem.itemid = VSConstants.VSITEMID_ROOT;
							vsItem.pHier = null;

							return true;
						}
					}
				}
				else
				{
					IVsProject2 pProject = pHier as IVsProject2;
					// See if the file is member of this project
					// Caveat: the IsDocumentInProject function is expensive for certain project types, 
					// you may want to limit its usage by creating your own maps of file2project or folder2project
					int fFound;
					uint itemid;
					VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];

					if (pProject != null && pProject.IsDocumentInProject(file, out fFound, prio, out itemid) == VSConstants.S_OK && fFound != 0)
					{
						VSITEMSELECTION vsItem;
						vsItem.itemid = itemid;
						vsItem.pHier = pHier;

						return true;
					}
					else
					{
						IVsSccProject2 sccPrj2 = pHier as IVsSccProject2;
						if (sccPrj2 != null)
						{
							string prjFile = _P4VsProvider.GetProjectFileName(sccPrj2);
							if (prjFile != null)
							{
								if (file.ToLower().CompareTo(prjFile.ToLower()) == 0)
								{
									VSITEMSELECTION vsItem;
									vsItem.itemid = VSConstants.VSITEMID_ROOT;
									vsItem.pHier = null;

									return true;
								}
							}
						}
					}
				}
			}

			return false;
		}

		private int IncrementCounters(int i, int totalProjects)
		{
			lastAccessedIndex++;
			if (lastAccessedIndex >= totalProjects)
			{
				// Reset the index to start over looping from first element
				lastAccessedIndex = 0;
			}

			i++;
			return i;
		}

		/// <summary>
		/// Returns a list of controlled projects containing the specified file
		/// </summary>
		public IList<VSITEMSELECTION> GetControlledProjectsContainingFiles([In] int cFiles, [In] string[] files)
        {
            // Accumulate all the controlled projects that contain this file
            IList<VSITEMSELECTION> nodes = new List<VSITEMSELECTION>();

            for (int idx = 0; idx < cFiles; idx++)
            {
                string file = files[idx];
                foreach (IVsHierarchy pHier in controlledProjects.Keys)
                {
                    IVsHierarchy solHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
                    if (solHier == pHier)
                    {
                        // This is the solution
                        if (file.ToLower().CompareTo(_P4VsProvider.GetSolutionFileName().ToLower()) == 0)
                        {
                            VSITEMSELECTION vsItem;
                            vsItem.itemid = VSConstants.VSITEMID_ROOT;
                            vsItem.pHier = null;
                            nodes.Add(vsItem);
                        }
                    }
                    else
                    {
                        IVsProject2 pProject = pHier as IVsProject2;
                        // See if the file is member of this project
                        // Caveat: the IsDocumentInProject function is expensive for certain project types, 
                        // you may want to limit its usage by creating your own maps of file2project or folder2project
                        int fFound;
                        uint itemid;
                        VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];
                        if (pProject != null && pProject.IsDocumentInProject(file, out fFound, prio, out itemid) == VSConstants.S_OK && fFound != 0)
                        {
                            VSITEMSELECTION vsItem;
                            vsItem.itemid = itemid;
                            vsItem.pHier = pHier;
                            nodes.Add(vsItem);
                        }
                    }
                }
            }
            return nodes;
        }

        /// <summary>
        /// Updates the status of the files in the cache and refreshes the 
        /// glyphs of the nodes containing the files
        /// </summary>
        public void UpdateProjectGlyphs(IList<string> files, bool usingDepotPaths)
        {
            if (files != null)
            {
                if (usingDepotPaths)
                {
                    IList<string> localPaths = ScmProvider.UpdateDepotFiles(files);
                    RefreshProjectGlyphs(localPaths, false);
                }
                else
                {
                    ScmProvider.UpdateFiles(files, true);
                    RefreshProjectGlyphs(files, false);
                }
            }
        }

        /// <summary>
        /// Updates the status of the file in the cache and refreshes the 
        /// glyphs of the nodes containing the files
        /// </summary>
        public void UpdateProjectGlyphs(string file, bool usingDepotPaths)
        {
            if ((file != null) && (ScmProvider != null))
            {
                if (usingDepotPaths)
                {
                    string localPath = ScmProvider.UpdateDepotFileInCache(file);
                    RefreshProjectGlyphs(localPath);
                }
                else
                {
                    ScmProvider.UpdateFileInCache(file, true);
                    RefreshProjectGlyphs(file);
                }
            }
        }

        // Goes together with: UpdatedFiles_Delegate in P4ScmCache.cs
        public delegate void RefreshProjectGlyphs_Delegate(IList<string> files, bool forceUpdate);

        /// <summary>
        /// Refreshes the glyphs of the nodes containing the files
        /// </summary>
        public void RefreshProjectGlyphs(IList<string> files, bool forceUpdate)
        {
            if (UiDispatcher != null)
            {
                UiDispatcher.BeginInvoke(new RefreshProjectGlyphs_Delegate(RefreshProjectGlyphs_Int), files, forceUpdate);
            }
        }

        public void RefreshProjectGlyphs_Int(IList<string> files, bool forceUpdate)
        {
            if ((files != null) && (ScmProvider != null))
            {
                _P4VsProvider.ResetCommandStatus();

                int numberOfFilesToCache = Preferences.LocalSettings.GetInt("Number_files_cache", 500);

                if (files.Count <= numberOfFilesToCache)
                {
                    RefreshProjectGlyphsFiles(files, forceUpdate);
                }
                else
                {
                    int updateIdx = 0;
                    IList<string> updateList = new List<string>();
                    while (updateIdx < files.Count)
                    {
                        updateList.Add(files[updateIdx]);
                        if ((updateIdx >= (files.Count - 1)) || (updateList.Count >= numberOfFilesToCache))
                        {
                            RefreshProjectGlyphsFiles(updateList, forceUpdate);
                            updateList = new List<string>();
                        }
                        updateIdx++;
                    }
                }
            }
        }
        /// <summary>
        /// Refreshes the glyphs of the nodes containing the file
        /// </summary>
        public void RefreshProjectGlyphs(string file, bool forceUpdate = true)
        {
            if (ScmProvider == null)
            {
                return;
            }

            _P4VsProvider.ResetCommandStatus();

            // And refresh the solution explorer glyphs of all the projects containing this file to reflect the checked out status
            IList<VSITEMSELECTION> nodes = GetControlledProjectsContainingFile(file);

            string[] rgpszFullPaths = new string[1];

            rgpszFullPaths[0] = file;
            VsStateIcon[] rgsiGlyphs = new VsStateIcon[1];
            uint[] rgdwSccStatus = new uint[1];

            if ((file.EndsWith("\\")) || (file.EndsWith("\\*")) || (file.EndsWith("\\...")))
            {
                List<string> files = new List<string>();
                files.Add(file);
                // directory
                ScmProvider.UpdateFiles(files, forceUpdate);
            }
            else
            {
                ScmProvider.UpdateFileInCache(file, forceUpdate);
            }
            GetSccGlyph(1, rgpszFullPaths, rgsiGlyphs, rgdwSccStatus);

            uint[] rguiAffectedNodes = new uint[1];

            uint itemid;
            int fFound;

            foreach (VSITEMSELECTION node in nodes)
            {
                IVsProject2 pscp = node.pHier as IVsProject2;
                IVsSccProject2 sccProject2 = node.pHier as IVsSccProject2;

                VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];
                if (pscp != null && pscp.IsDocumentInProject(file, out fFound, prio, out itemid) == VSConstants.S_OK && fFound != 0)
                {
                    rguiAffectedNodes[0] = itemid;
                    if (sccProject2 != null) sccProject2.SccGlyphChanged(1, rguiAffectedNodes, rgsiGlyphs, rgdwSccStatus);
                }
                else if (pscp == null)
                {
                    //sololution node
                    IVsHierarchy solHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
                    int hr = solHier.SetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_StateIconIndex, rgsiGlyphs[0]);
                }
            }
        }
        /// <summary>
        /// Refreshes the glyphs of the nodes containing the files
        /// </summary>
        public void RefreshProjectGlyphsFiles(IList<string> files, bool forceUpdate = true)
        {
            if (ScmProvider == null)
            {
                return;
            }

            _P4VsProvider.ResetCommandStatus();

            // And refresh the solution explorer glyphs of all the projects containing this file to reflect the checked out status
            IDictionary<string, IList<VSITEMSELECTION>> nodes = new Dictionary<string, IList<VSITEMSELECTION>>();

            string[] rgpszFullPaths = new string[files.Count];

            for (int i = 0; i < files.Count; i++)
            {
                nodes[files[i]] = GetControlledProjectsContainingFile(files[i]);
                rgpszFullPaths[i] = files[i];
            }

            VsStateIcon[] rgsiGlyphs = new VsStateIcon[files.Count];
            uint[] rgdwSccStatus = new uint[files.Count];

            // Update files
            ScmProvider.UpdateFiles(files, forceUpdate);

            GetSccGlyph(files.Count, rgpszFullPaths, rgsiGlyphs, rgdwSccStatus);

            uint[] rguiAffectedNodes = new uint[1];

            uint itemid;
            int fFound;
            int iterator = 0;

            foreach (var dic in nodes)
            {
                foreach (VSITEMSELECTION node in dic.Value)
                {
                    IVsProject2 pscp = node.pHier as IVsProject2;
                    IVsSccProject2 sccProject2 = node.pHier as IVsSccProject2;

                    VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];
                    if (pscp != null && pscp.IsDocumentInProject(dic.Key, out fFound, prio, out itemid) == VSConstants.S_OK && fFound != 0)
                    {
                        rguiAffectedNodes[0] = itemid;
                        if (sccProject2 != null) sccProject2.SccGlyphChanged(1, rguiAffectedNodes, new VsStateIcon[] { rgsiGlyphs[iterator] }, new uint[] { rgdwSccStatus[iterator] });
                    }
                    else if (pscp == null)
                    {
                        //sololution node
                        IVsHierarchy solHier = (IVsHierarchy)_P4VsProvider.GetService(typeof(SVsSolution));
                        int hr = solHier.SetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_StateIconIndex, rgsiGlyphs[iterator]);
                    }
                }
                iterator++;
            }
        }

        /// <summary>
        /// Checkout the file from source control and refreshes the glyphs of the files containing the file
        /// </summary>
        public int CheckoutFileAndRefreshProjectGlyphs(string file, int changelist, string newChangelistDescription)
        {
            // First, checkout the file
            int changeId = CheckoutFile(file, changelist, newChangelistDescription);

            if (changeId != -2)
            {
                ScmProvider.UpdateFileInCache(file, true);
                RefreshProjectGlyphs(file);
            }
            return changeId;
        }

        #endregion

        #region IVsSccGlyphs Members & IVsSccGlyphs2 Members

        private uint BaseImageIdx = 0;
        private ImageList Glyphs;

        public int GetCustomGlyphList(uint BaseIndex, out IntPtr pdwImageListHandle)
        {
            try
            {
                if (Glyphs != null && BaseIndex != BaseImageIdx)
                {
                    Glyphs.Dispose();
                    Glyphs = null;
                }

                // We give VS all our custom glyphs from baseindex upwards
                if (Glyphs == null)
                {
                    BaseImageIdx = BaseIndex;
                    if (BaseIndex == 0)
                    {
                        Glyphs = P4SolutionExplorer.CustomGlyphs;
                    }
                    else
                    {
                        Glyphs = P4SolutionExplorer.CustomGlyphsLongList;
                    }
                }
                pdwImageListHandle = unchecked(Glyphs.Handle);

                return VSConstants.S_OK;

            }
            catch (Exception)
            {
                pdwImageListHandle = (IntPtr)0;
                return VSConstants.S_FALSE;
            }
        }

        public IVsImageMonikerImageList GetCustomGlyphMonikerList(uint baseIndex)
        {
            IntPtr pdwImageListHandle;
            GetCustomGlyphList(baseIndex, out pdwImageListHandle);

            var imageService = (IVsImageService2)Package.GetGlobalService(typeof(SVsImageService));
            var vsImageMonikerImageList = imageService.CreateMonikerImageListFromHIMAGELIST(pdwImageListHandle);

            return vsImageMonikerImageList;
        }

        #endregion

        #region IVsSccControlNewSolution Members

        public int AddNewSolutionToSourceControl()
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            CreatingNewSolution = false;

            if ((ScmProvider == null) || ScmProvider.Connection.Disconnected)
            {
                _P4VsProvider.ConnectToScm(null, null, null);
            }
            else
            {
                _P4VsProvider.SuppressConnection = false;
            }
            if ((ScmProvider == null) || ScmProvider.Connection.Disconnected)
            {
                //user must have cancel connect
                return VSConstants.S_OK;
            }
            string solutionFileName = _P4VsProvider.GetSolutionFileName();

            IList<P4.FileMetaData> solutionFileMetaData = ScmProvider.GetFileMetaData(null, solutionFileName);

            string newChangeDescription = Resources.P4VsProvider_AddFilesDefaultChangelistDescription;
            int changeListId = 0;

            if (solutionFileMetaData == null || solutionFileMetaData.Count <= 0 || solutionFileMetaData[0] == null)
            {
                bool notInWorkspace = true;
                while (notInWorkspace)
                {
                    try
                    {
                        IList<P4.Changelist> changes = ScmProvider.GetAvailibleChangelists(-1);
                        changeListId = SelectChangelistDlg.ShowChooseChangelist(Resources.P4VsProviderService_AddNewSlnFiles, changes, ref newChangeDescription);
                        if (changeListId >= -1)
                        {
                            if (string.IsNullOrEmpty(newChangeDescription))
                            {
                                newChangeDescription = Resources.P4VsProvider_AddFilesDefaultChangelistDescription;
                            }

                            //solution file has not been checked in
                            SelectChangelistDlg.CurrentChangeList = ScmProvider.AddFiles(0, null, new string[] { solutionFileName });

                            P4.P4CommandResult results = ScmProvider.Connection.Repository.Connection.LastResults;
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
                                    if (Preferences.LocalSettings.GetBool("WarnSlnWorkspace", true))
                                    {
                                        MessageBox.Show(
                                            Resources.P4VsProviderService_CantAddSlnClientRootError, Resources.P4VS, MessageBoxButtons.OK);
                                        return VSConstants.S_OK;
                                    }
                                }
                            }
                            else
                            {
                                notInWorkspace = false;
                                P4ErrorDlg.Show(results, false);
                            }
                        }
                        else
                        {
                            // cancel
                            return VSConstants.S_OK;
                        }
                    }
                    catch (P4.P4Exception ex)
                    {
                        if ((ex.ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
                            (ex.ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient))
                        {
                            if (Preferences.LocalSettings.GetBool("WarnSlnWorkspace", true))
                            {
                                MessageBox.Show(
                                Resources.P4VsProviderService_CantAddSlnClientRootError, Resources.P4VS, MessageBoxButtons.OK);
                                return VSConstants.S_OK;
                            }
                        }
                        if ((ex.ErrorCode == P4.P4ClientError.MsgDm_IntegMovedUnmapped) ||
                            (ex.ErrorCode == P4.P4ClientError.MsgDm_ExVIEW) ||
                            (ex.ErrorCode == P4.P4ClientError.MsgDm_ExVIEW2))
                        {
                            notInWorkspace = false;
                        }
                        MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (notInWorkspace)
                    {
                        if (Preferences.LocalSettings.GetBool("WarnSlnWorkspace", true))
                        {
                            if (DialogResult.No == MessageBox.Show(
                            Resources.P4VsProviderService_AddSlnWantUpdateClientMapWarning, Resources.P4VS, MessageBoxButtons.YesNo))
                            {
                                return VSConstants.S_OK; ;
                            }
                            P4.Client client = ScmProvider.getClient(ScmProvider.Connection.Repository.Connection.Client.Name, null);
                            if (DlgEditWorkspace.EditWorkspace(ScmProvider, client) == null)
                            {
                                return VSConstants.S_OK; ;
                            }
                        }
                    }
                }
            }

            Hashtable hashUncontrolledProjects = new Hashtable();

            _P4VsProvider.SolutionHasDirtyProps = true;

            if (IsProjectControlled(null) == false)
            {
                RegisterSccProject(null, solutionFileName, "", "", _P4VsProvider.ProviderName);
            }
            if (string.IsNullOrEmpty(ScmProvider.SolutionFile))
            {
                ScmProvider.SolutionFile = solutionFileName;
            }
            // When the solution is selected, all the uncontrolled projects in the solution will be added to scc
            Hashtable hash = _P4VsProvider.GetLoadedControllableProjectsEnum();

            foreach (IVsHierarchy pHier in hash.Keys)
            {
                if (!IsProjectControlled(pHier))
                {
                    hashUncontrolledProjects[pHier] = true;
                }
            }

            if (hashUncontrolledProjects.Count > 0)
            {
                AddProjectsToSourceControl(ref hashUncontrolledProjects, true);
            }

            if (hashUncontrolledProjects.Count > 0)
            {
                foreach (IVsHierarchy pHier in hashUncontrolledProjects.Keys)
                {
                    IVsSccProject2 sccProject2 = (IVsSccProject2)pHier;
                    IList<string> files = _P4VsProvider.GetProjectFiles(sccProject2);

                    AddFilesToSourceControl(files, changeListId, newChangeDescription);

                    RefreshProjectGlyphs(files, true);
                }
            }

            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // now refresh the selected nodes' glyphs
                _P4VsProvider.Glyphs.RefreshNodesGlyphs(null, null);
            }
            finally
            {
                Cursor.Current = oldCursor;
            }

            return VSConstants.S_OK;
        }

        bool CreatingNewSolution = false;

        public int GetDisplayStringForAction(out string pbstrActionName)
        {
            CreatingNewSolution = true;
            pbstrActionName = Resources.P4VsProviderService_NewSlnDialogPromptString;
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsSelectionEvents Members

        public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            return VSConstants.S_OK;
        }

        public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            return VSConstants.S_OK;
        }

        public void UpdateChangesStatusBar(int change)
        {
            PendingChangeCount += change;
        }

        public void ResetSelection()
        {
            _selectedHierarchies = null;
            _selectedNodes = null;
            _selectedFiles = null;
            _isSolutionSelected = false;
            IsaControlledProjectSelected = false;
        }

        Hashtable _selectedHierarchies = null;
        IList<VSITEMSELECTION> _selectedNodes = null;
        bool _isSolutionSelected = false;

        private IList<string> _selectedFiles = null;
        private IList<string> _selectedFilesFolders = null;

        public IList<string> SelectedFiles
        {
            get
            {
                if (_selectedFiles == null)
                {
                    GetSelection();
                }
                if (_selectedFiles != null && (_isSolutionSelected && (_selectedFiles.Count == 1)))
                {
                    // after opening a solution, often the selection will only include the sln file and
                    // not any of the other files in the solution and projects, so update the selection
                    GetSelection();
                }
                return _selectedFiles;

            }
            internal set
            {
                if (value != null)
                {
                    _selectedFiles = value;
                }
                else
                {
                    _selectedFiles = new List<string>();
                }
            }
        }

        public IList<string> SelectedFilesFolders
        {
            get
            {
                if (_selectedFilesFolders == null)
                {
                    GetSelection();
                }
                return _selectedFilesFolders;

            }
            internal set
            {
                if (value != null)
                {
                    _selectedFilesFolders = value;
                }
                else
                {
                    _selectedFilesFolders = new List<string>();
                }
            }
        }

        public Hashtable SelectedHierarchies
        {
            get
            {
                if (_selectedHierarchies == null)
                {
                    GetSelection();
                }
                return _selectedHierarchies;
            }
            private set
            {
                if (value != null)
                {
                    _selectedHierarchies = value;
                }
                else
                {
                    _selectedHierarchies = new Hashtable();
                }
            }
        }

        public IList<VSITEMSELECTION> SelectedNodes
        {
            get
            {
                if (_selectedNodes == null)
                {
                    GetSelection();
                }
                return _selectedNodes;
            }
            private set
            {
                if (value != null)
                {
                    _selectedNodes = value;
                }
                else
                {
                    _selectedNodes = new List<VSITEMSELECTION>();
                }
            }
        }

        public bool IsaControlledProjectSelected { get; internal set; }

        public bool IsSolutionSelected
        {
            get { return _isSolutionSelected; }
            private set { _isSolutionSelected = value; }
        }

        public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
        {
            GetSelection();
            return VSConstants.S_OK;
        }

        public void GetSelection()
        {
            LogFunctionCall(MethodBase.GetCurrentMethod().Name);
            if (_P4VsProvider.InCommandLineMode())
            {
                return;
            }
            if ((ScmProvider == null) || (ScmProvider.Connection.Disconnected))
            {
                return;
            }
            try
            {
                _selectedFiles = _P4VsProvider.GetSelectedFilesInControlledProjectsInt(out _selectedFilesFolders, out _selectedNodes, out _selectedHierarchies, out _isSolutionSelected);

                for (int idx = 0; idx < _selectedFiles.Count; idx++)
                {
                    string file = _selectedFiles[idx];

                    if (string.IsNullOrEmpty(file))
                    {
                        _selectedFiles.RemoveAt(idx--);
                        continue;
                    }
                }
                _P4VsProvider.ResetCommandStatus();
                if (_selectedFiles.Count == 0)
                {
                    return;
                }

                if (Preferences.LocalSettings.GetBool("AutoUpdateFileData", false))
                {
                    bool LazyLoad = Preferences.LocalSettings.GetBool("LazyLoadStatus", true);

                    if (LazyLoad)
                    {
                        //lazy loading only refresh one file
                        List<string> files = new List<string>();
                        // find the first valid file name
                        for (int idx = 0; idx < _selectedFiles.Count; idx++)
                        {
                            if (string.IsNullOrEmpty(_selectedFiles[idx]) == false)
                            {
                                files.Add(_selectedFiles[idx]);
                                break;
                            }
                        }
                        if (files.Count > 0)
                        {
                            RefreshProjectGlyphs(files, true);
                        }
                    }
                    else
                    {
                        _P4VsProvider.Glyphs.RefreshNodesGlyphs(_selectedNodes, _selectedFiles);
                    }
                }

                SccHistoryToolWindow window = (SccHistoryToolWindow)_P4VsProvider.FindToolWindow(typeof(SccHistoryToolWindow), 0, false);

                if ((window != null) && window.isVisible)
                {
                    window.control.Files = _selectedFiles;
                }
            }
            catch (Exception ex)
            {
                logger.Trace("Exception in Helix SCM OnSelectionChanged {0}\r\n\t{1}", ex.Message, ex.StackTrace);
            }

            return;
        }

        #endregion

        public delegate void SetControlColorsDelegate();
        public event SetControlColorsDelegate SetControlColors;

        #region IVsPackageInstallerEvents Members

        //public delegate void VsPackageEventHandler();
        //public event VsPackageEventHandler PackageInstalling;

        //public void OnPackageInstalling()
        //{

        //}

        #endregion

#if VS2012
        #region IVsBroadcastMessageEvents Members

        public void OnThemeChanged(ThemeChangedEventArgs args)
        {
            if (SetControlColors != null)
            {
                foreach (SetControlColorsDelegate d in SetControlColors.GetInvocationList())
                {
                    try
                    {
                        d();
                    }
                    catch
                    {
                        SetControlColors -= d;
                    }
                }
            }
        }

        public bool IsItemUnderSCC(string ItemName)
        {
            if (ScmProvider != null)
            {
                ScmProvider.UpdateFileInCache(ItemName, true);
            }
            SourceControlStatus sourceControlStatus = GetFileStatus(ItemName);
            bool controlled = SourceControlStatus.scsUncontrolled != sourceControlStatus;
            return controlled;
        }

        public bool IsItemCheckedOut(string ItemName)
        {
            SourceControlStatus sourceControlStatus = GetFileStatus(ItemName);
            bool checkedOut = ((SourceControlStatus.scsCheckedOut & sourceControlStatus) != 0);
            return checkedOut;
        }

        public bool CheckOutItem(string ItemName)
        {
            // NuGet will make the file writeable, which will
            // interfere with the QueryEditFiles operation, so
            // change it back
            if (File.Exists(ItemName))
            {
                var attributes = File.GetAttributes(ItemName);
                if (!attributes.HasFlag(FileAttributes.ReadOnly))
                {
                    File.SetAttributes(ItemName, FileAttributes.ReadOnly);
                }
            }
            string[] rgpszMkDocuments = { ItemName };
            uint pfEditVerdict;
            uint prgfMoreInfo;
            int result = QueryEditFiles(0, 1, rgpszMkDocuments, null, null, out pfEditVerdict, out prgfMoreInfo);
            return (result == 0);
        }

        public bool CheckOutItems(ref object[] ItemNames)
        {
            throw new NotImplementedException();
        }

        public void ExcludeItem(string ProjectFile, string ItemName)
        {
            throw new NotImplementedException();
        }

        public void ExcludeItems(string ProjectFile, ref object[] ItemNames)
        {
            throw new NotImplementedException();
        }

        public DTE DTE => throw new NotImplementedException();

        public DTE Parent => throw new NotImplementedException();

        public void OnQueryRemoveFilesEx(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, uint[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            throw new NotImplementedException();
        }

        public void OnQueryRemoveDirectoriesEx(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, uint[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            throw new NotImplementedException();
        }

        public void OnAfterRemoveDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgFlags)
        {
            throw new NotImplementedException();
        }

        #endregion
#endif
    }
}
