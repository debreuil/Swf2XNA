/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Swf
{
	[Flags]
	public enum PreloadFlags
	{
		Empty = 0x00,
		This = 0x01,
		Arguments = 0x2,
		Super = 0x04,
		Root = 0x08,
		Parent = 0x10,
		Global = 0x20,
		End = 0x40,
	}
}
