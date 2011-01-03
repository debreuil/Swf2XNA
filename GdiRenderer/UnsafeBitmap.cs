using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace DDW.Gdi
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

        public void SetPixel(int x, int y, PixelData c)
        {
            PixelData* pixel = PixelAt(x, y);
            *pixel = c;
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
        public void ConvertToPremultiplied()
        {
            LockBitmap();
             
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle(
                  (int)boundsF.X,
                  (int)boundsF.Y,
                  (int)boundsF.Width,
                  (int)boundsF.Height);

            for (int x = 0; x < bounds.Width; x++)
            {
                for (int y = 0; y < bounds.Height; y++)
                {
                    PixelData c = *PixelAt(x, y);
                    float a = c.alpha / 255f;
                    PixelData preMult = new PixelData((int)(a * c.red), (int)(a * c.green), (int)(a * c.blue), c.alpha);
                    //PixelData preMult = new PixelData(128, c.alpha * c.green, c.alpha * c.blue, c.alpha);
                    PixelData* pixel = PixelAt(x, y);
                    *pixel = preMult;
                }
            }

            UnlockBitmap();
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
        public void Interpolate(float t, PixelData pd)
        {
            float it;
            if (t > 0)
            {
                it = 1f - t;
            }
            else
            {
                it = t;
                t = 1f - it;
            }

            if (alpha > 0 || pd.alpha > 0)
            {
                this.red = (byte)((this.red * t) + (pd.red * it));
                this.green = (byte)((this.green * t) + (pd.green * it));
                this.blue = (byte)((this.blue * t) + (pd.blue * it));
                this.alpha = (byte)((this.alpha * t) + (pd.alpha * it));
            }
            else
            {
                this.alpha = (byte)((this.alpha * t) + (pd.alpha * it));
            }
        }
        public byte blue;
        public byte green;
        public byte red;
        public byte alpha;
    }
}