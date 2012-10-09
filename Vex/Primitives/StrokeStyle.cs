/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DDW.Vex
{
	public abstract class StrokeStyle : IVexObject, IComparable
    {
        public static SolidStroke NoStroke = new SolidStroke(0, Color.Transparent);

        public float LineWidth = 1.0F;
        public Color Color;

        public abstract int CompareTo(Object o);

        [XmlIgnore]
        public int UserData { get; set; }
	}
}
