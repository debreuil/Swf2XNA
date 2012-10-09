/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using DDW.Vex.Primitives;
using System.Xml.Serialization;
using System.Globalization;

namespace DDW.Vex
{
	/// <summary>
	/// A timeline represents the activity defined within a symbol. 
	/// The activity is defined using one Lifetime per symbol used.
	/// </summary>
	public class Timeline : IDefinition
    {
        public uint Id { get; set; }
        private string name;
        public string Name{ get; set; }
        public Rectangle StrokeBounds { get; set; }
        public string Path { get; set; }
        public Point Center { get { return new Point(-StrokeBounds.Left, -StrokeBounds.Top); } }
        [XmlIgnore]
        public int UserData { get; set; }
        [XmlIgnore]
        public bool HasSaveableChanges { get; set; }
        public string WorkingPath { get; set; }

		public uint Duration;
		public uint FrameCount = 0;

        //public Dictionary<uint, string> Labels = new Dictionary<uint, string>();
        public List<Label> Labels = new List<Label>();

        [XmlIgnore]
        public List<IInstance> Instances = new List<IInstance>();
        [XmlIgnore]
        public uint[] InstanceIds;

        [XmlElement(ElementName = "Instances")]
        public string InstancesString
        {
            get
            {
                string result = "";
                string comma = "";
                for (int i = 0; i < Instances.Count; i++)
                {
                    result += comma + Instances[i].InstanceHash;
                    comma = ",";
                }
                return result;
            }
            set
            {
                // todo: need instanceManager, or store instance as indexes, or deserialize in level up
                string[] ids = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                InstanceIds = new uint[ids.Length];
                for (int i = 0; i < ids.Length; i++)
                {
                    InstanceIds[i] = (uint)int.Parse(ids[i], NumberStyles.Any);
                }
            }
        }

        public Timeline()
        {
        }
        public Timeline(uint id)
		{
			this.Id = id;
			this.Name = "$mc" + id.ToString();
		}

        public int InstanceCount { get { return Instances.Count; } }
        public void AddInstance(IInstance inst)
        {
            Instances.Add(inst);
            inst.ParentDefinitionId = this.Id;
            HasSaveableChanges = true;
        }
        public void InsertInstance(int depth, IInstance inst)
        {
            Instances.Insert(depth, inst);
            inst.ParentDefinitionId = this.Id;
            HasSaveableChanges = true;
        }
        public void RemoveInstance(IInstance inst)
        {
            Instances.Remove(inst);
            inst.ParentDefinitionId = 0;
            HasSaveableChanges = true;
        }
        public void ClearInstances()
        {
            for (int i = 0; i < Instances.Count; i++)
            {
                Instances[i].ParentDefinitionId = 0;
            }
            Instances.Clear();
            HasSaveableChanges = true;
        }
        public IInstance InstanceAt(int index)
        {
            return Instances[index];
        }

        public bool ContainsInstanceId(uint id)
        {
            bool result = false;
            for (int i = 0; i < Instances.Count; i++)
            {
                if (Instances[i].InstanceHash == id)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        public uint[] GetInstanceIds()
        {
            uint[] result = new uint[Instances.Count];
            for (int i = 0; i < Instances.Count; i++)
            {
                result[i] = Instances[i].InstanceHash;
            }
            return result;
        }


        #region Depths
        public int GetInstanceDepth(uint id)
        {
            int result = -1;
            for (int i = 0; i < Instances.Count; i++)
            {
                if (Instances[i].InstanceHash == id)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        public void SwapDepths(int[] from, int[] to)
        {
            if (from.Length == to.Length)
            {
                for (int i = 0; i < from.Length; i++)
                {
                    if (Instances.Count > from[i] && Instances.Count > to[i])
                    {
                        Vex.IInstance temp = Instances[to[i]];
                        Instances[to[i]] = Instances[from[i]];
                        Instances[to[i]].Depth = Instances[from[i]].Depth;
                        Instances[from[i]] = temp;
                        Instances[from[i]].Depth = temp.Depth;
                    }
                }
                HasSaveableChanges = true;
            }
        }
        public void ChangeDepth(int from, int to)
        {
            if (Instances.Count > from && Instances.Count > to)
            {
                Vex.IInstance temp = Instances[from];
                Instances.RemoveAt(from);
                Instances.Insert(to, temp);
            }
            HasSaveableChanges = true;
        }
        #endregion
		//public IDefinition LookupSymbol(float t, uint depth)
		//{
		//}

	}
}
