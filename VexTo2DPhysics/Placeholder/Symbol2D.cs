/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace DDW.Placeholder
{
    public class Symbol2D
    {
        public string SymbolName;
        public string Path;

        public Symbol2D(string symbolName, string path)
        {
            this.SymbolName = symbolName;
            this.Path = path;
        }

        public virtual void Dump(StringWriter sw)
        {
            string s0 = @"        [Embed(source=""" + Path + @""", symbol=""" + SymbolName + @""")]";
            string s1 = @"        public var " + SymbolName + ":Class";
            sw.WriteLine(s0);
            sw.WriteLine(s1);
        }
        public virtual void Dump(XmlWriter xw)
        {
            string p = System.IO.Path.GetFileName(Path);
            xw.WriteAttributeString("Source", p);

            if (SymbolName != null && SymbolName != "")
            {
                xw.WriteAttributeString("Symbol", SymbolName);
            }
        }
    }
}
