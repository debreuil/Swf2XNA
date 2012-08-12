using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public class Interval : IVexObject
	{
		public Instance Instance;
		public DateTime StartTime;
		public DateTime EndTime;

		public Interval(Instance instance, DateTime startTime, DateTime endTime)
		{
			this.Instance = instance;
			this.StartTime = startTime;
			this.EndTime = endTime;
		}
	}
}
