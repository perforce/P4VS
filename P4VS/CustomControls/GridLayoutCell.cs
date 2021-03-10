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
