/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public interface ISwfTag : IVexConvertable
	{
		TagType TagType{get;}

		//bool Validate();
	}
}
