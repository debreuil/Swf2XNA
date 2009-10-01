/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	public interface IAction: IVexConvertable
	{
        //ActionKind ActionId { get; }
		//uint Length { get; }
        void ToFlashAsm(IndentedTextWriter w); 
	}
}
