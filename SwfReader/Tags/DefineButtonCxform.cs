
/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class DefineButtonCxform : ISwfTag
	{
        /*
            ButtonId                UI16    Button ID for this information
            ButtonColorTransform    CXFORM  Character color transform
        */
		public const TagType tagType = TagType.DefineButtonCxform;
		public TagType TagType { get { return tagType; } }

        public uint ButtonId;
        public ColorTransform ButtonColorTransform;

		public DefineButtonCxform(SwfReader r)
		{
            ButtonId = r.GetUI16();
            ButtonColorTransform = new ColorTransform(r, false);
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendUI16(ButtonId);
            ButtonColorTransform.ToSwf(w);

            w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("DefineButtonCxform: ");
			w.WriteLine(); 
		}
	}
}
