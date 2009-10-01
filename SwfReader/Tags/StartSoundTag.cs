/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	/*
	 StartSound
		Header		RECORDHEADER	Tag type = 15.
		SoundId		UI16			ID of sound character to play.
		SoundInfo	SOUNDINFO		Sound style information.
	 */


	public class StartSoundTag : ISwfTag, IControlTag
	{
		private TagType tagType = TagType.StartSound;
		public TagType TagType { get { return tagType; } }

		public uint SoundId;
        public SoundInfo SoundInfo;

		public StartSoundTag(SwfReader r)
        {
			SoundId = r.GetUI16();
            SoundInfo = new SoundInfo(r);
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendUI16(SoundId);
            SoundInfo.ToSwf(w);

            w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("Start Sound");
		}
	}
}
