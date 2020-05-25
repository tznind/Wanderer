using System;
using System.Collections.Generic;

namespace Wanderer.Compilation
{
    public abstract class BaseExpression
    {
        public string Expression {get;set;}
        
        public string OperandA {get; protected set; }

        public double? ConstA {get; protected set;}

        public string Comparator {get; protected set;}

        public string OperandB {get; protected set;}

        public double? ConstB {get; protected set;}

        protected void SplitExpression(string expression , IEnumerable<string> operators)
        {
            
            Expression = expression;
            foreach(var op in operators)
            {
                int idx = expression.IndexOf(op, StringComparison.InvariantCulture);

                if(idx != -1)
                {
                    OperandA = expression.Substring(0,idx).Trim();
                    Comparator = op;
                    OperandB = expression.Substring(idx + op.Length).Trim();
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
    }
}