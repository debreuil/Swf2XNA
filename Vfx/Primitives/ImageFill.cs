/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
namespace DDW.Vex
{
	public class ImageFill : FillStyle
	{
		public override FillType FillType { get { return FillType.Image; } }

		public string ImagePath;
		public Matrix Matrix;
		public bool IsSmooth = false;
		public bool IsTiled = false;


		public override bool Equals(Object o)
		{
			bool result = false;
			if (o is ImageFill &&
				(this.ImagePath == ((ImageFill)o).ImagePath) &&
				(this.IsSmooth == ((ImageFill)o).IsSmooth) &&
				(this.IsTiled == ((ImageFill)o).IsTiled) &&
				(this.Matrix == ((ImageFill)o).Matrix)
				)
			{
				result = true;
			}
			return result;
		}

		public bool Equals(ImageFill o)
		{
			return (
				(this.ImagePath == o.ImagePath) &&
				(this.IsSmooth == o.IsSmooth) &&
				(this.IsTiled == o.IsTiled) &&  
				(this.Matrix == o.Matrix) );
		}

		public static bool operator ==(ImageFill a, ImageFill b)
		{
			return 
				(a.ImagePath == b.ImagePath) &&
				(a.IsSmooth == b.IsSmooth) &&
				(a.IsTiled == b.IsTiled) && 
				(a.Matrix == b.Matrix);
		}

		public static bool operator !=(ImageFill a, ImageFill b)
		{
			return !(
				(a.ImagePath == b.ImagePath) && 
				(a.IsSmooth == b.IsSmooth) &&
				(a.IsTiled == b.IsTiled) && 
				(a.Matrix == b.Matrix));
		}

		public override int GetHashCode()
		{
			return (int)(
				this.ImagePath.GetHashCode() + 
				this.Matrix.GetHashCode() + 
				(this.IsSmooth ? 7 : 17) +
				(this.IsTiled ? 27 : 37));
		}



		public override int CompareTo(Object o)
		{
			int result = 0;

			if (o is FillStyle && this.FillType != ((FillStyle)o).FillType)
			{
				result = (this.FillType > ((FillStyle)o).FillType) ? 1 : -1;
			}
			else if (o is ImageFill)
			{
				ImageFill bf = (ImageFill)o;
				if (this.ImagePath != bf.ImagePath)
				{
					result = (this.ImagePath.CompareTo(bf.ImagePath));
				}
				if (this.IsSmooth != bf.IsSmooth)
				{
					result = (this.IsSmooth) ? 1 : -1;
				}
				if (this.IsTiled != bf.IsTiled)
				{
					result = (this.IsTiled) ? 1 : -1;
				}
				else
				{
					result = this.Matrix.CompareTo(bf.Matrix);
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
			return "bmp: " + this.ImagePath;
		}
	}
}
