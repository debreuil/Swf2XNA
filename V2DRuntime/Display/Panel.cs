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
        public bool IsActive = true;
        public bool IsDialog;
        public Panel(Texture2D texture, V2DInstance inst): base(texture, inst){}

		public override bool OnPlayerInput(int playerIndex, Move move, TimeSpan time)
		{
			bool result = true;
			if (IsActive)
			{
				result = base.OnPlayerInput(playerIndex, move, time);
			}
			return result;
		}
		public virtual void Activate()
		{
			this.Visible = true;
			this.IsActive = true;
		}
		public virtual void Deactivate()
		{
			this.Visible = false;
			this.IsActive = false;
		}
    }
}
