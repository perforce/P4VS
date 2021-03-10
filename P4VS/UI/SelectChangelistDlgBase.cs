using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perforce.P4VS
{
	public class SelectChangelistDlgBase : AutoSizeForm
	{

		public static int CurrentChangeList { get; set; }

		protected static DateTime CurrentChangeListLastUse = DateTime.MinValue;

		protected static TimeSpan CurrentChangeListValid = TimeSpan.FromMilliseconds(500);

		protected static int ActiveChangeList
		{
			get
			{
				if ((DateTime.Now - CurrentChangeListLastUse) < CurrentChangeListValid)
				{
					return CurrentChangeList;
				}
				return -2;
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectChangelistDlgBase));
			this.SuspendLayout();
			// 
			// SelectChangelistDlgBase
			// 
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SelectChangelistDlgBase";
			this.ResumeLayout(false);

		}
	}
}
