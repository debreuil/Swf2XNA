/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class DefineFunction2 : IAction, IActionContainer
	{
		/*
			ActionDefineFunction2	UI8
			FunctionName			STRING
			NumParams				UI16
			RegisterCount			UI8
			PreloadParentFlag		UB[1]
			PreloadRootFlag			UB[1]
			SuppressSuperFlag		UB[1]
			PreloadSuperFlag		UB[1]
			SuppressArgumentsFlag	UB[1]
			PreloadArgumentsFlag	UB[1]
			SuppressThisFlag		UB[1]
			PreloadThisFlag			UB[1]
			Reserved				UB[7]
			PreloadGlobalFlag		UB[1]
			Parameters				REGISTERPARAM[NumParams]
			codeSize				UI16
			Register				UI8
			ParamName				STRING
		 */
		public ActionKind ActionId { get { return ActionKind.DefineFunction; } }
		public uint Version { get { return 7; } }
       public uint Length
       {
           get
           {
               uint len = 3;

               len += 7; // flags etc

               len += (uint)FunctionName.Length + 1;

               foreach (KeyValuePair<uint, string> d in Parameters)
               {
                  len += (uint)d.Value.Length + 1 + 1;
               }

               return len;
           }
       }

		public uint StackPops { get { return 0; } }
		public uint StackPushes { get { return 0; } }
		public int StackChange { get { return 0; } }

		private uint codeSize;
		public uint CodeSize { get { return codeSize; } set { codeSize = value; } }
		private List<IAction> statements = new List<IAction>();
		public List<IAction> Statements { get { return statements; } }

		public string FunctionName;
		public uint RegisterCount;
		public PreloadFlags Preloads;

		//public bool PreloadParentFlag;
		//public bool PreloadRootFlag;
		public bool SuppressSuperFlag;
		//public bool PreloadSuperFlag;
		public bool SuppressArgumentsFlag;
		//public bool PreloadArgumentsFlag;
		public bool SuppressThisFlag;
		//public bool PreloadThisFlag;
		//public bool PreloadGlobalFlag;

		public Dictionary<uint, string> Parameters = new Dictionary<uint,string>(); //uint Register, string ParamName;

		private ConstantPool cp;

		public DefineFunction2(SwfReader r, ConstantPool cp)
		{
			this.cp = cp;
			FunctionName = r.GetString();
			uint paramCount = r.GetUI16();
			RegisterCount = (uint)r.GetByte();

			Preloads = PreloadFlags.Empty;
			if (r.GetBit())
			{
				Preloads |= PreloadFlags.Parent;
			}
			if (r.GetBit())
			{
				Preloads |= PreloadFlags.Root;
			}

			SuppressSuperFlag = r.GetBit();

			if (r.GetBit())
			{
				Preloads |= PreloadFlags.Super;
			}

			SuppressArgumentsFlag = r.GetBit();

			if (r.GetBit())
			{
				Preloads |= PreloadFlags.Arguments;
			}

			SuppressThisFlag = r.GetBit();

			if (r.GetBit())
			{
				Preloads |= PreloadFlags.This;
			}

			r.GetBits(7); // reserved

			if (r.GetBit())
			{
				Preloads |= PreloadFlags.Global;
			}

			for (int i = 0; i < paramCount; i++)
			{
				uint reg = r.GetByte();
				string name = r.GetString();
				Parameters.Add(reg, name);
			}
			CodeSize = r.GetUI16();
		}

		public void ToFlashAsm(IndentedTextWriter w)
		{
			w.Write("function2 '" + FunctionName + "' ");
			string comma = "";

			if (Parameters.Count > 0)
			{
				w.Write("(");
				foreach (KeyValuePair<uint, string> p in Parameters)
				{
					w.Write(comma + "r:" + p.Key + "='" + p.Value + "'");
					comma = ", ";
				}
				w.Write(") ");
			}

			if (Preloads != PreloadFlags.Empty)
			{
				// in the order: this, arguments, super, _root, _parent, _global
				uint regIndex = 1; 
				comma = "";

				w.Write("(");
				int pl = (int)Preloads;
				for (int i = 1; i < (int)PreloadFlags.End; i*=2)
				{
					if((i & pl) != 0)
					{
						w.Write(comma + "r:" + regIndex++ + "=" + PreloadFlagToString( (PreloadFlags)i ) + " ");
						comma = ", ";
					}
				}
				//if ((PreloadFlags)(Preloads & PreloadFlags.This) != PreloadFlags.Empty)
				//{
				//    w.Write(comma + "r:" + regIndex++ + "=this ");
				//    comma = ", ";
				//}
				//if ((PreloadFlags)(Preloads & PreloadFlags.Arguments) != PreloadFlags.Empty)
				//{
				//    w.Write(comma + "r:" + regIndex++ + "=arguments ");
				//    comma = ", ";
				//}
				//if ((PreloadFlags)(Preloads & PreloadFlags.Super) != PreloadFlags.Empty)
				//{
				//    w.Write(comma + "r:" + regIndex++ + "=super ");
				//    comma = ", ";
				//}
				//if ((PreloadFlags)(Preloads & PreloadFlags.Root) != PreloadFlags.Empty)
				//{
				//    w.Write(comma + "r:" + regIndex++ + "=_root ");
				//    comma = ", ";
				//}
				//if ((PreloadFlags)(Preloads & PreloadFlags.Parent) != PreloadFlags.Empty)
				//{
				//    w.Write(comma + "r:" + regIndex++ + "=_parent ");
				//    comma = ", ";
				//}
				//if ((PreloadFlags)(Preloads & PreloadFlags.Global) != PreloadFlags.Empty)
				//{
				//    w.Write(comma + "r:" + regIndex++ + "=_global ");
				//    comma = ", ";
				//}
				w.Write(") ");
			}

			w.Write(ActionRecords.GetLabel((int)CodeSize) + " ");
			w.WriteLine("maxreg=" + RegisterCount);

			ActionRecords.CurrentConstantPool = cp;
			for (int i = 0; i < Statements.Count; i++)
			{
				ActionRecords.AutoLineLabel(w);
				Statements[i].ToFlashAsm(w);
			}
		}
		
		public static string PreloadFlagToString(PreloadFlags flag)
		{
			string result;
			switch (flag)
			{
				case PreloadFlags.This:
					result = "this";
					break;
				case PreloadFlags.Arguments:
					result = "arguments";
					break;
				case PreloadFlags.Super:
					result = "super";
					break;
				case PreloadFlags.Root:
					result = "_root";
					break;
				case PreloadFlags.Parent:
					result = "_parent";
					break;
				case PreloadFlags.Global:
					result = "_global";
					break;
				default:
					result = "";
					break;
			}
			return result;
		}
		public void ToSwf(SwfWriter w)
        {
            w.AppendByte((byte)ActionKind.DefineFunction2);
            w.AppendUI16(Length - 3); // don't incude def byte and len

            w.AppendString(FunctionName);
            w.AppendUI16((uint)Parameters.Count);
            w.AppendByte((byte)RegisterCount);

            w.AppendBit((Preloads & PreloadFlags.Parent) > 0);
            w.AppendBit((Preloads & PreloadFlags.Root) > 0);
            w.AppendBit(SuppressSuperFlag);
            w.AppendBit((Preloads & PreloadFlags.Super) > 0);
            w.AppendBit(SuppressArgumentsFlag);
            w.AppendBit((Preloads & PreloadFlags.Arguments) > 0);
            w.AppendBit(SuppressThisFlag);
            w.AppendBit((Preloads & PreloadFlags.This) > 0);
            w.AppendBits(0, 7);
            w.AppendBit((Preloads & PreloadFlags.Global) > 0);

            foreach (KeyValuePair<uint, string> d in Parameters)
            {
                w.AppendByte((byte)d.Key);
                w.AppendString(d.Value);
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
