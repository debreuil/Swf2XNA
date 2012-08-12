/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

namespace DDW.Vex
{
    public class SolidStroke : StrokeStyle, IXmlSerializable
	{
		public float LineWidth = 1.0F;
		public Color Color;

        public SolidStroke()
        {
        }
		public SolidStroke(float lineWidth, Color color)
		{
			this.LineWidth = lineWidth;
			this.Color = color;
		}

		public override bool Equals(Object o)
		{
			bool result = false;
			if (o is SolidStroke && (this.LineWidth == ((SolidStroke)o).LineWidth) && (this.Color == ((SolidStroke)o).Color))
			{
				result = true;
			}
			return result;
		}

		public bool Equals(SolidStroke o)
		{
			return ((this.LineWidth == o.LineWidth) && (this.Color == o.Color));
		}

		public static bool operator ==(SolidStroke a, SolidStroke b)
		{
			return (a.LineWidth == b.LineWidth) && (a.Color == b.Color);
		}

		public static bool operator !=(SolidStroke a, SolidStroke b)
		{
			return !((a.LineWidth == b.LineWidth) && (a.Color == b.Color));
		}

		public override int GetHashCode()
		{
			return (int)((int)this.LineWidth * 17 + this.Color.GetHashCode());
		}


		public override int CompareTo(Object o)
		{
			int result = 0;
			if (o is SolidStroke)
			{
				SolidStroke co = (SolidStroke)o;
				if (this.Color != co.Color)
				{
					result = this.Color.CompareTo(co.Color);
				}
				else if (this.LineWidth != co.LineWidth)
				{
					result = (this.LineWidth > co.LineWidth) ? 1 : -1;
				}
				
			}
			else
			{
				throw new ArgumentException("Objects being compared are not of the same type");
			}
			return result;
		}

		public override string ToString()
		{
			return "c: "  + this.Color + " w: " + this.LineWidth;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader r)
        {
            string s = r.GetAttribute("Color");
            string lw = r.GetAttribute("LineWidth");

            Color.FromString(s);
            LineWidth = float.Parse(lw, NumberStyles.Any);
            r.Read();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Color", Color.ToString());
            writer.WriteAttributeString("LineWidth", LineWidth.ToString());
        }
	}
}
