using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Perforce.P4VS
{
	public class FilterComboBox : ComboBox
	{
		internal MRUList mruValues { get; set; }
		internal bool mruLoaded { get; set; }

		public FilterComboBox()
		{
			mruLoaded = false;
			mruValues = new MRUList(10);
		}
		public void OnClose(string PreferenceKey)
		{
			if (PreferenceKey != null)
			{
				Preferences.LocalSettings[PreferenceKey] = mruValues;
			}
		}

		public void OnLoad(string PreferenceKey)
		{
			if ((PreferenceKey != null) && (Preferences.LocalSettings.ContainsKey(PreferenceKey)))
			{
				MRUList values = Preferences.LocalSettings[PreferenceKey] as MRUList;

				if ((values != null) && (values.Count > 0) && (values[0] != null))
				{
					mruValues = values;
					mruLoaded = true;
					base.Text = mruValues[0] as string;
				}
			}
		}

		public new string Text
		{
			get
			{
				if (base.Text != null)
				{
					mruValues.Add(base.Text);
				}
				return base.Text;
			}
			set
			{
				if (string.IsNullOrEmpty(value) == false)
				{
					mruValues.Add(value);
				}
				base.Text = value;
			}
		}

		protected override void OnDropDown(EventArgs e)
		{
            int widest = DropDownWidth;
			base.Items.Clear();
			for (int idx = 0; idx < mruValues.Capacity; idx++)
			{
				if ((mruValues[idx] != null) && (mruValues[idx] is string))
				{
					base.Items.Add(mruValues[idx] as string);
                    Image fakeImage = new Bitmap(1,1);
                    Graphics graphics = Graphics.FromImage(fakeImage);
                    SizeF measure = graphics.MeasureString(mruValues[idx].ToString(), Font);
                    if (measure.Width>widest)
                    {
                        widest = Convert.ToInt32(measure.Width);
                    }
				}
			}
            DropDownWidth = widest;
			base.OnDropDown(e);
		}
	}
}
