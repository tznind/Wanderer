using System;

namespace StarshipWanderer.Behaviours
{
    public enum Comparison
    {
        LessThan,
        LessThanOrEqual,
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