/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Swf
{
	public class Mp3Frame
	{
		/*
			Syncword		UB[11]	Frame sync.
									All bits must be set.
			MpegVersion		UB[2]	MPEG2.5 is an extension to
									MPEG2 that handles very low
									bitrates, allowing the use of
									lower sampling frequencies.
									0 = MPEG Version 2.5
									1 = reserved
									2 = MPEG Version 2
									3 = MPEG Version 1
			Layer			UB[2]	Layer is always equal to 1 for
									MP3 headers in SWF files. The
									“3” in MP3 refers to the Layer,
									not the MpegVersion.
									0 = reserved
									1 = Layer III
									2 = Layer II
									3 = Layer I
			ProtectionBit	UB[1]	If ProtectionBit == 0 a 16-bit
									CRC follows the header
									0 = Protected by CRC
									1 = Not protected
			Bitrate			UB[4]	Bitrates are in thousands of bits
									per second. For example, 128
									means 128000 bps.
									Value MPEG1 MPEG2.x
									---------------------
									0 free free
									1 32 8
									2 40 16
									3 48 24
									4 56 32
									5 64 40
									6 80 48
									7 96 56
									8 112 64
									9 128 80
									10 160 96
									11 192 112
									12 224 128
									13 256 144
									14 320 160
									15 bad bad
			SamplingRate	UB[2]	Sampling rate in Hz.
									Value MPEG1 MPEG2
									MPEG2.5
									-------------------------
									0 44100 22050 11025
									1 48000 24000 12000
									2 32000 16000 8000
									-- -- --
			PaddingBit		UB[1]	Padding is used to fit the bitrate
									exactly.
									0 = frame is not padded
									1 = frame is padded with one
									extra slot
			Reserved		UB[1]	
			ChannelMode		UB[2]	Dual-channel files are made of
									two independent mono
									channels. Each one uses
									exactly half the bitrate of the
									file.
			ModeExtension	UB[2]	
									0 = Stereo
									1 = Joint stereo (Stereo)
									2 = Dual channel
									2 = Single channel (Mono)
			Copyright		UB[1]	0 = Audio is not copyrighted
									1 = Audio is copyrighted
			Original		UB[1]	0 = Copy of original media
									1 = Original media
			Emphasis		UB[2]	0 = none
									1 = 50/15 ms
									2 = reserved
									3 = CCIT J.17
			SampleData		UB[size of sample data*]	The encoded audio samples.
		
		 */

		//public float MpegVersion;
		//public uint Layer;
		//public bool hasCrc;
		//public uint Bitrate;
		//public uint SamplingRate;
		//public bool isPadded;
		//// reserved
		//public bool IsDualChannel;
		//public bool IsCopyrighted;
		//public bool IsCopy;
		//public uint Emphasis;
		//public byte[] SampleData;

		public byte[] Bytes;

		public Mp3Frame(SwfReader r)
		{
			//r.GetBytes();
		}
	}
}
