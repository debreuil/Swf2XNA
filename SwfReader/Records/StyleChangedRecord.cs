/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public class StyleChangedRecord : IShapeRecord
	{
		/*
			STYLECHANGERECORD
			Field				Type					Comment
			TypeFlag			UB[1]					Non-edge record flag. Always 0.
			StateNewStyles		UB[1]					New styles flag. Used by DefineShape2 and DefineShape3 only.
			StateLineStyle		UB[1]					Line style change flag.
			StateFillStyle1		UB[1]					Fill style 1 change flag.
			StateFillStyle0		UB[1]					Fill style 0 change flag.
			StateMoveTo			UB[1]					Move to flag.		 
			MoveBits			If StateMoveTo 			Move bit count.	
									UB[5]	 
			MoveDeltaX			If StateMoveTo			Delta X value.
									SB[MoveBits]			
			MoveDeltaY			If StateMoveTo			Delta Y value.
									SB[MoveBits]			
			FillStyle1			If StateFillStyle1		Fill 1 Style.
									UB[FillBits]			
			FillStyle0			If StateFillStyle0		Fill 0 Style.
									UB[FillBits]			
			LineStyle			If StateLineStyle		Line Style.
									UB[LineBits]			
			FillStyles			If StateNewStyles		Array of new fill styles.
									FILLSTYLEARRAY			
			LineStyles			If StateNewStyles		Array of new line styles.
									LINESTYLEARRAY			
			NumFillBits			If StateNewStyles		Number of fill index bits for new styles.
									UB[4]
			NumLineBits			If StateNewStyles		Number of line index bits for new styles.
									UB[4]		
		 */

		public int  MoveDeltaX;
		public int  MoveDeltaY;
		public uint FillStyle1;
		public uint FillStyle0;
		public uint LineStyle;
		public FillStyleArray FillStyles;
		public LineStyleArray LineStyles;
		public bool HasMove;
		public bool HasFillStyle0;
		public bool HasFillStyle1;
		public bool HasLineStyle;
		public bool HasNewStyles;

		public StyleChangedRecord(SwfReader r, uint flags, ref uint fillBits, ref uint lineBits, ShapeType shapeType)
		{
			MoveDeltaX = 0;
			MoveDeltaY = 0;
			FillStyle1 = 0;
			FillStyle0 = 0;
			LineStyle = 0;

            HasNewStyles    = (flags & 0x10) != 0;
            HasLineStyle    = (flags & 0x08) != 0;
            HasFillStyle1   = (flags & 0x04) != 0;
            HasFillStyle0   = (flags & 0x02) != 0;
            HasMove         = (flags & 0x01) != 0;

            if (shapeType == ShapeType.Glyph) // glyphs really should be a different style changed record imo : )
            {                
                if (HasMove)
                {
                    uint moveBits = r.GetBits(5);
                    this.MoveDeltaX = r.GetSignedNBits(moveBits);
                    this.MoveDeltaY = r.GetSignedNBits(moveBits);
                }

                if (HasFillStyle0)
                {
                    this.FillStyle0 = r.GetBits(fillBits);
                }
            }
            else
            {
                if (HasMove)
                {
                    uint moveBits = r.GetBits(5);
                    this.MoveDeltaX = r.GetSignedNBits(moveBits);
                    this.MoveDeltaY = r.GetSignedNBits(moveBits);
                }
                if (HasFillStyle0)
                {
                    this.FillStyle0 = r.GetBits(fillBits);
                }

                if (HasFillStyle1)
                {
                    this.FillStyle1 = r.GetBits(fillBits);
                }

                if (HasLineStyle)
                {
                    this.LineStyle = r.GetBits(lineBits);
                }

                //r.Align();

                if (HasNewStyles)
                {
                    FillStyles = new FillStyleArray(r, shapeType);
                    LineStyles = new LineStyleArray(r, shapeType);
                    fillBits = r.GetBits(4);
                    lineBits = r.GetBits(4);
                }
                else
                {
                    FillStyles = new FillStyleArray();
                    LineStyles = new LineStyleArray();
                    HasNewStyles = false;
                }
            }
		}

		public void ToSwf(SwfWriter w)
		{
			throw new NotSupportedException("Use the overload: ToSwf(SwfWriter w, uint fillBits, uint lineBits, ShapeType shapeType)");
		}
		public void ToSwf(SwfWriter w, ref uint fillBits, ref uint lineBits, ShapeType shapeType)
		{
			// TypeFlag				UB[1]	Non-edge record flag. Always 0.
			// StateNewStyles		UB[1]	New styles flag. Used by DefineShape2 and DefineShape3 only.
			// StateLineStyle		UB[1]	Line style change flag.
			// StateFillStyle1		UB[1]	Fill style 1 change flag.
			// StateFillStyle0		UB[1]	Fill style 0 change flag.
			// StateMoveTo			UB[1]	Move to flag.	

            if (shapeType == ShapeType.Glyph)
            {
                w.AppendBit(false);
                w.AppendBit(false);//this.HasNewStyles);

                w.AppendBit(false);//HasLineStyle);
                w.AppendBit(false);//HasFillStyle1);
                w.AppendBit(HasFillStyle0);
                w.AppendBit(HasMove);//HasMove);

                if (HasMove)
                {
                    // not relative moves
                    uint bits = SwfWriter.MinimumBits(this.MoveDeltaX, this.MoveDeltaY);
                    w.AppendBits(bits, 5);
                    w.AppendSignedNBits(this.MoveDeltaX, bits);
                    w.AppendSignedNBits(this.MoveDeltaY, bits);
                }

                if (HasFillStyle0)
                {
                    w.AppendBits(this.FillStyle0, fillBits);
                }
            }
            else
            {
                w.AppendBit(false);
                w.AppendBit(this.HasNewStyles);

                w.AppendBit(HasLineStyle);
                w.AppendBit(HasFillStyle1);
                w.AppendBit(HasFillStyle0);
                w.AppendBit(HasMove);

                if (HasMove)
                {
                    uint bits = SwfWriter.MinimumBits(this.MoveDeltaX, this.MoveDeltaY);
                    w.AppendBits(bits, 5);
                    w.AppendSignedNBits(this.MoveDeltaX, bits);
                    w.AppendSignedNBits(this.MoveDeltaY, bits);
                }

                if (HasFillStyle0)
                {
                    w.AppendBits(this.FillStyle0, fillBits);
                }

                if (HasFillStyle1)
                {
                    w.AppendBits(this.FillStyle1, fillBits);
                }

                if (HasLineStyle)
                {
                    w.AppendBits(this.LineStyle, lineBits);
                }

                if (HasNewStyles)
                {
                    w.Align();
                    FillStyles.ToSwf(w, shapeType);
                    LineStyles.ToSwf(w, shapeType);

                    fillBits = SwfWriter.MinimumBits((uint)FillStyles.FillStyles.Count);
                    lineBits = SwfWriter.MinimumBits((uint)LineStyles.LineStyles.Count); ;

                    w.AppendBits(fillBits, 4);
                    w.AppendBits(lineBits, 4);
                }
            }
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("Style Change: dx:" + MoveDeltaX + " dy:" + MoveDeltaY + " fs0:" + FillStyle0 + " fs1:" + FillStyle1 + " ls: " + LineStyle);

            // dump new styles, if any. Thanks to Antti Huovilainen for pointing out this bug : )
            if (HasNewStyles)
            {
                w.WriteLine("");
                w.WriteLine("New Styles:");
                if (FillStyles != null)
                {
                    FillStyles.Dump(w);
                }
                if (LineStyles != null)
                {
                    LineStyles.Dump(w);
                }
            }
		}
	}
}
