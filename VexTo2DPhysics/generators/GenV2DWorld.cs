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
                V2DDefinition d = new V2DDefinition();
                v2dWorld.definitions.Add(d);
                d.Name = def.DefinitionName;
                d.Id = def.Id;
                d.LinkageName = def.LinkageName;// paths.ContainsKey(def.Id) ? paths[def.Id] : "";
                d.FrameCount = def.FrameCount;
                d.OffsetX = def.Bounds.Point.X;
                d.OffsetY = def.Bounds.Point.Y;
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
            result.Alpha = inst.Alpha;
            result.DefinitionName = inst.DefinitionName;
            result.Density = inst.Density;
            result.Depth = inst.Depth;
            result.Friction = inst.Friction;
            result.InstanceName = inst.InstanceName;
            //result.Joints = inst.Joints;
            DDW.Vex.Matrix m = inst.Matrix;
            result.Transform = new V2DMatrix(m.ScaleX, m.Rotate0, m.Rotate1, m.ScaleY, m.TranslateX, m.TranslateY);
            result.Restitution = inst.Restitution;
            result.Rotation = inst.Rotation;
            result.ScaleX = inst.ScaleX;
            result.ScaleY = inst.ScaleY;
            result.StartFrame = (int)inst.StartFrame;
            result.TotalFrames = (int)inst.TotalFrames;
            //result.Transform = inst.Transforms;
            result.Visible = inst.Visible;
            result.X = inst.X;
            result.Y = inst.Y;
            return result;

            //Dictionary<string, string> dict = null;
            //if (v2d.codeData.ContainsKey(inst.InstanceName))
            //{
            //    dict = v2d.codeData[inst.InstanceName];
            //}
            //return inst.GetV2DInstance(dict);
        }
    }
}
