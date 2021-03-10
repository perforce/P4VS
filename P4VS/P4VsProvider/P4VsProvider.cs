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
 * Name		: P4VsProvider.cs
 *
 * Author	: Duncan Barbee <dbarbee@perforce.com>
 *
 * Description	: Package class for P4VS
 *
 ******************************************************************************/

using System;
using System.IO;
using System.Resources;
using System.Diagnostics;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.CSharp;
using Microsoft.VisualStudio.Shell;
using NLog;
using EnvDTE;
using EnvDTE80;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;
using MsVsShell = Microsoft.VisualStudio.Shell;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
using Perforce.P4Scm;
using Microsoft.VisualStudio.AsyncPackageHelpers;

namespace Perforce.P4VS
{
	/////////////////////////////////////////////////////////////////////////////
	// P4VsProvider
	
#if VS2008
	[InstalledProductRegistration(false, "#100", "#101", "2011.1", IconResourceID=500)]
	[DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
	[ProvideLoadKey("Standard", "2011.1", "P4VS", "Perforce Software, Inc.", 104)]

	//internal class Images : Perforce.P4VS.Resources2010.Images { }
#elif VS2010
	[InstalledProductRegistration(true, null, null, null)]
	[DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\10.0Exp")]

	//internal class Images : Perforce.P4VS.Resources2010.Images { }
#elif VS2012
#pragma warning disable 618
	[InstalledProductRegistration(true, null, null, null)]
#pragma warning restore 618
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\11.0Exp")]

	//internal class Images : Perforce.P4VS.Resources2012.Images { }
#endif
    	
	// Register the package to have information displayed in Help/About dialog box
	// Declare that resources for the package are to be found in the managed assembly resources, and not in a satellite dll
	[PackageRegistration(UseManagedResourcesOnly = true)]

	// Register the resource ID of the CTMENU section (generated from compiling the VSCT file), so the IDE will know how to merge this package's menus with the rest of the IDE when "devenv /setup" is run
	// The menu resource ID needs to match the ResourceName number defined in the csproj project file in the VSCTCompile section
	// Every time the version number changes VS will automatically update the menus on startup; if the version doesn't change, you will need to run manually "devenv /setup /rootsuffix:Exp" to see VSCT changes reflected in IDE
	[ProvideMenuResource(1000, 1)]

	// Register a sample options page visible as Tools/Options/SourceControl/Perforce General Options when the provider is active
	[ProvideOptionPageAttribute(typeof(P4GeneralPreferences), "Source Control", "Perforce - General", 106, 107, false)]
	[ProvideToolsOptionsPageVisibility("Source Control", "Perforce - General", "FDA934F4-0492-4F67-A6EB-CBE0953649F0")]

	// Register a sample options page visible as Tools/Options/SourceControl/Perforce Connection Options when the provider is active
	[ProvideOptionPageAttribute(typeof(P4ConnectionPreferences), "Source Control", "Perforce - Connections", 106, 108, false)]
	[ProvideToolsOptionsPageVisibility("Source Control", "Perforce - Connections", "FDA934F4-0492-4F67-A6EB-CBE0953649F0")]

	// Register a sample options page visible as Tools/Options/SourceControl/Perforce Diff/Merge Options when the provider is active
	[ProvideOptionPageAttribute(typeof(P4DiffMergePreferences), "Source Control", "Perforce - Diff/Merge", 106, 109, false)]
	[ProvideToolsOptionsPageVisibility("Source Control", "Perforce - Diff/Merge", "FDA934F4-0492-4F67-A6EB-CBE0953649F0")]
	[ProvideOptionPageAttribute(typeof(P4LoggingPreferences), "Source Control", "Perforce - Logging", 106, 110, false)]
	[ProvideToolsOptionsPageVisibility("Source Control", "Perforce - Logging", "FDA934F4-0492-4F67-A6EB-CBE0953649F0")]
	[ProvideOptionPageAttribute(typeof(P4DataRetrievalPreferences), "Source Control", "Perforce - Data Retrieval", 106, 111, false)]
	[ProvideToolsOptionsPageVisibility("Source Control", "Perforce - Data Retrieval", "FDA934F4-0492-4F67-A6EB-CBE0953649F0")]
	[ProvideOptionPageAttribute(typeof(P4IgnorePreferences), "Source Control", "Perforce - Ignoring Files", 106, 112, false)]
	[ProvideToolsOptionsPageVisibility("Source Control", "Perforce - Ignoring Files", "FDA934F4-0492-4F67-A6EB-CBE0953649F0")]
	
	// Register a sample options page visible as Tools/Options/SourceControl/SampleOptionsPage when the provider is active
	//[ProvideOptionPageAttribute(typeof(P4Options), "Source Control", "Sample Options Page", 106, 107, false)]
	//[ProvideToolsOptionsPageVisibility("Source Control", "Sample Options Page", "FDA934F4-0492-4F67-A6EB-CBE0953649F0")]
	
	// Register a workspaces tool window visible only when the provider is active
	[ProvideToolWindow(typeof(WorkspaceToolWindow))]
	//[ProvideToolWindowVisibility(typeof(WorkspaceToolWindow), "FAF132F8-DA8E-45ED-BEB9-9ACF2E536425")]

	// Register a file history tool window SccHistoryToolWindow only when the provider is active
	[ProvideToolWindow(typeof(SccHistoryToolWindow))]
	//[ProvideToolWindowVisibility(typeof(SccHistoryToolWindow), "34386F7F-4CB3-4B5C-BC91-EBEED97A709B")]

	// Register a jobs tool window JobsToolWindow only when the provider is active
	[ProvideToolWindow(typeof(JobsToolWindow))]
	//[ProvideToolWindowVisibility(typeof(JobsToolWindow), "E3634C03-3AF6-48D3-9421-67DECFD845DC")]

	// Register a sample tool window P4ToolWindow only when the provider is active
	//[ProvideToolWindow(typeof(P4ToolWindow))]
	// [ProvideToolWindowVisibility(typeof(P4ToolWindow), "0623ABD4-9726-4138-946D-EE38871E1913")]

	// Register a submitted changelists tool window SubmittedChangelistsToolWindow only when the provider is active
	[ProvideToolWindow(typeof(SubmittedChangelistsToolWindow))]

	// Register a pending changelists tool window PendingChangelistsToolWindow only when the provider is active
	[ProvideToolWindow(typeof(PendingChangelistsToolWindow))]

	// Register a labels tool window LabelsToolWindow only when the provider is active
	[ProvideToolWindow(typeof(LabelsToolWindow))]

	// Register a swarm tool window SwarmToolWindow only when the provider is active
	[ProvideToolWindow(typeof(SwarmToolWindow))]

    // Register a labels tool window StreamsToolWindow only when the provider is active
    [ProvideToolWindow(typeof(StreamsToolWindow))]

	// Register a Reviews tool window ReviewsToolWindow only when the provider is active
	[ProvideToolWindow(typeof(SwarmReviewsToolWindow))]

	// Register the source control provider's service (implementing IVsScciProvider interface)
	[ProvideService(typeof(P4VsProviderService), ServiceName = "P4VS - Helix Plugin for Visual Studio")]

    // Register the source control provider to be visible in Tools/Options/SourceControl/Plugin dropdown selector
    [ProvideSourceControlProvider("P4VS - Helix Plugin for Visual Studio", "#100")]
    //[MsVsShell.ProvideSourceControlProvider("P4VS - Helix Plugin for Visual Studio", "#100",
    //       "{FDA934F4-0492-4F67-A6EB-CBE0953649F0}",
    //       "{8D316614-311A-48F4-85F7-DF7020F62357}",
    //       "{93C6B80C-A9E4-4F63-A605-51E7FCB9F906}", IsPublishSupported = false)]

    // Pre-load the package when the command UI context is asserted (the provider will be automatically loaded after restarting the shell if it was active last time the shell was shutdown)
    [Microsoft.VisualStudio.AsyncPackageHelpers.ProvideAutoLoad("FDA934F4-0492-4F67-A6EB-CBE0953649F0",PackageAutoLoadFlags.BackgroundLoad)]

    [Microsoft.VisualStudio.AsyncPackageHelpers.AsyncPackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Microsoft.VisualStudio.AsyncPackageHelpers.ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]


    // Register the key used for persisting solution properties, so the IDE will know to load the source control package when opening a controlled solution containing properties written by this package
    [ProvideSolutionProps(PersistSolutionProps._strSolutionPersistanceKey)]

#if !VS2012
   	// Declare the package guid
	[Guid("BBBB4F8F-5EDA-4623-8BAC-644EC6501F97")]
#else //VS2012
    // Declare the package guid
	[Guid("8D316614-311A-48F4-85F7-DF7020F62357")]
#endif
    public sealed class P4VsProvider : Action,
		IVsInstalledProduct,
		IOleCommandTarget, IVsShellPropertyEvents
    {
        private static Logger logger = null;    // Initialized in constructor for whole DLL
        
		private const string _strSolutionBindingsProperty = "SolutionBindings";

		// The guid of solution folders
		private Guid guidSolutionFolderProject = new Guid(0x2150e333, 0x8fdc, 0x42a3, 0x94, 0x74, 0x1a, 0x39, 0x56, 0xd4, 0x6d, 0xe8);

		private static P4VsProvider instance = null;
		public static P4VsProvider Instance { get { return instance; } }

		public static T Service<T>() where T : class { return (T)((instance != null) ? instance.GetService(typeof(T)) : null); }
		public static object Service(Type t) { return (instance != null) ? instance.GetService(t) : null; }

		public P4VsProvider()
		{
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            System.Diagnostics.Trace.WriteLine(string.Format("p4vs folder: {0}", assemblyFolder));
            try
            {
                // Useful for debugging logging. Note name of config needs to agree with vsixmanifest file which installs it.
                NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(assemblyFolder + "\\NLog.config", true);
                LogManager.ThrowExceptions = true;
                LogManager.EnableLogging();
                logger = LogManager.GetCurrentClassLogger();
                logger.Trace("Entering constructor for: {0}", this.ToString());
                LogManager.ThrowExceptions = false;
            } catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("P4VS Exception: {0}", ex.Message));
                System.Diagnostics.Trace.WriteLine(string.Format("P4VS Stack: {0}", ex.StackTrace));
                LogManager.ThrowExceptions = false;
                if (logger == null)
                {
                    logger = LogManager.GetCurrentClassLogger();
                    System.Diagnostics.Trace.WriteLine(string.Format("P4VS Logging will default to disabled"));
                }
            }

			// The provider implements the IVsPersistSolutionProps interface which is derived from IVsPersistSolutionOpts,
			// The base class Package also implements IVsPersistSolutionOpts, so we're overriding its functionality
			// Therefore, to persist user options in the suo file we will not use the set of AddOptionKey/OnLoadOptions/OnSaveOptions 
			// set of functions, but instead we'll use the IVsPersistSolutionProps functions directly.
		}

		/////////////////////////////////////////////////////////////////////////////
		// P4VsProvider Package Implementation
		#region Package Members

		public new Object GetService(Type serviceType)
		{
			return base.GetService(serviceType);
		}

        private uint _EventSinkCookie;
        protected override void Initialize()
		{
            try
            {
                // load the local settings
                Preferences.Initialize();
                Preferences.LocalSettings.Load(null);

                logger.Trace("Entering Initialize() of: {0}", this.ToString());
                base.Initialize();

                // Proffer the source control service implemented by the provider
                // The service provider implemented by the package
                SccService = new P4VsProviderService(this);
                ((IServiceContainer)this).AddService(typeof(P4VsProviderService), SccService, true);

                // Initilise Glyphs
                IVsHierarchy solHier = (IVsHierarchy)GetService(typeof(SVsSolution));
                Glyphs = new NodeGlyphs(SccService, solHier);

                // Add our command handlers for menu (commands must exist in the .vsct file)
                OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
                if (mcs != null)
                {
                    // View Menu commands
                    #region View Menu commands
                    // Workspaces ToolWindow Command
                    CommandID cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdViewWorkspaceToolWindow);
                    MenuCommand menuCmd = new MenuCommand(new EventHandler(P4VsViewWorkspaceToolWindow), cmd);
                    mcs.AddCommand(menuCmd);

                    // History ToolWindow
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdViewHistoryToolWindow);
                    menuCmd = new MenuCommand(new EventHandler(P4VsViewHistoryToolWindow), cmd);
                    mcs.AddCommand(menuCmd);

                    // Jobs ToolWindow
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdViewJobsToolWindow);
                    menuCmd = new MenuCommand(new EventHandler(P4VsViewJobsToolWindow), cmd);
                    mcs.AddCommand(menuCmd);

                    // P4 ToolWindow
                    //cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdViewP4ToolWindow);
                    //menuCmd = new MenuCommand(new EventHandler(P4VsViewP4ToolWindow), cmd);
                    //mcs.AddCommand(menuCmd);

                    // SubmittedChangelists ToolWindow
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdViewSubmittedChangelistsToolWindow);
                    menuCmd = new MenuCommand(new EventHandler(P4VsViewSubmittedChangelistsToolWindow), cmd);
                    mcs.AddCommand(menuCmd);

                    // PendingChangelists ToolWindow
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdViewPendingChangelistsToolWindow);
                    menuCmd = new MenuCommand(new EventHandler(P4VsViewPendingChangelistsToolWindow), cmd);
                    mcs.AddCommand(menuCmd);

                    // Labels ToolWindow
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdViewLabelsToolWindow);
                    menuCmd = new MenuCommand(new EventHandler(P4VsViewLabelsToolWindow), cmd);
                    mcs.AddCommand(menuCmd);

                    // Swarm ToolWindow
                    //cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdViewSwarmToolWindow);
                    //menuCmd = new MenuCommand(new EventHandler(P4VsViewSwarmToolWindow), cmd);
                    //mcs.AddCommand(menuCmd);

                    // Streams ToolWindow
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdViewStreamsToolWindow);
                    menuCmd = new MenuCommand(new EventHandler(P4VsViewStreamsToolWindow), cmd);
                    mcs.AddCommand(menuCmd);

                    // Reviews ToolWindow
                    //cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdViewReviewsToolWindow);
                    //menuCmd = new MenuCommand(new EventHandler(P4VsViewReviewsToolWindow), cmd);
                    //mcs.AddCommand(menuCmd);
                    #endregion

                    // Help Menu commands
                    #region Help Menu commands
                    // Help/P4VS Help
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdP4VSHelp);
                    menuCmd = new MenuCommand(new EventHandler(P4VsP4VSHelp), cmd);
                    mcs.AddCommand(menuCmd);

                    // Help/P4VS SystemInfo
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdP4VSSystemInfo);
                    menuCmd = new MenuCommand(new EventHandler(P4VsP4VSSystemInfo), cmd);
                    mcs.AddCommand(menuCmd);

                    #endregion

                    // File Menu commands
                    #region File Menu commands
                    // File/Open Solution in Perforce Depot
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdFileOpenInDepot);
                    menuCmd = new MenuCommand(new EventHandler(P4VsFileOpenInDepot), cmd);
                    mcs.AddCommand(menuCmd);

                    // File/Open Connection to a Perforce Depot
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdOpenConnection);
                    menuCmd = new MenuCommand(new EventHandler(P4VsOpenConnection), cmd);
                    mcs.AddCommand(menuCmd);

                    // File/Close Connection to the Perforce Depot
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdCloseConnection);
                    menuCmd = new MenuCommand(new EventHandler(P4VsCloseConnection), cmd);
                    mcs.AddCommand(menuCmd);

                    // Workspaces ToolWindow's ToolBar Command
                    //cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdToolWindowToolbarCommand);
                    //menuCmd = new MenuCommand(new EventHandler(P4VsWorkspaceToolWindowToolbarCommand), cmd);
                    //mcs.AddCommand(menuCmd);
                    #endregion

                    // Source control menu commands
                    #region File Menu commands
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdScmRefresh);
                    menuCmd = new MenuCommand(new EventHandler(P4VsScmRefresh), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdAddToSourceControl);
                    menuCmd = new MenuCommand(new EventHandler(P4VsAddToSourceControl), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdCheckin);
                    menuCmd = new MenuCommand(new EventHandler(P4VsCheckin), cmd);
                    mcs.AddCommand(menuCmd);

                    //cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdCheckout);
                    //menuCmd = new MenuCommand(new EventHandler(P4VsCheckout), cmd);
                    //mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdCheckout);
                    menuCmd = new OleMenuCommand(new EventHandler(P4VsCheckout), cmd);
                    //queryStatusMenuCommand.BeforeQueryStatus +=new EventHandler(OnBeforeQueryStatus);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdCheckoutProject);
                    menuCmd = new MenuCommand(new EventHandler(P4VsCheckoutEntireProjectOrSolution), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdCheckoutSolution);
                    menuCmd = new MenuCommand(new EventHandler(P4VsCheckoutEntireProjectOrSolution), cmd);
                    mcs.AddCommand(menuCmd);

                    //cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdUseSccOffline);
                    //menuCmd = new MenuCommand(new EventHandler(P4VsUseSccOffline), cmd);
                    //mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdReconcile);
                    menuCmd = new MenuCommand(new EventHandler(P4VsReconcileFiles), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdRevert);
                    menuCmd = new MenuCommand(new EventHandler(P4VsRevert), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdRevertUnchanged);
                    menuCmd = new MenuCommand(new EventHandler(P4VsRevertUnchanged), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdLock);
                    menuCmd = new MenuCommand(new EventHandler(P4VsLock), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdUnlock);
                    menuCmd = new MenuCommand(new EventHandler(P4VsUnlock), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdChangeFileType);
                    menuCmd = new MenuCommand(new EventHandler(P4VsChangeFileType), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdMoveToChangelist);
                    menuCmd = new MenuCommand(new EventHandler(P4VsMoveToChangelist), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdAddToIgnoreList);
                    menuCmd = new MenuCommand(new EventHandler(P4VsAddToIgnoreList), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdRemoveFromIgnoreList);
                    menuCmd = new MenuCommand(new EventHandler(P4VsRemoveFromIgnoreList), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdEditIgnoreList);
                    menuCmd = new MenuCommand(new EventHandler(P4VsEditIgnoreList), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdResolve);
                    menuCmd = new MenuCommand(new EventHandler(P4VsResolve), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdSync);
                    menuCmd = new MenuCommand(new EventHandler(P4VsSync), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdSyncHead);
                    menuCmd = new MenuCommand(new EventHandler(P4VsSyncHead), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdDiffVsHave);
                    menuCmd = new MenuCommand(new EventHandler(P4VsDiffVsHave), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdDiffVsAny);
                    menuCmd = new MenuCommand(new EventHandler(P4VsDiffVsAny), cmd);
                    mcs.AddCommand(menuCmd);

                    //cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdAddProjectToSCC);
                    //menuCmd = new MenuCommand(new EventHandler(P4VsAddProjectToSCC), cmd);
                    //mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdShowHistory);
                    menuCmd = new MenuCommand(new EventHandler(P4VsShowHistory), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdShelve);
                    menuCmd = new MenuCommand(new EventHandler(P4VsShelve), cmd);
                    mcs.AddCommand(menuCmd);

                    //cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdScmAttributes);
                    //menuCmd = new MenuCommand(new EventHandler(P4VsScmAttributes), cmd);
                    //mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdScmMerge);
                    menuCmd = new MenuCommand(new EventHandler(P4VsScmMerge), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdScmCopy);
                    menuCmd = new MenuCommand(new EventHandler(P4VsScmCopy), cmd);
                    mcs.AddCommand(menuCmd);
                    #endregion

                    // P4V commands
                    #region P4V Menu commands
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdP4V);
                    menuCmd = new MenuCommand(new EventHandler(P4VsP4V), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdTimeLapse);
                    menuCmd = new MenuCommand(new EventHandler(P4VsTimeLapse), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdRevGraph);
                    menuCmd = new MenuCommand(new EventHandler(P4VsRevGraph), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdStreamGraph);
                    menuCmd = new MenuCommand(new EventHandler(P4VsStreamGraph), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.cmdidCancelActiveCommand);
                    menuCmd = new MenuCommand(new EventHandler(Exec_CancelActiveCommand), cmd);
                    mcs.AddCommand(menuCmd);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.cmdidCurrentStream);
                    menuCmd = new MenuCommand(new EventHandler(Exec_CurrentStream), cmd);
                    mcs.AddCommand(menuCmd);
                    #endregion

                    // Status Bar commands
                    #region Status Bar commands
                    // Add to Source Control/Publish
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.icmdPublish);
                    menuCmd = new MenuCommand(new EventHandler(P4VsScmPublish), cmd);
                    mcs.AddCommand(menuCmd);
                    #endregion

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
                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, (int)CommandId.cmdidConnectionDropDownCombo);
                    omcConnectionComboSelChanged = new OleMenuCommand(new EventHandler(P4VsConnectionDropDownCombo), cmd);
                    omcConnectionComboSelChanged.ParametersDescription = "$"; // accept any argument string
                    mcs.AddCommand(omcConnectionComboSelChanged);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, (int)CommandId.cmdidConnectionDropDownComboGetList);
                    omcConnectionComboGetList = new OleMenuCommand(new EventHandler(P4VsConnectionDropDownComboGetList), cmd);
                    mcs.AddCommand(omcConnectionComboGetList);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, (int)CommandId.cmdidActiveChangelistDropDownCombo);
                    omcActiveChangelistComboSelChanged = new OleMenuCommand(new EventHandler(P4VsActiveChangelistDropDownCombo), cmd);
                    omcActiveChangelistComboSelChanged.ParametersDescription = "$"; // accept any argument string
                    mcs.AddCommand(omcActiveChangelistComboSelChanged);

                    cmd = new CommandID(GuidList.guidP4VsProviderCmdSet, (int)CommandId.cmdidActiveChangelistDropDownComboGetList);
                    omcActiveChangelistComboGetList = new OleMenuCommand(new EventHandler(P4VsActiveChangelistDropDownComboGetList), cmd);
                    mcs.AddCommand(omcActiveChangelistComboGetList);
                }
                
                // -- Set an eventlistener for shell property changes
                var shellService = GetService(typeof(SVsShell)) as IVsShell;
                if (shellService != null)
                {
                    ErrorHandler.ThrowOnFailure(shellService.AdviseShellPropertyChanges(this, out _EventSinkCookie));
                }

                // Register the provider with the source control manager
                // If the package is to become active, this will also callback on OnActiveStateChange and the menu commands will be enabled
                IVsRegisterScciProvider rscp = (IVsRegisterScciProvider)GetService(typeof(IVsRegisterScciProvider));
                rscp.RegisterSourceControlProvider(GuidList.guidP4VsProvider);

                instance = this;
            } catch (Exception ex)
            {
                logger.Error(ex, "Initialize failed");
                logger.Error("Error: {0}", ex.Message);
                logger.Error("Stacktrace: {0}", ex.StackTrace);
            }
		}

        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "ShowInitialToolbar",
                       string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));
        }
        OleMenuCommand omcConnectionComboGetList;
        
        OleMenuCommand omcActiveChangelistComboGetList;

		protected override void Dispose(bool disposing)
		{
            logger.Trace("Entering Dispose() of: {0}", this.ToString());

			SccService.Dispose();

			base.Dispose(disposing);
		}

		// This function is called by the IVsP4VsProvider service implementation when the active state of the provider changes
		// If the package needs to refresh UI or perform other tasks, this is a good place to add the code
		public void OnActiveStateChange()
		{
		}

#endregion

		//--------------------------------------------------------------------------------
		// IVsPersistSolutionProps specific functions
		//--------------------------------------------------------------------------------
		#region IVsPersistSolutionProps interface functions



		#endregion

		#region Source Control Commands Execution

	    public bool revertWarn()
		{
			if (Preferences.LocalSettings.ContainsKey("Revert_warn"))
			{
				if ((bool)Preferences.LocalSettings["Revert_warn"] == true)
				{
					return true;
				}
				return false;
			}
			return true;
		}

#if DEBUG
        public class KeyboardInfo
        {
            private KeyboardInfo() { }
            [DllImport("user32")]
            private static extern short GetKeyState(int vKey);
            public static KeyStateInfo GetKeyState(Keys key)
            {
                short keyState = GetKeyState((int)key);
                byte[] bits = BitConverter.GetBytes(keyState);
                bool toggled = bits[0] > 0, pressed = bits[1] > 0;
                return new KeyStateInfo(key, pressed, toggled);
            }
        }

        public struct KeyStateInfo
        {
            Keys _key;
            bool _isPressed,
                _isToggled;
            public KeyStateInfo(Keys key,
                            bool ispressed,
                            bool istoggled)
            {
                _key = key;
                _isPressed = ispressed;
                _isToggled = istoggled;
            }
            public static KeyStateInfo Default
            {
                get
                {
                    return new KeyStateInfo(Keys.None,
                                                false,
                                                false);
                }
            }
            public Keys Key
            {
                get { return _key; }
            }
            public bool IsPressed
            {
                get { return _isPressed; }
            }
            public bool IsToggled
            {
                get { return _isToggled; }
            }
        }
#endif

		internal static EnvDTE.DTE GetDTE()
		{
			if (P4VsProvider.Instance == null)
			{
				return null;
			}
			EnvDTE.DTE dte;
			dte = (EnvDTE.DTE)P4VsProvider.Instance.GetService(typeof(EnvDTE.DTE));

			return dte;
		}

		public void OnChangelistUpdated(object Sender, P4ScmProvider.ChangelistUpdateArgs Args)
		{
            if (Args.ChanglistId == ChangeLists.ActiveChangeList)
			{
				if ((Args.Action == P4ScmProvider.ChangelistUpdateArgs.UpdateType.Delete) ||
					(Args.Action == P4ScmProvider.ChangelistUpdateArgs.UpdateType.Submitted))
				{
					ChangeLists.SetActiveChangeList(0);
				}
			}
		}	

        private void Exec_CancelActiveCommand(object sender, EventArgs e)
        {
            KeepAliveMonitor.CancelAll();
		}

		private void Exec_CurrentStream(object sender, EventArgs e)
        {
			P4VsViewStreamsToolWindow(null, null);
		}

		#endregion

		#region Source Control Utility Functions


		/// <summary>
		/// Returns a list of source controllable files in the selection (recursive)
		/// </summary>
		internal IList<string> GetSelectedFilesInControlledProjects()
		{
			IList<VSITEMSELECTION> selectedNodes = null;
			return GetSelectedFilesInControlledProjects(out selectedNodes);
		}

		/// <summary>
		/// Returns a list of source controllable files in the selection (recursive)
		/// </summary>
		internal IList<string> GetSelectedFilesInControlledProjects(out IList<VSITEMSELECTION> selectedNodes)
		{
			bool isSolutionSelected = false;
			IList<string> _selectedFilesFolders;
			return GetSelectedFilesInControlledProjects(out _selectedFilesFolders, out selectedNodes, out  isSolutionSelected);
		}
		/// <summary>
		/// Returns a list of source controllable files in the selection (recursive)
		/// </summary>
		internal IList<string> GetSelectedFilesInControlledProjects(out IList<string> selectedFilesFolders, out IList<VSITEMSELECTION> selectedNodes, out bool isSolutionSelected)
		{
			selectedNodes = SccService.SelectedNodes;
			isSolutionSelected = SccService.IsSolutionSelected;
			selectedFilesFolders = SccService.SelectedFilesFolders; 
			return SccService.SelectedFiles;
		}

		/// <summary>
		/// Returns a list of source controllable files in the selection (recursive)
		/// </summary>
		internal IList<string> GetSelectedFilesInControlledProjectsInt(out IList<string> selectedFilesFolders, out IList<VSITEMSELECTION> selectedNodes)
		{
			bool isSolutionSelected;
			Hashtable hash;
			return GetSelectedFilesInControlledProjectsInt(out selectedFilesFolders, out selectedNodes, out hash, out isSolutionSelected);
		}

		/// <summary>
		/// Returns a list of source controllable files in the selection (recursive)
		/// </summary>
		internal IList<string> GetSelectedFilesInControlledProjectsInt(
			out IList<string> selectedFilesFolders, 
			out IList<VSITEMSELECTION> selectedNodes, 
			out bool isSolutionSelected)
		{
			Hashtable hash;
			return GetSelectedFilesInControlledProjectsInt( out selectedFilesFolders, out selectedNodes, out hash, out isSolutionSelected);
		}

		//IList<SelectedFile> SelectedFiles { get; set; }

		/// <summary>
		/// Returns a list of source controllable files in the selection (recursive)
		/// </summary>
		internal IList<string> GetSelectedFilesInControlledProjectsInt(
			out IList<string> selectedFilesFolders,
			out IList<VSITEMSELECTION> selectedNodes, 
			out Hashtable SelectedHierarchies, 
			out bool isSolutionSelected)
		{
			IDictionary<string, object> sccFiles = new Dictionary<string, object>();
			IDictionary<string, object> sccFilesFolders = new Dictionary<string, object>();

			//SelectedFiles = new List<SelectedFile>();

			selectedNodes = GetSelectedNodes();
			isSolutionSelected = false;
			SelectedHierarchies = GetSelectedHierarchies(ref selectedNodes, out isSolutionSelected);
			if (isSolutionSelected)
			{
				// Replace the selection with the root items of all controlled projects
				selectedNodes.Clear();
				SelectedHierarchies.Clear();

				Hashtable hashControllable = GetLoadedControllableProjectsEnum();
				foreach (IVsHierarchy pHier in hashControllable.Keys)
				{
					if (SccService.IsProjectControlled(pHier))
					{
						VSITEMSELECTION vsItemSelection;
						SelectedHierarchies[pHier] = true;
						vsItemSelection.pHier = pHier;
						vsItemSelection.itemid = VSConstants.VSITEMID_ROOT;
						selectedNodes.Add(vsItemSelection);
					}
				}

				// Add the solution file to the list
				if (SccService.IsProjectControlled(null))
				{
					IVsHierarchy solHier = (IVsHierarchy)GetService(typeof(SVsSolution));
					SelectedHierarchies[solHier] = true;
					VSITEMSELECTION vsItemSelection;
					vsItemSelection.pHier = solHier;
					vsItemSelection.itemid = VSConstants.VSITEMID_ROOT;
					selectedNodes.Add(vsItemSelection);
				}				
			}
			SccService.IsaControlledProjectSelected = false;

			// now look in the rest of selection and accumulate scc files
			foreach (VSITEMSELECTION vsItemSel in selectedNodes)
			{
				if (vsItemSel.itemid == VSConstants.VSITEMID_ROOT)
				{
					SccService.IsaControlledProjectSelected |= SccService.IsProjectControlled(vsItemSel.pHier);
				}
				IVsSccProject2 pscp2 = vsItemSel.pHier as IVsSccProject2;
				if (pscp2 == null)
				{
					string solutionFileName = GetSolutionFileName();

					// solution case
					sccFiles[solutionFileName] = null;

					string solutionFolder = Path.GetDirectoryName(solutionFileName);
					solutionFolder = string.Format("{0}\\...", solutionFolder);

					sccFilesFolders[solutionFolder] = true;
				}
				else
				{
					// if this is a project, add the projects directory to the list of selected folders
					string path = string.Empty;
					IVsProject pscp = pscp2 as IVsProject;
					if (pscp != null)
					{
						bool addSubFiles = false;

						if (isSolutionSelected == false)
						{
							//only add subpaths if the folder is not selected
							pscp.GetMkDocument(vsItemSel.itemid, out path);
                            if ((path!= null) && (vsItemSel.itemid == VSConstants.VSITEMID_ROOT))
							{
								// if it's a project
								path = Path.GetDirectoryName(path);
								path = string.Format("{0}\\...", path);
								sccFilesFolders[path] = null;
							}
                            else if ((path!= null) && path.EndsWith("\\"))
							{
								// if it's a folder
								path = string.Format("{0}...", path);
								sccFilesFolders[path] = null;
							}
							else
							{
								addSubFiles = true;
							}
						}
						IList<string> nodefilesrec = GetProjectFiles(pscp2, vsItemSel.itemid);
						foreach (string file in nodefilesrec)
						{
							sccFiles[file] = null;
							if (addSubFiles)
							{
								sccFilesFolders[file] = null;
							}
						}
					}
				}
			}
			selectedFilesFolders = sccFilesFolders.Keys.ToList();

			return sccFiles.Keys.ToList();
		}



		/// <summary>
		/// Checks whether the specified project is a solution folder
		/// </summary>
		public bool IsSolutionFolderProject(IVsHierarchy pHier)
		{
			IPersistFileFormat pFileFormat = pHier as IPersistFileFormat;
			if (pFileFormat != null)
			{
				Guid guidClassID;
				if (pFileFormat.GetClassID(out guidClassID) == VSConstants.S_OK &&
					guidClassID.CompareTo(guidSolutionFolderProject) == 0)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns a list of solution folders projects in the solution
		/// </summary>
		public Hashtable GetSolutionFoldersEnum()
		{
			Hashtable mapHierarchies = new Hashtable();

			IVsSolution sol = (IVsSolution)GetService(typeof(SVsSolution));
			Guid rguidEnumOnlyThisType = guidSolutionFolderProject;
			IEnumHierarchies ppenum = null;
			ErrorHandler.ThrowOnFailure(sol.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref rguidEnumOnlyThisType, out ppenum));

			IVsHierarchy[] rgelt = new IVsHierarchy[1];
			uint pceltFetched = 0;
			while (ppenum.Next(1, rgelt, out pceltFetched) == VSConstants.S_OK &&
				   pceltFetched == 1)
			{
				mapHierarchies[rgelt[0]] = true;
			}

			return mapHierarchies;
		}
	
		#endregion

		public int IdBmpSplash(out uint pIdBmp)
		{
			pIdBmp = 501;
			return Microsoft.VisualStudio.VSConstants.S_OK; 
		}

		public int IdIcoLogoForAboutbox(out uint pIdIco)
		{
			pIdIco = 500;// should be new icon
			return Microsoft.VisualStudio.VSConstants.S_OK; 
		}

		public int OfficialName(out string pbstrName)
		{
			pbstrName = Resources.P4VS_ProductName;
			return Microsoft.VisualStudio.VSConstants.S_OK; 
		}

		public int ProductDetails(out string pbstrProductDetails)
		{
			pbstrProductDetails = Resources.P4VS_ProductDescription;
			return Microsoft.VisualStudio.VSConstants.S_OK; 
		}

		public int ProductID(out string pbstrPID)
		{
            pbstrPID = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			return Microsoft.VisualStudio.VSConstants.S_OK; 
		}

		internal string GetProjectFileName(IVsHierarchy pRealHierarchy)
		{
			throw new NotImplementedException();
		}
        #region Implementation of IVsShellPropertyEvents

        public int OnShellPropertyChange(int propid, object propValue)
        {
            if (Preferences.LocalSettings.GetBool("P4_CONNECTION_TOOLBAR_ACTIVATED",false))
            {
                return VSConstants.S_OK;
            }
            // --- We handle the event if zombie state changes from true to false
            if ((int)__VSSPROPID.VSSPROPID_Zombie == propid)
            {
                if ((bool)propValue == false)
                {
                    // --- Show the commandbar
                    var dte = GetService(typeof(DTE)) as DTE2;
                    var cbs = ((CommandBars)dte.CommandBars);
                    CommandBar cb = cbs["Helix Connection"];
                    cb.Visible = true;

                    // --- Unsubscribe from events
                    var shellService = GetService(typeof(SVsShell)) as IVsShell;
                    if (shellService != null)
                    {
                        ErrorHandler.ThrowOnFailure(shellService.UnadviseShellPropertyChanges(_EventSinkCookie));
                    }
                    _EventSinkCookie = 0;

                    // set the pref that this has been force-set once
                    // if the user unsets it after that, we do not want to
                    // override that on every launch of Visual Studio
                    Preferences.LocalSettings["P4_CONNECTION_TOOLBAR_ACTIVATED"] = true;
                }
            }
            return VSConstants.S_OK;
        }

        #endregion

    }
}
