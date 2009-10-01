/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	[Flags]
	public enum ClipEvents : uint
	{
		KeyUp			= 0x80000000,
		KeyDown			= 0x40000000,
		MouseUp			= 0x20000000,
		MouseDown		= 0x10000000,
		MouseMove		=  0x8000000,
		Unload			=  0x4000000,
		EnterFrame		=  0x2000000,
		Load			=  0x1000000,

		DragOver		=   0x800000,
		RollOut			=   0x400000,
		RollOver		=   0x200000,
		ReleaseOutside	=   0x100000,
		Release			=    0x80000,
		Press			=    0x40000,
		Initialize		=    0x20000,
		Data			=    0x10000,

		//Reserved0		=     0x8000,
		//Reserved1		=     0x4000,
		//Reserved2		=     0x2000,
		//Reserved3		=     0x1000,
		//Reserved4		=      0x800,

		Construct		=      0x400,
		KeyPress		=      0x200,
		DragOut			=      0x100, 
	}
}
