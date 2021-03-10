using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Perforce.P4VS
{
	public class ConnectionData : IPreference
	{
		public string ServerPort { get; set; }
		public string UserName { get; set; }
		public string Workspace { get; set; }

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", ServerPort, UserName, Workspace);
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(ConnectionData))
			{
				return false;
			}
			ConnectionData con = obj as ConnectionData;
			return con != null && ((con.ServerPort == ServerPort) && (con.UserName == UserName) && (con.Workspace == Workspace));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region IPreference Members

		public void WriteTag(StringBuilder sb, ref int tabLevel, string tag)
		{
			PreferenceCache.StartBlock(sb, ref tabLevel, tag, GetType().FullName);

			PreferenceCache.WriteTag(sb, tabLevel + 1, "ServerPort", "string", ServerPort);
			PreferenceCache.WriteTag(sb, tabLevel + 1, "UserName", "string", UserName);
			PreferenceCache.WriteTag(sb, tabLevel + 1, "Workspace", "string", Workspace);

			PreferenceCache.EndBlock(sb, ref tabLevel, tag);
		}

		public IPreference FromXmlNode(System.Xml.XmlNode node)
		{
			foreach (XmlNode child in node.ChildNodes)
			{
				if (child.Name == "ServerPort")
				{
					ServerPort = child.InnerText;
				}
				if (child.Name == "UserName")
				{
					UserName = child.InnerText;
				}
				if (child.Name == "Workspace")
				{
					Workspace = child.InnerText;
				}
			}
			return this;
		}

		#endregion
	}
}
