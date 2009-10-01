using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	public class RemoveObject2 : ISwfTag
	{
		public const TagType tagType = TagType.RemoveObject2;
		public TagType TagType { get { return tagType; } }

		public uint Depth;

		public RemoveObject2(SwfReader r)
		{
			this.Depth = r.GetUI16();
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("Remove Object: " + this.Depth);
		}
	}
}
