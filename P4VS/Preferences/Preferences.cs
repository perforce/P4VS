using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Web;
using NLog;

namespace Perforce.P4VS
{
	public interface IPreference
	{
		void WriteTag(StringBuilder sb, ref int tabLevel, string tag);

		IPreference FromXmlNode(XmlNode node);
	}
	

	public class MyPreference : IPreference
	{
		public string StringProperty { get; set; }
		public int IntProperty { get; set; }
		public bool BoolProperty { get; set; }

		public MyPreference() {}

		#region IPreference Members


		public void WriteTag(StringBuilder sb, ref int tabLevel, string tag)
		{
			PreferenceCache.StartBlock(sb, ref tabLevel, tag, this.GetType().FullName);
			PreferenceCache.WriteTag(sb, tabLevel, "StringProperty", "string", StringProperty);
			PreferenceCache.WriteTag(sb, tabLevel, "IntProperty", "int", IntProperty);
			PreferenceCache.WriteTag(sb, tabLevel, "BoolProperty", "bool", BoolProperty);
			PreferenceCache.EndBlock(sb, ref tabLevel, tag);
		}

		public static IPreference Creator(XmlNode node)
		{
			return new MyPreference().FromXmlNode(node);
		}

		public IPreference FromXmlNode(XmlNode node)
		{
			XmlNode curNode = node.SelectSingleNode("StringProperty");
			if (curNode != null)
			{
				StringProperty = (string) PreferenceCache.LoadPreference(curNode);
			}
			else
			{
				StringProperty = null;
			}

			curNode = node.SelectSingleNode("IntProperty");
			if (curNode != null)
			{
				IntProperty = (int)PreferenceCache.LoadPreference(curNode);
			}
			else
			{
				IntProperty = 0;
			}

			curNode = node.SelectSingleNode("BoolProperty");
			if (curNode != null)
			{
				BoolProperty = (bool)PreferenceCache.LoadPreference(curNode);
			}
			else
			{
				BoolProperty = false;
			}

			return this;
		}

		#endregion
	}

	public class Preferences
	{
		public static PreferenceCache SolutionSettings { get; private set; }

		public static ProjectPreferenceCache ProjectSettings { get; private set; }

		public static LocalPreferenceCache LocalSettings { get; private set; }

		public static void Initialize()
		{
			SolutionSettings = new PreferenceCache();

			ProjectSettings = new ProjectPreferenceCache();

			LocalSettings = new LocalPreferenceCache();
		}
	}

	public class PreferenceCache : Dictionary<string, object>
	{
		protected object syncHead = new object();

		private static string XmlEncode(string s)
		{
			// < to &lt;
			// > to &gt;
			// " to &quot;
			// ' to &#39;
			// & to &amp;
			string v = s.Replace("&", "&amp;");
			v = v.Replace("<", "&lt;");
			v = v.Replace(">", "&gt;");
			v = v.Replace("\"", "&quot;");
			v = v.Replace("'", "&apos;");

			return v;
		}

		private static string XmlDecode(string s)
		{
			// < from &lt;
			// > from &gt;
			// " from &quot;
			// ' from &#39;
			// & from &amp;
			string v = s.Replace("&lt;", "<");
			v = v.Replace("&gt;", ">");
			v = v.Replace("&quot;", "\"");
			v = v.Replace("&apos;", "'");
			v = s.Replace("&amp;", "&");

			return v;
		}

		public static void WriteTag(StringBuilder sb, int tabLevel, string tag, string type, object value)
		{
			for (int i = 0; i < tabLevel; i++)
			{
				sb.Append('\t');
			}
			string line = null;
			if (value == null)
			{
				line = string.Format("<{0} type=\"{1}\"/>", tag, type);
			}
			else
			{
				string escapedValue = XmlEncode(value.ToString());

				line = string.Format("<{0} type=\"{1}\">{2}</{0}>", tag, type, escapedValue);
			}
			sb.AppendLine(line);
		}

		public static void StartBlock(StringBuilder sb, ref int tabLevel, string tag, string type)
		{
			for (int i = 0; i < tabLevel; i++)
			{
				sb.Append('\t');
			}
			string line = string.Format("<{0} type=\"{1}\">", tag, type);
			sb.AppendLine(line);
			tabLevel++;
		}

		public static void EndBlock(StringBuilder sb, ref int tabLevel, string tag)
		{
			tabLevel--;
			for (int i = 0; i < tabLevel; i++)
			{
				sb.Append('\t');
			}
			string line = string.Format("</{0}>", tag);
			sb.AppendLine(line);
		}


		public static T GetNewObject<T>() 
		{ 
			try
			{
				var constructorInfo = typeof (T).GetConstructor(new Type[] {});
				if (constructorInfo != null)
					return (T)constructorInfo.Invoke(new object[] { });
			}
			catch 
			{
				return default(T); 
			}
			return default(T);
		}

		public static object GetNewObject(Type t) 
		{ 
			try
			{
				var constructorInfo = t.GetConstructor(new Type[] {});
				if (constructorInfo != null) return constructorInfo.Invoke(new object[] { });
			}
			catch
			{ 
				return null; 
			}
			return null;
		}

		public static object LoadPreference(XmlNode node)
		{
			try
			{
				Object obj = null;

				string value = node.InnerText; // XmlDecode(node.InnerText); (already decoded by parser)

				if (node.Attributes != null && node.Attributes["type"] != null)
				{

					switch (node.Attributes["type"].InnerText)
					{
						case "null":
							return null;
						case "int":
							int v = 0;
							int.TryParse(value, out v);
							return v;
						case "bool":
							return (node.InnerText == "True");
						case "string":
							return value;
						//case "MyPreference":
						//    return new MyPreference().FromXmlNode(node);
						case "TimeSpan":
							TimeSpan ts = TimeSpan.Zero;
							TimeSpan.TryParse(value, out ts);
							return ts;
						default:
							//if (PreferenceTypeMap.ContainsKey(node.Attributes["type"].InnerText))
							//{

							Type type = null;
							try
							{
								type = Type.GetType(node.Attributes["type"].InnerText);
							}
							catch
							{
								type = null;
							}
							//Type type = PreferenceTypeMap[node.Attributes["type"].InnerText];
							if (type != null)
							{
								try
								{
									obj = GetNewObject(type);

									IPreference p = (IPreference)obj;
									return p.FromXmlNode(node);
								}
								catch { }
							}
							//}
							// unknown object type
							return null;
					}
				}
				// no type, so assume string
				return value;
			}
			catch { }

			return null;
		}

		public static void SavePreference(StringBuilder sb, ref int tabLevel, string key, object obj)
		{
			if (obj == null)
			{
				WriteTag(sb, tabLevel, key, "null", null);
			}
			else if (obj is int)
			{
				WriteTag(sb, tabLevel, key, "int", obj);
			}
			else if (obj is string)
			{
				WriteTag(sb, tabLevel, key, "string", obj);
			}
			else if (obj is bool)
			{
				bool v = (bool)obj;
				WriteTag(sb, tabLevel, key, "bool", v ? "True" : "False");
			}
			else if (obj is TimeSpan)
			{
				WriteTag(sb, tabLevel, key, "TimeSpan", obj);
			}
			else if (obj is IPreference)
			{
				IPreference v = (IPreference)obj;
				v.WriteTag(sb, ref tabLevel, key);
			}
		}

		public virtual void Load(string xmlStr)
		{
			XmlDocument xDoc = new XmlDocument();
			xDoc.LoadXml(xmlStr);

			XmlNode docNode = xDoc.DocumentElement;

			if (docNode != null)
				foreach (XmlNode curNode in docNode.ChildNodes)
				{
					if (curNode != null)
					{
						base[curNode.Name] = LoadPreference(curNode);
					}
				}
		}

		public virtual string Save()
		{
			StringBuilder val = new StringBuilder(4096);
			int tablevel = 0;

			StartBlock(val, ref tablevel, "SolutionSettings", "Preferences");

			int tabLevel = 1;
			foreach (string key in this.Keys)
			{
				object obj = base[key];
				SavePreference(val, ref tabLevel, key, obj);
			}
			EndBlock(val, ref tablevel, "SolutionSettings");

			return val.ToString();
		}

		public new virtual object this[string key]
		{
			get
			{
				if (this.ContainsKey(key))
				{
					return base[key];
				}
				return null;
			}
			set
			{
				base[key] = value;
			}
		}

		public virtual object Get(string key)
		{
			if (this.ContainsKey(key))
			{
				return this[key];
			}
			throw new ArgumentException(string.Format("The setting, \"{0}\", does not exist or is not an int.", key), "key");
		}

		public virtual object Get(string key, object defaultValue)
		{
			if (this.ContainsKey(key))
			{
				return this[key];
			}
			return defaultValue;
		}

		public virtual int GetInt(string key)
		{
			if (this.ContainsKey(key) && (this[key] is int))
			{
				return (int)this[key];
			}
			throw new ArgumentException(string.Format("The setting, \"{0}\", does not exist or is not an int.", key), "key");
		}

		public virtual int GetInt(string key, int defaultValue)
		{
			if (this.ContainsKey(key) && (this[key] is int))
			{
				return (int)this[key];
			}
			return defaultValue;
		}

		public virtual bool GetBool(string key)
		{
			if (this.ContainsKey(key) && (this[key] is bool))
			{
				return (bool)this[key];
			}
			throw new ArgumentException(string.Format("The setting, \"{0}\", does not exist or is not a bool.", key),"key");
		}

		public virtual bool GetBool(string key, bool defaultValue)
		{
			if (this.ContainsKey(key) && (this[key] is bool))
			{
				return (bool)this[key];
			}
			return defaultValue;
		}

		public virtual string GetString(string key)
		{
			if (this.ContainsKey(key) && (this[key] is string))
			{
				return (string)this[key];
			}
			throw new ArgumentException(string.Format("The setting, \"{0}\", does not exist or is not a string.", key),"key");
		}

		public virtual string GetString(string key, string defaultValue)
		{
			if (this.ContainsKey(key) && (this[key] is string))
			{
				return (string)this[key];
			}
			return defaultValue;
		}

		public virtual TimeSpan GetTimeSpan(string key)
		{
			if (this.ContainsKey(key) && (this[key] is TimeSpan))
			{
				return (TimeSpan)this[key];
			}
			throw new ArgumentException(string.Format("The setting, \"{0}\", does not exist or is not a bool.", key), "key");
		}

		public virtual TimeSpan GetTimeSpan(string key, TimeSpan defaultValue)
		{
			if (this.ContainsKey(key) && (this[key] is TimeSpan))
			{
				return (TimeSpan)this[key];
			}
			return defaultValue;
		}

		public virtual IPreference GetPreference(string key)
		{
			if (this.ContainsKey(key) && (this[key] is IPreference))
			{
				return (IPreference)this[key];
			}
			throw new ArgumentException(string.Format("The setting, \"{0}\", does not exist or is not an IPreference.", key), "key");
		}

		public virtual IPreference GetPreference(string key, IPreference defaultValue)
		{
			if (this.ContainsKey(key) && (this[key] is IPreference))
			{
				return (IPreference)this[key];
			}
			return defaultValue;
		}

		public virtual void Set(string key, int val)
		{
			this[key] = val;
		}

		public virtual void Set(string key, bool val)
		{
			this[key] = val;
		}

		public virtual void Set(string key, string val)
		{
			this[key] = val;
		}

		public virtual void Set(string key, TimeSpan val)
		{
			this[key] = val;
		}

		public virtual void Set(string key, IPreference val)
		{
			this[key] = val;
		}
	}

	public class LocalPreferenceCache : PreferenceCache
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string _appData = null;
#if DEBUG
		private string _backupData = null;
#endif
		private DateTime _appDataTimeStamp = DateTime.MinValue;
		private DateTime _appDataChecked = DateTime.MinValue;
		private TimeSpan _minCheckTime = TimeSpan.FromMilliseconds(100); // .1 seconds

		public override void Load(string xml)
		{
			lock (syncHead)
			{
				// load the supplied data unless a null is passed, then read the file
				string xmlStr = xml;
				if (xml == null)
				{
					if (_appData == null)
					{
						_appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
						_appData = Path.Combine(_appData, "Perforce\\P4VS\\LocalSettings.ini");
					}
#if DEBUG
				if (_backupData == null)
				{
					_backupData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
					_backupData = Path.Combine(_backupData, "Perforce\\P4VS\\LocalSettings.bak");
				}
#endif
					try
					{
						if (File.Exists(_appData))
						{
							_appDataTimeStamp = File.GetLastWriteTime(_appData);
							using (StreamReader sr = new StreamReader(_appData))
							{
								xmlStr = sr.ReadToEnd();
							}

						}
						if (string.IsNullOrEmpty(xmlStr) == false)
						{
							base.Load(xmlStr);
						}
					}
					catch (Exception ex)
					{
						logger.Trace("Error reading local preferences file: {0}\r\n\t{1}", ex.Message, ex.StackTrace);
						base.Clear();
					}
				}
			}
		}

		public override string Save()
		{
			lock (syncHead)
			{
				// save the local settings
				if (_appData == null)
				{
					_appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
					_appData = Path.Combine(_appData, "Perforce\\P4VS\\LocalSettings.ini");
				}
#if DEBUG
				if (_backupData == null)
				{
					_backupData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
					_backupData = Path.Combine(_backupData, "Perforce\\P4VS\\LocalSettings.bak");
				}
#endif
				string xmlStr = null;
				try
				{
					string appDataFolder = Path.GetDirectoryName(_appData);
					if (Directory.Exists(appDataFolder) == false)
					{
						Directory.CreateDirectory(appDataFolder);
					}
#if DEBUG
					if (File.Exists(_backupData))
					{
						System.IO.File.Delete(_backupData);
					}
					if (File.Exists(_appData))
					{
						System.IO.File.Move(_appData, _backupData);
					}
#endif
					xmlStr = base.Save();
					if (string.IsNullOrEmpty(xmlStr))
					{
						return xmlStr;
					}
					using (StreamWriter sw = new StreamWriter(_appData, false))
					{
						sw.Write(xmlStr);
					}
					//refresh the time stamp
					_appDataTimeStamp = File.GetLastWriteTime(_appData);
				}
				catch (Exception ex)
				{
					logger.Trace("Error writing local preferences file: {0}\r\n\t{1}", ex.Message, ex.StackTrace);
				}
				return xmlStr;
			}
		}

		private void RefreshAppData(bool writing)
		{
			if (!string.IsNullOrEmpty(_appData) && File.Exists(_appData))
			{
				// Only check to see if another dev studio instance has changed the prefeneces if it's 
				// been more than .1 seconds since we last checked
				// Always check if we're about to write the file.

				if (!writing && (DateTime.Now - _appDataChecked) < _minCheckTime)
				{
					return;
				}
				_appDataChecked = DateTime.Now;

				DateTime appDataTimeStamp = File.GetLastWriteTime(_appData);
				if (appDataTimeStamp > _appDataTimeStamp)
				{
					// file has been written to since the last time we read it
					Load(null);
				}
			}
		}

		public override object this[string key]
		{
			get
			{
				RefreshAppData(false);
				return base[key];
			}
			set
			{
				RefreshAppData(true);
				base[key] = value;
				Save();
			}
		}

		public override void Set(string key, bool val)
		{
			RefreshAppData(true);
			base.Set(key, val);
			Save();
		}

		public override void Set(string key, string val)
		{
			RefreshAppData(true);
			this[key] = val;
			Save();
		}

		public override void Set(string key, IPreference val)
		{
			RefreshAppData(true);
			this[key] = val; // also saves the file
            //Save();
		}

		public new bool ContainsKey(string key)
		{
			RefreshAppData(false);
			return base.ContainsKey(key);
		}
	}
	 
	public class ProjectPreferenceCache : Dictionary<string, PreferenceCache>
	{

		#region Dictionary<string,PreferenceCache> overrides

		public new PreferenceCache this[string key]
		{
			get
			{
				if (base.ContainsKey(key) == false)
				{
					base.Add(key, new PreferenceCache());
				}
				return base[key];
			}
			set
			{
				base[key] = value;
			}
		}
		#endregion
	}
}
