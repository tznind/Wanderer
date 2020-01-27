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

        public class SpeakLine
        {
            public string Person { get; set; }
            public string Text { get; set; }
        }

        public class SpeakVisitor : SpeakBaseVisitor<object>
        {
            public List<SpeakLine> Lines = new List<SpeakLine>();
            public override object VisitLine(SpeakParser.LineContext context)
            {            
                SpeakParser.NameContext name = context.name();
                SpeakParser.OpinionContext opinion = context.opinion();

                if (opinion != null)
                {
                    
                    SpeakLine line = new SpeakLine() { Person = name.GetText(), Text = opinion.GetText().Trim('"') };
                    Lines.Add(line);
                    return line;
                }

                return null;
            }
        }

        [Test]
        public void TestAntlr_Chat()
        {
            string text = @"dave says ""When life gives you LEMONS make lemonade""";
                    
            AntlrInputStream inputStream = new AntlrInputStream(text.ToString());
            SpeakLexer speakLexer = new SpeakLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            SpeakParser speakParser = new SpeakParser(commonTokenStream);
            SpeakParser.ChatContext chatContext = speakParser.chat();
            SpeakVisitor visitor = new SpeakVisitor();        
            visitor.Visit(chatContext);

            Assert.AreEqual("dave",visitor.Lines.Single().Person);
            Assert.AreEqual("When life gives you LEMONS make lemonade",visitor.Lines.Single().Text);
        }
    }
}
