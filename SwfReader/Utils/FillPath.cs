/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using DDW.Vex;

namespace DDW.Swf
{
	public class FillPath : IComparable
	{
		public FillStyle FillStyle;
		public IShapeData Segment;

		public FillPath(FillStyle fillStyle, IShapeData segment)
		{
			this.FillStyle = fillStyle;
			this.Segment = segment;
		}


		/// <summary>
		/// Converts a number of outline segments to a series of shapes, one per strokestyle.
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		public static List<DDW.Vex.Shape> ConvertToShapes(List<FillPath> paths)
		{
			List<DDW.Vex.Shape> result = new List<DDW.Vex.Shape>();
			if (paths == null || paths.Count == 0)
			{
				return result;
			}

			List<FillPath> curPaths = new List<FillPath>();
			//List<List<FillPath>> allPaths = new List<List<FillPath>>();
			//allPaths.Add(curPaths);

			FillStyle curFillStyle = paths[0].FillStyle;

			for (int i = 0; i < paths.Count; i++)
			{
				if (paths[i].FillStyle != curFillStyle)
				{
					result.Add(ConsolidatePaths(curPaths));
					curPaths.Clear(); 
					curFillStyle = paths[i].FillStyle;
				}
				curPaths.Add(paths[i]);
			}
			result.Add(ConsolidatePaths(curPaths));

			return result;
		}
		/// <summary>
		/// The input strokes are all of the same stroke style, we put them tip to tail here, and discard dups.
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		private static DDW.Vex.Shape ConsolidatePaths(List<FillPath> ps)
		{
			// don't destroy org path
			//List<FillPath> ps = new List<FillPath>(paths);

			DDW.Vex.Shape result = new DDW.Vex.Shape();
			if (ps.Count == 0)
			{
				return result;
			}
			result.Fill = ps[0].FillStyle;


			List<IShapeData> rs = result.ShapeData;

			rs.Add(ps[0].Segment);
			Point curStart = ps[0].Segment.StartPoint;
			Point curEnd = ps[0].Segment.EndPoint;
			ps.RemoveAt(0);

			bool hasMatch = true;
			while (hasMatch && ps.Count > 0)
			{
				hasMatch = false;
				for (int i = 0; i < ps.Count; i++)
				{
					if (ps[i].Segment.StartPoint == curEnd || ps[i].Segment.EndPoint == curEnd)
					{			
						FillPath fp = ps[i];
						ps.RemoveAt(i);

						// in this case, the segment is backwards (this can happen in swf)
						if (fp.Segment.EndPoint == curEnd)
						{
							fp.Segment.Reverse();
						}
						rs.Add(fp.Segment);
						curEnd = fp.Segment.EndPoint;

						hasMatch = true;
						if (curEnd == curStart)
						{
							if (ps.Count > 0)
							{
								curStart = ps[0].Segment.StartPoint;
								curEnd = ps[0].Segment.EndPoint;
								rs.Add(ps[0].Segment);
								ps.RemoveAt(0);
							}
							else
							{
								hasMatch = false;
							}
						}

						break;
					}
				}
			}

            result.CalcuateBounds();

			return result;
		}


		public int CompareTo(Object o)
		{
			int result = 0;
			if (o is FillPath)
			{
				FillPath co = (FillPath)o;
				if (this.FillStyle != co.FillStyle)
				{
					result = this.FillStyle.CompareTo(co.FillStyle);
				}
				else if (this.Segment != co.Segment)
				{
					result = this.Segment.CompareTo(co.Segment);
				}
			}
			else
			{
				throw new ArgumentException("Objects being compared are not of the same type");
			}
			return result;
		}

		public override string ToString()
		{
			return this.FillStyle + " : " + this.Segment;
		}
	}
}
