using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D;
using System.Text.RegularExpressions;

namespace DDW.Display
{
	public class TextAtom
	{
		private string text;
		public string FontName;
		public SpriteFont Font;
		public float Top;
		public float Left;
		public Color Color;
		public Vector2 Origin;

		public TextAtom()
		{
		}
		public TextAtom(float orgX, float orgY, V2DTextRun tr)
		{
			this.Text = tr.Text;
			this.Font = FontManager.Instance.GetFont(tr.FontName);
			//this.Origin = new Microsoft.Xna.Framework.Vector2(orgX + tr.Left, orgY + tr.Top);
			this.Origin = new Microsoft.Xna.Framework.Vector2(tr.Left, tr.Top);

			uint c = tr.Color;
			byte a = (byte)(c >> 24);
			byte r = (byte)(c >> 16);
			byte g = (byte)(c >> 8);
			byte b = (byte)(c >> 0);
			this.Color = new Color(r, g, b, a);
		}
		public string Text
		{
			get{return text;}
			set
			{
				text = StripHtml(value);
			}
		}
		private string StripHtml(string text)
		{
			string result = Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
			return result;
		}

		public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			batch.DrawString(Font, Text, Origin, Color);
		}
	}
}
