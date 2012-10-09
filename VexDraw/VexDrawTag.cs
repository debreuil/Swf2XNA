using System;

namespace DDW.VexDraw
{

	/// <summary>
    /// VexDrawTag Commands
	/// </summary>
	public enum VexDrawTag
	{
		None                            = 0x00,
        
        Header                          = 0x01,
	
        StrokeList                      = 0x05,
        SolidFillList					= 0x06,
        GradientFillList				= 0x07,
	
        ReplacementSolidFillList		= 0x09,
        ReplacementGradientFillList	    = 0x0A,
        ReplacementStrokeList			= 0x0B,

        SymbolDefinition                = 0x10,
        TimelineDefinition              = 0x11,
        ImageDefinition                 = 0x12,

        DefinitionNameTable             = 0x20,
        InstanceNameTable               = 0x21,
        ColorNameTable                  = 0x22,
        PathNameTable                   = 0x23,

		End                            = 0xFF,
	}
}
