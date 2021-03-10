using Perforce.P4Scm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using Perforce.P4VS;

namespace Perforce.P4VS
{
    public partial class SubmittedChangelistDlg : AutoSizeForm
    {
        private ImageList JobIcon;
        private ImageList FileIcon;
        private ImageList CheckboxIcons;

        private bool ReadOnly = false;

        private SubmittedChangelistDlg()
        {
            PreferenceKey = "SubmittedChangelistDlg";

            InitializeComponent();
            //this.slidingPanelContainer1.DlgParent = this;
            this.Icon = Images.submitted;

            JobIcon = new System.Windows.Forms.ImageList(this.components);
            // 
            // JobIcon
            // 
            JobIcon.Images.Add("jobs_icon", Images.jobs_icon);
            //JobsListLV.SmallImageList = JobIcon;

            CheckboxIcons = new System.Windows.Forms.ImageList(this.components);
            // 
            // CheckboxIcons
            // 
            CheckboxIcons.TransparentColor = System.Drawing.Color.Transparent;
            CheckboxIcons.Images.Add("noCheckBox.png", Images.noCheckBox);
            CheckboxIcons.Images.Add("CheckBox.png", Images.CheckBox);
            JobsListLV.SmallImageList = CheckboxIcons;

            FileIcon = new System.Windows.Forms.ImageList(this.components);
            // 
            // FileIcon
            // 
            FileIcon.Images.Add("portrait.png", Images.portrait);
            FileListLV.SmallImageList = FileIcon;

            slidingPanelContainer1.LayoutPanel();
            slidingPanelContainer1.Refresh();
        }

        public SubmittedChangelistDlg(P4ScmProvider scm, bool readOnly = false)
        {
            PreferenceKey = "SubmittedChangelistDlg";

            _scm = scm;

            InitializeComponent();
            //this.slidingPanelContainer1.DlgParent = this;
            this.Icon = Images.submitted;

            if (readOnly)
            {
                ReadOnly = readOnly;

                RestrictAccessCB.Enabled = false;
                DescriptionTB.ReadOnly = true;

                BrowseJobsBtn.Enabled = false;
                BrowseJobsBtn.Visible = false;
                BrowseJobsBtn.Height = 0;
                AddJobBtn.Enabled = false;
                AddJobBtn.Visible = false;
                AddJobBtn.Height = 0;
                JobLbl.Visible = false;
                JobLbl.Height = 0;
                JobTB.Enabled = false;
                JobTB.Multiline = true;
                JobTB.Height = 0;

                CancelBtn.Enabled = false;
                CancelBtn.Visible = false;

                OkBtn.Location = CancelBtn.Location;

                SelectAllJobsCB.Enabled = false;
                SelectAllJobsCB.Visible = false;
                SelectAllJobsCB.Text = SelectAllJobsCB.Text.TrimStart(' ');

                JobsListLV.CheckBoxes = false;
            }

            JobIcon = new System.Windows.Forms.ImageList(this.components);
            // 
            // JobIcon
            // 
            JobIcon.Images.Add("jobs_icon", Images.jobs_icon);
            //JobsListLV.SmallImageList = JobIcon;

            CheckboxIcons = new System.Windows.Forms.ImageList(this.components);
            // 
            // CheckboxIcons
            // 
            CheckboxIcons.TransparentColor = System.Drawing.Color.Transparent;
            CheckboxIcons.Images.Add("noCheckBox.png", Images.noCheckBox);
            CheckboxIcons.Images.Add("CheckBox.png", Images.CheckBox);
            JobsListLV.SmallImageList = CheckboxIcons;

            FileIcon = new System.Windows.Forms.ImageList(this.components);
            // 
            // FileIcon
            // 
            FileIcon.Images.Add("portrait.png", Images.portrait);
            FileListLV.SmallImageList = FileIcon;

            //DetailsPanel.Collapsed = Preferences.LocalSettings.GetBool("DetailsPanel.Collapsed",false);
            DescriptionPanel.Collapsed = Preferences.LocalSettings.GetBool("DescriptionPanel.Collapsed", false);
            if (DescriptionPanel.Collapsed)
            {
                DescriptionPanel.LayoutPanel();
            }
            FilesPanel.Collapsed = Preferences.LocalSettings.GetBool("FilesPanel.Collapsed", false);
            if (FilesPanel.Collapsed)
            {
                FilesPanel.LayoutPanel();
            }
            JobsPanel.Collapsed = Preferences.LocalSettings.GetBool("JobsPanel.Collapsed", true);
            if (JobsPanel.Collapsed)
            {
                JobsPanel.LayoutPanel();
            }

            slidingPanelContainer1.LayoutPanel();
            slidingPanelContainer1.Refresh();
        }

        public string Description
        {
            get { return DescriptionTB.Text; }
            set
            {
                DescriptionTB.Text = value;
                DescriptionTB_TextChanged(null, null);
            }
        }

        private P4ScmProvider _scm { get; set; }

        // map from depot path to list view item
        private IDictionary<string, P4FileTreeListViewItem> listItemMap = null;

        //// map from depot path to file spec
        //private IDictionary<string, P4.FileSpec> fileSpecMap = null;

        // map from depot path to meta data
        private IDictionary<string, P4.FileMetaData> fileMetaDataMap = null;

        private int _changelistId;
        public int ChangeListId
        {
            get { return _changelistId; }
            set
            {
                _changelistId = value;

                if (ChangeListId > 0)
                {
                    ChangeList = _scm.GetChangelist(ChangeListId, null);
                }
                else
                {
                    ChangeList = _scm.Connection.Repository.NewChangelist();
                    if (_changeList.Description.Trim() == Resources.DefaultChangeListDescription)
                    {
                        _changeList.Description = string.Empty;
                    }
                }
            }
        }

        public string UserText
        {
            set { UserTB.Text = value; }
            get { return UserTB.Text; }
        }
        public string WorkspaceText
        {
            set { WorkspaceTB.Text = value; }
            get { return WorkspaceTB.Text; }
        }

        P4.Changelist _changeList = null;

        public P4.Changelist ChangeList
        {
            get { return _changeList; }
            set
            {
                _changeList = value;
                if (_changeList != null)
                {
                    _changelistId = _changeList.Id;

                    ChangelistTB.Text = _changelistId.ToString();

                    UserTB.Text = _changeList.OwnerName;

                    if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                    {
                        DateTime local = _changeList.ModifiedDate;

                        // we need a pref for local time, until then, don't do this:
                        //local = TimeZone.CurrentTimeZone.ToLocalTime(local);
                        DateTB.Text = local.ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    else
                    {
                        DateTime local = _changeList.ModifiedDate;

                        // we need a pref for local time, until then, don't do this:
                        //local = TimeZone.CurrentTimeZone.ToLocalTime(local);
                        DateTB.Text = string.Format("{0} {1}", local.ToShortDateString(),
                                                            local.ToShortTimeString());
                    }


                    if (_changelistId < 0)
                    {
                        DateTB.Text = string.Empty;
                    }
                    WorkspaceTB.Text = _changeList.ClientId;
                    RestrictAccessCB.Checked = (_changeList.Type == ChangeListType.Restricted);

                    DescriptionTB.Text = _changeList.Description;

                    FileListLV.Items.Clear();

                    listItemMap = new Dictionary<string, P4FileTreeListViewItem>();

                    fileMetaDataMap = new Dictionary<string, P4.FileMetaData>();

                    IList<P4.FileMetaData> mdl = null;

                    if (ChangeListId > 0)
                    {
                        //it's either an existing numbered list
                        mdl = _changeList.Files;
                    }
                    else
                    {
                        // It's the 'default' change list to be turned into a new numbered changelist or
                        //  or a new change list to be edited.
                        P4.Options options = new P4.GetFileMetaDataCmdOptions(
                            P4.GetFileMetadataCmdFlags.Opened,
                            null, null, -1, null, "default", null);
                        string wsPath = string.Format("//{0}/...", _scm.Connection.Repository.Connection.Client.Name);
                        // get files opened in default changelist
                        mdl = _scm.Connection.Repository.GetFileMetaData(options, new P4.FileSpec(new P4.ClientPath(wsPath), null));
                    }

                    if (ChangeList.OwnerName != _scm.Connection.User)
                    {
                        RestrictAccessCB.Enabled = false;
                        DescriptionTB.Enabled = false;
                    }

                    if ((mdl != null) && (mdl.Count > 0))
                    {
                        TreeListViewItem[] newItems = new TreeListViewItem[mdl.Count];
                        listItemMap = new Dictionary<string, P4FileTreeListViewItem>(mdl.Count);
                        fileMetaDataMap = new Dictionary<string, P4.FileMetaData>(mdl.Count);

                        int idx = 0;

                        foreach (P4.FileMetaData file in mdl)
                        {
                            string fileName = file.DepotPath.GetFileName();
                            string inFolder = file.DepotPath.GetDirectoryName();
                            string rev = file.HeadRev.ToString();
                            string fileType = file.Type.ToString();
                            string action = file.Action.ToString();
                            string[] columns = { fileName, inFolder, rev, fileType, action };

                            TreeListViewItem item = new TreeListViewItem(null, columns, 0);
                            item.CenterImageIndices.Clear();
                            item.CenterImageIndices.Add(0);
                            item.Tag = file;
                            newItems[idx++] = item;
                        }
                        FileListLV.Items.AddRange(newItems);
                    }


                    JobsListLV.Items.Clear();
                    if ((_changeList.Jobs != null) && (_changeList.Jobs.Count > 0))
                    {
                        ListViewItem[] newItems = new ListViewItem[_changeList.Jobs.Count];
                        int idx = 0;
                        foreach (string JobId in _changeList.Jobs.Keys)
                        {
                            P4.Job job = _scm.getJob(JobId);
                            ListViewItem item = new ListViewItem(job.Id);
                            item.Tag = job;
                            if (job.ContainsKey("Status") && job["Status"] is string)
                            {
                                item.SubItems.Add(job["Status"] as string);
                            }
                            else
                            {
                                item.SubItems.Add(string.Empty);
                            }
                            if (job.ContainsKey("Description") && job["Description"] is string)
                            {
                                item.SubItems.Add(job["Description"] as string);
                            }
                            else
                            {
                                item.SubItems.Add(string.Empty);
                            }
                            item.Checked = true;
                            item.ImageKey = "jobs_icon";
                            item.ImageIndex = 0;
                            newItems[idx++] = item;
                        }
                        JobsListLV.Items.AddRange(newItems);
                    }
                }
            }
        }

        public IList<P4.FileMetaData> FileList
        {
            get
            {
                List<P4.FileMetaData> value = new List<P4.FileMetaData>();

                foreach (ListViewItem item in FileListLV.Items)
                {
                    value.Add((P4.FileMetaData)item.Tag);
                }
                return value;
            }
        }



        public IList<P4.Job> JobList
        {
            get
            {
                List<P4.Job> value = new List<P4.Job>();

                foreach (ListViewItem item in JobsListLV.Items)
                {
                    value.Add((P4.Job)item.Tag);
                }
                if (value.Count <= 0)
                {
                    return null;
                }
                return value;
            }
            set
            {
                JobsListLV.Items.Clear();
                foreach (P4.Job job in value)
                {
                    ListViewItem item = new ListViewItem(job.Id);
                    item.Tag = job;
                    if (job.ContainsKey("Status") && job["Status"] is string)
                    {
                        item.SubItems.Add(job["Status"] as string);
                    }
                    else
                    {
                        item.SubItems.Add(string.Empty);
                    }
                    if (job.ContainsKey("Description") && job["Description"] is string)
                    {
                        item.SubItems.Add(job["Description"] as string);
                    }
                    else
                    {
                        item.SubItems.Add(string.Empty);
                    }
                    item.Checked = true;
                    item.ImageKey = "jobs_icon";
                    item.ImageIndex = 0;
                    JobsListLV.Items.Add(item);
                }
            }
        }

        public IList<P4.Job> SelectedJobList
        {
            get
            {
                List<P4.Job> value = new List<P4.Job>();

                foreach (ListViewItem item in JobsListLV.Items)
                {
                    if (item.Checked)
                    {
                        value.Add((P4.Job)item.Tag);
                    }
                }
                if (value.Count <= 0)
                {
                    return null;
                }
                return value;
            }
        }

        public IList<P4.Job> UnselectedJobList
        {
            get
            {
                List<P4.Job> value = new List<P4.Job>();

                foreach (ListViewItem item in JobsListLV.Items)
                {
                    if (item.Checked == false)
                    {
                        value.Add((P4.Job)item.Tag);
                    }
                }
                if (value.Count <= 0)
                {
                    return null;
                }
                return value;
            }
        }

        private void DescriptionTB_TextChanged(object sender, EventArgs e)
        {
            DescriptionPanel.ShowAlert = DescriptionTB.Text.Length <= 0;

            OkBtn.Enabled = (DescriptionTB.Text.Length > 0);
        }


        private void JobTB_TextChanged(object sender, EventArgs e)
        {
            AddJobBtn.Enabled = (JobTB.Text.Length > 0);
        }

        private bool JobInList(P4.Job job)
        {
            foreach (ListViewItem item in JobsListLV.Items)
            {
                P4.Job j = (P4.Job)item.Tag;
                if (j.Id == job.Id)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddJobBtn_Click(object sender, EventArgs e)
        {
            P4.Job job = null;
            try
            {
                string filter = string.Format("Job={0}", JobTB.Text);
                IList<P4.Job> shortList = _scm.getJobs(P4.JobsCmdFlags.None, filter, 1, null);
                if (shortList != null && shortList.Count == 1)
                {
                    job = shortList[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if ((job == null) || (JobInList(job)))
            {
                // still null or it's in the list already;
                return;
            }
            ListViewItem item = new ListViewItem(job.Id);
            item.Tag = job;
            if (job.ContainsKey("Status") && job["Status"] is string)
            {
                item.SubItems.Add(job["Status"] as string);
            }
            else
            {
                item.SubItems.Add(string.Empty);
            }
            if (job.ContainsKey("Description") && job["Description"] is string)
            {
                item.SubItems.Add(job["Description"] as string);
            }
            else
            {
                item.SubItems.Add(string.Empty);
            }
            item.Checked = true;
            item.ImageKey = "jobs_icon";
            item.ImageIndex = 0;
            JobsListLV.Items.Add(item);
        }

        private void BrowseJobsBtn_Click(object sender, EventArgs e)
        {
            JobsBrowserDlg dlg = new JobsBrowserDlg(_scm);

            //dlg.scm =

            if (DialogResult.Cancel != dlg.ShowDialog())
            {
                IList<P4.Job> newJobsToAdd = dlg.SelectedJobs;

                if ((newJobsToAdd != null) && (newJobsToAdd.Count > 0))
                {
                    foreach (P4.Job job in newJobsToAdd)
                    {
                        ListViewItem item = new ListViewItem(job.Id);
                        item.Tag = job;

                        if ((job == null) || (JobInList(job)))
                        {
                            // it's null or in the list already;
                            return;
                        }

                        if (job.ContainsKey("Status") && job["Status"] is string)
                        {
                            item.SubItems.Add(job["Status"] as string);
                        }
                        else
                        {
                            item.SubItems.Add(string.Empty);
                        }
                        if (job.ContainsKey("Description") && job["Description"] is string)
                        {
                            item.SubItems.Add(job["Description"] as string);
                        }
                        else
                        {
                            item.SubItems.Add(string.Empty);
                        }
                        item.Checked = true;
                        item.ImageKey = "jobs_icon";
                        item.ImageIndex = 0;
                        JobsListLV.Items.Add(item);
                    }
                }
            }
        }

        private P4.Changelist SaveNumberedChangelist()
        {
            P4.Changelist editChange = _scm.GetChangelist(ChangeListId);
            IList<P4.Job> jobs = SelectedJobList;
            editChange.Jobs = null;
            if (jobs != null && jobs.Count > 0)
            {
                P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.None, -1, null);

                P4.Connection con = _scm.Connection.Repository.Connection;
                editChange.initialize(con);

                editChange.FixJobs(jobs, fixOptions);
            }

            jobs = UnselectedJobList;
            editChange.Jobs = null;
            if (jobs != null && jobs.Count > 0)
            {
                P4.Options fixOptions = new P4.FixJobsCmdOptions(P4.FixJobsCmdFlags.Delete, -1, null);

                P4.Connection con = _scm.Connection.Repository.Connection;
                editChange.initialize(con);

                editChange.FixJobs(jobs, fixOptions);
            }

            if (editChange.OwnerName != _scm.Connection.User)
            {
                return editChange;
            }

            editChange = _scm.GetChangelist(ChangeListId);

            editChange.Type = ChangeListType.Public;
            if (RestrictAccessCB.Checked == true)
            {
                editChange.Type = ChangeListType.Restricted;
            }

            editChange.Description = DescriptionTB.Text;

            editChange.Files = null;

            ChangeList = _scm.SaveSubmittedChangelist(editChange);

            return editChange;
        }

        private void ApplyBtn_Click(object sender, EventArgs e)
        {
            SaveNumberedChangelist();
            this.DialogResult = DialogResult.OK;
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            Preferences.LocalSettings["DescriptionPanel.Collapsed"] = DescriptionPanel.Collapsed;
            Preferences.LocalSettings["FilesPanel.Collapsed"] = FilesPanel.Collapsed;
            Preferences.LocalSettings["JobsPanel.Collapsed"] = JobsPanel.Collapsed;

            // Only save if in edit mode.
            if (!ReadOnly)
            {
                SaveNumberedChangelist();
            }
        }

        #region Drag?Drop support in files list
        private void FileListLV_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void FileListLV_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void FileListLV_DragLeave(object sender, EventArgs e)
        {

        }

        private void FileListLV_DragOver(object sender, DragEventArgs e)
        {

        }

        private void FileListLV_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

        private void FileListLV_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

        }
        #endregion

        private void SelectAllJobsCB_CheckedChanged(object sender, EventArgs e)
        {
            // turn off the individual item check handler, so it doesn't try to change the
            // state of the check all check box
            JobsListLV.ItemChecked += new ItemCheckedEventHandler(JobsListLV_ItemChecked);
            try
            {
                if (SelectAllJobsCB.Checked)
                {
                    foreach (ListViewItem item in JobsListLV.Items)
                    {
                        item.Checked = true;
                    }
                }

                else
                {
                    foreach (ListViewItem item in JobsListLV.Items)
                    {
                        item.Checked = false;
                    }
                }
            }
            finally
            {
                // turn back on the individual item check handler
                JobsListLV.ItemChecked += new ItemCheckedEventHandler(JobsListLV_ItemChecked);
            }
        }

        private void JobsListLV_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked == true)
            {
                e.Item.ImageIndex = 1;
            }
            else
            {
                e.Item.ImageIndex = 0;
            }
            // turn off the select all checked event, so it won't effect other controls
            SelectAllJobsCB.CheckedChanged -= new EventHandler(SelectAllJobsCB_CheckedChanged);
            try
            {
                if (e.Item.Checked == false)
                {
                    // uncheck select all
                    SelectAllJobsCB.Checked = false;
                }
                else
                {
                    foreach (ListViewItem item in JobsListLV.Items)
                    {
                        if (item != null && item.Checked == false)
                        {
                            // if any single one is unchecked, turn off check all
                            SelectAllJobsCB.Checked = false;
                            return;
                        }
                    }
                    // all must be checked, so check check all
                    SelectAllJobsCB.Checked = true;

                }
            }
            finally
            {
                // turn back on the select all checked event, so it won't effect other controls
                SelectAllJobsCB.CheckedChanged += new EventHandler(SelectAllJobsCB_CheckedChanged);
            }
        }

        // select all with ctrl-a
        private void DescriptionTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                DescriptionTB.SelectionStart = 0;
                DescriptionTB.SelectionLength = DescriptionTB.Text.Length;
            }
        }
        private void gridLayoutPanel1_AfterLayoutGrid()
        {
            Point l = this.JobsListLV.Location;

            l.X += 6;
            l.Y += 7;

            this.SelectAllJobsCB.Location = l;
        }

        private void diffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileListLV.SelectedItems != null)
            {
                TreeListViewItem selected = FileListLV.SelectedItems[0] as TreeListViewItem;
                if (selected != null)
                {
                    P4.FileMetaData fmd = (P4.FileMetaData)selected.Tag;
                    P4.FileSpec fileSelectedRev = new P4.FileSpec();
                    fileSelectedRev.DepotPath = fmd.DepotPath;
                    fileSelectedRev.Version = new P4.Revision(fmd.HeadRev);
                    P4.FileSpec filePreviousRev = new P4.FileSpec();
                    filePreviousRev.DepotPath = fmd.DepotPath;
                    filePreviousRev.Version = new P4.Revision(fmd.HeadRev - 1);
                    IList<P4.FileSpec> files = new List<P4.FileSpec>();
                    files.Add(filePreviousRev);
                    files.Add(fileSelectedRev);
                    if (files != null)
                        _scm.Diff2Files(files);
                }
            }
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileListLV.SelectedItems != null)
            {
                TreeListViewItem selected = FileListLV.SelectedItems[0] as TreeListViewItem;
                if (selected != null)
                {
                    P4.FileMetaData fmd = (P4.FileMetaData)selected.Tag;
                    IList<string> files = new List<string>();
                    files.Add(fmd.DepotPath.Path);
                    _scm.SccService._P4VsProvider.P4VsViewHistoryToolWindowExt(files);
                }
            }
        }

        private void TLVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileListLV.SelectedItems != null)
            {
                TreeListViewItem selected = FileListLV.SelectedItems[0] as TreeListViewItem;
                if (selected != null)
                {
                    P4.FileMetaData fmd = (P4.FileMetaData)selected.Tag;
                    _scm.SccService.ScmProvider.LaunchTimeLapseView(fmd.DepotPath.Path);
                }
            }
        }

        private void changelistContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (FileListLV.SelectedItems == null)
            {
                e.Cancel = true;
            }
            TreeListViewItem selected = FileListLV.SelectedItems[0] as TreeListViewItem;
            if (selected != null)
            {
                P4.FileMetaData fmd = (P4.FileMetaData)selected.Tag;
                diffToolStripMenuItem.Visible = fmd.HeadRev != 1;
            }

        }
    }
}
