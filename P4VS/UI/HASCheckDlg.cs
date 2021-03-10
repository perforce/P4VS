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
using Perforce.P4;
using System.Threading;

namespace Perforce.P4VS.UI
{

    public partial class HASCheckDlg : Form
    {
        internal string user = "";
        internal string port = "";
        internal string path = "";
        internal bool launch = false;

        public Repository Repository;

        public Credential Credential;

        public bool ExternalAuth = false;
        public HASCheckDlg()
        {
            InitializeComponent();
        }

        public HASCheckDlg(Repository repository)
        {
            Repository = repository;
            InitializeComponent();
            
            Text = string.Format(Resources.P4VS,
                user, port);

                pictureBox.Image = Resources.HMFA_spinner_78x78;
                authenticatingGridLbl.Text = Resources.HAS_Authenticating;
                timeoutGridLbl.Text = Resources.HAS_Timeout;
        }

         private void LaunchingHMFA_Shown(object sender, EventArgs e)
        {
            return;
        }

        //public void HASLogin(Repository repository)
        //{
        //    Repository = repository;
        //    bool test = false;

        //    P4Server p4Server = Repository.Connection.getP4Server();
        //    if (p4Server!=null)
        //    {
        //        test = p4Server.UrlHandled();

        //    }

        //    Task<Credential> task = Task.Run<Credential>(async () => await AsyncLogin());

        //    Thread.Sleep(3000);

        //    test = Repository.Connection.getP4Server().UrlHandled();
        //    if (test)
        //    {
        //        while(!task.IsCompleted)
        //        {
        //            ShowDialog();
        //        }
        //        Close();
        //    }
        //    return;
        //}

        public new DialogResult ShowDialog()
        {
            bool test = false;

            P4Server p4Server = Repository.Connection.getP4Server();
            if (p4Server != null)
            {
                test = p4Server.UrlHandled();

            }

            Task<Credential> task = Task.Run<Credential>(async () => await AsyncLogin());

            Thread.Sleep(3000);

            test = Repository.Connection.getP4Server().UrlHandled();
            if (test)
            {
                while (!task.IsCompleted)
                {
                    base.ShowDialog();
                }
                DialogResult = DialogResult.OK;
                base.Close();
                return DialogResult;
            }
            return DialogResult.Cancel;
        }
        public async Task<Credential> AsyncLogin()
        {
            Credential = null;
            try
            {
                Credential = await Task.Run(() => Repository.Connection.Login("", null));
                if (Credential == null)
                {
                    if (Repository.Connection.LastResults.TaggedOutput != null)
                    {
                        foreach (TaggedObject keyValuePairs in Repository.Connection.LastResults.TaggedOutput)
                        {
                            if (keyValuePairs.ContainsKey("TicketExpiration"))
                            {
                                ExternalAuth = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            DialogResult = DialogResult.OK;
            base.Close();
            return Credential;
        }

    }
    
}
