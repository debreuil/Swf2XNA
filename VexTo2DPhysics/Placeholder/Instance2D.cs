/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DDW.Vex;

namespace DDW.Placeholder
{
    /// <summary>
    /// Temporary holder for conversion to V2DInstance
    /// </summary>
    public class Instance2D
    {
        public string InstanceName;
        public string DefinitionName;
        public float Depth;
        public float X = 0;
        public float Y = 0;
        public float Rotation = 0;
        public float ScaleX = 1;
        public float ScaleY = 1;
        public float Alpha = 1;
        public bool Visible = true;
        public Matrix Matrix;
        public List<Transform> Transforms = new List<Transform>();

        public uint StartFrame;
        public uint TotalFrames;

        public float Density = 2.0f;
        public float Friction;
        public float Restitution;

        public Definition2D Definition;


        public Instance2D(string name, string type, float x, float y, float rotation, float xScale, float yScale)
        {
            this.InstanceName = name;
            this.DefinitionName = type;
            this.X = x;
            this.Y = y;
            this.Rotation = rotation;
            this.ScaleX = xScale;
            this.ScaleY = yScale;
        }

        public virtual bool IsPointInside(Point p)
        {
            bool result = false;
            System.Drawing.Drawing2D.Matrix m =
                this.Transforms[0].Matrix.GetDrawing2DMatrix();
            m.Invert();
            System.Drawing.PointF[] p2 = new System.Drawing.PointF[] { new System.Drawing.PointF(p.X, p.Y) };
            m.TransformPoints(p2);
            Point tp = new Point(p2[0].X, p2[0].Y);
            for (int i = 0; i < this.Definition.Shapes.Count; i++)
            {
                if (this.Definition.Shapes[i].IsPointInside(tp))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public void Dump(StringWriter sw)
        { 
            sw.WriteLine("            {" +
                "instName: \"" + InstanceName + 
                "\", type:\"" + DefinitionName + 
                "\", x:" + X + 
                ", y:" + Y + 
                ", rot:" + Rotation + 
                ", xScale:" + ScaleX + 
                ", yScale:" + ScaleY + "},");       
        }

    }
}
