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
 * Name		: P4ScmCache.cs
 *
 * Author	: Duncan Barbee <dbarbee@perforce.com>
 *
 * Description	: Cache for Perforce file data
 *
 ******************************************************************************/

using System;
using System.Collections.Generic;
#if !VS2008
using System.Collections.Concurrent;
#endif
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using NLog;

using Perforce;
using Perforce.P4VS;

namespace Perforce.P4Scm
{
	public class CachedFile
	{
		internal P4.FileMetaData _file;
		public CachedFile(P4.FileMetaData file)
		{
			_file = file;
			ScmStatus = new SourceControlStatus(file);
			LastUpdate = DateTime.Now;
		}
		public DateTime LastUpdate;

        static public implicit operator P4.FileMetaData(CachedFile cf)
        {
            if (cf == null)
            {
                return null;
            } 
            return cf._file; 
        }

		public SourceControlStatus ScmStatus { get; private set; }
	}

	public class P4ScmCache
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // Enforces conversion through type safety
        private class CacheKey
        {
            private string _key;
            public CacheKey(string key)
            {
                _key = key.ToLower().Replace('/', '\\');
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
				    return false;
    			if (obj is CacheKey)
    				return ((CacheKey)obj)._key == this._key;
                return false;
            }

            public override string ToString()
            {
                return _key;
            }

            public override int GetHashCode()
            {
                return _key.GetHashCode();
            }
        }

        private Dictionary<CacheKey, CachedFile> cache;
        private TimeSpan _refreshInterval = TimeSpan.FromSeconds(15);
        private P4ScmProvider _scm = null;

        public P4ScmCache(P4ScmProvider scm)
        {
            cache = new Dictionary<CacheKey, CachedFile>();
            lock (cache)
            {
                _scm = scm;

                int minutes = Preferences.LocalSettings.GetInt("Update_status", 5);
                _refreshInterval = TimeSpan.FromMinutes(minutes);

                if ((_scm != null) && (_scm.SccService != null))
                {
                    OnUpdatedFiles += new UpdatedFiles_Delegate(_scm.OnCacheFilesUpdated);
                }
            }
        }

        public int Count
        {
            get
            {
                lock (cache)
                {
                    return cache.Count;
                }
            }
        }

        private CacheKey GetKey(string path)
        {
            return new CacheKey(path);
        }

        /// <summary>
		/// Add data from the output of an GetFileMetaData command
		/// </summary>
		/// <param name="files"></param>
		/// <returns></returns>
		public void AddFileMetaData(IList<P4.FileMetaData> files)
		{
			lock (cache)
			{
				if (files == null || files.Count <= 0)
					return;

				foreach (P4.FileMetaData file in files)
				{
					if (file.LocalPath != null)
					{
						cache[GetKey(file.LocalPath.Path)] = new CachedFile(file);
                        logger.Trace("Caching: {0}", file.LocalPath.Path);
                    }
				}
				return;
			}
		}

		/// <summary>
		/// Add data from the output of an GetFileMetaData command
		/// </summary>
		/// <param name="pServer"></param>
		/// <param name="fstatInfo"></param>
		/// <returns></returns>
		public IList<string> UpdateFileMetaData(IList<P4.FileMetaData> files)
		{
			lock (cache)
			{
				if (files == null || files.Count <= 0)
					return null;

				IList<string> fileList = new List<string>();
				foreach (P4.FileMetaData file in files)
				{
					if (file.LocalPath != null)
					{
						fileList.Add(file.LocalPath.Path);
						cache[GetKey(file.LocalPath.Path)] = new CachedFile(file);
                        logger.Trace("Updating: {0}", file.LocalPath.Path);
                    }
				}
				return fileList;
			}
		}

		private P4.Repository _repository
		{
			get
			{
				lock (cache)
				{
					if (_scm != null)
					{
                        _scm.Connection.Repository.Connection.getP4Server().ProgramName = "P4VS";
                        _scm.Connection.Repository.Connection.getP4Server().ProgramVersion = Versions.product();
                        return _scm.Connection.Repository;
					}
					return null;
				}
			}
		}

		/// <summary>
		/// Check if the file has been updated in the specified timespan
		/// </summary>
		/// <param name="key">Local path of the file</param>
		/// <param name="maxAge">Max timespan  since last refresh</param>
		/// <returns>
		/// False: If not in cache or last update was more than maxAge ago.
		/// True: If last update less than maxAge ago.
		/// </returns>
		public bool CacheIsFresh(string key, TimeSpan maxAge)
		{
			lock (cache)
			{
				if (ContainsKey(key) == false)
					return false;

				CachedFile c = cache[GetKey(key)];
                bool result = ((DateTime.Now - c.LastUpdate) < maxAge);
                if (logger.IsTraceEnabled) 
                {
                    var details = "";
                    if (!result)
                    {
                        details = string.Format("({0}) ", (DateTime.Now - c.LastUpdate));
                    }
                    logger.Trace("CacheFresh {0} {1}timespan {2} and file {3}",
                        result, details, maxAge, key);
                }
				return result;
			}
		}

        public void Set(string key, CachedFile value)
        {
			lock (cache)
			{
				if (string.IsNullOrEmpty(key))
				{
					throw new ArgumentException("Key can't be null or blank", "key");
				}
                if ((key.EndsWith("\\")) || (key.EndsWith("\\*")) || (key.EndsWith("\\...")))
                {
                    throw new ArgumentException("Key can't be a folder", "key");
                }
                var k = GetKey(key);
				if (cache.ContainsKey(k))
				{
					cache[k] = value;
				}
				else
				{
					cache.Add(k, value);
				}
			}
        }

		public CachedFile Get(string key)
		{
			lock (cache)
			{
				if ((key.EndsWith("\\")) || (key.EndsWith("\\*")) || (key.EndsWith("\\...")))
				{
					throw new ArgumentException("Key can't be a folder", "key");
				}
                var k = GetKey(key);
				if (!cache.ContainsKey(k))
				{
					return null;
				}
				return cache[k];
			}
		}

		public bool ContainsKey(string key)
		{
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
			lock (cache)
			{
				return cache.ContainsKey(GetKey(key));
			}
		}

		public bool Remove(string key)
		{
			lock (cache)
			{
                logger.Trace("Removing: {0}", key);
				return cache.Remove(GetKey(key));
            }
		}

		/// <summary>
		/// Cast the map to P4.FileMetaDataList
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public static implicit operator List<P4.FileMetaData>(P4ScmCache m)
		{
			lock (m)
			{
				if (m == null)
					return null;

				List<P4.FileMetaData> value = new List<P4.FileMetaData>();
				foreach (P4.FileMetaData d in m.cache.Values)
					value.Add(d);
				return value;
			}
		}

		private void AddFile(P4.FileMetaData file)
		{
			lock (cache)
			{
				if (file.LocalPath != null)
				{
					cache[GetKey(file.LocalPath.Path)] = new CachedFile(file);
                    logger.Trace("Adding2: {0}", file.LocalPath.Path);
                }
			}
		}

		public void AddFile(string LocalPath)
		{
			lock (cache)
			{
				if (Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true))
				{
					string directoryPath = Path.GetDirectoryName(LocalPath);
					if (directoryPath != null && ((CachedDirectories != null) &&
												  (CachedDirectories.ContainsKey(GetKey(directoryPath))) &&
												  (CachedDirectories[GetKey(directoryPath)] != DateTime.MinValue)))
					{
						cache[GetKey(LocalPath)] = null;
                        logger.Trace("AddFile: {0}", LocalPath);
                        return;
					}
					else if (directoryPath != null && ((CachedDirectories == null) ||
													   !CachedDirectories.ContainsKey(GetKey(directoryPath))))
					{
						AddDirectoryFilesToCache(directoryPath, false);
                        logger.Trace("AddDir: {0}", directoryPath);
						return;
					}
				}
				P4.Options opts = new P4.Options();
				opts["-Olhp"] = null;
				IList<P4.FileMetaData> files = _repository.GetFileMetaData(opts, P4.FileSpec.LocalSpec(LocalPath));

				if ((files != null) && (files.Count > 0))
				{
					cache[GetKey(LocalPath)] = new CachedFile(files[0]);
                    logger.Trace("AddDirMeta: {0}", LocalPath);
				}
			}
		}

		private void AddFiles(IList<string> LocalPaths)
		{
			lock (cache)
			{
				P4.Options opts = new P4.Options();
				opts["-Olhp"] = null;
				IList<P4.FileMetaData> files = _repository.GetFileMetaData(P4.FileSpec.LocalSpecList(LocalPaths.ToArray()), opts);

				if ((files != null) && (files.Count > 0))
				{
					foreach (P4.FileMetaData file in files)
					{
						cache[GetKey(file.LocalPath.Path)] = new CachedFile(file);
                        logger.Trace("AddFiles: {0}", file.LocalPath.Path);
                    }
				}
			}
		}

		private IList<string> UpdateFiles(IList<string> LocalPaths)
		{
			IList<string> updatedFiles = new List<string>();

			lock (cache)
			{
				P4.Options opts = new P4.Options();
				opts["-Olhp"] = null;
				IList<P4.FileMetaData> files = _repository.GetFileMetaData(P4.FileSpec.LocalSpecList(LocalPaths.ToArray()), opts);

				if ((files != null) && (files.Count > 0))
				{
					foreach (P4.FileMetaData file in files)
					{
                        var key = GetKey(file.LocalPath.Path);
						if (cache.ContainsKey(key))
						{
							SourceControlStatus oldStat = cache[key].ScmStatus;
							SourceControlStatus newStat = new SourceControlStatus(file);
							if (oldStat.Flags != newStat.Flags)
							{
								cache[key] = new CachedFile(file);
								updatedFiles.Add(file.LocalPath.Path);
                                logger.Trace("UpdateFiles: {0}", file.LocalPath.Path);
                            }
						}
						else
						{
							cache[key] = new CachedFile(file);
							updatedFiles.Add(file.LocalPath.Path);
                            logger.Trace("UpdateFiles: {0}", file.LocalPath.Path);
                        }
					}
				}
			}
			return updatedFiles;
		}

		class CachedDirectory
		{
			public CachedDirectory(string path)
			{ Path = path; LastUpdate = DateTime.Now; }
			public CachedDirectory(string path, DateTime lastUpdate)
			{ Path = path; LastUpdate = lastUpdate; }
			public string Path { get; private set; }
			public DateTime LastUpdate { get; private set; }
			public static implicit operator DateTime(CachedDirectory it) { return it.LastUpdate; }
			public static implicit operator string (CachedDirectory it) { return it.Path; }
		}

		private Dictionary<CacheKey, CachedDirectory> CachedDirectories { get; set; }

		public IList<string> AddDirectoryFilesToCache(String directoryPath, bool recursive)
		{
			lock (cache)
			{
				try
				{
					String fileSpec;
					if (recursive)
					{
						fileSpec = String.Format("{0}/...", directoryPath);
					}
					else
					{
						fileSpec = String.Format("{0}/*", directoryPath);
					}

					var key = GetKey(directoryPath);

					if (CachedDirectories == null)
					{
						CachedDirectories = new Dictionary<CacheKey, CachedDirectory>();
					}
					else if ((CachedDirectories.ContainsKey(key)) &&
						(CachedDirectories[key] > DateTime.Now - TimeSpan.FromSeconds(25)))
					{
						return null;
					}
					//make sure the added path is under the client's root directory
                    if (_scm != null && !string.IsNullOrEmpty(_scm.Connection.WorkspaceRoot))
					{
						var wsRoot = GetKey(_scm.Connection.WorkspaceRoot);
                        logger.Trace("wsRoot: {0}", wsRoot);
						if (!key.ToString().StartsWith(wsRoot.ToString(), StringComparison.OrdinalIgnoreCase))
						{
							CachedDirectories[key] = new CachedDirectory(directoryPath);
                            logger.Trace("AddDir: {0}", key);
							return null;
						}
					}

					// add/update cache folder to the hash of folders that have been cached
					CachedDirectories[key] = new CachedDirectory(directoryPath);
					if (recursive)
					{
						// non recursive directory walk to add/update all the subdirectories 
						// to the hash table of directories that have been cached
						Queue<string> subdirs = new Queue<string>();
						subdirs.Enqueue(directoryPath);

						while (subdirs.Count > 0)
						{
							string subdir = subdirs.Dequeue();
							foreach (string sd in System.IO.Directory.GetDirectories(subdir))
							{
								// add to queue for so it'll be walked for subdirectories
								subdirs.Enqueue(sd);
								// add/update this subdirectory to the hash of cached directories
								CachedDirectories[GetKey(sd)] = new CachedDirectory(sd);
							}
						}
					}

					P4.Options opts = new P4.Options();
					opts["-Olhp"] = null;
					opts["-m"] = "100000";
                    logger.Trace("WorkspaceRoot: {0}", _scm.Connection.WorkspaceRoot);
                    
					IList<P4.FileMetaData> files =
						_repository.GetFileMetaData(opts, P4.FileSpec.LocalSpec(fileSpec));

					IList<string> updated = null;
					if (files != null)
					{
						updated = UpdateFileMetaData(files);
					}
					else
					{
						P4.P4CommandResult results = _repository.Connection.LastResults;

						if ((results != null) && (results.ErrorList != null) && (results.ErrorList.Count > 0) &&
							((results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderRoot) ||
							(results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDb_NotUnderClient) ||
							(results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_IntegMovedUnmapped) ||
							(results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_ExVIEW) ||
							(results.ErrorList[0].ErrorCode == P4.P4ClientError.MsgDm_ExVIEW2)))
						{
							if (CachedDirectories == null)
							{
								CachedDirectories = new Dictionary<CacheKey, CachedDirectory>();
							}
							CachedDirectories[key] = new CachedDirectory(directoryPath);
						}
					}
					return updated;
				}
				catch (Exception ex)
				{
					// If the error is because the repository is now null, it means
					// the connection was closed in the middle of a command, so ignore it.
					if (_repository != null)
					{
						_scm.ShowException(ex, false, true, (Preferences.LocalSettings.GetBool("Log_reporting", false) == true));
					}
					if (CachedDirectories == null)
					{
						CachedDirectories = new Dictionary<CacheKey, CachedDirectory>();
					}
					CachedDirectories[GetKey(directoryPath)] = new CachedDirectory(directoryPath, DateTime.MinValue);
					return null;
				}
			}
		}

		public bool IsDirectoryCached(string path)
		{
			if ((CachedDirectories == null))
			{
				return false;
			}
			return CachedDirectories.ContainsKey(GetKey(path));
		}

		public bool IsFilesDirectoryCached(string path)
		{
			string directoryPath = Path.GetDirectoryName(path);
			return IsDirectoryCached(directoryPath);
		}

		private void Init(string localHeadPath)
		{
			lock (cache)
			{
				if (Preferences.LocalSettings.GetBool("PreloadScmCache", true))
				{
					AddDirectoryFilesToCache(localHeadPath, true);
				}
			}
		}

		private Dictionary<string, bool> _ignoredFilesMap;

		public Dictionary<string, bool> IgnoredFilesMap
		{
			get
			{
				if (_ignoredFilesMap == null)
				{
					_ignoredFilesMap = new Dictionary<string, bool>();
				}
				lock (_ignoredFilesMap)
				{
					return _ignoredFilesMap;
				}
			}
		}

		Thread RefreshThread = null;

        // Goes together with: RefreshProjectGlyphs_Delegate in P4VsProviderServices.cs
		public delegate void UpdatedFiles_Delegate(IList<string> files, bool forceUpdate);

		private event UpdatedFiles_Delegate _onUpdatedFiles;
		public event UpdatedFiles_Delegate OnUpdatedFiles
		{
			add
			{
				_onUpdatedFiles += value;
				if (RefreshThread == null)
				{
					ThreadStart refreshThreadProc = null;
					if (Preferences.LocalSettings.GetBool("TreatProjectsAsFolders", true))
					{
						refreshThreadProc = new ThreadStart(RefreshFoldersThreadProc);
					}
					else
					{
						refreshThreadProc = new ThreadStart(RefreshFilesThreadProc);
					}
					RefreshThread = new Thread(refreshThreadProc);
					RefreshThread.IsBackground = true;
					RefreshThread.Priority = ThreadPriority.BelowNormal;

					RefreshThread.Start();
				}
			}
			remove
			{
				_onUpdatedFiles -= value;
				if ((_onUpdatedFiles == null) && (RefreshThread != null))
				{
					RefreshThread.Abort();
					RefreshThread = null;
				}
			}
		}
		bool _runRefreshThread = true;

		int maxFilestoRefresh = 100;

		private void RefreshFilesThreadProc()
		{
			try
			{
				DateTime updateStarted = DateTime.Now;
				while (_runRefreshThread)
				{
					int minutes = Preferences.LocalSettings.GetInt("Update_status", 5);
                    logger.Trace("CWD: {0}", _scm.Connection.Repository.Connection.CurrentWorkingDirectory);
                    logger.Trace("Entering loop in RefreshFilesThreadProc, LoadingSolution:{0}, WaitTime: {1} ",
						_scm.LoadingSolution, minutes);
					if ((_scm.LoadingSolution) || (minutes < 1))
					{
						minutes = 0;

						Thread.Sleep(15000); //sleep 15 seconds
						// then check to see if the refresh interval has changed
						// or the solution has completed loading
						continue; 
					}
                    logger.Trace("Finished waiting for solution load or postive refresh interval in RefreshFilesThreadProc");

					_refreshInterval = TimeSpan.FromMinutes(minutes);

					TimeSpan waitIncrement = TimeSpan.FromMilliseconds(1000);
					try
					{
						TimeSpan timeWaited = DateTime.Now - updateStarted;
						while (timeWaited < _refreshInterval)
						{
							if (_runRefreshThread == false)
							{
                                logger.Debug("Exiting refresh thread");
                                return;
							}
							Thread.Sleep(waitIncrement);
							timeWaited += waitIncrement;
						}
					}
					catch { }

					updateStarted = DateTime.Now;

					try
					{
						if (_runRefreshThread == false)
						{
                            logger.Debug("Exiting refresh thread");
                            return;
						}
						if ((minutes > 0) && (cache.Count > 0) && (_onUpdatedFiles != null))
						{
							if (_ignoredFilesMap != null)
							{
								lock (_ignoredFilesMap)
								{
									_ignoredFilesMap = null;    // TODO - RC - Why??
								}
							}
							CachedFile[] files = new CachedFile[cache.Count];
                            logger.Trace("Finished waiting for refresh interval in RefreshFilesThreadProc, {0} files in the cache", cache.Count);
							lock (cache)
							{
								cache.Values.CopyTo(files, 0);
							}
							int fileidx = 0;
							int fileCnt = cache.Count;
							while (fileidx < fileCnt)
							{
								IList<string> filesToRefresh = new List<string>();

								for (int batchCnt = 0; (fileidx < fileCnt) && (batchCnt < maxFilestoRefresh); fileidx++)
								{
									CachedFile file = files[fileidx];
									if (_runRefreshThread == false)
									{
                                        logger.Debug("Exiting refresh thread");
                                        return;
									}
									if (DateTime.Now - file.LastUpdate > _refreshInterval)
									{
										batchCnt++;
										if ((file != null) && (((P4.FileMetaData)file) != null) && (((P4.FileMetaData)file).LocalPath != null))
										{
											filesToRefresh.Add(((P4.FileMetaData)file).LocalPath.Path);
										}
									}
								}
								if ((filesToRefresh != null) && (filesToRefresh.Count > 0))
								{
                                    logger.Trace("Found {0} files to refresh in RefreshFilesThreadProc, starting with {1}", filesToRefresh.Count, filesToRefresh[0]);
									filesToRefresh = UpdateFiles(filesToRefresh);

									if ((filesToRefresh != null) && (filesToRefresh.Count > 0))
									{
										Delegate[] targetList = _onUpdatedFiles.GetInvocationList();
										foreach (UpdatedFiles_Delegate dlgRefreshProjectGlyphs in targetList)
										{
											if (_runRefreshThread == false)
											{
                                                logger.Debug("Exiting refresh thread");
												return;
											}
											try
											{
												dlgRefreshProjectGlyphs(filesToRefresh, false);
											}
											catch
											{
												// problem with delegate, so remove from the list
												OnUpdatedFiles -= dlgRefreshProjectGlyphs;
											}
										}
									}
									if (fileidx < fileCnt)
									{
										Thread.Sleep(1000);
									}
								}
							}
						}
					}
                    catch (Exception ex)
                    {
                        logger.Warn("Refresh thread exception: {0}", ex.Message);
                        logger.Debug("Refresh thread exception: {0}", ex.StackTrace);
                    }
				}
			}
            catch (ThreadAbortException ex)
            {
                logger.Warn("Refresh thread aborted: {0}", ex.Message);
                logger.Debug("Refresh thread exception: {0}", ex.StackTrace);
                Thread.ResetAbort();
            } 
            catch (Exception ex)
            {
                logger.Warn("Refresh thread exception: {0}", ex.Message);
                logger.Debug("Refresh thread exception: {0}", ex.StackTrace);
            }

		}

		//int maxGlyphstoRefresh = 250;

		private void RefreshFoldersThreadProc()
		{
			try
			{
				DateTime updateStarted = DateTime.Now;
				while (_runRefreshThread)
				{
					int minutes = Preferences.LocalSettings.GetInt("Update_status", 5);
					int maxGlyphstoRefresh = Preferences.LocalSettings.GetInt("Max_Glyphs_Refresh", 250);
					logger.Trace("Entering loop in RefreshFoldersThreadProc, LoadingSolution:{0}, WaitTime: {1} ",
						_scm.LoadingSolution, minutes);
					if ((_scm.LoadingSolution) || (minutes < 1))
					{
						minutes = 0;

						Thread.Sleep(15000); //sleep 15 seconds
						// then check to see if the refresh interval has changed
						// or the solution has completed loading
						continue;
					}

					_refreshInterval = TimeSpan.FromMinutes(minutes);

					TimeSpan waitIncrement = TimeSpan.FromMilliseconds(1000);
					try
					{
						TimeSpan timeWaited = DateTime.Now - updateStarted;
						while (timeWaited < _refreshInterval)
						{
							if (_runRefreshThread == false)
							{
                                logger.Debug("Exiting refresh thread");
                                return;
							}
							Thread.Sleep(waitIncrement);
							timeWaited += waitIncrement;
						}
					}
					catch { }
                    logger.Trace("Finished waiting for solution load or postive refresh interval in RefreshFoldersThreadProc");
					updateStarted = DateTime.Now;

					try
					{
						if (_runRefreshThread == false)
						{
                            logger.Trace("Exiting refresh thread");
							return;
						}
						if ((minutes > 0) && (cache.Count > 0) && (_onUpdatedFiles != null))
						{
							if (_ignoredFilesMap != null)
							{
								lock (_ignoredFilesMap)
								{
									_ignoredFilesMap = null;
								}
							}
							string[] directories = new string[CachedDirectories.Count];
                            logger.Trace("Finished waiting for refresh interval in RefreshFoldersThreadProc, {0} folders in the cache", CachedDirectories.Count);
							lock (cache)
							{
                                var keys = new List<string>();
                                foreach (var k in CachedDirectories.Keys)
                                    keys.Add(k.ToString());
								keys.CopyTo(directories, 0);
							}
							int dirIdx = 0;
							int dirCnt = CachedDirectories.Count;
							while (dirIdx < dirCnt)
							{
								if (DateTime.Now - CachedDirectories[GetKey(directories[dirIdx])] > _refreshInterval)
								{
									string directory = CachedDirectories[GetKey(directories[dirIdx])];
                                    logger.Trace("Checking for files to refresh in RefreshFoldersThreadProc in folder: {0}", directory);

                                    IList<string> updatedfiles = AddDirectoryFilesToCache(directory, false);
									if ((updatedfiles != null) && (updatedfiles.Count > 0))
									{
                                        logger.Trace("Found {0} files to refresh in RefreshFoldersThreadProc in folder: {1}", updatedfiles.Count, directory);
										if (updatedfiles.Count <= maxGlyphstoRefresh)
										{
                                            logger.Trace("Updating less than {0} files in RefreshFoldersThreadProc", maxGlyphstoRefresh);
											Delegate[] targetList = _onUpdatedFiles.GetInvocationList();
											foreach (UpdatedFiles_Delegate dlgRefreshProjectGlyphs in targetList)
											{
												if (_runRefreshThread == false)
												{
                                                    logger.Trace("Exiting refresh thread");
                                                    return;
												}
												try
												{
													dlgRefreshProjectGlyphs(updatedfiles, false);
												}
												catch
												{
													// problem with delegate, so remove from the list
													OnUpdatedFiles -= dlgRefreshProjectGlyphs;
												}
											}
										}
										else
										{
                                            logger.Trace("Updating more than {0} files in RefreshFoldersThreadProc", maxGlyphstoRefresh);
											int updateIdx = 0;
											IList<string> updateList = new List<string>();

											while (updateIdx < updatedfiles.Count)
											{
												updateList.Add(updatedfiles[updateIdx]);

												if ((updateIdx >= (updatedfiles.Count - 1)) || (updateList.Count >= maxGlyphstoRefresh))
												{
                                                    logger.Trace("Updating {0} files at idx, {1} in RefreshFoldersThreadProc", updateList.Count, updateIdx);
													Delegate[] targetList = _onUpdatedFiles.GetInvocationList();
													foreach (UpdatedFiles_Delegate dlgRefreshProjectGlyphs in targetList)
													{
														if (_runRefreshThread == false)
														{
                                                            logger.Trace("Exiting refresh thread");
                                                            return;
														}
														try
														{
															dlgRefreshProjectGlyphs(updateList, false);
														}
														catch
														{
															// problem with delegate, so remove from the list
															OnUpdatedFiles -= dlgRefreshProjectGlyphs;
														}
													}
													updateList = new List<string>();
													Thread.Sleep(TimeSpan.FromSeconds(1));
												}
												updateIdx++;
											}
										}
									}
								}
								dirIdx++;
								if (dirIdx < dirCnt)
								{
                                    logger.Trace("Done updating files in RefreshFoldersThreadProc in folder: {0}", directories[dirIdx]);
                                    Thread.Sleep(5000);
								}
                                else
                                    logger.Trace("Done updating files in RefreshFoldersThreadProc");
                            }
						}
					}
			        catch (Exception ex) 
                    {
                        logger.Warn("Refresh thread exception: {0}", ex.Message);
					}
				}
			}
			catch (ThreadAbortException ex)
			{
                logger.Warn("Refresh thread aborted: {0}", ex.Message);
				Thread.ResetAbort();
			}
			catch (Exception ex) 
            {
                logger.Warn("Refresh thread exception: {0}", ex.Message);
                logger.Debug("Refresh thread exception: {0}", ex.StackTrace);
            }
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				if (RefreshThread != null)
				{
					_runRefreshThread = false;
					RefreshThread.Join(TimeSpan.FromSeconds(2));
					RefreshThread = null;
				}
				_scm = null;
			}
			catch { }; 
		}

		#endregion
	}
}
