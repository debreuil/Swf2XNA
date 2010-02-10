using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DDW.V2D
{
	[XmlInclude(typeof(V2DText))]
    public class V2DDefinition
    {
        [XmlAttribute]
        public uint Id;
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string LinkageName;
        [XmlAttribute]
        public float OffsetX;
        [XmlAttribute]
        public float OffsetY;
        [XmlAttribute]
        public uint FrameCount;
        [XmlAttribute]
        public float Duration;
        [XmlAttribute]
        public float Width;
        [XmlAttribute]
        public float Height;

        public List<V2DShape> V2DShapes = new List<V2DShape>();
        public List<V2DInstance> Instances = new List<V2DInstance>();
        public List<V2DJoint> Joints = new List<V2DJoint>();

    }
}
