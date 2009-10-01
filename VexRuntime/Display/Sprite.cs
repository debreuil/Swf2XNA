using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using DDW.V2D;

namespace DDW.Display
{
    public class Sprite : DisplayObjectContainer, IRenderable
    {
        public Sprite(Texture2D texture)
        {
            Texture = texture;
        }
        public Sprite(Texture2D texture, V2DInstance inst)
        {
            Texture = texture;
            this.X = (int)inst.X;
            this.Y = (int)inst.Y;
            this.Rotation = inst.Rotation;
            this.Alpha = inst.Alpha;
            this.Scale = new Vector2(inst.ScaleX, inst.ScaleY);
            this.Visible = inst.Visible;
            this.FrameCount = inst.Definition.FrameCount;
            this.StartFrame = inst.StartFrame;
            this.TotalFrames = inst.TotalFrames;
        }

    }
}
