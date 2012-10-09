/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using System.Drawing;
using DDW.Swf;
using DDW.Vex;
using DDW.VexTo2DPhysics;
using DDW.Gdi;
using Microsoft.Xna.Framework.Content;
using DDW.V2D;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DDW.VexTo2DPhysics
{
    public class V2D
    {
        public static OutputType OUTPUT_TYPE = OutputType.Xna;
        public const string ROOT_NAME = "_root";
        public const string GROUND_NAME = "_ground";

        static void Main(string[] args)
        {
            Console.WriteLine("Swf to Box2D data files");
            Console.WriteLine("Robin Debreuil - 2008");
            Console.WriteLine("");
#if !DEBUG

            if (args.Length < 1)
            {
                Console.WriteLine("");
                Console.WriteLine("Usage: v2d <filename>");
                Console.WriteLine("Usage: v2d <dir> (all files in directory)");
                return;
            }
            string fileName = args[0];
#else
            string fileName = (args.Length < 1) ? Directory.GetCurrentDirectory() : args[0];
#endif
            if (File.Exists(fileName))
            {
                VexTree vt = ProcessFile(fileName);
                vt.WriteToXml();
            }
            else if (Directory.Exists(fileName))
            {
                string[] swfs = Directory.GetFiles(Path.GetFullPath(fileName), "*.swf");
                for (int i = 0; i < swfs.Length; i++)
                {
                    ProcessFile(swfs[i]);
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Error: file " + fileName + "doesn't exist.");
            }
        }

        public static VexTree ProcessSwf(SwfCompilationUnit scu)
        {
            VexTree result = null;
            if (scu.IsValid)
            {
                SwfToVex s2v = new SwfToVex();
                VexObject vo = s2v.Convert(scu);
                result = new VexTree();
                result.Convert(vo, scu);
            }
            return result;
        }

		public static V2DContentHolder SwfToV2DContent(SwfCompilationUnit scu, ContentProcessorContext context)
        {
			V2DContentHolder result = null;
            if (scu.IsValid)
            {
                SwfToVex s2v = new SwfToVex();
                VexObject vo = s2v.Convert(scu);
                VexTree vt = new VexTree();
                vt.Convert(vo, scu);
                vt.WriteToXml();

                result = vt.GetV2DContent(context);
            }
            return result;
        }

		public static V2DContentHolder UilToV2DContent(string path, ContentProcessorContext context)
        {
			V2DContentHolder result = null;

            VexObject vo = LoadFromUIL.Load(path);
            VexTree vt = new VexTree();
            vt.Convert(vo, null);
            vt.WriteToXml();

            result = vt.GetV2DContent(context);

            return result;
        }

        public static VexTree ProcessFile(string fileName)
        {
            VexTree v2tree = null;
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
                    v2tree = ProcessSwf(scu);
//SwfToV2DContent(scu, null);

                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("Not a valid swf file: " + fileName);
                    Console.WriteLine("Usage: v2d <filename>");
                }
            }
            else
            {
                Console.WriteLine(fileName + " does not exist.");
                Console.WriteLine("Usage: v2d <filename>");
            }
            return v2tree;
        }
    }
    
    public enum OutputType
    {
        Swf,
        Xna
    }
}
