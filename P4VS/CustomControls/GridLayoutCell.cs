using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Perforce.P4VS;

namespace Perforce.I18nControls
{
	public interface IGridLayoutCell
	{
		int Row { get; set; }
		int Column { get; set; }
		/// <summary>
		/// Vertical offset to get the font baselines aligned
		/// Measured from the original layout in the designer
		/// </summary>
		[Browsable(false)]
		int YOffset { get; set; }

		// Width of the cell of stretching and spanning adjacent cells
		[Browsable(false)]
		int CellHeight { get; set; }
		[Browsable(false)]
		int CellWidth { get; set; }

		int RowsSpanned { get; set; }
		int ColumnsSpanned { get; set; }
	}

	[Serializable]
	public class GridTextBox : TextBox, IGridLayoutCell 
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridLabel : Label, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridButton : Button, IGridLayoutCell
	{
		public GridButton()
		{
		}
		protected override void OnPaint(PaintEventArgs pevent)
		{
			base.OnPaint(pevent);
		}
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == 0x000F && !this.Enabled)
			{
				// Create a graphics object from the window handle to paint on top.
				using (Graphics g = Graphics.FromHwnd(this.Handle))
				{
					// Check for dark background
					int bVal = (int)(this.BackColor.R * 0.299 + this.BackColor.G * 0.587 + this.BackColor.B * 0.114);
					if (bVal < 128)
					{
						// Define formatting flags to center the text
						TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.WordBreak;

						// Draw the light text over the disabled button
						TextRenderer.DrawText(g, this.Text, this.Font, this.ClientRectangle, Color.LightGray, flags);
					}
				}
			}
		}

		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridComboBox : ComboBox, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		public int YOffset { get; set; }
		public Size DesignSize { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridCheckBox : CheckBox, IGridLayoutCell
	{
		public GridCheckBox()
		{
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		private bool isHovered = false;
		private bool isPressed = false;
		protected override void OnMouseEnter(EventArgs e) { isHovered = true; Invalidate(); base.OnMouseEnter(e); }
		protected override void OnMouseLeave(EventArgs e) { isHovered = false; Invalidate(); base.OnMouseLeave(e); }
		protected override void OnMouseDown(MouseEventArgs mevent) { isPressed = true; Invalidate(); base.OnMouseDown(mevent); }
		protected override void OnMouseUp(MouseEventArgs mevent) { isPressed = false; Invalidate(); base.OnMouseUp(mevent); }

		// Custom Paint to handle dark theme rendering for the checkbox
		protected override void OnPaint(PaintEventArgs pevent)
		{
			if (this.Appearance == Appearance.Button)
			{
				base.OnPaint(pevent);
				return;
			}

			// Clear background
			using (SolidBrush backBrush = new SolidBrush(this.BackColor))
			{
				pevent.Graphics.FillRectangle(backBrush, this.ClientRectangle);
			}

			// Draw CheckBox
			// We use ControlPaint which is safer if VisualStyles are missing or problematic
			ButtonState state = ButtonState.Normal;
			if (this.Checked)
				state |= ButtonState.Checked;

			if (!this.Enabled)
				state |= ButtonState.Inactive;

			if (this.ThreeState && this.CheckState == CheckState.Indeterminate)
				state |= ButtonState.Checked;

			if (isPressed)
				state |= ButtonState.Pushed;

			// Get the size of the check box glyph (standard 13x13 usually, but scaling matters)
			int glyphSize = 13;

			int glyphY = (this.Height - glyphSize) / 2;
			Rectangle glyphRect = new Rectangle(0, glyphY, glyphSize, glyphSize);

			// Use DrawCheckBox
			ControlPaint.DrawCheckBox(pevent.Graphics, glyphRect, state | ButtonState.Flat);

			// Draw Text
			Color textColor = this.ForeColor;
			if (!this.Enabled)
			{
				// Dark background check: if average brightness < 128
				int bVal = (int)(this.BackColor.R * 0.3 + this.BackColor.G * 0.59 + this.BackColor.B * 0.11);
				if (bVal < 100)
				{
					textColor = Color.LightGray;
				}
				else
				{
					textColor = SystemColors.GrayText;
				}
			}

			TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.WordBreak;
			// padding
			Rectangle textRect = new Rectangle(glyphSize + 3, 0, this.Width - glyphSize - 3, this.Height);
			TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, textRect, textColor, flags);

			if (this.Focused)
			{
				ControlPaint.DrawFocusRectangle(pevent.Graphics, this.ClientRectangle);
			}
		}
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridListBox : ListBox, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridPanel : Panel, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridDateTimePicker : DateTimePicker, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridGroupBox : GroupBox, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridRichTextBox : RichTextBox, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridRadioButton : RadioButton, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridP4ObjectTreeListView : P4ObjectTreeListView, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridTreeListView : TreeListView, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	public class GridLayoutSubpanel : GridLayoutPanel, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridFilterComboBox : FilterComboBox, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridSplitContainer : SplitContainer, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
    public class GridSlidingPanelContainer : SlidingPanelContainer, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridDoubleBufferedListView : DoubleBufferedListView, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
	[Serializable]
	public class GridPictureBox : PictureBox, IGridLayoutCell
	{
		public int Row { get; set; }
		public int Column { get; set; }
		[Browsable(false)]
		public int YOffset { get; set; }
		[Browsable(false)]
		public int CellHeight { get; set; }
		[Browsable(false)]
		public int CellWidth { get; set; }

		public int RowsSpanned { get; set; }
		public int ColumnsSpanned { get; set; }
	}
}
