using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDW.Vex.Bonds
{
    public enum BondType : int
    {
        Handle,
        Join,
        Lock,
        Pin,
        Spring,
        Anchor,
        Wrap,
        Self,
        LimitedHandle,

        Last,
    }
}
