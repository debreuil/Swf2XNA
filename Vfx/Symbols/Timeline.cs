/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	/// <summary>
	/// A timeline represents the activity defined within a symbol. 
	/// The activity is defined using one Lifetime per symbol used.
	/// </summary>
	public class Timeline : IDefinition
	{
		private uint id;
		public uint Id { get { return id; } set {id = value; } }

		private Rectangle strokeBounds;
		public Rectangle StrokeBounds { get { return strokeBounds; } set { strokeBounds = value; } }

		public uint Duration;
		public uint FrameCount = 0;

		public List<IInstance> Instances = new List<IInstance>();
		public Dictionary<uint, string> Labels = new Dictionary<uint, string>();

		private string name;
		public string Name { get { return name; } set { name = value; } }

        private int userData;
        public int UserData { get { return userData; } set { userData = value; } }

		public Timeline(uint id)
		{
			this.id = id;
			this.name = "mc" + id.ToString();
		}

		//public IDefinition LookupSymbol(float t, uint depth)
		//{
		//}

	}
}
