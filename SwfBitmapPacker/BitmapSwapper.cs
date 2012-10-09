using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Swf;

namespace DDW.SwfBitmapPacker
{
	public class BitmapSwapper
	{
		private byte[] jpegPrefix = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0, 1, 1, 1, 0, 0x48, 0, 0x48, 0, 0 };
		private byte[] jpegSuffix = new byte[] { 0xFF, 0xD9 };
		
		private SwfCompilationUnit swf;
        private UnsafeBitmap fullBitmap;
		private const float twips = 20F;
        Dictionary<uint, System.Drawing.Rectangle> rects;
        private bool addedFullBitmap = false;
        private uint fullBitmapId;

        public void Convert(SwfCompilationUnit swf, UnsafeBitmap fullBitmap, Dictionary<uint, System.Drawing.Rectangle> rects)
		{
			this.swf = swf;
            this.fullBitmap = fullBitmap;
            this.rects = rects;
			foreach(ISwfTag tag in swf.Tags)
			{
				this.ParseTag(tag);
			}
        }

		#region Shapes
        private void ParseDefineShapeTag(DefineShapeTag tag)
        {
            ParseShapeWithStyle(tag.Shapes);
        }
        private void ParseDefineShape2Tag(DefineShape2Tag tag)
        {
            ParseShapeWithStyle(tag.Shapes);
        }
		private void ParseDefineShape3Tag(DefineShape3Tag tag)
		{
			ParseShapeWithStyle(tag.Shapes);
		}
		private void ParseDefineShape4Tag(DefineShape4Tag tag)
		{
			ParseShapeWithStyle(tag.Shapes);
		}
		private void ParseShapeWithStyle(ShapeWithStyle tag)
		{
			ParseFillStyleArray(tag.FillStyles);
			foreach (IShapeRecord o in tag.ShapeRecords)
			{
                if (o is StyleChangedRecord)
				{
					StyleChangedRecord scr = (StyleChangedRecord)o;
					if (scr.HasNewStyles)
					{
						ParseFillStyleArray(scr.FillStyles);
					}
				}
			}
		}
		private void ParseFillStyleArray(FillStyleArray tag)
		{
			foreach (IFillStyle o in tag.FillStyles)
			{
				if (o is BitmapFill)
				{
				    ParseBitmapFill((BitmapFill)o);
				}
			}
		}
		private void ParseBitmapFill(BitmapFill tag)
		{
            if (rects.ContainsKey(tag.CharacterId))
            {
                System.Drawing.Rectangle r = rects[tag.CharacterId];
                tag.CharacterId = fullBitmapId;//bitmap id
                tag.Matrix.TranslateX = r.Left * twips; // x offset
                tag.Matrix.TranslateY = r.Top * twips; // y offset
            }
            else
            {
                Console.WriteLine("not found: " + tag.CharacterId);
            }
		}
        #endregion
		#region Display List
		private void ParseDefineSpriteTag(DefineSpriteTag tag)
		{
			foreach (ISwfTag t in tag.ControlTags)
			{
				this.ParseTag(t);
			}
        }
        #endregion
		#region Bitmaps

		private void ParseDefineBits(DefineBitsTag tag)
		{
            //DDW.Vex.ImageFill bf = new DDW.Vex.ImageFill();
            //string path = v.ResourceFolder + @"/" + VexObject.BitmapPrefix + tag.CharacterId + ".jpg";
            //WriteJpegToDisk(path, tag);
            //Size sz = Utils.GetJpgSize(path);

            //if (tag.HasAlphaData) // this is an alpha jpg, convert to png
            //{
            //    byte[] alphaData = SwfReader.Decompress(tag.CompressedAlphaData, (uint)(sz.Width * sz.Height));

            //    string bmpPath = path;
            //    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(bmpPath, false);
            //    path = v.ResourceFolder + @"/" + VexObject.BitmapPrefix + tag.CharacterId + ".png";

            //    Utils.WriteAlphaJpg(alphaData, bmp, path);

            //    bmp.Dispose();
            //    File.Delete(bmpPath);
            //}

            //Image img = new Vex.Image(path, v.NextId());
            //img.Id = tag.CharacterId;
            //img.StrokeBounds = new Rectangle(0, 0, sz.Width, sz.Height);

            //bitmapPaths.Add(img.Id, path);
            //v.Definitions.Add(img.Id, img);
		}
		private void ParseDefineBitsLossless(DefineBitsLosslessTag tag)
		{
            //DDW.Vex.ImageFill bf = new DDW.Vex.ImageFill();
            //string path = v.ResourceFolder + @"/" + VexObject.BitmapPrefix + tag.CharacterId + ".png";

            //WriteLosslessBitmapToDisk(path, tag);

            //Image img = new Vex.Image(path, v.NextId());
            //img.Id = tag.CharacterId;
            //img.StrokeBounds = new Rectangle(0, 0, tag.Width, tag.Height);

            //bitmapPaths.Add(img.Id, path);
            //v.Definitions.Add(img.Id, img);
		}

		#endregion

		private void ParseTag(ISwfTag tag)
		{
			switch (tag.TagType)
			{
				case TagType.DefineSprite:
					ParseDefineSpriteTag((DefineSpriteTag)tag);
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
                    ParseDefineBits((DefineBitsTag)tag);
					break;
				case TagType.DefineBitsJPEG3:
                    ParseDefineBits((DefineBitsTag)tag);
					break;
				case TagType.DefineBitsLossless:
					ParseDefineBitsLossless((DefineBitsLosslessTag)tag);
					break;
				case TagType.DefineBitsLossless2:
                    if (!addedFullBitmap)
                    {
                        addedFullBitmap = true;
                        DefineBitsLosslessTag t = (DefineBitsLosslessTag)tag;
                        fullBitmapId = t.CharacterId;
                        //DefineBitsLosslessTag t = new DefineBitsLosslessTag(true);
                        //t.CharacterId = ot.CharacterId;
                        //t.BitmapFormat = ot.BitmapFormat;
                        t.Width = (uint)fullBitmap.Bitmap.Width;
                        t.Height = (uint)fullBitmap.Bitmap.Height;
                        t.OrgBitmapData = null;
                        uint pxCount = t.Width * t.Height;
                        RGBA[] pxs = new RGBA[pxCount];

                        fullBitmap.LockBitmap();
                        for (int i = 0; i < pxCount; i++)
                        {
                            int x = (int)(i % t.Width);
                            int y = (int)(Math.Floor((double)(i / t.Width)));
                            PixelData pd = fullBitmap.GetPixel(x, y);
                            pxs[i] = new RGBA(pd.red, pd.green, pd.blue, pd.alpha);
                        }
                        t.BitmapData = pxs;
                        fullBitmap.UnlockBitmap();
                    }
					break;
				case TagType.UnsupportedDefinition:					
					break;
            }
		}
	}
}
