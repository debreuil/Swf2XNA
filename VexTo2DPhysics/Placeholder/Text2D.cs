using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex;
using DDW.V2D;

namespace DDW.Placeholder
{
	public class Text2D : Definition2D
	{
		public List<V2DTextRun> TextRuns;

		public Text2D()
		{
		}
		public Text2D(List<TextRun> trs)
		{
			SetTextRuns(trs);
		}
		public void SetTextRuns(List<TextRun> trs)
		{
			TextRuns = new List<V2DTextRun>();
			for (int i = 0; i < trs.Count; i++)
			{
				TextRuns.Add(GetRun(trs[i]));
			}
		}
		
		public V2DTextRun GetRun(TextRun tr)
		{
			V2DTextRun result = new V2DTextRun();

			result.Text = tr.Text;
			result.FontName = tr.FontName;
			result.FontSize = tr.FontSize;
			result.Top = tr.Top;
			result.Left = tr.Left;
			result.Color = (uint)tr.Color.ARGB;
			result.isHtml = tr.isHtml;
			result.isBold = tr.isBold;
			result.isItalic = tr.isItalic;
			result.isMultiline = tr.isMultiline;
			result.isUnderlined = tr.isUnderlined;
			result.isStrikeout = tr.isStrikeout;
			result.isWrapped = tr.isWrapped;
			result.isPassword = tr.isPassword;
			result.isEditable = tr.isEditable;
			result.isSelectable = tr.isSelectable;

			return result;
		}
	}
}
