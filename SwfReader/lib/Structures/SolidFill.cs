/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public struct SolidFill : IFillStyle
	{
		public RGBA Color;

		public FillType FillType{get{return FillType.Solid;}}

		public SolidFill(RGBA c)
		{
			this.Color = c;
		}
		
		public SolidFill(SwfReader r, bool useAlpha)
		{
			if (useAlpha)
			{
				this.Color = new RGBA(r.GetByte(), r.GetByte(), r.GetByte(), r.GetByte());
			}
			else
			{
				this.Color = new RGBA(r.GetByte(), r.GetByte(), r.GetByte());
			}
		}

		public void ToSwf(SwfWriter w)
		{
			ToSwf(w, true);
		}
		public void ToSwf(SwfWriter w, bool useAlpha)
		{
			w.AppendByte((byte)this.FillType);

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
			w.Write("Solid Fill: ");
			Color.Dump(w);
		}
	}
}
