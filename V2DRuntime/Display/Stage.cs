using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DDW.V2D;
using V2DRuntime.Shaders;
#if !(WINDOWS_PHONE)
using V2DRuntime.Audio;
#endif

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

#if !(WINDOWS_PHONE)
        public AudioManager audio;
        public AudioManager music;
#endif

        public float MillisecondsPerFrame = 1000f / 12f;

        private Color clearColor;
		
        protected Stage()
        {
            stage = this;
        }

#if !(WINDOWS_PHONE)

        public void InitializeAudio(string audioEnginePath, string waveBankPath, string soundBankPath)
        {
            audio = new AudioManager(audioEnginePath, waveBankPath, soundBankPath);
        }

        public void InitializeMusic(string musicEnginePath, string waveBankPath, string soundBankPath)
        {
            music = new AudioManager(musicEnginePath, waveBankPath, soundBankPath);
        }

#endif

        public void AddScreen(Screen scr)
        {
            if (scr.isPersistantScreen)
            {
                this.AddChild(scr);
            }
            else
            {
                screens.Add(scr);
            }
        }

        public void RemoveScreen(Screen scr)
        {
            screens.Remove(scr);
            if (scr == curScreen)
            {
                Screen nullScreen = null;
                SetScreen(nullScreen);
            }
        }

        public Screen GetCurrentScreen()
        {
            return curScreen;
        }
        public Screen GetPreviousScreen()
        {
            return prevScreen;
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
            if (curScreen != null)
            {
                prevScreen = curScreen;
                // can remove on fade etc here
                curScreen.isActive = false;
                //stage.RemoveChild(curScreen);
            }

            if (scr != null)
            {
                screenChanged = true;
                curScreenIndex = screens.IndexOf(scr);
                curScreen = scr;
            }
        }
        public void SetScreen(string screenName)
        {
            Screen s = null;
            for (int i = 0; i < screens.Count; i++)
            {
                if (screens[i].InstanceName == screenName)
                {
                    s = screens[i];
                    break;
                }
            }
            SetScreen(s);
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
				DepthCounter = 1;
                batch.GraphicsDevice.Clear(clearColor);

				base.Draw(batch);

				curScreen.DrawDebugData(batch);
        }
    }
}






