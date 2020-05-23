using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    /// <summary>
    /// Condition which checks for a given stat being higher/lower etc than a threshold
    /// </summary>
    public class StatCondition : Condition
    {
        ArithmeticComparisonExpression Expression {get;set;}
        
        public StatCondition(string expression, SystemArgsTarget check):base(check)
        {
            Expression = new ArithmeticComparisonExpression(expression);
        }

        public override bool IsMet(IWorld world, SystemArgs args)
        {
            var o = args.GetTarget(Check);
            return Expression.Calculate((s)=>o.BaseStats[world.AllStats.Get(s)]);
        }

        public override string ToString()
        {
            return Expression.Expression;
        }
    }
}