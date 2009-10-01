using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace DDW.SwfBitmapPacker
{
	public unsafe class UnsafeBitmap
	{
		Bitmap bitmap;

		// three elements used for MakeGreyUnsafe
		int width;
		BitmapData bitmapData = null;
		Byte* pBase = null;

		public UnsafeBitmap(Bitmap bitmap)
		{
			this.bitmap = new Bitmap(bitmap);
		}

		public UnsafeBitmap(int width, int height)
		{
			this.bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
		}

		public void Dispose()
		{
			bitmap.Dispose();
		}

		public Bitmap Bitmap
		{
			get
			{
				return (bitmap);
			}
		}

		private Point PixelSize
		{
			get
			{
				GraphicsUnit unit = GraphicsUnit.Pixel;
				RectangleF bounds = bitmap.GetBounds(ref unit);

				return new Point((int)bounds.Width, (int)bounds.Height);
			}
		}

		public void LockBitmap()
		{
			GraphicsUnit unit = GraphicsUnit.Pixel;
			RectangleF boundsF = bitmap.GetBounds(ref unit);
			Rectangle bounds = new Rectangle((int)boundsF.X,
		  (int)boundsF.Y,
		  (int)boundsF.Width,
		  (int)boundsF.Height);

			// Figure out the number of bytes in a row
			// This is rounded up to be a multiple of 4
			// bytes, since a scan line in an image must always be a multiple of 4 bytes
			// in length.
			width = (int)boundsF.Width * sizeof(PixelData);
			if (width % 4 != 0)
			{
				width = 4 * (width / 4 + 1);
			}
			bitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			pBase = (Byte*)bitmapData.Scan0.ToPointer();
		}

		public PixelData GetPixel(int x, int y)
		{
			PixelData returnValue = *PixelAt(x, y);
			return returnValue;
		}

		public void SetPixel(int x, int y, PixelData colour)
		{
			PixelData* pixel = PixelAt(x, y);
			*pixel = colour;
		}

		public void UnlockBitmap()
		{
			bitmap.UnlockBits(bitmapData);
			bitmapData = null;
			pBase = null;
		}
		public PixelData* PixelAt(int x, int y)
		{
			return (PixelData*)(pBase + y * width + x * sizeof(PixelData));
		}

        public static void Copy(Bitmap source, Rectangle sourceRect, UnsafeBitmap output,  Rectangle destRect)
        {
            Graphics g = Graphics.FromImage(output.Bitmap);
            g.DrawImage(source, destRect, sourceRect, GraphicsUnit.Pixel);
            g.Dispose();

        }
	}
	public struct PixelData
	{
		public PixelData(int r, int g, int b, int a)
		{
			this.red = (byte)r;
			this.green = (byte)g;
			this.blue = (byte)b;
			this.alpha = (byte)a;
		}
		public PixelData(byte r, byte g, byte b, byte a)
		{
			this.red = r;
            this.green = g;
            this.blue = b;
            this.alpha = a;
		}
		public byte blue;
		public byte green;
		public byte red;
		public byte alpha;
	}
}