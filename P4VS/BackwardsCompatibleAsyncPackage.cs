using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.AsyncPackageHelpers;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Events;
using OpenMcdf;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace Perforce.P4VS
{
    //[Guid(GuidList.guidBackwardsCompatibleAsyncPkgString)]
    //[Microsoft.VisualStudio.AsyncPackageHelpers.AsyncPackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    //[Microsoft.VisualStudio.AsyncPackageHelpers.ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    public class BackwardsCompatibleAsyncPackage : Package, IAsyncLoadablePackageInitialize
    {
        private bool isAsyncLoadSupported;

        /// <summary>
        /// Initialization of the package; this method is always called right after the package is sited on main UI thread of Visual Studio.
        /// 
        /// Both asynchronuos package and synchronous package loading will call this method initially so it is important to skip any initialization
        /// meant for async load phase based on AsyncPackage support in IDE.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            isAsyncLoadSupported = this.IsAsyncPackageSupported();

            // Only perform initialization if async package framework is not supported
            if (!isAsyncLoadSupported)
            {
                IVsUIShell shellService = this.GetService(typeof(SVsUIShell)) as IVsUIShell;
                this.MainThreadInitialization(shellService, isAsyncPath: false);
            }
        }


        /// <summary>
        /// Performs the asynchronous initialization for the package in cases where IDE supports AsyncPackage.
        /// 
        /// This method is always called from background thread initially.
        /// </summary>
        /// <param name="asyncServiceProvider">Async service provider instance to query services asynchronously</param>
        /// <param name="pProfferService">Async service proffer instance</param>
        /// <param name="IAsyncProgressCallback">Progress callback instance</param>
        /// <returns></returns>
        public IVsTask Initialize(IAsyncServiceProvider asyncServiceProvider, IProfferAsyncService pProfferService, IAsyncProgressCallback pProgressCallback)
        {
            if (!isAsyncLoadSupported)
            {
                throw new InvalidOperationException("Async Initialize method should not be called when async load is not supported.");
            }

            return ThreadHelper.JoinableTaskFactory.RunAsync<object>(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                IVsUIShell shellService = await asyncServiceProvider.GetServiceAsync<IVsUIShell>(typeof(SVsUIShell));
                this.MainThreadInitialization(shellService, isAsyncPath: true);
                return null;
            }).AsVsTask();
        }

        private void MainThreadInitialization(IVsUIShell shellService, bool isAsyncPath)
        {
            // Do operations requiring main thread utilizing passed in services
            if (isAsyncPath)
            {

                var solService = GetService(typeof(SVsSolution)) as IVsSolution;
                string dir = "";
                string file = "";
                string optsFile = "";
                solService.GetSolutionInfo(out dir, out file, out optsFile);

                if (string.IsNullOrEmpty(optsFile))
                {
                    return;
                }

                try
                {
                    FileStream fs = new FileStream(
                    optsFile,
                    FileMode.Open,
                    FileAccess.ReadWrite
                    );

                    CompoundFile cf = new CompoundFile(fs, CFSUpdateMode.Update, CFSConfiguration.SectorRecycle | CFSConfiguration.NoValidationException | CFSConfiguration.EraseFreeSectors);
                    IList<CFItem> list = cf.GetAllNamedEntries(PersistSolutionProps._strSolutionUserOptionsKey);

                    if (list != null && list.Count > 0)
                    {
                        // while checking the solution user options, get the port,
                        // user, and workspace. This needs to be done here as ReadUserOptions
                        // is no longer triggered on .sln load if Visual Studio is launched
                        // by opening a .sln.
                        CFStream foundStream = cf.RootStorage.GetStream(PersistSolutionProps._strSolutionUserOptionsKey);
                        byte[] data = foundStream.GetData();
                        Stream stream = new MemoryStream(data);

                        Hashtable hashProjectsUserData = new Hashtable();
                        if (stream.Length > 0)
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            hashProjectsUserData = formatter.Deserialize(stream) as Hashtable;
                        }

                        P4VsProviderService P4VSService = (P4VsProviderService)GetService(typeof(P4VsProviderService));

                        if (hashProjectsUserData != null && (hashProjectsUserData.ContainsKey("Connect_State") && (hashProjectsUserData["Connect_State"] != null)))
                        {
                            if (hashProjectsUserData.ContainsKey("Connect_Port"))
                            {
                                P4VSService.port = hashProjectsUserData["Connect_Port"] as string;
                            }
                            if (hashProjectsUserData.ContainsKey("Connect_User"))
                            {
                                P4VSService.user = hashProjectsUserData["Connect_User"] as string;
                            }
                            if (hashProjectsUserData.ContainsKey("Connect_Workspace"))
                            {
                                P4VSService.workspace = hashProjectsUserData["Connect_Workspace"] as string;
                            }
                            if (string.IsNullOrEmpty(P4VSService.LoadingControlledSolutionLocation))
                            {
                                P4VSService.LoadingControlledSolutionLocation = P4VSService.port;
                            }
                        }

                        // check to see if the sln is tagged
                        string sln = System.IO.File.ReadAllText(file);
                        P4VSService._P4VsProvider.SolutionFileTagged = sln.Contains("GlobalSection(" +
                            PersistSolutionProps._strSolutionPersistanceKey +") = preSolution\r\n\t\t" +
                            PersistSolutionProps._strSolutionControlledProperty + " = True\r\n\tEndGlobalSection");

                        P4VSService.AsyncLoad = true;
                        P4VSService.OnAfterOpenSolution(null, 0);
                    }
                    cf.Close();
                }
                catch
                {
                    // If we are here, most likely the .suo file was not found, or could not
                    // be opened, so just continue without attempting to get user settings.
                }
            }
        }
    }
}
