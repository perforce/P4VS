using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Perforce.JSONParser
{
    public abstract class JSONField
    {
		//public JSONField(string name) { Name = name; }
		//public string Name { get; private set; }

		public virtual string ToParam(string paramName)
		{
			return string.Format("{0}={1}", paramName, this.ToString());
		}

		public static implicit operator double(JSONField it)
		{
			if (it == null) { return 0; }
			if (it is JSONNumericalField)
			{
				return ((JSONNumericalField)it).Value;
			}
			return 0;
		}
		public static implicit operator int(JSONField it)
		{
			if (it == null) { return 0; }
			if (it is JSONNumericalField)
			{
				return (int)((JSONNumericalField)it).Value;
			}
			return 0;
		}
		public static implicit operator string(JSONField it)
		{
			if (it == null) { return null; }
			if (it is JSONStringField)
			{
				return ((JSONStringField)it).Value;
			}
			return null;
		}
		public static implicit operator bool(JSONField it)
		{
			if (it == null) { return false; }
			if (it is JSONBoolField)
			{
				return ((JSONBoolField)it).Value;
			}
			return false;
		}
		public static implicit operator List<int>(JSONField it)
		{
			if ((it is JSONArray) == false)
			{
				return null;
			}
			JSONArray a = it as JSONArray;
			if (a.Value.Count <= 0)
			{
				return null;
			}
			else
			{
				List<int> values = new List<int>();
				foreach (JSONField n in a.Value)
				{
					if (n is JSONNumericalField)
					{
						values.Add(n as JSONNumericalField);
					}
				}
				return values;
			}
		}

		public static implicit operator List<double>(JSONField it)
		{
			if ((it is JSONArray) == false)
			{
				return null;
			}
			JSONArray a = it as JSONArray;
			if (a.Value.Count <= 0)
			{
				return null;
			}
			else
			{
				List<double> values = new List<double>();
				foreach (JSONField n in a.Value)
				{
					if (n is JSONNumericalField)
					{
						values.Add(n as JSONNumericalField);
					}
				}
				return values;
			}
		}

		public static implicit operator List<string>(JSONField it)
		{
			if ((it is JSONArray) == false)
			{
				return null;
			}
			JSONArray a = it as JSONArray;
			if (a.Value.Count <= 0)
			{
				return null;
			}
			else
			{
				List<string> values = new List<string>();
				foreach (JSONField n in a.Value)
				{
					if (n is JSONStringField)
					{
						values.Add(n as JSONStringField);
					}
				}
				return values;
			}
		}

        public static implicit operator List<JSONObject>(JSONField it)
        {
            if (it is JSONArray)
            {
                JSONArray a = it as JSONArray;
                if (a.Value.Count <= 0)
                {
                    return null;
                }
                else
                {
                    List<JSONObject> values = new List<JSONObject>();
                    foreach (JSONField n in a.Value)
                    {
                        values.Add(n as JSONObject);
                    }
                    return values;
                }
            }
            return null;
        }

        //public delegate object convertJsonObject(JSONField);
        //public static implicit operator Dictionary<string, object>(JSONField it)
        //{
        //    if ((it is JSONObject) == false)
        //    {
        //        JSONObject o = it as JSONObject;
        //        if (o.Value.Count <= 0)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            Dictionary<string, JSONObject> values = new Dictionary<string, JSONObject>();
        //            foreach (string key in o.Value.Keys)
        //            {
        //                values[key] = o.Value[key];
        //            }
        //            return values;
        //        }
        //    }
        //    return null;
        //}
    }

    public class JSONNumericalField : JSONField
    {
		public JSONNumericalField(double value) { IsInt = false; Value = value; }
		public JSONNumericalField(int value) { IsInt = true; Value = value; }

		public double Value { get; private set; }
		public bool IsInt {get; private set;}

		public override string ToString() 
		{
			if (IsInt)
			{
				return ((int)Value).ToString();
			}
			return Value.ToString(); 
		}
	}

    public class JSONStringField : JSONField
    {
        public JSONStringField(string value) {Value = value;}
		public string Value { get; private set; }
		public override string ToString()
		{
			return Value;
		}
		public override string ToParam(string paramName)
		{
			return string.Format("{0}={1}", paramName, Uri.EscapeDataString(this.Value));
		}
	}

	public class JSONBoolField : JSONField
	{
		public JSONBoolField(bool value) { Value = value; }
		public bool Value { get; private set; }

		public override string ToString() { return Value?"true":"false"; }
	}

	public class JSONNullField : JSONField
	{
		public JSONNullField() {}

		public override string ToString() { return "null"; }
	}

	public class JSONObject : JSONField
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public JSONObject() { Value = new Dictionary<string, JSONField>(); }
		public JSONObject(string jsonStr) 
		{
			Value = new Dictionary<string, JSONField>(); 
			IList<JSONToken> tokens = JSONDoc.Tokenize(jsonStr);
			this.Parse(tokens); 
		}
		public IDictionary<string, JSONField> Value { get; private set; }
		public void Add(string name, JSONField newField) { Value[name] = newField; }

		public JSONField this[string name] 
		{
			get
			{
				if (this.Value.ContainsKey(name))
				{
					return this.Value[name];
				}
				logger.Trace("Could not find key in dictionary: {0}", name);
				return null;
			}
		}

        public bool ContainsKey(string name)
        {
            return this.Value.ContainsKey(name);
        }

		internal void Parse(IList<JSONToken> tokens)
		{
			int idx = 0;
			Parse(tokens, ref idx);
		}

		internal void Parse(IList<JSONToken> tokens, ref int idx)
		{
			if ((tokens[idx] is StartBlock) == false)
			{
				throw new Exception("JSON object must start with a '{'");
			}
			idx++;
			while (idx < tokens.Count)
			{
				JSONToken t1 = tokens[idx];
				if (t1 is EndBlock)
				{
					// end of (maybe empty) object
					idx++;
					break;
				}
				if (idx+3 >= tokens.Count)
				{
					// need at least 4 tokens for a field
					// <fieldname><:><value><,|}>
					throw new Exception("Invalid Json, looking for fieldname:");
				}
				JSONToken t2 = tokens[idx+1];
				JSONToken t3 = tokens[idx+2];
				JSONToken t4 = tokens[idx+3];
				if (t1 is StartArray)
				{
					// object is an array
					JSONArray arr = new JSONArray();
					arr.Parse(tokens, ref idx);
					Add("root", arr);
					t4 = tokens[idx];

					if ((t4 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t4 is EndBlock)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or '}'");
					}					

				}
				if (((t1 is StringLiteral) == false) || ((t2 is FieldNameToken) == false))
				{
					throw new Exception("Invalid Json, looking for fieldname:");
				}
				StringLiteral fieldName = t1 as StringLiteral;
				if (t3 is StringLiteral)
				{
					Add(fieldName.Value, new JSONStringField(((StringLiteral)t3).Value));
					idx+=3; //move to the next token
					if ((t4 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t4 is EndBlock)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or '}'");
					}					
				}
				else if (t3 is DoubleLiteral)
				{
					Add(fieldName.Value, new JSONNumericalField(((DoubleLiteral)t3).Value));
					idx += 3; //move to the next token
					if ((t4 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t4 is EndBlock)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or '}'");
					}
				}
				else if (t3 is IntLiteral)
				{
					Add(fieldName.Value, new JSONNumericalField(((IntLiteral)t3).Value));
					idx += 3; //move to the next token
					if ((t4 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t4 is EndBlock)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or '}'");
					}
				}
				else if (t3 is BoolLiteral)
				{
					Add(fieldName.Value, new JSONBoolField(((BoolLiteral)t3).Value));
					idx+=3; //move to the next token
					if ((t4 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t4 is EndBlock)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or '}'");
					}					
				}
				else if (t3 is NullLiteral)
				{
					Add(fieldName.Value, new JSONNullField());
					idx+=3; //move to the next token
					if ((t4 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t4 is EndBlock)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or '}'");
					}					
				}
				else if (t3 is StartBlock)
				{
					if (t4 is EndBlock)
					{
						// null object
						Add(fieldName.Value, new JSONNullField());
						idx += 4;

						t4 = tokens[idx];
						if ((t4 is FieldSeperatorToken))
						{
							idx++; //move to the next field
						}
					}
					else
					{
						// field value is an object
						JSONObject obj = new JSONObject();
						idx += 2; //skip over field name and ':'
						obj.Parse(tokens, ref idx);
						Add(fieldName.Value, obj);

						t4 = tokens[idx];
						if ((t4 is FieldSeperatorToken))
						{
							idx++; //move to the next field
						}
						else if (t4 is EndBlock)
						{
							// end of object
							idx++;
							break;
						}
						else
						{
							throw new Exception("Invalid Json, looking for ',' or '}'");
						}
					}			
				}
				else if (t3 is StartArray)
				{
					if (t4 is EndArray)
					{
						// null object
						Add(fieldName.Value, new JSONNullField());
						idx += 4;

						t4 = tokens[idx];
						if ((t4 is FieldSeperatorToken))
						{
							idx++; //move to the next field
						}
					}
					else
					{
						// field value is an object
						JSONArray arr = new JSONArray();
						idx += 2; //skip over field name and ':'
						arr.Parse(tokens, ref idx);
						Add(fieldName.Value, arr);
						t4 = tokens[idx];

						if ((t4 is FieldSeperatorToken))
						{
							idx++; //move to the next field
						}
						else if (t4 is EndBlock)
						{
							// end of object
							idx++;
							break;
						}
						else
						{
							throw new Exception("Invalid Json, looking for ',' or '}'");
						}
					}				
				}
				else
				{
					throw new Exception("Unexpected tokens");
				}
			}
		}
		public override string ToString()
		{
			StringBuilder val = new StringBuilder("{", 1024);
			bool first = true;
			foreach (string key in Value.Keys)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					val.Append(',');
				}
				if (Value[key] is JSONStringField)
				{
					// string fields need to be quoted
					val.AppendFormat("{0}:\"{1}\"", key, Value[key]);
				}
				else
				{
					val.AppendFormat("{0}:{1}", key, Value[key]);
				}
			}
			val.Append('}');

			return val.ToString();
		}
	}

	public class JSONArray : JSONField
	{
		public JSONArray() { Value = new List<JSONField>(); }

		public JSONArray(string jsonStr) 
		{
			Value = new List<JSONField>(); 
			IList<JSONToken> tokens = JSONDoc.Tokenize(jsonStr);
			this.Parse(tokens); 
		}

		public JSONArray(IList<string> values)
		{
			Value = new List<JSONField>();

			foreach (String v in values)
			{
				Add(new JSONStringField(v));
			}
		}

		public JSONArray(string[] values)
		{
			Value = new List<JSONField>();

			foreach (String v in values)
			{
				Add(new JSONStringField(v));
			}
		}

		public JSONArray(IList<int> values)
		{
			Value = new List<JSONField>();

			foreach (int v in values)
			{
				Add(new JSONNumericalField(v));
			}
		}

		public JSONArray(int[] values)
		{
			Value = new List<JSONField>();

			foreach (int v in values)
			{
				Add(new JSONNumericalField(v));
			}
		}

		public JSONArray(IList<double> values)
		{
			Value = new List<JSONField>();

			foreach (double v in values)
			{
				Add(new JSONNumericalField(v));
			}
		}

		public JSONArray(double[] values)
		{
			Value = new List<JSONField>();

			foreach (double v in values)
			{
				Add(new JSONNumericalField(v));
			}
		}

		public List<JSONField> Value { get; private set; }
		public void Add(JSONField newField) { Value.Add(newField); }
		public JSONField this[int idx] { get { return this.Value[idx]; } }

		internal void Parse(IList<JSONToken> tokens)
		{
			int idx = 0;
			this.Parse(tokens, ref idx);
		}

		internal void Parse(IList<JSONToken> tokens, ref int idx)
		{
			if ((tokens[idx] is StartArray) == false)
			{
				throw new Exception("JSON array must start with a '['");
			}
			idx++;
			while (idx < tokens.Count)
			{
				JSONToken t1 = tokens[idx];
				if (t1 is EndArray)
				{
					// end of (maybe empty) array
					idx++;
					break;
				}
				if (idx+1 >= tokens.Count)
				{
					// need at least 2 tokens for an array item 
					// <value><,|]>
					throw new Exception("Invalid Json, looking for fieldname:");
				}
				JSONToken t2 = tokens[idx+1];
				if (t1 is StringLiteral)
				{
					Add(new JSONStringField(((StringLiteral)t1).Value));
					idx++; //move to the next token
					if ((t2 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t2 is EndArray)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or ']'");
					}					
				}
				else if (t1 is DoubleLiteral)
				{
					Add(new JSONNumericalField(((DoubleLiteral)t1).Value));
					idx++; //move to the next token
					if ((t2 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t2 is EndArray)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or ']'");
					}					
				}
				else if (t1 is IntLiteral)
				{
					Add(new JSONNumericalField(((IntLiteral)t1).Value));
					idx++; //move to the next token
					if ((t2 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t2 is EndArray)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or ']'");
					}					
				}
				else if (t1 is BoolLiteral)
				{
					Add(new JSONBoolField(((BoolLiteral)t1).Value));
					idx++; //move to the next token
					if ((t2 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t2 is EndArray)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or ']'");
					}					
				}
				else if (t1 is NullLiteral)
				{
					Add(new JSONNullField());
					idx++; //move to the next token
					if ((t2 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t2 is EndArray)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or ']'");
					}					
				}
				else if (t1 is StartBlock)
				{
					// field value is an object
					JSONObject obj = new JSONObject();
					obj.Parse(tokens, ref idx);
					Add(obj);
					
					t2 = tokens[idx];
					if ((t2 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t2 is EndArray)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or ']'");
					}					
				}
				else if (t1 is StartArray)
				{
					// field value is an object
					JSONArray arr = new JSONArray();
					arr.Parse(tokens, ref idx);
					Add(arr);
					t2 = tokens[idx];

					if ((t2 is FieldSeperatorToken))
					{
						idx++; //move to the next field
					}
					else if (t2 is EndArray)
					{
						// end of object
						idx++;
						break;
					}
					else
					{
						throw new Exception("Invalid Json, looking for ',' or ']'");
					}					
				}
				else
				{
					throw new Exception("Unexpected tokens");
				}
			}
		}
		public override string ToString()
		{
			StringBuilder val = new StringBuilder("[", 1024);
			bool first = true;
			foreach (JSONField entry in Value)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					val.Append(',');
				}
				val.Append(entry.ToString());
			}
			val.Append(']');

			return val.ToString();
		}
		public override string ToParam(string paramName)
		{
			StringBuilder val = new StringBuilder(1024);
			bool first = true;
			string pName = paramName;
			if ((Value.Count > 1) && (pName.EndsWith("[]") == false))
			{
				// multiple values and doesn't have the '[]' to indicate array
				pName += "[]";
			}
			foreach (JSONField entry in Value)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					val.Append('&');
				}
				val.AppendFormat(entry.ToParam(pName));
			}

			return val.ToString();
		}

        public static implicit operator JSONArray(List<string> l) { return new JSONArray(l); }
	}
}
