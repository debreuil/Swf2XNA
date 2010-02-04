using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D;
using Microsoft.Xna.Framework;
using SharpNeatLib.Maths;

namespace V2DRuntime.Particles
{
	public class Particle
	{
		public ushort TextureIndex;

		//public Vector2 position;
		//public float rotation;
		public float Deviation0;
		public float Deviation1;
		public float X;
		public float Y;

		public Particle(FastRandom rnd)
		{
			Deviation0 = (float)rnd.NextDouble() * 2f - 1f;
			//Deviation1 = (float)rnd.NextDouble() * 2f - 1f;
			//TextureIndex = (ushort)rnd.Next(4);
		}
	}
}
