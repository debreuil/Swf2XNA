/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class Text : IDefinition
	{
        public uint Id { get; set; }
        public string Name { get; set; }
        public int UserData { get; set; }
        public Rectangle StrokeBounds { get; set; }
        public string Path { get; set; }

		public List<TextRun> TextRuns = new List<TextRun>();
		public Matrix Matrix;

		public Text(uint id)
		{
			this.Id = id;
		}
	}
}
