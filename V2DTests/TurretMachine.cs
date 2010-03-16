using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using V2DRuntime.Attributes;
using DDW.V2D;
using Box2D.XNA;
using Microsoft.Xna.Framework.Graphics;

namespace V2DTests
{
	public class TurretMachine : V2DSprite
	{
		[V2DSpriteAttribute(groupIndex = -2, depthGroup = 40, isSensor = false)]
		public V2DSprite turret;
		[V2DSpriteAttribute(groupIndex = -2, depthGroup = 40, isSensor = true)]
		public V2DSprite launchGear;
		[V2DSpriteAttribute(groupIndex = -2, depthGroup = 40, isSensor = true)]
		public V2DSprite gearA;
		[V2DSpriteAttribute(groupIndex = -2, depthGroup = 40, isSensor = true)]
		public V2DSprite steamBar;
		[V2DSpriteAttribute(groupIndex = -2, depthGroup = 35, isSensor = true)]
		public V2DSprite steamGauge;
		[V2DSpriteAttribute(groupIndex = -2, depthGroup = 38, isSensor = true)]
		public V2DSprite steamLight;
		[V2DSpriteAttribute(groupIndex = -2, depthGroup = 39, isSensor = true)]
		public V2DSprite gaugeDialA;

		[RevoluteJointAttribute(enableMotor=true, motorSpeed=500)]
		private RevoluteJoint rev;
		[GearJointAttribute(ratio = .2f, collideConnected = false)]
		private GearJoint rollerGear;
		[GearJointAttribute(ratio = .45f, collideConnected = false)]
		private GearJoint wedgeGear;
		[GearJointAttribute(ratio = -1.4f, collideConnected = false)]
		private GearJoint turretGear;

		public TurretMachine(Texture2D texture, V2DInstance instance)
			: base(texture, instance)
		{
		}
		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{			
			base.Update(gameTime);
		}
		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);
		}
	}
}
