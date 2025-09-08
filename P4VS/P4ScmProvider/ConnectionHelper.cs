using Microsoft.Build.Evaluation;
using NLog;
using Perforce.P4;
using Perforce.P4VS;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Perforce.P4VS.FileLogger;

namespace Perforce.P4Scm
{
    public class ConnectionHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Swarm Swarm { get; set; }

        // connection info
        public string Port { get; set; }
        public string User { get; set; }
        public string Workspace { get; set; }
        public string WorkspaceRoot { get; private set; }
        public string SolutionPath { get; set; }

        public P4.Repository Repository;

        internal string Ticket { get; private set; }
        internal string ServerDNS { get; private set; }

        private static int _id = 0;
        public int ID { get; private set; }

        public void Connect()
        {
            initConnection(false, null, false);
        }

        public void Connect(ConnectionData connection)
        {
            Port = connection.ServerPort;
            User = connection.UserName;
            Workspace = connection.Workspace;

            initConnection(true, null, false);
        }

        public void Connect(bool noUI, string path)
        {
            initConnection(noUI, path, false);
        }

        public void Connect(bool noUI, string path, bool clientNotRequired)
        {
            initConnection(noUI, path, clientNotRequired);
        }

        public void Connect(string port, string user, string workspace, bool noUi, string path)
        {
            Port = port;
            User = user;
            Workspace = workspace;

            initConnection(noUi, path, false);
        }

        private void initConnection(bool noUI, string path, bool clientNotRequired)
        {
            ID = _id++;

            if (P4.LogFile.ExternalLogFn == null)
            {
                P4.LogFile.LogMessageDelgate logfn = new LogFile.LogMessageDelgate(FileLogger.LogMessage);
                P4.LogFile.SetLoggingFunction(logfn);
            }

            // reset bool on init
            ssoSuccess = true;

			_disconnected = true;

			if (Repository != null)
            {
                Repository.Dispose();
                Repository = null;
            }

            while (Repository == null)
            {
                // Get connection settings from dialog, if UI required
                if (!noUI) {
                    // reset the SSO bool, so login will be attempted again
                    ssoSuccess = true;
                    // Open Connection dialog
                    DialogResult result = connectionDialog();
                    if (result == DialogResult.Cancel)
                    { 
                        return;
                    }
                }

                // Get the credentials for the Perforce Server
                try
                {
                    // Get configuration
                    getConfiguration(path);

                    // Create repository from server connection
                    Repository = RepositoryFactory.get(Port, User, Workspace);

                    // set Swarm here since Login will check Swarm availability
                    Swarm = new Swarm(Repository, User);
                    if (string.IsNullOrEmpty(Workspace)&& !clientNotRequired)
                    {
                        DialogResult res = P4ErrorDlg.Show(Resources.P4ScmProvider_WorkspaceEnvUnset,
                                           false, false);
                        P4VsProvider.BroadcastNewConnection(null);
                        return;
                    }

                    if (Repository==null||
                        Repository.Connection.Status == P4.ConnectionStatus.Disconnected)
                    {
                        Repository = null;
                        continue;
                    }

                    // Set configuration
                    setConfiguration();

                    // Check API level of server is greater than 28 (2009.2)
                    checkApiLevel(28);

                    // set Swarm here since Login will check Swarm availability
                    Swarm = new Swarm(Repository, User);

                    // Login if required
                    if (!string.IsNullOrEmpty(User) && !isLoggedIn())
                    {
                        if (!ssoSuccess)
                        { break; }
                        LoginResult loginResult = Login();
                        if (loginResult == LoginResult.HASTimeout)
                        {
                            P4VS.UI.P4VSMessage p4VSMessage = new P4VS.UI.P4VSMessage(Resources.P4VS,
                            string.Format(Resources.HAS_Auth_Fail,
                Repository.Connection.UserName, Repository.Connection.Server.Address.Uri));
                            p4VSMessage.ShowDialog();
                            return;
                        }
                        if (loginResult==LoginResult.Fail)
                        {
                            Repository = null;
                            continue;
                        }
                    }

                    // Exit if no client defined
                    if (!string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Workspace))
                    {
                        setWorkspace();

                        // Save Connection as User and Workspace are good
                        saveRecentConnection();
                    }
                    else
                    {
                        logger.Warn("Workspace not initialised.");
                    }

                    if (Repository.Connection.Server.Metadata.UnicodeEnabled)
                    {
                        string cs = Repository.Connection.CharacterSetName;
                        string m = string.Format(Resources.P4ScmProvider_ConnectingToUnicodeServer, cs);
                        P4VsOutputWindow.AppendMessage(m);
                        FileLogger.LogMessage(3, "P4ScmProvider", m);
                    }
                    
                    // Subscribe to the output events to display results in the command window
                    Repository.Connection.InfoResultsReceived += CommandLine.InfoResultsCallbackFn;
                    Repository.Connection.ErrorReceived += CommandLine.ErrorCallbackFn;
                    Repository.Connection.TextResultsReceived += CommandLine.TextResultsCallbackFn;
                    Repository.Connection.TaggedOutputReceived += CommandLine.TaggedOutputCallbackFn;
                    Repository.Connection.CommandEcho += CommandLine.CommandEchoCallbackFn;
					Repository.Connection.ResponseTimeEcho += CommandLine.CommandEchoCallbackFn;

					_disconnected = false;

                    // Connection OK, break out of loop
                    break;
                }
                catch (P4Exception ex)
                {
                    if (ex.ErrorCode == P4.P4ClientError.MsgServer_Login2Required)
                    {
                        P4VsProvider.CurrentScm = new P4ScmProvider(null);
                        if(P4VsProvider.CurrentScm.LaunchHelixMFA(User, Port) == 0)
                        {
                            noUI = true;
                            initConnection(noUI, path, clientNotRequired);
                        }
                    }
                    else
                    {
                        logger.Trace("caught an Exception: {0}\r\n{1}", ex.Message, ex.StackTrace);
                        if (Repository != null)
                        {
                            Repository.Dispose();
                            Repository = null;
                        }
                        MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        P4VsOutputWindow.AppendMessage(ex.Message);
                    }
                    // If we're not showing the connection dialog return a null
                    if (noUI)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    logger.Trace("caught an Exception: {0}\r\n{1}", ex.Message, ex.StackTrace);
                    if (Repository != null)
                    {
                        Repository.Dispose();
                        Repository = null;
                    }
                    MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    P4VsOutputWindow.AppendMessage(ex.Message);

                    // If we're not showing the login dialog return a null as the credentials are bad
                    if (noUI)
                    {
                        return;
                    }
                }
            }

			Repository.Connection.KeepAlive = new KeepAliveMonitor();
            JobsToolWindowControl.GotJobFields = false;
        }

        private bool _disconnected = true;
        public bool Disconnected
        {
            get
            {
                if(_disconnected || (Repository == null) || (Repository.Connection == null)) {
                    return true;
                }

                if (Repository.Connection.Status == P4.ConnectionStatus.Disconnected)
                {
                    Disconnect();
                    return true;
                }

                return false;
            }
        }
        private void Disconnect()
        {
            _disconnected = true;

            if (Repository != null && Repository.Connection != null)
            {
                Repository.Connection.Disconnect();
            }
            P4VsProvider.BroadcastNewConnection(null);
        }

        public bool ServerConnected()
        {
            if(Repository == null || Repository.Connection == null) {
                return false;
            }

            if (Repository.Connection.Status == P4.ConnectionStatus.Disconnected)
            {
                return false;
            }

            return true;
        }

        public enum LoginResult { Success, Fail, HASTimeout}
        public LoginResult Login()
        {
            string ssoVar = Repository.Connection.GetP4EnvironmentVar("P4LOGINSSO");
            if (ssoVar != null)
            {
                string password = "UsingSSO"; // dummy value server will not prompt for a password when using SSO
                logger.Debug("Using SSO: {0}", ssoVar);

                // The client side environment variable may be set, but if
                // the server side trigger is not enabled do not return false
                // or the silent login fail will repeat without a prompt of
                // any kind for the user.
                if (Login(password))
                {
                    if(!ssoSuccess)
                    {
                        Disconnect();
                    }
                    return LoginResult.Success;
                }
            }

            P4VS.UI.HASCheckDlg hASCheckDlg = new P4VS.UI.HASCheckDlg(Repository);

            if (hASCheckDlg.ShowDialog() == DialogResult.OK)
            {
                // OK is returned if the dialog has been launched,
                // which also means HandleUrl was called.
                if (hASCheckDlg.Credential != null ||
                    hASCheckDlg.ExternalAuth)
                {
                    return LoginResult.Success;
                }
                else
                {
                    return LoginResult.HASTimeout;
                }
            }

            string caption = Resources.P4ScmProvider_LoginCaption;
            string msg = string.Format(Resources.P4ScmProvider_LoginPrompt, User, Port);

            while (true)
            {
                string password = GetStringDlg.Show(caption, msg, string.Empty, true);
                if (password == null)
                {
                    // user canceled login, so ask for different credentials
                    User = null;
                    Workspace = null;

                    Disconnect();

                    return LoginResult.Fail;
                }
                bool passwordLogin =  Login(password);
                if (passwordLogin)
                {
                    return LoginResult.Success;
                }
                else
                {
                    return LoginResult.Fail;
                }
            }
        }

        private bool ssoSuccess = true;
        private bool Login(string password)
        {
            // if Swarm is null, don't attempt to set this
            if (Swarm!=null)
            {
                Swarm.SwarmCredential = null;
            }

            // unsure if this currently works with legacy SSO script support, but 
            // leaving it in as it is harmless if ssoScript is null and if not,
            // it should be the same as the ssoVar found in Login()
            string ssoScript = Environment.GetEnvironmentVariable("P4LOGINSSO");
            if (ssoScript != null)
            {
                password = "UsingSSO"; // dummy value server will not prompt for a password when using SSO
                logger.Debug("Using SSO: {0}", ssoScript);
            }

            try
            {
                bool UseAllHostTicket = !Preferences.LocalSettings.GetBool("Use_IP", false);
                if (string.IsNullOrEmpty(password) != true)
                {
                    // always first login on the local machine for an 'all machines token, as we can't read the property 
                    // to see if swarm is enabled till we log in
                    logger.Debug("Logging in to local machine");
                    LoginCmdOptions opts = new LoginCmdOptions(LoginCmdFlags.AllHosts, null);
                    P4.Credential Credential = Repository.Connection.Login(password, opts);
                    if (Credential != null)
                    {
                        // Now we can see if Swarm is enabled as since we logged in
                        // the property to see if swarm is enabled
                        Swarm.CheckForSwarm();
                        if (Swarm.SwarmEnabled)
                        {
                            Swarm.SwarmCredential = Credential;
                        }
                        if (Credential.Expires != DateTime.MaxValue)
                        {
                            string msg = string.Format(Resources.P4ScmProvider_LoginLoginExpireInfo, Credential.Expires);
                            P4VsOutputWindow.AppendMessage(msg);
                        }
                        return true;
                    }
                    else if (Repository.Connection.LastResults.ErrorList!=null)
                    {
                        // the error code for an SSO login fail will be sent by the trigger
                        // so it will be a trigger failed message.
                        if (Repository.Connection.LastResults.ErrorList[0].ErrorCode== P4.P4ClientError.MsgServer_TriggerFailed)
                        {
                            P4VsOutputWindow.AppendMessage(Repository.Connection.LastResults.ErrorList[0].ErrorMessage);
                            MessageBox.Show(Repository.Connection.LastResults.ErrorList[0].ErrorMessage,
                                Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ssoSuccess = false;
                            return true;
                        }
                        // Or if it is a fatal error, display that as well. This can
                        // happen if the P4LOGINSSO is set but the agent does not
                        // exist. Likely an edge case.
                        foreach(P4ClientError e in Repository.Connection.LastResults.ErrorList)
                        if (e.SeverityLevel == ErrorSeverity.E_FATAL)
                        {
                            P4VsOutputWindow.AppendMessage(Repository.Connection.LastResults.ErrorList[0].ErrorMessage);
                            MessageBox.Show(Repository.Connection.LastResults.ErrorList[0].ErrorMessage,
                                Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ssoSuccess = false;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                P4Exception p4ex = ex as P4Exception;
                if (p4ex.ErrorCode == P4.P4ClientError.MsgServer_Login2Required)
                {
                    throw p4ex;
                }
                P4VsOutputWindow.AppendMessage(ex.Message);
                MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }


        private class HandledException : Exception
        {
            public HandledException(string msg) : base(msg) { }
        }


        public bool Expired()
        {
            if (Disconnected)
            {
                return true;
            }

            // finally if we really think we are connected test with a 'login -s'
            if (!string.IsNullOrEmpty(User) && !isLoggedIn())
            {
                if (Login()==LoginResult.Fail)
                {
                    Disconnect();
                    return true;
                }
            }

            return false;
        }

        /// <summary>Simple login check using `p4 login -s`</summary>
        /// <para>Return 'true' if logged in (with or without a password).</para>
        public bool isLoggedIn()
        {
            try
            {
                string[] loginOptions = new string[] { "-s" };
                P4.P4Command LoginCmd = new P4Command(Repository, "login", true, loginOptions);
                P4CommandResult LoginResults = LoginCmd.Run();

                if ((LoginResults == null) || (LoginResults.TaggedOutput == null))
                {
                    return false;
                }

                string validated;
                LoginResults.TaggedOutput[0].TryGetValue("2FA", out validated);
                if (validated == "required")
                {
                    P4ClientError err = new P4ClientError(ErrorSeverity.E_FAILED,
                        "Second factor authentication required! Run 'p4 login2'.");
                    err.ErrorCode = 807673723;
                    P4Exception ex = new P4Exception(err);
                    throw ex;
                }

                // logged in. May not have a ticket if no password was required.
                return true;
            }
            catch (P4.P4Exception ex)
            {
                if (ex.ErrorCode == P4.P4ClientError.MsgServer_Login2Required)
                {
                    throw ex;
                }
                return false;
            }
        }

        /// <summary>Show Connection Dialog</summary>
        /// <returns>OK/CANCEL</returns>
        private DialogResult connectionDialog()
        {
            OpenConnectionDlg dlg = new OpenConnectionDlg();

            // Get the desired connection info
            if (Port != null)
            {
                dlg.ServerPort = Port;
            }
            if (User != null)
            {
                dlg.UserName = User;
            }
            if (Workspace != null)
            {
                dlg.Workspace = Workspace;
            }

            // Show dialog
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                Port = dlg.ServerPort;
                User = dlg.UserName;
                Workspace = dlg.Workspace;
            }

            return result;
        }

        /// <summary>Get Perforce Connection details from Config file or Environment</summary>
        /// <param name="path">Path to P4CONFIG file</param>
        private void getConfiguration(string path)
        {
            string msg;

            if ((Port == null) && (User == null) && (Workspace == null))
            {
                try
                {
                    P4.P4Server sslCheck = new P4.P4Server(path);
                }
                catch (P4Exception ex)
                {
                    if (ex.ErrorCode == P4.P4ClientError.MsgRpc_HostKeyUnknown)
                    {
                        string[] message = ex.Message.Split('\'');
                        string port = "ssl:"+message[1];
                        RepositoryFactory.get(port, null, null);
                   }
                }
                
                using (P4.P4Server ps = new P4.P4Server(path))
                {
                    Port = ps.Port;
                    User = ps.User;
                    Workspace = ps.Client;

                    string config = ps.Config;
                    if ((string.IsNullOrEmpty(config) == false) && (config != "noconfig"))
                    {
                        // api is using a config file
                        msg = String.Format(Resources.P4ScmProvider_UsingConfigSettingsToConnect, Port, User, Workspace, config);
                    }
                    else
                    {
                        msg = String.Format(Resources.P4ScmProvider_UsingEnvironmentSettingsToConnect, Port, User, Workspace);
                    }
                }
            }
            else
            {
                msg = String.Format(Resources.P4ScmProvider_ConnectingToPerforceServer, Port, User, Workspace);
            }

            // Log configuration settings
            P4VsOutputWindow.AppendMessage(msg);

			logger.Trace("scm ID:{0}", ID);
            FileLogger.LogMessage(3, "P4API.NET", msg);
			Preferences.LocalSettings.LogLocalSettings();
		}

        /// <summary>Set Perforce Connection for project</summary>
        private void setConfiguration()
        {
            // Set Environment properties, if required.
            if (Preferences.LocalSettings.GetBool("SetEnvironmentVars", true))
            {
                Environment.SetEnvironmentVariable("P4VS_PORT", Port);
                Environment.SetEnvironmentVariable("P4VS_USER", User);
                Environment.SetEnvironmentVariable("P4VS_CLIENT", Workspace);
            }

            // Set Project's properties
            setProjectProperties("P4PORT", Port);
            setProjectProperties("P4USER", User);
            setProjectProperties("P4CLIENT", Workspace);

            // Send the connection properties to a memory mapped file
            // for use by other projects/applications.
            string connectionInfo = Port + "," + User + "," + Workspace;
            sendConnectionToMMF(connectionInfo);
        }

        MemoryMappedFile memoryMappedFile;
        MemoryMappedViewAccessor viewAccessor;

        private void sendConnectionToMMF(string connectionInfo)
        {
            // Get the process ID of the currently running Visual Studio IDE
            var vsId = System.Diagnostics.Process.GetCurrentProcess().Id;

            try
            {
                if(memoryMappedFile != null)
                {
                    memoryMappedFile.Dispose();
                }
                memoryMappedFile = MemoryMappedFile.CreateOrOpen("connection-info-" + vsId.ToString(),
                    10000, MemoryMappedFileAccess.ReadWrite);

                viewAccessor = memoryMappedFile.CreateViewAccessor();

                byte[] fileContent = Encoding.UTF8.GetBytes(connectionInfo);
                viewAccessor.Write(0, fileContent.Length);
                viewAccessor.WriteArray<byte>(4, fileContent, 0, fileContent.Length);
                viewAccessor.Flush();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>Set project properties</summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void setProjectProperties(string key, string value)
        {
            ProjectCollection Projects = ProjectCollection.GlobalProjectCollection;
            if (Projects == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                logger.Debug("Removing property {0}", key);
                Projects.RemoveGlobalProperty(key);
            }
            else
            {
                logger.Debug("Setting property {0} to: {1}", key, value);
                Projects.SetGlobalProperty(key, value);
            }  
        }

        /// <summary>Save Connection to Recent Connections list</summary>
        private void saveRecentConnection()
        {
            ConnectionData cd = new ConnectionData();
            cd.ServerPort = Port;
            cd.UserName = User;
            cd.Workspace = Workspace;

            MRUList _recentConnections = (MRUList)Preferences.LocalSettings["RecentConnections"];
            if (_recentConnections == null)
            {
                _recentConnections = new MRUList(5);
            }
            _recentConnections.Add(cd);

            Preferences.LocalSettings["RecentConnections"] = _recentConnections;
        }

        /// <summary>Check Server API level</summary>
        /// <param name="min"></param>
        private void checkApiLevel(int min)
        {
            int level = Repository.Connection.ApiLevel;
            logger.Trace("Repository.Connection.ApiLevel: {0}", level);
            if (level < min)
            {
                Exception e = new Exception(Resources.P4ScmProvider_OldServerError);
                throw e;
            }
        }

        /// <summary>Set Client and VS Workspace</summary>
        private void setWorkspace()
        {
            // Set Client Workspace
            Repository.Connection.Client.Initialize(Repository.Connection);

            // Test Client
            if (!isClientCreated())
            {
                throw new Exception("Client '" + Workspace + "' does not exist.");
            }

            // Get Client Root
            WorkspaceRoot = getClientRoot();
            if (!WorkspaceRoot.Equals("null", StringComparison.CurrentCultureIgnoreCase) && !Directory.Exists(WorkspaceRoot))
            {
                createClientRoot(WorkspaceRoot);
            }

            // Set Current Working Directory
            if (string.IsNullOrEmpty(SolutionPath))
            {
                Repository.Connection.CurrentWorkingDirectory = WorkspaceRoot;
            }
            else
            {
                Repository.Connection.CurrentWorkingDirectory = SolutionPath;
            }   
        }

        private bool isClientCreated()
        {
            DateTime accessed = Repository.Connection.Client.Accessed;
            return accessed.Year > 1970;
        }

        private string getClientRoot()
        {
            // try Client 'Root'
            string root = Repository.Connection.Client.Root;
            if (rootPathValid(root)) { return root; }

            // try Client 'AltRoots'
            IList<string> roots = Repository.Connection.Client.AltRoots;
            if (roots != null)
            {
                foreach (string r in roots)
                {
                    if (rootPathValid(r)) { return r; }
                }
            }

            throw new Exception(string.Format(Resources.P4ScmProvider_WorkspaceRootInvalidError, root));
        }

        public bool rootPathValid(string root)
        {
            if (String.Equals(root, "null", StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            if ((root.IndexOfAny(Path.GetInvalidPathChars()) > 0) || !Path.IsPathRooted(root))
            {
                return false;
            }

            Regex drive = new Regex(@"^[a-zA-Z]:[\\/]");
            if (drive.IsMatch(root))
            {
                return true;
            }

            return false;
        }

        /// <summary>Create client root directory</summary>
        /// <param name="root">Client Root location</param>
        private void createClientRoot(String root)
        {
            String msg = string.Format(Resources.P4ScmProvider_CreatWorkspaceRootPrompt, root);
            DialogResult answer = MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (answer == DialogResult.Yes)
            {
                try
                {
                    Directory.CreateDirectory(root);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(Resources.P4ScmProvider_CannotCreateWorkspaceRootError, root, ex.Message), ex);
                }
            }
            else
            {
                throw new HandledException(string.Format(Resources.P4ScmProvider_NonexistentWorkspaceRootError, root));
            }
        }

        public bool isSwarmEnabled()
        {
            return (Swarm!=null ? Swarm.SwarmEnabled : false);
        }
    }
}
