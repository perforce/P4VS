using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.P4VS
{
	public partial class LabelsToolWindowControlDetails : UserControl
	{
		public LabelsToolWindowControlDetails()
		{
			InitializeComponent();
		}

		public string LabelName
		{
			get { return LableNameLbl.Text; }
			set { LableNameLbl.Text = value; }
		}

		public string DateModified
		{
			get { return DateModifiedLbl.Text; }
			set { DateModifiedLbl.Text = value; }
		}

		public string LastAccessed
		{
			get { return LastAccessedLbl.Text; }
			set { LastAccessedLbl.Text = value; }
		}

		public string Owner
		{
			get { return OwnerLbl.Text; }
			set { OwnerLbl.Text = value; }
		}

		public string Description
		{
			get { return DescriptionTB.Text; }
			set { DescriptionTB.Text = value; }
		}

		public bool Locked
		{
			get { return LockedCB.Checked; }
			set { LockedCB.Checked = value; }
		}

		public bool IncludeAutoreloadOption
		{
			get { return AutoreloadCB.Visible; }
			set { AutoreloadCB.Visible = value; }
		}

		public bool Autoreload
		{
			get { return AutoreloadCB.Checked; }
			set { AutoreloadCB.Checked = value; }
		}

		public string Revision
		{
			get { return RevisionLbl.Text; }
			set { RevisionLbl.Text = value; }
		}

		public ListBox.ObjectCollection View
		{
			get { return ViewLB.Items; }
		}

		private bool inLabelsToolWindowControlDetails_SizeChanged = false;

		private void LabelsToolWindowControlDetails_SizeChanged(object sender, EventArgs e)
		{
			if (inLabelsToolWindowControlDetails_SizeChanged ||
                this.Parent==null)
			{
				// don't recurse
				return;
			}
			try
			{
				inLabelsToolWindowControlDetails_SizeChanged = true;

				Point pos = this.Location;
				Size sz = this.Size;

				Size ParentSize = this.Parent.Size;

				sz.Width = ParentSize.Width - pos.X;

				sz.Height = ParentSize.Height - pos.Y;

				this.Size = sz;
			}
			finally
			{
				inLabelsToolWindowControlDetails_SizeChanged = false;
			}
		}

		private bool inDescriptionTB_SizeChanged = false;

		private void DescriptionTB_SizeChanged(object sender, EventArgs e)
		{
			if (inDescriptionTB_SizeChanged)
			{
				// don't recurse
				return;
			}
			try
			{
				inDescriptionTB_SizeChanged = true;

				Point pos = DescriptionTB.Location;
				Size sz = DescriptionTB.Size;

				Size ParentSize = this.Size;

				//only grows horizontally
				sz.Width = ParentSize.Width - pos.X - 6;

				DescriptionTB.Size = sz;
			}
			finally
			{
				inDescriptionTB_SizeChanged = false;
			}
		}

		private bool inViewLB_SizeChanged = false;

		private void ViewLB_SizeChanged(object sender, EventArgs e)
		{
			if (inViewLB_SizeChanged)
			{
				// don't recurse
				return;
			}
			try
			{
				inViewLB_SizeChanged = true;

				Point pos = ViewLB.Location;
				Size sz = ViewLB.Size;

				Size ParentSize = this.Size;

				sz.Width = ParentSize.Width - pos.X - 6;
				sz.Height = ParentSize.Height - pos.Y - 6;

				ViewLB.Size = sz;
			}
			finally
			{
				inViewLB_SizeChanged = false;
			}
		}

		private bool inOptionsGB_SizeChanged = false;

		private void OptionsGB_SizeChanged(object sender, EventArgs e)
		{
			if (inOptionsGB_SizeChanged)
			{
				// don't recurse
				return;
			}
			try
			{
				inOptionsGB_SizeChanged = true;

				Point pos = OptionsGB.Location;
				Size sz = OptionsGB.Size;

				Size ParentSize = this.Size;

				//only grows horizontally
				sz.Width = ParentSize.Width - pos.X - 6;

				OptionsGB.Size = sz;
			}
			finally
			{
				inOptionsGB_SizeChanged = false;
			}
		}
	}
}
