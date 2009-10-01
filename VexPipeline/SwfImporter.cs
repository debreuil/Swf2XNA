using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using DDW.Swf;
using DDW.VexTo2DPhysics;
using System.IO;
using DDW.V2D;

namespace DDW.VexPipeline
{
    [ContentImporter(".swf", DefaultProcessor = "SwfProcessor", DisplayName = "Swf Importer", CacheImportedData = true)]
    public class SwfImporter : ContentImporter<string>  
    {
        public override string Import(string fileName, ContentImporterContext context)
        {
            string result = fileName; 
            return result;
        }

    }
}
