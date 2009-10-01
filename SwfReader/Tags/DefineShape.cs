/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	/*
		Field		Type 				Comment
		Header		RECORDHEADER		Tag type = 2.
		ShapeId		UI16				ID for this character.
		ShapeBounds RECT				Bounds of the shape.
		Shapes		SHAPEWITHSTYLE		Shape information
	 */
	public class DefineShapeTag : ISwfTag
	{
		public const TagType tagType = TagType.DefineShape;
		public TagType TagType { get { return tagType; } }

		public uint ShapeId;
		public Rect ShapeBounds;
		public ShapeWithStyle Shapes;

		public DefineShapeTag(SwfReader r)
		{
			this.ShapeId = r.GetUI16();
			this.ShapeBounds = new Rect(r);
			this.Shapes = new ShapeWithStyle(r, ShapeType.DefineShape1);
			r.Align();
		}


		public void ToSwf(SwfWriter w)
		{
			uint start = (uint)w.Position;
			w.AppendTagIDAndLength(this.TagType, 0, true); // rewrite len after tag 

			w.AppendUI16(this.ShapeId);
			this.ShapeBounds.ToSwf(w);
			this.Shapes.ToSwf(w, ShapeType.DefineShape1);
			w.Align();

			w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("DefineShape id_" + ShapeId + ":");
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
