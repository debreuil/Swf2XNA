/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct ImplementsOp : IAction, IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.ImplementsOp;}}
		public uint Version {get{return 7;}}
		public uint Length {get{return 1;}}

		public uint StackPops { get { return 2 + ImplementedInterfaces; } }
		public uint StackPushes { get { return 1; } }
		public int StackChange { get { return (int)(-1 - ImplementedInterfaces); } }

		public uint ImplementedInterfaces { get { return 1; } } // todo: find interface count from stack

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("implements"); 
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.ImplementsOp);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
