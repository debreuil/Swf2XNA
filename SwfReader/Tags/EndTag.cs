/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	public struct EndTag : ISwfTag, IControlTag
	{
		public const TagType tagType = TagType.End;
		public TagType TagType { get { return tagType; } }

		public EndTag(SwfReader r)
		{
		}

		public void ToSwf(SwfWriter w)
		{
			uint len = 0;
			w.AppendTagIDAndLength(this.TagType, len, false);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("End");
		}
	}
}
