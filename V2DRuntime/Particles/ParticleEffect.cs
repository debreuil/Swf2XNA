using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using SharpNeatLib.Maths;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DDW.V2D;

namespace V2DRuntime.Particles
{
	public abstract class ParticleEffect : DisplayObjectContainer
	{		
		protected float t;
		protected int seed;
		protected float maxT = 1;
		protected FastRandom rnd = new FastRandom();
		protected int particleCount = 2000;
				
		private bool isFirstUpdate = false;

		public uint textureCount = 1;
		protected int particleHeight;
		protected int particleWidth;

		protected int count = 0;
		protected int steps = 500;
		protected Vector2 effectPosition;
		protected float effectRotation;
		protected Vector2 effectScale;
		protected Vector2 effectOrigin;

		protected Vector2 pPosition;
		protected float pRotation;
		protected Vector2 pScale;
		protected Vector2 pOrigin;
		protected Color pColor;
		protected float r0;
		protected float r1;
		protected float r2;
		protected float r3;

		protected TimeSpan StartTime = TimeSpan.Zero;
		protected TimeSpan Duration = new TimeSpan(0, 0, 1); // default 1 second

		public bool AutoStart { get; set; }
		public bool IsGroupMember { get; set; }

		public ParticleEffect()
		{
		}
		public ParticleEffect(Texture2D texture, V2DInstance inst): base(texture, inst)
		{
		}
		public override void Initialize()
		{
			base.Initialize();
            isActive = true;

			seed = (int)DateTime.Now.Ticks;

			if (texture == null)
			{
				foreach (DisplayObject d in children)
				{
					if (d.Texture != null)
					{
						// use only the first texture - have multiple textures in equal sized rectangles
						// horizontally across it. Multiple textures affect performance a lot.
						texture = d.Texture;
						break;
					}
				}
			}
		}
		public virtual void Begin()
		{
			isFirstUpdate = true;
		}
		public virtual void End()
		{
			isActive = false;
			this.DestroyAfterUpdate();
		}
		public override void AddedToStage(EventArgs e)
		{
			base.AddedToStage(e);
			if (AutoStart)
			{
				isFirstUpdate = true;
			}
		}

		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			if (isFirstUpdate)
			{
				EffectSetup(gameTime);
				TextureSetup();
				isFirstUpdate = false;
			}
			BatchUpdate(gameTime);
		}

		protected virtual void EffectSetup(Microsoft.Xna.Framework.GameTime gameTime)
		{
			isActive = true;
			StartTime = gameTime.TotalGameTime;
			t = 0;
			count = 0;
		}

		protected virtual void TextureSetup()
		{
			// should have a texture by now.
			if (texture == null)
			{
				throw new NullReferenceException("Texture on particle effect can not be null");
			}

			particleHeight = texture.Height;
			particleWidth = (int)(texture.Width / textureCount);
			sourceRectangle = new Rectangle(0, 0, particleWidth, particleHeight);
			State.Origin = new Vector2(particleWidth / 2, particleHeight / 2);
		}
		protected virtual void BatchUpdate(Microsoft.Xna.Framework.GameTime gameTime)
		{
			effectPosition = GetGlobalOffset(Vector2.Zero);
			effectRotation = GetGlobalRotation(0);
			effectScale = GetGlobalScale(new Vector2(1, 1));
			effectOrigin = State.Origin;

			pPosition = effectPosition;
			pRotation = effectRotation;
			pScale = effectScale;
			pOrigin = effectOrigin;
			pColor = color;

			count++;
			t = count / (float)steps;

			if (t >= maxT)
			{
				End();
			}
		}
		protected virtual void BatchDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			rnd.Reinitialise(seed);
			r1 = (float)rnd.NextDouble();
			r2 = (float)rnd.NextDouble();
			r3 = (float)rnd.NextDouble();

			for (int i = 0; i < particleCount; i++)
			{
				r0 = (float)rnd.NextDouble();
				ParticleDraw(i, batch);
				r3 = r2;
				r2 = r1;
				r1 = r0;
			}
		}
		protected virtual void ParticleDraw(int index, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			batch.Draw(texture, pPosition, sourceRectangle, pColor,
				pRotation, pOrigin, pScale, SpriteEffects.None, 1);
		}

		public virtual void BeginDrawAllParticles(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
		}
		public virtual void EndDrawAllParticles(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
		}
		public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			if (isActive)
			{
				if (!IsGroupMember)
				{
					BeginDrawAllParticles(batch);
					BatchDraw(batch);
					EndDrawAllParticles(batch);
				}
				else
				{
					BatchDraw(batch);
				}
			}
		}

	}
}
