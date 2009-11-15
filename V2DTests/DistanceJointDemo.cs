using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D;
using DDW.V2D.Serialization;
using Box2DX.Dynamics;
using DDW.Display;
using DDW.Input;
using VexRuntime.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using V2DTests;

namespace V2DTest
{
    public class DistanceJointDemo : V2DScreen
    {
        public Sprite bkg;
        public List<V2DSprite> hex;
        public V2DSprite star;
        public V2DSprite star2;
        public V2DSprite testCar;
        public MainMenuPanel mainMenuPanel;
        //public List<Button> menuItem;
        
        public DistanceJointDemo()
        {
            SymbolImport = new SymbolImport(@"Tests/DistanceJoint.xml");
        }
        public DistanceJointDemo(V2DContent v2dContent)  : base(v2dContent)
        {     
        }
        public DistanceJointDemo(SymbolImport si)  : base(si)
        {
            this.SymbolImport = si;
        }

        public override void SetValidInput()
        {
            base.SetValidInput();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
        }
        public override void Initialize()
        {
            base.Initialize();
            this.Play();
        }
    }
}








