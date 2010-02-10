using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DDW.V2D
{
    public class V2DTexture
    {
        public V2DTexture()
        {
        }
        [XmlAttribute("Source")]
        public string Source;
    }
}
