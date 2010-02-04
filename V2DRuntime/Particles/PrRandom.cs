using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2DRuntime.Particles
{
	public class PrRandom
	{
		public uint seed;
		public uint state;
		
		public PrRandom()
		{
			seed = 1;
		}
		public PrRandom(uint seed)
		{
			this.seed = seed;
			this.state = seed;
		}
		public void Reinitialise(uint seed)
		{
			this.seed = seed;
			this.state = seed;
		}
		public uint GetState()
		{
			return state;
		}
		public void SetState(uint state)
		{
			this.state = state;
		}
		/// <summary>
		/// provides the next pseudorandom number as an unsigned integer (31 bits)
		/// </summary>
		public uint NextInt()
		{
			return Gen();
		}
		/// <summary>
		/// provides the next pseudorandom number as a float between nearly 0 and nearly 1.0.
		/// </summary>
		public double NextDouble()
		{
			state = (uint)(state * 16807f) % 0x7FFFFFFF;
			return state / 2147483647.0f;
		}
		/// <summary>
		/// provides the next pseudorandom number as an unsigned integer (31 bits) betweeen a given range.
		/// </summary>
		public uint NextIntRange(float min, float max)
		{
			min -= .4999f;
			max += .4999f;
			return (uint)Math.Round(min + ((max - min) * NextDouble()));
		}
		
		/// <summary>
		/// provides the next pseudorandom number as a float between a given range.
		/// </summary>
		public double NextDoubleRange(float min, float max)
		{
			return min + ((max - min) * NextDouble());
		}
		//uint hi;
		//uint lo;
		private uint Gen()
		{
			//integer version 1, for max int 2^46 - 1 or larger.
			return state = (uint)(state * 16807f) % 0x7FFFFFFF;
			
			//integer version 2, for max int 2^31 - 1 (slowest)
			//int test = (int)(16807 * (state % 127773 >> 0) - 2836 * (state / 127773 >> 0));
			//return state = (uint)(test > 0 ? test : test + 0x7FFFFFFF);
			
			// david g. carta's optimisation is 15% slower than integer version 1
			//hi = 16807 * (state >> 16);
			//lo = 16807 * (state & 0xFFFF) + ((hi & 0x7FFF) << 16) + (hi >> 15);
			//return state = (lo & 0x7FFFFFFF) + (lo >> 31);
			//return state = (lo > 0x7FFFFFFF ? lo - 0x7FFFFFFF : lo);
		}
	}
}
