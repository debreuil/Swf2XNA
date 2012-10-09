/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

namespace DDW.Vex
{
    public struct Point : IVexObject, IComparable, IXmlSerializable
    {
        public static Point Empty = new Point(float.NaN, float.NaN);
        public static Point Zero = new Point(0, 0);
		public float X;
		public float Y;
        public bool IsEmpty { get { return float.IsNaN(X) || float.IsNaN(Y); } }

        public Point(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

		public static Point operator +(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		} 

		public override bool Equals(Object o)
		{
			bool result = false;
			if (o is Point && (this.X == ((Point)o).X) && (this.Y == ((Point)o).Y))
			{
				result = true;
			}
			return result;
		}

		public bool Equals(Point o)
		{
			return ((this.X == o.X) && (this.Y == o.Y));
		}

        public Point Negate()
        {
            return new Point(-X, -Y);
        }
        public Point Translate(int offsetX, int offsetY)
        {
            return new Point(X + offsetX, Y + offsetY);
        }
        public Point Translate(Point p)
        {
            return new Point(X + p.X, Y + p.Y);
        }

		public static bool operator ==(Point a, Point b)
		{
			return (a.X == b.X) && (a.Y == b.Y);
		}

		public static bool operator !=(Point a, Point b)
		{
			return !((a.X == b.X) && (a.Y == b.Y));
		}

		public override int GetHashCode()
		{
			return (int)(this.X * 17 + this.Y);
		}


		public int CompareTo(Object o)
		{
			int result = 0;
			if (o is Point)
			{
				Point co = (Point)o;
				if (this.X != co.X)
				{
					result = (this.X > co.X) ? 1 : -1;
				}
				else if (this.Y != co.Y)
				{
					result = (this.Y > co.Y) ? 1 : -1;
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
			return "{x:" + this.X + ",y:" + this.Y + "}";
        }
		public string Serialize()
		{
            return X.ToString("0.####") + "," + Y.ToString("0.####");
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(System.Xml.XmlReader r)
        {
            X = float.Parse(r.GetAttribute("X"), NumberStyles.Any);
            Y = float.Parse(r.GetAttribute("Y"), NumberStyles.Any);
            r.Read();
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("X", X.ToString());
            writer.WriteAttributeString("Y", Y.ToString());
        }
        public string GetSVG()
        {
            return X.ToString("0.##") + "," + Y.ToString("0.##");
        }
	}
}
