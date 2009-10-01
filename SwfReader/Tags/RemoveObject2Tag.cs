/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	public class RemoveObject2Tag : ISwfTag, IControlTag
	{
		public const TagType tagType = TagType.RemoveObject2;
		public TagType TagType { get { return tagType; } }

		public uint Depth;

		public RemoveObject2Tag(SwfReader r)
		{
			this.Depth = r.GetUI16();
		}

		public void ToSwf(SwfWriter w)
		{
			w.AppendTagIDAndLength(this.TagType, 2);
			w.AppendUI16(Depth);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("RemoveObject2: " + this.Depth);
		}
	}
}
