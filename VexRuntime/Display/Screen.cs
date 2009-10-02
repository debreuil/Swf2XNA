using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DDW.V2D;
using DDW.V2D.Serialization;

namespace DDW.Display
{
    public class Screen : DisplayObjectContainer
    {
        public V2DWorld v2dWorld;
        public float MillisecondsPerFrame = 1000f / 12f;
        public Dictionary<string, Texture2D> textures = new Dictionary<string,Texture2D>();
        private SymbolImport symbolImport;
        
        public Screen()
        {
        }
        public Screen(SymbolImport symbolImport)
        {
            if (symbolImport != null)
            {
                this.SymbolImport = symbolImport;
            }
        }
        public Screen(V2DContent v2dContent)
        {
            this.v2dWorld = v2dContent.v2dWorld;
            this.textures = v2dContent.textures;
        }

        public SymbolImport SymbolImport
        {
            get
            {
                return symbolImport;
            }
            set
            {
                symbolImport = value;
            }
        }
        public Texture2D GetTexture(string linkageName)
        {
            Texture2D result = null;
            if (this.textures.ContainsKey(linkageName))
            {
                result = this.textures[linkageName];
            }
            else
            {
                try
                {
                    result = V2DGame.contentManager.Load<Texture2D>(linkageName);
                }
                catch (Exception) { }
            }

            return result;
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
        }
    }
}
