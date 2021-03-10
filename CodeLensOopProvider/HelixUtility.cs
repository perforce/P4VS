using Perforce.P4;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Management;

namespace CodeLensOopProvider
{
	public static class HelixUtility
    {
        static MemoryMappedFile memoryMappedFile;
        static MemoryMappedViewAccessor viewAccessor;
        static Repository repository;
        static string currentConnectionInfo()
        {
            // Get the process ID of ServiceHub.Host.CLR.x86.exe
            //var sHHCLRId = Process.GetCurrentProcess().Id;
            //var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", sHHCLRId);
            //var search = new ManagementObjectSearcher("root\\CIMV2", query);
            //var results = search.Get().GetEnumerator();
            //results.MoveNext();
            //var queryObj = results.Current;
            //// Get the process ID of ServiceHub.Host.Node.x86.exe
            //var sHHNodeId = (uint)queryObj["ParentProcessId"];

            //query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", sHHNodeId);
            //search = new ManagementObjectSearcher("root\\CIMV2", query);
            //results = search.Get().GetEnumerator();
            //results.MoveNext();
            //queryObj = results.Current;
            //// Get the process ID of the currently running Visual Studio IDE
            //var vsId = (uint)queryObj["ParentProcessId"];

            Process[] allVS = Process.GetProcessesByName("devenv");

            string connectionInfo = "";
            foreach(Process vs in allVS)
            {
                try
                {
                    memoryMappedFile = MemoryMappedFile.OpenExisting("connection-info-" + vs.Id.ToString());
                    viewAccessor = memoryMappedFile.CreateViewAccessor();

                    byte[] fileContent = new byte[viewAccessor.ReadInt32(0)];
                    viewAccessor.ReadArray<byte>(4, fileContent, 0, fileContent.Length);
                    connectionInfo = Encoding.UTF8.GetString(fileContent, 0, fileContent.Length);


                    fileContent = Encoding.UTF8.GetBytes("");
                    viewAccessor.Write(0, fileContent.Length);
                    viewAccessor.WriteArray<byte>(4, fileContent, 0, fileContent.Length);
                    viewAccessor.Flush();

                    viewAccessor.Dispose();
                    memoryMappedFile.Dispose();

                    if (!string.IsNullOrEmpty(connectionInfo))
                    {
                        break;
                    }
                }
                catch (FileNotFoundException ex)
                {
                    // this will catch "Unable to find the specified file."
                    // in languages other than English.
                }
                catch (Exception ex)
                {
                    // If there is no connection, there will be a
                    // "Unable to find the specified file." error.
                    // Don't show that error
                    if (!ex.Message.Contains("Unable to find the specified file."))
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                    }
                }
            }
            
            return connectionInfo;
        }

        /// <summary>
        /// Gets a Helix repo and root path to the repo.
        /// </summary>
        public static Repository GetRepository(string path, out string repoRoot, out Changelist latest)
        {
            string port, user, client;
            Repository rep;
            // Check if a connection has been sent/saved in the memory
            // mapped file.

            string[] connectionInfo = currentConnectionInfo().Split(',');

            if (connectionInfo !=null && connectionInfo.Length>1)
            {
                port = connectionInfo[0];
                user = connectionInfo[1];
                client = connectionInfo[2];
                Server server = new Server(new ServerAddress(port));
                rep = new Repository(server);
                rep.Connection.UserName = user;
                rep.Connection.Client = new Client();
                rep.Connection.Client.Name = client;
                rep.Connection.Connect(null);
                repoRoot = rep.Connection.Client.Root;
                repository = rep;

                // clear changelist cache on new connection
                // at least for this file

                changelistCache.Clear();
                latest = GetLastCommit(rep, path);
                return rep;
            }
            else if (repository!=null&& repository.Connection.Status == ConnectionStatus.Connected)
            {
                rep = repository;
                repoRoot = rep.Connection.Client.Root;
                latest = GetLastCommit(rep, path);
                return rep;

            }
            // If connection was not in the memory mapped file,
            // return a null repository.
            else
            {
                repoRoot = null;
                latest = null;
                return null;
            }
        }

        public static string GetCurrentBranch(Repository repo)
        {
            return "GetCurrentBranch"; // repo.Head;
        }

        public static string GetTrackedBranch(Repository repo)
        {
            return "GetTrackedBranch"; // repo.Head.TrackedBranch;
        }

        static List<KeyValuePair<string, Changelist>> changelistCache = new List<KeyValuePair<string, Changelist>>();

        public static Changelist GetLastCommit(Repository repo, string filePath)
        {
            IList<Changelist> Changes = new List<Changelist>();
            // first check cache
            var items = changelistCache;
            var lookup = items.ToLookup(kvp => kvp.Key, kvp => kvp.Value);
            foreach (Changelist change in lookup[filePath])
            {
                Changes.Add(change);
            }

            if (Changes.Count < 1)
            {
                try
                {
                    ChangesCmdOptions opts = new ChangesCmdOptions(ChangesCmdFlags.FullDescription, null,
                        5, ChangeListStatus.Submitted, null);
                    Changes = repo.GetChangelists(opts, new FileSpec(null,
                        null, new LocalPath(filePath), null));
                }
                catch (P4Exception ex)
                {
                    if (!(ex.ErrorCode == P4ClientError.MsgDb_NotUnderRoot))
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                    }
                }
            }

            if (Changes == null || Changes.Count < 1 )
            {
                return null;
            }
            foreach (Changelist change in Changes)
            {
                var matches = from val in changelistCache
                              where val.Key == filePath
                              select val.Value as Changelist;

                Changelist c = matches.FirstOrDefault(ch =>
                ch.Id == change.Id);

                if (c == null)
                {
                    changelistCache.Add(new KeyValuePair<string, Changelist>(filePath, change));
                }
            }
            return Changes[0];
        }

        public static IList<Changelist> GetCommits(Repository repo, string filePath, int count)
        {
            IList<Changelist> Changes = new List<Changelist>();
            // first check cache
            var items = changelistCache;
            var lookup = items.ToLookup(kvp => kvp.Key, kvp => kvp.Value);
            foreach (Changelist change in lookup[filePath])
            {
                Changes.Add(change);
            }

            if (Changes.Count>0)
            {
                return Changes;
            }
            try
            {
                ChangesCmdOptions opts = new ChangesCmdOptions(ChangesCmdFlags.FullDescription, null,
                    5, ChangeListStatus.Submitted, null);

                Changes = repo.GetChangelists(opts, new FileSpec(null,
                    null, new LocalPath(filePath), null));

                foreach (Changelist change in Changes)
                {
                    changelistCache.Add(new KeyValuePair<string, Changelist>(filePath, change));
                }
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return Changes;
        }
    }
}
