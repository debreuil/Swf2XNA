/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class Push : IAction, IStackManipulator
	{
		public ActionKind ActionId{get{return ActionKind.Push;}}
		public uint Version {get{return 4;}}
		public uint Length
		{
			get
			{
				uint len = 3;
				foreach (IPrimitive p in Values)
				{
					len += (uint)p.Length;
				}
				return len;
			}
		}

		public uint StackPops { get { return 0; } }
		public uint StackPushes { get { return (uint)Values.Count; } }
		public int StackChange { get { return Values.Count; } }

		public PrimitiveType PrimitiveType;
		public List<IPrimitive> Values = new List<IPrimitive>();

		public Push(SwfReader r, uint maxLen)
		{
			PrimitiveType type;
			while (r.Position < maxLen)
			{
				type = (PrimitiveType)(r.GetByte());
				switch (type)
				{
					case PrimitiveType.String:
						{
							Values.Add(new PrimitiveString(r));
							break;
						}
					case PrimitiveType.Float:
						{
							Values.Add(new PrimitiveFloat(r));
							break;
						}
					case PrimitiveType.Null:
						{
							Values.Add(new PrimitiveNull(r));
							break;
						}
					case PrimitiveType.Register:
						{
							Values.Add(new PrimitiveRegister(r));
							break;
						}
					case PrimitiveType.Boolean:
						{
							Values.Add(new PrimitiveBoolean(r));
							break;
						}
					case PrimitiveType.Double:
						{
							Values.Add(new PrimitiveDouble(r));
							break;
						}
					case PrimitiveType.Integer:
						{
							Values.Add(new PrimitiveInteger(r));
							break;
						}
					case PrimitiveType.Constant8:
						{
							Values.Add(new PrimitiveConstant8(r));
							break;
						}
					case PrimitiveType.Constant16:
						{
							Values.Add(new PrimitiveConstant16(r));
							break;
						}
					case PrimitiveType.Undefined:
						{
							Values.Add(new PrimitiveUndefined(r));
							break;
						}
				}
			}
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			string comma = Values.Count > 1 ? ", " : "";
			w.Write("push ");
			for (int i = 0; i < Values.Count; i++)
			{
				Values[i].ToFlashAsm(w);
				w.Write(comma);
				if (i == Values.Count - 2)
				{
					comma = "";
				}
			}
			w.WriteLine("");
		}

		public void ToSwf(SwfWriter w)
		{
            w.AppendByte((byte)ActionKind.Push);
            w.AppendUI16(Length - 3); // don't incude def byte and len

            for (int i = 0; i < Values.Count; i++)
            {
                Values[i].ToSwf(w);
            }
		}

		public void Dump(IndentedTextWriter w)
		{
			if (Values.Count > 1)
			{
				w.WriteLine("Push:");
				w.Indent++;

				for (int i = 0; i < Values.Count; i++)
				{
					Values[i].Dump(w);
				}

				w.Indent--;
			}
			else
			{
				w.Write("Push: "); 
				Values[0].Dump(w);
			}
		}
	}
}
