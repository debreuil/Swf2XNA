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

namespace V2DTest
{
    public class DistanceJointDemo : V2DScreen
    {
        public Sprite bkg;
        public List<V2DSprite> hex;
        public V2DSprite star;
        public V2DSprite star2;
        public V2DSprite car;
        //public List<Button> menuItem;
        public ButtonTabGroup mainMenu;
        
        public DistanceJointDemo()
        {
            SymbolImport = new SymbolImport(@"Tests/DistanceJoint.xml");
        }
        public DistanceJointDemo(V2DContent v2dContent)  : base(v2dContent)
        {     
        }

        public override void SetValidInput()
        {
            base.SetValidInput();
        }
        public override void OnPlayerInput(int playerIndex, Move move, TimeSpan time)
        {
            if (move.Name == "Up")
            {
                mainMenu.PreviousFocus();
            }
            else if(move.Name == "Down")
            {
                mainMenu.NextFocus();
            }
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
            //bkg.Visible = false;
            bkg.Play();
            //star2.Play();
            //star.Play();
            //car.GotoAndStop(4);
            //star2.Alpha = .5f;
            //hex[0].Alpha = .5f;
            //hex[1].Alpha = .5f;
            //btPlay.Select();
            //menuItem[1].Visible = false;
            mainMenu.SetFocus(1);
        }
    }
}








