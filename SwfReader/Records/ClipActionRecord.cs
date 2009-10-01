/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class ClipActionRecord : IVexConvertable
	{
		public ClipEvents ClipEvents;
		public uint ActionRecordSize;
		public byte KeyCode;
		public ActionRecords ActionRecords;
		
		public ClipActionRecord(SwfReader r) : this(r, true)
		{
		}
		public ClipActionRecord(SwfReader r, bool isSwf6Plus)
		{
			uint highClip = r.GetBits(16) << 16;
			uint lowClip = 0;
			bool isEndRecord = false;
			if (highClip == 0)
			{
				if (isSwf6Plus)
				{
					lowClip = r.GetBits(16);
					if (lowClip == 0)
					{
						ClipEvents = (ClipEvents)0;
						ActionRecordSize = 4;
						isEndRecord = true;
					}
				}
				else
				{
					ClipEvents = (ClipEvents)0;
					ActionRecordSize = 2;
					isEndRecord = true;
				}
			}
			else
			{
				lowClip = r.GetBits(16);
			}

			if (!isEndRecord)
			{
				ClipEvents = (ClipEvents)(lowClip | highClip);
				ActionRecordSize = r.GetUI32();
				if ((ClipEvents & ClipEvents.KeyPress) > 0)
				{
					KeyCode = r.GetByte();
				}
				ActionRecords = new ActionRecords(r, ActionRecordSize); // always is init tag?
			}
		}

		public void ToSwf(SwfWriter w)
		{
			ToSwf(w, true);
		}
		public void ToSwf(SwfWriter w, bool isSwf6Plus)
		{
			if ((uint)ClipEvents == 0)
			{
				if (isSwf6Plus)
				{
					w.AppendUI32(0);
				}
				else
				{
					w.AppendUI16(0);
				}
			}
			else
			{
				w.AppendBits((uint)ClipEvents, 32);

				uint start = (uint)w.Position;
				w.AppendUI32(0); // write len after tag written

				if ((ClipEvents & ClipEvents.KeyPress) > 0)
				{
					w.AppendByte(KeyCode);
				}
				ActionRecords.ToSwf(w);

				uint end = (uint)w.Position;
				w.Position = start;
				w.AppendUI32(end - start - 4);
				w.Position = end;
			}
		}
		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("ClipActionRecord: ");
		}
	}
}
