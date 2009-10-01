using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VexRuntime.V2D
{
    public struct V2DRectangle
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public V2DRectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
    }
}
