/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public class ActionRecords : IActionContainer
	{
		private bool IsInitActions = false;
		private uint InitTarget;

		private uint codeSize;
		public uint CodeSize { get { return codeSize; } set { codeSize = value; } }
		private List<IAction> statements = new List<IAction>();
		public List<IAction> Statements { get { return statements; } }

		private Stack<IActionContainer> scopes = new Stack<IActionContainer>();
		private Stack<uint> scopeBounds = new Stack<uint>();

		private List<IAction> curStatements;
		private IActionContainer curScope;
		private uint curBounds = uint.MaxValue;


		/// <summary>The byte index, and which line number it represents (by its index).</summary>
		public SortedList<uint, uint> LineNumbers = new SortedList<uint, uint>();
		/// <summary>Which lines require labels (as they would be the target of jumps etc).</summary>
		public List<uint> LabeledLines = new List<uint>();
		private uint lineIndex = 1; // lines start at one

		public static ConstantPool CurrentConstantPool;
		public static SortedList<uint, uint> CurrentLineNumbers;
		public static List<uint> CurrentLabeledLines;

		public ActionRecords()
		{
		}
		public ActionRecords(SwfReader r, uint tagLen) : this(r, tagLen, false)
		{
		}
		public ActionRecords(SwfReader r, uint tagLen, bool isInitTag)
		{
			this.IsInitActions = isInitTag;
			if (isInitTag)
			{
				InitTarget = r.GetUI16();
			}

			CodeSize = tagLen;
			uint codeEnd = r.Position + tagLen; // clip events dont have an end tag

			AddScope(this, r);
			AddLineNumber(r.Position);

			ActionKind actionCode = (ActionKind)0xFF;
            bool hitEnd = false;
			while (r.Position < codeEnd && !hitEnd) //(actionCode != ActionKind.End))
			{
				if (r.Position >= curBounds)
				{
					RemoveScope();
				}
				actionCode = (ActionKind)r.GetByte();
				uint len = 0;
				if (((byte)actionCode & 0x80) > 0)
				{
					len = r.GetUI16();
				}
				AddLineNumber(r.Position + len);

				switch (actionCode)
				{
					case ActionKind.Add:
						curStatements.Add(new Add());
						break;
					case ActionKind.Add2:
						curStatements.Add(new Add2());
						break;
					case ActionKind.And:
						curStatements.Add(new And());
						break;
					case ActionKind.AsciiToChar:
						curStatements.Add(new AsciiToChar());
						break;
					case ActionKind.BitAnd:
						curStatements.Add(new BitAnd());
						break;
					case ActionKind.BitLShift:
						curStatements.Add(new BitLShift());
						break;
					case ActionKind.BitOr:
						curStatements.Add(new BitOr());
						break;
					case ActionKind.BitRShift:
						curStatements.Add(new BitRShift());
						break;
					case ActionKind.BitURShift:
						curStatements.Add(new BitURShift());
						break;
					case ActionKind.BitXor:
						curStatements.Add(new BitXor());
						break;
					case ActionKind.Call:
						curStatements.Add(new Call());
						break;
					case ActionKind.CallFunction:
						curStatements.Add(new CallFunction());
						break;
					case ActionKind.CallMethod:
						curStatements.Add(new CallMethod());
						break;
					case ActionKind.CastOp:
						curStatements.Add(new CastOp());
						break;
					case ActionKind.CharToAscii:
						curStatements.Add(new CharToAscii());
						break;
					case ActionKind.CloneSprite:
						curStatements.Add(new CloneSprite());
						break;
					case ActionKind.ConstantPool:
						ConstantPool cp = new ConstantPool(r); // todo: will need stack for cp
						CurrentConstantPool = cp;
						curStatements.Add(cp);
						break;
					case ActionKind.Decrement:
						curStatements.Add(new Decrement());
						break;
					case ActionKind.DefineFunction:
						DefineFunction df = new DefineFunction(r, CurrentConstantPool);
						curStatements.Add(df);
						AddScope(df, r);
						AddLineLabel((uint)(r.Position + df.CodeSize));
						break;
					case ActionKind.DefineFunction2:
						DefineFunction2 df2 = new DefineFunction2(r, CurrentConstantPool);
						curStatements.Add(df2);
						AddScope(df2, r);
						AddLineLabel((uint)(r.Position + df2.CodeSize));
						break;
					case ActionKind.DefineLocal:
						curStatements.Add(new DefineLocal());
						break;
					case ActionKind.DefineLocal2:
						curStatements.Add(new DefineLocal2());
						break;
					case ActionKind.Delete:
						curStatements.Add(new Delete());
						break;
					case ActionKind.Delete2:
						curStatements.Add(new Delete2());
						break;
					case ActionKind.Divide:
						curStatements.Add(new Divide());
						break;
					case ActionKind.End:
						curStatements.Add(new End());
                        hitEnd = true;
						break;
					case ActionKind.EndDrag:
						curStatements.Add(new EndDrag());
						break;
					case ActionKind.Enumerate:
						curStatements.Add(new Enumerate());
						break;
					case ActionKind.Enumerate2:
						curStatements.Add(new Enumerate2());
						break;
					case ActionKind.Equals:
						curStatements.Add(new Equals());
						break;
					case ActionKind.Equals2:
						curStatements.Add(new Equals2());
						break;
					case ActionKind.Extends:
						curStatements.Add(new Extends());
						break;
					case ActionKind.GetMember:
						curStatements.Add(new GetMember());
						break;
					case ActionKind.GetProperty:
						curStatements.Add(new GetProperty());
						break;
					case ActionKind.GetTime:
						curStatements.Add(new GetTime());
						break;
					case ActionKind.GetURL:
						curStatements.Add(new GetURL(r));
						break;
					case ActionKind.GetURL2:
						curStatements.Add(new GetURL2(r));
						break;
					case ActionKind.GetVariable:
						curStatements.Add(new GetVariable());
						break;
					case ActionKind.GoToLabel:
						curStatements.Add(new GoToLabel(r));
						break;
					case ActionKind.GotoFrame:
						curStatements.Add(new GotoFrame(r));
						break;
					case ActionKind.GotoFrame2:
						curStatements.Add(new GotoFrame2(r));
						break;
					case ActionKind.Greater:
						curStatements.Add(new Greater());
						break;
					case ActionKind.If:
						If ifTag = new If(r);
						curStatements.Add(ifTag);
						AddLineLabel((uint)(r.Position + ifTag.BranchOffset));
						break;
					case ActionKind.ImplementsOp:
						curStatements.Add(new ImplementsOp());
						break;
					case ActionKind.Increment:
						curStatements.Add(new Increment());
						break;
					case ActionKind.InitArray:
						curStatements.Add(new InitArray());
						break;
					case ActionKind.InitObject:
						curStatements.Add(new InitObject());
						break;
					case ActionKind.InstanceOf:
						curStatements.Add(new InstanceOf());
						break;
					case ActionKind.Jump:
						Jump jump = new Jump(r);
						curStatements.Add(jump);
						AddLineLabel((uint)(r.Position + jump.BranchOffset));
						break;
					case ActionKind.Less:
						curStatements.Add(new Less());
						break;
					case ActionKind.Less2:
						curStatements.Add(new Less2());
						break;
					case ActionKind.MBAsciiToChar:
						curStatements.Add(new MBAsciiToChar());
						break;
					case ActionKind.MBCharToAscii:
						curStatements.Add(new MBCharToAscii());
						break;
					case ActionKind.MBStringExtract:
						curStatements.Add(new MBStringExtract());
						break;
					case ActionKind.MBStringLength:
						curStatements.Add(new MBStringLength());
						break;
					case ActionKind.Modulo:
						curStatements.Add(new Modulo());
						break;
					case ActionKind.Multiply:
						curStatements.Add(new Multiply());
						break;
					case ActionKind.NewMethod:
						curStatements.Add(new NewMethod());
						break;
					case ActionKind.NewObject:
						curStatements.Add(new NewObject());
						break;
					case ActionKind.NextFrame:
						curStatements.Add(new NextFrame());
						break;
					case ActionKind.Not:
						curStatements.Add(new Not());
						break;
					case ActionKind.Or:
						curStatements.Add(new Or());
						break;
					case ActionKind.Play:
						curStatements.Add(new Play());
						break;
					case ActionKind.Pop:
						curStatements.Add(new Pop());
						break;
					case ActionKind.PreviousFrame:
						curStatements.Add(new PreviousFrame());
						break;
					case ActionKind.Push:
						curStatements.Add(new Push(r, len + r.Position));
						break;
					case ActionKind.PushDuplicate:
						curStatements.Add(new PushDuplicate());
						break;
					case ActionKind.RandomNumber:
						curStatements.Add(new RandomNumber());
						break;
					case ActionKind.RemoveSprite:
						curStatements.Add(new RemoveSprite());
						break;
					case ActionKind.Return:
						curStatements.Add(new Return());
						break;
					case ActionKind.SetMember:
						curStatements.Add(new SetMember());
						break;
					case ActionKind.SetProperty:
						curStatements.Add(new SetProperty());
						break;
					case ActionKind.SetTarget:
						curStatements.Add(new SetTarget(r));
						break;
					case ActionKind.SetTarget2:
						curStatements.Add(new SetTarget2());
						break;
					case ActionKind.SetVariable:
						curStatements.Add(new SetVariable());
						break;
					case ActionKind.StackSwap:
						curStatements.Add(new StackSwap());
						break;
					case ActionKind.StartDrag:
						curStatements.Add(new StartDrag());
						break;
					case ActionKind.Stop:
						curStatements.Add(new Stop());
						break;
					case ActionKind.StopSounds:
						curStatements.Add(new StopSounds());
						break;
					case ActionKind.StoreRegister:
						curStatements.Add(new StoreRegister(r));
						break;
					case ActionKind.StrictEquals:
						curStatements.Add(new StrictEquals());
						break;
					case ActionKind.StringAdd:
						curStatements.Add(new StringAdd());
						break;
					case ActionKind.StringEquals:
						curStatements.Add(new StringEquals());
						break;
					case ActionKind.StringExtract:
						curStatements.Add(new StringExtract());
						break;
					case ActionKind.StringGreater:
						curStatements.Add(new StringGreater());
						break;
					case ActionKind.StringLength:
						curStatements.Add(new StringLength());
						break;
					case ActionKind.StringLess:
						curStatements.Add(new StringLess());
						break;
					case ActionKind.Subtract:
						curStatements.Add(new Subtract());
						break;
					case ActionKind.TargetPath:
						curStatements.Add(new TargetPath());
						break;
					case ActionKind.Throw:
						//					curStatements.Add(new Throw(r));
						break;
					case ActionKind.ToInteger:
						curStatements.Add(new ToInteger());
						break;
					case ActionKind.ToNumber:
						curStatements.Add(new ToNumber());
						break;
					case ActionKind.ToString:
						curStatements.Add(new ToString());
						break;
					case ActionKind.ToggleQuality:
						curStatements.Add(new ToggleQuality());
						break;
					case ActionKind.Trace:
						curStatements.Add(new Trace());
						break;
					case ActionKind.Try:
						//					curStatements.Add(new Try(r));
						break;
					case ActionKind.TypeOf:
						curStatements.Add(new TypeOf());
						break;
					case ActionKind.WaitForFrame:
						curStatements.Add(new WaitForFrame(r));
						break;
					case ActionKind.WaitForFrame2:
						curStatements.Add(new WaitForFrame2(r));
						break;
					case ActionKind.With:
						//					curStatements.Add(new With(r));
						break;
				}
			}
			GenerateLabels();
		}

		private void GenerateLabels()
		{
		}

		private void AddScope(IActionContainer scope, SwfReader r)
		{
			scopes.Push(scope);
			scopeBounds.Push(r.Position + scope.CodeSize);
			curScope = scope;
			curBounds = r.Position + scope.CodeSize;
			curStatements = scope.Statements;
		}

		private void RemoveScope()
		{
			scopes.Pop();
			scopeBounds.Pop();
			curScope = scopes.Peek();
			curBounds = scopeBounds.Peek();
			curStatements = scopes.Peek().Statements;
		}

		private void AddLineNumber(uint linePos)
		{
			if (!LineNumbers.ContainsKey(linePos))
			{
				LineNumbers.Add(linePos, lineIndex++);
			}
			else // found a inserted target, need to give it a line number and add that to labeled line numbers
			{
				LineNumbers[linePos] = lineIndex;
				LabeledLines.Add(lineIndex);
				lineIndex++;
			}
		}
		private void AddLineLabel(uint pos)
		{
			if (!LineNumbers.ContainsKey(pos))
			{
				LineNumbers.Add(pos, 0); // don't know the line number for future lines
			}
			else
			{
				if (LineNumbers[pos] == 0)
				{
					// this is a duplicate forward jump, we can ignore it
				}
				else
				{
					// this is a back jump, so life is easy
					LabeledLines.Add(LineNumbers[pos]);
				}
			}
		}

		private const int codeIndent = 2;
		private static uint curLine = 1;
		public static void AutoLineLabel(IndentedTextWriter w)
		{
			if (CurrentLabeledLines.Contains(curLine))
			{
				// labeled line
				string label = "@" + curLine;
				string spaces = new string(' ', w.Indent * 4 - label.Length);
				w.Indent -= codeIndent;
				w.Write(label);
				w.Write(spaces);
				w.Indent += codeIndent;
			}
			curLine++;
		}
		public static string GetLabel(int offset)
		{
			string result;
			uint lineStart = curLine - 1;
			uint lineStartPos = CurrentLineNumbers.Keys[(int)lineStart];
			uint targetPos = (uint)(lineStartPos + offset);
			uint targetLine = CurrentLineNumbers[targetPos];
			if (CurrentLabeledLines.Contains(targetLine))
			{
				result = "@" + targetLine;
			}
			else
			{
				// error...
				result = "@xxx";
			}
			return result;
		}

		public static IAction CurrentDumpStatement;
		public void ToSwf(SwfWriter w)
        {
            if (IsInitActions)
            {
                w.AppendUI16(InitTarget);
            }

            CurrentLineNumbers = this.LineNumbers;
            CurrentLabeledLines = this.LabeledLines;

            for (int i = 0; i < this.curStatements.Count; i++)
            {
                //CurrentDumpStatement = this.curStatements[i]; // for register resolving
                this.curStatements[i].ToSwf(w);
            }
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("ActionRecords: ");

			if (IsInitActions)
			{
				w.WriteLine("#initclip");
			}

			curLine = 1;
			CurrentLineNumbers = this.LineNumbers;
			CurrentLabeledLines = this.LabeledLines;

			w.Indent += codeIndent;
			for (int i = 0; i < this.curStatements.Count; i++)
			{
				AutoLineLabel(w);
				CurrentDumpStatement = this.curStatements[i]; // for register resolving
				//this.curStatements[i].Dump(w);
				this.curStatements[i].ToFlashAsm(w);
			}
			w.WriteLine("");

			if (IsInitActions)
			{
				w.WriteLine("#endinitclip");
			}

			w.Indent -= codeIndent;
		}
	}
}
