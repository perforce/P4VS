using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.P4VS
{
	public class AutoSizeForm : Form
	{
		protected string PreferenceKey {get; set;}

		private void SaveControlSettings(Control.ControlCollection Controls)
		{
			foreach (Control c in Controls)
			{
				if (c is DoubleBufferedListView)
				{
					string listKey =
						string.Format("{0}_{1}", PreferenceKey, c.Name);
					((DoubleBufferedListView)c).OnClose(listKey);
				}
				else if (c is TreeListView)
				{
					string listKey =
						string.Format("{0}_{1}", PreferenceKey, c.Name);
					((TreeListView)c).OnClose(listKey);
				}
				else if (c is FilterComboBox)
				{
					string listKey =
						string.Format("{0}_{1}", PreferenceKey, c.Name);
					((FilterComboBox)c).OnClose(listKey);
				}
                else if (c is SplitContainer)
                {
                    string listKey =
                        string.Format("{0}_{1}", PreferenceKey, c.Name);
                    if (PreferenceKey != null && ((SplitContainer) c).SplitterDistance != 0)
                    {
                        Preferences.LocalSettings[listKey + "_SplitterDistance"] =
                            ((SplitContainer) c).SplitterDistance;
                    }
                }
                else if (c is P4ToolWindowControlBase)
                {
                    ((P4ToolWindowControlBase)c).SaveControlSettings();
                }
				if ((c.Controls != null) && (c.Controls.Count > 0))
				{
					SaveControlSettings(c.Controls);
				}
			}
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if (PreferenceKey != null)
			{
				Preferences.LocalSettings[PreferenceKey + "_Width"] = Width;
				Preferences.LocalSettings[PreferenceKey + "_Height"] = Height;

				Preferences.LocalSettings[PreferenceKey + "_Top"] = Top;
				Preferences.LocalSettings[PreferenceKey + "_Left"] = Left;

				SaveControlSettings(Controls);
			}
			base.OnClosing(e);
		}

		private void InitControlSettings(Control.ControlCollection Controls)
		{
			foreach (Control c in Controls)
			{
				if (c is DoubleBufferedListView)
				{
					string listKey =
						string.Format("{0}_{1}", PreferenceKey, c.Name);
					((DoubleBufferedListView)c).OnLoad(listKey);
				}
				else if (c is TreeListView)
				{
					string listKey =
						string.Format("{0}_{1}", PreferenceKey, c.Name);
					((TreeListView)c).OnLoad(listKey);
				}
				else if (c is FilterComboBox)
				{
					string listKey =
						string.Format("{0}_{1}", PreferenceKey, c.Name);
					((FilterComboBox)c).OnLoad(listKey);
				}
                else if (c is SplitContainer)
                {
                    string listKey =
                        string.Format("{0}_{1}", PreferenceKey, c.Name);
                    if (listKey != null && Preferences.LocalSettings.ContainsKey(listKey + "_SplitterDistance"))
                    {
                        ((SplitContainer)c).SplitterDistance =
                            (int)Preferences.LocalSettings[listKey + "_SplitterDistance"];
                    }
                }
                else if (c is P4ToolWindowControlBase)
                {
                    ((P4ToolWindowControlBase)c).Initialize();
                }
				if ((c.Controls != null) && (c.Controls.Count > 0))
				{
					InitControlSettings(c.Controls);
				}
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			if (PreferenceKey != null)
			{
				int width = Preferences.LocalSettings.GetInt(PreferenceKey + "_Width", -1);
				int height = Preferences.LocalSettings.GetInt(PreferenceKey + "_Height", -1);

				int top = Preferences.LocalSettings.GetInt(PreferenceKey + "_Top", -1);
				int left = Preferences.LocalSettings.GetInt(PreferenceKey + "_Left", -1);

				int minY = SystemInformation.VirtualScreen.Top;
				int minX = SystemInformation.VirtualScreen.Left;

				if ((Width > 0) && (Height > 0) && (top > minY) && (left > minX))
				{
					int maxY = SystemInformation.VirtualScreen.Height;
					int maxX = SystemInformation.VirtualScreen.Width;
					if (width > maxX)
					{
						width = maxX;
					}
					if (height > maxY)
					{
						height = maxY;
					}
					if (((top + height) > maxY) || ((left + width) > maxX))
					{
						StartPosition = FormStartPosition.CenterParent;
					}
					else
					{
						//top left corner of the dialog is visible on the screen
						StartPosition = FormStartPosition.Manual;
						Top = top;
						Left = left;
						Width = width;
						Height = height;
					}
				}
				else
				{
					StartPosition = FormStartPosition.CenterParent;
				}

				foreach (Control c in Controls)
				{
					if (c is I18nControls.GridLayoutPanel)
					{
						I18nControls.GridLayoutPanel pnl = c as I18nControls.GridLayoutPanel;
						pnl.InitializeLayout();
					}
				}

				InitControlSettings(Controls);
			}
			// Handle by the dialog class, so don't need this
			//if (this.AutoSize)
			//{
			//    int maxWidth = 0;
			//    int maxHeight = 0;
				
			//    foreach (Control c in Controls)
			//    {
			//        if (c.Right > maxWidth)
			//        {
			//            maxWidth = c.Right;
			//        }
			//        if (c.Bottom < maxHeight)
			//        {
			//            maxHeight = c.Bottom;
			//        }
			//    }
			//    System.Drawing.Size newSize  = new System.Drawing.Size(Size.Width, Size.Height);
			//    bool sizeSet = false;
			//    if (Size.Height < maxHeight)
			//    {
			//        newSize.Height = maxHeight;
			//        sizeSet = true;
			//    }
			//    if (Size.Width < maxWidth)
			//    {
			//        newSize.Width = maxWidth;
			//        sizeSet = true;
			//    }
			//    if (this.AutoSizeMode == System.Windows.Forms.AutoSizeMode.GrowAndShrink)
			//    {
			//        if (Size.Height > maxHeight)
			//        {
			//            newSize.Height = maxHeight;
			//            sizeSet = true;
			//        }
			//        if (Size.Width > maxWidth)
			//        {
			//            newSize.Width = maxWidth;
			//            sizeSet = true;
			//        }
			//    }
			//    if (sizeSet == true)
			//    {
			//        this.Size = newSize;
			//    }
			//}
			base.OnLoad(e);
		}

		public System.Drawing.SizeF ScaleFactor { get { return base.AutoScaleFactor; } }
	}
}
