/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public class DoActionTag : ISwfTag, IActionContainer, IControlTag
	{
		public const TagType tagType = TagType.DoAction;
		public TagType TagType { get { return tagType; } }

		public ActionRecords ActionRecords;
		public uint CodeSize { get { return ActionRecords.CodeSize; } set { ActionRecords.CodeSize = value; } }
        public List<IAction> Statements { get { return ActionRecords.Statements; } }

        private bool isInitTag = false;
        public bool IsInitTag { get { return isInitTag; } }

		public DoActionTag()
		{
			ActionRecords = new ActionRecords();
		}
		public DoActionTag(SwfReader r, uint tagLen) : this(r, tagLen, false)
		{
		}
		public DoActionTag(SwfReader r, uint tagLen, bool isInitTag)
		{
            this.isInitTag = isInitTag;
			ActionRecords = new ActionRecords(r, tagLen, isInitTag);
		}

		public void ToSwf(SwfWriter w)
        {
            if (!isInitTag)
            {
                w.AppendTagIDAndLength(this.TagType, CodeSize, true);
            }
            else
            {
                w.AppendTagIDAndLength(TagType.DoInitAction, CodeSize, true);
            }
            ActionRecords.ToSwf(w);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("DoActionTag: ");
			ActionRecords.Dump(w);
		}
	}
}
