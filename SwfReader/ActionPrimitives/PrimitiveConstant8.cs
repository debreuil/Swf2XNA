/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct PrimitiveConstant8 : IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return PrimitiveType.Constant8; } }
        public object Value { get { return Constant8Value; } }
        public int Length { get { return 1 + 1; } }
		public uint Constant8Value;

		public PrimitiveConstant8(SwfReader r)
		{
			Constant8Value = (uint)r.GetByte();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			if (ActionRecords.CurrentConstantPool != null && ActionRecords.CurrentConstantPool.Constants.Length > Constant8Value)
			{
				string s = ActionRecords.CurrentConstantPool.Constants[Constant8Value];
				w.Write("'" + PrimitiveString.EscapeString(s) + "'");
			}
			else
			{
				w.Write("cp: " + Constant8Value + " ");
			}
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)PrimitiveType);
            w.AppendByte((byte)Constant8Value);
		}

		public void Dump(IndentedTextWriter w)
		{
			if (ActionRecords.CurrentConstantPool != null && ActionRecords.CurrentConstantPool.Constants.Length > Constant8Value)
			{
				string s = ActionRecords.CurrentConstantPool.Constants[Constant8Value];
				w.WriteLine("cp_" + Constant8Value  + " \"" +
					PrimitiveString.EscapeString(s) + "\"");
			}
			else
			{
				w.WriteLine("cp: " + Constant8Value + " ");
			}
		}
	}
}
