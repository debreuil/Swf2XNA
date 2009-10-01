/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public struct Point : IVexObject, IComparable
	{
        public static Point Empty = new Point(float.NaN, float.NaN);
		public float X;
		public float Y;

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
	}
}
