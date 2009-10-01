/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct PrimitiveUndefined : IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return PrimitiveType.Undefined; } }
        public object Value { get { return null; } }
        public int Length { get { return 0 + 1; } }
		public PrimitiveUndefined(SwfReader r)
		{
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.Write("UNDEF");
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)PrimitiveType);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("undefined ");
		}
	}
}
