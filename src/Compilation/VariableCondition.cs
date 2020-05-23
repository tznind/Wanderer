using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    /// <summary>
    /// Condition which checks for a variable being higher/lower etc than a threshold (see <see cref="IHasStats.V"/>)
    /// </summary>
    public class VariableCondition : Condition
    {
        ArithmeticComparisonExpression Expression {get;set;}

        public VariableCondition(string expression, SystemArgsTarget check):base(check)
        {
            Expression = new ArithmeticComparisonExpression(expression);
        }

        public override bool IsMet(IWorld world, SystemArgs args)
        {
            var o = args.GetTarget(Check);
            return Expression.Calculate((s)=>o.V[s]);
        }

        public override string ToString()
        {
            return Expression.Expression;
        }
    }
}