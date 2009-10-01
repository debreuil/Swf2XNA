/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Swf
{
	[Flags]
	public enum SwfType
	{
		String,
		Float,
		Null,
		Undefined,
		Register,
		Boolean,
		Double,
		Integer,
		Constant8,
		Constant16,

		Number = Double | Float | Integer,
	}
}
