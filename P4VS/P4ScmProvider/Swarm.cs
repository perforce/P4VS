using Perforce.P4;
using Perforce.P4VS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perforce.P4Scm
{
    public class Swarm
    {
        private static string P4SwarmPropertyName = "P4.Swarm.URL";

        private P4.Repository repository;
        private string user;

        public Swarm(P4.Repository repository, string user)
        {
            this.repository = repository;
            this.user = user;
        }

        public SwarmApi.SSLCertificateHandler certHandler = null;

        public bool SwarmEnabled { get { return (string.IsNullOrEmpty(SwarmUrl) == false); } }
        public Credential SwarmCredential { get; set; }

        private string _swarmUrl;
        public string SwarmUrl
        {
            get
            {
                if (checkedForSwarm == false)
                {
                    // Hasn't been initialized, so look for the property 
                    //    on the Perforce server
                    CheckForSwarm();
                    return _swarmUrl;
                }
                return _swarmUrl;
            }
            private set { _swarmUrl = value; }
        }

        private bool _noSwarmPassword { get; set; }
        public string SwarmPassword
        {
            get
            {
                if (_noSwarmPassword)
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(SwarmUrl) || (repository == null) ||
                    (repository.Connection == null) || (repository.Connection.Server == null))
                {
                    // no Swarm server configured or not connected
                    return null;
                }
                if (SwarmCredential == null)
                {
                    Credential activeCred = repository.Connection.Login(null, new P4.LoginCmdOptions(LoginCmdFlags.DisplayStatus, null));
                    if ((activeCred == null) || string.IsNullOrEmpty(activeCred.Ticket) ||
                        (activeCred.Expires <= DateTime.Now))
                    {
                        return null;
                    }
                    SwarmCredential = activeCred;

                }
                return SwarmCredential.Ticket;
            }
        }
        public void noPassword()
        {
            _noSwarmPassword = true;
        }

        private bool checkedForSwarm = false;
        public void CheckForSwarm()
        {
            SwarmUrl = null;

            if ((repository == null) || (repository.Connection == null) ||
                repository.Connection.Status== ConnectionStatus.Disconnected ||checkedForSwarm)
            {
                return;
            }
            P4.P4Command propertyCmd = repository.Connection.CreateCommand("property", true);
            P4.Options opts = new P4.Options();
            opts["-l"] = null;
            opts["-n"] = P4SwarmPropertyName;

            P4.P4CommandResult results = null;
            try
            {
                results = propertyCmd.Run(opts);
            }
            catch (P4Exception ex)
            {
                if (ex.ErrorCode == P4.P4ClientError.MsgServer_Login2Required)
                {
                    throw ex;
                }
                // error getting property, likely not logged in
                return;
            }
            catch
            {
                // error getting property, likely not logged in
                return;
            }
            //command ran, so if no property than not attached to swarm
            checkedForSwarm = true;

            if (results.TaggedOutput != null)
            {
                foreach (TaggedObject tag in results.TaggedOutput)
                {
                    if (tag.ContainsKey("name") && tag["name"].StartsWith(P4SwarmPropertyName) && tag.ContainsKey("value"))
                    {
                        SwarmUrl = tag["value"].TrimEnd('/');

                        SwarmApi.SwarmServer sw = new SwarmApi.SwarmServer(SwarmUrl, user, SwarmPassword);
                             
                        if (certHandler == null)
                        {
                            certHandler = new ScmSSLCertificateHandler();
                            certHandler.Init(SwarmUrl);
                        }
                        SwarmVersion = sw.GetVersion;
            
						if (SwarmVersion==null)
						{
							SwarmUrl = null;
							return;
						}

                        string msg = String.Format(Resources.P4ScmProvider_ConnectedToSwarmServer,
                                                   SwarmUrl, SwarmVersion.version);
                        P4VsOutputWindow.AppendMessage(msg);

                        FileLogger.LogMessage(3, "P4API.NET", msg);
                        return;
                    }
                }
            }
            return;
        }

        public SwarmApi.SwarmServer.Version SwarmVersion { get; private set; }
        public bool SwarmAPI1_1
        {
            get
            {
                if ((SwarmVersion != null) && (SwarmVersion.apiVersions != null) && (SwarmVersion.apiVersions.Count > 0))
                {
                    foreach (string v in SwarmVersion.apiVersions)
                    {
                        if (v == "1.1")
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool SwarmAPI7
        {
            get
            {
                if ((SwarmVersion != null) && (SwarmVersion.apiVersions != null) && (SwarmVersion.apiVersions.Count > 0))
                {
                    foreach (string v in SwarmVersion.apiVersions)
                    {
                        if (v == "7")
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public SwarmApi.SwarmServer GetSwarmServer()
        {
            return new SwarmApi.SwarmServer(SwarmUrl, user, SwarmPassword);
        }

        public bool IsChangelistAttachedToReview(IDictionary<int, SwarmApi.SwarmServer.Review> changes)
        {
            if (SwarmEnabled == false)
            {
                return false;
            }
            SwarmApi.SwarmServer.ReviewList l = null;
      
            bool success = false;
            List<int> changeIds = null;

            SwarmApi.SwarmServer sw = new SwarmApi.SwarmServer(SwarmUrl, user, SwarmPassword);

            int[] allChangeIds = changes.Keys.ToArray();

            int idx = 0;
            while (idx < allChangeIds.Length)
            {
                changeIds = new List<int>();
                int cnt = 0;
                while ((idx < allChangeIds.Length) && (cnt < 50))
                {
                    changeIds.Add(allChangeIds[idx++]);
                    cnt++;
                }
                SwarmApi.Options ops = new SwarmApi.Options();
                ops["change[]"] = new JSONParser.JSONArray(changeIds.ToArray());

                l = sw.GetReviews(ops);
                if ((l != null) && (l.Count > 0) && (l[0] != null) && (l[0] is SwarmApi.SwarmServer.Review))
                {
                    foreach (SwarmApi.SwarmServer.Review r in l)
                    {
                        foreach (int c in r.changes)
                        {
                            if (changes.ContainsKey(c))
                            {
                                changes[c] = r;
                            }
                        }
                    }
                    success = true;
                }
            }
            return success;   
        }

        public SwarmApi.SwarmServer.Review IsChangelistAttachedToReview(int change)
        {
            if (SwarmEnabled == false)
            {
                return null;
            }
            SwarmApi.SwarmServer.ReviewList l = null;

            SwarmApi.SwarmServer sw = new SwarmApi.SwarmServer(SwarmUrl, user, SwarmPassword);

            SwarmApi.Options ops = new SwarmApi.Options();
            ops["change[]"] = new JSONParser.JSONArray(new int[] { change });

            l = sw.GetReviews(ops);
            if ((l != null) && (l.Count > 0) && (l[0] != null) && (l[0] is SwarmApi.SwarmServer.Review))
            {
                return ((SwarmApi.SwarmServer.Review)l[0]);
            }
            return null;
        }

        public string SwarmHost
        {
            get
            {
                if (string.IsNullOrEmpty(SwarmUrl))
                {
                    // don't know the url
                    return null;
                }
                int idx = SwarmUrl.IndexOf("://");
                if (idx < 0)
                {
                    // url is not of the form http(s)://
                    return SwarmUrl;
                }
                return SwarmUrl.Substring(idx + 3);
            }
        }

    }
}
