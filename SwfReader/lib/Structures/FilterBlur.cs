/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct FilterBlur : IFilter
	{
		public FilterKind FilterKind { get { return FilterKind.Blur; } }

		float BlurX;
		float BlurY;
		uint Passes;

		public FilterBlur(SwfReader r)
		{
			BlurX = r.GetFixed16_16();
			BlurY = r.GetFixed16_16();
			Passes = r.GetBits(5);
			r.GetBits(3); // reserved
			r.Align();
		}
		public void ToSwf(SwfWriter w)
		{
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(this);
		}
	}
}
