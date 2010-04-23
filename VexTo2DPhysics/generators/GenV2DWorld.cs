using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Drawing;
using DDW.V2D;
using DDW.V2D.Serialization;
using System.Reflection;
using DDW.Placeholder;
using DDW.Vex;

namespace DDW.VexTo2DPhysics
{
    public class GenV2DWorld
    {
        public V2DWorld v2dWorld;
        private VexTree v2d;
        private List<string> parsedDefs = new List<string>();
        private Dictionary<string, IDefinition> usedImages;

        public GenV2DWorld(VexTree v2d, Dictionary<string, IDefinition> usedImages)
        {
            this.v2d = v2d;
            this.usedImages = usedImages;
            v2dWorld = new V2DWorld();
        }

        public void Generate()
        {
            foreach (string k in v2d.worldData.Keys)
            {
                Type t = typeof(V2DWorld);
                FieldInfo fi = t.GetField(k, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (fi != null)
                {
                    Type ft = fi.FieldType;
                    string val = v2d.worldData[k];
                    Object o = Convert.ChangeType(val, fi.FieldType);
                    fi.SetValue(v2dWorld, o);
                }
            }

            parsedDefs.Clear();

            v2dWorld.textures.Clear();
            WriteTextures(v2dWorld);

            v2dWorld.definitions.Clear();
            WriteDefinitions(v2dWorld);
        }
        private void WriteTextures(V2DWorld v2dWorld)
        {
            List<string> parsed = new List<string>();
            foreach (string key in usedImages.Keys)
            {
                V2DTexture t = new V2DTexture();
                t.Source = key;// usedImages[key];
                v2dWorld.textures.Add(t);
            }
        }
        private void WriteDefinitions(V2DWorld v2dWorld)
        {
            foreach (Definition2D def in v2d.definitions)
            {
				V2DDefinition d;
				if (def is Text2D)
				{
					V2DText v2t = new V2DText();
					d = v2t;
					v2t.TextRuns = ((Text2D)def).TextRuns;
				}
				else
				{
					d = new V2DDefinition();
					d.V2DShapes.Clear();
					for (int i = 0; i < def.Shapes.Count; i++)
					{
						d.V2DShapes.Add(def.Shapes[i].GetV2DShape());
					}

					d.Instances.Clear();
					if (def.Children.Count > 0)
					{
						for (int i = 0; i < def.Children.Count; i++)
						{
							d.Instances.Add(GetV2DInstance(def.Children[i]));
						}
					}
				}

				v2dWorld.definitions.Add(d);
				d.Name = def.DefinitionName;
				d.Id = def.Id;
				d.LinkageName = def.LinkageName;// paths.ContainsKey(def.Id) ? paths[def.Id] : "";
				d.FrameCount = def.FrameCount;
				d.OffsetX = def.Bounds.Point.X;
				d.OffsetY = def.Bounds.Point.Y;
				d.Width = def.Bounds.Size.Width;
				d.Height = def.Bounds.Size.Height;

				d.Joints.Clear();
				if (def.Joints.Count > 0)
				{
					WriteJointData(d.Joints, def);
				}
            }
        }
        private void WriteJointData(List<V2DJoint> defJoints, Definition2D def)
        {
            Joint2D[] jts = new Joint2D[def.Joints.Count];
            def.Joints.Values.CopyTo(jts, 0);
            List<Joint2D> jList = new List<Joint2D>(jts);
            jList.Sort();

            foreach (Joint2D j in jList)
            {
                defJoints.Add(j.GetV2DJoint());
            }
        }
        private V2DInstance GetV2DInstance(Instance2D inst)
        {
            V2DInstance result = new V2DInstance();
            result.DefinitionName = inst.DefinitionName;
            result.Density = inst.Density;
            result.Depth = inst.Depth;
            result.Friction = inst.Friction;
            result.InstanceName = inst.InstanceName;
            //result.Joints = inst.Joints;
            DDW.Vex.Matrix m = inst.Matrix;
            //result.Matrix = new V2DMatrix(m.ScaleX, m.Rotate0, m.Rotate1, m.ScaleY, m.TranslateX, m.TranslateY);
            result.Restitution = inst.Restitution;
            result.StartFrame = inst.StartFrame;
            result.EndFrame = inst.TotalFrames + inst.StartFrame;
            result.Visible = inst.Visible;

            result.Alpha = inst.Alpha;
            result.X = inst.X;
            result.Y = inst.Y;
            result.Rotation = inst.Rotation;
            result.ScaleX = inst.ScaleX;
            result.ScaleY = inst.ScaleY;
            result.Transforms = TransformsConversion(result, inst.Transforms);

            return result;

            //Dictionary<string, string> dict = null;
            //if (v2d.codeData.ContainsKey(inst.InstanceName))
            //{
            //    dict = v2d.codeData[inst.InstanceName];
            //}
            //return inst.GetV2DInstance(dict);
        }
		public V2DGenericTransform[] TransformsConversion(V2DInstance obj, List<Transform> trs)
        {
            V2DGenericTransform[] result = new V2DGenericTransform[trs.Count];
			if (trs.Count > 0)
			{
				MatrixComponents firstMc = trs[0].Matrix.GetMatrixComponents();
				float firstAlpha = trs[0].Alpha;
				obj.X += firstMc.TranslateX;
				obj.Y += firstMc.TranslateY;
				obj.Rotation += (float)(firstMc.Rotation * Math.PI / 180);
				obj.ScaleX *= firstMc.ScaleX;
				obj.ScaleY *= firstMc.ScaleY;
				obj.Alpha *= firstAlpha;

				for (int i = 0; i < trs.Count; i++)
				{
					Transform tin = trs[i];

					uint sf = (uint)Math.Round(tin.StartTime / (1000d / v2dWorld.FrameRate));
					uint ef = (uint)Math.Round(tin.EndTime / (1000d / v2dWorld.FrameRate)) - 1;
					MatrixComponents mc = tin.Matrix.GetMatrixComponents();
                    V2DGenericTransform tout = new V2DGenericTransform(
						sf,
						ef,
						mc.ScaleX / firstMc.ScaleX,
						mc.ScaleY / firstMc.ScaleY,
						(float)(mc.Rotation * Math.PI / 180) - (float)(firstMc.Rotation * Math.PI / 180),
						mc.TranslateX - firstMc.TranslateX,
						mc.TranslateY - firstMc.TranslateY,
						tin.Alpha / firstAlpha);

					//V2DTransform tout = new V2DTransform(
					//    (uint)Math.Floor(tin.StartTime / (1000d / v2dWorld.FrameRate)), 
					//    (uint)Math.Floor(tin.EndTime / (1000d / v2dWorld.FrameRate)),
					//    new V2DMatrix(  tin.Matrix.ScaleX, tin.Matrix.Rotate0, tin.Matrix.Rotate1, tin.Matrix.ScaleX, 
					//                    tin.Matrix.TranslateX, tin.Matrix.TranslateY),
					//    tin.Alpha);

					tout.IsTweening = tin.IsTweening;

					result[i] = tout;
				}
			}
            return result;
        }
    }
}
