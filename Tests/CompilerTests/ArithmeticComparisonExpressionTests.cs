using NUnit.Framework;
using Wanderer.Compilation;

namespace Tests.CompilerTests
{
    class ArithmeticComparisonExpressionTests
    {
        [Test]
        public void ABEquals()
        {
            var expr = new ArithmeticComparisonExpression("a==b");
            Assert.AreEqual("a",expr.OperandA);
            Assert.AreEqual("==",expr.Comparator);
            Assert.AreEqual("b",expr.OperandB);
            Assert.IsTrue(expr.Calculate((o)=>6));
        }

        [Test]
        public void ABConstEquals()
        {
            var expr = new ArithmeticComparisonExpression("a == 6");
            Assert.AreEqual("a",expr.OperandA);
            Assert.AreEqual("==",expr.Comparator);
            Assert.AreEqual("6",expr.OperandB);
            Assert.IsTrue(expr.Calculate((o)=>6));
            Assert.IsFalse(expr.Calculate((o)=>5));
        }
    }
}
