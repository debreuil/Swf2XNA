/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Swf
{
	public interface IStackManipulator
	{
		uint StackPops { get;}
		uint StackPushes { get;}
		int StackChange { get;}
	}
}
