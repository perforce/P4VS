using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Perforce.P4VS
{
    public abstract class PersistSolutionProps : Connection, IVsPersistSolutionProps
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // The name of the solution section used to persist provider options (should be unique)
        public const string _strSolutionPersistanceKey = "PerforceSourceControlProviderSolutionProperties";

        // The names of the properties stored by the provider in the solution file
        internal const string _strSolutionControlledProperty = "SolutionIsControlled";

        // The name of the section in the solution user options file used to persist user-specific options (should be unique, shorter than 31 characters and without dots)
        internal const string _strSolutionUserOptionsKey = "PerforceSourceControlProvider";

#if !VS2012
		// The name of this provider (to be written in solution and project files)
		// As a best practice, to be sure the provider has an unique name, a guid like the provider guid can be used as a part of the name
		private const string _strProviderName = "Perforce Source Control Provider:{BBBB4F8F-5EDA-4623-8BAC-644EC6501F97}";
#else  //VS2012
        // The name of this provider (to be written in solution and project files)
        // As a best practice, to be sure the provider has an unique name, a guid like the provider guid can be used as a part of the name
        private const string _strProviderName = "Perforce Source Control Provider:{8D316614-311A-48F4-85F7-DF7020F62357}";
#endif

        // IVsPersistSolutionProps::LoadUserOptions
        public int LoadUserOptions([In] IVsSolutionPersistence pPersistence, [In] uint grfLoadOpts)
        {
            // This function gets called by the shell when a solution is opened and the SUO file is read.
            // Note this can be during opening a new solution, or may be during merging of 2 solutions.
            // The provider calls the shell back to let it know which options keys from the suo file were written by this provider.
            // If the shell will find in the suo file a section that belong to this package, it will create a stream, 
            // and will call back the provider on IVsPersistSolutionProps.ReadUserOptions() to read specific options 
            // under that option key.
            pPersistence.LoadPackageUserOpts(this, _strSolutionUserOptionsKey);
            return VSConstants.S_OK;
        }

        // IVsPersistSolutionProps::OnProjectLoadFailure
        public int OnProjectLoadFailure([In] IVsHierarchy pStubHierarchy, [In] string pszProjectName, [In] string pszProjectMk, [In] string pszKey)
        {
            return VSConstants.S_OK;
        }

        // IVsPersistSolutionProps::QuerySaveSolutionProps
        public int QuerySaveSolutionProps([In] IVsHierarchy pHierarchy, [Out] VSQUERYSAVESLNPROPS[] pqsspSave)
        {
            // This function is called by the IDE to determine if something needs to be saved in the solution.
            // If the package returns that it has dirty properties, the shell will callback on SaveSolutionProps

            // We will write solution properties only for the solution
            // A provider may consider writing in the solution project-binding-properties for each controlled project
            // that could help it locating the projects in the store during operations like OpenFromSourceControl
            if (!SccService.IsProjectControlled(null))
            {
                pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasNoProps;
            }
            else
            {
                if (SolutionHasDirtyProps)
                {
                    pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasDirtyProps;
                }
                else
                {
                    pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasNoDirtyProps;
                }
            }

            return VSConstants.S_OK;
        }

        // IVsPersistSolutionProps::ReadSolutionProps
        public int ReadSolutionProps([In] IVsHierarchy pHierarchy, [In] string pszProjectName, [In] string pszProjectMk, [In] string pszKey, [In] int fPreLoad, [In] IPropertyBag pPropBag)
        {
            // This function gets called by the shell when a solution controlled by this provider is opened in IDE.
            // The shell encounters the _strSolutionPersistanceKey section in the solution, and based based on 
            // registration info written by ProvideSolutionProps identifies this package as the section owner, 
            // loads this package if necessary and calls the package to read the persisted solution options.
            logger.Debug("P4VS Startup in ReadSolutionProps");
            SolutionFileTagged = false;

            if (_strSolutionPersistanceKey.CompareTo(pszKey) == 0)
            {
                SolutionFileTagged = true;

                // We were called to read the key written by this source control provider
                // First thing to do: register the source control provider with the source control manager.
                // This allows the scc manager to switch the active source control provider if necessary,
                // and set this provider active; the provider will be later called to provide source control services 
                // for this solution.
                // (This is how automatic source control provider switching on solution opening is implemented)
                IVsRegisterScciProvider rscp = (IVsRegisterScciProvider)GetService(typeof(IVsRegisterScciProvider));
                rscp.RegisterSourceControlProvider(GuidList.guidP4VsProvider);

                // Now we can read all the data and store it in memory
                // The read data will be used when the solution has completed opening
                object pVar;
                pPropBag.Read(_strSolutionControlledProperty, out pVar, null, 0, null);
                if (pVar.ToString().CompareTo(true.ToString()) == 0)
                {
                    try
                    {
                        SccService.LoadingControlledSolutionLocation = pVar.ToString();
                    }
                    catch { }
                }
            }
            return VSConstants.S_OK;
        }

        // IVsPersistSolutionProps::ReadUserOptions
        public int ReadUserOptions([In] IStream pOptionsStream, [In] string pszKey)
        {
            // This function is called by the shell if the _strSolutionUserOptionsKey section declared
            // in LoadUserOptions() as being written by this package has been found in the suo file. 
            // Note this can be during opening a new solution, or may be during merging of 2 solutions.
            // A good source control provider may need to persist this data until OnIVsSccProject2ution or OnAfterMergeSolution is called

            // The easiest way to read/write the data of interest is by using a binary formatter class
            DataStreamFromComStream pStream = new DataStreamFromComStream(pOptionsStream);
            Hashtable hashProjectsUserData = new Hashtable();
            if (pStream.Length > 0)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                hashProjectsUserData = formatter.Deserialize(pStream) as Hashtable;
            }

            string port = null;
            string user = null;
            string workspace = null;

            if (hashProjectsUserData != null && (hashProjectsUserData.ContainsKey("Connect_State") && (hashProjectsUserData["Connect_State"] != null)))
            {
                if (hashProjectsUserData.ContainsKey("Connect_Port"))
                {
                    port = hashProjectsUserData["Connect_Port"] as string;
                }
                if (hashProjectsUserData.ContainsKey("Connect_User"))
                {
                    user = hashProjectsUserData["Connect_User"] as string;
                }
                if (hashProjectsUserData.ContainsKey("Connect_Workspace"))
                {
                    workspace = hashProjectsUserData["Connect_Workspace"] as string;
                }
                if (string.IsNullOrEmpty(SccService.LoadingControlledSolutionLocation))
                {
                    SccService.LoadingControlledSolutionLocation = port;
                }
            }

            ConnectToScm(port, user, workspace);

            return VSConstants.S_OK;
        }

        // IVsPersistSolutionProps::SaveSolutionProps
        public int SaveSolutionProps([In] IVsHierarchy pHierarchy, [In] IVsSolutionPersistence pPersistence)
        {
            // This function gets called by the shell after determining the package has dirty props.
            // The package will pass in the key under which it wants to save its properties, 
            // and the IDE will call back on WriteSolutionProps

            // The properties will be saved in the Pre-Load section
            // When the solution will be reopened, the IDE will call our package to load them back before the projects in the solution are actually open
            // This could help if the source control package needs to persist information like projects translation tables, that should be read from the suo file
            // and should be available by the time projects are opened and the shell start calling IVsSccEnlistmentPathTranslation functions.
            pPersistence.SavePackageSolutionProps(1, null, this, _strSolutionPersistanceKey);

            // Once we saved our props, the solution is not dirty anymore
            SolutionHasDirtyProps = false;

            return VSConstants.S_OK;
        }

        // IVsPersistSolutionProps::SaveUserOptions
        public int SaveUserOptions([In] IVsSolutionPersistence pPersistence)
        {
            // This function gets called by the shell when the SUO file is saved.
            // The provider calls the shell back to let it know which options keys it will use in the suo file.
            // The shell will create a stream for the section of interest, and will call back the provider on 
            // IVsPersistSolutionProps.WriteUserOptions() to save specific options under the specified key.
            int pfResult = 0;
            SccService.AnyItemsUnderSourceControl(out pfResult);
            if (pfResult > 0)
            {
                pPersistence.SavePackageUserOpts(this, _strSolutionUserOptionsKey);
            }
            return VSConstants.S_OK;
        }

        // IVsPersistSolutionProps::WriteSolutionProps
        public int WriteSolutionProps([In] IVsHierarchy pHierarchy, [In] string pszKey, [In] IPropertyBag pPropBag)
        {
            // The package will only save one property in the solution, to indicate that solution is controlled

            // A good provider may need to persist as solution properties the controlled status of projects and their locations, too.
            // If an operation like OpenFromSourceControl has sense for the provider, and the user has selected to open from 
            // source control the solution file, the bindings written as solution properties will help identifying where the 
            // project files are in the source control database. The source control provider can download the project files 
            // before they are needed by the IDE to be opened.

            if (Preferences.LocalSettings.GetBool("TagSolutionProjectFiles", false))
            {
                string strControlled = true.ToString();
                object obj = strControlled;

                pPropBag.Write(_strSolutionControlledProperty, ref obj);
            }
            return VSConstants.S_OK;
        }

        // IVsPersistSolutionProps::WriteUserOptions
        public int WriteUserOptions([In] IStream pOptionsStream, [In] string pszKey)
        {
            // This function gets called by the shell to let the package write user options under the specified key.
            // The key was declared in SaveUserOptions(), when the shell started saving the suo file.
            Debug.Assert(pszKey.CompareTo(_strSolutionUserOptionsKey) == 0, "The shell called to read an key that doesn't belong to this package");

            Hashtable hashProjectsUserData = new Hashtable();

            if ((SccService != null) && (SccService.ScmProvider != null))
            {
                hashProjectsUserData["Connect_State"] = SccService.ScmProvider.Connected ? "Connected" : null;
                hashProjectsUserData["Connect_Port"] = SccService.ScmProvider.Connection.Port;
                hashProjectsUserData["Connect_User"] = SccService.ScmProvider.Connection.User;
                hashProjectsUserData["Connect_Workspace"] = SccService.ScmProvider.Connection.Workspace;
            }
            // The easiest way to read/write the data of interest is by using a binary formatter class
            // This way, we can write a map of information about projects with one call 
            // (each element in the map needs to be serializable though)
            // The alternative is to write binary data in any byte format you'd like using pOptionsStream.Write
            DataStreamFromComStream pStream = new DataStreamFromComStream(pOptionsStream);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(pStream, hashProjectsUserData);

            return VSConstants.S_OK;
        }

        // Returns the name of the source control provider
        public string ProviderName
        {
            get { return _strProviderName; }
        }
    }
}
