/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public struct ClipActions : IVexConvertable
	{
		public ClipEvents ClipEvents;
		public List<ClipActionRecord> ClipActionRecords;

		public ClipActions(SwfReader r) : this(r, true)
		{
		}
		public ClipActions(SwfReader r, bool isSwf6Plus)
		{
			r.GetUI16(); // reserved
			ClipEvents = (ClipEvents)r.GetBits(32);
			ClipActionRecords = new List<ClipActionRecord>();

			bool hasMoreRecords = true;
			while (hasMoreRecords)
			{
				ClipActionRecord car = new ClipActionRecord(r, isSwf6Plus);
				ClipActionRecords.Add(car);
				if ((uint)car.ClipEvents == 0)
				{
					hasMoreRecords = false;
				}
			}
		}

		public void ToSwf(SwfWriter w)
		{
			ToSwf(w, true);
		}
		public void ToSwf(SwfWriter w, bool isSwf6Plus)
		{
			w.AppendUI16(0); // reserved
			w.AppendBits((uint)ClipEvents, 32);

			for (int i = 0; i < ClipActionRecords.Count; i++)
			{
				ClipActionRecords[i].ToSwf(w, isSwf6Plus);
			}
		}
		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("ClipActions: ");
		}
	}
}
