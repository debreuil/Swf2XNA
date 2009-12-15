using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace DDW.Display
{
	public class FontManager
	{
		private static FontManager instance;
		private Dictionary<string, SpriteFont> fontList = new Dictionary<string, SpriteFont>();
		public string defaultFontName;
		private FontManager(){}

		public static FontManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new FontManager();
				}
				return instance;
			}
		}

		public void AddFont(string name, SpriteFont font)
		{
			if (!fontList.ContainsKey(name))
			{
				fontList.Add(name, font);

				// first added font is default unless set directly.
				if (defaultFontName == null) 
				{
					defaultFontName = name;
				}
			}
		}
		public void RemoveFont(string name)
		{
			if (fontList.ContainsKey(name))
			{
				fontList.Remove(name);
			}
		}
		public SpriteFont GetFont(string name)
		{
			return fontList[name];
		}
	}
}
