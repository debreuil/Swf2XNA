using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;

namespace V2DRuntime.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class PulleyJointAttribute : JointAttribute
	{
		/// <summary>
		/// The maximum length of the segment attached to body1.
		/// </summary>
		public float maxLength1;

		/// <summary>
		/// The maximum length of the segment attached to body2.
		/// </summary>
		public float maxLength2;

		/// <summary>
		/// The pulley ratio, used to simulate a block-and-tackle.
		/// </summary>
		public float ratio;

		public PulleyJointAttribute()
		{
			maxLength1 = 0.0f;
			maxLength2 = 0.0f;
			ratio = 1.0f;
			collideConnected = true;
		}

		public void ApplyAttribtues(PulleyJointDef j)
		{
			j.collideConnected = collideConnected;

			if (maxLength1 != 0.0f)
			{
				j.maxLengthA = maxLength1;
			}
			if (maxLength2 != 0.0f)
			{
				j.maxLengthB = maxLength2;
			}

			if (ratio != 1.0f)
			{
				j.ratio = ratio;
			}
		}
	}
}