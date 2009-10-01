/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace DDW.Swf
{
	/*
		Header 				RECORDHEADER 		(long) Tag type = 36
		CharacterID 		UI16 						ID for this character
		BitmapFormat 		UI8 						Format of compressed data
														3 = 8-bit colormapped image
														4 = 15-bit RGB image
														5 = 24-bit RGB image
		BitmapWidth 		UI16 						Width of bitmap image
		BitmapHeight 		UI16 						Height of bitmap image
		BitmapColorTableSize 
							If BitmapFormat = 3 UI8		This value is one less than the
							Otherwise absent			actual number of colors in the
														color table, allowing for up to
														256 colors.


		ZlibBitmapData 		If BitmapFormat = 3			ZLIB compressed bitmap data
								ALPHACOLORMAPDATA
							If BitmapFormat = 4 or 5
								ALPHABITMAPDATA
	*/
	public class DefineBitsLosslessTag : ISwfTag
	{
		private TagType tagType = TagType.DefineBitsLossless;
		public TagType TagType { get { return tagType; } }

		public UInt16 CharacterId;
		public uint Width;
		public uint Height;
		public bool isIndexedColors = false;

		public uint ColorCount;
		public RGBA[] ColorTable;
        public uint[] ColorData;
        public RGBA[] BitmapData;

        public BitmapFormat BitmapFormat;
        public bool HasAlpha;

        public byte[] OrgBitmapData;

        public DefineBitsLosslessTag(bool hasAlpha)
        {
        }
		public DefineBitsLosslessTag(SwfReader r, uint curTagLen, bool hasAlpha)
		{
            HasAlpha = hasAlpha;
            if (hasAlpha)
            {
                tagType = TagType.DefineBitsLossless2;
            }

			CharacterId = r.GetUI16();
			BitmapFormat = (BitmapFormat)r.GetByte();

			this.Width = r.GetUI16();
			this.Height = r.GetUI16();

            if (BitmapFormat == BitmapFormat.Colormapped8Bit) // 8-bit colormapped image
			{
				this.ColorCount = (uint)r.GetByte() + 1;

				this.isIndexedColors = true;
				uint colorBytes = hasAlpha ? (uint)4 : (uint)3;
				uint padWidth = this.Width + (4 - (this.Width % 4));

                // temp for debugging
                uint pos = r.Position;
                OrgBitmapData = r.GetBytes(curTagLen - 8);
                r.Position = pos;
                // end temp

				uint unzippedSize = (this.ColorCount * colorBytes) + (padWidth * this.Height);
				byte[] mapData = r.Decompress(curTagLen - 8, unzippedSize);

				uint index = 0;
				this.ColorTable = new RGBA[this.ColorCount];
				for (int i = 0; i < this.ColorCount; i++)
				{
					if (hasAlpha)
					{
						this.ColorTable[i] = new RGBA(mapData[index], mapData[index + 1], mapData[index + 2], mapData[index + 3]);
					}
					else
					{
						this.ColorTable[i] = new RGBA(mapData[index], mapData[index + 1], mapData[index + 2]);
					}
					index += colorBytes;
				}
				this.ColorData = new uint[this.Width * this.Height];
				index = 0;
                int offset = (int)(this.ColorCount * colorBytes);
				for (int i = 0; i < padWidth * this.Height; i++)
				{
					if ((i % padWidth) < this.Width)// exclude padding
					{
						this.ColorData[index++] = mapData[i + offset];
					}
				}
			}
            else if (BitmapFormat == BitmapFormat.RGB15Bit) // RGBx555
            {
                // todo: find a test file for rgb555
				uint colorBytes = 2;
				uint padWidth = this.Width * colorBytes;
				padWidth += (4 - padWidth) % 4;

                // temp for debugging
                uint pos = r.Position;
                OrgBitmapData = r.GetBytes(curTagLen - 7);
                r.Position = pos;
                // end temp

				uint unzippedSize = (padWidth * this.Height) * colorBytes;
				byte[] mapData = r.Decompress(curTagLen - 7, unzippedSize);

				int index = 0;
				this.BitmapData = new RGBA[this.Width * this.Height];
                for (uint i = 0; i < unzippedSize; i += colorBytes)
				{
					if ((i % padWidth) < (this.Width * colorBytes)) // exclude padding
					{
						byte b0 = mapData[i];
						byte b1 = mapData[i + 1];
						byte rd = (byte)((b0 & 0x7C) << 1);
						byte gr = (byte)(((b0 & 0x03) << 6) | ((b1 & 0xE0) >> 2));
						byte bl = (byte)((b1 & 0x1F) << 3);
						this.BitmapData[index++] = new RGBA(rd, gr, bl);
					}
				}
			}
            else if (BitmapFormat == BitmapFormat.RGB24Bit) // RGB 24
            {                
                // temp for debugging
                uint pos = r.Position;
                OrgBitmapData = r.GetBytes(curTagLen - 7);
                r.Position = pos;
                // end temp

				uint colorBytes = 4; // 4 bytes will always be byte aligned
				uint unzippedSize = (this.Width * this.Height) * colorBytes;
				byte[] mapData = r.Decompress(curTagLen - 7, unzippedSize);

				int index = 0;
				this.BitmapData = new RGBA[this.Width * this.Height];
                for (uint i = 0; i < unzippedSize; i += colorBytes)
				{
					if (hasAlpha)
					{
						this.BitmapData[index++] = new RGBA(mapData[i + 1], mapData[i + 2], mapData[i + 3], mapData[i]);
					}
					else
					{
						this.BitmapData[index++] = new RGBA(mapData[i + 1], mapData[i + 2], mapData[i + 3]);
					}
				}
			}
		}
		public Bitmap GetBitmap()
		{
			Bitmap bmp = new Bitmap((int)this.Width, (int)this.Height, PixelFormat.Format32bppArgb);

			if (isIndexedColors)
			{
				for (int y = 0; y < this.Height; y++)
				{
					for (int x = 0; x < this.Width; x++)
					{
						RGBA col = this.ColorTable[this.ColorData[y * this.Width + x]];
						bmp.SetPixel(x, y, Color.FromArgb(col.A, col.R, col.G, col.B));
					}
				}
			}
			else
			{
				for (int y = 0; y < this.Height; y++)
				{
					for (int x = 0; x < this.Width; x++)
					{
						RGBA col = this.BitmapData[y * this.Width + x];
						bmp.SetPixel(x, y, Color.FromArgb(col.A, col.R, col.G, col.B));
					}
				}
			}
			return bmp;
		}
		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendUI16(CharacterId);
            w.AppendByte((byte)BitmapFormat);
            w.AppendUI16(Width);
            w.AppendUI16(Height);

            if (BitmapFormat == BitmapFormat.Colormapped8Bit) // 8-bit colormapped image
            {
                w.AppendByte((byte)(ColorCount - 1));

                uint colorBytes = HasAlpha ? (uint)4 : (uint)3;
                uint padWidth = this.Width + (4 - (this.Width % 4));
                uint unzippedSize = (this.ColorCount * colorBytes) + (padWidth * this.Height);

                byte[] mapData = new byte[unzippedSize];

                for (int i = 0; i < this.ColorCount; i++)
                {
                    mapData[i * colorBytes + 0] = ColorTable[i].R;
                    mapData[i * colorBytes + 1] = ColorTable[i].G;
                    mapData[i * colorBytes + 2] = ColorTable[i].B;
                    if (HasAlpha)
                    {
                        mapData[i * colorBytes + 3] = ColorTable[i].A;
                    }
                }

                int index = 0;
                int st = (int)(this.ColorCount * colorBytes);
                for (int i = st; i < unzippedSize; i++)
                {
                    if (((i - st) % padWidth) < this.Width)// exclude padding
                    {
                        mapData[i] = (byte)this.ColorData[index++];
                    }
                    else
                    {
                        mapData[i] = 0;
                    }
                }
                byte[] zipped = SwfWriter.ZipBytes(mapData);
                if(OrgBitmapData != null)
                {
                    w.AppendBytes(OrgBitmapData);
                }
                else
                {
                    w.AppendBytes(zipped);
                }

            }

            else if (BitmapFormat == BitmapFormat.RGB15Bit) // rbg 15            
            {
                // todo: find a test file for rgb555
                uint colorBytes = 2;
                uint padWidth = this.Width * colorBytes;
                padWidth += (4 - padWidth) % 4;
                uint unzippedSize = (padWidth * this.Height) * colorBytes;

                byte[] mapData = new byte[unzippedSize];

                int index = 0;
                uint byteCount = padWidth * this.Height;
                for (uint i = 0; i < unzippedSize; i += colorBytes)
                {
                    byte rd = this.BitmapData[index].R;
                    byte gr = this.BitmapData[index].G;
                    byte bl = this.BitmapData[index].B;

                    byte b0 = 0;
                    byte b1 = 0;
                    b0 |= (byte)((rd & 0x7C) << 1);
                    b0 |= (byte)((gr & 0x03) << 6);
                    b1 |= (byte)((gr & 0xE0) >> 2);
                    b1 |= (byte)((bl & 0x1F) << 3);

                    mapData[i + 0] = b0;
                    mapData[i + 1] = b1;

                    index++;
                }
                byte[] zipped = SwfWriter.ZipBytes(mapData);
                if (OrgBitmapData != null)
                {
                    w.AppendBytes(OrgBitmapData);
                }
                else
                {
                    w.AppendBytes(zipped);
                }
            }

            else if (BitmapFormat == BitmapFormat.RGB24Bit) // RGB 24 
            {
                uint colorBytes = HasAlpha ? (uint)4 : (uint)3;
                uint unzippedSize = (this.Width * this.Height) * colorBytes;

                byte[] mapData = new byte[unzippedSize];

                int index = 0;
                for (uint i = 0; i < unzippedSize; i += colorBytes)
                {
                    mapData[i + 0] = this.BitmapData[index].A;
                    mapData[i + 1] = this.BitmapData[index].R;
                    mapData[i + 2] = this.BitmapData[index].G;
                    if (HasAlpha)
                    {
                        mapData[i + 3] = this.BitmapData[index].B;
                    }
                    index++;
                }
                byte[] zipped = SwfWriter.ZipBytes(mapData);
                if (OrgBitmapData != null)
                {
                    w.AppendBytes(OrgBitmapData);
                }
                else
                {
                    w.AppendBytes(zipped);
                }
            }


            w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("DefineBitsLossless id_" + CharacterId + " w:" + this.Width + " h:" + this.Height);
		}
	}
}
