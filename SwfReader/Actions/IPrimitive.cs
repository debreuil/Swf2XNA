/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Swf
{
	public interface IPrimitive : IAction
    {
        PrimitiveType PrimitiveType { get; }
        int Length { get; }
	}
}
