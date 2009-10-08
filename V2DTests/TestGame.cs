using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D;

namespace V2DTest
{
    public class TestGame : V2DGame
    {
        protected override void InitializeScreens()
        {
            screens.Add(new V2DScreen(contentManager.Load<V2DContent>("Germs")));
            //screens.Add(new DistanceJointDemo(contentManager.Load<V2DContent>("DistanceJoint")));
            //screens.Add(new V2DScreen(contentManager.Load<V2DContent>("Demo")));
            //screens.Add(new V2DScreen(contentManager.Load<V2DContent>("GearJoint")));
            //screens.Add(new V2DScreen(contentManager.Load<V2DContent>("SmuckLib")));
            //screens.Add(new V2DScreen(contentManager.Load<V2DContent>("Movieclip1")));
            //screens.Add(new V2DScreen(contentManager.Load<V2DContent>("PrismaticJoint")));
            //screens.Add(new V2DScreen(contentManager.Load<V2DContent>("RevoluteJoint")));
            //screens.Add(new V2DScreen(contentManager.Load<V2DContent>("PullyJoint")));
            //screens.Add(new V2DScreen(contentManager.Load<V2DContent>("Scene1Data")));
            //screens.Add(new V2DScreen(contentManager.Load<V2DContent>("Scene2Data")));
            //screens.Add(new V2DScreen(contentManager.Load<V2DContent>("Scene3Data")));
            //screens.Add(new V2DScreen(contentManager.Load<V2DContent>("Scene4Data")));

            //screens.Add(new V2DScreen(new SymbolImport(@"Tests/Movieclip1.xml", "nest1Inst")));
            //screens.Add(new V2DScreen(new SymbolImport(@"Tests/Movieclip1.xml", "nest2Inst")));
        }
    }
}
