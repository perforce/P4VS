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
    public partial class NoDiffsDlg : AutoSizeForm
    {
        public NoDiffsDlg(string message, List<ListViewItem> identicals)
        {
            PreferenceKey = "NoDiffsDlg";

            InitializeComponent();

            this.Icon = Images.icon_p4vs_16px;
            this.msgLbl.Text = message;
            foreach (ListViewItem item in identicals)
            {
                this.identicalFilesListView.Items.Add(item);
            }
            this.identicalFilesListView.Invalidate();
            this.Invalidate();

        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
