/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public struct BackgroundColorTag : ISwfTag
	{
		public const TagType tagType = TagType.BackgroundColor;
		public TagType TagType { get { return tagType; } }

		public RGBA Color;

		public BackgroundColorTag(SwfReader r)
		{
			this.Color = new RGBA(r.GetByte(), r.GetByte(), r.GetByte(), 0xFF);
		}

		public void ToSwf(SwfWriter w)
		{
			uint len = 3;
			w.AppendTagIDAndLength(this.TagType, len, false);

			w.AppendByte(Color.R);
			w.AppendByte(Color.G);
			w.AppendByte(Color.B);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("Background Color Tag: ");
			this.Color.Dump(w);
			w.WriteLine();
		}
	}
}
