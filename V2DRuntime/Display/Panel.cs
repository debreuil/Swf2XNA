using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using DDW.V2D;
using Microsoft.Xna.Framework.Graphics;
using DDW.Input;

namespace V2DRuntime.Display
{
    public abstract class Panel : Sprite
    {
        public bool IsDialog;
        public Panel(Texture2D texture, V2DInstance inst): base(texture, inst)
        {
            //isActive = false;
        }

		public override bool OnPlayerInput(int playerIndex, Move move, TimeSpan time)
		{
			bool result = true;
			if (isActive)
			{
				result = base.OnPlayerInput(playerIndex, move, time);
			}
			return result;
		}
		public override void Activate()
		{
            base.Activate();
			this.Visible = true;
		}
        public override void Deactivate()
		{
            base.Deactivate();
			this.Visible = false;
		}
    }
}
