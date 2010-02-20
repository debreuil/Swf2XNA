using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D;

namespace V2DRuntime.Components
{
    /// <summary>
    /// frame 0:normal, 1:over, 2:down, 3:hitArea, 4:focus, 5:disabled
    /// </summary>
    public class Button : Sprite
    {
        // todo: move the event managment into button

        private bool isOver;
        private bool isDown;
        private bool isSelected;
        private bool isEnabled = true;
        
		//public Button(Texture2D texture) : base(texture)
		//{
		//}
        public Button(Texture2D texture, V2DInstance inst): base(texture, inst)
        {
        }


        public void Enable()
        {
            isEnabled = true;
            ResetViewState();
        }
        public void Disable()
        {
            isEnabled = false;
            ResetViewState();
        }
        public void Select()
        {
            isSelected = true;
            ResetViewState();
        }
        public void Deselect()
        {
            isSelected = false;
            ResetViewState();
        }
        public void RollOver()
        {
            isOver = true;
            ResetViewState();
        }
        public void RollOut()
        {
            isOver = false;
            ResetViewState();
        }
        public void DragOver()
        {
            isOver = true;
            ResetViewState();
        }
        public void DragOut()
        {
            isOver = false;
            ResetViewState();
        }
        public void Press()
        {
            isDown = true;
            ResetViewState();
        }
        public void Release()
        {
            isDown = false;
            ResetViewState();
        }

        public void ResetViewState()
        {
            if (!isEnabled && FrameCount >= 6)
            {
                this.GotoAndStop(5);
            }
            else if (isDown && this.FrameCount >= 3)
            {
                this.GotoAndStop(2);
            }
            else if (isOver && this.FrameCount >= 2)
            {
                this.GotoAndStop(1);
            }
            else if (isSelected && this.FrameCount >= 5)
            {
                this.GotoAndStop(4);
            }
            else if (isSelected && this.FrameCount >= 3)  // use down if no select
            {
                this.GotoAndStop(1);
            }
            else
            {
                this.GotoAndStop(0);
            }
		}
		protected override void DrawChild(DisplayObject d, SpriteBatch batch)
		{
			base.DrawChild(d, batch);
		}
    }
}
