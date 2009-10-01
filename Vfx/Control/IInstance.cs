/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	public interface IInstance : IComparable
	{
		uint SortOrder { get;}
		uint DefinitionId { get; set; }
		uint StartTime { get; set; }
		uint InstanceID{ get; set; }
	}
}
