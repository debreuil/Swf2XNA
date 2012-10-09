using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex;

namespace DDW.VexDraw
{
    public class DrawTimeline
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public Rectangle StrokeBounds { get; set; }
        public List<IInstance> Instances;

        //public uint Duration;
        //public uint FrameCount = 0;
        //public List<Label> Labels = new List<Label>();

        public DrawTimeline(Timeline tl)
        {
            this.Id = tl.Id;
            this.Name = tl.Name;
            this.StrokeBounds = tl.StrokeBounds;
            this.Instances = tl.Instances;
        }

        public void ToJson(StringBuilder sb)
        {
            sb.Append("{\n");
            sb.Append("\"id\":" + Id.ToString() + ",");

            if (Name != null)
            {
                sb.Append("\"name\":\"" + Name + "\",");
            }

            sb.Append("\"bounds\":[" + StrokeBounds.GetSerializedString() + "],\n");


            sb.Append("\"instances\":[");
            string comma = "";
            for (int i = 0; i < Instances.Count; i++)
            {
                sb.Append(comma);
                InstanceToJson(sb, Instances[i]);
                comma = ",";
            }
            sb.Append("]\n");

            sb.Append("}");
        }

        private void InstanceToJson(StringBuilder sb, IInstance inst)
        {
            // [id,[x,y],[scaleX, scaleY, rotation*, skew*], "name"]
            sb.Append("[");
            sb.Append(inst.DefinitionId + ",");
            sb.Append(inst.InstanceHash + ",");

            Vex.Matrix m = inst.GetTransformAtTime(0).Matrix;
            sb.Append("[" + m.Location.GetSVG() + "]");

            if(m.HasScaleOrRotation())
            {
                Vex.MatrixComponents mc = m.GetMatrixComponents();
                sb.Append(",[" + mc.ScaleX + "," + mc.ScaleY );
                if (mc.Rotation != 0)
                {
                    sb.Append("," + mc.Rotation);
                    if (mc.Shear != 0)
                    {
                    sb.Append("," + mc.Shear);
                    }
                }
                sb.Append("]" );
            }

            if(inst.Name != null && inst.Name != "")
            {
                sb.Append(",\"" + inst.Name + "\"");
            }

            sb.Append("]"); 
        }

    }
}
