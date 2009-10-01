/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct NewMethod : IAction , IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.NewMethod;}}
		public uint Version {get{return 5;}}
		public uint Length { get { return 1; } }

		public uint StackPops { get { return 3 + NumArgs; } }
		public uint StackPushes { get { return 1; } }
		public int StackChange { get { return (int)(-2 - NumArgs); } }

		public uint NumArgs { get { return 0; } } // get num args from stack

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("newmethod");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.NewMethod);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
