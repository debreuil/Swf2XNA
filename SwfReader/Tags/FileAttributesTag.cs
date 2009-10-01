/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using DDW.Vex;

namespace DDW.Swf
{
	class FileAttributesTag : ISwfTag
	{
		/*
			FileAttributes
			Field			Type			Comment
			Header			RECORDHEADER	Tag type = 69
			Reserved		UB[3]			Must be 0
			HasMetadata		UB[1]			If 1, the SWF file contains the Metadata tag
											If 0, the SWF file does not contain the Metadata tag
			Reserved		UB[3]			Must be 0
			UseNetwork		UB[1]			If 1, this SWF file is given network file access when loaded locally
											If 0, this SWF file is given local file access when loaded locally
			Reserved		UB[24]			Must be 0
		 */

		public const TagType tagType = TagType.FileAttributes;
		public TagType TagType { get { return tagType; } }

		public UInt32 flags;

		public FileAttributesTag(SwfReader r)
		{
			this.flags = r.GetUI32();
		}


		public bool HasMetadata
		{
			get
			{
				if ((this.flags & 0x08) != 0)
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					this.flags = this.flags | 0x08;
				}
				else
				{
					this.flags = this.flags & 0xFFFFFFF7;
				}
			}
		}

		public bool UseNetwork
		{
			get
			{
				if ((this.flags & 0x80) != 0)
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					this.flags = this.flags | 0x80;
				}
				else
				{
					this.flags = this.flags & 0xFFFFFF7F;
				}
			}
		}

		public void ToSwf(SwfWriter w)
		{
			uint len = 4;
			w.AppendTagIDAndLength(this.TagType, len, false);
			w.AppendUI32(this.flags);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.Write("FileAttributes Tag: [");
			if (this.HasMetadata)
			{
				w.Write("hasMetadata ");
			}
			if (this.UseNetwork)
			{
				w.Write("useNetwork");
			}
			else
			{
				w.Write("useLocal");
			}
			w.WriteLine("]");
		}
	}
}
