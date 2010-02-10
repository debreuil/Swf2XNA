using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Globalization;
using Microsoft.Xna.Framework.Content;

namespace DDW.V2D
{
    public class V2DShape
    {
        public float[] Data;
        [XmlAttribute]
        public bool IsCircle;
        [XmlAttribute]
        public float Radius;
        [XmlAttribute]
        public float CenterX;
        [XmlAttribute]
        public float CenterY;

        public void EnsureClockwise()
        {
            if (!IsCircle)
            {
                if (Data != null && Data.Length > 2)
                {
                    // test for ccw
                    float aX = Data[0];
                    float aY = Data[1];
                    float bX = Data[2];
                    float bY = Data[3];
                    float cX = Data[4];
                    float cY = Data[5];
                    float area = aX * bY - aY * bX +
                                 aY * cX - aX * cY +
                                 bX * cY - cX * bY;
                    bool isCCW = area < 0;

                    if (isCCW)
                    {
                        for (int i = 0; i < (int)(Data.Length / 4); i++)
                        {
                            int st = i * 2;
                            int en = Data.Length - i * 2 - 2;
                            float tx = Data[st];
                            float ty = Data[st + 1];
                            Data[st] = Data[en];
                            Data[st + 1] = Data[en + 1];
                            Data[en] = tx;
                            Data[en + 1] = ty;
                        }
                    }
                }
            }
        }
        public bool IsPointInside(Point p)
        {
            bool result = true;

            if (IsCircle)
            {
                float xd = p.X - CenterX;
                float yd = p.Y - CenterY;
                float d = (float)Math.Sqrt(xd * xd + yd * yd);
                result =  d <= Radius;
            }
            else
            {
                if (Data != null && Data.Length > 2)
                {
                    for (int i = 1; i < Data.Length; i += 2)
                    {
                        float aX = Data[i + 0];
                        float aY = Data[i + 1];
                        float bX = Data[i + 2];
                        float bY = Data[i + 3];

                        if (IsToLeft(p, aX, aY, bX, bY))
                        {
                            result = false;
                            break;
                        }
                    }

                    if (result)
                    {
                        result = !IsToLeft(p, Data[Data.Length - 2], Data[Data.Length - 1], Data[0], Data[1]);
                    }
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }
        private bool IsToLeft(Point p, float aX, float aY, float bX, float bY)
        {
            float r = (p.Y - aY) * (bX - aX) - (p.X - aX) * (bY - aY);
            return (r <= 0);
        }
        private void ApplyMatrix(Matrix m)
        {
            //m.
        }

        //[ContentSerializerIgnore]
        //[XmlIgnore]
        //private Vector2[] parsedData;

        //[ContentSerializerIgnore]
        //[XmlIgnore]
        //public Vector2[] ParsedData
        //{
        //    get
        //    {
        //        if (parsedData == null)
        //        {
        //            ParseData();
        //        }
        //        return parsedData;
        //    }
        //}
        //private void ParseData()
        //{
        //    string s = Data.ToUpperInvariant();
        //    if (s.IndexOf('M') > -1)
        //    {
        //        s = s.Substring(s.IndexOf('M') + 1);
        //    }
        //    string[] sa = s.Split('L');
        //    parsedData = new Vector2[sa.Length];

        //    for (int i = 0; i < sa.Length; i++)
        //    {
        //        string[] pta = sa[i].Split(',');
        //        parsedData[i].X = float.Parse(pta[0], NumberStyles.Any);
        //        parsedData[i].Y = float.Parse(pta[1], NumberStyles.Any);
        //    }
        //}
    }
}
