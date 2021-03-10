using Perforce.P4Scm;
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
	public partial class UsersBrowserDlg : AutoSizeForm
	{
		private System.Windows.Forms.ImageList imageList1;

		public UsersBrowserDlg(P4ScmProvider scm, string sender)
		{
			PreferenceKey = "UsersBrowserDlg";
			
			Scm = scm;
			InitializeComponent();

            if (components == null)
            {
                components = new Container();
            }
            this.Icon = Images.icon_p4vs_16px;

			imageList1 = new System.Windows.Forms.ImageList(components);

			// 
			// imageList1
			// 
			imageList1.TransparentColor = System.Drawing.Color.Transparent;
			imageList1.Images.Add("users_icon.png", Images.users_icon);

			this.listView1.LargeImageList = this.imageList1;
			this.listView1.SmallImageList = this.imageList1;

			if (Scm != null)
			{
				IList<P4.User> users = Scm.GetUsers(null, null);
				foreach (P4.User user in users)
				{
					string id = user.Id;

                    DateTime localAccess = user.Accessed;

                    // we need a pref for local time, until then, don't do this:
                    //DateTime localAccess = TimeZone.CurrentTimeZone.ToLocalTime(user.Accessed);
                    string access = "";
                    if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                    {
                        access = localAccess.ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    else
                    {
                        access = string.Format("{0} {1}", localAccess.ToShortDateString(),
                                                            localAccess.ToShortTimeString());
                    }

				    string lastAccessed = access;
					string email = user.EmailAddress;
					string name = user.FullName;
					string[] theUser = new string[] { id, email, lastAccessed, name };
					ListViewItem lvi = new ListViewItem(theUser);
					lvi.Tag = user;
					lvi.ImageIndex = 0;
					listView1.Items.Add(lvi);
				}

			}
			ClosedByDoubleClick = false;
		}

		private UsersBrowserDlg()
		{
			PreferenceKey = "UsersBrowserDlg";

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
			imageList1 = new System.Windows.Forms.ImageList(components);

			// 
			// imageList1
			// 
			imageList1.TransparentColor = System.Drawing.Color.Transparent;
			imageList1.Images.Add("users_icon.png", Images.users_icon);

			this.listView1.LargeImageList = this.imageList1;
			this.listView1.SmallImageList = this.imageList1;
		}

		public P4.User SelectedUser
		{
			get
			{
				if (this.listView1.SelectedItems.Count > 0)
		{
		 return (P4.User)this.listView1.SelectedItems[0].Tag;
		}
				return null;
			}
			//return this.listView1.SelectedItems[0]; }
			//private set; 
		}

		public P4ScmProvider Scm { get; private set; }

		public P4.User Show(P4ScmProvider scm)
		{
			if (this.DialogResult == DialogResult.OK)
			{
				return SelectedUser;
			}
			return null;
		}

		private void OKBtn_Click(object sender, EventArgs e)
		{
			if (SelectedUser == null)
			{
				this.DialogResult=DialogResult.Cancel;
			}
		}

		public bool ClosedByDoubleClick { get; private set; }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SelectedUser != null)
            {
				ClosedByDoubleClick = true;
                this.DialogResult = DialogResult.OK;
            }
        }
	}
}
