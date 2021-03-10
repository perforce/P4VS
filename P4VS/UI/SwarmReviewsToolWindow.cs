
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

namespace Perforce.P4VS
{
    /// <summary>
    /// Summary description for P4ToolWindow.
    /// </summary>
	[Guid("5ECDCD36-6F6D-4A25-9E71-2F1D90361F13")]
	public class SwarmReviewsToolWindow : ToolWindowPane
    {
        private SwarmReviewsToolWindowControl control;

        public SwarmReviewsToolWindow() :base(null)
        {
            // set the window title
            this.Caption = Resources.SwarmReviewsToolWindowControl_Caption;

            // set the icon for the frame
            this.BitmapResourceID = CommandId.ibmpToolWindowsImages;  // bitmap strip resource ID
            this.BitmapIndex = CommandId.iconHistory;   // index in the bitmap strip

            control = new SwarmReviewsToolWindowControl();

			control.Dock = DockStyle.Fill;
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
    }
}
