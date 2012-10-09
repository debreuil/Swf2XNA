/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DDW.Vex
{
	public class Symbol : IDefinition
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public Rectangle StrokeBounds { get; set; }
        public string Path { get; set; }
        public Point Center { get { return new Point(-StrokeBounds.Left, -StrokeBounds.Top); } }
        [XmlIgnore]
        public int UserData { get; set; }
        [XmlIgnore]
        public bool HasSaveableChanges { get; set; }
        public string WorkingPath { get; set; }

		public Rectangle Bounds;
		public List<Shape> Shapes = new List<Shape>();

        public Symbol()
        {
        }
		public Symbol(uint id)
		{
			this.Id = id;
		}

	}
}
