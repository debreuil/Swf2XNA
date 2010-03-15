using System;
using DDW.V2D;
using Box2D.XNA;
using Microsoft.Xna.Framework;

namespace V2DRuntime.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class V2DSpriteAttribute : SpriteAttribute
	{
		public bool isStatic;
 
		public float friction;
		public float restitution;
		public float density;
		public short groupIndex;
		public ushort maskBits;
		public ushort categoryBits;
		public bool isSensor;

		public float centerOfMassX;
		public float centerOfMassY;
		public float mass;
		public float linearDamping;
		public float angularDamping;
		public bool allowSleep;
		public bool fixedRotation;
		public bool isBullet;

		public V2DSpriteAttribute()
		{
			isStatic = false;

			friction = 0.2f;
			restitution = 0.0f;
			density = 0.0f;
			categoryBits = 0x0001;
			maskBits = 0xFFFF;
			groupIndex = 0;
			isSensor = false;

			// body
			centerOfMassX = 0;
			centerOfMassY = 0;
			mass = 0.0f;
			linearDamping = 0.0f;
			angularDamping = 0.0f;
			allowSleep = true;
			fixedRotation = false;
			isBullet = false;
		}
		public void ApplyAttribtues(FixtureDef def)
		{
			Filter fd;
			if (friction != 0.2f)
			{
				def.friction = friction;
			}
			if (restitution != 0)
			{
				def.restitution = restitution;
			}
			if (density != 0)
			{
				def.density = density;
			}
			if (categoryBits != 0x0001)
			{
				def.filter.categoryBits = categoryBits;
			}
			if (maskBits != 0xFFFF)
			{
				def.filter.maskBits = maskBits;
			}
			if (groupIndex != 0)
			{
				def.filter.groupIndex = groupIndex;
			}
			if (isSensor != false)
			{
				def.isSensor = isSensor;
			}
		}
		public void ApplyAttribtues(BodyDef def)
		{
			//if (centerOfMassX != 0 || centerOfMassY != 0)
			//{
			//    def.MassData.Center = new Vector2(centerOfMassX, centerOfMassY);
			//}
			//if (mass != 0.0f)
			//{
			//    def.MassData.Mass = mass;
			//}
			if (linearDamping != 0.0f)
			{
				def.linearDamping = linearDamping;
			}
			if (angularDamping != 0.0f)
			{
				def.angularDamping = angularDamping;
			}
			if (allowSleep != true)
			{
				def.allowSleep = allowSleep;
			}
			if (fixedRotation != false)
			{
				def.fixedRotation = fixedRotation;
			}
			if (isBullet != false)
			{
				def.bullet = isBullet;
			}
		}
	} 
}
