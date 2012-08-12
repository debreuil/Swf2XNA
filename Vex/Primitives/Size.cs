/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

namespace DDW.Vex
{
	public struct Size : IVexObject, IXmlSerializable
	{
        public static Size Empty = new Size(0, 0);
		public float Width;
		public float Height;

		public Size(float w, float h)
		{
			this.Width = w;
			this.Height = h;
		}

		public Point ToPoint()
		{
			return new Point(Width, Height);
		}

		public override bool Equals(Object o)
		{
			bool result = false;
			if (o is Size && (this.Width == ((Size)o).Width) && (this.Height == ((Size)o).Height))
			{
				result = true;
			}
			return result;
		}

		public bool Equals(Size o)
		{
			return ((this.Width == o.Width) && (this.Height == o.Height));
		}

		public static bool operator ==(Size a, Size b)
		{
			return (a.Width == b.Width) && (a.Height == b.Height);
		}

		public static bool operator !=(Size a, Size b)
		{
			return !((a.Width == b.Width) && (a.Height == b.Height));
		}

		public override int GetHashCode()
		{
			return (int)(this.Width * 17 + this.Height);
		}


		public int CompareTo(Object o)
		{
			int result = 0;
			if (o is Size)
			{
				Size co = (Size)o;
				if (this.Width != co.Width)
				{
					result = (this.Width > co.Width) ? 1 : -1;
				}
				else if (this.Height != co.Height)
				{
					result = (this.Height > co.Height) ? 1 : -1;
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
            return "[s:" + this.Width + "," + this.Height + "]";
        }
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader r)
        {
            Width = float.Parse(r.GetAttribute("Width"), NumberStyles.Any);
            Height = float.Parse(r.GetAttribute("Height"), NumberStyles.Any);
            r.Read();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString()); 
        }
	}
}
