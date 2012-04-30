/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public struct Size : IVexObject
	{
        public static Size Empty = new Size(0, 0);
		public float Width;
		public float Height;

		public Size(float w, float h)
		{
			this.Width = w;
			this.Height = h;
		}

		public override bool Equals(Object o)
		{
			bool result = false;
			if (o is Size && (this.Width == ((Size)o).Width) && (this.Height == ((Size)o).Height))
			{
				result = true;
			}
			return result;
		}

		public bool Equals(Size o)
		{
			return ((this.Width == o.Width) && (this.Height == o.Height));
		}

		public static bool operator ==(Size a, Size b)
		{
			return (a.Width == b.Width) && (a.Height == b.Height);
		}

		public static bool operator !=(Size a, Size b)
		{
			return !((a.Width == b.Width) && (a.Height == b.Height));
		}

		public override int GetHashCode()
		{
			return (int)(this.Width * 17 + this.Height);
		}


		public int CompareTo(Object o)
		{
			int result = 0;
			if (o is Size)
			{
				Size co = (Size)o;
				if (this.Width != co.Width)
				{
					result = (this.Width > co.Width) ? 1 : -1;
				}
				else if (this.Height != co.Height)
				{
					result = (this.Height > co.Height) ? 1 : -1;
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
			return "[s:" + this.Width + "," + this.Height + "]";
		}
	}
}
