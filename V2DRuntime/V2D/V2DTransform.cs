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

        public Vector2 Position;
        public Vector2 Origin;
        public Vector2 Scale;
        public float Rotation;

		//public float ScaleX;
		//public float ScaleY;
		//public float Rotation;
		//public float TranslationX;
		//public float TranslationY;

        public float Alpha;
        public bool IsTweening = false;
        public bool Visible = true;


        public V2DTransform()
        {
        }
        public V2DTransform(uint startFrame, uint endFrame, float scaleX, float scaleY, float rotation, float translationX, float translationY, float alpha)
        {
            StartFrame = startFrame;
            EndFrame = endFrame;
            Scale.X = scaleX;
            Scale.Y = scaleY;
            Rotation = rotation;
			Position.X = translationX;
			Position.Y = translationY;
            Alpha = alpha;
        }
		public bool HasAlpha()
		{
			return this.Alpha != 1F;
		}
        public V2DTransform Clone()
        {
			return new V2DTransform(StartFrame, EndFrame, Scale.X, Scale.Y, Rotation, Position.X, Position.Y, Alpha);
        }

        public static Vector2 GetVector2(V2DVector2 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static V2DVector2 GetV2DVector2(Vector2 v)
        {
            return new V2DVector2(v.X, v.Y);
        }

        public static V2DTransform[] GetV2DTransformArray(V2DGenericTransform[] t)
        {
            V2DTransform[] result = new V2DTransform[t.Length];
            for (int i = 0; i < t.Length; i++)
            {
                result[i] = t[i];
            }
            return result;
        }
        public static implicit operator V2DTransform(V2DGenericTransform t)
        {
            V2DTransform result = new V2DTransform(t.StartFrame, t.EndFrame, t.Scale.X, t.Scale.Y, t.Rotation, t.Position.X, t.Position.Y, t.Alpha);
            result.IsTweening = t.IsTweening;
            result.Origin = GetVector2(t.Origin);
            result.Visible = t.Visible;
            return result;
        }
        public static implicit operator V2DGenericTransform(V2DTransform t)
        {
            V2DGenericTransform result = new V2DGenericTransform(t.StartFrame, t.EndFrame, t.Scale.X, t.Scale.Y, t.Rotation, t.Position.X, t.Position.Y, t.Alpha);
            result.IsTweening = t.IsTweening;
            result.Origin = GetV2DVector2(t.Origin);
            result.Visible = t.Visible;
            return result;
        }

	}
}
