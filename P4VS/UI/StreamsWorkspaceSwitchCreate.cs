using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.P4VS
{
    public partial class StreamsWorkspaceSwitchCreate : AutoSizeForm
    {
        public StreamsWorkspaceSwitchCreate()
        {
            InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
        }

        public StreamsWorkspaceSwitchCreate(string title, string workspace,
            string msg, string action)
        {
            InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
            this.Text = title;
            if (workspace == string.Empty)
            {
                this.msgLbl.Text = string.Format(Resources.StreamWorkspaceCreateText,
                    action);
            }
            else
            {
                this.msgLbl.Text = string.Format(Resources.StreamWorkspaceSwitchText,
                    workspace, action);
            }
        }

        private void yesBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            return;
        }

        private void noBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            return;
        }


      }
}
