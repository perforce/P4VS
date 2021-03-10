/*******************************************************************************

Copyright (c) 2011 Perforce Software, Inc.  All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1.  Redistributions of source code must retain the above copyright
	notice, this list of conditions and the following disclaimer.

2.  Redistributions in binary form must reproduce the above copyright
	notice, this list of conditions and the following disclaimer in the
	documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL PERFORCE SOFTWARE, INC. BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*******************************************************************************/

/*******************************************************************************
 * Name		: P4VsProviderService.cs
 *
 * Author	: Duncan Barbee <dbarbee@perforce.com>
 *
 * Description	: Implementation of P4VS Source Control Provider Service
 *
 ******************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using NLog;

using System.Windows.Forms;

using ScmFile = Perforce.P4.FileMetaData;
using FileMap = System.Collections.Generic.Dictionary<string, Perforce.P4.FileMetaData>;
using Perforce.P4Scm;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Perforce.P4VS
{
	public partial class P4VsProviderService :
IVsSccChanges // Implementing IVsSccChanges will light up the Pending Changes compartment on the Status Bar
    {
        /// <summary>
        /// Count of the outstanding number of pending changes in the current repository
        /// </summary>
        /// <remarks>
        /// This can be defined by an Source Control provider as to whether it is the number of files, lines, etc. as long as
        /// the information is explained in detail in PendingChangeDetail
        /// </remarks>
        public int PendingChangeCount
        {
            get
            {
                // pending changes count will always be at least
                // 1 with the default pending changelist
                _pendingChangeCount =1;

                IList <P4.Changelist> changes = _scmProvider.GetChangelists(P4.ChangesCmdFlags.None,
                    _scmProvider.Connection.Workspace, 0, P4.ChangeListStatus.Pending,
                    _scmProvider.Connection.User, null);
                if (changes!=null)
                {
                    _pendingChangeCount += changes.Count;
                }

                // don't bother with opened at this point,
                // add the default to the count, whether or
                // not it has any files opened.

                //P4.Options opts = new P4.Options();
                //opts["-c"] = "default";
                //IList<P4.File> opened = _scmProvider.GetOpenedFiles(null, opts);
                //if (opened!=null && opened.Count>0)
                //{
                    //_pendingChangeCount++;
                //}

                if (_pendingChangeCount==1)
                {
                    _pendingChangeLabel = " Pending Change";
                }
                else
                {
                    _pendingChangeLabel = " Pending Changes";
                }
                return _pendingChangeCount;
            }
            //get { return _pendingChangeCount; }
            set
            {
                if (_pendingChangeCount != value)
                {
                    _pendingChangeCount = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PendingChangeCount)));
                }
            }
        }

        private int _pendingChangeCount;

        /// <summary>
        /// Detailed information about the number of outstanding changes in the current repository
        /// </summary>
        public string PendingChangeDetail
        {
            get { return _pendingChangeDetail; }
            set
            {
                if (_pendingChangeDetail != value)
                {
                    _pendingChangeDetail = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PendingChangeDetail)));
                }
            }
        }

        private string _pendingChangeDetail = "View Pending Changelists";

        /// <summary>
        /// A label that will be temporarily displayed to indicate busy status
        /// </summary>
        public string PendingChangeLabel
        {
            get { return _pendingChangeLabel; }
            set
            {
                if (_pendingChangeLabel != value)
                {
                    _pendingChangeLabel = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PendingChangeLabel)));
                }
            }
        }

        private string _pendingChangeLabel = " Pending Changes";

        /// <summary>
        /// Handler called when the pending changes UI is clicked.
        /// A Source Control provider is expected to begin a workflow that will enable the user to 
        /// commit pending changes.
        /// </summary>
        async System.Threading.Tasks.Task IVsSccChanges.PendingChangesUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            _P4VsProvider.P4VsViewPendingChangelistsToolWindow(null, null);
            IVsUIShell uiShell = (IVsUIShell)ScmProvider.SccService;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
