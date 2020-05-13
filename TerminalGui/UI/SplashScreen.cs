using Terminal.Gui;

namespace Wanderer.TerminalGui
{
    public class SplashScreen: Label
    {
        
        public static string SplashText =
            @"
\ \        /               |                    
 \ \  \   / _` | __ \   _` |  _ \  __| _ \  __| 
  \ \  \ / (   | |   | (   |  __/ |    __/ |    
   \_/\_/ \__,_|_|  _|\__,_|\___|_|  \___|_|

              (Press F9 to start)
";


        public SplashScreen() : base(new Rect(0,0,50,6),SplashText.Replace("\r",""))
        {
        }
    }
}
