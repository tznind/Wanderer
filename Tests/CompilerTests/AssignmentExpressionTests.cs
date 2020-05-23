using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Wanderer.Compilation;

namespace Tests.CompilerTests
{
    class AssignmentExpressionTests : UnitTest
    {
        [Test]
        public void TestVariable_Assignment()
        {
            var expr = new AssignmentExpression("X=5");
            
            var result = expr.Calculate((a) =>999);
            Assert.AreEqual(5,result);
        }
    }
}
