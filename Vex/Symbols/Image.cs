/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using Vex = DDW.Vex;
using SysDraw = System.Drawing;
using System.Xml.Serialization;

namespace DDW.Vex
{
	public class Image : IDefinition
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public Rectangle StrokeBounds { get; set; }
        public string Path { get; set; }
        public Point Center { get { return Point.Empty; } }
        [XmlIgnore]
        public int UserData { get; set; }
        [XmlIgnore]
        public bool HasSaveableChanges { get; set; }
        public string WorkingPath { get; set; }

        public Image()
        {
        }
        public Image(string path): this(path, 0)
        {
        }
        public Image(string path, uint id)
        {
            this.Path = path;
            this.Id = id;
        }
	}
}
