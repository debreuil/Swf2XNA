/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;

namespace DDW.Vex
{
    public struct ColorTransform
    {
        public int RAdd;
        public int RMult;
        public int GAdd;
        public int GMult;
        public int BAdd;
        public int BMult;
        public int AAdd;
        public int AMult;

        public ColorTransform(int rAdd, int rMult, int gAdd, int gMult, int bAdd, int bMult, int aAdd, int aMult)
        {
            this.RAdd = rAdd;
            this.RMult = rMult;
            this.GAdd = gAdd;
            this.GMult = gMult;
            this.BAdd = bAdd;
            this.BMult = bMult;
            this.AAdd = aAdd;
            this.AMult = aMult;
        }

        public bool IsIdentity()
        {
            return 
                (RAdd == 0) && (RMult == 0) && 
                (GAdd == 0) && (GMult == 0) && 
                (BAdd == 0) && (BMult == 0) && 
                (AAdd == 0) && (AMult == 0);
        }
        private static ColorTransform _identity = new ColorTransform(0,0,0,0,0,0,0,0);
        public static ColorTransform Identity
        {
            get
            {
                return _identity;
            }
        }

        public override string ToString()
        {
            return "{ra:" + RAdd + ", rb:" + RMult + ", ga:" + GAdd + ", gb:" + GMult + ", ba:" + BAdd + ", bb:" + BMult + ", aa:" + AAdd + ", ab:" + AMult + "}";
        }
    }
}
