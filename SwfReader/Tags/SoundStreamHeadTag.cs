/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	public class SoundStreamHeadTag : ISwfTag, IControlTag
	{
		/*
			Header					RECORDHEADER	Tag type = 18.
			Reserved				UB[4]			Always zero.
			PlaybackSoundRate		UB[2]			Playback sampling rate
													0 = 5.5 kHz
													1 = 11 kHz
													2 = 22 kHz
													3 = 44 kHz
			PlaybackSoundSize		UB[1]			Playback sample size.
													Always 1 (16 bit).
			PlaybackSoundType		UB[1]			Number of playback channels:
													mono or stereo.
													0 = sndMono
													1 = sndStereo
			StreamSoundCompression	UB[4]			Format of streaming sound data.
													1 = ADPCM
													SWF 4 and later only:
													2 = MP3
			StreamSoundRate			UB[2]			The sampling rate of the
													streaming sound data.
													0 = 5.5 kHz
													1 = 11 kHz
													2 = 22 kHz
													3 = 44 kHz
			StreamSoundSize			UB[1]			The sample size of the
													streaming sound data.
													Always 1 (16 bit).
			StreamSoundType			UB[1]			Number of channels in the
													streaming sound data.
													0 = sndMono
													1 = sndStereo
			StreamSoundSampleCount	UI16			Average number of samples in
													each SoundStreamBlock. Not
													affected by mono/stereo
													setting; for stereo sounds this is
													the number of sample pairs.
			LatencySeek				If 
							StreamSoundCompression	See MP3 sound data. The value here should match the
							== 2 then SI16			SeekSamples field in the first SoundStreamBlock for this stream.
							Otherwise absent	
		 */

		public static readonly string[] SoundExtentions = new string[]
		{
			".wav",
			".wav",
			".mp3",
			".wav",
			"",
			"",
			"" // nellymoser not supported
		};
		private static uint soundIdCounter = 0;

		public uint SoundId;

		public const TagType tagType = TagType.SoundStreamHead2;
		public TagType TagType { get { return tagType; } }

		public uint PlaybackSoundRate;
		public uint PlaybackSoundSize;
		public bool IsStereo;
		public SoundCompressionType StreamSoundCompression;
		public uint StreamSoundRate;
		public uint StreamSoundSize;
		public bool StreamIsStereo;
		public uint StreamSoundSampleCount;
		public uint LatencySeek;

		private uint[] rates = new uint[] {55000, 11000, 22000, 44000};

		public SoundStreamHeadTag(SwfReader r)
		{
			SoundId = soundIdCounter++;

			r.GetBits(4); // reserved

			PlaybackSoundRate = rates[r.GetBits(2)];
			PlaybackSoundSize = r.GetBit() ? 16u : 0u;
			IsStereo = r.GetBit();
			StreamSoundCompression = (SoundCompressionType)r.GetBits(4); // Mp3 == 2

			StreamSoundRate = rates[r.GetBits(2)];
			StreamSoundSize = r.GetBit() ? 16u : 8u;
			StreamIsStereo = r.GetBit();

			r.Align();

			StreamSoundSampleCount = r.GetUI16();

			if (StreamSoundCompression == SoundCompressionType.MP3)
			{
				LatencySeek = r.GetUI16();
			}
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendBits(0, 4);
            switch (PlaybackSoundRate)
            {
                case 55000:
                    w.AppendBits(0, 2);
                    break;
                case 11000:
                    w.AppendBits(1, 2);
                    break;
                case 22000:
                    w.AppendBits(2, 2);
                    break;
                case 44000:
                    w.AppendBits(3, 2);
                    break;
            }
            w.AppendBit(PlaybackSoundSize == 16u);
            w.AppendBit(IsStereo);
            w.AppendBits((uint)StreamSoundCompression, 4);

            switch (StreamSoundRate)
            {
                case 55000:
                    w.AppendBits(0, 2);
                    break;
                case 11000:
                    w.AppendBits(1, 2);
                    break;
                case 22000:
                    w.AppendBits(2, 2);
                    break;
                case 44000:
                    w.AppendBits(3, 2);
                    break;
            }
            w.AppendBit(StreamSoundSize == 16u);
            w.AppendBit(StreamIsStereo);
            w.Align();

            w.AppendUI16(StreamSoundSampleCount);

            if (StreamSoundCompression == SoundCompressionType.MP3)
            {
                w.AppendUI16(LatencySeek);
            }

            w.ResetLongTagLength(this.TagType, start);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("SoundStreamHead");
		}
	}
}
