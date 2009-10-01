/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct FilterDropShadow : IFilter
	{
		public FilterKind FilterKind { get { return FilterKind.DropShadow; } }

		RGBA DropShadowColor;
		float BlurX;
		float BlurY;
		float Angle;
		float Distance;
		float Strength;
		bool InnerShadow;
		bool Knockout;
		bool CompositeSource;
		uint Passes;

		public FilterDropShadow(SwfReader r)
		{
			DropShadowColor = new RGBA(r.GetByte(), r.GetByte(), r.GetByte(), r.GetByte());

			BlurX = r.GetFixed16_16();
			BlurY = r.GetFixed16_16();
			Angle = r.GetFixed16_16();
			Distance = r.GetFixed16_16();
			Strength = r.GetFixed8_8(); 
			
			InnerShadow = r.GetBit();
			Knockout = r.GetBit();
			CompositeSource = r.GetBit();	
			Passes = (uint)r.GetBits(5);

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
