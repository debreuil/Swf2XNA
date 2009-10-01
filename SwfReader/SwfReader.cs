/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.IO;
using NZlib.Compression;
namespace DDW.Swf
{
	public class SwfReader
	{
		public const int twip = 20;
		public const float twipF = 20.0F;
		private const long MAX_SIZE = 1000 * 1024;
		private uint pos;
		private int len;
		private byte bitMask;
        private byte[] bytes;

        private static System.Text.UTF8Encoding utf8Encoder = new System.Text.UTF8Encoding(false);

		public SwfReader(byte[] rawSwf)
		{
			this.bytes = rawSwf;
			this.len = bytes.Length;
			Reset();
		}
		public uint Position
		{
			get
			{
				return this.pos;
			}
			set
			{
				this.pos = value;
				Align();
			}
		}
		public void			Reset()
		{
			pos = 0;
			bitMask = 0x80;
		}
		public bool			GetBit()
		{
			bool result = (bytes[pos] & bitMask) != 0;
			bitMask /= 2;
			if (bitMask == 0)
			{
				pos++;
				bitMask = 0x80;
			}
			return result;
		}
		public uint			GetBits(uint count)
		{
			uint result = 0;
			for(int i = 0; i < count; i++)
			{
				result = GetBit() ? (result << 1) | 1 : result << 1;
			}
			return result;
		}
		public int			GetSignedNBits(uint count)
		{
			uint result = GetBits(count);
			if((result & (1 << (int)(count - 1))) != 0)
			{
				result |= (uint)(-1 << (int)count);
			}
			return (int)result;
		}
		public byte			GetByte()
		{
			Align();
			return bytes[pos++];
		}
		public byte[]		GetBytes(uint count)
		{
			Align();
			byte[] byteArray = new byte[count];
			for (int i = 0; i < count; i++)
			{
				byteArray[i] = bytes[pos++];
			}
			return byteArray;
		}
		public byte			PeekByte()
		{
			Align();
			return bytes[pos];
		}
		public byte[]		PeekBytes(int start, int len)
		{
			Align();
			byte[] result = new byte[len];
			Array.Copy(bytes, start, result, 0, len);
			return result;
		}
		public UInt16		GetUI16()
		{
			Align();
			return (UInt16)((bytes[pos++]) + (bytes[pos++] << 8));
		}
		public Int16		GetInt16()
		{
			Align();
			//Int16 temp = (Int16)((bytes[pos++]) + (bytes[pos++] << 8));
			//Int16 result = (Int16)(temp & 0x7FFF);
			//if ((temp & 0x8000) > 0)
			//{
			//    result *= -1;
			//}
			Int16 result = (Int16)((bytes[pos++]) + (bytes[pos++] << 8));
			return result;
		}
		public Int32 GetInt32()
		{
			Align();
			Int32 result = (Int32)
				(
					(uint)(bytes[pos++]) +
					(uint)(bytes[pos++] << 8) +
					(uint)(bytes[pos++] << 16) +
					(uint)(bytes[pos++] << 24)
				);
			return result;
		}
		public uint			PeekUI16()
		{
			Align();
			return (uint)((bytes[pos]) +	(bytes[pos + 1] << 8));
		}
		public uint			GetUI32()
		{
			return	(uint)(bytes[pos++]) +
					(uint)(bytes[pos++] << 8) +
					(uint)(bytes[pos++] << 16) +
					(uint)(bytes[pos++] << 24);
		}
		public void			SkipBytes(uint count)
		{
			Align();
			pos += count;
		}
		public void			SkipBits(uint count)
		{
			pos += (uint)(count / 8);
			count = count % 8;
			for (int i = 0; i < count; i++)
			{
				bitMask /= 2;
				if (bitMask == 0)
				{
					pos++;
					bitMask = 0x80;
				}				
			}
		}
		public float		GetFixedNBits(uint nBits)
		{
			float result = this.GetSignedNBits(nBits);
			if(nBits > 1)
			{
				result = result / 0x10000;
			}
			else if(nBits == 1 && result == -1F)
			{
				result = -1F / (float)(0x100000);
			}
			else if (nBits == 1 && result == 0F)
			{
				result = 1F / (float)(0x100000);
			}
			else
			{
				result = result / 0x10000;
			}
			return result;
		}
		public float GetFixed16_16()
		{
			return this.GetFixedNBits(32);
		}
		public float GetFixed8_8()
		{
			float result = this.GetSignedNBits(16);
			return result / 0X100;
		}
		public float		GetFloat32()
		{
			byte[] bytes = this.GetBytes(4);
			return System.BitConverter.ToSingle(bytes, 0);
		}
		public double		GetDouble()
		{
			byte[] bytes = this.GetBytes(8);
			return System.BitConverter.ToDouble(bytes, 0);
		}

		public void			Align()
		{
			if (bitMask != 0x80)
			{
				bitMask = 0x80;
				pos++;
			}
		}
		public string GetString()
        {
            Align();
            uint start = pos;
            char c = (char)bytes[pos++];
            while ((byte)c != 0)
            {
                //result += c;
                c = (char)bytes[pos++];
            }
            uint end = pos;
            pos = start;
            string result = utf8Encoder.GetString(bytes, (int)start, (int)(end - start - 1));
            pos = end;

            return result;

            //string result = "";
            //Align();
            //char c = (char)bytes[pos++];
            //while ((byte)c != 0)
            //{
            //    result += c;
            //    c = (char)bytes[pos++];
            //}
            //return result;
		}		
		public string GetString(uint len)
        {
            Align();
            uint start = pos;
            char c = (char)bytes[pos++];
            while ((byte)c != 0)
            {
                c = (char)bytes[pos++];
            }
            uint end = pos;
            pos = start;
            string result = utf8Encoder.GetString(bytes, (int)start, (int)len - 1);
            pos = end;

            return result;

            //string result = "";
            //Align();
            //char c = (char)bytes[pos++];
            //for (int i = 0; i < len - 1; i++)
            //{
            //    result += c;
            //    c = (char)bytes[pos++];				
            //}
            //return result;
		}
		public void DecompressSwf()
		{
			byte[] buf = new byte[bytes.Length - 8];
			int len = bytes[4] + bytes[5] * 0x100 + bytes[6] * 0x10000 + bytes[7] * 0x1000000;
			byte[] unzipped = new byte[len];
			Array.Copy(bytes, 0, unzipped, 0, 8);
			Array.Copy(bytes, 8, buf, 0, bytes.Length - 8);
			Inflater inf = new Inflater();
			inf.SetInput(buf);
			int error = inf.Inflate(unzipped, 8, len - 8);
			if (error == 0)
			{
				throw new FileLoadException("The swf file could not be decompressed.");
			}
			unzipped[0] = (byte)'F';
			bytes = unzipped;
		}
		public byte[] Decompress(uint compressedSize, uint unzippedSize)
		{
			byte[] compressed = this.GetBytes(compressedSize);
			byte[] result = new byte[unzippedSize];
			Inflater inf = new Inflater();
			inf.SetInput(compressed);
			int error = inf.Inflate(result, 0, (int)unzippedSize);
			if (error == 0)
			{
				throw new FileLoadException("The a section of the swf file could not be decompressed.");
			}
			return result;
		}
		public static byte[] Decompress(byte[] compressed, uint unzippedSize)
		{
			byte[] result = new byte[unzippedSize];
			Inflater inf = new Inflater();
			inf.SetInput(compressed);
			int error = inf.Inflate(result, 0, (int)unzippedSize);
			if (error == 0)
			{
				throw new FileLoadException("The a section of the swf file could not be decompressed.");
			}
			return result;
		}
	}
}
