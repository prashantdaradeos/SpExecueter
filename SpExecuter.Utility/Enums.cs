using System;
using System.Collections.Generic;
using System.Text;

namespace SpExecuter.Utility
{
    public enum Lifetime
    {
        Scoped, Singleton, Transient
    }
    public enum ConditionType
    {
        AND,
        OR
    }
}
