/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Swf
{

	/*
	 SOUNDENVELOPE
		Pos44			UI32	Position of envelope point as a number of 44 kHz samples. Multiply accordingly if using a sampling rate less than 44kHz.
		LeftLevel		UI16	Volume level for left channel. Minimum is 0, maximum is 32768.
		RightLevel		UI16	Volume level for right channel. Minimum is 0, maximum is 32768.	
	*/
	public struct SoundEnvelope
	{
		public uint Pos44;
		public uint LeftLevel;
		public uint RightLevel;

		public SoundEnvelope(uint pos, uint left, uint right)
		{
			Pos44 = pos;
			LeftLevel = left;
			RightLevel = right;
		}

	}
}
