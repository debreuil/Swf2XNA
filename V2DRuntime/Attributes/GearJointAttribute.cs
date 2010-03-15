using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;

namespace V2DRuntime.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class GearJointAttribute : JointAttribute
	{
		/// <summary>
		/// The gear ratio.
		/// </summary>
		public float ratio;

		public GearJointAttribute()
		{
			ratio = 1.0f;
		}

		public void ApplyAttribtues(GearJointDef j)
		{
			j.collideConnected = collideConnected;

			if (ratio != 1.0f)
			{
				j.ratio = ratio;
			}
		}
	}
}