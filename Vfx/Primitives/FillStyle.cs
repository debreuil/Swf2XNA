/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public abstract class FillStyle : IVexObject, IComparable
	{
		public abstract FillType FillType{get;}
		public abstract int CompareTo(Object o);
	}
}
