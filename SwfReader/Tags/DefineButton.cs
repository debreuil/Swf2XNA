
/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class DefineButton : ISwfTag
	{
        /*
		    Header              RECORDHEADER                Tag type = 7
            ButtonId            UI16                        ID for this character
            Characters          BUTTONRECORD[one or more]   Characters that make up the button
            CharacterEndFlag    UI8                         Must be 0
            Actions             ACTIONRECORD[zero or more]  Actions to perform
            ActionEndFlag       UI8                         Must be 0
         */

        public const TagType tagType = TagType.DefineButton;
		public TagType TagType { get { return tagType; } }

        public uint ButtonId;        
        public List<ButtonRecord> Characters;     
        public uint CharacterEndFlag;
		public ActionRecords ActionRecords;
        public List<IAction> Actions { get { return ActionRecords.Statements; } }


		public DefineButton(SwfReader r)
        {
            ButtonId = r.GetByte();
            while (r.PeekByte() != 0)
            {
                Characters.Add(new ButtonRecord(r, TagType.DefineButton));
            }
            r.GetByte();// 0, end ButtonRecords

            uint start = r.Position;
            ActionRecords = new ActionRecords(r, int.MaxValue);
            ActionRecords.CodeSize = r.Position - start;
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendByte((byte)ButtonId);
            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].ToSwf(w);
            }

            ActionRecords.ToSwf(w);

            w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("DefineButton: ");
			w.WriteLine();
		}
	}
}
