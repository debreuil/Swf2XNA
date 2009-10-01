/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public struct LineStyle : ILineStyle
	{
		public ushort Width;
		public RGBA Color;

		public LineStyle(SwfReader r, bool useAlpha)
		{
			this.Width = r.GetUI16();
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
			w.AppendUI16(this.Width);

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
			w.Write("LS1 ");
			Color.Dump(w);
			w.Write(" w: " + this.Width);
		}
	}
}
