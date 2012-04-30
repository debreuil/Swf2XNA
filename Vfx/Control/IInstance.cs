/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public interface IInstance : IComparable
	{
        uint DefinitionId { get; set; }
        uint InstanceId { get; set; }
        uint Depth { get; set; }
        string Name { get; set; }
        uint SortOrder { get; }
        uint StartTime { get; set; }
        List<Transform> Transformations { get; }
	}
}
