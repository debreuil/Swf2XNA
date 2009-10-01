
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Bitmap = System.Drawing.Bitmap;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Xml.Serialization;

using DDW.Vex;
using DDW.V2D;
using DDW.V2D.Serialization;

namespace DDW.VexTo2DPhysics
{
    public class VexToScreen
    {
        private VexObject curVo;
        private DDW.Swf.SwfCompilationUnit scu;
        private V2DWorld v2dWorld;

        public static string OutputDirectory = Directory.GetCurrentDirectory();
        public static uint instAutoNumber = 0;

        private DDW.Vex.Color outlineColor = new DDW.Vex.Color(0xFF, 0x00, 0x00, 0xFF);
        private float outlineWidth = 1F;
        private string resourceFolder = "exports";

        public Dictionary<string, string> worldData = new Dictionary<string, string>();
        public Dictionary<string, Dictionary<string, string>> codeData = new Dictionary<string, Dictionary<string, string>>();

        List<V2DDefinition> defs = new List<V2DDefinition>();
        public Stack<V2DInstance> parentStack = new Stack<V2DInstance>();
        public V2DInstance root;

        private Dictionary<string, IDefinition> usedImages = new Dictionary<string, IDefinition>();
        
        public VexToScreen()
        {
        }

        public void Convert(VexObject vo, DDW.Swf.SwfCompilationUnit scu)
        {
            curVo = vo;
            this.scu = scu;
            Init();
            FilterMarkers();
            ParseActions();
            ParseTimeline(curVo.Root);
            GenerateWorldActionData(); // temp
            GenerateBitamps();

            //genV2d = new GenV2DWorld(this, paths);
            //genV2d.Generate();
        }

        private V2DInstance curInst { get { return parentStack.Peek(); } }
        private V2DDefinition curDef { get { return parentStack.Peek().Definition; } }

        public void Init()
        {
            V2DDefinition ground = new V2DDefinition();
            ground.Name = V2D.ROOT_NAME;
            ground.FrameCount = 1;

            V2DInstance inst = new V2DInstance();
            inst.DefinitionName = ground.Name;
            inst.InstanceName = V2D.ROOT_NAME;
            inst.Definition = ground;
            inst.Transform = Matrix.Identitiy.GetDrawing2DMatrix().Elements;
            inst.TotalFrames = (int)curVo.GetFrameFromMilliseconds(curVo.Root.Duration);

            parentStack.Push(inst);
            root = inst;
        }
        public void FilterMarkers()
        {
            foreach (IDefinition d in curVo.Definitions.Values)
            {
                string name = d.Name;
                if (name != null && jointKinds.Contains(name))
                {
                    d.UserData = (int)DefinitionKind.JointMarker;
                }
                else if (name != null && shapeKinds.Contains(name))
                {
                    d.UserData = (int)DefinitionKind.ShapeMarker;
                }
                else if (d is Text)
                {
                    d.UserData = (int)DefinitionKind.TextField;
                }
                else if (d is Timeline)
                {
                    if (IsVex2DSymbol(d))
                    {
                        d.UserData = (int)DefinitionKind.Vex2D;
                    }
                    else
                    {
                        d.UserData = (int)DefinitionKind.Timeline;
                    }
                }
                else if (d is Symbol)
                {
                    d.UserData = (int)DefinitionKind.Symbol;
                }

            }
        }

        private void ParseTimeline(Timeline t)
        {
            List<IInstance> insts = t.Instances;
            foreach (IInstance instance in insts)
            {
                if (!(instance is Instance))
                {
                    continue;
                }
                Instance inst = (Instance)instance;
                if (inst.Name == null)
                {
                    inst.Name = "$inst_" + instAutoNumber++;
                }

                IDefinition def = curVo.Definitions[inst.DefinitionId];
                DefinitionKind dk = (DefinitionKind)def.UserData;

                if ((dk & DefinitionKind.Ignore) != 0)
                {
                    continue;
                }

                if ((dk & DefinitionKind.JointMarker) != 0)
                {
                    V2DJointKind jointKind = Enum.Parse(V2DJointKind, def.Name, true);
                    ParseJoint(jointKind, inst);
                }

                if ((dk & DefinitionKind.ShapeMarker) != 0)
                {
                }

                if ((dk & DefinitionKind.TextField) != 0)
                {
                    Text txt = (Text)def;
                }

                if ((dk & DefinitionKind.Timeline) != 0)
                {                    
                    V2DDefinition vd = CreateDefinition(inst, def);
                    V2DInstance vi = CreateInstance(inst, vd);

                    parentStack.Push(vi);
                    ParseTimeline((Timeline)def);
                    parentStack.Pop();
                }

                if ((dk & DefinitionKind.Symbol) != 0)
                {
                    V2DInstance vi = CreateInstance(inst, def);
                }

                if ((dk & DefinitionKind.Vex2D) != 0)
                {
                    // todo: this is just adding images and/or instances
                    Body2D b2d = CreateDefinition(inst, def);
                    b2d.AddShapes(curVo, def, inst);
                }
            }
            GenerateJointData();
        }
        private V2DInstance CreateInstance(Instance inst, V2DDefinition def)
        {
            V2DInstance result = new V2DInstance();            

            result.InstanceName = inst.Name;
            result.Definition = def;
            result.DefinitionName = def.Name;
            result.Depth = (int)inst.Depth;
            result.StartFrame = curVo.GetFrameFromMillisecondsGetFrame(inst.StartTime);
            result.TotalFrames = curVo.GetFrameFromMillisecondsGetFrame(inst.EndTime - inst.StartTime);
            Matrix m = inst.Transformations[0].Matrix;
            result.UserData = m;
            AddSymbolImage(inst, def);

            curDef.Instances.Add(result);
            return result;
        }
        private V2DDefinition CreateDefinition(Instance inst, IDefinition def)
        {
            V2DDefinition result = defs.Find(d => d.Name == def.Name);

            if (result == null)
            {
                result = new V2DDefinition();
                result.DefinitionName = def.Name;
                result.LinkageName = def.Name; // todo: find real linkageName
                result.Bounds = def.StrokeBounds;
                result.FrameCount = (int)tlDef.FrameCount;
                //ParseBodyImage(result, def, inst);
            }
            return result;
        }

        private void ParseJoint(V2DJointKind jointKind, Instance inst)
        {
            List<V2DJoint> joints = curDef.Joints; //List<V2DJoint>
            string nm = inst.Name;
            int firstUs = nm.IndexOf('_');
            string jName = (firstUs > -1) ? nm.Substring(0, firstUs) : nm;
            string targName = (firstUs > -1) ? nm.Substring(firstUs + 1) : "";

            Point p = new Point(
                inst.Transformations[0].Matrix.TranslateX,
                inst.Transformations[0].Matrix.TranslateY);

            V2DJoint joint = joints.Find(jt => jt.Name == jName);
            if (joint == null)
            {
                joint = new V2DJoint();
                joint.Name = jName;
                joint.Type = jointKind;
                joint.X2 = p.X;
                joint.Y2 = p.Y;
                if (inst.Transformations.Count > 1) // pulley defined by joints on frame 2
                {
                    joint.GroundAnchor1X = inst.Transformations[1].Matrix.TranslateX;
                    joint.GroundAnchor1Y = inst.Transformations[1].Matrix.TranslateY;
                }
            }
            else
            {
                joint.X2 = p.X;
                joint.Y2 = p.Y;
                if (inst.Transformations.Count > 1) // pulley defined by joints on frame 2
                {
                    joint.GroundAnchor2X = inst.Transformations[1].Matrix.TranslateX;
                    joint.GroundAnchor2Y = inst.Transformations[1].Matrix.TranslateY;
                }
            }
        }
        private void GenerateJointActionData(V2DJoint jnt)
        {
            foreach (string k in codeData.Keys)
            {
                if (jnt.Name == k)
                {
                    Dictionary<string, string> d = codeData[k];
                    foreach (string s in d.Keys)
                    {
                        if (jnt.data.ContainsKey(s))
                        {
                            jnt.data[s] = d[s];
                        }
                        else
                        {
                            jnt.data.Add(s, d[s]);
                        }
                    }
                }
            }
        }
        private void GenerateWorldActionData()
        {
            foreach (string k in codeData.Keys)
            {
                if (k == "world")
                {
                    foreach (string s in codeData[k].Keys)
                    {
                        if (worldData.ContainsKey(s))
                        {
                            worldData[s] = codeData[k][s];
                        }
                        else
                        {
                            worldData.Add(s, codeData[k][s]);
                        }
                    }
                }
            }
            if (!worldData.ContainsKey("Width"))
            {
                worldData["Width"] = curVo.ViewPort.Size.Width.ToString();
            }
            if (!worldData.ContainsKey("Height"))
            {
                worldData["Height"] = curVo.ViewPort.Size.Height.ToString();
            }

        }
        
        private string AddSymbolImage(Instance inst, IDefinition sy)
        {
            string nm = (sy.Name == null) ? "sym_" + sy.Id.ToString() : sy.Name;
            string bmpPath = resourceFolder + @"\" + nm + ".png";

            bmpPath = OutputDirectory + "/" + bmpPath;
            bmpPath = bmpPath.Replace('\\', '/');

            if (!usedImages.ContainsKey(nm))
            {
                usedImages.Add(nm, sy);
            }
            return bmpPath;
        }

        private List<V2DInstance> FindTargetUnderPoint(Point p)
        {
            List<V2DInstance> result = new List<V2DInstance>();
            foreach (V2DInstance d in curDef.Instances)
            {
                if (IsPointInside(d, p))
                {
                    result.Add(d);
                }
            }
            result.Sort();
            return result;
        }
        private bool IsPointInside(V2DInstance inst, Point p)
        {
            bool result = false;
            System.Drawing.Drawing2D.Matrix m = ((Matrix)inst.UserData).GetDrawing2DMatrix();
            m.Invert();
            System.Drawing.PointF[] p2 = new System.Drawing.PointF[] { new System.Drawing.PointF(p.X, p.Y) };
            m.TransformPoints(p2);
            Point tp = new Point(p2[0].X, p2[0].Y);
            for (int i = 0; i < inst.Definition.V2DShapes.Count; i++)
            {
                if (Shapes[i].IsPointInside(tp))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        private void EnsureBodyTags(V2DJoint j)
        {
            bool hasB1 = j.data.ContainsKey("body1");
            bool hasB2 = j.data.ContainsKey("body2");
            bool b1FromCode = hasB1;

            if (!hasB1 || !hasB2)
            {
                List<Body2D> hits = FindTargetUnderPoint(j.Locations[0]);
                if (!hasB1 && hits.Count > 0)
                {
                    j.data.Add("body1", hits[0].InstanceName);
                    hasB1 = true;
                }

                if (!hasB2)
                {
                    Body2D b2 = null;
                    if (j.Locations.Count > 1)// && j.JointKind == JointKind.Distance)
                    {
                        List<Body2D> b2L = FindTargetUnderPoint(j.Locations[1]);

                        if (b2L.Count > 0)
                        {
                            b2 = b2L[0];
                        }
                        else
                        {
                            // ground
                        }
                    }
                    else if (hits.Count > 1)
                    {
                        b2 = hits[1];
                    }
                    else if (b1FromCode && hits.Count > 0)
                    {
                        b2 = hits[0];
                    }

                    if (!hasB1)
                    {
                        j.data.Add("body1", V2D.ROOT_NAME);
                    }

                    if (b2 != null)
                    {
                        j.data.Add("body2", b2.InstanceName);
                        hasB2 = true;
                    }
                    else
                    {
                        j.data.Add("body2", V2D.ROOT_NAME);
                    }
                }
            }
            GenerateJointActionData(j, curDef);

            // flip if a ground ref (I think all joints to ground use ground on body 1..?)
            if (j.data["body2"] == V2D.ROOT_NAME)
            {
                string temp = j.data["body2"];
                j.data["body2"] = j.data["body1"];
                j.data["body1"] = temp;
                if (j.Locations.Count > 1)
                {
                    Point temp2 = j.Locations[0];
                    j.Locations[0] = j.Locations[1];
                    j.Locations[1] = temp2;
                }
            }
        }
        private void GenerateJointData()
        {
            foreach (V2DJoint j in curDef.Joints)
            {
                string jName = j.Name;
                EnsureBodyTags(j);
                string b1Name = j.data["body1"];
                string b2Name = j.data["body2"];
                Body2D b1Body = (b1Name == V2D.ROOT_NAME) ? root : curDef.GetChildByName(b1Name);
                Body2D b2Body = (b2Name == V2D.ROOT_NAME) ? root : curDef.GetChildByName(b2Name);

                V2DJointKind jointKind = j.JointKind;

                if (!j.data.ContainsKey("location"))
                {
                    if (V2D.OUTPUT_TYPE == OutputType.Swf)
                    {
                        j.data.Add("location", "#" + j.Locations[0].ToString());
                    }
                    else
                    {
                        j.data.Add("X", j.Locations[0].X.ToString());
                        j.data.Add("Y", j.Locations[0].Y.ToString());
                    }
                }
                if ((j.Locations.Count > 1) && (jointKind == V2DJointKind.Distance || jointKind == V2DJointKind.Pully))
                {
                    if (!j.data.ContainsKey("location2"))
                    {
                        if (V2D.OUTPUT_TYPE == OutputType.Swf)
                        {
                            j.data.Add("location2", "#" + j.Locations[1].ToString());
                        }
                        else
                        {
                            j.data.Add("X2", j.Locations[1].X.ToString());
                            j.data.Add("Y2", j.Locations[1].Y.ToString());
                        }
                    }
                    if (jointKind == V2DJointKind.Pully)
                    {
                        Point groundAnchor1;
                        Point groundAnchor2;
                        if (b1Body.Transforms.Count > 1)
                        {
                            groundAnchor1 = new Point(
                                b1Body.Transforms[1].Matrix.TranslateX,
                                b1Body.Transforms[1].Matrix.TranslateY);
                            groundAnchor2 = new Point(
                                b2Body.Transforms[1].Matrix.TranslateX,
                                b2Body.Transforms[1].Matrix.TranslateY);
                        }
                        else
                        {
                            groundAnchor1 = j.Transforms[0];
                            groundAnchor2 = j.Transforms[1];
                        }

                        float d1x = groundAnchor1.X - j.Locations[0].X;// m1a.TranslateX;
                        float d1y = groundAnchor1.Y - j.Locations[0].Y;//m1a.TranslateY;
                        float d2x = groundAnchor2.X - j.Locations[1].X;//m2a.TranslateX;
                        float d2y = groundAnchor2.Y - j.Locations[1].Y;//m2a.TranslateY;
                        float maxLength1 = (float)Math.Sqrt(d1x * d1x + d1y * d1y);
                        float maxLength2 = (float)Math.Sqrt(d2x * d2x + d2y * d2y);
                        //j.data.Add("groundAnchor1", groundAnchor1.ToString());
                        //j.data.Add("groundAnchor2", groundAnchor2.ToString());
                        j.data.Add("groundAnchor1X", groundAnchor1.X.ToString());
                        j.data.Add("groundAnchor1Y", groundAnchor1.Y.ToString());
                        j.data.Add("groundAnchor2X", groundAnchor2.X.ToString());
                        j.data.Add("groundAnchor2Y", groundAnchor2.Y.ToString());
                        j.data.Add("maxLength1", maxLength1.ToString());
                        j.data.Add("maxLength2", maxLength2.ToString());
                    }
                }
                else if (b1Body.Transforms.Count > 1 || b2Body.Transforms.Count > 1)
                {
                    List<Transform> tr = (b1Body.Transforms.Count > 1) ?
                        b1Body.Transforms : b2Body.Transforms;
                    if (jointKind == V2DJointKind.Revolute)
                    {
                        float start = (float)(tr[0].Matrix.GetMatrixComponents().Rotation / 180 * Math.PI);
                        float rmin = (float)(tr[1].Matrix.GetMatrixComponents().Rotation / 180 * Math.PI);
                        float rmax = (float)(tr[2].Matrix.GetMatrixComponents().Rotation / 180 * Math.PI);
                        rmin = rmin - start;
                        rmax = rmax - start;
                        if (rmin > 0)
                        {
                            rmin = (float)(Math.PI * -2 + rmin);
                        }
                        j.data.Add("min", rmin.ToString());
                        j.data.Add("max", rmax.ToString());

                    }
                    else if (jointKind == V2DJointKind.Prismatic)
                    {
                        Point a = new Point(
                            tr[0].Matrix.TranslateX,
                            tr[0].Matrix.TranslateY);
                        Point p0 = new Point(
                            tr[1].Matrix.TranslateX,
                            tr[1].Matrix.TranslateY);
                        Point p1 = new Point(
                            tr[2].Matrix.TranslateX,
                            tr[2].Matrix.TranslateY);

                        double len = Math.Sqrt((p1.Y - p0.Y) * (p1.Y - p0.Y) + (p1.X - p0.X) * (p1.X - p0.X));
                        double r = ((p0.Y - a.Y) * (p0.Y - p1.Y) - (p0.X - a.X) * (p0.X - p1.X)) / (len * len);

                        float axisX = p1.X - p0.X;
                        float axisY = p1.Y - p0.Y;
                        float maxAxis = Math.Abs(axisX) > Math.Abs(axisY) ? axisX : axisY;
                        axisX /= maxAxis;
                        axisY /= maxAxis;

                        Point ap0 = new Point(p0.X - a.X, p0.Y - a.Y);
                        Point ap1 = new Point(p1.X - a.X, p1.Y - a.Y);
                        float min = (float)-(Math.Abs(r * len));
                        float max = (float)((1 - Math.Abs(r)) * len);
                        // Point ab = new Point(b.X - a.X, b.Y - a.Y);

                        // float r0 = (ap0.X * ab.X + ap0.Y * ab.Y) / (w * w); // dot product
                        //float r1 = (ap1.X * ab.X + ap1.Y * ab.Y) / (w * w); // dot product

                        j.data.Add("axisX", axisX.ToString());
                        j.data.Add("axisY", axisY.ToString());
                        j.data.Add("min", min.ToString());
                        j.data.Add("max", max.ToString());
                    }
                }
            }
        }

        private bool IsVex2DSymbol(IDefinition def)
        {
            bool result = false;
            if (def is Symbol)
            {
                List<Shape> shapes = ((Symbol)def).Shapes;
                foreach (Shape sh in shapes)
                {
                    if (sh.Fill == null || sh.Stroke is SolidStroke)
                    {
                        SolidStroke sf = (SolidStroke)sh.Stroke;
                        if ((sf.Color == outlineColor) && (sf.LineWidth <= outlineWidth))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            else if (def is Timeline)
            {
                for (int i = 0; i < ((Timeline)def).Instances.Count; i++)
                {
                    IInstance inst = ((Timeline)def).Instances[i];
                    IDefinition def2 = curVo.Definitions[inst.DefinitionId];
                    result = def2 is Symbol ? IsVex2DSymbol(def2) : false;
                    if (result)
                    {
                        break;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public V2DContent GenerateBitamps()
        {
            string path = scu.FullPath;
            Gdi.GdiRenderer gr = new Gdi.GdiRenderer();
            gr.filterColor = outlineColor;
            gr.filterWidth = outlineWidth;
            Dictionary<string, List<Bitmap>> bmps = new Dictionary<string, List<Bitmap>>();
            gr.GenerateFilteredBitmaps(curVo, usedImages, bmps);
            //paths = gr.ExportBitmaps(bmps);
            gr.ExportBitmaps(bmps);
        }
        public V2DContent GetV2DContent(ContentProcessorContext context)
        {
            V2DContent result = new V2DContent();
            result.v2dWorld = v2dWorld;

            //XmlSerializer xs = new XmlSerializer(typeof(V2DWorld));
            //StringWriter sw = new StringWriter();
            //xs.Serialize(sw, result.v2dWorld);

            result.contentTextures.Clear();
            foreach (string s in paths.Values)
            {
                ExternalReference<TextureContent> tr = new ExternalReference<TextureContent>(s);
                Texture2DContent texture = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(tr, null);
                result.contentTextures.Add(Path.GetFileNameWithoutExtension(s), texture);
            }

            return result;
        }


        private void ParseActions()
        {
            List<DDW.Swf.DoActionTag> dat = new List<DDW.Swf.DoActionTag>();
            foreach (DDW.Swf.ISwfTag tag in scu.Tags)
            {
                if (tag is DDW.Swf.DoActionTag)
                {
                    dat.Add((DDW.Swf.DoActionTag)tag);
                }
            }

            foreach (DDW.Swf.DoActionTag tag in dat)
            {
                DDW.Swf.ConstantPool cp = null;
                List<string> stack = new List<string>();
                for (int i = 0; i < tag.Statements.Count; i++)
                {
                    DDW.Swf.IAction a = tag.Statements[i];
                    if (a is DDW.Swf.ConstantPool)
                    {
                        cp = (DDW.Swf.ConstantPool)a;
                    }
                    else if (a is DDW.Swf.SetMember)
                    {
                        if (stack.Count > 2)
                        {
                            string s0 = stack[0];
                            string s1 = stack[1];
                            string s2 = stack[2];
                            if (!codeData.ContainsKey(s0))
                            {
                                codeData.Add(s0, new Dictionary<string, string>());
                            }
                            Dictionary<string, string> targData = codeData[s0];
                            targData.Add(s1, s2);
                        }
                        stack.Clear();
                    }
                    else if (a is DDW.Swf.Push)
                    {
                        DDW.Swf.Push push = (DDW.Swf.Push)a;
                        for (int j = 0; j < push.Values.Count; j++)
                        {
                            DDW.Swf.IPrimitive v = push.Values[j];
                            switch (v.PrimitiveType)
                            {
                                case DDW.Swf.PrimitiveType.String:
                                    stack.Add(((DDW.Swf.PrimitiveString)v).StringValue);
                                    break;

                                case DDW.Swf.PrimitiveType.Constant8:
                                    stack.Add(cp.Constants[((DDW.Swf.PrimitiveConstant8)v).Constant8Value]);
                                    break;
                                case DDW.Swf.PrimitiveType.Constant16:
                                    stack.Add(cp.Constants[((DDW.Swf.PrimitiveConstant16)v).Constant16Value]);
                                    break;

                                case DDW.Swf.PrimitiveType.Integer:
                                    stack.Add(((DDW.Swf.PrimitiveInteger)v).IntegerValue.ToString());
                                    break;
                                case DDW.Swf.PrimitiveType.Float:
                                    stack.Add(((DDW.Swf.PrimitiveFloat)v).FloatValue.ToString());
                                    break;
                                case DDW.Swf.PrimitiveType.Double:
                                    stack.Add(((DDW.Swf.PrimitiveDouble)v).DoubleValue.ToString());
                                    break;
                                case DDW.Swf.PrimitiveType.Boolean:
                                    stack.Add(((DDW.Swf.PrimitiveBoolean)v).BooleanValue.ToString().ToLowerInvariant());
                                    break;
                                case DDW.Swf.PrimitiveType.Null:
                                    stack.Add("null");
                                    break;
                                case DDW.Swf.PrimitiveType.Undefined:
                                    stack.Add("null");
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
