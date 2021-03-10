using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Perforce.P4VS
{
    public abstract class SelectedNodes : CommandStatus
    {
        /// <summary>
        /// Gets the list of directly selected VSITEMSELECTION objects
        /// </summary>
        /// <returns>A list of VSITEMSELECTION objects</returns>
        public IList<VSITEMSELECTION> GetSelectedNodes()
        {
            // Retrieve shell interface in order to get current selection
            IVsMonitorSelection monitorSelection = this.GetService(typeof(IVsMonitorSelection)) as IVsMonitorSelection;
            Debug.Assert(monitorSelection != null, "Could not get the IVsMonitorSelection object from the services exposed by this project");
            if (monitorSelection == null)
            {
                throw new InvalidOperationException();
            }

            List<VSITEMSELECTION> selectedNodes = new List<VSITEMSELECTION>();
            IntPtr hierarchyPtr = IntPtr.Zero;
            IntPtr selectionContainer = IntPtr.Zero;
            try
            {
                // Get the current project hierarchy, project item, and selection container for the current selection
                // If the selection spans multiple hierarchies, hierarchyPtr is Zero
                uint itemid;
                IVsMultiItemSelect multiItemSelect = null;
                ErrorHandler.ThrowOnFailure(monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out multiItemSelect, out selectionContainer));

                if (itemid != VSConstants.VSITEMID_SELECTION)
                {
                    // We only care if there are nodes selected in the tree
                    if (itemid != VSConstants.VSITEMID_NIL)
                    {
                        if (hierarchyPtr == IntPtr.Zero)
                        {
                            // Solution is selected
                            VSITEMSELECTION vsItemSelection;
                            vsItemSelection.pHier = null;
                            vsItemSelection.itemid = itemid;
                            selectedNodes.Add(vsItemSelection);
                        }
                        else
                        {
                            IVsHierarchy hierarchy = (IVsHierarchy)Marshal.GetObjectForIUnknown(hierarchyPtr);
                            // Single item selection
                            VSITEMSELECTION vsItemSelection;
                            vsItemSelection.pHier = hierarchy;
                            vsItemSelection.itemid = itemid;
                            selectedNodes.Add(vsItemSelection);
                        }
                    }
                }
                else
                {
                    if (multiItemSelect != null)
                    {
                        // This is a multiple item selection.

                        //Get number of items selected and also determine if the items are located in more than one hierarchy
                        uint numberOfSelectedItems;
                        int isSingleHierarchyInt;
                        ErrorHandler.ThrowOnFailure(multiItemSelect.GetSelectionInfo(out numberOfSelectedItems, out isSingleHierarchyInt));
                        bool isSingleHierarchy = (isSingleHierarchyInt != 0);

                        // Now loop all selected items and add them to the list 
                        Debug.Assert(numberOfSelectedItems > 0, "Bad number of selected items");
                        if (numberOfSelectedItems > 0)
                        {
                            VSITEMSELECTION[] vsItemSelections = new VSITEMSELECTION[numberOfSelectedItems];
                            ErrorHandler.ThrowOnFailure(multiItemSelect.GetSelectedItems(0, numberOfSelectedItems, vsItemSelections));
                            foreach (VSITEMSELECTION vsItemSelection in vsItemSelections)
                            {
                                selectedNodes.Add(vsItemSelection);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
                if (selectionContainer != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainer);
                }
            }

            return selectedNodes;
        }


        /// <summary>
        /// Gets the list of selected controllable project hierarchies
        /// </summary>
        /// <returns>True if a solution was created.</returns>
        public Hashtable GetSelectedHierarchies(ref IList<VSITEMSELECTION> sel, out bool solutionSelected)
        {
            // Initialize output arguments
            solutionSelected = false;

            Hashtable mapHierarchies = new Hashtable();
            foreach (VSITEMSELECTION vsItemSel in sel)
            {
                if (vsItemSel.pHier == null ||
                    (vsItemSel.pHier as IVsSolution) != null)
                {
                    solutionSelected = true;
                }

                // See if the selected hierarchy implements the IVsSccProject2 interface
                // Exclude from selection projects like FTP web projects that don't support SCC
                IVsSccProject2 sccProject2 = vsItemSel.pHier as IVsSccProject2;
                if (sccProject2 != null)
                {
                    mapHierarchies[vsItemSel.pHier] = true;
                }
            }

            return mapHierarchies;
        }
    }
}
