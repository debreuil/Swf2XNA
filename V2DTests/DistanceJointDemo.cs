using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D;
using DDW.V2D.Serialization;
using Box2DX.Dynamics;
using DDW.Display;

namespace V2DTest
{
    public class DistanceJointDemo : V2DScreen
    {
        public Sprite bkg;
        public List<V2DSprite> hex;
        public V2DSprite star;
        public V2DSprite star2;
        public V2DSprite car;
        
        public DistanceJointDemo()
        {
            SymbolImport = new SymbolImport(@"Tests/DistanceJoint.xml");
        }
        public DistanceJointDemo(V2DContent v2dContent)  : base(v2dContent)
        {
        }
        public override void Initialize()
        {
            base.Initialize();
            bkg.Play();
            star2.Play();
            //star2.Alpha = .5f;
            star.Play();
            car.GotoAndStop(4);
            //hex[0].Alpha = .5f;
           // hex[1].Alpha = .5f;
        }
    }
}








