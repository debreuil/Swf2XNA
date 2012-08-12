using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DDW.Swf;

namespace DDW.SwfRenderer
{
    public class SwfRenderer
    {
        public const int TWIP = 20;

        public SwfRenderer()
        {
        }

        public List<Bitmap> GenerateBitmaps(SwfCompilationUnit scu)
        {
            List<Bitmap> result = new List<Bitmap>();
            Dictionary<uint, Bitmap> dbmp = GenerateMappedBitmaps(scu);
            result.AddRange(dbmp.Values);
            return result;
        }

        public Dictionary<uint, Bitmap> GenerateMappedBitmaps(SwfCompilationUnit scu)
        {
            Dictionary<uint, Bitmap> result = new Dictionary<uint, Bitmap>();

            foreach (ISwfTag tag in scu.Tags)
            {
                Edge[] edges = null;
                if (tag is DefineShape2Tag)
                {
                    DefineShape2Tag def = (DefineShape2Tag)tag;
                    edges = ParseEdges(def.Shapes, def.ShapeBounds);
                    Bitmap b = DrawShape(def.Shapes, edges, def.ShapeBounds);
                }
                else if (tag is DefineShape3Tag)
                {
                    DefineShape3Tag def = (DefineShape3Tag)tag;
                    edges = ParseEdges(def.Shapes, def.ShapeBounds);
                    Bitmap b = DrawShape(def.Shapes, edges, def.ShapeBounds);
                }
                if (tag is DefineShape4Tag)
                {
                    DefineShape4Tag def = (DefineShape4Tag)tag;
                    edges = ParseEdges(def.Shapes, def.ShapeBounds);
                    Bitmap b = DrawShape(def.Shapes, edges, def.ShapeBounds);
                }

                if (edges != null)
                {
                    Program.form.edges = edges;
                    break;
                }
            }
            return result;
        }

        Bitmap DrawShape(ShapeWithStyle s, Edge[] edges, Rect r)
        {
            int top = r.YMin / TWIP;
            int left = r.XMin / TWIP;
            int w = (r.XMax - r.XMin) / TWIP;
            int h = (r.YMax - r.YMin) / TWIP;

            colors.Clear();
            colors.Add(new PixelData(0xFF, 0xFF, 0xFF, 0x00));
            foreach (IFillStyle fs in s.FillStyles.FillStyles)
            {
                if (fs.FillType == FillType.Solid)
                {
                    RGBA c = ((SolidFill)fs).Color;
                    colors.Add(new PixelData(c.R, c.G, c.B, c.A));
                }
                else
                {
                    colors.Add(new PixelData(0x80, 0x80, 0x80, 0xFF));
                }
            }
            //UnsafeBitmapP b = new UnsafeBitmapP(w, h);
            UnsafeBitmap b = new UnsafeBitmap(w + left, h + top);
            b.LockBitmap();

            List<Edge> activeEdges = new List<Edge>();
            int edgeIndex = 0;
            byte[,] bytes = new byte[w, h];

            bool orderChanged = true;

            int syTwip = 0;
            for (int sy = 0; sy < h + top; sy++)
            {
                syTwip += TWIP;

                //AdjustActiveTable
                //remove stale
                for(int i = activeEdges.Count - 1; i >= 0; i--)
                {
                    if (activeEdges[i].endY < sy * TWIP)
                    {
                        activeEdges.RemoveAt(i);
                    }
                }

                // add new elements
                while (edgeIndex < edges.Length && edges[edgeIndex].startY <= sy * TWIP)
                {
                    activeEdges.Add(edges[edgeIndex]);
                    edgeIndex++;
                    orderChanged = true;
                }

                // sort
                if(orderChanged)
                {
                    activeEdges.Sort(Edge.CurrentXComparer);
                    orderChanged = false;
                }

                if (activeEdges.Count > 0)
                {
                    int xIndex = 0;
                    byte curFill = 0;
                    Edge edge = activeEdges[xIndex];
                    PixelData col = colors[0];
                    PixelData nextCol = colors[0];
                    int sxTwip = 0;
                    float pc = 0;
                    
                    for (int sx = 0; sx < w + left; sx++)
                    {
                        sxTwip += TWIP;
                        if (sxTwip >= edge.currentX)
                        {
                            pc = (edge.currentX % TWIP) / TWIP;
                            if ((int)edge.currentX != edge.startX)
                            {
                                nextCol = colors[edge.fill0];
                                col.Interpolate(pc, nextCol);
                            }

                            while (sxTwip >= edge.currentX)
                            {
                                xIndex++;
                                if (xIndex < activeEdges.Count)
                                {
                                    if (xIndex > 0 && activeEdges[xIndex - 1].currentX > activeEdges[xIndex].currentX)
                                    {
                                    }
                                    else
                                    {
                                        edge = activeEdges[xIndex];
                                        curFill = edge.fill1;
                                    }
                                }
                                else
                                {
                                    b.SetPixel(sx, sy, col);
                                    goto ENDXLOOP;
                                }
                            }
                        }

                        if (sy == 80)
                        {
                            //col.blue = 255;
                        }

                        if (col.alpha > 0)
                        {
                            b.SetPixel(sx, sy, col);
                        }

                        col = colors[curFill];
                    }
                }
            ENDXLOOP:

                for (int i = 0; i < activeEdges.Count; i++)
                {
                    activeEdges[i].IncX(syTwip);
                }
            }


            b.UnlockBitmap();
            Program.form.Bitmap = b.Bitmap;
           // b.Bitmap.Save("test.png");

            return null;
        }

        List<PixelData> colors = new List<PixelData>();

        Edge[] ParseEdges(ShapeWithStyle s, Rect bounds)
        {
            List<Edge> edges = new List<Edge>();

            uint f0 = 0;
            uint f1 = 0;
            uint ln = 0;
            int curX = -bounds.XMin; // always 0,0
            int curY = -bounds.YMin;

            FillStyleArray curFills = s.FillStyles;
            LineStyleArray curLines = s.LineStyles;
            int fillOffset = 0;
            int lineOffset = 0;

            foreach (IShapeRecord rec in s.ShapeRecords)
            {

                if (rec is StyleChangedRecord)
                {
                    StyleChangedRecord r = (StyleChangedRecord)rec;
                    if (r.HasNewStyles) // todo: this also sets 'grouped' elements (and therefore layers)
                    {
                        fillOffset = curFills.FillStyles.Count;
                        lineOffset = curLines.LineStyles.Count;
                        curFills.FillStyles.AddRange(r.FillStyles.FillStyles);
                        curLines.LineStyles.AddRange(r.LineStyles.LineStyles);
                    }

                    if (r.HasFillStyle0)
                    {
                        f0 = r.FillStyle0 == 0 ? 0 : (uint)(r.FillStyle0 + fillOffset); 
                    }

                    if (r.HasFillStyle1)
                    {
                        f1 = r.FillStyle1 == 0 ? 0 : (uint)(r.FillStyle1 + fillOffset);
                    }

                    if (r.HasLineStyle)
                    {
                        ln = r.LineStyle == 0 ? 0 : (uint)(r.LineStyle + lineOffset);
                    }

                    if(r.HasMove)
                    {
                        curX = r.MoveDeltaX; // this is based on the initial location, which is now 0,0
                        curY = r.MoveDeltaY;
                    }
                }
                else if (rec is StraightEdgeRecord)
                {
                    StraightEdgeRecord r = (StraightEdgeRecord)rec;
                    Edge e = new Edge(curX, curY, curX + r.DeltaX, curY + r.DeltaY, (byte)f0, (byte)f1, (byte)ln);
                    curX += r.DeltaX;
                    curY += r.DeltaY;

                    edges.Add(e);
                }
                else if (rec is CurvedEdgeRecord)
                {
                    CurvedEdgeRecord r = (CurvedEdgeRecord)rec;
                    int curveX = curX + r.ControlX;
                    int curveY = curY + r.ControlY;
                    int destX = curveX + r.AnchorX;
                    int destY = curveY + r.AnchorY;

                    // split nodes if curve is outside start/end Y
                    if(curveY >= curY && curveY <= destY)
                    {
                        Edge e = new CurvedEdge(curX, curY, curveX, curveY, destX, destY, (byte)f0, (byte)f1, (byte)ln);
                        edges.Add(e);
                    }
                    else
                    {
                        Edge[] split = CurvedEdge.SplitAtPeak(curX, curY, curveX, curveY, destX, destY, (byte)f0, (byte)f1, (byte)ln);
                        edges.Add(split[0]);
                        edges.Add(split[1]);
                    }

                    curX = destX;
                    curY = destY;
                }
            }
            edges.Sort(Edge.MaxYComparer);
            return edges.ToArray();
        }
    }

}
