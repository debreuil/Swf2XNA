using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;

namespace V2DRuntime.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public abstract class JointAttribute : System.Attribute
	{
		/// <summary>
		/// Set this flag to true if the attached bodies should collide.
		/// </summary>
		public bool collideConnected;

		public JointAttribute()
		{
		}
	}
}
