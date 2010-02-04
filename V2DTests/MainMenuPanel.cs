using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using V2DRuntime.Components;
using DDW.Input;
using DDW.V2D;
using Microsoft.Xna.Framework.Graphics;
using V2DRuntime.Display;

namespace V2DTests
{
    public class MainMenuPanel : Panel
    {
        public ButtonTabGroup mainMenu;
        public MainMenuPanel(Texture2D texture, V2DInstance inst) : base(texture, inst) { }

        void mainMenu_OnClick(Button sender, int playerIndex, TimeSpan time)
        {
            Console.WriteLine("Click " + sender);
        }

        public override void Initialize()
        {
            base.Initialize();
            mainMenu.SetFocus(1);
            mainMenu.OnClick += new ButtonTabGroup.ButtonEventHandler(mainMenu_OnClick);
        }

    }
}
