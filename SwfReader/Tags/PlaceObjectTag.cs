/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	public class PlaceObjectTag : ISwfTag, IControlTag, IPlaceObject
	{
		protected TagType tagType = TagType.PlaceObject;
		public TagType TagType { get { return tagType; } }

		public bool HasCharacter = true;
		public bool HasMatrix = true;
		public bool HasColorTransform = false;

		public uint Character;
		public uint Depth;
		public Matrix Matrix = Matrix.Empty;
		public ColorTransform ColorTransform;

		public PlaceObjectTag()
		{
		}
		public PlaceObjectTag(SwfReader r)
		{
			Character = r.GetUI16();
			Depth = r.GetUI16();
			Matrix = new Matrix(r);
		}
		public PlaceObjectTag(SwfReader r, uint tagEnd)
		{
			Character = r.GetUI16();
			Depth = r.GetUI16();
			Matrix = new Matrix(r);

			if (tagEnd != r.Position)
			{
				HasColorTransform = true;
				ColorTransform = new ColorTransform(r, false);
			}
		}

		public virtual void ToSwf(SwfWriter w)
		{
			uint start = (uint)w.Position;
			w.AppendTagIDAndLength(this.TagType, 0, true);

			w.AppendUI16(this.Character);
			w.AppendUI16(this.Depth);
			Matrix.ToSwf(w);
			if (HasColorTransform)
			{
				ColorTransform.ToSwf(w, false);
			}

			w.ResetLongTagLength(this.TagType, start);
		}

		public virtual void Dump(IndentedTextWriter w)
		{
			w.Write("PlaceObject ");
			w.Write("id:" + Character);
			w.Write(" dp:" + Depth);
			w.Write(" ");
			Matrix.Dump(w);
			if (HasColorTransform)
			{
				//ColorTransform.Dump(w);
			}
			w.WriteLine();
		}
	}
}
