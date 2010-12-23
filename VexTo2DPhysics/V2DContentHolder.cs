using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace DDW.VexTo2DPhysics
{
    public class V2DContentHolder
    {
        public V2DWorld v2dWorld;
        //public Dictionary<string, Texture2D> textures = new Dictionary<string,Texture2D>();

        public Dictionary<string, Texture2DContent> contentTextures = new Dictionary<string,Texture2DContent>();
    }
}
