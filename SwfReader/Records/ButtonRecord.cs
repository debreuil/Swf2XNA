
/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class ButtonRecord
	{
        /*
            ButtonReserved          UB[2]                    Reserved bits; always 0
            ButtonHasBlendMode      UB[1]                    0 = No blend mode
                                                             1 = Has blend mode (SWF 8 and later only)
            ButtonHasFilterList     UB[1]                    0 = No filter list
                                                             1 = Has filter list (SWF 8 and later only)
            ButtonStateHitTest      UB[1]                    Present in hit test state
            ButtonStateDown         UB[1]                    Present in down state
            ButtonStateOver         UB[1]                    Present in over state
            ButtonStateUp           UB[1]                    Present in up state
            CharacterID             UI16                     ID of character to place
            PlaceDepth              UI16                     Depth at which to place character                                   
            PlaceMatrix             MATRIX                   Transformation matrix for character placement
         
            ColorTransform          If DefineButton2         Character color transform
                                    CXFORMWITHALPHA
         
            FilterList              If DefineButton2 and     List of filters on this button
                                    ButtonHasFilterList = 1,
                                    FILTERLIST
            
            BlendMode               If DefineButton2 and     0 or 1 = normal 
                                    ButtonHasBlendMode = 1,  2 = layer
                                    UI8                      3 = multiply   
                                                             4 = screen   
                                                             5 = lighten   
                                                             6 = darken   
                                                             7 = add   
                                                             8 = subtract   
                                                             9 = difference   
                                                             10 = invert   
                                                             11 = alpha   
                                                             12 = erase   
                                                             13 = overlay   
                                                             14 = hardlight   
                                                             Values 15 to 255 are reserved.
         */

        public bool ButtonHasBlendMode;                    
        public bool ButtonHasFilterList;                   
        public bool ButtonStateHitTest;
        public bool ButtonStateDown;   
        public bool ButtonStateOver;   
        public bool ButtonStateUp;      
        public uint CharacterID;
        public uint PlaceDepth;        
        public Matrix PlaceMatrix;
        public ColorTransform ColorTransform; 
        public List<IFilter> FilterList;   
        public BlendMode BlendMode;

        public TagType containerTag;

        public ButtonRecord(SwfReader r, TagType containerTag)
		{
            this.containerTag = containerTag;

            r.GetBits(2);
            ButtonHasBlendMode = r.GetBit();
            ButtonHasFilterList = r.GetBit();
            ButtonStateHitTest = r.GetBit();
            ButtonStateDown = r.GetBit();
            ButtonStateOver = r.GetBit();
            ButtonStateUp = r.GetBit();

            CharacterID = r.GetUI16();
            PlaceDepth = r.GetUI16();
            PlaceMatrix = new Matrix(r);

            if (containerTag == TagType.DefineButton2)
            {
                ColorTransform = new ColorTransform(r, true);

                if (ButtonHasFilterList)
                {
                    // some dup code from placeObject3 : (
                    uint filterCount = (uint)r.GetByte();
                    FilterList = new List<IFilter>();
                    for (int i = 0; i < filterCount; i++)
                    {
                        FilterKind kind = (FilterKind)r.GetByte();
                        switch (kind)
                        {
                            case FilterKind.Bevel:
                                FilterList.Add(new FilterBevel(r));
                                break;
                            case FilterKind.Blur:
                                FilterList.Add(new FilterBlur(r));
                                break;
                            case FilterKind.ColorMatrix:
                                FilterList.Add(new FilterColorMatrix(r));
                                break;
                            case FilterKind.Convolution:
                                FilterList.Add(new FilterConvolution(r));
                                break;
                            case FilterKind.DropShadow:
                                FilterList.Add(new FilterDropShadow(r));
                                break;
                            case FilterKind.Glow:
                                FilterList.Add(new FilterGlow(r));
                                break;
                            case FilterKind.GradientBevel:
                                FilterList.Add(new FilterGradientBevel(r));
                                break;
                            case FilterKind.GradientGlow:
                                FilterList.Add(new FilterGradientGlow(r));
                                break;

                            default:
                                // unsupported filter
                                break;
                        }
                    }
                }
                if (ButtonHasBlendMode)
                {
                    BlendMode = (BlendMode)r.GetByte();
                }
            }

		}

		public void ToSwf(SwfWriter w)
        {
            w.AppendBits(0, 2);
            w.AppendBit(ButtonHasBlendMode);
            w.AppendBit(ButtonHasFilterList);
            w.AppendBit(ButtonStateHitTest);
            w.AppendBit(ButtonStateDown);
            w.AppendBit(ButtonStateOver);
            w.AppendBit(ButtonStateUp);

            w.AppendUI16(CharacterID);
            w.AppendUI16(PlaceDepth);
            PlaceMatrix.ToSwf(w);

            if (containerTag == TagType.DefineButton2)
            {
                ColorTransform.ToSwf(w, true);

                if (ButtonHasFilterList)
                {
                    w.AppendByte((byte)FilterList.Count);
                    for (int i = 0; i < FilterList.Count; i++)
                    {
                        FilterList[i].ToSwf(w);
                    }
                }
                if (ButtonHasBlendMode)
                {
                    w.AppendByte((byte)BlendMode);
                }
            }

		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("ButtonRecord: ");
			w.WriteLine();
		}
	}
}
