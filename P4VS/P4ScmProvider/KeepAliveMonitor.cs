using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using NLog;
using Perforce.P4VS;

namespace Perforce.P4Scm
{
	class KeepAliveMonitor :P4.IKeepAlive
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private enum CommandState { Running, Completed, Canceled };
		private static Dictionary<uint, CommandState> CmdMap;
		private static Dictionary<uint, P4.P4Server> SvrMap;

		public static bool IsCommandRunning
		{
			get { return true;} // keep the cancel button enabled
			// return ((CmdMap != null) && (CmdMap.Count > 0)); }
		}

		public static void CancelAll()
		{
			lock (CmdMap)
			{
				uint[] keys = CmdMap.Keys.ToArray();
				for (int idx = keys.Length - 1; idx >= 0; idx--)
				{
					CmdMap[keys[idx]] = CommandState.Canceled;
					SvrMap[keys[idx]].CancelCommand(keys[idx]);
				}
			}
		}

		public KeepAliveMonitor()
		{
			if (CmdMap == null)
			{
				CmdMap = new Dictionary<uint, CommandState>();
			}
			if (SvrMap == null)
			{
				SvrMap = new Dictionary<uint, P4.P4Server>();
			}
		}

		#region IKeepAlive Members

		public void CommandCompleted(uint cmdId)
		{
			// OK if the cmd is not in the map, the API might send this multiple times,
			// to ensure the he UI gets dismissed.
			if (CmdMap.ContainsKey(cmdId) == false)
			{
				return;
			}

			CmdMap[cmdId] = CommandState.Completed;
		}

		private delegate void ShowDialogDelegate();

		public bool StartQueryCancel(P4.P4Server server, uint cmdId, Thread runCmdThread, string cmdLine)
		{
			EnvDTE.DTE dte = P4VsProvider.GetDTE();

			try
			{
				const int _maxChars = 77;
				//int progressCnt = 0;
				string progressMsg = string.Format("P4 {0}", cmdLine);
				if (progressMsg.Length > _maxChars)
				{
					progressMsg = string.Format("{0}...", progressMsg.Substring(0, _maxChars));
				}
				if (dte != null)
				{
					dte.StatusBar.Text = progressMsg;
					//dte.StatusBar.Progress(true, progressMsg, progressCnt, 10);
					dte.StatusBar.Animate(true, EnvDTE.vsStatusAnimation.vsStatusAnimationSync);
				}

				CmdMap[cmdId] = CommandState.Running;
				SvrMap[cmdId] = server;

				DateTime start = DateTime.Now;

				// if on the UI thread only wait a millisecond to keep the UI alive
				while (runCmdThread.IsAlive && (CmdMap[cmdId] == CommandState.Running))
				{
					Application.DoEvents();
				}
				return (CmdMap[cmdId] == CommandState.Canceled);
			}
			catch (Exception ex)
			{
				logger.Trace(ex.Message);
			}
			finally
			{
				CmdMap.Remove(cmdId);
				SvrMap.Remove(cmdId);
				if (dte != null)
				{
					dte.StatusBar.Clear();
					//dte.StatusBar.Progress(false);
					dte.StatusBar.Animate(false, EnvDTE.vsStatusAnimation.vsStatusAnimationSync);
				}
			}
			return false;
		}

		#endregion
	}
}
