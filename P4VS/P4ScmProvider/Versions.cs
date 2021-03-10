using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perforce.P4Scm
{
    public class Versions
    {
        private static string _productVersion = null;
        /// <summary>
        /// Get the file version for the executing assembly.
        /// </summary>
        public static string product()
        {
            if (_productVersion == null)
            {
                // Get the file version for the executing assembly.
                string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

                if (assemblyPath.StartsWith("file:"))
                {
                    assemblyPath = assemblyPath.Substring(5).TrimStart('\\', '/');
                }
                FileVersionInfo.GetVersionInfo(assemblyPath);
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyPath);

                // return the file version number.
                _productVersion = myFileVersionInfo.FileVersion;
            }
            return _productVersion;
        }


        public static Version V6_2 = new Version(2006, 2);
        public static Version V8_2 = new Version(2008, 2);
        public static Version V9_1 = new Version(2009, 1);
        public static Version V9_2 = new Version(2009, 2);
        public static Version V11_1 = new Version(2011, 1);
        public static Version V12_1 = new Version(2012, 1);
        public static Version V13_1 = new Version(2013, 1);
        public static Version V15_2 = new Version(2015, 2);
    }
}
