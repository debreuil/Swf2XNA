/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class Symbol : IDefinition
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public int UserData { get; set; }
        public Rectangle StrokeBounds { get; set; }
        public string Path { get; set; }

		public Rectangle Bounds;
		public List<Shape> Shapes = new List<Shape>();

		public Symbol(uint id)
		{
			this.Id = id;
		}
	}
}
