/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class JPEGTables : ISwfTag
	{
		public const TagType tagType = TagType.JPEGTables;
		public TagType TagType { get { return tagType; } }

        public byte[] JpegTable;

		public JPEGTables(SwfReader r, uint curTagLen)
		{
			if(curTagLen > 8)
			{
				r.GetBytes(2); //jpg SOI Marker
				JpegTable = r.GetBytes(curTagLen - 4);
				r.GetBytes(2); //jpg EOI Marker
			}
            else
            {
                JpegTable = new byte[0];
            }
		}

		public void ToSwf(SwfWriter w)
		{
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            if(JpegTable.Length > 0)
            {
                    w.AppendByte(0xFF);
                    w.AppendByte(0xD8);

                    w.AppendBytes(JpegTable);

                    w.AppendByte(0xFF);
                    w.AppendByte(0xD9);
            }

            w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("JPEGTables: ");
			w.WriteLine();
		}
	}
}
