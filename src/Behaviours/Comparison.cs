using System;

namespace Wanderer.Behaviours
{
    public enum Comparison
    {
        LessThan,
        LessThanOrEqual,
        EqualTo,
        GreaterThanOrEqual,
        GreaterThan
    }

    public static class ComparisonExtensions
    {
        public static bool IsMet(this Comparison c,double lhs, double rhs)
        {
            switch (c)
            {
                case Comparison.LessThan:
                    return lhs < rhs;
                case Comparison.LessThanOrEqual:
                    return lhs <= rhs;
                case Comparison.EqualTo:
                    return Math.Abs(lhs - rhs) < 0.001;
                case Comparison.GreaterThanOrEqual:
                    return lhs >= rhs;
                case Comparison.GreaterThan:
                    return lhs > rhs;
                default:
                    throw new ArgumentOutOfRangeException(nameof(c), c, null);
            }
        }
    }
}