using System;
using System.Collections.Generic;
using DDW.Display;
using DDW.Input;
using DDW.V2D.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#if !(WINDOWS_PHONE)
using Microsoft.Xna.Framework.Net;
using V2DRuntime.Network;
#endif
using Microsoft.Xna.Framework.GamerServices;

namespace DDW.V2D 
{
    public abstract class V2DGame : Microsoft.Xna.Framework.Game
    {
        public static V2DGame instance;
        public static Stage stage;
        public static ContentManager contentManager;
        public const string ROOT_NAME = V2DWorld.ROOT_NAME;
        public static string currentRootName = V2DWorld.ROOT_NAME;

        public static PlayerIndex activeController = PlayerIndex.One;
        private bool wasTrialMode;

        protected GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        protected bool keyDown = false;
        protected bool isFullScreen = false;

#if !(WINDOWS_PHONE)
		public List<NetworkGamer> gamers = new List<NetworkGamer>();
#endif

        protected V2DGame()
        {
            if (instance != null)
            {
                throw new Exception("There can be only one game class.");
            }
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            contentManager = Content;
            Content.RootDirectory = "Content";
			stage = new V2DStage();

            GetCursor();
        }

        public virtual bool HasCursor { get { return false; } }

        private Cursor cursor;
        public Cursor GetCursor()
        {
            if (HasCursor && cursor == null)
            {
                cursor = new Cursor(this);
                Components.Add(cursor);
            }
            return cursor;
        }

        protected virtual void CreateScreens()
        {
            //screenPaths.Add(symbolImports[i]);
        }
        public virtual void AddingScreen(Screen screen) { }
        public virtual void RemovingScreen(Screen screen) { }
        protected override void Initialize()
        {
            base.Initialize();

            stage.Initialize();
            CreateScreens();
            stage.SetScreen(0);
        }

        public void SetSize(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                graphics.SynchronizeWithVerticalRetrace = true;
                graphics.PreferredBackBufferWidth = width;
                graphics.PreferredBackBufferHeight = height;
                graphics.IsFullScreen = this.isFullScreen;
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

#if !(WINDOWS_PHONE)

		public virtual void ExitToMainMenu()
        {
			NetworkManager.Instance.LeaveSession();
        }
        public virtual void AllLevelsComplete()
        {
        }

#endif

        public virtual void UnlockTrial()
        {
            SignedInGamer gamer = Gamer.SignedInGamers[V2DGame.activeController];
            if (gamer != null)
            {
                if (gamer.Privileges.AllowPurchaseContent)
                {
                    ShowMarketPlace();
                }
                else
                {
                    if (gamer != null)
                    {
                        if (stage != null && stage.GetCurrentScreen() != null)
                        {
                            stage.GetCurrentScreen().SignInToLive();
                        }
                        else
                        {
                            ShowSignIn();
                        }
                    }
                    else
                    {
                        ShowSignIn();
                    }
                }
            }
            else
            {
                ShowSignIn();
            }
        }
        public virtual void FullGameUnlocked()
        {
        }
        public virtual void ExitGame()
        {
			this.Exit();
        }

        public virtual void ShowMarketPlace()
        {
            try
            {
                Guide.ShowMarketplace(V2DGame.activeController);
            }
            catch (Exception){}
        }
        public virtual void ShowSignIn()
        {
            if (!Guide.IsVisible)
            {
                try
                {
                    Guide.ShowSignIn(1, true);
                }
	            catch (Exception){}
            }
        }
        public static bool CanPlayerBuyGame(PlayerIndex player)
        {
            bool result = false;
            SignedInGamer gamer = Gamer.SignedInGamers[player];
            if (gamer != null)
            {
                result = gamer.Privileges.AllowPurchaseContent;
            }
            return result;
        }

#if !(WINDOWS_PHONE)

		public virtual void AddGamer(NetworkGamer gamer, int gamerIndex)
		{
			if (!gamers.Contains(gamer))
			{
				gamers.Add(gamer);
			}
		}
		public virtual void RemoveGamer(NetworkGamer gamer)
		{
			if (gamers.Contains(gamer))
			{
				gamers.Remove(gamer);
			}
		}

#endif

        protected override void Update(GameTime gameTime)
        {
			stage.Update(gameTime);
            base.Update(gameTime);

            if (!Guide.IsTrialMode && wasTrialMode)
            {
                FullGameUnlocked();
            }
            wasTrialMode = Guide.IsTrialMode;
        }

        protected override void Draw(GameTime gameTime)
		{
            stage.Draw(spriteBatch);

            base.Draw(gameTime);
        }

    }
}
