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
	public partial class GroupsBrowserDlg : AutoSizeForm
	{
		private System.Windows.Forms.ImageList imageList1;

		public GroupsBrowserDlg(P4ScmProvider scm, string sender)
		{
			PreferenceKey = "GroupsBrowserDlg";
			
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
			imageList1.Images.Add("groups_icon.png", Images.groups_icon);
            imageList1.Images.Add("users_icon.png", Images.users_icon);

            this.groupGridP4ObjectTreeListView.LargeImageList = this.imageList1;
			this.groupGridP4ObjectTreeListView.SmallImageList = this.imageList1;

			if (Scm != null)
			{
				IList<P4.Group> groups = Scm.GetGroups(null, null);
				foreach (P4.Group group in groups)
				{
					string id = group.Id;
                    TreeListViewItem tlvi = new TreeListViewItem();
                    tlvi.Name = id;
                    tlvi.Text = id;
                    tlvi.ChildNodes.Add(new TreeListViewItem());
                    tlvi.Collapse();
					tlvi.Tag = group;
					tlvi.ImageIndex = 0;
					groupGridP4ObjectTreeListView.Items.Add(tlvi);
				}

			}
			ClosedByDoubleClick = false;
		}

		private GroupsBrowserDlg()
		{
			PreferenceKey = "GroupsBrowserDlg";

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
			imageList1 = new System.Windows.Forms.ImageList(components);

			// 
			// imageList1
			// 
			imageList1.TransparentColor = System.Drawing.Color.Transparent;
			imageList1.Images.Add("groups_icon.png", Images.groups_icon);
            imageList1.Images.Add("users_icon.png", Images.users_icon);

            this.groupGridP4ObjectTreeListView.LargeImageList = this.imageList1;
			this.groupGridP4ObjectTreeListView.SmallImageList = this.imageList1;
		}

		public P4.Group SelectedGroup
        {
            get
            {
                if (this.groupGridP4ObjectTreeListView.SelectedItems.Count > 0)
                {
                    return (P4.Group)this.groupGridP4ObjectTreeListView.SelectedItems[0].Tag;
                }
                return null;
            }
        }

		public P4ScmProvider Scm { get; private set; }

		public P4.Group Show(P4ScmProvider scm)
		{
			if (this.DialogResult == DialogResult.OK)
			{
				return SelectedGroup;
			}
			return null;
		}

		private void OKBtn_Click(object sender, EventArgs e)
		{
			if (SelectedGroup == null)
			{
				this.DialogResult=DialogResult.Cancel;
			}
		}

		public bool ClosedByDoubleClick { get; private set; }

        private void groupGridP4ObjectTreeListView_BeforeExpand(object Sender, TreeListViewEventArgs args)
        {
            TreeListViewItem tlvi = args.Node;
            tlvi.ChildNodes.Clear();
            P4.Group group = Scm.GetGroup(tlvi.Name);

            if (group.UserNames!=null)
            {
                foreach (string user in group.UserNames)
                {
                    TreeListViewItem groupMemeber = new TreeListViewItem();
                    groupMemeber.Name = user;
                    groupMemeber.Text = user;
                    groupMemeber.ImageIndex = 1;
                    tlvi.ChildNodes.Add(groupMemeber);
                }
            }

            if (group.SubGroups != null)
            {
                foreach (string subGroup in group.SubGroups)
                {
                    P4.Group groupSpec = new P4.Group(subGroup);
                    TreeListViewItem groupMemeber = new TreeListViewItem();
                    groupMemeber.Tag = groupSpec;
                    groupMemeber.Name = subGroup;
                    groupMemeber.Text = subGroup;
                    groupMemeber.ImageIndex = 0;
                    groupMemeber.ChildNodes.Add(new TreeListViewItem());
                    groupMemeber.Collapse();
                    tlvi.ChildNodes.Add(groupMemeber);
                }
            }
        }

        private void groupGridP4ObjectTreeListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SelectedGroup != null)
            {
                ClosedByDoubleClick = true;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void groupGridP4ObjectTreeListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeListViewItem selection = new TreeListViewItem();
            if (groupGridP4ObjectTreeListView.SelectedItems!=null&&
                groupGridP4ObjectTreeListView.SelectedItems.Count>0)
            {
                selection = (TreeListViewItem)groupGridP4ObjectTreeListView.SelectedItems[0];
            }

            OKBtn.Enabled = !(selection == null || selection.Tag == null);
        }
    }
}
