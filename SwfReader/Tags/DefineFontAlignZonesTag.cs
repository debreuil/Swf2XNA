/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class DefineFontAlignZonesTag : ISwfTag
	{
		/*
			Header			RECORDHEADER	Tag type = 73.
			FontID			UI16			ID of font to use, specified by DefineFont3.
			CSMTableHint	UB[2]			Font thickness hint. Refers to the thickness of the typical stroke used in the font.
											0 = thin
											1 = medium
											2 = thick 
											Flash Player maintains a selection of CSM tables for many fonts. 
											However, if the font is not found in Flash Player's internal table, 
											this hint is used to choose an appropriate table. 
			Reserved		UB[6]			Must be 0.
			ZoneTable		ZONERECORD		Alignment zone information for each glyph.
							[GlyphCount]	
		 */

		public const TagType tagType = TagType.DefineFontAlignZones;
		public TagType TagType { get { return tagType; } }

		public uint FontId;
		public uint CSMTableHint;
		public ZoneRecord[] ZoneTable;
        private Dictionary<uint, DefineFont2_3> Fonts;

		public DefineFontAlignZonesTag(SwfReader r, Dictionary<uint, DefineFont2_3> fonts)
		{
            Fonts = fonts;

			FontId = r.GetUI16();
			CSMTableHint = r.GetBits(2);
			r.SkipBits(6);
			r.Align();

			DefineFont2_3 font = Fonts[FontId];
			uint glyphCount = font.NumGlyphs;

			ZoneTable = new ZoneRecord[glyphCount];
			for (int i = 0; i < glyphCount; i++)
			{
				ZoneTable[i] = new ZoneRecord(r);
			}
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendUI16(FontId);
            w.AppendBits(CSMTableHint, 2);
            w.AppendBits(0, 6);
            w.Align();

			DefineFont2_3 font = Fonts[FontId];
			uint glyphCount = font.NumGlyphs;

			for (int i = 0; i < glyphCount; i++)
			{
				ZoneTable[i].ToSwf(w);
			}

            w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("DefineFontAlignZones: ");
			w.WriteLine();
		}
	}
}

