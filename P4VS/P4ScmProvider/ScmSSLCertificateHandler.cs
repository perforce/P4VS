using Perforce.P4VS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Perforce.P4Scm
{
    class ScmSSLCertificateHandler : SwarmApi.SSLCertificateHandler
    {
        public string CertHash { get; private set; }
        public bool ExceptionApproved { get; private set; }// approved
        public bool ExceptionRejected { get; private set; }// rejected

        public ScmSSLCertificateHandler()
        {
            CertHash = null;
            ExceptionApproved = false;
        }
        
        /// <summary>
        /// Validates the SSL server certificate.
        /// </summary>
        /// <param name="sender">An object that contains state information for this
        /// validation.</param>
        /// <param name="cert">The certificate used to authenticate the remote party.</param>
        /// <param name="chain">The chain of certificate authorities associated with the
        /// remote certificate.</param>
        /// <param name="sslPolicyErrors">One or more errors associated with the remote
        /// certificate.</param>
        /// <returns>Returns a boolean value that determines whether the specified
        /// certificate is accepted for authentication; true to accept or false to
        /// reject.</returns>
        public override bool ValidateServerCertficate(
                object sender,
                X509Certificate cert,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                // Good certificate.
                return true;
            }

            string msg = string.Format(P4VS.Resources.SSLCertificateHandler_SSLCertificateError, sslPolicyErrors);
            P4VS.P4VsOutputWindow.AppendMessage(msg);
            P4VS.FileLogger.LogMessage(3, "SwarmAPI", msg);

            string certHash = cert.GetCertHashString();

            if (certHash == CertHash)
            {
                // same certificate we looked at before
                if (ExceptionApproved == true)
                {
                    // Already approved
                    return true;
                }
                if (ExceptionRejected == true)
                {
                    //Already rejected
                    return false;
                }
            }
            if ((!ExceptionRejected) && ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNotAvailable) == 0))
            {
                // likely a self signed certificate.
                string serverName = ServerUrl;
                int slashIdx = ServerUrl.LastIndexOf('/');
                if (slashIdx > 0)
                {
                    serverName = ServerUrl.Substring(slashIdx + 1);
                }
                string key = string.Format("SSlCertificateException_{0}", serverName.Replace(':', '_'));
                CertHash = P4VS.Preferences.LocalSettings.GetString(key, null);
                if ((CertHash == null) || (certHash != CertHash))
                {
                    // haven't seen one for this URL or it's a different certificate
                    SSLCertificateErrorDlg dlg = new SSLCertificateErrorDlg();
                    dlg.CertificateText = cert.ToString(true);
                    List<string> certErrors = new List<string>();
                    if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
                    {
                        certErrors.Add(Resources.P4ScmProvider_RemoteCertificateNameMismatch);
                    }
                    if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
                    {
                        foreach (X509ChainStatus err in chain.ChainStatus)
                        {
                            certErrors.Add(err.StatusInformation);
                        }
                    }
                    dlg.CertificateErrors = certErrors;

                    if (System.Windows.Forms.DialogResult.Yes == dlg.ShowDialog())
                    {
                        // add the exeption
                        P4VS.Preferences.LocalSettings[key] = certHash;
                        ExceptionApproved = true;
                        return true;
                    }
                    ExceptionRejected = true;
                    return false;
                }
                ExceptionApproved = true;
                return true;
            }

            // Return true => allow unauthenticated server,
            //        false => disallow unauthenticated server.
            return false;
        }
    }
}
