using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perforce.P4VS
{
    public abstract class Checkout : SelectedNodes
    {
        public void CheckoutResource(IList<string> sel)
        {
            IList<VSITEMSELECTION> nodes = SccService.SelectedNodes;

            IList<string> lockedFiles = new List<string>();
            IList<string> lockUsers = new List<string>();
            IList<string> staleFiles = new List<string>();
            IList<string> checkoutFiles = new List<string>();

            for (int idx = 0; idx < sel.Count; idx++)
            {
                string file = sel[idx];

                bool isWildcard = file.EndsWith("...") || file.EndsWith("*");

                if (isWildcard == false)
                {
                    P4.FileMetaData fmd = null;

                    if (string.IsNullOrEmpty(file) == false)
                    {
                        fmd = SccService.ScmProvider.Fetch(file);
                    }

                    if (fmd!=null && 
                        fmd.HeadType.Modifiers.HasFlag(P4.FileTypeModifier.ExclusiveOpen) &&
                        fmd.OtherOpen>0)
                    {
                        // Show the error for exclusive open file and continue
                        // without adding file to list of files to checkout
                        P4ErrorDlg.Show(Resources.P4VsProvider_ExclusiveOpenError, false, false);
                        continue;
                    }
                    if ((fmd == null) || (fmd.HeadAction == P4.FileAction.Delete))
                    {
                        sel.RemoveAt(idx--);
                        continue;
                    }
                    if (fmd.OtherLock)
                    {
                        lockedFiles.Add(fmd.LocalPath.Path);
                        string otherlocks = string.Empty;
                        foreach (string otherLock in fmd.OtherLockUserClients)
                        {
                            if (string.IsNullOrEmpty(otherlocks) == false)
                            {
                                otherlocks += ",";
                            }
                            otherlocks += otherLock;
                        }
                        lockUsers.Add(otherlocks);
                    }
                    if (fmd.IsStale)
                    {
                        staleFiles.Add(fmd.LocalPath.Path);
                    }
                }
                else
                {
                    IList<P4.FileMetaData> fmdList = SccService.ScmProvider.GetFileMetaData(null, file);

                    foreach (P4.FileMetaData fmd in fmdList)
                    {
                        if (fmd.OtherLock)
                        {
                            lockedFiles.Add(fmd.LocalPath.Path);
                            string otherlocks = string.Empty;
                            foreach (string otherLock in fmd.OtherLockUserClients)
                            {
                                if (string.IsNullOrEmpty(otherlocks) == false)
                                {
                                    otherlocks += ",";
                                }
                                otherlocks += otherLock;
                            }
                            lockUsers.Add(otherlocks);
                        }
                        if (fmd.IsStale)
                        {
                            staleFiles.Add(fmd.LocalPath.Path);
                        }
                    }
                }
                checkoutFiles.Add(file);
            }
            if ((lockedFiles != null) && (lockedFiles.Count > 0))
            {
                string lockWarning = Resources.WarningStyle_Locked_Prompt;
                if (FileListWarningDlg.Show(lockedFiles, lockUsers, FileListWarningDlg.WarningStyle.Locked) == DialogResult.Cancel)
                {
                    return;
                }
            }

            if ((staleFiles != null) && (staleFiles.Count > 0))
            {
                DialogResult answer = FileListWarningDlg.Show(staleFiles, null, FileListWarningDlg.WarningStyle.GetLatest);
                if (answer == DialogResult.Cancel)
                {
                    return;
                }
                if (answer == DialogResult.OK)
                {
                    if (!SccService.SyncFiles(null, staleFiles))
                    {
                        return;
                    }
                }
            }

            if(checkoutFiles.Count<1)
            {
                return;
            }

            P4.Changelist getDesc = SccService.ScmProvider.GetChangelist(-1);
            if (getDesc == null)
            {
                return;
            }

            string newChangeDescription = getDesc.Description;
            if (newChangeDescription == Resources.DefaultChangeListDescription)
            {
                newChangeDescription = Resources.P4VsProvider_CheckoutFilesDefaultChangelistDescription;
            }
            IList<P4.Changelist> changes = SccService.ScmProvider.GetAvailibleChangelists(-1);

            int changeListId = -1;
            string prompt = Resources.P4VsProvider_CheckoutFilePrompt;
            if (sel.Count >= 2)
            {
                prompt = Resources.P4VsProvider_CheckoutFilesPrompt;
            }
            changeListId = SelectChangelistDlg2.ShowChooseChangelistYesNo(prompt, checkoutFiles, changes, ref newChangeDescription);

            if (changeListId < -1)
            {
                // user hit cancel
                return;
            }
            if (string.IsNullOrEmpty(newChangeDescription))
            {
                newChangeDescription = Resources.P4VsProvider_CheckoutFilesDefaultChangelistDescription;
            }
            SelectChangelistDlg.CurrentChangeList = SccService.CheckoutFiles(sel, changeListId, newChangeDescription);

            // now refresh the selected nodes' glyphs
            Glyphs.RefreshNodesGlyphs(nodes, sel);
        }

    }
}
