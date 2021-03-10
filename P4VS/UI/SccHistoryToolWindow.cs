
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

using Perforce;
using Microsoft.VisualStudio.Utilities;

namespace Perforce.P4VS
{
	/// <summary>
	/// Summary description for SccProviderToolWindow.
	/// </summary>
	[Guid("34386F7F-4CB3-4B5C-BC91-EBEED97A709B")]
	public class SccHistoryToolWindow : Microsoft.VisualStudio.Shell.ToolWindowPane
    {
		public SccHistoryToolWindowControl control;

		public SccHistoryToolWindow() :base(null)
		{
			// set the window title
			this.Caption = Resources.SccHistoryToolWindow_Caption;

			// set the icon for the frame
#if VS2008
			this.BitmapResourceID = 507;// CommandId.ibmpHistoryToolWindowsImage;  // bitmap strip resource ID
            this.BitmapIndex = 0;   // index in the bitmap strip
#else
            this.BitmapResourceID = 508;// CommandId.ibmpHistoryToolWindowsImage;  // bitmap strip resource ID
            this.BitmapIndex = 0;   // index in the bitmap strip
#endif

            using (DpiAwareness.EnterDpiScope(DpiAwarenessContext.SystemAware))
            {
                control = new SccHistoryToolWindowControl();
            }
        }

		override public IWin32Window Window
		{
			get
			{
				return (IWin32Window)control;
			}
		}

		public bool isVisible
		{
			get 
			{
				IVsWindowFrame frame = Frame as IVsWindowFrame;
				if (frame != null)
				{
					int pos = 0;

					return (frame.IsOnScreen(out pos) == Microsoft.VisualStudio.VSConstants.S_OK) && (pos != 0);
				}
				return false; 
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
		public void ToolWindowToolbarCommand()
		{
			if (this.control.BackColor == Color.Coral)
				this.control.BackColor = Color.White;
			else
				this.control.BackColor = Color.Coral;
		}
	}
}
