using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Globalization;
using Newtonsoft.Json;
using DDW.Swf;
using DDW.SwfDump;
using DDW.Vex;

namespace DDW.FlaFormat
{
    class Program
    {
        static void Main(string[] args)
        {

            string fileName = "test2";
            //StreamReader sr = new StreamReader(fileName + ".vex", Encoding.ASCII);
            FileStream fs = new FileStream(fileName + ".vex", FileMode.Open, FileAccess.Read);
            BufferedStream bs = new BufferedStream(fs);
            JsonLexer lx = new JsonLexer(bs);
            List<Token> tokens = lx.Lex();
            fs.Close();
            bs.Close();
           // Document doc = JavaScriptConvert.DeserializeObject<Document>(json);
            Debug.WriteLine(tokens);

            //var g = from s in doc.Library.Items
            //        from l in s.Timeline.Layers
            //        select
            //            new
            //            {
            //                Name = s.Timeline.Name,
            //                Frames =
            //                    from f in l.Frames
            //                    where f.StartFrame == f.FrameIndex
            //                    select f
            //            };
            //try
            //{
            //    foreach (var tl in g)
            //    {
            //        foreach (var e in tl.Frames)
            //        {
            //            if (e != null)
            //            {
            //                Debug.WriteLine("tl: " + tl.Name + " \tl:" + e.LayerIndex + " f:" + e.FrameIndex + " \tsf:" + e.StartFrame);
            //            }
            //        }
            //    }
            //}
            //catch(Exception)
            //{
            //}

            //SwfCompilationUnit scu;
            //VexObject v;
            //Debug.WriteLine(Convert(fileName + ".swf", out scu, out v));
            //var tags = from ts in scu.Tags
            //        where ts.TagType == TagType.DefineSprite
            //        select (DefineSpriteTag)ts;

            //foreach (var t in tags)
            //{
            //    Debug.WriteLine(t.SpriteId + " t: " + t.FrameCount);
            //}      

        }

        public static string Convert(string fileName, out SwfCompilationUnit scu, out VexObject v)
        {
            v = null;
            string result = "Failed to convert.";
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            string name = Path.GetFileNameWithoutExtension(fileName);
            SwfReader r = new SwfReader(br.ReadBytes((int)fs.Length));
            scu = new SwfCompilationUnit(r, name);
            if (scu.IsValid)
            {
                result = "\n\n**** Converting to SwfCompilationUnit ****\n";

#if DEBUG
                StringWriter sw = new StringWriter();
                IndentedTextWriter w = new IndentedTextWriter(sw);
                scu.Dump(w);
                Debug.WriteLine(sw.ToString());
#endif

                result += scu.Log.ToString();

                SwfToVex s2v = new SwfToVex();
                v = s2v.Convert(scu);
                result += "\n\n**** Converting to Vex ****\n";
                result += s2v.Log.ToString();
            }
            return result;
        }
    }
}
