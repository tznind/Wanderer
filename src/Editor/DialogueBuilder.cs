using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Wanderer.Dialogues;

namespace Wanderer.Editor
{
    public class DialogueBuilder
    {
        private string _overRead;
        private List<DialogueNode> _allNodes;

        public int GetIndentation(string currentLine)
        {
            var match = Regex.Match(currentLine, @"^\ *");

            if (!match.Success)
                return 0;

            return match.Length;
        }

        public List<DialogueNode> Build(string simpleDialogueTree)
        {
            _overRead = null;
            _allNodes = new List<DialogueNode>();

            var sr = new StringReader(simpleDialogueTree);

            //read the first line
            string currentLine = GetNextLine(sr);

            if (currentLine == null)
                return null;

            GetNode(sr, 0, currentLine);

            return _allNodes;
        }


        /// <summary>
        /// Returns the next non blank non comment line from the <paramref name="sr"/>
        /// or null if the end has been reached
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        private string GetNextLine(StringReader sr)
        {
            try
            {
                if (_overRead != null)
                    return _overRead;
            }
            finally
            {
                _overRead = null;
            }
            

            var line = sr.ReadLine();
            
            while (line != null && IsBlankOrComment(line))
            {
                line = sr.ReadLine();
            }

            return line;
        }

        private void AddOption(StringReader reader, DialogueNode currentNode, int oldIndentation, string currentLine)
        {
            var option = new DialogueOption()
            {
                Text = currentLine.Trim()
            };

            currentNode.Options.Add(option);

            //read next line
            while (currentLine != null)
            {
                currentLine = GetNextLine(reader);

                if (currentLine != null)
                {
                    int newIndentation = GetIndentation(currentLine);

                    //add another option
                    if (oldIndentation == newIndentation)
                        AddOption(reader,currentNode, newIndentation, currentLine);
                    else
                    if (oldIndentation < newIndentation)
                    {
                        //add a destination
                        var node = GetNode(reader, newIndentation, currentLine);
                        option.Destination = node.Identifier;
                    }
                    else if (oldIndentation > newIndentation)
                    {
                        _overRead = currentLine;
                        return;
                    }
                }
            }
        }

        private DialogueNode GetNode(StringReader reader, int oldIndentation, string currentLine)
        {
            //create the new node
            var currentNode = new DialogueNode();
            currentNode.Identifier = Guid.NewGuid();
            _allNodes.Add(currentNode);

            //with the root text
            var newBlock = new TextBlock(currentLine.Trim());
            currentNode.Body.Add(newBlock);

            while (currentLine != null)
            {
                currentLine = GetNextLine(reader);

                if (currentLine != null)
                {
                    int newIndentation = GetIndentation(currentLine);

                    if (oldIndentation == newIndentation)
                        currentNode.Body.Add(new TextBlock(currentLine.Trim()));
                    else if (oldIndentation < newIndentation)
                        AddOption(reader,currentNode, newIndentation, currentLine);
                    else if (oldIndentation > newIndentation)
                    {
                        _overRead = currentLine;
                        return currentNode;
                    }
                        

                }
            }

            return currentNode;
        }

        private bool IsBlankOrComment(string line)
        {
            return string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith('#');
        }
    }
}
