using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perforce.P4VS.UI
{
    public partial class P4VSMessage : Form
    {
        public P4VSMessage(string title, string message)
        {
            InitializeComponent();
            Text = title;
            messageTB.Text = message;
        }

        private void messageTB_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(messageTB.Text, messageTB.Font);
            messageTB.Width = size.Width;
            messageTB.Height = size.Height;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void messageTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                if (sender != null)
                    ((TextBox)sender).SelectAll();
                e.Handled = true;
            }
        }

        private void messageTB_DoubleClick(object sender, EventArgs e)
        {
            if (sender != null)
                ((TextBox)sender).SelectAll();
        }

    }
}
