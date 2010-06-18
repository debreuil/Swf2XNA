using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D;
using Microsoft.Xna.Framework;

namespace V2DRuntime.Components
{
	public class TextBox : Sprite
	{
		private StringBuilder sb;
		private List<TextAtom> textAtoms = new List<TextAtom>();

		public TextBox() : base(null, null)
		{
		}
		public TextBox(string text) : base(null, null)
		{
			this.Text = text;
		}
		public TextBox(Texture2D texture, V2DInstance inst) : base(texture, inst)
		{
			if (inst.Definition is V2DText)
			{
				sb = new StringBuilder();
				V2DText t = (V2DText)inst.Definition;
				textAtoms = new List<TextAtom>();
				for (int i = 0; i < t.TextRuns.Count; i++)
				{
					TextAtom ta = new TextAtom(this.X, this.Y, t.TextRuns[i]);
					textAtoms.Add(ta);
					sb.Append(ta.Text);
				}
				RecreateTextRuns();
			}
		}

		public string  Text 
		{
			get{ return sb.ToString(); }
			set
			{
				sb = new StringBuilder(value);
				if (value != null)
				{
					RecreateTextRuns();
				}
			}
		}
		public void Append(string value)
		{
			if (sb == null)
			{
				sb = new StringBuilder();
			}
			sb.Append(value);
			RecreateTextRuns(); 
		}

		//private void EnsureTextRuns()
		//{
		//    for (int i = 0; i < textRuns.Count; i++)
		//    {
		//        TextAtom tr = textRuns[i];
		//        tr.XnaText = tr.Text;
		//        tr.XnaFont = FontManager.Instance.GetFont(tr.FontName);
		//        tr.XnaOrigin = new Microsoft.Xna.Framework.Vector2(X + tr.Left, Y + tr.Top);

		//        uint c = tr.Color;
		//        byte a = (byte)(c >> 24);
		//        byte r = (byte)(c >> 16);
		//        byte g = (byte)(c >> 8);
		//        byte b = (byte)(c >> 0);
		//        tr.XnaColor = new Color(r, g, b, a);
		//    }
		//}
		private void RecreateTextRuns()
		{
			// todo: html, spacing, multiline etc etc
			if (textAtoms == null)
			{
				textAtoms = new List<TextAtom>();
			}

			TextAtom ta = null;
			if(textAtoms.Count > 0)
			{
				ta = textAtoms[0];
			}
			textAtoms.Clear();

			if (ta == null)
			{
				ta = new TextAtom();
				ta.FontName = FontManager.Instance.defaultFontName;
				ta.Font = FontManager.Instance.GetFont(ta.FontName);
				//ta.Origin = new Microsoft.Xna.Framework.Vector2(X + ta.Left, Y + ta.Top);
				ta.Origin = ta.TopLeft;
				ta.Color = color;
			}
			ta.Text = sb.ToString();

			if (ta.Align == TextAlign.Center)
			{
				Vector2 tw = ta.Font.MeasureString(ta.Text);
				ta.TopLeft.X = (this.Width - ta.Font.MeasureString(ta.Text).X) / 2;
			}
			else if (ta.Align == TextAlign.Right)
			{
				ta.TopLeft.X = (this.Width - ta.Font.MeasureString(ta.Text).X);
			}

			textAtoms.Add(ta);
		}
		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.Update(gameTime);
		}
		public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			//base.Draw(batch);

			for (int i = 0; i < textAtoms.Count; i++)
			{
				batch.DrawString(
                    textAtoms[i].Font,
                    textAtoms[i].Text,
                    textAtoms[i].Origin + textAtoms[i].TopLeft + CurrentState.Position, 
                    textAtoms[i].Color,
					CurrentState.Rotation, 
                    CurrentState.Origin,
                    CurrentState.Scale, 
                    se, 
                    .5f / DepthCounter++
                    );
			}
		}
	}
}








