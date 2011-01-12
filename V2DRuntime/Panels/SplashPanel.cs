using System;
using DDW.V2D;
using Microsoft.Xna.Framework.Graphics;
using V2DRuntime.Components;
using V2DRuntime.Display;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using DDW.Display;

namespace V2DRuntime.Panels
{
    public delegate void VideoEvent(DisplayObjectContainer sender, VideoPlayer videoPlayer);

	public class SplashPanel : Panel
	{
        public Sprite videoHolder;

        public event VideoEvent VideoEnded;
        public bool hasPlayedOnce = false;

        protected Video splashVideo;
        protected VideoPlayer videoPlayer;
        protected bool started = false;
        protected string videoName;
        protected Rectangle videoRect;


		public SplashPanel(Texture2D texture, V2DInstance inst) : base(texture, inst) { }

        public override void Initialize()
        {
            base.Initialize();
            videoName = "logoMovie";
        }

        public override void Activate()
        {
            base.Activate();
            videoPlayer = new VideoPlayer();
            splashVideo = V2DGame.instance.Content.Load<Video>(videoName);
            videoRect = new Rectangle((int)videoHolder.X, (int)videoHolder.Y, splashVideo.Width, splashVideo.Height);

            videoPlayer.Play(splashVideo);
            started = true;
        }
        public override void Deactivate()
        {
            base.Deactivate();

            if (videoPlayer != null)
            {
                videoPlayer.Stop();
                videoPlayer.Dispose();
            }
            splashVideo = null;
            videoPlayer = null;
        }
        protected virtual void OnVideoEnded()
        {
        }
        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);

            if (videoPlayer.State == MediaState.Playing)
            {
                batch.Draw(videoPlayer.GetTexture(), videoRect, Color.White);
            }
            else if (started)
            {
                started = false;
                hasPlayedOnce = true;
                if (VideoEnded != null)
                {
                    VideoEnded(this, videoPlayer);
                    videoPlayer.Dispose();
                }
                OnVideoEnded();
                //((Screen)parent).nextState = MenuState.MainMenu;
            }
        }
	}
}
