/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct WaitForFrame2 : IAction, IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.WaitForFrame2;}}
		public uint Version {get{return 4;}}
		public uint Length{get{return 4;}}

		public uint StackPops { get { return 1; } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return -1; } }
		
		public uint SkipCount;

		public WaitForFrame2(SwfReader r)
		{
			SkipCount = (uint)r.GetByte();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("ifframeloadedexpr");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.WaitForFrame2);
            w.AppendUI16(Length - 3); // don't incude def byte and len

            w.AppendByte((byte)SkipCount);

		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
