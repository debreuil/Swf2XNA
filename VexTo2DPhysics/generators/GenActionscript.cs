using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DDW.VexTo2DPhysics
{
    public class GenActionscript
    {
        private StringWriter sw = new StringWriter();
        private VexTree v2d;

        string path;
        List<Body2D> allBodies;

        public GenActionscript(VexTree v2d, string path)
        {
            this.v2d = v2d;
            this.path = path;
            Generate();
        }

        public void Generate()
        {
            string className = Path.GetFileNameWithoutExtension(path);
            sw.WriteLine("package Library\n{");
            sw.WriteLine("  import DDW.Vex2D.*;\n");
            sw.WriteLine("  public class " + className + " extends SceneData\n  {");
            
            allBodies = new List<Body2D>();
            GenBodyList(v2d.root);

            WriteEmbeds(sw);
            WriteWorldData(sw);
            WriteAccessors(sw);
            //WriteMapping(sw);
            WriteDefinitions(sw);
            //WriteShapeData(sw);
            WriteJointData(sw, v2d.root); // todo: is now nested
            WriteInstanceData(sw);

            sw.WriteLine("  }\n}");
         
            TextWriter tw2 = new StreamWriter(path);
            tw2.WriteLine(sw.ToString());
            tw2.Close();
        }

        private void GenBodyList(Body2D b)
        {
            if (!allBodies.Contains(b))
            {
                allBodies.Add(b);
                for (int i = 0; i < b.Children.Count; i++)
                {
                    GenBodyList(b.Children[i]);
                }
            }
        }

        private void WriteWorldData(StringWriter sw)
        {
            sw.WriteLine("");
            sw.Write(@"        public var _world:Object = {");

            string comma = "";
            foreach (string s in v2d.worldData.Keys)
            {
                sw.Write(comma + s + ":" + v2d.worldData[s]);
                comma = ", ";
            }

            sw.WriteLine(@"};");
        }
        private void WriteEmbeds(StringWriter sw)
        {
            List<string> parsed = new List<string>();
            foreach (Body2D b in allBodies)
            {
                if (b.Symbol != null && !parsed.Contains(b.Symbol.SymbolName))
                {
                    b.Symbol.Dump(sw);
                    parsed.Add(b.Symbol.SymbolName);
                }
            }
        }
        private void WriteAccessors(StringWriter sw)
        {
            sw.WriteLine("");
            sw.WriteLine(@"		public override function get world():Object{return _world;}");
            sw.WriteLine(@"		public override function get definitions():Array{return _definitions;}");
            //sw.WriteLine(@"		public override function get mapping():Object{return _mapping;}");
            //sw.WriteLine(@"		public override function get data():Object{return _data;}");
            sw.WriteLine(@"		public override function get joints():Array{return _joints;}");
            sw.WriteLine(@"		public override function get instances():Array{return _instances;}");
        }
        private void WriteDefinitions(StringWriter sw)
        {
            sw.WriteLine("");
            sw.WriteLine(@"        public var _definitions:Array = ");
            sw.WriteLine(@"        [");

            List<string> parsed = new List<string>();
            string comma = "";
            foreach (Body2D b in allBodies)
            {
                if (b.Symbol != null && !parsed.Contains(b.Symbol.SymbolName))
                {
                    sw.Write(comma + "            {name:\"" + b.TypeName + "\", linkageName:\"" + b.Symbol.SymbolName + "\", shapes:[");
                    b.DumpShapeData(sw);
                    sw.Write("]}");
                    parsed.Add(b.Symbol.SymbolName);
                    comma = ",\n";
                }
            }

            sw.WriteLine("");
            sw.WriteLine("        ];");
        }
        private void WriteMapping(StringWriter sw)
        {
            sw.WriteLine("");
            sw.Write(@"        public var _mapping:Object = {");

            List<string> parsed = new List<string>();
            string comma = "";
            foreach (Body2D b in allBodies)
            {
                if (b.Symbol != null && !parsed.Contains(b.Symbol.SymbolName))
                {
                    sw.Write(comma + b.TypeName + ":\"" + b.Symbol.SymbolName + "\"");
                    parsed.Add(b.Symbol.SymbolName);
                    comma = ", ";
                }
            }

            sw.WriteLine(@"};");
        }
        private void WriteShapeData(StringWriter sw)
        {
            sw.WriteLine("");
            sw.WriteLine("        public var _data:Object = ");
            sw.WriteLine("        {");

            List<string> parsed = new List<string>();
            string comma = "";
            foreach (Body2D b in allBodies)
            {
                if (b.Symbol != null && !parsed.Contains(b.Symbol.SymbolName))
                {
                    sw.Write(comma);
                    b.DumpShapes(sw);
                    parsed.Add(b.Symbol.SymbolName);
                    comma = "          ,\n";
                }
                else if (b.HasShapes() && !parsed.Contains(b.InstanceName))
                {
                    sw.Write(comma);
                    b.DumpShapes(sw);
                    parsed.Add(b.InstanceName);
                    comma = "          ,\n";
                }
            }

            sw.WriteLine("        };");
        }
        private void WriteJointData(StringWriter sw, Body2D body)
        {
            sw.WriteLine("");
            sw.WriteLine("        public var _joints:Array = ");
            sw.WriteLine("        [");

            Joint2D[] jts = new Joint2D[body.Joints.Count];
            body.Joints.Values.CopyTo(jts, 0);
            List<Joint2D> jList = new List<Joint2D>(jts);
            jList.Sort();

            string comma = "";
            foreach (Joint2D j in jList)
            {
                sw.Write(comma);
                j.Dump(sw);
                comma = ",\n";
            }

            sw.WriteLine("\n        ];");
        }
        private void WriteInstanceData(StringWriter sw)
        {
            sw.WriteLine("");
            sw.WriteLine("        public var _instances:Array = ");
            sw.WriteLine("        [");

            Body2D[] ba = new Body2D[allBodies.Count];
            allBodies.CopyTo(ba, 0);
            List<Body2D> b2d = new List<Body2D>(ba);
            b2d.Sort();

            string comma = "";
            foreach (Body2D b in b2d)
            {
                if (b.InstanceName != V2D.ROOT_NAME)
                {
                    sw.Write(comma);
                    Dictionary<string, string> dict = null;
                    if (v2d.codeData.ContainsKey(b.InstanceName))
                    {
                        dict = v2d.codeData[b.InstanceName];
                    }
                    b.DumpInstance(sw, dict);
                    comma = ",\n";
                }
            }

            sw.WriteLine("\n        ];");
        }

    }
}
