
using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	/// <summary>
	/// This is the lifetime of a single instance on a timeline. It can pertain to one symbol at the most, 
	/// and and a continous duration.
	/// Akin to a symbol instance in flash, although it also contains the activity (eg, a full tween, or group of moves etc)
	/// </summary>
	public class SymbolLifetime : ILifetime
	{
		Symbol Symbol;

		private List<Transformation> transformations;
		public List<Transformation> Transformations { get { return transformations; } }
	}
}
