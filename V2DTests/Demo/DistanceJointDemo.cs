using System.Collections.Generic;
using DDW.Display;
using DDW.V2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace V2DTest
{
    public class DistanceJointDemo : V2DScreen
	{
        public Sprite bkg;
        public List<V2DSprite> hex;
        
        public DistanceJointDemo(V2DContent v2dContent)  : base(v2dContent)
        {     
        }
        public DistanceJointDemo(SymbolImport si)  : base(si)
        {
        }

		public override void Initialize()
		{
			base.Initialize();
			this.Play();
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
    }
}








