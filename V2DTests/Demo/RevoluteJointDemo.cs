using System.Collections.Generic;
using DDW.Display;
using DDW.V2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;
using V2DRuntime.Attributes;
using V2DRuntime.V2D;

namespace V2DTest
{
    [V2DScreenAttribute(backgroundColor=0x222222, gravityX=0, gravityY=50,debugDraw=false)]
    public class RevoluteJointDemo : V2DScreen
    {
        public V2DSprite ghostThing;

        [RevoluteJointAttribute(enableMotor=true, maxMotorTorque=10000, motorSpeed=4)]
        public RevoluteJoint rFlower;

        public RevoluteJointDemo(V2DContent v2dContent) : base(v2dContent) { }
        public RevoluteJointDemo(SymbolImport si) : base(si) { }

        public override void Initialize()
        {
            base.Initialize();
            ghostThing.PlayAll();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}








