/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct KerningRecord
	{
		public uint FontKerningCode1;
		public uint FontKerningCode2;
		public int FontKerningAdjustment;		

		public KerningRecord(uint fontKerningCode1, uint fontKerningCode2, int fontKerningAdjustment)
		{
			this.FontKerningCode1 = fontKerningCode1;
			this.FontKerningCode2 = fontKerningCode2;
			this.FontKerningAdjustment = fontKerningAdjustment;
		}
	}
}
