using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D;
using DDW.V2D.Serialization;
using Box2D.XNA;
using DDW.Display;
using DDW.Input;
using V2DRuntime.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using V2DTests;
using XnaColor = Microsoft.Xna.Framework.Graphics.Color;

namespace V2DTest
{
    public class DistanceJointDemo : V2DScreen
	{
		SpriteFont spriteFont1;

        public Sprite bkg;
        public List<V2DSprite> hex;
        public V2DSprite star;
        public V2DSprite star2;
        public V2DSprite testCar;
        public MainMenuPanel mainMenuPanel;

		public TextBox txTitle;
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
        }

		public override void Initialize()
		{
			base.Initialize();
			spriteFont1 = V2DGame.contentManager.Load<SpriteFont>(@"Arial");
			TextBox tb = new TextBox(null);
			tb.State.StartFrame = 0;
			tb.State.EndFrame = 9;
			tb.Color = Microsoft.Xna.Framework.Graphics.Color.DarkSalmon;
			tb.X = 100;
			tb.Y = 200;
			tb.Text = "ZZZZ ZZZZZZ";
			this.AddChild(tb);
			this.Play();
		}
        public override void SetValidInput()
        {
            base.SetValidInput();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
			//txTitle.Text = "changed changed changed changed changed \n changed changed changed ";
        }
        public override void Draw(SpriteBatch batch)
        {
			base.Draw(batch);
        }
    }
}








