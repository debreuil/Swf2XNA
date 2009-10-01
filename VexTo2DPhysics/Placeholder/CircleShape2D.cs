/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using DDW.Vex;
using DDW.V2D;

namespace DDW.Placeholder
{
    public class CircleShape2D : Shape2D
    {
        public Point Center;
        public float Radius;

        public CircleShape2D(string containerName, Point center, float radius)
        {
            this.ContainerName = containerName;
            this.Center = center;
            this.Radius = radius;
        }

        public override void EnsureClockwise(List<IShapeData> pts)
        {
        }
        public override bool IsPointInside(Point p)
        {
            float xd = p.X - Center.X;
            float yd = p.Y - Center.Y;
            float d = (float)Math.Sqrt(xd * xd + yd * yd);
            return d <= Radius;
        }
        public override void Dump(StringWriter sw)
        {
            sw.Write("{");
            sw.Write("center:" + Center.ToString() + ", ");
            sw.Write("radius:" + Radius.ToString());
            sw.Write("}");
        }
        public override void Dump(XmlWriter xw)
        {
            xw.WriteStartElement("V2DCircle");
            xw.WriteAttributeString("CenterX", Center.X.ToString());
            xw.WriteAttributeString("CenterY", Center.Y.ToString());
            xw.WriteAttributeString("Radius", Radius.ToString());
            xw.WriteEndElement(); // V2DCircle
        }
        public override V2DShape GetV2DShape()
        {
            V2DShape result = new V2DShape();
            result.IsCircle = true;
            result.CenterX = Center.X;
            result.CenterY = Center.Y;
            result.Radius = Radius;
            return result;
        }
    }
}