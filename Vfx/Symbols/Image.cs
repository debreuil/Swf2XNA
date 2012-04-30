/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class Image : IDefinition
	{
		private uint id;
		public uint Id { get { return id; } set { id = value; } }

		private string name;
        public string Name { get { return name; } set { name = value; } }

        private int userData;
        public int UserData { get { return userData; } set { userData = value; } }

		private Rectangle strokeBounds;
		public Rectangle StrokeBounds { get { return strokeBounds; } set { strokeBounds = value; } }

		public string Path;
		public Image(string path, uint id)
		{
			this.id = id;
			this.Path = path;
		}
	}
}
