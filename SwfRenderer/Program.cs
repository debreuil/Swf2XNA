using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DDW.Swf;
using System.IO;

namespace DDW.SwfRenderer
{
    static class Program
    {
        public static Form1 form;

        [STAThread]
        static void Main(string[] args)
        {
            string fileName = (args.Length < 1) ? "test.swf" : args[0];
            if (File.Exists(fileName))
            {
                fileName = Path.GetFullPath(fileName);
                Directory.SetCurrentDirectory(Path.GetDirectoryName(fileName));
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);

                string name = Path.GetFileNameWithoutExtension(fileName);
                SwfReader r = new SwfReader(br.ReadBytes((int)fs.Length));
                SwfCompilationUnit scu = new SwfCompilationUnit(r, name);
                if (scu.IsValid)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    form = new Form1();
                    //SwfToVex s2v = new SwfToVex();
                    //VexObject vo = s2v.Convert(scu);
                    SwfRenderer sr = new SwfRenderer();
                    sr.GenerateMappedBitmaps(scu);

                    Application.Run(form);

                }
                else
                {
                    Console.WriteLine("Not a valid swf file: " + fileName);
                    Console.WriteLine("Usage: swfrenderer <filename>");
                }
            }
            else
            {
                Console.WriteLine(fileName + " does not exist.");
                Console.WriteLine("Usage: swfrenderer <filename>");
            }
        }

    }
}
