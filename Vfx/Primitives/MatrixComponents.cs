/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class MatrixComponents
	{
		public float ScaleX;
		public float ScaleY;
		public float Rotation;
		public float Shear;
		public float TranslateX;
		public float TranslateY;
		public MatrixComponents(float scaleX, float scaleY, float rotation, float shear, float translateX, float translateY)
		{
			this.ScaleX = scaleX;
			this.ScaleY = scaleY;
			this.Rotation = rotation;
			this.Shear = shear;
			this.TranslateX = translateX;
			this.TranslateY = translateY;
		}
	}
}
