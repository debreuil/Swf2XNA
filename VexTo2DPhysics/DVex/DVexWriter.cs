using System;
using System.Text;
using System.Collections.Generic;

using DDW.Vex;

namespace DDW.DVex
{
	/// <summary>
	/// Summary description for AsDrawWriter.
	/// </summary>
	public class DVexWriter
	{
		private StringBuilder	    sb				= new StringBuilder(); // underlying store
		private	uint			    curNum			= 0;
		private	uint			    curBit			= 0x80000000;
		private	List<FillStyle>		p_fillDefs      = new List<FillStyle>();
		private	List<StrokeStyle>	p_strokeDefs    = new List<StrokeStyle>();
		StringBuilder			    fillSb          = new StringBuilder();
		
		//		string		localFills			= "AsdFills";
		//		string		localStrokes		= "AsdStrokes";
		//		string		globalFills			= "_global.AsdFills";
		//		string		globalStrokes		= "_global.AsdStrokes";

		public DVexWriter()
		{
            p_fillDefs.Add(new SolidFill(Color.Transparent));
            p_strokeDefs.Add(new SolidStroke(0f, Color.Transparent));
		}

		public void		WritePaths(IShapeData[][] ips)					
		{
			WriteStartArray();
			WriteBits((int)DVex.PathDefinition, 8);
			WriteBits(ips.Length, 11); // number of path defs
			for(int i = 0; i < ips.Length; i++)
			{
				WritePathBody(ips[i]);
			}
			
			FlushBits();
			WriteEndArray();
		}


		public void		WritePathBody(IShapeData[] ips)				
		{
			if(ips.Length < 1) return;
			// write header

			int count = 0;
            List<int> alVals = new List<int>();
            List<string> alTypes = new List<string>();
			Point prevPoint = Point.Empty;
			for(int i = 0; i < ips.Length; i++)
			{
				// figure out if it is a line, curve, or (needs a) move record
				// moveTo
				if(ips[i].StartPoint != prevPoint)
				{
					alTypes.Add("M");
                    alVals.Add((int)(ips[i].StartPoint.X * 20));
                    alVals.Add((int)(ips[i].StartPoint.Y * 20));
					count += 2;
				}
				// lineTo
				if(ips[i] is Line)
				{
					alTypes.Add("L");
                    alVals.Add((int)(ips[i].StartPoint.X * 20));
                    alVals.Add((int)(ips[i].StartPoint.Y * 20));
					count += 2;
				}

				// curveTo
				if(ips[i] is QuadBezier)
				{
					alTypes.Add("C");
                    QuadBezier qb = (QuadBezier)ips[i];
					alVals.Add((int)(qb.Control.X*20));
					alVals.Add((int)(qb.Control.Y*20));
					alVals.Add((int)(qb.Anchor1.X*20));
					alVals.Add((int)(qb.Anchor1.Y*20));
					count += 4;
				}
				prevPoint = ips[i].EndPoint;
			}
			int[] vals = new int[alVals.Count];
			for(int i = 0; i < alVals.Count; i++)
			{
				vals[i] = (int)alVals[i];
			}
			int maxBits = MinBits(vals) + 1; // sign
			// nBits
			WriteBits(2-2, 5); 
			// data count
			WriteBits(alTypes.Count, 11);
			// write out all type data first as it is even (line/curve/move)
			for(int i = 0; i < alTypes.Count; i++)
			{
				// L:0 C:1 M:2
				string st = (string)alTypes[i];
				int type = (st == "M") ? 2 : (st == "C") ? 0 : 1;
				WriteBits(type, 2);
			}

			// nBits
			WriteBits(maxBits-2, 5);
			// data count
			WriteBits(alVals.Count, 11);
			// write out int data nBits * (L2, C4, M2) for data)
			for(int i = 0; i < vals.Length; i++)
			{
				WriteBits(vals[i], maxBits);
			}
		}

		
		public void		WriteNbitColorDefs(SolidFill[] argbs)			
		{
			if(argbs.Length == 0) return;

			int count = argbs.Length;
			int[] vals = new int[argbs.Length];
			for(int i = 0; i < argbs.Length; i++)
			{
				FillDefs.Add(argbs[i]);
				vals[i] = argbs[i].Color.ARGB & 0x00FFFFFF;
				// alpha inverted so solid (common case) is zero
				vals[i] |= (~argbs[i].Color.A) << 24; 
			}
			int nBits = MinBits(vals); // always positive
			// header
			WriteStartArray();
			WriteBits((int)DVex.ArgbDefinitions, 8); // type 0x40

			int wnBits = nBits > 1 ? nBits - 2 : 0;
			WriteBits(wnBits, 5);
			WriteBits(count, 11); // 2074 max
			for(int i = 0; i < vals.Length; i++)
			{
				WriteBits(vals[i], wnBits+2);
			}
			FlushBits();
			WriteEndArray();
		}


		public void		WriteNbitStrokeDefs(SolidStroke[] strokes)			
		{
			if(strokes.Length == 0) return;

			WriteStartArray();
			WriteBits((int)DVex.StrokeDefinitions, 8); // type 0x40
			int count = strokes.Length;

			// Stroke Colors
			// first adjust argb color vals
			int[] cols = new int[count];
			for(int i = 0; i < count; i++)
			{
				// TODO: I think the stroke/fill defs aren't refed in this class
				StrokeDefs.Add(strokes[i]); 
				cols[i] = strokes[i].Color.ARGB & 0x00FFFFFF;
				// alpha inverted so solid (common case) is zero
				cols[i] |= (~strokes[i].Color.A) << 24; 
			}
			int nBits = MinBits(cols); // always positive

			int wnBits = nBits > 1 ? nBits - 2 : 0;
			WriteBits(wnBits, 5);
			WriteBits(count, 11); // 2074 max
			for(int i = 0; i < count; i++)
			{
				WriteBits(cols[i], wnBits+2);
			}

			// Stroke Widths in next sequence
			int[] widths = new int[count];
			for(int i = 0; i < count; i++)
			{
				StrokeDefs.Add(strokes[i]); 
				widths[i] = (int)(strokes[i].LineWidth * 20);
			}
			nBits = MinBits(widths); // always positive

			wnBits = nBits > 1 ? nBits - 2 : 0;
			WriteBits(wnBits, 5); 
			WriteBits(count, 11); // 2074 max
			for(int i = 0; i < count; i++)
			{
				WriteBits(widths[i], wnBits+2);
			}
			FlushBits();
			WriteEndArray();
		}


		public void		WriteNbitGradientDefs(GradientFill[] gfs)		
		{
			if(gfs.Length == 0) return;
			int count = gfs.Length;

			WriteStartArray();
			WriteBits((int)DVex.GradientDefs, 8);
			WriteBits(gfs.Length, 11); // note: no nBits here!!
			for(int index = 0; index < gfs.Length; index++)
			{
				FillDefs.Add(gfs[index]);
				// first type - all non radial will be solid (as as doesn do bmp fills)
				int type = (gfs[index].FillType == FillType.Radial) ? 1 : 0;
				WriteBits(type, 1);

				// now argb colors
				List<Color> cols = gfs[index].Fills;
				List<float> positions = gfs[index].Stops;
				// flash and gdi store colors & pos in opposite order for radials
				if(gfs[index].FillType == FillType.Radial)
				{
					int len = cols.Count;
                    List<Color> tempc = new List<Color>(len);
                    List<float> tempp = new List<float>(len);
					for(int col = 0; col < len; col++)
					{
						tempc[col] = cols[len-col-1];
						tempp[col] = 1-positions[len-col-1];
					}
					cols = tempc;
					positions = tempp;
				}

				int sampCount = cols.Count;
				if(sampCount > 8)
				{
					sampCount = 8;
					Console.WriteLine("*Flash only supports 8 colors max in gradients");
				}
				int[] wCols = new int[sampCount];
				for(int i = 0; i < sampCount; i++)
				{
					wCols[i] = cols[i].ARGB & 0x00FFFFFF;
					wCols[i] |= (~cols[i].A) << 24; 
				}
				int colBits = MinBits(wCols);
				int wcolBits = colBits > 1 ? colBits - 2 : 0;
				WriteBits(wcolBits, 5);
				WriteBits(sampCount, 11);
				for(int i = 0; i < sampCount; i++)
				{
					WriteBits(wCols[i], wcolBits+2);
				}

				// now ratios

				int[] rats = new int[positions.Count];
				for(int i = 0; i < sampCount; i++)
				{
					rats[i] = (int)(positions[i]*255);
				}
				int ratBits = MinBits(rats);
				int wratBits = ratBits > 1 ? ratBits - 2 : 0;
				WriteBits(wratBits, 5);
				WriteBits(sampCount, 11);
				for(int i = 0; i < sampCount; i++)
				{
					WriteBits(rats[i], wratBits+2);
				}

				// now matrix
				System.Drawing.Drawing2D.Matrix clone = gfs[index].Transform.GetDrawing2DMatrix();
                clone.Scale(GradientFill.GradientVexRect.Size.Width, GradientFill.GradientVexRect.Size.Height);
				float[] mx = clone.Elements;
				clone.Dispose();

				int[] mxs = new int[6]{	(int)mx[0]*20, (int)mx[1]*20, (int)mx[2]*20, 
										(int)mx[3]*20, (int)mx[4]*20, (int)mx[5]*20};
				int mxBits = MinBits(mxs)+1; // neg
				int wmxBits = mxBits > 1 ? mxBits - 2 : 0;
				WriteBits(wmxBits, 5);
				WriteBits(6, 11);
				for(int i = 0; i < 6; i++)
				{
					WriteBits(mxs[i], wmxBits+2);
				}
			}
			FlushBits();
			WriteEndArray();
		}


		public void		WriteGradientColorDefs(GradientFill[] gfs)		
		{
			if(gfs.Length == 0) return;
			int count = gfs.Length;
			int totalBytes = 4;
			for(int i = 0; i < gfs.Length; i++)
			{
				int samples = gfs[i].Fills.Count;
				totalBytes += (samples * 4) + (samples * 1); // rgba ratio
				totalBytes += 1 + 6*2; // header and matrix
			}
			byte[] bytes = new byte[totalBytes]; // id8, count16
			bytes[0] = (byte)DVex.GradientDefs;
			bytes[1] = 0;
			bytes[2] = (byte)((count & 0xFF00) >> 8);
			bytes[3] = (byte)(count & 0xFF);
			int index = 4;
			for(int i = 0; i < gfs.Length; i++)
			{
				int fillType = 0x00; // linearFill
				if(gfs[i].FillType == FillType.Radial)
				{
					fillType = 0x10;
				}
                int colorCount = gfs[i].Fills.Count;
				if(colorCount > 8)
				{
					colorCount = 8;
					Console.WriteLine("*Flash only supports 8 colors max in gradients");
				}
				bytes[index++] = (byte)(fillType | colorCount);

				// add rgba+ratio  array
				List<Color> colors = gfs[i].Fills;
				List<float> positions = gfs[i].Stops;
				// flash and gdi store colors & pos in opposite order for radials
				if(gfs[i].FillType == FillType.Radial)
				{
					int len = colors.Count;
                    List<Color> tempc = new List<Color>(len);
                    List<float> tempp = new List<float>(len);
					for(int col = 0; col < len; col++)
					{
						tempc[col] = colors[len-col-1];
						tempp[col] = 255-positions[len-col-1];
					}
					colors = tempc;
					positions = tempp;
				}
				for(int j = 0; j < colorCount; j++)
				{
					bytes[index++] = colors[j].R;
					bytes[index++] = colors[j].G;
					bytes[index++] = colors[j].B;
					int a = (int)(Math.Floor(colors[j].A / 2.55));
					if (a >= 98) a = 100;
					if (a <= 2) a = 0;
					bytes[index++] = (byte)a;
					bytes[index++] = (byte) ( ((int)(positions[j] * 255)) & 0xFF);
				}

				// add matrix
			System.Drawing.Drawing2D.Matrix clone = gfs[i].Transform.GetDrawing2DMatrix();
                clone.Scale(GradientFill.GradientVexRect.Size.Width, GradientFill.GradientVexRect.Size.Height);
				float[] mx = clone.Elements;
				clone.Dispose();

				// add elements
				bytes[index++] = (byte) ((((int)mx[0])& 0xFF00) >> 8);
				bytes[index++] = (byte) ( ((int)mx[0])& 0xFF);
				bytes[index++] = (byte) ((((int)mx[1])& 0xFF00) >> 8);
				bytes[index++] = (byte) ( ((int)mx[1])& 0xFF);
				bytes[index++] = (byte) ((((int)mx[2])& 0xFF00) >> 8);
				bytes[index++] = (byte) ( ((int)mx[2])& 0xFF);
				bytes[index++] = (byte) ((((int)mx[3])& 0xFF00) >> 8);
				bytes[index++] = (byte) ( ((int)mx[3])& 0xFF);

				bytes[index++] = (byte) ((((int)mx[4])& 0xFF00) >> 8);
				bytes[index++] = (byte) ( ((int)mx[4])& 0xFF);
				bytes[index++] = (byte) ((((int)mx[5])& 0xFF00) >> 8);
				bytes[index++] = (byte) ( ((int)mx[5])& 0xFF);

				FillDefs.Add(gfs[i]);
			}
			WriteByteArray(bytes);
		}


		public void		WriteUses(ShapeRecord[] srs)					
		{
			WriteStartArray();
			uint val = (uint)DVex.InsertPath;
			WriteBits((int)val, 8);

			int maxBits = 0;
			for(int i = 0; i < srs.Length; i++)
			{
				int minBits = MinBits((uint)Math.Abs(srs[i].Path));
				if(minBits > maxBits) maxBits = minBits;
				minBits = MinBits((uint)Math.Abs(srs[i].Fill));
				if(minBits > maxBits) maxBits = minBits;
				minBits = MinBits((uint)Math.Abs(srs[i].Stroke));
				if(minBits > maxBits) maxBits = minBits;
			}
			maxBits++; // sign  - indexes always positive though...
			// nBits
			WriteBits(maxBits-2, 5);
			// data count
			WriteBits(srs.Length*3, 11);

			for(int i = 0; i < srs.Length; i++)
			{
				WriteBits(srs[i].Path, maxBits);
				WriteBits(srs[i].Fill, maxBits);
				WriteBits(srs[i].Stroke, maxBits);
			}
			FlushBits();
			WriteEndArray();
		}


		/// <summary>
		/// Minimum bits to represent an unsigned number.
		/// </summary>
		private void		WriteBits(int num, int nBits)				
			//private void WriteBits(int num, int count)
		{
			// curBit is the bit about to be written to
			// curNum is the number being created
			// WriteNum(uint) writes a number to the 'stream'
			for(int i = nBits-1; i >= 0; i--)
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
					WriteNum(curNum);
					curBit = 0x80000000;
					curNum = 0;
				}
			}
		}
		private void		FlushBits()									
		{
			if(curBit != 0x80000000)
			{
				WriteNum(curNum);
			}
			curBit = 0x80000000;
			curNum = 0;
		}
		private void		WriteByteArray(byte[] bytes)				
		{			
			WriteStartArray();
			int writeVal = 0;
			int i = 0;
			for(; i < bytes.Length; i++)
			{
				writeVal |= bytes[i] << (3 - (i%4)) * 8;
				if(i % 4 == 3)
				{
					WriteNum((uint)writeVal);
					//sb.Append(writeVal.ToString("D") + ", ");
					writeVal = 0;
				}
			}
			// write last bytes, if any - check this
			if(i % 4 != 0)
			{
				WriteNum((uint)writeVal);
				//sb.Append(writeVal.ToString("D") + ", ");
			}
			WriteEndArray();
		}

		private int			MinBits( uint val )							
		{
			if(val == 0) return 1;
			uint mask = 1;
			int nbits;
			for(nbits = 1; nbits < 32; nbits++)
			{
				mask <<= 1;
				if(mask > val) break;
			}
			return nbits;
		}

		private int			MinBits(int[] ints)							
		{
			int min = 0;
			for(int i = 0; i < ints.Length; i++)
			{
				int curMin = MinBits((uint)ints[i]);
				if(curMin > min) min = curMin;
			}
			return min;
		}

		private void		WriteNum(uint num)							
		{
			//int iNum = (int)num;
			//sb.Append(iNum.ToString("D") + ", ");
			if((num & 0x80000000) > 0)
			{
				sb.Append("-0x" + ((~num) + 1).ToString("X8") + ",");
			}
			else
			{
				sb.Append(" 0x" + (num & 0x7FFFFFFF).ToString("X8") + ",");
			}
		}
		private void		WriteStartArray()							
		{
			sb.Append("\n[");
		}
		private void		WriteEndArray()								
		{
			if(sb.ToString().EndsWith(",")) sb.Remove(sb.Length - 1, 1);
			sb.Append("],");
		}
		public string		Results										
		{
			get
			{
				// get rid of any trailing commas
				sb.Replace(",","\n",sb.Length - 1, 1);
				return fillSb.ToString() + sb.ToString();
			}
		}
		public List<FillStyle>	FillDefs									
		{
			get
			{
				return p_fillDefs;
			}
		}
		public List<StrokeStyle>	StrokeDefs									
		{
			get
			{
				return p_strokeDefs;
			}
		}
	}
}
