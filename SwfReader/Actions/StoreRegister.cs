/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct StoreRegister : IAction , IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.StoreRegister;}}
		public uint Version {get{return 5;}}	
		public uint Length{get{return 4;}}
		
		public uint StackPops { get { return 0; } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return 0; } }

		public uint Register;

		public StoreRegister(SwfReader r)
		{
			Register = (uint)r.GetByte();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("setregister r:" + Register);
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.StoreRegister);
            w.AppendUI16(Length - 3); // don't incude this part

            w.AppendByte((byte)Register);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("StoreRegister r:" + Register);
		}
	}
}
