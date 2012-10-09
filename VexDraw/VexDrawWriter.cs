using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex;
using System.IO;
using System.Collections;

namespace DDW.VexDraw
{
    public class VexDrawWriter
    {
        public static uint idBitCount = 16;

        private Stream stream;
		private	uint curNum = 0;
		private	uint curBit = 0x80000000;

        public uint fillNBits;
        public uint strokeNBits;

        public VexDrawWriter(Stream stream)
        {
            this.stream = stream;
        }

        public void WriteDrawObject(DrawObject drawObject)
        {
            WriteNameTable(drawObject.definitionNames, VexDrawTag.DefinitionNameTable);
            WriteNameTable(drawObject.instanceNames, VexDrawTag.InstanceNameTable);
            WriteStringTable(drawObject.paths, VexDrawTag.PathNameTable);

            WriteStrokeDefs(drawObject.strokes);
            uint fillCount = (uint)(drawObject.solidFills.Count + drawObject.gradientFills.Count);
            WriteSolidFills(drawObject.solidFills, fillCount);
            WriteGradientDefs(drawObject.gradientFills);

            for (int i = 0; i < drawObject.drawImages.Count; i++)
            {
                WriteImage(drawObject.drawImages[i]);
            }

            for (int i = 0; i < drawObject.drawSymbols.Count; i++)
            {
                WriteSymbol(drawObject.drawSymbols[i]);
            }

            for (int i = 0; i < drawObject.drawTimelines.Count; i++)
            {
                WriteTimeline(drawObject.drawTimelines[i]);
            }

            WriteBits((int)VexDrawTag.End, 8);
        }
        private void WriteStringTable(List<string> stringTable, VexDrawTag tableKind)
        {
            WriteTag(tableKind);

            uint namesNBits = GetMaxNBits(stringTable);
            WriteNBitsCount(namesNBits);

            WriteBits(stringTable.Count, 16);

            foreach (string s in stringTable)
            {
                WriteBits(s.Length, 16);
                foreach (char c in s)
                {
                    WriteBits((uint)c, namesNBits);
                }
            }

            FlushTag();
        }
        private void WriteNameTable(Dictionary<uint, string> nameTable, VexDrawTag tableKind)
        {
            if (nameTable.Count > 0)
            {
                WriteTag(tableKind);

                //uint idNBits = MinBits((uint)nameTable.Count);
                string[] names = nameTable.Values.ToArray();
                uint namesNBits = GetMaxNBits(names);

                //WriteNBitsCount(idNBits);
                WriteNBitsCount(namesNBits);

                WriteBits(nameTable.Count, 16);

                foreach (uint id in nameTable.Keys)
                {
                    WriteBits(id, idBitCount);
                    string s = nameTable[id];
                    WriteBits(s.Length, 16);
                    foreach (char c in s)
                    {
                        WriteBits((uint)c, namesNBits);
                    }
                }

                FlushTag();
            }
        }
        private void WriteStrokeDefs(List<StrokeStyle> strokes)
        {
            WriteTag(VexDrawTag.StrokeList);

            strokeNBits = MinBits((uint)strokes.Count); // always positive
            WriteBits(strokeNBits, 8);

            // Stroke Colors
            uint[] cols = new uint[strokes.Count];
            cols[0] = 0; // no color
            for (int i = 1; i < strokes.Count; i++)
            {
                cols[i] = strokes[i].Color.AFlipRGB;
            }
            uint colorNBits = MinBits(cols); // always positive

            // Stroke Widths in next sequence
            uint[] widths = new uint[strokes.Count];
            for (int i = 0; i < strokes.Count; i++)
            {
                widths[i] = (uint)(strokes[i].LineWidth * DrawObject.twips);
            }
            uint lineWidthNBits = MinBits(widths); // always positive
            
            // write
            WriteNBitsCount(colorNBits);
            WriteNBitsCount(lineWidthNBits);

            WriteBits(strokes.Count, 11); // stroke count
            for (int i = 0; i < strokes.Count; i++)
            {
                WriteBits(cols[i], colorNBits);
                WriteBits(widths[i], lineWidthNBits);
            }

            FlushTag();
        }
		private void WriteSolidFills(List<SolidFill> fills, uint totalFillCount)
        {
            WriteTag(VexDrawTag.SolidFillList);

            fillNBits = MinBits(totalFillCount);
            WriteBits(fillNBits, 8);

            uint[] vals = new uint[fills.Count];
            for (int i = 0; i < fills.Count; i++)
			{
                vals[i] = fills[i].Color.AFlipRGB;
			}

			uint nBits = MinBits(vals); // always positive
            WriteNBitsCount(nBits);

            WriteBits(fills.Count, 11); // 2074 max
			for(int i = 0; i < vals.Length; i++)
			{
				WriteBits(vals[i], nBits);
			}
			FlushTag();
		}        
		private void WriteGradientDefs(List<GradientFill> gradients)
		{
            WriteTag(VexDrawTag.GradientFillList);
			WriteBits(0, 5); // note: no nBits here, just padding
            WriteBits(gradients.Count, 11); 
			for(int index = 0; index < gradients.Count; index++)
			{
                GradientFill gf = gradients[index];
				// first type - all non radial will be solid
                // use 8 bits for future expansion and to keep semi aligned
                int type = (gf.FillType == FillType.Radial) ? 1 : 0;
				WriteBits(type, 3);

                float[] tltr = DrawObject.GetGradientLine(gf);
                int[] values = new int[4];
                for (int i = 0; i < tltr.Length; i++)
                {
                    values[i] = (int)(tltr[i] * DrawObject.twips);
                }
                WriteFourNBitValues(values);

                // new argb colors
                List<Color> colors = new List<Color>(gf.Fills);
                List<float> positions = new List<float>(gf.Stops);
				if(gf.FillType == FillType.Radial)
				{
                    gf.ReverseStops(colors, positions);
				}

                // stops and ratios [[col,rat],[col,rat]...]
                int stopCount = colors.Count;
				if(stopCount > 8)
				{
					stopCount = 8;
					Console.WriteLine("*Flash only supports 8 colors max in gradients");
				}

				uint[] wCols = new uint[stopCount];
				for(int i = 0; i < stopCount; i++)
				{
                    wCols[i] = colors[i].AFlipRGB; 
				}
                uint colBits = MinBits(wCols);
                WriteNBitsCount(colBits);

                int[] ratios = new int[positions.Count];
				for(int i = 0; i < stopCount; i++)
				{
					ratios[i] = (int)(positions[i]*255);
				}
				uint ratioBits = MinBits(ratios);
                WriteNBitsCount(ratioBits);

				WriteBits(stopCount, 11);

				for(int i = 0; i < stopCount; i++)
				{
                    WriteBits(wCols[i], colBits);
                    WriteBits(ratios[i], ratioBits);
				}
			}
			FlushTag();
		}
        private void WriteImage(DrawImage img)
        {
            WriteTag(VexDrawTag.ImageDefinition);
            WriteBits(img.Id, idBitCount);
            WriteRect(img.SourceRectangle);
            WriteBits(img.PathId, 11);
            FlushTag();
        }

        private void WriteSymbol(DrawSymbol symbol)
        {
            WriteTag(VexDrawTag.SymbolDefinition);

            WriteBits(symbol.Id, idBitCount);
            WriteRect(symbol.StrokeBounds);
            
            // write paths, get nBits for path numbers
            WriteBits(symbol.Paths.Count, 11); // number of path defs
            uint pathIndexNBits = MinBits((uint)symbol.Paths.Count);
            // ***WriteBits(pathIndexNBits, 5); // number of path defs
            WriteNBitsCount(pathIndexNBits);

            for (int i = 0; i < symbol.Paths.Count; i++)
            {
                WritePath(symbol.Paths[i]);
            }

            WriteBits(symbol.Shapes.Count, 11); // number of shape defs
            for (int i = 0; i < symbol.Shapes.Count; i++)
            {
                WriteShape(symbol.Shapes[i], pathIndexNBits);
            }

            FlushTag();
        }

        private void WritePath(DrawPath path)
        {
            int count = 0;
            List<SegmentType> types = new List<SegmentType>();
            List<int> values = new List<int>();

            Point prevPoint = Point.Empty;
            for (int i = 0; i < path.Segments.Count; i++)
            {
                IShapeData sd = path.Segments[i];

                // moveTo
                if (sd.StartPoint != prevPoint)
                {
                    types.Add(SegmentType.Move);
                    values.Add((int)(sd.StartPoint.X * DrawObject.twips));
                    values.Add((int)(sd.StartPoint.Y * DrawObject.twips));
                    count += 2;
                }

                types.Add(sd.SegmentType);

                switch (sd.SegmentType)
                {
                    case SegmentType.Line:
                        values.Add((int)(sd.EndPoint.X * DrawObject.twips));
                        values.Add((int)(sd.EndPoint.Y * DrawObject.twips));
                        count += 2;
                        break;

                    case SegmentType.QuadraticBezier:
                        QuadBezier qb = (QuadBezier)sd;
                        values.Add((int)(qb.Control.X * DrawObject.twips));
                        values.Add((int)(qb.Control.Y * DrawObject.twips));
                        values.Add((int)(qb.EndPoint.X * DrawObject.twips));
                        values.Add((int)(qb.EndPoint.Y * DrawObject.twips));
                        count += 4;
                        break;

                    case SegmentType.CubicBezier:
                        CubicBezier cb = (CubicBezier)sd;
                        values.Add((int)(cb.Control0.X * DrawObject.twips));
                        values.Add((int)(cb.Control0.Y * DrawObject.twips));
                        values.Add((int)(cb.Control1.X * DrawObject.twips));
                        values.Add((int)(cb.Control1.Y * DrawObject.twips));
                        values.Add((int)(cb.EndPoint.X * DrawObject.twips));
                        values.Add((int)(cb.EndPoint.Y * DrawObject.twips));
                        count += 6;
                        break;
                }

                prevPoint = sd.EndPoint;
            }

            uint dataBits = MinBits(values);
            WriteNBitsCount(dataBits);
            WriteBits(types.Count, 11);

            int[] lens = new int[]{2,2,4,6};
            int valIndex = 0;
            for (int i = 0; i < types.Count; i++)
            {
                // types  M:0 L:1 Q:2 C:3
                int type = (int)types[i];
                WriteBits(type, 2);
                // data
                for (int j = 0; j < lens[type]; j++)
                {
                    WriteBits(values[valIndex++], dataBits);
                }
            }
        }
        private void WriteShape(DrawShape shape, uint pathNBits)
        {
            WriteBits(shape.StrokeIndex, strokeNBits);
            WriteBits(shape.FillIndex, fillNBits);
            WriteBits(shape.PathIndex, pathNBits);
        }
        private void WriteTimeline(DrawTimeline timeline)
        {
            WriteTag(VexDrawTag.TimelineDefinition);

            WriteBits(timeline.Id, idBitCount);

            // todo: name

            WriteRect(timeline.StrokeBounds);
            

            WriteBits(timeline.Instances.Count, 11); // number of path defs
            for (int i = 0; i < timeline.Instances.Count; i++)
            {
                WriteInstance(timeline.Instances[i]);
            }

			FlushTag();
		}
        private void WriteInstance(IInstance inst)
        {
            // [defid,hasVals[7:bool], x?,y?,scaleX?, scaleY?, rotation?, skew?, "name"?
            //[9,[262.5,53.26]],
            //[5,[519.83,248.82],[5.042175,5.0422,54.15462]],
            //[3,[122.32,70.4],[0.9999654,0.9999616,-30.16027],"name"]

            WriteBits(inst.DefinitionId, idBitCount);
            WriteBits(inst.InstanceHash, idBitCount);
            
            Vex.Matrix m = inst.GetTransformAtTime(0).Matrix;
            Vex.MatrixComponents mc = m.GetMatrixComponents();
            int[] vals = new int[]            
            {
                (int)(mc.TranslateX * DrawObject.twips),
                (int)(mc.TranslateY * DrawObject.twips), 
                (int)(mc.ScaleX * DrawObject.twips),
                (int)(mc.ScaleY * DrawObject.twips),
                (int)(mc.Rotation * DrawObject.twips), 
                (int)(mc.Shear * DrawObject.twips) 
            };
            bool[] hasVals = new bool[]
            {
                vals[0] != 0,
                vals[1] != 0,
                vals[2] != DrawObject.twips,
                vals[3] != DrawObject.twips,
                vals[4] != 0,
                vals[5] != 0                
            };
            bool hasName = !((inst.Name == null) || (inst.Name == ""));

            bool hasNumber = false;
            for (int i = 0; i < hasVals.Length; i++)
            {
                WriteBit(hasVals[i]);
                if (hasVals[i])
                {
                    hasNumber = true;
                }
            }
            WriteBit(hasName);

            if (hasNumber)
            {
                uint nBits = MinBits(vals); // always positive
                WriteNBitsCount(nBits);

                for (int i = 0; i < hasVals.Length; i++)
                {
                    if (hasVals[i])
                    {
                        WriteBits(vals[i], nBits);
                    }
                }
            }

            if (hasName)
            {
                // todo: write name strings
            }

        }

        #region writer
        private void WriteNBitsCount(uint count)
        {
            if (count < 2)
            {
                throw new ArgumentException("Too few nBits in compressed export.");
            }
            WriteBits(count - 2, 5);
        }
        private void WriteBit(bool val)
        {
            WriteBits(val ? 1 : 0, 1);
        }
        private void WriteBits(uint num, uint nBits)
        {
            WriteBits((int)num, nBits);
        }
        private VexDrawTag lastTag;
        private long lastTagIndex;
        private void WriteTag(VexDrawTag tag)
        {
            lastTag = tag;
            lastTagIndex = stream.Position;
            WriteBits((int)tag, 8);
            WriteBits(0xFFFFFF, 24); // length, tbd after write
        }
        private void WriteTagLength()
        {
            // don't include tag type and length spedifier measures in taglen
            int tagHeaderSize = 4;
            long len = stream.Position - lastTagIndex - tagHeaderSize;
            stream.Seek(-(len + tagHeaderSize), SeekOrigin.Current);
            WriteBits((int)lastTag, 8);
            WriteBits((int)len, 24);
            stream.Seek(len, SeekOrigin.Current);

            lastTag = VexDrawTag.None;
            lastTagIndex = 0;
        }
        private void WriteBits(int num, uint nBits)
		{
			// curBit is the bit about to be written to
			// curNum is the number being created
			// WriteNum(uint) writes a number to the 'stream'
			for(int i = (int)nBits - 1; i >= 0; i--)
			{
				if((num & (int)Math.Pow(2,i) ) != 0)
				{
					// bit set
					curNum |= curBit;
				}
				else
				{
					// bit not set
					curNum &= ~curBit;
				}
				curBit >>= 1;
				if(curBit == 0)
				{
					WriteUint(curNum);
					curBit = 0x80000000;
					curNum = 0;
				}
			}
		}
		private void WriteUint(uint num)
		{            
            stream.WriteByte((byte)((num >> 24) & 0xFF));
            stream.WriteByte((byte)((num >> 16) & 0xFF));
            stream.WriteByte((byte)((num >> 8) & 0xFF));
            stream.WriteByte((byte)(num & 0xFF));
		}
        private void WriteRect(Rectangle r)
        {
            int[] values = new int[]
            {
                (int)(r.Left * DrawObject.twips),
                (int)(r.Top * DrawObject.twips),
                (int)(r.Width * DrawObject.twips),
                (int)(r.Height * DrawObject.twips),
            };
            WriteFourNBitValues(values);
        }

        private void WriteFourNBitValues(int[] values)
        {
            if (values.Length != 4)
            {
                throw new ArgumentException("Array requires four values in WriteFourNBitValues");
            }
            uint dataBits = MinBits(values);
            WriteNBitsCount(dataBits);

            for (int i = 0; i < values.Length; i++)
            {
                WriteBits(values[i], dataBits);
            }
        }

        private void FlushTag()
		{
			if(curBit != 0x80000000)
			{
				WriteUint(curNum);
			}
			curBit = 0x80000000;
			curNum = 0;

            WriteTagLength();
		}

        private uint MinBits(uint val)	
		{
			if(val == 0) return 1;
			uint mask = 1;
			uint nbits;
			for(nbits = 1; nbits < 32; nbits++)
			{
				mask <<= 1;
                if (mask > val)
                {
                    break;
                }
			}

			return nbits < 2 ? 2 : nbits;
		}
        private uint MinBits(IEnumerable<uint> uints)
        {
            uint min = 0;
            foreach (uint val in uints)
            {
                uint curMin = MinBits((uint)val);
                if (curMin > min)
                {
                    min = curMin;
                }
            }
            return min;
        }
        private uint MinBits(IEnumerable<int> ints)
        {
            uint min = 0;
            foreach (int val in ints)
            {
                uint v = (uint)Math.Abs(val);
                uint curMin = MinBits(v) + 1;
                
                if (curMin > min)
                {
                    min = curMin;
                }
            }
            return min;
        }
        private uint GetMaxNBits(IEnumerable<string> table)
        {
            uint result = 0;
            foreach (string s in table)
            {
                foreach (char c in s)
                {
                    if ((uint)c > result)
                    {
                        result = (uint)c;
                    }
                }
            }
            result = (uint)Math.Floor(Math.Log(result, 2)) + 1;
            return result < 2 ? 2 : result;
        }

        #endregion
    }
}
