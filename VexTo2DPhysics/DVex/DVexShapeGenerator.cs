#region Using...
using System;
using System.Text;
using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
using System.Collections;

using DDW.Vex;
#endregion
namespace DDW.DVex
{
	public class AsDrawShapeGenerator
	{
		public AsDrawShapeGenerator()								
		{
		}

        public static string ShapeToAsDraw(Symbol symbol)	
		{
            string s = ShapeToAsDraw(symbol, "");
			return s;
		}

        public static string ShapeToAsDraw(Symbol symbol, string filename)					
		{
			DVexWriter	writer				= new DVexWriter();
            ConsolidatePaths(symbol, writer);
			return writer.Results.ToString();
		}


        private static void ConsolidatePaths(Symbol symbol, DVexWriter writer)		
		{
            List<FillStyle> fills = new List<FillStyle>();
            List<StrokeStyle> strokes = new List<StrokeStyle>();
			fills.Add( new SolidFill(Color.Transparent) );
			strokes.Add( new SolidStroke(0.0F, Color.Transparent) );
			ArrayList allPaths = new ArrayList();
			ArrayList allSrs = new ArrayList();

			// Find all used colors/strokes, and the F0,F1,S info for each seg
			foreach(Shape sh in symbol.Shapes)
			{
				foreach(IShapeData s in sh.ShapeData)
				{
					int fill = 0;
					int stroke = 0;
                    if (!fills.Contains(shape.Fills[s.FillIndex])) 
					{
						fill = fills.Add(shape.Fills[s.FillIndex]);
					}
					else
					{
						fill = fills.IndexOf(shape.Fills[s.FillIndex]);
					}
					if( !strokes.Contains(shape.Strokes[s.StrokeIndex]) )	
					{
						stroke = strokes.Add(shape.Strokes[s.StrokeIndex]);
					}
					else
					{
						stroke = strokes.IndexOf(shape.Strokes[s.StrokeIndex]);
					}
					// break path into shape records
					foreach(IPathPrimitive ipp in s.Path)
					{
						if(ipp is IShapeData)
						{
							IShapeData ip = (IShapeData)ipp;
							if(allPaths.Contains(ip))
							{
								// this must be a fill1 if it is a dup
								int index = allPaths.IndexOf(ip);
								Shrec sr = (Shrec)allSrs[index];
								Shrec newShrec = new Shrec(0, 0);
								newShrec.F0 = (sr.F0 == 0) ? fill : sr.F0 ;
								newShrec.F1 = (sr.F1 == 0) ? fill : sr.F1 ;
								newShrec.S = (sr.S == 0) ? stroke : sr.S ;
								allSrs[index] = newShrec;
							}
							else
							{
								allSrs.Add(new Shrec(fill, stroke));
								allPaths.Add(ip);
							}
						}
					}
				} // end groups
			} // end shapes


			// ok, now write out colors
			// sort fills by rgb, argb, and gradients
			ArrayList orderedFills = new ArrayList();
			ArrayList rgbas = new ArrayList();
			ArrayList gfs = new ArrayList();
			foreach(Fill sf in fills)
			{
				if(sf is SolidFill)
				{
					if( ((SolidFill)sf).Color.A == 255 ||
						(SolidFill)sf == fills[0]) // 'no fill'
					{
						orderedFills.Add(sf);
					}
					else
					{
						rgbas.Add(sf);
					}
				}
				else if(sf is GradientFill)
				{
					gfs.Add(sf);
				}
				else
				{
					// bitmap fills
					orderedFills.Add(new SolidFill(Color.Gray));
				};
			}
			
			SolidFill[] wrgbs = new SolidFill[orderedFills.Count];
			wrgbs[0] = new SolidFill(Color.FromArgb(255,0,0,0));
			int fRgb = 1;
			foreach(Fill f in orderedFills)
			{
				if(f != fills[0])
				{
					wrgbs[fRgb++] = (SolidFill)f;
				}
			}
			int fRgba = 0;
			SolidFill[] wrgbas = new SolidFill[rgbas.Count];
			foreach(Fill f in rgbas)
			{
				orderedFills.Add(f);
				wrgbas[fRgba++] = (SolidFill)f;
			}

			int fGr = 0;
			GradientFill[] wgfs = new GradientFill[gfs.Count];
			foreach(Fill f in gfs)
			{
				orderedFills.Add(f);
				wgfs[fGr++] = (GradientFill)(f);
			}

			writer.WriteNbitColorDefs(wrgbs);
			writer.WriteNbitColorDefs(wrgbas);
			writer.WriteNbitGradientDefs(wgfs);
			//writer.WriteRgbColorDefs(wrgbs);
			//writer.WriteRgbaColorDefs(wrgbas);
			//writer.WriteGradientColorDefs(wgfs);



			// ok, colors written, now strokes
			// write out all the stroke defs second
			// get counts
			int wrgbCount = 0;
			int wrgbaCount = 0;
			foreach(Stroke st in strokes)
			{
				if(st.Color.A == 255 || st == strokes[0])
					{wrgbCount++;}
					else{wrgbaCount++;}
			}
			// create stroke arrays
			Stroke[] wsrgbs = new Stroke[wrgbCount];
			Stroke[] wsrgbas = new Stroke[wrgbaCount];
			int sRgb = 0;
			int sRgba = 0;
			foreach(Stroke st in strokes)
			{
				if( st.Color.A == 255 || st == strokes[0])
				{
					wsrgbs[sRgb++] = st;
				}
				else 
				{
					wsrgbas[sRgba++] = st;
				}
			}
			// now write the stroke data
			writer.WriteNbitStrokeDefs(wsrgbs);
			writer.WriteNbitStrokeDefs(wsrgbas);
			//writer.WriteRgbStrokeDefs(wsrgbs);
			//writer.WriteRgbaStrokeDefs(wsrgbas);


			// and now paths
			// valid pathsegs must have the same F0, F1, and S
			ArrayList tempPaths = new ArrayList();
			ArrayList tempSrsAl = new ArrayList();
			PathCollection pc = new PathCollection();
			Shrec curShrec = Shrec.Empty;
			for(int i = 0; i < allSrs.Count; i++) //Shrec sr in srsAl)
			{
				Shrec sr = (Shrec)allSrs[i];
				if(sr.Equals(curShrec) || curShrec.Equals(Shrec.Empty))
				{
					//add to path
					pc.Add((IShapeData)allPaths[i]);
				}
				else
				{
					// write to hash
					tempPaths.Add(pc);
					tempSrsAl.Add(curShrec);

					pc = new PathCollection();
					pc.Add((IShapeData)allPaths[i]);
				}
				curShrec = sr;
			}
			if(!tempSrsAl.Contains(curShrec))
			{
				tempPaths.Add(pc);
				tempSrsAl.Add(curShrec);
			}
			// split non contig paths
			ArrayList paths = new ArrayList();
			ArrayList srsAl = new ArrayList();
			foreach(PathCollection pcoll in tempPaths)
			{
				//pcoll.ReorderPath(); 
				PathCollection[] pcolls = pcoll.SplitPath();
				foreach(PathCollection splitP in pcolls)
				{
					paths.Add(splitP);
					srsAl.Add(tempSrsAl[tempPaths.IndexOf(pcoll)] );
					//writer.WritePath(splitP.PointSegments);
				}
			}
			IShapeData[][] ips = new IShapeData[paths.Count][];
			for(int i = 0; i < paths.Count; i++)
			{
				ips[i] = ((PathCollection)paths[i]).PointSegments;
			}
			writer.WritePaths(ips);

			
			// convert to array
			Shrec[] srs = new Shrec[srsAl.Count];
			for(int i = 0; i < srsAl.Count; i++)
			{
				srs[i] = (Shrec)srsAl[i];
			}

			// and finally, uses - must be sorted by fill color
			// use order Fill1 (no strokes), fill0[stroke], stroke only's
			// for each fill index{..}, then dangling strokes

			ArrayList shapeRecords = new ArrayList();

			// start at 1 to avoid empty fills
			foreach(Fill f in orderedFills)
			{
				int curFill = fills.IndexOf(f); 
				if(curFill != 0)
				{
					// all F1's of this color first
					ArrayList Fs = new ArrayList();
					for(int i = 0; i < srs.Length; i++)
					{
						if(srs[i].F0 == curFill)
						{
							// add use for F0
							ShapeRecord curSr = new ShapeRecord();

							curSr.Fill = orderedFills.IndexOf(f);
							curSr.Stroke = srs[i].S;
							curSr.Path = i;							
							Fs.Add(curSr);
						}
						if(srs[i].F1 == curFill )
						{
							// add use for F1
							ShapeRecord curSr = new ShapeRecord();
							curSr.Fill = orderedFills.IndexOf(f);
							curSr.Stroke = 0;
							curSr.Path = i;
							Fs.Add(curSr);
						}
					}
					//now sort the F1s from tip to tail
					if(Fs.Count > 0)
					{						
						ArrayList finalFs = new ArrayList();
						finalFs.Add(Fs[0]);
						PointF end = 
							((PathCollection)paths[((ShapeRecord)Fs[0]).Path]).LastPoint;
						Fs.RemoveAt(0);
						while(Fs.Count > 0)
						{
							bool found = false;
							foreach(ShapeRecord sr in Fs)
							{
								PathCollection srp = (PathCollection)paths[sr.Path];
								if(srp.FirstPoint == end)
								{
									end = srp.LastPoint;
									finalFs.Add(sr);
									Fs.Remove(sr);
									found = true;
									break;
								}
							}
							if(found == false)
							{
								finalFs.Add(Fs[0]);
								end = ( (PathCollection)paths[
									((ShapeRecord)Fs[0]).Path] ).LastPoint;
								Fs.RemoveAt(0);
							}
						}
						// and write them
						foreach(ShapeRecord sr in finalFs)
						{
							shapeRecords.Add(sr);
						}
					}

				}
			}
			for(int i = 0; i < srs.Length; i++)
			{
				if(srs[i].F0 == 0 && srs[i].F1 == 0)
				{
					// must be stroke
					ShapeRecord curSr = new ShapeRecord();
					curSr.Fill = 0;
					curSr.Stroke = srs[i].S;
					curSr.Path = i;
					shapeRecords.Add(curSr);
				}
			}
			// convert to array
			ShapeRecord[] srecs = new ShapeRecord[shapeRecords.Count];
			for(int i = 0; i < shapeRecords.Count; i++)
			{
				srecs[i] = (ShapeRecord)shapeRecords[i];
			}

			writer.WriteUses(srecs);
		}

		private static int Twips(float f)					
		{
			return (int)(f * 20);
		}

	}

	public struct Shrec								
	{
		private int		p_fill0;
		private int		p_fill1;
		private int		p_stroke;
		public Shrec(int fill0Index, int shapeIndex)
		{
			p_fill0 = fill0Index;
			p_stroke = shapeIndex;
			p_fill1 = 0;
		}
		public int		F0		
		{
			get
			{
				return p_fill0;
			}
			set
			{
				p_fill0 = value;
			}
		}
		public int		F1		
		{
			get
			{
				return p_fill1;
			}
			set
			{
				p_fill1 = value;
			}
		}
		public int S			
		{
			get
			{
				return p_stroke;
			}
			set
			{
				p_stroke = value;
			}
		}
		public static Shrec Empty
		{
			get
			{
				return new Shrec(-99, -99);
				}
		}
		public override string ToString()
		{
			return "f0: "+p_fill0+" f1: "+p_fill1+" s: "+p_stroke;
		}
	}






	public struct ShapeRecord								
	{
		private int		p_path;
		private int		p_fill;
		private int		p_stroke;
		public int		Path		
		{
			get
			{
				return p_path;
			}
			set
			{
				p_path = value;
			}
		}
		public int		Fill		
		{
			get
			{
				return p_fill;
			}
			set
			{
				p_fill = value;
			}
		}
		public int Stroke			
		{
			get
			{
				return p_stroke;
			}
			set
			{
				p_stroke = value;
			}
		}
	}
}
