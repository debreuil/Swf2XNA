/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public struct GradientRecord : IRecord
	{
		public byte Ratio;
		public RGBA Color;
		public GradientRecord(SwfReader r, bool useAlpha)
		{
			Ratio = r.GetByte();
			if (useAlpha)
			{
				Color = new RGBA(r.GetByte(), r.GetByte(), r.GetByte(), r.GetByte());
			}
			else
			{
				Color = new RGBA(r.GetByte(), r.GetByte(), r.GetByte());
			}
		}


		public void ToSwf(SwfWriter w)
		{
			throw new NotSupportedException("use overload that specifies alpha");
		}
		public void ToSwf(SwfWriter w, bool useAlpha)
		{
			w.AppendByte(Ratio);
			w.AppendByte(Color.R);
			w.AppendByte(Color.G);
			w.AppendByte(Color.B);
			if (useAlpha)
			{
				w.AppendByte(Color.A);
			}
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("[");
			w.Write("r:" + Ratio.ToString("X2") + " c:");
			Color.Dump(w);
			w.Write("]");
		}
	}
}
