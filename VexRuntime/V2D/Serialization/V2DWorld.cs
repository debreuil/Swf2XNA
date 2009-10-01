using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using Box2DX.Common;

namespace DDW.V2D.Serialization
{
    public class V2DWorld
    {
        [XmlAttribute]
        public int Width = 800;
        [XmlAttribute]
        public int Height = 600;

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
                    rootInstance.DefinitionName = V2DGame.ROOT_NAME;
                    rootInstance.Definition = GetDefinitionByName(V2DGame.ROOT_NAME);
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
