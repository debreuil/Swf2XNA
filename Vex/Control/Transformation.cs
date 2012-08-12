using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	/// <summary>
	/// A Transformation describes a single transformation state of a symbol, though it does not specify the symbol.
	/// The start time is always implicitly zero.
	/// A Lifetime contains the symbol and list of transformations.
	/// </summary>
	public class Transformation
	{
		// color transform
		// filters
		// easing
		// character script

		TimeSpan Duration;
		Matrix Transform;
	}
}
