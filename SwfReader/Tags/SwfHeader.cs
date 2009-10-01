/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.IO;
using DDW.Vex;

namespace DDW.Swf
{
	/*
		Signature	UI8		Signature byte:
							“F” indicates uncompressed
							“C” indicates compressed (SWF 6 and later only)
		Signature	UI8		Signature byte always “W”
		Signature	UI8		Signature byte always “S”
		Version		UI8		Single byte file version (for example, 0x06 for SWF 6)
		FileLength	UI32	Length of entire file in bytes
		FrameSize	RECT	Frame size in twips
		FrameRate	UI16	Frame delay in 8.8 fixed number of frames per second
		FrameCount	UI16	Total number of frames in file
	 */


	public struct SwfHeader : IVexConvertable
	{
		public bool		IsSwf;
		public bool		IsCompressed;

		public byte		Signature0;
		public byte		Signature1;
		public byte		Signature2;

		public byte		Version;
		public UInt32	FileLength;
		public Rect		FrameSize;
		public float	FrameRate;
		public UInt16	FrameCount;

		public SwfHeader(SwfReader r)
		{
			this.Signature0 = r.GetByte();

			if (this.Signature0 == 'C')
			{
				this.IsCompressed = true;
				r.DecompressSwf();
			}
			else
			{
				this.IsCompressed = false;
			}

			this.Signature1 = r.GetByte();
			this.Signature2 = r.GetByte();

			this.IsSwf = (Signature2 == 'S') && (Signature1 == 'W') && ((Signature0 == 'C') || (Signature0 == 'F'));

			if (IsSwf)
			{
				this.Version = r.GetByte();
				this.FileLength = r.GetUI32();
				this.FrameSize = new Rect(r);
				UInt16 frate = r.GetUI16();
				this.FrameRate = (frate >> 8) + ((frate & 0xFF) / 0xFF);
				this.FrameCount = r.GetUI16();
			}
			else
			{
				this.Version = 0;
				this.FileLength = 0;
				this.FrameSize = new Rect(0,0,0,0);
				this.FrameRate = 0;
				this.FrameCount = 0;
			}
		}

		public bool Validate()
		{
			return false;
		}

		public void ToSwf(SwfWriter w)
		{
			if (IsCompressed)
			{
				w.AppendByte((byte)'C');
			}
			else
			{
				w.AppendByte((byte)'F');
			}
			w.AppendByte((byte)'W');
			w.AppendByte((byte)'S');
			
			w.AppendByte(this.Version);

			w.AppendUI32(this.FileLength);
			this.FrameSize.ToSwf(w);

			ushort frateWhole = (ushort)this.FrameRate;
			uint frateDec = (((uint)this.FrameRate * 0x100) & 0xFF);
			w.AppendUI16((uint)((frateWhole << 8) + frateDec));

			w.AppendUI16(this.FrameCount);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("Header");
			w.Indent++;

			w.Write((char)this.Signature0);
			w.Write((char)this.Signature1);
			w.Write((char)this.Signature2);
			w.WriteLine();

			w.WriteLine("version: " + this.Version);
			w.WriteLine("fileLength: " + this.FileLength);

			this.FrameSize.Dump(w);

			w.WriteLine("frameRate: " + this.FrameRate);
			w.WriteLine("frameCount: " + this.FrameCount);

			w.Indent--;
			w.WriteLine("End Header");
		}
	}
}
