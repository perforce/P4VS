using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perforce.P4VS
{
    public class P4VStatus
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // cache values
        private static double _versionnum = -1;
        private static bool _P4VexistsChecked = false;
        private static bool _P4Vexists = false;

        /// <summary>
        /// find install location of Perforce applications
        /// </summary>
        /// <returns>install path</returns>
        private string P4InstallLocation()
        {
            const string x64subkey = "SOFTWARE\\WOW6432Node\\Perforce\\Environment";
            const string x86subkey = "SOFTWARE\\Perforce\\Environment";

            RegistryKey key;
            object item = null;

            // x64 local machine
            key = Registry.LocalMachine.OpenSubKey(x64subkey);
            if (key != null)
            {
                item = key.GetValue("P4INSTROOT");
                if (item != null) { return item.ToString(); }
            }

            // x64 local user
            key = Registry.CurrentUser.OpenSubKey(x64subkey);
            if (key != null)
            {
                item = key.GetValue("P4INSTROOT");
                if (item != null) { return item.ToString(); }
            }

            // x86 local machine
            key = Registry.LocalMachine.OpenSubKey(x86subkey);
            if (key != null)
            {
                item = key.GetValue("P4INSTROOT");
                if (item != null) { return item.ToString(); }
            }

            // x86 local user
            key = Registry.CurrentUser.OpenSubKey(x86subkey);
            if (key != null)
            {
                item = key.GetValue("P4INSTROOT");
                if (item != null) { return item.ToString(); }
            }

            return null;
        }
        public bool exists()
        {
            try
            {
                if (_P4VexistsChecked)
                {
                    return _P4Vexists;
                }
                _P4VexistsChecked = true;

                if (Preferences.LocalSettings.ContainsKey("P4V_path"))
                {
                    if (File.Exists(Preferences.LocalSettings["P4V_path"].ToString()))
                    {
                        _P4Vexists = true;
                        return _P4Vexists;
                    }
                }

                string installRoot = P4InstallLocation();
                if (installRoot !=null && File.Exists(installRoot + "p4v.exe"))
                {
                    Preferences.LocalSettings["P4V_path"] = installRoot + "p4v.exe";
                    _P4Vexists = true;
                    return _P4Vexists;
                }

                _P4Vexists = false;
                return _P4Vexists;
            }
            catch (Exception ex)
            {
                logger.Trace("Error in P4Vexists(), {0}: \r\n{1}", ex.Message, ex.StackTrace);

                _P4Vexists = false;
                return _P4Vexists;
            }
        }

        public double version()
        {
            try
            {
                if (_versionnum != -1)
                {
                    return _versionnum;
                }
                string version = FileVersionInfo.GetVersionInfo(Preferences.LocalSettings["P4V_path"].ToString()).ToString();

                string[] filelines = version.Split('\n');

                string line = filelines[3].Replace("FileVersion:", "").Trim();

                line = line.Remove(6);

                _versionnum = Convert.ToDouble(line);

                return _versionnum;
            }
            catch (Exception ex)
            {
                logger.Trace("Error in P4Vversion(), {0}: \r\n{1}", ex.Message, ex.StackTrace);

                _versionnum = -1;
                return 2010.2;
            }
        }
    }
}
