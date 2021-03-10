using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows;

namespace Perforce.P4VS.UI
{

    public partial class LaunchingHMFA : Form
    {
        internal string user = "";
        internal string port = "";
        internal string path = "";
        internal bool launch = false;
        public LaunchingHMFA()
        {
            InitializeComponent();
        }

        public LaunchingHMFA(bool appInstalled, string u, string p, string appPath)
        {
            InitializeComponent();
            user = u;
            port = p;
            path = appPath;
            Text = string.Format(Resources.HMFA_Title,
                user, port);

            if (appInstalled)
            {
                pictureBox.Image = Resources.HMFA_spinner_78x78;
                launchingGridLbl.Text = Resources.HMFA_Launch;
                downloadBtn.Visible = false;
                downloadBtn.Enabled = false;
                launch = true;
            }
            else
            {
                pictureBox.Image = Resources.download_icon;
                launchingGridLbl.Text = Resources.HMFA_Download;
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { this.Close(); }));
            }
            else
            {
                this.Close();
            }
        }

        private void LaunchingHMFA_Shown(object sender, EventArgs e)
        {
            if (launch)
            { LaunchApp(); }
        }

        public int exitCode = 1;
        private void LaunchApp()
        {
            Process launchHelixMFA = new Process();
            launchHelixMFA.StartInfo.Arguments = port + " " + user;
            launchHelixMFA.StartInfo.FileName = path;
            string msg = string.Format("==>{0} {1}", path, launchHelixMFA.StartInfo.Arguments);
            P4VsOutputWindow.AppendMessage(msg);
            launchHelixMFA.StartInfo.CreateNoWindow = true;
            launchHelixMFA.EnableRaisingEvents = true;
            launchHelixMFA.Start();
            launchHelixMFA.Exited += (sender, e) => { exitCode = launchHelixMFA.ExitCode; closeBtn_Click(sender, e); };
        }

        private void downloadBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://www.perforce.com/downloads/helix-mfa-authenticator");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            closeBtn_Click(null, null);
        }
    }
    
}
