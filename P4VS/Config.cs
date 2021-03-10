using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perforce.P4VS
{
	internal class Config
	{
		public static void Init()
		{
			UseExistingConnection = false;
		}

		public static bool UseExistingConnection { get; set; }

		public static String serverPort = "play:1999";
		public static String ServerPort
		{ 
			get { return UseExistingConnection ? null : serverPort; }
			set { serverPort = value; } 
		}

		public static String user = "bbaffy";
		public static String User
		{
			get { return UseExistingConnection ? null : user; }
			set { user = value; }
		}
		public static String password = null;
		public static String Password
		{
			get { return UseExistingConnection ? null : password; }
			set { user = value; }
		}

		public static String client = "bbaffy_win7_1999";
		public static String Client
		{
			get { return UseExistingConnection ? null : client; }
			set { client = value; }
		}
	}
}
