/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using DDW.Swf;
using DDW.Vex;
using DDW.VexTo2DPhysics;

namespace DDW.SwfDump
{
	public class SwfDump
	{
		static void Main(string[] args)
		{
			Console.WriteLine("SWF DUMP TOOL - swf v8");
			Console.WriteLine("");
#if !DEBUG

			if(args.Length < 1)
			{
				Console.WriteLine("");
				Console.WriteLine("Need some swf file format help? Parser tools? Generators?");
				Console.WriteLine("Just give me a shout : )   robin@debreuil.com");
				Console.WriteLine("");
				Console.WriteLine("");
				Console.WriteLine("Usage: swfdump <filename>");
				return;
			}
#endif

            string fileName = (args.Length < 1) ? "test.swf" : args[0];
			if(File.Exists(fileName))
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
					StringWriter sw = new StringWriter();
					IndentedTextWriter w = new IndentedTextWriter(sw);
//					scu.Dump(w);
//					Console.WriteLine(sw.ToString());
                    
                    SwfToVex s2v = new SwfToVex();
                    VexObject vo = s2v.Convert(scu);

                    DumpTextObject(vo);
                    //VexTo2DPhysics.VexTo2DPhysics v2dp = new VexTo2DPhysics.VexTo2DPhysics();
                    //v2dp.Convert(vo, scu);

					Console.WriteLine("");
					Console.WriteLine("Need some swf file format help? robin@debreuil.com");
					Console.WriteLine("");

					//temp
					MemoryStream ms = new MemoryStream();
					SwfWriter swfWriter = new SwfWriter(ms);
					scu.Header.IsCompressed = false;
					scu.ToSwf(swfWriter);
					byte[] swfBytes = swfWriter.ToArray();
					FileStream fsw = new FileStream("dump.swf", FileMode.Create, FileAccess.Write);
					fsw.Write(swfBytes, 0, swfBytes.Length);
					fsw.Close();
					ms.Close();

					//Console.ReadLine();
				}
				else
				{
					Console.WriteLine("Not a valid swf file: " + fileName);
					Console.WriteLine("Usage: swfdump <filename>");
				}
			}
			else
			{
				Console.WriteLine(fileName + " does not exist.");
				Console.WriteLine("Usage: swfdump <filename>");
			}
		}

        private static void DumpTextObject(VexObject vo)
        {
            foreach(uint key in vo.Definitions.Keys)
            {
                IDefinition def = vo.Definitions[key];
                if (def is Text)
                {
                    string s = "";
                    Text t = (Text)def;
                    for (int i = 0; i < t.TextRuns.Count; i++)
                    {
                        s += t.TextRuns[i].Text;
                    }
                    Console.WriteLine(s);
                }
            }
        }
	}
}
