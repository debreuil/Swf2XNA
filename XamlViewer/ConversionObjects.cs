/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

using DDW.Swf;
using DDW.Vex;

namespace DDW.XamlViewer
{
	public class ConversionObjects
	{
		public SwfCompilationUnit scu;
		public VexObject v;
		public string xamlFileName;
		public string message;

		public ConversionObjects(SwfCompilationUnit scu, VexObject v, string xamlFileName, string message)
		{
			this.scu = scu;
			this.v = v;
			this.xamlFileName = xamlFileName;
			this.message = message;
		}
	}
}
