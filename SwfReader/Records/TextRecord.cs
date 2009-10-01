/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class TextRecord
	{
		private bool TextRecordType;			// UB[1]	
		private uint StyleFlagsReserved;		// UB[3]
		public bool StyleFlagsHasFont;		// UB[1]
		public bool StyleFlagsHasColor;		// UB[1]
		public bool StyleFlagsHasYOffset;	// UB[1]
		public bool StyleFlagsHasXOffset;	// UB[1]
		public uint FontID;					// If StyleFlagsHasFont UI16
		public RGBA TextColor;				// If StyleFlagsHasColor RGB. If this record is part of a DefineText2 tag then RGBA
		public int XOffset;					// If StyleFlagsHasXOffset SI16
		public int YOffset;					// If StyleFlagsHasYOffset SI16
		public uint TextHeight;				// If hasFont UI16
		public uint GlyphCount;				// UI8
		public GlyphEntry[] GlyphEntries;	// GLYPHENTRY[GlyphCount]

		public TextRecord(SwfReader r,  uint glyphBits, uint advanceBits, bool hasAlpha)
		{
			TextRecordType = r.GetBit();
			StyleFlagsReserved = r.GetBits(3);
			StyleFlagsHasFont = r.GetBit(); 
			StyleFlagsHasColor = r.GetBit(); 
			StyleFlagsHasYOffset = r.GetBit(); 
			StyleFlagsHasXOffset = r.GetBit(); 

			if(StyleFlagsHasFont)
			{
				FontID = r.GetUI16();
			}
			if(StyleFlagsHasColor)
			{
				TextColor = new RGBA(r.GetByte(), r.GetByte(), r.GetByte());
				if(hasAlpha)
				{
					TextColor.A = r.GetByte();
				}
			}
			if(StyleFlagsHasXOffset)
			{
				XOffset = r.GetInt16();
			}
			if(StyleFlagsHasYOffset)
			{
				YOffset = r.GetInt16();
			}
			if(StyleFlagsHasFont)
			{
				TextHeight = r.GetUI16();
			}

			GlyphCount = (uint)r.GetByte();
			GlyphEntries = new GlyphEntry[GlyphCount];
			for (int i = 0; i < GlyphCount; i++)
			{
				uint index = r.GetBits(glyphBits);
				int advance = r.GetSignedNBits(advanceBits);
				GlyphEntries[i] = new GlyphEntry(index, advance);
			}
            r.Align();//
		}

        public void ToSwf(SwfWriter w, uint glyphBits, uint advanceBits, bool hasAlpha)
        {
            w.AppendBit(TextRecordType);
            w.AppendBits(StyleFlagsReserved, 3);
            w.AppendBit(StyleFlagsHasFont);
            w.AppendBit(StyleFlagsHasColor);
            w.AppendBit(StyleFlagsHasYOffset);
            w.AppendBit(StyleFlagsHasXOffset);
            w.Align();

            if (StyleFlagsHasFont)
            {
                w.AppendUI16(FontID);
            }
            if (StyleFlagsHasColor)
            {
                w.AppendByte(TextColor.R);
                w.AppendByte(TextColor.G);
                w.AppendByte(TextColor.B);
                if (hasAlpha)
                {
                    w.AppendByte(TextColor.A);
                }
            }
            if (StyleFlagsHasXOffset)
            {
                w.AppendInt16(XOffset);
            }
            if (StyleFlagsHasYOffset)
            {
                w.AppendInt16(YOffset);
            }
            if (StyleFlagsHasFont)
            {
                w.AppendUI16(TextHeight);
            }

            w.AppendByte((byte)GlyphEntries.Length);

            for (int i = 0; i < GlyphEntries.Length; i++)
            {
                w.AppendBits(GlyphEntries[i].GlyphIndex, glyphBits);
                w.AppendSignedNBits(GlyphEntries[i].GlyphAdvance, advanceBits);
            }
            w.Align();
        }
	}
}














