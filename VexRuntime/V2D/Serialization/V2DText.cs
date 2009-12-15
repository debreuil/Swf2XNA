using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DDW.V2D
{
	public class V2DText : V2DDefinition
	{
		[XmlElement]
		public List<V2DTextRun> TextRuns;
	}
}
