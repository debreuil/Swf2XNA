/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct Play : IAction
	{
		public ActionKind ActionId{get{return ActionKind.Play;}}
		public uint Version {get{return 3;}}
		public uint Length { get { return 1; } }

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("play");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.Play);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("Play");
		}
	}
}
