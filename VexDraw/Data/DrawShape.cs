using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex;

using System.IO;

namespace DDW.VexDraw
{
    public class DrawShape
    {
        public uint FillIndex;
        public uint StrokeIndex;
        public uint PathIndex;

        public DrawShape(uint strokeIndex, uint fillIndex, uint pathIndex)
        {
            this.StrokeIndex = strokeIndex;
            this.FillIndex = fillIndex;
            this.PathIndex = pathIndex;
        }

        public void ToJson(StringBuilder sb)
        {
            sb.Append("[");
            sb.Append(StrokeIndex.ToString() + ",");
            sb.Append(FillIndex.ToString() + ",");
            sb.Append(PathIndex.ToString() + "");
            //sb.Append("\"" + Shape.GetSvgString(shapeData) + "\"");
            sb.Append("]");
        }

    }
}
