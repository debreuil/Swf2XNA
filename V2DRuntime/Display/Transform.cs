using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DDW.V2D;

namespace VexRuntime.Display
{
    public class Transform
    {
        public uint StartFrame;
        public uint EndFrame;
        public Matrix Matrix;
        public float Alpha;
        public Transform()
        {
        }
        public Transform(V2DTransform t)
		{
            StartFrame = t.StartFrame;
            EndFrame = t.EndFrame;
            Matrix = t.Matrix.GetMatrix();
			Alpha = t.Alpha;
		}
        public Transform(uint startFrame, uint endFrame, V2DMatrix matrix, float alpha)
		{
            StartFrame = startFrame;
            EndFrame = endFrame;
            Matrix = matrix.GetMatrix();
			Alpha = alpha;
		}
    }
}
