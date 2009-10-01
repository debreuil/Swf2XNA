using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DDW.Display
{
    public class Stage : DisplayObjectContainer
    {
        protected Stage()
        {
        }
        internal virtual void ObjectAddedToStage(DisplayObject o)
        {
        }
        internal virtual void ObjectRemovedFromStage(DisplayObject o)
        {
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch batch)
        {
            batch.Begin();
            base.Draw(batch);
            batch.End();
        }
    }
}
