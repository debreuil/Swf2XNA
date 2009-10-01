/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct WaitForFrame : IAction
	{
		// this is 'ifFrameLoaded' in swf, deprecated...
		public ActionKind ActionId{get{return ActionKind.WaitForFrame;}}
		public uint Version {get{return 3;}}
		public uint Length{get{return 6;}}
		
		public int Frame;		
		public uint SkipCount;

		public WaitForFrame(SwfReader r)
		{
			Frame = r.GetUI16();
			SkipCount = (uint)r.GetByte();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("ifframeloaded");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.WaitForFrame);
            w.AppendUI16(Length - 3); // don't incude def byte and len

            w.AppendUI16((uint)Frame);
            w.AppendByte((byte)SkipCount);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("WaitForFrame: " + this.Frame + " or skip: " + this.SkipCount);
		}
	}
}
