/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DDW.Vex
{
	public abstract class FillStyle : IVexObject, IComparable
    {
        public static SolidFill NoFill = new SolidFill(Color.Transparent);

		public abstract FillType FillType{get;}
        public abstract int CompareTo(Object o);

        [XmlIgnore]
        public int UserData { get; set; }
	}
}
