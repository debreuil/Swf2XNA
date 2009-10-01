/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Text;

namespace DDW.Swf
{
	public struct BitmapFill : IFillStyle
	{
		public uint CharacterId;
		public Matrix Matrix;

		private FillType fillType;
		public FillType FillType { get { return fillType; } }

		public BitmapFill(uint characterId, Matrix matrix, FillType fillType)
		{
			this.CharacterId = characterId;
			this.Matrix = matrix;
			this.fillType = fillType;
		}

		public void ToSwf(SwfWriter w)
		{
			throw new NotSupportedException("Use overload that specifies alpha.");
		}

		public void ToSwf(SwfWriter w, bool useAlpha)
		{
            w.AppendByte((byte)fillType);
            w.AppendUI16(CharacterId);
            Matrix.ToSwf(w);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("Bitmap Fill id_" + this.CharacterId + " type: ");			
			w.Write(Enum.GetName(typeof(FillType), this.FillType));
			w.Write(" ");
			Matrix.Dump(w);
		}
	}
}