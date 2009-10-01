using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;

namespace DDW.V2D
{
    public class V2DJoint
    {
        [XmlAttribute]
        public V2DJointKind Type;
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string Body1;
        [XmlAttribute]
        public string Body2;
        [XmlAttribute]
        public float X; // todo: make these vector2s
        [XmlAttribute]
        public float Y;
        [XmlAttribute]
        public float X2;
        [XmlAttribute]
        public float Y2;

        [XmlAttribute]
        public bool CollideConnected = true;
        [XmlAttribute]
        public float DampingRatio;
        [XmlAttribute]
        public float FrequencyHz;
        [XmlAttribute]
        public float Length = -1;
        [XmlAttribute]
        public float Min;
        [XmlAttribute]
        public float Max;
        [XmlAttribute]
        public bool EnableLimit = true;
        [XmlAttribute]
        public float MaxMotorTorque = 10f;
        [XmlAttribute]
        public float MotorSpeed = 0f;
        [XmlAttribute]
        public bool EnableMotor = true;
        [XmlAttribute]
        public float AxisX;
        [XmlAttribute]
        public float AxisY;
        [XmlAttribute]
        public float GroundAnchor1X;
        [XmlAttribute]
        public float GroundAnchor1Y;
        [XmlAttribute]
        public float GroundAnchor2X;
        [XmlAttribute]
        public float GroundAnchor2Y;
        [XmlAttribute]
        public float MaxLength1;
        [XmlAttribute]
        public float MaxLength2;
        [XmlAttribute]
        public float Ratio = 1f;
    }

}
