/* Copyright (C) 2008 Robin Debreuil -- Released under the GNU General Public License (GPL) v2 */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using DDW.Swf;
using DDW.Vex;
using DDW.Gdi;
using DDW.Xaml;

namespace DDW.XnaAnimator
{
	class XnaAnimator
	{
		//public static GdiForm gf;

		static void Main(string[] args)
		{
			string fileName = "test14.swf";
			if (args.Length > 0)
			{
				fileName = args[0];
			}

			FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			BinaryReader br = new BinaryReader(fs);

			SwfReader r = new SwfReader(br.ReadBytes((int)fs.Length));
			SwfCompilationUnit scu = new SwfCompilationUnit(r);

			//StringWriter sw = new StringWriter();
			//IndentedTextWriter w = new IndentedTextWriter(sw);
			//scu.Dump(w);
			//Debug.WriteLine(sw.ToString());

			SwfToVex s2v = new SwfToVex();
			VexObject v = s2v.Convert(scu);

			XamlRenderer xr = new WPFRenderer();
			xr.GenerateXaml(v);
			

			//GdiRenderer gr = new GdiRenderer();
			//List<Bitmap> bmps = gr.GenerateBitmaps(v);
			////gr.ExportBitmaps(bmps);

			//gf = new GdiForm(bmps); 
			//Application.EnableVisualStyles();
			//Application.Run(gf);

		}
	}
}
