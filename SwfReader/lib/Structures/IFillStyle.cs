/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public interface IFillStyle : IVexConvertable
	{
	/*
		FILLSTYLE
		Field				Type				Comment
		FillStyleType		UI8					Type of fill style:
													0x00 = solid fill
													0x10 = linear gradient fill
													0x12 = radial gradient fill
													0x13 = focal radial gradient fill
													(SWF 8 file format and later only)
													0x40 = repeating bitmap fill
													0x41 = clipped bitmap fill
													0x42 = non-smoothed	repeating bitmap
													0x43 = non-smoothed clipped	bitmap

		Color				If type = 00			Solid fill color with transparency information.
								RGBA (Shape3)
								RGB (Shape1/2)
		
		GradientMatrix		If type = 10,12			Matrix for gradient fill.
								MATRIX
		
		Gradient			If type = 10,12			Gradient fill.
								GRADIENT
							If type = 13
								FOCALGRADIENT
		
		BitmapId			If type = 40,41,42,43	ID of bitmap character for fill.
								UI16

		BitmapMatrix		If type = 40,41,42,43	Matrix for bitmap fill.
								MATRIX
	*/

		FillType FillType { get;}
		void ToSwf(SwfWriter w, bool useAlpha);
	}
}
