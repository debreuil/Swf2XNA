/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.IO;
using NZlib.Compression;
using NZlib.Streams;

namespace DDW.Swf
{
	public class SwfWriter
    {
        public const int twip = 20;
        public const float twipF = 20.0F;
        private const long MAX_SIZE = 1000 * 1024;

        private byte bitMask;
        private byte curBits = 0;
        private MemoryStream stream;

		private static System.Text.UTF8Encoding utf8Encoder = new System.Text.UTF8Encoding(false);

		public SwfWriter(MemoryStream stream)
        {
            this.stream = stream;
            Reset();
        }
        public long Position
        {
            get
            {
				return stream.Position;
            }
            set
            {
                Align();
				stream.Position = value;
            }
        }
        public void Reset()
		{
			stream.Position = 0;
            curBits = 0;
            bitMask = 0x80;
        }
        public void AppendBit(bool value)
        {
            curBits = value ? (byte)(curBits | bitMask) : curBits;
            bitMask /= 2;
            if (bitMask == 0)
            {
                stream.WriteByte(curBits);
				bitMask = 0x80;
				curBits = 0x00;
            }
        }
        public void AppendBits(uint value, uint count)
		{
			if (count == 0)
			{
				return;
			}
			count--;
			count &= 0x1F;

            uint mask = (uint)Math.Pow(2, count);
            for (int i = (int)count; i >= 0; i--)
            {
                AppendBit((value & mask) > 0);
                mask /= 2;
            }
        }
        public void AppendSignedNBits(int value, uint count)
        {
			count = Math.Min(count, 32);

			uint mask = (uint)Math.Pow(2, count - 1);
            for (int i = 0; i < count; i++)
            {
                AppendBit((value & mask) > 0);
                mask >>= 1;
            }
        }
        public void AppendByte(byte value)
        {
            stream.WriteByte(value);
        }
		public void AppendBytes(byte[] values)
        {
            Align();
			stream.Write(values, 0, values.Length);
        }
        public void AppendUI16(uint value)
        {
            Align();
			stream.WriteByte((byte)(value & 0xFF));
			stream.WriteByte((byte)(value >> 8));
        }
        public void AppendInt16(int value)
        {
			AppendUI16((uint)value);
        }
        public void AppendUI32(uint value)
        {
            Align();
			stream.WriteByte((byte)(value & 0x00FF));
			stream.WriteByte((byte)((value >> 8) & 0x00FF));
			stream.WriteByte((byte)((value >> 16) & 0x00FF));
			stream.WriteByte((byte)(value >> 24));
        }
        public void AppendInt32(int value)
        {
            AppendUI32((uint)value);
        }
        public void AppendFixedNBits(double value, uint bits)
		{
			int bitValue = (int)(value * 0x10000);
			AppendSignedNBits(bitValue, bits);
        }
		public void AppendFixed16_16(double value)
        {
			Align();
			uint bot = (uint)(value * 0x10000);
			stream.WriteByte((byte)(bot & 0xFF));
			stream.WriteByte((byte)((bot >> 8) & 0xFF));

			uint top = (uint)value;
			stream.WriteByte((byte)(top & 0xFF));
			stream.WriteByte((byte)((top >> 8) & 0xFF));
        }
		public void AppendFixed8_8(float value)
        {
			Align();
			uint bot = (uint)(value * 0x100);
			stream.WriteByte((byte)(bot & 0xFF));

			uint top = (uint)value;
			stream.WriteByte((byte)(top & 0xFF));
        }
		public void AppendFloat32(float value)
        {
			Align();

			byte[] bytes = BitConverter.GetBytes(value);

			AppendByte(bytes[0]);
			AppendByte(bytes[1]);
			AppendByte(bytes[2]);
			AppendByte(bytes[3]);
        }
		public void AppendDouble(double value)
		{
			Align();

			byte[] bytes = BitConverter.GetBytes(value);

			AppendByte(bytes[4]);
			AppendByte(bytes[5]);
			AppendByte(bytes[6]);
			AppendByte(bytes[7]);

			AppendByte(bytes[0]);
			AppendByte(bytes[1]);
			AppendByte(bytes[2]);
			AppendByte(bytes[3]);
        }

        public void Align()
        {
            if (bitMask != 0x80)
            {
				AppendByte(curBits);
                bitMask = 0x80;
				curBits = 0x00;
            }
        }
        public void AppendString(string s)
		{
			AppendBytes(utf8Encoder.GetBytes(s));
			AppendByte(0);
        }
        public void AppendString(string s, uint len)
		{
			AppendBytes(utf8Encoder.GetBytes(s.ToCharArray(), 0, (int)len));
			AppendByte(0);
        }
		public bool AppendTagIDAndLength(TagType tagType, uint bodyLength)
		{
			bool isLong = (bodyLength >= 0x3f);
			if (isLong)
			{
				AppendTagIDAndLength(tagType, bodyLength, true);
			}
			else
			{
				AppendTagIDAndLength(tagType, bodyLength, false);
			}
			return isLong;
		}
		public bool AppendTagIDAndLength(TagType tagType, uint bodyLength, bool isLong)
		{
			uint highBits = ((uint)tagType) << 6;
			if (isLong)
			{
				highBits |= 0x3F;
				AppendUI16(highBits);
				AppendUI32(bodyLength);
			}
			else
			{
				highBits |= bodyLength;
				AppendUI16(highBits);
			}
			return isLong;
		}
        public void ResetLongTagLength(TagType tagType, uint start)
        {
            ResetLongTagLength(tagType, start, false);
        }
		public void ResetLongTagLength(TagType tagType, uint start, bool alwaysLong)
		{
			uint bodyLength = (uint)((Position - start) - 6);
			Position = start;

            bool isLong = true;
            if (alwaysLong)
            {
                AppendTagIDAndLength(tagType, (uint)bodyLength, true);
            }
            else
            {
                isLong = AppendTagIDAndLength(tagType, (uint)bodyLength);
                if (!isLong)
                {
                    byte[] bytes = new byte[bodyLength];
                    Position = start + 6;
                    this.stream.Read(bytes, 0, (int)bodyLength);
                    Position = start + 2;
                    this.stream.Write(bytes, 0, (int)bodyLength);
                }
            }
	        uint headLength = isLong ? 6u : 2u;
	        Position = start + bodyLength + headLength;
		}

		// utils

		public static uint MinimumBits(uint value)
		{
			if (value == 0)
			{
				return 0;
			}
			uint mask = 1;
			uint bits = 1;
			for (; bits < 32; bits++)
			{
				mask <<= 1;
				if (mask > value)
				{
					break;
				}
			}
			return bits;
		}

		public static uint MinimumBits(int value)
		{
			if (value == 0)
			{
				return 1;
			}
			return MinimumBits((uint)Math.Abs(value)) + 1;
		}

		public static uint MinimumBits(double value)
		{
			uint result = 16 + 1; //decimal bits and sign
			if (value != 0)
			{
				result += MinimumBits((uint)Math.Abs(value));
			}
			return result;
		}

		public static uint MinimumBits(params int[] values)
		{
			uint max = 1;
			for (int i = 0; i < values.Length; i++)
			{
				uint bits = MinimumBits(values[i]);
				max = (bits > max) ? bits : max;
			}
			return max;
		}
		public static uint MinimumBits(params uint[] values)
		{
			return MinimumBits(values, true);
		}

		public static uint MinimumBits(params float[] values)
		{
			uint[] converted = new uint[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				converted[i] = (uint)values[i];
			}
			return MinimumBits(converted, true) + 1 + 16; // one of sign, 16 because the decimal part is always 16
		}

		public static uint MinimumBits(uint[] values, bool dummy)
		{
			uint maximumBits = 1;
			uint curbits;
			for (int i = 0; i < values.Length; i++)
			{
				curbits = MinimumBits(values[i]);
				if (curbits > maximumBits)
				{
					maximumBits = curbits;
				}
			}
			return maximumBits;
		}
		public byte[] ToArray()
		{
			return stream.ToArray();
		}
		public void Zip()
		{
			uint headerLen = 8;
			byte[] compressPart = new byte[Position - headerLen];
			byte[] source = stream.ToArray();
			Array.Copy(source, headerLen, compressPart, 0, Position - headerLen);
			byte[] compressed = ZipBytes(compressPart);

			Position = 0;
			AppendByte((byte)'C');

			Position = headerLen;
			AppendBytes(compressed);

			stream.SetLength(headerLen + compressed.Length);
		}

		public static byte[] ZipBytes(byte[] input)
        {
            MemoryStream ms = new MemoryStream();
            Deflater zipper = new Deflater();
            zipper.SetLevel(5);
            Stream st = new DeflaterOutputStream(ms, zipper);
            st.Write(input, 0, input.Length);
            st.Flush();
            st.Close();
            byte[] result = (byte[])ms.ToArray();
            return result;
		}

    }
}
