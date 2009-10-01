/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class GradientFill : FillStyle
	{
		public List<Color> Fills = new List<Color>();
		public List<float> Stops = new List<float>();
		public GradientType GradientType = GradientType.Linear;
		public Rectangle Rectangle;
		public Matrix Transform;

        public static readonly Rectangle GradientVexRect =
            new Rectangle(-16384 / 20F, -16384 / 20F, 32768 / 20F, 32768 / 20F);

		public override FillType FillType 
		{ 
			get 
			{
				if (GradientType == GradientType.Radial)
				{
					return FillType.Radial;
				}
				else
				{
					return FillType.Linear;
				}
			} 
		}

		public override bool Equals(Object o)
		{
			bool result = false;
			if (this.CompareTo(o) == 0)
			{
				result = true;
			}
			return result;
		}

		public bool Equals(GradientFill o)
		{
			return (this.CompareTo(o) == 0);
		}

		public static bool operator ==(GradientFill a, GradientFill b)
		{
			return (a.CompareTo(b) == 0);
		}

		public static bool operator !=(GradientFill a, GradientFill b)
		{
			return !(a.CompareTo(b) == 0);
		}

		public override int GetHashCode()
		{
			int result = (int)this.FillType * 17;
			result += (int)this.GradientType;
			for (int i = 0; i < this.Fills.Count; i++)
			{
				result += this.Fills[i].GetHashCode();
				result += (int)(this.Stops[i] * 17);
			}
			result += this.Rectangle.GetHashCode();
			result += this.Transform.GetHashCode();
			return result;
		}


		public override int CompareTo(Object o)
		{
			int result = 0;

			if (o is FillStyle && this.FillType != ((FillStyle)o).FillType)
			{
				result = (this.FillType > ((FillStyle)o).FillType) ? 1 : -1;
			}
			else if (o is GradientFill)
			{
				GradientFill co = (GradientFill)o;
				if ((int)this.GradientType != (int)co.GradientType)
				{
					result = ((int)this.GradientType > (int)co.GradientType) ? 1 : -1;
				}
				else if (this.Fills.Count != co.Fills.Count)
				{
					result = (this.Fills.Count > co.Fills.Count) ? 1 : -1;
				}
				else if (this.Stops.Count != co.Stops.Count)
				{
					result = (this.Stops.Count > co.Stops.Count) ? 1 : -1;
				}
				else if (this.Stops.Count != this.Fills.Count)
				{
					result = -1;
				}
				else if (co.Stops.Count != co.Fills.Count)
				{
					result = 1;
				}
				else
				{
					for (int i = 0; i < this.Fills.Count; i++)
					{
						if (this.Fills[i] != co.Fills[i])
						{
							result = this.Fills[i].CompareTo(co.Fills[i]);
							break;
						}
						if (this.Stops[i] != co.Stops[i])
						{
							result = this.Fills[i].CompareTo(co.Fills[i]);
							break;
						}
					}
					if (result == 0)
					{
						if (this.Rectangle != co.Rectangle)
						{
							result = this.Rectangle.CompareTo(co.Rectangle);
						}
						else if (this.Transform != co.Transform)
						{
							result = this.Transform.CompareTo(co.Transform);
						}
					}
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
			StringBuilder result = new StringBuilder();
			result.Append("t: " + Enum.GetName(typeof(GradientType), this.GradientType));
			for (int i = 0; i < this.Fills.Count; i++)
			{
				result.Append(" f:" + this.Fills[i]);
				if (this.Stops.Count > i)
				{
					result.Append(" r:" + this.Stops[i]);
				}
			}
			result.Append(this.Rectangle);
			result.Append(this.Transform);

			return result.ToString();
		}
	}
}
