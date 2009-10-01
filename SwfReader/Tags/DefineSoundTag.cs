/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	/*
		Header		RECORDHEADER	Tag type = 14
		SoundId		UI16			ID for this sound.
		SoundFormat	UB[4]			Format of SoundData:
									0 = uncompressed
									1 = ADPCM
									SWF 4 or later only:
									2 = MP3
									3 = uncompressed little-endian
									SWF 6 or later only:
									6 = Nellymoser
		SoundRate	UB[2]			The sampling rate. 5.5kHz is not allowed for MP3.
									0 = 5.5 kHz
									1 = 11 kHz
									2 = 22 kHz
									3 = 44 kHz
		SoundSize	UB[1]			Size of each sample. Always 16 bit for compressed formats. May be 8 or 16 bit for uncompressed formats.
									0 = snd8Bit
									1 = snd16Bit
		SoundType	UB[1]			Mono or stereo sound
									For Nellymoser: always 0
									0 = sndMono
									1 = sndStereo
		SoundSampleCount	UI32	Number of samples. Not affected by mono/stereo setting; for stereo sounds this is the number of sample pairs.
		SoundData	UI8[size]		The sound data; varies by format.
	*/
	public class DefineSoundTag : ISwfTag, IControlTag
	{
		public const TagType tagType = TagType.DefineSound;
		public TagType TagType { get { return tagType; } }

		public uint SoundId;
		public SoundCompressionType SoundFormat;
		public uint SoundRate;
		public uint SoundSize;
		public bool IsStereo;
		public uint SoundSampleCount;
		public byte[] SoundData;


		public DefineSoundTag(SwfReader r, uint tagLen)
		{
			SoundId = r.GetUI16();
			SoundFormat = (SoundCompressionType)r.GetBits(4);
			uint sr = r.GetBits(2);
			switch (sr)
			{
				case 0:
					SoundRate = 5512; // ?
					break;
				case 1:
					SoundRate = 11025;
					break;
				case 2:
					SoundRate = 22050;
					break;
				case 3:
					SoundRate = 44100;
					break;
			}
			SoundSize = r.GetBit() ? 16U : 8U;
			IsStereo = r.GetBit();
			r.Align();

			SoundSampleCount = r.GetUI32();
			// todo: this needs to decompress if mp3 etc
			SoundData = r.GetBytes(tagLen - 7);
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);
            
            w.AppendUI16(SoundId);

            w.AppendBits((uint)SoundFormat, 4);
            switch (SoundRate)
            {
                case 5512:
                    w.AppendBits(0, 2);
                    break;
                case 11025:
                    w.AppendBits(1, 2);
                    break;
                case 22050:
                    w.AppendBits(2, 2);
                    break;
                case 44100:
                    w.AppendBits(3, 2);
                    break;
            }
            w.AppendBit(SoundSize == 16U);
            w.AppendBit(IsStereo); 
            w.Align();

            w.AppendUI32(SoundSampleCount);
            w.AppendBytes(SoundData);

            w.ResetLongTagLength(this.TagType, start);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("Define Sound");
		}
	}
}
