using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D;
using DDW.Display;

namespace V2DRuntime.V2D
{
	public interface IJointable
	{
		float X { get; }
		float Y { get; }
		V2DScreen VScreen { get; }
		string DefinitionName { get; }
		DisplayObject GetChildByName(string name);
	}
}
