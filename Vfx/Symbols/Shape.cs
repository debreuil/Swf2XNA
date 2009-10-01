/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class Shape
	{
		public FillStyle Fill;
		public StrokeStyle Stroke;
		public List<IShapeData> ShapeData = new List<IShapeData>();
	}
}
