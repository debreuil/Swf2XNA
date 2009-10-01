/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	/*
		Field		Type 				Comment
		Header		RECORDHEADER		Tag type = 32.
		ShapeId		UI16				ID for this character.
		ShapeBounds RECT				Bounds of the shape.
		Shapes		SHAPEWITHSTYLE		Shape information (RGBA)
	 */
	public class DefineShape3Tag : ISwfTag
	{		
		public const TagType tagType = TagType.DefineShape3;
		public TagType TagType { get { return tagType; } }

		public UInt16 ShapeId;
		public Rect ShapeBounds;
		public ShapeWithStyle Shapes;

		public DefineShape3Tag(SwfReader r)
		{
			this.ShapeId = r.GetUI16();
			this.ShapeBounds = new Rect(r);
			this.Shapes = new ShapeWithStyle(r, ShapeType.DefineShape3);
			r.Align();
		}


		public void ToSwf(SwfWriter w)
		{
			uint start = (uint)w.Position;
			w.AppendTagIDAndLength(this.TagType, 0, true); // rewrite len after tag 

			w.AppendUI16(this.ShapeId);
			this.ShapeBounds.ToSwf(w);
			this.Shapes.ToSwf(w, ShapeType.DefineShape3);
			w.Align();

			w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("DefineShape3 id_" + ShapeId + ":");
			w.Indent++;

			w.Write("shape bounds:");
			this.ShapeBounds.Dump(w);
			w.WriteLine();
			this.Shapes.Dump(w);

			w.WriteLine();
			w.Indent--;
		}
	}

}
