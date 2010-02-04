using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D;

namespace V2DRuntime.Particles
{
	public class ParticleGroup : DisplayObjectContainer
	{
		public ParticleGroup()
        {
        }
		public ParticleGroup(Texture2D texture, V2DInstance inst) : base(texture, inst)
        {
        }

		public override void AddChild(DisplayObject o)
		{
			base.AddChild(o);
			if (!(o is ParticleEffect))
			{
				throw new ArgumentException("Particle Groups can only contain ParticleEffects");
			}
			((ParticleEffect)o).IsGroupMember = true;
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
