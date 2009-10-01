/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct PrimitiveFloat : IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return PrimitiveType.Float; } }
        public object Value { get { return FloatValue; } }
        public int Length { get { return 4 + 1; } }
		public float FloatValue;

		public PrimitiveFloat(SwfReader r)
		{
			FloatValue = r.GetFixedNBits(32);
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.Write(FloatValue);
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)PrimitiveType);
            w.AppendFixedNBits(FloatValue, 32);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(FloatValue + " ");
		}
	}
}
