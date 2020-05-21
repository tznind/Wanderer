using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    /// <summary>
    /// Condition which checks for a given stat being higher/lower etc than a threshold
    /// </summary>
    public class StatCondition : ICondition
    {

        ArithmeticComparisonExpression Expression {get;set;}

        public StatCondition(string expression)
        {
            Expression = new ArithmeticComparisonExpression(expression);

        }

        public bool IsMet(IWorld world, SystemArgs args)
        {
            var o = args.AggressorIfAny ?? args.Recipient;
            return Expression.Calculate((s)=>o.BaseStats[world.AllStats.Get(s)]);
        }

        public override string ToString()
        {
            return Expression.Expression;
        }
    }
}