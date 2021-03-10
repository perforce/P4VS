using Perforce.P4Scm;
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
	public partial class FileHistoryDialog : AutoSizeForm
	{
		public FileHistoryDialog(P4ScmProvider scm)
		{
			PreferenceKey = "FileHistoryDialog";

			InitializeComponent();
			sccHistoryToolWindowControl1.Scm = scm;
		}

		public IList<string> Files
		{
			get { return this.sccHistoryToolWindowControl1.Files; }
			set { this.sccHistoryToolWindowControl1.Files = value; }
		}
	}
}
