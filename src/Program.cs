using System;
using StarshipWanderer.UI;
using Terminal.Gui;

namespace StarshipWanderer
{
    class Program
    {


        static void Main(string[] args)
        {
            Application.Init();
            var mainWindow = new MainWindow(new World(new You()));
            Application.Top.Add(mainWindow);
            Application.Run();
        }
    }
}
