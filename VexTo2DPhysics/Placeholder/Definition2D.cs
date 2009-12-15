/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using DDW.Vex;
using DDW.V2D;
using System.Reflection;
using DDW.VexTo2DPhysics;

namespace DDW.Placeholder
{
    /// <summary>
    /// Temporary holder for conversion to V2DDefinition
    /// </summary>
    public class Definition2D
    {
        public uint Id;
        public string DefinitionName;
        public string LinkageName;
        public uint FrameCount;
        //public int Duration;
        public Rectangle Bounds;
        public List<Instance2D> Children = new List<Instance2D>();
        public List<Shape2D> Shapes = new List<Shape2D>();
        public Symbol2D Symbol;
        public Dictionary<string, Joint2D> Joints = new Dictionary<string, Joint2D>();

        //public Body2D(string instanceName, string typeName, int depth)
        //    : this(instanceName, typeName, depth, null)
        //{
        //}
        public Definition2D()
        {
        }

        public Instance2D GetChildByName(string name)
        {
            Instance2D result = null;
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].InstanceName == name)
                {
                    result = Children[i];
                    break;
                }
            }
            return result;
        }

        public void SetSymbol(string name, string path)
        {
            this.Symbol = new Symbol2D(name, path);
        }
        public bool HasShapes()
        {
            return Shapes.Count > 0;
        }
        public void AddShapes(VexObject vo, IDefinition def, Matrix m)
        {
            if (def.Name == "circleShape")
            {
                //Matrix m = orgInst.Transformations[0].Matrix;
                Point c = new Point(m.TranslateX, m.TranslateY);
                float r = m.ScaleX * 100 / 2;
                Shapes.Add(new CircleShape2D(def.Name, c, r));
            }
            else if (def is Symbol)
            {
                //Matrix m = orgInst.Transformations[0].Matrix;
                AddShape(def.Name, (Symbol)def, m);
            }
            else if (def is Timeline)
            {
                foreach (Instance inst in ((Timeline)def).Instances)
                {
                    IDefinition def2 = vo.Definitions[inst.DefinitionId];
                    if (def2 is Symbol && def2.UserData == (int)DefinitionKind.OutlineStroke)
                    {
                        //Matrix m = inst.Transformations[0].Matrix;
                        AddShape(def.Name, (Symbol)def2, inst.Transformations[0].Matrix);
                    }
                }
            }
        }
        public void AddShape(string name, Symbol sy, Matrix m)
        {
            for (int i = 0; i < sy.Shapes.Count; i++)
            {
                if (sy.Shapes[i].ShapeData.Count > 2) // must have 3 point for triangle
                {
                    Shapes.Add(new PolygonShape2D(name, sy.Shapes[i], m));
                }
            }             
        }

        //public void DumpInstance(StringWriter sw, Dictionary<string, string> dict)
        //{
        //    MatrixComponents mc = this.Transforms[0].Matrix.GetMatrixComponents();
        //    Point p = new Point(mc.TranslateX, mc.TranslateY);
        //    float rot = (float)(mc.Rotation * Math.PI / 180);
        //    float sx = mc.ScaleX;
        //    float sy = mc.ScaleY;

        //    sw.Write("          {" +
        //        "instName:\"" + this.InstanceName + "\", " +
        //        "type:\"" + this.DefinitionName + "\", " + 
        //        "location:" +  p.ToString()
        //        );

        //    if (rot != 0)
        //    {
        //        sw.Write(", rotation:" + rot);
        //    }
        //    if (Math.Abs(sx - 1) > .01)
        //    {
        //        sw.Write(", scaleX:" + sx);
        //    }
        //    if (Math.Abs(sy - 1) > .01)
        //    {
        //        sw.Write(", scaleY:" + sy);
        //    }
        //    if (!Transforms[0].ColorTransform.IsIdentity())
        //    {
        //        sw.Write(", colorTransform:" + Transforms[0].ColorTransform.ToString());
        //    }

        //    if (dict != null)
        //    {
        //        foreach (string k in dict.Keys)
        //        {
        //            sw.Write(", " + k + ":" + dict[k]);
        //        }
        //    }
        //    sw.Write("}");
        //}
        //public V2DInstance GetV2DInstance(Dictionary<string, string> dict)
        //{
        //    V2DInstance result = new V2DInstance();

        //    MatrixComponents mc = new MatrixComponents(1, 1, 0, 0, 0, 0);// this.Transforms[0].Matrix.GetMatrixComponents();
        //    Point p = new Point(mc.TranslateX, mc.TranslateY);
        //    float rot = (float)(mc.Rotation * Math.PI / 180);
        //    float sx = mc.ScaleX;
        //    float sy = mc.ScaleY;

        //    result.LikageName = LikageName;
        //    result.DefinitionName = DefinitionName;
        //    result.X = p.X;
        //    result.Y = p.Y;
        //    result.Depth = Depth;

        //    if (rot != 0)
        //    {
        //        result.Rotation = rot;
        //    }
        //    if (Math.Abs(sx - 1) > .01)
        //    {
        //        result.ScaleX = sx;
        //    }
        //    if (Math.Abs(sy - 1) > .01)
        //    {
        //        result.ScaleY = sy;
        //    }
        //    //if (!Transforms[0].ColorTransform.IsIdentity())
        //    //{
        //    //    // todo: color xform
        //    //}

        //    if (dict != null)
        //    {
        //        foreach (string k in dict.Keys)
        //        {
        //            Type t = result.GetType();
        //            FieldInfo fi = t.GetField(k, BindingFlags.IgnoreCase);
        //            if (fi != null)
        //            {
        //                Type ft = fi.FieldType;
        //                string val = dict[k];
        //                Object o = Convert.ChangeType(val, fi.FieldType);
        //                fi.SetValue(result, o);
        //            }
        //        }
        //    }
        //    return result;
        //}

        public void DumpInstance(XmlWriter xw, Dictionary<string, string> dict)
        {
            xw.WriteStartElement("V2DInstance");

            MatrixComponents mc = new MatrixComponents(1, 1, 0, 0, 0, 0);// this.Transforms[0].Matrix.GetMatrixComponents();
            Point p = new Point(mc.TranslateX, mc.TranslateY);
            float rot = (float)(mc.Rotation * Math.PI / 180);
            float sx = mc.ScaleX;
            float sy = mc.ScaleY;

            xw.WriteAttributeString("DefinitionName", DefinitionName);
            xw.WriteAttributeString("LinkageName", LinkageName);
            xw.WriteAttributeString("X", p.X.ToString());
            xw.WriteAttributeString("Y", p.Y.ToString());

            if (rot != 0)
            {
                xw.WriteAttributeString("Rotation", rot.ToString());
            }
            if (Math.Abs(sx - 1) > .01)
            {
                xw.WriteAttributeString("ScaleX", sx.ToString());
            }
            if (Math.Abs(sy - 1) > .01)
            {
                xw.WriteAttributeString("ScaleY", sy.ToString());
            }
            //if (!Transforms[0].ColorTransform.IsIdentity())
            //{
            //    //sw.Write(", colorTransform:" + Transforms[0].ColorTransform.ToString());
            //}

            if (dict != null)
            {
                foreach (string k in dict.Keys)
                {
                    xw.WriteAttributeString(k, dict[k]);
                }
            }
            xw.WriteEndElement(); // V2DInstance
        }
        public void DumpShapes(StringWriter sw)
        {
            sw.WriteLine("          " + this.DefinitionName + ":");
            sw.WriteLine("          [");

            for (int i = 0; i < Shapes.Count; i++)
            {
                Shape2D sh = Shapes[i];
                sw.Write("            ");
                sh.Dump(sw);
                if (i < Shapes.Count - 1)
                {
                    sw.Write(",");
                }
                sw.WriteLine("");
            }
            sw.WriteLine("          ]");
        }
        public void DumpShapeData(StringWriter sw)
        {
            for (int i = 0; i < Shapes.Count; i++)
            {
                Shape2D sh = Shapes[i];
                sh.Dump(sw);
                if (i < Shapes.Count - 1)
                {
                    sw.Write(",");
                }
            }
        }
        public void DumpShapeData(XmlWriter xw)
        {
            for (int i = 0; i < Shapes.Count; i++)
            {
                Shape2D sh = Shapes[i];
                sh.Dump(xw);
            }
        }

    }
}
