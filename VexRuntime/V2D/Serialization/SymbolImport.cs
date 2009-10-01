using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.V2D
{
    public class SymbolImport
    {
        public string fileName;
        public string instanceName = V2DGame.ROOT_NAME;

        public SymbolImport(string fileName)
        {
            this.fileName = fileName;
        }
        public SymbolImport(string fileName, string instanceName)
        {
            this.fileName = fileName;
            this.instanceName = instanceName;
        }
    }
}
