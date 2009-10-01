/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct PrimitiveInteger : IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return PrimitiveType.Integer; } }
        public object Value { get { return IntegerValue; } }
        public int Length { get { return 4 + 1; } }
		public int IntegerValue;

		public PrimitiveInteger(SwfReader r)
		{
			IntegerValue = r.GetInt32();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.Write(IntegerValue);
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)PrimitiveType);
            w.AppendInt32(IntegerValue);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(IntegerValue + " ");
		}
	}
}
