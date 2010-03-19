using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace V2DRuntime.V2D
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class V2DScreenAttribute : ScreenAttribute
	{
		public float gravityX = 0f;
		public float gravityY = 10f;
		public bool debugDraw = false;

		public V2DScreenAttribute()
		{
		}
	} 
}