/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using Drawing2D = System.Drawing.Drawing2D;
using DDW.Vex;

namespace DDW.Swf
{
	public struct Matrix : IRecord
	{
		/*
			MATRIX
			Field			Type								Comment
			HasScale		UB[1]								Has scale values if equal to 1
			NScaleBits		If HasScale = 1, UB[5]				Bits in each scale value field
			ScaleX			If HasScale = 1, FB[NScaleBits]		x scale value
			ScaleY			If HasScale = 1, FB[NScaleBits]		y scale value
			HasRotate		UB[1]								Has rotate and skew values if equal to 1
			NRotateBits		If HasRotate = 1, UB[5]				Bits in each rotate value field
			RotateSkew0		If HasRotate = 1,					First rotate and skew value
							FB[NRotateBits]			
			RotateSkew1		If HasRotate = 1,					Second rotate and skew value
							FB[NRotateBits]			
			NTranslateBits	UB[5]								Bits in each translate value field
			TranslateX		SB[NTranslateBits]					x translate value in twips
			TranslateY		SB[NTranslateBits]					y translate value in twips
		*/

		public static Matrix Empty = new Matrix(0, 0, 0, 0, 0, 0);
		public static Matrix Identity = new Matrix(1, 0, 0, 1, 0, 0);

		public float ScaleX;
		public float ScaleY;
		public float Rotate0;
		public float Rotate1;
		public float TranslateX;
		public float TranslateY;

		public Matrix(float scaleX, float rotate0, float rotate1, float scaleY, float translateX, float translateY)
		{
			this.ScaleX = scaleX;
			this.Rotate0 = rotate0;
			this.Rotate1 = rotate1;
			this.ScaleY = scaleY;
			this.TranslateX = translateX;
			this.TranslateY = translateY;
		}
		public Matrix(SwfReader r)
		{
			float sx = 1.0F;
			float sy = 1.0F;
			float r0 = 0.0F;
			float r1 = 0.0F;
			float tx = 0.0F;
			float ty = 0.0F;
			r.Align();
			bool hasScale = r.GetBit();
			if (hasScale)
			{
				uint scaleBits = r.GetBits(5);
				sx = r.GetFixedNBits(scaleBits);
				sy = r.GetFixedNBits(scaleBits);
			}
			bool hasRotate = r.GetBit();
			if (hasRotate)
			{
				uint nRotateBits = r.GetBits(5);
				r0 = r.GetFixedNBits(nRotateBits);
				r1 = r.GetFixedNBits(nRotateBits);
			}
			// always has translation
			uint nTranslateBits = r.GetBits(5);
			tx = r.GetSignedNBits(nTranslateBits);
			ty = r.GetSignedNBits(nTranslateBits);
			r.Align();
			
			this.ScaleX = sx;
			this.Rotate0 = r0;
			this.Rotate1 = r1;
			this.ScaleY = sy;
			this.TranslateX = tx;
			this.TranslateY = ty;
		}
		public void TransformPoints(Point[] pts)
		{
			Drawing2D.Matrix m = new Drawing2D.Matrix
				(
					this.ScaleX,
					this.Rotate0,
					this.Rotate1,
					this.ScaleY,
					this.TranslateX,
					this.TranslateY 
				);
			System.Drawing.PointF[] dpts = new System.Drawing.PointF[pts.Length];
			for (int i = 0; i < pts.Length; i++)
			{
				dpts[i] = new System.Drawing.PointF(pts[i].X, pts[i].Y);
			}
			m.TransformPoints(dpts);
			for (int i = 0; i < pts.Length; i++)
			{
				pts[i] = new Point(dpts[i].X, dpts[i].Y);
			}
		}

		public void ToSwf(SwfWriter w)
		{
			bool scale = (this.ScaleX != 1) || (this.ScaleY != 1);
			w.AppendBit(scale);
			if (scale)
			{
				uint bits = SwfWriter.MinimumBits((int)(this.ScaleX * 0x10000), (int)(this.ScaleY * 0x10000));
				w.AppendBits(bits, 5);
				w.AppendFixedNBits(this.ScaleX, bits);
				w.AppendFixedNBits(this.ScaleY, bits);
			}

			bool rotate = (this.Rotate0 != 0) || (this.Rotate1 != 0);
			w.AppendBit(rotate);
			if (rotate)
			{
				uint bits = SwfWriter.MinimumBits((int)(this.Rotate0 * 0x10000), (int)(this.Rotate1 * 0x10000));
				w.AppendBits(bits, 5);
				w.AppendFixedNBits(this.Rotate0, bits);
				w.AppendFixedNBits(this.Rotate1, bits);
			}

			// translate
			uint minbits = 0;
			if (this.TranslateX != 0 || this.TranslateY != 0)
			{
				minbits = SwfWriter.MinimumBits((int)this.TranslateX, (int)this.TranslateY);
			}
			w.AppendBits(minbits, 5);
			w.AppendBits((uint)(this.TranslateX), minbits);
			w.AppendBits((uint)(this.TranslateY), minbits);

			w.Align();
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("[");
			w.Write("sx:" + this.ScaleX.ToString("F3"));
			w.Write(" r0:" + this.Rotate0.ToString("F3") + " ");
			w.Write(" r1:" + this.Rotate1.ToString("F3") + " ");
			w.Write(" sy:" + this.ScaleY.ToString("F3") + " ");
			w.Write(" tx:" + this.TranslateX + " ");
			w.Write(" ty:" + this.TranslateY);
			w.Write("]");
		}
	}
}


