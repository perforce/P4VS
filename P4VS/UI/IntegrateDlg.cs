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
	public partial class IntegrateDlg : AutoSizeForm
	{
		public IntegrateDlg(List<string> source, string target, string action, P4ScmProvider scm)
		{
			PreferenceKey = "IntegrateDlg";

			_target = target;
			_source = source;
			_action = action;
			_scm = scm;
			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
            if (scm.Connection.Repository==null)
            {
                return;
            }
            Text = Text + " (" + scm.Connection.Repository.Server.Address.Uri + ", " +
                scm.Connection.User + ", " + scm.Connection.Workspace + ")";
		}

		public IList<P4.FileSpec> returnedFiles = new List<P4.FileSpec>();
		public string _target { get; set; }
		public List<string> _source { get; set; }
		public string _action { get; set; }

		private P4ScmProvider _scm { get; set; }

		public static IList<P4.FileSpec> Show(List<string> source, string target, string action, P4ScmProvider scm)
		{
			IntegrateDlg dlg = new IntegrateDlg(source, target, action, scm);
            if (action == "merge")
           {
                dlg.addToPendingRB.Enabled = false;
                dlg.submitCopiedRB.Enabled = false;
                dlg.addToPendingRB.Visible = false;
                dlg.submitCopiedRB.Visible = false;
                dlg.gridLayoutSubpanel1.Visible = false;

				dlg.Text = Resources.IntegrateDlg_Merge;
				dlg.methodLbl.Text = Resources.IntegrateDlg_MergeMethod;
				dlg.actionBtn.Text = Resources.IntegrateDlg_Merge;
				dlg.methodTB.Text = Resources.IntegrateDlg_StreamToStream;
			}
			if (action == "copy")
			{
                dlg.addToPendingRB.Enabled = true;
                dlg.submitCopiedRB.Enabled = true;
                dlg.addToPendingRB.Visible = true;
                dlg.submitCopiedRB.Visible = true;
                dlg.gridLayoutSubpanel1.Visible = true;

                dlg.addToPendingRB.Checked =
                    (Preferences.LocalSettings.GetBool("no_autosubmit_copy", true));
                dlg.submitCopiedRB.Checked = !(dlg.addToPendingRB.Checked);

				dlg.Text = Resources.IntegrateDlg_Copy;
				dlg.methodLbl.Text = Resources.IntegrateDlg_CopyMethod;
				dlg.actionBtn.Text = Resources.IntegrateDlg_Copy;
				dlg.methodTB.Text = Resources.IntegrateDlg_StreamToStream;
			}
            dlg.gridLayoutPanel1.LayoutGrid();

            if (scm.Connection.Repository == null)
            {
                return new List<P4.FileSpec>();
            }
            dlg.Text = dlg.Text + " (" + scm.Connection.Repository.Server.Address.Uri + ", " +
               scm.Connection.User + ", " + scm.Connection.Workspace + ")";
            
			if (source != null)
			{
                if (source.Count>=2)
                {
                    dlg.sourceCB.Enabled = true;
                    dlg.sourceCB.Visible = true;
                    dlg.sourceTB.Enabled = false;
                    dlg.sourceTB.Visible = false;
                    foreach (string item in source)
                    {
                        dlg.sourceCB.Items.Add(item);
                    }
                    if (dlg.sourceCB.Items.Count > 0)
                    {
                        dlg.sourceCB.SelectedIndex = 0;
                    }
                }
                else
                {
                    dlg.sourceCB.Enabled = false;
                    dlg.sourceCB.Visible = false;
                    dlg.sourceTB.Enabled = true;
                    dlg.sourceTB.Visible = true;
                    if (source[0]!=null)
                    {
                        dlg.sourceTB.Text = source[0];
                    }
                }
			}
			dlg.targetTB.Text = target;

			if (dlg.ShowDialog() != DialogResult.Cancel)
			{
				IList<P4.FileSpec> files = dlg.returnedFiles;
				if (files != null)
				{
					return files;
				}
			}
			return null;
		}

		private void previewBtn_Click(object sender, EventArgs e)
		{
			this.previewTB.Clear();
            string sourceSelection=string.Empty;
            if (sourceCB.Enabled)
            {
                sourceSelection= this.sourceCB.SelectedItem.ToString();
            }
            else if (sourceTB.Enabled)
            {
                sourceSelection = this.sourceTB.Text;
            }


			P4.Stream effectiveTarget = _scm.GetStream(_target,null,null);
            P4.Stream effectiveSource = _scm.GetStream(sourceSelection, null, null);

			if (effectiveTarget.Type == P4.StreamType.Virtual)
			{
				if (effectiveTarget.BaseParent != null)
				{
                    effectiveTarget = _scm.GetStream(effectiveTarget.BaseParent.Path, null, null);
				}
				else if (effectiveTarget.Parent != null)
				{
                    effectiveTarget = _scm.GetStream(effectiveTarget.Parent.Path, null, null);
				}
			}

			if (effectiveSource.Type == P4.StreamType.Virtual)
			{
				if (effectiveSource.BaseParent != null)
				{
                    effectiveSource = _scm.GetStream(effectiveSource.BaseParent.Path, null, null);
				}
				else if (effectiveSource.Parent != null)
				{
                    effectiveSource = _scm.GetStream(effectiveSource.Parent.Path, null, null);
				}
			}

			P4.Options opts = new P4.Options();
			IList<P4.FileSpec> files = new List<P4.FileSpec>();
			opts["-n"] = null;

			if ((effectiveSource.Parent!=null&&effectiveSource.Parent.Path == effectiveTarget.Id) || (effectiveSource.BaseParent != null && effectiveSource.BaseParent.Path == effectiveTarget.Id))
			{
				opts["-S"] = effectiveSource.Id;
			}
			else if ((effectiveTarget.Parent != null && effectiveTarget.Parent.Path == effectiveSource.Id) || (effectiveTarget.BaseParent!=null&&effectiveTarget.BaseParent.Path == effectiveSource.Id))
			{
				opts["-S"] = effectiveTarget.Id;
				opts["-r"] = null;
			}

			if (_action == "merge")
			{
				files = _scm.MergeFiles(opts);
			}
			else if (_action == "copy")
			{
				files = _scm.CopyFiles(opts);
			}

            P4.P4CommandResult results = _scm.Connection.Repository.Connection.LastResults;
			string preview = string.Empty;
            this.previewTB.Text = preview;

            if (_scm.Connection.Repository.Connection.LastResults.ErrorList != null)
			{
                foreach (P4.P4ClientError error in _scm.Connection.Repository.Connection.LastResults.ErrorList)
				{
                    this.previewTB.AppendText(error.ErrorMessage + "\r\n");
				}
			}

			if (files != null)
			{
				foreach (P4.FileSpec file in files)
				{
                    this.previewTB.AppendText(file.DepotPath.ToString() + "\r\n");
				}

                if (results.TaggedOutput!=null)
                {
                    int count = files.Count;
                    if (_action == "merge")
                        if (count > 1)
                        {
                            this.previewTB.AppendText(string.Format(Resources.IntegrateDlg_FilesToBeMerged, count));
                        }
                        else
                        {
                            this.previewTB.AppendText(string.Format(Resources.IntegrateDlg_FileToBeMerged, count));
                        }
                    else if (_action == "copy")
                        if (count > 1)
                        {
                            this.previewTB.AppendText(string.Format(Resources.IntegrateDlg_FilesToBeCopied, count));
                        }
                        else
                        {
                            this.previewTB.AppendText(string.Format(Resources.IntegrateDlg_FileToBeCopied, count));
                        }
                }
			}

			this.previewTB.Enabled = true;
			this.previewTB.Visible = true;
		}

		private void actionBtn_Click(object sender, EventArgs e)
		{
			previewTB.Clear();
            string sourceSelection = string.Empty;
            if (sourceCB.Enabled)
            {
                sourceSelection = this.sourceCB.SelectedItem.ToString();
            }
            else if (sourceTB.Enabled)
            {
                sourceSelection = this.sourceTB.Text;
            }

            P4.Stream effectiveTarget = _scm.GetStream(_target, null, null);
            P4.Stream effectiveSource = _scm.GetStream(sourceSelection, null, null);

            if(effectiveSource==null||effectiveTarget==null)
            {
                return;
            }

			if (effectiveTarget.Type == P4.StreamType.Virtual)
			{
				if (effectiveTarget.BaseParent != null)
				{
                    effectiveTarget = _scm.GetStream(effectiveTarget.BaseParent.Path, null, null);
				}
				else if (effectiveTarget.Parent != null)
				{
                    effectiveTarget = _scm.GetStream(effectiveTarget.Parent.Path, null, null);
				}
			}

			if (effectiveSource.Type == P4.StreamType.Virtual)
			{
				if (effectiveSource.BaseParent != null)
				{
                    effectiveSource = _scm.GetStream(effectiveSource.BaseParent.Path, null, null);
				}
				else if (effectiveSource.Parent != null)
				{
                    effectiveSource = _scm.GetStream(effectiveSource.Parent.Path, null, null);
				}
			}

			P4.Options opts = new P4.Options();
			IList<P4.FileSpec> files = new List<P4.FileSpec>();

			if ((effectiveSource.Parent != null && effectiveSource.Parent.Path == effectiveTarget.Id) || (effectiveSource.BaseParent!=null&&effectiveSource.BaseParent.Path == effectiveTarget.Id))
			{
				opts["-S"] = effectiveSource.Id;
			}
			else if ((effectiveTarget.Parent != null && effectiveTarget.Parent.Path == effectiveSource.Id) || (effectiveTarget.BaseParent!=null&&effectiveTarget.BaseParent.Path == effectiveSource.Id))
			{
				opts["-S"] = effectiveTarget.Id;
				opts["-r"] = null;
			}

			if (_action == "merge")
			{
				files = _scm.MergeFiles(opts);
			}
			else if (_action == "copy")
			{
				files = _scm.CopyFiles(opts);
			}

            P4.P4CommandResult results = _scm.Connection.Repository.Connection.LastResults;
			string preview = null;

            if (_scm.Connection.Repository.Connection.LastResults.ErrorList != null)
			{
				previewTB.Text = preview;
                foreach (P4.P4ClientError error in _scm.Connection.Repository.Connection.LastResults.ErrorList)
				{
					previewTB.AppendText(error.ErrorMessage);
				}
				DialogResult = DialogResult.None;
				return;
			}
			if (files != null)
            {
                // if there are files to merge or copy, offer
                // changelist selection and submit or save
                int count = files.Count();
                string target = targetTB.Text;
                string action = null;
                string summary= string.Empty;
                P4.Changelist getDesc = _scm.GetChangelist(-1);
                string newChangeDescription = getDesc.Description;
                IList<P4.Changelist> changes = _scm.GetAvailibleChangelists(-1);

                if (count > 1)
                {
                    if (_action == "merge")
                    {
                        summary = string.Format(Resources.P4VsProviderService_MergeSummary, count);
                        if (newChangeDescription == null ||
                        newChangeDescription == Resources.DefaultChangeListDescription)
                        {
                            newChangeDescription = string.Format(
                            Resources.P4VsProviderService_MergeChangeDescription, target);
                        }
                    }
                    else if (_action == "copy")
                    {
                        summary = string.Format(Resources.P4VsProviderService_CopySummary, count);
                        if (newChangeDescription == null ||
                        newChangeDescription == Resources.DefaultChangeListDescription)
                        {
                            newChangeDescription = string.Format(
                                Resources.P4VsProviderService_CopyChangeDescription, target);
                        }
                        if (submitCopiedRB.Checked)
                        {
                            string[] fileStringArray=new string[count];
                            for (int idx =0; idx<count;idx++)
                            {
                               fileStringArray[idx]=files[idx].LocalPath.Path;
                            }
                            _scm.SubmitFiles(newChangeDescription,null,false,fileStringArray);
                            return;
                        }
                    }
                }
                else
                {
                    if (_action == "merge")
                    {
                        summary = Resources.P4VsProviderService_Merge1Summary;
                        if (newChangeDescription == null ||
                       newChangeDescription == Resources.DefaultChangeListDescription)
                        {
                            newChangeDescription = string.Format(
    Resources.P4VsProviderService_Merge1ChangeDescription, target);
                        }
                    }
                    else if (_action == "copy")
                    {
                        summary = Resources.P4VsProviderService_Copy1Summary;
                        if (newChangeDescription == null ||
                      newChangeDescription == Resources.DefaultChangeListDescription)
                        {
                            newChangeDescription = string.Format(
    Resources.P4VsProviderService_Copy1ChangeDescription, target);
                        }
                        if (submitCopiedRB.Checked)
                        {
                            _scm.SubmitFiles(newChangeDescription, null, false, files[0].LocalPath.Path);
                        return;
                        }
                    }
                }

                int changeListId = -1;
                P4.Changelist changelist = SelectChangelistDlg.ShowChooseChangelistSubmit(
                    summary,
                    changes, ref newChangeDescription, out action, true, true, _scm);
                changeListId = changelist.Id;

                if (changeListId == -2)
                {
                    // user hit 'No'
                    return ;
                }
                opts = new P4.Options();

                if (changeListId > 0)
                {
                    opts["-c"] = changeListId.ToString();
                }

                P4.Changelist changeToSubmit = _scm.Connection.Repository.NewChangelist();
                if (changeListId == -1)
                {
                    // Overwrite new changelist files. If default has files in it, a new 
                    // changelist will automatically get those files.
                    changeToSubmit.Files = files as IList<P4.FileMetaData>;
                    changeToSubmit.Description = changelist.Description;
                    changeToSubmit = _scm.SaveChangelist(changeToSubmit, null);
                    opts["-c"] = changeToSubmit.Id.ToString();
                    changeListId = changeToSubmit.Id;
                }

                if (opts.ContainsKey("-c"))
                {
                    Int32 c = Convert.ToInt32(opts["-c"]);
                    changeToSubmit = _scm.GetChangelist(c);
                    _scm.MoveFilesToChangeList(c, changelist.Description, files);
                    changeListId = changeToSubmit.Id;
                }

                // done, unless submit was hit
                if (action == "submit")
                {
                    changeToSubmit.Description = changelist.Description;

                    if (changeListId > 0)
                    {
                        SubmitDlg.SubmitPendingChanglist(changeToSubmit, changeToSubmit.Files, _scm);
                    }
                    else
                    {
                        IList<string> list = new List<string>();
                        foreach (P4.FileSpec fs in files)
                        {
                            list.Add(fs.LocalPath.Path.ToString());
                        }
                        SubmitDlg.SubmitFiles(list, _scm, false, changeToSubmit.Description);
                    }
                }
            }
            returnedFiles = files;
            DialogResult = DialogResult.OK;
			return;
		}

		private void sourceCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			previewTB.Clear();
		}

        private void addToPendingRB_CheckedChanged(object sender, EventArgs e)
        {
            if (addToPendingRB.Checked)
            {
                Preferences.LocalSettings.Set("no_autosubmit_copy", true);
            }
            else
            {
                Preferences.LocalSettings.Set("no_autosubmit_copy", false);
            }
        }
	}
}
