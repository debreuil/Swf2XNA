using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DDW.V2D
{
	public class V2DTextRun
	{
		[XmlAttribute]
		public string Text;
		[XmlAttribute]
		public string FontName;
		[XmlAttribute]
		public float FontSize;
		[XmlAttribute]
		public float Top;
		[XmlAttribute]
		public float Left;
		[XmlAttribute]
		public uint Color;

		[XmlAttribute]
		public bool isHtml;
		[XmlAttribute]
		public bool isBold;
		[XmlAttribute]
		public bool isItalic;
		[XmlAttribute]
		public bool isMultiline;
		[XmlAttribute]
		public bool isUnderlined;
		[XmlAttribute]
		public bool isStrikeout;
		[XmlAttribute]
		public bool isWrapped;
		[XmlAttribute]
		public bool isPassword;
		[XmlAttribute]
		public bool isEditable;
		[XmlAttribute]
		public bool isSelectable;

		[XmlAttribute]
		public bool isContinuous;

		public V2DTextRun()
		{
		}
	}
}
