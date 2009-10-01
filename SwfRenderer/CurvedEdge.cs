using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.SwfRenderer
{
    public class CurvedEdge : Edge
    {
        public int curveX;
        public int curveY;
        private float tStep;

        public CurvedEdge(int startX, int startY, int curveX, int curveY, int endX, int endY, byte fill0, byte fill1, byte line) :
            base(startX, startY, endX, endY, fill0, fill1, line)
        {
            this.curveX = curveX;
            this.curveY = curveY;
            tStep = Math.Abs(1f/((float)(endY - startY)/(float)SwfRenderer.TWIP));
            GenerateXTable();
        }

        public override void Init()
        {
            base.Init();
        }

        public override int MaxX { get { return (startX > endX) ?  (startX > curveX) ? startX : curveX :
                                                          (endX > curveX) ? endX : curveX; } }
        public override int MinX { get { return (startX <= endX) ? (startX <= curveX) ? startX : curveX :
                                                          (endX <= curveX) ? endX : curveX; } }

        public override int GetX(int y)
        {
            int result = startX;
            if (endY - startY != 0)
            {
                float t = (y - startY) / (float)(endY - startY);
                float u = 1f - t;
                return (int)(u * u * startX + 2f * u * t * curveX + t * t * endX);
            }                
            return result;
        }

        public override void IncX(int y)
        {
            // x(t) = axt2 + bxt + cx
            // y(t) = ayt2 + byt + cy

            //int P0 = startX;
            //int P1 = curveX;
            //int P2 = endY;
            //float a = P0 - 2 * P1 + P2;
            //float b = 2 * P0 + 2 * P1;
            //float c = P0;
            int index = (int)((y - startY) / SwfRenderer.TWIP) - 1;
            currentX = xTable[index];
        }
        int[] xTable;
        public void GenerateXTable()
        {
            //int[] result = int[(int)((endY - startY) / SwfRenderer.TWIP)];
            List<int> result = new List<int>();
            float t = 0f;
            float u = 1f;
            float inc = 1f / ((endY - startY) / SwfRenderer.TWIP);
            float tempY;
            int curY = startY;
            int prevX = startX;
            int curX = startX;
           // result.Add(curX);

            while (t < 1)
            {
                t += inc;
                u = 1f - t;
                tempY = u * u * startY + 2f * u * t * curveY + t * t * endY;
                if (tempY > curY)
                {
                    curX = (int)(u * u * startX + 2f * u * t * curveX + t * t * endX);
                    int len = (int)((Math.Floor(tempY) - curY) / SwfRenderer.TWIP);
                    int dist = (int)(curX - prevX);
                    for (int i = 1; i < len + 1; i++)
                    {
                        result.Add(prevX + (int)(dist * (i / (float)len)) );                        
                    }
                    prevX = curX;
                    curY = result.Count * SwfRenderer.TWIP + startY;
                }
            }
            result.Add(endX);

            xTable = result.ToArray();
        }

        public static Edge[] SplitAtPeak(
            int p0x, int p0y, int p1x, int p1y, int p2x, int p2y, byte fill0, byte fill1, byte line)
        {
            Edge[] result = new Edge[2];

            // find the place where the y's are the same, that is the peak
            // first height divided by total height
            float h0 = (float)(p0y - p1y);
            float h1 = (float)(p2y - p1y);
            float t = h0 / (h0 + h1);
            float u = 1f - t;

            float mx = u * u * p0x + 2f * u * t * p1x + t * t * p2x;
            float my = u * u * p0y + 2f * u * t * p1y + t * t * p2y;


            float m0x = (p0x - p1x) * u + p1x;
            float m1x = (p2x - p1x) * t + p1x;
            //float mx = (m1x - m0x) * t + m0x;

            //float m0y = (p1y - p0y) * t + p0y;
            //float m1y = (p1y - p2y) * u + p2y;
            //float my = (m1y - m0y) * t + m0y;

            result[0] = new CurvedEdge(p0x, p0y, (int)m0x, (int)my, (int)mx, (int)my, fill0, fill1, line);
            result[1] = new CurvedEdge((int)mx, (int)my, (int)m1x, (int)my, p2x, p2y, fill0, fill1, line);


            if (result[0].startY > (int)my || result[0].endY < (int)my)
            {
                Console.WriteLine(my);
            }
            if (result[1].startY > (int)my || result[1].endY < (int)my)
            {
                Console.WriteLine(my);
            }

            return result;
        }
    }
}







