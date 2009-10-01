/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct StringExtract : IAction, IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.StringExtract;}}
		public uint Version {get{return 4;}}
		public uint Length {get{return 1;}}

		public SwfType[] Types { get { return new SwfType[] { SwfType.Number, SwfType.Number, SwfType.String, SwfType.String }; } }
		public uint StackPops { get { return 3; } }
		public uint StackPushes { get { return 1; } }
		public int StackChange { get { return -2; } }

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("substring");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.StringExtract);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
