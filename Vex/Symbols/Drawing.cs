/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class Drawing
	{
		public uint Id;
		public List<Shape> Shapes = new List<Shape>();
		public Rectangle Bounds;
		public Rectangle StrokeBounds;
	}
}
