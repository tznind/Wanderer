using Antlr4.Runtime;
using NUnit.Framework;
using StarshipWanderer;

namespace Tests.DialoguesTests
{
    class TestAntlr
    {
        public class CVisitor : CBaseVisitor<object>
        {
            public string? Method { get; set; }

            public override object VisitIdentifierList(CParser.IdentifierListContext context)
            {
                return base.VisitIdentifierList(context);
            }

            public override object VisitExpression(CParser.ExpressionContext context)
            {
                if (!context.IsEmpty)
                    Method = context.start.Text;

                return base.VisitExpression(context);
            }
        }

        CParser Setup(string command)
        {
            AntlrInputStream inputStream = new AntlrInputStream(command);
            CLexer lexer = new CLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            CParser parser = new CParser(commonTokenStream);

            return parser;
        }
        [Test]
        public void Method_NoArgs()
        {
            string text = @"GoNuts();";

            var parser = Setup(text);

            var visitor = new CVisitor();
            visitor.Visit(parser.statement());

            Assert.AreEqual("GoNuts",visitor.Method);
        }

        [Test]
        public void TestAntlr_MethodWithArg()
        {
            string text = @"GoNuts(Fish)";
                    
            var visitor = new CVisitor();
            visitor.Visit(Setup(text).statement());

            Assert.AreEqual("GoNuts",visitor.Method);
        }
    }
}
