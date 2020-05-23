using System;
using System.Collections.Generic;

namespace Wanderer.Compilation
{
    public class AssignmentExpression : BaseExpression
    {
        Dictionary<string,Func<double,double,double>> _operators = new Dictionary<string, Func<double, double, double>>
        {
            {"+=", (a,b) => a + b},
            {"++", (a,b) => a + 1},
            {"+", (a,b) => a + b},
            
            {"-=", (a,b) => a - b},
            {"--", (a,b) => a - 1},
            {"-", (a,b) => a - b},

            {"=", (a,b) => b},

        };

        public AssignmentExpression(string expression)
        {
            SplitExpression(expression,_operators.Keys);

            if(ConstA.HasValue)
                throw new ArgumentException($"Error with expression '{expression}', left hand side of expression must be a variable not a decimal");
        }

        
        /// <summary>
        /// Calculates the RHS of the equation and returns the absolute value that should be assigned to the LHS
        /// </summary>
        /// <param name="operandFunc"></param>
        /// <returns></returns>
        public double Calculate(Func<string,double> operandFunc)
        {
            return _operators[Comparator](ConstA ?? operandFunc(OperandA),ConstB ?? operandFunc(OperandB));
        }
    }
}