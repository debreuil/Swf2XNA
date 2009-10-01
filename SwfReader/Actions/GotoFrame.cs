/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct GotoFrame : IAction
	{
		public ActionKind ActionId{get{return ActionKind.GotoFrame;}}
		public uint Version {get{return 3;}}
		public uint Length	{get{return 5;}}	
		public int Frame;

		public GotoFrame(SwfReader r)
		{
			Frame = r.GetInt16();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("gotoandplay"); // todo: need to look at prev/next instr to find out if this is play or stop
		}
		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.GotoFrame);
            w.AppendUI16(Length - 3);// don't incude this part
            w.AppendInt16(Frame);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("GotoFrame: " + this.Frame);
		}
	}
}
