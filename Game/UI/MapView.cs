using System;
using Wanderer;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;
using System.Collections.Generic;

namespace Game.UI
{
    public class MapView : View
    {


        Attribute _attBlack;


        Dictionary<Color,Attribute> _regularColors = new Dictionary<Color, Attribute>();

        Dictionary<Color,Attribute> _invertedColors = new Dictionary<Color, Attribute>();
        public IWorld World {get;set;}
        
        public MapView()
        {
            _attBlack = Attribute.Make((Color) ConsoleColor.Black,(Color) ConsoleColor.Black);

            foreach(Color c in Enum.GetValues(typeof(Color)))
            {
                _regularColors.Add(c,Attribute.Make(c,Color.Black));
                _invertedColors.Add(c,Attribute.Make(Color.Black,c));
            }
        }

        public override void Redraw(Rect bounds)
        {
            if(World == null)
                return;

            
            var mapWidth = bounds.Width;
            var mapHeight = bounds.Height;

            
            var home = World.Map.GetPoint(World.Player.CurrentLocation);
            
            //var centre world location 0,0
            var toCentreX = -mapWidth / 2;
            var toCentreY = -mapHeight / 2;

            
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    var pointToRender = new Point3(x + toCentreX, y +toCentreY, 0).Offset(home);

                    if (World.Map.ContainsKey(pointToRender) && World.Map[pointToRender].IsExplored)
                    {
                        Driver.SetAttribute(Equals(pointToRender, home) 
                                    //flip the colors where you are
                                    ? _invertedColors[(Color) World.Map[pointToRender].Color]
                                :_regularColors[(Color)World.Map[pointToRender].Color]
                                );
                        AddRune(x,mapHeight - y,World.Map[pointToRender].Tile);
                    }
                    else
                    {
                        Driver.SetAttribute(_attBlack);
                        AddRune(x,mapHeight - y,' ');
                    }
                }
            }
        }
    }
}


