/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct PrimitiveDouble : IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return PrimitiveType.Double; } }
        public object Value { get { return DoubleValue; } }
        public int Length { get { return 8 + 1; } }
		public double DoubleValue;

		public PrimitiveDouble(SwfReader r)
		{
            byte[] bytes = new byte[8];
            bytes[4] = r.GetByte();
            bytes[5] = r.GetByte();
            bytes[6] = r.GetByte();
            bytes[7] = r.GetByte();

            bytes[0] = r.GetByte();
            bytes[1] = r.GetByte();
            bytes[2] = r.GetByte();
            bytes[3] = r.GetByte();

            DoubleValue = BitConverter.ToDouble(bytes, 0);
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.Write(DoubleValue);
		}
		public void ToSwf(SwfWriter w)
        {
            byte[] bytes = BitConverter.GetBytes(DoubleValue);



            w.AppendByte((byte)PrimitiveType);

            w.AppendByte(bytes[4]);
            w.AppendByte(bytes[5]);
            w.AppendByte(bytes[6]);
            w.AppendByte(bytes[7]);

            w.AppendByte(bytes[0]);
            w.AppendByte(bytes[1]);
            w.AppendByte(bytes[2]);
            w.AppendByte(bytes[3]);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(DoubleValue + " ");
		}
	}
}
