
/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class DefineButton2 : ISwfTag
	{
        /*
            Header              RECORDHEADER    Tag type = 34
            ButtonId            UI16            ID for this character
            ReservedFlags       UB[7]           Always 0
            TrackAsMenu         UB[1]           0 = track as normal button
                                                1 = track as menu button
            ActionOffset        UI16            Offset in bytes from start of this
                                                field to the first
                                                BUTTONCONDACTION, or 0
                                                if no actions occur
            Characters          BUTTONRECORD    Characters that make up the button
                                [one or more]
           
            CharacterEndFlag    UI8                 Must be 0
            Actions             BUTTONCONDACTION    Actions to execute at particular button events
                                [zero or more]
                                
            
         */
        public const TagType tagType = TagType.DefineButton2;
		public TagType TagType { get { return tagType; } }

        public uint ButtonId;      
        public bool TrackAsMenu;  
        public uint ActionOffset;
        public List<ButtonRecord> Characters = new List<ButtonRecord>();
        public List<ButtonCondAction> ButtonCondActions = new List<ButtonCondAction>();

		public DefineButton2(SwfReader r)
        {
            ButtonId = r.GetUI16();
            r.GetBits(7);
            TrackAsMenu = r.GetBit();
            ActionOffset = r.GetUI16();

            while (r.PeekByte() != 0)
            {
                Characters.Add(new ButtonRecord(r, TagType.DefineButton2));
            }
            r.GetByte();// 0, end ButtonRecords

            if (ActionOffset > 0)
            {
                ButtonCondAction bca;
                do
                {
                    bca = new ButtonCondAction(r);
                    ButtonCondActions.Add(bca);
                }
                while (bca.CondActionSize > 0);
            }
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendUI16(ButtonId);
            w.AppendBits(0, 7);
            w.AppendBit(TrackAsMenu);
            w.AppendUI16(ActionOffset); // todo: calc offset

            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].ToSwf(w);
            }
            w.AppendByte(0);

            if(ActionOffset > 0)
            {
                ButtonCondActions[ButtonCondActions.Count - 1].CondActionSize = 0;
                for (int i = 0; i < ButtonCondActions.Count; i++)
                {
                    ButtonCondActions[i].ToSwf(w);
                }
            }

            w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("DefineButton2: ");
			w.WriteLine();
		}
	}
}
