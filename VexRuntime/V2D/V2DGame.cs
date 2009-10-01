using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX.Dynamics;
using DDW.V2D.Serialization;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using Box2DX.Common;
using Microsoft.Xna.Framework.Graphics;
using DDW.Display;
using Microsoft.Xna.Framework.Input;
using DDW.Input;
using Microsoft.Xna.Framework.Content;

namespace DDW.V2D
{
    public abstract class V2DGame : Microsoft.Xna.Framework.Game
    {
        public static V2DStage stage;
        public static ContentManager contentManager;
        public const string ROOT_NAME = "_root";

        public static V2DGame instance;
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        protected List<Screen> screens = new List<Screen>();
        protected int curScreenIndex = 0;
        protected Screen curScreen;
        bool keyDown = false;
        Microsoft.Xna.Framework.Graphics.Color bkgColor = new Microsoft.Xna.Framework.Graphics.Color(60, 60, 80);

        protected V2DGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            contentManager = Content;
            GetCursor();
            stage = V2DStage.GetInstance();
            if (instance != null)
            {
                throw new Exception("There can be only one game class.");
            }
            instance = this;
        }
        
        private Cursor cursor;
        public Cursor GetCursor()
        {
            if (cursor == null)
            {
                cursor = new Cursor(this);
                Components.Add(cursor);
            }
            return cursor;
        }

        protected virtual void InitializeScreens()
        {
            //screenPaths.Add(symbolImports[i]);
        }
        protected override void Initialize()
        {
            base.Initialize();
            stage.Initialize();
            InitializeScreens();
            SetScreen(0);
        }

        public void SetScreen(int index)
        {
            if (curScreen != null)
            {
                stage.RemoveChild(curScreen);
            }
            curScreenIndex = index;
            if (curScreenIndex >= screens.Count)
            {
                curScreenIndex = 0;
            }
            else if (curScreenIndex < 0)
            {
                curScreenIndex = screens.Count - 1;
            }

            curScreen = screens[curScreenIndex];
            stage.AddChild(curScreen);
            if (curScreen is V2DScreen)
            {
                SetSize(((V2DScreen)curScreen).v2dWorld.Width, ((V2DScreen)curScreen).v2dWorld.Height);
            }
            // temp
            //if (curScreenIndex == 1)
            //{
            //    DisplayObject d = curScreen.GetChildByName("nest2Inst");
            //    d.Visible = false;
            //}

        }
        public void NextScreen()
        {
            SetScreen(curScreenIndex + 1);
        }
        public void PreviousScreen()
        {
            SetScreen(curScreenIndex - 1);
        }

        public void SetSize(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                graphics.PreferredBackBufferWidth = width;
                graphics.PreferredBackBufferHeight = height;
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
                stage.SetBounds(0, 0, width, height);
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }
            KeyboardState ks = Keyboard.GetState();
            if (!keyDown && ks.IsKeyDown(Keys.Left))
            {
                keyDown = true;
                PreviousScreen();
            }
            else if (!keyDown && ks.IsKeyDown(Keys.Right))
            {
                keyDown = true;
                NextScreen();
            }
            else if (keyDown && (ks.IsKeyUp(Keys.Left) && ks.IsKeyUp(Keys.Right)))
            {
                keyDown = false;
            }

            stage.Update(gameTime);

            base.Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(ClearOptions.Stencil, bkgColor, 1, 1);
            GraphicsDevice.Clear(bkgColor);

            stage.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
