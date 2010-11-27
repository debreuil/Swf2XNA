using DDW.Display;
using DDW.V2D.Serialization;

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
            // curScreen may be null if there are only persistant screens added (huds etc)
            if (curScreen != null)
            {
                curScreen.SetBounds(x, y, w, h);
            }
		}
    }
}
