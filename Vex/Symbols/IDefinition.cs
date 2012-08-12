/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DDW.Vex
{
	public interface IDefinition
    {
        uint Id { get; set; }
        Rectangle StrokeBounds { get; set; }
        Point Center { get; }
        string Name { get; set; }
        string Path { get; set; }
        [XmlIgnore]
        int UserData { get; set; }
        [XmlIgnore]
        bool HasSaveableChanges { get; set; }
	}
}
