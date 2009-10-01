/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public interface IShapeData : IVexObject, IComparable
	{
		SegmentType SegmentType { get;}
		Point StartPoint { get;}
		Point EndPoint { get;}
		void Reverse();
	}
}
