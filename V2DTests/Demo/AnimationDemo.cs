
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
    [ScreenAttribute(backgroundColor=0x222222)]
    public class AnimationDemo : Screen
    {
        protected Sprite sneeze;

        public AnimationDemo(SymbolImport si) : base(si) { }

        public override void Initialize()
        {
            base.Initialize();
            sneeze.PlayAll();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}








