/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct CallFunction : IAction , IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.CallFunction;}}
		public uint Version {get{return 5;}}
		public uint Length {get{return 1;}}

		public uint StackPops { get { return 2 + NumArgs; } }
		public uint StackPushes { get { return 1; } }
		public int StackChange { get { return (int)(-1 - NumArgs); } }

		public uint NumArgs { get { return 0; } } // todo: find num args from stack

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("callfunction");
		}

		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)ActionKind.CallFunction);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
