using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex;

namespace DDW.VexDraw
{
    public class DrawPath
    {
        private List<IShapeData> path;
        public List<IShapeData> Segments { get { return path; } }

        private string svgPath;
        public string SVGString { get { return svgPath; } }

        public DrawPath(List<IShapeData> path)
        {
            this.path = path;
            svgPath = Shape.GetSvgString(path);
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is DrawPath)
            {
                result = (svgPath == ((DrawPath)obj).svgPath);
            }
            return result;
        }

        public override int GetHashCode()
        {
            return svgPath.GetHashCode();
        }
    }
}
