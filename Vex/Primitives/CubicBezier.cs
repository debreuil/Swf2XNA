/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public struct CubicBezier : IShapeData
	{
		public Point Anchor0;
		public Point Control0;
		public Point Control1;
		public Point Anchor1;

		public CubicBezier(Point anchor0, Point control0, Point control1, Point anchor1)
		{
			this.Anchor0 = anchor0;
			this.Control0 = control0;
			this.Control1 = control1;
			this.Anchor1 = anchor1;
		}
		public SegmentType SegmentType { get { return SegmentType.CubicBezier; } }
		public Point StartPoint { get { return Anchor0; } }
		public Point EndPoint { get { return Anchor1; } }

        public void CheckExtremas(ref float left, ref float right, ref float top, ref float bottom)
        {
            if (Anchor0.X < left) left = Anchor0.X;
            if (Control0.X < left) left = Control0.X;
            if (Control1.X < left) left = Control1.X;
            if (Anchor1.X < left) left = Anchor1.X;

            if (Anchor0.X > right) right = Anchor0.X;
            if (Control0.X > right) right = Control0.X;
            if (Control1.X > right) right = Control1.X;
            if (Anchor1.X > right) right = Anchor1.X;

            if (Anchor0.Y < top) top = Anchor0.Y;
            if (Control0.Y < top) top = Control0.Y;
            if (Control1.Y < top) top = Control1.Y;
            if (Anchor1.Y < top) top = Anchor1.Y;

            if (Anchor0.Y > bottom) bottom = Anchor0.Y;
            if (Control0.Y > bottom) bottom = Control0.Y;
            if (Control1.Y > bottom) bottom = Control1.Y;
            if (Anchor1.Y > bottom) bottom = Anchor1.Y;
        }
		public void Reverse()
		{
			Point temp = this.Anchor0;
			this.Anchor0 = this.Anchor1;
			this.Anchor1 = temp;
			temp = this.Control0;
			this.Control0 = this.Control1;
			this.Control1 = temp;
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
				else if (o is CubicBezier)
				{
					CubicBezier cb = (CubicBezier)o;
					if (this.Control0 != cb.Control0)
					{
						result = this.Control0.CompareTo(cb.Control0);
					}
					else if (this.Control1 != cb.Control1)
					{
						result = this.Control1.CompareTo(cb.Control1);
					}
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
			return "{CB:" + this.Anchor0 + ",.,.," + this.Anchor1 + "}";
        }
        public string Serialize()
        {
            return "C" + Anchor0.Serialize() + "," + Control0.Serialize() + "," + Control1.Serialize() + "," + Anchor1.Serialize() + " ";
        }
        public static CubicBezier Deserialize(string s)
        {
            string[] ar = s.Substring(1).Split(new char[] { ',' });
            return new CubicBezier(
                new Point(float.Parse(ar[0], System.Globalization.NumberStyles.Any), float.Parse(ar[1], System.Globalization.NumberStyles.Any)),
                new Point(float.Parse(ar[2], System.Globalization.NumberStyles.Any), float.Parse(ar[3], System.Globalization.NumberStyles.Any)),
                new Point(float.Parse(ar[4], System.Globalization.NumberStyles.Any), float.Parse(ar[5], System.Globalization.NumberStyles.Any)),
                new Point(float.Parse(ar[5], System.Globalization.NumberStyles.Any), float.Parse(ar[7], System.Globalization.NumberStyles.Any)));
        }
	}
}
