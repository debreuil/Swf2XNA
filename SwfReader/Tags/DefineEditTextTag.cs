/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DDW.Swf
{
	public class DefineEditTextTag : ISwfTag
		{
			/*
				Header			RECORDHEADER	Tag type = 37. 
				CharacterID		UI16			ID for this dynamic text character. 
				Bounds			RECT			Rectangle that completely encloses the text field. 
				HasText			UB[1]			0 = text field has no default text. 
			 									1 = text field initially displays the string specified by InitialText.
				WordWrap		UB[1]			0 = text will not wrap and will scroll sideways. 
			 									1 = text will wrap automatically when the end of line is reached. 
				Multiline		UB[1]			0 = text field is one line only. 
			 									1 = text field is multi-line and scrollable. 
				Password		UB[1]			0 = characters are displayed as typed. 
			 									1 = all characters are displayed as an asterisk. 
				ReadOnly		UB[1]			0 = text editing is enabled. 
			 									1 = text editing is disabled. 
				HasTextColor	UB[1]			0 = use default color. 
			 									1 = use specified color (TextColor). 
				HasMaxLength	UB[1]			0 = length of text is unlimited. 
			 									1 = maximum length of string is specified by MaxLength. 
				HasFont			UB[1]			0 = use default font. 
			 									1 = use specified font (FontID) and height (FontHeight). 
				Reserved		UB[1]			Always 0. 
				AutoSize		UB[1]			0 = fixed size. 
			 									1 = sizes to content (SWF 6 or later only). 
				HasLayout		UB[1]			Layout information provided. 
				NoSelect		UB[1]			Enables or disables interactive text selection. 
				Border			UB[1]			Causes a border to be drawn around the text field. 
				Reserved		UB[1]			Always 0. 
				HTML			UB[1]			0 = plaintext content. 
			 									1 = HTML content (see following). 
				UseOutlines		UB[1]			0 = use device font. 
			 									1 = use glyph font. 			  
			  
				FontID			If HasFont UI16			ID of font to use. 
				FontHeight		If HasFont UI16			Height of font in twips. 
				TextColor		If HasTextColor RGBA	Color of text. 
				MaxLength		If HasMaxLength UI16	Text is restricted to this length. 
				Align			If HasLayout UI8		0 = Left 
			 											1 = Right 
			 											2 = Center 
			 											3 = Justify 
				LeftMargin		If HasLayout UI16		Left margin in twips. 
				RightMargin		If HasLayout UI16		Right margin in twips.
				Indent			If HasLayout UI16		Indent in twips. 
				Leading			If HasLayout SI16		Leading in twips (vertical distance between bottom of descender of one line and top of ascender of the next).
				VariableName	STRING					Name of the variable where the contents of the text field are stored. 
			 												May be qualified with dot syntax or slash syntax for non-global variables. 
				InitialText		If HasText STRING		Text that is initially displayed.
			*/

		public const TagType tagType = TagType.DefineEditText;
		public TagType TagType { get { return tagType; } }

		public uint CharacterID;
		public Rect Bounds;

		public bool HasText;
		public bool WordWrap;
		public bool Multiline;
		public bool Password;
		public bool ReadOnly;
		public bool HasTextColor;
		public bool HasMaxLength;
		public bool HasFont;
		//public bool Reserved;
		public bool AutoSize;
		public bool HasLayout;
		public bool NoSelect;
		public bool Border;
		//public bool Reserved;
		public bool HTML;
		public bool UseOutlines;

		public uint FontID;
		public uint FontHeight;
		public RGBA TextColor;
		public uint MaxLength;
		public uint Align;
		public uint LeftMargin;
		public uint RightMargin;
		public uint Indent;
		public int Leading;
		public string VariableName;
		public string InitialText;

		public DefineEditTextTag(SwfReader r)
		{
			CharacterID = r.GetUI16();
			Bounds = new Rect(r);

			HasText = r.GetBit();
			WordWrap = r.GetBit();
			Multiline = r.GetBit();
			Password = r.GetBit();
			ReadOnly = r.GetBit();
			HasTextColor = r.GetBit();
			HasMaxLength = r.GetBit();
			HasFont = r.GetBit();
			r.GetBit();// resreved
			AutoSize = r.GetBit();
			HasLayout = r.GetBit();
			NoSelect = r.GetBit();
			Border = r.GetBit();
			r.GetBit();// resreved
			HTML = r.GetBit();
			UseOutlines = r.GetBit();

			if (HasFont)
			{
				FontID = r.GetUI16();
				FontHeight = r.GetUI16();
			}
			if (HasTextColor)
			{
				TextColor = new RGBA(r.GetByte(), r.GetByte(), r.GetByte(), r.GetByte());
			}
			if (HasMaxLength)
			{
				MaxLength = r.GetUI16();
			}
			if (HasLayout)
			{
				Align = (uint)r.GetByte();
				LeftMargin = r.GetUI16();
				RightMargin = r.GetUI16();
				Indent = r.GetUI16();
				Leading = r.GetInt16();
			}
			VariableName = r.GetString();
			if (HasText)
			{
				InitialText = r.GetString();
			}
		}

		public void ToSwf(SwfWriter w)
        {
            uint start = (uint)w.Position;
            w.AppendTagIDAndLength(this.TagType, 0, true);

            w.AppendUI16(CharacterID);
            Bounds.ToSwf(w);

            w.AppendBit(HasText);
            w.AppendBit(WordWrap);
            w.AppendBit(Multiline);
            w.AppendBit(Password);
            w.AppendBit(ReadOnly);
            w.AppendBit(HasTextColor);
            w.AppendBit(HasMaxLength);
            w.AppendBit(HasFont);
            w.AppendBit(false);// resreved
            w.AppendBit(AutoSize);
            w.AppendBit(HasLayout);
            w.AppendBit(NoSelect);
            w.AppendBit(Border);
            w.AppendBit(false);// resreved
            w.AppendBit(HTML);
            w.AppendBit(UseOutlines);

			if (HasFont)
            {
                w.AppendUI16(FontID);
                w.AppendUI16(FontHeight);
			}
			if (HasTextColor)
            {
                w.AppendByte((byte)TextColor.R);
                w.AppendByte((byte)TextColor.G);
                w.AppendByte((byte)TextColor.B);
                w.AppendByte((byte)TextColor.A);
			}
			if (HasMaxLength)
			{
                w.AppendUI16(MaxLength);
			}
			if (HasLayout)
			{
                w.AppendByte((byte)Align);
                w.AppendUI16(LeftMargin);
                w.AppendUI16(RightMargin);
                w.AppendUI16(Indent);
                w.AppendUI16((uint)Leading);
			}

			w.AppendString(VariableName);

			if (HasText)
			{
                w.AppendString(InitialText);
			}

            w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("DefineEditText: ");
			w.WriteLine();
		}
	}
}
 



 















































 


































