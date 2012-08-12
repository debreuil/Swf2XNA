using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DDW.Vex.Bonds
{
    public class Bond : IComparable
    {
        private uint sourceInstanceId;
        private BondAttachment sourceAttachment;
        private BondType bondType;

        public ChainType ChainType;
        public Bond Next;
        public Bond Previous;
        public bool IsStart { get { return Previous == null; } }
        public bool IsEnd { get { return Next == null; } }

        public uint SourceInstanceId { get { return sourceInstanceId; } }
        public BondAttachment SourceAttachment { get { return sourceAttachment; } }
        public BondType BondType { get { return bondType; } }
        public Point TargetLocation { get; set; }
        public bool GuideMoved = false;

        public Bond(uint sourceInstanceId, BondAttachment sourceAttachment, BondType bondType)
        {
            this.sourceInstanceId = sourceInstanceId;
            this.sourceAttachment = sourceAttachment;
            this.bondType = bondType;
        }

        public long GetHandleHash()
        {
            return ((long)SourceInstanceId * 10000) + (int)SourceAttachment;
        }
        public long GetHandleHash(BondAttachment ba)
        {
            return ((long)SourceInstanceId * 10000) + (int)ba;
        }

        public Bond GetFirst()
        {
            Bond result = this;
            while (result.Previous != null)
            {
                result = result.Previous;
                if (result.Previous == this) // prevent infinite loop on closed circuit
                {
                    break;
                }
            }
            return result;
        }
        public Bond GetLast()
        {
            Bond result = this;
            while (result.Next != null)
            {
                result = result.Next;
                if (result.Next == this) // prevent infinite loop on closed circuit
                {
                    break;
                }
            }
            return result;
        }

        public void GetRelatedIds(List<uint> list)
        {
            if (Next != null)
            {
                list.Add(Next.SourceInstanceId);
            }

            if (Previous != null)
            {
                list.Add(Previous.SourceInstanceId);
            }
        }
        public Bond RemoveSelf()
        {
            Bond orphan = null;

            if (Next != null)
            {
                Next.Previous = Previous;
                if (Next.IsStart && Next.IsEnd)
                {
                    orphan = Next;
                }
            }

            if (Previous != null)
            {
                Previous.Next = Next;
                if (Previous.IsStart && Previous.IsEnd)
                {
                    orphan = Previous;
                }
            }

            //ChainType = ChainType.None;
            //Next = null;
            //Previous = null;
            return orphan;
        }
        public void InsertBefore(Bond front)
        {
            if (front.Previous != null)
            {
                Bond back = front.Previous;
                this.Next = front;
                this.Previous = back;
                back.Next = this;
                front.Previous = this;
            }
            else
            {
                this.Previous = null;
                this.Next = front;
                front.Previous = this;
            }
        }
        public void InsertAfter(Bond back)
        {
            if (back.Next != null)
            {
                Bond front = back.Previous;
                this.Next = front;
                this.Previous = back;
                back.Next = this;
                front.Previous = this;
            }
            else
            {
                this.Previous = back;
                this.Next = null;
                back.Next = this;
            }
        }

        public Bond GetCollidingBond(BondAttachment ba)
        {
            Bond result = null;

            if (ChainType.IsDistributed())
            {
                if (Previous != null && ChainType.GetAttachment() == ba)
                {
                    result = this;
                }
                else if (Next != null && ChainType.GetOppositeAttachment() == ba)
                {
                    result = Next;
                }
            }
            else if(sourceAttachment == ba)
            {
                result = this;
            }
            return result;
        }

        public int CompareTo(object obj)
        {
            int result = -1;
            if (obj is Bond)
            {
                if (ChainType.IsHorizontal())
                {
                    result = this.TargetLocation.X.CompareTo(((Bond)obj).TargetLocation.X);
                }
                else
                {
                    result = this.TargetLocation.Y.CompareTo(((Bond)obj).TargetLocation.Y);
                }
            }
            return result;
        }
    }
}
