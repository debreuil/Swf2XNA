using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace V2DRuntime.Shaders
{
	public abstract class V2DShader
	{
		protected Effect effect;

		public V2DShader(params float[] parameters)
		{
		}
		protected abstract void LoadContent();

        public abstract void Begin(SpriteBatch batch);
        public abstract void End(SpriteBatch batch);

		public virtual void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			if(obj is V2DShader)
			{
				result = effect.Equals(((V2DShader)obj).effect);
			}
			return result;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() + effect.GetHashCode();
		}
	}
}
