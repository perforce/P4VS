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
	public partial class LabelsBrowserDlg : AutoSizeForm
	{
		public LabelsBrowserDlg()
		{
			InitializeComponent();
            this.Icon = Images.label;
            labelsToolWindowControl1.Scm = P4VsProvider.CurrentScm;
		}

		public P4.Label Label { get { return this.labelsToolWindowControl1.Label; } }
	}
}
