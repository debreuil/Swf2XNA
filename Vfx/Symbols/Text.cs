/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class Text : IDefinition
	{
		private uint id;
		public uint Id { get { return id; } set { id = value; } }

		private string name;
		public string Name { get { return name; } set { name = value; } }

        private int userData;
        public int UserData { get { return userData; } set { userData = value; } }

		private Rectangle strokeBounds;
		public Rectangle StrokeBounds { get { return strokeBounds; } 
            set { strokeBounds = value; } }

		public List<TextRun> TextRuns = new List<TextRun>();
		public Matrix Matrix;

		public Text(uint id)
		{
			this.id = id;
		}
	}
}
