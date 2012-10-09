/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DDW.Vex
{
    public struct Color : IVexObject, IComparable, IXmlSerializable
    {
        public static Color Transparent = new Color(0, 0, 0, 0);

		public byte R;
		public byte G;
		public byte B;
		public byte A;

		public Color(byte r, byte g, byte b, byte a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		public Color(byte r, byte g, byte b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = 0xFF;
		}

        public uint ARGB
        {
            get
            {
                return (uint)((this.A << 24) + (this.R << 16) + (this.G << 8) + (this.B));
            }
        }
        public uint AFlipRGB
        {
            get
            {
                return (uint)(((0xFF - this.A) << 24) + (this.R << 16) + (this.G << 8) + (this.B));
            }
        }
        public void SetARGB(uint val)
        {
            A = (byte)((val & 0xFF000000) >> 24);
            R = (byte)((val & 0x00FF0000) >> 16);
            G = (byte)((val & 0x0000FF00) >> 8);
            B = (byte)((val & 0x000000FF) >> 0);
        }
		public uint RGBA
		{
			get
			{
				return (uint)((this.R << 24) + (this.G << 16) + (this.B << 8) + (this.A));
			}
		}
		public uint RGB
		{
			get
			{
                return (uint)((this.R << 16) + (this.G << 8) + (this.B));
			}
		}



		public override bool Equals(Object o)
		{
			bool result = false;
			if (o is Color && this.Value == ((Color)o).Value)
			{
				result = true;
			}
			return result;
		}

		public bool Equals(Color o)
		{
			return (this.Value == o.Value);
		}

		public static bool operator ==(Color a, Color b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(Color a, Color b)
		{
			return !(a.Value == b.Value);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public int CompareTo(Object o)
		{
			int result = 0;
			if (o is Color)
			{
				Color co = (Color)o;
				if (this.Value != co.Value)
				{
					result = (this.Value > co.Value) ? 1 : -1;
				}
			}
			else
			{
				throw new ArgumentException("Objects being compared are not of the same type");
			}
			return result;
		}

		public uint Value { get { return (uint)((this.R << 24) + (this.G << 16) + (this.B << 8) + this.A); } }


        public override string ToString()
        {
            return "#" + ARGB.ToString("X8");
        }
        public void FromString(string s)
        {
            string sx = s.Substring(1);
            SetARGB(uint.Parse(sx, System.Globalization.NumberStyles.HexNumber));
        }
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader r)
        {
            string s = r.GetAttribute("Value");
            string sx = s.Substring(1);
            SetARGB(uint.Parse(sx, System.Globalization.NumberStyles.HexNumber));
            r.Read();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Value", "#" + ARGB.ToString("X8"));
        }
	}
}
