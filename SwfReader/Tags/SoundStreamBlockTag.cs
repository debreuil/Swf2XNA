/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	public class SoundStreamBlockTag : ISwfTag
	{
		public const TagType tagType = TagType.SoundStreamBlock;
		public TagType TagType { get { return tagType; } }

		public uint SampleCount;
		//public Mp3SoundData[] SoundData;
		public byte[] SoundData;

		public SoundStreamBlockTag(SwfReader r, uint tagLen)
		{
			SampleCount = r.GetUI16();
			SoundData = r.GetBytes(tagLen - 2);

			// assume mp3 for now
			//SoundData = new Mp3SoundData[SampleCount];
			//for (int i = 0; i < SampleCount; i++)
			//{
			//    SoundData[i] = new Mp3SoundData(r);
			//}
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendUI16(SampleCount);
            w.AppendBytes(SoundData);

            w.ResetLongTagLength(this.TagType, start);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("SoundStreamBlock " );
		}
	}
}
