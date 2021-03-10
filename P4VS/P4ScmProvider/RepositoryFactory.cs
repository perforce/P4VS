using NLog;
using Perforce.P4VS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perforce.P4Scm
{
    public class RepositoryFactory
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static P4.Repository Repository;

        public static P4.Repository get(String Port, String User, String Workspace)
        {
            // Verify Paramiters
            if (string.IsNullOrEmpty(Port))
            {
                throw new P4.P4Exception(P4.ErrorSeverity.E_FATAL, "P4PORT is invalid or not set");
            }

            // Set connection options
            P4.Options options = new P4.Options();
            options["ProgramName"] = "P4VS";
            options["ProgramVersion"] = Versions.product();

            // Get Server connection
            P4.Server server = new P4.Server(new P4.ServerAddress(Port));

            // Create new Repository
            Repository = new P4.Repository(server);

            // set User
            if (!string.IsNullOrEmpty(User))
            {
                Repository.Connection.UserName = User;
            }

            // set Client
            if (!string.IsNullOrEmpty(Workspace))
            {
                P4.Client client = new P4.Client();
                client.Name = Workspace;
                Repository.Connection.Client = client;
            }
            
            // Connect...
            try
            {
                logger.Debug("AsyncConnectToRepository - Connect");
                bool connectResults = Repository.Connection.Connect(options);
                logger.Debug("AsyncConnectToRepository, result {0}", connectResults);
            }
            catch (P4.P4Exception ex)
            {
                // Look for trust errors
                checkTrust(ex);
            }

            // Set Command Timeout limits
            TimeSpan defCmdTimeout = TimeSpan.FromSeconds(30);
            TimeSpan cmdTimeout = Preferences.LocalSettings.GetTimeSpan("CommandTimeOut", defCmdTimeout);
            Repository.Connection.CommandTimeout = cmdTimeout;

            return Repository;
        }

        private static void checkTrust(P4.P4Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            logger.Trace("ConnectToRepository(options) threw P4Exception: {0}\r\n{1}", ex.Message, ex.StackTrace);

            string exception = ex.Message.Replace("Perforce", "Helix Core server");
            string[] sslMsg = exception.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);


            DialogResult result = DialogResult.Cancel;

            if (ex.ErrorCode == P4.P4ClientError.MsgRpc_HostKeyMismatch)
            {
                result = SslPromptDlg.ShowNewFingerprint(sslMsg);
            }
            else if (ex.ErrorCode == P4.P4ClientError.MsgRpc_HostKeyUnknown)
            {
                result = SslPromptDlg.ShowFirstContact(sslMsg);
            }
            else
            {
                throw ex;
            }

            // User canceled
            if (result == DialogResult.Cancel)
            {
                throw new P4.P4TrustException(P4.ErrorSeverity.E_FAILED, exception);
            }

            string fingerprint = SslPromptDlg.FingerPrint;
            Repository.Connection.TrustAndConnect(null, "-i", fingerprint);
        }
    }
}
