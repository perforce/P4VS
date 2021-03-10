using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;

using System.Runtime.InteropServices;
using System.Threading;

namespace Perforce.P4VS
{
	public class TreeListView : ListView
	{
		public int IndentSize
		{
			get
			{
				if (SmallImageList != null)
				{
					return SmallImageList.ImageSize.Width;
				}
				if (StateImageList != null)
				{
					return StateImageList.ImageSize.Width;
				}
				return 16;
			}
		}

		//private ImageList currenttStateImageList;
		public bool TreeView { get; set; }
		public new bool CheckBoxes { get; set; }

        // check boxes on the root items only
        public bool RootCheckBoxes { get; set; }

		internal ImageList defaultStateImageList;
        internal ImageList classicStateImageList;

        public bool EnableSorting { get; set; }
        public int ActionColumn { get; set; }
		public bool EnableIconOverlays { get; set; }
		public int OverlayOffset { get; set; }
        public int _maxLineOffset { get; set; }

		public ImageList LeftImageList;
		public ImageList RightImageList;

        internal ImageList checkboxImageList;

		private ListViewColumnSorter lvwColumnSorter;
		private ContextMenuStrip ChooseColumnsCtxMnu;

		public bool UseClassicImageList
		{
			get { return StateImageList == classicStateImageList; }
			set
			{
				if (value)
				{
					StateImageList = classicStateImageList;
				}
				else
				{
					StateImageList = defaultStateImageList;
				}
			}
		}
		private System.ComponentModel.IContainer components;
		// Make View read only (Detail view only)
		public new View View
		{
			get { return View.Details; }
			set { base.View = View.Details; }
		}

		public TreeListView()
		{
            ActionColumn = -1;
            EnableSorting = true;
			AllowColumnReorder = true;
			TreeView = true;
			Nodes = new List<TreeListViewItem>();

			InitializeComponent();

			defaultStateImageList = new System.Windows.Forms.ImageList(components);
			classicStateImageList = new System.Windows.Forms.ImageList(components);
			checkboxImageList = new System.Windows.Forms.ImageList(components);
			// 
			// defaultStateImageList
			// 
			defaultStateImageList.TransparentColor = System.Drawing.Color.White;
			defaultStateImageList.Images.Add("blank.png", Images.blank);
			defaultStateImageList.Images.Add("collapsed.png", Images.collapsed);
			defaultStateImageList.Images.Add("expanded.png", Images.expanded);
			// 
			// classicStateImageList
			// 
			classicStateImageList.TransparentColor = System.Drawing.Color.Transparent;
			classicStateImageList.Images.Add("blank.png", Images.blank);
			classicStateImageList.Images.Add("plus.png", Images.plus);
			classicStateImageList.Images.Add("minus.png", Images.minus);
			// 
			// checkboxImageList
			// 
			checkboxImageList.TransparentColor = System.Drawing.Color.Transparent;
			checkboxImageList.Images.Add("noCheckBox.png", Images.noCheckBox);
			checkboxImageList.Images.Add("CheckBox.png", Images.CheckBox);

			StateImageList = defaultStateImageList;

			// Create an instance of a ListView column sorter and assign it 
			// to the ListView control.
			lvwColumnSorter = new ListViewColumnSorter();
			this.ListViewItemSorter = lvwColumnSorter;

			DoubleBuffered = true;

			//OverlayOffset = 3;

			ColumnContextMenuClicked += new ColumnContextMenuHandler(OnColumnContextMenuClicked);
		}

		public IList<TreeListViewItem> Nodes {get; private set;}

		protected override void InitLayout()
		{
			this.OwnerDraw = true;
			this.View = View.Details;
			this.DoubleBuffered = true;

			this.DrawItem += new DrawListViewItemEventHandler(OnDrawItem);
			this.DrawSubItem += new DrawListViewSubItemEventHandler(OnDrawSubItem);
            this.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(OnDrawColumnHeader);

			// Add handlers for various events to compensate for an 
			// extra DrawItem event that occurs the first time the mouse 
			// moves over each row. 
			this.MouseMove += new MouseEventHandler(OnMouseMove);
			this.ColumnWidthChanged += new ColumnWidthChangedEventHandler(OnColumnWidthChanged);
			this.Invalidated += new InvalidateEventHandler(OnInvalidated);

			this.MouseClick +=new MouseEventHandler(OnMouseClick);

			//currenttStateImageList = defaultStateImageList;

			base.InitLayout();
        }

        protected override void OnNotifyMessage(Message message)
		{
			//Filter out the WM_ERASEBKGND message
			if (message.Msg != 0x14)
			{
				base.OnNotifyMessage(message);
			}
		}

		//protected override void WndProc(ref Message message)
		//{
		//    //Filter out the WM_ERASEBKGND message
		//    if (message.Msg != 0x14)
		//    {
		//        base.WndProc(ref message);
		//    }
		//}
 
        private static Color InvertColor(Color c)
        {
            Int32 argb = c.ToArgb();
            argb ^= 0x00FFFFFF;

           return Color.FromArgb(argb);
        }

        // Draws the backgrounds for entire ListView items.
        private void OnDrawItem(object sender, DrawListViewItemEventArgs e)
		{
		    _maxLineOffset = 0;
			TreeListViewItem tlvi = e.Item as TreeListViewItem;

			if (tlvi != null && ((tlvi.Tagged) && ((e.State & ListViewItemStates.Selected) == 0)))
				return;

			if (tlvi != null)
			{
				tlvi.Tagged = true;

				bool fullLine = tlvi.FullLine;
			}

			Rectangle bounds = e.Bounds;
			if (TreeView)
			{
				if (tlvi != null && tlvi.IndentCount > 0)
				{
					bounds.X += IndentSize * tlvi.IndentCount;
					bounds.Width -= IndentSize * tlvi.IndentCount;

				}
				if (tlvi != null && ((StateImageList != null) && (tlvi.treeStateImageIndex >= 0)))
				{
					bounds.X += StateImageList.ImageSize.Width;
					bounds.Width -= StateImageList.ImageSize.Width;
				}
			}
			if ((CheckBoxes) && (checkboxImageList != null))
			{
				bounds.X += checkboxImageList.ImageSize.Width;
				bounds.Width -= checkboxImageList.ImageSize.Width;
			}
            else if ((RootCheckBoxes) && (checkboxImageList != null) && (tlvi != null)
                && (tlvi.IndentCount == 0)&&(tlvi.ParentItem==null))
            {
                bounds.X += checkboxImageList.ImageSize.Width;
                bounds.Width -= checkboxImageList.ImageSize.Width;
            }
			if (SmallImageList != null)
			{
				if (EnableIconOverlays)
				{
					bounds.X += (2 * OverlayOffset) + SmallImageList.ImageSize.Width;
					bounds.Width -= (2 * OverlayOffset) + SmallImageList.ImageSize.Width;
				}
				else if (tlvi != null && tlvi.ImageIndex >= 0)
				{
					bounds.X += SmallImageList.ImageSize.Width;
					bounds.Width -= SmallImageList.ImageSize.Width;
				}
			}
			if (bounds.X > e.Bounds.Width)
			{
				bounds.X = e.Bounds.Width;
			}
			if (bounds.Width < 0)
			{
				bounds.Width = 0;
			}

			Color BkColor = base.BackColor;
            if (e.Item.Selected)
            {
                // Draw the background and focus rectangle for a selected item.
#if VS2012
                BkColor = InvertColor(BkColor);
#else
                BkColor = SystemColors.Highlight;
#endif
            }
            using (Brush brush = new SolidBrush(BkColor))
			{
				e.Graphics.FillRectangle(brush, bounds);
				//e.DrawFocusRectangle();
			}

            // Draw the item text for views other than the Details view.
            if (base.View != View.Details)
            {
                e.DrawText();
            }
		}

        // Draws subitem text and applies content-based formatting.
        private void OnDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            TreeListViewItem tlvi = e.Item as TreeListViewItem;

            Color BkColor = base.BackColor;
            Color FgColor = base.ForeColor;

            Rectangle bounds = e.Bounds;

            if (e.ColumnIndex == 0)
            {
                if (TreeView)
                {
                    if (tlvi != null && tlvi.IndentCount > 0)
                    {
                        bounds.X += IndentSize * tlvi.IndentCount;
                        bounds.Width -= IndentSize * tlvi.IndentCount;

                    }
                    if (tlvi != null && ((StateImageList != null) && (tlvi.treeStateImageIndex >= 0)))
                    {
                        if (bounds.X <= (e.Bounds.Width - StateImageList.ImageSize.Width))
                        {
                            base.StateImageList.Draw(e.Graphics, bounds.X, bounds.Y, tlvi.treeStateImageIndex);
                        }
                        bounds.X += StateImageList.ImageSize.Width;
                        bounds.Width -= StateImageList.ImageSize.Width;
                    }
                }
                if ((CheckBoxes) && (checkboxImageList != null))
                {
                    if (bounds.X <= (e.Bounds.Width - checkboxImageList.ImageSize.Width))
                    {
                        checkboxImageList.Draw(e.Graphics, bounds.X, bounds.Y, tlvi != null && tlvi.Checked ? 1 : 0);
                    }
                    bounds.X += checkboxImageList.ImageSize.Width;
                    bounds.Width -= checkboxImageList.ImageSize.Width;
                }
                else if ((RootCheckBoxes) && (checkboxImageList != null) && (tlvi != null)
                    && (tlvi.IndentCount == 0) && (tlvi.ParentItem == null))
                {
                    if (bounds.X <= (e.Bounds.Width - checkboxImageList.ImageSize.Width))
                    {
                        checkboxImageList.Draw(e.Graphics, bounds.X, bounds.Y, tlvi != null && tlvi.Checked ? 1 : 0);
                    }
                    bounds.X += checkboxImageList.ImageSize.Width;
                    bounds.Width -= checkboxImageList.ImageSize.Width;
                }
                if (SmallImageList != null)
                {
                    if (EnableIconOverlays)
                    {
                        int iconWidth = SmallImageList.ImageSize.Width;
                        if (bounds.X <= (e.Bounds.Width - (2 * OverlayOffset + iconWidth)))
                        {
                            if (tlvi != null && tlvi.ImageIndex >= 0)
                            {
                                base.SmallImageList.Draw(e.Graphics, bounds.X + OverlayOffset, bounds.Y, tlvi.ImageIndex);
                            }
                            if (tlvi != null && ((tlvi.LeftImageIndices != null) && (LeftImageList != null)))
                            {
                                foreach (int idx in tlvi.LeftImageIndices)
                                {
                                    LeftImageList.Draw(e.Graphics, bounds.X, bounds.Y, idx);
                                }
                            }
                            if (tlvi != null && ((tlvi.RightImageIndices != null) && (RightImageList != null)))
                            {
                                foreach (int idx in tlvi.RightImageIndices)
                                {
                                    RightImageList.Draw(e.Graphics, bounds.X + (2 * OverlayOffset), bounds.Y, idx);
                                }
                            }
                            if (tlvi != null && ((tlvi.CenterImageIndices != null) && (SmallImageList != null)))
                            {
                                foreach (int idx in tlvi.CenterImageIndices)
                                {
                                    base.SmallImageList.Draw(e.Graphics, bounds.X + OverlayOffset, bounds.Y, idx);
                                }
                            }
                        }
                        bounds.X += 2 * SmallImageList.ImageSize.Width;
                        bounds.Width -= (2 * OverlayOffset) + SmallImageList.ImageSize.Width;
                    }
                    else if (tlvi != null && tlvi.ImageIndex >= 0)

                    {
                        if (bounds.X <= (e.Bounds.Width - SmallImageList.ImageSize.Width))
                        {
                            base.SmallImageList.Draw(e.Graphics, bounds.X, bounds.Y, tlvi.ImageIndex);
                        }
                        bounds.X += SmallImageList.ImageSize.Width;
                        bounds.Width -= SmallImageList.ImageSize.Width;
                    }
                }
                if (bounds.X > e.Bounds.Width)
                {
                    bounds.X = e.Bounds.Width;
                }
                if (bounds.Width < 0)
                {
                    bounds.Width = 0;
                }
                if (bounds.Width > _maxLineOffset)
                {
                    _maxLineOffset = bounds.Width;
                }

            }
            if (e.Item.Selected)
            {
#if VS2012
                BkColor = InvertColor(BkColor);
                FgColor = InvertColor(FgColor);
#else
                BkColor = SystemColors.Highlight;
				FgColor = SystemColors.HighlightText;
#endif
            }

            // drawing items for action column
            if (ActionColumn > -1)
            {
                if (e.Header.DisplayIndex == ActionColumn)
                {
                    TreeListViewItem actionItem = e.Item as TreeListViewItem;
                    if (actionItem.action1 > -1)
                    {
                        var imageRect = new Rectangle(e.Bounds.X + 10, e.Bounds.Y, e.Bounds.Height, e.Bounds.Height);
                        e.Graphics.DrawImage(SmallImageList.Images[actionItem.action1], imageRect);
                    }

                    if (actionItem.action2 > -1)
                    {
                        var imageRect = new Rectangle(e.Bounds.X + 28, e.Bounds.Y, e.Bounds.Height, e.Bounds.Height);
                        e.Graphics.DrawImage(SmallImageList.Images[actionItem.action2], imageRect);

                    }
                }
            }

            if (bounds.Width > 0)
            {
                // Draw normal text for a subitem 
                //e.DrawText(flags);
                using (StringFormat sf = new StringFormat())
                {
                    string replaceLineBreaksWithSpaces = e.SubItem.Text;
                    replaceLineBreaksWithSpaces = replaceLineBreaksWithSpaces.TrimEnd('\n', '\r');
                    replaceLineBreaksWithSpaces = replaceLineBreaksWithSpaces.TrimStart('\n', '\r');
                    replaceLineBreaksWithSpaces = replaceLineBreaksWithSpaces.Replace('\n', ' ');
                    replaceLineBreaksWithSpaces = replaceLineBreaksWithSpaces.Replace("\r", "");

                    using (Font f = new Font(base.Font, FontStyle.Regular))
                    {
                        sf.Alignment = StringAlignment.Near;
                        sf.LineAlignment = StringAlignment.Far;
                        if (tlvi != null && tlvi.FullLine)
                        {
                            sf.Trimming = StringTrimming.None;
                            sf.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
                        }
                        else
                        {
                            sf.Trimming = StringTrimming.EllipsisCharacter;
                            sf.FormatFlags = StringFormatFlags.NoWrap;
                        }
                        if (e.Item.ForeColor == Color.Gray)
                        {
                            e.Graphics.DrawString(replaceLineBreaksWithSpaces, f,
                            Brushes.Gray, bounds, sf);
                        }
                        else
                        {
                            e.Graphics.DrawString(replaceLineBreaksWithSpaces, f,
                                new SolidBrush(FgColor), bounds, sf);
                        }
                    }
                }
            }
        }

		private int _headerHeight = 24;

		// Draws column headers.
		private void OnDrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
            if (HeadersResized)
            {
                ResizeHeaders();
            }

            int top = e.Bounds.Top;
			int left = e.Bounds.Left;
			int right = e.Bounds.Right;
			int center = left + ((right - left) / 2) ;

            Color BgColor = base.BackColor;
            Color FgColor = base.ForeColor;

            using (Brush fgBrush = new SolidBrush(FgColor), bgBrush = new SolidBrush(BgColor))
            {

                _headerHeight = e.Bounds.Height;

                using (StringFormat sf = new StringFormat())
                {
                    using (Pen pen = new Pen(FgColor, 1))
                    {
                        sf.Alignment = StringAlignment.Near;
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Trimming = StringTrimming.EllipsisCharacter;
                        sf.FormatFlags = StringFormatFlags.NoWrap;

                        //e.DrawBackground();

                        Rectangle bounds = e.Bounds;

                        //if(e.Header.DisplayIndex >= e.Header.ListView.Columns.Count-1)
                        //{
                        //    // Last header on the right. May need to expand the with to include
                        //    // all the space to the left when we erease the background
                        //    if (e.Bounds.Right < e.Header.ListView.Right)
                        //    {
                        //        bounds.Width +=(e.Header.ListView.Right - e.Bounds.Right)+1;
                        //    }
                        //}
                        // Draw the standard header background.
                        e.Graphics.FillRectangle(bgBrush, bounds);
                        Rectangle borderRect = e.Bounds;
                        borderRect.Width -= 1;
                        borderRect.Height -= 1;
                        e.Graphics.DrawRectangle(pen, borderRect);

                        //e.DrawText();
                        e.Graphics.DrawString(e.Header.Text, base.Font,
                            fgBrush, e.Bounds, sf);
                    }
                }
                if (EnableSorting)
                {
                    // Draw the triangle
                    // Create pen.
                    using (Pen pen = new Pen(FgColor, 3))
                    {
                        Point[] curvePoints = new Point[]
                                              {
                                              new Point(0, 0),
                                              new Point(0, 0),
                                              new Point(0, 0)
                                              };
                        if (lvwColumnSorter.SortColumn == e.ColumnIndex)
                        {
                            if (lvwColumnSorter != null && lvwColumnSorter.Order == SortOrder.Descending)
                            {
                                // Create triangle pointing down.
                                Point point1 = new Point(center, top + 3);
                                Point point2 = new Point(center + 2, top + 1);
                                Point point3 = new Point(center - 2, top + 1);
                                curvePoints = new Point[]
                                              {
                                          point1,
                                          point2,
                                          point3,
                                              };
                                // Draw polygon to screen.
                                e.Graphics.DrawPolygon(pen, curvePoints);
                            }
                            if (lvwColumnSorter != null && lvwColumnSorter.Order == SortOrder.Ascending)
                            {
                                // Create triangle pointing up.
                                Point point4 = new Point(center, top + 1);
                                Point point5 = new Point(center + 2, top + 3);
                                Point point6 = new Point(center - 2, top + 3);
                                curvePoints = new Point[]
                                              {
                                          point4,
                                          point5,
                                          point6
                                              };
                                // Draw polygon to screen.
                                e.Graphics.DrawPolygon(pen, curvePoints);
                            }

                        }
                    }
                }
            }
			return;
		}
		// Forces each row to repaint itself the first time the mouse moves over 
		// it, compensating for an extra DrawItem event sent by the wrapped 
		// Win32 control. This issue occurs each time the ListView is invalidated.
		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			TreeListViewItem item = base.GetItemAt(e.X, e.Y) as TreeListViewItem;
			if (item != null && !item.Tagged)
			{
				this.Invalidate(item.Bounds);
				item.Tagged = true;
			}
		}

		public event TreeListViewEvent AfterCollapse;
		public event TreeListViewEvent AfterExpand;
		public event TreeListViewEvent BeforeCollapse;
		public event TreeListViewEvent BeforeExpand;

		private void OnMouseClick(object sender, MouseEventArgs e)
		{
			//Trace.TraceInformation("/r/nEntering mouse click event, ({0},{1})", e.X,e.Y);
			TreeListViewItem item = base.GetItemAt(e.X, e.Y) as TreeListViewItem;

			int x = 0;

			if (item == null)
			{
				return;
			}
			// if this item has a subtree, see if the user clicked on the state
			// icon, an if so, expand/collapse the node;
			if ((TreeView) && (StateImageList != null) && (item.ChildNodes.Count > 0))
			{
				BeginUpdate();
				try
				{
					if (item.IndentCount > 0)
					{
						x += IndentSize * item.IndentCount;
						//Trace.TraceInformation("Indent, ({0}:{1})", item.IndentCount, x);
					}
					//Trace.TraceInformation("Hit test expand button range: ({0}:{1})", x, x + StateImageList.ImageSize.Width);
					if ((e.X >= x) && (e.X <= (x + StateImageList.ImageSize.Width)))
					{
						SelectedListViewItemCollection selection = this.SelectedItems;
						TreeListViewItem top = TopItem as TreeListViewItem;

						//Trace.TraceInformation("Clicked on expand button");

						TreeListViewEventArgs args =
							new TreeListViewEventArgs(item.Expanded ? TreeListViewAction.Expand : TreeListViewAction.Collapse, item);
						if (item.Expanded==false)
						{
							if (BeforeExpand != null)
							{
								//Trace.TraceInformation("Calling BeforeExpand()");
								BeforeExpand(this, args);
							}
							ExpandNode(item);
						}
						else
						{
							if (BeforeCollapse != null)
							{
								//Trace.TraceInformation("Calling BeforeCollapse()");
								BeforeCollapse(this, args);
							}
							CollapseNode(item);
						}

						//BuildTreeList();
						ensureSelectionVisible(selection, top, item);

						if (item.Expanded)
						{
							if (AfterExpand != null)
							{
								//Trace.TraceInformation("Calling AfterExpand()");
								AfterExpand(this, args);
							}
						}
						else
						{
							if (AfterCollapse != null)
							{
								//Trace.TraceInformation("Calling AfterCollapse()");
								AfterCollapse(this, args);
							}
						}

						return;
					}
					//else
					//{
					//    Trace.TraceInformation("Didn't click on expand button");
					//}
					x += StateImageList.ImageSize.Width;
				}
				finally
				{
					EndUpdate();
				}
			}
			if ((CheckBoxes) && (checkboxImageList != null))
			{
				//Trace.TraceInformation("Hit test expand button range: ({0}:{1})", x, x + checkboxImageList.ImageSize.Width);
				if ((e.X >= x) && (e.X <= (x + checkboxImageList.ImageSize.Width)))
				{
					// clicked on the check box
					item.Checked = !item.Checked;

					//Trace.TraceInformation("Clicked on checkbox, item is now {0}",item.Checked?"checked":"unchecked");

					ItemCheckedEventArgs ce = new ItemCheckedEventArgs(item);
					OnItemChecked(ce);

					Refresh();
				}
				//else
				//{
				//    Trace.TraceInformation("Didn't click on checkbox");
				//}
				if (StateImageList != null) x += StateImageList.ImageSize.Width;
				x += checkboxImageList.ImageSize.Width;
			}
            else if ((RootCheckBoxes) && (checkboxImageList != null) && (item != null)
                && (item.IndentCount == 0) && (item.ParentItem == null))
            {
                //Trace.TraceInformation("Hit test expand button range: ({0}:{1})", x, x + checkboxImageList.ImageSize.Width);
                if ((e.X >= x) && (e.X <= (x + checkboxImageList.ImageSize.Width)))
                {
                    // clicked on the check box
                    item.Checked = !item.Checked;

                    //Trace.TraceInformation("Clicked on checkbox, item is now {0}",item.Checked?"checked":"unchecked");

                    ItemCheckedEventArgs ce = new ItemCheckedEventArgs(item);
                    OnItemChecked(ce);

                    Refresh();
                }
                //else
                //{
                //    Trace.TraceInformation("Didn't click on checkbox");
                //}
                if (StateImageList != null) x += StateImageList.ImageSize.Width;
                x += checkboxImageList.ImageSize.Width;
            }
			// toggle selected flag
			//item.Selected = !item.Selected;
		}

		private void ensureSelectionVisible(SelectedListViewItemCollection selection, 
											TreeListViewItem top,
											TreeListViewItem item)
		{
			try
			{
				if ((selection != null) && (selection.Count > 0))
				{
					foreach (TreeListViewItem lvi in selection)
					{
						if ((lvi != null) && (Items.IndexOf(lvi) >= 0))
						{
							lvi.Selected = true;
						}
					}
				}
				if (top != null)
				{
					while ((top != null) && (Items.IndexOf(top) < 0))
					{
						top = top.ParentItem;
					}
					if (top == null)
					{
						top = item;
					}
					if (top != null)
					{
						try
						{
							TopItem = top;
						}
						catch
						{
							//Sometimes setting TopItem throws, so fall back to this method 
							// to scroll back to the previous top back on top
							Items[Items.Count - 1].EnsureVisible();
							top.EnsureVisible();
						}
					}
				}
				if (item != null)
				{
					if (item.Expanded && (item.ChildNodes != null) && (item.ChildNodes.Count > 0))
					{
						item.ChildNodes[item.ChildNodes.Count - 1].EnsureVisible();
						//item.ChildNodes[0].EnsureVisible();
						item.EnsureVisible();
					}
				}
			}
			catch (Exception ex)
			{
				FileLogger.LogException("here", ex);
			}
		}

		// Resets the item tags. 
		void OnInvalidated(object sender, InvalidateEventArgs e)
		{
			if (Updating)
			{
				return;
			}
			foreach (TreeListViewItem item in base.Items)
			{
				if (item != null)
				{
					item.Tagged = false;
				}
			}
		}

        bool HeadersResized = false;

		// Forces the entire control to repaint if a column width is changed.
		void OnColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
		{
            //if (this.Columns[0].Width < _maxLineOffset)
            //    {
            //        this.Columns[0].Width = _maxLineOffset;
            //    }
            if (this.Columns[0].Width < 64)
            {
                this.Columns[0].Width = 64;
            }

            //ResizeHeaders();

            base.Invalidate();

            HeadersResized = true;
        }

		// Forces the entire control to repaint column order is changed.
		void OnColumnReordered(object sender, ColumnReorderedEventArgs e)
		{
            {
                if (e.NewDisplayIndex == 0 | e.OldDisplayIndex == 0)
                {
                    e.Cancel = true;
                }
            }
			base.Invalidate();
		}

		public void BuildTreeList()
		{
			try
			{
				BeginUpdate();
				// reset which tree nodes are displayed in the tree
				//foreach (TreeListViewItem item in Items)
				//{
				//    item.Displayed = false;
				//}

				Items.Clear();

				AddNodesFromTree(Nodes, 0);
			}
			finally
			{
				EndUpdate();
			}
		}

		private void AddNodesFromTree(IList<TreeListViewItem> nodes, int indentLevel)
		{
			try
			{
				BeginUpdate();
				for (int idx = 0; idx < nodes.Count; idx++)
				{
					nodes[idx].IndentCount = indentLevel;
					////nodes[idx].Displayed = true;

					if (nodes[idx].ChildNodes.Count <= 0)
					{
						nodes[idx].treeStateImageIndex = 0;
					}
					else if (nodes[idx].Expanded)
					{
						nodes[idx].treeStateImageIndex = 2;
					}
					else
					{
						nodes[idx].treeStateImageIndex = 1;
					}

                    // if an item exists already, overwrite it
                    if (Items.Contains(nodes[idx]))
                    {
                        Items.Remove(nodes[idx]);
                    }

					Items.Add(nodes[idx]);
					if ((nodes[idx].ChildNodes.Count > 0) && (nodes[idx].Expanded))
					{
						AddNodesFromTree(nodes[idx].ChildNodes, indentLevel + 1);
					}
				}
			}
			finally
			{
				EndUpdate();
			}
		}

		public void ExpandNode(TreeListViewItem item)
		{
			if (item.Expanded)
			{
				return;
			}
			// need to check again to see if item has children, after the call to BeforeExpand for
			// views that dynamically add/remove children.
			if (item.ChildNodes.Count > 0)
			{
				// add the node's children to the list after the node 
				int listIdx = Items.IndexOf(item) + 1;
				AddChildNodes(item, item.IndentCount+1, ref listIdx);
				item.treeStateImageIndex = 2;
				item.Expanded = true;
//				item.Selected = true;
			}
			else
			{
				item.Expanded = true;
				item.treeStateImageIndex = 0;
//				item.Selected = true;
			}
		}

		public void CollapseNode(TreeListViewItem item)
		{
			if (item.Expanded == false)
			{
				return;
			}
			// remove the nodes children from the list
			RemoveChildNodes(item);
			item.treeStateImageIndex = 1;
			item.Expanded = false;
			this.Invalidate(item.Bounds);
		}

		private void AddChildNodes(TreeListViewItem parentNode, int indentLevel, ref int listIdx)
		{
			try
			{
				BeginUpdate();
				for (int idx = 0; idx < parentNode.ChildNodes.Count; idx++)
				{
					TreeListViewItem childNode = parentNode.ChildNodes[idx];
					childNode.IndentCount = indentLevel;
					//childNode.Displayed = true;

					if (childNode.ChildNodes.Count <= 0)
					{
						childNode.treeStateImageIndex = 0;
					}
					else if (parentNode.ChildNodes[idx].Expanded)
					{
						childNode.treeStateImageIndex = 2;
					}
					else
					{
						childNode.treeStateImageIndex = 1;
					}

					Items.Insert(listIdx++, childNode);

					if ((childNode.ChildNodes.Count > 0) && (childNode.Expanded))
					{
						AddChildNodes(childNode, indentLevel + 1, ref listIdx);
					}
				}
			}
			finally
			{
				EndUpdate();
			}
		}

		private void RemoveChildNodes(TreeListViewItem parentNode)
		{
			for (int idx = 0; idx < parentNode.ChildNodes.Count; idx++)
			{
				TreeListViewItem childNode = parentNode.ChildNodes[idx];

				if ((childNode.ChildNodes.Count > 0) && (childNode.Expanded))
				{
					RemoveChildNodes(childNode);
				}

				int childIdx = Items.IndexOf(childNode);
				if (childIdx >= 0)
				{
					Items.RemoveAt(childIdx);
				}
			}
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.ChooseColumnsCtxMnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // ChooseColumnsCtxMnu
            // 
            this.ChooseColumnsCtxMnu.Name = "ChooseColumnsCtxMnu";
            this.ChooseColumnsCtxMnu.Size = new System.Drawing.Size(61, 4);
            // 
            // TreeListView
            // 
            this.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.TreeListView_ColumnClick);
            this.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.OnColumnReordered);
            this.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.OnColumnWidthChanged);
            this.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.OnDrawColumnHeader);
            this.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.TreeListView_ItemSelectionChanged);
            this.SizeChanged += new System.EventHandler(this.TreeListView_SizeChanged);
            this.ResumeLayout(false);

		}

		public bool Updating
		{
			get
			{
				return (1 == Interlocked.CompareExchange(ref UpdatingLock, 1, 1));
			}
		}
		private int UpdatingLock = 0;

		public new void BeginUpdate()
		{
			//0 indicates that the control is not updating
			if (0 == Interlocked.CompareExchange(ref UpdatingLock, 1, 0))
			{
				base.BeginUpdate();
			}
		}

		public new void EndUpdate()
		{
			//1 indicates that the control is updating
			if (1 == Interlocked.CompareExchange(ref UpdatingLock, 0, 1))
			{
				base.EndUpdate();
			}
		}

		public delegate void ColumnContextMenuHandler(object sender, ColumnHeader columnHeader, Point loc);
		public event ColumnContextMenuHandler ColumnContextMenuClicked = null;

		public void OnColumnContextMenuItemClicked(Object sender, EventArgs e)
		{
			ToolStripMenuItem it = sender as ToolStripMenuItem;
			ColumnHeader h = it.Tag as ColumnHeader;

			if (it.Checked)
			{
				h.Width = 0;
			}
			else
			{
				h.Width = -2;
			}
		}

		public void OnColumnContextMenuClicked(object sender, ColumnHeader columnHeader, Point loc)
		{
			ChooseColumnsCtxMnu.Items.Clear();

			if ((this.Columns == null) || (this.Columns.Count <= 0))
			{
				return;
			}
			foreach (ColumnHeader h in this.Columns)
			{
				if (h == null)
				{
					continue;
				}
				ToolStripMenuItem it = new ToolStripMenuItem(h.Text, null, new EventHandler(OnColumnContextMenuItemClicked));
				it.Tag = h;
				it.Checked = h.Width > 0;
				ChooseColumnsCtxMnu.Items.Add(it);
			}

			ChooseColumnsCtxMnu.Show(loc);
		}

		bool _OnItemsArea = false;
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			_OnItemsArea = true;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			_OnItemsArea = false;
		}

		// Windows messages 
		private const int WM_PAINT = 0x000F;
		private const int WM_HSCROLL = 0x0114;
		private const int WM_VSCROLL = 0x0115;
		private const int WM_MOUSEWHEEL = 0x020A;
		private const int WM_KEYDOWN = 0x0100;
		private const int WM_LBUTTONUP = 0x0202;
		private const int WM_CONTEXTMENU = 0x007B;

		// ScrollBar types 
		private const int SB_HORZ = 0;
		private const int SB_VERT = 1;

		// ScrollBar interfaces 
		private const int SIF_TRACKPOS = 0x10;
		private const int SIF_RANGE = 0x01;
		private const int SIF_POS = 0x04;
		private const int SIF_PAGE = 0x02;
		private const int SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS;

		// ListView messages 
		private const uint LVM_SCROLL = 0x1014;
		private const int LVM_FIRST = 0x1000;
		private const int LVM_SETGROUPINFO = (LVM_FIRST + 147);

		public enum ScrollBarCommands : int
		{
			SB_LINEUP = 0,
			SB_LINELEFT = 0,
			SB_LINEDOWN = 1,
			SB_LINERIGHT = 1,
			SB_PAGEUP = 2,
			SB_PAGELEFT = 2,
			SB_PAGEDOWN = 3,
			SB_PAGERIGHT = 3,
			SB_THUMBPOSITION = 4,
			SB_THUMBTRACK = 5,
			SB_TOP = 6,
			SB_LEFT = 6,
			SB_BOTTOM = 7,
			SB_RIGHT = 7,
			SB_ENDSCROLL = 8
		}

		protected override void WndProc(ref Message message)
		{
			//Filter out the WM_ERASEBKGND message
			if (message.Msg != 0x14)
			{
				base.WndProc(ref message);
			}

			ScrollEventArgs sargs = null;
			int newPos = -1;

			switch (message.Msg)
			{
				case WM_CONTEXTMENU:
					Point p = base.PointToClient(MousePosition);

					if ((!_OnItemsArea) && (p.Y < _headerHeight))
					{
						int totalWidth = 0;
						ColumnHeader clicked = null;
						foreach (ColumnHeader column in base.Columns)
						{
							totalWidth += column.Width;
							if (p.X < totalWidth)
							{
								clicked = column;
								break;
							}
						}
						if (ColumnContextMenuClicked != null)
						{
							ColumnContextMenuClicked(this, clicked, MousePosition);
						}
					}
					break;

				case WM_VSCROLL:
					Int32 action = (Int32)(message.WParam.ToInt64() & 0x0000FFFF);
					if (action == (Int32)ScrollBarCommands.SB_ENDSCROLL)
					{
						newPos = GetScrollPos(this.Handle, SB_VERT);

						sargs = new ScrollEventArgs(ScrollEventType.EndScroll, newPos);
						OnScroll(this, sargs);
					}
					break;

				case WM_MOUSEWHEEL:
					{
						newPos = GetScrollPos(this.Handle, SB_VERT);

						sargs = new ScrollEventArgs(ScrollEventType.EndScroll, newPos);
					}
					break;

				case WM_KEYDOWN:
					Int32 key = message.WParam.ToInt32();
					if ((key == (int)Keys.Left) ||
						(key == (int)Keys.Right) ||
						(key == (int)Keys.Down) ||
						(key == (int)Keys.Up) ||
						(key == (int)Keys.PageDown) ||
						(key == (int)Keys.PageUp) ||
						(key == (int)Keys.Home) ||
						(key == (int)Keys.End)
						)
					{
						newPos = GetScrollPos(this.Handle, SB_VERT);

						switch (key)
						{
							case (int)Keys.Left:
								sargs = null;
								if ((SelectedItems != null) && (SelectedItems.Count > 0))
								{
									TreeListViewItem item = (TreeListViewItem)SelectedItems[0];
									if (item.Expanded)
									{
										SelectedListViewItemCollection selection = this.SelectedItems;
										TreeListViewItem top = TopItem as TreeListViewItem;

										if ((item.ChildNodes != null) && (item.ChildNodes.Count > 0))
										{
											TreeListViewEventArgs args =
												new TreeListViewEventArgs(item.Expanded ? TreeListViewAction.Expand : TreeListViewAction.Collapse, item);

											if (BeforeCollapse != null)
											{
												//Trace.TraceInformation("Calling BeforeCollapse()");
												BeforeCollapse(this, args);
											}
											CollapseNode(item);

											ensureSelectionVisible(selection, top, item);
											//SelectedItems.Clear();
											//item.Selected = true;

											if (AfterCollapse != null)
											{
												//Trace.TraceInformation("Calling AfterCollapse()");
												AfterCollapse(this, args);
											}
										}
										else if ((item.ParentItem != null) &&
											(item.ParentItem.ChildNodes != null) && (item.ParentItem.ChildNodes.Count > 0))
										{
											TreeListViewEventArgs args =
												new TreeListViewEventArgs(item.ParentItem.Expanded ? TreeListViewAction.Expand : TreeListViewAction.Collapse, item.ParentItem);

											if (BeforeCollapse != null)
											{
												//Trace.TraceInformation("Calling BeforeCollapse()");
												BeforeCollapse(this, args);
											}
											CollapseNode(item.ParentItem);

											ensureSelectionVisible(selection, top, item.ParentItem);
											//SelectedItems.Clear();
											//item.Selected = true;

											if (AfterCollapse != null)
											{
												//Trace.TraceInformation("Calling AfterCollapse()");
												AfterCollapse(this, args);
											}
										}
									}
								}
								break;
							case (int)Keys.Right:
								sargs = null;
								if ((SelectedItems != null) && (SelectedItems.Count > 0))
								{
									TreeListViewItem item = (TreeListViewItem)SelectedItems[0];
									if (item.Expanded==false)
									{
										SelectedListViewItemCollection selection = this.SelectedItems;
										TreeListViewItem top = TopItem as TreeListViewItem;

										if ((item.ChildNodes != null) && (item.ChildNodes.Count > 0))
										{
											TreeListViewEventArgs args =
												new TreeListViewEventArgs(item.Expanded ? TreeListViewAction.Expand : TreeListViewAction.Collapse, item);

											if (BeforeExpand != null)
											{
												//Trace.TraceInformation("Calling BeforeExpand()");
												BeforeExpand(this, args);
											}
											if (item.ChildNodes.Count==0)
											{
												item.treeStateImageIndex = 0;
												Invalidate(item.Bounds);
												break;
											}
											ExpandNode(item);

											ensureSelectionVisible(selection, top, item.ParentItem);
											//SelectedItems.Clear();
											//item.Selected = true;

											if (AfterExpand != null)
											{
												//Trace.TraceInformation("Calling AfterExpand()");
												AfterExpand(this, args);
											}
										}
									}
								}
								break;
							case (int)Keys.Down:
								sargs = new ScrollEventArgs(ScrollEventType.SmallDecrement, newPos);
								break;
							case (int)Keys.Up:
								sargs = new ScrollEventArgs(ScrollEventType.SmallIncrement, newPos);
								break;
							case (int)Keys.PageDown:
								sargs = new ScrollEventArgs(ScrollEventType.SmallDecrement, newPos);
								break;
							case (int)Keys.PageUp:
								sargs = new ScrollEventArgs(ScrollEventType.SmallIncrement, newPos);
								break;
							case (int)Keys.Home:
								sargs = new ScrollEventArgs(ScrollEventType.First, newPos);
								break;
							case (int)Keys.End:
								sargs = new ScrollEventArgs(ScrollEventType.Last, newPos);
								break;
						}
						if (sargs != null)
						{
							OnScroll(this, sargs);
						}
					}
					break;
			}
			if (sargs != null)
			{
				SCROLLINFO si = new SCROLLINFO();
				si.fMask = SIF_ALL;
				si.cbSize = (uint)Marshal.SizeOf(si);
				GetScrollInfo(message.HWnd, SB_VERT, ref si);

				PageSize = si.nPage;

				if (newPos >= (si.nMax - si.nPage))
				{
					OnMaxScroll(this, sargs);
				}
			}
		}

		public uint PageSize { get; private set; }
		public int ScrollPosition
		{
			get
			{
				return GetScrollPos(this.Handle, SB_VERT);
			}
			set
			{
				int prevPos;
				int scrollVal;

				if (ShowGroups == true)
				{
					prevPos = GetScrollPos(this.Handle, SB_VERT);
					scrollVal = -(prevPos - value);
				}
				else
				{
					scrollVal = value;
				}

				SendMessage(this.Handle, LVM_SCROLL, (IntPtr)0, (IntPtr)scrollVal);
			}
		}

		public event ScrollEventHandler onScroll;

		public virtual void OnScroll(object sender, ScrollEventArgs args)
		{
			if ((onScroll != null) && (onScroll.GetInvocationList().Length > 0))
			{
				foreach (ScrollEventHandler h in onScroll.GetInvocationList())
				{
					try
					{
						h(sender, args);
					}
					catch
					{
						onScroll -= h;
					}
				}
			}
		}

		public event ScrollEventHandler onMaxScroll;

		public virtual void OnMaxScroll(object sender, ScrollEventArgs args)
		{
			if ((onMaxScroll != null) && (onMaxScroll.GetInvocationList().Length > 0))
			{
				foreach (ScrollEventHandler h in onMaxScroll.GetInvocationList())
				{
					try
					{
						h(sender, args);
					}
					catch
					{
						onMaxScroll -= h;
					}
				}
			}
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

		[DllImport("user32.dll")]
		public static extern int SendMessage(
			  int hWnd,      // handle to destination window 
			  uint Msg,       // message 
			  long wParam,  // first message parameter 
			  long lParam   // second message parameter 
			  );

		[DllImport("user32.dll")]
		static extern int SendMessage(IntPtr hWnd, int wMsg,
									   int wParam, int lParam);

		[DllImport("user32.dll")]
		static extern int SendMessage(IntPtr hWnd, uint wMsg,
									   IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern int GetScrollPos(IntPtr hWnd, int nBar);


		[StructLayout(LayoutKind.Sequential)]
		struct SCROLLINFO
		{
			public uint cbSize;
			public uint fMask;
			public int nMin;
			public int nMax;
			public uint nPage;
			public int nPos;
			public int nTrackPos;
		}

		private void TreeListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// Determine if clicked column is already the column that is being sorted.
			if (e.Column == lvwColumnSorter.SortColumn)
			{
				// Reverse the current sort direction for this column.
				if (lvwColumnSorter.Order == SortOrder.Ascending)
				{
					lvwColumnSorter.Order = SortOrder.Descending;
				}
				else
				{
					lvwColumnSorter.Order = SortOrder.Ascending;
				}
			}
			else
			{
                // set the header text to what it already is to force a
                // redraw of the previously selected column header.
                this.Columns[lvwColumnSorter.SortColumn].Text =
                    this.Columns[lvwColumnSorter.SortColumn].Text;

                // Set the column number that is to be sorted; default to ascending.

                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			this.Sort();
		}

		public void OnClose(string PreferenceKey)
		{
			if (PreferenceKey != null)
			{
				Preferences.LocalSettings[PreferenceKey] = new ListHeaderSettings(Columns);
			}
		}

		public void OnLoad(string PreferenceKey)
		{
			if (PreferenceKey != null)
			{
				if (Preferences.LocalSettings.ContainsKey(PreferenceKey))
				{
					ListHeaderSettings settings = Preferences.LocalSettings[PreferenceKey] as ListHeaderSettings;
					if (settings != null)
					{
						for (int idx = 0; idx < Columns.Count && idx < settings.Count; idx++)
						{
							Columns[idx].Width = settings[idx].Width;
							Columns[idx].DisplayIndex = settings[idx].DisplayIndex;
						}
					}
				}
				else
				{
					bool gotOne = false;
					for (int idx = 0; idx < Columns.Count; idx++)
					{
						string key = string.Format("{0}_Column_{1}_Width", PreferenceKey, idx);

						if (Preferences.LocalSettings.ContainsKey(key))
						{
							int width = Preferences.LocalSettings.GetInt(key, -1);
							if (width > 0)
							{
								gotOne = true;
								Columns[idx].Width = width;
							}
							Preferences.LocalSettings.Remove(key); // delete old style settings
						}
					}
					if (gotOne)
					{
						OnClose(PreferenceKey); // save them in the new format
					}
				}
			}
		}

        //public new bool MultiSelect 
        //{
        //    get { return base.MultiSelect; }
        //    private set { base.MultiSelect = value; }
        //}
        [Flags]
        public enum MultiSelectCondition
        {
            none = 0x0000,
            SameClass = 0x0001, // only allow section of items with same class
            SameLevel = 0x0002, // only allow section of items on the same level of the tree
            SameParent = 0x0004, // only allow section of items on the same level of the tree
        }

        public MultiSelectCondition MultiSelectConditions { get; set; }

        //bool inTreeListView_ItemSelectionChanged = false;
        TreeListViewItem firstItemInSelection; 

        private void TreeListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if ((MultiSelect== false) || (e.IsSelected == false))
            {
                // action is unselect

                //TODO:
                //if its the first item, replace it with the new first item
                return;
            }
            if (SelectedItems.Count == 0)
            {
                // nothing left selected
                return;
            }
            if (SelectedItems.Count == 1)
            {
                // first item selected
                firstItemInSelection = e.Item as TreeListViewItem;
                return;
            }
            TreeListViewItem newItem = e.Item as TreeListViewItem;

            if (((MultiSelectConditions & MultiSelectCondition.SameClass) != 0) &&
                (firstItemInSelection.GetType() != newItem.GetType()))
            {
                e.Item.Selected = false;
            }
            if (((MultiSelectConditions & MultiSelectCondition.SameLevel) != 0) &&
                (firstItemInSelection.IndentCount != newItem.IndentCount))
            {
                e.Item.Selected = false;
            }
            if (((MultiSelectConditions & MultiSelectCondition.SameParent) != 0) &&
                (firstItemInSelection.ParentItem != newItem.ParentItem))
            {
                e.Item.Selected = false;
            }
        }

        private void TreeListView_SizeChanged(object sender, EventArgs e)
        {
            ResizeHeaders();
        }

        private bool inResizeHeaders = false;

        private void ResizeHeaders()
        {
            // prevent recusive calls
            if (inResizeHeaders)
            {
                return;
            }
            inResizeHeaders = true;
            try
            {
                int headerWidth = 0;
                for (int idx = 0; idx < Columns.Count; idx++)
                {
                    headerWidth += Columns[idx].Width;
                }
                if ((Columns.Count > 0) && (Width > headerWidth))
                {
                    // Last header on the right needs to expand the with to include
                    // all the space to the left when we erease the background
                    Columns[Columns.Count - 1].Width += (Width - headerWidth) - 2;
                }
                HeadersResized = false;
            }
            finally
            {
                inResizeHeaders = false;
            }
        }

    }

    public enum TreeListViewAction { Expand, Collapse };

	public class TreeListViewEventArgs : EventArgs
	{
		public TreeListViewEventArgs() { }
		public TreeListViewEventArgs(TreeListViewAction action, TreeListViewItem node)
		{
			Action = action;
			Node = node;
		}

		public TreeListViewAction Action { get; private set; }
		public TreeListViewItem Node { get; private set; }
	}

	public delegate void TreeListViewEvent(object Sender, TreeListViewEventArgs args);
}
