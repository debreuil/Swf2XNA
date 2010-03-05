using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D;
using Microsoft.Xna.Framework;

namespace V2DRuntime.Particles
{
	public class ParticleGroup : DisplayObjectContainer
	{
		public bool isComplete = false;
		public TimeSpan duration = new TimeSpan(0, 0, 0, 5);
		private bool isFirstUpdate = true;
		private TimeSpan startTime;

		public ParticleGroup()
        {
        }
		public ParticleGroup(Texture2D texture, V2DInstance inst) : base(texture, inst)
        {
        }

		public override void AddChild(DisplayObject o)
		{
			if (!isComplete)
			{
				base.AddChild(o);
				if (!(o is ParticleEffect))
				{
					throw new ArgumentException("Particle Groups can only contain ParticleEffects");
				}
				((ParticleEffect)o).IsGroupMember = true;
			}
		}
		public override void RemoveChild(DisplayObject o)
		{
			base.RemoveChild(o);
			if (!(o is ParticleEffect))
			{
				throw new ArgumentException("Particle Groups can only contain ParticleEffects");
			}
			((ParticleEffect)o).IsGroupMember = false;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (isFirstUpdate)
			{
				startTime = gameTime.TotalGameTime;
				isFirstUpdate = false;
			}

			if (gameTime.TotalGameTime - startTime > duration)
			{
				isComplete = true;
			}

			if (isComplete && this.children.Count == 0)
			{
				this.DestroyAfterUpdate();
			}
		}

		public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			if (children.Count > 0)
			{
				((ParticleEffect)children[0]).BeginDrawAllParticles(batch);
				base.Draw(batch);
				((ParticleEffect)children[0]).EndDrawAllParticles(batch);
			}
			else
			{
				base.Draw(batch);
			}
		}
	}
}
