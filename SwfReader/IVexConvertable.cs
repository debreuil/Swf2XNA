/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public interface IVexConvertable
	{
		void Dump(IndentedTextWriter w);
		void ToSwf(SwfWriter w);

		//IVexObject GetVexEquivalent(VexWriter v);
	}
}
