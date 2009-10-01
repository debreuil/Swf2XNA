/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Swf
{
	/*
		00	End				20	SetTarget2		40	NewObject		60	BitAnd			
		04	NextFrame		21	StringAdd		41	DefineLocal2	61	BitOr			
		05	PreviousFrame	22	GetProperty		42	InitArray		62	BitXor			
		06	Play			23	SetProperty		43	InitObject		63	BitLShift		
		07	Stop			24	CloneSprite		44	TypeOf			64	BitRShift		
		08	ToggleQuality	25	RemoveSprite	45	TargetPath		65	BitURShift		
		09	StopSounds		26	Trace			46	Enumerate		66	StrictEquals	
		0A	Add				27	StartDrag		47	Add2			67	Greater			
		0B	Subtract		28	EndDrag			48	Less2			68	StringGreater	
		0C	Multiply		29	StringLess		49	Equals2			69	Extends			
		0D	Divide			2A	Throw			4A	ToNumber		81	GotoFrame		
		0E	Equals			2B	CastOp			4B	ToString		83	GetURL			
		0F	Less			2C	ImplementsOp	4C	PushDuplicate	87	StoreRegister	
		10	And				30	RandomNumber	4D	StackSwap		88	ConstantPool	
		11	Or				31	MBStringLength	4E	GetMember		8A	WaitForFrame	
		12	Not				32	CharToAscii		4F	SetMember		8B	SetTarget		
		13	StringEquals	33	AsciiToChar		50	Increment		8C	GoToLabel		
		14	StringLength	34	GetTime			51	Decrement		8D	WaitForFrame2	
		15	StringExtract	35	MBStringExtract	52	CallMethod		8E	DefineFunction2	
		17	Pop				36	MBCharToAscii	53	NewMethod		8F	Try				
		18	ToInteger		37	MBAsciiToChar	54	InstanceOf		94	With			
		1C	GetVariable		3A	Delete			55	Enumerate2		96	Push			
		1D	SetVariable		3B	Delete2			59	DoInitAction	99	Jump			
							3C	DefineLocal							9A	GetURL2			
							3D	CallFunction						9B	DefineFunction	
							3E	Return								9D	If				
							3F	Modulo								9E	Call			
																	9F	GotoFrame2	
	 */
	public enum ActionKind : byte
	{
		Add				= 0x0A,
		Add2			= 0x47,
		And				= 0x10,
		AsciiToChar		= 0x33,
		BitAnd			= 0x60,
		BitLShift		= 0x63,
		BitOr			= 0x61,
		BitRShift		= 0x64,
		BitURShift		= 0x65,
		BitXor			= 0x62,
		Call			= 0x9E,
		CallFunction	= 0x3D,
		CallMethod		= 0x52,
		CastOp			= 0x2B,
		CharToAscii		= 0x32,
		CloneSprite		= 0x24,
		ConstantPool	= 0x88,
		Decrement		= 0x51,
		DefineFunction	= 0x9B,
		DefineFunction2	= 0x8E,
		DefineLocal		= 0x3C,
		DefineLocal2	= 0x41,
		Delete			= 0x3A,
		Delete2			= 0x3B,
		Divide			= 0x0D,
		DoInitAction	= 0x59,
		End				= 0x00,
		EndDrag			= 0x28,
		Enumerate		= 0x46,
		Enumerate2		= 0x55,
		Equals			= 0x0E,
		Equals2			= 0x49,
		Extends			= 0x69,
		GetMember		= 0x4E,
		GetProperty		= 0x22,
		GetTime			= 0x34,
		GetURL			= 0x83,
		GetURL2			= 0x9A,
		GetVariable		= 0x1C,
		GoToLabel		= 0x8C,
		GotoFrame		= 0x81,
		GotoFrame2		= 0x9F,
		Greater			= 0x67,
		If				= 0x9D,
		ImplementsOp	= 0x2C,
		Increment		= 0x50,
		InitArray		= 0x42,
		InitObject		= 0x43,
		InstanceOf		= 0x54,
		Jump			= 0x99,
		Less			= 0x0F,
		Less2			= 0x48,
		MBAsciiToChar	= 0x37,
		MBCharToAscii	= 0x36,
		MBStringExtract	= 0x35,
		MBStringLength	= 0x31,
		Modulo			= 0x3F,
		Multiply		= 0x0C,
		NewMethod		= 0x53,
		NewObject		= 0x40,
		NextFrame		= 0x04,
		Not				= 0x12,
		Or				= 0x11,
		Play			= 0x06,
		Pop				= 0x17,
		PreviousFrame	= 0x05,
		Push			= 0x96,
		PushDuplicate	= 0x4C,
		RandomNumber	= 0x30,
		RemoveSprite	= 0x25,
		Return			= 0x3E,
		SetMember		= 0x4F,
		SetProperty		= 0x23,
		SetTarget		= 0x8B,
		SetTarget2		= 0x20,
		SetVariable		= 0x1D,
		StackSwap		= 0x4D,
		StartDrag		= 0x27,
		Stop			= 0x07,
		StopSounds		= 0x09,
		StoreRegister	= 0x87,
		StrictEquals	= 0x66,
		StringAdd		= 0x21,
		StringEquals	= 0x13,
		StringExtract	= 0x15,
		StringGreater	= 0x68,
		StringLength	= 0x14,
		StringLess		= 0x29,
		Subtract		= 0x0B,
		TargetPath		= 0x45,
		Throw			= 0x2A,
		ToInteger		= 0x18,
		ToNumber		= 0x4A,
		ToString		= 0x4B,
		ToggleQuality	= 0x08,
		Trace			= 0x26,
		Try				= 0x8F,
		TypeOf			= 0x44,
		WaitForFrame	= 0x8A,
		WaitForFrame2	= 0x8D,
		With			= 0x94,
	}
}