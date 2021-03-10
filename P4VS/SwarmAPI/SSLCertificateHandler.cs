using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Perforce.SwarmApi
{
    public abstract class SSLCertificateHandler : IDisposable
    {
        public string ServerUrl { get; private set; }

        /// <summary>
        /// Somewhere in your application's startup/init sequence...
        /// </summary>
        public void Init(string serverUrl)
        {
            ServerUrl = serverUrl;
            // Override automatic validation of SSL server certificates.
            ServicePointManager.ServerCertificateValidationCallback =
                   ValidateServerCertficate;
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
        public abstract bool ValidateServerCertficate(
                object sender,
                X509Certificate cert,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ServicePointManager.ServerCertificateValidationCallback = null;
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SSLCertificateHandler() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
