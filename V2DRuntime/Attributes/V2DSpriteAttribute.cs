using System;
using DDW.V2D;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;

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
		public void ApplyAttribtues(ShapeDef def)
		{
			if (friction != 0.2f)
			{
				def.Friction = friction;
			}
			if (restitution != 0)
			{
				def.Restitution = restitution;
			}
			if (density != 0)
			{
				def.Density = density;
			}
			if (categoryBits != 0x0001)
			{
				def.Filter.CategoryBits = categoryBits;
			}
			if (maskBits != 0xFFFF)
			{
				def.Filter.MaskBits = maskBits;
			}
			if (groupIndex != 0)
			{
				def.Filter.GroupIndex = groupIndex;
			}
			if (isSensor != false)
			{
				def.IsSensor = isSensor;
			}
		}
		public void ApplyAttribtues(BodyDef def)
		{
			if (centerOfMassX != 0 || centerOfMassY != 0)
			{
				def.MassData.Center = new Vec2(centerOfMassX, centerOfMassY);
			}
			if (mass != 0.0f)
			{
				def.MassData.Mass = mass;
			}
			if (linearDamping != 0.0f)
			{
				def.LinearDamping = linearDamping;
			}
			if (angularDamping != 0.0f)
			{
				def.AngularDamping = angularDamping;
			}
			if (allowSleep != true)
			{
				def.AllowSleep = allowSleep;
			}
			if (fixedRotation != false)
			{
				def.FixedRotation = fixedRotation;
			}
			if (isBullet != false)
			{
				def.IsBullet = isBullet;
			}
		}
	} 
}
