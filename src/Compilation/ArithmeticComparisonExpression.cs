using System;
using System.Collections.Generic;

namespace Wanderer.Compilation
{
    public class ArithmeticComparisonExpression : BaseExpression
    {
       Dictionary<string,Func<double,double,bool>> _operators = new Dictionary<string, Func<double, double, bool>>
        {
            {"==", (a,b) => Math.Abs(a - b) <= 0.000001},
            {"!=", (a,b) => Math.Abs(a - b) >= 0.000001},

            {">=", (a,b) => a >= b},
            {"<=", (a,b) => a <= b},
            
            {"=", (a,b) => Math.Abs(a - b) <= 0.000001},

            {">", (a,b) => a > b},
            {"<", (a,b) => a < b},

        };

        public ArithmeticComparisonExpression(string expression)
        {
            SplitExpression(expression,_operators.Keys);
        }

        

        public bool Calculate(Func<string,double> operandFunc)
        {
            return _operators[Comparator](ConstA ?? operandFunc(OperandA),ConstB ?? operandFunc(OperandB));
        }
    }
}