using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D;
using DDW.Display;
using V2DRuntime.Attributes;
using Box2D.XNA;
using V2DRuntime.Components;
using Microsoft.Xna.Framework;
using V2DRuntime.V2D;

namespace V2DTests
{
	[V2DScreenAttribute(backgroundColor = 0x000001, gravityX = 10, debugDraw = false)]
    public class Demo : V2DScreen
	{
		public Sprite bkg;
        public V2DSprite ball;

		[V2DSpriteAttribute(isBullet=true)]
        public List<V2DSprite> flo;
        public List<Sprite> tank;
        public TextBox txTest;

		[GearJointAttribute(ratio = 2)]
        public GearJoint g1;

		[RevoluteJointAttribute(motorSpeed = 50, maxMotorTorque = 500)]
        public RevoluteJoint r2;
        public V2DSprite ghost;

        [V2DSpriteAttribute(maskBits = 1, categoryBits = 1, density=4000, mass=4000,restitution=9999)]
        public List<V2DSprite> hex;

        public List<V2DSprite> arch;
		[V2DSpriteAttribute(depthGroup = 40)]
		public TurretMachine turretMachine;

		public Demo(SymbolImport si) : base(si)
        {
        }
		public override void Initialize()
		{
			base.Initialize();
            flo[1].Alpha = .5f;
			//bkg.Visible = false;
			//boy.Play();
		}

        void b_PlayheadWrap(DisplayObjectContainer sender)
        {
            sender.Stop();
        }
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}
	}
}
