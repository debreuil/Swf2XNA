using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class Vex
	{
		public int Width = 800;
		public int Height = 600;
		public Color BackgroundColor = new Color(0,0,0);

		List<Symbol> Symbols = new List<Symbol>();
		List<Interval> Intervals = new List<Interval>();
	}
}
