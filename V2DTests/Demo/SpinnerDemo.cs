
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
    [V2DScreenAttribute(backgroundColor=0x222222, gravityX=0, gravityY=0,debugDraw=false)]
    public class SpinnerDemo : V2DScreen
    {
        [V2DSpriteAttribute(fixedRotation = false)]
        protected V2DSprite pivot;
        [V2DSpriteAttribute(fixedRotation = false)]
        protected V2DSprite anchor;
        [V2DSpriteAttribute(isSensor=true)]
        protected V2DSprite arm;

        [RevoluteJointAttribute(enableMotor=true, maxMotorTorque=50000, motorSpeed=4)]
        public RevoluteJoint rPivot;

        [RevoluteJointAttribute(enableMotor=true, maxMotorTorque=10000, motorSpeed=2)]
        public RevoluteJoint rAnchor;

        public RevoluteJoint[] rCirc;

        public SpinnerDemo(V2DContent v2dContent) : base(v2dContent) { }
        public SpinnerDemo(SymbolImport si) : base(si) { }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}








