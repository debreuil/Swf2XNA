using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D.Serialization;

namespace DDW.V2D
{
    public class V2DContent
    {
        public V2DWorld v2dWorld;
        public Dictionary<string, Texture2D> textures = new Dictionary<string,Texture2D>();
        //public Dictionary<string, Texture2DContent> contentTextures = new Dictionary<string,Texture2DContent>();
    }
}
