using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Tests.Cookbook
{
    class CookBookSyncTest
    {
        [Test]
        public void TestCookbookCodeIsInSync()
        {
          Regex links = new Regex(@"<sup>(.*)</sup>");

          Regex codeStrings = new Regex("@\"([^\"]*)\"",RegexOptions.Singleline);

          string cookbookDir = Path.Combine(TestContext.CurrentContext.TestDirectory,"../../../../");
          var cookbook = Path.Combine(cookbookDir,"Cookbook.md");
          FileAssert.Exists(cookbook);
          
          string content = File.ReadAllText(cookbook);
          
          foreach(Match match in links.Matches(content))
          {
              var linkIdx = match.Groups[1].Value.IndexOf("./Tests/Cookbook/");

              if(linkIdx == -1)
                continue;

              var link = match.Groups[1].Value.Substring(linkIdx).TrimEnd(')');

              TestContext.Out.WriteLine("Looking for " + link);

              var codeFile = Path.Combine(cookbookDir,link);
              FileAssert.Exists(codeFile);

              string codeContent = File.ReadAllText(codeFile);

              foreach(Match codeMatch in codeStrings.Matches(codeContent))
              {
                  if(!content.Contains(codeMatch.Groups[1].Value.Trim()))
                  {
                      Assert.Fail($"Cookbook recipe in file '{codeFile}' had code block not in Cookbook.md.  Code block was:{codeMatch.Groups[1].Value}");
                  }
              }

          }
        }
    }
}