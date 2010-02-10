using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D;
using DDW.Input;
using Microsoft.Xna.Framework.GamerServices;

namespace V2DRuntime.Components
{
    public class ButtonTabGroup : Group<Button>
    {
        private int focusIndex = -1;
		public bool wrapAround = false;
		private bool selectionDown = false;

        public ButtonTabGroup(Texture2D texture, V2DInstance inst) : base(texture, inst)
        {
        }

        public void SetFocus(int index)
        {
            focusIndex = index;
            if (focusIndex > element.Count - 1)
            {
                if (wrapAround)
                {
                    focusIndex = 0;
                }
                else
                {
                    focusIndex = element.Count - 1;
                }
            }
            else if (focusIndex < 0)
            {
                if (wrapAround)
                {
                    focusIndex = element.Count - 1;
                }
                else
                {
                    focusIndex = 0;
                }
            }

            for (int i = 0; i < element.Count; i++)
            {
                if (i == focusIndex)
                {
                    element[i].Select();
                }
                else
                {
                    element[i].Deselect();
                }
            }

        }
        public void NextFocus()
        {
            SetFocus(focusIndex + 1);
        }
        public void PreviousFocus()
        {
            SetFocus(focusIndex - 1);
        }
        public override bool OnPlayerInput(int playerIndex, DDW.Input.Move move, TimeSpan time)
        {
            bool result = base.OnPlayerInput(playerIndex, move, time);

			if (result && !Guide.IsVisible)
			{
				if (move == Move.Up)
				{
					PreviousFocus();
				}
				else if (move == Move.Down)
				{
					NextFocus();
				}
				else if (focusIndex > -1 && move == Move.ButtonA)
				{
					selectionDown = true;
					element[focusIndex].Press();
				}
				else if (move == Move.Empty)
				{
					element[focusIndex].Release();
					if (selectionDown && OnClick != null)
					{
						OnClick(element[focusIndex], playerIndex, time);
					}
					selectionDown = false;
				}
			}
			return result;
        }
        public delegate void ButtonEventHandler(Button sender, int playerIndex, TimeSpan time);
        public event ButtonEventHandler OnClick;

		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);
		}
    }
}
