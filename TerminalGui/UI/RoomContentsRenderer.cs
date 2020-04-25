using System;
using System.Collections;
using System.Collections.Generic;
using NStack;
using Terminal.Gui;
using Wanderer;
using Wanderer.Actors;

namespace Wanderer.TerminalGui
{
    internal class RoomContentsRenderer : IListDataSource
    {
        private List<IHasStats> _roomContentsObjects;

        Terminal.Gui.Attribute _green;
        Terminal.Gui.Attribute _red;
        Terminal.Gui.Attribute _normal;

        public RoomContentsRenderer()
        {
            _red = Terminal.Gui.Attribute.Make(Color.BrightRed,Color.Black);
            _green = Terminal.Gui.Attribute.Make(Color.BrightGreen,Color.Black);
            _normal = Terminal.Gui.Attribute.Make(Color.White,Color.Black);
        }

        public void SetCollection(List<IHasStats> roomContentsObjects)
        {
            _roomContentsObjects = roomContentsObjects;            
        }

        public int Count => _roomContentsObjects.Count;

        public bool IsMarked(int item)
        {
            return false;
        }

        void RenderLine (ConsoleDriver driver, IHasStats toRender, int col, int line, int width)
        {
            string suffix ="   ";

            //Get relation to player
            if(toRender is IActor a)
            {
                var world = a.CurrentLocation.World;
                var relationship = world.Relationships.SumBetween(a,world.Player);

                if(relationship < -40)
                    suffix = "---";
                else
                if(relationship < -20)
                    suffix = "-- ";
                else
                if(relationship < 0)
                    suffix = "-  ";
                else
                if(relationship > 40)
                    suffix = "+++";
                else
                if(relationship > 20)
                    suffix = "++ ";
                else
                if(relationship > 0)
                    suffix = "+  ";
            }

            //allow for the suffix
            var ustring = toRender.ToString();
            ustring = ustring.Substring(0,Math.Min(ustring.Length,width-3)) + suffix;
            ustring = ustring.PadRight(width);

            int byteLen = ustring.Length;
            int used = 0;
            for (int i = 0; i < byteLen;) 
            {
                (var rune, var size) = Utf8.DecodeRune (ustring, i, i - byteLen);
                var count = Rune.ColumnWidth (rune);
                if (used+count >= width)
                    break;

                if(rune == '-')
                    driver.SetAttribute(_red);
                else
                if(rune == '+')
                    driver.SetAttribute(_green);
                else
                    driver.SetAttribute(_normal);

                driver.AddRune (rune);
                used += count;
                i += size;
            }
            for (; used < width; used++) {
                driver.AddRune (' ');
            }
        }

        public void Render (ListView container, ConsoleDriver driver, bool marked, int item, int col, int line, int width)
        {
            container.Move (col, line);
            RenderLine (driver, _roomContentsObjects[item], col, line, width);
        }

        public void SetMark(int item, bool value)
        {
            
        }
    }
}
