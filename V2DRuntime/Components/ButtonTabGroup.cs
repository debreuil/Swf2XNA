using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D;
using DDW.Input;
using Microsoft.Xna.Framework.GamerServices;
using V2DRuntime.Display;

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
            int orgIndex = focusIndex;
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

            if (orgIndex != -1 && orgIndex != focusIndex && OnFocusChanged != null)
            {
                OnFocusChanged(element[focusIndex]);
            }
        }

        public void NextFocus()
        {
            int sanityCount = 0;
            do
            {
                SetFocus(focusIndex + 1);
            }
            while (!element[focusIndex].Visible && sanityCount++ < element.Count); // allow for non visible buttons to be skipped
        }

        public void PreviousFocus()
        {
            int sanityCount = 0;
            do
            {
                SetFocus(focusIndex - 1);
            }
            while (!element[focusIndex].Visible && sanityCount++ < element.Count); // allow for non visible buttons to be skipped
        }

        public override bool OnPlayerInput(int playerIndex, DDW.Input.Move move, TimeSpan time)
        {
            bool result = base.OnPlayerInput(playerIndex, move, time);

			if (result && isActive)// && !Guide.IsVisible)
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

        public event ButtonEventHandler OnClick;
        public event FocusChangedEventHandler OnFocusChanged;

    }
}
