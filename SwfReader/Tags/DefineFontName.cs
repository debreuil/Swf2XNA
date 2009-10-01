/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class DefineFontName : ISwfTag
	{
		/*
            Header                  RECORDHEADER            Tag type = 88
            FontID                  UI16                    ID for this font to which this refers
            FontName                STRING                  Name of the font. For fonts starting as Type 1, this is the
                                                            PostScript FullName. For fonts starting in sfnt formats such as
                                                            TrueType and OpenType, this is name ID 4, platform ID 1,
                                                            language ID 0 (Full name, Mac OS, English).
            FontCopyright           STRING                  Arbitrary string of copyright information
		 */

		public const TagType tagType = TagType.DefineFontName;
		public TagType TagType { get { return tagType; } }

        public uint FontID;
        public string FontName;
        public string FontCopyright;

		public DefineFontName(SwfReader r)
		{
            FontID = r.GetUI16();
            FontName = r.GetString();
            FontCopyright = r.GetString();
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendUI16(FontID);
            w.AppendString(FontName);
            w.AppendString(FontCopyright);
            w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("DefineFontName: ");
			w.WriteLine();
		}
	}
}
