using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex;
using System.IO;

namespace DDW.VexDraw
{
    public class VexDrawDataGenerator
    {
        public VexDrawDataGenerator()
        {
        }

        public void GenerateBinaryData(VexObject vo, string path, out string filename)
        {
            string dataName = vo.Name + "Data";
            filename = path + dataName + ".dat";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            FileStream stream = new FileStream(filename, FileMode.Create);
            VexDrawWriter writer = new VexDrawWriter(stream);

            DrawObject drawObject = new DrawObject(vo);
            writer.WriteDrawObject(drawObject);
            
            stream.Flush();

            //byte[] bytes = stream.ToArray();
            stream.Close();
        }
        public void GenerateJsonData(VexObject vo, string path, out string filename)
        {
            StringBuilder sb = new StringBuilder();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DrawObject drawObject = new DrawObject(vo);
            drawObject.ToJson(sb);

            string dataName = vo.Name + "Data";
            string prefix = "var " + dataName + " = ";
            string suffix = "\n;\nvar data = " + dataName + ";\n";

            filename = path + dataName + ".js";
            File.WriteAllText(filename, prefix + sb.ToString() + suffix);
        }
    }
}
