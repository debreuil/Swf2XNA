/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	public struct PrimitiveString : IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return PrimitiveType.String; } }
        public object Value { get { return StringValue; } }
        public int Length { get { return StringValue.Length + 2; } }
		public string StringValue;

		public PrimitiveString(SwfReader r)
		{
			StringValue = r.GetString();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.Write("'" + EscapeString(StringValue) + "'");
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)PrimitiveType);
            w.AppendString(StringValue);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(StringValue + " ");
		}

		public static string EscapeString(string s)
		{
			if(s.IndexOf('\n') > -1)
			{
				s = s + "";
			}
			s = s.Replace("\n", "\\n");
			s = s.Replace("\r", "\\r");
			s = s.Replace("\t", "\\t");
			return s;
		}
	}
}
