/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

namespace DDW.Vex
{
    public class Shape : IComparable
	{
		public FillStyle Fill;
		public StrokeStyle Stroke;
        [XmlIgnore]
		public List<IShapeData> ShapeData = new List<IShapeData>();

        private Rectangle bounds = Rectangle.Empty;

        public string EncodedShapeData
        {
            get
            {
                return GetSerializedShapeString(ShapeData);
            }
            set
            {
                 ParseSerializedShapeString(value, ShapeData);
            }
        }

        public static DDW.Vex.Color outlineColor = new DDW.Vex.Color(0xFF, 0x00, 0x00, 0xFF);
        public static float outlineWidth = 1F;

        public void CalcuateBounds()
        {
            float left = float.MaxValue;
            float right = float.MinValue;
            float top = float.MaxValue;
            float bottom = float.MinValue;
            for (int i = 0; i < ShapeData.Count; i++)
            {
                ShapeData[i].CheckExtremas(ref left, ref right, ref top, ref bottom);
            }

            bounds = new Rectangle(left, top, right - left, bottom - top);
        }

        public int CompareTo(object obj)
        {
            int result = 0;
            if (obj is Shape)
            {
                Shape co = (Shape)obj;
                float size = bounds.Width * bounds.Height;
                float coSize = co.bounds.Width * co.bounds.Height;
                if (size != coSize)
                {
                    result = size < coSize ? 1 : -1;
                }
            }
            else
            {
                throw new ArgumentException("Objects being compared are not of the same type");
            }
            return result;
        }

        public bool IsV2DShape()
        {
            bool result = false;
            if (Fill == null && Stroke is SolidStroke && ShapeData.Count > 2)
            {
                SolidStroke sf = (SolidStroke)Stroke;
                if ((sf.Color == outlineColor) && (sf.LineWidth <= outlineWidth))
                {
                    result = true;
                }
            }
            return result;
        }

        private static char[] spc = new char[] { ' ' };
        public static void ParseSerializedShapeString(string s, List<IShapeData> data)
        {
            string[] ar = s.Split(spc, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < ar.Length; i++)
            {
                switch (ar[i][0])
                {
                    case 'L':
                        data.Add(Line.Deserialize(ar[i]));
                        break;

                    case 'C':
                        data.Add(QuadBezier.Deserialize(ar[i]));
                        break;

                    case 'Q':
                        data.Add(QuadBezier.Deserialize(ar[i]));
                        break;
                }
            }
        }

        public static string GetSerializedShapeString(List<IShapeData> shapeData)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < shapeData.Count; i++)
            {
                sb.Append(shapeData[i].Serialize());
            }
            
            return sb.ToString();
        }
        public static string GetSvgString(List<IShapeData> sh)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("M" + sh[0].StartPoint.GetSVG()+" ");

            Point lastPoint = sh[0].StartPoint;
            String spc = "";
            for (int i = 0; i < sh.Count; i++)
            {
                IShapeData sd = sh[i];
                if (lastPoint != sd.StartPoint)
                {
                    sb.Append(spc + "M" + sd.StartPoint.GetSVG());
                }
                switch (sd.SegmentType)
                {
                    case SegmentType.Line:
                        sb.Append(spc + "L" + sd.EndPoint.GetSVG());
                        lastPoint = sd.EndPoint;
                        break;

                    case SegmentType.CubicBezier:
                        CubicBezier cb = (CubicBezier)sd;
                        sb.Append(spc + "C" + cb.Control0.GetSVG() + "," + cb.Control1.GetSVG() + "," + cb.Anchor1.GetSVG());
                        lastPoint = cb.EndPoint;
                        break;

                    case SegmentType.QuadraticBezier:
                        QuadBezier qb = (QuadBezier)sd;
                        sb.Append(spc + "Q" + qb.Control.GetSVG() + "," + qb.Anchor1.GetSVG());
                        lastPoint = qb.EndPoint;
                        break;
                }

                spc = " ";

            }
            
            return sb.ToString();
        }

    }
}
