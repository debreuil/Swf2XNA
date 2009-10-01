/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct FilterConvolution : IFilter
	{
		public FilterKind FilterKind { get { return FilterKind.Convolution; } }

		uint MatrixX;
		uint MatrixY;
		float Divisor;
		float Bias;
		float[] Matrix;
		RGBA DefaultColor;
		bool Clamp;
		bool PreserveAlpha;

		public FilterConvolution(SwfReader r)
		{
			MatrixX = (uint)r.GetByte();
			MatrixY = (uint)r.GetByte();
			Divisor = r.GetFloat32();
			Bias = r.GetFloat32();

			uint mxCount = MatrixX * MatrixY;
			Matrix = new float[mxCount];
			for (int i = 0; i < mxCount; i++)
			{
				Matrix[i] = r.GetFloat32();
			}

			DefaultColor = new RGBA(r.GetByte(), r.GetByte(), r.GetByte(), r.GetByte());

			r.GetBits(6);
			Clamp = r.GetBit();
			PreserveAlpha = r.GetBit();

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
