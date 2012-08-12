using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.Vex.Bonds
{
    public enum BondAttachment : uint
    {
        None = 0,

        // anchor locations (keep together)
        SymbolHandle,
        ObjectHandleTL,
        ObjectHandleT,
        ObjectHandleTR,
        ObjectHandleR,
        ObjectHandleBR,
        ObjectHandleB,
        ObjectHandleBL,
        ObjectHandleL,
        ObjectCenter,        

        RotationPoint,

        // locks
        Template,
        HGuide,
        VGuide,
        CornerGuide,
        
        // pins
        GridPoint,
    }

    public static class BondAttachmentExtensions
    {
        public static int GetHandleIndex(this BondAttachment ba)
        {
            int result = -1;
            if (ba >= BondAttachment.ObjectHandleTL && ba <= BondAttachment.ObjectCenter)
            {
                result = (int)(ba - BondAttachment.ObjectHandleTL);
            }
            return result;
        }
        public static BondAttachment GetTargetFromHandleIndex(int index)
        {
            return (BondAttachment)(index + (int)BondAttachment.ObjectHandleTL);
        }
        public static BondAttachment GetBondAttachment(ChainType chainType)
        {
            BondAttachment bt = BondAttachment.None;

            switch (chainType)
            {
                case ChainType.AlignedLeft:
                    bt = BondAttachment.ObjectHandleL;
                    break;
                case ChainType.AlignedCenterVertical:
                    bt = BondAttachment.ObjectCenter;
                    break;
                case ChainType.AlignedRight:
                    bt = BondAttachment.ObjectHandleR;
                    break;
                case ChainType.AlignedTop:
                    bt = BondAttachment.ObjectHandleT;
                    break;
                case ChainType.AlignedCenterHorizontal:
                    bt = BondAttachment.ObjectCenter;
                    break;
                case ChainType.AlignedBottom:
                    bt = BondAttachment.ObjectHandleB;
                    break;
                case ChainType.DistributedHorizontal:
                    bt = BondAttachment.ObjectHandleL;
                    break;
                case ChainType.DistributedVertical:
                    bt = BondAttachment.ObjectHandleT;
                    break;
            }

            return bt;
        }

        public static bool IsHandle(this BondAttachment bondTarget)
        {
            return (bondTarget >= BondAttachment.SymbolHandle && bondTarget <= BondAttachment.ObjectCenter); 
        }
        public static bool IsGuide(this BondAttachment bondTarget)
        {
            return
                bondTarget == BondAttachment.HGuide ||
                bondTarget == BondAttachment.VGuide ||
                bondTarget == BondAttachment.Template || 
                bondTarget == BondAttachment.CornerGuide;
        }
        public static bool IsHGuide(this BondAttachment bondTarget)
        {
            return (bondTarget == BondAttachment.HGuide || bondTarget == BondAttachment.CornerGuide);
        }
        public static bool IsVGuide(this BondAttachment bondTarget)
        {
            return (bondTarget == BondAttachment.VGuide || bondTarget == BondAttachment.CornerGuide); 
        }
        public static bool IsBottomOrLeft(this BondAttachment bondTarget)
        {
            return (bondTarget == BondAttachment.ObjectHandleL || bondTarget == BondAttachment.ObjectHandleB); 
        }

        public static AspectConstraint GetAspectConstraint(this BondAttachment ba)
        {
            AspectConstraint result = AspectConstraint.None;
            switch (ba)
            {
                case BondAttachment.ObjectHandleTL:
                case BondAttachment.ObjectHandleTR:
                case BondAttachment.ObjectHandleBR:
                case BondAttachment.ObjectHandleBL:
                    result = AspectConstraint.Locked;
                    break;
                case BondAttachment.ObjectHandleT:
                case BondAttachment.ObjectHandleB:
                    result = AspectConstraint.Vertical;
                    break;
                case BondAttachment.ObjectHandleR:
                case BondAttachment.ObjectHandleL:
                    result = AspectConstraint.Horizontal;
                    break;
            }
            return result;
        }
    }
}
