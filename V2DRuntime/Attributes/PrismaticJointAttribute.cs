using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX.Dynamics;

namespace V2DRuntime.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class PrismaticJointAttribute : JointAttribute
	{
		/// <summary>
		/// Enable/disable the joint limit.
		/// </summary>
		public bool enableLimit;

		/// <summary>
		/// The lower translation limit, usually in meters.
		/// </summary>
		public float lowerTranslation;

		/// <summary>
		/// The upper translation limit, usually in meters.
		/// </summary>
		public float upperTranslation;

		/// <summary>
		/// Enable/disable the joint motor.
		/// </summary>
		public bool enableMotor;

		/// <summary>
		/// The maximum motor torque, usually in N-m.
		/// </summary>
		public float maxMotorForce;

		/// <summary>
		/// The desired motor speed in radians per second.
		/// </summary>
		public float motorSpeed;

		public PrismaticJointAttribute()
		{
			enableLimit = false;
			lowerTranslation = 0.0f;
			upperTranslation = 0.0f;
			enableMotor = false;
			maxMotorForce = 0.0f;
			motorSpeed = 0.0f;
		}
		public void ApplyAttribtues(PrismaticJoint j)
		{
			j.SetCollideConnected(collideConnected);

			if (enableLimit != false)
			{
				j._enableLimit = enableLimit;
			}

			if (lowerTranslation != 0.0f)
			{
				j._lowerTranslation = lowerTranslation;
			}

			if (upperTranslation != 0.0f)
			{
				j._upperTranslation = upperTranslation;
			}

			if (enableMotor != false)
			{
				j._enableMotor = enableMotor;
			}

			if (maxMotorForce != 0.0f)
			{
				j._maxMotorForce = maxMotorForce;
			}

			if (motorSpeed != 0.0f)
			{
				j._motorSpeed = motorSpeed;
			}
		}
	}
}