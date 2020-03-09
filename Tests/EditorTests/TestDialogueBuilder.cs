using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Wanderer.Compilation;
using Wanderer.Editor;
using Wanderer.Factories.Blueprints;
using YamlDotNet.Serialization;

namespace Tests.EditorTests
{
    class TestDialogueBuilder
    {
        [Test]
        public void Test_GetIndentation()
        {
            var builder = new DialogueBuilder();
            Assert.AreEqual(0, builder.GetIndentation("fff"));
            Assert.AreEqual(2, builder.GetIndentation("  fff"));
            Assert.AreEqual(4, builder.GetIndentation("    fff"));
            Assert.AreEqual(6, builder.GetIndentation("      fff"));
        }

        
        [Test]
        public void TestSimpleTree()
        {

            string dialogue = @"
Where you from stranger?
  Nowhere special
  I'm from little rock friend
    I've heard of that place, it's nice
      Wasn't when I was there
      Yeah it's nice enough
  I'm from quasar
    Your messing with me
      Maybe a bit
        Never lie to me kid
      No for real
        I do not believe you
";

            
            var builder = new DialogueBuilder();
            var nodes = builder.Build(dialogue);

            Assert.AreEqual("Where you from stranger?",nodes[0].Body[0].Text);
            Assert.AreEqual("Nowhere special",nodes[0].Options[0].Text);
            Assert.AreEqual(null,nodes[0].Options[0].Destination);
            
            Assert.AreEqual("I'm from little rock friend",nodes[0].Options[1].Text);
            Assert.AreEqual(nodes[1].Identifier,nodes[0].Options[1].Destination);
            Assert.AreEqual("I've heard of that place, it's nice",nodes[1].Body[0].Text);
            Assert.AreEqual("Wasn't when I was there",nodes[1].Options[0].Text);
            Assert.AreEqual(null,nodes[1].Options[0].Destination);
            Assert.AreEqual("Yeah it's nice enough",nodes[1].Options[1].Text);
            Assert.AreEqual(null,nodes[1].Options[1].Destination);

            
            Assert.AreEqual("I'm from quasar",nodes[0].Options[2].Text);
            Assert.AreEqual(nodes[2].Identifier,nodes[0].Options[2].Destination);

            Assert.AreEqual("Your messing with me",nodes[2].Body[0].Text);
            Assert.AreEqual(nodes[3].Identifier,nodes[2].Options[0].Destination);
            Assert.AreEqual(nodes[4].Identifier,nodes[2].Options[1].Destination);

            Assert.AreEqual("Never lie to me kid",nodes[3].Body[0].Text);
            Assert.AreEqual("I do not believe you",nodes[4].Body[0].Text);
            Assert.IsEmpty(nodes[3].Options);
            Assert.IsEmpty(nodes[4].Options);
            

        }

        [Test]
        public void TestSerialize()
        {
            string dlg = @"
Where you from stranger?
  Nowhere special
  I'm from little rock friend
    I've heard of that place, it's nice
      Wasn't when I was there
      Yeah it's nice enough";


            var builder = new DialogueBuilder();
            var result = builder.BuildAndSerialize(dlg);

            StringAssert.Contains("Body:", result); 
            StringAssert.Contains("- Text: I've heard of that place, it's nice", result);
            StringAssert.Contains("Options:", result);
            StringAssert.Contains("- Text: Wasn't when I was there", result);
            StringAssert.Contains("- Text: Yeah it's nice enough",result);

        }

        [Test]
        public void BuildAndSerialize_NoFile()
        {
            var b = new DialogueBuilder();
            Assert.Throws<FileNotFoundException>(()=>b.BuildAndSerialize(new FileInfo("troll")));
        }

        [Test] public void BuildAndSerialize_NoYamlFiles()
        {
            var b = new DialogueBuilder();

            var input = new FileInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory,"troll2.yaml"));
            File.WriteAllText(input.FullName,"#emptyfile");

            Assert.Throws<NotSupportedException>(()=>b.BuildAndSerialize(input));
        }


        [Test]
        public void BuildAndSerialize_EmptyFile()
        {
            var b = new DialogueBuilder();

            var input = new FileInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory,"troll.z"));
            File.WriteAllText(input.FullName,"#emptyfile");

            var expectedOut = Path.Combine(TestContext.CurrentContext.WorkDirectory,"troll.yaml");
            if (File.Exists(expectedOut)) 
                File.Delete(expectedOut);

            var output = b.BuildAndSerialize(input);

            Assert.IsNull(output);
            Assert.IsFalse(File.Exists(expectedOut));
        }

        [Test]
        public void BuildAndSerialize_OutputFileAlreadyExists()
        {
            var b = new DialogueBuilder();

            var input = new FileInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory,"troll.z"));
            File.WriteAllText(input.FullName,"#emptyfile");

            var expectedOut = Path.Combine(TestContext.CurrentContext.WorkDirectory,"troll.yaml");
            if (File.Exists(expectedOut)) 
                File.Delete(expectedOut);
            File.WriteAllText(expectedOut,"blahblah");

            var ex = Assert.Throws<Exception>(()=>b.BuildAndSerialize(input));

            StringAssert.StartsWith("Output file path already exists",ex.Message);
        }

        [Test]
        public void BuildAndSerialize_Success()
        {
            var b = new DialogueBuilder();

            var input = new FileInfo(Path.Combine(TestContext.CurrentContext.WorkDirectory,"troll.z"));
            File.WriteAllText(input.FullName,"hey there wanderer");

            var expectedOut = Path.Combine(TestContext.CurrentContext.WorkDirectory,"troll.yaml");
            if (File.Exists(expectedOut)) 
                File.Delete(expectedOut);

            var output = b.BuildAndSerialize(input);

            Assert.AreEqual(expectedOut,output.FullName);

            StringAssert.Contains(@"- Text: hey there wanderer",File.ReadAllText(expectedOut));
        }
    }
}
