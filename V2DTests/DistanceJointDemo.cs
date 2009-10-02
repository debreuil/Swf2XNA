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
            //hex[0].Alpha = .5f;
            //bkg.Play();
            //star.Play();
            star2.Play();
            //bkg.Visible = false;
            //star.body.SetMassFromShapes();
        }
    }
}
