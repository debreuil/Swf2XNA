/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class DefineFunction : IAction , IStackManipulator, IActionContainer
	{
		/*
			FunctionName STRING
			NumParams UI16
			param 1 STRING
			param 2 STRING
			...
			param N STRING
			codeSize UI16
		 */
		public ActionKind ActionId{get{return ActionKind.DefineFunction;}}
		public uint Version {get{return 5;}}
		public uint Length
		{
			get
			{
				uint len = 7;
				len += (uint)FunctionName.Length + 1;
				for(int i = 0; i < Params.Length; i++)
				{
					len += (uint)Params[i].Length + 1;
				}
				return len;
			}
		}

		public uint StackPops { get { return 0; } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return 0; } }

		private uint codeSize;
		public uint CodeSize { get { return codeSize; } set { codeSize = value; } } // todo: calc code size
		private List<IAction> statements = new List<IAction>();
		public List<IAction> Statements { get { return statements; } }

		public string FunctionName;
		public string[] Params;

		private ConstantPool cp;

		public DefineFunction(SwfReader r, ConstantPool cp)
		{
			this.cp = cp;
			FunctionName = r.GetString();
			uint paramCount = r.GetUI16();
			Params = new string[paramCount];
			for (int i = 0; i < paramCount; i++)
			{
				Params[i] = r.GetString();
			}

            CodeSize = r.GetUI16();
		}


		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.WriteLine("function '" + FunctionName + "' " + ActionRecords.GetLabel((int)CodeSize)); // todo: function end code label
			ActionRecords.CurrentConstantPool = cp;
			for (int i = 0; i < Statements.Count; i++)
			{
				ActionRecords.AutoLineLabel(w);
				Statements[i].ToFlashAsm(w);
			}
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)ActionKind.DefineFunction);
            w.AppendUI16(Length - 3); // don't incude def byte and len

            w.AppendString(FunctionName);
            w.AppendUI16((uint)Params.Length);
            for (int i = 0; i < Params.Length; i++)
            {
                w.AppendString(Params[i]);
            }

            w.AppendUI16(CodeSize); // temp
            long startPos = w.Position;

            for (int i = 0; i < Statements.Count; i++)
            {
                Statements[i].ToSwf(w);
            }

            // adjust code size
            long curPos = w.Position;
            if (codeSize != (curPos - startPos))
            {
                codeSize = (uint)(curPos - startPos);
                w.Position = startPos - 2;
                w.AppendUI16(CodeSize); // acutal
                w.Position = curPos;
            }
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine(Enum.GetName(typeof(ActionKind), this.ActionId));
			ActionRecords.CurrentConstantPool = cp;
			for (int i = 0; i < Statements.Count; i++)
			{
				ActionRecords.AutoLineLabel(w);
				Statements[i].Dump(w);
			}
		}
	}
}
