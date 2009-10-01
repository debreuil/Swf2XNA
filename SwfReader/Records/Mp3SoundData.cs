/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Swf
{
	public class Mp3SoundData
	{
		public uint SeekSamples;
		public Mp3Frame[] Frames;

		public Mp3SoundData(SwfReader r)
		{
			this.SeekSamples = r.GetUI16();
		}
	}
}
