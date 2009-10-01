/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using System.Text;
using DDW.Vex;

namespace DDW.Swf
{
	public class SwfCompilationUnit : IVexConvertable
	{
		private SwfReader r;

		private TagType curTag;
		private uint curTagLen;

		public string Name;
		public string FullPath;
		public SwfHeader Header;
		public List<ISwfTag> Tags = new List<ISwfTag>();
		public JPEGTables JpegTable;

		public bool IsValid;
		public List<byte[]> TimelineStream = new List<byte[]>();
		public Dictionary<uint, DefineFont2_3> Fonts = new Dictionary<uint, DefineFont2_3>();
		public StringBuilder Log;

		public SwfCompilationUnit(SwfReader r) : this(r, "Nameless")
		{
		}
		public SwfCompilationUnit(SwfReader r, string name)
		{
			this.Name = name;
			this.FullPath = Directory.GetCurrentDirectory() + "/" + name + ".swf";
			this.r = r;
			Log = new StringBuilder();
			
			this.IsValid = ParseHeader();

			if (IsValid)
			{
				ParseTags();
			}
			this.r = null;
			this.curTagLen = 0;
		}

		private bool ParseHeader()
		{
			this.Header = new SwfHeader(r);
			return this.Header.IsSwf;
		}

		private void ParseTags()
		{
			bool tagsRemain = true;

			while (tagsRemain)
			{
				/*
					RECORDHEADER (short)
					Field				Type	Comment
					TagCodeAndLength	UI16	Upper 10 bits: tag typeLower 6 bits: tag length
				 * 
					RECORDHEADER (long)
					Field				Type	Comment
					TagCodeAndLength	UI16	Tag type and length of 0x3F Packed together as in short header
					Length				SI32	Length of tag
				*/
				uint b = r.GetUI16();
				curTag = (TagType)(b >> 6);
				curTagLen = b & 0x3F;
				if (curTagLen == 0x3F)
				{
					curTagLen = r.GetUI32();
				}
				uint tagEnd = r.Position + curTagLen;
                //Debug.WriteLine(r.Position + " type: " + ((uint)curTag).ToString("X2") + " -- " + Enum.GetName(typeof(TagType), curTag));
					
				switch (curTag)
				{
					case TagType.End:
						Tags.Add(new EndTag(r));
						tagsRemain = false;
						break;

					case TagType.FileAttributes:
						Tags.Add(new FileAttributesTag(r));
						break;

					case TagType.BackgroundColor:
						Tags.Add(new BackgroundColorTag(r));
						break;

					case TagType.DefineShape:
						Tags.Add(new DefineShapeTag(r));
						break;

					case TagType.DefineShape2:
						Tags.Add(new DefineShape2Tag(r));
						break;

					case TagType.DefineShape3:
						Tags.Add(new DefineShape3Tag(r));
						break;

					case TagType.DefineShape4:
						Tags.Add(new DefineShape4Tag(r));
						break;

					case TagType.PlaceObject:
						Tags.Add(new PlaceObjectTag(r, tagEnd));
						break;

					case TagType.PlaceObject2:
						Tags.Add(new PlaceObject2Tag(r, this.Header.Version));
						break;

					case TagType.PlaceObject3:
						Tags.Add(new PlaceObject3Tag(r));
						break;

					case TagType.RemoveObject:
						Tags.Add(new RemoveObjectTag(r));
						break;

					case TagType.RemoveObject2:
						Tags.Add(new RemoveObject2Tag(r));
						break;

					case TagType.ShowFrame:
						Tags.Add(new ShowFrame(r));
						break;

					case TagType.FrameLabel:
						Tags.Add(new FrameLabelTag(r));
						break;

					case TagType.DefineSprite:
						DefineSpriteTag sp = new DefineSpriteTag(r, this.Header.Version);
						Tags.Add(sp);
						break;

					// Bitmaps

					case TagType.JPEGTables:
                        JpegTable = new JPEGTables(r, curTagLen);
                        Tags.Add(JpegTable);
						break;

					case TagType.DefineBits:
						Tags.Add(new DefineBitsTag(r, curTagLen, false, false));
					    break;

					case TagType.DefineBitsJPEG2:
						Tags.Add(new DefineBitsTag(r, curTagLen, true, false));
						break;

					case TagType.DefineBitsJPEG3:
						Tags.Add(new DefineBitsTag(r, curTagLen, true, true));
					    break;

					case TagType.DefineBitsLossless:
						Tags.Add(new DefineBitsLosslessTag(r, curTagLen, false));
					    break;

					case TagType.DefineBitsLossless2:
						Tags.Add(new DefineBitsLosslessTag(r, curTagLen, true));
					    break;

					// Sound

					case TagType.DefineSound:
						Tags.Add(new DefineSoundTag(r, curTagLen));
						break;

					case TagType.StartSound:
						Tags.Add(new StartSoundTag(r));
						break;

					case TagType.SoundStreamHead:
					case TagType.SoundStreamHead2:
					    Tags.Add(new SoundStreamHeadTag(r));
					    break;

					case TagType.SoundStreamBlock:
						SoundStreamBlockTag ssb = new SoundStreamBlockTag(r, curTagLen);
						TimelineStream.Add(ssb.SoundData);
						Tags.Add(ssb);
						break;

                    // text

					case TagType.DefineFontInfo:
						break;
					case TagType.DefineFontInfo2:
						break;
					case TagType.DefineFont:
						break;
					case TagType.DefineFont2:
						DefineFont2_3 df2 = new DefineFont2_3(r, false);
						Tags.Add(df2);
						Fonts.Add(df2.FontId, df2);
						break;
					case TagType.DefineFont3:
						DefineFont2_3 df3 = new DefineFont2_3(r, true);
						Tags.Add(df3);
						Fonts.Add(df3.FontId, df3);
						break;
					case TagType.DefineFontAlignZones:
						DefineFontAlignZonesTag dfaz = new DefineFontAlignZonesTag(r, Fonts);
						Tags.Add(dfaz);
						break;
					case TagType.CSMTextSettings:
						CSMTextSettingsTag csm = new CSMTextSettingsTag(r);
						Tags.Add(csm);
						break;
					case TagType.DefineText:
						DefineTextTag dt = new DefineTextTag(r, false);
						Tags.Add(dt);
						break;
					case TagType.DefineText2:
						DefineTextTag dt2 = new DefineTextTag(r, true);
						Tags.Add(dt2);
                        break;
                    case TagType.DefineEditText:
                        Tags.Add(new DefineEditTextTag(r));
                        break;
                    case TagType.DefineFontName:
                        Tags.Add(new DefineFontName(r));
                        break;

                    // buttons
                    case TagType.DefineButton:
                        Tags.Add(new DefineButton(r));
                        break;
                    case TagType.DefineButton2:
                        Tags.Add(new DefineButton2(r));
                        break;
                    case TagType.DefineButtonCxform:
                        Tags.Add(new DefineButtonCxform(r));
                        break;
                    case TagType.DefineButtonSound:
                        Tags.Add(new DefineButtonSound(r));
                        break;

                    // actions
					case TagType.ExportAssets:
						Tags.Add(new ExportAssetsTag(r));
						break;

					case TagType.DoAction:
						Tags.Add(new DoActionTag(r, curTagLen));
						break;

					case TagType.DoInitAction:
						Tags.Add(new DoActionTag(r, curTagLen, true));
						break;

					// todo: defineMorphShape
					case TagType.DefineMorphShape:
						Tags.Add(new UnsupportedDefinitionTag(r, "Morphs not supported"));
						r.SkipBytes(curTagLen); 
						break;
					// todo: defineVideoStream
					case TagType.DefineVideoStream:
						Tags.Add(new UnsupportedDefinitionTag(r, "Video not supported"));
						r.SkipBytes(curTagLen); 
						break;

                    case TagType.ImportAssets:
                    case TagType.ImportAssets2:
						Tags.Add(new UnsupportedDefinitionTag(r, "Import Assets not yet supported"));
						r.SkipBytes(curTagLen); 
                        break;
                        // todo: ImportAssets tags

					default:
						// skip if unknown
#if(DEBUG)
						Debug.WriteLine("unknown type: " + ((uint)curTag).ToString("X2") + " -- " + Enum.GetName(typeof(TagType), curTag));
						r.SkipBytes(curTagLen); 
						break;
                       // throw new Exception("not defined "  + ((uint)curTag).ToString("X2") + " -- " + Enum.GetName(typeof(TagType), curTag));
#else
						Debug.WriteLine("unknown type: " + ((uint)curTag).ToString("X2") + " -- " + Enum.GetName(typeof(TagType), curTag));
						Log.AppendLine("Unhandled swf tag: " + ((uint)curTag).ToString("X2") + " -- " + Enum.GetName(typeof(TagType), curTag));
						r.SkipBytes(curTagLen); 
						break;
#endif
				}
				if (tagEnd != r.Position)
				{
					Debug.WriteLine("bad tag: " + Enum.GetName(typeof(TagType), curTag));
					Log.AppendLine("Tag not fully parsed: " + Enum.GetName(typeof(TagType), curTag)); 
						
					r.Position = tagEnd;
				}
			}
		}

		public void ToSwf(SwfWriter w)
		{
			// write header
			this.Header.ToSwf(w);
            
			for (int i = 0; i < Tags.Count; i++)
			{
                //if (i == 0x2de)//w.Position >= 0x3cd20)//
                //{
                //    i = i;
                //}
				if (Tags[i] is PlaceObject2Tag)
				{
					bool is6Plus = this.Header.Version > 5;
					((PlaceObject2Tag)Tags[i]).ToSwf(w, is6Plus);
				}
				else
				{
					Tags[i].ToSwf(w);
				}
			}

			if (Header.IsCompressed)
			{
				w.Zip();
			}

			uint len = (uint)w.Position;
			w.Position = 4;
			w.AppendUI32(len);
			w.Position = len;
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("");
			w.WriteLine("*************************************************");

			this.Header.Dump(w);
			foreach (ISwfTag tag in this.Tags)
			{
				tag.Dump(w);
			}

			w.WriteLine("*************************************************");
		}
	}
}
