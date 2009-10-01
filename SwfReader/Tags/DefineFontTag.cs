/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class DefineFontTag : ISwfTag
	{
		/*
			Header				RECORDHEADER	Tag type = 10
			FontID				UI16			ID for this font character
			OffsetTable			UI16[nGlyphs]	Array of shape offsets
			GlyphShapeTable		SHAPE[nGlyphs]	Array of shapes
		 */

		public const TagType tagType = TagType.DefineFont;
		public TagType TagType { get { return tagType; } }

		public uint FontId;
		public uint[] OffsetTable;
		//public List<GlyphShape> GlyphShapeTable;

		public DefineFontTag(SwfReader r)
		{
		}

		public void ToSwf(SwfWriter w)
		{
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("DefineFont: ");
			w.WriteLine();
		}
	}
}


						
						
						
						












