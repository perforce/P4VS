
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using NLog;
using System.Globalization;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using MsVsShell = Microsoft.VisualStudio.Shell;

namespace Perforce.P4VS
{
	/// <summary>
	/// Summary description for P4ConnectionPreferences.
	/// </summary>
	/// 
	[Guid("1B4E7694-738F-4FB2-8625-17317B04B706")]
	public class P4ConnectionPreferences : MsVsShell.DialogPage
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
		private P4ConnectionPreferencesControl page = null;

		/// <include file='doc\DialogPage.uex' path='docs/doc[@for="DialogPage".Window]' />
		/// <devdoc>
		///     The window this dialog page will use for its UI.
		///     This window handle must be constant, so if you are
		///     returning a Windows Forms control you must make sure
		///     it does not recreate its handle.  If the window object
		///     implements IComponent it will be sited by the 
		///     dialog page so it can get access to global services.
		/// </devdoc>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected override IWin32Window Window
		{
			get
			{
				page = new P4ConnectionPreferencesControl();
				page.Location = new Point(0, 0);
				page.Dock = DockStyle.Fill;
				page.OptionsPage = this;
				return page;
			}
		}

		/// <include file='doc\DialogPage.uex' path='docs/doc[@for="DialogPage.OnActivate"]' />
		/// <devdoc>
		///     This method is called when VS wants to activate this
		///     page.  If the Cancel property of the event is set to true, the page is not activated.
		/// </devdoc>
		protected override void OnActivate(CancelEventArgs e)
		{
		    logger.Trace("In OnActivate");
			 base.OnActivate(e);
		}

		/// <include file='doc\DialogPage.uex' path='docs/doc[@for="DialogPage.OnClosed"]' />
		/// <devdoc>
		///     This event is raised when the page is closed.   
		/// </devdoc>
		protected override void OnClosed(EventArgs e)
		{
			logger.Trace("In OnClosed");
			base.OnClosed(e);
		}

		/// <include file='doc\DialogPage.uex' path='docs/doc[@for="DialogPage.OnDeactivate"]' />
		/// <devdoc>
		///     This method is called when VS wants to deactivate this
		///     page.  If true is set for the Cancel property of the event, 
		///     the page is not deactivated.
		/// </devdoc>
		protected override void OnDeactivate(CancelEventArgs e)
		{
			logger.Trace("In OnDeactivate");
			base.OnDeactivate(e);
		}

		/// <include file='doc\DialogPage.uex' path='docs/doc[@for="DialogPage.OnApply"]' />
		/// <devdoc>
		///     This method is called when VS wants to save the user's 
		///     changes then the dialog is dismissed.
		/// </devdoc>
		protected override void OnApply(PageApplyEventArgs e)
		{
			page.OnApply();

			//logger.Trace("In OnApply");
			//string messageText = Resources.ResourceManager.GetString("ApplyProviderOptions");
			//string messageCaption = Resources.ResourceManager.GetString("ProviderName");

			//IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
			//Guid clsid = Guid.Empty;
			//int result = VSConstants.S_OK;
			//if (uiShell.ShowMessageBox(0, ref clsid,
			//                    messageCaption,
			//                    messageText,
			//                    string.Empty,
			//                    0,
			//                    OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL,
			//                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
			//                    OLEMSGICON.OLEMSGICON_QUERY,
			//                    0,        // false = application modal; true would make it system modal
			//                    out result) != VSConstants.S_OK
			//    || result != (int)DialogResult.OK)
			//{
			//    logger.Trace("Cancelled the OnApply event");
			//    e.ApplyBehavior = ApplyKind.Cancel;
			//}
			//else
			//{
			//    base.OnApply(e);
			//}
		}
	}
}
