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
	public partial class P4LicenseDlg : AutoSizeForm
	{
        private Point _defOKBtnLocation;

		private P4LicenseDlg(string msg)
		{
			PreferenceKey = "P4LicenseDlg";

			InitializeComponent();

			_defOKBtnLocation = OkBtn.Location;

			this.Icon = Images.icon_p4vs_16px;

			ErrorsTB.Text = msg;

            PromptLbl.Text = "License:";
		}

        public P4LicenseDlg()
		{
			InitializeComponent();

			_defOKBtnLocation = OkBtn.Location;
		}

		public static DialogResult Show(string msg)
		{
            P4LicenseDlg dlg = new P4LicenseDlg(msg);
			return dlg.ShowDialog();
		}

	}
}
