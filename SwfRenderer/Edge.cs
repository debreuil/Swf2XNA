using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.SwfRenderer
{
    public class Edge
    {
        public byte fill0;
        public byte fill1;
        public byte line;

        // todo: store these in lookup tables maybe...
        public int startX;
        public int startY;
        public int endX;
        public int endY;

        public float currentX;
        private float stepX;

        public int tempID = 0;
        public static int tempIDCount = 0;

        public Edge(int startX, int startY, int endX, int endY, byte fill0, byte fill1, byte line)
        {
            tempID = tempIDCount++;

            // start y is always north of endY
            bool inverted = (startY > endY);
            this.startX = inverted ? endX : startX;
            this.startY = inverted ? endY : startY;
            this.endX = inverted ? startX : endX;
            this.endY = inverted ? startY : endY;

            this.fill0 = inverted ? fill1 : fill0;
            this.fill1 = inverted ? fill0 : fill1;
            this.line = line;
            Init();
        }

        public virtual void Init()
        {
            stepX = (float)(endX - startX) / (float)(endY - startY) * SwfRenderer.TWIP;
            ResetCurrentX();
        }

        public virtual int MaxX { get { return (startX > endX) ? startX : endX; } }
        public virtual int MinX { get { return (startX <= endX) ? startY : endX; } }
        public int MaxY { get { return endY; } }
        public int MinY { get { return startY; } }

        public virtual int GetX(int y)
        {
            return (int)( (endX - startX) / (float)(endY - startY) * (y - startY) + + startX );
        }
        public virtual void IncX(int y)
        {
            currentX += stepX;
        }

        public void ResetCurrentX()
        {
            currentX = startX;
        }
        public static int MaxYComparer(Edge e0, Edge e1)
        {
            return (e0.startY > e1.startY) ? 1 : (e0.startY == e1.startY) ? 0 : -1;
        }

        public static int CurrentXComparer(Edge e0, Edge e1)
        {
            int result = 0;

            if (e0 is CurvedEdge || e1 is CurvedEdge)
            {
                //if (e0 is CurvedEdge && e1 is CurvedEdge)
                //{
                //    result = ((CurvedEdge)e0).curveX > ((CurvedEdge)e1).curveX ? 1 : 
                //                ((CurvedEdge)e0).curveX < ((CurvedEdge)e1).curveX ? -1 : 0;
                //}
                //else
                {
                    // get intersect Y value
                    int mny = (int)Math.Max(e0.MinY, e1.MinY);
                    int mxy = (int)Math.Min(e0.MaxY, e1.MaxY);
                    int y = (int)((mxy - mny) / 2) + mny;
                    // get x values of each
                    int x0 = e0.GetX(y);
                    int x1 = e1.GetX(y);
                    // compare
                    result = (x0 > x1) ? 1 : (x0 < x1) ? -1 : 0;
                }
            }
            else
            {
                result =    (e0.startX > e1.startX) ? 1 : 
                            (e0.startX < e1.startX) ? -1 :
                            (e0.endX > e1.endX) ? 1 : 
                            (e0.endX < e1.endX) ? -1 : 0;
            }
            return result;
        }

    }
}
