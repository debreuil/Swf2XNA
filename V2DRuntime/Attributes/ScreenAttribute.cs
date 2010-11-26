using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace V2DRuntime.V2D
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ScreenAttribute : System.Attribute
    {
        public uint backgroundColor;
        public bool isPersistantScreen;
        public short depthGroup;

		public ScreenAttribute()
		{
		}
	} 
}