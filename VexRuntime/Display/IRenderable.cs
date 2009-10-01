using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DDW.Display
{
    interface IRenderable : IDrawable
    {
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        float Rotation { get; set; }
        Vector2 Scale { get; set; }
        Color Color { get; set; }
        float LayerDepth { get; set; }
    }
}
