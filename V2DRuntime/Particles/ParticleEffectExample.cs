//#define useParticles

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D;
using SharpNeatLib.Maths;


namespace V2DRuntime.Particles
{
	public class ParticleEffectExample : ParticleEffect
	{
		public ParticleEffectExample()
		{
		}
		public ParticleEffectExample(Texture2D texture, V2DInstance inst)
			: base(texture, inst)
		{
		}
		public override void Initialize()
		{
			base.Initialize();
			AutoStart = false;
			maxT = 1;
		}

		protected override void EffectSetup(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.EffectSetup(gameTime);
			textureCount = 4;
		}
		protected override void BatchUpdate(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.BatchUpdate(gameTime);

			pColor.A = (byte)Easing.Sin(t, 0, 40, .5f);
			pColor.R = (byte)Easing.Linear(t, 0, 255f);
			pColor.G = (byte)Easing.Linear(t, 255, 0);
			effectOrigin.X = Easing.Linear(t, 0, r1 * 50);//r2 * 2;			
			//particleCount = (int)Easing.Sin(t, 0, 10000, .5f);
		}
		protected override void BatchDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			base.BatchDraw(batch);
		}
		protected override void ParticleDraw(int index, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			sourceRectangle.X = (index % 4) * particleWidth;
			pRotation = Easing.EaseOutQuad(t, 0, r0 * 31.4159f * 10);

			float len = Easing.EaseOutElastic(t * .5f, 0, r0 * 100, 400);
			float pt = index / (float)particleCount;
			pPosition.X = (float)Easing.Sin(pt, 0, len) + effectPosition.X;
			pPosition.Y = (float)Easing.Cos(pt, 0, len) + effectPosition.Y;

			base.ParticleDraw(index, batch);
		}
	}
}
