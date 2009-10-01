using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DDW.V2D
{
    public enum V2DJointKind
    {
        [XmlEnum(Name = "Unknown")]
        Unknown,
        [XmlEnum(Name = "Distance")]
        Distance,
        [XmlEnum(Name = "Revolute")]
        Revolute,
        [XmlEnum(Name = "Prismatic")]
        Prismatic,
        [XmlEnum(Name = "Pully")]
        Pully,
        [XmlEnum(Name = "Mouse")]
        Mouse,
        [XmlEnum(Name = "Gear")]
        Gear,
    }
}
