/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DDW.Vex
{
	public class Transform
	{
		// color transform
		// filters
		// easing
		// character script

        public static Transform Identity = new Transform();

        [DefaultValue(0)]
        public uint StartTime;
        [DefaultValue(1)]
        public uint EndTime;
        [DefaultValue(1)]
        public float Alpha; // this will eventually hold full color transforms
        [DefaultValue(false)]
        public bool IsTweening = false;

        public Matrix Matrix = Matrix.Identity;
        public ColorTransform ColorTransform = ColorTransform.Identity;

        public bool ShouldSerializeMatrix()
        {
            return Matrix != Matrix.Identity;
        }
        public bool ShouldSerializeColorTransform()
        {
            return !ColorTransform.IsIdentity();
        }

        public Transform()
        {
        }
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
	}
}
