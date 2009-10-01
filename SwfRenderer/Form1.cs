using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DDW.SwfRenderer
{
    public partial class Form1 : Form
    {
        public Edge[] edges;
        private Bitmap bmpNormal;
        public Bitmap bmpLarge;

        public Form1()
        {
            InitializeComponent();
        }
        public Bitmap Bitmap
        {
            set
            {
                bmpNormal = value;
                int w = value.Width;
                int h = value.Height;
                int scale = 5;
                UnsafeBitmap bn = new UnsafeBitmap(bmpNormal);
                UnsafeBitmap b = new UnsafeBitmap(w * scale, h * scale);
                PixelData gridPx = new PixelData(0,0,0,255);

                bn.LockBitmap();
                b.LockBitmap();
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        PixelData px = bn.GetPixel(x, y);
                        for (int xs = 0; xs < scale; xs++)
                        {
                            for (int ys = 0; ys < scale; ys++)
                            {
                                b.SetPixel(x * scale + xs, y * scale + ys, px);                                
                            }
                            
                        }
                        if (y % 10 == 0 && x % 10 == 0)
                        {
                            b.SetPixel(x * scale + scale / 2, y * scale + scale / 2, gridPx); 
                            b.SetPixel(x * scale + scale / 2 + 1, y * scale + scale / 2 + 1, gridPx); 
                            b.SetPixel(x * scale + scale / 2 + 1, y * scale + scale / 2, gridPx);
                            b.SetPixel(x * scale + scale / 2, y * scale + scale / 2 + 1, gridPx);
                        }
                    }
                }
                b.UnlockBitmap();
                bn.UnlockBitmap();
                bmpLarge = b.Bitmap;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int offset = 50;
            float twip = 20f;
            float q2c = 2 / 3f;

            e.Graphics.SmoothingMode = SmoothingMode.None;
            if (bmpNormal != null)
            {
                e.Graphics.DrawImage(bmpNormal, offset, offset);
                e.Graphics.DrawImage(bmpLarge, offset + bmpNormal.Width + 50, offset);
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (edges != null)
            {
                Pen p2 = new Pen(Color.FromArgb(0xFF, 0,0x66,0x66), 2);
                Pen p = new Pen(Color.FromArgb(0xFF, 0,0,0x66), 2);
                foreach (Edge edge in edges)
                {
                    if (edge is CurvedEdge)
                    {
                        CurvedEdge ce = (CurvedEdge)edge;
                        e.Graphics.DrawBezier(p2,
                            ce.startX / twip + offset, 
                            ce.startY / twip + offset,

                            ((ce.curveX - ce.startX) * q2c + ce.startX) / twip + offset,
                            ((ce.curveY - ce.startY) * q2c + ce.startY) / twip + offset,
                            (ce.endX - (ce.endX - ce.curveX) * q2c) / twip + offset,
                            (ce.endY - (ce.endY - ce.curveY) * q2c) / twip + offset,

                            ce.endX / twip + offset, 
                            ce.endY / twip + offset);
                    }
                    else
                    {
                        e.Graphics.DrawLine(p,
                            edge.startX / twip + offset, edge.startY / twip + offset,
                            edge.endX / twip + offset, edge.endY / twip + offset);
                    }
                }
            }
        }
    }
}













