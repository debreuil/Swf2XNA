using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D.Serialization;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.IO;

namespace DDW.VexPipeline
{
    public class V2DReader : ContentTypeReader<V2DContent>
    {
        protected override V2DContent Read(ContentReader input, V2DContent existingInstance)
        {
            if (existingInstance == null)
            {
                existingInstance = new V2DContent();
            }

            //string xml = input.ReadObject<string>();
            //XmlSerializer s = new XmlSerializer(typeof(V2DWorld));
            //existingInstance.v2dWorld = (V2DWorld)s.Deserialize(new StringReader(xml));

            // need two steps as content and output can be two types (eg textures)
            existingInstance.v2dWorld = input.ReadObject<V2DWorld>();
            existingInstance.textures = input.ReadObject<Dictionary<string, Texture2D>>();

            return existingInstance; 
        }

    }
}
