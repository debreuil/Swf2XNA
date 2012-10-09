
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex;

namespace DDW.VexDraw
{
    public class DrawSymbol
    {
        public uint Id;
        public string ExportName;
        public Rectangle StrokeBounds;
        public List<DrawShape> Shapes;
        public List<DrawPath> Paths;
         
        public DrawSymbol(Symbol symbol)
        {
            Id = symbol.Id;
            ExportName = symbol.Name;
            StrokeBounds = symbol.StrokeBounds;

            ParseShapes(symbol.Shapes);
        }

        private void ParseShapes(List<Shape> shapes)
        {
            Shapes = new List<DrawShape>();
            Paths = new List<DrawPath>();
            foreach (Shape s in shapes)
            {
                uint strokeIndex = (s.Stroke == null) ? 0 : (uint)s.Stroke.UserData;
                uint fillIndex = (s.Fill == null) ? 0 : (uint)s.Fill.UserData;
                DrawPath path = new DrawPath(new List<IShapeData>(s.ShapeData)); // clones data
                int pathIndex = Paths.IndexOf(path);
                if (pathIndex == -1)
                {
                    Paths.Add(path);
                    pathIndex = Paths.Count - 1;
                }
                DrawShape ds = new DrawShape(strokeIndex, fillIndex, (uint)pathIndex);
                Shapes.Add(ds);
            }
        }
        
        public void ToJson(StringBuilder sb)
        {
            sb.Append("{\n");
            sb.Append("\"id\":" + Id.ToString() + ",");

            if (ExportName != null)
            {
                sb.Append("\"name\":\"" + ExportName + "\",");
            }

            sb.Append("\"bounds\":[" + StrokeBounds.GetSerializedString() + "],\n");

            sb.Append("\"paths\":\n[");
            string comma = "";
            foreach (DrawPath path in Paths)
            {
                sb.Append(comma);
                sb.Append("\"" + path.SVGString + "\"");
                comma = ",\n";
            }
            sb.Append("\n],\n");

            sb.Append("\"shapes\":\n["); 
            comma = "";
            foreach (DrawShape ds in Shapes)
            {
                sb.Append(comma);
                ds.ToJson(sb);
                comma = ",";
            }
            sb.Append("]");


            sb.Append("\n}");
        }
    }
}
