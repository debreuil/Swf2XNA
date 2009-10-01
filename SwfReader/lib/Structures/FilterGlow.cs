/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct FilterGlow : IFilter
	{
		public FilterKind FilterKind { get { return FilterKind.Glow; } }

		RGBA GlowColor;
		float BlurX;
		float BlurY;
		float Strength;
		bool InnerGlow;
		bool Knockout;
		bool CompositeSource;
		uint Passes;

		public FilterGlow(SwfReader r)
		{
			GlowColor = new RGBA(r.GetByte(), r.GetByte(), r.GetByte(), r.GetByte());

			BlurX = r.GetFixed16_16();
			BlurY = r.GetFixed16_16();
			Strength = r.GetFixed8_8(); 
			
			InnerGlow = r.GetBit();
			Knockout = r.GetBit();
			CompositeSource = r.GetBit();	
			Passes = r.GetBits(5);

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
