/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public enum SegmentType : int
    {
        Move,
        Line,
		QuadraticBezier,
		CubicBezier,
	}
}
