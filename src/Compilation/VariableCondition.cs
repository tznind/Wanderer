using Wanderer.Systems;

namespace Wanderer.Compilation
{
    /// <summary>
    /// Condition which checks for a variable being higher/lower etc than a threshold (see <see cref="IHasStats.V"/>)
    /// </summary>
    public class VariableCondition : ICondition
    {

        ArithmeticComparisonExpression Expression {get;set;}
        public bool RecipientOnly { get; set; }

        public VariableCondition(string expression)
        {
            Expression = new ArithmeticComparisonExpression(expression);

        }

        public bool IsMet(IWorld world, SystemArgs args)
        {
            var o = RecipientOnly ? args.Recipient : args.AggressorIfAny ?? args.Recipient;
            return Expression.Calculate((s)=>o.V[s]);
        }

        public override string ToString()
        {
            return Expression.Expression;
        }
    }
}