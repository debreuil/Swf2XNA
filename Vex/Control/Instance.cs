
/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace DDW.Vex
{
	public class Instance : IInstance
    {
        [XmlIgnore]
		public uint SortOrder { get { return 99; } }

        public uint DefinitionId { get; set; }
        [DefaultValue(0)]
        public uint StartTime { get; set; }
        [DefaultValue(1)]
        public uint EndTime { get; set; }
        [DefaultValue(0)]
        public int Depth { get; set; }
        public string Name { get; set; }
        [DefaultValue(0)]
        public uint ParentDefinitionId { get; set; }
        public Point RotationCenter { get; set; }
        public AspectConstraint AspectConstraint { get; set; }
        [XmlIgnore]
        public bool HasSaveableChanges { get; set; }

        private List<Transform> transformations = new List<Transform>();
        public List<Transform> Transformations { get { return transformations; } }

		private uint instanceHash;
        public uint InstanceHash
        {
            get { return instanceHash; }
            set
            {
                instanceHash = value;
            }
        }


        [DefaultValue(false)]
        public bool IsMask = false;
        [DefaultValue(0)]
		public uint MaskDepth = 0;

        public Instance()
        {
            HasSaveableChanges = true;
        }

        [XmlIgnore]
        public Point Location
        {
            get { return GetTransformAtTime(0).Matrix.Location; }
            set
            {
                if (transformations.Count == 0)
                {
                    Matrix m = new Matrix(1, 0, 0, 1, value.X, value.Y);
                    transformations.Add(new Transform(0, 1, m, 1, ColorTransform.Identity));
                }
                else
                {
                    Matrix m = GetTransformAtTime(0).Matrix;
                    m.TranslateX = value.X;
                    m.TranslateY = value.Y;
                    GetTransformAtTime(0).Matrix = m;
                }

            }
        }

		public int CompareTo(Object o)
		{
			int result = 0;
			if (o is Instance)
			{
				Instance inst = (Instance)o;
				if (this.Depth != inst.Depth)
				{
					result = ((int)this.Depth) > ((int)inst.Depth) ? 1 : -1;
				}	
			}
			else if (o is IInstance)
			{
				IInstance inst = (IInstance)o;
				result = ((int)this.SortOrder) > ((int)inst.SortOrder) ? 1 : -1;
			}
			else
			{
				throw new ArgumentException("Objects being compared are not of the same type");
			}
			return result;
		}

        public Transform GetTransformAtTime(uint time)
        {            
            //Transform result = null;
            //int index = transformations.FindIndex(item => (item.StartTime <= time) && (item.EndTime >= time) );
            //if (index > -1)
            //{
            //    result = transformations[index];
            //}
            //return result;

            Transform result;
            if (transformations.Count == 0)
            {
                result = Transform.Identity;
            }
            else
            {
                int index = transformations.FindIndex(item => (item.StartTime <= time) && (item.EndTime >= time));
                if (index > -1)
                {
                    result = transformations[index];
                }
                else
                {
                    result = Transform.Identity;
                }
            }

            return result;
        }

		//private uint endTime;
		//public uint EndTime
		//{
		//    get
		//    {
		//        return endTime;
		//    }
		//    set
		//    {
		//        endTime = value;
		//    }
		//}
	}
}
