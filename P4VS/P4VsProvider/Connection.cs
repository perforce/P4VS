using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Perforce.P4Scm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perforce.P4VS
{
    public abstract class Connection : Solution
    {
        
        public delegate void NewConnectionDelegate(P4ScmProvider newScm);
        public static event NewConnectionDelegate NewConnection;

        public OleMenuCommand omcConnectionComboSelChanged;
        public OleMenuCommand omcActiveChangelistComboSelChanged;

        public string currentConnectionDropDownComboChoice = Resources.ConnectionDropDownCombo_NoConnection;

        // Used to open a connection to a Perforce depot
        public void OpenConnection(string Port, string User, string Workspace)
        {
#if DB_DEBUG
			P4VsOutputWindow.AppendMessage(string.Format("Opening connection, CurrentScm scm ID:{0}",
				CurrentScm!=null?CurrentScm.__Id:-1));
#endif

            P4ScmProvider scm = new P4ScmProvider(SccService);
            scm.LoadingSolution = !string.IsNullOrEmpty(SccService.LoadingControlledSolutionLocation);

#if DB_DEBUG
			P4VsOutputWindow.AppendMessage(string.Format("Opening connection, new scm ID:{0}", scm.__Id));
#endif
            bool noUi = InCommandLineMode() || (Port != null) && (User != null) && (Workspace != null);

            // trigger the connection dialog
            scm.Connection.Connect(Port, User, Workspace, noUi, null);
            if (scm.Connected)
            {
#if DB_DEBUG
				P4VsOutputWindow.AppendMessage(string.Format("Opening connection user clicked OK, new scm ID:{0}", scm.__Id));
#endif
                if (SccService.ScmProvider != null)
                {
                    BroadcastNewConnection(null);
                    SccService.Dispose();
                    SccService.ScmProvider = null;
                }
                SccService.ScmProvider = scm;

                createActiveChangelists(SccService);

                if (string.IsNullOrEmpty(SccService.ScmProvider.SolutionFile))
                {
                    string solutionFile = GetSolutionFileName();
                    if (string.IsNullOrEmpty(solutionFile) == false)
                    {
                        SccService.ScmProvider.SolutionFile = GetSolutionFileName();

                        if (Preferences.LocalSettings.GetBool("TagSolutionProjectFiles", false))
                        {
                            // Need to tag sln file if tagging is enabled, so mark dirty props so
                            // it'll get tagged
                            SolutionHasDirtyProps = true;
                        }
                    }

                    if (!SccService.IsProjectControlled(null))
                    {
                        SccService.OnAfterOpenSolution(null, 0);
                    }
                }
                Cursor oldCursor = Cursor.Current;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    // now refresh the selected nodes' glyphs
                    if (Preferences.LocalSettings.GetBool("LazyLoadStatus", false) == false)
                    {
                        Glyphs.RefreshNodesGlyphs(null, null);
                    }
                }
                finally
                {
                    Cursor.Current = oldCursor;
                }

                BroadcastNewConnection(SccService.ScmProvider);

                MRUList recentConnections = (MRUList)Preferences.LocalSettings["RecentConnections"];
                currentConnectionDropDownComboChoice = recentConnections[0].ToString();

                SuppressConnection = true;
                LastConnectionInfo = new ConnectionData();
                LastConnectionInfo.ServerPort = SccService.ScmProvider.Connection.Port;
                LastConnectionInfo.UserName = SccService.ScmProvider.Connection.User;
                LastConnectionInfo.Workspace = SccService.ScmProvider.Connection.Workspace;

#if _DBB_DEBUG
				// How to set a setting for the ide (Generally from tool/options dialog)
				// The category and page values are listed on MSDN at:
				// http://msdn.microsoft.com/en-us/library/ms165643(v=vs.100).aspx
				// The category and pages no longer follow the layout in the options dialog,
				// they seem to still follow the old layout fro VS 2003. There's a general 
				// discussion on MSDN about properties here:
				// http://msdn.microsoft.com/en-us/library/ms165641(v=vs.100).aspx
				// and:
				// http://msdn.microsoft.com/en-us/library/awdwz11a(v=vs.100).aspx
				// The Property object is documented here:
				// http://msdn.microsoft.com/en-us/library/envdte.property(v=vs.100).aspx
				// The Properties interface is documented here:
				// http://msdn.microsoft.com/en-us/library/envdte.properties(v=vs.100).aspx
				try
				{
					EnvDTE.DTE dte2;
					dte2 = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
					EnvDTE.Properties generalPnS = dte2.DTE.Properties["Environment", "ProjectsAndSolution"];

					EnvDTE.Property prop2 = generalPnS.Item("ProjectsLocation");
					string val = prop2.Value as string;

					//dynamic v2 = val + "\\temp";
					prop2.Value = val;

					val = prop2.Value as string;
				}
				catch (Exception ex)
				{
					string msg = ex.Message;
					MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
#endif
                // How to set a setting for the ide (Generally from tool/options dialog)
                // The category and page values are listed on MSDN at:
                // http://msdn.microsoft.com/en-us/library/ms165643(v=vs.100).aspx
                // The category and pages no longer follow the layout in the options dialog,
                // they seem to still follow the old layout fro VS 2003. There's a general 
                // discussion on MSDN about properties here:
                // http://msdn.microsoft.com/en-us/library/ms165641(v=vs.100).aspx
                // and:
                // http://msdn.microsoft.com/en-us/library/awdwz11a(v=vs.100).aspx
                // The Property object is documented here:
                // http://msdn.microsoft.com/en-us/library/envdte.property(v=vs.100).aspx
                // The Properties interface is documented here:
                // http://msdn.microsoft.com/en-us/library/envdte.properties(v=vs.100).aspx

                if (Preferences.LocalSettings.GetBool("SetProjectFileLocation", true) &&
                    !string.IsNullOrEmpty(scm.Connection.WorkspaceRoot))
                {
                    try
                    {
                        EnvDTE.Property prop = null;
                        EnvDTE.DTE dte2;
                        dte2 = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));

                        EnvDTE.Properties generalPnS = dte2.get_Properties("Environment", "ProjectsAndSolution");
                        foreach (EnvDTE.Property temp in generalPnS)
                        {
                            prop = temp;
                            if (prop.Name == "ProjectsLocation")
                            {
                                if (Directory.Exists(scm.Connection.WorkspaceRoot))
                                {
                                    prop.Value = scm.Connection.WorkspaceRoot.Replace("/","\\");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
#if DB_DEBUG
			P4VsOutputWindow.AppendMessage(string.Format("Leaving open connection, CurrentScm scm ID:{0}",
				CurrentScm != null ? CurrentScm.__Id : -1));
#endif
        }

        private void createActiveChangelists(P4VsProviderService P4Service)
        {
            string key = "ActiveChangelist_" + P4Service.ScmProvider.Connection.Repository.Connection.Server.Address.Uri.Replace(':', '_') +
                       "_" + P4Service.ScmProvider.Connection.User + "_" + P4Service.ScmProvider.Connection.Workspace;

            ChangeLists = new ActiveChangeListCombo(P4Service);
            string newActiveChangeListComboChoice = Preferences.LocalSettings.GetString(key, Resources.Changelist_Default);
            ChangeLists.SetActiveChangeList(newActiveChangeListComboChoice);
        }

        // Used to open a connection to a Perforce depot
        public void CloseConnection()
        {
            if (SccService.ScmProvider != null)
            {
                BroadcastNewConnection(null);
                SccService.ScmProvider.Dispose();
                SccService.ScmProvider = null;
            }
            SuppressConnection = false;
            SccService.ScmProvider = null;

            BroadcastNewConnection(SccService.ScmProvider);

            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // now refresh the selected nodes' glyphs
                Glyphs.RefreshNodesGlyphs(null, null);

                omcConnectionComboSelChanged.Invoke(Resources.ConnectionDropDownCombo_NoConnection, IntPtr.Zero);
                omcActiveChangelistComboSelChanged.Invoke(Resources.Changelist_Default, IntPtr.Zero);

            }
            catch (Exception ex)
            {
                FileLogger.LogException("P4VsProvider.CloseConnection", ex);
            }
            finally
            {
                Cursor.Current = oldCursor;
            }
        }

        internal static void BroadcastNewConnection(P4ScmProvider scm)
        {
            P4VsProvider.Instance.SccService.RefreshEventSubscriptions();
            CurrentScm = scm;

            P4VsProvider.Instance.ClientStream.clearClient();

            if (NewConnection != null)
            {
                foreach (Delegate d in NewConnection.GetInvocationList())
                {
                    try
                    {
                        NewConnectionDelegate nd = (NewConnectionDelegate)d;
                        nd.DynamicInvoke(scm);
                    }
                    catch
                    {
#if DEBUG
                        try
                        {
                            NewConnectionDelegate nd = (NewConnectionDelegate)d;
                            nd.DynamicInvoke(scm);
                        }
                        catch
                        {
                        }
#endif
                        NewConnection -= (NewConnectionDelegate)d;
                    }
                }
                if (scm==null)
                {
                    P4VsProvider.Instance.currentConnectionDropDownComboChoice=Resources.ConnectionDropDownCombo_NoConnection;
                }
            }
        }

        public bool SuppressConnection { get; set; }
        public ConnectionData LastConnectionInfo { get; protected set; }

        internal void ConnectToScm(string port, string user, string workspace)
        {
            if (SuppressConnection)
            {
                SuppressConnection = false;
                if ((SccService.ScmProvider.Connection.Disconnected) && (LastConnectionInfo != null))
                {
                    if (SccService.ScmProvider != null)
                    {
                        BroadcastNewConnection(null);
                        SccService.ScmProvider.Dispose();
                        SccService.ScmProvider = null;
                    }
                    SccService.ScmProvider = new P4ScmProvider(SccService);
                    SccService.ScmProvider.Connection.Connect(LastConnectionInfo);
                    SccService.ScmProvider.LoadingSolution = !string.IsNullOrEmpty(SccService.LoadingControlledSolutionLocation);
                    ChangeLists = new ActiveChangeListCombo(SccService);
                }
                if (SccService.ScmProvider.Connected)
                {
                    return;
                }
            }
            bool showConnectDlg = false;

            ConnectionPreference pref = ConnectionPreference.ShowDialog;
            if (Preferences.LocalSettings.ContainsKey("ConnectPreference"))
            {
                pref = (ConnectionPreference)Preferences.LocalSettings["ConnectPreference"];
            }
            string _port = null;
            string _user = null;
            string _workspace = null;

            switch (pref)
            {
                default:
                case ConnectionPreference.ShowDialog:
                    showConnectDlg = true;
                    break;
                case ConnectionPreference.UseRecent:
                    if ((SccService.ScmProvider != null) && (SccService.ScmProvider.CheckConnection()))
                    {
                        //already connected
                        return;
                    }
                    MRUList _recentConnections = (MRUList)Preferences.LocalSettings["RecentConnections"];
                    if (_recentConnections != null)
                    {
                        ConnectionData con = _recentConnections[0] as ConnectionData;
                        if (con != null)
                        {
                            _port = con.ServerPort;
                            _user = con.UserName;
                            _workspace = con.Workspace;
                        }
                    }
                    else
                    {
                        // no recent connections
                        showConnectDlg = true;
                    }
                    break;
                case ConnectionPreference.UseSolution:
                    if (string.IsNullOrEmpty(port) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(workspace))
                    {
                        // didn't contain all the connection data, so show dialog
                        showConnectDlg = true;
                    }
                    else
                    {
                        _port = port;
                        _user = user;
                        _workspace = workspace;
                    }
                    break;
                case ConnectionPreference.UseEnvironment:
                    _port = null;
                    _user = null;
                    _workspace = null;
                    break;
            }
            if (SccService.ScmProvider != null)
            {
                BroadcastNewConnection(null);
                SccService.ScmProvider.Dispose();
                SccService.ScmProvider = null;
            }
            SccService.ScmProvider = new P4ScmProvider(SccService);
            SccService.ScmProvider.LoadingSolution = !string.IsNullOrEmpty(SccService.LoadingControlledSolutionLocation);
            ChangeLists = new ActiveChangeListCombo(SccService);

            if ((showConnectDlg == false) || (InCommandLineMode()))
            {
                SccService.ScmProvider.Connection.Connect(_port, _user, _workspace, true, Path.GetDirectoryName(GetSolutionFileName()));
            }
            else if ((showConnectDlg || SccService.ScmProvider.Connection.Disconnected) && (!InCommandLineMode()))
            {
                SccService.ScmProvider.Connection.Connect();
            }
            if ((SolutionFileTagged == false) &&
                (Preferences.LocalSettings.GetBool("TagSolutionProjectFiles", false)))
            {
                // Need to tag sln file if tagging is enabled, so mark dirty props so
                // it'll get tagged
                SolutionHasDirtyProps = true;
            }

            BroadcastNewConnection(SccService.ScmProvider);

            if (SccService.ScmProvider.Connected)
            {
                ConnectionData cd = new ConnectionData();
                cd.ServerPort = SccService.ScmProvider.Connection.Port;
                cd.UserName = SccService.ScmProvider.Connection.User;
                cd.Workspace = SccService.ScmProvider.Connection.Workspace;

                MRUList recentConnections = (MRUList)Preferences.LocalSettings["RecentConnections"];

                if (recentConnections == null)
                {
                    recentConnections = new MRUList(5);
                }
                recentConnections.Add(cd);

                Preferences.LocalSettings["RecentConnections"] = recentConnections;

                currentConnectionDropDownComboChoice = cd.ToString();
                string key = "ActiveChangelist_" + SccService.ScmProvider.Connection.Repository.Connection.Server.Address.Uri.Replace(':', '_') +
                    "_" + SccService.ScmProvider.Connection.User + "_" + SccService.ScmProvider.Connection.Workspace;

                string newActiveChangeListComboChoice = Preferences.LocalSettings.GetString(key, Resources.Changelist_Default);
                ChangeLists.SetActiveChangeList(newActiveChangeListComboChoice);
            }

            if ((SccService.ScmProvider != null) && (SccService.ScmProvider.Connected))
            {
                SccService.ScmProvider.SolutionFile = GetSolutionFileName();
            }
        }




        internal void CheckLazyLoadStatus()
        {
            if (Preferences.LocalSettings.GetBool("LazyLoadStatus", false))
            {
                // if lazy laoding, update status of the selected files that haven't had 
                // their status loaded
                IList<string> files = SccService.SelectedFiles;
                IList<string> llfiles = new List<string>(files.Count);
                foreach (string file in files)
                {
                    SourceControlStatus status = SccService.GetFileStatus(file);
                    if (status.Test(SourceControlStatus.scsUnknown))
                    {
                        llfiles.Add(file);
                    }
                }
                CurrentScm.UpdateFiles(llfiles, true);
                Glyphs.RefreshFilesAndGlyphs(llfiles);
            }
        }


        /// <summary>
        /// Checks whether the provider is invoked in command line mode
        /// </summary>
        public bool InCommandLineMode()
        {
            IVsShell shell = (IVsShell)GetService(typeof(SVsShell));
            object pvar;
            if (shell.GetProperty((int)__VSSPROPID.VSSPROPID_IsInCommandLineMode, out pvar) == VSConstants.S_OK &&
                (bool)pvar)
            {
                return true;
            }

            return false;
        }

    }
}
