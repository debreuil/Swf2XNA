/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	public struct FrameLabelTag : ISwfTag, IControlTag
	{
		public const TagType tagType = TagType.FrameLabel;
		public TagType TagType { get { return tagType; } }

		public string TargetName;

		public FrameLabelTag(SwfReader r)
		{
			this.TargetName = r.GetString();
		}

		public void ToSwf(SwfWriter w)
		{
			w.AppendTagIDAndLength(this.TagType, (uint)TargetName.Length + 1, true);
			w.AppendString(TargetName); // todo: check for unicode implications on labels
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("FrameLabel: " + TargetName);
		}
	}
}
