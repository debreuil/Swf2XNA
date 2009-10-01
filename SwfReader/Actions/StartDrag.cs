/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct StartDrag : IAction, IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.StartDrag;}}
		public uint Version {get{return 4;}}
		public uint Length {get{return 1;}}

		public uint StackPops { get { return (uint)(3 + (Constrained ? 4 : 0)); } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return (int)(-3 - (Constrained ? 4 : 0)); } }

		public bool Constrained { get { return false; } } // todo: find if constrained from stack

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("stopdrag");
		}
		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.StartDrag);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
