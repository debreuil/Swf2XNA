/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class Shape
	{
		public FillStyle Fill;
		public StrokeStyle Stroke;
		public List<IShapeData> ShapeData = new List<IShapeData>();

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
	}
}
