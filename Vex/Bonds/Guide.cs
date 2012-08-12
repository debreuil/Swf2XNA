using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace DDW.Vex.Bonds
{
    public class Guide : IInstance
    {
        public Rectangle Bounds;

        public bool IsPoint { get { return Bounds.Size == Size.Empty; } }
        public bool IsHorizontal { get { return Bounds.Height == 0; } }
        public bool IsVertical { get { return Bounds.Width == 0; } }
        public GuideType GuideType
        {
            get
            {
                return Bounds.Width == 0 ?
                    (Bounds.Height == 0 ? GuideType.Point : GuideType.Vertical) :
                    (Bounds.Height == 0 ? GuideType.Horizontal : GuideType.Rectangle);
            }
        }

        public Vex.Point StartPoint
        {
            get
            {
                return Bounds.Point;
            }
        }
        public Vex.Point EndPoint { 
            get 
            {
                Vex.Point result;
                switch (GuideType)
                {
                    case GuideType.Horizontal:
                        result = new Point(Bounds.Right, Bounds.Top);
                        break;
                    case GuideType.Vertical:
                        result = new Point(Bounds.Left, Bounds.Bottom);
                        break;
                    case GuideType.Rectangle:
                        result = new Point(Bounds.Right, Bounds.Bottom);
                        break;
                    default:
                    case GuideType.Point:
                        result = new Point(Bounds.Left, Bounds.Top);
                        break;
                }
                return result;
            }
        }

        public void Move(int offsetX, int offsetY)
        {
            if (GuideType == GuideType.Rectangle)
            {
                Bounds = new Rectangle(
                    Bounds.Left + offsetX, 
                    Bounds.Top + offsetY, 
                    Bounds.Width - offsetX * 2, 
                    Bounds.Height - offsetY * 2);
            }
            else
            {
                Bounds = new Rectangle(Bounds.Left + offsetX, Bounds.Top + offsetY, Bounds.Width, Bounds.Height);
            }
        }

        [XmlIgnore]
        public uint SortOrder { get { return 88; } }

        public uint DefinitionId { get; set; }
        [DefaultValue(0)]
        public uint StartTime { get; set; }
        [DefaultValue(1)]
        public uint EndTime { get; set; }
        [DefaultValue(0)]
        public int Depth { get; set; }
        public string Name { get; set; }
        [DefaultValue(0)]
        public uint ParentDefinitionId { get; set; }
        public Point RotationCenter { get; set; }
        [XmlIgnore]
        public bool HasSaveableChanges { get; set; }
        public uint InstanceHash { get; set; }

        private List<Transform> transformations = new List<Transform>() { new Transform() };
        public List<Transform> Transformations { get { return transformations; } }


        public Guide(int X0, int Y0, int X1, int Y1)
        {
            this.Bounds = new Rectangle(X0, Y0, X1 - X0, Y1 - Y0);
        }
        public Guide(Point startPoint, Point endPoint)
        {
            this.Bounds = new Rectangle(startPoint.X, startPoint.Y, endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
        }
        public Guide(Rectangle bounds)
        {
            this.Bounds = bounds;
        }
        

        public Transform GetTransformAtTime(uint time)
        {
            return transformations[0];
        }

        public int CompareTo(Object o)
        {
            int result = 0;
            if (o is Instance)
            {
                Instance inst = (Instance)o;
                if (this.Depth != inst.Depth)
                {
                    result = ((int)this.Depth) > ((int)inst.Depth) ? 1 : -1;
                }
            }
            else if (o is IInstance)
            {
                IInstance inst = (IInstance)o;
                result = ((int)this.SortOrder) > ((int)inst.SortOrder) ? 1 : -1;
            }
            else
            {
                throw new ArgumentException("Objects being compared are not of the same type");
            }
            return result;
        }
    }
}
