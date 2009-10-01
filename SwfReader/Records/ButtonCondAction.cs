/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
    public class ButtonCondAction
    {
        /*
            CondActionSize          UI16        Offset in bytes from start of this field to next BUTTONCONDACTION, or 0 if last action
            CondIdleToOverDown      UB[1]       Idle to OverDown
            CondOutDownToIdle       UB[1]       OutDown to Idle
            CondOutDownToOverDown   UB[1]       OutDown to OverDown
            CondOverDownToOutDown   UB[1]       OverDown to OutDown
            CondOverDownToOverUp    UB[1]       OverDown to OverUp
            CondOverUpToOverDown    UB[1]       OverUp to OverDown
            CondOverUpToIdle        UB[1]       OverUp to Idle
            CondIdleToOverUp        UB[1]       Idle to OverUp

            CondKeyPress            UB[7]       SWF 4 or later: key code
                                                Otherwise: always 0
                                                Valid key codes:
                                                1 = left arrow
                                                2 = right arrow
                                                3 = home
                                                4 = end
                                                5 = insert
                                                6 = delete
                                                8 = backspace
                                                13 = enter
                                                14 = up arrow
                                                15 = down arrow
                                                16 = page up
                                                17 = page down
                                                18 = tab
                                                19 = escape
                                                32 to 126: follows ASCII

            CondOverDownToIdle  UB[1]           OverDown to Idle

            Actions             ACTIONRECORD    Actions to perform. See DoAction.
                                [zero or more]
            
            ActionEndFlag       UI8             Must be 0
         */
        public uint CondActionSize;              
        public bool CondIdleToOverDown;  
        public bool CondOutDownToIdle;    
        public bool CondOutDownToOverDown;
        public bool CondOverDownToOutDown;
        public bool CondOverDownToOverUp; 
        public bool CondOverUpToOverDown; 
        public bool CondOverUpToIdle;     
        public bool CondIdleToOverUp; 
        public uint CondKeyPress;
        public bool CondOverDownToIdle;
        public ActionRecords ActionRecords;
        public List<IAction> Actions { get { return ActionRecords.Statements; } }

        public ButtonCondAction(SwfReader r)
		{
            CondActionSize = r.GetUI16();
            CondIdleToOverDown = r.GetBit();
            CondOutDownToIdle = r.GetBit();
            CondOutDownToOverDown = r.GetBit();
            CondOverDownToOutDown = r.GetBit();
            CondOverDownToOverUp = r.GetBit();
            CondOverUpToOverDown = r.GetBit();
            CondOverUpToIdle = r.GetBit();
            CondIdleToOverUp = r.GetBit();
            CondKeyPress = r.GetBits(7);
            CondOverDownToIdle = r.GetBit();

            uint start = r.Position;
            ActionRecords = new ActionRecords(r, int.MaxValue);
            ActionRecords.CodeSize = r.Position - start;
        }
        public void ToSwf(SwfWriter w)
        {
            w.AppendUI16(CondActionSize);
            w.AppendBit(CondIdleToOverDown);
            w.AppendBit(CondOutDownToIdle);
            w.AppendBit(CondOutDownToOverDown);
            w.AppendBit(CondOverDownToOutDown);
            w.AppendBit(CondOverDownToOverUp);
            w.AppendBit(CondOverUpToOverDown);
            w.AppendBit(CondOverUpToIdle);
            w.AppendBit(CondIdleToOverUp);
            w.AppendBits(CondKeyPress, 7);
            w.AppendBit(CondOverDownToIdle);

            ActionRecords.ToSwf(w);
        }
        public void Dump(IndentedTextWriter w)
        {
            w.Write("ButtonCondAction: ");
            w.WriteLine();
        }
    }
}
