using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DDW.V2D
{
    public struct V2DVector2
    {
        [XmlAttribute]
        public float X;
        [XmlAttribute]
        public float Y;

        public V2DVector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
