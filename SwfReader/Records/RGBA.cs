/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public struct RGBA : IRecord
	{
		public byte R;
		public byte G;
		public byte B;
		public byte A;

		public RGBA(byte r, byte g, byte b, byte a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		public RGBA(byte r, byte g, byte b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = 0xFF;
		}

		public void ToSwf(SwfWriter w)
        {
            throw new Exception("RGB must be written manually as rgb vs rgba is ambigous");
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("#");
			w.Write(this.R.ToString("X2"));
			w.Write(this.G.ToString("X2"));
			w.Write(this.B.ToString("X2"));
			w.Write(this.A.ToString("X2"));
			w.Write(" ");
		}
	}
}
