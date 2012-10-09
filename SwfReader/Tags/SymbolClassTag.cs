using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
    public class SymbolClassTag : ISwfTag
    {
        protected TagType tagType = TagType.SymbolClass;
        public TagType TagType { get { return tagType; } }

        public uint[] Ids;
        public string[] Names;

		public SymbolClassTag()
		{
		}
        public SymbolClassTag(SwfReader r, uint tagEnd)
		{
            int symbolCount = r.GetUI16();
            Ids = new uint[symbolCount];
            Names = new string[symbolCount];

            for (int i = 0; i < symbolCount; i++)
            {
                Ids[i] = r.GetUI16();
                Names[i] = r.GetString(); 
            }

			if (tagEnd != r.Position)
			{
			}
        }

        public virtual void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            uint len = (uint)Ids.Length;
            w.AppendUI16(len);

            for (int i = 0; i < len; i++)
            {
                w.AppendUI16(Ids[i]);
                w.AppendString(Names[i]);
            }

            w.ResetLongTagLength(this.TagType, start, true);
        }

        public virtual void Dump(IndentedTextWriter w)
        {
            w.Write("SymbolClass ");
            w.WriteLine();
            for (int i = 0; i < Ids.Length; i++)
            {
                w.WriteLine("id:" + Ids[i] + " name:" + Names[i]);
            }
            w.WriteLine();
        }

    }
}
