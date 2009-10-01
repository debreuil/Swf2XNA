/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct Enumerate : IAction , IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.Enumerate;}}
		public uint Version {get{return 5;}}
		public uint Length { get { return 1; } }

		public uint StackPops { get { return 1 + PropertyCount; } }
		public uint StackPushes { get { return 1; } }
		public int StackChange { get { return (int)PropertyCount; } }

		public uint PropertyCount { get { return 0; } } // todo: get number of props from, hmm...

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("enumerate");
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)ActionKind.Enumerate);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
