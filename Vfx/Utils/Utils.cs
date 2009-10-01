/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace DDW.Vex
{
	public class Utils
	{
		public static void CopyFontToResources(string fontName, string resourceFolder)
		{
			char ds = Path.DirectorySeparatorChar;
			string fontPath = FontUtil.GetInstance().FontMap[fontName];
			string fileName = fontPath.Substring(fontPath.LastIndexOf(ds));
			string targetPath = resourceFolder + ds + fileName;
			Utils.EnsurePath(targetPath);
			if(!File.Exists(targetPath))
			{
				File.Copy(fontPath, targetPath);
			}

		}

		public static void EnsurePath(string path)
		{
			string folder = "";
			if (path.IndexOf(@"\") > -1)
			{
				folder = path.Substring(0, path.LastIndexOf(@"\"));
			}
			else if (path.IndexOf(@"/") > -1)
			{
				folder = path.Substring(0, path.LastIndexOf(@"/"));
			}
			if (folder != "" && !Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
		}
	}
}
