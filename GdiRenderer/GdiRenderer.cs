/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using DDW.Vex;

using Ms = System.Drawing;
using Vex = DDW.Vex;

namespace DDW.Gdi
{
	public class GdiRenderer
	{
		private Graphics g;
		private System.Drawing.Drawing2D.Matrix translateMatrix;

        public List<Bitmap> GenerateBitmaps(VexObject v)
        {
            List<Bitmap> result = new List<Bitmap>();
            Dictionary<uint, Bitmap> dbmp = GenerateMappedBitmaps(v);
            result.AddRange(dbmp.Values);
            return result;
        }
        /// <summary>
        /// Generates a dictionary of Symbol id and rendered bitmap.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<uint, Bitmap> GenerateMappedBitmaps(VexObject v)
        {
            Dictionary<uint, Bitmap> result = new Dictionary<uint, Bitmap>();

            foreach (uint key in v.Definitions.Keys)
            {
                IDefinition def = v.Definitions[key];
                if (def is Symbol)
                {
                    Symbol symbol = (Symbol)def;
                    Bitmap myBitmap = new Bitmap(
                        (int)symbol.StrokeBounds.Size.Width + 1,
                        (int)symbol.StrokeBounds.Size.Height + 1);
                    myBitmap.SetResolution(96, 96);

                    translateMatrix = new System.Drawing.Drawing2D.Matrix(
                        1, 0, 0, 1, -symbol.StrokeBounds.Point.X, -symbol.StrokeBounds.Point.Y);
                    g = Graphics.FromImage(myBitmap);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    DrawSymbol(symbol);

                    result.Add(symbol.Id, myBitmap);
                    translateMatrix.Dispose();
                }
            }
            return result;
        }

        public void GenerateFilteredBitmaps(VexObject v, Dictionary<string, IDefinition> usedImages, Dictionary<string, List<Bitmap>> genImages)
		{
            foreach (string s in usedImages.Keys)
			{
                uint key = usedImages[s].Id;
				IDefinition def = v.Definitions[key];
				if (def is Timeline)
				{
                    Timeline tl = (Timeline)def;
                    Timeline namedSymbol = GetNamedSymbol(v, tl);
                    if (namedSymbol != null && HasSymbols(v, namedSymbol))
                    {
                        List<Bitmap> bmpFrames = new List<Bitmap>();

                        for (int i = 0; i < tl.FrameCount; i++)
                        {
                            Bitmap myBitmap = new Bitmap(
                                (int)namedSymbol.StrokeBounds.Size.Width + 1,
                                (int)namedSymbol.StrokeBounds.Size.Height + 1);

                            myBitmap.SetResolution(96, 96);
                            translateMatrix = new System.Drawing.Drawing2D.Matrix(
                                1, 0, 0, 1, -namedSymbol.StrokeBounds.Point.X, -namedSymbol.StrokeBounds.Point.Y);
                            g = Graphics.FromImage(myBitmap);
                            g.SmoothingMode = SmoothingMode.AntiAlias;

                            DrawFilteredTimeline(v, namedSymbol);
                            translateMatrix.Dispose();
                            bmpFrames.Add(myBitmap);
                        }

                        string name = s + "#" + namedSymbol.Id;
                        genImages.Add(name, bmpFrames);
                    }
				}
                else if (def is Symbol)
                {
                    Symbol sy = (Symbol)def;
                    List<Bitmap> bmpFrames = new List<Bitmap>();

                    Bitmap myBitmap = new Bitmap(
                        (int)sy.StrokeBounds.Size.Width + 1,
                        (int)sy.StrokeBounds.Size.Height + 1);

                    myBitmap.SetResolution(96, 96);
                    translateMatrix = new System.Drawing.Drawing2D.Matrix(
                        1, 0, 0, 1, -sy.StrokeBounds.Point.X, -sy.StrokeBounds.Point.Y);
                    g = Graphics.FromImage(myBitmap);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    DrawFilteredSymbol(sy);
                    translateMatrix.Dispose();
                    bmpFrames.Add(myBitmap);

                    string name = s + "#" + sy.Id;
                    genImages.Add(name, bmpFrames);
                }
                else if (def is Text)
                {
                    Text tx = (Text)def;
                    List<Bitmap> bmpFrames = new List<Bitmap>();

                    Bitmap myBitmap = new Bitmap(
                        (int)tx.StrokeBounds.Size.Width + 1,
                        (int)tx.StrokeBounds.Size.Height + 1);

                    myBitmap.SetResolution(96, 96);
                    translateMatrix = new System.Drawing.Drawing2D.Matrix(
                        1, 0, 0, 1, -tx.StrokeBounds.Point.X, -tx.StrokeBounds.Point.Y);
                    g = Graphics.FromImage(myBitmap);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    DrawText(tx, tx.Matrix);
                    translateMatrix.Dispose();
                    bmpFrames.Add(myBitmap);

                    string name = s + "#" + tx.Id;
                    genImages.Add(name, bmpFrames);
                }
			}
		}

        public bool HasSymbols(VexObject v, Timeline tl)
        {
            bool result = false;
            for (int i = 0; i < tl.Instances.Count; i++)
            {
                if (v.Definitions[tl.Instances[i].DefinitionId] is Symbol)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private Timeline GetNamedSymbol(VexObject v, Timeline tl)
        {
            Timeline result = null;

            if (tl.Name != "")
            {
                result = tl;
            }
            else
            {
                for (int i = 0; i < tl.Instances.Count; i++)
                {
                    uint defId = tl.Instances[i].DefinitionId;
                    if (v.Definitions[defId] is Timeline && ((Timeline)v.Definitions[defId]).Name != "")
                    {
                        result = (Timeline)(v.Definitions[defId]);
                    }
                }
            }

            return result;
        }
		public void ExportBitmaps(List<Bitmap> bmps)
		{
			for (int i = 0; i < bmps.Count; i++)
			{
				bmps[i].Save(@"exports/inst_" + i + ".bmp");
				bmps[i].Dispose();
			}
		}

        public Dictionary<uint, string> ExportBitmaps(Dictionary<string, List<Bitmap>> bmps)
        {
            //temp: do this bitmap export right
            var result = new Dictionary<uint, string>();

            foreach (string key in bmps.Keys)
            {
                string[] nms = key.Split('#');
                string dir = Path.GetDirectoryName(nms[0]);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                bmps[key][0].Save(nms[0], System.Drawing.Imaging.ImageFormat.Png);
                bmps[key][0].Dispose();
                result.Add(uint.Parse(nms[1]), nms[0]);
            }
            return result;
        }
		public void Render(Symbol symbol, Graphics g)
		{
			this.g = g;
			translateMatrix = new System.Drawing.Drawing2D.Matrix();
			g.SmoothingMode = SmoothingMode.AntiAlias;
			DrawSymbol(symbol);
			translateMatrix.Dispose();
		}

		private void DrawBackground(VexObject v)
		{
			Brush b = new SolidBrush(this.GetColor(v.BackgroundColor));
			RectangleF r = new RectangleF(
				v.ViewPort.Point.X, 
				v.ViewPort.Point.X, 
				v.ViewPort.Size.Width, 
				v.ViewPort.Size.Height);

			g.FillRectangle(b, r);

		}

        private void DrawTimeline(VexObject v, Timeline tl)
        {
            for (int i = 0; i < tl.Instances.Count; i++)
            {
                IDefinition def = v.Definitions[tl.Instances[i].DefinitionId];
                if (def is Symbol)
                {
                    DrawSymbol((Symbol)def);
                }
                else if (def is Timeline)
                {
                    DrawTimeline(v, (Timeline)def);
                }
            }
        }
        public DDW.Vex.Color filterColor;
        public float filterWidth;
        private void DrawFilteredSymbol(Symbol sy)
        {
            bool ignore = false;
            foreach (Shape sh in sy.Shapes)
            {
                if (sh.Fill == null || sh.Stroke is SolidStroke)
                {
                    SolidStroke sf = (SolidStroke)sh.Stroke;
                    if ((sf.Color == filterColor) && (sf.LineWidth <= filterWidth))
                    {
                        ignore = true;
                        break;
                    }
                }
            }

            if (!ignore)
            {
                DrawSymbol(sy);
            }
        }
        private void DrawFilteredTimeline(VexObject v, Timeline tl)
        {
            for (int i = 0; i < tl.Instances.Count; i++)
            {
                Instance inst = (Instance)tl.Instances[i];
                IDefinition def = v.Definitions[inst.DefinitionId];
                if (def is Symbol)
                {
                    DrawFilteredSymbol((Symbol)def);
                }
                else if (def is Timeline)
                {
                    DrawFilteredTimeline(v, (Timeline)def);
                }
                else if (def is Text)
                {
                    DrawText((Text)def, inst.Transformations[0].Matrix);
                }
            }
        }
		private void DrawSymbol(Symbol symbol)
		{
			for (int i = 0; i < symbol.Shapes.Count; i++)
			{
				Shape sh = symbol.Shapes[i];
				DrawShape(sh);
			}
		}
		private void DrawText(Text txt, DDW.Vex.Matrix matrix)
		{
            for (int i = 0; i < txt.TextRuns.Count; i++)
			{
                TextRun tr = txt.TextRuns[i];
                string s = tr.Text;
                FontStyle style = tr.isBold ? FontStyle.Bold : FontStyle.Regular;
                if(tr.isItalic)
                {
                    style |= FontStyle.Italic;
                }
                Font font = new Font(tr.FontName, tr.FontSize, style, GraphicsUnit.Pixel);

                System.Drawing.Color col = System.Drawing.Color.FromArgb(tr.Color.A, tr.Color.R, tr.Color.G, tr.Color.B);
                Brush b = new SolidBrush(col);
			    g.DrawString(s, font, b, matrix.TranslateX, matrix.TranslateY); 

                b.Dispose();
			}
		}

		private void DrawShape(Shape sh)
		{
			List<GraphicsPath> paths;
			if (sh.Fill != null)
			{
				paths = GetPath(sh.ShapeData, true);
				FillPaths(sh.Fill, paths);
			}

			if (sh.Stroke != null)
			{
				paths = GetPath(sh.ShapeData, false);
				StrokePaths(sh.Stroke, paths);
			}
			else
			{
				// this gets rid of slight aliasing spaces between touching vectors
				// todo: do average colors for gradients or something.
				if (sh.Fill.FillType == FillType.Solid)
				{
					StrokeStyle ss = new SolidStroke(.1F, ((SolidFill)sh.Fill).Color);
					paths = GetPath(sh.ShapeData, false);
					StrokePaths(ss, paths);
				}
			}
		}

		private List<GraphicsPath> GetPath(List<IShapeData> shapes, bool isFilled)
		{
			List<GraphicsPath> result = new List<GraphicsPath>();
			if (shapes.Count == 0)
			{
				return result;
			}
			DDW.Vex.Point endPoint = shapes[0].EndPoint;
			GraphicsPath gp = new GraphicsPath();
			gp.FillMode = FillMode.Alternate;
			result.Add(gp);

			for (int i = 0; i < shapes.Count; i++)
			{
				IShapeData sd = shapes[i];

				if (sd.StartPoint != endPoint)
				{
					if (isFilled)
					{
						gp.CloseFigure();
					}
					else
					{
						gp = new GraphicsPath();
						gp.FillMode = FillMode.Alternate;
						result.Add(gp);
					}
				}
				switch (sd.SegmentType)
				{
					case SegmentType.Line :
						Line l = (Line)sd;
						gp.AddLine(l.Anchor0.X, l.Anchor0.Y, l.Anchor1.X, l.Anchor1.Y);
						break;
					case SegmentType.CubicBezier:
						CubicBezier cb = (CubicBezier)sd;
						gp.AddBezier(
							cb.Anchor0.X, cb.Anchor0.Y,
							cb.Control0.X, cb.Control0.Y,
							cb.Control1.X, cb.Control1.Y,
							cb.Anchor1.X, cb.Anchor1.Y);
						break;

					case SegmentType.QuadraticBezier:
						QuadBezier qb = (QuadBezier)sd;
						CubicBezier qtc = qb.GetCubicBezier();
						gp.AddBezier(
							qtc.Anchor0.X, qtc.Anchor0.Y,
							qtc.Control0.X, qtc.Control0.Y,
							qtc.Control1.X, qtc.Control1.Y,
							qtc.Anchor1.X, qtc.Anchor1.Y);
						break;
				}
				endPoint = sd.EndPoint;
			}
			if (isFilled)
			{
				gp.CloseFigure();
			}
			return result;
		}

		private void FillPaths(FillStyle fill, List<GraphicsPath> paths)
		{
			Brush b = null;
			foreach (GraphicsPath path in paths)
			{
				path.Transform(translateMatrix);
				switch (fill.FillType)
				{
					case FillType.Solid:
						SolidFill sf = (SolidFill)fill;
						b = new SolidBrush(GetColor(sf.Color));
						break;

					case FillType.Linear:
						GradientFill lf = (GradientFill)fill;
						RectangleF rect = GetRectangleF(lf.Rectangle);
						LinearGradientBrush lgb = new LinearGradientBrush(
							rect,
							Ms.Color.White,
							Ms.Color.White,
							1.0F
							);
						lgb.InterpolationColors = GetColorBlend(lf);
						lgb.Transform = GetMatrix(lf.Transform);
						lgb.WrapMode = WrapMode.TileFlipX;
						ExtendGradientBrush(lgb, path);
						b = lgb;
						break;

					case FillType.Radial:
						GradientFill rf = (GradientFill)fill;

						ColorBlend cb = GetColorBlend(rf);

						SolidBrush bkgCol = new SolidBrush(cb.Colors[0]);
						g.FillPath(bkgCol, path);
						bkgCol.Dispose();

						// radial fill part
						GraphicsPath gp = new GraphicsPath();
						gp.AddEllipse(GetRectangleF(rf.Rectangle));

						PathGradientBrush pgb = new PathGradientBrush(gp);
						pgb.InterpolationColors = GetColorBlend(rf);
						pgb.Transform = GetMatrix(rf.Transform);
						b = pgb;
						break;

                    case FillType.Image:
                        ImageFill imgFill = (ImageFill)fill;
                        Bitmap bmp = new Bitmap(imgFill.ImagePath);
                        b = new TextureBrush(bmp);
                        break;

					default:
						b = new SolidBrush(Ms.Color.Red);
						break;
				}
				g.FillPath(b, path);
			}
			if (b != null)
			{
				b.Dispose();
			}

		}

		private ColorBlend GetColorBlend(GradientFill fill)
		{
			List<float> positions = new List<float>();
			List<Ms.Color> colors = new List<Ms.Color>();

			int numGradients = fill.Fills.Count;

			for (int i = 0; i < numGradients; i++)
			{
				positions.Add(fill.Stops[i]);
				colors.Add(GetColor(fill.Fills[i]));
			}

			// GDI color blends must start at 0.0 and end at 1.0 or they will crash
			if ((float)positions[0] != 0.0F)
			{
				positions.Insert(0, 0.0F);
				colors.Insert(0, colors[0]);
			}
			if ((float)positions[positions.Count - 1] != 1.0F)
			{
				positions.Add(1.0F);
				colors.Add(colors[colors.Count - 1]);
			}

			ColorBlend cb = new ColorBlend(positions.Count);
			cb.Colors = colors.ToArray();
			cb.Positions = positions.ToArray();

			return cb;
		}
		private void StrokePaths(StrokeStyle stroke, List<GraphicsPath> paths)
		{
			Pen p = null;
			foreach (GraphicsPath path in paths)
			{
				path.Transform(translateMatrix);
				if (stroke is SolidStroke)
				{
					SolidStroke ss = (SolidStroke)stroke;
					p = new Pen(GetColor(ss.Color), ss.LineWidth);
				}
				else
				{
					p = new Pen(Ms.Color.Black, 1);
				}
				p.StartCap = LineCap.Round;
				p.EndCap = LineCap.Round;
				p.LineJoin = LineJoin.Round;
				g.DrawPath(p, path);
				p.Dispose();
			}
		}

		private Ms.Color GetColor(Vex.Color c)
		{
			return Ms.Color.FromArgb(c.ARGB);
		}
		private RectangleF GetRectangleF(Vex.Rectangle rect)
		{
			return new RectangleF(rect.Point.X, rect.Point.Y, rect.Size.Width, rect.Size.Height);
		}
		private System.Drawing.Drawing2D.Matrix GetMatrix(Vex.Matrix m)
		{
			return new System.Drawing.Drawing2D.Matrix(
				m.ScaleX, m.Rotate0, 
				m.Rotate1, m.ScaleY, 
				m.TranslateX, m.TranslateY);
		}










		private static LinearGradientBrush ExtendGradientBrush(LinearGradientBrush brush, GraphicsPath path)
		{
			// get the untransformed gradient rectangle
			RectangleF gradRect = brush.Rectangle;
			// put it into a points array starting with top right
			PointF[] gradPoints = new PointF[4]
					{ 
						new PointF(gradRect.Right, gradRect.Top), 
						new PointF(gradRect.Right, gradRect.Bottom),
						new PointF(gradRect.Left,  gradRect.Bottom),
						new PointF(gradRect.Left,  gradRect.Top)
					};
			// transform the points to get the two edges of the gradient as 
			// tr-br and bl-tl. The width of the gradient can be found at the bottom.
			// This makes it easier to figure out which corners need to be tested.
			brush.Transform.TransformPoints(gradPoints);

			RectangleF pathRect = path.GetBounds();

			// find the corner point to test to see if it might be past the gradient
			// first make the forward(AfBf) and back(AbBb) edge lines of the gradient
			PointF Af = gradPoints[0];
			PointF Bf = gradPoints[1];
			PointF Ab = gradPoints[2];
			PointF Bb = gradPoints[3];

			// set forward and back test corner
			PointF Cb = pathRect.Location;
			PointF Cf = pathRect.Location;
			if (Af.X >= Bf.X)
			{
				Cf.Y += pathRect.Height;
			}
			else
			{
				Cb.Y += pathRect.Height;
			}
			if (Af.Y < Bf.Y)
			{
				Cf.X += pathRect.Width;
			}
			else
			{
				Cb.X += pathRect.Width;
			}
			// gradient width is the connection lines if grad isn't skewed (same for both)
			// check if gradients can ever be skewed... if so, calc line to line dist.
			float gradW = (float)Math.Sqrt(
				(Bf.X - Ab.X) * (Bf.X - Ab.X) + (Bf.Y - Ab.Y) * (Bf.Y - Ab.Y));
			// length of gradient edge (same for both sides)
			float gradH = (float)Math.Sqrt(
				(Bf.X - Af.X) * (Bf.X - Af.X) + (Bf.Y - Af.Y) * (Bf.Y - Af.Y));

			// now check if the path data might be bigger than the gradient
			int hasFRatio = 0;
			int hasBRatio = 0;
			// in the forward direction 
			float distToLineF = 0;
			float distToLineB = 0;
			float sf = ((Af.Y - Cf.Y) * (Bf.X - Af.X) - (Af.X - Cf.X) * (Bf.Y - Af.Y)) / (gradH * gradH);
			if (sf > 0)
			{
				// graphic may be bigger than fill so 
				// figure out how much bigger the fill has to be 
				// (meaning how much smaller the original gradient must be)
				distToLineF = Math.Abs(sf) * gradH;
				hasFRatio = 1;
			}
			// in the back direction 
			float sb = ((Ab.Y - Cb.Y) * (Bb.X - Ab.X) - (Ab.X - Cb.X) * (Bb.Y - Ab.Y)) / (gradH * gradH);
			if (sb > 0)
			{
				distToLineB = Math.Abs(sb) * gradH; ;
				hasBRatio = 1;
			}

			// Now we have the info we need to tell if the gradient doesn't fit in the path
			if ((hasFRatio + hasBRatio) > 0)
			{
				float totalNewWidth = distToLineF + distToLineB + gradW;
				float ratioB = distToLineB / totalNewWidth;
				float ratioF = distToLineF / totalNewWidth;
				float compressRatio = gradW / totalNewWidth;
				float expandRatio = totalNewWidth / gradW; // eg. 1/compressRatio

				float[] pos = brush.InterpolationColors.Positions;
				float[] newPos = new float[pos.Length + hasFRatio + hasBRatio];
				Ms.Color[] cols = brush.InterpolationColors.Colors;
				Ms.Color[] newCols = new Ms.Color[cols.Length + hasFRatio + hasBRatio];
				if (hasBRatio == 1)
				{
					newPos[0] = 0;
					newCols[0] = cols[0];
				}
				for (int i = 0; i < pos.Length; i++)
				{
					newPos[i + hasBRatio] = pos[i] * compressRatio + ratioB;
					newCols[i + hasBRatio] = cols[i];
				}
				newPos[newPos.Length - 1] = 1;
				newCols[newCols.Length - 1] = cols[cols.Length - 1];

				ColorBlend cb2 = new ColorBlend(newPos.Length);
				cb2.Positions = newPos;
				cb2.Colors = newCols;
				brush.InterpolationColors = cb2;

				System.Drawing.Drawing2D.Matrix m2 = brush.Transform;
				// scale it with the edge at the orgin
				m2.Translate(-Bb.X, -Bb.Y, MatrixOrder.Append);
				m2.Scale(expandRatio, expandRatio, MatrixOrder.Append);
				// now move it back to be on the back edge, whatever that is
				if (hasBRatio == 1)
				{
					m2.Translate(Cb.X, Cb.Y, MatrixOrder.Append);
				}
				else
				{
					m2.Translate(Bb.X, Bb.Y, MatrixOrder.Append);
				}
				brush.Transform = m2;
			}
			return brush;
		}
	}
}
