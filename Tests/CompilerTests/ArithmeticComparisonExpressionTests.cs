using NUnit.Framework;
using Wanderer.Compilation;

namespace Tests.CompilerTests
{
    class ArithmeticComparisonExpressionTests
    {
        [Test]
        public void TestConstructor()
        {
            var expr = new ArithmeticComparisonExpression("a==b");
            Assert.AreEqual("a",expr.OperandA);
            Assert.AreEqual("==",expr.Comparator);
            Assert.AreEqual("b",expr.OperandB);
        }
        
    }
}
