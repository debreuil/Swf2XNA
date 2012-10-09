/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public struct Line : IShapeData
	{
		public Point Anchor0;
        public Point Anchor1;
        public Line(Point anchor0, Point anchor1)
        {
            this.Anchor0 = anchor0;
            this.Anchor1 = anchor1;
        }
        public Line(float f0, float f1, float f2, float f3)
        {
            this.Anchor0 = new Point(f0, f1);
            this.Anchor1 = new Point(f2, f3);
        }
		public SegmentType SegmentType { get { return SegmentType.Line; } }
		public Point StartPoint { get { return Anchor0; } }
		public Point EndPoint { get { return Anchor1; } }

        public void CheckExtremas(ref float left, ref float right, ref float top, ref float bottom)
        {
            if (Anchor0.X < left) left = Anchor0.X;
            if (Anchor1.X < left) left = Anchor1.X;

            if (Anchor0.X > right) right = Anchor0.X;
            if (Anchor1.X > right) right = Anchor1.X;

            if (Anchor0.Y < top) top = Anchor0.Y;
            if (Anchor1.Y < top) top = Anchor1.Y;

            if (Anchor0.Y > bottom) bottom = Anchor0.Y;
            if (Anchor1.Y > bottom) bottom = Anchor1.Y;
        }
		public void Reverse()
		{
			Point temp = this.Anchor0;
			this.Anchor0 = this.Anchor1;
			this.Anchor1 = temp;
		}

		public override bool Equals(Object o)
		{
			bool result = false;
			if (o is Line && (this.Anchor0 == ((Line)o).Anchor0) && (this.Anchor1 == ((Line)o).Anchor1))
			{
				result = true;
			}
			return result;
		}

		public bool Equals(Line o)
		{
			return ((this.Anchor0 == o.Anchor0) && (this.Anchor1 == o.Anchor1));
		}

		public static bool operator ==(Line a, Line b)
		{
			return (a.Anchor0 == b.Anchor0) && (a.Anchor1 == b.Anchor1);
		}

		public static bool operator !=(Line a, Line b)
		{
			return !((a.Anchor0 == b.Anchor0) && (a.Anchor1 == b.Anchor1));
		}

		public override int GetHashCode()
		{
			return (int)(this.Anchor0.GetHashCode() * 17 + this.Anchor1.GetHashCode());
		}
		public int CompareTo(Object o)
		{
			int result = 0;
			if (o is IShapeData)
			{
				IShapeData co = (IShapeData)o;
				if (this.StartPoint != co.StartPoint)
				{
					result = this.StartPoint.CompareTo(co.StartPoint);
				}
				else if (this.EndPoint != co.EndPoint)
				{
					result = this.EndPoint.CompareTo(co.EndPoint);
				}
				else if (this.SegmentType != co.SegmentType)
				{
					result = ((int)this.SegmentType) > ((int)co.SegmentType) ? 1 : -1;
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
			return "{L:" + this.Anchor0 + "," + this.Anchor1 + "}";
		}
        public string Serialize()
        {
            return "L" + Anchor0.Serialize() + "," + Anchor1.Serialize() + " ";
        }
        public static Line Deserialize(string s)
        {
            string[] ar = s.Substring(1).Split(new char[] { ',' });
            return new Line(
                float.Parse(ar[0], System.Globalization.NumberStyles.Any),
                float.Parse(ar[1], System.Globalization.NumberStyles.Any),
                float.Parse(ar[2], System.Globalization.NumberStyles.Any),
                float.Parse(ar[3], System.Globalization.NumberStyles.Any));
        }
	}
}
