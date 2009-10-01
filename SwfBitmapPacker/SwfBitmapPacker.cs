using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using DDW.Swf;
using DDW.Vex;

namespace DDW.SwfBitmapPacker
{
    public class SwfBitmapPacker
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SWF BITMAP PACKER - swf v8");
            Console.WriteLine("");

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
                    SwfToVex s2v = new SwfToVex();
                    VexObject vo = s2v.Convert(scu);
                    Dictionary<uint, string> bitmapPaths = s2v.bitmapPaths;
                    Dictionary<uint, System.Drawing.Rectangle> rectResult = new Dictionary<uint, System.Drawing.Rectangle>();
                    UnsafeBitmap fullBitmap = BitmapPacker.PackBitmaps(bitmapPaths, rectResult);
                    foreach (var rect in rectResult)
                    {
                        Console.WriteLine("\t" + rect.Value + ",");                        
                    }
                    fullBitmap.Bitmap.Save("fullBitmap.png");

                    BitmapSwapper bs = new BitmapSwapper();
                    bs.Convert(scu, fullBitmap, rectResult);

                    MemoryStream ms = new MemoryStream();
                    SwfWriter swfWriter = new SwfWriter(ms);
                    scu.ToSwf(swfWriter);
                    byte[] swfBytes = swfWriter.ToArray();
                    FileStream fsw = new FileStream("result.swf", FileMode.Create, FileAccess.Write);
                    fsw.Write(swfBytes, 0, swfBytes.Length);
                    fsw.Close();
                    ms.Close();
                }
                else
                {
                    Console.WriteLine("Not a valid swf file: " + fileName);
                    Console.WriteLine("Usage: SwfBitmapPacker <filename>");
                }
            }
            else
            {
                Console.WriteLine(fileName + " does not exist.");
                Console.WriteLine("Usage: SwfBitmapPacker <filename>");
            }
        }
        private void MapBitmapFills(SwfCompilationUnit scu)
        {
            //scu.Tags.FindAll(tag => tag.TagType == TagType.
        }
    }
}
