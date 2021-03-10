using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Perforce;
using Perforce.P4;
using NLog;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class DepotPathDlg : AutoSizeForm
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public DepotPathDlg(P4VsProviderService sccService, string title, bool showOpenConnectionBtn)
		{
			PreferenceKey = "DepotPathDlg";

			SccService = sccService;

            ShowOpenConnectionBtn = showOpenConnectionBtn;
            
            UseEnvConnect=Preferences.LocalSettings.GetBool("DepotPathDlg.EnvCB.Checked", false); 

            FilterByWorkspace = Preferences.LocalSettings.GetBool("DepotPathDlg.FilterCB.Checked", true);

            InitializeComponent();

            this.Icon = Images.icon_p4vs_16px;
            // 
			// DepotTreeViewImageList
			// 
			this.DepotTreeViewImageList = new System.Windows.Forms.ImageList(this.components);

			DepotTreeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            DepotTreeViewImageList.Images.Add("depot.png", Images.depot);
			DepotTreeViewImageList.Images.Add("folder.png", Images.folder);
			DepotTreeViewImageList.Images.Add("greyfile.png", Images.greyfile);
            DepotTreeViewImageList.Images.Add("stream_depot.png", Images.stream_depot);

			mDepotTreeView.ImageList = DepotTreeViewImageList;

            if (title != null)
            {
                this.Text = title;
            }

            Init();
		}

        public bool UseEnvConnect { get; set; }
        public bool FilterByWorkspace { get; set; }
        public bool ShowOpenConnectionBtn { get; set; }

        public String SelectedFile = "";

        P4VsProviderService SccService { get; set; }

		public void Init()
		
{
            envCB.Checked = UseEnvConnect;
            filterCB.Visible = true;
            filterCB.Enabled = true;
            if (ShowOpenConnectionBtn)
            {
                filterCB.Checked = Preferences.LocalSettings.GetBool("DepotPathDlg.FilterCB.Checked", true);
            }
            else
            {
                filterCB.Checked = true;
                filterCB.Enabled = false;
                OpenConnectionBtn.Visible = false;
                OpenConnectionBtn.Enabled = false;
            }

            if (SccService.ScmProvider==null||SccService.ScmProvider.Connected == false||
                string.IsNullOrEmpty(SccService.ScmProvider.Connection.Workspace))
            {
                connectionLbl.Text = string.Format(Resources.DepotPathDlg_NoConnection);
                mDepotTreeView.Nodes.Clear();
                mDepotTreeView.Enabled = false;
                return;
            }

            if ((SccService.ScmProvider.Connection.Disconnected != true)
                && (SccService.ScmProvider.ServerVersion != null)
                &&(SccService.ScmProvider.ServerVersion < Versions.V12_1))
            {
                filterCB.Visible = false;
                filterCB.Enabled = false;
            }

			mDepotTreeView.Nodes.Clear();

            connectionLbl.Text = string.Format(Resources.DepotPathDlg_Connection,
                SccService.ScmProvider.Connection.Port + ", " + SccService.ScmProvider.Connection.User +
                ", " + SccService.ScmProvider.Connection.Workspace); 

			IList<P4.Depot> depots = SccService.ScmProvider.GetDepots();
			if (depots != null)
			{
			    foreach (P4.Depot depot in depots)
			    {
                    if (!(depot.Type == DepotType.Local) && !(depot.Type == DepotType.Stream))
                    {
                        continue;
                    }

                    if (filterCB.Checked)
                    {
                            bool inView = false;
                            ViewMap vm = SccService.ScmProvider.Connection.Repository.Connection.Client.ViewMap;
                            foreach (MapEntry m in vm)
                            {
                                if (m.Left.Path.Contains(string.Format("//{0}/", depot.Id)))
                                {
                                    inView = true;
                                    break;
                                }
                            }
                            if (!inView)
                            {
                                continue;
                            }
                    }
                    
                    int badIdx = depot.Id.IndexOf('\\');

                    // Initialize the depot tree view
                    P4Directory root = new P4Directory(SccService.ScmProvider, null, depot.Id, string.Format("//{0}", depot.Id), null, null);
                    TreeNode rootNode = new TreeNode(depot.Id);
                    rootNode.Tag = root;
                    rootNode.ImageIndex = 0;
                    rootNode.SelectedImageIndex = 0;
                    if (depot.Type == DepotType.Stream)
                    {
                         rootNode.ImageIndex = 3;
                    rootNode.SelectedImageIndex = 3;
                    }
                    if (badIdx >= 0)
                    {
                        rootNode.Text += "(*)";
                        rootNode.ForeColor = Color.Red;
                    }
                    else
                    {
                        rootNode.Nodes.Add(new TreeNode("empty"));
                    }
                    mDepotTreeView.Nodes.Add(rootNode);
				}
				mDepotTreeView.Enabled = true;
			}
		}

		private void mDepotTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			// insanity check, should never be null
			if (e.Node == null)
			{
				e.Cancel = true;
				return;
			}
			try
			{
				TreeNode node = e.Node;
				// clear any old data
				node.Nodes.Clear();

				P4Directory p4Dir = (P4Directory)node.Tag;
			    
                char[] bad = Path.GetInvalidPathChars();
				int badIdx = p4Dir.DepotPath.IndexOfAny(bad);

                if (filterCB.Checked)
                {
                    if (String.IsNullOrEmpty(p4Dir.DepotPath) ||
                    (p4Dir.DepotPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0) ||
                    !p4Dir.Expand(true))
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (String.IsNullOrEmpty(p4Dir.DepotPath) ||
					(p4Dir.DepotPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)|| 
					!p4Dir.Expand(false))
				{
					e.Cancel = true;
					return;
				}

				if ((p4Dir.Subdirectories != null) && (p4Dir.Subdirectories.Count > 0))
				{
					foreach (P4Directory p4SubDir in p4Dir.Subdirectories)
					{
						if (!p4SubDir.InDepot)
							continue;

                        TreeNode child = new TreeNode(p4SubDir.Name);
						child.Tag = p4SubDir;
						child.ImageIndex = 1;
						child.SelectedImageIndex = 1;
						child.Nodes.Add(new TreeNode("<empty>"));
						e.Node.Nodes.Add(child);
					}
				}

                if ((p4Dir.Files != null) && (p4Dir.Files.Count > 0))
                {
                    foreach (P4.FileMetaData file in p4Dir.Files)
                    {
                        try
                        {
                            if (ShowOpenConnectionBtn)
                            {
                                string ext = Path.GetExtension(file.DepotPath.Path);
                                if (ext != null && ((ext == ".sln") || ext.Contains("proj") || ext.Contains("proj")))
                                {
                                    TreeNode child = new TreeNode(file.DepotPath.Path);
                                    child.Tag = file;
                                    child.ImageIndex = 2;
                                    child.SelectedImageIndex = 2;
                                    e.Node.Nodes.Add(child);
                                }
                            }
                            else
                            {
                                {
                                    TreeNode child = new TreeNode(file.DepotPath.Path);
                                    child.Tag = file;
                                    child.ImageIndex = 2;
                                    child.SelectedImageIndex = 2;
                                    e.Node.Nodes.Add(child);
                                }
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            logger.Trace("Error in file name, {0}: {1}\r\n\t{2}", file.DepotPath.Path, ex.Message, ex.StackTrace);
                        }
                    }
                }

			}
			catch { }
		}

		private void mDepotTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			Object obj  = mDepotTreeView.SelectedNode.Tag;
            if (ShowOpenConnectionBtn)
            {
                if (obj is P4.FileMetaData)
                {
                    SelectedFile = mDepotTreeView.SelectedNode.Text.ToString();
                    string file = SelectedFile.Substring(SelectedFile.LastIndexOf('/')+1);
                    mSolutionPathTxt.Text = file;
                }
                else
                {
                    mSolutionPathTxt.Text = string.Empty;
                }
            }
            else
            {
                if (obj is P4.FileMetaData)
                {
                    SelectedFile = mDepotTreeView.SelectedNode.Text.ToString();
                    mSolutionPathTxt.Text = SelectedFile;
                }
                else
                {
                    P4Directory p4Dir = (P4Directory)mDepotTreeView.SelectedNode.Tag;
                    SelectedFile = p4Dir.DepotPath + "/...";
                    mSolutionPathTxt.Text = SelectedFile;
                }
            }
		}

		private void mSolutionPathTxt_TextChanged(object sender, EventArgs e)
		{
			string path = mSolutionPathTxt.Text;
			
			mOkBtn.Enabled = !string.IsNullOrEmpty(path);
		}

		public ConnectionData ConnectionInfo { get; private set; }

		private void OpenConnectionBtn_Click(object sender, EventArgs e)
		{
            P4ScmProvider newConection = new P4ScmProvider(SccService);
            if (UseEnvConnect)
            {
                newConection.Connection.Connect(true,null);
            }
            else
            {
                // trigger the connection dialog
                newConection.Connection.Connect();
            }
            // only replace ScmProvider if a connection has been made
            // through env vars or connection dlg
            if (newConection.Connected)
            {
                SccService.ScmProvider = newConection;
            }
            if (SccService.ScmProvider.Connection.Repository != null&&
                !(string.IsNullOrEmpty(SccService.ScmProvider.Connection.Workspace)))
			{
				if (SccService.ScmProvider != null)
				{
					SccService.ScmProvider.LoadingSolution = !string.IsNullOrEmpty(SccService.LoadingControlledSolutionLocation);
				}
				Init();

				P4VsProvider.BroadcastNewConnection(SccService.ScmProvider);

				ConnectionInfo = new ConnectionData();
                ConnectionInfo.ServerPort = SccService.ScmProvider.Connection.Port;
                ConnectionInfo.UserName = SccService.ScmProvider.Connection.User;
                ConnectionInfo.Workspace = SccService.ScmProvider.Connection.Workspace;

				P4VsProvider.Instance.currentConnectionDropDownComboChoice = ConnectionInfo.ToString();

                string key = "ActiveChangelist_" + SccService.ScmProvider.Connection.Repository.Connection.Server.Address.Uri.Replace(':', '_') +
                    "_" + SccService.ScmProvider.Connection.User + "_" + SccService.ScmProvider.Connection.Workspace;

                P4VsProvider.Instance.ChangeLists = new ActiveChangeListCombo(SccService); 
				string newActiveChangeListComboChoice = Preferences.LocalSettings.GetString(key, Resources.Changelist_Default);
                P4VsProvider.Instance.ChangeLists.SetActiveChangeList(newActiveChangeListComboChoice);
			}
            else
            {
                P4VsProvider.BroadcastNewConnection(null);
            }
		}

		private void mDepotTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
		{
            FileMetaData fmd = mDepotTreeView.SelectedNode.Tag as FileMetaData;
            if(fmd==null)
            {
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void filterCB_CheckedChanged(object sender, EventArgs e)
        {
            Preferences.LocalSettings["DepotPathDlg.FilterCB.Checked"] = filterCB.Checked;
            FilterByWorkspace = filterCB.Checked;
            Init();
        }

        private void mOkBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void envCB_CheckedChanged(object sender, EventArgs e)
        {
            OpenConnectionBtn.Enabled = !envCB.Checked;
            Preferences.LocalSettings["DepotPathDlg.EnvCB.Checked"] = envCB.Checked;
            if (envCB.Checked)
            {
                UseEnvConnect = true;
                if (SccService != null)
                {
                    OpenConnectionBtn_Click(null, null);
                    Init();
                }
            }
            else
            {
                UseEnvConnect = false;
            }
            Init();
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = !filterCB.Checked;
            if (mDepotTreeView.SelectedNode==null)
            {
                e.Cancel = true;
            }
        }

        private void getLatestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IList<P4.FileSpec> filesChanged = new List<FileSpec>();
            Object obj = mDepotTreeView.SelectedNode.Tag;
            Options opts = new P4.SyncFilesCmdOptions(
                    SyncFilesCmdFlags.None, 0);

            if (obj is P4.FileMetaData)
            {
                FileMetaData fmd = obj as FileMetaData;

                SccService.ScmProvider.SyncFiles(out filesChanged,
                    opts, fmd.DepotPath.Path);
            }
            else
            {
                P4Directory p4Dir = (P4Directory)mDepotTreeView.SelectedNode.Tag;
                SccService.ScmProvider.SyncFiles(out filesChanged,
                    opts, p4Dir.DepotPath + "/...");
            }
            
        }

        private void removeFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IList<P4.FileSpec> filesChanged = new List<FileSpec>();
            Object obj = mDepotTreeView.SelectedNode.Tag;
            Options opts = new P4.SyncFilesCmdOptions(
                    SyncFilesCmdFlags.None, 0);

            if (obj is P4.FileMetaData)
            {
                FileMetaData fmd = obj as FileMetaData;
                FileSpec fs = new FileSpec(fmd.DepotPath, null,null,
                    new NoneRevision());
                filesChanged.Add(fs);
                SccService.ScmProvider.SyncFiles(opts, filesChanged);
            }
            else
            {
                P4Directory p4Dir = (P4Directory)mDepotTreeView.SelectedNode.Tag;
                FileSpec fs = new FileSpec(new DepotPath(p4Dir.DepotPath+"/..."),
                    null, null, new NoneRevision());
                filesChanged.Add(fs);
                SccService.ScmProvider.SyncFiles(opts, filesChanged);
            }
        }

        private void mDepotTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            mDepotTreeView.SelectedNode = e.Node;
        }
    }
}
