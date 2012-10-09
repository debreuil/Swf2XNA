/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace DDW.Swf
{
	public class Utils
	{
		public const uint TwipsPerPixel = 20;


		public static void WriteAlphaJpg(byte[] alphaData, Bitmap jpg, string pngDest)
		{
			int w = jpg.Width;
			int h = jpg.Height;
			Bitmap png = new Bitmap(w, h, PixelFormat.Format32bppArgb);
			

		   BitmapData jpgData = jpg.LockBits(
			   new Rectangle(0, 0, w, h),
			   ImageLockMode.ReadOnly, 
			   PixelFormat.Format24bppRgb);

		   BitmapData pngData = png.LockBits(
			   new Rectangle(0, 0, w, h),
			   ImageLockMode.WriteOnly, 
			   PixelFormat.Format32bppArgb);

			unsafe
			{
				byte* jpgStart = (byte*)jpgData.Scan0.ToPointer();
				byte* pngStart = (byte*)pngData.Scan0.ToPointer();
				int jpgStride = jpgData.Stride;
				int pngStride = pngData.Stride;

				for (int y = 0; y < h; y++)
				{
					byte* jpgRow = (byte*)(jpgStart + y * jpgStride);
					byte* pngRow = (byte*)(pngStart + y * pngStride);

					for (int x = 0; x < w; x++) // 3 bpp
					{
						int pngX = x * 4;
						int jpgX = x * 3;
						pngRow[pngX] = jpgRow[jpgX];
						pngRow[pngX + 1] = jpgRow[jpgX + 1];
						pngRow[pngX + 2] = jpgRow[jpgX + 2];
						pngRow[pngX + 3] = alphaData[y*w + x];
					}
				}
			}		   
			jpg.UnlockBits(jpgData);
			png.UnlockBits(pngData);

			DDW.Vex.IOUtils.EnsurePath(pngDest);
			png.Save(pngDest);
			png.Dispose();
		}

		public static Vex.Size GetJpgSize(string path)
		{
			Bitmap bmp = new Bitmap(path, false);
			Vex.Size sz = new DDW.Vex.Size(bmp.Width, bmp.Height);
			bmp.Dispose();
			return sz;
		}

		public static Matrix ScaleBitmapMatrix(Matrix m)
		{
			System.Drawing.Drawing2D.Matrix mx = 
				new System.Drawing.Drawing2D.Matrix(m.ScaleX, m.Rotate0, m.Rotate1, m.ScaleY, m.TranslateX, m.TranslateY);
			mx.Scale(1 / 20F, 1 / 20F);
			float[] els = mx.Elements;
			return new Matrix(els[0], els[1], els[2], els[3], els[4], els[5]);
		}
	}
}
