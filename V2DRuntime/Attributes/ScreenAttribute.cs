using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Box2DX.Common;

namespace V2DRuntime.V2D
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ScreenAttribute : System.Attribute
	{
		public uint backgroundColor;

		public ScreenAttribute()
		{
		}
	} 
}