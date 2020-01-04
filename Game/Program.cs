using Game.UI;
using StarshipWanderer;
using Terminal.Gui;

namespace Game
{
    class Program
    {


        static void Main(string[] args)
        {
            Application.Init();
            
            var mainWindow = new MainWindow(new WorldFactory());
            Application.Top.Add(mainWindow);
            Application.Run();
        }
    }
}
