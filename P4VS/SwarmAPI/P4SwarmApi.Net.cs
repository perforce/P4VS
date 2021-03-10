using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

// using statements for web requests and JSON Serialization
//using System.Xml;
using System.Net;
//using System.Runtime.Serialization.Json;

using NLog;
using Perforce;
using Perforce.JSONParser;
using Perforce.P4VS;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace Perforce.SwarmApi
{
	/// <summary>
	/// Encapsulate calls to a SwarmServer
	/// </summary>
	public class SwarmServer
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public class SwarmException : Exception
		{
			public SwarmException(string msg) : base(msg) { }
			public SwarmException(string msg,Exception innerException) : base(msg,innerException) { }
		};

		public SwarmServer()
		{
			SetConnection(null, null, null);
		}
		public SwarmServer(string swarmUri, string user, string password)
		{
			SetConnection(swarmUri, user, password);
		}

		/// <summary>
		/// Host name or ip address of the swarm host
		/// </summary>
		public string SwarmUri { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string User { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Password { get; set; }

		private string SwarmHost
		{
			get
			{
				if (string.IsNullOrEmpty(SwarmUri))
				{
					return null;
				}
				int idx = SwarmUri.IndexOf("://");
				if (idx < 0)
				{
					return null;
				}
				return SwarmUri.Substring(idx + 3);
			}
		}

		private string SwarmProtocol
		{
			get
			{
				if (string.IsNullOrEmpty(SwarmUri))
				{
					return null;
				}
				int idx = SwarmUri.IndexOf("://");
				if (idx < 1)
				{
					return null;
				}
				return SwarmUri.Substring(0, idx);
			}
		}
		private NetworkCredential credentials;

		//private string Ticket { get; set; }

		/// <summary>
		/// Set the values used to connect to the Swarm host
		/// </summary>
		/// <param name="swarmUri">Host name or ip address of the swarm host</param>
		/// <param name="user">Swarm user name</param>
		/// <param name="password">Can be the user password or a ticket valid on the Swarm host</param>
		/// <returns></returns>
		public void SetConnection(string swarmUri, string user, string password)
		{
			SwarmUri = swarmUri;
			User = user;
			Password = password;

			credentials = new NetworkCredential(user, password);
		}

		public void LaunchSwarmInBrowser(string location)
		{
			if (location.StartsWith("/") == false)
			{
				location = '/' + location;
			}
			// this string inserts user password "{0}://{1}:{2}@{3}{4}"
			// this string does not add user password "{0}://{3}{4}"
			string sUrl = string.Format("{0}://{3}{4}", 
				SwarmProtocol, User, Password, SwarmHost, location);

            if(Preferences.LocalSettings.GetBool("Launch_Swarm_Browser", false))
            {
                try
                {
                    System.Diagnostics.Process.Start(sUrl);
                }
                catch (Exception exc1)
                {
                    // System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser. 
                    // It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,
                    // then attempt to open the URL in IE instead.
                    if (exc1.GetType().ToString() != "System.ComponentModel.Win32Exception")
                    {
                        // sometimes throws exception so we have to just ignore
                        // this is a common .NET bug that no one online really has a great reason for so now we just need to try to open
                        // the URL using IE if we can.
                        try
                        {
                            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("IExplore.exe", sUrl);
                            System.Diagnostics.Process.Start(startInfo);
                            startInfo = null;
                        }
                        catch (Exception)
                        {
                            // still nothing we can do so just show the error to the user here.
                        }
                    }
                }
            }
            else
            {
                IVsWindowFrame ppFrame;
                var service = Package.GetGlobalService(typeof(IVsWebBrowsingService)) as IVsWebBrowsingService;
                service.Navigate(sUrl, 0, out ppFrame);
            }
        }

		public void ShowReviewInBrowser(int reviewId)
		{
			string location = string.Format("/reviews/{0}", reviewId);

			LaunchSwarmInBrowser(location);
		}

		public void SetBasicAuthHeader(WebRequest request, NetworkCredential credentials)
		{
			string authInfo = credentials.UserName + ":" + credentials.Password;
			authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
			request.Headers["Authorization"] = "Basic " + authInfo;

//			request.PreAuthenticate = true;
		}

		static int buffSize = 1024;
		static int stringBuilderSize = 4096 * 4;

		public JSONDoc HttpGet(string RequestUrl)
		{
			JSONDoc jDoc = null;

			try
			{
				HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(RequestUrl);
				SetBasicAuthHeader(req, credentials);
				req.ContentType = "application/x-www-form-urlencoded";
				req.Credentials = credentials;
				req.Method = "GET";

				WebResponse res = req.GetResponse();
				if (req.HaveResponse && (res.GetResponseStream() != null))
				{
					// Sometimes the stream reader 'stalls' when we try to tokenize while reading 
					//  the stream and only returns the partial result, so need to read the entire
					//  response stream before passing it to the JSON parser
					//TextReader str = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
					//jDoc = new JSONParser.JSONDoc(str);

					// read the entire response buffer into a string
					string str = string.Empty;
					if (res.ContentLength > 0)
					{
						// the easy way, we know how much we need to read
						byte[] buff;
						buff = new byte[res.ContentLength];
						int cnt = 0;
						while (cnt < (int)res.ContentLength)
						{
							int bytesLeft = (int)res.ContentLength - cnt;
							cnt += res.GetResponseStream().Read(buff, cnt, bytesLeft);
						}

						str = Encoding.UTF8.GetString(buff);
					}
					else
					{
						// we didn't get a length, so read the response in chunks and dynamically build the string
						// the easy way, we know how much we need to read
						byte[] buff = new byte[buffSize];

						StringBuilder sb = new StringBuilder(stringBuilderSize);
						int bytesRead = 1; // trick it first time through
						while (bytesRead > 0)
						{
							bytesRead = res.GetResponseStream().Read(buff, 0, buffSize);
							if (bytesRead > 0)
							{
								sb.Append(Encoding.UTF8.GetString(buff, 0, bytesRead));
							}
						}
						str = sb.ToString();
					}
					jDoc = new JSONParser.JSONDoc(str);
				}
				res.Close();

				return jDoc;
			}
			catch (WebException ex)
			{
				if ((ex.Response != null) && (ex.Response.GetResponseStream() != null))
				{
					if (ex.Response.ContentLength > 0)
					{
						// the easy way, we know how much we need to read
						byte[] buff;
						buff = new byte[ex.Response.ContentLength];
						int cnt = 0;
						while (cnt < (int)ex.Response.ContentLength)
						{
							int bytesLeft = (int)ex.Response.ContentLength - cnt;
							cnt += ex.Response.GetResponseStream().Read(buff, cnt, bytesLeft);
						}

						string str = Encoding.UTF8.GetString(buff);

						jDoc = new JSONParser.JSONDoc(str);

						throw new SwarmException(jDoc.ToString(), ex);
					}
					else
					{
						TextReader str = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8);
						jDoc = new JSONParser.JSONDoc(str);

						throw new SwarmException(jDoc.ToString(), ex);
					}
				}
				logger.Trace("Exception in SwarmServer.HttpPost: {0}", ex.Message);
				logger.Trace(ex.StackTrace);

				throw new SwarmException(ex.Message, ex);
			}
			catch (Exception ex)
			{
				logger.Trace("Exception in SwarmServer.HttpPost: {0}", ex.Message);
				logger.Trace(ex.StackTrace);

				throw;
			}
		}

		public JSONDoc HttpPost(string RequestUrl, string RequestContent)
		{
			JSONDoc jDoc = null;
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(RequestUrl);

			SetBasicAuthHeader(req, credentials);

			req.ContentType = "application/x-www-form-urlencoded";
			req.Method = "POST";
			try
			{
				byte[] bytedata = Encoding.UTF8.GetBytes(RequestContent);
                req.ContentLength = bytedata.Length;
				req.GetRequestStream().Write(bytedata, 0, bytedata.Length);

				WebResponse res = req.GetResponse();

				if (req.HaveResponse && (res != null) && (res.GetResponseStream() != null))
				{
					if (res.ContentLength > 0)
					{
						// the easy way, we know how much we need to read
						byte[] buff;
						buff = new byte[res.ContentLength];
						int cnt = 0;
						while (cnt < (int)res.ContentLength)
						{
							int bytesLeft = (int)res.ContentLength - cnt;
							cnt += res.GetResponseStream().Read(buff, cnt, bytesLeft);
						}
						string str = Encoding.UTF8.GetString(buff);
						jDoc = new JSONParser.JSONDoc(str);
					}
					else
					{
						TextReader str = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
						jDoc = new JSONParser.JSONDoc(str);
					}
				}
				res.Close();

				return jDoc;
			}
			catch (WebException ex)
			{
				if ((ex.Response != null) && (ex.Response.GetResponseStream() != null))
				{
					if (ex.Response.ContentLength > 0)
					{
						// the easy way, we know how much we need to read
						byte[] buff;
						buff = new byte[ex.Response.ContentLength];
						int cnt = 0;
						while (cnt < (int)ex.Response.ContentLength)
						{
							int bytesLeft = (int)ex.Response.ContentLength - cnt;
							cnt += ex.Response.GetResponseStream().Read(buff, cnt, bytesLeft);
						}
						string str = Encoding.UTF8.GetString(buff);
						jDoc = new JSONParser.JSONDoc(str);
						throw new SwarmException(jDoc.ToString(), ex);
					}
					else
					{
						TextReader str = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8);
						jDoc = new JSONParser.JSONDoc(str);
						throw new SwarmException(jDoc.ToString());
					}
				}
				logger.Trace("Exception in SwarmServer.HttpPost: {0}", ex.Message);
				logger.Trace(ex.StackTrace);

				throw new SwarmException(ex.Message);
			}
			catch (Exception ex)
			{
				logger.Trace("Exception in SwarmServer.HttpPost: {0}", ex.Message);
				logger.Trace(ex.StackTrace);

				throw;
			}
		}

		public class Version
		{
			public Version(JSONDoc o)
			{
				if (o == null) { return; }
				version = o["version"];
				year = o["year"];
				api = o["api"];
				apiVersions = null;
				if (o.ContainsKey("apiVersions"))
				{
					JSONArray a = o["apiVersions"] as JSONArray;
					if (a != null)
					{
						apiVersions = new List<string>();
						foreach (JSONField f in a.Value)
						{
							apiVersions.Add(f.ToString());
						}
					}
				}
				if (apiVersions == null)
				{
					apiVersions = new List<string>();
					apiVersions.Add("1");
				}

			}
			public string version { get; set; }
			public string year { get; set; }
			public string api { get; set; }
			public List<string> apiVersions { get; set; }
		}

		//private string VersionRequest = "http://{0}/api/v1/version";
		private string VersionRequest = "{0}/api/version";
		/// <summary>
		/// The version information from the Swarm server
		/// </summary>
		public Version GetVersion
		{
			get
			{
				Version val = null;

				string RequestUrl = string.Format(VersionRequest, SwarmUri);
				JSONParser.JSONDoc jDoc=null;
				try
				{
					jDoc = HttpGet(RequestUrl);
				}
				catch (Exception ex)
				{
					P4VsOutputWindow.AppendMessage(ex.Message);
					FileLogger.LogMessage(3, "SwarmAPI", ex.Message);
					return val;
				}
				val = new Version(jDoc);

				return val;
			}
		}
			/// <summary>
			/// 
		/// <summary>
		/// 
		/// </summary>
		public class Review
		{
			/// </summary>
			public class Version
			{
				public int change { get; set; }
				public string user { get; set; }
				public int time { get; set; }
				public bool pending { get; set; }
				public Version()
				{
				}

				#region use_JsonReaderWriterFactory
#if use_JsonReaderWriterFactory
				public Version(XmlNode node)
				{
					int i;
					XmlNode e = node["change"];
					if (e != null)
					{
						i = -1;
						int.TryParse(e.InnerText, out i);
						change = i;
					}
					e = node["user"];
					if (e != null)
					{
						user = e.InnerText;
					}
					e = node["time"];
					if (e != null)
					{
						i = -1;
						int.TryParse(e.InnerText, out i);
						time = i;
					}
					e = node["pending"];
					if (e != null)
					{
						pending = e.InnerText.StartsWith("true");
					}
				}
#endif
				#endregion
				public Version(JSONObject o)
				{
					if (o == null) { return; }
					change = o["change"];
					user = o["user"];
					time = o["time"];
					pending = o["pending"];
				}
				public static explicit operator Version(JSONField it)
				{
					JSONObject o = it as JSONObject;
					if (o == null) { return null; }

					Version v = new Version();
					v.change = o["change"];
					v.user = o["user"];
					v.time = o["time"];
					v.pending = o["pending"];

					return v;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public class Participant
			{
				int vote;
				bool required;

#if use_JsonReaderWriterFactory
				public Participant(XmlNode node)
				{
				}
#endif
				public Participant(JSONObject o)
				{
					if (o == null) { return; }
					vote = o["vote"];
					required = o["required"];
				}
				public Participant(bool _required, int _vote)
				{
					vote = _vote;
					required = _required;
				}

				public static implicit operator JSONObject(Participant p)
				{
					JSONObject value = new JSONObject();
					value.Value["vote"] = new JSONNumericalField(p.vote);
					value.Value["required"] = new JSONBoolField( p.required);
					return value;
				}
			}
			public Review(JSONDoc jdoc)
				: this(jdoc as JSONObject)
			{
			}

			public Review(JSONObject jdoc)
			{
				if (jdoc == null)
				{
					throw new Exception("Null object passed to Review creator");
				}
				JSONObject ReviewObj = jdoc;
				if ((jdoc.ContainsKey("review")) && (jdoc["review"] is JSONObject))
				{
					ReviewObj = (JSONObject) jdoc["review"];
				}
				id = (int) ReviewObj["id"];
				type = ReviewObj["type"];
				changes = ReviewObj["changes"];
				commits = ReviewObj["commits"];

				List<JSONObject> a = ReviewObj["versions"];
				versions = new List<Review.Version>();
				if (a != null)
				{
					foreach (JSONObject n in a)
					{
						versions.Add(new Review.Version(n));
					}
				}
				else
				{
					versions = null;
				}
				author = ReviewObj["author"];
				participants = new Dictionary<string, Participant>();
				JSONObject participantsData = ReviewObj["participants"] as JSONObject;
				if (participantsData == null)
				{
					participants = null;
				}
				else
				{
					foreach (string key in participantsData.Value.Keys)
					{
						JSONObject participiantData = participantsData[key] as JSONObject;
						if (participiantData != null)
						{
							participants.Add(key, new Participant(participiantData));
						}
						else
						{
							participants.Add(key, null);
						}
					}
				}

//				hasReviewer = ReviewObj["hasReviewer"];
				description = ReviewObj["description"];
				created = ReviewObj["created"];
				updated = ReviewObj["updated"];

				projects = null;// jDoc["projects"];

				state = ReviewObj["state"];
				stateLabel = ReviewObj["stateLabel"];
				testStatus = ReviewObj["testStatus"];

				testDetails = null; // jDoc["testDetails"];

				deployStatus = ReviewObj["deployStatus"];

				deployDetails = null; // jDoc["deployDetails"];

				pending = ReviewObj["pending"];

				commitStatus = null; // jDoc["commitStatus"];
			}

			#region use_JsonReaderWriterFactory
#if use_JsonReaderWriterFactory
			public Review(XmlDocument xDoc)
			{
				int i = -1;

				// Walk the XML to create the Review Object
				XmlNode ReviewNode = xDoc.DocumentElement.ChildNodes.Item(0);
				foreach (XmlNode cNode in ReviewNode.ChildNodes)
				{
					switch (cNode.Name)
					{
						case "id":
							i = -1;
							int.TryParse(cNode.InnerText, out i);
							id = i;
							break;

						case "type":
							type = cNode.InnerText;
							break;

						case "changes":
							if (cNode.HasChildNodes == false)
							{
								changes = null;
							}
							else
							{
								changes = new List<int>();
								foreach (XmlNode n in cNode.ChildNodes)
								{
									i = -1;
									int.TryParse(n.InnerText, out i);
									changes.Add(i);
								}
							}
							break;

						case "commits":
							if (cNode.HasChildNodes == false)
							{
								commits = null;
							}
							else
							{
								commits = new List<int>();
								foreach (XmlNode n in cNode.ChildNodes)
								{
									i = -1;
									int.TryParse(n.InnerText, out i);
									commits.Add(i);
								}
							}
							break;

						case "versions":
							if (cNode.HasChildNodes == false)
							{
								versions = null;
							}
							else
							{
								versions = new List<Review.Version>();
								foreach (XmlNode n in cNode.ChildNodes)
								{
									versions.Add(new Review.Version(n));
								}
							}
							break;

						case "author":
							author = cNode.InnerText;
							break;

						case "participants":
							if (cNode.HasChildNodes == false)
							{
								participants = null;
							}
							else
							{
								participants = new List<string>();
								foreach (XmlNode n in cNode.ChildNodes)
								{
									participants.Add(n.InnerText);
								}
							}
							break;
/* Old structure
						case "participantsData":
							if (cNode.HasChildNodes == false)
							{
								participantsData = null;
							}
							else
							{
								participantsData = new Dictionary<string, Review.Participant>();
								foreach (XmlNode n in cNode.ChildNodes)
								{
									participantsData[n.Name] = new Review.Participant(n);
								}
							}
							break;

						case "hasReviewer":
							i = -1;
							int.TryParse(cNode.InnerText, out i);
							hasReviewer = i;
							break;
*/
						case "description":
							description = cNode.InnerText;
							break;

						case "created":
							i = -1;
							int.TryParse(cNode.InnerText, out i);
							created = i;
							break;

						case "updated":
							i = -1;
							int.TryParse(cNode.InnerText, out i);
							updated = i;
							break;

						case "projects":
							if (cNode.HasChildNodes == false)
							{
								projects = null;
							}
							else
							{
								projects = new List<object>();
								foreach (XmlNode n in cNode.ChildNodes)
								{
									projects.Add(n.InnerXml);
								}
							}
							break;

						case "state":
							state = cNode.InnerText;
							break;

						case "stateLabel":
							stateLabel = cNode.InnerText;
							break;

						case "testStatus":
							testStatus = cNode.InnerXml;
							break;

						case "testDetails":
							if (cNode.HasChildNodes == false)
							{
								testDetails = null;
							}
							else
							{
								testDetails = new List<object>();
								foreach (XmlNode n in cNode.ChildNodes)
								{
									testDetails.Add(n.InnerXml);
								}
							}
							break;

						case "deployStatus":
							deployStatus = cNode.InnerXml;
							break;

						case "deployDetails":
							if (cNode.HasChildNodes == false)
							{
								deployDetails = null;
							}
							else
							{
								deployDetails = new List<object>();
								foreach (XmlNode n in cNode.ChildNodes)
								{
									deployDetails.Add(n.InnerXml);
								}
							}
							break;

						case "pending":
							pending = cNode.InnerText.StartsWith("true");
							break;

						case "commitStatus":
							if (cNode.HasChildNodes == false)
							{
								commitStatus = null;
							}
							else
							{
								commitStatus = new List<object>();
								foreach (XmlNode n in cNode.ChildNodes)
								{
									commitStatus.Add(n.InnerXml);
								}
							}
							break;

						case "Fibar":
							break;

						default:
							break;
					}
				}
			}
#endif
			#endregion
			public int id { get; set; }
			public string author { get; set; }
			public List<int> changes { get; set; }
			public List<int> commits { get; set; }
			public List<object> commitStatus { get; set; }
			public int created { get; set; }
			public List<object> deployDetails { get; set; }
			public object deployStatus { get; set; }
			public string description { get; set; }
			public IDictionary<string, Review.Participant> participants { get; set; }
			public bool pending { get; set; }
			public List<object> projects { get; set; }
			public string state { get; set; }
			public string stateLabel { get; set; }
			public List<object> testDetails { get; set; }
			public object testStatus { get; set; }
			public string type { get; set; }
			public int updated { get; set; }
			public List<Review.Version> versions { get; set; }

			public override string ToString()
			{
				 return ToString(0);
			}

			public string ToString(int indentLevel)
			{
				StringBuilder val = new StringBuilder(1024);
				string tabs = string.Empty;
				for (int idx = 0; idx < indentLevel; idx++)
				{
					tabs += '\t';
				}
				val.AppendFormat("{0}Review: {1}\r\n",tabs, id);
				val.AppendFormat("\t{0}Author: {1}\r\n",tabs, author);
				val.AppendFormat("\t{0}Changes:",tabs);

				bool first = true;
				foreach (int c in changes)
				{
					if (first)
						val.AppendFormat(" {0}",c);
					else
						val.AppendFormat(", {0}",c);
					first = false;
				}
				val.AppendFormat("\r\n");

				val.AppendFormat("\t{0}Commits:", tabs);
				first = true;
				foreach (int c in commits)
				{
					if (first)
						val.AppendFormat(" {0}",c);
					else
						val.AppendFormat(", {0}",c);
					first = false;
				}
				val.AppendFormat("\r\n");

				val.AppendFormat("\t{0}commitStatus:\r\n", tabs);
				foreach (object c in commitStatus)
				{
					val.AppendFormat("/t{0}{1}\r\n",tabs,c);
				}

				val.AppendFormat("\t{0}Created: {1}", tabs, created);

				val.AppendFormat("\t{0}DeployDetails:\r\n", tabs);
				foreach (object c in deployDetails)
				{
					val.AppendFormat("/t{0}{1}\r\n",tabs,c);
				}

				val.AppendFormat("/t{0}DeployStatus{1}\r\n", tabs, deployStatus);
				val.AppendFormat("/t{0}DeployStatus{1}\r\n", tabs, deployStatus);

/*
				public string description { get; set; }
				public IDictionary<string, Review.Participant> participants { get; set; }
				public bool pending { get; set; }
				public List<object> projects { get; set; }
				public string state { get; set; }
				public string stateLabel { get; set; }
				public List<object> testDetails { get; set; }
				public object testStatus { get; set; }
				public string type { get; set; }
				public int updated { get; set; }
				public List<Review.Version> versions { get; set; }
*/
				return val.ToString();
			}  

//			public List<string> participants { get; set; }
//			public Dictionary<string, Participant> participantsData { get; set; }
//			public int hasReviewer { get; set; }
		}

		//private string ReviewDetailsRequest = "http://{0}/api/v1/reviews/{1}";
		private string ReviewDetailsRequest = "{0}/api/v1/reviews/{1}";

		public Review GetReview(int reviewId)
		{
			Review val = null;

			string RequestUrl = string.Format(ReviewDetailsRequest, SwarmUri, reviewId);
#if use_DataContractJsonSerializer
				//MessageBox.Show(swarmUri + "\r\n\r\n" + ReqData);

				HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(RequestUrl);
				req.ContentType = "application/x-www-form-urlencoded";
				req.Credentials = credentials;
				req.Method = "GET";

				WebResponse res = req.GetResponse();
				if (req.HaveResponse && (res.GetResponseStream() != null))
				{
					/* 
					 * Method 1, use a DataContractJsonSerializer to convert the JSON
					 * to a c# object
					 * 
					 * Advantages: Class Library, and easy to code
					 * Downsides: The C# objects are somewhat simplistic and rigidly tied to the JSON
					 */

			try
			{
					DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Review));
					//while ((val = (SwarmVersion)jsonSerializer.ReadObject(res.GetResponseStream())) != null)
					//{
					//    ;
					//}
					val = (Review)jsonSerializer.ReadObject(res.GetResponseStream());
					if (val == null)
					{
						res.GetResponseStream().Seek(0, System.IO.SeekOrigin.Begin);
						byte[] buff = new byte[res.ContentLength];
						int cnt = 0;
						while (cnt < (int)res.ContentLength)
						{
							int bytesLeft = (int)res.ContentLength - cnt;
							cnt += res.GetResponseStream().Read(buff, cnt, bytesLeft);
						}
						//res.GetResponseStream().Position = 0;

						string s = Encoding.UTF8.GetString(buff);
						throw new SwarmException(s);
					}
#endif

			/* 
					 * Method 2, use a JsonReaderWriterFactory to convert the JSON to an XML Doc
					 * to a c# object
					 * 
					 * Advantages: Class Library, XML well supported in C#
					 * Downsides: Still have to walk the tree to convert to a C# object.
					 */

#if use_JsonReaderWriterFactory
					XmlDictionaryReader jrw = JsonReaderWriterFactory.CreateJsonReader(res.GetResponseStream(), XmlDictionaryReaderQuotas.Max);
					XmlDocument xDoc = new XmlDocument();
					xDoc.Load(jrw);

					string xmlStr = xDoc.OuterXml;

					val = new Review(xDoc);
#endif
			/* 
					 * Method 2, use JSONParse to convert the JSON to an easy to interpret JSONdoc
					 * to a c# object
					 * 
					 * Advantages: Easy to convert JSon to c#
					 * Downsides: Internal library
					 */

#if true //use_JsonParser
#if debug_dump

					//res.GetResponseStream().Seek(0, System.IO.SeekOrigin.Begin);
					byte[] buff = new byte[res.ContentLength];
					int cnt = 0;
					while (cnt < (int)res.ContentLength)
					{
						int bytesLeft = (int)res.ContentLength - cnt;
						cnt += res.GetResponseStream().Read(buff, cnt, bytesLeft);
					}
					//res.GetResponseStream().Position = 0;

					string s = Encoding.UTF8.GetString(buff);
//					throw new SwarmException(s);

					res = req.GetResponse();

//					res.GetResponseStream().Seek(0, System.IO.SeekOrigin.Begin);
			}
			//catch (SwarmException)
			//{
			//    throw;
			//}
			catch (Exception ex)
			{
				logger.Trace("Exception in SwarmServer.Version: {0}", ex.Message));
				logger.Trace(ex.StackTrace);
			}
				}

#endif
			JSONParser.JSONDoc jDoc = HttpGet(RequestUrl);
			val = new Review(jDoc);
#endif
			return val;
		}

		public class ReviewList: List<Review>
		{
			public ReviewList() { }
			public ReviewList(JSONDoc jdoc)
			{
				lastSeen = jdoc["lastSeen"];
				totalCount = jdoc["totalCount"];

				JSONArray reviews = jdoc["reviews"] as JSONArray;
                if (reviews != null)
                {
                    foreach (JSONObject review in reviews.Value)
                    {
                        this.Add(new Review(review));
                    }
                }
			}
			public int lastSeen { get; set; }
			public object totalCount { get; set; }
		}

		//private string ReviewSearchRequest = "http://{0}/api/v1/reviews/?{1}";
		private string ReviewSearchRequest = "{0}/api/v1/reviews/?{1}";

		public ReviewList GetReviews()
		{
			ReviewList val = null;

			string RequestUrl = string.Format(ReviewSearchRequest, SwarmUri, string.Empty);
			JSONParser.JSONDoc jDoc = HttpGet(RequestUrl);
			val = new ReviewList(jDoc);

			return val;
		}

		public ReviewList GetReviews(Options options)
		{
			ReviewList val = null;

			string RequestUrl = string.Format(ReviewSearchRequest, SwarmUri, options.ToString());
			JSONParser.JSONDoc jDoc = HttpGet(RequestUrl);
			val = new ReviewList(jDoc);

			return val;
		}

		//private string AddChangeRequest = "http://{0}/api/v1/reviews/{1}";
		private string AddChangeRequest = "{0}/api/v1/reviews/{1}/changes";
		private string AddChangeRequest1_1 = "{0}/api/v1.1/reviews/{1}/changes";
		private string AddChangeContent = "change={0}";

		public Review AddChangeToReview(int ReviewId, int ChangeId)
		{
			string RequestUrl = string.Format(AddChangeRequest, SwarmUri, ReviewId);
			string RequestContent = string.Format(AddChangeContent, ChangeId);

			JSONParser.JSONDoc jDoc = HttpPost(RequestUrl, RequestContent);

			return new Review(jDoc);
		}

		public Review AddChangeToReview1_1(int ReviewId, int ChangeId)
		{
			string RequestUrl = string.Format(AddChangeRequest1_1, SwarmUri, ReviewId);
			string RequestContent = string.Format(AddChangeContent, ChangeId);

			JSONParser.JSONDoc jDoc = HttpPost(RequestUrl, RequestContent);

			return new Review(jDoc);
		}
		//private string CreateReviewRequest = "http://{0}/api/v1/reviews/";
		private string CreateReviewRequest = "{0}/api/v1/reviews/";
		private string CreateReviewRequest1_1 = "{0}/api/v1.1/reviews/";
        private string CreateReviewRequest7 = "{0}/api/v7/reviews/";
        private string CreateReviewContent = "change={0}";

		public Review CreateReview(int ChangeId, Options options)
		{
			string RequestUrl = string.Format(CreateReviewRequest, SwarmUri);

			string RequestContent = string.Format(CreateReviewContent, ChangeId);
			if ((options != null) && (options.Count > 0))
			{
				RequestContent += "&" + options.ToString();
            }
			JSONParser.JSONDoc jDoc = HttpPost(RequestUrl, RequestContent);

			return new Review(jDoc);
		}

		public Review CreateReview1_1(int ChangeId, Options options)
		{
			string RequestUrl = string.Format(CreateReviewRequest1_1, SwarmUri);

			string RequestContent = string.Format(CreateReviewContent, ChangeId);
			if ((options != null) && (options.Count > 0))
			{
				RequestContent += "&" + options.ToString();
			}
			JSONParser.JSONDoc jDoc = HttpPost(RequestUrl, RequestContent);

			return new Review(jDoc);
		}

        public Review CreateReview7(int ChangeId, Options options)
        {
            string RequestUrl = string.Format(CreateReviewRequest7, SwarmUri);

            string RequestContent = string.Format(CreateReviewContent, ChangeId);
            if ((options != null) && (options.Count > 0))
            {
                RequestContent += "&" + options.ToString();
            }
            JSONParser.JSONDoc jDoc = HttpPost(RequestUrl, RequestContent);

            return new Review(jDoc);
        }
    }
}
