/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{

	public class TextRun
	{
		public string Text;
		public string FontName;
		public float FontSize;
		public float Top;
		public float Left;
		public Color Color;

		public bool isHtml;
		public bool isBold;
		public bool isItalic;
		public bool isMultiline;
		public bool isUnderlined;
		public bool isStrikeout;
		public bool isWrapped;
		public bool isPassword;
		public bool isEditable;
		public bool isSelectable;

		public bool isContinuous; 

		public TextRun()
		{
		}
	}
}
