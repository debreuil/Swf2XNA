/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	/*
		Header				RECORDHEADER		Tag type = 11.
		CharacterID			UI16				ID for this text character.
		TextBounds			RECT				Bounds of the text.
		TextMatrix			MATRIX				Transformation matrix for the text.
		GlyphBits			UI8					Bits in each glyph index.
		AdvanceBits			UI8					Bits in each advance value.
		TextRecords			TEXTRECORD[0+]		Text records.
		EndOfRecordsFlag	UI8					Must be 0.
	 */
	public class DefineTextTag : ISwfTag
	{
		private TagType tagType = TagType.DefineText;
		public TagType TagType { get { return tagType; } }

		public uint CharacterId;
		public Rect TextBounds;
		public Matrix TextMatrix;		
		//public uint GlyphBits;	
		//public uint AdvanceBits;	
		public List<TextRecord> TextRecords = new List<TextRecord>();
		//public uint EndOfRecordsFlag;

        private uint glyphBits;
        private uint advanceBits;

		public DefineTextTag(SwfReader r, bool useAlpha)
		{
			if (useAlpha)
			{
				tagType = TagType.DefineText2;
			}
			CharacterId = r.GetUI16();
			TextBounds = new Rect(r);
			TextMatrix = new Matrix(r);
			glyphBits = (uint)r.GetByte();
			advanceBits = (uint)r.GetByte();

			while (r.PeekByte() != 0x00)
			{
				TextRecords.Add(new TextRecord(r, glyphBits, advanceBits, useAlpha));
			}
			byte end = r.GetByte();
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendUI16(CharacterId); 
			TextBounds.ToSwf(w);
			TextMatrix.ToSwf(w);

            w.AppendByte((byte)glyphBits); // TODO: gen nbits
            w.AppendByte((byte)advanceBits); // TODO: gen nbits

            for (int i = 0; i < TextRecords.Count; i++)
            {
                TextRecords[i].ToSwf(w, glyphBits, advanceBits, tagType >= TagType.DefineText2);
            }

            w.AppendByte(0); // end

            w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("DefineText: ");
			w.WriteLine();
		}
	}
}
