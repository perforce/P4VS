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
	public partial class ShelveFileCreateChangelistDlg : AutoSizeForm
	{
		public ShelveFileCreateChangelistDlg()
		{
			PreferenceKey = "ShelveFileCreateChangelistDlg";

			InitializeComponent();
            this.Icon = Images.icon_p4vs_16px;
		}

		public string Description
		{
			get { return DescriptionTB.Text; }
			set { DescriptionTB.Text = value; }
		}

		public string Prompt
		{
			get { return PromptLbl.Text; }
			set { PromptLbl.Text = value; }
		}

		private void DescriptionTB_TextChanged(object sender, EventArgs e)
		{
			OkBtn.Enabled = DescriptionTB.Text.Length > 0;
		}
	}
}
