using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

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

        /// <summary>
        /// Result in xywh format
        /// </summary>
        /// <returns>Result in xywh format</returns>
        public Vector4 GetShapeRectangle()
        {
            float t = 0;
            float l = 0;
            float b = 0;
            float r = 0;
            foreach (V2DShape sh in V2DShapes)
	        {
                if (sh.IsCircle)
                {
                    if (sh.CenterX - sh.Radius < l) l = sh.CenterX - sh.Radius;
                    if (sh.CenterX + sh.Radius > r) r = sh.CenterX + sh.Radius;
                    if (sh.CenterY - sh.Radius < t) t = sh.CenterY - sh.Radius;
                    if (sh.CenterY + sh.Radius > b) b = sh.CenterY + sh.Radius;
                }
                else
                {
                    for (int i = 0; i < sh.Data.Length; i += 2)
			        {
                        if(sh.Data[i] < l) l = sh.Data[i];
                        if(sh.Data[i] > r) r = sh.Data[i];
			        }
                    for (int i = 1; i < sh.Data.Length; i += 2)
			        {
                        if(sh.Data[i] < t) t = sh.Data[i];
                        if(sh.Data[i] > b) b = sh.Data[i];
			        }
                }
	        }

            return new Vector4(l, t, r - l, b - t);
        }
    }
}
