/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public interface IDefinition
	{
		uint Id { get; set;}
        Rectangle StrokeBounds { get; set; }
        string Name { get; set; }
        string Path { get; set; }
        int UserData { get; set; }
	}
}
