
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex;

namespace DDW.VexDraw
{
    public class DrawImage
    {
        public uint Id;
        public string ExportName;
        public Rectangle SourceRectangle;
        public string Path;
        public uint PathId;

        public DrawImage(Image image, uint pathId)
        {
            Id = image.Id;
            ExportName = image.Name;
            SourceRectangle = image.StrokeBounds;
            Path = image.Path;
        }

        public void ToJson(StringBuilder sb)
        {
            sb.Append("{");
            sb.Append("\"id\":" + Id.ToString() + ",");

            if (ExportName != null)
            {
                sb.Append("\"name\":\"" + ExportName + "\",");
            }

            sb.Append("\"sourceRectangle\":[" + SourceRectangle.GetSerializedString() + "],");

            sb.Append("\"path\":\"" + Path.ToString() + "\"");
            
            sb.Append("}");
        }
    }
}
