/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Bitmap = System.Drawing.Bitmap;
using DDW.Vex;
using DDW.V2D;
using xgr = Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using DDW.V2D.Serialization;
using System.Xml.Serialization;
using DDW.Placeholder;
using System.Diagnostics;

namespace DDW.VexTo2DPhysics
{
    public class VexTree
    {
        #region Get rid of this later
        public List<string> jointKinds = new List<string>(new string[] { 
            "distanceJoint", 
            "revoluteJoint", 
            "prismaticJoint",
            "pullyJoint",
            "gearJoint",
            "mouseJoint"
        });
        public List<string> shapeKinds = new List<string>(new string[] { 
            "circleShape"
        });

        public List<V2DJointKind> jointKindMap = new List<V2DJointKind>(new V2DJointKind[] { 
            V2DJointKind.Distance, 
            V2DJointKind.Revolute, 
            V2DJointKind.Prismatic, 
            V2DJointKind.Pully, 
            V2DJointKind.Gear, 
            V2DJointKind.Mouse
        });
        #endregion

        public static string OutputDirectory = Directory.GetCurrentDirectory();
        public static uint instAutoNumber = 0;

        private VexObject curVo;
        private string resourceFolder = "exports";

        public Dictionary<string, string> worldData = new Dictionary<string, string>();
        public List<Definition2D> definitions = new List<Definition2D>();
        //public Dictionary<string, Joint2D> joints = new Dictionary<string, Joint2D>();
        public Dictionary<string, Dictionary<string, string>> codeData = new Dictionary<string, Dictionary<string, string>>();
        public Stack<Instance2D> parentStack = new Stack<Instance2D>();
        public Definition2D root;
        public Instance2D rootInstance;
        private Dictionary<string, IDefinition> usedImages = new Dictionary<string, IDefinition>();
        private GenV2DWorld genV2d;
		private List<string> fontNames = new List<string>();

        public VexTree()
        {
        }

        public void Convert(VexObject vo, DDW.Swf.SwfCompilationUnit scu)
        {
            curVo = vo;
            Init(scu);
            FilterMarkers();
            ParseActions(scu);
            ParseRootTimeline();
            ParseNamedDefinitions();
            GenerateWorldActionData();

            Gdi.GdiRenderer gr = new Gdi.GdiRenderer();
            Dictionary<string, List<Bitmap>> bmps = new Dictionary<string, List<Bitmap>>();
            gr.GenerateFilteredBitmaps(vo, usedImages, bmps);
            gr.ExportBitmaps(bmps);

            genV2d = new GenV2DWorld(this, usedImages);
            genV2d.Generate();
        }
        public void WriteToXml()
        {
            if (genV2d != null && genV2d.v2dWorld != null)
            {
                XmlSerializer xs = new XmlSerializer(typeof(V2DWorld));
                StringWriter sw = new StringWriter();
                xs.Serialize(sw, genV2d.v2dWorld);

                string xmlPath = resourceFolder + @"\" + curVo.Name + @"\data.xml";
                TextWriter tw = new StreamWriter(xmlPath);
                tw.Write(sw.ToString());
                tw.Close();
                sw.Close();

                Debug.WriteLine("********** Wrote file data to " + Directory.GetCurrentDirectory()+ "\\" + xmlPath + ".");
            }
        }
		public V2DContentHolder GetV2DContent(ContentProcessorContext context)
        {
			V2DContentHolder result = new V2DContentHolder();
            result.v2dWorld = genV2d.v2dWorld;// V2DWorld.CreateFromXml(genV2d.path);

            //XmlSerializer xs = new XmlSerializer(typeof(V2DWorld));
            //StringWriter sw = new StringWriter();
            //xs.Serialize(sw, result.v2dWorld);

            result.contentTextures.Clear();
            foreach (string s in usedImages.Keys)
            {
                ExternalReference<TextureContent> tr = new ExternalReference<TextureContent>(s);
                Texture2DContent texture = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(tr, null);
                result.contentTextures.Add(Path.GetFileNameWithoutExtension(s), texture);
            }

            return result;
        }

        public void Init(DDW.Swf.SwfCompilationUnit scu)
        {
            root = new Definition2D();
            root.DefinitionName = V2D.ROOT_NAME;
            root.LinkageName = V2D.ROOT_NAME;
            root.Bounds = Rectangle.Empty;
            root.FrameCount = 1;
            definitions.Add(root);

            rootInstance = new Instance2D(V2D.ROOT_NAME, root.DefinitionName, 0, 0, 0, 1, 1);
            rootInstance.Definition = root;
            rootInstance.Transforms.Add(new Transform(
                0, 
                (uint)(scu.Header.FrameCount * (1000 / scu.Header.FrameRate)), 
                new Matrix(1, 0, 0, 1, 0, 0), 
                1, 
                ColorTransform.Identity));

            parentStack.Push(rootInstance);
            
        }

        private void ParseNamedDefinitions()
        {
            foreach(uint key in curVo.Definitions.Keys)
            {
                IDefinition def = curVo.Definitions[key];

                if (def.Name != null &&
                    !def.Name.StartsWith("$") &&
                    definitions.Find(d => d.DefinitionName == def.Name) == null)
                {
                    EnsureDefinition(null, def);
                }                
            }
        }
        private void ParseRootTimeline()
        {
            ParseTimeline(curVo.Root);
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
                if (!curVo.Definitions.ContainsKey(inst.DefinitionId))
                {
                    throw new KeyNotFoundException("Defs have no key: " + inst.DefinitionId + " named: " + inst.Name);
                }
                if (inst.Name == null)
                {
                    inst.Name = "$" + instAutoNumber++ + "$";
                }

                IDefinition def = curVo.Definitions[inst.DefinitionId];

                EnsureDefinition(inst, def);
            }
            GenerateJointData(parentStack.Peek().Definition);
        }

        private void EnsureDefinition(Instance inst, IDefinition def)
        {
			DefinitionKind dk = (DefinitionKind)def.UserData;
			bool addInst = (inst != null);
			Matrix m = (inst == null) ? Matrix.Identitiy : inst.Transformations[0].Matrix;

			if ((dk & DefinitionKind.Ignore) == 0)
			{
				if (((dk & DefinitionKind.JointMarker) != 0) && (inst != null))
				{
					V2DJointKind jointKind = jointKindMap[jointKinds.IndexOf(def.Name)];
					ParseJoint(jointKind, inst);
				}
				else if (((dk & DefinitionKind.ShapeMarker) != 0) && (inst != null))
				{
					if (!parentStack.Peek().Definition.IsDefined)
					{
						parentStack.Peek().Definition.AddShapes(curVo, def, m);
					}
				}

				if ((dk & DefinitionKind.TextField) != 0)
				{
					Text txt = (Text)def;
					if (def.Name == null)
					{
						def.Name = "$tx_" + def.Id;
					}
					Definition2D d2d = GetTextDefinition(m, txt);
					AddSymbolImage(def);

					if (addInst)
					{
						AddInstance(inst, def);
					}
				}

				if ((dk & DefinitionKind.Timeline) != 0)
				{
					Definition2D d2d = GetDefinition2D(m, def);

					Instance2D i2d;
					if (addInst)
					{
						i2d = AddInstance(inst, def);
					}
					else
					{
						i2d = CreateInstance2D(def);
					}
					parentStack.Push(i2d);
					ParseTimeline((Timeline)def);
					i2d.Definition.IsDefined = true;
					parentStack.Pop();
				}

				if ((dk & DefinitionKind.Symbol) != 0)
				{
					// todo: this is just adding images
					//Body2D b2d = CreateBody2D(inst, def);
					if (def.Name == null)
					{
						def.Name = "$sym_" + def.Id;
					}
					Definition2D d2d = GetDefinition2D(m, def);
					AddSymbolImage(def);

					if (addInst)
					{
						AddInstance(inst, def);
					}
				}

				if ((dk & DefinitionKind.Vex2D) != 0)
				{
					Definition2D d2d = GetDefinition2D(m, def);

					if (!def.IsDefined)
					{
						d2d.AddShapes(curVo, def, m);
						//parentStack.Peek().Definition.AddShapes(curVo, def, m);
					}

					Instance2D i2d;
					if (addInst)
					{
						i2d = AddInstance(inst, def);
					}
					else
					{
						i2d = CreateInstance2D(def);
					}

					parentStack.Push(i2d);
					ParseTimeline((Timeline)def);
					i2d.Definition.IsDefined = true;
					parentStack.Pop();
				}
			}

			def.IsDefined = true;
        }
        private Instance2D AddInstance(Instance inst, IDefinition def)
        {
            Instance2D result = null;
			if (inst != null)
			{
				result = CreateInstance2D(inst, def);
				if (!parentStack.Peek().Definition.IsDefined)
				{
					parentStack.Peek().Definition.Children.Add(result);
				}
			}
            return result;
        }
        private Instance2D CreateInstance2D(Instance inst, IDefinition def)
        {
			//MatrixComponents mc = inst.Transformations[0].Matrix.GetMatrixComponents();
			//Instance2D result = new Instance2D(inst.Name, def.Name, mc.TranslateX, mc.TranslateY, (float)(mc.Rotation * Math.PI / 180), mc.ScaleX, mc.ScaleY);
            Instance2D result = new Instance2D(inst.Name, def.Name, 0,0,0,1,1);
            result.Depth = inst.Depth;
            result.Transforms = inst.Transformations;
            result.Definition = definitions.Find(d => d.Id == inst.DefinitionId);
            result.StartFrame = curVo.GetFrameFromMilliseconds(inst.StartTime);
            result.TotalFrames = curVo.GetFrameFromMilliseconds(inst.EndTime) - result.StartFrame - 1; // end frame is last ms of frame, so -1
            return result;
        }
        private Instance2D CreateInstance2D(IDefinition def)
        {
			//MatrixComponents mc = Matrix.Identitiy.GetMatrixComponents();
			//Instance2D result = new Instance2D(def.Name, def.Name, mc.TranslateX, mc.TranslateY, (float)(mc.Rotation * Math.PI / 180), mc.ScaleX, mc.ScaleY);
            Instance2D result = new Instance2D(def.Name, def.Name, 0,0,0,1,1);
            result.Depth = 0;
            result.Transforms = new List<Transform>();
            result.Transforms.Add(new Transform(0, 0, Matrix.Identitiy, 1, ColorTransform.Identity));
            result.Definition = definitions.Find(d => d.Id == def.Id);
            result.StartFrame = 0;
            result.TotalFrames = 0; 
            return result;
        }
        private Definition2D GetDefinition2D(Matrix m, IDefinition def)
        {
            Definition2D result = definitions.Find(d => d.DefinitionName == def.Name);
            if (result == null)
            {
                result = CreateDefinition2D(def);
                definitions.Add(result);
            }
            return result;
        }
        private Definition2D CreateDefinition2D(IDefinition def)
        {
            Definition2D result = new Definition2D();

            result.Id = def.Id;
            result.DefinitionName = def.Name; 
            // todo: get linkage names from export assests tags (in vex format) 
            result.LinkageName = def.Name;
            result.Bounds = def.StrokeBounds;

            if (def is Timeline)
            {
                Timeline tlDef = (Timeline)def;
                result.FrameCount = tlDef.FrameCount;
            }
            else if (def is Symbol)
            {
//              Symbol instDef = (Symbol)def;
//              result.StartTime = (int)instDef.StartTime;
//              result.EndTime = (int)instDef.EndTime;
                ParseBodyImage(result, def);
            }

            return result;
        }

		private Definition2D GetTextDefinition(Matrix m, Text def)
		{
			Definition2D result = definitions.Find(d => d.DefinitionName == def.Name);
			if (result == null)
			{
				result = CreateTextDefinition(def);
				definitions.Add(result);
			}
			return result;
		}
		private Text2D CreateTextDefinition(Text def)
		{
			Text2D result = new Text2D(def.TextRuns);

			result.Id = def.Id;
			result.DefinitionName = def.Name;
			result.LinkageName = def.Name;
			result.Bounds = def.StrokeBounds;
			AddFonts(def.TextRuns);
			return result;
		}
		private void AddFonts(List<TextRun> runs)
		{
			for (int i = 0; i < runs.Count; i++)
			{
				if (!fontNames.Contains(runs[i].FontName))
				{
					fontNames.Add(runs[i].FontName);
				}
			}
		}
        private void ParseJoint(V2DJointKind jointKind, Instance inst)
        {
            Dictionary<string, Joint2D> joints = parentStack.Peek().Definition.Joints;
            string nm = inst.Name;
            int firstUs = nm.IndexOf('_');
            string jName = (firstUs > -1) ? nm.Substring(0, firstUs) : nm;
            string targName = (firstUs > -1) ? nm.Substring(firstUs + 1) : "";

            Matrix m = inst.Transformations[0].Matrix;
            float rot = (float)((m.GetMatrixComponents().Rotation + 90) * Math.PI / 180);

            Point p = new Point(
                inst.Transformations[0].Matrix.TranslateX,
                inst.Transformations[0].Matrix.TranslateY);

            if (joints.ContainsKey(jName))
            {
                joints[jName].AppendData(p, rot);
            }
            else
            {
                joints.Add(jName, new Joint2D(jName, targName, jointKind, p, rot));
            }

            if (inst.Transformations.Count > 1) // pulley defined by joints on frame 2
            {
                joints[jName].Transforms.Add(new Point(
                    inst.Transformations[1].Matrix.TranslateX,
                    inst.Transformations[1].Matrix.TranslateY));
            }
        }
        private void ParseBodyImage(Definition2D b2d, IDefinition sy)
        {
            string bmpPath = AddSymbolImage(sy);
        }
        private string AddSymbolImage(IDefinition sy)
        {
            string nm = (sy.Name == null) ? "sym_" + sy.Id.ToString() : sy.Name;
            string bmpPath = resourceFolder + @"\" + curVo.Name + @"\" + nm + ".png";

            bmpPath = OutputDirectory + "/" + bmpPath;
            bmpPath = bmpPath.Replace('\\', '/');

            if (!usedImages.ContainsKey(bmpPath))
            {
                usedImages.Add(bmpPath, sy);
            }
            return bmpPath;
        }

        private void GenerateJointActionData(Joint2D jnt, Definition2D body)
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
        private List<Instance2D> FindTargetUnderPoint(Point p)
        {
            List<Instance2D> result = new List<Instance2D>();
            foreach (Instance2D b in parentStack.Peek().Definition.Children)
            {
                if (b.IsPointInside(p))
                {
                    result.Add(b);
                }
            }
            result.Sort((x, y) => x.Depth.CompareTo(y.Depth));

            return result;
        }
        private void EnsureBodyTags(Joint2D j)
        {
            bool hasB1 = j.data.ContainsKey("body1");
            bool hasB2 = j.data.ContainsKey("body2");
            bool b1FromCode = hasB1;

            if (!hasB1 || !hasB2)
            {
                List<Instance2D> hits = FindTargetUnderPoint(j.Locations[0]);
                if (!hasB1 && hits.Count > 0)
                {
                    j.data.Add("body1", hits[0].InstanceName);
                    hasB1 = true;
                }

                if (!hasB2)
                {
                    Instance2D b2 = null;
                    if (j.Locations.Count > 1)// && j.JointKind == JointKind.Distance)
                    {
                        List<Instance2D> b2L = FindTargetUnderPoint(j.Locations[1]);

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
            GenerateJointActionData(j, parentStack.Peek().Definition);

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
        private void GenerateJointData(Definition2D def)
        {
			if (!def.IsDefined) // todo: really really need to do this isDefined crap right.
			{
				foreach (Joint2D j in def.Joints.Values)
				{
					string jName = j.Name;
					EnsureBodyTags(j);
					string b1Name = j.data["body1"];
					string b2Name = j.data["body2"];
					Instance2D b1Body = (b1Name == V2D.ROOT_NAME) ? rootInstance : parentStack.Peek().Definition.GetChildByName(b1Name);
					Instance2D b2Body = (b2Name == V2D.ROOT_NAME) ? rootInstance : parentStack.Peek().Definition.GetChildByName(b2Name);

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
						List<Transform> tr = (b1Body.Transforms.Count > 1) ? b1Body.Transforms : b2Body.Transforms;
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
        }


        public void FilterMarkers()
        {
            List<uint> ignoreSymbols = new List<uint>();
            //for (uint i = 0; i < curVo.Definitions.Count; i++)
            foreach (IDefinition d in curVo.Definitions.Values)
            {
                //IDefinition d = curVo.Definitions[i + 1];
                string name = d.Name;
                if (name != null && jointKinds.Contains(name))
                {
                    d.UserData = (int)DefinitionKind.JointMarker;
                    if (d is Timeline && ((Timeline)d).Instances.Count > 0)
                    {
                        ignoreSymbols.Add(((Timeline)d).Instances[0].DefinitionId);
                    }
                }
                else if (name != null && shapeKinds.Contains(name))
                {
                    d.UserData = (int)DefinitionKind.ShapeMarker;
                    if (d is Timeline && ((Timeline)d).Instances.Count > 0)
                    {
                        ignoreSymbols.Add(((Timeline)d).Instances[0].DefinitionId);
                    }
                }
                else if (d is Text)
                {
                    d.UserData = (int)DefinitionKind.TextField;
                }
                else if (d is Timeline)
                {
                    Timeline tl = (Timeline)d;
                    if (IsVex2DSymbol(d))
                    {
                        d.UserData |= (int)DefinitionKind.Vex2D;
                    }
                    else
                    {
                        d.UserData = (int)DefinitionKind.Timeline;
                        //bool hasChildren = false;
                        //for (int chi = 0; chi < tl.Instances.Count; chi++)
                        //{
                        //    if (curVo.Definitions[tl.Instances[chi].DefinitionId] is Timeline)
                        //    {
                        //        hasChildren = true;
                        //        break;
                        //    }
                        //}

                        //if (hasChildren)
                        //{
                        //    d.GetUserData() = (int)DefinitionKind.Timeline;
                        //}
                        //else
                        //{
                        //    d.GetUserData() = (int)DefinitionKind.Symbol;
                        //}
                    }
                }
                else if (d is Symbol)
                {
                    d.UserData = (int)DefinitionKind.Symbol;
                }

            }

            // ignore placeholder symbols
            for (int i = 0; i < ignoreSymbols.Count; i++)
            {
                curVo.Definitions[ignoreSymbols[i]].UserData = (int)DefinitionKind.Ignore;
            }
        }

        private void ParseActions(DDW.Swf.SwfCompilationUnit scu)
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
        private bool IsVex2DSymbol(IDefinition def)
        {
            bool result = false;
            if (def is Symbol)
            {
                List<Shape> shapes = ((Symbol)def).Shapes;
                foreach (Shape sh in shapes)
                {
                    if (sh.IsV2DShape())
                    {
                        result = true;
                        def.UserData |= (int)DefinitionKind.OutlineStroke;
                        break;
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

            return result;
        }
    }

}
