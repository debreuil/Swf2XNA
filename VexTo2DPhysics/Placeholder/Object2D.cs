/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace DDW.Placeholder
{
    public abstract class Object2D
    {
        public abstract void Add(string key, string value);

        protected void DumpValue(StringWriter sw, string key, string value)
        {
            if (value.StartsWith("#"))
            {
                sw.Write(key + ":" + value.Substring(1) + "");
            }
            else
            {
                sw.Write(key + ":\"" + value + "\"");
            }
        }
        protected void DumpValue(XmlWriter xw, string key, string value)
        {
            if (key.Length > 0 && char.IsLower(key[0]))
            {
                key = key[0].ToString().ToUpperInvariant() + key.Substring(1);
            }

            if (value.StartsWith("#"))
            {
                xw.WriteAttributeString(key, value.Substring(1));
            }
            else
            {
                xw.WriteAttributeString(key, value);
            }
        }
    }
}
