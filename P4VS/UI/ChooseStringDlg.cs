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
	public partial class ChooseStringDlg : AutoSizeForm
	{
		public static string Show(string caption, string prompt, string[] items, string selectedItem)
		{
			ChooseStringDlg dlg = new ChooseStringDlg(caption, prompt, items, selectedItem);
			if (dlg.ShowDialog() == DialogResult.Cancel)
			{
				return null;
			}
			return dlg.Result;
		}

		public ChooseStringDlg(string caption, string prompt, string[] items, string selectedItem)
		{
			PreferenceKey = "ChooseStringDlg";

			InitializeComponent();

            this.Icon = Images.icon_p4vs_16px;
			this.Text = caption;
			PromptLbl.Text = prompt;

			foreach (string item in items)
			{
				ItemsLB.Items.Add(item);
			}
			if (string.IsNullOrEmpty(selectedItem) == false)
			{
				ItemsLB.SelectedItem = selectedItem;
			}
			else
			{
				// Clear the selection
				ItemsLB.SelectedIndex = -1;
			}
			OkBtn.Enabled = ItemsLB.SelectedIndex >= 0;
		}

		public string Result
		{
			get 
			{
				if (ItemsLB.SelectedIndex >= 0)
				{
					return (string)ItemsLB.SelectedItem;
				}
				return string.Empty;
			}
		}

		private void ChooseDlg_HelpButtonClicked(object sender, CancelEventArgs e)
		{

		}

		private void ItemsLB_SelectedIndexChanged(object sender, EventArgs e)
		{
			OkBtn.Enabled = ItemsLB.SelectedIndex >= 0;
		}

		private void ItemsLB_DoubleClick(object sender, EventArgs e)
		{
			OkBtn.Enabled = ItemsLB.SelectedIndex >= 0;
			if (OkBtn.Enabled)
			{
				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
