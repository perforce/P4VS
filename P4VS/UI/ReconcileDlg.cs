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
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	public partial class ReconcileDlg : SelectChangelistDlgBase
    {
        public P4VsProviderService SccService { get; set; }
        private void Init(IList<P4.FileSpec> addFiles, IList<P4.FileSpec> delFiles,
            IList<P4.FileSpec> editFiles, IList<P4.Changelist> changelists,
            P4VsProviderService sccService)
        {
			PreferenceKey = "ReconcileDlg";

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
            SccService = sccService;

            listView1.CheckBoxes = true;
            if (addFiles != null)
            {
                notInDepotText.Text += " [" + addFiles.Count + "]";
                foreach (P4.FileSpec fs in addFiles)
                {
                    ListViewItem addFile = new ListViewItem();
                    addFile.Checked = true;
                    addFile.SubItems.Add(fs.LocalPath.GetFileName().ToString());
                    addFile.SubItems.Add(fs.LocalPath.Path.Remove(fs.LocalPath.Path.LastIndexOf(@"\")));
                    addFile.Tag = fs.LocalPath.Path;
                    listView1.Items.Add(addFile);
                }
            }

            listView2.CheckBoxes = true;
            if (editFiles!=null)
            {
                modifiedText.Text += " [" + editFiles.Count + "]";
                foreach (P4.FileSpec fs in editFiles)
                {
                    ListViewItem editFile = new ListViewItem();
                    editFile.Checked = true;
                    editFile.SubItems.Add(fs.LocalPath.GetFileName().ToString());
                    editFile.SubItems.Add(fs.LocalPath.Path.Remove(fs.LocalPath.Path.LastIndexOf(@"\")));
                    editFile.Tag = fs.LocalPath.Path;
                    listView2.Items.Add(editFile);
                }
            }

            listView3.CheckBoxes = true;
            if (delFiles != null)
            {
                missingText.Text += " [" + delFiles.Count + "]";
                foreach (P4.FileSpec fs in delFiles)
                {
                    ListViewItem deleteFile = new ListViewItem();
                    deleteFile.Checked = true;
                    deleteFile.SubItems.Add(fs.LocalPath.GetFileName().ToString());
                    deleteFile.SubItems.Add(fs.LocalPath.Path.Remove(fs.LocalPath.Path.LastIndexOf(@"\")));
                    deleteFile.Tag = fs.LocalPath.Path;
                    listView3.Items.Add(deleteFile);
                }
            }

            ItemsCB.Items.Add("New");
            ItemsCB.Items.Add("Default");
            if (changelists != null)
            {
                foreach (P4.Changelist cl in changelists)
                {
                    ItemsCB.Items.Add(cl.Id.ToString() + " - " + cl.Description);
                }
            }
            ItemsCB.SelectedIndex = 1;

        }
		public ReconcileDlg(IList<P4.FileSpec> addFiles, IList<P4.FileSpec> delFiles,
            IList<P4.FileSpec> editFiles, IList<P4.Changelist> changelists,
            P4VsProviderService sccService)

        {
            
			Init(addFiles,delFiles, editFiles, changelists, sccService);
		}

		public int Result
		{
			get 
			{
				if (ItemsCB.SelectedIndex >= 2)
				{
					string[] parts = ItemsCB.Text.Split(' ');
					int changeId = -1;
					if ((parts.Length > 0) && (int.TryParse(parts[0], out changeId)))
					{
						return changeId;
					}
					return -1;
				}
				else if (ItemsCB.SelectedIndex == 0)
				{
					return -1;
				}
				// otherwise it's the default
				return 0;
			}
		}

		private void OkBtn_Click(object sender, EventArgs e)
		{
            int changeId = -1;

            if (ItemsCB.SelectedIndex >= 2)
            {
                string[] parts = ItemsCB.Text.Split(' ');
                
                if (parts.Length > 0)
                {
                    int.TryParse(parts[0], out changeId);
                }
                
            }
            else if (ItemsCB.SelectedIndex == 0)
            {
                // new changelist
                P4.Changelist change = SccService.ScmProvider.Connection.Repository.NewChangelist();
                change.Description = "Reconciled offline work";
                // Make sure files is empty. If default has files in it, a new changelist
                // will automatically get those files.
                change.Files = new List<P4.FileMetaData>();
                change = SccService.ScmProvider.SaveChangelist(change, null);
                
                changeId= change.Id;
            }
            // otherwise it's the default
            else if (ItemsCB.SelectedIndex == 1)
            {
                changeId = 0;
            }

            IList<P4.FileSpec> dlgList = new List<P4.FileSpec>();

            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    P4.FileSpec fs = new P4.FileSpec();
                    fs.LocalPath = new P4.LocalPath(item.Tag.ToString());
                    dlgList.Add(fs);
                }
            }

            foreach (ListViewItem item in listView2.Items)
            {
                if (item.Checked)
                {
                    P4.FileSpec fs = new P4.FileSpec();
                    fs.LocalPath = new P4.LocalPath(item.Tag.ToString());
                    dlgList.Add(fs);
                }
            }

            foreach (ListViewItem item in listView3.Items)
            {
                if (item.Checked)
                {
                    P4.FileSpec fs = new P4.FileSpec();
                    fs.LocalPath = new P4.LocalPath(item.Tag.ToString());
                    dlgList.Add(fs);
                }
            }

            // do nothing if there are no files selected on click of Reconcile
            if (dlgList.Count>0)
            {
                P4.Options sFlags = new P4.Options(P4.ReconcileFilesCmdFlags.NotOpened,
    changeId);
                IList<P4.FileSpec> recList =
                    SccService.ScmProvider.ReconcileStatus(dlgList, sFlags);

                SccService.ScmProvider.BroadcastChangelistUpdate(this, new P4ScmProvider.ChangelistUpdateArgs(changeId,
                    P4ScmProvider.ChangelistUpdateArgs.UpdateType.ContentUpdate));
            }

            this.Close();
        }

		private void ItemsCB_SelectedIndexChanged(object sender, EventArgs e)
		{

			//if (true)
			//{
			//	// changed to/from 'New'
			//	if (true)
			//	{
			//		// was read only, so now description for new changelist
			//	}
			//	else
			//	{
			//		// 'New' is no longer selected, so save the description for new
			//		// in case the user comes back to it.
			//	}
			//}
			//int changelistId = 0;

			//if (ItemsCB.SelectedIndex > 1)
			//{
			//	int idx = ((string) ItemsCB.SelectedItem).IndexOf(' ');
			//	string str = ((string) ItemsCB.SelectedItem).Substring(0,idx);

			//	changelistId = 0;
			//	int.TryParse(str, out changelistId);
			//}
			//else if (ItemsCB.SelectedIndex == 1)
			//{
			//	changelistId = 0;
			//}
			//else
			//{
			//	changelistId = -1;
			//}

			//if (changeMap[changelistId] != null)
			//{
			//}
			//else if (changelistId == 0)
			//{
			//}

			//OkBtn.Enabled = ItemsCB.SelectedIndex >= 0;

			//if (true)
			//{
   //         }
   //         if (OkBtn.Enabled)
   //         {
   //         }
   //         else
   //         {
			//}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
            ColumnClickEventArgs args = new ColumnClickEventArgs(0);
            if (listView1.Items.Count>0)
            {
                listView1_ColumnClick(null, args);
            }

            if (listView2.Items.Count > 0)
            {
                listView2_ColumnClick(null, args);
            }

            if (listView3.Items.Count > 0)
            {
                listView3_ColumnClick(null, args);
            }
		}

		private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.DrawBackground();
                bool value = false;
                try
                {
                    value = Convert.ToBoolean(e.Header.Tag);
                }
                catch (Exception)
                {
                }
                // additional check for all items checked
                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (!lvi.Checked)
                    {
                        value = false;
                        break;
                    }
                    else
                    {
                        value = true;
                    }
                }
                CheckBoxRenderer.DrawCheckBox(e.Graphics,
                    new Point(e.Bounds.Left + 4, e.Bounds.Top + 4),
                    value ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal :
                    System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == 0)
            {
                bool value = false;
                try
                {
                    value = Convert.ToBoolean(this.listView1.Columns[e.Column].Tag);
                }
                catch (Exception)
                {
                }
                this.listView1.Columns[e.Column].Tag = !value;
                foreach (ListViewItem item in this.listView1.Items)
                    item.Checked = !value;

                this.listView1.Invalidate();
            }
        }

        private void listView2_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.DrawBackground();
                bool value = false;
                try
                {
                    value = Convert.ToBoolean(e.Header.Tag);
                }
                catch (Exception)
                {
                }
                // additional check for all items checked
                foreach (ListViewItem lvi in listView2.Items)
                {
                    if (!lvi.Checked)
                    {
                        value = false;
                        break;
                    }
                    else
                    {
                        value = true;
                    }
                }
                CheckBoxRenderer.DrawCheckBox(e.Graphics,
                    new Point(e.Bounds.Left + 4, e.Bounds.Top + 4),
                    value ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal :
                    System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void listView2_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView2_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }
        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == 0)
            {
                bool value = false;
                try
                {
                    value = Convert.ToBoolean(this.listView2.Columns[e.Column].Tag);
                }
                catch (Exception)
                {
                }
                this.listView2.Columns[e.Column].Tag = !value;
                foreach (ListViewItem item in this.listView2.Items)
                    item.Checked = !value;

                this.listView2.Invalidate();
            }
        }

        private void listView3_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.DrawBackground();
                bool value = false;
                try
                {
                    value = Convert.ToBoolean(e.Header.Tag);
                }
                catch (Exception)
                {
                }
                // additional check for all items checked
                foreach (ListViewItem lvi in listView3.Items)
                {
                    if (!lvi.Checked)
                    {
                        value = false;
                        break;
                    }
                    else
                    {
                        value = true;
                    }
                }
                CheckBoxRenderer.DrawCheckBox(e.Graphics,
                    new Point(e.Bounds.Left + 4, e.Bounds.Top + 4),
                    value ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal :
                    System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
            }
            else
            {
                e.DrawDefault = true;
            }
        }
        private void listView3_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView3_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }
        private void listView3_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == 0)
            {
                bool value = false;
                try
                {
                    value = Convert.ToBoolean(this.listView3.Columns[e.Column].Tag);
                }
                catch (Exception)
                {
                }
                this.listView3.Columns[e.Column].Tag = !value;
                foreach (ListViewItem item in this.listView3.Items)
                    item.Checked = !value;

                this.listView3.Invalidate();
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!e.Item.Checked)
            {
                listView1.Columns[0].Tag = false;
            }
            listView1.Refresh();
        }

        private void listView2_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!e.Item.Checked)
            {
                listView2.Columns[0].Tag = false;
            }
            listView2.Refresh();
        }

        private void listView3_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!e.Item.Checked)
            {
                listView3.Columns[0].Tag = false;
            }
            listView3.Refresh();
        }
    }
}
