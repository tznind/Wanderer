using System.Runtime.InteropServices.ComTypes;
using Terminal.Gui;

namespace StarshipWanderer.UI
{
    public class SplashScreen: Label
    {
        
        public static string SplashText =
            @"
\ \        /               |                    
 \ \  \   / _` | __ \   _` |  _ \  __| _ \  __| 
  \ \  \ / (   | |   | (   |  __/ |    __/ |    
   \_/\_/ \__,_|_|  _|\__,_|\___|_|  \___|_|";


        public SplashScreen() : base(new Rect(0,0,50,4),SplashText.Replace("\r",""))
        {
        }
    }
}