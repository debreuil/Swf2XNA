/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DDW.Vex
{
	public class Text : IDefinition
	{
        public uint Id { get; set; }
        public string Name { get; set; }
        public Rectangle StrokeBounds { get; set; }
        public string Path { get; set; }
        public Point Center { get { return Point.Empty; } }
        [XmlIgnore]
        public int UserData { get; set; }
        [XmlIgnore]
        public bool HasSaveableChanges { get; set; }
        public string WorkingPath { get; set; }

		public List<TextRun> TextRuns = new List<TextRun>();
		public Matrix Matrix;

		public Text(uint id)
		{
			this.Id = id;
		}

	}
}
