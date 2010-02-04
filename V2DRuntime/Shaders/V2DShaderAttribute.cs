using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace V2DRuntime.Shaders
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false)]
	public class V2DShaderAttribute : System.Attribute
	{
		public Type shaderType;
		public float param0;
		public float param1;
		public float param2;
		public float param3;
		public float param4;

		public V2DShaderAttribute()
		{
		}
	} 
}