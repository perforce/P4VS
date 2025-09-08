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
 * Name		: P4ScmProvider.cs
 *
 * Author	: Duncan Barbee <dbarbee@perforce.com>
 *
 * Description	: Provides Perforce SCM interface
 *
 ******************************************************************************/

#define USE_P4IGNORE

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Perforce;
using NLog;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Build.Evaluation;

// alias for P4.FileMetaData
using Perforce.P4;
using File = System.IO.File;
using scmFile = Perforce.P4.FileMetaData;
using Perforce.P4VS;
using Microsoft.Win32;

namespace Perforce.P4Scm
{
	public class P4ScmProvider : IDisposable
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ConnectionHelper Connection = new ConnectionHelper();

        private P4VsProviderService _sccService = null;
        public P4VsProviderService SccService { get { return _sccService; } }

        private bool Offline {
            get
            {
                return ((Connection == null) || (Connection.Disconnected));
            }
        }

        public P4ScmProvider(P4VsProviderService sccService)
        {
            _sccService = sccService;
        }

        public P4ScmProvider(string solutionFile, P4VsProviderService sccService)
        {
            SolutionFile = solutionFile;
            _sccService = sccService;
        }

        private string _solutionFile;
		public string SolutionFile
		{
			get { return _solutionFile; }
			set
			{
				if (_solutionFile == value)
				{
					//no change
					return;
				}
				_solutionFile = value;

				if (string.IsNullOrEmpty(_solutionPath) || 
                    (value.StartsWith(_solutionPath, StringComparison.OrdinalIgnoreCase) == false))
				{
					SolutionPath = Path.GetDirectoryName(_solutionFile);
				}
                if ((Preferences.LocalSettings.GetBool("PreloadScmCache", true)==false) && 
                    (Connection.Repository != null) && (Connection.Repository.Connection != null))
                {
                    // only add if not already loaded by setting the SolutionPath
                    AddFileToCache(SolutionFile);
                }
            }
        }

		private string _solutionPath { get; set; }
		string SolutionPath
		{
			get { return _solutionPath; }
			set 
			{ 
				_solutionPath = value;
                if (Preferences.LocalSettings.GetBool("PreloadScmCache", true) && (Connection.Repository != null) && (Connection.Repository.Connection != null))
				{
                    Connection.Repository.Connection.CurrentWorkingDirectory = _solutionPath;

					AddDirectoryFilesToCache(_solutionPath, true);
				}
            }
		}

		// Local ignore to minimize fstating
		private bool _enforceLocalIgnore = false;
		private bool _enforceLocalIgnoreInitialized = false;
		private string _enforceLocalIgnorePrefKey = null;
		public bool EnforceLocalIgnore
		{
			get
			{
				if (!Connected)
				{
					return false;
				}
				if (_enforceLocalIgnoreInitialized == false)
				{
					if (_enforceLocalIgnorePrefKey == null)
					{
						_enforceLocalIgnorePrefKey = "EnforceLocalIgnore" + "_" +
                           Connection.Repository.Connection.Server.Address.Uri.Replace(':', '_') + "_" +
                           Connection.Repository.Connection.Client.Name;
					} 
					if (Preferences.LocalSettings.ContainsKey(_enforceLocalIgnorePrefKey))
					{
						_enforceLocalIgnore = Preferences.LocalSettings.GetBool(_enforceLocalIgnorePrefKey, false);
					}
					else
					{
						_enforceLocalIgnore = Preferences.LocalSettings.GetBool("EnforceLocalIgnore", false);
					}
				}
				return _enforceLocalIgnore;
			}
			set
			{
				if (_enforceLocalIgnore != value)
				{
					_enforceLocalIgnore = value;
					if (_enforceLocalIgnorePrefKey == null)
					{
						_enforceLocalIgnorePrefKey = "EnforceLocalIgnore" + "_" +
                           Connection.Repository.Connection.Server.Address.Uri.Replace(':', '_') + "_" +
                           Connection.Repository.Connection.Client.Name;
					}
					Preferences.LocalSettings[_enforceLocalIgnorePrefKey] = value;
				}
				_enforceLocalIgnoreInitialized = true;
			}
		}
		P4ScmCache fileCache = null;
		public bool Connected
		{
			get { return Offline == false; }
		}

		/// <summary>
		/// Verify that we are connected to the repository
		/// </summary>
		/// <returns></returns>
		public bool CheckConnection()
		{
            if (Offline)
			{
				return false;
			}

            if (Connection.Expired())
            {
                return false;
            }

            return true;
		}

		public bool IsChangelistAttachedToReview(IDictionary<int, SwarmApi.SwarmServer.Review> changes)
		{
            if (Connection.Swarm.SwarmEnabled == false)
			{
				return false;
			}
			SwarmApi.SwarmServer.ReviewList l = null;
			try
			{
				bool success = false;
				List<int> changeIds = null;

                SwarmApi.SwarmServer sw = new SwarmApi.SwarmServer(Connection.Swarm.SwarmUrl, Connection.User, Connection.Swarm.SwarmPassword);

				int[] allChangeIds = changes.Keys.ToArray();

				int idx = 0;
				while (idx < allChangeIds.Length)
				{
					changeIds = new List<int>();
					int cnt = 0;
					while ((idx < allChangeIds.Length) && (cnt < 50))
					{
						changeIds.Add(allChangeIds[idx++]);
						cnt++;
					}
					SwarmApi.Options ops = new SwarmApi.Options();
					ops["change[]"] = new JSONParser.JSONArray(changeIds.ToArray());

					l = sw.GetReviews(ops);
					if ((l != null) && (l.Count > 0) && (l[0] != null) && (l[0] is SwarmApi.SwarmServer.Review))
					{
						foreach (SwarmApi.SwarmServer.Review r in l)
						{
							foreach (int c in r.changes)
							{
								if (changes.ContainsKey(c))
								{
									changes[c] = r;
								}
							}
						}
						success = true;
					}
				}
				return success;
			}
			catch (Exception ex)
			{
				ShowException(ex);
				return false;
			}
		}



		private static string _productVersion = null;
		/// <summary>
		/// Get the file version for the executing assembly.
		/// </summary>
		public static string ProductVersion
		{
			get
			{
				if (_productVersion == null)
				{
					// Get the file version for the executing assembly.
					string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

					if (assemblyPath.StartsWith("file:"))
					{
						assemblyPath = assemblyPath.Substring(5).TrimStart('\\', '/');
					}
					FileVersionInfo.GetVersionInfo(assemblyPath);
					FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyPath);

					// return the file version number.
					_productVersion = myFileVersionInfo.ProductVersion;
				}
				return _productVersion;
			}
		}



		public Version ServerVersion
		{
			get
			{
				Version value = new Version();

                if (Connection.Repository != null)
                {
                    P4.ServerMetaData serverInfo = Connection.Repository.Server.Metadata;

                    if (serverInfo == null)
                    {
                        serverInfo = GetServerMetaData();
                    }

                    if (serverInfo == null)
                    {
                        return value;
                    }
                    string[] parts = serverInfo.Version.Major.Split('.');
                    value = new Version(string.Format("{0}.{1}.0.0", parts[0], parts[1]));
                }
				return value;
			}
		}



		






		public int Count
		{
			get
			{
				if (fileCache != null)
				{
					return fileCache.Count;
				}
				else
				{
					return 0;
				}
			}
		}



		public class FileStatusUpdateArgs : EventArgs
		{
			public FileStatusUpdateArgs(string path, UpdateType action)
			{
				Path = path;
				Action = action;
			}

			public string Path { get; private set; }

			public enum UpdateType { Add, Delete, StatusUpdate }

			public UpdateType Action { get; private set; }
		}

		public delegate void FileStatusUpdatedDelegate(object Sender, FileStatusUpdateArgs Args);

		public event FileStatusUpdatedDelegate FileStatusUpdated;

		public void BroadcastFileStatusUpdate(object Sender, FileStatusUpdateArgs Args)
		{
			if (ChangelistUpdated == null)
			{
				return;
			}
			foreach (FileStatusUpdatedDelegate d in FileStatusUpdated.GetInvocationList())
			{
				try
				{
					d(Sender, Args);
				}
				catch
				{
					// ignore errors in the delegate calls
				}
			}
		}

		public class ChangelistUpdateArgs : EventArgs
		{
			public ChangelistUpdateArgs(int changlistId, UpdateType action)
			{
				ChanglistId = changlistId;
				Action = action;
			}

			public int ChanglistId { get; private set; }

			public enum UpdateType { Edit, Add, Delete, ContentUpdate, Submitted }

			public UpdateType Action { get; private set; }
		}

		public delegate void ChangelistUpdatedDelegate(object Sender, ChangelistUpdateArgs Args);

		public event ChangelistUpdatedDelegate ChangelistUpdated;

		public void BroadcastChangelistUpdate(object Sender, ChangelistUpdateArgs Args)
		{
				if (ChangelistUpdated == null)
				{
					return;
				}
				foreach (ChangelistUpdatedDelegate d in ChangelistUpdated.GetInvocationList())
				{
					try
					{
						d(Sender, Args);
					}
					catch
					{
						// ignore errors in the delegate calls
					}
				}
		}

        private P4ScmCache FileCache
        {
            get
            {
                if (fileCache == null)
                {
                    fileCache = new P4ScmCache(this);
                }
                return fileCache;
            }
        }

		public void ClearCache()
		{
			if (fileCache != null)
			{
				fileCache.Dispose();
			}
			fileCache = new P4ScmCache(this);
		}

		public void OnCacheFilesUpdated(IList<string> files, bool force)
		{
			if (_sccService != null)
			{
				_sccService.RefreshProjectGlyphs(files, force);
			}
		}

		public bool Contains(string file)
		{
			return FileCache.ContainsKey(file);
		}

		public void PreloadFile(string file)
		{
			if (FileCache.ContainsKey(file))
			{
				return;
			}
			FileCache.Set(file, null);
		}

		public CachedFile Fetch(string path)
		{
			if (Offline)
			{
				return null;
			}
			if (!Contains(path))
			{
                if (Preferences.LocalSettings.GetBool("LazyLoadStatus", false))
                {
                    return null;
                }
				if (Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true))
				{
					string directoryPath = Path.GetDirectoryName(path);
					// file is not in the cache, but the directory has already been cached,
					// the file is not in the repository, so return a NULL
					if (FileCache.IsDirectoryCached(directoryPath))
					{
						return null;
					}
					// Directory has not been cached already, so add its files to the cache
					AddDirectoryFilesToCache(directoryPath, false);
					if (!Contains(path))
					{
						//if the file is still not in the cache, it is not in the repository
						return null;
					}
				}
				else
				{
					AddFileToCache(path);
				}
			}
			return FileCache.Get(path);
		}

		public bool IsFileCached(string path)
		{
			return ((Connected) && (Contains(path)) && (FileCache.Get(path) != null));
		}

		public P4.FileMetaData GetCachedFile(string path)
		{
            if ((Connected) && (Contains(path)))
			{
				return FileCache.Get(path);
			}
			return null;
		}

		public IList<P4.FileMetaData> GetFileMetaData(IList<string> paths, P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			return GetFileMetaData(P4.FileSpec.LocalSpecList(paths), options);
		}

		public IList<P4.FileMetaData> GetFileMetaData(IList<P4.FileSpec> files, P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			try
			{
                IList<P4.FileMetaData> value = Connection.Repository.GetFileMetaData(files, options);

                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				return value;
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
		}

		public void AddDirectoryFilesToCache(string directoryPath, bool recursive)
		{
			if (Offline)
			{
				return;
			}

			try
			{
				FileCache.AddDirectoryFilesToCache(directoryPath, recursive);
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex, false, true, (Preferences.LocalSettings.GetBool("Log_reporting", false) == true));
				}
				return;
			}
		}

		private IList<P4.FileSpec> CreateFileSpecList(IList<string> paths)
		{
			if ((paths == null) || (paths.Count <= 0))
				return null;

			IList<P4.FileSpec> value = new List<P4.FileSpec>();

			foreach (string path in paths)
			{
				value.Add(P4.FileSpec.LocalSpec(path));
			}

			return value;
		}

		public void AddFilesToCache(params string[] files)
		{
			if (Offline)
			{
				return;
			}
			try
			{
				IList<P4.FileSpec> fileList = new List<P4.FileSpec>();

                if (!string.IsNullOrEmpty(Connection.WorkspaceRoot))
				{
                    string wsRoot = Connection.WorkspaceRoot.ToLower();

					foreach (string file in files)
					{
						if ((FileCache.ContainsKey(file) == false) && (file.ToLower().StartsWith(wsRoot)))
							fileList.Add(P4.FileSpec.LocalSpec(file));
					}
				}

				if (fileList.Count <= 0)
				{
					// no new files
					return;
				}

				if (Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true))
				{
					foreach (P4.FileSpec fs in fileList)
					{
						string path = fs.LocalPath.Path;

						string directoryPath = Path.GetDirectoryName(path);
						// file is not in the cache, but the directory has already been cached,
						// the file is not in the repository, so return a NULL
						if (FileCache.IsDirectoryCached(directoryPath) == false)
						{
							// Directory has not been cached already, so add its files to the cache
							AddDirectoryFilesToCache(directoryPath, false);
						}
					}
				}
				else
				{
					int batchSize = 500;
					//Processing in batches to reduce server load and to improve performance
					for (int i = 0; i < fileList.Count; i += batchSize)
					{
						var batch = fileList.Skip(i).Take(batchSize).ToList();

					P4.Options options = new P4.Options();
					options["-Olhp"] = null;
						IList<P4.FileMetaData> fileMetdata = Connection.Repository.GetFileMetaData(batch, options);

					if (fileMetdata != null)
					{
						FileCache.AddFileMetaData(fileMetdata);
					}
				}
			}
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex, false, true, (Preferences.LocalSettings.GetBool("Log_reporting", false) == true));
				}
				return;
			}
		}

		public scmFile AddFileToCache(string file)
		{
			try
			{
				if (Offline)
				{
					return null;
				}
				if (!string.IsNullOrEmpty(Connection.WorkspaceRoot) && Connection.WorkspaceRoot.ToLower() != "null")
				{
					string wsRoot = Connection.WorkspaceRoot.ToLower();
					if ((FileCache.ContainsKey(file) == false) && (file.ToLower().StartsWith(wsRoot) == false))
					{
						FileCache.Set(file, null);
					}
				}
				if (file.EndsWith("\\..."))
				{
					// strip off the \...
					file = file.Substring(0, file.Length - 4);
					// it's a directory
					AddDirectoryFilesToCache(file, true);
				}
				if (file.EndsWith("\\*"))
				{
					// strip off the \*
					file = file.Substring(0, file.Length - 2);
					// it's a directory
					AddDirectoryFilesToCache(file, false);
				}
				else if (file.EndsWith("\\"))
				{
					// strip off the \
					file = file.Substring(0, file.Length - 1);
					// it's a directory
					AddDirectoryFilesToCache(file, false);
				}
				else
				{
					if (Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true))
					{
						string directoryPath = Path.GetDirectoryName(file);
						// file is not in the cache, but the directory has already been cached,
						// the file is not in the repository, so return a NULL
						if (FileCache.IsDirectoryCached(directoryPath) == false)
						{
							// Directory has not been cached already, so add its files to the cache
							AddDirectoryFilesToCache(directoryPath, false);
						}
					}
					if (FileCache.ContainsKey(file))
						return FileCache.Get(file);

					FileCache.AddFile(file);

					if (FileCache.ContainsKey(file))
						return FileCache.Get(file);

					FileCache.Set(file, null);
				}
				return null;
			}
			catch (System.Collections.Generic.KeyNotFoundException)
			{
				//not in cache, don't log exception 
                FileCache.Set(file, null);
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex, false, true, (Preferences.LocalSettings.GetBool("Log_reporting", false) == true));
				}
                FileCache.Set(file, null);
				return null;
			}
		}

		private bool _loadingSolution;
		public bool LoadingSolution
		{
			get { return _loadingSolution; }
			set { _loadingSolution = value; } 
		}

		public void UpdateFiles(IList<string> files, bool forceUpdate)
		{
			if ((Offline) || (files == null) || (files.Count <= 0))
			{
				return;
			}

			IList<string> oldFiles = new List<string>();
			foreach (string f in files)
			{
				if (f == null)
				{
					continue;
				}
				string file = f;
				if (file.EndsWith("\\..."))
				{
					// strip off the \...
					file = file.Substring(0, file.Length - 4);
					// it's a directory
					AddDirectoryFilesToCache(file, true);
				}
				else if (file.EndsWith("\\*"))
				{
					// strip off the \*
					file = file.Substring(0, file.Length - 2);
					// it's a directory
					AddDirectoryFilesToCache(file, false);
				}
				else if (file.EndsWith("\\"))
				{
					// strip off the \
					file = file.Substring(0, file.Length - 1);
					// it's a directory
					AddDirectoryFilesToCache(file, false);
				}
				else if (!LoadingSolution && (forceUpdate || !FileCache.CacheIsFresh(file, TimeSpan.FromSeconds(60))))
				{
					oldFiles.Add(file);
				}
			}
			if ((oldFiles == null) || (oldFiles.Count <= 0))
			{
				return;
			}
			foreach (string file in oldFiles)
			{
				if (FileCache.ContainsKey(file))
				{
                    logger.Trace("Removing file from cache: {0}", file);
					FileCache.Remove(file);
				}
			}
			//Batching server requests of size 500
			
			int batchSize = 500;
			//Processing in batches to reduce server load and to improve performance
			for (int i = 0; i < oldFiles.Count; i += batchSize)
			{
				var batch = oldFiles.Skip(i).Take(batchSize).ToList();
			IList<P4.FileMetaData> fileMetdata = null;
			try
			{
				P4.Options options = new P4.Options();
				options["-Olhp"] = null;
					fileMetdata = Connection.Repository.GetFileMetaData(options, P4.FileSpec.LocalSpecArray(batch));
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex, false, true, (Preferences.LocalSettings.GetBool("Log_reporting", false) == true));
				}
				return;
			}
			if (fileMetdata != null)
			{
				FileCache.AddFileMetaData(fileMetdata);
			}
			else
			{
					foreach (string path in batch)
				{
					FileCache.Set(path, null);
				}
			}
		}
		}

		public IList<string> UpdateDepotFiles(IList<string> files)
		{
			if ((Offline) || (files == null) || (files.Count <= 0))
			{
				return null;
			}
			
			IList<P4.FileMetaData> fileMetdata = null;
			try
			{
				P4.Options options = new P4.Options();
				options["-Olhp"] = null;
                fileMetdata = Connection.Repository.GetFileMetaData(P4.FileSpec.DepotSpecList(files.ToArray()), options);
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex, false, true, (Preferences.LocalSettings.GetBool("Log_reporting", false) == true));
				}
				return null;
			}
			FileCache.AddFileMetaData(fileMetdata);
			List<string> value = new List<string>();
			if (fileMetdata != null)
			{
				foreach (P4.FileMetaData fmd in fileMetdata)
				{
                    if (fmd.LocalPath!=null)
                    {
                        value.Add(fmd.LocalPath.Path);
                    }
				}
			}
			return value;
		}

		public string UpdateDepotFileInCache(string file)
		{
			if ((Offline) || (string.IsNullOrEmpty(file)))
			{
				return null;
			}

			IList<P4.FileMetaData> fileMetadata = null;
			try
			{
                P4.Options options = new P4.Options();
                options["-Olhp"] = null;
                fileMetadata = Connection.Repository.GetFileMetaData(options, P4.FileSpec.DepotSpec(file));
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex, false, true, (Preferences.LocalSettings.GetBool("Log_reporting", false) == true));
				}
				return null;
			}

			if (fileMetadata != null)
			{
				FileCache.AddFileMetaData(fileMetadata);
			}
			else
			{
				FileCache.Set(file, null);
			}
			if (fileMetadata != null) 
                return fileMetadata[0].LocalPath.Path;
			return null;
		}


		public CachedFile UpdateFileInCache(string file, bool forceUpdate)
		{
			if ((Offline) || (string.IsNullOrEmpty(file)))
			{
				return null;
			}
			if (!LoadingSolution && (forceUpdate || !FileCache.CacheIsFresh(file, TimeSpan.FromSeconds(60))))
			{
                logger.Trace("Removing file from cache: {0}", file);
                FileCache.Remove(file);
			}
			else
			{
				return FileCache.Get(file);
			}
			IList<P4.FileMetaData> fileMetadata = null;
			try
			{
				P4.Options options = new P4.Options();
				options["-Olhp"] = null;
                fileMetadata = Connection.Repository.GetFileMetaData(options, P4.FileSpec.LocalSpec(file));
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex, false, true, (Preferences.LocalSettings.GetBool("Log_reporting", false) == true));
				}
				// put a null in the cache to show we fsated this file
				FileCache.Set(file, null);
				return null;
			}
			if (fileMetadata != null)
			{
				FileCache.AddFileMetaData(fileMetadata);
			}
			else
			{
				// put a null in the cache to show we fstated this file
				FileCache.Set(file, null);
			}
			return FileCache.Get(file);
		}

		internal bool ShowException(Exception ex)
		{
			return ShowException(ex, false, false, true);
		}

		internal bool ShowException(Exception ex, bool showCancel, bool suppressUI, bool logReportCmdOutput)
		{
            if ((ex is P4.P4Exception) && (((P4.P4Exception)ex).ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot))
            {
                P4VsOutputWindow.AppendMessage(ex.Message);
                return false; ;
            }
            if (logReportCmdOutput)
			{
				P4VsOutputWindow.AppendMessage("[E_ERROR] " + ex.Message);
			}
			if (ex.Message.Contains("Object reference not set to an instance of an object"))
			{
				FileLogger.LogException("P4API.NET: Unexpected API error", ex);
			}
			else
			{
				FileLogger.LogException("P4API.NET", ex);
			}

			if (Offline)
			{
				P4VsProvider.BroadcastNewConnection(null);
			}

			if (ex is SwarmApi.SwarmServer.SwarmException)
			{
				return (DialogResult.Cancel == P4ErrorDlg.Show((SwarmApi.SwarmServer.SwarmException)ex, showCancel, suppressUI));
			}
			else if (ex is P4.P4CommandTimeOutException)
			{
				Dispose();

				string msg = string.Format(Resources.P4ScmProvider_CommandTimeOutError, Connection.Port);
				DialogResult res = P4ErrorDlg.Show(msg, showCancel, suppressUI);
				if (res == DialogResult.Yes)
				{
					Connection = new ConnectionHelper();
				}
				return (DialogResult.Cancel == res);
			}
			else if ((ex is P4.P4LostConnectionException) || ((ex is P4.P4Exception) && (P4.P4ClientError.IsTCPError(((P4.P4Exception)ex).ErrorCode))))
			{
				Dispose();

				string msg = string.Format(Resources.P4ScmProvider_LostConectionError, Connection.Port);
				DialogResult res = P4ErrorDlg.Show(msg, showCancel, suppressUI);
				if (res == DialogResult.Yes)
				{
					Connection = new ConnectionHelper();
				}
				return (DialogResult.Cancel == res);
			}
			else if (ex is P4.P4Exception)
			{
                if (ex.Message.Contains("Perforce password (P4PASSWD) invalid or unset."))
                {
                    P4VsProvider.Instance.currentConnectionDropDownComboChoice = Resources.ConnectionDropDownCombo_NoConnection;
                    Dispose();
                    DialogResult res = P4ErrorDlg.Show(Resources.P4ErrorDlg_LoggedOutMsg,
                        showCancel, suppressUI);
                    return (DialogResult.Cancel == res||DialogResult.OK==res);
                }
                if (ex.Message.Contains("Connect to server failed;"))
                {
                    P4VsProvider.Instance.currentConnectionDropDownComboChoice = Resources.ConnectionDropDownCombo_NoConnection;
                    Dispose();
                    DialogResult res = P4ErrorDlg.Show(string.Format(Resources.P4ScmProvider_LostConectionError, Connection.Port),
                        showCancel, suppressUI);
                    return (DialogResult.Cancel == res || DialogResult.OK == res);
                }
                if (ex.Message.Contains("Your session has expired"))
                {
                    P4VsProvider.Instance.currentConnectionDropDownComboChoice = Resources.ConnectionDropDownCombo_NoConnection;
                    Dispose();
                    DialogResult res = P4ErrorDlg.Show(string.Format(Resources.P4ScmProvider_LoginExpired, Connection.Port),
                        showCancel, suppressUI);
                    return (DialogResult.Cancel == res || DialogResult.OK == res);
                }
                return (DialogResult.Cancel == P4ErrorDlg.Show((P4.P4Exception)ex, showCancel, suppressUI));
			}
			else if (suppressUI)
			{
				return false;
			}
			else if (showCancel)
			{
				if (ex.Message.Contains("Object reference not set to an instance of an object"))
				{
					return (DialogResult.Cancel == MessageBox.Show("Unexpected API error", "Perforce SCM", MessageBoxButtons.OKCancel));
				}
				return (DialogResult.Cancel == MessageBox.Show(ex.Message, "Perforce SCM", MessageBoxButtons.OKCancel));
			}
			else if (ex.Message.Contains("Object reference not set to an instance of an object"))
			{
				MessageBox.Show("Unexpected API error", "Perforce SCM", MessageBoxButtons.OK);
			}
			else
			{
				MessageBox.Show(ex.Message, "Perforce SCM", MessageBoxButtons.OK);
			}
			return false;
		}

		public bool RenameFile(string[] oldNames, string[] newNames, int changeListId)
		{
			if (Offline)
			{
				return false;
			}
			P4.ServerMetaData info = GetServerMetaData();

			if (ServerVersion < Versions.V9_2|| info.MoveEnabled== false)
			{
				IList<P4.FileSpec> oldFiles = new List<P4.FileSpec>();
				IList<P4.FileSpec> newFiles = new List<P4.FileSpec>();

				string change = changeListId.ToString();

				if (changeListId == 0)
				{
					// default
					change = null;
				}

				if (changeListId == -1)
				{
					// new
					change = null;
				}


				P4.Options opts = new P4.Options();
				if (changeListId > 0)
				{
					opts["-c"] = change;
				}

				opts["-f"] = null;

				int idx = 0;
				foreach (string file in oldNames)
				{
					P4.FileSpec oldFile = new P4.FileSpec(new P4.LocalPath(oldNames[idx]), null);
					P4.FileSpec newFile = new P4.FileSpec(new P4.LocalPath(newNames[idx]), null);

                    Connection.Repository.Connection.Client.IntegrateFiles(oldFile, opts, newFile);

					opts = new P4.Options();
					if (changeListId > 0)
					{
						opts["-c"] = change;
					}

					opts["-v"] = null;
					oldFiles.Add(oldFile);

                    Connection.Repository.Connection.Client.DeleteFiles(oldFiles, opts);

					idx++;
				}

						BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changeListId, ChangelistUpdateArgs.UpdateType.ContentUpdate));
			}

			else
			{
					MoveFiles(changeListId, oldNames, newNames);
				
					BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changeListId, ChangelistUpdateArgs.UpdateType.ContentUpdate));
			}

			return true;
		}

		public int CheckoutFiles(int changelistID, string newChangeDescription, params string[] files)
		{
			if (Connection == null || Offline)
			{
				return -2;
			}
			try
			{
				P4.Options options = null;
				if (changelistID > 0)
				{
					options = new P4.Options(P4.EditFilesCmdFlags.None, changelistID, null);
				}

                IList<P4.FileSpec> openedFiles = Connection.Repository.Connection.Client.EditFiles(P4.FileSpec.LocalSpecList(files), options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				// lock files on checkout if that preference is set
                if (Preferences.LocalSettings.ContainsKey("Checkout_lock"))
				{
					if ((bool)Preferences.LocalSettings["Checkout_lock"] == true)
					{
						LockFiles(files);
					}
				}

				// If specified, create a new change list and move the opened files into the new list
				if (changelistID <= -1)
				{
                    if (newChangeDescription==null||
                        newChangeDescription == Resources.DefaultChangeListDescription)
					{
						newChangeDescription = Resources.P4VsProvider_CheckoutFilesDefaultChangelistDescription;
					}
					P4.Changelist newChange = MoveFilesToChangeList(changelistID, newChangeDescription, openedFiles);
					
					return newChange.Id;
				}
				BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changelistID, ChangelistUpdateArgs.UpdateType.ContentUpdate));
					
				return changelistID;
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return -2;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return -2;
			}
		}

		public P4.Changelist MoveFilesToChangeList(int changelistID, string Description, params string[] files)
		{
			if (Connection == null || Offline)
			{
				return null;
			}
			P4.Changelist moved= MoveFilesToChangeList(changelistID, Description, P4.FileSpec.LocalSpecList(files));
			
			return moved;
		}

		public P4.Changelist MoveFilesToChangeList(int changelistID, string Description, IList<P4.FileSpec> files)
		{
			if (Connection == null || Offline)
			{
				return null;
			}
			if ((files == null) || (files.Count <= 0))
			{
				//nothing to move
				return null;
			}
			P4.Changelist Change = null;
			int changeID = changelistID;

			IDictionary<int, object> changelistIds = MapChanges(files);

			BroadcastChangelistUpdates(changelistIds, ChangelistUpdateArgs.UpdateType.ContentUpdate);

			// If specified, create a new change list and move the opened files into the new list
			if (changelistID <= -1)
			{
				if (string.IsNullOrEmpty(Description))
				{
					return null;
				}
                Change = Connection.Repository.NewChangelist();
                // Make sure files is empty. If default has files in it, a new changelist
                // will automatically get those files.
                Change.Files = new List<P4.FileMetaData>();
                Change.Description = Description;

                Change = Connection.Repository.CreateChangelist(Change);

				changeID = Change.Id;

				BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changeID, ChangelistUpdateArgs.UpdateType.Add));
			}
			try
			{
                Connection.Repository.Connection.Client.ReopenFiles(files, new P4.Options(changeID, null));

                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			BroadcastChangelistUpdates(changelistIds, ChangelistUpdateArgs.UpdateType.ContentUpdate);

			BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changeID, ChangelistUpdateArgs.UpdateType.ContentUpdate));

			return Change;
		}


		
		/// <summary>
		/// Submit one or more files from the default changelist
		/// </summary>
		/// <param name="file"></param>
		/// <param name="Description"></param>
		public int SubmitFiles(string Description, P4.ClientSubmitOptions submitOptions, bool reopenFiles, params string[] files)
		{
			if (Connection == null || Offline)
			{
				return -2;
			}
			try
			{
				if ((files == null) || (files.Length <= 0))
				{
					// submitting all files in the default changelist
					return SubmitChangelist(0, Description, submitOptions, reopenFiles);
				}

				P4.P4CommandResult results = null;
				P4.Options options = null;
				P4.FileSpec fs = null;
				P4.SubmitResults submitResults = null;
				P4.Changelist change = null;

				if (reopenFiles && (submitOptions == null))
				{
					submitOptions = new P4.ClientSubmitOptions();
				}
				if (submitOptions != null)
				{
					submitOptions.Reopen = reopenFiles;
				}

				P4.SubmitFilesCmdFlags flags = P4.SubmitFilesCmdFlags.None;

                if (Preferences.LocalSettings.GetBool("DisableParallelSubmit", false))
                {
                    flags = SubmitFilesCmdFlags.DisableParallel;
                }

                // submitting a single file in the default changelist
                if (files.Length == 1)
				{
					options = new P4.SubmitCmdOptions(flags, -1, null, Description, submitOptions);

					// submitting a single file
					fs = new P4.FileSpec(new P4.LocalPath(files[0]));
                    submitResults = Connection.Repository.Connection.Client.SubmitFiles(options, fs);

                    results = Connection.Repository.Connection.LastResults;
					P4ErrorDlg.Show(results, false);

					BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(0, ChangelistUpdateArgs.UpdateType.ContentUpdate));

					return submitResults.ChangeIdAfterSubmit;
				}
				// submitting multiple files from the default changelist
				change = Connection.Repository.NewChangelist();
                change.Description = Description;
                change.OwnerName = Connection.Repository.Connection.UserName;

				foreach (string file in files)
				{
					P4.FileMetaData fmd = Fetch(file);
					if (fmd == null)
					{
						fmd = GetFileMetaData(null,file)[0];
					}
					change.Files.Add(fmd);
				}

				// submit them in the new changelist
				options = new P4.SubmitCmdOptions(flags, -1, change, null, submitOptions);

                submitResults = Connection.Repository.Connection.Client.SubmitFiles(options, null);

                results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				if (results.Success)
				{
					BroadcastChangelistUpdate(this,
						new ChangelistUpdateArgs(submitResults.ChangeIdAfterSubmit, ChangelistUpdateArgs.UpdateType.Submitted));
				}
				else
				{
					// if the submit failed, there will be a new pending changelist
					BroadcastChangelistUpdate(this, 
						new ChangelistUpdateArgs(submitResults.ChangeIdBeforeSubmit, ChangelistUpdateArgs.UpdateType.ContentUpdate));
				}
				return submitResults.ChangeIdAfterSubmit;
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return -2;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				try
				{
				string findNewChange = ex.Message;
				int idx = findNewChange.LastIndexOf("-c ");
					if (idx >= 0)
					{
						findNewChange = findNewChange.Remove(0, idx + 3);
				idx = findNewChange.LastIndexOf("'");
				findNewChange = findNewChange.Remove(idx);
				Int32 changeId = Convert.ToInt32(findNewChange);

				BroadcastChangelistUpdate(this,
						new ChangelistUpdateArgs(changeId, ChangelistUpdateArgs.UpdateType.ContentUpdate));
					}
				}
				catch (Exception innerEx)// don't let errors get thrown out of the exception handler
				{
					P4VsOutputWindow.AppendMessage("[E_ERROR] " + innerEx.Message);
					FileLogger.LogException("P4API.NET", innerEx);
				}
				return -2;
			}
		}

		/// <summary>
		/// Submit a changelist
		/// </summary>
		/// <param name="changelistID"></param>
		/// <param name="Description"></param>
		/// <returns></returns>
		public int SubmitChangelist(int changelistID, string Description, P4.ClientSubmitOptions submitOptions, bool reopenFiles)
		{
			if (Connection == null || Offline)
			{
				return -2;
			}
			try
			{
				if (changelistID < 0)
				{
					throw new ArgumentException(Resources.P4ScmProvider_CantSubmitNewChangelistError, "changelistID");
				}
				P4.P4CommandResult results = null;
				P4.Options options = null;
				P4.Changelist change = null;
				P4.SubmitResults submitResults =  null;

				if (reopenFiles && (submitOptions == null))
				{
					submitOptions = new P4.ClientSubmitOptions();
				}
				if (submitOptions != null)
				{
					submitOptions.Reopen = reopenFiles;
				}

				P4.SubmitFilesCmdFlags flags = P4.SubmitFilesCmdFlags.None;

                if (Preferences.LocalSettings.GetBool("DisableParallelSubmit", false))
                {
                    flags = SubmitFilesCmdFlags.DisableParallel;
                }

                if (changelistID == 0)
				{

					// submitting all of the files in the default changelist
					options = new P4.SubmitCmdOptions(flags, -1, null, Description, submitOptions);

                    submitResults = Connection.Repository.Connection.Client.SubmitFiles(options, null);

                    results = Connection.Repository.Connection.LastResults;
					P4ErrorDlg.Show(results, false);

					if (results.Success)
					{
						BroadcastChangelistUpdate(this, 
							new ChangelistUpdateArgs(submitResults.ChangeIdAfterSubmit, ChangelistUpdateArgs.UpdateType.Submitted));
					}
					else
					{
						// if the submit failed, there will be a new pending changelist
						BroadcastChangelistUpdate(this, 
							new ChangelistUpdateArgs(submitResults.ChangeIdBeforeSubmit, ChangelistUpdateArgs.UpdateType.ContentUpdate));
					}

					return submitResults.ChangeIdAfterSubmit;
				}
				if (changelistID > 0)
				{
					// submitting all the files in a numbered changelist
					change = GetChangelist(changelistID);
					if (Description != change.Description)
					{
						// the description change so update the changelist
						change.Description = Description;
						SaveChangelist(change, null);
					}
					options = new P4.SubmitCmdOptions(flags, changelistID, null, null, submitOptions);

                    submitResults = Connection.Repository.Connection.Client.SubmitFiles(options, null);

                    results = Connection.Repository.Connection.LastResults;
					P4ErrorDlg.Show(results, false);

					BroadcastChangelistUpdate(this,
						new ChangelistUpdateArgs(changelistID, ChangelistUpdateArgs.UpdateType.Submitted));

					return submitResults.ChangeIdAfterSubmit;
				}
				
				return -2;
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return -2;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return -2;
			}
		}

        /// <summary>
        /// Submit a shelved changelist
        /// </summary>
        /// <param name="changelistID"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int SubmitShelvedChangelist(int changelistID)
        {
            if (Connection == null || Offline)
            {
                return -2;
            }
            try
            {
                if (changelistID < 0)
                {
                    throw new ArgumentException(Resources.P4ScmProvider_CantSubmitNewChangelistError, "changelistID");
                }
                P4.P4CommandResult results = null;
                P4.SubmitResults submitResults = null;

                P4.Options opts = new P4.Options(
                    P4.SubmitFilesCmdFlags.SubmitShelved,
                    changelistID,
                    null,
                    null,
                    null
                    );

                if (Preferences.LocalSettings.GetBool("DisableParallelSubmit", false))
                {
                    opts["--parallel"] = "0";
                }
                submitResults = Connection.Repository.Connection.Client.SubmitFiles(opts, null);

                results = Connection.Repository.Connection.LastResults;
                P4ErrorDlg.Show(results, false);

                BroadcastChangelistUpdate(this,
                                          new ChangelistUpdateArgs(changelistID,
                                                                   ChangelistUpdateArgs.UpdateType.ContentUpdate));
                // Refresh the files in solution explorer
                foreach (FileSubmitRecord rec in  submitResults.Files)
                {
                    IList<FileSpec> localFiles =
                        Connection.Repository.Connection.Client.GetClientFileMappings(new FileSpec(rec.File.DepotPath,
                        null, null, null));
                    if (localFiles!=null && localFiles.Count>0)
                    {
                        SccService.RefreshProjectGlyphs(localFiles[0].LocalPath.Path, true);
                    }
                }
                return submitResults.ChangeIdAfterSubmit;
            }
            catch (ThreadAbortException)
            {
                // ignore thread aborts
                return -2;
            }
            catch (Exception ex)
            {
                // If the error is because the repository is now null, it means
                // the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
                {
                    ShowException(ex);
                }
                return -2;
            }
        }

		/// <summary>
		/// Shelve files
		/// </summary>
		/// <param name="changelistID"></param>
		/// <param name="Description"></param>
		/// <returns></returns>
		public bool ShelveFiles(int changelistID, string Description,
								P4.ShelveFilesCmdFlags flags, bool depotFiles, params string[] files)
		{
			if (Connection == null || Offline)
			{
				return false;
			}
			try
			{
				int changeIdUsed = 0;

				P4.Options options = null;
				if (changelistID > 0)
				{
					changeIdUsed = changelistID;
					options = new P4.ShelveFilesCmdOptions(flags, null, changelistID);
				}
				else
				{
					// if the files are in the default changelist, create a new numbered
					// changelist with the files.
					P4.Changelist change = Connection.Repository.NewChangelist();
                    change.Description = Description;
                    change.ClientId = Connection.Repository.Connection.Client.Name;
                    change.OwnerName = Connection.Repository.Connection.UserName;
					change.Files = new List<P4.FileMetaData>();
					foreach (string file in files)
					{
						if (depotFiles)
						{
							change.Files.Add(GetFileMetaData(file));
						}
						else
						{
							change.Files.Add(Fetch(file));
						}
					}
                    P4.Changelist newChange = Connection.Repository.CreateChangelist(change);
					BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(0, ChangelistUpdateArgs.UpdateType.ContentUpdate));
					changeIdUsed = newChange.Id;

					options = new P4.ShelveFilesCmdOptions(flags, null, newChange.Id);
				}
                if (Preferences.LocalSettings.GetBool("DisableParallelShelve", false))
                {
                    if (options == null)
                    {
                        options = new Options();
                    }
                    options["--parallel"] = "0";
                }
                if (files != null)
				{
					if (depotFiles)
					{
                        Connection.Repository.Connection.Client.ShelveFiles(P4.FileSpec.DepotSpecList(files), options);
					}
					else
					{
                        Connection.Repository.Connection.Client.ShelveFiles(P4.FileSpec.LocalSpecList(files), options);
					}
				}
				else
				{
					// all files in the changelist
                    Connection.Repository.Connection.Client.ShelveFiles(options, null);
				}
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
                
				BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changeIdUsed, ChangelistUpdateArgs.UpdateType.ContentUpdate));

				return results.Success;
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
		}


		/// <summary>
		/// Shelve files
		/// </summary>
		/// <param name="changelistID"></param>
		/// <param name="Description"></param>
		/// <returns></returns>
		public bool ShelveFiles(int changelistID, string Description,
								P4.ShelveFilesCmdFlags flags, IList<P4.FileMetaData> files)
		{
			if (Offline)
			{
				return false;
			}
			try
			{
				int changeIdUsed = 0;

				P4.Options options = null;
				if (changelistID > 0)
				{
					changeIdUsed = changelistID;
					options = new P4.ShelveFilesCmdOptions(flags, null, changelistID);
				}
				else
				{
					// if the files are in the default changelist, create a new numbered
					// changelist with the files.

                    P4.Changelist change = Connection.Repository.NewChangelist();

                    //P4.Changelist change = Repository.NewChangelist(changelistID);
				    change.Files = files;
                    change.Description = Description;
                    P4.Changelist newChange = SaveChangelist(change,null);
                    BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(0, ChangelistUpdateArgs.UpdateType.ContentUpdate));
                    changeIdUsed = newChange.Id;

					options = new P4.ShelveFilesCmdOptions(flags, null, newChange.Id);
				}
                if (Preferences.LocalSettings.GetBool("DisableParallelShelve", false))
                {
                    if (options == null)
                    {
                        options = new Options();
                    }
                    options["--parallel"] = "0";
                }
                if (files != null)
				{
					List<P4.FileSpec> fileList = new List<P4.FileSpec>();
					foreach (P4.FileMetaData file in files)
					{
						P4.FileSpec fs = (P4.FileSpec)file;
						if (fs.Version != null)
						{
							fs.Version = null;
						}
						fileList.Add(fs);
					}
                    Connection.Repository.Connection.Client.ShelveFiles(fileList, options);
				}
				else
				{
					// all files in the changelist
                    Connection.Repository.Connection.Client.ShelveFiles(options, null);
				}
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changeIdUsed, ChangelistUpdateArgs.UpdateType.ContentUpdate));

				return true;
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="changelistID"></param>
		/// <param name="Description"></param>
		/// <param name="flags"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		public bool UnshelveFiles(int fromChangelistID, int toChangelistID, 
						string Description, P4.UnshelveFilesCmdFlags flags,
						bool depotFiles, bool revertFirst, params string[] files)
		{
			if (Offline)
			{
				return false;
			}
			try
			{
				int changeIdUsed = toChangelistID;
				P4.Options options = new P4.UnshelveFilesCmdOptions(flags,fromChangelistID,0);
				if (toChangelistID > 0)
				{
					options = new P4.UnshelveFilesCmdOptions(flags, fromChangelistID, toChangelistID);
				}
				else if (toChangelistID == 0)
				{
					changeIdUsed = 0;
					options = new P4.UnshelveFilesCmdOptions(flags, fromChangelistID, 0);
				}
				else
				{
					bool preview = (flags & P4.UnshelveFilesCmdFlags.Preview) != 0;
                    if (preview == false)
                    {
                        // user selected a new changelist as the target.
                        P4.Changelist change = Connection.Repository.NewChangelist();
                        change.Description = Description;
                        // Make sure files is empty. If default has files in it, a new changelist
                        // will automatically get those files.
                        change.Files = new List<P4.FileMetaData>();
                        P4.Changelist newChange = Connection.Repository.CreateChangelist(change);
                        changeIdUsed = newChange.Id;

                        options = new P4.UnshelveFilesCmdOptions(flags, fromChangelistID, newChange.Id);
                    }
				}
				
				if (files != null)
				{
					if (depotFiles)
					{
                        Connection.Repository.Connection.Client.UnshelveFiles(P4.FileSpec.DepotSpecList(files), options);
					}
					else
					{
                        Connection.Repository.Connection.Client.UnshelveFiles(P4.FileSpec.LocalSpecList(files), options);
					}
					
				}
				else
				{
					// all files in the changelist
                    Connection.Repository.Connection.Client.UnshelveFiles(null, options);
				}
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				if (results.InfoOutput != null&& results.InfoOutput.Count>0)
				{
					string info = string.Empty;
					foreach (P4.P4ClientInfoMessage obj in results.InfoOutput)
					{
						if (obj.Message.Contains("needs fromfile") || obj.Message.Contains("needs tofile"))
						{
							info = info + obj.ToString()+"\r\n";
						}
						if (revertFirst == false && obj.Message.Contains("can't unshelve"))
						{
							info = info + obj.Message + "\r\n";
						}
					}
					if (!(string.IsNullOrEmpty(info)))
					{
						MessageBox.Show(info);
						return false;
					}
					return true;
				}
				P4ErrorDlg.Show(results, false);

				BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changeIdUsed, ChangelistUpdateArgs.UpdateType.ContentUpdate));
				if (changeIdUsed != fromChangelistID)
				{
					BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(fromChangelistID, ChangelistUpdateArgs.UpdateType.ContentUpdate));
				}
				return true;
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
		}
		public int AddFiles(int changelistID, string newChangeDescription, params string[] files)
		{
			if ((Offline) || (files == null) || (files.Length <= 0))
			{
				return -2;
			}
			try
			{
				const int batchSize = 500;
				List<P4.FileSpec> allOpenedFiles = new List<P4.FileSpec>();
				List<P4.P4CommandResult> allResults = new List<P4.P4CommandResult>();

				P4.Options options = new P4.Options();
				if (changelistID > 0)
				{
					options = new P4.Options(P4.AddFilesCmdFlags.None,changelistID, null);
				}
				
				options["-f"] = null;
				
				//Batch server requests for AddFiles to reduce server load and to improve performance
				for (int i = 0; i < files.Length; i += batchSize)
				{
					var batch = files.Skip(i).Take(batchSize).ToArray();
					var openedFiles = Connection.Repository.Connection.Client.AddFiles(P4.FileSpec.LocalSpecList(batch), options);
					var batchResult = Connection.Repository.Connection.LastResults;
					
					if (openedFiles != null)
						allOpenedFiles.AddRange(openedFiles);

					if (batchResult != null)
						allResults.Add(batchResult);
				}

				if (allResults.Count > 0)
				{
					P4ErrorDlg.Show(allResults[allResults.Count - 1], false);
				}

				// If specified, create a new change list and move the opened files into the new list
				if (changelistID <= -1 && allOpenedFiles.Count > 0)
				{
					var newChange = MoveFilesToChangeList(changelistID, newChangeDescription, allOpenedFiles);
					if (newChange!=null)
						return newChange.Id;
					}
				BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changelistID, ChangelistUpdateArgs.UpdateType.ContentUpdate));
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return -2;
			}
			catch (Exception ex)
			{
                // If the error is because the repository is now null, it means
                // the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
                {
                    if ((ex is P4.P4Exception) && (((P4.P4Exception)ex).ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot))
                    {
                        P4VsOutputWindow.AppendMessage(ex.Message);
                    }
                    else
                    {
                        ShowException(ex);
                    }
                }
				return -2;
			}

			return changelistID;
		}

		public bool MoveFiles(int changelistID, string[] fromFiles, string[] toFiles)
		{
			if (Offline)
			{
				return false;
			}
			try
			{
				P4.Options opts = new P4.Options();
				if (changelistID > 0)
				{
					opts["-c"] = changelistID.ToString();
				}
				opts["-k"] = null;
				for (int idx = 0; idx < fromFiles.Length; )
				{
					P4.FileSpec fromfs = new P4.FileSpec(new P4.LocalPath(fromFiles[idx]), null);
					P4.FileSpec tofs = new P4.FileSpec(new P4.LocalPath(toFiles[idx]), null);

                    Connection.Repository.Connection.Client.MoveFiles(fromfs, tofs, opts);
					idx++;
				}
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changelistID, ChangelistUpdateArgs.UpdateType.ContentUpdate));
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
			return true;
		}

		private IDictionary<int, object> MapChanges(params string[] files)
		{
			return MapChanges(false, files);
		}

		private IDictionary<int, object> MapChanges(bool DepotPaths, params string[] files)
		{
			Dictionary<int, object> changeMap = new Dictionary<int, object>();
			foreach (string file in files)
			{
				if (file == null)
				{
					continue;
				}
				else if ((file.EndsWith("\\")) || (file.EndsWith("\\*")) || (file.EndsWith("\\...")))
				{
					List<P4.FileSpec> fileSpecs = new List<P4.FileSpec>();
					fileSpecs.Add(P4.FileSpec.DepotSpec(file));
                    IList<P4.File> opened = Connection.Repository.GetOpenedFiles(fileSpecs, null);
					if (opened != null)
					{
						foreach (P4.File f in opened)
						{
							if ((f != null) && (changeMap.ContainsKey(f.ChangeId) == false) &&
								(f.Action != P4.FileAction.None))
							{
								changeMap.Add(f.ChangeId, f);
							}
						}
					}
					continue;
				}
				P4.FileMetaData fmd = null;
				if (DepotPaths)
				{
					fmd = GetFileMetaData(file);
				}
				else
				{
					fmd = Fetch(file);
				}
				if ((fmd != null) && (changeMap.ContainsKey(fmd.Change) == false) &&
					(fmd.Action != P4.FileAction.None))
				{
					changeMap.Add(fmd.Change, fmd);
				}
			}
			if (changeMap.Count > 0)
			{
			return changeMap;
		}
			return null;
		}

		private IDictionary<int, object> MapChanges(IList<P4.FileSpec> files)
		{
			Dictionary<int, object> changeMap = new Dictionary<int, object>();

			IList<P4.FileSpec> fileSpecs = P4.FileSpec.UnversionedSpecList(files);

            IList<P4.File> opened = Connection.Repository.GetOpenedFiles(fileSpecs, null);
			if (opened != null)
			{
				foreach (P4.File f in opened)
				{
					if ((f != null) && (changeMap.ContainsKey(f.ChangeId) == false) &&
						(f.Action != P4.FileAction.None))
					{
						changeMap.Add(f.ChangeId, f);
					}
				}
			}
			if (changeMap.Count > 0)
			{
				return changeMap;
			}
			return null;
		}

		internal void BroadcastChangelistUpdates(IDictionary<int, object> changeMap, ChangelistUpdateArgs.UpdateType changeType)
		{
			if (changeMap != null)
			{
				foreach (int changeId in changeMap.Keys)
				{
					BroadcastChangelistUpdate(this,
						new ChangelistUpdateArgs(changeId, changeType));
				}
			}
		}

        public IList<FileSpec> ReconcileStatus(IList<FileSpec> Files, Options options)
        {
            IList<FileSpec> results =
                Connection.Repository.Connection.Client.ReconcileStatus(Files, options);
            return results;
        }

        public IVsProject3 RemoveFromProject(string file)
		{
				Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
				string[] remove = new string[1] { file };
				remove[0] = remove[0].Replace("/", "\\");

				// find the project containing both by finding the file to be removed
				IVsProject3 project = null;
				var projects = new List<VSITEMSELECTION>();
				projects.AddRange(SccService.GetControlledProjectsContainingFiles(remove.Length, remove));
				if (projects.Count > 0)
				{
					project = (IVsProject3)projects[0].pHier;

                    // remove a file from the project
					int iFound = 0;
					uint itemId = 0;
					//EnvDTE.ProjectItem item;
					VSDOCUMENTPRIORITY[] pdwPriority = new VSDOCUMENTPRIORITY[1];
					project.IsDocumentInProject(remove.ToArray()[0], out iFound,
						 pdwPriority, out itemId);
					int retVal;
					int result;
					retVal = project.RemoveItem(0, itemId, out result);
				}

					return project;
		}

		public bool AddToProject(IVsProject3 project, string file)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			// add the reverted move/delete
			int retVal;
			VSADDRESULT[] addResArr = new VSADDRESULT[1];
			string[] fileNames = new string[1] { file };
			fileNames[0] = fileNames[0].Replace("/", "\\");
			uint cFilesToOpen = 1;

			retVal = project.AddItem(Microsoft.VisualStudio.VSConstants.VSITEMID_ROOT,
                        VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE, file, cFilesToOpen,
                        fileNames, IntPtr.Zero, addResArr);

			return true;
		}

		public List<string> PreviewRevertFiles(P4.Options options, P4.Changelist changelist, bool depotFiles, params string[] files)
		{
			if (Offline)
			{
				return null;
			}

			List<string> preview = new List<string>();

			try
			{
                if(depotFiles)
                {
                    Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(files), options);
                }
                else
                {
                    Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.LocalSpecList(files), options);
                }

                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				if (results.TaggedOutput != null)
				{
					foreach (P4.TaggedObject obj in results.TaggedOutput)
					{
						if (obj.ContainsKey("depotFile"))
						{
							preview.Add(obj["depotFile"].ToString());
						}
						int idx = 0;
						string key = String.Format("depotFile{0}", idx);
						if (obj.ContainsKey(key))
						{
							while (obj.ContainsKey(key))
							{
								preview.Add(obj[key].ToString());
								idx++;
								key = String.Format("depotFile{0}", idx);
							}
						}
					}
				}

				return preview;
			}
			catch
			(System.Threading.ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch
			(Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
		}


		public IList<string> RevertFiles(bool unchangedOnly, bool DepotFiles, P4.Options opts, params string[] files)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			if (Offline || files == null)
            {
				return null;
			}
			try
			{
				List<P4.FileSpec> filesReverted = new List<FileSpec>();

				Dictionary<string, string> movedForRevert = new Dictionary<string, string>();
				P4.Options options = opts;
				if (unchangedOnly)
				{
					options = new P4.Options(P4.RevertFilesCmdFlags.UnchangedOnly, -1);
				}
				if (DepotFiles)
				{
					if (options != null && (options.ContainsKey("-n") || options.ContainsKey("-a")))
					{
                        IList<P4.FileSpec> fr = Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(files), options);
						if (fr != null)
						{
							filesReverted.AddRange(fr);
						}
					}

					// check the preference to see if the project should be updated
					else if (Preferences.LocalSettings.GetBool("Update_Project", true))
					{
						foreach (string file in files)
						{
							P4.FileMetaData fmd = GetFileMetaData(file);
							if (fmd !=null&&fmd.Action == P4.FileAction.MoveAdd)
							{
								string addPath = fmd.ClientPath.Path;
								P4.FileMetaData fmd2 = GetFileMetaData(fmd.MovedFile.Path);
								string delPath = fmd2.ClientPath.Path;
								IVsProject3 project = RemoveFromProject(addPath);
								if (project != null)
								{
                                    IList<P4.FileSpec> fr = Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(file), options);
									if (fr!=null)
                                    {
                                       filesReverted.AddRange(fr);
                                    }
									AddToProject(project, delPath);
								}
								else
								{
                                    IList<P4.FileSpec> fr = Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(file), options);
                                    if (fr != null)
                                    {
                                        filesReverted.AddRange(fr);
                                    }
								}
							}
							else
							{
                                IList<P4.FileSpec> fr = Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(file), options);
                                if (fr != null)
                                {
                                    filesReverted.AddRange(fr);
                                }
							}
						}
					}
					else
					{
                        IList<P4.FileSpec> fr = Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(files), options);
                        if (fr != null)
                        {
                            filesReverted.AddRange(fr);
                        }
					}
				}
				else
				{
					if (options != null && (options.ContainsKey("-n") || options.ContainsKey("-a")))
					{
                        IList<P4.FileSpec> fr = Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.LocalSpecList(files), options);
						if (fr != null)
						{
							filesReverted.AddRange(fr);
						}
					}
					// change this to else if with preference check
					else if (Preferences.LocalSettings.GetBool("Update_Project", true))
					{
						foreach (string file in files)
						{
							P4.FileMetaData fmd = GetFileMetaData(file);
							if (fmd == null)
							{
								continue;
							}
							if (fmd.Action == P4.FileAction.MoveAdd)
							{
								string addPath = fmd.ClientPath.Path;
								P4.FileMetaData fmd2 = GetFileMetaData(fmd.MovedFile.Path);
								string delPath = fmd2.ClientPath.Path;
								IVsProject3 project = RemoveFromProject(addPath);
								if (project != null)
								{
                                    IList<P4.FileSpec> fr = Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.LocalSpecList(file), options);
                                    if (fr != null)
                                    {
                                        filesReverted.AddRange(fr);
                                    }
									AddToProject(project, delPath);

									string[] del = new string[1] { delPath };
									var projects = new List<VSITEMSELECTION>();
									projects.AddRange(SccService.GetControlledProjectsContainingFiles(del.Length, del));
									if (projects.Count > 0)
									{
										IVsSolution sol = (IVsSolution)P4VsProvider.Instance.GetService(typeof(SVsSolution));
										uint opts2 = (uint)(__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty & __VSSLNSAVEOPTIONS.SLNSAVEOPT_SkipDocs &
										__VSSLNSAVEOPTIONS.SLNSAVEOPT_SkipSolution & __VSSLNSAVEOPTIONS.SLNSAVEOPT_SkipUserOptFile);
										sol.SaveSolutionElement(opts2, projects[0].pHier, 0);
										project = (IVsProject3)projects[0].pHier;
									}
								}
								else
								{
                                    IList<P4.FileSpec> fr = Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.LocalSpecList(file), options);
                                    if (fr != null)
                                    {
                                        filesReverted.AddRange(fr);
                                    }
								}
							}
							else
							{
								if (fmd.Action != P4.FileAction.MoveDelete)
								{
                                    IList<P4.FileSpec> fr = Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.LocalSpecList(file), options);
                                    if (fr != null)
                                    {
                                        filesReverted.AddRange(fr);
                                    }
								}
							}
						}
					}
					else
					{
                        IList<P4.FileSpec> fr = Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.LocalSpecList(files), options);
                        if (fr != null)
                        {
                            filesReverted.AddRange(fr);
                        }
					}
				}
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				if (filesReverted != null)
				{
					List<string> revertedFiles = new List<string>();

					foreach (P4.FileSpec fs in filesReverted)
					{
						if (fs.LocalPath != null)
						{
							revertedFiles.Add(fs.LocalPath.Path);
						}
						else if (fs.DepotPath != null)
						{
							revertedFiles.Add(fs.DepotPath.Path);
						}
					}
					IDictionary<int, object> changelistIds = MapChanges(DepotFiles, revertedFiles.ToArray());
                    if (changelistIds != null)
                    {
                        BroadcastChangelistUpdates(changelistIds, ChangelistUpdateArgs.UpdateType.ContentUpdate);
                    }

					return revertedFiles;
				}
				return null;
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
		}

		public bool RevertFilesInChangelist(P4.Options options, P4.Changelist changelist, params string[] files)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			if (Offline)
			{
				return false;
			}
			try
			{
				options["-c"] = changelist.Id.ToString();
				if (changelist.Id < 1)
				{
					options["-c"] = "default";
				}

				if (options != null && (options.ContainsKey("-n") || options.ContainsKey("-a")))
				{
                    Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(files), options);
                    BroadcastChangelistUpdate(this,
                    new ChangelistUpdateArgs(changelist.Id, ChangelistUpdateArgs.UpdateType.ContentUpdate));
				    return true;
				}

				// check the preference to see if the project should be updated
				if (Preferences.LocalSettings.GetBool("Update_Project", true))
				{
					if (options["-c"] == "default")
					{
						IList<P4.File> fs = GetOpenedFiles(P4.FileSpec.DepotSpecList(files), options);
						foreach (P4.File f in fs)
						{
							P4.FileMetaData fmd = GetFileMetaData(f.DepotPath.Path);
							if (fmd.Action == P4.FileAction.MoveAdd)
							{
								string addPath = fmd.LocalPath.Path;
								P4.FileMetaData fmd2 = GetFileMetaData(fmd.MovedFile.Path);
								string delPath = fmd2.LocalPath.Path;
								IVsProject3 project = RemoveFromProject(addPath);
								if (project != null)
								{
                                    Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(f.DepotPath.Path), options);
									AddToProject(project, delPath);

									string[] del = new string[1]{delPath};
									var projects = new List<VSITEMSELECTION>();
									projects.AddRange(SccService.GetControlledProjectsContainingFiles(del.Length, del));
									if (projects.Count > 0)
									{
										IVsSolution sol = (IVsSolution)P4VsProvider.Instance.GetService(typeof(SVsSolution));
										uint opts = (uint)(__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty & __VSSLNSAVEOPTIONS.SLNSAVEOPT_SkipDocs &
										__VSSLNSAVEOPTIONS.SLNSAVEOPT_SkipSolution & __VSSLNSAVEOPTIONS.SLNSAVEOPT_SkipUserOptFile);
										sol.SaveSolutionElement(opts, projects[0].pHier, 0);
									project = (IVsProject3)projects[0].pHier;
									}
								}
								else
								{
                                    Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(f.DepotPath.Path), options);
								}
							}
							else
							{
								if (fmd.Action != P4.FileAction.MoveDelete)
								{
                                    Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(f.DepotPath.Path), options);
								}
							}
						}
					}
					else
					{
						P4.Options opts = new P4.Options();
						opts["-e"]=options["-c"];
						opts["-Olhp"] = null;
						opts["-Rco"] = null;
						IList<P4.FileMetaData> fmd = GetFileMetaData(opts, files[0]);
						foreach (P4.FileMetaData f in fmd)
							if (f.DepotPath != null && f.Action == P4.FileAction.MoveAdd)
							{
								string addPath = f.LocalPath.Path;
								P4.FileMetaData f2 = GetFileMetaData(f.MovedFile.Path);
								string delPath = f2.LocalPath.Path;
								IVsProject3 project = RemoveFromProject(addPath);
								if (project != null)
								{
                                    Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(f.DepotPath.Path), options);
									AddToProject(project, delPath);
								}
								else if (f.DepotPath != null)
								{
                                    Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(f.DepotPath.Path), options);
								}
							}
							else if (f.DepotPath != null && f.Action != P4.FileAction.MoveDelete)
							{
                                Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(f.DepotPath.Path), options);
							}
					}
				}

				else
				{
                    Connection.Repository.Connection.Client.RevertFiles(P4.FileSpec.DepotSpecList(files), options);
				}

                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				BroadcastChangelistUpdate(this,
					new ChangelistUpdateArgs(changelist.Id, ChangelistUpdateArgs.UpdateType.ContentUpdate));
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
			return true;
		}

		public bool LockFiles(params string[] files)
		{
			if (Offline)
			{
				return false;
			}
			try
			{
				IDictionary<int, object> changelistIds = MapChanges(files);

                Connection.Repository.Connection.Client.LockFiles(P4.FileSpec.LocalSpecList(files), (P4.Options)null);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				BroadcastChangelistUpdates(changelistIds, ChangelistUpdateArgs.UpdateType.ContentUpdate);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
			return true;
		}

		public bool UnlockFiles(params string[] files)
		{
			if (Offline)
			{
				return false;
			}
			try
			{
				IDictionary<int, object> changelistIds = MapChanges(files);

                IList<P4.FileSpec> f = Connection.Repository.Connection.Client.UnlockFiles(P4.FileSpec.LocalSpecList(files), (P4.Options)null);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				BroadcastChangelistUpdates(changelistIds, ChangelistUpdateArgs.UpdateType.ContentUpdate);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
			return true;
		}

		public bool DeleteFiles(int changelistID, string newChangeDescription, params string[] files)
		{
			int changeIdUsed;
			return DeleteFiles( changelistID, newChangeDescription, out changeIdUsed, files);
		}
		public bool DeleteFiles(int changelistID, string newChangeDescription, out int changeIdUsed, params string[] files)
		{
			changeIdUsed = changelistID;
			if (Offline)
			{
				return false;
			}
			try
			{
				P4.Options options = new P4.Options(DeleteFilesCmdFlags.ServerOnly, changelistID);

                IList<P4.FileSpec> openedFiles = Connection.Repository.Connection.Client.DeleteFiles(P4.FileSpec.LocalSpecList(files), options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				// If specified, create a new change list and move the opened files into the new list
				if ((changelistID <= -1)&&(openedFiles!=null))
				{
					P4.Changelist newChange = MoveFilesToChangeList(changelistID, newChangeDescription, openedFiles);

					changeIdUsed = newChange.Id;
				}
				BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changeIdUsed, ChangelistUpdateArgs.UpdateType.ContentUpdate));
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
			return true;
		}

        /// <summary>
        /// find install location of Perforce applications
        /// </summary>
        /// <returns>install path</returns>
        private string P4InstallLocation()
        {
            string location = "";
            foreach (ProductInstallation product in
                ProductInstallation.GetRelatedProducts("{70A9FDC7-885B-4D6D-BAFD-CB2D27AB2963}"))
            {
                if (product.InstallLocation != null)
                {
                    location = product.InstallLocation;
                    return location;
                }
            }
            return null;
        }

        private string _p4MergePath = null;

		public bool P4MergeExists()
		{
			string mergePath;
			if (Preferences.LocalSettings.GetBool("P4Merge_app", true))
			{
				// use configured application
				mergePath = P4MergePath();
				if (string.IsNullOrEmpty(mergePath) || !File.Exists(mergePath))
				{
                    logger.Warn("Cannot locate P4Merge");
					return false;
				}
			}
			else
			{
				mergePath = Preferences.LocalSettings.GetString("P4Merge_path", string.Empty);
				if (string.IsNullOrEmpty(mergePath) || !File.Exists(mergePath))
				{
                    logger.Warn("The configured merge application does not exist at the specified path");
                    return false;
				}
			}
			return true;
		}

        /// <summary>
        /// find P4Merge.exe
        /// </summary>
        /// <returns></returns>
        private string P4MergePath()
		{
			if (_p4MergePath != null)
			{
				if (_p4MergePath.EndsWith("P4Merge.exe")&& File.Exists(_p4MergePath))
				{
					return _p4MergePath;
				}
			}
			string prefString = Preferences.LocalSettings.GetString("P4Merge_path", null);

			if ((prefString != null) && (prefString.EndsWith("P4Merge.exe"))
                && File.Exists(prefString))
			{
				_p4MergePath = prefString;
			}
			else
			{
                string installRoot = P4InstallLocation();
                if (installRoot!=null && File.Exists(installRoot +"P4Merge.exe"))
				{
					Preferences.LocalSettings["P4Merge_path"] = installRoot + "P4Merge.exe";
					_p4MergePath = installRoot + "P4Merge.exe";
                    return _p4MergePath;
				}
				else
				{
                    MessageBox.Show(Resources.P4ScmProvider_CannotFindP4VMerge, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			return _p4MergePath;
		}

		private string _p4VPath = null;
        private string _p4VCPath = null;

		/// <summary>
		/// find P4V.exe
		/// </summary>
		/// <returns></returns>
        private string P4VPath()
        {
            if (_p4VPath != null)
            {
                if (_p4VPath.EndsWith("P4V.exe") && File.Exists(_p4VPath))
                {
                    return _p4VPath;
                }
            }
            string prefString = Preferences.LocalSettings.GetString("P4V_path", null);

            if ((prefString != null) && (prefString.EndsWith("P4V.exe"))
                && File.Exists(prefString))
            {
                _p4VPath = prefString;
            }
            else
            {
                string installRoot = P4InstallLocation();
                if (installRoot!=null && File.Exists(installRoot + "P4V.exe"))
                {
                    Preferences.LocalSettings["P4V_path"] = installRoot + "P4V.exe";
                    _p4VPath = installRoot + "P4V.exe";
                    return _p4VPath;
                }
                else
                {
                    MessageBox.Show(Resources.P4ScmProvider_CannotFindP4VError, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return _p4VPath;
        }

        /// <summary>
        /// find P4VC.exe
        /// </summary>
        /// <returns></returns>
        private string P4VCPath()
        {
            if (_p4VCPath != null)
            {
                if ((_p4VCPath.ToLower().EndsWith("p4vc.bat") && File.Exists(_p4VCPath))||
                    (_p4VCPath.ToLower().EndsWith("p4vc.exe")) && File.Exists(_p4VCPath))
                {
                    return _p4VCPath;
                }
            }

            string prefString = Preferences.LocalSettings.GetString("P4VC_path", null);
            string installRoot = P4InstallLocation();

            if ((prefString != null) && File.Exists(prefString) && (prefString.ToLower().EndsWith("p4vc.bat") || prefString.ToLower().EndsWith("p4vc.exe")))
            {   
				_p4VCPath = prefString;
				return _p4VCPath;
			}
            
            
            if (installRoot != null && File.Exists(installRoot + "p4vc.exe"))
            {
                Preferences.LocalSettings["P4VC_path"] = installRoot + "p4vc.exe";
                _p4VCPath = installRoot + "p4vc.exe";
                return _p4VCPath;
            }

            if (installRoot != null && File.Exists(installRoot + "p4vc.bat"))
            {
                Preferences.LocalSettings["P4VC_path"] = installRoot + "p4vc.bat";
                _p4VCPath = installRoot + "p4vc.bat";
                return _p4VCPath;
            }
            
            MessageBox.Show(Resources.P4ScmProvider_CannotFindP4VError);

            return _p4VCPath;
        }

        private string _helixMFAPath = null;
        /// <summary>
		/// find HelixMFA.exe
		/// </summary>
		/// <returns></returns>
        private string HelixMFAPath()
        {
            if (_helixMFAPath != null)
            {
                if (_helixMFAPath.EndsWith("HelixMFA.exe") && File.Exists(_helixMFAPath))
                {
                    return _helixMFAPath;
                }
            }
            string prefString = Preferences.LocalSettings.GetString("HelixMFA_path", null);

            if ((prefString != null) && (prefString.EndsWith("HelixMFA.exe"))
                && File.Exists(prefString))
            {
                _helixMFAPath = prefString;
            }
            else
            {
                // check for standalone install of HMFA
                foreach (ProductInstallation product in
                    ProductInstallation.GetRelatedProducts("{771150B5-B57D-47EC-B4E4-299711C020C4}"))
                {
                    if (product.InstallLocation != null)
                    {
                        _helixMFAPath = product.InstallLocation + "HelixMFA.exe";
                        if (File.Exists(_helixMFAPath))
                        {
                            Preferences.LocalSettings["HelixMFA_path"] = _helixMFAPath;
                            return _helixMFAPath;
                        }
                    }
                }
                // check if HMFA was installed with P4V
                string installRoot = P4InstallLocation();
                if (installRoot != null && File.Exists(installRoot + "HelixMFA.exe"))
                {
                    Preferences.LocalSettings["HelixMFA_path"] = installRoot + "HelixMFA.exe";
                    _helixMFAPath = installRoot + "HelixMFA.exe";
                    return _helixMFAPath;
                }
            }
            return _helixMFAPath;
        }

        public string GetFileVersion(FileReference tempFile, P4.FileSpec fileSpec)
		{
			if ((Offline) || (fileSpec == null))
			{
				return null;
			}
			IList<string> fileContents = null;
			try
			{
                fileContents = Connection.Repository.GetFileContents(
						new P4.GetFileContentsCmdOptions(P4.GetFileContentsCmdFlags.None, tempFile), fileSpec);
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
            P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
			if (DialogResult.Ignore !=  P4ErrorDlg.Show(results, false))
			{
				return null;
			}

            try
            {
                System.IO.File.SetAttributes(tempFile, System.IO.FileAttributes.ReadOnly);
            }
            catch
            {
                // file doesn't exist, likely deleted at the revision specified
                return null;
            }
			// if the FileSpec passed in does not have the depot path, grab it from 
			// the data returned by the 'print' command.
			if ((fileSpec.DepotPath) == null && (fileContents[0] != null))
			{
				fileSpec.DepotPath = new P4.DepotPath(fileContents[0]);
			}
			return fileContents[0];
		}

		/// <summary>
		/// Run the diff from a background thread so it can delete the temp file 
		/// after the diff program is closed
		/// </summary>
		/// <param name="param"></param>
		public void DiffFileThreadProc(object param)
		{
			try
			{
				bool useP4Merge = Preferences.LocalSettings.GetBool("P4Diff_app", true);

				string diffPath = null;

				if (useP4Merge)
				{
					// use configured application
					diffPath = P4MergePath();
					if (string.IsNullOrEmpty(diffPath) || !File.Exists(diffPath))
					{
                        logger.Warn("Cannot locate P4Merge");
                        return;
					}
				}
				else
				{
					diffPath = Preferences.LocalSettings.GetString("P4Diff_path", string.Empty);
					if (string.IsNullOrEmpty(diffPath) || !File.Exists(diffPath))
					{
						MessageBox.Show(Resources.P4ScmProvider_CannotFindConfiguredDiffApp, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}

				if (diffPath != null)
				{
					string localFile = param as string;

					if (localFile == null)
					{
						return;
					}
					if (!FileCache.ContainsKey(localFile))
					{
						AddFileToCache(localFile);
					}
					using (TempFile tempFile = new TempFile())
					{
						scmFile depotFile = FileCache.Get(localFile);
						P4.FileSpec fs = (P4.FileSpec)depotFile;
						if ((depotFile == null) || (fs == null))
						{
							return;
						}

						if (depotFile.MovedFile != null)
						{
							fs = P4.FileSpec.DepotSpec(depotFile.MovedFile.Path, depotFile.MovedRev);
						}
						if (GetFileVersion(tempFile, fs) != null) 
						{
							string args = string.Empty;

							if (useP4Merge)
							{
								args = string.Format("-nl \"{0} (depot file)\" -nr \"{2} (local file)\" \"{1}\" \"{2}\"",
														((P4.FileSpec)depotFile).ToString(),
														(string)tempFile, localFile);
							}
							else
							{
								args = Preferences.LocalSettings.GetString("Diff_args", string.Empty);

								args = args.Replace("%1", "\"{0}\"");
								args = args.Replace("%2", "\"{1}\"");

								args = string.Format(args, (string)tempFile, localFile);
							}

							Process launchP4Merge = new Process();

							launchP4Merge.StartInfo.FileName = diffPath;
							launchP4Merge.StartInfo.Arguments = args;
							launchP4Merge.StartInfo.CreateNoWindow = true;
							launchP4Merge.StartInfo.UseShellExecute = true;
							launchP4Merge.Start();

							launchP4Merge.WaitForExit();

							launchP4Merge.Dispose();
						}
					}
				}
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
				// ignore thread aborts
				return;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return;
			}
		}

		public class Diff2FileThreadProcParameters
		{
			public object File1 = null;
			public string Label1 = null;
			public object File2 = null;
			public string Label2 = null;
		}
		/// <summary>
		/// Run the diff from a background thread so it can delete the temp file(s)
		/// after the diff program is closed
		/// </summary>
		/// <param name="param"></param>
		public void Diff2FileThreadProc(object param)
		{
			try
			{
				bool useP4Merge = Preferences.LocalSettings.GetBool("P4Diff_app", true);

				string diffPath = null;

				if (useP4Merge)
				{
					// use configured application
					diffPath = P4MergePath();
					if (string.IsNullOrEmpty(diffPath) || !File.Exists(diffPath))
					{
                        logger.Warn("Cannot locate P4Merge");
                        return;
					}
				}
				else
				{
					diffPath = Preferences.LocalSettings.GetString("P4Diff_path", string.Empty);
					if (string.IsNullOrEmpty(diffPath) || !File.Exists(diffPath))
					{
						MessageBox.Show(Resources.P4ScmProvider_CannotFindConfiguredDiffApp, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}

				if (diffPath != null)
				{
					Diff2FileThreadProcParameters fileData = param as Diff2FileThreadProcParameters;
					if (fileData == null)
					{
						return;
					}

					FileReference tempFile1 = null;
					FileReference tempFile2 = null;
					try
					{
						string path1 = null;
						string path2 = null;


						if (fileData.File1 is P4.FileSpec)
						{
							// file in depot
							P4.FileSpec fs = fileData.File1 as P4.FileSpec;
							if ((fs.LocalPath != null) && (fs.Version == null))
							{
								// this is a local path
								path1 = fs.LocalPath.Path;
							}
							else
							{
								// get it from the depot
								tempFile1 = new TempFile();
								if (GetFileVersion(tempFile1, fs) == null)
								{
									return;
								}
								path1 = (string)tempFile1;
							}
							if (fileData.Label1 == null)
							{
								fileData.Label1 = fs.ToString();
							}
						}
						else if ((fileData.File1 is string) || (fileData.File1 is FileReference))
						{
							// local file path
							path1 = fileData.File1.ToString();
							if (fileData.Label1 == null)
							{
								fileData.Label1 = string.Format(Resources.P4ScmProvider_LocalFileLabel, path1);
							}
						}

						if (fileData.File2 is P4.FileSpec)
						{
							// file in depot
							P4.FileSpec fs = fileData.File2 as P4.FileSpec;
							if ((fs.LocalPath != null) && (fs.Version == null))
							{
								// this is a local path
								path2 = fs.LocalPath.Path;
							}
							else
							{
								tempFile2 = new TempFile();
								if (GetFileVersion(tempFile2, fs) == null)
								{
									return;
								}
								path2 = (string)tempFile2;
							}
							if (fileData.Label2 == null)
							{
								fileData.Label2 = fs.ToString();
							}
						}
						else if ((fileData.File2 is string) || (fileData.File2 is FileReference))
						{
							// local file path
							path2 = fileData.File2.ToString();
							if (fileData.Label2 == null)
							{
								fileData.Label2 = string.Format(Resources.P4ScmProvider_LocalFileLabel, path2);
							}
						}

						if ((path1 == null) || (path2 == null))
						{
							return;
						}
						string args = string.Empty;

						if (useP4Merge)
						{
							args = string.Format("-nl \"{2}\" -nr \"{3}\" \"{0}\" \"{1}\"",
													path1, path2, fileData.Label1, fileData.Label2);
						}
						else
						{
							args = Preferences.LocalSettings.GetString("Diff_args", string.Empty);

							args = args.Replace("%1", "\"{0}\"");
							args = args.Replace("%2", "\"{1}\"");

							args = string.Format(args, path1, path2);
						} 

						Process launchP4Merge = new Process();

						launchP4Merge.StartInfo.FileName = diffPath;
						launchP4Merge.StartInfo.Arguments = args;
						launchP4Merge.StartInfo.CreateNoWindow = true;
						launchP4Merge.StartInfo.UseShellExecute = true;
						launchP4Merge.Start();

						launchP4Merge.WaitForExit();

						launchP4Merge.Dispose();
					}
					catch (Exception ex)
					{
						ShowException(ex);
						return;
					}
					finally
					{
						if (tempFile1 != null)
						{
							tempFile1.Dispose();
						}
						if (tempFile1 != null)
						{
							tempFile1.Dispose();
						}
					}
				}
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
				// ignore thread aborts
				return;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return;
			}
		}


		public bool DiffFiles(params string[] files)
		{
            List<ListViewItem> identicals = new List<ListViewItem>();

			if (Offline)
			{
				return false;
			}

			try
			{
				foreach (string localFile in files)
				{
					if (GetFileStatus(localFile).TestNone(SourceControlStatus.scsUncontrolled | SourceControlStatus.scsIgnored))
					{
						if (File.Exists(localFile) != false)
						{
							string[] args = new string[1];
							args[0] = localFile;
							P4.Options options = new P4.Options();
							options["-f"] = null;

                            P4.P4Command cmd = new P4.P4Command(Connection.Repository.Connection, "diff", true, args);
							P4.P4CommandResult results = cmd.Run(options);

							if (results.ErrorList != null)
							{
								MessageBox.Show(string.Format(Resources.P4ScmProvider_ErrorRunningServerDiff, results.ErrorList[0].ErrorMessage), Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
								continue;
							}
							if (results.TextOutput == null)
							{
                                ListViewItem identicalPair = new ListViewItem(localFile);
                                string depotFile;
                                results.TaggedOutput[0].TryGetValue("depotFile", out depotFile);
                                identicalPair.SubItems.Add(depotFile);
                                identicals.Add(identicalPair);

								continue;
							}
							ParameterizedThreadStart pts = new ParameterizedThreadStart(DiffFileThreadProc);

							Thread diffThread = new Thread(pts);
							diffThread.IsBackground = true;
							diffThread.Start(localFile);
						}
						else
						{
							string msg = string.Format(Resources.P4ScmProvider_CantDiffMissingLocalFile, localFile);
							MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
					else
					{
						string msg = string.Format(Resources.P4ScmProvider_CantDiffMissingDepotFile, localFile);
						MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
            if (identicals != null && identicals.Count > 0)
            {
                string message = string.Empty;
                if (identicals.Count > 1)
                {
                    message = string.Format(Resources.NoDiffsDlg_MultiFile, identicals.Count);
                }
                else
                {
                    message = string.Format(Resources.NoDiffsDlg_SingleFile, identicals.Count);
                }
                NoDiffsDlg dlg = new NoDiffsDlg(message, identicals);
                dlg.ShowDialog();
            }
			return true;
		}

		public bool Diff2Files(IList<P4.FileSpec> files)
		{
			return Diff2Files(files[0], null, files[1], null);
		}

		public bool Diff2Files(object file1, string label1, object file2, string label2)
		{
			if (Offline)
			{
				return false;
			}

			try
			{
				P4.FileSpec spec1 =file1 as P4.FileSpec;
				P4.FileSpec spec2 = file2 as P4.FileSpec;
				if ((spec1 != null) && (spec2 != null))
				{
					string topFile = file1.ToString();
					string bottomFile = file2.ToString();
					if (topFile.StartsWith("\\"))
					{
						topFile = topFile.Replace("\\", "/");
					}
					if (bottomFile.StartsWith("\\"))
					{
						bottomFile = bottomFile.Replace("\\", "/");
					}

                    if (spec1.Version == null)
					{
						string[] args = new string[1];
						args[0] = bottomFile;
						P4.Options options = new P4.Options();
						options["-f"] = null;
						options["-se"] = null;

                        P4.P4Command cmd = new P4.P4Command(Connection.Repository.Connection, "diff", true, args);
						P4.P4CommandResult results = cmd.Run(options);

						if (results.ErrorList != null)
						{
							MessageBox.Show(
								string.Format(Resources.P4ScmProvider_ErrorRunningServerDiff,results.ErrorList[0].ErrorMessage), 
								Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
							return false;
						}
						if (results.TaggedOutput != null && results.TaggedOutput[0].ContainsValue("identical"))
						{
							MessageBox.Show(Resources.P4ScmProvider_FilesAreIdentical, Resources.P4VS, 
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							return false;
						}
                        if (results.TaggedOutput ==null)
                        {
                            MessageBox.Show(Resources.P4ScmProvider_FilesAreIdentical, Resources.P4VS,
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
					}
					if (spec2.Version == null)
					{
						string[] args = new string[1];
						args[0] = topFile;
						P4.Options options = new P4.Options();
						options["-f"] = null;
						options["-se"] = null;

                        P4.P4Command cmd = new P4.P4Command(Connection.Repository.Connection, "diff", true, args);
						P4.P4CommandResult results = cmd.Run(options);

						if (results.ErrorList != null)
						{
							MessageBox.Show(string.Format(Resources.P4ScmProvider_ErrorRunningServerDiff, results.ErrorList[0].ErrorMessage), 
								Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
							return false;
						}
						if (results.TaggedOutput != null && results.TaggedOutput[0].ContainsValue("identical"))
						{
							MessageBox.Show(Resources.P4ScmProvider_FilesAreIdentical, Resources.P4VS,
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							return false;
						}
                        if (results.TaggedOutput == null)
                        {
                            MessageBox.Show(Resources.P4ScmProvider_FilesAreIdentical, Resources.P4VS,
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
					}

					if (topFile.StartsWith("//") && bottomFile.StartsWith("//"))
					{
                        Connection.Repository.GetDepotFileDiffs(topFile, bottomFile, null);
                        P4.P4CommandResult results2 = Connection.Repository.Connection.LastResults;
						if (results2.ErrorList != null)
						{
							MessageBox.Show(string.Format(Resources.P4ScmProvider_ErrorRunningServerDiff, results2.ErrorList[0].ErrorMessage),
								Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
							return false;
						}
						if (results2.TaggedOutput != null && results2.TaggedOutput[0].ContainsValue("identical"))
						{
							MessageBox.Show(Resources.P4ScmProvider_FilesAreIdentical, Resources.P4VS,
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							return false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}

			try
			{
				string mergePath = null;

				if (Preferences.LocalSettings.GetBool("P4Diff_app", true))
				{
					// use configured application
					mergePath = P4MergePath();
					if (string.IsNullOrEmpty(mergePath) || !File.Exists(mergePath))
					{
                        logger.Warn("Cannot locate P4Merge");
                        return false;
					}
				}
				else
				{
					mergePath = Preferences.LocalSettings.GetString("P4Diff_path", string.Empty);
					if (string.IsNullOrEmpty(mergePath) || !File.Exists(mergePath))
					{
						MessageBox.Show(Resources.P4ScmProvider_CannotFindConfiguredDiffApp, 
							Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}

				if (mergePath != null)
				{
					ParameterizedThreadStart pts = new ParameterizedThreadStart(Diff2FileThreadProc);

					Thread diffThread = new Thread(pts);
					diffThread.IsBackground = true;

					Diff2FileThreadProcParameters param = new Diff2FileThreadProcParameters();
                    param.File1 = file1;
                    param.Label1 = label1;
                    param.File2 = file2;
                    param.Label2 = label2;

					diffThread.Start(param);
				}
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
			return true;
		}

		public bool SyncFile(P4.Options options, string file)
		{
			if (Offline)
			{
				return false;
			}
			try
			{
				IDictionary<int, object> changelistIds = null;
				if ((options == null) || (options.ContainsKey("-n") == false))
				{
					changelistIds = MapChanges(file);
				}
                IList<P4.FileSpec> f = Connection.Repository.Connection.Client.SyncFiles(P4.FileSpec.DepotSpecList(file), options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				if (changelistIds != null)
				{
					BroadcastChangelistUpdates(changelistIds, ChangelistUpdateArgs.UpdateType.ContentUpdate);
				}
				return results.Success && (f != null) && (f.Count > 0);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return true;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
		}

		public bool SyncFiles(P4.Options options, params string[] files)
		{
			IList<P4.FileSpec> filesChanged;
			return SyncFiles(out filesChanged, options, files);
		}

		public bool SyncFiles(out IList<string> filesChanged, P4.Options options, params string[] files)
		{
			filesChanged = null;
			IList<P4.FileSpec> filespecs;

			bool ret = SyncFiles(out filespecs, options, files);

			if (filespecs != null)
			{
				filesChanged = new List<string>();
				foreach (P4.FileSpec fs in filespecs)
				{
					if (fs.LocalPath != null)
					{
						filesChanged.Add(fs.LocalPath.Path);
					}
					else if (fs.DepotPath != null)
					{
						filesChanged.Add(fs.DepotPath.Path);
					}
				}
			} 
			return ret;
		}

		public bool SyncFiles(out IList<P4.FileSpec> filesChanged, P4.Options options, params string[] files)
		{
			filesChanged = null;
			if (Offline)
			{
				return false;
			}
			try
			{
                if (Preferences.LocalSettings.GetBool("DisableParallelSync", false))
                {
                    if (options==null)
                    {
                        options = new Options();
                    }
                    options["--parallel"] = "0";
                }
                filesChanged = Connection.Repository.Connection.Client.SyncFiles(P4.FileSpec.DepotSpecList(files), options);

                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				if ((results.InfoOutput != null) && (results.InfoOutput.Count > 0))
				{
					foreach (P4.P4ClientInfoMessage l in results.InfoOutput)
					{
						if (l.Message.Contains("must resolve"))
						{
							int idx = l.Message.IndexOf(" - ");		
							string path = l.Message.Substring(0, idx);
							if (path.StartsWith("//"))
							{
								//depot path
                                IList<P4.FileMetaData> f = Connection.Repository.GetFileMetaData(null, P4.FileSpec.DepotSpec(path));
								if ((f != null) && (f.Count > 0) && (f[0].LocalPath != null))
								{
									path = f[0].LocalPath.Path;
								}
							}
							if (filesChanged == null)
							{
								filesChanged = new List<P4.FileSpec>();
							}
							filesChanged.Add(P4.FileSpec.LocalSpec(path));
						}
					}
				}
				IDictionary<int, object> changelistIds = null;
				if ((filesChanged != null) && ((options == null) || (options.ContainsKey("-n") == false)))
				{
					changelistIds = MapChanges(filesChanged);
				}
				P4ErrorDlg.Show(results, false);

				if (changelistIds != null)
				{
					BroadcastChangelistUpdates(changelistIds, ChangelistUpdateArgs.UpdateType.ContentUpdate);
				}
				return results.Success;
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return true;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
		}

		public bool SyncFiles(P4.Options options, IList<P4.FileSpec> files)
		{
			if (Offline)
			{
				return false;
			}
			try
			{
                if (Preferences.LocalSettings.GetBool("DisableParallelSync", false))
                {
                    if (options == null)
                    {
                        options = new Options();
                    }
                    options["--parallel"] = "0";
                }
                IDictionary<int, object> changelistIds = null;
				if ((options == null) || (options.ContainsKey("-n") == false))
				{
					changelistIds = MapChanges(files);
				}
                IList<P4.FileSpec> f = Connection.Repository.Connection.Client.SyncFiles(options, files.ToArray());
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);

				if (changelistIds != null)
				{
					BroadcastChangelistUpdates(changelistIds, ChangelistUpdateArgs.UpdateType.ContentUpdate);
				}
				return results.Success;
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
		}

		public void ResetIgnoredFilesMap()
		{
			if (FileCache.IgnoredFilesMap != null)
			{
				FileCache.IgnoredFilesMap.Clear();
			}
		}

		public void ResetIgnoredFilesMap(string file)
		{
			if ((FileCache.IgnoredFilesMap != null) && (FileCache.IgnoredFilesMap.ContainsKey(file)))
			{
				FileCache.IgnoredFilesMap.Remove(file);
			}
		}

		public static bool P4IgnoreSet
		{
			get
			{
				try
				{
					return string.IsNullOrEmpty(P4.P4Server.Get("P4IGNORE")) == false;
				}
				catch (P4Exception)
				{
					return false;
				}
			}
		}
		public static string P4Ignore
		{
			get
			{
				return P4.P4Server.Get("P4IGNORE");
			}
			set
			{
				P4.P4Server.Set("P4IGNORE", value);
			}
		}

		private object _isFileIgnoredSyncLoc = new object();

		public bool isFileIgnored(string file)
		{
			if ((Offline) || (file == null) || (P4IgnoreSet == false))
			{
				return false;
			}
			if (FileCache.IgnoredFilesMap.ContainsKey(file))
			{
				return FileCache.IgnoredFilesMap[file];
			}
			lock (_isFileIgnoredSyncLoc)
			{
				try
				{
                    if (file.StartsWith(Connection.WorkspaceRoot, StringComparison.OrdinalIgnoreCase) == false)
					{
						return false;
					}
#if USE_P4IGNORE
                    return Connection.Repository.Connection.IsFileIgnored(file);
#else
					P4.P4CommandResult results = null;

					P4.Options options = new P4.Options();

					options["-f"] = null;

					options["-n"] = null;

					P4.P4Command addCmd = Repository.Connection.CreateCommand("add", false, file);

					results = addCmd.Run(options);

					if (results.Success)
					{
						if (results.InfoOutput != null)
						{
							foreach (P4.InfoLine info in results.InfoOutput)
							{
								if (info.Info.Contains("ignored file can't be added"))
								{
									FileCache.IgnoredFilesMap.Add(file, true);
									return true;
									;
								}
							}
						}
						if (results.ErrorList != null)
						{
							foreach (P4.P4ClientError error in results.ErrorList)
							{
								if (error.ErrorMessage.Contains("ignored file can't be added"))
								{
									FileCache.IgnoredFilesMap.Add(file, true);
									return true;
								}
							}
						}
						FileCache.IgnoredFilesMap.Add(file, false);
						return false;
					}
					return false;
#endif
				}
				catch (ThreadAbortException)
				{
					// ignore thread aborts
					return false;
				}
				catch (Exception)
				{
					// Background operation so don't show errors

					// If the error is because the repository is now null, it means
					// the connection was closed in the middle of a command, so ignore it.
					return false;
				}
			}
		}

        public bool IsFileExclusiveOpened(string fileName)
        {
            try
            {
                CachedFile f = Fetch(fileName);
                if (f != null)
                {
                    if (f._file.HeadType.Modifiers.HasFlag(FileTypeModifier.ExclusiveOpen) &&
                        f.ScmStatus.Flags.HasFlag(SourceControlStatus.scsOtherCheckedOut))

                    {
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }
		public SourceControlStatus GetFileStatus(string fileName)
		{
			SourceControlStatus status = SourceControlStatus.scsUncontrolled;

			if (Offline)
			{
				return status;
			}

			if (EnforceLocalIgnore)
			{
				if (isFileIgnored(fileName))
				{
					// file was ignored
					status |= SourceControlStatus.scsIgnored;
					return status;
				}
			}
			try
			{
				CachedFile f = Fetch(fileName);
				if (f != null)
				{
					status = f.ScmStatus;
				}
				else if ((Contains(fileName) == false) &&
					(Preferences.LocalSettings.GetBool("LazyLoadStatus", false)))
				{
					status = SourceControlStatus.scsUnknown;
				}
			}
			catch (Exception)
			{
				return SourceControlStatus.scsUncontrolled;
			}
			if (status == SourceControlStatus.scsUncontrolled)
			{
				if (isFileIgnored(fileName))
				{
					// file was ignored
					status |= SourceControlStatus.scsIgnored;
				}
			}
			return status;
		}

		public string GetToolTip(string fileName)
		{
			if (Offline)
			{
				return null;
			}
			// Initialize output parameters
			try
			{
				P4.FileMetaData f = null;
				if (Preferences.LocalSettings.GetBool("AutoUpdateFileData", false))
				{
					f = UpdateFileInCache(fileName, false);
				}
				else
				{
					f = Fetch(fileName);
				}
				if (f == null)
				{
					if (Preferences.LocalSettings.GetBool("LazyLoadStatus", false) && (Contains(fileName) == false))
					{
						return Resources.P4ScmProvider_ToolTipLazyLoad;
					}
					if (P4IgnoreSet && isFileIgnored(fileName))
					{
						return Resources.P4ScmProvider_ToolTipIgnoredFile;
					}
					return Resources.P4ScmProvider_ToolTipNotInPerforce;
				}
				string TooltipText = string.Empty;

				if (f.Unresolved)
				{
					TooltipText += Resources.P4ScmProvider_ToolTipNeedsResolve;
				}
				if (f.HeadAction == P4.FileAction.Delete)
				{
					TooltipText += Resources.P4ScmProvider_ToolTipDeletedAtHeadRevision;
				}
				P4.FileType ft = f.HeadType;
				if (f.Type!=null)
				{
					ft =f.Type;
				}
				int headRev = f.HeadRev >= 0 ? f.HeadRev : 0;
				int haveRev = f.HaveRev >= 0 ? f.HaveRev : 0;
				TooltipText += String.Format(Resources.P4ScmProvider_ToolTipFileRevisionAndType, haveRev, headRev, ft);

				string users = String.Empty;

				if (f.Action != P4.FileAction.None)
				{
					users = string.Format(Resources.P4ScmProvider_ToolTipSelfAction, 
						System.Enum.GetName(typeof(P4.FileAction), f.Action), 
						(f.Change > 0) ? f.Change.ToString() : Resources.Changelist_Default);
				}
				if (f.OurLock)
					users += Resources.P4ScmProvider_ToolTipLocked;

				if (f.OtherOpen > 0)
				{
					int cnt = 0;
					foreach (P4.OtherFileUser ofi in f.OtherUsers.Values)
					{
						if (cnt < 2)
						{
							if (!String.IsNullOrEmpty(users))
								users += ", ";
							users += string.Format(Resources.P4ScmProvider_ToolTipOtherAction, 
								ofi.Client, System.Enum.GetName(typeof(P4.FileAction), ofi.Action),
								(f.OtherChanges[cnt] > 0) ? f.OtherChanges[cnt].ToString() : Resources.Changelist_Default);
							if (ofi.hasLock)
							{
								users += Resources.P4ScmProvider_ToolTipLocked;
							}
						}
						else
						{
							users += ",...";
							break;
						}
					}
				}

				if (!string.IsNullOrEmpty(users))
				{
					TooltipText += string.Format(Resources.P4ScmProvider_ToolTipOpenedBy, users);
				}
				else
				{
					TooltipText += Resources.P4ScmProvider_ToolTipCheckedIn;
				}
				return TooltipText;
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return string.Empty;
			}
			catch { }

			return string.Empty;
		}

		public P4.Client getClient(string client, P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			P4.Client value = null;
			try
			{
                value = Connection.Repository.GetClient(client, options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public IList<P4.Client> getClients(P4.ClientsCmdFlags flags, string owner, string nameFilter, int maxItems, string stream)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.Client> value = null;
			try
			{
                value = Connection.Repository.GetClients(new P4.Options(flags, owner, nameFilter, maxItems, stream));
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.Client createClient(P4.Client client, P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			P4.Client value = null;
			try
			{
                value = Connection.Repository.CreateClient(client, options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				ShowException(ex);
				return null;
			}
			return value;
		}

		public string RenameClient(string oldClientName, string newClientName)
		{
			string result ;
            if (Offline)
            {
                return string.Empty;
            }

            result = Connection.Repository.RenameClient(oldClientName, newClientName);
          
            return result;

        }
		public void deleteClient(P4.Client client, P4.Options options)
		{
			if (Offline)
			{
				return;
			}
			try
			{
                Connection.Repository.DeleteClient(client, options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return;
			}
		}

		public IList<P4.Job> getJobs(P4.JobsCmdFlags flags, string keywords, int maxItems, P4.FileSpec path)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.Job> value = null;
			try
			{
                value = Connection.Repository.GetJobs((new P4.Options(flags, keywords, maxItems)), path);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.Job getJob(string job)
		{
			if (Offline)
			{
				return null;
			}
			P4.Job value = null;
			try
			{
                value = Connection.Repository.GetJob(job);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore ti.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

        public P4.Job createJob(P4.Job job)
        {
            if (Offline)
            {
                return null;
            }
            P4.Job value = null;
            try
            {
                value = Connection.Repository.CreateJob(job);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
                P4ErrorDlg.Show(results, false);
            }
            catch (ThreadAbortException)
            {
                // ignore thread aborts
                return null;
            }
            catch (Exception ex)
            {
                // If the error is because the repository is now null, it means
                // the connection was closed in the middle of a command, so ignore ti.
                if (Connection.Repository != null)
                {
                    ShowException(ex);
                }
                return null;
            }
            return value;
        }

        public P4.Job saveJob(P4.Job job)
        {
            if (Offline)
            {
                return null;
            }
            P4.Job value = null;
            try
            {
                value = Connection.Repository.UpdateJob(job);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
                P4ErrorDlg.Show(results, false);
            }
            catch (ThreadAbortException)
            {
                // ignore thread aborts
                return null;
            }
            catch (Exception ex)
            {
                // If the error is because the repository is now null, it means
                // the connection was closed in the middle of a command, so ignore ti.
                if (Connection.Repository != null)
                {
                    ShowException(ex);
                }
                return null;
            }
            return value;
        }

		public List<KeyValuePair<string, string>> GetClientSpec()
		{
            P4.P4Command cmd = Connection.Repository.Connection.CreateCommand("spec", true, "-o", "client");
			P4.P4CommandResult results = cmd.Run();
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			if (results.TaggedOutput != null)
			{
				foreach (KeyValuePair<string, string> kvp in results.TaggedOutput[0])
				{
					list.Add(kvp);
				}
			}
			return list;
		}

		public List<KeyValuePair<string, string>> GetChangeSpec()
		{
            P4.P4Command cmd = Connection.Repository.Connection.CreateCommand("spec", true, "-o", "change");
			P4.P4CommandResult results = cmd.Run();
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			if (results.TaggedOutput != null)
			{
				foreach (KeyValuePair<string, string> kvp in results.TaggedOutput[0])
				{
					list.Add(kvp);
				}
			}
			return list;
		}
		
		public P4.Stream GetStream(string stream, string parent, P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			P4.Stream value = null;
			try
			{
                value = Connection.Repository.GetStream(stream, parent, options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public IList<P4.Stream> GetStreams(P4.Options options, params P4.FileSpec[] files)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.Stream> value = null;
			try
			{
                value = Connection.Repository.GetStreams(options, files);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore ti.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

        public StreamMetaData GetStreamMetaData(P4.Stream stream,P4.Options options )
        {
            if (Offline)
            {
                return null;
            }
            StreamMetaData value = null;
            try
            {
                value = Connection.Repository.GetStreamMetaData(stream, options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
                P4ErrorDlg.Show(results, false);
            }
            catch (ThreadAbortException)
            {
                // ignore thread aborts
                return null;
            }
            catch (Exception ex)
            {
                // If the error is because the repository is now null, it means
                // the connection was closed in the middle of a command, so ignore ti.
                if (Connection.Repository != null)
                {
                    ShowException(ex);
                }
                return null;
            }
            return value;
        }

		public IList<P4.File> GetOpenedFiles(IList<P4.FileSpec> filespecs, P4.Options options)
		{
			IList<P4.File> value = null;
			try
			{
                value = Connection.Repository.GetOpenedFiles(filespecs, options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore ti.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		/// <summary>
		/// Get the changelist available for this user/client to use to open files
		/// </summary>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		public IList<P4.Changelist> GetAvailibleChangelists(int maxItems)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.Changelist> value = null;
			try
			{
                value = Connection.Repository.GetChangelists((new P4.Options(
					P4.ChangesCmdFlags.FullDescription,
                    Connection.Repository.Connection.Client.Name, 
					maxItems,
					P4.ChangeListStatus.Pending,
                    Connection.Repository.Connection.UserName)));
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public IList<P4.Changelist> GetChangelists(P4.ChangesCmdFlags flags, string workspace, int maxItems, P4.ChangeListStatus status, string user, P4.FileSpec path)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.Changelist> value = null;
			try
			{
                value = Connection.Repository.GetChangelists((new P4.Options(flags, workspace, maxItems, status, user)), path);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.Changelist GetChangelist(int changeId, P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			P4.Changelist value = null;
			try
			{
                value = Connection.Repository.GetChangelist(changeId, options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.Changelist GetChangelist(int changeId)
		{
			return GetChangelist(changeId, null);
		}

        public P4.Changelist SaveChangelist(P4.Changelist change, P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			P4.Changelist value = null;
			try
			{
				if (change.Id <=0)
				{
                    value = Connection.Repository.CreateChangelist(change);
				}
				else
				{
                    value = Connection.Repository.UpdateChangelist(change);
				}
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.Changelist SaveSubmittedChangelist(P4.Changelist change)
		{
			if (Offline)
			{
				return null;
			}
			P4.Changelist value = null;
			try
			{
                Connection.Repository.UpdateSubmittedChangelist(change, null);

                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.Changelist CreateChangelist(string description)
		{
			if (Offline)
			{
				return null;
			}
			P4.Changelist change = null;
			try
			{
				change = Connection.Repository.NewChangelist();
                change.Description = description;
                Connection.Repository.CreateChangelist(change);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return change;
		}

		public P4.Changelist DeleteChangelist(P4.Changelist change)
		{
			if (Offline)
			{
				return null;
			}
			P4.Changelist value = null;
			try
			{
				P4.Options opts = new P4.Options();
                Connection.Repository.DeleteChangelist(change, opts);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public IList<P4.FileSpec> IntegrateFiles(IList<P4.FileSpec> toFiles, P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.FileSpec> value = null;
			try
			{
                value = Connection.Repository.Connection.Client.IntegrateFiles(toFiles, options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public IList<P4.FileSpec> MergeFiles(P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.FileSpec> value = null;
			try
			{
                value = Connection.Repository.Connection.Client.MergeFiles(options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				int changelistID = 0;
				if (options.ContainsKey("-c"))
				{
					changelistID = Convert.ToInt32((options["-c"]));
				}
                if (value != null)
                {
                    BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changelistID, ChangelistUpdateArgs.UpdateType.ContentUpdate));
                }
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch
			{
				return null;
			}
			return value;
		}

		public IList<P4.FileSpec> CopyFiles(P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.FileSpec> value = null;
			try
			{
                value = Connection.Repository.Connection.Client.CopyFiles(options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				int changelistID = 0;
				if (options.ContainsKey("-c"))
				{
					changelistID = Convert.ToInt32((options["-c"]));
				}
                if (value!=null)
                {
                    BroadcastChangelistUpdate(this, new ChangelistUpdateArgs(changelistID, ChangelistUpdateArgs.UpdateType.ContentUpdate));
                }
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch
			{
				return null;
			}
			return value;
		}

		public IList<P4.FileSpec> ReopenFiles(IList<P4.FileSpec> files, P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.FileSpec> value = null;
			try
			{
                value = Connection.Repository.Connection.Client.ReopenFiles(files, options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public IList<P4.FileSpec> EditFiles(IList<P4.FileSpec> files, P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.FileSpec> value = null;
			try
			{
                value = Connection.Repository.Connection.Client.EditFiles(files, options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.ClientMetadata GetClientMetadata()
		{
			if (Offline)
			{
				return null;
			}
            return Connection.Repository.GetClientMetadata();
		}

		P4.ServerMetaData ServerMetaData = null;

		public P4.ServerMetaData GetServerMetaData()
		{
			if (ServerMetaData != null)
			{
				return ServerMetaData;
			}
			if (Offline)
			{
				return null;
			}
            ServerMetaData = Connection.Repository.GetServerMetaData(null);
			return ServerMetaData;
		}

        public IList<P4.Group> GetGroups(IList<string> groups, P4.Options options)
        {
            if (Offline)
            {
                return null;
            }
            IList<P4.Group> value = null;
            try
            {
                if (groups != null)
                {
                    value = Connection.Repository.GetGroups(options, groups.ToArray());
                }
                else
                {
                    value = Connection.Repository.GetGroups(options, null);
                }
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
                P4ErrorDlg.Show(results, false);
            }
            catch (ThreadAbortException)
            {
                // ignore thread aborts
                return null;
            }
            catch (Exception ex)
            {
                // If the error is because the repository is now null, it means
                // the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
                {
                    ShowException(ex);
                }
                return null;
            }
            return value;
        }
        public P4.Group GetGroup(string name)
        {
            if (Offline)
            {
                return null;
            }
            P4.Group value = null;
            try
            {
                value = Connection.Repository.GetGroup(name);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
                P4ErrorDlg.Show(results, false);
            }
            catch (ThreadAbortException)
            {
                // ignore thread aborts
                return null;
            }
            catch (Exception ex)
            {
                // If the error is because the repository is now null, it means
                // the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
                {
                    ShowException(ex);
                }
                return null;
            }
            return value;
        }
        public IList<P4.User> GetUsers(IList<string> users, P4.Options options)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.User> value = null;
			try
			{
				if (users != null)
				{
                    value = Connection.Repository.GetUsers(users, options);
				}
				else
				{
                    value = Connection.Repository.GetUsers(options, null);
				}
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.User GetUser(string name)
		{
			if (Offline)
			{
				return null;
			}
			P4.User value = null;
			try
			{
                value = Connection.Repository.GetUser(name);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.User NewUser(P4.User user)
		{
			if (Offline)
			{
				return null;
			}
			P4.User value = null;
			try
			{
                try
                {
                    value = Connection.Repository.CreateUser(user);
                }
                catch (P4.P4Exception p4ex)
                {
					if (p4ex.ErrorCode == P4.P4ClientError.MsgServer_MustSetPassword)
                    {
                        Connection.Repository.Connection.SetPassword(null, user.Password);
                    }
                    else
                    {
                        throw;
                    }
                }

                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				if ( ex is P4.P4Exception)
                {
					if (P4.P4ClientError.IsBadPasswdError(((P4.P4Exception)ex).ErrorCode))
                    {
                        throw;
                    }
                }

				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
                return null;
			}
			return value;
		}

		public IList<P4.FileHistory> GetFileHistory(string file)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.FileHistory> value = null;
			try
			{
				P4.FilelogCmdOptions options = new P4.FilelogCmdOptions(P4.FileLogCmdFlags.LongOutput, -1, -1);

                value = Connection.Repository.GetFileHistory(options, P4.FileSpec.ClientSpec(file));
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.FileMetaData GetFileMetaData(P4.Options options, string file, out P4.P4CommandResult results)
		{
			results = null;
			if (Offline)
			{
				return null;
			}

			IList<P4.FileMetaData> values = null;
			try
			{
                values = Connection.Repository.GetFileMetaData(options, P4.FileSpec.DepotSpec(file));
                results = Connection.Repository.Connection.LastResults;
			}
			catch 
			{
                results = Connection.Repository.Connection.LastResults;
				return null;
			}
			if ((values != null) && (values.Count > 0))
			{
				return values[0];
			}
			return null;
		}

		public IList<P4.FileMetaData> GetFileMetaData(P4.Options options, P4.FileSpec file)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.FileMetaData> value = null;
			try
			{
                value = Connection.Repository.GetFileMetaData(options, file);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public IList<P4.FileMetaData> ListFileMetaData(P4.Options options, IList<P4.FileSpec> files)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.FileMetaData> value = null;
			try
			{
                value = Connection.Repository.GetFileMetaData(files, options);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public IList<P4.FileMetaData> GetFileMetaData(P4.Options options, string file)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.FileMetaData> value = null;
			try
			{
                value = Connection.Repository.GetFileMetaData(options, P4.FileSpec.DepotSpec(file));
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.FileMetaData GetFileMetaData(string depotFilePath)
		{
			if (Offline)
			{
				return null;
			}
			try
			{
				P4.Options options = new P4.GetFileMetaDataCmdOptions(P4.GetFileMetadataCmdFlags.FileSize, null, null, -1, null, null, null);
                IList<P4.FileMetaData> list = Connection.Repository.GetFileMetaData(options, P4.FileSpec.DepotSpec(depotFilePath));
				if ((list != null))
				{
					return list[0];
				}
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return null;
		}

		public IList<P4.FileMetaData> GetFileMetaData(IList<string> depotFilePaths)
		{
			if (Offline)
			{
				return null;
			}
			try
			{
				P4.Options options = new P4.GetFileMetaDataCmdOptions(P4.GetFileMetadataCmdFlags.FileSize, null, null, -1, null, null, null);
                IList<P4.FileMetaData> list = Connection.Repository.GetFileMetaData(P4.FileSpec.DepotSpecList(depotFilePaths.ToArray()), options);
				if ((list != null))
				{
					return list;
				}
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return null;
		}

		public IList<P4.FileSpec> GetFiles(P4.Options options, IList<P4.FileSpec> files)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.FileSpec> value = null;
			try
			{
                value = Connection.Repository.GetDepotFiles(files, null);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.FileSpec GetFile(P4.Options options, P4.FileSpec file)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.FileSpec> value = null;
			try
			{
				List<P4.FileSpec> list = new List<P4.FileSpec>();
				list.Add(file);
                value = Connection.Repository.GetDepotFiles(list, null);
				if (value == null)
				{
					return null;
				}
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value[0];
		}

		public IList<P4.Depot> GetDepots()
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.Depot> value = null;
			try
			{
                value = Connection.Repository.GetDepots();
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public IList<string> GetDepotDirs(P4.Options options, params string[] dirs)
		{
			if (Offline)
			{
				return null;
			}
			IList<string> value = null;
			try
			{
                value = Connection.Repository.GetDepotDirs(options, (string[])dirs);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public IList<P4.Label> GetLabels(P4.FileSpec file)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.Label> value = null;
			try
			{
                value = Connection.Repository.GetLabels(null, file);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}


		public IList<P4.Label> GetLabels(string LabelFilter, bool MatchCase, string OwnerFilter, string FileFilter, int MaxItems)
		{
			if (Offline)
			{
				return null;
			}
			IList<P4.Label> value = null;
			try
			{
				P4.LabelsCmdFlags flags = P4.LabelsCmdFlags.None;

				P4.LabelsCmdOptions options = new P4.LabelsCmdOptions(
					flags, OwnerFilter, null,
					MaxItems, null);

				if (LabelFilter != null)
				{
					if (MatchCase)
					{
						options["-e"] = LabelFilter;
					}
					else
					{
						options["-E"] = LabelFilter;
					}
				}
				options["-t"] = null;

				P4.FileSpec fs = null;
				if (string.IsNullOrEmpty(FileFilter) == false)
				{
					if (FileFilter.StartsWith("//"))
					{
						// depot path
						fs = P4.FileSpec.DepotSpec(FileFilter);
					}
					else
					{
						fs = P4.FileSpec.LocalSpec(FileFilter);
					}
				}
                value = Connection.Repository.GetLabels(options, fs);
                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public P4.Label GetLabel(string LabelId)
		{
			if (Offline)
			{
				return null;
			}
			P4.Label value = null;
			try
			{
                value = Connection.Repository.GetLabel(LabelId);

                P4.P4CommandResult results = Connection.Repository.Connection.LastResults;
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public string GetProperty(string PropertyName)
		{
			if ((Offline) || (ServerVersion < Versions.V13_1))
			{
				return null;
			}
			string value = null;
			try
			{
                P4Command propCmd = new P4Command(Connection.Repository, "property", true, null);

				StringList flags = new StringList(new string[] {"-l", "-n", "*"});
				flags[2] = PropertyName;
				P4CommandResult results = propCmd.Run(flags);

				value = results.TaggedOutput[0]["value"];

				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public IList<string> GetProperties(string PropertyName)
		{
			if ((Offline) || (ServerVersion < Versions.V13_1))
			{
				return null;
			}
			IList<string> value = null;
			try
			{
                P4Command propCmd = new P4Command(Connection.Repository, "property", true, null);

				StringList flags = new StringList(new string[] {"-l", "-n", "*"});
				flags[2] = PropertyName;
				P4CommandResult results = propCmd.Run(flags);

				if ((results.TaggedOutput != null) && (results.TaggedOutput.Count > 0))
				{
					value = new List<string>();

					foreach (TaggedObject obj in results.TaggedOutput)
					{
						value.Add(obj["value"]);
					}
				}
				P4ErrorDlg.Show(results, false);
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return null;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return null;
			}
			return value;
		}

		public bool MergeLocalFiles(string baseFile, string file1, string file2, string mergedFile)
		{
			string cmdLine = string.Format("merge3 \"{0}\" \"{1}\" \"{2}\"",
				baseFile.Trim('\"'),
				file1.Trim('\"'),
				file2.Trim('\"')
				);

			Process proc = new Process();
			proc.StartInfo = new ProcessStartInfo();

			proc.StartInfo.UseShellExecute = true;
			proc.StartInfo.FileName = "p4";
			proc.StartInfo.Arguments = cmdLine;
			proc.StartInfo.RedirectStandardOutput = true; ;
			proc.StartInfo.CreateNoWindow = true; ;

            try
            {
                proc.Start();

                // TODO: read standard out in blocks so we don't go over the size limit.
                string output = proc.StandardOutput.ReadToEnd();

                proc.WaitForExit();
                using (StreamWriter sw = new StreamWriter(mergedFile))
                {
                    sw.Write(output);
                }
                return true;
            }
            catch
            {
                try
                {
                    File.Delete(mergedFile);
                }
                catch { }// ignore errors if trying to delete the temp file, may not have been created;
                return false;
            }
        }

        public void LaunchHistoryWindow()
        {
            P4VsProvider.Instance.P4VsViewHistoryToolWindowExt();
        }
        public int LaunchHelixMFA(string user, string port)
        {
            string helixMFAPath = HelixMFAPath();
            P4VS.UI.LaunchingHMFA launchDlg;

            if (string.IsNullOrEmpty(helixMFAPath) || !File.Exists(helixMFAPath))
            {
                launchDlg = new P4VS.UI.LaunchingHMFA(false,
                user, port, helixMFAPath);
            }
            else
            {
                launchDlg = new P4VS.UI.LaunchingHMFA(true,
                user, port, helixMFAPath);
            }

            launchDlg.ShowDialog();
            return launchDlg.exitCode;
        }
		public void LaunchP4V(string Path)
		{
			if (Offline)
			{
				return;
			}
			try
			{
				string p4vPath = null;
				if (Preferences.LocalSettings.GetBool("P4V_path", true))
				{
					// use configured application
					p4vPath = P4VPath();
					if (string.IsNullOrEmpty(p4vPath) || !File.Exists(p4vPath))
					{
						MessageBox.Show(Resources.P4ScmProvider_CannotFindP4VError, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
				if (p4vPath != null)
				{
					P4.FileMetaData fmd = Fetch(Path);
					if (fmd != null)
					{
						string fileName = Preferences.LocalSettings["P4V_path"].ToString();
						string version = FileVersionInfo.GetVersionInfo(fileName).ToString();
						string[] filelines = version.Split('\n');
						string line = filelines[3].Replace("FileVersion:", "").Trim();
						line = line.Remove(6);
						double versionnum = Convert.ToDouble(line);

						Process launchP4V = new Process();
						launchP4V.StartInfo.FileName = fileName;
                        string charset = SccService.ScmProvider.Connection.Repository.Connection.CharacterSetName;
						if (charset == null || charset=="")
						{
							charset = string.Empty;
						}
						else
						{
							charset = " -C " + charset;
						}

                        // P4V deprecated options p4v -s and p4v -t in 2024.1 release.
                        // These options needs to be replaced by p4vc -workspacewindow -s / -t
                        if (versionnum >= 2024.1)
                        {
                            string p4vcPath = null;
                            if (Preferences.LocalSettings.GetBool("P4VC_path", true))
                            {
                                // use configured application
                                p4vcPath = P4VCPath();
                                if (string.IsNullOrEmpty(p4vcPath) || !File.Exists(p4vcPath))
                                {
                                    MessageBox.Show(Resources.P4ScmProvider_CannotFindP4VError, Resources.P4VS,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }

                            if (!string.IsNullOrEmpty(p4vcPath))
                            {
                                // Need to reset path from p4v.exe to p4vc.bat to launch P4V.
                                launchP4V.StartInfo.FileName = Preferences.LocalSettings["P4VC_path"].ToString();
                                launchP4V.StartInfo.Arguments = " -p " + Connection.Port + " -u " + Connection.User + " -c " + Connection.Workspace + charset + " workspacewindow -s \"" + Path + "\"";
                                launchP4V.StartInfo.UseShellExecute = false;
                            }
                        }
                        else if (versionnum >= 2010.2)
                        {
                            launchP4V.StartInfo.Arguments = " -p " + Connection.Port + " -u " + Connection.User + " -c " + Connection.Workspace + charset + " -s \"" + Path + "\"";
                        }
                        else
                        {
                            launchP4V.StartInfo.Arguments = " -p " + Connection.Port + " -u " + Connection.User + " -c " + Connection.Workspace + charset + "\"" + Path + "\"";
                        }

						string msg = string.Format("==>{0} {1}", fileName, launchP4V.StartInfo.Arguments);
						P4VsOutputWindow.AppendMessage(msg);

						launchP4V.StartInfo.CreateNoWindow = true;
						launchP4V.Start();

						if (launchP4V.WaitForExit(200) == true)
						{
							// P4V exited too quickly
						}
						launchP4V.Dispose();
						return;
					}
				}
				else
				{
                    MessageBox.Show(string.Format(Resources.P4ScmProvider_DepotPathDoesNotExist, Path, Connection.Port));
				}
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return;
			}
		}
		public void LaunchTimeLapseView(string Path)
		{
			if (Offline)
			{
				return;
			}
		    P4CommandResult results = null;
		    FileMetaData fmd = GetFileMetaData(null, Path, out results);
            if (fmd != null)
            {
                Path = fmd.DepotPath.Path;
            }
			try
			{
				string p4vcPath = null;
				if (Preferences.LocalSettings.GetBool("P4VC_path", true))
				{
					// use configured application
                    p4vcPath = P4VCPath();
                    if (string.IsNullOrEmpty(p4vcPath) || !File.Exists(p4vcPath))
					{
						MessageBox.Show(Resources.P4ScmProvider_CannotFindP4VError, Resources.P4VS, 
							MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
                if (p4vcPath != null)
				{
					Process launchP4VC = new Process();
                    launchP4VC.StartInfo.FileName = Preferences.LocalSettings["P4VC_path"].ToString();
				    string fileName = launchP4VC.StartInfo.FileName;
                    string charset = SccService.ScmProvider.Connection.Repository.Connection.CharacterSetName;
					if (charset == null || charset == "")
					{
						charset = string.Empty;
					}
					else
					{
						charset = " -C " + charset;
					}

                    launchP4VC.StartInfo.Arguments =
                        " -p " + Connection.Port +
                        " -u " + Connection.User +
                        " -c " + Connection.Workspace
                        + charset + " tlv " + "\"" + Path + "\"";

                    string msg = string.Format("==>{0} {1}", fileName, launchP4VC.StartInfo.Arguments);
                    P4VsOutputWindow.AppendMessage(msg);
                    launchP4VC.StartInfo.CreateNoWindow = true;
                    launchP4VC.StartInfo.UseShellExecute = false;
                    launchP4VC.Start();
                    launchP4VC.Dispose();
					return;
				}
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return;
			}
		}

		public void LaunchRevisionGraphView(string Path)
		{
			if (Offline)
			{
				return;
			}
            P4CommandResult results = null;
            FileMetaData fmd = GetFileMetaData(null, Path, out results);
            if (fmd != null)
            {
                Path = fmd.DepotPath.Path;
            }
			try
			{
				string p4vcPath = null;
				if (Preferences.LocalSettings.GetBool("P4VC_path", true))
				{
					// use configured application
                    p4vcPath = P4VCPath();
                    if (string.IsNullOrEmpty(p4vcPath) || !File.Exists(p4vcPath))
					{
						MessageBox.Show(Resources.P4ScmProvider_CannotFindP4VError, Resources.P4VS,
							MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
                if (p4vcPath != null)
				{
					Process launchP4VC = new Process();
                    launchP4VC.StartInfo.FileName = Preferences.LocalSettings["P4VC_path"].ToString();
                    string fileName = launchP4VC.StartInfo.FileName;
					string version = FileVersionInfo.GetVersionInfo(Preferences.LocalSettings["P4VC_path"].ToString()).ToString();
                    string charset = SccService.ScmProvider.Connection.Repository.Connection.CharacterSetName;
					if (charset == null || charset == "")
					{
						charset = string.Empty;
					}
					else
					{
						charset = " -C " + charset;
					}

                    launchP4VC.StartInfo.Arguments =
                        " -p " + Connection.Port +
                        " -u " + Connection.User +
                        " -c " + Connection.Workspace +
                        charset + " revgraph " + "\"" + Path + "\"";

					string msg = string.Format("==>{0} {1}", fileName, launchP4VC.StartInfo.Arguments);
                    P4VsOutputWindow.AppendMessage(msg);
                    launchP4VC.StartInfo.CreateNoWindow = true;
                    launchP4VC.StartInfo.UseShellExecute = false;
                    launchP4VC.Start();
                    launchP4VC.Dispose();
					return;
				}
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return;
			}
		}

		public void LaunchStreamGraph()
		{
			if (Offline)
			{
				return;
			}
			try
			{
				string p4vcPath = null;
				if (Preferences.LocalSettings.GetBool("P4VC_path", true))
				{
					// use configured application
                    p4vcPath = P4VCPath();
                    if (string.IsNullOrEmpty(p4vcPath) || !File.Exists(p4vcPath))
					{
						MessageBox.Show(Resources.P4ScmProvider_CannotFindP4VError, Resources.P4VS,
							MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
                if (p4vcPath != null)
				{
                    P4.Client current = getClient(Connection.Workspace, null);
					string[] stream = current.Stream.Split('/');
					string streamDepot = stream[2];
					Process launchP4VC = new Process();
                    launchP4VC.StartInfo.FileName = Preferences.LocalSettings["P4VC_path"].ToString();
                    string fileName = launchP4VC.StartInfo.FileName;
                    string charset = SccService.ScmProvider.Connection.Repository.Connection.CharacterSetName;
					if (charset == null || charset == "")
					{
						charset = string.Empty;
					}
					else
					{
						charset = " -C " + charset;
					}

                    launchP4VC.StartInfo.Arguments =
                        " -p " + Connection.Port +
                        " -u " + Connection.User +
                        " -c " + Connection.Workspace +
                        charset + " streamgraph " + streamDepot;
					
                    string msg = string.Format("==>{0} {1}", fileName, launchP4VC.StartInfo.Arguments);
                    P4VsOutputWindow.AppendMessage(msg);
                    launchP4VC.StartInfo.CreateNoWindow = true;
                    launchP4VC.Start();
                    launchP4VC.Dispose();
					return;
				}
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return;
			}
		}

		public bool MergeFiles(string SourceFile, string SourcefileLabel,
							   string TargetFile, string TargetFileLabel,
							   string BaseFile, string BaseFileLabel,
							   string ResultFile)
		{
			if (Offline)
			{
				return false;
			}
			try
			{
				string mergePath = null;
				string args = string.Empty;

				if (Preferences.LocalSettings.GetBool("P4Merge_app", true))
				{
					// use configured application
					mergePath = P4MergePath();
					if (string.IsNullOrEmpty(mergePath) || !File.Exists(mergePath))
					{
                        logger.Warn("Cannot locate P4Merge");
                        return false;
					}
					args = string.Format("-nb \"{0}\" -nl \"{1}\" -nr \"{2}\" -nm \"Merge file used for resolve\" -ll \"Source:\" -rl \"Target:\" \"{3}\" \"{4}\" \"{5}\" \"{6}\"",
												BaseFileLabel, SourcefileLabel, TargetFileLabel, BaseFile, SourceFile, TargetFile, ResultFile);
                    logger.Trace("P4Merge " + args);
				}
				else
				{
					mergePath = Preferences.LocalSettings.GetString("P4Merge_path", string.Empty);
					if (string.IsNullOrEmpty(mergePath) || !File.Exists(mergePath))
					{
						MessageBox.Show(Resources.P4ScmProvider_CannotFindConfiguredDiffApp, Resources.P4VS, 
							MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
					args = Preferences.LocalSettings.GetString("Merge_args", string.Empty);

					args = args.Replace("%b", "\"{0}\"");
					args = args.Replace("%1", "\"{1}\"");
					args = args.Replace("%2", "\"{2}\"");
					args = args.Replace("%r", "\"{3}\"");

					args = string.Format(args, BaseFile, SourceFile, TargetFile, ResultFile);
                    logger.Trace("{0} {1}", Path.GetFileName(mergePath), args);
				}
				if (!string.IsNullOrEmpty(mergePath))
				{
					
						Process launchP4Merge = new Process();

						launchP4Merge.StartInfo.FileName = mergePath;
						launchP4Merge.StartInfo.Arguments = args;
						launchP4Merge.StartInfo.CreateNoWindow = true;
						launchP4Merge.StartInfo.UseShellExecute = true;
						launchP4Merge.Start();

						launchP4Merge.WaitForExit();

						launchP4Merge.Dispose();
				}
			}
			catch (ThreadAbortException)
			{
				// ignore thread aborts
				return false;
			}
			catch (Exception ex)
			{
				// If the error is because the repository is now null, it means
				// the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
				{
					ShowException(ex);
				}
				return false;
			}
			return true;
        }

        public IDictionary<int, int> GetShelvedFileSizes(string Path)
        {
            Dictionary<int, int> value = new Dictionary<int, int>();
            P4Command sizesCmd = new P4Command(Connection.Repository, "sizes", true, "-S", Path);
            try
            {
                P4CommandResult sizesRes = sizesCmd.Run();
                if ((sizesRes.Success) && (sizesRes.TaggedOutput != null) && (sizesRes.TaggedOutput.Count > 0))
                {
                    foreach (TaggedObject taggedData in sizesRes.TaggedOutput)
                    {
                        int id = -1;
                        if (int.TryParse(taggedData["change"], out id))
                        {
                            int size = 0;
                            if (int.TryParse(taggedData["fileSize"], out size))
                            {
                                value.Add(id, size);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If the error is because the repository is now null, it means
                // the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
                {
                    ShowException(ex);
                }
                return null;
            }
            if (value.Count > 0)
            {
                return value;
            }
            return null;
        }

        public long GetShelvedFileSize(int changelistId, string Path)
        {
            P4Command sizesCmd = new P4Command(Connection.Repository, "sizes", true, "-S", Path);
            try
            {
                P4CommandResult sizesRes = sizesCmd.Run();
                if ((sizesRes.Success) && (sizesRes.TaggedOutput != null) && (sizesRes.TaggedOutput.Count > 0))
                {
                    foreach (TaggedObject taggedData in sizesRes.TaggedOutput)
                    {
                        int id = -1;
                        if (int.TryParse(taggedData["change"], out id) && (id == changelistId))
                        {
                            long size = 0;
                            if (long.TryParse(taggedData["fileSize"], out size))
                            {
                                return size;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If the error is because the repository is now null, it means
                // the connection was closed in the middle of a command, so ignore it.
                if (Connection.Repository != null)
                {
                    ShowException(ex);
                }
                return -1;
            }
            // didn't find it
            return -1;
        }

        ~P4ScmProvider()
		{
			Dispose();
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				logger.Trace("disposing scm ID:{0}", Connection.ID);

				if (fileCache != null)
				{
					fileCache.Dispose();
				}
				if (Connection.Swarm != null && Connection.Swarm.certHandler != null)
				{
					Connection.Swarm.certHandler.Dispose();
				}
				if ((Connection.Repository != null) && (Connection.Repository.Connection != null) &&
					(Connection.Repository.Connection.Status != P4.ConnectionStatus.Disconnected))
				{
					if (Preferences.LocalSettings.GetBool("Auto_logoff", false))
					{
						Connection.Repository.Connection.Logout(null);
					}
					// Unsubscribe to the output events to display results in the command window
					Connection.Repository.Connection.InfoResultsReceived -= CommandLine.InfoResultsCallbackFn;
					Connection.Repository.Connection.ErrorReceived -= CommandLine.ErrorCallbackFn;
					Connection.Repository.Connection.TextResultsReceived -= CommandLine.TextResultsCallbackFn;
					Connection.Repository.Connection.TaggedOutputReceived -= CommandLine.TaggedOutputCallbackFn;
					Connection.Repository.Connection.CommandEcho -= CommandLine.CommandEchoCallbackFn;
					Connection.Repository.Connection.ResponseTimeEcho -= CommandLine.CommandEchoCallbackFn;

					Connection.Repository.Dispose();
					Connection.Repository = null;
				}
			}
			catch (Exception ex)
			{
				ShowException(ex);
			}
		}

		#endregion
	}
}
