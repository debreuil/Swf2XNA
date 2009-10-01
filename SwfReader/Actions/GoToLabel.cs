/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct GoToLabel : IAction
	{
		public ActionKind ActionId{get{return ActionKind.GoToLabel;}}
		public uint Version {get{return 3;}}
		
		public string Label;
        public uint Length { get { return (uint)(3 + Label.Length + 1); } }

		public GoToLabel(SwfReader r)
		{
			Label = r.GetString();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("gotolabel");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.GoToLabel);
            w.AppendUI16(Length - 3); // don't incude def byte and len

            w.AppendString(Label);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("Goto: " + this.Label);
		}
	}
}
