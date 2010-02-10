using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

//using Box2DX.Dynamics;
//using DDW.Display;
using Microsoft.Xna.Framework.Content;

namespace DDW.V2D
{
    public class V2DInstance
    {
        [XmlAttribute]
        public string InstanceName;
        [XmlAttribute]
        public string DefinitionName;
        [XmlAttribute]
        public float Depth;
        [XmlAttribute]
        public float X;
        [XmlAttribute]
        public float Y;
        [XmlAttribute]
        public float Rotation = 0.0f;
        [XmlAttribute]
        public float ScaleX = 1.0f;
        [XmlAttribute]
        public float ScaleY = 1.0f;
        [XmlAttribute]
        public uint StartFrame;
        [XmlAttribute]
        public uint EndFrame;

        [XmlAttribute]
        public float Alpha = 1.0f;
        [XmlAttribute]
        public bool Visible = true;
        [XmlAttribute]
        public float Density = 2.0f;
        [XmlAttribute]
        public float Friction = 0.3f;
        [XmlAttribute]
        public float Restitution = 0.1f;
        [XmlElement]
        public V2DMatrix Matrix;
        [XmlElement]
        public V2DTransform[] Transforms;

        [ContentSerializerIgnore]
        [XmlIgnore]
        public object UserData;

        [ContentSerializerIgnore]
        [XmlIgnore]
        public V2DDefinition Definition;
    }
}
