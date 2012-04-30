/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
	/// <summary>
	/// A timeline represents the activity defined within a symbol. 
	/// The activity is defined using one Lifetime per symbol used.
	/// </summary>
	public class Timeline : IDefinition
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public int UserData { get; set; }
        public Rectangle StrokeBounds { get; set; }
        public string Path { get; set; }

		public uint Duration;
		public uint FrameCount = 0;

		public List<IInstance> Instances = new List<IInstance>();
		public Dictionary<uint, string> Labels = new Dictionary<uint, string>();

		public Timeline(uint id)
		{
			this.Id = id;
			this.Name = "$mc" + id.ToString();
		}

        public bool ContainsInstanceId(uint id)
        {
            bool result =false;
            for (int i = 0; i < Instances.Count; i++)
            {
                if (Instances[i].InstanceId == id)
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
                result[i] = Instances[i].InstanceId;
            }
            return result;
        }
		//public IDefinition LookupSymbol(float t, uint depth)
		//{
		//}

	}
}
