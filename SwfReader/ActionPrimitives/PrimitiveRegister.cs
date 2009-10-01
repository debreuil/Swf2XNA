/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct PrimitiveRegister : IPrimitive
    {
        public PrimitiveType PrimitiveType { get { return PrimitiveType.Register; } }
        public object Value { get { return RegisterValue; } }
        public int Length { get { return 1 + 1; } }
		public byte RegisterValue;

		public PrimitiveRegister(SwfReader r)
		{
			RegisterValue = r.GetByte();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.Write("r:" + RegisterValue);
			/* 
			 * // THIS ROUTINE WILL WRITE THE ACTUAL REG VALUE (THIS, SUPER ETC) FOR WHEN ELEMENTS ARE PRELOADED
			 * 
			if (RegisterValue < 6 && DoActionTag.CurrentDumpStatement is DefineFunction2)
			{
				int pf = (int)((DefineFunction2)DoActionTag.CurrentDumpStatement).Preloads;
				int active = 0;
				uint index = 0;
				for (int pow = 1; pow < (int)PreloadFlags.End; pow*=2)
				{
					if ((pf & pow) > 0)
					{
						active = pow;
						if (++index == RegisterValue)
						{
							break;
						}
					}
				}
				if (index == RegisterValue)
				{
					w.Write(DefineFunction2.PreloadFlagToString((PreloadFlags)active));
				}
				else
				{
					w.Write("r:" + RegisterValue);
				}
			}
			else
			{
				w.Write("r:" + RegisterValue);
			}
			*/
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)PrimitiveType);
            w.AppendByte(RegisterValue);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(RegisterValue + " ");
		}
	}
}
