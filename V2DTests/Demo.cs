using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D;
using DDW.Display;
using V2DRuntime.Attributes;
using Box2DX.Dynamics;

namespace V2DTests
{
    public class Demo : V2DScreen
	{
		private Sprite bkg;
		private V2DSprite hex;
		private List<V2DSprite> flo;
		
		[RevoluteJointAttribute(motorSpeed = 50, maxMotorTorque = 500)]
		private RevoluteJoint r2;

		public Demo(SymbolImport si) : base(si)
        {
        }
		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
