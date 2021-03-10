using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.P4VS
{
	public partial class SelectSubmittedChangelistDlg : AutoSizeForm
	{
		public SelectSubmittedChangelistDlg()
		{
			PreferenceKey = "SelectSubmittedChangelistDlg";

			InitializeComponent();
            this.Icon = Images.submitted;
		}
	}
}
