
/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public struct UnsupportedDefinitionTag : ISwfTag
	{
		public const TagType tagType = TagType.UnsupportedDefinition;
		public TagType TagType { get { return tagType; } }
		public string Message;

		public UnsupportedDefinitionTag(SwfReader r, string msg)
		{
			this.Message = msg;
		}

		public void ToSwf(SwfWriter w)
		{
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("UNSUPPORTED DEFINITION TAG! " + this.Message);
		}
	}
}
