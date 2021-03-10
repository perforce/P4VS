using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NLog;

namespace Perforce.P4VS
{
	[Guid("63FC1BC7-2238-498B-A04C-253B1346DC22")]
	class P4VsOutputWindow
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Guid guid = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(typeof(P4VsOutputWindow), typeof(GuidAttribute))).Value);

		public static Guid Guid { get { return guid; } }

		private static IVsOutputWindowPane pane = null;

		public static void Init()
		{
			var outputWindow = P4VsProvider.Instance.GetService(
						typeof(SVsOutputWindow)) as IVsOutputWindow;

			Guid guid = P4VsOutputWindow.Guid;

			if (outputWindow != null)
			{
				int ret = outputWindow.GetPane(ref guid, out pane);

				if (ret == VSConstants.S_OK)
				{
					// the pane has already been created, so return
					return;
				}

				ret = outputWindow.CreatePane(ref guid, Resources.P4VsOutputWindow_PanelName, 1, 0);
                if (ret != VSConstants.S_OK)
                {
                    var msg = "Could not create an output pane";
                    logger.Error(msg);
                    throw new ApplicationException(msg);
                }

				ret = outputWindow.GetPane(ref guid, out pane);
				if (ret != VSConstants.S_OK)
                {
                    var msg = "Could not retrieve the output pane";
                    logger.Error(msg);
                    throw new ApplicationException(msg);
                }
			}
		}

		public delegate void AppendMessageDelegate(string msg);

		public static void AppendMessage(String format, params object[] args)
		{
			AppendMessage(string.Format(format, args));
		}

		public static void AppendMessage(String msg)
		{
            logger.Info(msg);
#if VS2008
			AppendMessage_Int(msg);
#else
			if ((P4VsProvider.Instance != null) && (P4VsProvider.Instance.SccService != null) && 
                (P4VsProvider.Instance.SccService.UiDispatcher != null))		
			{
				lock (P4VsProvider.Instance.SccService.UiDispatcher)
				{
					P4VsProvider.Instance.SccService.UiDispatcher.BeginInvoke(new AppendMessageDelegate(AppendMessage_Int), msg);
				}
			}
#endif
		}

		public static void AppendMessage_Int(String msg)
		{
			if (pane == null)
				Init();

			if (pane == null)
				return;

			try
			{
				string msg2 = msg;
				if (msg.Length > 1024)
				{
					msg2 = msg.Substring(0, 1021) + "...";
				}
				pane.OutputStringThreadSafe(msg);
				pane.OutputStringThreadSafe("\n");
			}
			catch { }
		}
	}
}
