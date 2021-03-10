using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Perforce.P4;
using Label = System.Windows.Forms.Label;
using Perforce.P4Scm;

namespace Perforce.P4VS.UI
{
    public partial class DlgEditJob : AutoSizeForm
    {
        public DlgEditJob()
        {
            PreferenceKey = "JobDlg";
            InitializeComponent();
            this.Icon = Images.job;

            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            this.fixesLV.ListViewItemSorter = lvwColumnSorter;
        }

        private ListViewColumnSorter lvwColumnSorter;

        public static bool p4Date()
        {
            if (Preferences.LocalSettings.ContainsKey("P4Date_format"))
            {
                if ((bool) Preferences.LocalSettings["P4Date_format"] == true)
                {
                    return true;
                }
            }
            return false;
        }

        public static P4.Job EditJob(P4ScmProvider Scm, P4.Job job)
        {

            P4.Job editedjob = DlgEditJob.Show(Scm, job);
            return editedjob;
        }

        public static P4.FormSpec jobspec { get; set; }
        public static P4ScmProvider scm { get; set; }
        public static P4.Job currentJob { get; set; }
        public static bool dlgRetry = false;
        public static P4.Job Show(P4ScmProvider Scm, P4.Job job)
        {
            if (Scm != null)
            {
                scm = Scm;
            }
            if (job == null)
            {
                return null;
            }
            if (dlgRetry == false)
            {
                attachedFixes = new List<Changelist>();
            }
            dlgRetry = false;
            currentJob = job;

            P4.Options opts = new P4.Options();
            opts["-o"] = null;
            jobspec = Scm.Connection.Repository.GetFormSpec(opts, "job");

            DlgEditJob dlg = new DlgEditJob();
            if (dlg.DialogResult == DialogResult.Cancel)
            {
                return null;
            }

            string jobName = job.Id;
            if (job.Id == "new")
            {
                jobName = "New";
            }
            dlg.Text = dlg.Text + " " + jobName + " (" +
                       Scm.Connection.Repository.Server.Address.ToString() + ", " +
                       Scm.Connection.User + ")";

            dlg.addBtn.Enabled = false;
            dlg.messageLbl.Text = "";

            P4.Job jobInfo = job;

            Dictionary<string, string> fields = jobspec.FieldMap;
            Dictionary<string, string> valueMap = jobspec.Values;

            //string[] fields = Regex.Split(spec, ";;");

            int singleline = 0;

            // determine how many single line fields 
            // exist in the job
            foreach (P4.SpecField field in jobspec.Fields)
            {
                if (field.DataType == SpecFieldDataType.Bulk ||
                    field.DataType == SpecFieldDataType.Text)
                {
                    continue;
                }
                singleline++;
            }

            int leftCol = (singleline + 1)/2;
            int leftcount = 0; //job is there
            int rightcount = 0;
            int bottomcount = 0;
            int idx = 0;
            int lidx = 0; // Job: is 0
            int ridx = 0;
            int bidx = 0;
            int longestLeftLabel = 0;
            int longestRightLabel = 0;
            int longestBottomLabel = 0;
            
            Label[] leftLabels = new Label[jobspec.Fields.Count];
            TextBox[] leftTextboxes = new TextBox[jobspec.Fields.Count]; 
            ComboBox[] leftComboboxes = new ComboBox[jobspec.Fields.Count];
            Label[] rightLabels = new Label[jobspec.Fields.Count];
            TextBox[] rightTextboxes = new TextBox[jobspec.Fields.Count];
            ComboBox[] rightComboboxes = new ComboBox[jobspec.Fields.Count];
            Label[] bottomLabels = new Label[jobspec.Fields.Count];
            RichTextBox[] bottomTextboxes = new RichTextBox[jobspec.Fields.Count];
            ComboBox[] bottomComboboxes = new ComboBox[jobspec.Fields.Count];


            foreach (P4.SpecField field in jobspec.Fields)
            {
                string fieldname = field.Name;
                if (field.DataType == SpecFieldDataType.Bulk ||
                    field.DataType == SpecFieldDataType.Text)
                {
                    bottomLabels[bidx] = new Label();
                    bottomLabels[bidx].AutoSize = true;
                    
                    // set name and location for large text box labels
                    bottomLabels[bidx].Text = fieldname + ":";
                    Point location = dlg.jobLbl.Location;
                    location.Y = location.Y + (25 * leftCol) + (125*bottomcount);
                    bottomLabels[bidx].Location = location;
                    bottomLabels[bidx].Name = fieldname + "Lbl";
                    bottomLabels[bidx].Width = bottomLabels[bidx].PreferredWidth;

                    // determine size of label and check to determine if
                    // the current label is the widest

                    if (bottomLabels[bidx]!=null&& bottomLabels[longestBottomLabel]!=null)
                        if (bidx > 0 && bottomLabels[bidx].Width > bottomLabels[longestBottomLabel].Width)
                    {
                        longestBottomLabel = bidx;
                    }

                    // anchoring
                    bottomLabels[bidx].Anchor = AnchorStyles.Left | AnchorStyles.Top;

                    bottomTextboxes[bidx] = new RichTextBox();
                    location.X += (bottomLabels[bidx].Width + 5);
                    bottomTextboxes[bidx].Name = fieldname + "TB";
                    bottomTextboxes[bidx].Location = location;
                    bottomTextboxes[bidx].Height = 120;
                    bottomTextboxes[bidx].Width = 350;
                    bottomTextboxes[bidx].Multiline = true;
                    bottomTextboxes[bidx].ScrollBars = RichTextBoxScrollBars.Vertical;
                    bottomTextboxes[bidx].WordWrap = true;
                    bottomTextboxes[bidx].ShortcutsEnabled = true;
                    if (job.ContainsKey(field.Name))
                    {
                        if (job[field.Name].ToString() == "<enter description here>\n")
                        {
                            bottomTextboxes[bidx].Text = job[field.Name].ToString();
                        }
                        else
                        {
                            bottomTextboxes[bidx].Text = job[field.Name].ToString().Replace("\n", "\r\n");
                        }
                    }

                    // anchoring
                    bottomTextboxes[bidx].Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;

                    bottomcount++;
                    bidx++;
                }

                else
                {

                    Point location = new Point();
                    
                    
                    if (leftcount < leftCol)
                    {
                        leftLabels[lidx] = new Label();
                        leftLabels[lidx].Text = fieldname + ":";
                        location = dlg.jobLbl.Location;
                        location.Y += (25*leftcount);

                        // anchoring
                        leftLabels[lidx].Anchor = AnchorStyles.Left | AnchorStyles.Top;

                        leftLabels[lidx].Location = location;
                        leftLabels[lidx].Name = fieldname + "Lbl";
                        leftLabels[lidx].Width = leftLabels[lidx].PreferredWidth;

                        // determine size of label and check to determine if
                        // the current label is the widest

                        if (leftLabels[lidx] != null && leftLabels[longestLeftLabel] != null)
                            if (lidx > 0 && leftLabels[lidx].Width > leftLabels[longestLeftLabel].Width)
                            {
                                longestLeftLabel = lidx;
                            }
                    }
                    else
                    {
                        rightLabels[ridx] = new Label();
                        //string fieldname = field.Name;
                        rightLabels[ridx].Text = fieldname + ":";
                        location = dlg.jobLbl.Location;
                        location.Y += (25*rightcount);
                        //int indent = (dlg.jobLbl.Width + 160);
                        //location.X = location.X + indent;

                        location.X = dlg.splitContainer.Panel1.Width/2;
                        
                        
                        // anchoring
                        rightLabels[ridx].Anchor = AnchorStyles.Top | AnchorStyles.Left;

                        rightLabels[ridx].Location = location;
                        rightLabels[ridx].Name = fieldname + "Lbl";
                        rightLabels[ridx].Width = rightLabels[ridx].PreferredWidth;

                        // determine size of label and check to determine if
                        // the current label is the widest

                        if (rightLabels[ridx] != null && rightLabels[longestRightLabel] != null)
                            if (ridx > 0 && rightLabels[ridx].Width > rightLabels[longestRightLabel].Width)
                            {
                                longestRightLabel = ridx;
                            }
                    }
                    

                    

                    if (field.DataType == SpecFieldDataType.Select)
                    {
                        

                        // anchoring
                        if (leftcount < leftCol)
                        {
                            leftComboboxes[lidx] = new ComboBox();
                            leftComboboxes[lidx].DropDownStyle = ComboBoxStyle.DropDownList;
                            leftComboboxes[lidx].Width = 100;
                            leftComboboxes[lidx].Height = 20;
                            int tab = leftLabels[lidx].Width + 5;
                            location.X = leftLabels[lidx].Location.X + tab;
                            leftComboboxes[lidx].Name = fieldname + "TB";
                            leftComboboxes[lidx].Location = location;
                            string values = valueMap[field.Name];
                            string[] val = values.Split('/');
                            leftComboboxes[lidx].Items.AddRange(val);
                            if (job.ContainsKey(fieldname))
                            {
                                object sel = null;
                                job.TryGetValue(fieldname, out sel);
                                string selected = sel.ToString();
                                if (selected != null && selected != "\"\"")
                                {

                                    leftComboboxes[lidx].SelectedItem = selected;
                                }
                                else
                                {
                                    leftComboboxes[lidx].SelectedItem = val[0];
                                }
                            }
                            leftComboboxes[lidx].Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            lidx++;
                            leftcount++;
                        }
                        else
                        {
                            rightComboboxes[ridx] = new ComboBox();
                            rightComboboxes[ridx].DropDownStyle = ComboBoxStyle.DropDownList;
                            rightComboboxes[ridx].Width = 100;
                            rightComboboxes[ridx].Height = 20;
                            int tab = rightLabels[ridx].Width + 5;
                            location.X = rightLabels[ridx].Location.X + tab;
                            rightComboboxes[ridx].Name = fieldname + "TB";
                            rightComboboxes[ridx].Location = location;
                            string values = valueMap[field.Name];
                            string[] val = values.Split('/');
                            rightComboboxes[ridx].Items.AddRange(val);
                            if (job.ContainsKey(fieldname))
                            {
                                object sel = null;
                                job.TryGetValue(fieldname, out sel);
                                string selected = sel.ToString();
                                if (selected != null && selected != "\"\"")
                                {

                                    rightComboboxes[ridx].SelectedItem = selected;
                                }
                                else
                                {
                                    rightComboboxes[ridx].SelectedItem = val[0];
                                }
                            }
                            rightComboboxes[ridx].Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                            ridx++;
                            rightcount++;
                        }
                    }

                    else
                    {
                        // anchoring
                        if (leftcount < leftCol)
                        {
                            leftTextboxes[lidx] = new TextBox();

                            //leftTextboxes[lidx].AutoSize = true;                 
                            int tab = leftLabels[lidx].Width + 5;
                            location.X = leftLabels[lidx].Location.X + tab;
                            leftTextboxes[lidx].Name = fieldname + "TB";
                            leftTextboxes[lidx].Location = location;
                            leftTextboxes[lidx].Height = 15;
                            leftTextboxes[lidx].Width = 100;
                            leftTextboxes[lidx].ShortcutsEnabled = true;
                            if (field.FieldType == SpecFieldFieldType.Always ||
                                field.FieldType == SpecFieldFieldType.Once ||
                                field.Code == 101)
                            {
                                leftTextboxes[lidx].ReadOnly = true;
                            }
                            if (job.ContainsKey(field.Name))
                            {
                                if ((field.DataType == P4.SpecFieldDataType.Date)
                                    && p4Date() == false)
                                {
                                    DateTime d;
                                    DateTime.TryParse(job[field.Name].ToString(), out d);
                                    leftTextboxes[lidx].Text = d.ToString("MM/d/yyyy h:mm:ss tt");
                                }
                                else
                                {
                                    leftTextboxes[lidx].Text = job[field.Name].ToString();
                                }
                            }
                            leftTextboxes[lidx].Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            lidx++;
                            leftcount++;
                        }
                        else
                        {
                            rightTextboxes[ridx] = new TextBox();
                            int tab = rightLabels[ridx].Width + 8;
                            location.X = rightLabels[ridx].Location.X + tab;
                            rightTextboxes[ridx].Name = fieldname + "TB";
                            rightTextboxes[ridx].Location = location;
                            rightTextboxes[ridx].Height = 15;
                            rightTextboxes[ridx].Width = 100;
                            rightTextboxes[ridx].ShortcutsEnabled = true;
                            if (field.FieldType == SpecFieldFieldType.Always ||
                                field.FieldType == SpecFieldFieldType.Once ||
                                field.Code == 101)
                            {
                                rightTextboxes[ridx].ReadOnly = true;
                            }
                            if (job.ContainsKey(field.Name))
                            {
                                if ((field.DataType == P4.SpecFieldDataType.Date)
                                    && p4Date() == false)
                                {
                                    DateTime d;
                                    DateTime.TryParse(job[field.Name].ToString(), out d);
                                    rightTextboxes[ridx].Text = d.ToString("MM/d/yyyy h:mm:ss tt");
                                }
                                else
                                {
                                    rightTextboxes[ridx].Text = job[field.Name].ToString();
                                }
                            }
                            rightTextboxes[ridx].Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                            ridx++;
                            rightcount++;
                        }
                    }
                }
            }

            // adjust all the placements based on largest label size
            // and dialog real estate remaining

            int fieldX = 0;
            int fieldWidth = 0;

            // bottom labels and textboxes
            

            if (leftLabels[longestLeftLabel].Width >= bottomLabels[longestBottomLabel].Width)
            {
                fieldX = leftLabels[longestLeftLabel].Width + 5 +
                leftLabels[longestLeftLabel].Location.X;
                fieldWidth = dlg.splitContainer.Panel1.Width - (fieldX + 10);
            }
            else
            {
                fieldX = bottomLabels[longestBottomLabel].Width + 5 +
                leftLabels[longestBottomLabel].Location.X;
                fieldWidth = dlg.splitContainer.Panel1.Width - (fieldX+10);
            }
            foreach (RichTextBox bottomTextbox in bottomTextboxes)
            {
                if (bottomTextbox != null)
                {
                    bottomTextbox.SetBounds(fieldX,bottomTextbox.Location.Y,
                        fieldWidth, bottomTextbox.Size.Height);
                }
            }

 
            // left labels and textboxes / comboboxes
            if (leftLabels[longestLeftLabel].Width >= bottomLabels[longestBottomLabel].Width)
            {
                fieldX = leftLabels[longestLeftLabel].Width + 5 +
                leftLabels[longestLeftLabel].Location.X;
                fieldWidth = (dlg.splitContainer.Panel1.Width / 2) - fieldX;
            }
            else
            {
                fieldX = bottomLabels[longestBottomLabel].Width + 5 +
                leftLabels[longestBottomLabel].Location.X;
                fieldWidth = (dlg.splitContainer.Panel1.Width/2) - fieldX;
            }
                foreach (ComboBox leftCombobox in leftComboboxes)
                {
                    if (leftCombobox != null)
                    {
                        leftCombobox.SetBounds(fieldX,leftCombobox.Location.Y,
                            fieldWidth,leftCombobox.Size.Height);
                    }
                }
                foreach (TextBox leftTextbox in leftTextboxes)
                {
                    if (leftTextbox != null)
                    {
                        leftTextbox.SetBounds(fieldX,leftTextbox.Location.Y,
                            fieldWidth,leftTextbox.Size.Height);
                    }
                }

            // right labels and textboxes / comboboxes
                fieldX = dlg.splitContainer.Panel1.Width/2+5+
                    rightLabels[longestRightLabel].Width;
                fieldWidth = (dlg.splitContainer.Panel1.Width / 2) -
                    (rightLabels[longestRightLabel].Width + 10);
                foreach (ComboBox rightCombobox in rightComboboxes)
                {
                    if (rightCombobox != null)
                    {
                        rightCombobox.SetBounds(fieldX,rightCombobox.Location.Y,
                            fieldWidth,rightCombobox.Size.Height);
                    }
                }
                foreach (TextBox rightTextbox in rightTextboxes)
                {
                    if (rightTextbox != null)
                    {
                        rightTextbox.SetBounds(fieldX,rightTextbox.Location.Y,
                        fieldWidth,rightTextbox.Size.Height);
                    }
                }
 

            dlg.splitContainer.Panel1.Controls.AddRange(leftLabels);
            dlg.splitContainer.Panel1.Controls.AddRange(leftTextboxes);
            dlg.splitContainer.Panel1.Controls.AddRange(leftComboboxes);
            dlg.splitContainer.Panel1.Controls.AddRange(rightLabels);
            dlg.splitContainer.Panel1.Controls.AddRange(rightTextboxes);
            dlg.splitContainer.Panel1.Controls.AddRange(rightComboboxes);
            dlg.splitContainer.Panel1.Controls.AddRange(bottomLabels);
            dlg.splitContainer.Panel1.Controls.AddRange(bottomTextboxes);
            dlg.splitContainer.Panel1.Controls.AddRange(bottomComboboxes);

            dlg.splitContainer.Panel1.Refresh();

            // add fixes here
            Options fixOpts = new Options(GetFixesCmdFlags.IncludeIntegrations, -1, job.Id, -1);
            IList<Fix> fixes = Scm.Connection.Repository.GetFixes(null, fixOpts);

            if (fixes != null)
            {
                foreach (Fix fix in fixes)
                {
                    Changelist change = Scm.GetChangelist(fix.ChangeId);

                    if (change != null)
                    {
                        dlg.addChangeToFixesLV(change);
                        attachedFixes.Add(change);
                    }
                }
            }
            
            if (attachedFixes.Count>0)
                foreach (Changelist fix in attachedFixes)
                {
                    if (fix != null)
                    {
                        dlg.addChangeToFixesLV(fix);
                    }
                }


            if (dlg.ShowDialog() != DialogResult.Cancel)
            {
                idx = 0;
                if (dlg.DialogResult == DialogResult.OK)
                {
                    foreach (P4.SpecField field in jobspec.Fields)
                    {
                        string fieldname = field.Name;

                        Control[] control = dlg.splitContainer.Panel1.Controls.Find(fieldname + "TB", false);
                        if (control != null &&control.Length>0&& control[0] != null)
                        {

                            if ((field.DataType == SpecFieldDataType.Bulk ||
                                field.DataType == SpecFieldDataType.Text)&&
                                control[0].Text != "<enter description here>\n")
                            {
                                job.IsFieldMultiLine[fieldname] = true;
                            }
                            if (field.DataType == SpecFieldDataType.Date)
                            {
                                job[fieldname] = "";
                            }
                            else
                            {
                                job[fieldname] = control[0].Text;
                            }

                        }
                        idx++;

                    }

                    P4.Job savedJob = scm.saveJob(job);
                    if (savedJob == null)
                    {
                        dlgRetry = true;
                        //dlg.Show();
                        Show(Scm, job);
                        return job;
                    }
                   if (dlg.SelectedChangelistList != null)
                    {
                       
                       foreach (P4.Changelist change in dlg.SelectedChangelistList)
                       {
                           try
                           {
                               opts = new P4.Options(P4.FixJobsCmdFlags.None, -1, null);
                               IList<P4.Fix> fixToAttach = change.FixJobs(opts, savedJob);
                           }
                           catch (Exception ex)
                           {
                               Scm.ShowException(ex);
                           }
                       }
                    }

                    if (dlg.UnselectedChangelistList != null)
                    {
                        
                        foreach (P4.Changelist change in dlg.UnselectedChangelistList)
                        {
                            if (change != null&& (change.Jobs!=null&&change.Jobs.ContainsKey(currentJob.Id)))
                            {
                                try
                                {
                                    opts = new P4.Options(P4.FixJobsCmdFlags.Delete, -1, null);
                                    IList<P4.Fix> fixToRemove = change.FixJobs(opts, savedJob);
                                }
                                catch (Exception ex)
                                {
                                    Scm.ShowException(ex);
                                }
                            }
                        }
                    }

                    return savedJob;
                }
            }

            return null;
        }

        public IList<P4.Changelist> SelectedChangelistList
        {
            get
            {
                List<P4.Changelist> value = new List<P4.Changelist>();

                foreach (P4FileTreeListViewItem item in fixesLV.Items)
                {
                    if (item.Checked)
                    {
                        // if the item already exists as a fix, don't add
                        // if to the list
                        P4.Changelist checkForExistingFix = (P4.Changelist) item.Tag;
                        if (checkForExistingFix.Jobs != null &&
                            checkForExistingFix.Jobs.Keys.Contains(currentJob.Id))
                        {
                            continue;
                        }
                        value.Add((P4.Changelist)item.Tag);
                    }
                }
                if (value.Count <= 0)
                {
                    return null;
                }
                return value;
            }
        }

        public IList<P4.Changelist> UnselectedChangelistList
        {
            get
            {
                List<P4.Changelist> value = new List<P4.Changelist>();

                foreach (P4FileTreeListViewItem item in fixesLV.Items)
                {
                    if (item.Checked == false)
                    {
                        value.Add(item.Tag as P4.Changelist);
                        //value.Add((P4.Changelist)item.Tag);
                    }
                }
                if (value.Count <= 0)
                {
                    return null;
                }
                return value;
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            // add changelist to list if it exists
            Int32 changeNum=0;
            if (changelistTB.Text!=null)
            {
                changeNum = Convert.ToInt32(changelistTB.Text);
            }
            Changelist change = scm.GetChangelist(changeNum);
            if (change!=null)
            {
                addChangeToFixesLV(change);
                attachedFixes.Add(change);
                changelistTB.Text = "";
            }
            else if (scm.Connection.Repository.Connection.LastResults.ErrorList != null)
            {
                if (scm.Connection.Repository.Connection.LastResults.ErrorList[0].ErrorMessage.Contains("no such"))
                {
                    messageLbl.Text = "Changelist  '" + changelistTB.Text + "' does not exist.";
                }
                addBtn.Enabled = false;
            }
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            
                SubmittedChangelistsBrowserDlg dlg = new SubmittedChangelistsBrowserDlg(scm, "get_revision");

                if (DialogResult.Cancel != dlg.ShowDialog())
                {
                    if (dlg.SelectedChangelist != null)
                    {
                        P4.Changelist changeToAdd = scm.GetChangelist(dlg.SelectedChangelist.Id);
                        if (changeToAdd!=null)
                        {
                            addChangeToFixesLV(changeToAdd);
                            attachedFixes.Add(changeToAdd);
                        }
                    }
                }

        }

        private static IList<P4.Changelist > attachedFixes=new List<Changelist>();

        private void addChangeToFixesLV(P4.Changelist change)
        {
            if (change != null)
            {
                ListViewItem[] dupe = fixesLV.Items.Find(change.Id.ToString(),false);
                
                if (dupe.Length==0)
                {
                    List<object> changeFields = new List<object>();
                    changeFields.Add(change.Id.ToString());
                    changeFields.Add(change.ClientId.ToString());
                    // check for date format
                    if (p4Date() == true)
                    {
                        DateTime local = change.ModifiedDate;

                        // we need a pref for local time, until then, don't do this:
                        //local = TimeZone.CurrentTimeZone.ToLocalTime(local);
                        changeFields.Add(local.ToString("yyyy/MM/dd HH:mm:ss"));
                    }
                    else
                    {
                        DateTime local = change.ModifiedDate;

                        // we need a pref for local time, until then, don't do this:
                        //local = TimeZone.CurrentTimeZone.ToLocalTime(local);
                        changeFields.Add(string.Format("{0} {1}", local.ToShortDateString(),
                                                       local.ToShortTimeString()));
                    }

                    changeFields.Add(change.OwnerName.ToString());
                    changeFields.Add(change.Type.ToString());
                    changeFields.Add(change.Description.ToString());
                    P4FileTreeListViewItem changeItem = new P4FileTreeListViewItem(null, null, changeFields);
                    changeItem.Tag = change;
                    if (change.Pending)
                    {
                        changeItem.CenterImageIndices.Clear();
                        changeItem.CenterImageIndices.Add(6);
                    }
                    else
                    {
                        changeItem.CenterImageIndices.Clear();
                        changeItem.CenterImageIndices.Add(15);
                    }
                    changeItem.Checked = true;
                    changeItem.Name = change.Id.ToString();
                    FileMetaData blank = new FileMetaData();
                    P4FileTreeListViewItem childItem = new P4FileTreeListViewItem(changeItem, blank, changeFields);
                    changeItem.ChildNodes.Add(childItem);
                    changeItem.Collapse();
                    fixesLV.Nodes.Add(changeItem);
                }
            }
            fixesLV.BuildTreeList();
        }

        private void fixesContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // check to confirm that an item is selected
            if (fixesLV.SelectedItems!=null&&fixesLV.SelectedItems.Count>0)
            {
                // check here to confirm that a changelist is the selected item
                P4FileTreeListViewItem selected = fixesLV.SelectedItems[0] as P4FileTreeListViewItem;
                P4.FileMetaData file = selected.FileData;
                if (selected.ParentItem == null)
                {
                    P4.Changelist fix = selected.Tag as P4.Changelist;
                    if (fix != null && fix.Jobs!=null && fix.Jobs.ContainsKey(currentJob.Id))
                    {
                        removeFixToolStripMenuItem.Enabled = true;
                        removeFixToolStripMenuItem.Visible = true;
                        diffAgainstPreviousToolStripMenuItem.Enabled = false;
                        diffAgainstPreviousToolStripMenuItem.Visible = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else if (file != null && file.HeadRev > 1)
                {
                    diffAgainstPreviousToolStripMenuItem.Enabled = true;
                    diffAgainstPreviousToolStripMenuItem.Visible = true;
                    removeFixToolStripMenuItem.Enabled = false;
                    removeFixToolStripMenuItem.Visible = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

 
        private void fixesLV_BeforeExpand(object Sender, TreeListViewEventArgs args)
        {
            P4.Changelist changeToExpand = args.Node.Tag as P4.Changelist;
            args.Node.ChildNodes.Clear();
            if ((changeToExpand.Files == null || changeToExpand.Files.Count == 0) &&
                (changeToExpand.Jobs == null || changeToExpand.Jobs.Count == 0))
            {
                return;
            }

            foreach (P4.FileMetaData file in changeToExpand.Files)
            {
                IList<object> fields = new List<object>();
                fields.Add(file.DepotPath+"#"+file.HeadRev.ToString());
                P4FileTreeListViewItem childItem = new P4FileTreeListViewItem(args.Node,
                    file, fields);
                childItem.Tag = file;
                if (!(changeToExpand.Pending))
                {
                    childItem.LeftImageIndices.Clear();
                    childItem.RightImageIndices.Clear();

                    if (file.Action == P4.FileAction.Add ||
                    file.Action == P4.FileAction.AddInto ||
                    file.Action == P4.FileAction.Added)
                    {
                        childItem.CenterImageIndices.Clear();
                        childItem.CenterImageIndices.Add(17); //add icon
                    }

                    if (file.Action == P4.FileAction.None)
                    {
                        childItem.CenterImageIndices.Clear();
                        childItem.CenterImageIndices.Add(18); //archive icon
                    }

                    if (file.Action == P4.FileAction.Branch ||
                        file.Action == P4.FileAction.BranchFrom ||
                        file.Action == P4.FileAction.BranchInto)
                    {
                        childItem.CenterImageIndices.Clear();
                        childItem.CenterImageIndices.Add(19); //branch icon
                    }

                    if (file.Action == P4.FileAction.Delete ||
                        file.Action == P4.FileAction.DeleteFrom ||
                        file.Action == P4.FileAction.DeleteInto)
                    {
                        childItem.CenterImageIndices.Clear();
                        childItem.CenterImageIndices.Add(20); //delete icon
                    }

                    if (file.Action == P4.FileAction.Edit ||
                        file.Action == P4.FileAction.EditFrom ||
                        file.Action == P4.FileAction.EditIgnored ||
                        file.Action == P4.FileAction.EditInto)
                    {
                        childItem.CenterImageIndices.Clear();
                        childItem.CenterImageIndices.Add(21); //edit icon
                    }

                    if (file.Action == P4.FileAction.Integrate ||
                        file.Action == P4.FileAction.CopyFrom ||
                        file.Action == P4.FileAction.CopyInto ||
                        file.Action == P4.FileAction.MergeFrom ||
                        file.Action == P4.FileAction.MergeInto)
                    {
                        childItem.CenterImageIndices.Clear();
                        childItem.CenterImageIndices.Add(22); //integrate icon
                    }

                    if (file.Action == P4.FileAction.MoveAdd)
                    {
                        childItem.CenterImageIndices.Clear();
                        childItem.CenterImageIndices.Add(23); //moveadd icon
                    }

                    if (file.Action == P4.FileAction.MoveDelete)
                    {
                        childItem.CenterImageIndices.Clear();
                        childItem.CenterImageIndices.Add(24); //movedelete icon
                    }

                    if (file.Action == P4.FileAction.Purge)
                    {
                        childItem.CenterImageIndices.Clear();
                        childItem.CenterImageIndices.Add(25); //purge icon
                    }
                }
                
                args.Node.ChildNodes.Add(childItem);
            }

            if (changeToExpand.Jobs != null)
            {
                foreach (KeyValuePair<string, string> job in changeToExpand.Jobs)
                {
                    IList<object> fields = new List<object>();
                    fields.Add(job.Key);
                    P4FileTreeListViewItem childItem = new P4FileTreeListViewItem(args.Node,
                                                                                  null, fields);
                    childItem.CenterImageIndices.Clear();
                    childItem.LeftImageIndices.Clear();
                    childItem.RightImageIndices.Clear();
                    childItem.CenterImageIndices.Add(16); //job icon

                    args.Node.ChildNodes.Add(childItem);
                }
            }
            
        }

        private void removeFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // remove the selected changelist from the job

            P4.Changelist selected = fixesLV.SelectedItems[0].Tag as P4.Changelist;
            try
            {
                P4.Options opts = new P4.Options(P4.FixJobsCmdFlags.Delete, -1, null);
                IList<P4.Fix> fixToRemove = selected.FixJobs(opts, currentJob);
            }
            catch (Exception ex)
            {
                scm.ShowException(ex);
                return;
            }
            attachedFixes.Remove(selected);
            P4FileTreeListViewItem itemToRemove = fixesLV.SelectedItems[0] as P4FileTreeListViewItem;
            foreach (TreeListViewItem child in itemToRemove.ChildNodes)
            {
                fixesLV.Items.Remove(child);
            }

            fixesLV.Nodes.Remove(itemToRemove);
            fixesLV.Items.Remove(itemToRemove);
            fixesLV.BuildTreeList();
        }

        private void changelistTB_TextChanged(object sender, EventArgs e)
        {
            messageLbl.Text = "";
            if (string.IsNullOrEmpty(changelistTB.Text))
            {
                addBtn.Enabled = false;
            }
            else
            {
                addBtn.Enabled = true;
            }
        }

        private void changelistTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)
                &&(e.KeyChar!='\b'))
            {
                e.Handled = true;
            }
            
            if (char.IsDigit(e.KeyChar))
            {
                string newDigit = e.KeyChar.ToString();
                string testText = changelistTB.Text;
                string selected = changelistTB.SelectedText;
                if (changelistTB.SelectedText != "")
                {
                    testText = testText.Replace(selected, newDigit);
                }
                else
                {
                    int x = changelistTB.Cursor.Handle.ToInt32();
                    testText = testText.Insert(changelistTB.SelectionStart, newDigit);
                }
                Int32 intCheck = 0;
                int.TryParse(testText, out intCheck);
                if (intCheck == 0)
                {
                    e.Handled = true;
                }
            }
        }

        private void diffAgainstPreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fixesLV.SelectedItems != null && fixesLV.SelectedItems.Count > 0)
            {
                TreeListViewItem selected = fixesLV.SelectedItems[0] as TreeListViewItem;
                if (selected != null)
                {
                    P4.FileMetaData fmd = (P4.FileMetaData)selected.Tag;
                    P4.FileSpec file = new P4.FileSpec();
                    file.DepotPath = fmd.DepotPath;
                    file.Version = new P4.Revision(fmd.HeadRev);
                    P4.FileSpec file2 = new P4.FileSpec();
                    file2.DepotPath = fmd.DepotPath;
                    file2.Version = new P4.Revision(fmd.HeadRev - 1);
                    IList<P4.FileSpec> files = new List<P4.FileSpec>();
                    files.Add(file);
                    files.Add(file2);
                    if (files != null)
                        scm.Diff2Files(files);
                }
            }
        }

        private void fixesLV_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            if (e.NewDisplayIndex == 0 | e.OldDisplayIndex == 0)
            {
                e.Cancel = true;
            }
        }

        private void fixesLV_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // set the header text to what it already is to force a
                // redraw of the previously selected column header.
                fixesLV.Columns[lvwColumnSorter.SortColumn].Text =
                    fixesLV.Columns[lvwColumnSorter.SortColumn].Text;

                // Set the column number that is to be sorted; default to ascending.

                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.fixesLV.Sort();
        }

        private void fixesLV_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            //if (e.ColumnIndex == 0)
            //{
            //    if (fixesLV.Columns[0].Width<140)
            //    {
            //        fixesLV.Columns[0].Width = 140;
            //    }
            //}
        }

        private void fixesLV_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.Cancel = true;
            }
        }

    }


}
