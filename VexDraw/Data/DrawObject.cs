using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDW.Vex;
using DDW.Utils;
using System.IO;
using Draw2D = System.Drawing.Drawing2D;

namespace DDW.VexDraw
{
    public class DrawObject
    {
        public static int twips = 32;

        public List<DrawSymbol> drawSymbols;
        public List<DrawImage> drawImages;
        public List<FillStyle> fills;
        public List<StrokeStyle> strokes;
        public List<DrawTimeline> drawTimelines;

        public Dictionary<uint, string> definitionNames;
        public Dictionary<uint, string> instanceNames;
        public List<string> paths;

        public List<SolidFill> solidFills;
        public List<GradientFill> gradientFills;


        public int firstGradientIndex;

        public DrawObject(VexObject vo)
        {
            List<Image> allImages = vo.GetAllImages();
            List<Symbol> allSymbols = vo.GetAllSymbols();
            List<Timeline> allTimelines = vo.GetAllTimelines();

            GenerateNameTables(allTimelines);
            //GetColorNames(allSymbols);

            GetFillsAndStrokes(allSymbols);
            GetDrawSymbols(allSymbols);
            GetDrawImages(allImages);
            GetDrawTimelines(allTimelines);
        }


        public void ToJson(StringBuilder sb)
        {
            sb.Append("\n{\n");

            // Definition Names
            sb.Append("\"definitionNameTable\":[");
            string comma = "";
            foreach (uint id in definitionNames.Keys)
            {
                sb.Append(comma + "[" + id + ",\"" + definitionNames[id] + "\"]");
                comma = ",";
            }
            sb.Append("],\n");

            // Instance Names
            sb.Append("\"instanceNameTable\":[");
            comma = "";
            foreach (uint id in instanceNames.Keys)
            {
                sb.Append(comma + "[" + id + ",\"" + instanceNames[id] + "\"]");
                comma = ",";
            }
            sb.Append("],\n");


            // strokes
            sb.Append("\"strokes\":[");
            //sb.Append("\\\\ strokes\n[");
            comma = "";
            foreach (SolidStroke ss in strokes)
            {
                sb.Append(comma + ss.LineWidth.ToString("0.##") + ",");
                sb.Append(ss.Color.AFlipRGB.ToString());
                comma = ",";
            }
            sb.Append("],\n");


            // solid fills
            sb.Append("\"fills\":[\n");
            //sb.Append("[\n\\\\ solid fills\n");
            comma = "";
            bool inGradients = false;
            foreach (FillStyle fs in fills)
            {
                if (fs.FillType == FillType.Solid)
                {
                    SolidFill sf = (SolidFill)fs;
                    sb.Append(comma + sf.Color.AFlipRGB.ToString());
                    comma = ",";
                }
                else
                {
                    if(!inGradients)
                    {
                        sb.Append(",\n");
                        //sb.Append(",\n\\\\ gradients\n");
                        comma = "";
                    }
                    inGradients = true;

                    GradientFill gf = (GradientFill)fs;

                    sb.Append(comma + "[");
                    sb.Append(gf.GradientType == GradientType.Linear ? "\"L\"," : "\"R\",");
                    sb.Append("[" + GetGradientLineString(gf) + "],");

                    // new argb colors
                    List<Color> colors = new List<Color>(gf.Fills);
                    List<float> positions = new List<float>(gf.Stops);
                    if (gf.FillType == FillType.Radial)
                    {
                        gf.ReverseStops(colors, positions);
                    }

                    string comma2 = "";
                    sb.Append("[");
                    for (int i = 0; i < colors.Count; i++)
                    {
                        Color c = colors[i];
                        sb.Append(comma2 + c.AFlipRGB.ToString() + ",");
                        sb.Append(positions[i].ToString("0.##"));
                        comma2 = ",";
                    }
                    sb.Append("]]");
                    comma = ",\n";
                }
            }
            sb.Append("\n],\n");

            sb.Append("\"images\":[\n");
            comma = "";
            foreach (DrawImage image in drawImages)
            {
                sb.Append(comma);
                image.ToJson(sb);
                comma = ",\n";
            }
            sb.Append("\n],\n");


            sb.Append("\"symbols\":[\n");
            comma = "";
            foreach (DrawSymbol symbol in drawSymbols)
            {
                sb.Append(comma);
                symbol.ToJson(sb);
                comma = ",\n";
            }
            sb.Append("\n],\n");

            sb.Append("\"timelines\":[\n");
            comma = "";
            foreach (DrawTimeline tl in drawTimelines)
            {
                sb.Append(comma);
                tl.ToJson(sb);
                comma = ",\n";
            }
            sb.Append("\n]\n");


            sb.Append("\n}\n");
        }

        private string GetGradientLineString(GradientFill gf)
        {
            string result = "";

            float[] pts = GetGradientLine(gf);
            string comma = "";
            for (int i = 0; i < pts.Length; i++)
            {
                result += comma + pts[i].ToString("0.##");
                comma = ",";
            }

            return result;
        }
        public static float[] GetGradientLine(GradientFill gf)
        {
            float[] result = new float[4];

            System.Drawing.PointF[] pts = GradientFill.GradientVexRect.SysPointFs();
            using (Draw2D.Matrix m = gf.Transform.SysMatrix())
            {
                m.TransformPoints(pts);
            }

            if (gf.GradientType == GradientType.Linear)
            {
                result[0] = pts[0].X;
                result[1] = pts[0].Y;
                result[2] = pts[1].X;
                result[3] = pts[1].Y;
            }
            else // radial is center to rightCenter edge
            {
                result[0] = pts[0].X + (pts[1].X - pts[0].X) / 2;
                result[1] = pts[0].Y + (pts[2].Y - pts[0].Y) / 2;
                result[2] = pts[1].X;
                result[3] = pts[1].Y + (pts[2].Y - pts[0].Y) / 2;
            }

            return result;
        }

        private void GetDrawSymbols(List<Symbol> symbols)
        {
            drawSymbols = new List<DrawSymbol>();

            foreach (Symbol symbol in symbols)
            {
                DrawSymbol ds = new DrawSymbol(symbol);
                drawSymbols.Add(ds);
            }
        }
        private void GetDrawImages(List<Image> images)
        {
            drawImages = new List<DrawImage>();
            paths = new List<string>();

            foreach (Image image in images)
            {
                DrawImage di = new DrawImage(image, (uint)paths.Count);
                drawImages.Add(di);
                paths.Add(image.Path);
            }
        }
        private void GetDrawTimelines(List<Timeline> timelines)
        {
            drawTimelines = new List<DrawTimeline>();

            foreach (Timeline tl in timelines)
            {
                DrawTimeline dt = new DrawTimeline(tl);
                drawTimelines.Add(dt);
            }
        }

        private void GenerateNameTables(List<Timeline> timelines)
        {
            definitionNames = new Dictionary<uint, string>();
            instanceNames = new Dictionary<uint, string>();

            foreach (Timeline tl in timelines)
            {
                if (tl.Name != null && tl.Name != "" && !tl.Name.StartsWith("$"))
                {
                    definitionNames.Add(tl.Id, tl.Name);
                }

                foreach (Instance inst in tl.Instances)
                {
                    if (inst.Name != null && inst.Name != "" && !inst.Name.StartsWith("$"))
                    {
                        instanceNames.Add(inst.InstanceHash, inst.Name);
                    }
                }
            }
            definitionNames.OrderBy(key => key.Value);
            instanceNames.OrderBy(key => key.Value);
        }

        private void GetFillsAndStrokes(List<Symbol> symbols)
        {
            Dictionary<SolidFill, int> solidFillMap = new Dictionary<SolidFill,int>();
            Dictionary<GradientFill, int> gradientFillMap = new Dictionary<GradientFill,int>();
            Dictionary<SolidStroke, int> strokeMap = new Dictionary<SolidStroke,int>();


            solidFillMap.Add(FillStyle.NoFill, 0);
            strokeMap.Add(StrokeStyle.NoStroke, 0);

            int solidFillCounter = 1;
            int gradientFillCounter = 0;
            int strokeCounter = 1;

            for (int i = 0; i < symbols.Count; i++)
            {
                Symbol symbol = symbols[i];
                foreach (Shape shape in symbol.Shapes)
                {
                    if (shape.Fill != null)
                    {
                        if (shape.Fill is SolidFill)
                        {
                            if (solidFillMap.ContainsKey((SolidFill)shape.Fill))
                            {
                                shape.Fill.UserData = solidFillMap[(SolidFill)shape.Fill];
                            }
                            else
                            {
                                shape.Fill.UserData = solidFillCounter++;
                                solidFillMap.Add((SolidFill)shape.Fill, shape.Fill.UserData);
                            }
                        }
                        else if (shape.Fill is GradientFill)
                        {
                            if (gradientFillMap.ContainsKey((GradientFill)shape.Fill))
                            {
                                shape.Fill.UserData = gradientFillMap[(GradientFill)shape.Fill];
                            }
                            else
                            {
                                GradientFill gf = (GradientFill)shape.Fill;
                                gf.UserData = gradientFillCounter++;
                                gradientFillMap.Add(gf, gf.UserData);
                            }
                        }
                    }

                    if (shape.Stroke != null && shape.Stroke is SolidStroke)
                    {
                        if (strokeMap.ContainsKey((SolidStroke)shape.Stroke))
                        {
                            shape.Stroke.UserData = strokeMap[(SolidStroke)shape.Stroke];
                        }
                        else
                        {
                            shape.Stroke.UserData = strokeCounter++;
                            strokeMap.Add((SolidStroke)shape.Stroke, shape.Stroke.UserData);
                        }
                    }
                }
            }


            List<GradientFill> gradients = new List<GradientFill>(gradientFillMap.Keys);
            gradients.Sort((a, b) => a.UserData.CompareTo(b.UserData));

            solidFills = solidFillMap.Keys.ToList();
            solidFills.Sort((a, b) => a.UserData.CompareTo(b.UserData));
            fills = new List<FillStyle>(solidFills);

            firstGradientIndex = fills.Count;

            strokes = new List<StrokeStyle>(strokeMap.Keys);
            strokes.Sort((a, b) => a.UserData.CompareTo(b.UserData));

            int fillLen = fills.Count;
            for (int i = 0; i < gradients.Count; i++)
            {
                gradients[i].UserData += fillLen;
            }
            fills.AddRange(gradients);
            gradientFills = gradients.ToList();
        }

    }
}
