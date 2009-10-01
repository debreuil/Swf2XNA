/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using DDW.Vex;

namespace DDW.Swf
{
	public interface IShapeRecord : IRecord
	{
		void ToSwf(SwfWriter w, ref uint fillBits, ref uint lineBits, ShapeType shapeType);
	}
}
