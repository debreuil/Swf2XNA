using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.V2D.Serialization;

namespace DDW.V2D
{
    public class SymbolImport
    {
        public string assetName;
        public string instanceName = V2DWorld.ROOT_NAME;

        public SymbolImport(string fileName)
        {
            this.assetName = fileName;
        }
        public SymbolImport(string fileName, string instanceName)
        {
            this.assetName = fileName;
            this.instanceName = instanceName;
        }
    }
}
