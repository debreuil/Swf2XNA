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

	public class VideoPanel : Panel
	{
        public Sprite videoHolder;

        public event VideoEvent VideoEnded;
        public bool hasPlayedOnce = false;

        protected Video video;
        protected VideoPlayer videoPlayer;
        protected bool started = false;
        protected bool completed = false;
        protected string videoName;
        protected Rectangle videoRect;


		public VideoPanel(Texture2D texture, V2DInstance inst) : base(texture, inst) { }

        public override void Initialize()
        {
            base.Initialize();
            videoName = "logoMovie";
        }

        public override void Activate()
        {
            base.Activate();
            videoPlayer = new VideoPlayer();
            video = V2DGame.instance.Content.Load<Video>(videoName);
            videoRect = new Rectangle((int)videoHolder.X, (int)videoHolder.Y, video.Width, video.Height);

            videoPlayer.Play(video);
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
            video = null;
            videoPlayer = null;
        }
        protected virtual void OnVideoEnded()
        {
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (completed)
            {
                completed = false;
                if (VideoEnded != null)
                {
                    VideoEnded(this, videoPlayer);
                }
                OnVideoEnded();
            }
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
                hasPlayedOnce = true;
                started = false; 
                completed = true;
                //((Screen)parent).nextState = MenuState.MainMenu;
            }
        }
	}
}
