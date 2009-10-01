/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using DDW.Swf;
using DDW.Vex;
using DDW.Xaml;

using Microsoft.Win32;

namespace DDW.SwfToXaml
{
	public class SwfToXaml
	{
		private static ConvertedUI cui;
		private static TrialExpired txp;

		[STAThread]
		static void Main(string[] args)
		{
			if (IsTimedOut())
			{
				txp = new TrialExpired();
				Application.Run(txp);	
			}
			else
			{
				if (args.Length > 0)
				{
					for (int i = 0; i < args.Length; i++)
					{
						Convert(args[i], false);
					}
				}
				else
				{
					cui = new ConvertedUI();
					Application.Run(cui);
				}
			}
		}
		static bool IsTimedOut()
		{
#if(IS_TRIAL)
			bool result = false;
			DateTime installDate = DateTime.MinValue;
			RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\DDW\theConverted");
			if (key == null)
			{
				key = Registry.LocalMachine.CreateSubKey(@"Software\DDW\theConverted");
			}
			if (key.GetValue("year") != null && key.GetValue("month") != null && key.GetValue("day") != null)
			{
				installDate = new DateTime((int)key.GetValue("year"), (int)key.GetValue("month"), (int)key.GetValue("day"));
			}
			else
			{
				installDate = DateTime.Now;
				key.SetValue("year", installDate.Year);
				key.SetValue("month", installDate.Month);
				key.SetValue("day", installDate.Day);
			}
			if (DateTime.Now > installDate + TimeSpan.FromDays(60))
			{
				result = true;
			}
			return result;
#else
			return false;
#endif
		}
		public static string Convert(string fileName, bool isSilverlight)
		{
			SwfCompilationUnit scu;
			VexObject v;
			string xamlFileName;
			return Convert(fileName, isSilverlight, out scu, out v, out xamlFileName);
		}
		public static string Convert(
			string fileName, 
			bool isSilverlight,
			out SwfCompilationUnit scu, 
			out VexObject v, 
			out string xamlFileName)
		{
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

				XamlRenderer xr;
				if (isSilverlight)
				{
					xr = new Silverlight10Renderer();
				}
				else
				{
					xr = new WPFRenderer();
				}
				xr.GenerateXaml(v, out xamlFileName);
				result += "\n\n**** Converting to Xaml ****\n";
				result += xr.Log.ToString();
				result += "\n\nSuccess.";
			}
			else
			{
				result = "Not a valid swf file: " + fileName;
				v = null;
				xamlFileName = "";
			}
			return result;
		}

		public static string Convert(
			bool isSilverlight,
			SwfCompilationUnit scu,
			VexObject v,
			out string xamlFileName)
		{
			string result = "Failed to convert.";

			SwfToVex s2v = new SwfToVex();
			v = s2v.Convert(scu);
			result = "\n\n**** Converting to Vex ****\n";
			result += s2v.Log.ToString();

			XamlRenderer xr;
			if (isSilverlight)
			{
				xr = new Silverlight10Renderer();
			}
			else
			{
				xr = new WPFRenderer();
			}
			xr.GenerateXaml(v, out xamlFileName);
			result += "\n\n**** Converting to Xaml ****\n";
			result += xr.Log.ToString();
			result += "\n\nSuccess.";

			return result;
		}
	}
}
