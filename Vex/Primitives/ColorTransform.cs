/* Copyright (C) 2008 Robin Debreuil -- Released under the BSD License */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

namespace DDW.Vex
{
    public struct ColorTransform : IXmlSerializable
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
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader r)
        {
            string[] adds = r.GetAttribute("Adds").Split(new char[]{','});
            string[] mults = r.GetAttribute("Multiplies").Split(new char[] { ',' });
            AAdd = int.Parse(adds[0].Substring(adds[0].IndexOf("a:")), NumberStyles.Any);
            RAdd = int.Parse(adds[1].Substring(adds[1].IndexOf("r:")), NumberStyles.Any);
            GAdd = int.Parse(adds[2].Substring(adds[2].IndexOf("g:")), NumberStyles.Any);
            BAdd = int.Parse(adds[3].Substring(adds[3].IndexOf("b:")), NumberStyles.Any);

            AMult = int.Parse(mults[0].Substring(mults[0].IndexOf("a:")), NumberStyles.Any);
            RMult = int.Parse(mults[1].Substring(mults[1].IndexOf("r:")), NumberStyles.Any);
            GMult = int.Parse(mults[2].Substring(mults[2].IndexOf("g:")), NumberStyles.Any);
            BMult = int.Parse(mults[3].Substring(mults[3].IndexOf("b:")), NumberStyles.Any);
            r.Read();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Adds", "a:" + AAdd.ToString() + "r:" + RAdd.ToString() + " g:" + GAdd.ToString() +  " b:" + BAdd.ToString());
            writer.WriteAttributeString("Multiplies", "a:" + AMult.ToString() + "r:" + RMult.ToString() + " g:" + GMult.ToString() +  " b:" + BMult.ToString()); 
        }
    }
}
