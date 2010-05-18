using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using Microsoft.Xna.Framework;

namespace V2DRuntime.Tween
{
    public class TweenState
    {
        public Vector2 Position;
        public Vector2 Scale;
        public float Rotation;
        public float Alpha;
        public uint Frame;

        //public float Color;
        //public float speed, friction, restitution etc;

        public TweenState() { }
        public TweenState(Vector2 position, float rotation, Vector2 scale, float alpha, uint frame)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
            this.Alpha = alpha;
            this.Frame = frame;
        }

        public TweenState(DisplayObject obj)
        {
            this.Position = obj.Position;
            this.Rotation = obj.Rotation;
            this.Scale = obj.Scale;
            this.Alpha = obj.Alpha;

            if (obj is DisplayObjectContainer)
            {
                this.Frame = ((DisplayObjectContainer)obj).CurChildFrame;
            }
            else
            {
                this.Frame = 0;
            }
        }
    }
}
