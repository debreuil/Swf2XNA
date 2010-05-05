using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DDW.Display
{
	public class TextAtom
	{
		private string text;
		public string FontName;
		public SpriteFont Font;
		public Vector2 TopLeft;
		public Color Color;
		public Vector2 Origin;
		public float Kerning;
		public float LetterSpacing;
		public TextAlign Align;

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

			this.LetterSpacing = GetAttributeValue(tr.Text, "letterSpacing");
			this.Kerning = GetAttributeValue(tr.Text, "kerning");
			this.Align = GetAlign(tr.Text);
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
			if (result.Contains('&'))
			{
				result = Regex.Replace(result, @"&apos;", "'");
				result = Regex.Replace(result, @"&nbsp;", @"");
				result = Regex.Replace(result, @"&gt;", @"");
				result = Regex.Replace(result, @"&lt;", @"");
				result = Regex.Replace(result, @"&amp;", @"");

				//result = Regex.Replace(text, @"&cent;", @"¢");
				//result = Regex.Replace(text, @"&pound;", @"£");
				//result = Regex.Replace(text, @"&yen;", @"¥");
				//result = Regex.Replace(text, @"&euro;", @"€");
				//result = Regex.Replace(text, @"&sect;", @"§");
				//result = Regex.Replace(text, @"&copy;", @"©");
				//result = Regex.Replace(text, @"&reg;", @"®");
			}
			return result;
		}
		private string GetAttributeValueString(string s, string attributeName)
		{
			string result = "";
			int ix = s.IndexOf(attributeName + "=\"");
			if (ix > -1)
			{
				try
				{
					int st = ix + attributeName.Length + 2;
					result = s.Substring(st, s.IndexOf('\"', st) - st);
				}
				catch (Exception) { }
			}

			return result;
		}
		private float GetAttributeValue(string s, string attributeName)
		{
			float result = 0;
			string val;
			try
			{
				val = GetAttributeValueString(s, attributeName);
                if (val != "")
                {
                    result = float.Parse(val, NumberStyles.Any);
                }
			}
			catch (Exception)
			{
                Console.WriteLine("Error with attribute. Name: " + attributeName + "  String: " + s);
			}

			return result;
		}
		private TextAlign GetAlign(string s)
		{
			TextAlign result = TextAlign.Left;
			string val;
			try
			{
				val = GetAttributeValueString(s, "align");
				switch (val.ToLower(CultureInfo.InvariantCulture))
				{
					case "left":
						result = TextAlign.Left;
						break;
					case "right":
						result = TextAlign.Right;
						break;
					case "center":
						result = TextAlign.Center;
						break;
					default :
						result = TextAlign.Left;
						break;
				}
			}
			catch (Exception)
			{
			}

			return result;
		}

		public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			batch.DrawString(Font, Text, Origin, Color);
		}
	}

	public enum TextAlign { Left, Center, Right }
}
