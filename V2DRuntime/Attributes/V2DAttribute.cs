using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace V2DRuntime.V2D
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class V2DAttribute : System.Attribute
	{
		public bool isStatic;
		public short groupIndex;
		public short depthGroup;

		public V2DAttribute()
		{
		}
	} 
}
