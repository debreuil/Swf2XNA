using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using DDW.V2D;

namespace DDW.Display
{
    public class Sprite : DisplayObjectContainer, IRenderable
    {
		//public Sprite(Texture2D texture)
		//{
		//    Texture = texture;
		//}
        public Sprite(Texture2D texture, V2DInstance inst) : base(texture, inst)
		{
        }
    }
}
