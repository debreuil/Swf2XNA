/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public struct StraightEdgeRecord : IShapeRecord
	{
		/*
			STRAIGHTEDGERECORD
			Field			Type		Comment
			TypeFlag		UB[1]		This is an edge record. Always 1.
			StraightFlag	UB[1]		Straight edge. Always 1.
			NumBits			UB[4]		Number of bits per value (2 less than the actual number).
		 
			GeneralLineFlag UB[1]		General Line equals 1. 
										Vert/Horz Line equals 0.
		 
			VertLineFlag	If GeneralLineFlag = 0 
							SB[1]		Vertical Line equals 1.
										Horizontal Line equals 0.
		  
			DeltaX			If GeneralLineFlag = 1 or X delta.
							if VertLineFlag = 0
							SB[NumBits+2] 
		  
			DeltaY			If GeneralLineFlag = 1 or Y delta.
							if VertLineFlag = 1
							SB[NumBits+2]			
		 */

		public int DeltaX;
		public int DeltaY;

		public StraightEdgeRecord(int deltaX, int deltaY)
		{
			this.DeltaX = deltaX;
			this.DeltaY = deltaY;
		}

		public StraightEdgeRecord(SwfReader r)
		{
			this.DeltaX = 0;
			this.DeltaY = 0;

			uint nbits = r.GetBits(4) + 2;
			bool isGeneralLine = r.GetBit(); // not vertical or horizontal
			if (isGeneralLine)
			{
				DeltaX = r.GetSignedNBits(nbits);
				DeltaY = r.GetSignedNBits(nbits);
			}
			else
			{
				bool isHorz = r.GetBit();
				if (isHorz)
				{
					DeltaY = r.GetSignedNBits(nbits);
				}
				else
				{
					DeltaX = r.GetSignedNBits(nbits);
				}
			}
		}

		public void ToSwf(SwfWriter w)
		{
			throw new NotSupportedException("Use the overload: ToSwf(SwfWriter w, uint fillBits, uint lineBits, ShapeType shapeType)");
		}
		public void ToSwf(SwfWriter w, ref uint fillBits, ref uint lineBits, ShapeType shapeType)
		{
			w.AppendBit(true);
			w.AppendBit(true);
			uint bits = SwfWriter.MinimumBits(DeltaX, DeltaY);
            bits = bits < 2 ? 2 : bits; // min 2 bits

			w.AppendBits(bits - 2, 4);
			if (DeltaX != 0 && DeltaY != 0)
			{
				w.AppendBit(true);
				w.AppendSignedNBits(DeltaX, bits);
				w.AppendSignedNBits(DeltaY, bits);
			}
			else if (DeltaX == 0)
			{
				w.AppendBit(false);
				w.AppendBit(true);
				w.AppendSignedNBits(DeltaY, bits);
			}
			else
			{
				w.AppendBit(false);
				w.AppendBit(false);
				w.AppendSignedNBits(DeltaX, bits);
			}
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("Line [");
			w.Write("dx: " + this.DeltaX + ", ");
			w.Write("dy: " + this.DeltaY);
			w.Write("]");
		}
		public override string ToString()
		{
			float scale = 400F;
			string s = "Line [";
			s += "dx: " + (this.DeltaX / scale) + ", ";
			s += "dy: " + (this.DeltaY / scale);
			s += "]";
			return s;
		}
	}
}
