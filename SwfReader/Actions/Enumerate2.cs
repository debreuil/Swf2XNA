/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct Enumerate2 : IAction
	{
		public ActionKind ActionId{get{return ActionKind.Enumerate2;}}
		public uint Version {get{return 6;}}
		public uint Length { get { return 1; } }

		public uint StackPops { get { return 1; } }
		public uint StackPushes { get { return 1 + PropertyCount; } }
		public int StackChange { get { return (int)PropertyCount; } }

		public uint PropertyCount { get { return 0; } } // todo: somehow deterime number of props on object

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("enumeratevalue");
		}
		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.Enumerate2);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
