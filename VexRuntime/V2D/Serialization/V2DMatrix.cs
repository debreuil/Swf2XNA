using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DDW.V2D
{
    public struct V2DMatrix
    {
        public static readonly V2DMatrix Empty = new V2DMatrix(0, 0, 0, 0, 0, 0);
        public static readonly V2DMatrix Identity = new V2DMatrix(1, 0, 0, 1, 0, 0);

        public float ScaleX;
        public float ScaleY;
        public float Rotate0;
        public float Rotate1;
        public float TranslateX;
        public float TranslateY;

        public V2DMatrix(float scaleX, float rotate0, float rotate1, float scaleY, float translateX, float translateY)
        {
            this.ScaleX = scaleX;
            this.Rotate0 = rotate0;
            this.Rotate1 = rotate1;
            this.ScaleY = scaleY;
            this.TranslateX = translateX;
            this.TranslateY = translateY;
        }

        public Matrix GetMatrix()
        {
            Matrix result = Matrix.Identity;
            result.M11 = ScaleX;
            result.M12 = Rotate0;
            result.M21 = Rotate1;
            result.M22 = ScaleY;
            result.M31 = TranslateX;
            result.M32 = TranslateY;
            result.M33 = 1;

            return result;
        }
    }
}
