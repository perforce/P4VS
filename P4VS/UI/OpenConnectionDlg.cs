using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Perforce.P4Scm;
using System.Net;
using System.Reflection;

namespace Perforce.P4VS
{
	public partial class OpenConnectionDlg : AutoSizeForm
    {
		MRUList _recentConnections = null;

		public string ServerPort
		{
			get { return ServerTB.Text.Trim(); }
			set { ServerTB.Text = value; }
		}
		public string UserName 
		{ 
			get { return UserTB.Text.Trim(); ; }
			set { UserTB.Text = value; }
		}
		public string Workspace 
		{ 
			get { return WorkspaceTB.Text.Trim(); ; }
			set { WorkspaceTB.Text = value; }
		}
		public string Password { get; private set; }

//        ThemeManager ThemeMgr = null;

        public OpenConnectionDlg()
        {
            PreferenceKey = "OpenConnectionDlg";

            InitializeComponent();
            //if (!DesignMode)
            //{
            //    ThemeMgr = new ThemeManager(Controls);
            //}
            //else
            //{
            //    ThemeMgr = null;
            //}

            this.Icon = Images.icon_p4vs_16px;
            // Display the file version number.
            VersionLbl.Text = P4ScmProvider.ProductVersion;

            //VersionLbl.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            ConfigureSandboxBtn.Enabled = SandboxConfigPath != null;
            ConfigureSandboxBtn.Visible = ConfigureSandboxBtn.Enabled;

            RecentConnectionsCB.Items.Clear();
            //	if (Preferences.LocalSettings != null)
            _recentConnections = (MRUList)Preferences.LocalSettings["RecentConnections"];
            if (_recentConnections != null)
            {
                foreach (ConnectionData con in _recentConnections)
                {
                    if (con != null)
                    {
                        RecentConnectionsCB.Items.Add(con.ToString());
                    }
                }
                if (RecentConnectionsCB.Items.Count > 0)
                {
                    RecentConnectionsCB.SelectedIndex = 0;
                }
                else
                {
                    RecentConnectionsCB.SelectedIndex = -1;
                }
                if (_recentConnections[0] != null)
                {
                    ConnectionData cd = _recentConnections[0] as ConnectionData;
                    if (cd != null)
                    {
                        ServerTB.Text = cd.ServerPort;
                        UserTB.Text = cd.UserName;
                        WorkspaceTB.Text = cd.Workspace;
                    }
                }
            }
            BrowseWorkspaceBtn.Enabled = ServerTB.Text.Length > 0;
            NewWorkspaceBtn.Enabled = ServerTB.Text.Length > 0;
            BrowseUserBtn.Enabled = ServerTB.Text.Length > 0;
            NewUserBtn.Enabled = ServerTB.Text.Length > 0;
            OkBtn.Enabled = (ServerTB.Text.Length > 0) && (UserTB.Text.Length > 0) && (WorkspaceTB.Text.Length > 0);
        }

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    ThemeMgr.Dispose();
        //    base.OnClosing(e);
        //}

        private void RecentConnectionsCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (RecentConnectionsCB.SelectedIndex >= 0)
			{
				ConnectionData cd = _recentConnections[RecentConnectionsCB.SelectedIndex] as ConnectionData;
				if (cd != null)
				{
					ServerTB.Text = cd.ServerPort;
					UserTB.Text = cd.UserName;
					WorkspaceTB.Text = cd.Workspace;
				}
			}
			else
			{
				ConnectionData cd = _recentConnections[RecentConnectionsCB.SelectedIndex] as ConnectionData;
				ServerTB.Text = string.Empty;
				UserTB.Text = string.Empty;
				WorkspaceTB.Text = string.Empty;
			}
		}

		private string _sandboxConfigPath = null;
		private string SandboxConfigPath
		{
			get
			{
				if ((_sandboxConfigPath != null) && (File.Exists(_sandboxConfigPath)))
				{
					return _sandboxConfigPath;
				}
				if (_sandboxConfigPath == null)
				{
					if (Preferences.LocalSettings.ContainsKey("SandboxConfig_path"))
					{
						if (File.Exists(Preferences.LocalSettings["SandboxConfig_path"] as string))
						{
							_sandboxConfigPath = Preferences.LocalSettings["SandboxConfig_path"] as string;
						}
						else
						{
							Preferences.LocalSettings.Remove("SandboxConfig_path");
						}
					}
				}
				if (_sandboxConfigPath == null)
				{
					if (File.Exists("c:\\program files\\perforce\\p4sandbox-config.exe"))
					{
						_sandboxConfigPath = "c:\\program files\\perforce\\p4sandbox-config.exe";
					}

					else if (File.Exists("c:\\program files (x86)\\perforce\\p4sandbox-config.exe"))
					{
						_sandboxConfigPath = "c:\\program files (x86)\\perforce\\p4sandbox-config.exe";
					}
					if (_sandboxConfigPath == null)
					{
						Preferences.LocalSettings["SandboxConfig_path"] = _sandboxConfigPath;
					}
				}
				return _sandboxConfigPath;
			}
			set { _sandboxConfigPath = value; }
		}

		//string SandboxResults = null;

		//public void SandboxDataReceived(Object sender, DataReceivedEventArgs e)
		//{
		//    SandboxResults += e.Data;
		//}

		private void ConfigureSandboxBtn_Click(object sender, EventArgs e)
		{
			if (SandboxConfigPath == null)
			{
				return;
			}

			P4ScmProvider _scm = new P4ScmProvider(null);

            _scm.Connection.Port = ServerPort;
            _scm.Connection.User = UserName;
            _scm.Connection.Workspace = Workspace;

			string charset = null;

            _scm.Connection.Connect(true, null);
			if (_scm.Connected)
			{
                charset = _scm.Connection.Repository.Connection.CharacterSetName;
			}
			_scm.Dispose();

			this.TopMost = false;
			Process ConfigSandboxProc = new Process();

			string args = string.Format("-p {0} -u {1} -c {2}", ServerTB.Text, UserTB.Text, WorkspaceTB.Text);

			if (charset != null)
			{
				args += string.Format(" -C {0}", charset);
			}
			ConfigSandboxProc.StartInfo = new ProcessStartInfo(SandboxConfigPath);
			ConfigSandboxProc.StartInfo.Arguments = args;
			ConfigSandboxProc.StartInfo.RedirectStandardOutput = true;
			ConfigSandboxProc.StartInfo.UseShellExecute = false;
			ConfigSandboxProc.StartInfo.CreateNoWindow = false;

			ParameterizedThreadStart threadProc = new ParameterizedThreadStart(ConfigureSandboxRunThreadProc);
			Thread t = new Thread(threadProc);
			t.Start(ConfigSandboxProc);

			this.Enabled = false;

			//ConfigureSandboxRunThreadProc((object)ConfigSandboxProc);
		}

		private void ConfigureSandboxRunThreadProc(object param)
		{
			if (param == null)
			{
				this.TopMost = true;
				this.Enabled = true;
				return;
			}
			Process ConfigSandboxProc = param as Process;

			if (ConfigSandboxProc == null)
			{
				this.TopMost = true;
				this.Enabled = true;
				return;
			}

			string SandboxResults = string.Empty;

			ConfigSandboxProc.Start();
			SandboxResults = ConfigSandboxProc.StandardOutput.ReadToEnd();
			ConfigSandboxProc.WaitForExit();

			if (InvokeRequired)
			{
				Invoke(new ConfigureSandboxCompleteDelegate(ConfigureSandboxComplete), SandboxResults);
			}
			else
			{
				ConfigureSandboxComplete(SandboxResults);
			}
		}

		delegate void ConfigureSandboxCompleteDelegate(string SandboxResults);

		private void ConfigureSandboxComplete(string SandboxResults)
		{
			if ((SandboxResults != null) && (SandboxResults.Length > 0))
			{
				string[] parts = SandboxResults.Split(' ');

				for (int idx = 0; idx < parts.Length; idx++)
				{
					if ((parts[idx] == "-p") && (idx + 1 < parts.Length))
					{
						ServerTB.Text = parts[idx + 1];
						idx++;
					}
					if ((parts[idx] == "-u") && (idx + 1 < parts.Length))
					{
						UserTB.Text = parts[idx + 1];
						idx++;
					}
					if ((parts[idx] == "-c") && (idx + 1 < parts.Length))
					{
						WorkspaceTB.Text = parts[idx + 1];
						idx++;
					}
				}
			}
			this.TopMost = true;
			this.Enabled = true;
		}

		private void BrowseUserBtn_Click(object sender, EventArgs e)
		{
			this.TopMost = false;

			P4ScmProvider _scm = new P4ScmProvider(null);

            _scm.Connection.Port = ServerPort;

            _scm.Connection.Connect(true, null, true);
            if (_scm.Connected && _scm.Connection.ServerConnected())
			{
				UsersBrowserDlg dlg = new UsersBrowserDlg(_scm, null);

				dlg.TopMost = true; ;

				if ((DialogResult.Cancel != dlg.ShowDialog()) && (dlg.SelectedUser.Id != null))
				{
					UserTB.Text = dlg.SelectedUser.Id;
				}
			}
			_scm.Dispose();

			this.TopMost = true; ;
		}

		private void NewUserBtn_Click(object sender, EventArgs e)
		{
			this.TopMost = false;

			P4ScmProvider _scm = new P4ScmProvider(null);

			_scm.Connection.Port = ServerPort;

            _scm.Connection.Connect(true, null, true);
            if (_scm.Connected && _scm.Connection.ServerConnected())
			{
				NewUserDlg dlg = new NewUserDlg(_scm);
				P4.User newUser = dlg.Show(_scm);
				dlg.TopMost = true;
				if (null != newUser)
				{
					//_scm.NewUser(newUser);
					UserTB.Text = newUser.Id;
				}
				_scm.Dispose();
			}
			this.TopMost = true;
		}

		private void BrowseWorkspaceBtn_Click(object sender, EventArgs e)
		{
			this.TopMost = false;

			P4ScmProvider _scm = new P4ScmProvider(null);

			_scm.Connection.Port = ServerPort;
			_scm.Connection.User = UserName;

            _scm.Connection.Connect(true, null, true);
			if (_scm.Connected && _scm.Connection.isLoggedIn())
			{
				WorkspacesBrowserDlg dlg = new WorkspacesBrowserDlg(_scm, "connection",null,null);
				dlg.TopMost = true;

				if (DialogResult.Cancel != dlg.ShowDialog())
				{
					if ((dlg.SelectedWorkspace != null) && (dlg.SelectedWorkspace.Name != null))
						WorkspaceTB.Text = dlg.SelectedWorkspace.Name.ToString();
				}
				_scm.Dispose();
			}
			this.TopMost = true;
		}

		private void NewWorkspaceBtn_Click(object sender, EventArgs e)
		{
			this.TopMost = false;

			P4ScmProvider _scm = new P4ScmProvider(null);

			_scm.Connection.Port = ServerPort;
			_scm.Connection.User = UserName;

            _scm.Connection.Connect(true, null, true);
			if (_scm.Connected && _scm.Connection.isLoggedIn())
			{
				P4.Client workspace = null;
				P4.Client clientInfo = new P4.Client();
				string newName = GetStringDlg.Show(Resources.OpenConnectionDlg_NewWorkspaceDlgTitle,
					Resources.OpenConnectionDlg_NewWorkspaceDlgPrompt, null);
				if ((newName != null) && (newName != string.Empty))
				{
					if (newName.Contains(" "))
					{
						MessageBox.Show(Resources.OpenConnectionDlg_NameContainsSpacesWarning, Resources.P4VS,
							MessageBoxButtons.OK, MessageBoxIcon.Information); 
					}

					IList<P4.Client> checkExisting = _scm.getClients(P4.ClientsCmdFlags.None, null, newName, 1, null);
					if (checkExisting == null)
					{
						clientInfo = _scm.getClient(newName, null);
						if (clientInfo != null)
						{
							// adjust root here based on users dir
							string root = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
							int idx = root.LastIndexOf(@"\");
							root = root.Remove(idx + 1);
							root += newName;
							clientInfo.Root = root;
							workspace = DlgEditWorkspace.EditWorkspace(_scm,clientInfo);
						}
					}
					else
					{
						MessageBox.Show(string.Format(Resources.OpenConnectionDlg_WorkspaceExistsWarning, newName), 
							Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Information);
						NewWorkspaceBtn_Click(null, null);
					}

				}
				else
				{
					if (newName == string.Empty)
					{
						MessageBox.Show(Resources.OpenConnectionDlg_EmptyWorkspaceNameWarning,
							Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Information);
						NewWorkspaceBtn_Click(null, null);
					}
				}
				if (workspace != null)
				{
					WorkspaceTB.Text = workspace.Name;
				}
			}
			this.TopMost = true;
		}

		public ConnectionData ConnectionInfo { get; private set; }

		private void OkBtn_Click(object sender, EventArgs e)
		{
			ConnectionData cd = new ConnectionData();
			cd.ServerPort = ServerTB.Text;
			cd.UserName = UserTB.Text;
			cd.Workspace = WorkspaceTB.Text;

			ConnectionInfo = cd;

            // moved this to P4ScmProvider so we can decide whether to save the most
            // recent connection after we know it was successful.

            //if (_recentConnections == null)
            //{
            //    _recentConnections = new MRUList(5);
            //}
            //_recentConnections.Add(cd);

            //Preferences.LocalSettings["RecentConnections"] = _recentConnections;
		}

		private void HelpBtn_Click(object sender, EventArgs e)
		{
			this.TopMost = false;
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion; // 2011.1.0.0 is the format
            version = version.Remove(0, 2);
            string[] versionSplit = version.Split('.');
            string relDir = "r" + versionSplit[0] + "." + versionSplit[1];
            try
            {
                bool pageExists = false;
                string helpPath = @"https://www.perforce.com/perforce/doc.current/manuals/p4vs/intro.connecting.html";
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
            this.TopMost = true;
        }

        private void ServerTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				OkBtn.PerformClick();
		}

		private void UserTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				OkBtn.PerformClick();
		}

		private void WorkspaceTB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				OkBtn.PerformClick();
		}

		private void EnableButtons()
		{
			BrowseWorkspaceBtn.Enabled = ServerTB.Text.Length > 0;
			NewWorkspaceBtn.Enabled = ServerTB.Text.Length > 0;
			BrowseUserBtn.Enabled = ServerTB.Text.Length > 0;
			NewUserBtn.Enabled = ServerTB.Text.Length > 0;

			OkBtn.Enabled = (ServerTB.Text.Length > 0) && (UserTB.Text.Length > 0) && (WorkspaceTB.Text.Length > 0);
		}
		private void ServerTB_TextChanged(object sender, EventArgs e)
		{
			EnableButtons();
		}

		private void UserTB_TextChanged(object sender, EventArgs e)
		{
			EnableButtons();
		}

		private void WorkspaceTB_TextChanged(object sender, EventArgs e)
		{
			EnableButtons();
		}

        private void RecentConnectionsCB_DropDown(object sender, EventArgs e)
        {
            int widest = RecentConnectionsCB.DropDownWidth;
            if (RecentConnectionsCB.Items.Count>0)
            {
                foreach(object item in RecentConnectionsCB.Items)
                {
                    Image fakeImage = new Bitmap(1, 1);
                    Graphics graphics = Graphics.FromImage(fakeImage);
                    SizeF measure = graphics.MeasureString(item.ToString(), Font);
                    if (measure.Width > widest)
                    {
                        widest = Convert.ToInt32(measure.Width);
                    }
                }
                RecentConnectionsCB.DropDownWidth = widest;
            }
        }

        private void OpenConnectionDlg_Load(object sender, EventArgs e)
        {
            if (StartPosition == FormStartPosition.CenterParent)
            {
                // if opening in the default location, move it uup the screen so it can't
                // end up behind the VS initializing progress box, which is also always on top.
                StartPosition = FormStartPosition.Manual;
                Top -= Top / 2;
            }
        }
    }
}
