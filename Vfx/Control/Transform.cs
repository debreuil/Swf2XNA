/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class Transform
	{
		// color transform
		// filters
		// easing
		// character script

		public uint StartTime;
		public uint EndTime;
		public Matrix Matrix;
        public float Alpha; // this will eventually hold full color transforms
        public bool IsTweening = false;
        public ColorTransform ColorTransform;

		public Transform(uint startTime, uint endTime, Matrix matrix, float alpha, ColorTransform colorTransform)
		{
			this.StartTime = startTime;
			this.EndTime = endTime;
			this.Matrix = matrix;
			this.Alpha = alpha;
            this.ColorTransform = colorTransform;
		}
		public bool HasMatrix()
		{
			return this.Matrix != Matrix.Empty;
		}
		public bool HasAlpha()
		{
			return this.Alpha != 1F;
		}
        public Point Location
        {
            get
            {
                return new Point(Matrix.TranslateX, Matrix.TranslateY);
            }
            set
            {
                Matrix = new Matrix(Matrix.ScaleX, Matrix.Rotate0, Matrix.Rotate1, Matrix.ScaleY, value.X, value.Y); 
            }
        }
	}
}
