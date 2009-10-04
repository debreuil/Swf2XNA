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
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        float Rotation { get; set; }
        Vector2 Scale { get; set; }
        Color Color { get; set; }
        float Depth { get; set; }
    }
}
