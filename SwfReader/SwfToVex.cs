/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
//using System.Drawing;
using DDW.Vex;
using DDW.Vex.Primitives;

namespace DDW.Swf
{
	public class SwfToVex
	{
		public Timeline RootTimeline;
		public StringBuilder Log;
		private SortedDictionary<uint, Instance> rootDepthChart = new SortedDictionary<uint,Instance>();
		private byte[] jpegPrefix = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0, 1, 1, 1, 0, 0x48, 0, 0x48, 0, 0 };
		private byte[] jpegSuffix = new byte[] { 0xFF, 0xD9 };
		public Dictionary<uint, string> bitmapPaths = new Dictionary<uint, string>();
		private Dictionary<uint, string> soundPaths = new Dictionary<uint, string>();

		private VexObject v;
		private SwfCompilationUnit swf;
		private const float twips = 20F;
		private Symbol curSymbol;
		private Timeline curTimeline;
		private SortedDictionary<uint, Instance> curDepthChart;
		private uint curFrame = 0;
		private SoundInstance lastSoundDef;


		public VexObject Convert(SwfCompilationUnit swf)
		{
			this.swf = swf;
			this.v = new VexObject(swf.Name);
			Log = new StringBuilder();

			RootTimeline = new Timeline(v.NextId());
			RootTimeline.Name = "_root";

			this.curTimeline = RootTimeline;
			curTimeline.FrameCount = swf.Header.FrameCount;
			curTimeline.Duration = GetDuration(swf.Header.FrameCount);

			v.Root = curTimeline;
			this.curDepthChart = rootDepthChart;

			this.ParseHeader();
			foreach(ISwfTag tag in swf.Tags)
			{
				this.ParseTag(tag);
			}

			WriteEndTimes();
			return v;
		}

		#region Basic Tags
		private void ParseHeader()
		{
			v.FrameRate = swf.Header.FrameRate;
			v.ViewPort = ParseRect(swf.Header.FrameSize);
		}
		private void ParseFileAttributesTag(FileAttributesTag tag)
		{
		}
		private void ParseBackgroundColor(BackgroundColorTag tag)
		{
			v.BackgroundColor = ParseRGBA(tag.Color);
		}
		private void ParseExportAssets(ExportAssetsTag tag)
		{
			foreach(uint key in tag.Exports.Keys)
			{
				if(v.Definitions.ContainsKey(key))
				{
						v.Definitions[key].Name = tag.Exports[key];
						break;
				}
			}
		}
		#endregion
		#region Primitives
		private DDW.Vex.Matrix ParseMatrix(Matrix tag)
		{
			return new DDW.Vex.Matrix(
				tag.ScaleX,
				tag.Rotate0,
				tag.Rotate1,
				tag.ScaleY,
				tag.TranslateX / twips,
				tag.TranslateY / twips);
		}
		private Color ParseRGBA(RGBA tag)
		{
			return new Color(tag.R, tag.G, tag.B, tag.A);
		}
		private Rectangle ParseRect(Rect tag)
		{
			return new Rectangle(
				tag.XMin / twips,
				tag.YMin / twips,
				tag.XMax / twips - tag.XMin / twips,
				tag.YMax / twips - tag.YMin / twips);
		}
		#endregion
		#region Shapes
        private void ParseDefineShapeTag(DefineShapeTag tag)
        {
            curSymbol = new Symbol(v.NextId());

            curSymbol.StrokeBounds = ParseRect(tag.ShapeBounds);
            curSymbol.Id = tag.ShapeId;
            v.Definitions.Add(curSymbol.Id, curSymbol);

            ParseShapeWithStyle(tag.Shapes);
        }
        private void ParseDefineShape2Tag(DefineShape2Tag tag)
        {
            curSymbol = new Symbol(v.NextId());

            curSymbol.StrokeBounds = ParseRect(tag.ShapeBounds);
            curSymbol.Id = tag.ShapeId;
            v.Definitions.Add(curSymbol.Id, curSymbol);

            ParseShapeWithStyle(tag.Shapes);
        }
		private void ParseDefineShape3Tag(DefineShape3Tag tag)
		{
			curSymbol = new Symbol(v.NextId());

			curSymbol.StrokeBounds = ParseRect(tag.ShapeBounds);
			curSymbol.Id = tag.ShapeId;
			v.Definitions.Add(curSymbol.Id, curSymbol);

			ParseShapeWithStyle(tag.Shapes);
		}
		private void ParseDefineShape4Tag(DefineShape4Tag tag)
		{
			curSymbol = new Symbol(v.NextId());

			curSymbol.Bounds = ParseRect(tag.EdgeBounds);
			curSymbol.StrokeBounds = ParseRect(tag.ShapeBounds);
			curSymbol.Id = tag.ShapeId;
			v.Definitions.Add(curSymbol.Id, curSymbol);

			ParseShapeWithStyle(tag.Shapes);
		}
		private void ParseEndShapeRecord(EndShapeRecord tag)
		{
		}
		private void ParseShapeWithStyle(ShapeWithStyle tag)
		{
			List<FillStyle> curFillStyles = ParseFillStyleArray(tag.FillStyles);
			List<StrokeStyle> curStrokeStyles = ParseLineStyleArray(tag.LineStyles);
			int curX = 0;
			int curY = 0;
			int curFill0 = 0;
			int curFill1 = 0;
			int curStroke = 0;

			List<FillPath> fillPaths = new List<FillPath>();
			List<StrokePath> strokePaths = new List<StrokePath>();

			foreach (IShapeRecord o in tag.ShapeRecords)
			{
				if (o is StraightEdgeRecord)
				{
					StraightEdgeRecord r = (StraightEdgeRecord)o;
					Line line = new Line(
						new Point(curX / twips, curY / twips),
						new Point((curX + r.DeltaX) / twips, (curY + r.DeltaY) / twips));

					if (curFill0 > 0)
					{
						if (curFill0 > curFillStyles.Count)
						{
							fillPaths.Add(new FillPath(new DDW.Vex.SolidFill(new Color(255, 0, 0)), line));
						}
						else
						{
							fillPaths.Add(new FillPath(curFillStyles[curFill0 - 1], line));
						}
					}
					if (curFill1 > 0)
					{
						if (curFill1 > curFillStyles.Count)
						{
							fillPaths.Add(new FillPath(new DDW.Vex.SolidFill(new Color(255, 0, 0)), line));
						}
						else
						{
							fillPaths.Add(new FillPath(curFillStyles[curFill1 - 1], line));
						}
					}
					if (curStroke > 0)
					{
						strokePaths.Add(new StrokePath(curStrokeStyles[curStroke - 1], line));
					}

					curX += r.DeltaX;
					curY += r.DeltaY;
				}
				else if (o is CurvedEdgeRecord)
				{
					CurvedEdgeRecord r = (CurvedEdgeRecord)o;
					Point a0 = new Point(curX / twips, curY / twips);
					Point c0 = new Point((curX + r.ControlX) / twips, (curY + r.ControlY) / twips);
					curX += r.ControlX;
					curY += r.ControlY;
					Point a1 = new Point((curX + r.AnchorX) / twips, (curY + r.AnchorY) / twips);
					curX += r.AnchorX;
					curY += r.AnchorY;
					QuadBezier bez = new QuadBezier(a0, c0, a1);

					if (curFill0 > 0)
					{
						if (curFill1 > curFillStyles.Count)
						{
							fillPaths.Add(new FillPath(new DDW.Vex.SolidFill(new Color(255, 0, 0)), bez));
						}
						else
						{
							fillPaths.Add(new FillPath(curFillStyles[curFill0 - 1], bez));
						}
					}
					if (curFill1 > 0)
					{
						if (curFill1 > curFillStyles.Count)
						{
							fillPaths.Add( new FillPath( new DDW.Vex.SolidFill(new Color(255,0,0)), bez) );
						}
						else
						{
							fillPaths.Add(new FillPath(curFillStyles[curFill1 - 1], bez));
						}
					}
					if (curStroke > 0)
					{
						strokePaths.Add(new StrokePath(curStrokeStyles[curStroke - 1], bez)); 
					}
				}
				else if (o is StyleChangedRecord)
				{
					StyleChangedRecord scr = (StyleChangedRecord)o;
					if (scr.HasMove)
					{
						curX = scr.MoveDeltaX;
						curY = scr.MoveDeltaY;
					}

					if (scr.HasFillStyle0)
					{
						curFill0 = (int)scr.FillStyle0;
					}
					if (scr.HasFillStyle1)
					{
						curFill1 = (int)scr.FillStyle1;
					}
					if (scr.HasLineStyle)
					{

						curStroke = (int)scr.LineStyle;
					}
					if (scr.HasNewStyles)
					{
						ProcessPaths(fillPaths, strokePaths);
						fillPaths.Clear();
						strokePaths.Clear();

						curFillStyles = ParseFillStyleArray(scr.FillStyles);
						curStrokeStyles = ParseLineStyleArray(scr.LineStyles);
					}
				}
				else if (o is EndShapeRecord)
				{
					// end
				}
			}
			ProcessPaths(fillPaths, strokePaths);
		}
		private void ProcessPaths(List<FillPath> fillPaths, List<StrokePath> strokePaths)
		{
			fillPaths.Sort();
			List<DDW.Vex.Shape> fshapes = FillPath.ConvertToShapes(fillPaths);

			strokePaths.Sort();
			List<DDW.Vex.Shape> sshapes = StrokePath.ConvertToShapes(strokePaths);

            fshapes.Sort();
			// order is important here, as strokes go on top
			curSymbol.Shapes.AddRange(fshapes);
			curSymbol.Shapes.AddRange(sshapes);

			//fillPaths.ForEach(delegate(FillPath p) { Debug.WriteLine(p); });
		}
		private void ParseStraightEdgeRecord(StraightEdgeRecord tag)
		{
		}
		private void ParseCurvedEdgeRecord(CurvedEdgeRecord tag)
		{
		}
		private void ParseStyleChangedRecord(StyleChangedRecord tag)
		{
		}
		private List<FillStyle> ParseFillStyleArray(FillStyleArray tag)
		{
			List<FillStyle> ls = new List<FillStyle>();
			foreach (IFillStyle o in tag.FillStyles)
			{
				if (o is SolidFill)
				{
					ls.Add(ParseSolidFill((SolidFill)o));
				}
				else if (o is Gradient)
				{
					ls.Add(ParseGradient((Gradient)o));
				}
				else if (o is BitmapFill)
				{
					ls.Add(ParseBitmapFill((BitmapFill)o));
				}
			}
			return ls;
		}
		private DDW.Vex.SolidFill ParseSolidFill(SolidFill tag)
		{
			return new DDW.Vex.SolidFill(ParseRGBA(tag.Color));
		}
		private DDW.Vex.ImageFill ParseBitmapFill(BitmapFill tag)
		{
			DDW.Vex.ImageFill bf = new DDW.Vex.ImageFill();

			bf.ImagePath = bitmapPaths.ContainsKey(tag.CharacterId) ? bitmapPaths[tag.CharacterId] : "";
			
			Swf.Matrix sm = Utils.ScaleBitmapMatrix(tag.Matrix);
			bf.Matrix = new DDW.Vex.Matrix(
				sm.ScaleX,
				sm.Rotate0,
				sm.Rotate1,
				sm.ScaleY,
				(sm.TranslateX / twips),
				(sm.TranslateY / twips));
			switch (tag.FillType)
			{
				case FillType.ClippedBitmap:
					bf.IsSmooth = true;
					bf.IsTiled = false;
					break;
				case FillType.RepeatingBitmap:
					bf.IsSmooth = true;
					bf.IsTiled = true;
					break;
				case FillType.NSClippedBitmap:
					bf.IsSmooth = false;
					bf.IsTiled = false;
					break;
				case FillType.NSRepeatingBitmap:
					bf.IsSmooth = false;
					bf.IsTiled = true;
					break;
			}
			return bf;
		}
		private GradientFill ParseGradient(Gradient tag)
		{
			GradientFill result = new DDW.Vex.GradientFill();

			switch(tag.FillType)
			{
				case FillType.Linear:
					result.GradientType = GradientType.Linear;
					break;
				case FillType.Radial:
					result.GradientType = GradientType.Radial;
					break;
			}
			result.Transform = ParseMatrix(tag.GradientMatrix);

			// bug in flash, can add two stops at 1.0
			bool hasStart = false;
			bool hasEnd = false;

			int tagCount = tag.Records.Count;
			for (int i = 0; i < tagCount; i++)
			{
				//int index = tagCount - 1 - i;
				GradientRecord r = tag.Records[i];

				float stop = r.Ratio / 255F;
				bool safeToAdd = true;

				if (stop == 0F)
				{
					if (hasStart)
					{
						safeToAdd = false;
					}
					hasStart = true;
				}
				if (stop == 1F)
				{
					if (hasEnd)
					{
						safeToAdd = false;
					}
					hasEnd = true;
				}

				if (safeToAdd)
				{
					result.Fills.Add(ParseRGBA(r.Color));
					result.Stops.Add(stop);
				}
				
			}
			// colors are opposite of swf in Vex (and like gdi) for radial fills
			if (result.GradientType == GradientType.Radial)
			{
				result.Fills.Reverse();
				result.Stops.Reverse();
				for (int i = 0; i < result.Stops.Count; i++)
				{
					result.Stops[i] = 1.0F - result.Stops[i];
				}
			}
			return result;
		}
		private List<StrokeStyle> ParseLineStyleArray(LineStyleArray tag)
		{
			List<StrokeStyle> result = new List<StrokeStyle>();
			foreach (ILineStyle o in tag.LineStyles)
			{
				if (o is LineStyle)
				{
					result.Add(ParseLineStyle((LineStyle)o));
				}
				else if (o is LineStyle2)
				{
					result.Add(ParseLineStyle2((LineStyle2)o));
				}
			}
			return result;
		}
		private StrokeStyle ParseLineStyle(LineStyle tag)
		{
			float w = tag.Width < 10 ? 10 : (int)tag.Width; // deal with hairlines to make them similar to swf
			return new SolidStroke(w / twips, ParseRGBA(tag.Color));
		}
		private StrokeStyle ParseLineStyle2(LineStyle2 tag)
		{
			float w = tag.Width < 10 ? 10 : (int)tag.Width; // deal with hairlines to make them similar to swf
			return new SolidStroke(w / twips, ParseRGBA(tag.Color));
		}
		#endregion
		#region Display List
		private void ParseDefineSpriteTag(DefineSpriteTag tag)
		{
			uint rootFrame = curFrame;
			curFrame = 0;
			curTimeline = new Timeline(v.NextId());
			curTimeline.FrameCount = tag.FrameCount;
            curTimeline.Id = tag.SpriteId;
            curTimeline.Duration = GetDuration(tag.FrameCount);

			v.Definitions.Add(curTimeline.Id, curTimeline);
			curDepthChart = new SortedDictionary<uint, Instance>();

			foreach (ISwfTag t in tag.ControlTags)
			{
				this.ParseTag(t);
			}
			WriteEndTimes();

			curTimeline.StrokeBounds = ParseSpriteBounds(tag);

			curTimeline = RootTimeline;
			curDepthChart = rootDepthChart;
			curFrame = rootFrame;
		}
		private void WriteEndTimes()
		{
			SortedDictionary<uint, Instance>.ValueCollection insts = this.curDepthChart.Values;
			foreach (Instance inst in insts)
			{
				inst.EndTime = curTimeline.Duration;
			}
		}


		private void ParsePlaceObjectTag(PlaceObjectTag tag)
		{
			uint curTime = (uint)((curFrame * (1 / swf.Header.FrameRate)) * 1000);
            uint totalTime = GetDuration(this.curTimeline.FrameCount);

			Vex.Matrix mx = ParseMatrix(tag.Matrix);
			float alpha = 1;

			Instance inst = new Instance();
			inst.DefinitionId = tag.Character;
			inst.StartTime = curTime;
			inst.EndTime = totalTime;
            inst.Depth = (int)tag.Depth;

            // error from flashDevelop files
            if (curDepthChart.ContainsKey(tag.Depth))
            {
                curDepthChart.Remove(tag.Depth);
            }
			curDepthChart.Add(tag.Depth, inst);
			this.curTimeline.AddInstance(inst);

			if (tag.HasColorTransform && (tag.ColorTransform.HasAddTerms || tag.ColorTransform.HasMultTerms))
			{
				int addMult = tag.ColorTransform.AMultTerm + tag.ColorTransform.AAddTerm;
				alpha = addMult < 0 ? 0 : addMult / 256F;
			}
            ColorTransform c = tag.ColorTransform;
            Vex.ColorTransform ct = new Vex.ColorTransform(c.RAddTerm, c.RMultTerm, c.GAddTerm, c.GMultTerm, c.BAddTerm, c.BMultTerm, c.AAddTerm, c.AMultTerm);

			inst.Transformations.Add(new Transform(curTime, totalTime, mx, alpha, ct));
		}

		private void ParsePlaceObject2Tag(PlaceObject2Tag tag)
		{
            uint curTime = (uint)((curFrame * (1 / swf.Header.FrameRate)) * 1000);
            uint totalTime = GetDuration(this.curTimeline.FrameCount); 

			Vex.Matrix mx = ParseMatrix(tag.Matrix);
			float alpha = 1;
			Instance inst;
			if (tag.HasCharacter && !tag.Move) // a new symbol at this depth, none preexisting
			{
				inst = new Instance();
				inst.DefinitionId = tag.Character;
				inst.StartTime = curTime;
				inst.EndTime = totalTime;
				inst.Depth = (int)tag.Depth;
				curDepthChart.Add(tag.Depth, inst);
				this.curTimeline.AddInstance(inst);
			}
			else if (!tag.HasCharacter && tag.Move) // an old symbol is modified
			{
				inst = curDepthChart[tag.Depth];
				Transform lastT = inst.Transformations[inst.Transformations.Count - 1];
				lastT.EndTime = curTime;
				inst.EndTime = curTime;
				alpha = lastT.Alpha;
			}
			else if (tag.HasCharacter && tag.Move) // a new symbol replaces an old one
			{
				Instance old = curDepthChart[tag.Depth];
				Transform lastT = old.Transformations[old.Transformations.Count - 1];
				lastT.EndTime = curTime;
				old.EndTime = curTime;

				curDepthChart.Remove(tag.Depth);

				inst = new Instance();
				// note: when replacing old symbol, the previous matrix is used
				if (!tag.HasMatrix)
				{
					mx = lastT.Matrix;
				}
				// as is old color xform
				if (!tag.HasColorTransform)
				{
					alpha = lastT.Alpha; // include rest of xform as it comes
				}
				inst.DefinitionId = tag.Character;
				inst.StartTime = curTime;
				inst.EndTime = totalTime;
                inst.Depth = (int)tag.Depth;

				curDepthChart.Add(tag.Depth, inst);
                this.curTimeline.AddInstance(inst);
			}
			else
			{
				throw new ArgumentException("swf error, no character to modify");
			}
			if (tag.HasColorTransform && (tag.ColorTransform.HasAddTerms || tag.ColorTransform.HasMultTerms))
			{
				int addMult = tag.ColorTransform.AMultTerm + tag.ColorTransform.AAddTerm;
				alpha = addMult < 0 ? 0 : addMult / 256F;
			}

			if (tag.HasClipDepth)
			{
				inst.IsMask = true;
				inst.MaskDepth = tag.ClipDepth;
			}
			if (tag.HasName)
			{
				inst.Name = tag.Name;
            }

            ColorTransform c = tag.ColorTransform;
            Vex.ColorTransform ct = new Vex.ColorTransform(c.RAddTerm, c.RMultTerm, c.GAddTerm, c.GMultTerm, c.BAddTerm, c.BMultTerm, c.AAddTerm, c.AMultTerm);
			inst.Transformations.Add(new Transform(curTime, totalTime, mx, alpha, ct));
		}
		private void ParsePlaceObject3Tag(PlaceObject3Tag tag)
		{ 
			// the new features of PO3 aren't used
			ParsePlaceObject2Tag(tag);
		}
		private void ParseRemoveObjectTag(RemoveObjectTag tag)
		{
			uint curTime = (uint)((curFrame * (1 / swf.Header.FrameRate)) * 1000);
			Instance inst = curDepthChart[tag.Depth];
			Transform lastT = inst.Transformations[inst.Transformations.Count - 1];
			lastT.EndTime = curTime;
			inst.EndTime = curTime;

			curDepthChart.Remove(tag.Depth);
		}

		private void ParseRemoveObject2Tag(RemoveObject2Tag tag)
		{
			uint curTime = (uint)((curFrame * (1 / swf.Header.FrameRate)) * 1000);
			Instance inst = curDepthChart[tag.Depth];
			Transform lastT = inst.Transformations[inst.Transformations.Count - 1];
			lastT.EndTime = curTime;
			inst.EndTime = curTime;

			curDepthChart.Remove(tag.Depth);
		}
		#endregion
		#region Bitmaps and Sound

		private void ParseDefineBits(DefineBitsTag tag)
		{
			DDW.Vex.ImageFill bf = new DDW.Vex.ImageFill();
			string path = v.ResourceFolder + @"/" + VexObject.BitmapPrefix + tag.CharacterId + ".jpg";
			WriteJpegToDisk(path, tag);
			Size sz = Utils.GetJpgSize(path);

			if (tag.HasAlphaData) // this is an alpha jpg, convert to png
			{
				byte[] alphaData = SwfReader.Decompress(tag.CompressedAlphaData, (uint)(sz.Width * sz.Height));

				string bmpPath = path;
				System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(bmpPath, false);
				path = v.ResourceFolder + @"/" + VexObject.BitmapPrefix + tag.CharacterId + ".png";

				Utils.WriteAlphaJpg(alphaData, bmp, path);

				bmp.Dispose();
				File.Delete(bmpPath);
			}

			Image img = new Vex.Image(path, v.NextId());
			img.Id = tag.CharacterId;
			img.StrokeBounds = new Rectangle(0, 0, sz.Width, sz.Height);

			bitmapPaths.Add(img.Id, path);
			v.Definitions.Add(img.Id, img);
		}
		private void ParseDefineBitsLossless(DefineBitsLosslessTag tag)
		{
			DDW.Vex.ImageFill bf = new DDW.Vex.ImageFill();
			string path = v.ResourceFolder + @"/" + VexObject.BitmapPrefix + tag.CharacterId + ".png";

			WriteLosslessBitmapToDisk(path, tag);

			Image img = new Vex.Image(path, v.NextId());
			img.Id = tag.CharacterId;
			img.StrokeBounds = new Rectangle(0, 0, tag.Width, tag.Height);

			bitmapPaths.Add(img.Id, path);
			v.Definitions.Add(img.Id, img);
		}

		
		private void ParseSoundStreamHead(SoundStreamHeadTag tag)
		{
			if (tag.StreamSoundCompression == SoundCompressionType.Nellymoser)
			{
				Debug.WriteLine("Nellymoser Sound not supported");
				Log.AppendLine(
					"Nellymoser Sound not supported" +
					" at frame " + this.curFrame +
					" in clip " + this.curTimeline.Name);
				return;
			}
			else if (tag.StreamSoundCompression == SoundCompressionType.ADPMC)
			{
				Debug.WriteLine("ADPMC Sound not supported");
				Log.AppendLine(
					"ADPMC Sound not supported" +
					" at frame " + this.curFrame +
					" in clip " + this.curTimeline.Name);
				return;
			}

			// some swfs have soundStreamHeader, but no blocks
			if (swf.TimelineStream.Count > 0)
			{
				string ext = SoundStreamHeadTag.SoundExtentions[(int)tag.StreamSoundCompression];
				string path = v.ResourceFolder + @"/" + VexObject.SoundPrefix + tag.SoundId + ext;

				WriteSoundToDisk(path, swf.TimelineStream);

				uint curTime = (uint)((curFrame * (1 / swf.Header.FrameRate)) * 1000);
				SoundInstance snd = new SoundInstance(path, v.NextId());
				snd.StartTime = curTime;
				lastSoundDef = snd;

                this.curTimeline.AddInstance(snd);
			}

		}
		private void ParseDefineSound(DefineSoundTag tag)
		{
			if (tag.SoundFormat == SoundCompressionType.Nellymoser)
			{
				Debug.WriteLine("Nellymoser Sound not supported");
				Log.AppendLine(
					"Nellymoser Sound not supported" +
					" at frame " + this.curFrame +
					" in clip " + this.curTimeline.Name);
				soundPaths[tag.SoundId] = null;
				return;
			}
			else if (tag.SoundFormat == SoundCompressionType.ADPMC)
			{
				Debug.WriteLine("ADPMC Sound not supported");
				Log.AppendLine(
					"ADPMC Sound not supported" +
					" at frame " + this.curFrame +
					" in clip " + this.curTimeline.Name);
				soundPaths[tag.SoundId] = null;
				return;
			}
			string ext = SoundStreamHeadTag.SoundExtentions[(int)tag.SoundFormat];
			string path = v.ResourceFolder + @"/" + VexObject.SoundPrefix + tag.SoundId + ext;
			soundPaths[tag.SoundId] = path;

			WriteSoundToDisk(path, tag);

		}
		private void ParseStartSound(StartSoundTag tag)
		{
			//v.Definitions.Add(null);
			string path = soundPaths[tag.SoundId];
			if (path != null)
			{
				uint curTime = (uint)((curFrame * (1 / swf.Header.FrameRate)) * 1000);
				SoundInstance snd = new SoundInstance(path, v.NextId());
				snd.StartTime = curTime;
                this.curTimeline.AddInstance(snd);
			}
			else
			{
                this.curTimeline.AddInstance(null);
			}
		}
		#endregion

		#region Text
		
		private void ParseDefineText(DefineTextTag tag)
		{
			Text t = new Text(v.NextId());
            
			t.StrokeBounds = ParseRect(tag.TextBounds);
			t.Matrix = ParseMatrix(tag.TextMatrix);
			GetTextFromRecords(tag.TextRecords, t.TextRuns);
			t.Id = tag.CharacterId;

			v.Definitions.Add(t.Id, t);
		}

		private void ParseEditText(DefineEditTextTag tag)
		{
			Text t = new Text(v.NextId());

			t.Id = tag.CharacterID;
			t.StrokeBounds = ParseRect(tag.Bounds);
			t.Matrix = Vex.Matrix.Identity;

			ConvertStringToTextRuns(tag, t.TextRuns);

			v.Definitions.Add(t.Id, t);
		}
		#endregion
		#region Utils

		private void ConvertStringToTextRuns(DefineEditTextTag tag, List<TextRun> runs)
		{
			string s = (tag.InitialText == null) ? "" : tag.InitialText;
			string[] sts = s.Split('\r'); // seems to be the only gen'd linebreak

            string fontName = "";
            if (swf.Fonts.ContainsKey(tag.FontID))
            {
                fontName = swf.Fonts[tag.FontID].FontName;
            }
			Color c = ParseRGBA(tag.TextColor);
			bool isMultiline = sts.Length > 1;
			
			for (int i = 0; i < sts.Length; i++)
			{
				TextRun tr = new TextRun();
				tr.isEditable = true;
				tr.isSelectable = true;
				tr.Color = c;
				tr.isMultiline = isMultiline;

				if(tag.HasFont)
				{
					tr.FontName = fontName;
				}
				tr.FontSize = tag.FontHeight / 20;
				tr.Text = sts[i];
				runs.Add(tr);				
			}
		}
		public void GetTextFromRecords(List<TextRecord> records, List<TextRun> runs)
		{
			float fontSize = 12F; // default
			string fontName = "Arial"; // default
			Color fontColor = new Color(0,0,0); // default black
			uint fontId;
            uint[] codeTable = null;
            bool isBold = false;
            bool isItalic = false;

			int yOffset = 0;
			TextRun prevRun = null;
			float vpad = 0;

			for (int i = 0; i < records.Count; i++)
			{
				TextRecord r = records[i];
				TextRun run = new TextRun();
				runs.Add(run);
				string text = "";

				if (r.YOffset > yOffset)
				{
					yOffset = r.YOffset;
					if (prevRun != null)
					{
						prevRun.isMultiline = true;
					}
					else if(r.TextHeight != 0)
					{
						// the space between the lines of the font
						vpad = (r.TextHeight - r.YOffset)/twips;
					}
				}
				if (r.StyleFlagsHasFont)
				{
					fontSize = r.TextHeight / twips;
					fontId = r.FontID;
					if (swf.Fonts.ContainsKey(fontId))
					{
                        DefineFont2_3 f = swf.Fonts[fontId];
						fontName = f.FontName;
                        codeTable = f.CodeTable;
                        isBold = f.FontFlagsBold;
                        isItalic = f.FontFlagsItalic;
					}
				}
				if (r.StyleFlagsHasColor)
				{
					fontColor = ParseRGBA(r.TextColor);
				}
				if (codeTable != null)
				{
					for (int j = 0; j < r.GlyphEntries.Length; j++)
					{
						GlyphEntry ge = r.GlyphEntries[j];
						uint charCode = codeTable[ge.GlyphIndex];
						text += (char)charCode;
					}
				}
				run.FontName = fontName;
				run.FontSize = fontSize;
				run.Color = fontColor;
				run.Text = text;
				run.isContinuous = !(r.StyleFlagsHasYOffset || r.StyleFlagsHasXOffset);
                run.isBold = isBold;
                run.isItalic = isItalic;
				//if ()
				//{
				//    run.Top = (r.YOffset - r.TextHeight) / twips;
				//}
				run.Top = (yOffset / twips) - fontSize + vpad;
				if (r.StyleFlagsHasXOffset)
				{
					run.Left = r.XOffset / twips;
				}
				prevRun = run;
			}
		}

		private Rectangle ParseSpriteBounds(DefineSpriteTag tag)
		{
			Rectangle result = Rectangle.Empty;
			for (int i = 0; i < tag.FirstFrameObjects.Count; i++)
			{
				PlaceObjectTag o = (PlaceObjectTag)tag.FirstFrameObjects[i];
				// yahoo has forward refs here... guard for now
				uint index = o.Character;
				if (!v.Definitions.ContainsKey(index))
				{
					continue;
				}
				IDefinition def = this.v.Definitions[index];
				Rectangle bnds = def.StrokeBounds;
				if (o.HasMatrix)
				{
					Point[] pts = new Point[] { bnds.Point, new Point(bnds.Point.X + bnds.Size.Width, bnds.Point.Y + bnds.Size.Height) };
					// bugfix: needed to transform the translation from twips
                    Matrix m = new Matrix(o.Matrix.ScaleX, o.Matrix.Rotate0, o.Matrix.Rotate1, o.Matrix.ScaleY, o.Matrix.TranslateX / twips, o.Matrix.TranslateY / twips);
                    m.TransformPoints(pts);
					bnds = new Rectangle(pts[0], new Size(pts[1].X - pts[0].X, pts[1].Y - pts[0].Y));
				}
				if (result == Rectangle.Empty)
				{
					result = bnds; // first placeObject
				}
				else
				{
					result = result.Union(bnds); // second+
				}
			}
			tag.ShapeBounds = new Rect(
				(int)result.Point.X,
				(int)(result.Point.X + result.Size.Width), 
				(int)result.Point.Y, 
				(int)(result.Point.Y + result.Size.Height));
			return result;
		}

		private void WriteJpegToDisk(string path, DefineBitsTag tag)
		{
            IOUtils.EnsurePath(path);
			FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
			if (!tag.HasOwnTable)
			{
				fs.Write(jpegPrefix, 0, jpegPrefix.Length);
				fs.Write(swf.JpegTable.JpegTable, 0, swf.JpegTable.JpegTable.Length);
				fs.Write(tag.JpegData, 0, tag.JpegData.Length);
				fs.Write(jpegSuffix, 0, jpegSuffix.Length);
			}
			else
			{
				fs.Write(tag.JpegData, 0, tag.JpegData.Length);
			}
			fs.Close();
		}	
		private void WriteLosslessBitmapToDisk(string path, DefineBitsLosslessTag tag)
		{
            IOUtils.EnsurePath(path);
			System.Drawing.Bitmap bmp = tag.GetBitmap();
			bmp.Save(path);
			bmp.Dispose();
		}
		private void WriteSoundToDisk(string path, List<byte[]> tag)
		{
            IOUtils.EnsurePath(path);
			FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
			int skipHeader = 2;
			for (int i = 0; i < tag.Count; i++)
			{
				fs.Write(tag[i], skipHeader, tag[i].Length - skipHeader);
			}
			fs.Close();
		}
		private void WriteSoundToDisk(string path, DefineSoundTag tag)
		{
			byte[] bytes = tag.SoundData;

            IOUtils.EnsurePath(path);
			FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
			if (tag.SoundFormat == SoundCompressionType.MP3)
			{
				int skipHeader = 2;
				fs.Write(bytes, skipHeader, bytes.Length - skipHeader);
			}
			else if (tag.SoundFormat == SoundCompressionType.Uncompressed || tag.SoundFormat == SoundCompressionType.UncompressedLE)
			{
				byte[] header = GetWavHeader(tag);
				fs.Write(header, 0, header.Length);
				fs.Write(bytes, 0, bytes.Length);
			}
			else if(tag.SoundFormat == SoundCompressionType.ADPMC)
			{
				// not supported
				Log.AppendLine(
					"ADPMC Sound not supported" +
					" at frame " + this.curFrame +
					" in clip " + this.curTimeline.Name);
			}
			fs.Close();
		}
		private byte[] GetWavHeader(DefineSoundTag tag)
		{
			/*
				52 49 46 46 // 'RIFF'
			XX	D0 3B 01 00 // Chunk Data Size (file size) - 8
				57 41 56 45 // 'WAVE'

				66 6D 74 20 // 'fmt '
			XX	10 00 00 00 // size -- 16 + extra format bytes
				01 00 		// Compression code
			XX	02 00 		// Number of channels
				22 56 00 00 // Sample rate
				88 58 01 00 // Average bytes per second
				04 00 		// Block align
				10 00 		// Significant bits per sample
							// Extra format bytes (may be 2 bytes that specify additonal fmt byte count)
 
				64 61 74 61 // 'data'
			XX	7C 3B 01 00 // chunkSize
				F8 FF		// ?? ??
			 */

			// Compression Code
			//	0 (0x0000) 		Unknown
			//	1 (0x0001) 		PCM/uncompressed			**
			//	2 (0x0002) 		Microsoft ADPCM				**
			//	6 (0x0006) 		ITU G.711 a-law
			//	7 (0x0007) 		ITU G.711 µ-law
			//	17 (0x0011) 	IMA ADPCM
			//	20 (0x0016) 	ITU G.723 ADPCM (Yamaha)
			//	49 (0x0031) 	GSM 6.10
			//	64 (0x0040) 	ITU G.721 ADPCM
			//	80 (0x0050) 	MPEG
			//	65,536 (0xFFFF) Experimental


			byte[] header = new byte[] { 0x52, 0x49, 0x46, 0x46, 0xD0, 0x3B, 0x01, 0x00, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20, 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x22, 0x56, 0x00, 0x00, 0x88, 0x58, 0x01, 0x00, 0x04, 0x00, 0x10, 0x00, 0x64, 0x61, 0x74, 0x61, 0x7C, 0x3B, 0x01, 0x00, 0x00, 0x00};
			uint dataLen = (uint)tag.SoundData.Length;
			uint riffChunkSize = dataLen + 38;

			uint compression = 1; // uncompressed and uncompressedLE
			if (tag.SoundFormat == SoundCompressionType.ADPMC)
			{
				compression = 2; // more work needs to be done here to support the 'extra byte' info.
			}
			uint channelCount = tag.IsStereo ? 2U : 1U;
			uint sampleRate = tag.SoundRate;
			uint blockAlign = (tag.SoundSize / 8) * channelCount;
			uint avgBytesPS = sampleRate * blockAlign;
			uint sigBitsPerSample = tag.SoundSize;

			uint dataChunkSize = dataLen + 2; // swf misses 2 first bytes?

			header[4] = (byte)(riffChunkSize & 0xFF);
			header[5] = (byte)((riffChunkSize >> 8) & 0xFF);
			header[6] = (byte)((riffChunkSize >> 16) & 0xFF);
			header[7] = (byte)((riffChunkSize >> 24) & 0xFF);

			header[20] = (byte)compression;
			header[22] = (byte)channelCount;

			header[24] = (byte)(sampleRate & 0xFF);
			header[25] = (byte)((sampleRate >> 8) & 0xFF);
			header[26] = (byte)((sampleRate >> 16) & 0xFF);
			header[27] = (byte)((sampleRate >> 24) & 0xFF);

			header[28] = (byte)(avgBytesPS & 0xFF);
			header[29] = (byte)((avgBytesPS >> 8) & 0xFF);
			header[30] = (byte)((avgBytesPS >> 16) & 0xFF);
			header[31] = (byte)((avgBytesPS >> 24) & 0xFF);

			header[32] = (byte)blockAlign;
			header[34] = (byte)sigBitsPerSample;

			header[40] = (byte)(dataChunkSize & 0xFF);
			header[41] = (byte)((dataChunkSize >> 8) & 0xFF);
			header[42] = (byte)((dataChunkSize >> 16) & 0xFF);
			header[43] = (byte)((dataChunkSize >> 24) & 0xFF);

			return header;
		}
		private uint GetDuration(uint frameCount)
		{
            // was (framecount + 1), changed oct3 2009 // robin
            return (uint)(frameCount * (1 / swf.Header.FrameRate) * 1000);
		}

		#endregion

		private void ParseTag(ISwfTag tag)
		{
			switch (tag.TagType)
			{
				case TagType.FileAttributes:
					ParseFileAttributesTag((FileAttributesTag)tag);
					break;
				case TagType.BackgroundColor:
					ParseBackgroundColor((BackgroundColorTag)tag);
					break;
				case TagType.End:
					// nothing to do
					break;
				case TagType.DefineSprite:
					ParseDefineSpriteTag((DefineSpriteTag)tag);
					break;
				case TagType.PlaceObject:
					ParsePlaceObjectTag((PlaceObjectTag)tag);
					break;
				case TagType.PlaceObject2:
					ParsePlaceObject2Tag((PlaceObject2Tag)tag);
					break;
				case TagType.PlaceObject3:
					ParsePlaceObject3Tag((PlaceObject3Tag)tag);
					break;
				case TagType.RemoveObject:
					ParseRemoveObjectTag((RemoveObjectTag)tag);
					break;
				case TagType.RemoveObject2:
					ParseRemoveObject2Tag((RemoveObject2Tag)tag);
					break;
				case TagType.ShowFrame:
					curFrame += 1;
					break;
				case TagType.FrameLabel:
					uint curTime = (uint)((curFrame * (1 / swf.Header.FrameRate)) * 1000);
					curTimeline.Labels.Add(new Label(curTime, ((FrameLabelTag)tag).TargetName));
                    break;
                case TagType.DefineShape:
                    ParseDefineShapeTag((DefineShapeTag)tag);
                    break;
                case TagType.DefineShape2:
                    ParseDefineShape2Tag((DefineShape2Tag)tag);
                    break;
				case TagType.DefineShape3:
					ParseDefineShape3Tag((DefineShape3Tag)tag);
					break;
				case TagType.DefineShape4:
					ParseDefineShape4Tag((DefineShape4Tag)tag);
					break;
				case TagType.JPEGTables:
					// not retained
					break;
				case TagType.DefineBits:
					ParseDefineBits((DefineBitsTag)tag);
					break;
				case TagType.DefineBitsJPEG2:
                   // v.NextId();
                    ParseDefineBits((DefineBitsTag)tag);
					//ParseDefineBitsJPEG2((DefineBitsJPEG2Tag)tag);
					break;
				case TagType.DefineBitsJPEG3:
                    //v.NextId();
                    ParseDefineBits((DefineBitsTag)tag);
					//ParseDefineBitsJPEG3((DefineBitsJPEG3Tag)tag);
					break;
				case TagType.DefineBitsLossless:
					ParseDefineBitsLossless((DefineBitsLosslessTag)tag);
					break;
				case TagType.DefineBitsLossless2:
					ParseDefineBitsLossless((DefineBitsLosslessTag)tag);
					break;
				case TagType.DefineSound:
					ParseDefineSound((DefineSoundTag)tag);
					break;
				case TagType.StartSound:
					ParseStartSound((StartSoundTag)tag);
					break;
				case TagType.SoundStreamHead:
					ParseSoundStreamHead((SoundStreamHeadTag)tag);
					break;
				case TagType.SoundStreamBlock:
					if (lastSoundDef != null)
					{
						lastSoundDef.StartTime = (uint)((curFrame * (1 / swf.Header.FrameRate)) * 1000);
						lastSoundDef = null;
					}
					break;
				case TagType.DefineFontInfo:
					break;
				case TagType.DefineFontInfo2:
					break;
				case TagType.DefineFont:  //temp
					v.NextId();
					break;
				case TagType.DefineFont2: //temp
					v.NextId();
					break;
				case TagType.DefineFont3: //temp
					v.NextId();
					break;
				case TagType.DefineFontAlignZones:
					break;
				case TagType.CSMTextSettings:
					break;
				case TagType.DefineText:
					ParseDefineText((DefineTextTag)tag);
					break;
				case TagType.DefineText2:
					ParseDefineText((DefineTextTag)tag);
					break;
				case TagType.DefineEditText:
					ParseEditText((DefineEditTextTag)tag);
					break;
				case TagType.ExportAssets:
				    ParseExportAssets((ExportAssetsTag)tag);
				    break;

				case TagType.UnsupportedDefinition:
					v.NextId();
					Log.AppendLine(
						((UnsupportedDefinitionTag)tag).Message + 
						" at frame " + this.curFrame + 
						" in clip " + this.curTimeline.Name);
					break;


				//case TagType.DefineButtonCxform:
				//	ParseDefineButtonCxform((DefineButtonCxformTag)tag);
				//	break;

				//case TagType.Protect:
				//	ParseProtect((ProtectTag)tag);
				//	break;

				//case TagType.PathsArePostScript:
				//	ParsePathsArePostScript((PathsArePostScriptTag)tag);
				//	break;

				//case TagType.SyncFrame:
				//	ParseSyncFrame((SyncFrameTag)tag);
				//	break;

				//case TagType.FreeAll:
				//	ParseFreeAll((FreeAllTag)tag);
				//	break;

				//case TagType.DefineText2:
				//	ParseDefineText2((DefineText2Tag)tag);
				//	break;

				//case TagType.DefineButton2:
				//	ParseDefineButton2((DefineButton2Tag)tag);
				//	break;

				//case TagType.DefineSprite:
				//	ParseDefineSprite((DefineSpriteTag)tag);
				//	break;

				//case TagType.NameCharacter:
				//	ParseNameCharacter((NameCharacterTag)tag);
				//	break;

				//case TagType.SerialNumber:
				//	ParseSerialNumber((SerialNumberTag)tag);
				//	break;

				//case TagType.DefineTextFormat:
				//	ParseDefineTextFormat((DefineTextFormatTag)tag);
				//	break;

				//case TagType.SoundStreamHead2:
				//	ParseSoundStreamHead2((SoundStreamHead2Tag)tag);
				//	break;

				//case TagType.DefineMorphShape:
				//	ParseDefineMorphShape((DefineMorphShapeTag)tag);
				//	break;

				//case TagType.FrameTag:
				//	ParseFrameTag((FrameTagTag)tag);
				//	break;

				//case TagType.DefineFont2:
				//	ParseDefineFont2((DefineFont2Tag)tag);
				//	break;

				//case TagType.GenCommand:
				//	ParseGenCommand((GenCommandTag)tag);
				//	break;

				//case TagType.DefineCommandObj:
				//	ParseDefineCommandObj((DefineCommandObjTag)tag);
				//	break;

				//case TagType.CharacterSet:
				//	ParseCharacterSet((CharacterSetTag)tag);
				//	break;

				//case TagType.FontRef:
				//	ParseFontRef((FontRefTag)tag);
				//	break;

				//case TagType.ImportAssets:
				//	ParseImportAssets((ImportAssetsTag)tag);
				//	break;

				//case TagType.EnableDebugger:
				//	ParseEnableDebugger((EnableDebuggerTag)tag);
				//	break;

				//case TagType.EnableDebugger2:
				//	ParseEnableDebugger2((EnableDebugger2Tag)tag);
				//	break;

				//case TagType.ScriptLimits:
				//	ParseScriptLimits((ScriptLimitsTag)tag);
				//	break;

				//case TagType.SetTabIndex:
				//	ParseSetTabIndex((SetTabIndexTag)tag);
				//	break;

				//case TagType.DefineEditText:
				//	ParseDefineEditText((DefineEditTextTag)tag);
				//	break;

				//case TagType.DefineVideo:
				//	ParseDefineVideo((DefineVideoTag)tag);
				//	break;

				//case TagType.FreeCharacter:
				//	ParseFreeCharacter((FreeCharacterTag)tag);
				//	break;

				//case TagType.PlaceObject:
				//	ParsePlaceObject((PlaceObjectTag)tag);
				//	break;

				//case TagType.RemoveObject:
				//	ParseRemoveObject((RemoveObjectTag)tag);
				//	break;

				//case TagType.DefineButton:
				//	ParseDefineButton((DefineButtonTag)tag);
				//	break;



				//case TagType.DefineFont:
				//	ParseDefineFont((DefineFontTag)tag);
				//	break;

				//case TagType.DefineText:
				//	ParseDefineText((DefineTextTag)tag);
				//	break;

				//case TagType.DoAction:
				//	ParseDoAction((DoActionTag)tag);
				//	break;

				//case TagType.DefineFontInfo:
				//	ParseDefineFontInfo((DefineFontInfoTag)tag);
				//	break;

				//case TagType.DefineSound:
				//	ParseDefineSound((DefineSoundTag)tag);
				//	break;

				//case TagType.StartSound:
				//	ParseStartSound((StartSoundTag)tag);
				//	break;

				//case TagType.DefineButtonSound:
				//	ParseDefineButtonSound((DefineButtonSoundTag)tag);
				//	break;

				//case TagType.SoundStreamHead:
				//	ParseSoundStreamHead((SoundStreamHeadTag)tag);
				//	break;



			}
		}
	}
}
