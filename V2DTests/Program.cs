using System;
using DDW.V2D;

namespace V2DTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (V2DGame game = new TestGame())
            {
                game.Window.Title = "Flash to XNA with Box2D";
                game.IsFixedTimeStep = false;
                game.Run();
            }
        }
    }
}

