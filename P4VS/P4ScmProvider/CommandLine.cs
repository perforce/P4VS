using NLog;
using Perforce.P4VS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perforce.P4Scm
{
    class CommandLine
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static bool logOutput = true;
        private static bool commandLogged = true;
        private static string lastCommand = null;

        static string[] ErrorSeverity = new string[] 
		{
			Resources.P4ScmProvider_ErrorSeverityEmpty,
			Resources.P4ScmProvider_ErrorSeverityInfo,
			Resources.P4ScmProvider_ErrorSeverityWarning,
			Resources.P4ScmProvider_ErrorSeverityFailed,
			Resources.P4ScmProvider_ErrorSeverityFatal
		};

        static int[] MessageSeverity = new int[] 
		{
			4, //"E_EMPTY",
			3, //"E_INFO",
			2, //"E_WARN",
			1, //"E_FAILED",
			0  //"E_FATAL",
		};

        public static void ErrorCallbackFn(uint cmdId, int severity, int errorNumber, string data)
        {
            try
            {
                if ((commandLogged == false) && (lastCommand != null))
                {
                    FileLogger.LogMessage(4, "P4API.NET", lastCommand);
                }

                string severityStr = string.Empty;
                if ((severity < 0) || (severity >= ErrorSeverity.Length))
                {
                    severityStr = string.Format("E_{0}", severity);
                }
                else
                {
                    severityStr = ErrorSeverity[severity];
                }

                string msg = string.Format("{0}: {1}", severityStr, data);

                if (logOutput)
                {
                }
                if ((severity < 0) || (severity >= ErrorSeverity.Length))
                {
                    FileLogger.LogMessage(0, "P4API.NET", msg);
                }
                else
                {
                    FileLogger.LogMessage(MessageSeverity[severity], "P4API.NET", msg);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error trying to log message: [{0}]: {1}", severity, data);
                P4VsOutputWindow.AppendMessage(msg);
                FileLogger.LogMessage(0, "P4API.NET", msg);
                P4VsOutputWindow.AppendMessage(ex.Message);
                FileLogger.LogException("CommandLine_InfoResultsCallbackFn", ex);
            }
        }

        public static void InfoResultsCallbackFn(uint cmdId, int msgId, int level, string data)
        {
            if (logOutput)
            {
                // level is generally from 0-9, though for some reason trying to add an 
                // ignored file sends a message with a level of 34, so ignor level of 34
                if (level == 34)
                {
                    level = 0;
                }
                try
                {
                    string msg = "--->";
                    for (int idx = 0; idx < level; idx++)
                    {
                        msg += ". ";
                    }
                    msg += (data != null) ? data : string.Empty;
                    P4VsOutputWindow.AppendMessage(msg);
                    FileLogger.LogMessage(3, "P4API.NET", msg);
                }
                catch (Exception ex)
                {
                    P4VsOutputWindow.AppendMessage(ex.Message);
                    FileLogger.LogException("CommandLine_InfoResultsCallbackFn", ex);
                }
            }
        }

        public static void TaggedOutputCallbackFn(uint cmdId, int ObjId, P4.TaggedObject Obj)
        {
            if (logOutput)
            {
                try
                {
                    if (Obj == null)
                    {
                        return;
                    }
                    string msg = "--->Tagged Data: { ";
                    foreach (string key in Obj.Keys)
                    {
                        msg += string.Format("{{{0}:{1}}} ", key, Obj[key]);
                    }
                    msg += "}";
                    P4VsOutputWindow.AppendMessage(msg);
                    FileLogger.LogMessage(3, "P4API.NET", msg);
                }
                catch (Exception ex)
                {
                    P4VsOutputWindow.AppendMessage(ex.Message);
                    FileLogger.LogException("CommandLine_TaggedOutputCallbackFn", ex);
                }
            }
        }

        public static void TextResultsCallbackFn(uint cmdId, string data)
        {
            if (logOutput)
            {
                string msg = string.Format("--->{0}", data);
                P4VsOutputWindow.AppendMessage(msg);
                FileLogger.LogMessage(3, "P4API.NET", msg);
            }
        }

        public static void CommandEchoCallbackFn(string data)
        {
            if (data.StartsWith("DataSet set to:"))
            {
                // echoing the commands data set, record this only to the log file.
                FileLogger.LogMessage(3, "P4API.NET", data);
                return;
            }
            lastCommand = data;

            string[] cmds = data.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            string cmd = cmds[0];

            bool log = false;
            switch (cmd)
            {
                case "edit":
                case "add":
                case "delete":
                case "move":
                case "copy":
                case "merge":
                case "integ":
                case "sync":
                case "revert":
                case "submit":
                case "fix":
                case "shelve":
                case "resolve":
                case "unshelve":
                case "Lock":
                case "Unlock":
                case "Attribute":
                case "Counter":
                case "Labelsync":
                case "List":
                case "Populate":
                case "Reconcile":
                case "Status":
                case "Tag":
                case "Reopen":
                    log = true;
                    break;

                case "depot":
                case "client":
                case "workspace":
                case "user":
                case "group":
                case "changelist":
                case "job":
                case "branch":
                case "label":
                case "stream":
                case "protect":
                case "triggers":
                    if ((data.IndexOf(" -i ") > 0) || (data.EndsWith(" -i")) ||
                        (data.IndexOf(" -d ") > 0) || (data.EndsWith(" -d")))
                    {
                        log = true;
                    }
                    break;
                case "fstat":
                    logger.Trace(data);
                    break;

                default:
                    break;
            }
            if ((log) || (Preferences.LocalSettings.GetBool("Log_reporting", false) == true))
            {
                logOutput = log && Preferences.LocalSettings.GetBool("Log_command", false);
                string msg = string.Format("->{0}", data);
                logger.Trace("{0}: {1}", DateTime.Now.ToString("hh:mm:ss.fff"), msg);
                P4VsOutputWindow.AppendMessage(msg);
                FileLogger.LogMessage(3, "P4API.NET", msg);
                commandLogged = true;
                return;
            }
            commandLogged = false;
            logOutput = false;
        }
    }
}
