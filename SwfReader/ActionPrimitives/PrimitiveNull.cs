/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct PrimitiveNull : IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return PrimitiveType.Null; } }
        public object Value { get { return null; } }
        public int Length { get { return 0 + 1; } }
		public PrimitiveNull(SwfReader r)
		{
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.Write("NULL");
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)PrimitiveType);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("null ");
		}
	}
}
