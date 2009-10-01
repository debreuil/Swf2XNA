/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public enum FillType : byte
	{
		Solid				= 0x00, // solid fill
		Linear				= 0x10, // linear gradient fill
		Radial				= 0x12, // radial gradient fill
		Focal				= 0x13, // focal radial gradient fill

		// (SWF 8 file format and later only)

		RepeatingBitmap		= 0x40, // repeating bitmap fill
		ClippedBitmap		= 0x41, // clipped bitmap fill
		NSRepeatingBitmap	= 0x42, // non-smoothed repeating bitmap
		NSClippedBitmap		= 0x43, // non-smoothed clipped bitmap
	}
}
