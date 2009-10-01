/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct If : IAction, IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.If;}}
		public uint Version {get{return 4;}}
		public uint Length { get { return 5; } }

		public uint StackPops { get { return 1; } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return -1; } }

		
		public int BranchOffset;

		public If(SwfReader r)
		{
			BranchOffset = r.GetInt16();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("branchiftrue " + ActionRecords.GetLabel(BranchOffset)); // todo: add label logic to if's
		}

		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)ActionKind.If);
            w.AppendUI16(Length - 3); // don't incude def byte and len

            w.AppendInt16(BranchOffset);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
