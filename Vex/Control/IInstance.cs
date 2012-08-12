/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DDW.Vex
{
	public interface IInstance : IComparable
	{
        uint DefinitionId { get; set; }
        uint InstanceHash { get; set; }
        int Depth { get; set; }
        string Name { get; set; }
        uint SortOrder { get; }
        uint StartTime { get; set; }
        uint ParentDefinitionId { get; set; }
        Point RotationCenter { get; set; }
        [XmlIgnore]
        bool HasSaveableChanges { get; set; }

        List<Transform> Transformations { get; }
        Transform GetTransformAtTime(uint time);
	}
}
