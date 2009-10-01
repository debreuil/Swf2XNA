/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class ConstantPool : IAction , IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.ConstantPool;}}
		public uint Version {get{return 5;}}
		public uint Length
		{
			get
			{
				uint len = 5;
				foreach (string s in Constants)
				{
					len += (uint)s.Length + 1;
				}
				return len;
			}
		}

		public uint StackPops { get { return 0; } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return 0; } }


		public string[] Constants;

		public ConstantPool(SwfReader r)
		{
            uint len = r.GetUI16();

			Constants = new string[len];
			for (int i = 0; i < len; i++)
			{
				Constants[i] = r.GetString();
			}
		}
		// todo: getIndex

		public void ToFlashAsm(IndentedTextWriter w)
		{
			string comma = Constants.Length > 1 ? ", " : "";
			w.Write("constants ");
			for (int i = 0; i < Constants.Length; i++)
			{
				w.Write("'" + PrimitiveString.EscapeString(Constants[i]) + "'" + comma);
				if (i == Constants.Length - 2)
				{
					comma = "";
				}
			}
			w.WriteLine("");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.ConstantPool);

            long lenPos = w.Position;
            w.AppendUI16(0); // length

            w.AppendUI16((uint)Constants.Length);

            for (int i = 0; i < Constants.Length; i++)
            {
                w.AppendString(Constants[i]);
            }
            long temp = w.Position;
            w.Position = lenPos;
            w.AppendUI16((uint)(temp - lenPos - 2)); // skip len bytes
            w.Position = temp;

		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("Action: " + Enum.GetName(typeof(ActionKind), this.ActionId));
			for (int i = 0; i < Constants.Length; i++)
			{
				w.Write(i + ":" + PrimitiveString.EscapeString(Constants[i]) + "\t");
			}
			w.WriteLine("");
		}
	}
}
