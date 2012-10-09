using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vex = DDW.Vex;
using System.Drawing.Drawing2D;

namespace DDW.Utils
{
    public static class VexExtentions
    {
        public static Rectangle SysRectangle(this Vex.Rectangle r)
        {
            return new Rectangle(
                (int)Math.Floor(r.Left),
                (int)Math.Floor(r.Top),
                (int)Math.Ceiling(r.Width),
                (int)Math.Ceiling(r.Height));
        }
        public static RectangleF SysRectangleF(this Vex.Rectangle r)
        {
            return new RectangleF(r.Left, r.Top, r.Width, r.Height);
        }
        public static PointF[] SysPointFs(this Vex.Rectangle r)
        {
            PointF[] result = new PointF[4];

            result[0] = new PointF(r.Left, r.Top);
            result[1] = new PointF(r.Right, r.Top);
            result[2] = new PointF(r.Right, r.Bottom);
            result[3] = new PointF(r.Left, r.Bottom);

            return result;
        }
        public static PointF[] Points(this RectangleF r)
        {
            PointF[] result = new PointF[4];

            result[0] = new PointF(r.Left, r.Top);
            result[1] = new PointF(r.Right, r.Top);
            result[2] = new PointF(r.Right, r.Bottom);
            result[3] = new PointF(r.Left, r.Bottom);

            return result;
        }
        public static Point[] Points(this Rectangle r)
        {
            Point[] result = new Point[4];

            result[0] = new Point(r.Left, r.Top);
            result[1] = new Point(r.Right, r.Top);
            result[2] = new Point(r.Right, r.Bottom);
            result[3] = new Point(r.Left, r.Bottom);

            return result;
        }
        public static PointF[] PointFs(this Rectangle r)
        {
            PointF[] result = new PointF[4];

            result[0] = new PointF(r.Left, r.Top);
            result[1] = new PointF(r.Right, r.Top);
            result[2] = new PointF(r.Right, r.Bottom);
            result[3] = new PointF(r.Left, r.Bottom);

            return result;
        }


        public static Point SysPoint(this Vex.Point p)
        {
            return new Point(
                (int)Math.Ceiling(p.X),
                (int)Math.Ceiling(p.Y));
        }
        public static PointF SysPointF(this Vex.Point p)
        {
            return new PointF(p.X, p.Y);
        }

        public static Size SysSize(this Vex.Size s)
        {
            return new Size(
                (int)Math.Ceiling(s.Width),
                (int)Math.Ceiling(s.Height));
        }

        public static Matrix SysMatrix(this Vex.Matrix m)
        {
            return new Matrix(m.ScaleX, m.Rotate0, m.Rotate1, m.ScaleY, m.TranslateX, m.TranslateY);
        }
        public static Color SysColor(this Vex.Color c)
        {
            return Color.FromArgb((int)c.ARGB);
        }
        public static Vex.Color VexColor(this Color c)
        {
            return new Vex.Color(c.R, c.G, c.B, c.A);
        }

        public static void TranslatePoints(this PointF[] pts, float offsetX, float offsetY)
        {
            for (int i = 0; i < pts.Length; i++)
            {
                pts[i].X += offsetX;
                pts[i].Y += offsetY;
            }
        }
        public static PointF[] GetMidpointsAndCenter(this PointF[] pts)
        {
            PointF[] result = new PointF[pts.Length * 2 + 1];

            // handles
            for (int i = 0; i < pts.Length; i++)
            {
                PointF p2 = (i == pts.Length - 1) ? pts[0] : pts[i + 1];
                result[i * 2] = pts[i];
                result[i * 2 + 1] = pts[i].MidPoint(p2);
            }

            // center
            PointF pBR = pts[pts.Length / 2];
            result[result.Length - 1] = pts[0].MidPoint(pBR);

            return result;
        }
        public static Rectangle GetBounds(this PointF[] pts)
        {
            float left = pts.Min(p => p.X);
            float right = pts.Max(p => p.X);
            float top = pts.Min(p => p.Y);
            float bottom = pts.Max(p => p.Y);
            RectangleF extent = new RectangleF(left, top, right - left + 1, bottom - top + 1);
            return Rectangle.Ceiling(extent);
        }

        public static bool IsPositiveRect(this Rectangle r)
        {
            return r.Width >= 0 && r.Height >= 0;
        }
        public static Rectangle GetPositiveRect(this Point p0, Point p1)
        {
            int rw = p1.X - p0.X;
            int rh = p1.Y - p0.Y;
            return new Rectangle(
                    rw >= 0 ? p0.X : p0.X + rw,
                    rh >= 0 ? p0.Y : p0.Y + rh,
                    Math.Abs(rw),
                    Math.Abs(rh));
        }
        public static Rectangle GetPositiveRect(this Rectangle r)
        {
            int rw = r.Right - r.Left;
            int rh = r.Bottom - r.Top;
            return new Rectangle(
                    rw >= 0 ? r.Left : r.Left + rw,
                    rh >= 0 ? r.Top : r.Top + rh,
                    Math.Abs(rw),
                    Math.Abs(rh));
        }

        public static bool IsIdentity(this System.Drawing.Drawing2D.Matrix m)
        {
            bool result = false;
            float[] els = m.Elements;
            if (els[0] == 1 && els[1] == 0 && els[2] == 0 && els[3] == 1 && els[4] == 0 && els[5] == 0)
            {
                result = true;
            }
            return result;
        }
        public static void ScaleOnlyAt(this Matrix result, float scaleX, float scaleY, PointF center)
        {
            Matrix mInv = result.Clone();
            mInv.Invert();
            //Matrix m2 = new Matrix(scaleX, 0, 0, scaleY, 0, 0);
            float offsetX = result.OffsetX;
            float offsetY = result.OffsetY;

            // move to orgin and scale
            result.Translate(-offsetX, -offsetY, MatrixOrder.Append);
            result.Scale(scaleX, scaleY);
            //result.Multiply(m2, MatrixOrder.Append);

            // find new orgin
            PointF dif = new PointF(center.X - offsetX, center.Y - offsetY);
            PointF[] pts1 = new PointF[] { dif };
            mInv.TransformVectors(pts1);
            result.TransformVectors(pts1);
            PointF finalOffset = new PointF(center.X - pts1[0].X, center.Y - pts1[0].Y);

            result.Translate(finalOffset.X, finalOffset.Y, MatrixOrder.Append);

            //m2.Dispose();
            mInv.Dispose();
        }

        public static void ScaleAt(this Matrix result, float scaleX, float scaleY, PointF center)
        {
            Matrix mReg = new Matrix(result.Elements[0], result.Elements[1], result.Elements[2], result.Elements[3], 0, 0);
            Matrix mInv = result.Clone();
            mInv.Invert();
            float offsetX = result.OffsetX;
            float offsetY = result.OffsetY;

            // move to orgin and scale
            result.Reset();
            result.Scale(scaleX, scaleY, MatrixOrder.Append);
            result.Multiply(mReg, MatrixOrder.Append);

            // find new orgin
            PointF dif = new PointF(center.X - offsetX, center.Y - offsetY);
            PointF[] pts1 = new PointF[] { dif };
            mInv.TransformVectors(pts1);
            result.TransformVectors(pts1);
            PointF finalOffset = new PointF(center.X - pts1[0].X, center.Y - pts1[0].Y);

            result.Translate(finalOffset.X, finalOffset.Y, MatrixOrder.Append);

            mReg.Dispose();
            mInv.Dispose();
        }


        public static Vex.Rectangle VexRectangle(this Rectangle r)
        {
            return new Vex.Rectangle(
                r.Left,
                r.Top,
                r.Width,
                r.Height);
        }
        public static Vex.Rectangle VexRectangle(this RectangleF r)
        {
            return new Vex.Rectangle(
                r.Left,
                r.Top,
                r.Width,
                r.Height);
        }
        public static PointF Center(this RectangleF r)
        {
            return new PointF(r.X + r.Width / 2, r.Y + r.Height / 2);
        }
        public static PointF Center(this Rectangle r)
        {
            return new PointF(r.X + r.Width / 2, r.Y + r.Height / 2);
        }
        public static PointF LocalCenter(this RectangleF r)
        {
            return new PointF(r.Width / 2, r.Height / 2);
        }
        public static PointF LocalCenter(this Rectangle r)
        {
            return new PointF(r.Width / 2, r.Height / 2);
        }
        public static PointF MidPoint(this PointF p, PointF p2)
        {
            return new PointF(
                p.X + ((p2.X - p.X) / 2f),
                p.Y + ((p2.Y - p.Y) / 2f));
        }

        public static Point MidPoint(this Point p, Point p2)
        {
            return new Point(
                p.X + (int)((p2.X - p.X) / 2f),
                p.Y + (int)((p2.Y - p.Y) / 2f));
        }
        public static Vex.Point VexPoint(this Point p)
        {
            return new Vex.Point(
                p.X,
                p.Y);
        }
        public static Vex.Point VexPoint(this PointF p)
        {
            return new Vex.Point(
                p.X,
                p.Y);
        }

        public static Vex.Size VexSize(this Size s)
        {
            return new Vex.Size(s.Width, s.Height);
        }

        public static Vex.Matrix VexMatrix(this Matrix m)
        {
            float[] e = m.Elements;
            return new Vex.Matrix(e[0], e[1], e[2], e[3], e[4], e[5]);
        }
        public static bool IsScaledOrSheared(this Matrix m)
        {
            float[] e = m.Elements;
            return !((e[0] == 1) && (e[1] == 0) && (e[2] == 0) && (e[3] == 1));
        }


        // binary search for ILists
        public static int FindFirstAbove(this IList<float> list, float value)
        {
            int result = -1;
            int lower = 0;
            int upper = list.Count - 1;

            while (lower <= upper)
            {
                int middle = (int)((lower + upper) / 2f);
                int comparisonResult = value.CompareTo(list[middle]);
                if (comparisonResult == 0)
                {
                    result = middle;
                    break;
                }
                else if (comparisonResult < 0)
                {
                    upper = middle - 1;
                }
                else
                {
                    lower = middle + 1;
                }
            }

            result = lower;

            return result;
        }
    }
}
