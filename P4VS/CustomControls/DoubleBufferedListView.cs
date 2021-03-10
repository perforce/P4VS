using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Perforce.P4VS
{
	public class DoubleBufferedListView : ListView
	{
		public DoubleBufferedListView()
			: base()
		{
			OwnerDraw = true;
			DoubleBuffered = true;
			AllowColumnReorder = true;
			InitializeComponent();

			// Create an instance of a ListView column sorter and assign it 
			// to the ListView control, this generic sorter can be overridden 
			// by setting ColumnSorter
			ColumnSorter = new ListViewColumnSorter();

			ColumnContextMenuClicked += new ColumnContextMenuHandler(OnColumnContextMenuClicked);

			this.MouseMove += new MouseEventHandler(DoubleBufferedListView_MouseMove);

			base.InitLayout();
		}

		// Forces each row to repaint itself the first time the mouse moves over  
		// it, compensating for an extra DrawItem event sent by the wrapped  
		// Win32 control. This issue occurs each time the ListView is invalidated. 
		private void DoubleBufferedListView_MouseMove(object sender, MouseEventArgs e)
		{
			ListViewItem item = this.GetItemAt(e.X, e.Y);
			if (item != null)
			{
				this.Invalidate(item.Bounds);
			}
		}

		private ColumnSorter _columnSorter;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColumnSorter ColumnSorter
		{
			get { return _columnSorter;}
			set 
			{
				_columnSorter = value;
				this.ListViewItemSorter = value;
			}
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
					for (int idx = 0; idx < Columns.Count && idx < settings.Count; idx++)
					{
						if (settings[idx].Width >= 0)
						{
							Columns[idx].Width = settings[idx].Width;
						}
						if (settings[idx].DisplayIndex >= 0)
						{
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
			//ItemChecked += OnItemChecked;
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
				ToolStripMenuItem it = new ToolStripMenuItem(h.Text, null,
															 new EventHandler(OnColumnContextMenuItemClicked));
				it.Tag = h;
				it.Checked = h.Width > 0;
				ChooseColumnsCtxMnu.Items.Add(it);
			}

			ChooseColumnsCtxMnu.Show(loc);
		}

		private ContextMenuStrip ChooseColumnsCtxMnu;

		private bool _OnItemsArea = false;

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
		private const int WM_CONTEXTMENU = 0x007B;

		//private int WndProcReentryCnt = 0;

		protected override void WndProc(ref Message message)
		{
			//try
			//{
			//	WndProcReentryCnt++;
			//	if (WndProcReentryCnt > 100)
			//	{
			//		System.Diagnostics.logger.Trace("Reentering WndProc(): stack depth = {0}", WndProcReentryCnt));
			//	}
			if (message.Msg == WM_CONTEXTMENU)
			{
				if (!_OnItemsArea)
				{
					Point p = base.PointToClient(MousePosition);
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
					return;
				}
			}
			base.WndProc(ref message);
			//}
			//finally
			//{
			//	WndProcReentryCnt--;
			//}
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.ChooseColumnsCtxMnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // ChooseColumnsCtxMnu
            // 
            this.ChooseColumnsCtxMnu.Name = "ChooseColumnsCtcMnu";
            this.ChooseColumnsCtxMnu.Size = new System.Drawing.Size(61, 4);
            // 
            // DoubleBufferedListView
            // 
            this.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.DoubleBufferedListView_ColumnClick);
            this.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.DoubleBufferedListView_ColumnReordered);
            this.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.DoubleBufferedListView_ColumnWidthChanged);
            this.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.OnDrawColumnHeader);
            this.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.DoubleBufferedListView_DrawItem);
            this.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.DoubleBufferedListView_DrawSubItem);
            this.SizeChanged += new System.EventHandler(this.DoubleBufferedListView_SizeChanged);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DoubleBufferedListView_MouseClick);
            this.ResumeLayout(false);

		}

		private System.ComponentModel.IContainer components;

		private void DoubleBufferedListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// Determine if clicked column is already the column that is being sorted.
			if (e.Column == ColumnSorter.SortColumn)
			{
				// Reverse the current sort direction for this column.
				if (ColumnSorter.Order == SortOrder.Ascending)
				{
					ColumnSorter.Order = SortOrder.Descending;
				}
				else
				{
					ColumnSorter.Order = SortOrder.Ascending;
				}
			}
			else
			{
				// set the header text to what it already is to force a
				// redraw of the previously selected column header.
				this.Columns[ColumnSorter.SortColumn].Text =
					this.Columns[ColumnSorter.SortColumn].Text;

				// Set the column number that is to be sorted; default to ascending.

				ColumnSorter.SortColumn = e.Column;
				ColumnSorter.Order = SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			this.Sort();
		}

		//protected override void InitLayout()
		//{
		//    this.OwnerDraw = true;
		//    this.View = View.Details;
		//    this.DoubleBuffered = true;

		//    //this.DrawItem += new DrawListViewItemEventHandler(OnDrawItem);
		//    //this.DrawSubItem += new DrawListViewSubItemEventHandler(OnDrawSubItem);
		//    //this.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(OnDrawColumnHeader);

		//    // Add handlers for various events to compensate for an 
		//    // extra DrawItem event that occurs the first time the mouse 
		//    // moves over each row. 
		//    //this.MouseMove += new MouseEventHandler(OnMouseMove);
		//    //this.ColumnWidthChanged += new ColumnWidthChangedEventHandler(OnColumnWidthChanged);
		//    //this.Invalidated += new InvalidateEventHandler(OnInvalidated);

		//    //this.MouseClick += new MouseEventHandler(OnMouseClick);

		//    //currenttStateImageList = defaultStateImageList;

		//    base.InitLayout();
		//}

		private void DoubleBufferedListView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
		{
			if (e.NewDisplayIndex == 0 | e.OldDisplayIndex == 0)
			{
				e.Cancel = true;
			}
		}

		public bool HideActionsColumn { get; set; }

        bool HeadersResized = false;

		private void DoubleBufferedListView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
		{
			if ((this.Columns != null) && (this.Columns.Count > 0))
			{
				if (HideActionsColumn)
				{
					if (this.Columns[0].Width > 0)
					{
						this.Columns[0].Width = 0;
					}
					base.Invalidate();
				}
				else
				{
					if (this.Columns[0].Width < 64)
					{
						this.Columns[0].Width = 64;
					}

					base.Invalidate();
				}
			}
            HeadersResized = true;
        }

		private int _headerHeight = 24;

        private void OnDrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (HeadersResized)
            {
                ResizeHeaders();
            }
            // Draws column headers.
            int top = e.Bounds.Top;
            int left = e.Bounds.Left;
            int right = e.Bounds.Right;
            int center = left + ((right - left) / 2);

            _headerHeight = e.Bounds.Height;

            Color BgColor = base.BackColor;
            Color FgColor = base.ForeColor;

            using (Pen pen = new Pen(FgColor, 1))
            {
                using (Brush fgBrush = new SolidBrush(FgColor), bgBrush = new SolidBrush(BgColor))
                {
                    using (StringFormat sf = new StringFormat())
                    {
                        sf.Alignment = StringAlignment.Near;
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Trimming = StringTrimming.EllipsisCharacter;
                        sf.FormatFlags = StringFormatFlags.NoWrap;

                        // Draw the standard header background.
                        Rectangle eRect = new Rectangle(e.Bounds.Location, e.Bounds.Size);
                        if ((e.Header.DisplayIndex >= Columns.Count-1) && (eRect.Right < this.Right))
                        {
                            eRect.Width += Right - eRect.Right;
                        }
                        e.Graphics.FillRectangle(bgBrush, eRect);
                        Rectangle borderRect = e.Bounds;
                        borderRect.Width -= 1;
                        borderRect.Height -= 1;
                        // Inflate can move the rect as well, so we don't want to use it
                        //borderRect.Inflate(0, -1); 
                        e.Graphics.DrawRectangle(pen, borderRect);

                        //e.DrawText();
                        e.Graphics.DrawString(e.Header.Text, base.Font,
                                              fgBrush, e.Bounds, sf);
                    }
                    // Draw the triangle
                    // Create pen.
                    Point[] curvePoints = new Point[]
                                              {
                                          new Point(0, 0),
                                          new Point(0, 0),
                                          new Point(0, 0)
                                              };
                    if (ColumnSorter.SortColumn == e.ColumnIndex)
                    {
                        if (ColumnSorter != null && ColumnSorter.Order == SortOrder.Descending)
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
                        if (ColumnSorter != null && ColumnSorter.Order == SortOrder.Ascending)
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
			return;
		}

		private void DoubleBufferedListView_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			Rectangle bounds = e.Bounds;
			// not everything uses small image list, check
			if (SmallImageList != null)
			{
				bounds.X += SmallImageList.ImageSize.Width;
				bounds.Width -= SmallImageList.ImageSize.Width;
			}

			Color BkColor = base.BackColor;
			if (e.Item.Selected)
			{
				// Draw the background and focus rectangle for a selected item.
				BkColor = SystemColors.Highlight;
			}
			using (Brush brush = new SolidBrush(BkColor))
			{
				e.Graphics.FillRectangle(brush, bounds);
				//e.DrawFocusRectangle();
			}

			if (SmallImageList != null)
			{
				if (CheckBoxes)
				{
					// set image index based on checked state
					e.Item.ImageIndex = e.Item.Checked ? 1 : 0;
				}

				if (e.Item.ImageIndex > -1)
				{
					base.SmallImageList.Draw(e.Graphics, 2, bounds.Y, e.Item.ImageIndex);
				}
			}

			//Invalidate(bounds,true);
			//// the invalidate seems to cause controls on the parent
			//// not to be drawn, so refresh the parent
			//if (Parent != null)
			//{
			//    Parent.Refresh();
			//}

			//// some listviews are nested under multiple parents
			//// dialog -> sliding panel -> listview
			//// in case controls are not drawn on the highest level
			//// parent, we need to refresh at that level
			//if (Parent.Parent != null)
			//{
			//    Parent.Parent.Refresh();
			//}

			ListViewItem test = e.Item;
			e.Item.Position.Offset(10, 0);
		}

        private void DoubleBufferedListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            Rectangle bounds = e.Bounds;
            if (e.ColumnIndex == 0 && SmallImageList != null)
            {
                bounds.X += SmallImageList.ImageSize.Width;
                bounds.Width -= SmallImageList.ImageSize.Width;
            }

            Color FgColor = base.ForeColor;

            using (Brush fgBrush = new SolidBrush(FgColor))
            {
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
                        sf.Trimming = StringTrimming.EllipsisCharacter;
                        sf.FormatFlags = StringFormatFlags.NoWrap;
                        e.Graphics.DrawString(replaceLineBreaksWithSpaces, f,
                            fgBrush, bounds, sf);
                    }
                }
            }
        }

		private void DoubleBufferedListView_MouseClick(object sender, MouseEventArgs e)
		{
			//base.OnMouseClick(e);
			if ((Control.ModifierKeys == Keys.Control) || (Control.ModifierKeys == Keys.Shift))
			{
				// user has the control or shift key down to extend selection, so make sure all 
				// highlighted items are selected/deselected.
				ListViewItem lvi = GetItemAt(e.X, e.Y);
				bool isChecked = lvi.Checked;
				foreach (ListViewItem i in SelectedItems)
				{
					i.Checked = isChecked;
				}
			}
		}

        private void DoubleBufferedListView_SizeChanged(object sender, EventArgs e)
        {
            ResizeHeaders();
        }
        private void ResizeHeaders()
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

        //private void OnItemChecked(object sender, ItemCheckedEventArgs e)
        //{
        //    if (CheckBoxes)
        //    {
        //        e.Item.ImageIndex = e.Item.Checked ? 1 : 0;
        //    }
        //}
    }
}
