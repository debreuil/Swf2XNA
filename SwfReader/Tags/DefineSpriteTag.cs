/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace DDW.Swf
{
	public class DefineSpriteTag : ISwfTag
	{
		/*
			Header			RECORDHEADER		Tag type = 39
			Sprite			ID UI16				Character ID of sprite
			FrameCount		UI16				Number of frames in sprite
			ControlTags		TAG[one or more]	A series of tags
		*/

		public const TagType tagType = TagType.DefineSprite;
		public TagType TagType { get { return tagType; } }

		public UInt16 SpriteId;
		public UInt16 FrameCount;
		public Rect ShapeBounds;
		public List<IControlTag> ControlTags = new List<IControlTag>();

		public List<IPlaceObject> FirstFrameObjects = new List<IPlaceObject>();
		private TagType curTag;
		private uint curTagLen;

		public DefineSpriteTag(SwfReader r, byte swfVersion)
		{
			this.SpriteId = r.GetUI16();
			this.FrameCount = r.GetUI16();
			ParseTags(r, swfVersion);
		}

		private void ParseTags(SwfReader r, byte swfVersion)
		{
			bool tagsRemain = true;

			uint curFrame = 0;
			while (tagsRemain)
			{
				uint b = r.GetUI16();
				curTag = (TagType)(b >> 6);
				curTagLen = b & 0x3F;
				if (curTagLen == 0x3F)
				{
					curTagLen = r.GetUI32();
				}
				uint tagEnd = r.Position + curTagLen;
				Debug.WriteLine("sprite type: " + ((uint)curTag).ToString("X2") + " -- " + Enum.GetName(typeof(TagType), curTag));

				switch (curTag)
				{
					case TagType.End:
						tagsRemain = false;
						ControlTags.Add(new EndTag(r));
						break;

					case TagType.PlaceObject:
						PlaceObjectTag pot = new PlaceObjectTag(r, tagEnd);
						FirstFrameObjects.Add(pot);
						ControlTags.Add(pot);
						break;
					case TagType.PlaceObject2:
						PlaceObject2Tag po2t = new PlaceObject2Tag(r, swfVersion);
						if (po2t.HasCharacter)
						{
							FirstFrameObjects.Add(po2t);
						}
						ControlTags.Add(po2t);
						break;
					case TagType.PlaceObject3:
						PlaceObject3Tag po3t = new PlaceObject3Tag(r);
						if (po3t.HasCharacter)
						{
							FirstFrameObjects.Add(po3t);
						}
						ControlTags.Add(po3t);
						break;

					case TagType.RemoveObject:
						ControlTags.Add(new RemoveObjectTag(r));
						break;

					case TagType.RemoveObject2:
						ControlTags.Add(new RemoveObject2Tag(r));
						break;

					case TagType.ShowFrame:
						ControlTags.Add(new ShowFrame(r));
						curFrame++;
						break;

					case TagType.SoundStreamHead:
					case TagType.SoundStreamHead2:
						ControlTags.Add(new SoundStreamHeadTag(r));
						break;

					case TagType.FrameLabel:
						ControlTags.Add(new FrameLabelTag(r));
						break;

					case TagType.DoAction:
						ControlTags.Add(new DoActionTag(r, curTagLen));
						break;

					case TagType.DoInitAction:
						ControlTags.Add(new DoActionTag(r, curTagLen, true));
						break;

					default:
						// skip if unknown
						Debug.WriteLine("invalid sprite tag: " + ((uint)curTag).ToString("X2") + " -- " + Enum.GetName(typeof(TagType), curTag));
						r.SkipBytes(curTagLen);
						break;
				}
				if (tagEnd != r.Position)
				{
					Console.WriteLine("bad tag in sprite: " + Enum.GetName(typeof(TagType), curTag));
				}
			}
		}

		public void ToSwf(SwfWriter w)
		{
			uint start = (uint)w.Position;
			w.AppendTagIDAndLength(this.TagType, 0, true);

			w.AppendUI16(SpriteId);
			w.AppendUI16(FrameCount);

			for (int i = 0; i < ControlTags.Count; i++)
			{
				ControlTags[i].ToSwf(w);
			}

			// note: Flash always writes this as a long tag.
			w.ResetLongTagLength(this.TagType, start, true);
		}

		public void Dump(IndentedTextWriter w)
		{
			w.WriteLine("DefineSprite id_" + SpriteId + " fc: " + FrameCount);
			w.Indent++;

			foreach (ISwfTag tag in ControlTags)
			{
				tag.Dump(w);
			}

			w.Indent--;
		}
	}
}
