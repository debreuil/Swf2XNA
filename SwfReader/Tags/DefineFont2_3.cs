/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class DefineFont2_3 : ISwfTag
	{
		/*
			Header					RECORDHEADER					Tag type = 75.
			FontID					UI16							ID for this font character.
			FontFlagsHasLayout		UB[1]							Has font metrics/layout information.
			FontFlagsShiftJIS		UB[1]							ShiftJIS encoding.
			FontFlagsSmallText		UB[1]							SWF 7 or later: Font is small. Character glyphs are aligned on pixel boundaries for dynamic and input text.
			FontFlagsANSI			UB[1]							ANSI encoding.
			FontFlagsWideOffsets	UB[1]							If 1, uses 32 bit offsets.
			FontFlagsWideCodes		UB[1]							Must be 1.
			FontFlagsItalic			UB[1]							Italic Font.
			FontFlagsBold			UB[1]							Bold Font.
			LanguageCode			LANGCODE (U8)					SWF 5 or earlier: always 0 SWF 6 or later: language code
			FontNameLen				UI8								Length of name.
			FontName				UI8[FontNameLen]				Name of font (see DefineFontInfo).
			NumGlyphs				UI16							Count of glyphs in font. May be zero for device fonts.
			OffsetTable				If FontFlagsWideOffsets			Same as in DefineFont.
									UI32[NumGlyphs]
									Otherwise UI16[NumGlyphs]
			CodeTableOffset			If FontFlagsWideOffsets			Byte count from start of OffsetTable to start of CodeTable.
									UI32							
									Otherwise UI16					
			GlyphShapeTable			SHAPE[NumGlyphs]				Same as in DefineFont.
			CodeTable				UI16[NumGlyphs]					Sorted in ascending order. Always UCS-2 in SWF 6 or later.
			FontAscent				If FontFlagsHasLayout SI16		Font ascender height.
			FontDescent				If FontFlagsHasLayout SI16		Font descender height.
			FontLeading				If FontFlagsHasLayout SI16		Font leading height (see following).
			FontAdvanceTable		If FontFlagsHasLayout			Advance value to be used for each glyph in dynamic glyph text.	
									SI16[NumGlyphs]					
			FontBoundsTable			If FontFlagsHasLayout			Not used in Flash Player through version 7 (but must be present).
									RECT[NumGlyphs]					
			KerningCount			If FontFlagsHasLayout UI16		Not used in Flash Player through version 7 (always set to 0 to save space).
			FontKerningTable		If FontFlagsHasLayout			Not used in Flash Player through version 7 (omit with KerningCount of 0).
									KERNINGRECORD[KerningCount]
		*/

		private  TagType tagType = TagType.DefineFont2; // may be 3
		public TagType TagType { get { return tagType; } }
			
		public uint FontId;
		public bool FontFlagsHasLayout;	
		public bool FontFlagsShiftJIS;	
		public bool FontFlagsSmallText;	
		public bool FontFlagsANSI;		
		public bool FontFlagsWideOffsets;
		public bool FontFlagsWideCodes;	
		public bool FontFlagsItalic;		
		public bool FontFlagsBold;	
	
		public uint LanguageCode;			
		public string FontName;			
		public uint NumGlyphs;			
		public uint[] OffsetTable;			
		public uint CodeTableOffset;
		public List<Shape> GlyphShapeTable;		
		public uint[] CodeTable;		
		public int FontAscent;			
		public int FontDescent;			
		public int FontLeading;			
		public int[] FontAdvanceTable;	
		public Rect[] FontBoundsTable;		
		public uint KerningCount;	
		public KerningRecord[] FontKerningTable;

		public bool IsHighRes = false;

		public DefineFont2_3(SwfReader r, bool isHighRes)
		{
            this.IsHighRes = isHighRes; // true;
			if (isHighRes)
			{
				this.tagType = TagType.DefineFont3;
			}
			this.FontId = r.GetUI16();
			this.FontFlagsHasLayout = r.GetBit();
			this.FontFlagsShiftJIS = r.GetBit();
			this.FontFlagsSmallText = r.GetBit();
			this.FontFlagsANSI = r.GetBit();
			this.FontFlagsWideOffsets = r.GetBit();
			this.FontFlagsWideCodes = r.GetBit();
			this.FontFlagsItalic = r.GetBit();
			this.FontFlagsBold = r.GetBit();

			r.Align();

			this.LanguageCode = (uint)r.GetByte();
			uint fontNameLen = (uint)r.GetByte();
            this.FontName = r.GetString(fontNameLen);
			this.NumGlyphs = r.GetUI16(); 

			this.OffsetTable = new uint[this.NumGlyphs];
			for (int i = 0; i < this.NumGlyphs; i++)
			{
				this.OffsetTable[i] = this.FontFlagsWideOffsets ? r.GetUI32() : r.GetUI16();
			}

			this.CodeTableOffset = this.FontFlagsWideOffsets ? r.GetUI32() : r.GetUI16();

			GlyphShapeTable = new List<Shape>();
			for (int i = 0; i < this.NumGlyphs; i++)
			{
				Shape s = new Shape(r);
				GlyphShapeTable.Add(s);
			}
			
			this.CodeTable = new uint[this.NumGlyphs];
			for (int i = 0; i < this.NumGlyphs; i++)
			{
				this.CodeTable[i] = r.GetUI16();
			}

			if (this.FontFlagsHasLayout)
			{
				this.FontAscent = r.GetInt16();
				this.FontDescent = r.GetInt16();
				this.FontLeading = r.GetInt16();

				this.FontAdvanceTable = new int[this.NumGlyphs];
				for (int i = 0; i < this.NumGlyphs; i++)
				{
					this.FontAdvanceTable[i] = r.GetInt16();
				}

				this.FontBoundsTable = new Rect[this.NumGlyphs];
				for (int i = 0; i < this.NumGlyphs; i++)
				{
					this.FontBoundsTable[i] = new Rect(r);
				}

				this.KerningCount = r.GetUI16();

				this.FontKerningTable = new KerningRecord[this.KerningCount];
				if(this.FontFlagsWideCodes)
				{
					for (int i = 0; i < this.KerningCount; i++)
					{
						this.FontKerningTable[i] = new KerningRecord(r.GetUI16(), r.GetUI16(), r.GetInt16());
					}
				}
				else
				{
					for (int i = 0; i < this.KerningCount; i++)
					{
						this.FontKerningTable[i] = new KerningRecord((uint)r.GetByte(), (uint)r.GetByte(), r.GetInt16());
					}
				}
			}
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendUI16(FontId);

            w.AppendBit(FontFlagsHasLayout);
            w.AppendBit(FontFlagsShiftJIS);
            w.AppendBit(FontFlagsSmallText);
            w.AppendBit(FontFlagsANSI);
            w.AppendBit(FontFlagsWideOffsets);
            w.AppendBit(FontFlagsWideCodes);
            w.AppendBit(FontFlagsItalic);
            w.AppendBit(FontFlagsBold);
            w.Align();

            w.AppendByte((byte)LanguageCode);
            w.AppendByte((byte)(FontName.Length + 1)); // add trailing /0
            w.AppendString(FontName, (uint)FontName.Length);

            w.AppendUI16(NumGlyphs);
			for (int i = 0; i < this.NumGlyphs; i++)
			{
                if (this.FontFlagsWideOffsets)
                {
                    w.AppendUI32(this.OffsetTable[i]);
                }
                else
                {
                    w.AppendUI16(this.OffsetTable[i]);
                }
			}


            if (this.FontFlagsWideOffsets)
            {
                w.AppendUI32(this.CodeTableOffset);
            }
            else
            {
                w.AppendUI16(this.CodeTableOffset);
            }

			for (int i = 0; i < this.NumGlyphs; i++)
			{
                GlyphShapeTable[i].ToSwf(w);
			}
			
			for (int i = 0; i < this.NumGlyphs; i++)
			{
                w.AppendUI16(this.CodeTable[i]);
			}

			if (this.FontFlagsHasLayout)
            {
                w.AppendInt16(FontAscent);
                w.AppendInt16(FontDescent);
                w.AppendInt16(FontLeading);

				for (int i = 0; i < this.NumGlyphs; i++)
				{
                    w.AppendInt16(this.FontAdvanceTable[i]);
				}

				for (int i = 0; i < this.NumGlyphs; i++)
				{
                    this.FontBoundsTable[i].ToSwf(w);
				}

                w.AppendUI16(this.KerningCount);

				if(this.FontFlagsWideCodes)
				{
					for (int i = 0; i < this.KerningCount; i++)
                    {
                        w.AppendUI16(this.FontKerningTable[i].FontKerningCode1);
                        w.AppendUI16(this.FontKerningTable[i].FontKerningCode2);
                        w.AppendInt16(this.FontKerningTable[i].FontKerningAdjustment);
					}
				}
				else
				{
					for (int i = 0; i < this.KerningCount; i++)
					{
                        w.AppendByte((byte)this.FontKerningTable[i].FontKerningCode1);
                        w.AppendByte((byte)this.FontKerningTable[i].FontKerningCode2);
                        w.AppendInt16(this.FontKerningTable[i].FontKerningAdjustment);
					}
				}
            }

            w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("DefineFont2_3: ");
			w.WriteLine();
		}
	}
}