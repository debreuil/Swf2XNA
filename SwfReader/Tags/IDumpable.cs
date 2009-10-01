using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW
{
	public interface IDumpable
	{
		void Dump(IndentedTextWriter w);
	}
}
