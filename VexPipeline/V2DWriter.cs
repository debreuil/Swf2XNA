using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.VexTo2DPhysics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;
using DDW.V2D;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using DDW.V2D.Serialization;
using System.Xml.Serialization;
using System.IO;

namespace DDW.VexPipeline
{
    [ContentTypeWriter]
	public class V2DWriter : ContentTypeWriter<V2DContentHolder>
    {
        protected override void Write(ContentWriter contentWriter, V2DContentHolder v2dContent)
        {
            //XmlSerializer xs = new XmlSerializer(typeof(V2DWorld));
            //StringWriter sw = new StringWriter();
            //xs.Serialize(sw, v2dContent.v2dWorld);
            //contentWriter.WriteObject<string>(sw.ToString());

            // need two steps as content and output can be two types (eg textures)
            contentWriter.WriteObject<V2DWorld>(v2dContent.v2dWorld);
            //contentWriter.WriteObject<Dictionary<string, Texture2DContent>>(v2dContent.contentTextures);
			contentWriter.WriteObject<Dictionary<string, Texture2DContent>>(v2dContent.contentTextures);
        }
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(V2DContent).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(V2DReader).AssemblyQualifiedName;

        }

    }
}
