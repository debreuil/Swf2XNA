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
    [V2DScreenAttribute(backgroundColor=0x222222, gravityX=0, gravityY=100,debugDraw=true)]
    public class PulleyJointDemo : V2DScreen
    {
        [PulleyJointAttribute(ratio=2)]
        public PulleyJoint pu1;

        public PulleyJointDemo(V2DContent v2dContent) : base(v2dContent) { }
        public PulleyJointDemo(SymbolImport si) : base(si) { }

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








