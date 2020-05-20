using System;
using System.Collections.Generic;

namespace Wanderer.Compilation
{
    public class ArithmeticComparisonExpression
    {
        Dictionary<string,Func<double,double,bool>> _operators = new Dictionary<string, Func<double, double, bool>>
        {
            {"==", (a,b) => a == b},
            {"!=", (a,b) => a != b},
            {"=", (a,b) => a == b},
            {">=", (a,b) => a >= b},
            {"<=", (a,b) => a <= b},
            {">", (a,b) => a < b},
            {"<", (a,b) => a > b},

        };

        public string OperandA {get;set;}

        public string Comparator {get;set;}

        public string OperandB {get;set;}
        public ArithmeticComparisonExpression(string expression)
        {
            foreach(var op in _operators)
            {
                int idx = expression.IndexOf(op.Key);

                if(idx != -1)
                {
                    OperandA = expression.Substring(0,idx);
                    Comparator = op.Key;
                    OperandB = expression.Substring(idx + op.Key.Length);
                    break;
                }
            }

            if(Comparator == null)
                throw new ArgumentException($"No arithmetic comparator found in expression '{expression}'");
        }
        public bool Calculate(double a , double b)
        {
            return _operators[Comparator](a,b);
        }
    }
}