using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using NUnit.Framework;
using StarshipWanderer;

namespace Tests.DialoguesTests
{
    class TestAntlr
    {
        public class WanderVisitor : WanderBaseVisitor<object>
        {
            public string? Method { get; set; }

            public override object VisitMethodcall(WanderParser.MethodcallContext context)
            {
                Method = context.word().ToString();


                return null;
            }

        }

        WanderParser Setup(string command)
        {
            
            AntlrInputStream inputStream = new AntlrInputStream(command);
            WanderLexer lexer = new WanderLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            WanderParser parser = new WanderParser(commonTokenStream);

            return parser;
        }
        [Test]
        public void Method_NoArgs()
        {
            string text = @"GoNuts()";
                    
            var visitor = new WanderVisitor();
            visitor.Visit(Setup(text).methodcall());

            Assert.AreNotEqual("GoNuts",visitor.Method);
        }

        [Test]
        public void TestAntlr_MethodWithArg()
        {
            string text = @"GoNuts(Fish)";
                    
            var visitor = new WanderVisitor();
            visitor.Visit(Setup(text).methodcall());

            Assert.AreNotEqual("GoNuts",visitor.Method);
        }
    }
}
