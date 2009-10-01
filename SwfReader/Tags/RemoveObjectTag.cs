/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	public class RemoveObjectTag : ISwfTag, IControlTag
	{
		public const TagType tagType = TagType.RemoveObject;
		public TagType TagType { get { return tagType; } }

		public uint Character;
		public uint Depth;

		public RemoveObjectTag(SwfReader r)
		{
			this.Character = r.GetUI16();
			this.Depth = r.GetUI16();
		}

		public void ToSwf(SwfWriter w)
		{
			w.AppendTagIDAndLength(this.TagType, 2);
			w.AppendUI16(Character);
			w.AppendUI16(Depth);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("RemoveObject: " + this.Depth);
		}
	}
}
