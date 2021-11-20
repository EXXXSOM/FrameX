using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastComparable : IEqualityComparer<int>
{
    public static FastComparable Default = new FastComparable();

    public bool Equals(int x, int y)
    {
        return x == y;
    }

    public int GetHashCode(int obj)
    {
        return obj.GetHashCode();
    }
}

public class ReferenceEqualityComparer : EqualityComparer<object>
{
    public override bool Equals(object x, object y)
    {
        return ReferenceEquals(x, y);
    }

    public override int GetHashCode(object obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}