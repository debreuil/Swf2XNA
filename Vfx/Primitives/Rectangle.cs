/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public struct Rectangle : IVexObject
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

        public float Left { get { return Point.X; } }
        public float Top { get { return Point.Y; } }
        public float Width { get { return Size.Width; } }
        public float Height { get { return Size.Height; } }

        public bool IsEmpty { get { return this == Empty; } }
		public Rectangle Union(Rectangle r)
		{
			float lf = Math.Min(this.Point.X, r.Point.X);
			float tp = Math.Min(this.Point.Y, r.Point.Y);
			float rt = Math.Max(this.Point.X + this.Size.Width, r.Point.X + r.Size.Width);
			float bt = Math.Max(this.Point.Y + this.Size.Height, r.Point.Y + r.Size.Height);
			return new Rectangle(lf, tp, rt-lf, bt-tp);
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
	}
}
