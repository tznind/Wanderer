using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Wanderer.Compilation
{
    /// <summary>
    /// Condition which checks for a given stat being higher/lower etc than a threshold
    /// </summary>
    public class StatCondition<T> : SimpleCondition<T>
    {

        ArithmeticComparisonExpression Expression {get;set;}

        public StatCondition(string expression)
        {
            Expression = new ArithmeticComparisonExpression(expression);

        }

        protected override bool IsMetImpl(IWorld world, IHasStats o)
        {
            return Expression.Calculate((s)=>o.BaseStats[world.AllStats.Get(s)]);
        }
    }
}