using System;
using System.Collections.Generic;

namespace Wanderer.Compilation
{
    public class ArithmeticComparisonExpression
    {
        public string Expression {get;set;}

        Dictionary<string,Func<double,double,bool>> _operators = new Dictionary<string, Func<double, double, bool>>
        {
            {"==", (a,b) => Math.Abs(a - b) <= 0.000001},
            {"!=", (a,b) => Math.Abs(a - b) >= 0.000001},

            {">=", (a,b) => a >= b},
            {"<=", (a,b) => a <= b},
            
            {"=", (a,b) => Math.Abs(a - b) <= 0.000001},

            {">", (a,b) => a < b},
            {"<", (a,b) => a > b},

        };

        public string OperandA {get;}

        public double? ConstA {get;}

        public string Comparator {get;}

        public string OperandB {get;}

        public double? ConstB {get;}
        public ArithmeticComparisonExpression(string expression)
        {
            Expression = expression;
            foreach(var op in _operators)
            {
                int idx = expression.IndexOf(op.Key);

                if(idx != -1)
                {
                    OperandA = expression.Substring(0,idx).Trim();
                    Comparator = op.Key;
                    OperandB = expression.Substring(idx + op.Key.Length).Trim();
                    break;
                }
            }

            if(Comparator == null)
                throw new ArgumentException($"No arithmetic comparator found in expression '{expression}'");

            if(double.TryParse(OperandA,out double resultA))
                ConstA = resultA;

            if(double.TryParse(OperandB,out double resultB))
                ConstB = resultB;
        }

        public bool Calculate(Func<string,double> operandFunc)
        {
            return _operators[Comparator](ConstA ?? operandFunc(OperandA),ConstB ?? operandFunc(OperandB));
        }
    }
}