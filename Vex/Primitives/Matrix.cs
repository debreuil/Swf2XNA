/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Globalization;

namespace DDW.Vex
{
    public struct Matrix : IVexObject, IXmlSerializable
	{
		public static readonly Matrix Empty = new Matrix(0, 0, 0, 0, 0, 0);
		public static readonly Matrix Identity = new Matrix(1, 0, 0, 1, 0, 0);

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

        public Matrix Clone()
        {
            return new Matrix(this.ScaleX, this.Rotate0, this.Rotate1, this.ScaleY, this.TranslateX, this.TranslateY);
        }
        public Vex.Point Location
        {
            get
            {
                return new Vex.Point(TranslateX, TranslateY);
            }
            set
            {
                TranslateX = value.X;
                TranslateY = value.Y;
            }
        }

        public bool HasScaleOrRotation()
        {
            return ScaleX != 1 || Rotate0 != 0 || Rotate1 != 0 || ScaleY != 1;
        }

        public System.Drawing.Drawing2D.Matrix GetDrawing2DMatrix()
        {
            return new System.Drawing.Drawing2D.Matrix(
                this.ScaleX,
                this.Rotate0,
                this.Rotate1,
                this.ScaleY,
                this.TranslateX,
                this.TranslateY);
        }
		public MatrixComponents GetMatrixComponents()
		{
			System.Drawing.Drawing2D.Matrix srcMatrix = new System.Drawing.Drawing2D.Matrix(
				this.ScaleX,
				this.Rotate0,
				this.Rotate1,
				this.ScaleY,
				this.TranslateX,
				this.TranslateY);
			System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, 0, 0); // identity
			// an 'identity' box
			PointF[] modPts = new PointF[]{	new PointF(0,0),
											new PointF(1,0),
											new PointF(1,1),
											new PointF(0,1) };

			float sx, sy, rot, shear, tx, ty;

			srcMatrix.TransformPoints(modPts);

			// translation
			tx = srcMatrix.OffsetX;
			ty = srcMatrix.OffsetY;
			m.Translate(-tx, -ty);
			m.TransformPoints(modPts);
			m.Reset();

			// rotation
			rot = (float)Math.Atan2(modPts[1].Y, modPts[1].X); // x axis
			rot = (float)(rot / Math.PI * 180);
			if (rot == -0) rot = 0;
			if (rot == -180) rot = 180;
			m.Rotate(-1 * rot);
			m.TransformPoints(modPts);
			m.Reset();

            // scale
            //sx = Dist(modPts[0], modPts[3]); // bug it seems..?
            sx = Dist(modPts[0], modPts[1]);
			sy = Dist(modPts[0], new PointF(0, modPts[3].Y));
			if (modPts[0].Y > modPts[3].Y)
			{
				sy *= -1;
			}
			m.Scale(1 / sx, 1 / sy);
			m.TransformPoints(modPts);
			m.Reset();

			// skew
			// ySkew is impossible at this rotation
			shear = modPts[3].X / Dist(modPts[0], modPts[1]);
			// rounding
			shear = Math.Abs(shear) < 0.001 ? 0 : shear;
			m.Shear(-shear, 0);
			m.TransformPoints(modPts);

			m.Dispose();
			srcMatrix.Dispose();

			return new MatrixComponents(sx, sy, rot, shear, tx, ty);
		}

		private float Dist(PointF a, PointF b)
		{
			float dx = b.X - a.X;
			float dy = b.Y - a.Y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

		public void Translate(float offsetX, float offsetY)
		{
            TranslateX += offsetX;
            TranslateY += offsetY;
		}
		public void Translate(Vex.Point p)
		{
            TranslateX += p.X;
            TranslateY += p.Y;
		}

		public Matrix Invert()
		{
            return new Matrix(1 / ScaleX, -Rotate0, -Rotate1, 1 / ScaleY, -TranslateX, -TranslateY);
		}



		public override bool Equals(Object o)
		{
			bool result = false;
			if(!(o is Matrix))
			{
				return false;
			}
			Matrix co = (Matrix)o;
			if ((this.ScaleX == co.ScaleX) && (this.ScaleY == co.ScaleY) && 
				(this.Rotate0 == co.Rotate0) && (this.Rotate1 == co.Rotate1) &&
				(this.TranslateX == co.TranslateX) && (this.TranslateY == co.TranslateY))
			{
				result = true;
			}
			return result;
		}
		public bool Equals(Matrix o)
		{
				return	(this.ScaleX == o.ScaleX) && (this.ScaleY == o.ScaleY) &&
						(this.Rotate0 == o.Rotate0) && (this.Rotate1 == o.Rotate1) &&
						(this.TranslateX == o.TranslateX) && (this.TranslateY == o.TranslateY);
		}

		public static bool operator ==(Matrix a, Matrix b)
		{
			return	(a.ScaleX == b.ScaleX) && (a.ScaleY == b.ScaleY) &&
					(a.Rotate0 == b.Rotate0) && (a.Rotate1 == b.Rotate1) &&
					(a.TranslateX == b.TranslateX) && (a.TranslateY == b.TranslateY);
		}

		public static bool operator !=(Matrix a, Matrix b)
		{
			return !(	(a.ScaleX == b.ScaleX) && (a.ScaleY == b.ScaleY) &&
						(a.Rotate0 == b.Rotate0) && (a.Rotate1 == b.Rotate1) &&
						(a.TranslateX == b.TranslateX) && (a.TranslateY == b.TranslateY));
		}

		public override int GetHashCode()
		{
			return (int)((this.ScaleX * 17 + this.ScaleY) * 17 + 
						 (this.Rotate0 * 23 + this.Rotate1) * 23 + 
						 (this.TranslateX * 27 + this.TranslateY) * 27);


		}
		public int CompareTo(Object o)
		{
			int result = 0;
			if (o is Matrix)
			{
				Matrix co = (Matrix)o;
				if (this.ScaleX != co.ScaleX)
				{
					result = (this.ScaleX > co.ScaleX) ? 1 : -1;
				}
				else if (this.ScaleY != co.ScaleY)
				{
					result = (this.ScaleY > co.ScaleY) ? 1 : -1;
				}
				else if (this.Rotate0 != co.Rotate0)
				{
					result = (this.Rotate0 > co.Rotate0) ? 1 : -1;
				}
				else if (this.Rotate1 != co.Rotate1)
				{
					result = (this.Rotate1 > co.Rotate1) ? 1 : -1;
				}
				else if (this.TranslateX != co.TranslateX)
				{
					result = (this.TranslateX > co.TranslateX) ? 1 : -1;
				}
				else if (this.TranslateY != co.TranslateY)
				{
					result = (this.TranslateY > co.TranslateY) ? 1 : -1;
				}
			}
			else
			{
				throw new ArgumentException("Objects being compared are not of the same type");
			}
			return result;
		}
		public override string ToString()
		{
			return	"{mx:" + this.ScaleX +
					"," + this.Rotate0 + "," + this.Rotate1 + 
					"," + this.ScaleY + 
					"," + this.TranslateX + "," + this.TranslateY + "}";
        }
        public string GetSerializedString()
        {
            return this.ScaleX.ToString("0.####") +
                "," + this.Rotate0.ToString("0.####") + "," + this.Rotate1.ToString("0.####") +
                "," + this.ScaleY.ToString("0.####") +
                "," + this.TranslateX.ToString("0.####") + "," + this.TranslateY.ToString("0.####");
        }
        public string GetScaleRotationString()
        {
            return this.ScaleX.ToString("0.####") +
                "," + this.Rotate0.ToString("0.####") + "," + this.Rotate1.ToString("0.####") +
                "," + this.ScaleY.ToString("0.####");
        }
        public void DeserializeString(string s)
        {
            string[] vals = s.Split(new char[] { ',' });
            ScaleX = float.Parse(vals[0], NumberStyles.Any);
            Rotate0 = float.Parse(vals[1], NumberStyles.Any);
            Rotate1 = float.Parse(vals[2], NumberStyles.Any);
            ScaleY = float.Parse(vals[3], NumberStyles.Any);
            TranslateX = float.Parse(vals[4], NumberStyles.Any);
            TranslateY = float.Parse(vals[5], NumberStyles.Any);
        }
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader r)
        {
            string s = r.GetAttribute("Values");
            DeserializeString(s);
            r.Read();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Values", GetSerializedString());
        }
	}
}
