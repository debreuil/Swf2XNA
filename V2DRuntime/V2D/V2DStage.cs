using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX.Dynamics;
using DDW.V2D.Serialization;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using Box2DX.Common;
using Microsoft.Xna.Framework.Graphics;
using DDW.Display;
using Microsoft.Xna.Framework.Input;
using DDW.Input;
using V2DRuntime.Enums;

namespace DDW.V2D
{
    public class V2DStage : Stage
    {
        public V2DWorld v2dWorld;
    

        public V2DStage()
        {
        }


		public override void AddChild(DisplayObject o)
		{
			base.AddChild(o);
		}
        internal override void ObjectAddedToStage(DisplayObject o)
        {
            if (o is V2DSprite)
            {
                V2DSprite sp = (V2DSprite)o;
                sp.AddBodyInstanceToRuntime();
            }

            base.ObjectAddedToStage(o);
        }
        internal override void ObjectRemovedFromStage(DisplayObject o)
        {
            base.ObjectRemovedFromStage(o);

            if (o is V2DScreen)
            {
                V2DScreen scr = (V2DScreen)o;
                scr.Deactivate();
            }
			//else if (o is V2DSprite)
			//{
			//    if (o.Parent != null)
			//    {
			//        V2DSprite sp = (V2DSprite)o;
			//        sp.RemoveBodyInstanceFromRuntime();
			//    }
			//}
		}
		public override void SetBounds(float x, float y, float w, float h)
		{
			curScreen.SetBounds(x, y, w, h);
		}
    }
}
