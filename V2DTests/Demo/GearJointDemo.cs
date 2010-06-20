using System.Collections.Generic;
using DDW.Display;
using DDW.V2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;
using V2DRuntime.Attributes;

namespace V2DTest
{
    public class GearJointDemo : V2DScreen
    {
        public Sprite bkg;

        [PrismaticJointAttribute(enableMotor = true, maxMotorForce = 500, motorSpeed = 5)]
        public PrismaticJoint pj;

        public GearJointDemo(V2DContent v2dContent) : base(v2dContent) { }
        public GearJointDemo(SymbolImport si) : base(si) { }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (pj._limitState == LimitState.AtUpper)
            {
                pj.SetMotorSpeed(-5);
            }
            else if (pj._limitState == LimitState.AtLower)
            {
                pj.SetMotorSpeed(5);
            }
        }
    }
}








