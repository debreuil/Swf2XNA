/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using DDW.Vex;
using DDW.V2D;

namespace DDW.Placeholder
{
    public abstract class Shape2D
    {
        public string ContainerName;

        public virtual bool IsPointInside(Point p)
        {
            return false;
        }
        public abstract void EnsureClockwise(List<IShapeData> pts);
        public abstract void Dump(StringWriter sw);
        public abstract void Dump(XmlWriter sw);
        public abstract V2DShape GetV2DShape();
    }
}
