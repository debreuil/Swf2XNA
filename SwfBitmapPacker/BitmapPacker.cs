using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DDW.SwfBitmapPacker
{
    // based on SpriteSheetWindows for XNA
    public class BitmapPacker
    {
        public static UnsafeBitmap PackBitmaps(Dictionary<uint, string> sourceBitmapPaths, Dictionary<uint, Rectangle> outputBitmaps)
        {
            if (sourceBitmapPaths.Count == 0)
            {
                throw new Exception("There are no bitmaps to arrange");
            }
            List<Bitmap> sourceBitmaps = new List<Bitmap>(sourceBitmapPaths.Count);
            List<ArrangedBitmap> bitmaps = new List<ArrangedBitmap>();
            //for (int i = 0; i < sourceBitmapPaths.Count; i++)
            int index = 0;
            foreach(KeyValuePair<uint, string> pair in sourceBitmapPaths)
            {
                ArrangedBitmap bitmap = new ArrangedBitmap();
                sourceBitmaps.Add(new Bitmap(pair.Value));
                bitmap.Width = sourceBitmaps[index].Width + 2;
                bitmap.Height = sourceBitmaps[index].Height + 2;
                bitmap.Index = index;
                bitmap.Id = pair.Key;
                bitmaps.Add(bitmap);
                index++;
            }

            // Sort so the largest bitmaps get arranged first.
            bitmaps.Sort(CompareBitmapSizes);

            // Work out how big the output bitmap should be.
            int outputWidth = GuessOutputWidth(bitmaps);
            int outputHeight = 0;
            int totalBitmapSize = 0;

            // Choose positions for each bitmap, one at a time.
            for (int i = 0; i < bitmaps.Count; i++)
            {
                PositionBitmap(bitmaps, i, outputWidth);

                outputHeight = Math.Max(outputHeight, bitmaps[i].Y + bitmaps[i].Height);

                totalBitmapSize += bitmaps[i].Width * bitmaps[i].Height;
            }

            // Sort the bitmaps back into index order.
            bitmaps.Sort(CompareBitmapIndices);

            Console.WriteLine("Packed {0} bitmaps into a {1}x{2} sheet, {3}% efficiency",
                bitmaps.Count, outputWidth, outputHeight,
                totalBitmapSize * 100 / outputWidth / outputHeight);

            return CopyBitmapsToOutput(bitmaps, sourceBitmaps, outputBitmaps,
                                       outputWidth, outputHeight);
        }


        /// <summary>
        /// Once the arranging is complete, copies the bitmap data for each
        /// bitmap to its chosen position in the single larger output bitmap.
        /// </summary>
        static UnsafeBitmap CopyBitmapsToOutput(List<ArrangedBitmap> bitmaps,
                                                 IList<Bitmap> sourceBitmaps,
                                                 Dictionary<uint, Rectangle> outputBitmaps,
                                                 int width, int height)
        {
            UnsafeBitmap output = new UnsafeBitmap(width, height);

            foreach (ArrangedBitmap bitmap in bitmaps)
            {
                Bitmap source = sourceBitmaps[bitmap.Index];

                int x = bitmap.X;
                int y = bitmap.Y;

                int w = source.Width;
                int h = source.Height;

                // Copy the main bitmap data to the output sheet.
                UnsafeBitmap.Copy(source, new Rectangle(0, 0, w, h),
                                   output, new Rectangle(x + 1, y + 1, w, h));

                // Copy a border strip from each edge of the bitmap, creating
                // a one pixel padding area to avoid filtering problems if the
                // bitmap is scaled or rotated.
                UnsafeBitmap.Copy(source, new Rectangle(0, 0, 1, h),
                                   output, new Rectangle(x, y + 1, 1, h));

                UnsafeBitmap.Copy(source, new Rectangle(w - 1, 0, 1, h),
                                   output, new Rectangle(x + w + 1, y + 1, 1, h));

                UnsafeBitmap.Copy(source, new Rectangle(0, 0, w, 1),
                                   output, new Rectangle(x + 1, y, w, 1));

                UnsafeBitmap.Copy(source, new Rectangle(0, h - 1, w, 1),
                                   output, new Rectangle(x + 1, y + h + 1, w, 1));

                // Copy a single pixel from each corner of the bitmap,
                // filling in the corners of the one pixel padding area.
                UnsafeBitmap.Copy(source, new Rectangle(0, 0, 1, 1),
                                   output, new Rectangle(x, y, 1, 1));

                UnsafeBitmap.Copy(source, new Rectangle(w - 1, 0, 1, 1),
                                   output, new Rectangle(x + w + 1, y, 1, 1));

                UnsafeBitmap.Copy(source, new Rectangle(0, h - 1, 1, 1),
                                   output, new Rectangle(x, y + h + 1, 1, 1));

                UnsafeBitmap.Copy(source, new Rectangle(w - 1, h - 1, 1, 1),
                                   output, new Rectangle(x + w + 1, y + h + 1, 1, 1));

                // Remember where we placed this bitmap.
                outputBitmaps.Add(bitmap.Id, new Rectangle(x + 1, y + 1, w, h));
            }

            return output;
        }


        /// <summary>
        /// Internal helper class keeps track of a bitmap while it is being arranged.
        /// </summary>
        class ArrangedBitmap
        {
            public int Index;
            public uint Id;

            public int X;
            public int Y;

            public int Width;
            public int Height;
        }


        /// <summary>
        /// Works out where to position a single bitmap.
        /// </summary>
        static void PositionBitmap(List<ArrangedBitmap> bitmaps,
                                   int index, int outputWidth)
        {
            int x = 0;
            int y = 0;

            while (true)
            {
                // Is this position free for us to use?
                int intersects = FindIntersectingBitmap(bitmaps, index, x, y);

                if (intersects < 0)
                {
                    bitmaps[index].X = x;
                    bitmaps[index].Y = y;

                    return;
                }

                // Skip past the existing bitmap that we collided with.
                x = bitmaps[intersects].X + bitmaps[intersects].Width;

                // If we ran out of room to move to the right,
                // try the next line down instead.
                if (x + bitmaps[index].Width > outputWidth)
                {
                    x = 0;
                    y++;
                }
            }
        }


        /// <summary>
        /// Checks if a proposed bitmap position collides with anything
        /// that we already arranged.
        /// </summary>
        static int FindIntersectingBitmap(List<ArrangedBitmap> bitmaps,
                                          int index, int x, int y)
        {
            int w = bitmaps[index].Width;
            int h = bitmaps[index].Height;

            for (int i = 0; i < index; i++)
            {
                if (bitmaps[i].X >= x + w)
                    continue;

                if (bitmaps[i].X + bitmaps[i].Width <= x)
                    continue;

                if (bitmaps[i].Y >= y + h)
                    continue;

                if (bitmaps[i].Y + bitmaps[i].Height <= y)
                    continue;

                return i;
            }

            return -1;
        }


        /// <summary>
        /// Comparison function for sorting bitmaps by size.
        /// </summary>
        static int CompareBitmapSizes(ArrangedBitmap a, ArrangedBitmap b)
        {
            int aSize = a.Height * 1024 + a.Width;
            int bSize = b.Height * 1024 + b.Width;

            return bSize.CompareTo(aSize);
        }


        /// <summary>
        /// Comparison function for sorting bitmaps by their original indices.
        /// </summary>
        static int CompareBitmapIndices(ArrangedBitmap a, ArrangedBitmap b)
        {
            return a.Index.CompareTo(b.Index);
        }


        /// <summary>
        /// Heuristic guesses what might be a good output width for a list of bitmaps.
        /// </summary>
        static int GuessOutputWidth(List<ArrangedBitmap> bitmaps)
        {
            // Gather the widths of all our bitmaps into a temporary list.
            List<int> widths = new List<int>();

            foreach (ArrangedBitmap bitmap in bitmaps)
            {
                widths.Add(bitmap.Width);
            }

            // Sort the widths into ascending order.
            widths.Sort();

            // Extract the maximum and median widths.
            int maxWidth = widths[widths.Count - 1];
            int medianWidth = widths[widths.Count / 2];

            // Heuristic assumes an NxN grid of median sized bitmaps.
            int width = medianWidth * (int)Math.Round(Math.Sqrt(bitmaps.Count));

            // Make sure we never choose anything smaller than our largest bitmap.
            return Math.Max(width, maxWidth);
        }
    }
}
