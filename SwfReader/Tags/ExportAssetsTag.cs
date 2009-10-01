/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	public class ExportAssetsTag : ISwfTag
	{
		public const TagType tagType = TagType.ExportAssets;
		public TagType TagType { get { return tagType; } }

		public Dictionary<uint, string> Exports = new Dictionary<uint, string>();

		public ExportAssetsTag(SwfReader r)
		{
			uint count = r.GetUI16();
			for (int i = 0; i < count; i++)
			{
				uint index = r.GetUI16();
				string name = r.GetString();
				Exports.Add(index, name);
			}
		}

		public void ToSwf(SwfWriter w)
		{
			uint start = (uint)w.Position;
			w.AppendTagIDAndLength(this.TagType, 0, true);

			w.AppendUI16((uint)Exports.Count);
			foreach (uint index in Exports.Keys)
			{
				w.AppendUI16(index);
				w.AppendString(Exports[index]);				
			}

			w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("Export Assets: ");
			w.Indent++;
			foreach (uint key in Exports.Keys)
			{
				w.WriteLine(key + " : " + Exports[key]);
			}
			w.Indent--;
		}
	}
}
