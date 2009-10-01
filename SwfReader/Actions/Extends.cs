/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct Extends : IAction
	{
		public ActionKind ActionId{get{return ActionKind.Extends;}}
		public uint Version {get{return 7;}}
		public uint Length { get { return 1; } }

		public uint StackPops { get { return 2; } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return -2 ; } }


		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("extends");
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)ActionKind.Extends);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
