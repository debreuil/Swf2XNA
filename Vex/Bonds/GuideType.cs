using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.Vex.Bonds
{
    public enum GuideType
    {
        Rectangle,
        Horizontal,
        Vertical,
        Point,
    }

    public static class GuideTypeExtensions
    {
        public static bool HasHorizontal(this GuideType g)
        {
            return g == GuideType.Horizontal || g == GuideType.Rectangle;
        }
        public static bool HasVertical(this GuideType g)
        {
            return g == GuideType.Vertical || g == GuideType.Rectangle;
        }
    }
}
