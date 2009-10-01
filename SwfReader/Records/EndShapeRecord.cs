/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public struct EndShapeRecord : IShapeRecord
	{
		/*
			ENDSHAPERECORD
			Field		Type	Comment
			TypeFlag	UB[1]	Non-edge record flag. Always 0.
			EndOfShape	UB[5]	End of shape flag. Always 0.
		 */

		public void ToSwf(SwfWriter w)
		{
			w.AppendBit(false);
			w.AppendBits(0, 5);
		}
		public void ToSwf(SwfWriter w, ref uint fillBits, ref uint lineBits, ShapeType shapeType)
		{
			w.AppendBit(false);
			w.AppendBits(0, 5);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("End Shape");
		}
	}
}
