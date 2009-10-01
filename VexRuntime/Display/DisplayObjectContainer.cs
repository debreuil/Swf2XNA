using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DDW.Display
{
    public class DisplayObjectContainer : DisplayObject
    {
        protected List<DisplayObject> children = new List<DisplayObject>();

        public int NumChildren
        {
            get
            {
                return children.Count;
            }
        }       
	    public virtual void AddChild(DisplayObject o)
        {
            o.Parent = this;
		    children.Add(o);
            o.Added(EventArgs.Empty);
	    }
        public virtual void AddChildAt(int index, DisplayObject o)
        {
            o.Parent = this;
		    children.Insert(index, o);
            o.Added(EventArgs.Empty);
	    }
        public virtual bool Contains(DisplayObject o)
	    {
		    return children.Contains(o);
	    }
        public virtual Rectangle GetBounds()
        {
            return Rectangle.Empty;
        }
        public virtual DisplayObject GetChildAt(int index)
        {
            DisplayObject result = null;
            if(index > 0 && index < children.Count)
            {
		        result = children[index];
            }
            return result;
        }
        public virtual DisplayObject GetChildByName(string name)
        {
            DisplayObject result = null;
            for (int i = 0; i < children.Count; i++)
		    {
                if (children[i].InstanceName == name)
                {
                    result = children[i];
                    break;
                }
		    }
            return result;
        }
        public virtual int GetChildIndex(DisplayObject o)
        {
		    return children.IndexOf(o);
        }
        public virtual void RemoveChild(DisplayObject o)
        {
            o.Removed(EventArgs.Empty);
		    children.Remove(o);
        }
        public virtual void RemoveChildAt(int index)
        {
            if (index > 0 && index < children.Count)
            {
                RemoveChild(children[index]); // todo: display objects may need to compare on depth
            }
        }
        public virtual void Clear()
        {
            children.ForEach(RemoveChild);
        }

        public virtual void SetChildIndex(int index, DisplayObject o)
        {
            int curIndex = GetChildIndex(o);
            if (curIndex != -1 && curIndex != index && index > 0 && index < children.Count)
            {
                children.RemoveAt(curIndex);
                children.Insert(index, o);
            }
        }
        public virtual void SwapChildren(DisplayObject a, DisplayObject b)
        {
            int index0 = children.IndexOf(a);
            int index1 = children.IndexOf(b);
            SwapChildrenAt(index0, index1);
        }
        public virtual void SwapChildrenAt(int index0, int index1)
        {
            if (index0 > 0 && index0 < children.Count && index1 > 0 && index1 < children.Count)
            {
                DisplayObject a = children[index0];
                DisplayObject b = children[index1];

                children.RemoveAt(index0);
                children.RemoveAt(index1);

                if (index0 < index1)
                {
                    children.Insert(index0, a);
                }
                else
                {
                    children.Insert(index1, b);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (DisplayObject d in children)
            {
                d.Update(gameTime);
            }
        }
        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            foreach (DisplayObject d in children)
            {
                if (d.Visible && d.Alpha > 0)
                {
                    if (CurFrame >= d.StartFrame && CurFrame < d.StartFrame + d.TotalFrames)
                    {
                        d.Draw(batch);
                    }
                }
            }
        }

        public override void Added(EventArgs e)
        {
            foreach (DisplayObject d in children)
            {
                d.Added(e);
            }
            base.Added(e);
        }
        public override void Removed(EventArgs e)
        {
            foreach (DisplayObject d in children)
            {
                d.Removed(e);
            }
            base.Removed(e);
        }
        public override void AddedToStage(EventArgs e)
        {
            foreach (DisplayObject d in children)
            {
                d.AddedToStage(e);
            }
            base.AddedToStage(e);
        }
        public override void RemovedFromStage(EventArgs e)
        {
            foreach (DisplayObject d in children)
            {
                d.RemovedFromStage(e);
            }
            base.RemovedFromStage(e);
            children.Clear();
        }
    }
}
