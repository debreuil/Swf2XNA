using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	/// <summary>
	/// This is the lifetime of a single raw drawwing instance on a timeline. It can pertain to one drawing at the most, 
	/// and a continous duration.
	/// Akin to a raw graphics on a frame in flash, that exist for a set duration. Unlike flash, these can still
	/// have transformations applied to them.
	/// </summary>
	public class DrawingLifetime : ILifetime
	{
		Drawing Drawing;

		private List<Transformation> transformations;
		public List<Transformation> Transformations { get { return transformations; } }
	}
}
