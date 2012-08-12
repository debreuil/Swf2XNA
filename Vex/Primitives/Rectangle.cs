/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

namespace DDW.Vex
{
	public struct Rectangle : IVexObject, IXmlSerializable
	{
		public static readonly Rectangle Empty = new Rectangle(0, 0, 0, 0);

		public Point Point;
		public Size Size;
		public Rectangle(Point point, Size size)
		{
			this.Point = point;
			this.Size = size;
		}
		public Rectangle(float x, float y, float w, float h)
		{
			this.Point = new Point(x, y);
			this.Size = new Size(w, h);
		}

        public Rectangle Clone() { return new Rectangle(Point, Size); }

        public float Left { get { return Point.X; } }
        public float Top { get { return Point.Y; } }
        public float Width { get { return Size.Width; } }
        public float Height { get { return Size.Height; } }

        public float Right { get { return Point.X + Size.Width; } }
        public float Bottom { get { return Point.Y + Size.Height; } }

        public Point Center { get { return new Point(Left + Width / 2f, Top + Height / 2f); } }
        public Point LocalCenter { get { return new Point(Width / 2f, Height / 2f); } }

        public bool IsEmpty { get { return this == Empty; } }
        public Rectangle Intersect(Rectangle r)
        {
            float lf = Math.Max(this.Point.X, r.Point.X);
            float tp = Math.Max(this.Point.Y, r.Point.Y);
            float rt = Math.Min(this.Point.X + this.Size.Width, r.Point.X + r.Size.Width);
            float bt = Math.Min(this.Point.Y + this.Size.Height, r.Point.Y + r.Size.Height);
            return new Rectangle(lf, tp, rt - lf, bt - tp);
        }
        public Rectangle Union(Rectangle r)
        {
            float lf = Math.Min(this.Point.X, r.Point.X);
            float tp = Math.Min(this.Point.Y, r.Point.Y);
            float rt = Math.Max(this.Point.X + this.Size.Width, r.Point.X + r.Size.Width);
            float bt = Math.Max(this.Point.Y + this.Size.Height, r.Point.Y + r.Size.Height);
            return new Rectangle(lf, tp, rt - lf, bt - tp);
        }
        public Rectangle TranslatedRectangle(float x, float y)
        {
            return new Rectangle(Point.X + x, Point.Y + y, Width, Height);
        }
        public System.Drawing.Rectangle SystemRectangle { get { return new System.Drawing.Rectangle((int)Point.X, (int)Point.Y, (int)Size.Width, (int)Size.Height); } }
		public override bool Equals(Object o)
		{
			bool result = false;
			if (o is Rectangle && (this.Point == ((Rectangle)o).Point) && (this.Size == ((Rectangle)o).Size))
			{
				result = true;
			}
			return result;
		}
		public bool Equals(Rectangle o)
		{
			return ((this.Point == o.Point) && (this.Size == o.Size));
		}

		public static bool operator ==(Rectangle a, Rectangle b)
		{
			return (a.Point == b.Point) && (a.Size == b.Size);
		}

		public static bool operator !=(Rectangle a, Rectangle b)
		{
			return !((a.Point == b.Point) && (a.Size == b.Size));
		}

		public override int GetHashCode()
		{
			return (int)(this.Point.GetHashCode() * 17 + this.Size.GetHashCode());
		}
		public int CompareTo(Object o)
		{
			int result = 0;
			if (o is Rectangle)
			{
				Rectangle co = (Rectangle)o;
				if (this.Point != co.Point)
				{
					result = this.Point.CompareTo(co.Point);
				}
				else if (this.Size != co.Size)
				{
					result = this.Size.CompareTo(co.Size);
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
			return "[r:" + this.Point + "-" + this.Size + "]";
		}

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader r)
        {
            float x = float.Parse(r.GetAttribute("X"), NumberStyles.Any);
            float y = float.Parse(r.GetAttribute("Y"), NumberStyles.Any);
            float w = float.Parse(r.GetAttribute("Width"), NumberStyles.Any);
            float h = float.Parse(r.GetAttribute("Height"), NumberStyles.Any);
            Point = new Point(x, y);
            Size = new Size(w, h);
            r.Read();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("X", Left.ToString());
            writer.WriteAttributeString("Y", Top.ToString());
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString()); 
        }
    }
}
