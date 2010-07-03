using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DDW.V2D;
using V2DRuntime.Shaders;
using V2DRuntime.Audio;
using Microsoft.Xna.Framework.GamerServices;

namespace DDW.Display
{
    public class Stage : DisplayObjectContainer
    {
        public static Matrix SpriteBatchMatrix = Matrix.Identity;

        protected List<Screen> screens = new List<Screen>();
        protected int curScreenIndex = 0;
        protected Screen curScreen;
		protected Screen prevScreen;
		public bool pause = false;
        public V2DShader defaultEffect;

        public AudioManager audio;
        public AudioManager music;

        public float MillisecondsPerFrame = 1000f / 12f;

        private Color clearColor;
		
        protected Stage()
        {
            stage = this;
        }

        public void InitializeAudio(string audioEnginePath, string waveBankPath, string soundBankPath)
        {
            audio = new AudioManager(audioEnginePath, waveBankPath, soundBankPath);
        }

        public void InitializeMusic(string musicEnginePath, string waveBankPath, string soundBankPath)
        {
            music = new AudioManager(musicEnginePath, waveBankPath, soundBankPath);
        }

        public void AddScreen(Screen scr)
        {
            screens.Add(scr);
        }
        public void RemoveScreen(Screen scr)
        {
            screens.Remove(scr);
        }

		public Screen GetCurrentScreen()
		{
			return curScreen;
		}
		public Screen GetNextScreen()
        {
            int indx = curScreenIndex + 1 >= screens.Count ? 0 : curScreenIndex + 1;
            return screens[indx];
        }
        public Screen GetScreenByIndex(int indx)
        {
            Screen result = null;
            if (indx >= 0 && indx < screens.Count)
            {
                result = screens[indx];
            }
            return result;
        }
        private bool screenChanged = true;
        public void SetScreen(Screen scr)
        {
            if (scr != null)
            {
                if (curScreen != null)
                {
                    screenChanged = true;
                    prevScreen = curScreen;
                    // can remove on fade etc here
                    curScreen.isActive = false;
                    //stage.RemoveChild(curScreen);
                }

                curScreenIndex = screens.IndexOf(scr);
                curScreen = scr;
            }
        }
        public void SetScreen(string screenName)
        {
            Screen scr = screens.Find(sc => sc.InstanceName == screenName);
            SetScreen(scr);
        }
        public void SetScreen(int index)
        {
            if (index >= screens.Count)
            {
                index = 0;
            }
            else if (index < 0)
            {
                index = screens.Count - 1;
            }

            SetScreen(screens[index]);
        }
        public void NextScreen()
        {
            SetScreen(curScreenIndex + 1);
        }
        public void PreviousScreen()
        {
            SetScreen(curScreenIndex - 1);
        }

        internal virtual void ObjectAddedToStage(DisplayObject o)
        {
			o.AddedToStage(EventArgs.Empty);
        }
        internal virtual void ObjectRemovedFromStage(DisplayObject o)
        {
        }

		public virtual void SetBounds(float x, float y, float w, float h)
		{
		}

        public override void Update(GameTime gameTime)
        {
            //base.Update(gameTime); // dont update stage itself as it is fixed pos
			if (V2DGame.instance.IsActive)
			{
				foreach (DisplayObject d in children)
				{
					d.Update(gameTime);
				}

				if (prevScreen != null && children.Contains(prevScreen) && !prevScreen.isActive)
				{
                    prevScreen.DestroyView();
                    V2DGame.instance.RemovingScreen(prevScreen);
					this.RemoveChild(prevScreen);
					prevScreen = null;
				}

				if (screenChanged)// && !children.Contains(curScreen))
				{
                    screenChanged = false;
                    V2DGame.instance.AddingScreen(curScreen);
                    this.AddChild(curScreen);
                    clearColor = curScreen.Color;
                    clearColor.A = 0;
				}
			}
		}
        public override void Draw(SpriteBatch batch)
        {
            //if (V2DGame.instance.IsActive)
            //{

				DepthCounter = 1;
                batch.GraphicsDevice.Clear(clearColor);
				//batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
				batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, Stage.SpriteBatchMatrix);
				base.Draw(batch);
				batch.End();

				curScreen.DrawDebugData(batch);
			//}
        }
    }
}






