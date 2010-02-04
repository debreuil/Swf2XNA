using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace V2DRuntime.V2D
{
    public enum V2DShapeKind
    {
        [XmlEnum(Name = "Circle")]
        Circle,
    }
}
