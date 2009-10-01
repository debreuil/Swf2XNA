using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	/// <summary>
	/// A timeline represents the activity defined within a symbol. 
	/// The activity is defined using one Lifetime per symbol used.
	/// </summary>
	class Timeline
	{
		// todo: ecapsulate these in an type when it is finalized
		List<DateTime> startTimes;
		List<TimeSpan> durations;
		List<ILifetime> lifetimes;

	}
}
