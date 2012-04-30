/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using Vex = DDW.Vex;
using SysDraw = System.Drawing;

namespace DDW.Vex
{
	public class Image : IDefinition
    {
        private SysDraw.Bitmap image;

        public uint Id { get; set; }
        public string Name { get; set; }
        public int UserData { get; set; }
        public Rectangle StrokeBounds { get; set; }
        public string Path { get; set; }

        public Image(string path) : this(path, 0)
        {
        }
        public Image(string path, uint id)
        {
            this.Path = path;
            this.Id = id;
            GetCachedBitmap();
        }
        public SysDraw.Bitmap GetCachedBitmap()
        {
            if (image == null)
            {
                SysDraw.Image img = SysDraw.Image.FromFile(Path);
                image = new SysDraw.Bitmap(img);
                image.SetResolution(96, 96);
                StrokeBounds = new Vex.Rectangle(0, 0, image.Width, image.Height);
            }
            return image;
        }
	}
}
