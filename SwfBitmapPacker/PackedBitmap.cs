using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DDW.SwfBitmapPacker
{
    public class PackedBitmap
    {

        public Bitmap FullBitmap;
        public List<Rectangle> BitmapRectangles = new List<Rectangle>();
        public Dictionary<string, int> BitmapNames = new Dictionary<string, int>();
    }
}
