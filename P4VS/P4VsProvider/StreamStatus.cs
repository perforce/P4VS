using Microsoft.VisualStudio.Shell;
using NLog;
using Perforce.P4Scm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perforce.P4VS
{
    public class StreamStatus : BackwardsCompatibleAsyncPackage
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool _StreamexistsChecked = false;
        private bool _Streamexists = false;

        public bool exists()
        {
            try
            {
                if (_StreamexistsChecked)
                {
                    return _Streamexists;
                }
                _StreamexistsChecked = true;

				P4ScmProvider scm = P4VsProvider.CurrentScm;

				IList<P4.Depot> depots = scm.GetDepots();

                foreach (P4.Depot depot in depots)
                {
                    if (depot.Type == P4.DepotType.Stream)
                    {
                        _Streamexists = true; ;
                        return _Streamexists;
                    }
                }

                _Streamexists = false;
                return _Streamexists;
            }
            catch (Exception ex)
            {
                logger.Trace("Error in Streamexists(), {0}: \r\n{1}", ex.Message, ex.StackTrace);

                _Streamexists = false;
                return _Streamexists;
            }
        }

        public void clearExists()
        {
            _StreamexistsChecked = false;
        }


        bool _StreamclientChecked = false;
        private string _Streamclient = null;
        public string getStream()
        {
            try
            {
                if (_StreamclientChecked)
                {
                    return _Streamclient;
                }
                _StreamclientChecked = true;

				P4ScmProvider scm = P4VsProvider.CurrentScm;

				if (scm.Connection.Disconnected)
                {
                    _Streamclient = null;
                    return _Streamclient;
                }

                P4.Client current = scm.getClient(scm.Connection.Workspace.ToString(), null);

                if (current.Stream != null)
                {
                    _Streamclient = current.Stream;
                    return _Streamclient;
                }
            }
            catch (Exception ex)
            {
                logger.Trace("Error in P4Vversion(), {0}: \r\n{1}", ex.Message, ex.StackTrace);
            }
            _Streamclient = null;
            return _Streamclient;
        }

        public void clearClient()
        {
            _StreamclientChecked = false;
        }

    }
}
