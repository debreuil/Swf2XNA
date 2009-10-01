/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class DefineFontInfoTag : ISwfTag
	{
		/*
			Header					RECORDHEADER			Tag type = 13.
			FontID					UI16					Font ID this information is for.
			FontNameLen				UI8						Length of font name.
			FontName				UI8[FontNameLen]		Name of the font.
			FontFlagsReserved		UB[2]					Reserved bit fields.
			FontFlagsSmallText		UB[1]					SWF 7 file format or later: Font is small. 
															Character glyphs are aligned on pixel boundaries for dynamic and input text.
			FontFlagsShiftJIS		UB[1]					ShiftJIS character codes.
			FontFlagsANSI			UB[1]					ANSI character codes.
			FontFlagsItalic			UB[1]					Font is italic.
			FontFlagsBold			UB[1]					Font is bold.
			FontFlagsWideCodes		UB[1]					If 1, CodeTable is UI16 array; otherwise, CodeTable is UI8 array.
			CodeTable				If FontFlagsWideCodes	Glyph to code table, sorted in ascending order.
									UI16[nGlyphs]
									Otherwise UI8[nGlyphs]						
		 */

		public const TagType tagType = TagType.DefineFontInfo;
		public TagType TagType { get { return tagType; } }

		public uint FontId;
		public string FontName;
		public bool IsSmallText;
		public bool IsShiftJis;
		public bool IsAnsi;
		public bool IsItalic;
		public bool IsBold;
		public bool IsWideCodes;
		public Dictionary<uint, uint> GlyphMap;

		public DefineFontInfoTag(SwfReader r)
		{
		}

		public void ToSwf(SwfWriter w)
		{
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("DefineFontInfo: ");
			w.WriteLine();
		}
	}
}
