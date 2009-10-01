/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using DDW.Vex;
using DDW.V2D;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DDW.Placeholder
{
    public class PolygonShape2D : Shape2D
    {
        public List<Point> Points = new List<Point>();
        //public PolygonShape2D(string containerName, List<Point> points)
        //{
        //    this.ContainerName = containerName;
        //    this.Points = points;
        //}
        public PolygonShape2D(string containerName, Shape sh, Matrix m)
        {
            this.ContainerName = containerName;
            List<IShapeData> sd = sh.ShapeData;
            EnsureClockwise(sd);

            System.Drawing.PointF[] pts = new System.Drawing.PointF[ sd.Count];
            for (int i = 0; i < sd.Count; i++)
            {
                pts[i] = new System.Drawing.PointF(sd[i].StartPoint.X, sd[i].StartPoint.Y);
            }
            System.Drawing.Drawing2D.Matrix m2 = m.GetDrawing2DMatrix();
            m2.TransformPoints(pts);
            for (int i = 0; i < pts.Length; i++)
            {
                Points.Add(new Point(pts[i].X, pts[i].Y));
            }
        }

        public override bool IsPointInside(Point p)
        {
            bool result = true;
            for (int i = 1; i < Points.Count; i++)
            {
                Point a = Points[i - 1];
                Point b = Points[i];
                if (IsToLeft(p, a, b))
                {
                    result = false;
                    break;
                }
            }
            if (result)
            {
                result = !IsToLeft(p, Points[Points.Count - 1], Points[0]);
            }
            return result;
        }
        private bool IsToLeft(Point p, Point a, Point b)
        {
            float r = (p.Y - a.Y) * (b.X - a.X) - (p.X - a.X) * (b.Y - a.Y);
            return (r <= 0);
        }

        public override void EnsureClockwise(List<IShapeData> points)
        {
            // test for ccw
            Point a = points[0].StartPoint;
            Point b = points[0].EndPoint;
            Point c = points[1].EndPoint;
            float area = a.X * b.Y - a.Y * b.X +
                         a.Y * c.X - a.X * c.Y +
                         b.X * c.Y - c.X * b.Y;
            bool isCCW = area < 0;

            if (isCCW)
            {
                points.Reverse();
                for (int i = 0; i < points.Count; i++)
                {
                    points[i].Reverse();
                }
            }
        }

        public override void Dump(StringWriter sw)
        {
                sw.Write("[");
                string comma = "";
                for (int i = 0; i < Points.Count; i++)
                {
                    sw.Write(comma +
                        Points[i].X + "," +
                        Points[i].Y);
                    comma = ", ";
                }
                sw.Write("]");
        }
        public override void Dump(XmlWriter xw)
        {
            xw.WriteStartElement("V2DPolygon");
            string s = "";
            string comma = "M ";
            for (int i = 0; i < Points.Count; i++)
            {
                s+= comma + Points[i].X + "," + Points[i].Y;
                comma = " L ";
            }
            xw.WriteAttributeString("Data", s);
            xw.WriteEndElement(); // V2DPolygon
        }
        public override V2DShape GetV2DShape()
        {
            V2DShape result = new V2DShape();
            float[] pts = new float[Points.Count * 2];
            for (int i = 0; i < Points.Count; i++)
            {
                pts[i * 2] = Points[i].X;
                pts[i * 2 + 1] = Points[i].Y;
            }
            result.Data = pts;
            result.EnsureClockwise();
            //List<Vector2> pts = new List<Vector2>();
            //for (int i = 0; i < Points.Count; i++)
            //{
            //    pts.Add(new Vector2(Points[i].X, Points[i].Y));
            //}
            //result.Data = pts.ToArray();
            result.IsCircle = false;
            return result;
        }

        public void EnsureClockwise()
        {
            // test for ccw
            Point a = Points[0];
            Point b = Points[1];
            Point c = Points[2];
            float area = a.X * b.Y - a.Y * b.X +
                         a.Y * c.X - a.X * c.Y +
                         b.X * c.Y - c.X * b.Y;
            bool isCCW = area < 0;

            if (isCCW)
            {
                Points.Reverse();
            }
        }
    }
}
