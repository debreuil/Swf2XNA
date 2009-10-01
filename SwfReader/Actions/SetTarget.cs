/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct SetTarget : IAction
	{
		public ActionKind ActionId{get{return ActionKind.SetTarget;}}
		public uint Version {get{return 3;}}		
		public string TargetName;
		public uint Length
		{
			get
			{
                return (uint)(3 + TargetName.Length + 1); 
			}
		}

		public SetTarget(SwfReader r)
		{
			TargetName = r.GetString();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("settarget");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.SetTarget);
            w.AppendUI16(Length - 3); // don't incude def byte and len

            w.AppendString(TargetName);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("SetTarget: " + TargetName);
		}
	}
}
