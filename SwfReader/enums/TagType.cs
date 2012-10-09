/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
namespace DDW.Swf
{
	
	
	
	public enum TagType
	{
		End 				= 0x00,
		ShowFrame 			= 0x01,
		DefineShape		 	= 0x02,
		FreeCharacter 		= 0x03,
		PlaceObject 		= 0x04,
		RemoveObject 		= 0x05,
		DefineBits 			= 0x06,
		DefineButton 		= 0x07,
		JPEGTables 			= 0x08,
		BackgroundColor	    = 0x09,
		DefineFont			= 0x0A,
		DefineText			= 0x0B,
		DoAction			= 0x0C,
		DefineFontInfo		= 0x0D,
		DefineSound			= 0x0E,
		StartSound			= 0x0F,
		DefineButtonSound	= 0x11,
		SoundStreamHead		= 0x12,
		SoundStreamBlock	= 0x13,
		DefineBitsLossless	= 0x14,
		DefineBitsJPEG2		= 0x15,
		DefineShape2		= 0x16,
		DefineButtonCxform	= 0x17,
		Protect				= 0x18,
		PathsArePostScript	= 0x19,

		// **** Flash 3 ****
		PlaceObject2		= 0x1A,
		RemoveObject2		= 0x1C,
		SyncFrame			= 0x1D,
		FreeAll				= 0x1F,
		DefineShape3		= 0x20,
		DefineText2			= 0x21,
		DefineButton2		= 0x22,
		DefineBitsJPEG3		= 0x23,
		DefineBitsLossless2 = 0x24,
		DefineSprite		= 0x27,
		NameCharacter		= 0x28,
		SerialNumber		= 0x29,
		DefineTextFormat	= 0x2A,
		FrameLabel			= 0x2B,
		SoundStreamHead2	= 0x2D,
		DefineMorphShape	= 0x2E,
		FrameTag			= 0x2F,
		DefineFont2			= 0x30,
		GenCommand			= 0x31,
		DefineCommandObj	= 0x32,
		CharacterSet		= 0x33,
		FontRef				= 0x34,
		ExportAssets		= 0x38,
		ImportAssets		= 0x39,
		EnableDebugger		= 0x3A,
		EnableDebugger2		= 0x40,
		ScriptLimits		= 0x41,
		SetTabIndex			= 0x42,

		// **** Flash 4 ****
		DefineEditText		= 0x25,
		DefineVideo			= 0x26,

		// **** Flash 5+ ****
		FileAttributes		= 0x45,
		DefineShape4		= 0x53,

		DoInitAction		= 0x3B,
		DefineVideoStream	= 0x3C,
        DefineFontInfo2     = 0x3E,
        PlaceObject3        = 0x46,
        ImportAssets2       = 0x47,
		DefineFontAlignZones = 0x49,
		CSMTextSettings		= 0x4A,
		DefineFont3			= 0x4B,

		// **** Flash 9 ****
        SymbolClass         = 0x4C,
		DefineFontName		= 0x58,

		// Tags greater than 255
		DefineBitsPtr		= 0x3FF,
		UnsupportedDefinition	= 0xFFF, 
	};
}








