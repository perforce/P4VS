using System.Collections;
using System.Windows.Forms;
using Perforce.P4VS;
using System;

public abstract class ColumnSorter : IComparer
{
	/// <summary>
	/// Specifies the column to be sorted
	/// </summary>
	public int SortColumn;
	/// <summary>
	/// Specifies the order in which to sort (i.e. 'Ascending').
	/// </summary>
	public SortOrder Order;

	public abstract int Compare(object x, object y);
}

/// <summary>
/// This class is an implementation of the 'IComparer' interface.
/// </summary>
public class ListViewColumnSorter : ColumnSorter
{
    /// <summary>
    /// Case insensitive comparer object
    /// </summary>
    private CaseInsensitiveComparer ObjectCompare;

    /// <summary>
    /// Class constructor.  Initializes various elements
    /// </summary>
    public ListViewColumnSorter()
    {
        // Initialize the column to '0'
		SortColumn = 0;

        // Initialize the sort order to 'none'
        Order = SortOrder.None;

        // Initialize the CaseInsensitiveComparer object
        ObjectCompare = new CaseInsensitiveComparer();
    }

    /// <summary>
    /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
    /// </summary>
    /// <param name="x">First object to be compared</param>
    /// <param name="y">Second object to be compared</param>
    /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
    public override int Compare(object x, object y)
    {
        bool areWorkspaces = false;
        int compareResult;
        TreeListViewItem listviewX, listviewY;
        ListViewItem workspaceX, workspaceY;
        workspaceX = (ListViewItem)x;
        workspaceY = (ListViewItem)y;

        // Cast the objects to be compared to ListViewItem objects
        listviewX = x as TreeListViewItem;
        if (listviewX != null && listviewX.ParentItem != null)
        {
            // If it is a file object or a shelved filed folder
            // use the parent item for the compare. First check
            // if it is a shelved file, then use the top parent
            // item for the compare.
            if (listviewX.ParentItem.ParentItem!=null)
            {
                listviewX = listviewX.ParentItem.ParentItem;
            }
            else
            {
                listviewX = listviewX.ParentItem;
            }
        }
        else
        {
            listviewX = x as TreeListViewItem;
        }
        if (listviewX==null)
        {
            areWorkspaces = true;
            workspaceX = (ListViewItem) x;
        }
        listviewY = y as TreeListViewItem;
        if (listviewY != null && listviewY.ParentItem != null)
        {
            // If it is a file object or a shelved filed folder
            // use the parent item for the compare. First check
            // if it is a shelved file, then use the top parent
            // item for the compare.
            if (listviewY.ParentItem.ParentItem != null)
            {
                listviewY = listviewY.ParentItem.ParentItem;
            }
            else
            {
                listviewY = listviewY.ParentItem;
            }
        }
        else
        {
            listviewY = y as TreeListViewItem;
        }
        if (listviewY == null)
        {
            areWorkspaces = true;
            workspaceY = (ListViewItem)y;
        }

        // Compare the two items
        long numX;
        long numY;
        DateTime timeX;
        DateTime timeY;

        if (areWorkspaces)
        {
			if (long.TryParse(workspaceX.SubItems[SortColumn].Text, out numX) && long.TryParse(workspaceY.SubItems[SortColumn].Text, out numY))
            {
                compareResult = ObjectCompare.Compare(numX, numY);
            }
			else if (DateTime.TryParse(workspaceX.SubItems[SortColumn].Text, out timeX) && DateTime.TryParse(workspaceY.SubItems[SortColumn].Text, out timeY))
            {
                compareResult = ObjectCompare.Compare(timeX, timeY);
            }
            else
            {
				compareResult = ObjectCompare.Compare(workspaceX.SubItems[SortColumn].Text, workspaceY.SubItems[SortColumn].Text);
            }
        }
        else
        {
            // protect against out of range error if subitem does not exist
            if (listviewX.SubItems.Count>SortColumn && listviewY.SubItems.Count > SortColumn)
            {
                if (long.TryParse(listviewX.SubItems[SortColumn].Text, out numX) &&
    long.TryParse(listviewY.SubItems[SortColumn].Text, out numY))
                {
                    compareResult = ObjectCompare.Compare(numX, numY);
                }
                else if (DateTime.TryParse(listviewX.SubItems[SortColumn].Text, out timeX) &&
                    DateTime.TryParse(listviewY.SubItems[SortColumn].Text, out timeY))
                {
                    compareResult = ObjectCompare.Compare(timeX, timeY);
                }
                else
                {
                    compareResult = ObjectCompare.Compare(listviewX.SubItems[SortColumn].Text, listviewY.SubItems[SortColumn].Text);
                }
            }
            else
            {
                // Return '0' if the subitem does not exist
                return 0;
            }
        }

        // Calculate correct return value based on object comparison
        if (Order == SortOrder.Ascending)
        {
            // Ascending sort is selected, return normal result of compare operation
            return compareResult;
        }
        else if (Order == SortOrder.Descending)
        {
            // Descending sort is selected, return negative result of compare operation
            return (-compareResult);
        }
        else
        {
            // Return '0' to indicate they are equal
            return 0;
        }
    }

}