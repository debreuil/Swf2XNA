using System;

namespace V2DRuntime.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class SpriteAttribute : System.Attribute
	{
		public short depthGroup;
	
		public SpriteAttribute()
		{
		}
	} 
}
