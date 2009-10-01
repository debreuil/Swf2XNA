/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public interface ILineStyle : IVexConvertable
	{
		void ToSwf(SwfWriter w, bool useAlpha);
	}
}
