using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perforce.JSONParser
{
    /// <summary>
    /// 
    /// </summary>
	public class JSONDoc : JSONObject
    {
        public static bool ThrowOnError { get; set; }

		public JSONDoc(string jsonStr)
		{
			LoadJSONDoc(jsonStr);
		}

		public JSONDoc(TextReader jsonStr)
		{
			LoadJSONDoc(jsonStr);
		}

		public JSONDoc()
		{
		}

        public bool LoadJSONDoc(string jsonStr)
        {
            try
            {
				this.Value.Clear();

                IList<JSONToken> tokens = JSONDoc.Tokenize(jsonStr);

				if ((tokens[0] is StartBlock) == false)
				{
					throw new Exception("JSON document must start with a '{'");
				}
				if ((tokens[tokens.Count-1] is EndBlock) == false)
				{
					throw new Exception("JSON document must end with a '}'");
				}

				int idx = 0;
				this.Parse(tokens, ref idx);

                return true;
            }
			catch (Exception)
            {
                return false;
            }

        }

		public bool LoadJSONDoc(TextReader jsonStr)
        {
            try
            {
				this.Value.Clear();
#if DEBUG
				StreamContentBuf = new StringBuilder(4*4096);
#endif
				IList<JSONToken> tokens = JSONDoc.Tokenize(jsonStr);
#if DEBUG
				StreamContent = StreamContentBuf.ToString();
#endif
				if ((tokens[0] is StartBlock) == false)
				{
					throw new Exception("JSON document must start with a '{'");
				}
				if ((tokens[tokens.Count-1] is EndBlock) == false)
				{
					throw new Exception("JSON document must end with a '}'");
				}

				int idx = 0;
				this.Parse(tokens, ref idx);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

		private static bool IsJSONSpecialChar(char c)
		{
			return ((c == '{') || (c == '}') || (c == '[') || (c == ']') || (c == '"') ||
				(c == ':') || (c == ','));
		}

		private static StringLiteral ScanString(string jsonStr, ref int idx)
		{
			StringBuilder val = new StringBuilder(64);
			idx++; // skip the leading "
			while (idx < jsonStr.Length - 1) // -1 cause it has to end with a '"'
			{
				char c = jsonStr[idx];

				switch (c)
				{
					case '\"':
						// unescaped " must end the string
						idx++;
						return new StringLiteral(val);
					case '\\':
						// escaped character 
						char c2 = jsonStr[idx + 1];
						switch (c2)
						{
							case 'u':
								// unicode surrogate character, just copy all 6 chars into the literal as is 
								for (int idx2 = 0; idx2 < 6; idx2++)
								{
									val.Append(jsonStr[idx++]);
								}
								break;
							case '\"':
								val.Append('\"');
								idx += 2;
								break;
							case '\'':
								val.Append('\'');
								idx += 2;
								break;
							case '\\':
								val.Append('\\');
								idx += 2;
								break;
							case '/':
								val.Append('/');
								idx += 2;
								break;
							case 'n':
								val.Append('\n');
								idx += 2;
								break;
							case 'r':
								val.Append('\r');
								idx += 2;
								break;
							case 't':
								val.Append('\t');
								idx += 2;
								break;
							case 'b':
								val.Append('\b');
								idx += 2;
								break;
							case 'f':
								val.Append('\f');
								idx += 2;
								break;
							case 'v': // not valid in JSON string
								val.Append('\v');
								idx += 2;
								break;
							case '0': // not valid in JSON string
								val.Append('\0');
								idx += 2;
								break;
							default: // not valid escaped character
								throw new Exception("Bad escape sequence: \\" + c2);
						}
						break;

					default:
						val.Append(c);
						idx++;
						break;
				}
			}
			return null;
		}


		public static StringLiteral ScanString(TextReader jsonStr)
		{
			StringBuilder val = new StringBuilder(64);
			jsonStr.Read(); // skip the leading "
			while (jsonStr.Peek() > 0) 
			{
				char c = (char)jsonStr.Read();
#if DEBUG
				StreamContentBuf.Append(c);
#endif

				switch (c)
				{
					case '\"':
						// unescaped " must end the string
						return new StringLiteral(val);
					case '\\':
						// escaped character 
						char c2 = (char) jsonStr.Read();
						switch (c2)
						{
							case 'u':
								// unicode surrogate character, just copy all 6 chars into the literal as is 
								val.Append("\\u");
								for (int idx2 = 0; idx2 < 4; idx2++)
								{
									val.Append((char)jsonStr.Read());
								}
								break;
							case '\"':
								val.Append('\"');
								break;
							case '\'':
								val.Append('\'');
								break;
							case '/':
								val.Append('/');
								break;
							case '\\':
								val.Append('\\');
								break;
							case 'n':
								val.Append('\n');
								break;
							case 'r':
								val.Append('\r');
								break;
							case 't':
								val.Append('\t');
								break;
							case 'b':
								val.Append('\b');
								break;
							case 'f':
								val.Append('\f');
								break;
							case 'v': // not valid in JSON string
								val.Append('\v');
								break;
							case '0': // not valid in JSON string
								val.Append('\0');
								break;
							default: // not valid escaped character
								throw new Exception("Bad escape sequence: \\" + c2);
						}
						break;

					default:
						val.Append(c);
						break;
				}
			}
			return null;
		}

#if DEBUG
		static StringBuilder StreamContentBuf = null;
		string StreamContent = null;
#endif

		public static IList<JSONToken> Tokenize(string jsonStr)
        {
            List<JSONToken> tokens = new List<JSONToken>();

            int idx = 0;
            while (idx < jsonStr.Length)
            {
                char c = jsonStr[idx];

                switch (c)
                {
                    case '{':
                        tokens.Add(new StartBlock());
						idx++;
                        break;
                    case '}':
                        tokens.Add(new EndBlock());
						idx++;
                        break;
                    case '[':
                        tokens.Add(new StartArray());
						idx++;
                        break;
                    case ']':
                        tokens.Add(new EndArray());
						idx++;
                        break;
                    case ':':
                        tokens.Add(new FieldNameToken());
						idx++;
                        break;
                    case ',':
                        tokens.Add(new FieldSeperatorToken());
						idx++;
                        break;
                    case '"':
                        // start of a string literal
                        StringLiteral sl = ScanString(jsonStr, ref idx);
                        tokens.Add(sl);
                        break;
                    default:
                        //ignore white space
                        if (char.IsWhiteSpace(c))
                            idx++;
                        else // must be an unquoted word or number
                        {
                            StringBuilder sb = new StringBuilder(64);
                            while (!char.IsWhiteSpace(c) && !IsJSONSpecialChar(c) && (c!='\0'))
                            {
                                sb.Append(c);
								if (idx+1 < jsonStr.Length)
								{
									c = jsonStr[++idx];
								}
								else 
								{
									c = '\0';
								}
							}
                            string v = sb.ToString();
                            if (v == "null")
                            {
                                tokens.Add(new NullLiteral());
                            }
                            else if ((v == "true") || (v == "TRUE"))
                            {
                                tokens.Add(new BoolLiteral(true));
                            }
                            else if ((v == "false") || (v == "FALSE"))
                            {
                                tokens.Add(new BoolLiteral(false));
                            }
                            else
                            {
								if (v.IndexOf('.') >= 0)
								{
									// has a decimal point so treat as a double
									double d = 0;
									if (!double.TryParse(v, out d))
									{
										throw new Exception("Bad number literal: " + v);
									}
									tokens.Add(new DoubleLiteral(d));
								}
								else
								{
									// no decimal point so treat as an int
									int i = 0;
									if (!int.TryParse(v, out i))
									{
										throw new Exception("Bad number literal: " + v);
									}
									tokens.Add(new IntLiteral(i));
								}
                            }
                        }
                        break;
                }
            }
            if (tokens.Count > 0)
                return tokens;
            return null;
        }

		public static IList<JSONToken> Tokenize(TextReader jsonStr)
		{
			List<JSONToken> tokens = new List<JSONToken>();

			while (jsonStr.Peek() > 0)
			{
				char c = (char)jsonStr.Peek();
				switch (c)
				{
					case '{':
						tokens.Add(new StartBlock());
#if DEBUG
				StreamContentBuf.Append(c);
#endif
						jsonStr.Read(); //advance the position
						break;
					case '}':
						tokens.Add(new EndBlock());
#if DEBUG
				StreamContentBuf.Append(c);
#endif
						jsonStr.Read(); //advance the position
						break;
					case '[':
						tokens.Add(new StartArray());
#if DEBUG
				StreamContentBuf.Append(c);
#endif
						jsonStr.Read(); //advance the position
						break;
					case ']':
						tokens.Add(new EndArray());
#if DEBUG
				StreamContentBuf.Append(c);
#endif
						jsonStr.Read(); //advance the position
						break;
					case ':':
						tokens.Add(new FieldNameToken());
#if DEBUG
				StreamContentBuf.Append(c);
#endif
						jsonStr.Read(); //advance the position
						break;
					case ',':
						tokens.Add(new FieldSeperatorToken());
#if DEBUG
				StreamContentBuf.Append(c);
#endif
						jsonStr.Read(); //advance the position
						break;
					case '"':
						// start of a string literal
						StringLiteral sl = ScanString(jsonStr);
						tokens.Add(sl);
						break;
					default:
						//ignore white space
						if (char.IsWhiteSpace(c))
						{
#if DEBUG
							StreamContentBuf.Append(c);
#endif
							jsonStr.Read(); //advance the position
						}
						else // must be an unquoted word or number
						{
							StringBuilder sb = new StringBuilder(64);
							while (!char.IsWhiteSpace(c) && !IsJSONSpecialChar(c))
							{
								sb.Append(c);
#if DEBUG
								StreamContentBuf.Append(c);
#endif
								jsonStr.Read(); //advance the position
								c = (char)jsonStr.Peek();
							}
							string v = sb.ToString();
							if (v == "null")
							{
								tokens.Add(new NullLiteral());
							}
							else if ((v == "true") || (v == "TRUE"))
							{
								tokens.Add(new BoolLiteral(true));
							}
							else if ((v == "false") || (v == "FALSE"))
							{
								tokens.Add(new BoolLiteral(false));
							}
							else
							{
								if (v.IndexOf('.') >= 0)
								{
									// has a decimal point so treat as a double
									double d = 0;
									if (!double.TryParse(v, out d))
									{
										throw new Exception("Bad number literal: " + v);
									}
									tokens.Add(new DoubleLiteral(d));
								}
								else
								{
									// no decimal point so treat as an int
									int i = 0;
									if (!int.TryParse(v, out i))
									{
										throw new Exception("Bad number literal: " + v);
									}
									tokens.Add(new IntLiteral(i));
								}
							}
						}
						break;
				}
			}
			if (tokens.Count > 0)
				return tokens;
			return null;
		}
	}
}
