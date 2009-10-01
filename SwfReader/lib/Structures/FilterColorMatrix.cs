/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct FilterColorMatrix : IFilter
	{
		public FilterKind FilterKind { get { return FilterKind.ColorMatrix; } }

		float[] Matrix;
		public FilterColorMatrix(SwfReader r)
		{
			this.Matrix = new float[20];
			for (int i = 0; i < 20; i++)
			{
				this.Matrix[i] = r.GetFloat32();
			}
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
