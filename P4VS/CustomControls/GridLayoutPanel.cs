//#define DEBUG_LAYOUT

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.I18nControls
{
	public partial class GridLayoutPanel : Panel
    {
#if DEBUG_LAYOUT
		[Browsable(false)]
        bool DebugLayout = true;
#else
        [Browsable(false)]
#endif

		[Flags]
		public enum ControlGridLayoutFlags
		{
			None = 0x0000,
			StretchWidth = 0x0001,
			StretchHeight = 0x0002,
		}

		public class GridRow : List<IGridLayoutCell>
		{
			public int Idx { get; set; }
			private GridRow() { }

			public GridRow(int idx) { Idx = idx; }

			public ControlGridLayoutFlags LayoutFlags = ControlGridLayoutFlags.None;

			public int Height;

			public int Position;

			public bool AllHidden; // all the controls on this row are not visible
		}

		public class GridColumn : List<IGridLayoutCell>
		{
			public int Idx { get; set; }

			private GridColumn() { }

			public GridColumn(int idx) { Idx = idx; }

			public ControlGridLayoutFlags LayoutFlags = ControlGridLayoutFlags.None;

			public int Width;

			public int Position;

			public bool AllHidden; // all the controls in this column are not visible
		}

		public GridLayoutPanel()
		{
			MinimumColumnWidth = 10;
			MinimumRowHeight = 10;

			InitializeComponent();
			//#if DEBUG
			//            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			//#endif
		}

		private bool _gridInitialized = false;

		public bool EnableDesignerLayout { get; set; }

		public bool EnableDesignerGrid { get; set; }

		public bool EnableParentResize { get; set; }

		public void InitializeGrid()
		{
			InitializeGrid(false);
		}

		public void InitializeGrid(bool reinitialize)
		{
			if (reinitialize)
			{
				_gridInitialized = false;
			}
			if (Controls.Count <= 0)
			{
#if DEBUG_LAYOUT
				logger.Trace("InitializeGrid() entered before controls were added");
#endif
				return;
			}
			if (_gridInitialized)
			{
#if DEBUG_LAYOUT
				logger.Trace("InitializeGrid() reentered");
#endif
				return;
			}
#if DEBUG_LAYOUT
			logger.Trace("InitializeGrid() entered for " + this.Name);
#endif
			try
			{
				Rows = new List<GridRow>();
				Columns = new List<GridColumn>();

				foreach (Control c in Controls)
				{
					if (c is IGridLayoutCell)
					{
						IGridLayoutCell gc = c as IGridLayoutCell;
						if (Rows.Count <= gc.Row)
						{
							for (int idx = Rows.Count; idx <= gc.Row; idx++)
							{
								Rows.Add(new GridRow(idx));
							}
						}
						else if (Rows[gc.Row] == null)
						{
							Rows[gc.Row] = new GridRow(gc.Row);
						}
						if (Rows[gc.Row].Count <= gc.Column)
						{
							for (int idx = Rows[gc.Row].Count; idx <= gc.Column; idx++)
							{
								Rows[gc.Row].Add(null);
							}
						}
						Rows[gc.Row][gc.Column] = gc;

						if (Columns.Count <= gc.Column)
						{
							for (int idx = Columns.Count; idx <= gc.Column; idx++)
							{
								Columns.Add(new GridColumn(idx));
							}
						}
						else if (Columns[gc.Column] == null)
						{
							Columns[gc.Column] = new GridColumn(gc.Column);
						}
						if (Columns[gc.Column].Count <= gc.Row)
						{
							for (int idx = Columns[gc.Column].Count; idx <= gc.Row; idx++)
							{
								Columns[gc.Column].Add(null);
							}
						}
						Columns[gc.Column][gc.Row] = gc;
					}
				}
				// measure the relative vertical positions of the controls on a row.
				// assuming the user laid the controls out in his preferred alignment
				// (usually to get the font baselines of the various controls to line up.
				foreach (GridRow row in Rows)
				{
					// figure out the uppermost control on each row
					int minYPos = int.MaxValue;
					row.LayoutFlags &= ~ControlGridLayoutFlags.StretchHeight;
					foreach (IGridLayoutCell gc in row)
					{
						if (gc == null)
						{
							continue;
						}
						Control c = null;
						try
						{
							c = (Control)gc;
						}
						catch { continue; }

						if (c.Top < minYPos)
						{
							minYPos = c.Top;
						}

						//gc.DesignSize = c.Size;
					}
					// now record the relative position for each control in the row 
					// in reference to the uppermost control, before we start moving,
					// the controls around in layout
					row.LayoutFlags &= ~ControlGridLayoutFlags.StretchHeight;
					foreach (IGridLayoutCell gc in row)
					{
						if (gc == null)
						{
							continue;
						}
						Control c = null;
						try
						{
							c = (Control)gc;
						}
						catch { continue; }

						gc.YOffset = c.Top - minYPos;
					}
				}
			}
#if DEBUG_LAYOUT
			catch (Exception ex)
			{
				logger.Trace(ex.Message);
				logger.Trace(ex.StackTrace);
				return;
			}
#else
            catch (Exception)
            {
                return;
            }
#endif
            _gridInitialized = true;
		}

		protected List<GridRow> Rows { get; set; }

		protected List<GridColumn> Columns { get; set; }

		public int MinimumColumnWidth { get; set; }
		public int MinimumRowHeight { get; set; }

		//protected override void OnLayout(LayoutEventArgs levent)
		//{
		//    base.OnLayout(levent);
		//    if (this.DesignMode)
		//    {
		//        LayoutGrid();
		//    }
		//}

		public delegate void GridLayoutEvent();
		public event GridLayoutEvent BeforeLayoutGrid;
		public event GridLayoutEvent AfterLayoutGrid;

		bool LayoutPerformed = false;

		public void InitializeLayout()
		{
			if (LayoutPerformed == false)
			{
				LayoutGrid();
			}
		}

		public virtual void LayoutGrid()
		{
			LayoutGrid(false);
		}

		public virtual void LayoutGrid(bool reinitialize)
		{
			if (reinitialize)
			{
				_gridInitialized = false;
			}
#if DEBUG_LAYOUT
			logger.Trace("LayoutGrid() entered for " + this.Name);
			if ((this.Visible == false) || ((this.DesignMode) && (EnableDesignerLayout == false)))
#else
			if (this.LayoutSuspended || (this.Visible == false))
#endif
			{
				return;
			}
			base.SuspendLayout();
			LayoutSuspendedCnt++;
			try
			{
				if (BeforeLayoutGrid != null)
				{
					BeforeLayoutGrid();
				}

				LayoutRows();
				LayoutColumns();

				if (EnableParentResize && (this.Dock == DockStyle.Fill) && (this.Parent != null))
				{
					Parent.ClientSize = this.Size;
				}
				if (AfterLayoutGrid != null)
				{
					AfterLayoutGrid();
				}
			}
			finally
			{
				LayoutPerformed = true;
				LayoutSuspendedCnt--;
				//base.ResumeLayout();
			}
		}

		public void LayoutRows()
		{
			InitializeGrid();

			if (Rows == null)
			{
				return;
			}
			int StretchableRowCnt = 0;
			int TotalHeight = 0;

			bool canStretchRows = false;
			bool canGrowGrid = this.AutoSize;
			bool canShrinkGrid = canGrowGrid && (this.AutoSizeMode == System.Windows.Forms.AutoSizeMode.GrowAndShrink);

			// go through the rows and figure out the correct height based on the
			// heights of the controls on the row
			foreach (GridRow row in Rows)
			{
				row.Height = 0;
				row.LayoutFlags &= ~ControlGridLayoutFlags.StretchHeight;
				row.AllHidden = true;
				foreach (IGridLayoutCell gc in row)
				{
					if (gc == null)
					{
						continue;
					}
					Control c = null;
					try
					{
						c = (Control)gc;
					}
					catch { continue; }

					//					Size cp = c.PreferredSize;

					row.AllHidden &= !c.Visible;

					int ch = c.Height;
					if (((c.Anchor & AnchorStyles.Top) != 0) && ((c.Anchor & AnchorStyles.Bottom) != 0) && (gc.RowsSpanned == 0))
					{
						// control can be stretched
						row.LayoutFlags |= ControlGridLayoutFlags.StretchHeight;
						// don't use the controls stretched size to determine the
						// new height for the row, use it's original design time size

						ch = 0;
					}
					else if (gc.RowsSpanned > 0)
					{
						// control can overrun at least one row below
						ch = 0;
					}
					if ((ch + c.Margin.Vertical) > row.Height)
					{
						row.Height = ch + c.Margin.Vertical;
					}
				}
				if (row.AllHidden)
				{
					// all the controls on this row are hidden, so set it's height
					// to zero to not leave a gap in the vertical layout
					row.Height = 0;
					// Don't use hidden rows to stretch the height
					row.LayoutFlags &= ~ControlGridLayoutFlags.StretchHeight;
				}
				else if (row.Height < MinimumRowHeight)
				{
					row.Height = MinimumRowHeight;
				}
				TotalHeight += row.Height;
				if ((row.LayoutFlags & ControlGridLayoutFlags.StretchHeight) != 0)
				{
					StretchableRowCnt++;
					canStretchRows = true;
				}
			}
			// Figure out if there is extra space at the bottom 
			// (a negative value indicates there's not enough space)
			int extraHeight = this.Height - this.Padding.Vertical - TotalHeight;
			int fudge = 0;
			int slop = 0;
			int rowSpacing = 0;

			if (canStretchRows)
			{
				fudge = extraHeight / StretchableRowCnt;
				slop = extraHeight - (fudge * StretchableRowCnt);

				// Now stretch the size of stretchable rows as needed
				for (int idx = 0; idx < Rows.Count; idx++)
				{
					GridRow row = Rows[idx];

					if ((row.LayoutFlags & ControlGridLayoutFlags.StretchHeight) != 0)
					{
						row.Height += fudge + slop;
						slop = 0; // only add the slop in once
					}
				}
			}
			else if (!canShrinkGrid && (extraHeight > 0) && (Rows.Count > 0))
			{
				// need to shrink grid but aren't allowed
				rowSpacing = extraHeight / Rows.Count;
			}
			if ((extraHeight < 0) && !canStretchRows && canGrowGrid)
			{
				// there are no stretchable columns, so need to grow the size 
				// of the grid
				this.Height -= extraHeight;
			}
			else if ((extraHeight > 0) && !canStretchRows && canShrinkGrid)
			{
				// there are no stretchable columns, so need to adjust the size 
				// of the grid
				this.Height -= extraHeight;
			}

			// Now position each row and each of their controls from the top
			// and stretch the heights of controls in stretchable rows as needed
			int CurrentPos = this.Padding.Top + (rowSpacing / 2);
			for (int idx = 0; idx < Rows.Count; idx++)
			{
				GridRow row = Rows[idx];

				row.Position = CurrentPos;

				if (row.AllHidden)
				{
					// row is hidden don't layout it's control
					continue;
				}
				foreach (IGridLayoutCell gc in row)
				{
					if (gc == null)
					{
						continue;
					}
					Control c = null;
					try
					{
						c = (Control)gc;
					}
					catch { continue; }

					gc.CellHeight = row.Height; ;
					if (idx < (Rows.Count - 1))
					{
						gc.CellHeight += rowSpacing;
					}
					if ((idx == 0) || (idx == (Rows.Count - 1)))
					{
						gc.CellHeight += rowSpacing / 2;
					}
					if (gc.RowsSpanned > 0)
					{
						for (int idx2 = 1; idx2 <= gc.RowsSpanned; idx2++)
						{
							if ((idx + idx2) >= Rows.Count)
							{
								break;
							}
							gc.CellHeight += Rows[idx + idx2].Height;
							if ((idx + idx2) < (Rows.Count - 1))
							{
								gc.CellHeight += rowSpacing;
							}
							if (((idx + idx2) == 0) || ((idx + idx2) == (Rows.Count - 1)))
							{
								gc.CellHeight += rowSpacing / 2;
							}
						}
					}
					if (gc.CellHeight < MinimumRowHeight)
					{
						gc.CellHeight = MinimumRowHeight;
					}
					c.Top = CurrentPos + gc.YOffset + c.Margin.Top;
					bool stretched = false;
					//if ((row.LayoutFlags & ControlGridLayoutFlags.StretchHeight) != 0)
					//{
					if (((c.Anchor & AnchorStyles.Top) != 0) && ((c.Anchor & AnchorStyles.Bottom) != 0))
					{
						// stretch the control
						c.Height = gc.CellHeight - (c.Margin.Top + c.Margin.Bottom);
						stretched = true;
					}
					//}
					if (stretched == false)
					{
						// figure out where to put the control in the cell
						if ((c.Anchor & AnchorStyles.Top) != 0)
						{
							// top do nothing it's already in the right place
						}
						else if ((c.Anchor & AnchorStyles.Bottom) != 0)
						{
							c.Top = (CurrentPos + gc.CellHeight) - (c.Height + c.Margin.Bottom);
						}
						else
						{
							c.Top = CurrentPos + ((gc.CellHeight - (c.Height + c.Margin.Vertical)) / 2) + c.Margin.Top;
						}
					}
				}
				CurrentPos += row.Height + rowSpacing;
			}
		}

		public void LayoutColumns()
		{
			InitializeGrid();

			if (Columns == null)
			{
				return;
			}

			int StretchableClmCnt = 0;
			int TotalWidth = 0;

			bool canStretchClms = false;
			bool canGrowGrid = this.AutoSize;
			bool canShrinkGrid = canGrowGrid && (this.AutoSizeMode == System.Windows.Forms.AutoSizeMode.GrowAndShrink);

#if DEBUG_LAYOUT
			if (DebugLayout)
			{
				logger.Trace("Laying out {2} columns, canGrowGrid: {0}, canShrinkGrid: {1}",
					canGrowGrid ? "True" : "false", canShrinkGrid ? "True" : "false", Columns.Count);
			}
#endif
			// go through the columns and figure out the correct width based on the
			// design widths of the controls on the columns
			foreach (GridColumn clm in Columns)
			{
#if DEBUG_LAYOUT
				if (DebugLayout)
				{
					logger.Trace("Determining width of column {0}", clm.Idx);
				}
#endif
				clm.Width = 0;
				clm.AllHidden = true;
				clm.LayoutFlags &= ~ControlGridLayoutFlags.StretchWidth;
				foreach (IGridLayoutCell gc in clm)
				{
					if (gc == null)
					{
						continue;
					}
					Control c = null;
					try
					{
						c = (Control)gc;
					}
					catch { continue; }

					int cw = c.Width;
					clm.AllHidden &= !c.Visible;

					if (((c.Anchor & AnchorStyles.Left) != 0) && ((c.Anchor & AnchorStyles.Right) != 0) && (gc.ColumnsSpanned == 0))
					{
#if DEBUG_LAYOUT
						if (DebugLayout)
						{
							logger.Trace("Control [{0},{1}] can be stretched and spans {2} columns",
								gc.Row, gc.Column, gc.ColumnsSpanned);
						}
#endif
						// control can be stretched
						clm.LayoutFlags |= ControlGridLayoutFlags.StretchWidth;
						// don't use the controls stretched size to determine the
						// new height for the row, use it's original design time size
						cw = 0;
					}
					else if (gc.ColumnsSpanned > 0)
					{
						// control can overrun at least one column on the right
						cw = 0;
					}
#if DEBUG_LAYOUT
					if (DebugLayout)
					{
						logger.Trace("Width of control [{0},{1}] = {2}",
							gc.Row, gc.Column, cw);
					}
#endif
					if ((cw + c.Margin.Left + c.Margin.Right) > clm.Width)
					{
						clm.Width = cw + c.Margin.Left + c.Margin.Right;
					}
				}
				if (clm.AllHidden)
				{
					clm.Width = 0;
					clm.LayoutFlags &= ~ControlGridLayoutFlags.StretchWidth;
				}
				else if (clm.Width < MinimumColumnWidth)
				{
					clm.Width = MinimumColumnWidth;
				}
				TotalWidth += clm.Width;
				if ((clm.LayoutFlags & ControlGridLayoutFlags.StretchWidth) != 0)
				{
					StretchableClmCnt++;
					canStretchClms = true;
				}
#if DEBUG_LAYOUT
				if (DebugLayout)
				{
					logger.Trace("Width of column[{0}] = {1}",
						clm.Idx, clm.Width);
				}
#endif
			}
			// 2: Figure out if there is extra space at the right side 
			// (a negative value indicates there's not enough space)
			int extraWidth = this.Width - this.Padding.Horizontal - TotalWidth;
			int fudge = 0;
			int slop = 0;
			int clmSpacing = 0;

			if (canStretchClms)
			{
				fudge = extraWidth / StretchableClmCnt;
				slop = extraWidth - (fudge * StretchableClmCnt);
#if DEBUG_LAYOUT
				if (DebugLayout)
				{
					logger.Trace("Stretching {0} columns, fudge = {1}, slop = {2}",
						StretchableClmCnt, fudge, slop);
				}
#endif
				// Now stretch the size of stretchable columns as needed
				for (int idx = 0; idx < Columns.Count; idx++)
				{
					GridColumn clm = Columns[idx];

					if ((clm.LayoutFlags & ControlGridLayoutFlags.StretchWidth) != 0)
					{
#if DEBUG_LAYOUT
						if (DebugLayout)
						{
							if (slop > 0)
							{
								logger.Trace("Added slop to column {0} ",
								   idx);
							}
						}
#endif
						clm.Width += fudge + slop;
						slop = 0; // only add the slop in once
#if DEBUG_LAYOUT
						if (DebugLayout)
						{
							logger.Trace("Setting width of column[{0}] to {1} ",
							   idx, clm.Width);
						}
#endif
					}
				}
			}
			else if (!canShrinkGrid && (extraWidth > 0) && (Columns.Count > 0))
			{
				// need to shrink grid but aren't allowed
				clmSpacing = extraWidth / Columns.Count;
#if DEBUG_LAYOUT
				if (DebugLayout)
				{
					logger.Trace("Spacing columns {0} ",
						clmSpacing);
				}
#endif
			}
			if ((extraWidth < 0) && !canStretchClms && canGrowGrid)
			{
				// there are no stretchable columns, so need to grow the size 
				// of the grid
				this.Width -= extraWidth;
#if DEBUG_LAYOUT
				if (DebugLayout)
				{
					logger.Trace("Growing grid by {0} ",
						clmSpacing);
				}
#endif
			}
			else if ((extraWidth > 0) && !canStretchClms && canShrinkGrid)
			{
				// there are no stretchable columns, so need to adjust the size 
				// of the grid
				this.Width -= extraWidth;
#if DEBUG_LAYOUT
				if (DebugLayout)
				{
					logger.Trace("Shrinking grid by {0} ",
						clmSpacing);
				}
#endif
			}

			// 3: Now position each column and each of their controls from the left side
			// stretch the size of controls in stretchable columns as needed
			int CurrentPos = this.Padding.Left + (clmSpacing / 2);
			for (int idx = 0; idx < Columns.Count; idx++)
			{
				GridColumn clm = Columns[idx];

				clm.Position = CurrentPos;

#if DEBUG_LAYOUT
				if (DebugLayout)
				{
					logger.Trace("Column[{0}] position is {1}",
						clm.Idx, clm.Position);
				}
#endif
				if (clm.AllHidden)
				{
					continue;
				}
				foreach (IGridLayoutCell gc in clm)
				{
					if (gc == null)
					{
						continue;
					}
					Control c = null;
					try
					{
						c = (Control)gc;
					}
					catch { continue; }

					gc.CellWidth = clm.Width;
					if (idx < (Columns.Count - 1))
					{
						gc.CellWidth += clmSpacing;
					}
					if ((idx == 0) || (idx == (Columns.Count - 1)))
					{
						gc.CellWidth += clmSpacing / 2;
					}
#if DEBUG_LAYOUT
					string traceMsg = null;
					if (DebugLayout)
					{
						traceMsg = string.Format("Stretching Control[{0},{1}] (initial width {2})",
							gc.Row, gc.Column, c.Width);
					}
#endif
					if (gc.ColumnsSpanned > 0)
					{
#if DEBUG_LAYOUT
						if (DebugLayout)
						{
							traceMsg += string.Format(" which spans {0} columns",
								gc.RowsSpanned);
						}
#endif
						for (int idx2 = 1; idx2 <= gc.ColumnsSpanned; idx2++)
						{
							if ((idx + idx2) >= Columns.Count)
							{
								break;
							}
#if DEBUG_LAYOUT
							if (DebugLayout)
							{
								traceMsg += string.Format(" +{0}",
									Columns[idx + idx2].Width);
							}
#endif
							gc.CellWidth += Columns[idx + idx2].Width;
							if ((idx + idx2) < (Columns.Count - 1))
							{
								gc.CellWidth += clmSpacing;
							}
							if (((idx + idx2) == 0) || ((idx + idx2) == (Columns.Count - 1)))
							{
								gc.CellWidth += clmSpacing / 2;
							}
						}
						//}
					}
					if (gc.CellWidth < MinimumColumnWidth)
					{
#if DEBUG_LAYOUT
						if (DebugLayout)
						{
							traceMsg += string.Format(" using column minimum width :{0}", MinimumColumnWidth);
						}
#endif
						gc.CellWidth = MinimumColumnWidth;
					}

					c.Left = CurrentPos + c.Margin.Left;
					bool stretched = false;
					//if ((clm.LayoutFlags & ControlGridLayoutFlags.StretchWidth) != 0)
					//{
					if (((c.Anchor & AnchorStyles.Left) != 0) && ((c.Anchor & AnchorStyles.Right) != 0))
					{
						// stretch the control
						c.Width = gc.CellWidth - (c.Margin.Left + c.Margin.Right);

#if DEBUG_LAYOUT
						if (DebugLayout)
						{
							traceMsg += string.Format(" new width :{0}", c.Width);
							logger.Trace(traceMsg);
						}
#endif

						stretched = true;
					}
					if (stretched == false)
					{
						// figure out where to put the control in the cell
						if ((c.Anchor & AnchorStyles.Left) != 0)
						{
							// top do nothing it's already in the right place
						}
						else if ((c.Anchor & AnchorStyles.Right) != 0)
						{
							c.Left = (CurrentPos + gc.CellWidth) - (c.Width + c.Margin.Right);
						}
						else
						{
							c.Left = CurrentPos + ((gc.CellWidth - (c.Width + c.Margin.Horizontal)) / 2) + c.Margin.Left;
						}
					}
				}
				CurrentPos += clm.Width + clmSpacing;
			}
		}

		public void OnSizeChanged() { OnSizeChanged(null); }

		private int LayoutSuspendedCnt = 0;

		private bool LayoutSuspended
		{
			get
			{
				return (LayoutSuspendedCnt > 0);
			}
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			if (!LayoutSuspended)
			{
				LayoutGrid();
			}
			base.OnSizeChanged(e);
		}

		public new void SuspendLayout()
		{
			LayoutSuspendedCnt++;
			base.SuspendLayout();
		}
		public new void ResumeLayout(bool b)
		{
			LayoutSuspendedCnt--;
			if (b)
			{
				LayoutGrid();
			}
			//base.ResumeLayout(b);
		}

		public new void PerformLayout()
		{
			if (!LayoutSuspended)
			{
				LayoutGrid();
			}
			//base.PerformLayout();
		}
		protected override void OnPaint(PaintEventArgs e)
		{
#if DEBUG_LAYOUT
			if (EnableDesignerGrid)
			{
				if (_gridInitialized == false)
				{
					if (EnableDesignerLayout)
					{
						LayoutGrid();
					}
					else
					{
						return;
					}
				}
				if ((Rows == null) || (Columns == null))
				{
					return;
				}
				using (Pen p = new Pen(new SolidBrush(Color.DarkRed), 1))
				{
					foreach (Column c in Columns)
					{
						if (c.Position > 0)
						{
							e.Graphics.DrawLine(p, c.Position, 0, c.Position, this.Size.Height);
						}
					}
					Column cl = Columns[Columns.Count - 1];
					if ((cl.Position + cl.Width) < this.Width)
					{
						e.Graphics.DrawLine(p, cl.Position + cl.Width, 0, cl.Position + cl.Width, this.Size.Height);
					}
					foreach (Row r in Rows)
					{
						if (r.Position > 0)
						{
							e.Graphics.DrawLine(p, 0, r.Position, this.Size.Width, r.Position);
						}
					}
					Row rl = Rows[Rows.Count - 1];
					if ((rl.Position + rl.Height) < this.Height)
					{
						e.Graphics.DrawLine(p, 0, rl.Position + rl.Height, this.Size.Width, rl.Position + rl.Height);
					}
				}
			}
#endif
			base.OnPaint(e);
		}
		//public Cell this[int row, int column]
		//{
		//    get
		//    {
		//        return Controls[column + (row * Columns)] as Cell;
		//    }
		//}
	}
}
