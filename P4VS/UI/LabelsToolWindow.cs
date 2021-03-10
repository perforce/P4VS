
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.Utilities;

namespace Perforce.P4VS
{
    /// <summary>
    /// Summary description for P4ToolWindow.
    /// </summary>
    [Guid("6A212058-4CD3-4F79-B223-B09A2776B092")]
    public class LabelsToolWindow : ToolWindowPane
    {
		private LabelsToolWindowControl control;

		public LabelsToolWindow()
			: base(null)
        {
            // set the window title
			this.Caption = Resources.LabelsToolWindowControl_Caption;

			//// set the CommandID for the window ToolBar
			//this.ToolBar = new CommandID(GuidList.guidP4VsProviderCmdSet, CommandId.imnuToolWindowToolbarMenu);

            // set the icon for the frame
            this.BitmapResourceID = CommandId.ibmpToolWindowsImages;  // bitmap strip resource ID
			this.BitmapIndex = CommandId.iconlabels;   // index in the bitmap strip

            using (DpiAwareness.EnterDpiScope(DpiAwarenessContext.SystemAware))
            {
                control = new LabelsToolWindowControl();
                control.Dock = DockStyle.Fill;
            }
        }

        override public IWin32Window Window
        {
            get
            {
                return (IWin32Window)control;
            }
        }

        /// <include file='doc\WindowPane.uex' path='docs/doc[@for="WindowPane.Dispose1"]' />
        /// <devdoc>
        ///     Called when this tool window pane is being disposed.
        /// </devdoc>
        override protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (control != null)
                {
                    try
                    {
                        if (control is IDisposable)
                            control.Dispose();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Fail(String.Format("Failed to dispose {0} controls.\n{1}", this.GetType().FullName, e.Message));
                    }
                    control = null;
                } 
                
                IVsWindowFrame windowFrame = (IVsWindowFrame)this.Frame;
                if (windowFrame != null)
                {
                    // Note: don't check for the return code here.
                    windowFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_SaveIfDirty);
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// This function is only used to "do something noticeable" when the toolbar button is clicked.
        /// It is called from the package.
        /// A typical tool window may not need this function.
        /// 
        /// The current behavior change the background color of the control
        /// </summary>
        public void LabelsToolWindowToolbarCommand()
        {
            if (this.control.BackColor == Color.Coral)
                this.control.BackColor = Color.White;
            else
                this.control.BackColor = Color.Coral;
        }

		protected override bool PreProcessMessage(ref Message m)
		{
			return base.PreProcessMessage(ref m);
		}
    }
}
