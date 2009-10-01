/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class With : IAction , IStackManipulator, IActionContainer
	{
		public ActionKind ActionId{get{return ActionKind.With;}}
		public uint Version { get { return 5; } }
		public uint Length
		{
			get
			{
				uint len = 3 + 2;
				len += (uint)WithBlock.Length + 1;
				return len;
			}
		}

		public uint StackPops { get { return 1; } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return -1; } }

		private uint codeSize;
		public uint CodeSize { get { return codeSize; } set { codeSize = value; } }
		private List<IAction> statements = new List<IAction>();
		public List<IAction> Statements { get { return statements; } }

		public uint Size
		{
			get
			{
				if(Statements == null) return 0;
				return (uint)Statements.Count;
			}
		}
		
		public string WithBlock;
		
		public List<IAction>[] ActionCollections
		{
			get
			{
				List<IAction>[] actions = 
					new List<IAction>[1]{Statements};
				return actions;
			}
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("with");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.With);
            w.AppendUI16(Length - 3); // don't incude def byte and len

            // todo: ctor missing
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
		}
	}
}
