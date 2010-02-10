using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace DDW.V2D
{
	public class V2DTransform
	{
		// color transform
		// filters
		// easing
		// character script
        //public ColorTransform ColorTransform; // this will eventually hold full color transforms

		public uint StartFrame;
		public uint EndFrame;

        //public V2DMatrix Matrix;

        public float ScaleX;
        public float ScaleY;
        public float Rotation;
        public float TranslationX;
        public float TranslationY;

        public float Alpha;
        public bool IsTweening = false;


        public V2DTransform()
        {
        }
        public V2DTransform(uint startFrame, uint endFrame, float scaleX, float scaleY, float rotation, float translationX, float translationY, float alpha)
        {
            StartFrame = startFrame;
            EndFrame = endFrame;
            ScaleX = scaleX;
            ScaleY = scaleY;
            Rotation = rotation;
            TranslationX = translationX;
            TranslationY = translationY;
            Alpha = alpha;
        }
        //public V2DTransform(uint startFrame, uint endFrame, V2DMatrix matrix, float alpha)
        //{
        //    StartFrame = startFrame;
        //    EndFrame = endFrame;
        //    Matrix = matrix;
        //    Alpha = alpha;
        //}

        //public bool HasMatrix()
        //{
        //    return !(this.Matrix.Equals(V2DMatrix.Empty));
        //}
		public bool HasAlpha()
		{
			return this.Alpha != 1F;
		}
        public V2DTransform Clone()
        {
            return new V2DTransform(StartFrame, EndFrame, ScaleX, ScaleY, Rotation, TranslationX, TranslationY, Alpha);
        }
        //public V2DTransform Clone()
		//{
        //    return new V2DTransform(StartFrame, EndFrame, Matrix, Alpha);
		//}
	}
}
