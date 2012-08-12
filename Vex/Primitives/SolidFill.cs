/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DDW.Vex
{
    public class SolidFill : FillStyle, IXmlSerializable
	{
		public Color Color;

        public SolidFill()
        {
        }
        public SolidFill(Color color)
        {
            this.Color = color;
        }

		public override FillType FillType { get { return FillType.Solid;  } }


		public override bool Equals(Object o)
		{
			bool result = false;
			if (o is SolidFill && (this.FillType == ((SolidFill)o).FillType) && (this.Color == ((SolidFill)o).Color))
			{
				result = true;
			}
			return result;
		}

		public bool Equals(SolidFill o)
		{
			return ((this.FillType == o.FillType) && (this.Color == o.Color));
		}

		public static bool operator ==(SolidFill a, SolidFill b)
		{
			return (a.FillType == b.FillType) && (a.Color == b.Color);
		}

		public static bool operator !=(SolidFill a, SolidFill b)
		{
			return !((a.FillType == b.FillType) && (a.Color == b.Color));
		}

		public override int GetHashCode()
		{
			return (int)((int)this.FillType * 17 + this.Color.GetHashCode());
		}



		public override int CompareTo(Object o)
		{
			int result = 0;

			if (o is FillStyle && this.FillType != ((FillStyle)o).FillType)
			{
				result = (this.FillType > ((FillStyle)o).FillType) ? 1 : -1;
			}
			else if (o is SolidFill)
			{
				SolidFill sf = (SolidFill)o;
				result = this.Color.CompareTo(sf.Color);
			}
			else
			{
				throw new ArgumentException("Objects being compared are not of the same type");
			}
			return result;
		}
        public override string ToString()
        {
            return "c: " + this.Color;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader r)
        {
            string s = r.GetAttribute("Color");
            Color.FromString(s);
            r.Read();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Color", Color.ToString());
        }
	}
}
