using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NLog;

namespace Perforce.P4VS
{
	class FileLogger
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static string LogFilePath { get; set; }
		static string RolloverFilePath { get; set; }
		static int LogFileMaxSize { get; set; }

		static bool _intialized = false;

		public enum FileLogLevel
		{ 
			Fatal = 0,
			Error = 1, 
			Warning = 2, 
			Info = 3,
			Debug = 4
		}

        internal static void Init()
		{
			string logFileDirectory;
			LogFilePath = Preferences.LocalSettings.GetString("Log_path", null);

			if (LogFilePath != null)
			{
				logFileDirectory = Path.GetDirectoryName(LogFilePath);
				if (Directory.Exists(logFileDirectory) == false)
				{
					LogFilePath = null;
				}
			}
			if (LogFilePath == null)
			{
				string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
				appPath = Path.GetDirectoryName(appPath);

				LogFilePath = Path.Combine(appPath, "P4VS_Log.txt");
			}
            logger.Debug("Logging to: {0}", LogFilePath);
			LogFileMaxSize = Preferences.LocalSettings.GetInt("Log_size", 50);

			string ext = Path.GetExtension(LogFilePath);
			string baseName = Path.GetFileNameWithoutExtension(LogFilePath);
			logFileDirectory = Path.GetDirectoryName(LogFilePath);

			RolloverFilePath = string.Format("{0}_prev{1}", baseName, ext);
			RolloverFilePath = Path.Combine(logFileDirectory, RolloverFilePath);

			_intialized = true;
		}

		public static void LogMessage(int msg_level,
										String source,
										String message)
		{
			if (Preferences.LocalSettings.GetBool("Log_save", false))
			{
				// Log level determines which massages actually go in the log to keep from 
				// cluttering it with lots of extra debugging data, unless it's needed
				// by default, level 3 logs info level messages and higher priority 
				// (0 being the highest priority)

				int LogLevel = Preferences.LocalSettings.GetInt("DebugLogLevel", 4);
				if (msg_level <= LogLevel)
				{
					if (_intialized == false)
					{
						Init();
					}
					string LogLevelStr = null;
					switch (msg_level)
					{
						case 0:
							LogLevelStr = "Fatal";
                            logger.Error("Fatal: {0} {1}", source, message);
							break;
						case 1:
							LogLevelStr = "Error";
                            logger.Error("{0} {1}", source, message);
							break;
						case 2:
							LogLevelStr = "Warning";
                            logger.Warn("{0} {1}", source, message);
							break;
						case 3:
							LogLevelStr = "Info";
                            logger.Info("{0} {1}", source, message);
							break;
						default:
							LogLevelStr = string.Format("Debug-{0}", msg_level);
                            logger.Debug("{0} {1}", source, message);
							break;
					}
					DateTime now = DateTime.Now;
					String msg = String.Format("[{0}: {1}] {2} : {3}\r\n",
						LogLevelStr, source, now.ToString("dd/MM/yyyy HH:mm:ss.ffff"), message);

					try
					{
						if (File.Exists(LogFilePath))
						{
							FileInfo fi = new FileInfo(LogFilePath);

							if ((fi.Length > 1024) && ((fi.Length / 1024) > (LogFileMaxSize / 2)))
							{
								// File has grown to half the allotted size, delete the existing rollover log
								// (if any), move the current log to the rollover, and start a new log

								if (File.Exists(RolloverFilePath))
								{
									File.Delete(RolloverFilePath);
								}
								File.Move(LogFilePath, RolloverFilePath);
							}
						}
						using (StreamWriter sr = new StreamWriter(LogFilePath, true))
						{
							sr.Write(msg);
						}
					}
					catch { } // never fail because of an error writing a log message

					// TODO: Implement an internal logging function
				}
			}
		}

		public static void LogException(String source, Exception ex)
		{
			if (Preferences.LocalSettings.GetBool("Log_save", false))
			{
				try
				{
                    logger.Error("LogException {0}", ex.Message);
                    logger.Error(ex.StackTrace);
					String msg = String.Format("{0}:{1}\r\n{2}",
						ex.GetType().ToString(),
						ex.Message,
						ex.StackTrace);
					LogMessage(1, source, msg);

					if (ex.InnerException != null)
						LogException("Inner Exception", ex.InnerException);
				}
				catch { } // never fail because of an error writing a log message
			}
		}
	}
}
