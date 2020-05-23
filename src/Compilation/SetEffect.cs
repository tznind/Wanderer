using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class SetEffect : IEffect
    {
        AssignmentExpression Expression {get;set;}

        public bool RecipientOnly { get; set; }

        public SetEffect(string expression)
        {
            Expression = new AssignmentExpression(expression);
        }

        public void Apply(SystemArgs args)
        {
            var o = RecipientOnly ? args.Recipient : args.AggressorIfAny ?? args.Recipient;
            var val = Expression.Calculate(f=>GetOperand(args,o,f));

            //if LHS is a stat
            if (args.World.AllStats.Contains(Expression.OperandA))
                o.BaseStats[Expression.OperandA] = val;
            else
                o.V[Expression.OperandA] = val; //otherwise treat it as a variable
        }

        private double GetOperand(SystemArgs args, IHasStats hasStats, string operand)
        {
            if (args.World.AllStats.Contains(operand))
                return hasStats.BaseStats[operand];

            return hasStats.V[operand];
        }

        public override string ToString()
        {
            return Expression.Expression;
        }

    }
}