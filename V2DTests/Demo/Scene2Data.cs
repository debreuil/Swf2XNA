
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
    public class Scene2Data : V2DScreen
    {
        [V2DSpriteAttribute(fixedRotation=false)]
        public V2DSprite triangleBase;

        public Scene2Data(V2DContent v2dContent) : base(v2dContent) { }
        public Scene2Data(SymbolImport si) : base(si) { }

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
















