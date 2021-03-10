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
    public partial class SSLCertificateErrorDlg : Perforce.P4VS.AutoSizeForm
    {
        public SSLCertificateErrorDlg()
        {
            InitializeComponent();
        }

        public string ErrorMessage
        {
            get { return MessageLbl.Text; }
            set { MessageLbl.Text =value; }
        }
        public string CertificateText
        {
            get { return CertificateTextTB.Text; }
            set { CertificateTextTB.Text = value; }
        }

        public IList<string> CertificateErrors
        {
            get { return (IList<string>)ErrorsLB.Items; }
            set
            {
                ErrorsLB.Items.Clear();
                foreach (string s in value)
                {
                    ErrorsLB.Items.Add(s);
                }
            }
        }

        private void SSLCertificateErrorDlg_Load(object sender, EventArgs e)
        {
            CertificateTextTB.Select(0, 0);
            NoBtn.Focus();
        }

        private void CopyToClipboardBtn_Click(object sender, EventArgs e)
        {
            StringBuilder msg = new StringBuilder(Resources.SSLCertificateErrorDlg_Errors);
            foreach (object o in ErrorsLB.Items)
            {
                msg.Append("\r\n");
                msg.Append(o.ToString());
            }
            msg.Append(Resources.SSLCertificateErrorDlg_CertificateData);
            msg.Append(CertificateTextTB.Text);
            System.Windows.Forms.Clipboard.SetText(msg.ToString());
        }
    }
}
