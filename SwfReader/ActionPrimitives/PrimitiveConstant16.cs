/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct PrimitiveConstant16 : IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return PrimitiveType.Constant16; } }
        public object Value { get { return Constant16Value; } }
        public int Length { get { return 2 + 1; } }
		public uint Constant16Value;

		public PrimitiveConstant16(SwfReader r)
		{
			Constant16Value = r.GetUI16();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			if (ActionRecords.CurrentConstantPool != null && ActionRecords.CurrentConstantPool.Constants.Length > Constant16Value)
			{
				string s = ActionRecords.CurrentConstantPool.Constants[Constant16Value];
				w.Write("'" + PrimitiveString.EscapeString(s) + "'");
			}
			else
			{
				w.Write("cp: " + Constant16Value);
			}
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)PrimitiveType);
            w.AppendUI16(Constant16Value);
		}

		public void Dump(IndentedTextWriter w)
		{
			if (ActionRecords.CurrentConstantPool != null && ActionRecords.CurrentConstantPool.Constants.Length > Constant16Value)
			{
				string s = ActionRecords.CurrentConstantPool.Constants[Constant16Value];
				w.WriteLine("cp_" + Constant16Value + " \"" +
					PrimitiveString.EscapeString(s) + "\"");
			}
			else
			{
				w.WriteLine("cp: " + Constant16Value + " ");
			}
		}
	}
}
