using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX.Dynamics;

namespace V2DRuntime.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class DistanceJointAttribute : JointAttribute
	{
		/// <summary>
		/// The equilibrium length between the anchor points.
		/// </summary>
		public float length;

		/// <summary>
		/// The response speed.
		/// </summary>
		public float frequencyHz;

		/// <summary>
		/// The damping ratio. 0 = no damping, 1 = critical damping.
		/// </summary>
		public float dampingRatio;

		public DistanceJointAttribute()
		{
			length = 1.0f;
			frequencyHz = 0.0f;
			dampingRatio = 0.0f;
		}

		public void ApplyAttribtues(DistanceJoint j)
		{
			j.SetCollideConnected(collideConnected);

			if (length != 1.0f)
			{
				j._length = length;
			}

			if(frequencyHz != 0.0f)
			{
				j._frequencyHz = frequencyHz;
			}

			if (dampingRatio != 0.0f)
			{
				j._dampingRatio = dampingRatio;
			}
		}
	}
}