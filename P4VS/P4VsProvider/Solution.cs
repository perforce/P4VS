using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NLog;
using Perforce.P4Scm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Perforce.P4VS
{
    public abstract class Solution : BackwardsCompatibleAsyncPackage
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public P4VsProviderService SccService { get; set; }

        public static P4ScmProvider CurrentScm { get; internal set; }

        public StreamStatus ClientStream = new StreamStatus();
        public P4VStatus P4V = new P4VStatus();
        public ActiveChangeListCombo ChangeLists;
        public NodeGlyphs Glyphs;
 
        /// <summary>
        /// Gets the list of source controllable files in the solution
        /// </summary>
        public IList<string> GetSolutionFiles()
        {
            IList<string> solutionFiles = new List<string>();

            // When the solution is selected, all the uncontrolled projects in the solution will be added to scc
            Hashtable hash = GetLoadedControllableProjectsEnum();

            foreach (IVsHierarchy pHier in hash.Keys)
            {
                IList<string> sccFiles = GetProjectFiles(pHier as IVsSccProject2, VSConstants.VSITEMID_ROOT);
                foreach (string file in sccFiles)
                {
                    solutionFiles.Add(file);
                }
            }

            return solutionFiles;
        }

        /// <summary>
        /// Returns the filename of the solution
        /// </summary>
        public string GetSolutionFileName()
        {
            IVsSolution sol = (IVsSolution)GetService(typeof(SVsSolution));
            string solutionDirectory, solutionFile, solutionUserOptions;
            if (sol.GetSolutionInfo(out solutionDirectory, out solutionFile, out solutionUserOptions) == VSConstants.S_OK)
            {
                return solutionFile;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a list of controllable projects in the solution
        /// </summary>
        internal Hashtable GetLoadedControllableProjectsEnum()
        {
            Hashtable mapHierarchies = new Hashtable();

            IVsSolution sol = (IVsSolution)GetService(typeof(SVsSolution));
            Guid rguidEnumOnlyThisType = new Guid();
            IEnumHierarchies ppenum = null;
            ErrorHandler.ThrowOnFailure(sol.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref rguidEnumOnlyThisType, out ppenum));

            IVsHierarchy[] rgelt = new IVsHierarchy[1];
            uint pceltFetched = 0;
            while (ppenum.Next(1, rgelt, out pceltFetched) == VSConstants.S_OK &&
                   pceltFetched == 1)
            {
                IVsSccProject2 sccProject2 = rgelt[0] as IVsSccProject2;
                if (sccProject2 != null)
                {
                    mapHierarchies[rgelt[0]] = true;
                }
            }

            return mapHierarchies;
        }

        /// <summary>
        /// Gets the list of source controllable files in the specified project
        /// </summary>
        public IList<string> GetProjectFiles(IVsSccProject2 pscp2Project)
        {
            return GetProjectFiles(pscp2Project, VSConstants.VSITEMID_ROOT);
        }

        /// <summary>
        /// Gets the list of source controllable files in the specified project
        /// </summary>
        public IList<string> GetProjectFiles(IVsSccProject2 pscp2Project, uint startItemId)
        {
            IDictionary<string, object> projectFiles = new Dictionary<string, object>();
            IVsHierarchy hierProject = (IVsHierarchy)pscp2Project;
            IList<uint> projectItems = GetProjectItems(hierProject, startItemId);

            foreach (uint itemid in projectItems)
            {
                IList<string> sccFiles = GetNodeFiles(pscp2Project, itemid);
                foreach (string file in sccFiles)
                {
                    projectFiles[file] = null;
                }
            }

            return projectFiles.Keys.ToList();
        }

        /// <summary>
        /// Gets the list of ItemIDs that are nodes in the specified project
        /// </summary>
        private IList<uint> GetProjectItems(IVsHierarchy pHier)
        {
            // Start with the project root and walk all expandable nodes in the project
            return GetProjectItems(pHier, VSConstants.VSITEMID_ROOT);
        }

        /// <summary>
        /// Gets the list of ItemIDs that are nodes in the specified project, starting with the specified item
        /// </summary>
        private IList<uint> GetProjectItems(IVsHierarchy pHier, uint startItemid)
        {
            List<uint> projectNodes = new List<uint>();

            try
            {
                // The method does a breadth-first traversal of the project's hierarchy tree
                Queue<uint> nodesToWalk = new Queue<uint>();
                nodesToWalk.Enqueue(startItemid);

                while (nodesToWalk.Count > 0)
                {
                    uint node = nodesToWalk.Dequeue();
                    projectNodes.Add(node);

                    DebugWalkingNode(pHier, node);

                    object property = null;
                    if (pHier.GetProperty(node, (int)__VSHPROPID.VSHPROPID_FirstChild, out property) == VSConstants.S_OK)
                    {
                        uint childnode = (uint)(int)property;
                        if (childnode == VSConstants.VSITEMID_NIL)
                        {
                            continue;
                        }

                        DebugWalkingNode(pHier, childnode);

                        bool isExpandable = false;
                        if (pHier.GetProperty(childnode, (int)__VSHPROPID.VSHPROPID_Expandable, out property) == VSConstants.S_OK)
                        {
                            if (property is bool)
                            {
                                isExpandable = (bool)property;
                            }
                            else if (property is int)
                            {
                                isExpandable = ((int)property != 0);
                            }
                        }
                        bool isContainer = false;
                        if (pHier.GetProperty(childnode, (int)__VSHPROPID2.VSHPROPID_Container, out property) == VSConstants.S_OK)
                        {
                            if (property is bool)
                            {
                                isContainer = (bool)property;
                            }
                            else if (property is int)
                            {
                                isContainer = ((int)property != 0);
                            }
                        }
                        if (isExpandable || isContainer)
                        {
                            nodesToWalk.Enqueue(childnode);
                        }
                        else
                        {
                            projectNodes.Add(childnode);
                        }

                        while (pHier.GetProperty(childnode, (int)__VSHPROPID.VSHPROPID_NextSibling, out property) == VSConstants.S_OK)
                        {
                            childnode = (uint)(int)property;
                            if (childnode == VSConstants.VSITEMID_NIL)
                            {
                                break;
                            }

                            DebugWalkingNode(pHier, childnode);

                            isExpandable = false;
                            if (pHier.GetProperty(childnode, (int)__VSHPROPID.VSHPROPID_Expandable, out property) == VSConstants.S_OK)
                            {
                                if (property is bool)
                                {
                                    isExpandable = (bool)property;
                                }
                                else if (property is int)
                                {
                                    isExpandable = ((int)property != 0);
                                }
                            }
                            isContainer = false;
                            if (pHier.GetProperty(childnode, (int)__VSHPROPID2.VSHPROPID_Container, out property) == VSConstants.S_OK)
                            {
                                if (property is bool)
                                {
                                    isContainer = (bool)property;
                                }
                                else if (property is int)
                                {
                                    isContainer = ((int)property != 0);
                                }
                            }
                            if (isExpandable || isContainer)
                            {
                                nodesToWalk.Enqueue(childnode);
                            }
                            else
                            {
                                projectNodes.Add(childnode);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Trace("Exception in P4 SCM {0}\r\n\t{1}", ex.Message, ex.StackTrace);
            }
            return projectNodes;
        }

        /// <summary>
        /// Returns the filename of the specified controllable project 
        /// </summary>
        public string GetProjectFileName(IVsSccProject2 pscp2Project)
        {
            // Note: Solution folders return currently a name like "NewFolder1{1DBFFC2F-6E27-465A-A16A-1AECEA0B2F7E}.storage"
            // Your provider may consider returning the solution file as the project name for the solution, if it has to persist some properties in the "project file"
            // UNDONE: What to return for web projects? They return a folder name, not a filename! Consider returning a pseudo-project filename instead of folder.

            IVsHierarchy hierProject = (IVsHierarchy)pscp2Project;
            IVsProject project = (IVsProject)pscp2Project;

            // Attempt to get first the filename controlled by the root node 
            IList<string> sccFiles = GetNodeFiles(pscp2Project, VSConstants.VSITEMID_ROOT);
            if (sccFiles.Count > 0 && sccFiles[0] != null && sccFiles[0].Length > 0)
            {
                return sccFiles[0];
            }

            // If that failed, attempt to get a name from the IVsProject interface
            string bstrMKDocument;
            if (project.GetMkDocument(VSConstants.VSITEMID_ROOT, out bstrMKDocument) == VSConstants.S_OK &&
                bstrMKDocument != null && bstrMKDocument.Length > 0)
            {
                return bstrMKDocument;
            }

            // If that fails, attempt to get the filename from the solution
            IVsSolution sol = (IVsSolution)GetService(typeof(SVsSolution));
            string uniqueName;
            if (sol.GetUniqueNameOfProject(hierProject, out uniqueName) == VSConstants.S_OK &&
                uniqueName != null && uniqueName.Length > 0)
            {
                // uniqueName may be a full-path or may be relative to the solution's folder
                if (uniqueName.Length > 2 && uniqueName[1] == ':')
                {
                    return uniqueName;
                }

                // try to get the solution's folder and relativize the project name to it
                string solutionDirectory, solutionFile, solutionUserOptions;
                if (sol.GetSolutionInfo(out solutionDirectory, out solutionFile, out solutionUserOptions) == VSConstants.S_OK)
                {
                    uniqueName = solutionDirectory + "\\" + uniqueName;

                    // UNDONE: eliminate possible "..\\.." from path
                    return uniqueName;
                }
            }

            // If that failed, attempt to get the project name from 
            string bstrName;
            if (hierProject.GetCanonicalName(VSConstants.VSITEMID_ROOT, out bstrName) == VSConstants.S_OK)
            {
                return bstrName;
            }

            // if everything we tried fail, return null string
            return null;
        }

        /// <summary>
        /// Returns a list of source controllable files associated with the specified node
        /// </summary>
        public IList<string> GetNodeFiles(IVsHierarchy hier, uint itemid)
        {
            //object objOut;
            //hier.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_Parent, out objOut);
            //Type oType = objOut.GetType();
            //uint parentid = (uint)objOut;

            IVsSccProject2 pscp2 = hier as IVsSccProject2;
            return GetNodeFiles(pscp2, itemid);
        }

        /// <summary>
        /// Returns a list of source controllable files associated with the specified node
        /// </summary>
        public IList<string> GetNodeFiles(IVsSccProject2 pscp2, uint itemid)
        {
            // NOTE: the function returns only a list of files, containing both regular files and special files
            // If you want to hide the special files (similar with solution explorer), you may need to return 
            // the special files in a hashtable (key=master_file, values=special_file_list)

            // Initialize output parameters
            IList<string> sccFiles = new List<string>();
            if (pscp2 != null)
            {
                CALPOLESTR[] pathStr = new CALPOLESTR[1];
                CADWORD[] flags = new CADWORD[1];

                if (pscp2.GetSccFiles(itemid, pathStr, flags) == VSConstants.S_OK)
                {
                    if (pathStr[0].cElems > 0)
                    {
                        for (int elemIndex = 0; elemIndex < pathStr[0].cElems; elemIndex++)
                        {
                            IntPtr pathIntPtr = Marshal.ReadIntPtr(pathStr[0].pElems, elemIndex * IntPtr.Size);
                            String path = Marshal.PtrToStringAuto(pathIntPtr);

                            sccFiles.Add(path);

                            // See if there are special files
                            if (flags.Length > 0 && flags[0].cElems > 0)
                            {
                                int flag = Marshal.ReadInt32(flags[0].pElems, elemIndex * IntPtr.Size);

                                if (flag != 0)
                                {
                                    // We have special files
                                    CALPOLESTR[] specialFiles = new CALPOLESTR[1];
                                    CADWORD[] specialFlags = new CADWORD[1];

                                    if (pscp2.GetSccSpecialFiles(itemid, path, specialFiles, specialFlags) == VSConstants.S_OK)
                                    {
                                        for (int i = 0; i < specialFiles[0].cElems; i++)
                                        {
                                            IntPtr specialPathIntPtr = Marshal.ReadIntPtr(specialFiles[0].pElems, i * IntPtr.Size);
                                            String specialPath = Marshal.PtrToStringAuto(specialPathIntPtr);

                                            sccFiles.Add(specialPath);
                                            Marshal.FreeCoTaskMem(specialPathIntPtr);
                                        }

                                        if (specialFiles[0].cElems > 0)
                                        {
                                            Marshal.FreeCoTaskMem(specialFiles[0].pElems);
                                        }
                                    }
                                }
                            }
                            Marshal.FreeCoTaskMem(pathIntPtr);
                        }
                    }
                    else
                    {
                        // This is a special file, so the GetSccFiles will not return it's path, 
                        // so get it directly from the project
                        string path = string.Empty;
                        IVsProject pscp = pscp2 as IVsProject;

                        if (pscp != null) pscp.GetMkDocument(itemid, out path);

                        // can't be null, blank or end with back slash (that's a directory)
                        if ((string.IsNullOrEmpty(path) == false) && (path.EndsWith("\\") == false) &&
                            (SccService.ScmProvider.IsFileCached(path)))
                        {
                            //got a valid path
                            sccFiles.Add(path);
                        }
                    }
                }
            }

            return sccFiles;
        }

        /// <summary>
        /// Checks whether a solution exist
        /// </summary>
        /// <returns>True if a solution was created.</returns>
        internal bool IsThereASolution()
        {
            bool status = true;
            if (GetSolutionFileName() == null || CurrentScm == null)
            {
                status = false;
            }
            else
            {
                status = CurrentScm.Connected;
            }
            return status;
        }

        // Whether the solution was just added to source control and the provider needs to saved source control properties in the solution file when the solution is saved
        private bool _solutionHasDirtyProps = false;

        /// <summary>
        /// Returns whether source control properties must be saved in the solution file
        /// </summary>
        public bool SolutionHasDirtyProps
        {
            get { return _solutionHasDirtyProps; }
            set { _solutionHasDirtyProps = value; }
        }

        public bool SolutionFileTagged { get; set; }

        private void DebugWalkingNode(IVsHierarchy pHier, uint itemid)
        {
#if DEBUG_DB
			object property = null;
			if (pHier.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_Name, out property) == VSConstants.S_OK)
			{
				logger.Trace("Walking hierarchy node: {0}", (string)property);
			}
#endif
        }


    }
}
