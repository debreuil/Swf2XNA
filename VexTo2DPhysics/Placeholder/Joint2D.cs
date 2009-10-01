/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using DDW.Vex;
using DDW.V2D;

namespace DDW.Placeholder
{
    public class Joint2D : Object2D, IComparable
    {
        public string Name;
        public string TargetName;
        public V2DJointKind JointKind;
        public List<Point> Locations = new List<Point>();
        public List<Point> Transforms = new List<Point>(); // for pulley joint
        public List<float> Rotations = new List<float>();
        public Dictionary<string, string> data = new Dictionary<string, string>();

        public Joint2D(string name, string targetName, V2DJointKind kind, Point location, float rotation)
        {
            this.Name = name;
            this.TargetName = targetName;
            this.JointKind = kind;
            this.Locations.Add(location);
            this.Rotations.Add(rotation);
        }
        public void AppendData(Point location, float rotation)
        {
            this.Locations.Add(location);
            this.Rotations.Add(rotation);
        }
        public override void Add(string key, string value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }
        public virtual V2DJoint GetV2DJoint()
        {
            V2DJoint result = new V2DJoint();
            result.Type = this.JointKind;
            result.Name = this.Name;

            foreach (string k in data.Keys)
            {
                Type t = result.GetType();
                FieldInfo fi = t.GetField(k, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (fi != null)
                {
                    Type ft = fi.FieldType;
                    string val = data[k];
                    Object o = Convert.ChangeType(val, fi.FieldType); 
                    fi.SetValue(result, o);
                }
            }
            return result;
        }
        public int CompareTo(object o)
        {
            int result = -1;
            if (o is Joint2D)
            {
                result = this.JointKind.CompareTo(((Joint2D)o).JointKind);
            }
            return result;
        }





        public virtual void Dump(StringWriter sw)
        {
            string kind = Enum.GetName(typeof(V2DJointKind), this.JointKind);

            sw.Write("          {" +
                "type:\"" + kind + "\", " +
                "name:\"" + this.Name + "\", "
                );
            string comma = "";
            foreach (string k in data.Keys)
            {
                sw.Write(comma);
                this.DumpValue(sw, k, data[k]);
                comma = ", ";
            }
            sw.Write("}");
        }
        public virtual void Dump(XmlWriter xw)
        {
            string kind = Enum.GetName(typeof(V2DJointKind), this.JointKind);

            xw.WriteStartElement("V2DJoint");

            xw.WriteAttributeString("Type", kind);
            xw.WriteAttributeString("Name", Name);

            foreach (string k in data.Keys)
            {
                this.DumpValue(xw, k, data[k]);
            }

            xw.WriteEndElement(); // V2DJoint

        }
    }
}
