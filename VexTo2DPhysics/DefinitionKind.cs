using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.VexTo2DPhysics
{
    [Flags]
    public enum DefinitionKind
    {
        Undefined = 0,
        Ignore = 1,
        JointMarker = 2,
        ShapeMarker = 4,
        Vex2D = 8,
        TextField = 16,
        Timeline = 32,
        Symbol = 64,
        OutlineStroke = 128,
    }

}
