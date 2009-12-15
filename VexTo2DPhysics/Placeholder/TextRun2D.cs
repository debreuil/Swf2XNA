using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex;

namespace DDW.Placeholder
{
	public class TextRun2D
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

		public TextRun2D()
		{
		}
		public TextRun2D(TextRun tr)
		{
			this.Text = tr.Text;
			this.FontName = tr.FontName;
			this.FontSize = tr.FontSize;
			this.Top = tr.Top;
			this.Left = tr.Left;
			this.Color = tr.Color;
			this.isHtml = tr.isHtml;
			this.isBold = tr.isBold;
			this.isItalic = tr.isItalic;
			this.isMultiline = tr.isMultiline;
			this.isUnderlined = tr.isUnderlined;
			this.isStrikeout = tr.isStrikeout;
			this.isWrapped = tr.isWrapped;
			this.isPassword = tr.isPassword;
			this.isEditable = tr.isEditable;
			this.isSelectable = tr.isSelectable;
		}
	}
}
