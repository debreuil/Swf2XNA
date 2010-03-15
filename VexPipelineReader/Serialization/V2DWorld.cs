using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
//using Box2D.XNA;
using Microsoft.Xna.Framework;
//using Box2D.XNA.Collision;
//using Box2D.XNA.Common;

namespace DDW.V2D.Serialization
{
    public class V2DWorld
	{
		public const string ROOT_NAME = "_root";

        [XmlAttribute]
        public int Width = 800;
        [XmlAttribute]
        public int Height = 600;
        [XmlAttribute]
        public int FrameRate = 12;

        [XmlElement("V2DTexture")]
        public List<V2DTexture> textures = new List<V2DTexture>();
        [XmlElement("V2DDefinition")]
        public List<V2DDefinition> definitions = new List<V2DDefinition>();

        private V2DInstance rootInstance;

        public V2DWorld()
        {
        }
        public static V2DWorld CreateFromXml(string xmlPath)
        {
            XmlSerializer s = new XmlSerializer(typeof(V2DWorld));
            TextReader r = new StreamReader(xmlPath);
            V2DWorld w = (V2DWorld)s.Deserialize(r);
            r.Close();

            return w;
        }
        public V2DInstance RootInstance
        {
            get
            {
                if (rootInstance == null)
                {
                    rootInstance = new V2DInstance();
                    rootInstance.DefinitionName = ROOT_NAME;
                    rootInstance.InstanceName = ROOT_NAME;
                }
                return rootInstance;
            }
        }
        public V2DDefinition GetDefinitionByName(string name)
        {
            return definitions.Find(d => d.Name == name);
        }
    }
}
