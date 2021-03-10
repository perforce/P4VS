using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Perforce.P4VS
{
	public class SlidingPanelContainer : Panel
	{
		//EventHandler _parentSizeChangedHandler = null;

		//private AutoSizeForm _dlgParent;
		//public AutoSizeForm DlgParent
		//{
		//    get { return _dlgParent; }
		//    set
		//    {
		//        if ((_dlgParent != null) && (_parentSizeChangedHandler != null))
		//        {
		//            _dlgParent.SizeChanged -= _parentSizeChangedHandler;
		//        }
		//        _dlgParent = value;
		//        if (_dlgParent != null)
		//        {
		//            if (_parentSizeChangedHandler == null)
		//            {
		//                _parentSizeChangedHandler = new EventHandler(DlgParent_SizeChanged);
		//            }
		//            _dlgParent.SizeChanged += _parentSizeChangedHandler;
		//        }
		//    }
		//}

		//public AutoSizeForm DlgParent;

		public SlidingPanelContainer()
		{
			//DlgParent = null;
			InitializeComponent();

#if DEBUG_LAYOUT
			this.BorderStyle = BorderStyle.Fixed3D;
#else
			this.BorderStyle = BorderStyle.None;
#endif
		}

		public new bool Visible
		{
			get { return base.Visible; }
			set
			{
				base.Visible = value;
				if (value)
				{
					BringToFront();
				}
				else
				{
					SendToBack();
				}
				foreach (Control c in Controls)
				{
					c.Visible = value;
					if (value)
					{
						c.BringToFront();
					}
					else
					{
						c.SendToBack();
					}
				}
			}
		}
		//bool ScaleControlFactorDisplayed = false;

		SizeF scaleFactor = new SizeF(1,1);

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			//if (ScaleControlFactorDisplayed == false)
			//{
			//    string msg = string.Format("SlidingPanelContainer.ScaleControl factor(x,y): {0}, {1}", factor.Width, factor.Height);
			//    msg += string.Format("\r\nthis.Size(x,y): {0}, {1}", this.Size.Width, this.Size.Height);
			//    int totHeight = (int)(this.Size.Height * factor.Height);
			//    msg += string.Format("\r\ntotalHeight: {0}", totHeight);
			//    MessageBox.Show(msg);
			//    ScaleControlFactorDisplayed = true;
			//}
			scaleFactor = factor;

			//base.ScaleControl(factor, specified);
			Rectangle bounds = new Rectangle(this.Location, this.Size);
			bounds = this.GetScaledBounds(bounds,factor,specified);

			//if ((this.DesignMode == false) && (ScaleControlFactorDisplayed == false))
			//{
			//    string msg = string.Format("SlidingPanelContainer.ScaleControl factor(x,y): {0}, {1}", factor.Width, factor.Height);
			//    msg += string.Format("\r\nthis.oldBounds(x,y,Width,Height): {0}, {1}, {2}, {3}", this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);
			//    msg += string.Format("\r\nthis.newBounds(x,y,Width,Height): {0}, {1}, {2}, {3}", bounds.X, bounds.Y, bounds.Width, bounds.Height);
			//    if (DlgParent != null)
			//    {
			//        msg += string.Format("\r\nDlgParent.Bounds(x,y,Width,Height): {0}, {1}, {2}, {3}", DlgParent.Location.X, DlgParent.Location.Y, DlgParent.Size.Width, DlgParent.Size.Height);
			//        ScaleControlFactorDisplayed = true;
			//    }
			//    MessageBox.Show(msg);
			//}

			if ((specified & BoundsSpecified.Location) != 0)
			{
				this.Location = bounds.Location;
			}
			if ((specified & BoundsSpecified.Location) != 0)
			{
				this.Size = bounds.Size;
			}
			foreach (Control c in this.Controls)
			{
				if ((c is SlidingPanel) == false)
				{
					// don't scale subpanels
					c.Scale(factor);
					continue;
				}
			}
		}

		public void LayoutPanel()
		{
			if (this.DesignMode)
			{
				return;
			}
			this.SuspendLayout();

			//if (DlgParent != null)
			//{
			//    scaleFactor = DlgParent.ScaleFactor;

			//    if (ScaleFactorDisplayed == false)
			//    {
			//        int totHeight = (int)(this.Size.Height / scaleFactor.Height);
			//        string msg = string.Format("Scale Factor(x,y): {0}, {1}", scaleFactor.Width, scaleFactor.Height);
			//        msg += string.Format("\r\nthis.Size(x,y): {0}, {1}", this.Size.Width, this.Size.Height);
			//        msg += string.Format("\r\ntotalHeight: {0}", totHeight);
			//        MessageBox.Show(msg);
			//        ScaleFactorDisplayed = true;
			//    }
			//
			//}

			int totalHeight = this.Size.Height; // (int)(this.Size.Height * scaleFactor.Height);
			int collapsedPanelsHeight = 0;
			int totalWeight = 0;

			foreach (Control c in this.Controls)
			{
				if ((c is SlidingPanel) == false)
				{
					// only interested in subpanels
					continue;
				}
				SlidingPanel subPanel = c as SlidingPanel;
				if (!subPanel.Hidden)
				{
					if (subPanel.Collapsed)
					{
						collapsedPanelsHeight += (int)(subPanel.CollapsedHeight * scaleFactor.Height);
					}
					else// if (subPanel.Visible)
					{
						totalWeight += subPanel.Weight;
					}
				}
			}

			float unitHeight = 1;
			if (totalWeight > 0)
			{
				unitHeight = (totalHeight - collapsedPanelsHeight) / totalWeight;
			}
			int currentPosition = 0;

			SlidingPanel LastPanelSized = null;

			foreach (Control c in this.Controls)
			{
				if ((c is SlidingPanel) == false)
				{
					// only interested in subpanels
					continue;
				}
				SlidingPanel subPanel = c as SlidingPanel;

				if (!subPanel.Hidden)
				{
					c.Location = new Point(0, currentPosition);

					subPanel.PanelHeight = subPanel.CollapsedHeight;
					if (subPanel.Collapsed == false)
					{
						subPanel.PanelHeight = (int)(subPanel.Weight * unitHeight);

						LastPanelSized = subPanel;
					}
					currentPosition += subPanel.PanelHeight;
					//subPanel.PanelHeight = (int)(subPanel.PanelHeight / scaleFactor.Height);
				}
			}
			int slop = totalHeight - currentPosition;
			if ((slop > 0) && (LastPanelSized != null))
			{
				//leftover verticle space
				LastPanelSized.PanelHeight = LastPanelSized.PanelHeight + slop;
			}
			
			currentPosition = 0;

			foreach (Control c in this.Controls)
			{
				if ((c is SlidingPanel) == false)
				{
					// only interested in subpanels
					continue;
				}
				SlidingPanel subPanel = c as SlidingPanel;

				if (!subPanel.Hidden)
				{
					c.Location = new Point(0, currentPosition);

					currentPosition += subPanel.PanelHeight;
					//subPanelHeight = (int)(subPanelHeight / scaleFactor.Height);

					c.Size = new Size(this.Size.Width, subPanel.PanelHeight);
//					c.Size = new Size((int)(this.Size.Width * scaleFactor.Width), subPanelHeight);
				}
			}
			//if (parent != null)
			//{
			//    parent.AutoScaleMode = oldAutoScaleMode;
			//}

			this.ResumeLayout();
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// SlidingPanel
			// 
			this.SizeChanged += new System.EventHandler(this.SlidingPanel_SizeChanged);

			this.ResumeLayout(false);

		}

		private void SlidingPanel_SizeChanged(object sender, EventArgs e)
		{
			LayoutPanel();
		}

		//bool ClientSizeDisplayed = false;

		//private void DlgParent_SizeChanged(object sender, EventArgs e)
		//{
		//    if ((this.DesignMode == false) && (DlgParent != null) && (ClientSizeDisplayed == false) && ((this.Size.Height != DlgParent.ClientSize.Height) || (this.Size.Width != DlgParent.ClientSize.Width)))
		//    {
		//        string msg = string.Format("Client size mismatch!!!\r\n");
		//        msg += string.Format("\r\nthis.Size(x,y): {0}, {1}", this.Size.Width, this.Size.Height);
		//        msg += string.Format("\r\nDlgParent.ClientSize(x,y): {0}, {1}", DlgParent.ClientSize.Width, DlgParent.ClientSize.Height);
		//        MessageBox.Show(msg);

		//        ClientSizeDisplayed = true;
		//    }
		//    if ((DlgParent != null) && ((this.Size.Height != DlgParent.ClientSize.Height) || (this.Size.Width != DlgParent.ClientSize.Width)))
		//    {
		//        this.Location = new Point(0, 0);
		//        this.Size = DlgParent.ClientSize;
		//    }
		//}
	}

	//class ShowButton : Button
	//{
	//    public ShowButton()
	//    {
	//        this.OnClick +=
	//    }

	//    bool _buttonDown = false;
	//    bool ButtonDown
	//    {
	//        get { return _buttonDown; } 
	//        set
	//        {
	//            _buttonDown = value;
	//        }
	//    }
	//}

}
