/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using DDW.Vex;

namespace DDW.Swf
{
	public class StrokePath : IComparable
	{
		public StrokeStyle StrokeStyle;
		public IShapeData Segment;

		public StrokePath(StrokeStyle strokeStyle, IShapeData segment)
		{
			this.StrokeStyle = strokeStyle;
			this.Segment = segment;
		}

		/// <summary>
		/// Converts a number of outline segments to a series of shapes, one per strokestyle.
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		public static List<DDW.Vex.Shape> ConvertToShapes(List<StrokePath> paths)
		{
			List<DDW.Vex.Shape> result = new List<DDW.Vex.Shape>();
			if (paths == null || paths.Count == 0)
			{
				return result;
			}

			List<StrokePath> curPaths = new List<StrokePath>();
			StrokeStyle curStrokeStyle = paths[0].StrokeStyle;

			for (int i = 0; i < paths.Count; i++)
			{
				if (!paths[i].StrokeStyle.Equals(curStrokeStyle))
				{
					result.Add(ConsolidatePaths(curPaths));
					curPaths.Clear();
					curStrokeStyle = paths[i].StrokeStyle;
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
		private static DDW.Vex.Shape ConsolidatePaths(List<StrokePath> ps)
		{
			// don't destroy org path
			//List<StrokePath> ps = new List<StrokePath>(paths);

			DDW.Vex.Shape result = new DDW.Vex.Shape();
			if (ps.Count == 0)
			{
				return result;
			}
			result.Stroke = ps[0].StrokeStyle;

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
					if (ps[i].Segment.StartPoint.Equals(curEnd) || ps[i].Segment.EndPoint.Equals(curEnd))
					{
						StrokePath fp = ps[i];
						ps.RemoveAt(i);

						// in this case, the segment is backwards (this can happen in swf)
						if (fp.Segment.EndPoint.Equals(curEnd))
						{
							fp.Segment.Reverse();
						}
						rs.Add(fp.Segment);
						curEnd = fp.Segment.EndPoint;

						hasMatch = true;
						if (curEnd == curStart)
						{
							hasMatch = false;
						}
						break;
					}
				}
				if (hasMatch == false && ps.Count > 0)
				{
					curStart = ps[0].Segment.StartPoint;
					curEnd = ps[0].Segment.EndPoint;
					rs.Add(ps[0].Segment);
					ps.RemoveAt(0); 
					hasMatch = true;
				}
			}

			return result;
		}


		///// <summary>
		///// Converts a number of outline segments to a series of shapes, one per strokestyle.
		///// </summary>
		///// <param name="paths"></param>
		///// <returns></returns>
		//public static List<Shape> ConvertToShapes(List<StrokePath> paths)
		//{
		//    List<Shape> result = new List<Shape>();
		//    if (paths == null || paths.Count == 0)
		//    {
		//        return result;
		//    }

		//    List<StrokePath> curPaths = new List<StrokePath>();
		//    StrokeStyle curStrokeStyle = paths[0].StrokeStyle;

		//    for (int i = 0; i < paths.Count; i++)
		//    {
		//        if (paths[i].StrokeStyle == curStrokeStyle)
		//        {
		//            curPaths.Add(paths[i]);
		//        }
		//        else
		//        {
		//            result.Add(ConsolidatePaths(curPaths));
		//            curPaths.Clear();
		//            curPaths.Add(paths[i]);
		//            curStrokeStyle = paths[i].StrokeStyle;
		//        }
		//    }

		//    result.Add(ConsolidatePaths(curPaths));
		//    curPaths.Clear();

		//    return result;
		//}
		///// <summary>
		///// The input strokes are all of the same stroke style, we put them tip to tail here, and discard dups.
		///// </summary>
		///// <param name="paths"></param>
		///// <returns></returns>
		//private static Shape ConsolidatePaths(List<StrokePath> paths)
		//{
		//    Shape result = new Shape();
		//    if (paths.Count == 0)
		//    {
		//        return result;
		//    }
		//    result.Stroke = paths[0].StrokeStyle;

		//    List<IShapeData> rs = result.ShapeData;

		//    StrokePath prev = null;
		//    for (int i = 0; i < paths.Count; i++)
		//    {
		//        StrokePath p = paths[i];
		//        if (p == prev)
		//        {
		//            continue;
		//        }
		//        prev = p;
		//        bool hasInsertion = false;
		//        for (int j = 0; j < rs.Count; j++)
		//        {
		//            if (rs[j].StartPoint == p.Segment.EndPoint)
		//            {
		//                rs.Insert(j, p.Segment);
		//                hasInsertion = true;
		//                break;
		//            }
		//        }
		//        if (!hasInsertion)
		//        {
		//            rs.Add(p.Segment);
		//        }				
		//    }

		//    return result;
		//}

		public int CompareTo(Object o)
		{
			int result = 0;
			if (o is StrokePath)
			{
				StrokePath co = (StrokePath)o;
				if (this.StrokeStyle != co.StrokeStyle)
				{
					result = this.StrokeStyle.CompareTo(co.StrokeStyle);
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
			return this.StrokeStyle + " : " + this.Segment;
		}
	}
}

