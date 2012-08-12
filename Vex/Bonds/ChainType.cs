using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.Vex.Bonds
{
    public enum ChainType
    {
        None,

        DistributedHorizontal,
        DistributedVertical,

        AlignedTop,
        AlignedRight,
        AlignedBottom,
        AlignedLeft,

        AlignedCenterHorizontal,
        AlignedCenterVertical,
    }

    public static class ChainTypeExtensions
    {
        public static bool IsDistributed(this ChainType ct)
        {
            return ct >= ChainType.DistributedHorizontal && ct <= ChainType.DistributedVertical;
        }
        public static bool IsAligned(this ChainType ct)
        {
            return ct >= ChainType.AlignedTop && ct <= ChainType.AlignedCenterVertical;
        }
        public static bool IsHorizontal(this ChainType ct)
        {
            return (int)ct % 2 == 1;
        }

        public static BondAttachment GetAttachment(this ChainType ct)
        {
            BondAttachment result;

            switch (ct)
            {
                case ChainType.DistributedHorizontal:
                    result = BondAttachment.ObjectHandleL;
                    break;
                case ChainType.DistributedVertical:
                    result = BondAttachment.ObjectHandleT;
                    break;
                case ChainType.AlignedTop:
                    result = BondAttachment.ObjectHandleT;
                    break;
                case ChainType.AlignedRight:
                    result = BondAttachment.ObjectHandleR;
                    break;
                case ChainType.AlignedBottom:
                    result = BondAttachment.ObjectHandleB;
                    break;
                case ChainType.AlignedLeft:
                    result = BondAttachment.ObjectHandleL;
                    break;
                case ChainType.AlignedCenterHorizontal:
                case ChainType.AlignedCenterVertical:
                    result = BondAttachment.ObjectCenter;
                    break;
                default:
                    result = BondAttachment.None;
                    break;
            }

            return result;
        }
        public static BondAttachment GetOppositeAttachment(this ChainType ct)
        {
            BondAttachment result;

            switch (ct)
            {
                case ChainType.DistributedHorizontal:
                    result = BondAttachment.ObjectHandleR;
                    break;
                case ChainType.DistributedVertical:
                    result = BondAttachment.ObjectHandleB;
                    break;
                case ChainType.AlignedTop:
                    result = BondAttachment.ObjectHandleB;
                    break;
                case ChainType.AlignedRight:
                    result = BondAttachment.ObjectHandleL;
                    break;
                case ChainType.AlignedBottom:
                    result = BondAttachment.ObjectHandleT;
                    break;
                case ChainType.AlignedLeft:
                    result = BondAttachment.ObjectHandleR;
                    break;
                case ChainType.AlignedCenterHorizontal:
                case ChainType.AlignedCenterVertical:
                    result = BondAttachment.ObjectCenter;
                    break;
                default:
                    result = BondAttachment.None;
                    break;
            }

            return result;
        }
    }
}
