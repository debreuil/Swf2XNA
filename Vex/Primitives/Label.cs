using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.Vex.Primitives
{
    public class Label
    {
        string Name{get; set;}
        uint Frame{get; set;}

        public Label()
        {
        }

        public Label(uint frame, string name)
        {
            Frame = frame;
            Name = name;
        }
    }
}
