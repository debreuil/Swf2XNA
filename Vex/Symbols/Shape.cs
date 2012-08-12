/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

namespace DDW.Vex
{
    public class Shape
	{
		public FillStyle Fill;
		public StrokeStyle Stroke;
        [XmlIgnore]
		public List<IShapeData> ShapeData = new List<IShapeData>();

        public string EncodedShapeData
        {
            get
            {
                return GetSerializedShapeString();
            }
            set
            {
                ParseSerializedShapeString(value);
            }
        }

        public static DDW.Vex.Color outlineColor = new DDW.Vex.Color(0xFF, 0x00, 0x00, 0xFF);
        public static float outlineWidth = 1F;

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

        private char[] spc = new char[] { ' ' };
        public void ParseSerializedShapeString(string s)
        {
            string[] ar = s.Split(spc, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < ar.Length; i++)
            {
                switch (ar[i][0])
                {
                    case 'L':
                        ShapeData.Add(Line.Deserialize(ar[i]));
                        break;

                    case 'C':
                        ShapeData.Add(QuadBezier.Deserialize(ar[i]));
                        break;

                    case 'Q':
                        ShapeData.Add(QuadBezier.Deserialize(ar[i]));
                        break;
                }
            }
        }


        public string GetSerializedShapeString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < ShapeData.Count; i++)
            {                
                sb.Append(ShapeData[i].Serialize());
            }
            
            return sb.ToString();
        }
    }
}
