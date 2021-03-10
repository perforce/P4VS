using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perforce.JSONParser
{
        public abstract class JSONToken {}
        /// <summary>
        /// a '{'
        /// </summary>
		public class StartBlock : JSONToken { }
        /// <summary>
        /// a '}'
        /// </summary>
		public class EndBlock : JSONToken { }

        /// <summary>
        /// a '['
        /// </summary>
		public class StartArray : JSONToken { }
        /// <summary>
        /// a ']'
        /// </summary>
		public class EndArray : JSONToken { }
        
        /// <summary>
        /// a ':' 
        /// </summary>
		public class FieldNameToken : JSONToken { }
        
        /// <summary>
        /// a ',' 
        /// </summary>
		public class FieldSeperatorToken : JSONToken { }

        /// <summary>
        /// A string delineated by '"'
        /// </summary>
        /// <returns></returns>
		public class StringLiteral : JSONToken
        {
            public StringLiteral(string v) { Value = v; }
            public StringLiteral(StringBuilder v) { Value = v.ToString(); }
            public string Value { get; private set; }
        }

        /// <summary>
        /// A double floating number
        /// </summary>
        /// <returns></returns>
		public class DoubleLiteral : JSONToken
        {
            public DoubleLiteral(double v) { Value = v; }
            public double Value { get; private set; }
        }

        /// <summary>
        /// An
        /// </summary>
        /// <returns></returns>
		public class IntLiteral : JSONToken
        {
			public IntLiteral(int v) { Value = v; }
            public int Value { get; private set; }
        }

        /// <summary>
        /// A bool
        /// </summary>
        /// <returns></returns>
		internal class BoolLiteral : JSONToken
        {
            public BoolLiteral(bool v) { Value = v; }
            public bool Value { get; private set; }
        }

        /// <summary>
        /// A bool
        /// </summary>
        /// <returns></returns>
		internal class NullLiteral : JSONToken { }
}
