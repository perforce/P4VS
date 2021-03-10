using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Perforce.JSONParser;

namespace Perforce.SwarmApi
{
	public class Options : Dictionary<string, JSONField>
	{
		public override string ToString()
		{
			StringBuilder val = new StringBuilder(1024);

			bool first = true;
			foreach (string key in Keys)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					val.Append('&');
				}
				val.Append(this[key].ToParam(key));
			}
			return val.ToString();
		}
	}
}
