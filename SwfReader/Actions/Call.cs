/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct Call : IAction, IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.Call;}}
		public uint Version {get{return 4;}}
		public uint Length { get { return 6; } }

		public uint StackPops { get { return 1; } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return -1; } }

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("callframe");
		}

		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)ActionKind.Call);
            w.AppendUI16(Length - 3); // don't incude def byte and len

		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("Call: " + Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
