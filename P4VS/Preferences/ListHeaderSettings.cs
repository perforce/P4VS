using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace Perforce.P4VS
{
	class  ListHeaderColumnSettings : IPreference
	{
		public int Width { get; set; }
		public int DisplayIndex { get; set; }

		public ListHeaderColumnSettings()
		{
			Width = -1;
			DisplayIndex = -1;
		}

		public ListHeaderColumnSettings(int width, int displayIndex)
		{
			Width = width;
			DisplayIndex = displayIndex;
		}

		#region IPreference Members

		public void WriteTag(StringBuilder sb, ref int tabLevel, string tag)
		{
			PreferenceCache.StartBlock(sb, ref tabLevel, tag, GetType().FullName);

			PreferenceCache.WriteTag(sb, tabLevel + 1, "Width", "int", Width);
			PreferenceCache.WriteTag(sb, tabLevel + 1, "DisplayIndex", "int", DisplayIndex);

			PreferenceCache.EndBlock(sb, ref tabLevel, tag);
		}

		public IPreference FromXmlNode(System.Xml.XmlNode node)
		{
			foreach (XmlNode child in node.ChildNodes)
			{
				if (child.Name == "Width")
				{
					int v = -1;
					int.TryParse(child.InnerText, out v);
					Width = v;
				}
				if (child.Name == "DisplayIndex")
				{
					int v = -1;
					int.TryParse(child.InnerText, out v);
					DisplayIndex = v;
				}
			}
			return this;
		}

		#endregion
	}
	class ListHeaderSettings : List<ListHeaderColumnSettings>, IPreference
	{
		public ListHeaderSettings()
		{
		}
	
		public ListHeaderSettings(ListView.ColumnHeaderCollection Columns)
		{
			for (int idx = 0; idx < Columns.Count; idx++)
			{
				Add(new ListHeaderColumnSettings(Columns[idx].Width, Columns[idx].DisplayIndex));
			}
		}

		#region IPreference Members

		public void WriteTag(StringBuilder sb, ref int tabLevel, string tag)
		{
			string t = string.Format("{0} ColumnCount=\"{1}\"", tag, Count);
			PreferenceCache.StartBlock(sb, ref tabLevel, t, GetType().FullName);
			for (int idx = 0; idx < Count; idx++)
			{
				PreferenceCache.SavePreference(sb, ref tabLevel, "ColumnSettings", this[idx]);
			}
			PreferenceCache.EndBlock(sb, ref tabLevel, tag);
		}

		public IPreference FromXmlNode(System.Xml.XmlNode node)
		{
			if (node.Attributes != null && node.Attributes["ColumnCount"] != null)
			{
				int v;
				int.TryParse(node.Attributes["ColumnCount"].InnerText, out v);
				Capacity = v;

				for (int idx = 0; idx < v; idx++)
				{
					Add(null);
				}
			}
			else
			{
				Capacity = node.ChildNodes.Count;
			}

			for (int idx = 0; idx < node.ChildNodes.Count; idx++)
			{
				object o = (object)PreferenceCache.LoadPreference(node.ChildNodes[idx]);
				this[idx] = o as ListHeaderColumnSettings;
			}
			return this;
		}

		#endregion
	}
}
