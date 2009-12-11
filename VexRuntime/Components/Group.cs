using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Display;
using Microsoft.Xna.Framework.Graphics;
using DDW.V2D;

namespace VexRuntime.Components
{
    public class Group<T>:Sprite 
    {
        public List<T> element = new List<T>();

        public Group(Texture2D texture, V2DInstance inst)
            : base(texture, inst)
        {
        }
        //void ProcessItems<T>(IList<T> coll);
    }
}
