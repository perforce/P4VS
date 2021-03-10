using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.P4VS
{
	public class TreeListViewItem : ListViewItem
	{ 
		internal bool Tagged { get; set; }
		internal List<TreeListViewItem> ChildNodes;

		public TreeListViewItem ParentItem { get; set; }
        public int action1 { get; set; }
        public int action2 { get; set; }
        public bool firmer { get; set; }
		public new bool Checked { get; set; }
		public bool FullLine { get; set; }
		public bool Expanded { get; set; }
		/// <summary>
		/// True if the item is visible in the tree because it is either a root node or
		/// it's parent is expanded
		/// </summary>
		//public bool Displayed { get; set; }

		public int treeStateImageIndex { get; set; }
		public IList<int> LeftImageIndices { get; set; }
		public IList<int> RightImageIndices { get; set; }
		public IList<int> CenterImageIndices { get; set; }

		public TreeListViewItem()
		{
			ParentItem = null;

			Tagged = false;
			FullLine = false;
			//Displayed = false;

			this.treeStateImageIndex = 0;
			ChildNodes = new List<TreeListViewItem>();

			LeftImageIndices = new List<int>();
			RightImageIndices = new List<int>();
			CenterImageIndices = new List<int>();
		}

		public TreeListViewItem(TreeListViewItem parentItem, string itemText, bool fullLine)
			: base(itemText)
		{
			ParentItem = parentItem;

			Tagged = false;
			FullLine = fullLine;
			//Displayed = false;

			this.treeStateImageIndex = 0;
			ChildNodes = new List<TreeListViewItem>();

			LeftImageIndices = new List<int>();
			RightImageIndices = new List<int>();
			CenterImageIndices = new List<int>();
		}

		public TreeListViewItem(TreeListViewItem parentItem, params string[] items)
			: base(items)
		{
			ParentItem = parentItem;

			Tagged = false;
			FullLine = false;
			//Displayed = false;

			this.treeStateImageIndex = 0;
			ChildNodes = new List<TreeListViewItem>();

			LeftImageIndices = new List<int>();
			RightImageIndices = new List<int>();
			CenterImageIndices = new List<int>();
		}

		public TreeListViewItem(TreeListViewItem parentItem, string[] items, int imageIndex)
			: base(items, imageIndex)
		{
			ParentItem = parentItem;

			Tagged = false;
			FullLine = false;
			//Displayed = false;

			this.treeStateImageIndex = 0;
			ChildNodes = new List<TreeListViewItem>();

			LeftImageIndices = new List<int>();
			RightImageIndices = new List<int>();
			CenterImageIndices = new List<int>();
		}

		public void Expand()
		{
			if (ChildNodes.Count <= 0)
			{
				return;
			}

			this.treeStateImageIndex = 2;
		}

		public void Collapse()
		{
			if (ChildNodes.Count <= 0)
			{
				return;
			}

			this.treeStateImageIndex = 1;
		}
	}
}
