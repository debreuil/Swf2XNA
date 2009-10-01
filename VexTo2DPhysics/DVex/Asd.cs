using System;

namespace DDW.DVex
{

	/// <summary>
    /// DVex Commands
	/// </summary>
	public enum DVex
	{
		None				= 0x00,

		LineTo				= 0x10,	
		LineToRelative		= 0x11,	
		CurveTo				= 0x14,	
		CurveToRelative		= 0x15,	
		MoveTo				= 0x18,	
		MoveToRelative		= 0x19,	

		SolidFill			= 0x20,	
		GradientFill		= 0x21,	
		reserved			= 0x22,	
		Stroke				= 0x23,	
		EndFill				= 0x28,	

		BeginFill			= 0x30,	
		BeginStroke			= 0x31,	
		InsertPath			= 0x32,	
		InsertSymbol		= 0x33,	
		InsertControl		= 0x34,	
		AttachMovie			= 0x38,	

		ArgbDefinitions		= 0x40,	
		StrokeDefinitions	= 0x41,	
		PathDefinition		= 0x42,	
		SymbolDefinition	= 0x43,	
		ControlDefinition	= 0x44,	
		RGBColorDefs		= 0x48,
		RGBStrokeDefs		= 0x49,
		RGBAColorDefs		= 0x4A,
		RGBAStrokeDefs		= 0x4B,
		GradientDefs		= 0x4C,

		BeginSprite			= 0x50,	
		EndSprite			= 0x51,	

		FillFilter			= 0x60,	
		StrokeFilter		= 0x61,	
		PathFilter			= 0x62,	
		ColorFilter 		= 0x63,	
		FullFilter 			= 0x64,	
		CallFunction		= 0x68,	
		DefineFunction		= 0x69,	

		Rectangle			= 0xC0,	
		Ellipse				= 0xC1,	
		Polygon				= 0xC2,	
		Arrow				= 0xC3,	
	}
	public enum AsdScope
	{
		Local = 0,
		Global = 1,
		System = 2,
		User = 3
	}
	public enum AsdDefIdType
	{
		Indexed = 0x00,
		Named = 0x01
	}

}
