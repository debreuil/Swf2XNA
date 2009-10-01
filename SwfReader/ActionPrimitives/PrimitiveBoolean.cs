/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct PrimitiveBoolean : IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return PrimitiveType.Boolean; } }
		public object Value { get { return BooleanValue; } }
        public int Length { get { return 1 + 1; } }
		public bool BooleanValue;

		public PrimitiveBoolean(SwfReader r)
		{
			BooleanValue = r.GetByte() > 0 ? true : false; // stored as byte
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.Write(BooleanValue.ToString().ToUpper());
		}
		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)PrimitiveType);
            byte b = (BooleanValue == true) ? (byte)0x01 : (byte)0x00;
            w.AppendByte(b);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(BooleanValue + " ");
		}
	}
}
