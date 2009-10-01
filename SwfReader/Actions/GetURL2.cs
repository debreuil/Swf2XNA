/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct GetURL2 : IAction, IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.GetURL2;}}
		public uint Version {get{return 4;}}
		public uint Length { get { return 4; } }

		public uint StackPops { get { return 2; } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return -2; } }


		public SendVarsMethod SendVarsMethod;		
		public bool TargetIsSprite; // or sprite		
		public bool LoadVariables;

		public GetURL2(SwfReader r)
		{
			SendVarsMethod = (SendVarsMethod)r.GetBits(2);
			r.GetBits(4); // reserved
			TargetIsSprite = r.GetBit();
			LoadVariables = r.GetBit();
            r.Align();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("geturl2");
		}
		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.GetURL2);
            w.AppendUI16(Length - 3); // don't incude def byte and len

            w.AppendBits((uint)SendVarsMethod, 2);
            w.AppendBits(0, 4);
            w.AppendBit(TargetIsSprite);
            w.AppendBit(LoadVariables);
            w.Align();
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}

	public enum SendVarsMethod
	{
		None = 0,
		Get = 1,
		Post = 2,
	}
}
