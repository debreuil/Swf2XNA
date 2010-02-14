using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX.Dynamics;

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

		public void ApplyAttribtues(GearJoint j)
		{
			j.SetCollideConnected(collideConnected);

			if (ratio != 1.0f)
			{
				j._ratio = ratio;
			}
		}
	}
}