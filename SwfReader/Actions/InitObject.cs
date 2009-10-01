/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct InitObject : IAction , IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.InitObject;}}
		public uint Version {get{return 5;}}
		public uint Length { get { return 1; } }

		public uint StackPops { get { return 1 + NumArgs*2; } }
		public uint StackPushes { get { return 1; } }
		public int StackChange { get { return (int)(-NumArgs * 2); } }

		public uint NumArgs { get { return 0; } } // get num args from stack

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("initobject");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.InitObject);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
