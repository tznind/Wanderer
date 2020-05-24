using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class SetEffect : Effect
    {
        AssignmentExpression Expression {get;set;}
        
        public SetEffect(string expression, SystemArgsTarget target):base(target)
        {
            Expression = new AssignmentExpression(expression);
        }

        public override void Apply(SystemArgs args)
        {
            var o = args.GetTarget(Target);
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