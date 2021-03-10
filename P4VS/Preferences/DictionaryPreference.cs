using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perforce.P4VS
{
	public class KeyValuePairPreference : IPreference
	{
		public string Key { get; private set; }
		public object Value { get; private set; }

		public KeyValuePairPreference()
		{
		}

		public KeyValuePairPreference(string key, object value)
		{
			this.Key = key;
			this.Value = value;
		}

		public KeyValuePairPreference(KeyValuePairPreference kvp)
		{
			this.Key = kvp.Key;
			this.Value = kvp.Value;
		}

		public static KeyValuePairPreference Clone(KeyValuePairPreference kvp)
		{
			return new KeyValuePairPreference(kvp);
		}

		public KeyValuePairPreference Clone()
		{
			return new KeyValuePairPreference(this);
		}

		#region IPreference Members

		public void WriteTag(StringBuilder sb, ref int tabLevel, string tag)
		{
			PreferenceCache.StartBlock(sb, ref tabLevel, tag, GetType().FullName);
			PreferenceCache.SavePreference(sb, ref tabLevel, "Key", Key);
			PreferenceCache.SavePreference(sb, ref tabLevel, "Value", Value);
			PreferenceCache.EndBlock(sb, ref tabLevel, tag);
		}

		public IPreference FromXmlNode(System.Xml.XmlNode node)
		{
			this.Key = (string)PreferenceCache.LoadPreference(node.ChildNodes[0]);
			this.Value = (IPreference)PreferenceCache.LoadPreference(node.ChildNodes[1]);
			return this;
		}

		#endregion
	}

	public class DictionaryPreference : Dictionary<string, object>, IPreference
	{
		public DictionaryPreference()
		{
		}

		public DictionaryPreference(DictionaryPreference dict)
		{
			foreach (string key in dict.Keys)
			{
				this.Add(key, dict[key]);
			}
		}

		public static DictionaryPreference Clone(DictionaryPreference mru)
		{
			return new DictionaryPreference(mru);
		}

		public DictionaryPreference Clone()
		{
			return new DictionaryPreference(this);
		}

		#region IPreference Members

		public void WriteTag(StringBuilder sb, ref int tabLevel, string tag)
		{
			PreferenceCache.StartBlock(sb, ref tabLevel, tag, GetType().FullName);
			foreach (string key in this.Keys)
			{
				KeyValuePairPreference kvp = new KeyValuePairPreference(key, this[key]);
				PreferenceCache.SavePreference(sb, ref tabLevel, "Key", kvp);
			}
			PreferenceCache.EndBlock(sb, ref tabLevel, tag);
		}

		public IPreference FromXmlNode(System.Xml.XmlNode node)
		{
			int Capacity = node.ChildNodes.Count;
			for (int idx = 0; idx < node.ChildNodes.Count; idx++)
			{
				KeyValuePairPreference kvp = PreferenceCache.LoadPreference(node.ChildNodes[idx]) as KeyValuePairPreference;
				this.Add(kvp.Key, kvp.Value);
			}
			return this;
		}

		#endregion
	}
}
