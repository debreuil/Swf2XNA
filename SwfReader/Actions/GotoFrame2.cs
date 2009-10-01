/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public struct GotoFrame2 : IAction, IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.GotoFrame2;}}
		public uint Version {get{return 4;}}
		public uint Length
		{
			get
			{
				uint len = 4;
				if(SceneBiasFlag) len += 2;
				return len;
			}
		}

		public uint StackPops { get { return 1; } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return -1; } }

		
		public bool SceneBiasFlag;		
		public bool PlayFlag;		
		public uint SceneBias;

		public GotoFrame2(SwfReader r)
		{
			r.GetBits(6); // reserved
			SceneBiasFlag = r.GetBit();
			PlayFlag = r.GetBit();
			r.Align();
			if (SceneBiasFlag)
			{
				SceneBias = r.GetUI16();
			}
			else
			{
				SceneBias = 0;
			}
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("gotoframe");
		}

		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)ActionKind.GotoFrame2);
            w.AppendUI16(Length - 3); // don't incude def byte and len

            w.AppendBits(0, 6);
            w.AppendBit(SceneBiasFlag);
            w.AppendBit(PlayFlag);
            w.Align();
			if (SceneBiasFlag)
			{
                w.AppendUI16(SceneBias);
			}
		}

		public void Dump(IndentedTextWriter w)
		{
			string play = this.PlayFlag ? "play" : "stop";
			w.WriteLine("GotoFrame2 (stack): " + play);
		}
	}
}
